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
        //获取与指定好友的历史聊天记录
        public List<Message> GetHistoryMessages(string friendId)
        {
            var packet = new ChatPacket
            {
                Type = MessageType.GetHistoryMessagesRequest,
                Sender = GlobalClient.CurrentUserId,
                Content = friendId,
                MessageId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now
            };
            SendPacket(packet);
            var response = WaitForResponse(packet.MessageId, MessageType.GetHistoryMessagesResponse);

            if (response != null && response.Content == "SUCCESS")
            {
                if (response.Extras.TryGetValue("Messages", out string json))
                {
                    return JsonConvert.DeserializeObject<List<Message>>(json);
                }
            }
            return new List<Message>(); // 失败或没有消息时返回空列表
        }
        //标记与指定好友的未读消息为已读
        public bool MarkMessagesAsRead(string friendId)
        {
            var packet = new ChatPacket
            {
                Type = MessageType.MarkMessagesReadRequest,
                Sender = GlobalClient.CurrentUserId,
                Content = friendId,
                MessageId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now
            };
            SendPacket(packet);
            var response = WaitForResponse(packet.MessageId, MessageType.MarkMessagesReadResponse);
            return response != null && response.Content == "SUCCESS";
        }
        //根据用户名获取用户详细信息
        public User GetUserInfo(string userId)
        {
            var packet = new ChatPacket
            {
                Type = MessageType.GetUserInfoRequest,
                Sender = GlobalClient.CurrentUserId,
                Content = userId,
                MessageId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now
            };
            SendPacket(packet);
            var response = WaitForResponse(packet.MessageId, MessageType.GetUserInfoResponse);

            if (response != null && response.Content == "SUCCESS")
            {
                if (response.Extras.TryGetValue("UserInfo", out string json))
                {
                    return JsonConvert.DeserializeObject<User>(json);
                }
            }
            return null;
        }
        // 更新当前用户的个人信息
        public bool UpdateUserInfo(User updatedUser)
        {
            var packet = new ChatPacket
            {
                Type = MessageType.UpdateUserInfoRequest,
                Sender = GlobalClient.CurrentUserId,
                Content = JsonConvert.SerializeObject(updatedUser),
                MessageId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now
            };
            SendPacket(packet);
            var response = WaitForResponse(packet.MessageId, MessageType.UpdateUserInfoResponse);
            return response != null && response.Content == "SUCCESS";
        }
        public bool SearchId(string fromUserId, string userId)
        {
            var packet = new ChatPacket
            {
                Type = MessageType.SearchId,
                Sender = fromUserId,
                Content = userId,
                Timestamp = DateTime.Now,
                MessageId = Guid.NewGuid().ToString() // 添加唯一ID，用于匹配响应
            };

            SendPacket(packet);
            var response = WaitForResponse(packet.MessageId, MessageType.SearchIdResponse);
            return response != null && response.Content == "SUCCESS";
        }
        public List<Friend> SearchAllFriends(string userId)
        {
            var packet = new ChatPacket
            {
                Type = MessageType.SearchAllFriendsRequest,
                Sender = userId,
                Timestamp = DateTime.Now,
                MessageId = Guid.NewGuid().ToString()
            };

            SendPacket(packet);
            var response = WaitForResponse(packet.MessageId, MessageType.SearchAllFriendsResponse);

            if (response != null && response.Content == "SUCCESS")
            {
                // 从响应包的 Extras 中获取好友列表 JSON
                if (response.Extras.TryGetValue("FriendsList", out string friendsJson))
                {
                    return JsonConvert.DeserializeObject<List<Friend>>(friendsJson);
                }
            }
            return null; // 或返回空列表
        }
        //返回的是执行加好友操作是否成功，与成功添加好友没有关系
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
        public bool AcceptFriendRequest(string fromUserId)
        {
            var packet = new ChatPacket
            {
                Type = MessageType.AcceptFriendRequest,
                Sender = GlobalClient.CurrentUserId, // 当前登录用户（接受者）
                Content = fromUserId,                 // 发起者账号
                Timestamp = DateTime.Now,
                MessageId = Guid.NewGuid().ToString()
            };

            SendPacket(packet);
            var response = WaitForResponse(packet.MessageId, MessageType.AcceptFriendResponse);
            return response != null && response.Content == "SUCCESS";
        }
        public bool RejectFriendRequest(string fromUserId)
        {
            var packet = new ChatPacket
            {
                Type = MessageType.RejectFriendRequest,
                Sender = GlobalClient.CurrentUserId,
                Content = fromUserId,
                Timestamp = DateTime.Now,
                MessageId = Guid.NewGuid().ToString()
            };

            SendPacket(packet);
            var response = WaitForResponse(packet.MessageId, MessageType.RejectFriendResponse);
            return response != null && response.Content == "SUCCESS";
        }
        public List<Message> GetOfflineMessages(out List<string> friendRequests)
        {
            friendRequests = null;
            var packet = new ChatPacket
            {
                Type = MessageType.GetOfflineMessagesRequest,
                Sender = GlobalClient.CurrentUserId, // 需确保已设置
                MessageId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now
            };
            SendPacket(packet);
            var response = WaitForResponse(packet.MessageId, MessageType.GetOfflineMessagesResponse);

            if (response != null && response.Content == "SUCCESS")
            {
                List<Message> messages = null;
                if (response.Extras.TryGetValue("OfflineMessages", out string msgJson))
                {
                    messages = JsonConvert.DeserializeObject<List<Message>>(msgJson);
                }
                if (response.Extras.TryGetValue("FriendRequests", out string reqJson))
                {
                    friendRequests = JsonConvert.DeserializeObject<List<string>>(reqJson);
                }
                return messages;
            }
            return null;
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
                    Console.WriteLine($"[ReceiveLoop] 致命异常: {ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
                    // 其他致命异常，记录并退出
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
        // 获取当前用户加入的群组列表
        public List<Group> GetGroupList()
        {
            var packet = new ChatPacket
            {
                Type = MessageType.GetGroupListRequest,
                Sender = GlobalClient.CurrentUserId,
                MessageId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now
            };
            SendPacket(packet);
            var response = WaitForResponse(packet.MessageId, MessageType.GetGroupListResponse);

            if (response != null && response.Content == "SUCCESS")
            {
                if (response.Extras.TryGetValue("GroupList", out string json))
                {
                    return JsonConvert.DeserializeObject<List<Group>>(json);
                }
            }
            return new List<Group>();
        }

        // 发送群聊消息
        public bool SendGroupMessage(string groupId, string content)
        {
            try
            {
                var packet = new ChatPacket
                {
                    Type = MessageType.GroupChatMessage,
                    Sender = GlobalClient.CurrentUserId,
                    Receiver = groupId,          // 接收者为群ID
                    Content = content,
                    Timestamp = DateTime.Now,
                    MessageId = Guid.NewGuid().ToString()
                };
                SendPacket(packet);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发送群消息失败: {ex.Message}");
                return false;
            }
        }

        // 获取群历史消息
        public List<GroupMessage> GetGroupHistory(string groupId, int limit = 50)
        {
            var packet = new ChatPacket
            {
                Type = MessageType.GetGroupHistoryRequest,
                Sender = GlobalClient.CurrentUserId,
                Content = groupId,
                Extras = new Dictionary<string, string> { ["Limit"] = limit.ToString() },
                MessageId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now
            };
            SendPacket(packet);
            var response = WaitForResponse(packet.MessageId, MessageType.GetGroupHistoryResponse);

            if (response != null && response.Content == "SUCCESS")
            {
                if (response.Extras.TryGetValue("GroupMessages", out string json))
                {
                    return JsonConvert.DeserializeObject<List<GroupMessage>>(json);
                }
            }
            return new List<GroupMessage>();
        }
        // 创建群聊
        // "groupName"群名称
        // "description"群简介（可选）
        // 成功返回群ID，失败返回null
        public string CreateGroup(string groupName, string description = "")
        {
            var packet = new ChatPacket
            {
                Type = MessageType.CreateGroupRequest,
                Sender = GlobalClient.CurrentUserId,
                Content = JsonConvert.SerializeObject(new { GroupName = groupName, Description = description }),
                MessageId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now
            };
            SendPacket(packet);
            var response = WaitForResponse(packet.MessageId, MessageType.CreateGroupResponse);

            if (response != null && response.Content == "SUCCESS")
            {
                if (response.Extras.TryGetValue("GroupId", out string groupId))
                    return groupId;
            }
            return null;
        }
        protected virtual void OnMessageReceived(ChatPacket packet)
        {
            Console.WriteLine($"[OnMessageReceived] Type={packet.Type}, Sender={packet.Sender}, Content={packet.Content}");
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(packet));
        }

        protected virtual void OnConnectionChanged(bool isConnected, string message)
        {
            ConnectionChanged?.Invoke(this, new ConnectionEventArgs(isConnected, message));
        }
    }
}