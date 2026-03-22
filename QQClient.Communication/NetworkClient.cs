using QQCommon.Interfaces;
using QQCommon.Models;
using QQCommon.Protocols;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QQClient.Communication
{

    // 精简版网络客户端 - 只负责底层通信

    public class NetworkClient : INetworkClient
    {
        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private Thread _receiveThread;
        private bool _isRunning;
        private readonly object _streamLock = new object();

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<ConnectionEventArgs> ConnectionChanged;

    
        /// 检查是否已连接
    
        public bool IsConnected()
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

    
        /// 连接服务器
    
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
            catch (Exception ex)
            {
                Console.WriteLine($"[NetworkClient] 连接失败: {ex.Message}");
                return false;
            }
        }

    
        /// 断开连接
    
        public void Disconnect()
        {
            Console.WriteLine($"[NetworkClient] Disconnect 被调用");
            _isRunning = false;

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

    
        /// 发送数据包（核心方法）
    
        public void SendPacket(ChatPacket packet)
        {
            if (packet == null)
                throw new ArgumentNullException(nameof(packet));

            if (!IsConnected())
                throw new InvalidOperationException("未连接到服务器");

            string json = packet.ToJson();
            byte[] data = Encoding.UTF8.GetBytes(json);
            byte[] lengthBytes = BitConverter.GetBytes(data.Length);

            lock (_streamLock)
            {
                _stream.Write(lengthBytes, 0, lengthBytes.Length);
                _stream.Write(data, 0, data.Length);
            }

            Console.WriteLine($"[NetworkClient] 发送数据包: Type={packet.Type}, MessageId={packet.MessageId}");
        }

    
        /// 接收循环（运行在独立线程）
    
        private void ReceiveLoop()
        {
            Console.WriteLine("[NetworkClient] 接收线程启动");
            while (_isRunning && IsConnected())
            {
                try
                {
                    // 等待数据可用
                    while (_isRunning && IsConnected() && !_stream.DataAvailable)
                    {
                        Thread.Sleep(10);
                    }
                    if (!_isRunning || !IsConnected()) break;

                    var packet = ReceivePacketInternal();
                    if (packet == null)
                    {
                        Console.WriteLine("[NetworkClient] ReceivePacketInternal 返回 null，连接已关闭");
                        break;
                    }

                    Console.WriteLine($"[NetworkClient] 收到数据包: Type={packet.Type}, MessageId={packet.MessageId}");
                    OnMessageReceived(packet);
                }
                catch (TimeoutException)
                {
                    continue;
                }
                catch (ObjectDisposedException)
                {
                    Console.WriteLine("[NetworkClient] 流已关闭，退出循环");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[NetworkClient] 接收循环异常: {ex.Message}");
                    break;
                }
            }

            Console.WriteLine("[NetworkClient] 接收线程退出");
            OnConnectionChanged(false, "连接已断开");
            Disconnect();
        }

    
        /// 内部接收数据包
    
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

        protected virtual void OnMessageReceived(ChatPacket packet)
        {
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(packet));
        }

        protected virtual void OnConnectionChanged(bool isConnected, string message)
        {
            ConnectionChanged?.Invoke(this, new ConnectionEventArgs(isConnected, message));
        }

        // ========== INetworkClient 接口的旧业务方法（临时实现，逐步移除）==========
        // 注意：这些方法在精简版中不应该被调用，但为了满足接口要求，临时实现
        // 当所有 UI 迁移到业务服务后，这些方法将被移除

        [Obsolete("请使用 UserService.LoginAsync 代替")]
        public bool Login(string username, string password)
        {
            throw new NotSupportedException("请使用业务服务层的方法");
        }

        [Obsolete("请使用 UserService.RegisterAsync 代替")]
        public bool Register(string username, string password, string nickname)
        {
            throw new NotSupportedException("请使用业务服务层的方法");
        }

        [Obsolete("请使用 MessageService.SendMessageAsync 代替")]
        public bool SendMessage(string username, string receiver, string content)
        {
            throw new NotSupportedException("请使用业务服务层的方法");
        }

        [Obsolete("请使用 FriendService.AddFriendAsync 代替")]
        public bool AddFriend(string fromUserId, string toUserId)
        {
            throw new NotSupportedException("请使用业务服务层的方法");
        }

        [Obsolete("请使用 FriendService.SearchUserAsync 代替")]
        public bool SearchId(string fromUserId, string userId)
        {
            throw new NotSupportedException("请使用业务服务层的方法");
        }

        [Obsolete("请使用 FriendService.AcceptFriendRequestAsync 代替")]
        public bool AcceptFriendRequest(string fromUserId)
        {
            throw new NotSupportedException("请使用业务服务层的方法");
        }

        [Obsolete("请使用 FriendService.RejectFriendRequestAsync 代替")]
        public bool RejectFriendRequest(string fromUserId)
        {
            throw new NotSupportedException("请使用业务服务层的方法");
        }

        [Obsolete("请使用 FriendService.GetFriendListAsync 代替")]
        public System.Collections.Generic.List<Friend> SearchAllFriends(string userId)
        {
            throw new NotSupportedException("请使用业务服务层的方法");
        }

        [Obsolete("请使用 MessageService.GetOfflineMessagesAsync 代替")]
        public System.Collections.Generic.List<Message> GetOfflineMessages(out System.Collections.Generic.List<string> friendRequests)
        {
            throw new NotSupportedException("请使用业务服务层的方法");
        }

        [Obsolete("请使用 MessageService.GetHistoryMessagesAsync 代替")]
        public System.Collections.Generic.List<Message> GetHistoryMessages(string friendId)
        {
            throw new NotSupportedException("请使用业务服务层的方法");
        }

        [Obsolete("请使用 MessageService.MarkMessagesAsReadAsync 代替")]
        public bool MarkMessagesAsRead(string friendId)
        {
            throw new NotSupportedException("请使用业务服务层的方法");
        }

        [Obsolete("请使用 UserService.GetUserInfoAsync 代替")]
        public User GetUserInfo(string userId)
        {
            throw new NotSupportedException("请使用业务服务层的方法");
        }

        [Obsolete("请使用 UserService.UpdateUserInfoAsync 代替")]
        public bool UpdateUserInfo(User updatedUser)
        {
            throw new NotSupportedException("请使用业务服务层的方法");
        }

        [Obsolete("请使用 GroupService.GetGroupListAsync 代替")]
        public System.Collections.Generic.List<Group> GetGroupList()
        {
            throw new NotSupportedException("请使用业务服务层的方法");
        }

        [Obsolete("请使用 GroupService.SendGroupMessageAsync 代替")]
        public bool SendGroupMessage(string groupId, string content)
        {
            throw new NotSupportedException("请使用业务服务层的方法");
        }

        [Obsolete("请使用 GroupService.GetGroupHistoryAsync 代替")]
        public System.Collections.Generic.List<GroupMessage> GetGroupHistory(string groupId, int limit = 50)
        {
            throw new NotSupportedException("请使用业务服务层的方法");
        }

        [Obsolete("请使用 GroupService.CreateGroupAsync 代替")]
        public string CreateGroup(string groupName, string description = "")
        {
            throw new NotSupportedException("请使用业务服务层的方法");
        }

        [Obsolete("请使用 GroupService.InviteToGroupAsync 代替")]
        public bool InviteToGroup(string groupId, string invitedUserId)
        {
            throw new NotSupportedException("请使用业务服务层的方法");
        }

        [Obsolete("请使用 GroupService.SearchGroupsAsync 代替")]
        public System.Collections.Generic.List<Group> SearchGroups(string keyword)
        {
            throw new NotSupportedException("请使用业务服务层的方法");
        }

        [Obsolete("请使用 GroupService.JoinGroupAsync 代替")]
        public bool JoinGroup(string groupId)
        {
            throw new NotSupportedException("请使用业务服务层的方法");
        }
    }
}