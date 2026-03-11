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
            while (_isRunning && IsConnected())
            {
                try
                {
                    var packet = ReceivePacketInternal();
                    if (packet == null) break;

                    // 检查是否为某个挂起请求的响应
                    if (!string.IsNullOrEmpty(packet.MessageId) &&
                        _pendingRequests.TryRemove(packet.MessageId, out var tcs))
                    {
                        tcs.TrySetResult(packet);
                    }
                    else
                    {
                        // 是服务器推送的消息，触发事件
                        OnMessageReceived(packet);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"接收循环异常: {ex.Message}");
                    break;
                }
            }

            // 连接断开，触发事件并清理
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

        // 保留原 ReceivePacket 方法签名（但不再使用，改为内部调用 ReceivePacketInternal）
        private ChatPacket ReceivePacket(MessageType messageType)
        {
            // 此方法已废弃，保留仅用于兼容，实际不会调用
            throw new NotSupportedException("请使用异步等待机制");
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
        {
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