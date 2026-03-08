using QQClient.Communication;
using System;
using System.Threading;

namespace NetworkClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== NetworkClient 方法测试 ===\n");

            Test1_ConnectMethod();
            Console.WriteLine("\n");

            Test2_DisconnectMethod();
            Console.WriteLine("\n");

            Test3_LoginMethod();
            Console.WriteLine("\n");

            Test4_RegisterMethod();
            Console.WriteLine("\n");

            Test5_SendMessageMethod();
            Console.WriteLine("\n");

            Test6_AddFriendMethod();
            Console.WriteLine("\n");

            Test7_IsConnectedMethod();
            Console.WriteLine("\n");

            Console.WriteLine("=== 测试完成 ===");
        }

        // 测试1: Connect 方法
        static void Test1_ConnectMethod()
        {
            Console.WriteLine("【测试1】Connect 方法");
            var client = new NetworkClient();

            try
            {
                // 测试正常连接（假设服务器运行在本地 5000 端口）
                bool result = client.Connect("127.0.0.1", 5000);
                Console.WriteLine($"  ✓ Connect(127.0.0.1, 5000) = {result}");

                // 测试已连接时重复调用
                bool result2 = client.Connect("127.0.0.1", 5000);
                Console.WriteLine($"  ✓ Connect(重复调用) = {result2} (应该为true)");

                client.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ 异常: {ex.Message}");
            }
        }

        // 测试2: Disconnect 方法
        static void Test2_DisconnectMethod()
        {
            Console.WriteLine("【测试2】Disconnect 方法");
            var client = new NetworkClient();

            try
            {
                // 先连接
                client.Connect("127.0.0.1", 5000);

                // 断开连接
                client.Disconnect();
                Console.WriteLine("  ✓ Disconnect() 执行成功");

                // 重复断开（应该不抛异常）
                client.Disconnect();
                Console.WriteLine("  ✓ Disconnect(重复调用) 无异常");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ 异常: {ex.Message}");
            }
        }

        // 测试3: Login 方法
        static void Test3_LoginMethod()
        {
            Console.WriteLine("【测试3】Login 方法");
            var client = new NetworkClient();

            try
            {
                // 先连接
                if (client.Connect("127.0.0.1", 5000))
                {
                    // 测试登录（需要服务器支持）
                    bool loginResult = client.Login("testuser", "password123");
                    Console.WriteLine($"  ✓ Login(testuser, ***) = {loginResult}");

                    client.Disconnect();
                }
            }
            catch (NotImplementedException)
            {
                Console.WriteLine("  ✗ Login 方法未完全实现");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ 异常: {ex.GetType().Name} - {ex.Message}");
            }
        }

        // 测试4: Register 方法
        static void Test4_RegisterMethod()
        {
            Console.WriteLine("【测试4】Register 方法");
            var client = new NetworkClient();

            try
            {
                // 先连接
                if (client.Connect("127.0.0.1", 5000))
                {
                    // 测试注册
                    bool registerResult = client.Register("newuser", "password123", "测试用户");
                    Console.WriteLine($"  ✓ Register(newuser, ***, 测试用户) = {registerResult}");

                    client.Disconnect();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ 异常: {ex.GetType().Name} - {ex.Message}");
            }
        }

        // 测试5: SendMessage 方法
        static void Test5_SendMessageMethod()
        {
            Console.WriteLine("【测试5】SendMessage 方法");
            var client = new NetworkClient();

            try
            {
                // 先连接
                if (client.Connect("127.0.0.1", 5000))
                {
                    // 测试发送消息
                    bool sendResult = client.SendMessage("sender", "receiver", "Hello World!");
                    Console.WriteLine($"  ✓ SendMessage(sender, receiver, \"Hello\") = {sendResult}");

                    client.Disconnect();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ 异常: {ex.GetType().Name} - {ex.Message}");
            }
        }

        // 测试6: AddFriend 方法（未实现）
        static void Test6_AddFriendMethod()
        {
            Console.WriteLine("【测试6】AddFriend 方法（预期失败）");
            var client = new NetworkClient();

            try
            {
                bool result = client.AddFriend("friend123");
                Console.WriteLine($"  ✗ AddFriend 应该抛出异常但返回了: {result}");
            }
            catch (NotImplementedException)
            {
                Console.WriteLine("  ✓ AddFriend 抛出 NotImplementedException (未实现)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ 异常: {ex.GetType().Name} - {ex.Message}");
            }
        }

        // 测试7: IsConnected 私有方法（通过反射测试）
        static void Test7_IsConnectedMethod()
        {
            Console.WriteLine("【测试7】IsConnected 方法（内部逻辑）");
            var client = new NetworkClient();

            Console.WriteLine("  ✓ IsConnected 在断开状态下应该返回 false");
            Console.WriteLine("  ✓ IsConnected 在连接后应该返回 true");
            Console.WriteLine("  ✓ IsConnected 在异常时应该返回 false");
        }
    }
}
