using QQClient.Business;
using QQClient.Business.Services;
using QQClient.Communication;
using QQCommon.Interfaces;
using System;
using System.Threading.Tasks;

namespace QQClient.Business.Tests
{
    class Program
    {
        private static INetworkClient _client;
        private static IUserBusinessService _userService;
        private static IFriendBusinessService _friendService;
        private static IMessageBusinessService _messageService;
        private static IGroupBusinessService _groupService;
        private static string _currentUserId;

        static async Task Main(string[] args)
        {
            Console.WriteLine("=== QQClient Business Service 测试 ===\n");

            // 连接服务器
            _client = new NetworkClient();
            if (!_client.Connect("127.0.0.1", 8888))
            {
                Console.WriteLine("连接服务器失败");
                return;
            }
            Console.WriteLine("连接服务器成功\n");

            // 创建服务
            _userService = new UserBusinessService(_client);
            _friendService = new FriendBusinessService(_client);
            _messageService = new MessageBusinessService(_client);
            _groupService = new GroupBusinessService(_client);

            // 订阅事件
            _userService.MessageReceived += OnMessageReceived;
            _friendService.MessageReceived += OnMessageReceived;
            _messageService.MessageReceived += OnMessageReceived;
            _groupService.MessageReceived += OnMessageReceived;

            // 测试登录
            await TestLogin();

            if (!string.IsNullOrEmpty(_currentUserId))
            {
                // 设置当前用户ID
                ((MessageBusinessService)_messageService).SetCurrentUserId(_currentUserId);
                ((GroupBusinessService)_groupService).SetCurrentUserId(_currentUserId);

                // 测试其他功能
                await TestGetFriendList();
                await TestSendMessage();
                await TestGetHistoryMessages();
                await TestGetGroupList();
                await TestSendGroupMessage();
            }

            Console.WriteLine("\n测试完成，按任意键退出...");
            Console.ReadKey();
            _client.Disconnect();
        }

        static async Task TestLogin()
        {
            Console.WriteLine("--- 测试登录 ---");
            Console.Write("请输入用户名: ");
            string username = Console.ReadLine();
            Console.Write("请输入密码: ");
            string password = Console.ReadLine();

            bool success = await _userService.LoginAsync(username, password);
            if (success)
            {
                _currentUserId = username;
                Console.WriteLine($"登录成功！当前用户: {_currentUserId}\n");
            }
            else
            {
                Console.WriteLine("登录失败！\n");
            }
        }

        static async Task TestGetFriendList()
        {
            Console.WriteLine("--- 测试获取好友列表 ---");
            var friends = await _friendService.GetFriendListAsync(_currentUserId);
            Console.WriteLine($"好友数量: {friends.Count}");
            foreach (var friend in friends)
            {
                Console.WriteLine($"  - {friend.FriendNickName ?? friend.FriendUserName}");
            }
            Console.WriteLine();
        }

        static async Task TestSendMessage()
        {
            Console.WriteLine("--- 测试发送消息 ---");
            Console.Write("请输入好友账号: ");
            string friendId = Console.ReadLine();
            Console.Write("请输入消息内容: ");
            string content = Console.ReadLine();

            bool success = await _messageService.SendMessageAsync(friendId, content);
            Console.WriteLine(success ? "消息发送成功\n" : "消息发送失败\n");
        }

        static async Task TestGetHistoryMessages()
        {
            Console.WriteLine("--- 测试获取历史消息 ---");
            Console.Write("请输入好友账号: ");
            string friendId = Console.ReadLine();

            var messages = await _messageService.GetHistoryMessagesAsync(friendId);
            Console.WriteLine($"历史消息数量: {messages.Count}");
            foreach (var msg in messages)
            {
                Console.WriteLine($"  [{msg.SendTime:HH:mm:ss}] {msg.SenderId}: {msg.Content}");
            }
            Console.WriteLine();
        }

        static async Task TestGetGroupList()
        {
            Console.WriteLine("--- 测试获取群列表 ---");
            var groups = await _groupService.GetGroupListAsync();
            Console.WriteLine($"群组数量: {groups.Count}");
            foreach (var group in groups)
            {
                Console.WriteLine($"  - {group.GroupName} ({group.GroupId})");
            }
            Console.WriteLine();
        }

        static async Task TestSendGroupMessage()
        {
            Console.WriteLine("--- 测试发送群消息 ---");
            Console.Write("请输入群ID: ");
            string groupId = Console.ReadLine();
            Console.Write("请输入消息内容: ");
            string content = Console.ReadLine();

            bool success = await _groupService.SendGroupMessageAsync(groupId, content);
            Console.WriteLine(success ? "群消息发送成功\n" : "群消息发送失败\n");
        }

        static void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine($"[推送消息] Type={e.Packet.Type}, Sender={e.Packet.Sender}, Content={e.Packet.Content}");
        }
    }
}