using QQCommon.Interfaces;
using QQCommon.Models;
using QQCommon.Protocols;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace QQClient.Communication
{
    public class NetworkClient : INetworkClient
    {
        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private Thread _receiveThread;
        private bool _isRunning;
        private readonly object _streamLock = new object();
        private readonly ConcurrentDictionary<string, TaskCompletionSource<ChatPacket>> _pendingRequests
            = new ConcurrentDictionary<string, TaskCompletionSource<ChatPacket>>();

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<ConnectionEventArgs> ConnectionChanged;

        public bool SearchId(string fromUserId, string userId)
        {
            var packet = new ChatPacket
            {
                Type = MessageType.SearchId,
                Sender = fromUserId,
                Content = userId,
                Timestamp = DateTime.Now,
                MessageId = Guid.NewGuid().ToString() // 添加唯一ID用于匹配响应
            };

            SendPacket(packet);
            var response = WaitForResponse(packet.MessageId, MessageType.SearchIdResponse);
            return response != null && response.Content == "SUCCESS";
        }
        public List<User> SearchAllId(string fromUserId)
        {
            var packet = new ChatPacket
            {
                Type = MessageType.SearchId,
                Sender = fromUserId,
                Timestamp = DateTime.Now,
                MessageId = Guid.NewGuid().ToString() // 添加唯一ID用于匹配响应
            };
            SendPacket(packet);
            var response = WaitForResponse(packet.MessageId, MessageType.SearchIdResponse);
            return null;
        }
        public bool AddFriend(string fromUserId, string toUserId)
        {
            var packet = new ChatPacket
            {
                Type = MessageType.AddFriendRequest,
                Sender = fromUserId,
                Content = toUserId,
                Timestamp = DateTime.Now,
                MessageId = Guid.NewGuid().ToString()
            };

            SendPacket(packet);
            var response = WaitForResponse(packet.MessageId, MessageType.AddFriendResponse);
            return response != null && response.Content == "SUCCESS";
        }

        private bool IsConnected()
        {
            if (_tcpClient == null || !_tcpClient.Connected)
                return false;
            try
            {
                return !(_tcpClient.Client.Poll(1, SelectMode.SelectRead) && _tcpClient.Client.Available == 0);
            }
            catch
            {
                return false;
            }
        }

        public bool Connect(string serverIp, int port)
        {
            if (IsConnected())
                return true;

            Disconnect();

            try
            {
                _tcpClient = new TcpClient();
                var connectTask = _tcpClient.ConnectAsync(serverIp, port);
                if (!connectTask.Wait(5000))
                    return false;

                _stream = _tcpClient.GetStream();
                _isRunning = true;
                _receiveThread = new Thread(ReceiveLoop);
                _receiveThread.IsBackground = true;
                _receiveThread.Start();

                OnConnectionChanged(true, "连接成功");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Disconnect()
        {
            Console.WriteLine($"[Disconnect] 被调用，调用堆栈: {Environment.StackTrace}");
            _isRunning = false;

            // 取消所有等待的请求
            foreach (var kv in _pendingRequests)
            {
                kv.Value.TrySetCanceled();
            }
            _pendingRequests.Clear();

            try
            {
                lock (_streamLock)
                {
                    if (_stream != null)
                    {
                        _stream.Close();
                        _stream = null;
                    }
                }

                if (_tcpClient != null)
                {
                    if (_tcpClient.Connected)
                        _tcpClient.Close();
                    _tcpClient = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"断开连接时出错: {ex.Message}");
            }
            finally
            {
                OnConnectionChanged(false, "已断开连接");
            }
        }

        public bool Login(string username, string password)
        {
            var packet = new ChatPacket
            {
                Type = MessageType.LoginRequest,
                Sender = username,
                Content = password,
                Timestamp = DateTime.Now,
                MessageId = Guid.NewGuid().ToString()
            };

            SendPacket(packet);
            var response = WaitForResponse(packet.MessageId, MessageType.LoginResponse);
            return response != null && response.Content == "SUCCESS";
        }

        private void SendPacket(ChatPacket packet)
        {
            string json = packet.ToJson();
            byte[] data = Encoding.UTF8.GetBytes(json);
            byte[] lengthBytes = BitConverter.GetBytes(data.Length);

            lock (_streamLock)
            {
                _stream.Write(lengthBytes, 0, lengthBytes.Length);
                var writeTask = _stream.WriteAsync(data, 0, data.Length);
                if (!writeTask.Wait(5000))
                    throw new TimeoutException("发送数据超时");
            }
        }

        // 等待特定消息ID的响应
        private ChatPacket WaitForResponse(string messageId, MessageType expectedType, int timeout = 10000)
        {
            var tcs = new TaskCompletionSource<ChatPacket>();
            _pendingRequests[messageId] = tcs;

            try
            {
                if (tcs.Task.Wait(timeout))
                    return tcs.Task.Result;
                else
                    throw new TimeoutException($"等待响应超时: {expectedType}");
            }
            finally
            {
                _pendingRequests.TryRemove(messageId, out _);
            }
        }

        // 接收循环（运行在独立线程）
        private void ReceiveLoop()
        {
            Console.WriteLine("[ReceiveLoop] 线程启动");
            while (_isRunning && IsConnected())
            {
                try
                {
                    // 等待数据可用，避免无数据时频繁 Read 导致超时异常
                    while (_isRunning && IsConnected() && !_stream.DataAvailable)
                    {
                        Thread.Sleep(10); // 短暂休眠，降低 CPU 占用
                    }
                    if (!_isRunning || !IsConnected()) break;

                    // 尝试读取一个完整的数据包
                    var packet = ReceivePacketInternal();
                    if (packet == null)
                    {
                        Console.WriteLine("[ReceiveLoop] ReceivePacketInternal 返回 null，连接已关闭");
                        break;
                    }

                    // 判断是请求响应还是服务器推送
                    if (!string.IsNullOrEmpty(packet.MessageId) &&
                        _pendingRequests.TryRemove(packet.MessageId, out var tcs))
                    {
                        tcs.TrySetResult(packet); // 完成等待的请求
                    }
                    else
                    {
                        OnMessageReceived(packet); // 触发推送消息事件
                    }
                }
                catch (TimeoutException)
                {
                    // 超时是正常的，说明在 ReadTimeout 内没有完整数据到达
                    // 继续等待下一轮
                    Console.WriteLine("[ReceiveLoop] 读取超时，继续等待");
                    continue;
                }
                catch (IOException ex) when (ex.InnerException is SocketException se &&
                                             se.SocketErrorCode == SocketError.TimedOut)
                {
                    // 另一种超时情况
                    Console.WriteLine("[ReceiveLoop] 套接字超时，继续等待");
                    continue;
                }
                catch (ObjectDisposedException)
                {
                    Console.WriteLine("[ReceiveLoop] 流已关闭，退出循环");
                    break;
                }
                catch (Exception ex)
                {
                    // 其他致命异常，记录并退出
                    Console.WriteLine($"[ReceiveLoop] 致命异常: {ex.GetType().Name} - {ex.Message}");
                    break;
                }
            }

            Console.WriteLine("[ReceiveLoop] 退出循环");
            OnConnectionChanged(false, "连接已断开");
            Disconnect();
        }

        // 内部接收数据包（不加锁，由ReceiveLoop单线程调用，或加锁保护）
        private ChatPacket ReceivePacketInternal()
        {
            byte[] lengthBytes = new byte[4];
            int totalRead = 0;
            while (totalRead < 4)
            {
                int bytesRead;
                lock (_streamLock)
                {
                    var readTask = _stream.ReadAsync(lengthBytes, totalRead, 4 - totalRead);
                    if (!readTask.Wait(5000))
                        throw new TimeoutException("接收数据超时");
                    bytesRead = readTask.Result;
                }
                if (bytesRead == 0)
                    return null;
                totalRead += bytesRead;
            }

            int dataLength = BitConverter.ToInt32(lengthBytes, 0);
            byte[] buffer = new byte[dataLength];
            totalRead = 0;
            while (totalRead < dataLength)
            {
                int bytesRead;
                lock (_streamLock)
                {
                    var readTask = _stream.ReadAsync(buffer, totalRead, dataLength - totalRead);
                    if (!readTask.Wait(5000))
                        throw new TimeoutException("接收数据超时");
                    bytesRead = readTask.Result;
                }
                if (bytesRead == 0)
                    return null;
                totalRead += bytesRead;
            }

            string json = Encoding.UTF8.GetString(buffer, 0, dataLength);
            return ChatPacket.FromJson(json);
        }

        public bool Register(string username, string password, string nickname)
        {
            try
            {
                var user = new User
                {
                    Username = username,
                    Password = password,
                    Nickname = string.IsNullOrEmpty(nickname) ? username : nickname,
                    RegisterTime = DateTime.Now
                };

                var packet = new ChatPacket
                {
                    Type = MessageType.RegisterRequest,
                    Sender = username,
                    Content = JsonConvert.SerializeObject(user),
                    Timestamp = DateTime.Now,
                    MessageId = Guid.NewGuid().ToString()
                };

                SendPacket(packet);
                var response = WaitForResponse(packet.MessageId, MessageType.RegisterResponse);
                return response != null && response.Content == "SUCCESS";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"注册失败: {ex.Message}");
                return false;
            }
        }

        public bool SendMessage(string username, string receiver, string content)
        {//
            try
            {
                var packet = new ChatPacket
                {
                    Type = MessageType.ChatMessage,
                    Sender = username,
                    Receiver = receiver,
                    Content = content,
                    Timestamp = DateTime.Now,
                    MessageId = Guid.NewGuid().ToString() // 可选，用于送达确认
                };

                SendPacket(packet);
                // 不需要等待响应，送达确认会通过事件异步通知
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发送消息失败: {ex.Message}");
                return false;
            }
        }

        protected virtual void OnMessageReceived(ChatPacket packet)
        {
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(packet));
        }

        protected virtual void OnConnectionChanged(bool isConnected, string message)
        {
            ConnectionChanged?.Invoke(this, new ConnectionEventArgs(isConnected, message));
        }
    }
}