using QQClient.Business;
using QQCommon.Interfaces;
using QQCommon.Models;
using QQCommon.Protocols;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QQClientBusiness.Tests
{
    //模拟的网络客户端（用于测试）
    public class MockNetworkClient : INetworkClient
    {
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<ConnectionEventArgs> ConnectionChanged;

        private ChatPacket _mockResponse;
        private bool _shouldTimeout;

        public MockNetworkClient(ChatPacket mockResponse, bool shouldTimeout = false)
        {
            _mockResponse = mockResponse;
            _shouldTimeout = shouldTimeout;
        }

        public bool Connect(string serverIp, int port)
        {
            return true;
        }

        public void Disconnect()
        {
            // 模拟断开
        }

        public bool IsConnected()
        {
            return true;
        }

        public void SendPacket(ChatPacket packet)
        {
            Console.WriteLine($"[MockClient] 发送请求: Type={packet.Type}, MessageId={packet.MessageId}");

            if (_shouldTimeout)
            {
                // 不发送响应，模拟超时
                return;
            }

            // 模拟异步响应
            Task.Delay(100).ContinueWith(_ =>
            {
                // 创建响应包，使用相同的 MessageId
                var response = new ChatPacket
                {
                    Type = GetResponseType(packet.Type),
                    MessageId = packet.MessageId,
                    Content = "SUCCESS",
                    Timestamp = DateTime.Now
                };

                Console.WriteLine($"[MockClient] 模拟发送响应: Type={response.Type}, MessageId={response.MessageId}");
                MessageReceived?.Invoke(this, new MessageReceivedEventArgs(response));
            });
        }

        private MessageType GetResponseType(MessageType requestType)
        {
            switch (requestType)
            {
                case MessageType.LoginRequest:
                    return MessageType.LoginResponse;
                case MessageType.RegisterRequest:
                    return MessageType.RegisterResponse;
                case MessageType.GetUserInfoRequest:
                    return MessageType.GetUserInfoResponse;
                case MessageType.UpdateUserInfoRequest:
                    return MessageType.UpdateUserInfoResponse;
                default:
                    return MessageType.Error;
            }
        }

        // 实现 INetworkClient 接口的其他方法
        public bool Login(string username, string password) => false;
        public bool Register(string username, string password, string nickname) => false;
        public bool SendMessage(string username, string receiver, string content) => false;
        public bool AddFriend(string fromUserId, string toUserId) => false;
        public bool SearchId(string fromUserId, string userId) => false;
        public bool AcceptFriendRequest(string fromUserId) => false;
        public bool RejectFriendRequest(string fromUserId) => false;
        public List<Friend> SearchAllFriends(string userId) => null;
        public List<Message> GetOfflineMessages(out List<string> friendRequests) { friendRequests = null; return null; }
        public List<Message> GetHistoryMessages(string friendId) => null;
        public bool MarkMessagesAsRead(string friendId) => false;
        public User GetUserInfo(string userId) => null;
        public bool UpdateUserInfo(User updatedUser) => false;
        public List<Group> GetGroupList() => null;
        public bool SendGroupMessage(string groupId, string content) => false;
        public List<GroupMessage> GetGroupHistory(string groupId, int limit = 50) => null;
        public string CreateGroup(string groupName, string description = "") => null;
        public bool InviteToGroup(string groupId, string invitedUserId) => false;
        public List<Group> SearchGroups(string keyword) => null;
        public bool JoinGroup(string groupId) => false;
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== RequestManager 测试开始 ===\n");

            // 测试1: 正常请求-响应
            await TestNormalRequest();

            // 测试2: 超时测试
            await TestTimeout();

            // 测试3: 取消所有等待请求
            await TestCancelAll();

            Console.WriteLine("\n=== 所有测试完成 ===");
            Console.ReadLine();
        }

        static async Task TestNormalRequest()
        {
            Console.WriteLine("测试1: 正常请求-响应");

            var mockResponse = new ChatPacket
            {
                Type = MessageType.LoginResponse,
                Content = "SUCCESS"
            };

            var mockClient = new MockNetworkClient(mockResponse);

            var request = new ChatPacket
            {
                Type = MessageType.LoginRequest,
                Sender = "testuser",
                Content = "password",
                MessageId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now
            };

            try
            {
                var response = await RequestManager.SendRequestAsync(mockClient, request, 5000);
                Console.WriteLine($"结果: 成功收到响应, Type={response.Type}, Content={response.Content}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"结果: 失败 - {ex.Message}\n");
            }
        }

        static async Task TestTimeout()
        {
            Console.WriteLine("测试2: 请求超时");

            var mockClient = new MockNetworkClient(null, true);

            var request = new ChatPacket
            {
                Type = MessageType.RegisterRequest,
                Sender = "testuser",
                Content = "testdata",
                MessageId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.Now
            };

            try
            {
                var response = await RequestManager.SendRequestAsync(mockClient, request, 2000);
                Console.WriteLine($"结果: 意外成功 - {response.Type}\n");
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine($"结果: 超时异常 - {ex.Message}\n");
            }
        }

        static async Task TestCancelAll()
        {
            Console.WriteLine("测试3: 取消所有等待请求");

            var mockClient = new MockNetworkClient(null, true);

            // 发送多个请求但不等待响应
            for (int i = 0; i < 3; i++)
            {
                var request = new ChatPacket
                {
                    Type = MessageType.LoginRequest,
                    Sender = $"user{i}",
                    Content = "password",
                    MessageId = Guid.NewGuid().ToString(),
                    Timestamp = DateTime.Now
                };

                // 不等待结果，直接发送
                _ = RequestManager.SendRequestAsync(mockClient, request, 10000)
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                            Console.WriteLine($"请求 {request.MessageId} 失败: {t.Exception?.InnerException?.Message}");
                    });
            }

            Console.WriteLine($"发送了3个请求，当前等待数量: {RequestManager.GetPendingCount()}");

            // 等待1秒让请求注册完成
            await Task.Delay(1000);

            // 取消所有等待请求
            RequestManager.CancelAllPendingRequests();

            await Task.Delay(500);
            Console.WriteLine($"取消后等待数量: {RequestManager.GetPendingCount()}\n");
        }
    }
}