// QQServer.Communication/SocketServer.cs
using Newtonsoft.Json;
using QQCommon.Interfaces;
using QQCommon.Models;
using QQCommon.Protocols;
using QQServer.Business;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace QQServer.Communication
{
    public class SocketServer
    {
        private TcpListener _listener;
        private bool _isRunning;
        private List<ClientInfo> _clients = new List<ClientInfo>();

        // 业务服务实例
        private IUserService _userService;
        private IMessageService _messageService;
        private IFriendService _friendService;

        public SocketServer()
        {
            // 初始化业务服务
            _userService = new UserService();
            _messageService = new MessageService();
            _friendService = new FriendService();
        }

        // 启动服务器
        public void Start(int port)
        {
            try
            {
                _listener = new TcpListener(IPAddress.Any, port);
                _listener.Start();
                _isRunning = true;

                Console.WriteLine("========================================");
                Console.WriteLine($"QQ服务器启动成功！");
                Console.WriteLine($"监听端口: {port}");
                Console.WriteLine($"启动时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                Console.WriteLine("========================================");

                // 启动接受客户端连接的线程
                Thread acceptThread = new Thread(AcceptClients);
                acceptThread.IsBackground = true;
                acceptThread.Start();

                // 启动心跳检测线程
                Thread heartbeatThread = new Thread(HeartbeatCheck);
                heartbeatThread.IsBackground = true;
                heartbeatThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"服务器启动失败: {ex.Message}");
                throw;
            }
        }

        // 停止服务器
        public void Stop()
        {
            _isRunning = false;

            // 关闭所有客户端连接
            lock (_clients)
            {
                foreach (var client in _clients)
                {
                    try
                    {
                        client.TcpClient.Close();
                    }
                    catch { }
                }
                _clients.Clear();
            }

            _listener?.Stop();
            Console.WriteLine("服务器已停止");
        }

        // 接受客户端连接
        private void AcceptClients()
        {
            while (_isRunning)
            {
                try
                {
                    TcpClient tcpClient = _listener.AcceptTcpClient();

                    // 创建客户端信息
                    var clientInfo = new ClientInfo
                    {
                        TcpClient = tcpClient,
                        Stream = tcpClient.GetStream(),
                        RemoteEndPoint = tcpClient.Client.RemoteEndPoint.ToString(),
                        ConnectedTime = DateTime.Now
                    };

                    lock (_clients)
                    {
                        _clients.Add(clientInfo);
                    }

                    Console.WriteLine($"新客户端连接: {clientInfo.RemoteEndPoint}");
                    Console.WriteLine($"当前在线客户端数: {_clients.Count}");

                    // 为每个客户端创建处理线程
                    Thread clientThread = new Thread(() => HandleClient(clientInfo));
                    clientThread.IsBackground = true;
                    clientThread.Start();
                }
                catch (Exception ex)
                {
                    if (_isRunning)
                        Console.WriteLine($"接受客户端连接出错: {ex.Message}");
                }
            }
        }
        private ChatPacket ReceivePacketFromClient(NetworkStream stream)
        {
            try
            {
                Console.WriteLine("[ReceivePacketFromClient] 开始接收数据...");
                // 读取4字节长度
                byte[] lengthBytes = new byte[4];
                int bytesRead = 0;
                while (bytesRead < 4)
                {
                    Console.WriteLine($"[ReceivePacketFromClient] 尝试读取长度，已读 {bytesRead}/4");
                    int read = stream.Read(lengthBytes, bytesRead, 4 - bytesRead);
                    Console.WriteLine($"[ReceivePacketFromClient] 实际读取: {read} 字节");
                    if (read == 0) {
                        Console.WriteLine("[ReceivePacketFromClient] 连接关闭 (read==0)");
                        return null; 
                    } // 连接关闭
                    bytesRead += read;
                }
                Console.WriteLine($"[ReceivePacketFromClient] 长度字节: {BitConverter.ToString(lengthBytes)}");
                int dataLength = BitConverter.ToInt32(lengthBytes, 0);
                Console.WriteLine($"[ReceivePacketFromClient] 解析得到数据长度: {dataLength}");
                // 读取数据
                byte[] data = new byte[dataLength];
                bytesRead = 0;
                while (bytesRead < dataLength)
                {
                    Console.WriteLine($"[ReceivePacketFromClient] 尝试读取数据，已读 {bytesRead}/{dataLength}");
                    int read = stream.Read(data, bytesRead, dataLength - bytesRead);
                    Console.WriteLine($"[ReceivePacketFromClient] 实际读取数据: {read} 字节");
                    if (read == 0) { 
                        return null; 
                    }
                    bytesRead += read;
                }

                string json = Encoding.UTF8.GetString(data);
                Console.WriteLine($"[ReceivePacketFromClient] 收到JSON: {json}");
                return ChatPacket.FromJson(json);
            }
            catch (IOException ex) when (ex.InnerException is SocketException se &&
                                 se.SocketErrorCode == SocketError.TimedOut)
            {
                // 将超时异常包装为 TimeoutException 抛出
                throw new TimeoutException("接收数据超时", ex);
            }
        }
        // 处理单个客户端
        private void HandleClient(ClientInfo clientInfo)
        {
            NetworkStream stream = clientInfo.Stream;
            while (_isRunning && clientInfo.TcpClient.Connected)
            {
                try
                {
                    ChatPacket packet = ReceivePacketFromClient(stream);
                    if (packet == null) break; // 连接断开

                    // 处理消息
                    ProcessPacket(packet, clientInfo);

                    clientInfo.LastActivityTime = DateTime.Now;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"客户端 {clientInfo.RemoteEndPoint} 断开: {ex.Message}");
                    break;
                }
            }
            RemoveClient(clientInfo);
        }

        // 处理不同类型的数据包
        private void ProcessPacket(ChatPacket packet, ClientInfo clientInfo)
        {
            switch (packet.Type)
            {
                case MessageType.LoginRequest:
                    HandleLogin(packet, clientInfo);
                    break;

                case MessageType.RegisterRequest:
                    HandleRegister(packet, clientInfo);
                    break;

                case MessageType.ChatMessage:
                    HandleChatMessage(packet, clientInfo);
                    break;

                case MessageType.AddFriendRequest:
                    HandleAddFriend(packet, clientInfo);
                    break;

                case MessageType.Heartbeat:
                    HandleHeartbeat(packet, clientInfo);
                    break;

                case MessageType.Disconnect:
                    RemoveClient(clientInfo);
                    break;

                case MessageType.SearchId:
                    SearchIdChat(packet,clientInfo);
                    break;

                case MessageType.SearchAllFriendsRequest:
                    HandleSearchAllFriends(packet, clientInfo);
                    break;

                case MessageType.GetOfflineMessagesRequest:
                    HandleGetOfflineMessages(packet, clientInfo);
                    break;

                case MessageType.AcceptFriendRequest:
                    HandleAcceptFriendRequest(packet, clientInfo);
                    break;

                case MessageType.RejectFriendRequest:
                    HandleRejectFriendRequest(packet, clientInfo);
                    break;

                default:
                    Console.WriteLine($"未知消息类型: {packet.Type}");
                    break;
            }
        }
        private void HandleAcceptFriendRequest(ChatPacket packet, ClientInfo clientInfo)
        {
            string fromUserId = packet.Content; // 发起请求的用户
            string toUserId = clientInfo.Username; // 当前登录用户（接受者）

            Console.WriteLine($"接受好友请求: {toUserId} 接受 {fromUserId}");

            // 调用业务层接受请求，内部会更新请求状态并创建双向好友关系
            bool success = _friendService.AcceptFriendRequest(fromUserId, toUserId);

            // 构造响应包
            ChatPacket response = new ChatPacket
            {
                Type = MessageType.AcceptFriendResponse,
                Sender = "Server",
                Receiver = toUserId,
                MessageId = packet.MessageId,
                Content = success ? "SUCCESS" : "FAILED",
                Timestamp = DateTime.Now
            };
            SendToClient(response, clientInfo);

            if (success)
            {
                // 通知原发起者（fromUserId）好友请求已被接受
                // 使用 FriendStatusUpdate 类型，Content 设为 "FRIEND_ACCEPTED"
                ChatPacket notify = new ChatPacket
                {
                    Type = MessageType.FriendStatusUpdate,
                    Sender = toUserId,
                    Receiver = fromUserId,
                    Content = "FRIEND_ACCEPTED",
                    Timestamp = DateTime.Now
                };
                SendToUser(notify, fromUserId);
            }
        }
        private void HandleRejectFriendRequest(ChatPacket packet, ClientInfo clientInfo)
        {
            string fromUserId = packet.Content; // 发起请求的用户
            string toUserId = clientInfo.Username; // 当前登录用户（拒绝者）

            Console.WriteLine($"拒绝好友请求: {toUserId} 拒绝 {fromUserId}");

            // 调用业务层拒绝请求，更新请求状态为“已拒绝”
            bool success = _friendService.RejectFriendRequest(fromUserId, toUserId);

            // 构造响应包
            ChatPacket response = new ChatPacket
            {
                Type = MessageType.RejectFriendResponse,
                Sender = "Server",
                Receiver = toUserId,
                MessageId = packet.MessageId,
                Content = success ? "SUCCESS" : "FAILED",
                Timestamp = DateTime.Now
            };
            SendToClient(response, clientInfo);

            if (success)
            {
                // 可选：通知原发起者请求被拒绝
                ChatPacket notify = new ChatPacket
                {
                    Type = MessageType.FriendStatusUpdate,
                    Sender = toUserId,
                    Receiver = fromUserId,
                    Content = "FRIEND_REJECTED",
                    Timestamp = DateTime.Now
                };
                SendToUser(notify, fromUserId);
            }
        }
        // 处理登录请求
        private void HandleLogin(ChatPacket packet, ClientInfo clientInfo)
        {
            string username = packet.Sender;
            string password = packet.Content;

            Console.WriteLine($"登录请求: {username}");

            // 调用业务层验证登录
            var user = _userService.Login(username, password);

            ChatPacket response = null;

            if (user != null)
            {
                // 登录成功
                clientInfo.Username = username;
                clientInfo.UserId = user.UserId;

                // 设置用户在线状态
                _userService.UpdateUserStatus(user.UserId, true);

                // 创建成功响应
                response = new ChatPacket
                {
                    Type = MessageType.LoginResponse,
                    Sender = "Server",
                    Receiver = username,
                    MessageId = packet.MessageId,
                    Content = "SUCCESS",
                    Timestamp = DateTime.Now
                };

                // 添加用户信息
                response.Extras["UserInfo"] = JsonConvert.SerializeObject(user);

                Console.WriteLine($"登录成功: {username}");
            }
            else
            {
                // 登录失败
                response = new ChatPacket
                {
                    Type = MessageType.LoginResponse,
                    Sender = "Server",
                    Receiver = username,
                    MessageId = packet.MessageId,
                    Content = "FAILED",
                    Timestamp = DateTime.Now
                };

                Console.WriteLine($"登录失败: {username}");
            }

            // 发送响应
            SendToClient(response, clientInfo);
        }

        // 处理注册请求 - 完整实现
        private void HandleRegister(ChatPacket packet, ClientInfo clientInfo)
        {
            Console.WriteLine($"注册请求");

            try
            {
                // 解析用户信息
                User newUser = JsonConvert.DeserializeObject<User>(packet.Content);

                // 调用业务层注册
                bool success = _userService.Register(newUser);

                // 创建响应
                var response = new ChatPacket
                {
                    Type = MessageType.RegisterResponse,
                    Sender = "Server",
                    Receiver = packet.Sender,
                    MessageId = packet.MessageId,
                    Content = success ? "SUCCESS" : "FAILED",
                    Timestamp = DateTime.Now
                };

                if (success)
                {
                    Console.WriteLine($"注册成功: {newUser.Username}");
                }
                else
                {
                    Console.WriteLine($"注册失败: {newUser.Username} (用户名已存在)");
                }

                // 发送响应
                SendToClient(response, clientInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"注册处理出错: {ex.Message}");

                var response = new ChatPacket
                {
                    Type = MessageType.RegisterResponse,
                    Sender = "Server",
                    Receiver = packet.Sender,
                    MessageId = packet.MessageId,
                    Content = "ERROR",
                    Timestamp = DateTime.Now
                };

                SendToClient(response, clientInfo);
            }
        }

        private void SearchIdChat(ChatPacket packet, ClientInfo senderInfo)
        {
            string sender = packet.Sender;
            string content = packet.Content;

            Console.WriteLine($"查询请求请求: {content}");
            // 调用业务层验证登录
            var user = _userService.SearchUser(content);

            ChatPacket response = null;

            if (user != null)
            {
                // 查询成功
                // 创建成功响应
                response = new ChatPacket
                {
                    Type = MessageType.SearchIdResponse,
                    Sender = "Server",
                    Content = "SUCCESS",
                    MessageId = packet.MessageId,
                    Timestamp = DateTime.Now
                };

                // 添加用户信息
                response.Extras["UserInfo"] = JsonConvert.SerializeObject(user);

                Console.WriteLine($"查询成功: {sender}");
            }
            else
            {
                response = new ChatPacket
                {
                    Type = MessageType.SearchIdResponse,
                    Sender = "Server",
                    Content = "FAILED",
                    MessageId = packet.MessageId,
                    Timestamp = DateTime.Now
                };

                Console.WriteLine($"查询失败: {sender}");
            }

            // 发送响应
            SendToClient(response, senderInfo);

        }
        private void HandleGetOfflineMessages(ChatPacket packet, ClientInfo clientInfo)
        {
            string userId = packet.Sender; // 登录后已设置
            Console.WriteLine($"处理离线消息请求: {userId}");

            // 获取未读私聊消息
            var unreadMessages = _messageService.GetUnreadMessages(userId);

            // 获取待处理的好友请求（FromUserId 列表）
            var friendRequests = _friendService.GetFriendRequests(userId);

            // 获取未读群消息
            // var unreadGroupMessages = _groupMessageService.GetUnreadMessages(userId);

            var response = new ChatPacket
            {
                Type = MessageType.GetOfflineMessagesResponse,
                Sender = "Server",
                Receiver = userId,
                MessageId = packet.MessageId,
                Content = "SUCCESS",
                Timestamp = DateTime.Now
            };

            // 将数据序列化放入 Extras
            if (unreadMessages != null && unreadMessages.Count > 0)
                response.Extras["OfflineMessages"] = JsonConvert.SerializeObject(unreadMessages);

            if (friendRequests != null && friendRequests.Count > 0)
                response.Extras["FriendRequests"] = JsonConvert.SerializeObject(friendRequests);

            // 发送响应
            SendToClient(response, clientInfo);
        }

        private void HandleSearchAllFriends(ChatPacket packet, ClientInfo clientInfo)
        {
            string userId = packet.Sender;
            Console.WriteLine($"查询所有好友请求: {userId}");

            // 调用业务层获取好友列表
            var friends = _friendService.GetFriendList(userId); 

            ChatPacket response;
            if (friends != null)
            {
                string friendsJson = JsonConvert.SerializeObject(friends);
                response = new ChatPacket
                {
                    Type = MessageType.SearchAllFriendsResponse,
                    Sender = "Server",
                    Receiver = userId,
                    MessageId = packet.MessageId,
                    Content = "SUCCESS",
                    Timestamp = DateTime.Now
                };
                response.Extras["FriendsList"] = friendsJson;
            }
            else
            {
                response = new ChatPacket
                {
                    Type = MessageType.SearchAllFriendsResponse,
                    Sender = "Server",
                    Receiver = userId,
                    MessageId = packet.MessageId,
                    Content = "FAILED",
                    Timestamp = DateTime.Now
                };
            }

            SendToClient(response, clientInfo);
        }
        // 处理聊天消息 - 完整实现
        private void HandleChatMessage(ChatPacket packet, ClientInfo senderInfo)
        {
            string sender = packet.Sender;
            string receiver = packet.Receiver;
            string content = packet.Content;

            Console.WriteLine($"聊天消息: {sender} -> {receiver}: {content}");

            // 1. 保存消息到数据库
            var message = new Message
            {
                SenderId = packet.Sender,
                ReceiverId = receiver, // 可能是用户名或"ALL"
                Content = content,
                SendTime = packet.Timestamp,
                IsRead = false,
                MessageType = 1 // 文本消息
            };

            _messageService.SendMessage(message);

            // 2. 转发给接收者
            if (receiver == "ALL")
            {
                // 广播给所有在线用户
                BroadcastMessage(packet, senderInfo.Username);
            }
            else
            {
                // 私聊 - 转发给指定用户
                SendToUser(packet, receiver);
            }

            // 3. 发送送达确认（可选）
            var ack = new ChatPacket
            {
                Type = MessageType.MessageReceived,
                Sender = "Server",
                Receiver = sender,
                MessageId = packet.MessageId,
                Content = "DELIVERED",
                Timestamp = DateTime.Now
            };
            SendToClient(ack, senderInfo);
        }
        
        // 处理添加好友请求
        private void HandleAddFriend(ChatPacket packet, ClientInfo senderInfo)
        {
            string fromUser = packet.Sender;
            string toUser = packet.Content;
            Console.WriteLine($"========== 处理添加好友请求 ==========");
            Console.WriteLine($"发送者: {fromUser}");
            Console.WriteLine($"接收者: {toUser}");
            Console.WriteLine($"消息ID: {packet.MessageId}");
            Console.WriteLine($"添加好友请求: {fromUser} -> {toUser}");
            Console.WriteLine($"[SocketServer] 调用 FriendService.AddFriendRequest({fromUser}, {toUser})");
            // 调用业务层处理好友请求
            bool success = _friendService.AddFriendRequest(fromUser, toUser);
            Console.WriteLine($"[SocketServer] FriendService 返回结果: {success}");
            var response = new ChatPacket
            {
                Type = MessageType.AddFriendResponse,
                Sender = "Server",
                Receiver = fromUser,
                MessageId = packet.MessageId,
                Content = success ? "SUCCESS" : "FAILED",
                Timestamp = DateTime.Now
            };
            Console.WriteLine($"[SocketServer] 发送响应给 {fromUser}, Content: {response.Content}");
            SendToClient(response, senderInfo);

            // 如果接收者在线，通知他有好友请求
            if (success)
            {
                var notification = new ChatPacket
                {
                    Type = MessageType.AddFriendRequest,
                    Sender = fromUser,
                    Receiver = toUser,
                    MessageId = packet.MessageId,
                    Content = $"{fromUser} 请求添加你为好友",
                    Timestamp = DateTime.Now
                };
                SendToUser(notification, toUser);
            }
        }

        // 处理心跳包
        private void HandleHeartbeat(ChatPacket packet, ClientInfo clientInfo)
        {
            clientInfo.LastHeartbeatTime = DateTime.Now;

            // 回复心跳
            var response = new ChatPacket
            {
                Type = MessageType.Heartbeat,
                Sender = "Server",
                Receiver = clientInfo.Username ?? "Unknown",
                Timestamp = DateTime.Now
            };

            SendToClient(response, clientInfo);
        }

        // 发送消息给指定客户端
        private void SendToClient(ChatPacket packet, ClientInfo clientInfo)
        {
            try
            {
                string json = packet.ToJson();
                byte[] data = Encoding.UTF8.GetBytes(json);
                byte[] lengthBytes = BitConverter.GetBytes(data.Length);

                // 发送长度
                clientInfo.Stream.Write(lengthBytes, 0, lengthBytes.Length);
                // 发送数据
                clientInfo.Stream.Write(data, 0, data.Length);
                clientInfo.Stream.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发送失败: {ex.Message}");
                RemoveClient(clientInfo);
            }
        }

        // 发送消息给指定用户
        private void SendToUser(ChatPacket packet, string username)
        {
            lock (_clients)
            {
                var client = _clients.Find(c => c.Username == username);
                if (client != null)
                {
                    SendToClient(packet, client);
                }
            }
        }

        // 广播消息给所有在线用户（除了发送者）
        private void BroadcastMessage(ChatPacket packet, string excludeUser = null)
        {
            lock (_clients)
            {
                foreach (var client in _clients)
                {
                    if (client.Username != excludeUser)
                    {
                        SendToClient(packet, client);
                    }
                }
            }
        }

        // 移除客户端
        private void RemoveClient(ClientInfo clientInfo)
        {
            lock (_clients)
            {
                if (_clients.Contains(clientInfo))
                {
                    _clients.Remove(clientInfo);

                    // 更新用户在线状态
                    if (!string.IsNullOrEmpty(clientInfo.Username))
                    {
                        _userService.UpdateUserStatus(clientInfo.UserId, false);

                        // 通知其他用户该用户下线
                        var offlinePacket = new ChatPacket
                        {
                            Type = MessageType.FriendStatusUpdate,
                            Content = "OFFLINE",
                            Sender = clientInfo.Username,
                            Timestamp = DateTime.Now
                        };
                        BroadcastMessage(offlinePacket, clientInfo.Username);
                    }

                    try
                    {
                        clientInfo.TcpClient.Close();
                    }
                    catch { }

                    Console.WriteLine($"客户端断开: {clientInfo.RemoteEndPoint}");
                    Console.WriteLine($"当前在线客户端数: {_clients.Count}");
                }
            }
        }

        // 心跳检测
        private void HeartbeatCheck()
        {
            while (_isRunning)
            {
                Thread.Sleep(30000); // 每30秒检查一次

                lock (_clients)
                {
                    var deadClients = new List<ClientInfo>();

                    foreach (var client in _clients)
                    {
                        // 如果超过10分钟没有收到心跳，认为客户端已死
                        if ((DateTime.Now - client.LastHeartbeatTime).TotalSeconds > 600)
                        {
                            deadClients.Add(client);
                        }
                    }

                    foreach (var client in deadClients)
                    {
                        Console.WriteLine($"客户端心跳超时: {client.RemoteEndPoint}");
                        RemoveClient(client);
                    }
                }
            }
        }

        // 获取服务器状态
        public ServerStatus GetServerStatus()
        {
            return new ServerStatus
            {
                IsRunning = _isRunning,
                ClientCount = _clients.Count,
                StartTime = DateTime.Now,
                Clients = _clients.ConvertAll(c => new ClientInfoBrief
                {
                    Username = c.Username,
                    RemoteEndPoint = c.RemoteEndPoint,
                    ConnectedTime = c.ConnectedTime,
                    LastActivityTime = c.LastActivityTime
                })
            };
        }
    }

   
}