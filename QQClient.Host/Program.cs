using QQClient.Business;
using QQClient.Business.Services;
using QQClient.Communication;
using QQClient.DataAccess;
using QQClient.DataAccess.Repositories;
using QQCommon.Interfaces;
using QQCommon.Models;
using System;
using System.Windows.Forms;

namespace QQClient.Host
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string ip = "127.0.0.1";
            int port = 8888;

            // ========== 初始化新架构 ==========
            try
            {
                // 1. 创建新版网络客户端（精简版）
                var networkClient = new NetworkClient();
                bool clientConnected = networkClient.Connect(ip, port);

                if (clientConnected)
                {
                    Console.WriteLine("[Program] 网络客户端连接成功");

                    // 2. 初始化数据访问层
                    string connectionString = "Data Source=.;Initial Catalog=QQClient;Integrated Security=True;";
                    var messageRepo = new MessageRepository();
                    var friendRepo = new FriendRepository();
                    var groupRepo = new GroupRepository();

                    // 3. 创建业务服务实例
                    var userService = new UserBusinessService(networkClient);
                    var friendService = new FriendBusinessService(networkClient, messageRepo, friendRepo, groupRepo);
                    var messageService = new MessageBusinessService(networkClient, messageRepo, friendRepo, groupRepo, null);
                    var groupService = new GroupBusinessService(networkClient, messageRepo, friendRepo, groupRepo, null);

                    // 4. 注册服务到容器
                    ServiceContainer.Register<IUserBusinessService>(userService);
                    ServiceContainer.Register<IFriendBusinessService>(friendService);
                    ServiceContainer.Register<IMessageBusinessService>(messageService);
                    ServiceContainer.Register<IGroupBusinessService>(groupService);
                    ServiceContainer.Register<INetworkClient>(networkClient);

                    Console.WriteLine("[Program] 服务注册完成");

                    // 5. 设置旧版兼容（逐步废弃）
                    GlobalClient.Current = networkClient;

                    // 6. 启动登录窗体
                    Application.Run(new QQClient.UI.login_new());
                    return;
                }
                else
                {
                    Console.WriteLine("[Program] 网络客户端连接失败");
                    MessageBox.Show("连接服务器失败");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Program] 初始化失败: {ex.Message}");
                MessageBox.Show($"初始化失败: {ex.Message}");
            }
        }
    }
}