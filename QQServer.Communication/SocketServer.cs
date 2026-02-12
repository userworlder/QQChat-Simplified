// QQServer.Communication/SocketServer.cs
using Newtonsoft.Json;
using QQCommon.Interfaces;
using QQCommon.Models;
using QQCommon.Protocols;
using QQServer.Business;
using System;
using System.Collections.Generic;
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

        // 业务服务实例（由成员B实现）
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

        // 处理单个客户端
        private void HandleClient(ClientInfo clientInfo)
        {
            NetworkStream stream = clientInfo.Stream;
            byte[] buffer = new byte[8192]; // 8KB缓冲区

            while (_isRunning && clientInfo.TcpClient.Connected)
            {
                try
                {
                    // 读取消息（以\n分隔）
                    List<byte> messageBytes = new List<byte>();
                    int b;
                    while ((b = stream.ReadByte()) != '\n')
                    {
                        if (b == -1)
                        {
                            // 客户端断开连接
                            throw new Exception("客户端断开连接");
                        }
                        messageBytes.Add((byte)b);
                    }

                    if (messageBytes.Count == 0)
                        continue;

                    // 解析JSON
                    string json = Encoding.UTF8.GetString(messageBytes.ToArray());
                    ChatPacket packet = ChatPacket.FromJson(json);

                    // 处理消息
                    ProcessPacket(packet, clientInfo);

                    // 更新最后活动时间
                    clientInfo.LastActivityTime = DateTime.Now;
                }
                catch (Exception ex)
                {
                    // 客户端断开连接
                    Console.WriteLine($"客户端 {clientInfo.RemoteEndPoint} 断开连接: {ex.Message}");
                    RemoveClient(clientInfo);
                    break;
                }
            }
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

                default:
                    Console.WriteLine($"未知消息类型: {packet.Type}");
                    break;
            }
        }

        // 处理登录请求
        private void HandleLogin(ChatPacket packet, ClientInfo clientInfo)
        {
            string username = packet.Sender;
            string password = packet.Content;

            Console.WriteLine($"🔐 登录请求: {username}");

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
                    Content = "ERROR",
                    Timestamp = DateTime.Now
                };

                SendToClient(response, clientInfo);
            }
        }

        // 处理聊天消息 - 完整实现
        private void HandleChatMessage(ChatPacket packet, ClientInfo senderInfo)
        {
            string sender = packet.Sender;
            string receiver = packet.Receiver;
            string content = packet.Content;

            Console.WriteLine($"💬 聊天消息: {sender} -> {receiver}: {content}");

            // 1. 保存消息到数据库
            var message = new Message
            {
                SenderId = senderInfo.UserId,
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
                Content = "DELIVERED",
                Timestamp = DateTime.Now
            };
            SendToClient(ack, senderInfo);
        }

        // 处理添加好友请求
        private void HandleAddFriend(ChatPacket packet, ClientInfo senderInfo)
        {
            string fromUser = packet.Sender;
            string toUser = packet.Receiver;

            Console.WriteLine($"添加好友请求: {fromUser} -> {toUser}");

            // 调用业务层处理好友请求
            bool success = _friendService.AddFriendRequest(fromUser, toUser);

            var response = new ChatPacket
            {
                Type = MessageType.AddFriendResponse,
                Sender = "Server",
                Receiver = fromUser,
                Content = success ? "SUCCESS" : "FAILED",
                Timestamp = DateTime.Now
            };

            SendToClient(response, senderInfo);

            // 如果接收者在线，通知他有好友请求
            if (success)
            {
                var notification = new ChatPacket
                {
                    Type = MessageType.AddFriendRequest,
                    Sender = fromUser,
                    Receiver = toUser,
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
                byte[] data = Encoding.UTF8.GetBytes(json + "\n");

                if (clientInfo.Stream != null && clientInfo.TcpClient.Connected)
                {
                    clientInfo.Stream.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 发送消息失败: {ex.Message}");
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

                    Console.WriteLine($"📴 客户端断开: {clientInfo.RemoteEndPoint}");
                    Console.WriteLine($"📊 当前在线客户端数: {_clients.Count}");
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
                        // 如果超过1分钟没有收到心跳，认为客户端已死
                        if ((DateTime.Now - client.LastHeartbeatTime).TotalSeconds > 60)
                        {
                            deadClients.Add(client);
                        }
                    }

                    foreach (var client in deadClients)
                    {
                        Console.WriteLine($"💀 客户端心跳超时: {client.RemoteEndPoint}");
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

    // 客户端信息类
    public class ClientInfo
    {
        public TcpClient TcpClient { get; set; }
        public NetworkStream Stream { get; set; }
        public string RemoteEndPoint { get; set; }
        public string Username { get; set; }
        public string UserId { get; set; }
        public DateTime ConnectedTime { get; set; }
        public DateTime LastActivityTime { get; set; }
        public DateTime LastHeartbeatTime { get; set; }

        public ClientInfo()
        {
            LastActivityTime = DateTime.Now;
            LastHeartbeatTime = DateTime.Now;
        }
    }

    // 客户端信息简表（用于状态查询）
    public class ClientInfoBrief
    {
        public string Username { get; set; }
        public string RemoteEndPoint { get; set; }
        public DateTime ConnectedTime { get; set; }
        public DateTime LastActivityTime { get; set; }
    }

    // 服务器状态
    public class ServerStatus
    {
        public bool IsRunning { get; set; }
        public int ClientCount { get; set; }
        public DateTime StartTime { get; set; }
        public List<ClientInfoBrief> Clients { get; set; }
    }
}