using System;
using QQServer.Communication;

namespace QQServer.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "QQ服务器";

            var server = new SocketServer();

            try
            {
                server.Start(8888);

                Console.WriteLine("\n服务器命令:");
                Console.WriteLine("  status - 查看服务器状态");
                Console.WriteLine("  quit/q - 关闭服务器");
                Console.WriteLine("  help   - 显示帮助");

                while (true)
                {
                    Console.Write("\n> ");
                    string command = Console.ReadLine()?.ToLower().Trim();

                    if (command == "quit" || command == "q" || command == "exit")
                    {
                        break;
                    }
                    else if (command == "status" || command == "st")
                    {
                        var status = server.GetServerStatus();
                        Console.WriteLine("\n=== 服务器状态 ===");
                        Console.WriteLine($"运行状态: {(status.IsRunning ? "运行中" : "已停止")}");
                        Console.WriteLine($"在线客户端: {status.ClientCount}");
                        Console.WriteLine($"启动时间: {status.StartTime:yyyy-MM-dd HH:mm:ss}");

                        if (status.Clients.Count > 0)
                        {
                            Console.WriteLine("\n在线客户端列表:");
                            foreach (var client in status.Clients)
                            {
                                Console.WriteLine($"  - {client.Username ?? "未登录"} [{client.RemoteEndPoint}]");
                                Console.WriteLine($"    连接时间: {client.ConnectedTime:HH:mm:ss}");
                                Console.WriteLine($"    最后活动: {client.LastActivityTime:HH:mm:ss}");
                            }
                        }
                    }
                    else if (command == "help")
                    {
                        Console.WriteLine("\n可用命令:");
                        Console.WriteLine("  status/st - 查看服务器状态");
                        Console.WriteLine("  quit/q    - 关闭服务器");
                        Console.WriteLine("  help      - 显示此帮助");
                    }
                }

                server.Stop();
                Console.WriteLine("服务器已关闭，按任意键退出...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"服务器运行错误: {ex.Message}");
                Console.WriteLine("按任意键退出...");
                Console.ReadKey();
            }
        }
    }
}
