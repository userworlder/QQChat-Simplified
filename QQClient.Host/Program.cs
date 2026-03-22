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
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 启用 WinForms 的视觉样式，使控件外观与系统一致
            Application.EnableVisualStyles();
            // 设置控件文本渲染使用兼容方式（避免高DPI问题）
            Application.SetCompatibleTextRenderingDefault(false);

            // 服务器地址和端口（本地调试）
            string ip = "127.0.0.1";
            int port = 8888;

            // ========== 初始化新架构（供后续迁移使用）==========
            try
            {
                // 1. 创建新版网络客户端（精简版，只负责通信，不处理业务逻辑）
                var newNetworkClient = new NetworkClient();
                // 尝试连接服务器
                bool newClientConnected = newNetworkClient.Connect(ip, port);

                if (newClientConnected)
                {
                    Console.WriteLine("[Program] 新版网络客户端连接成功");

                    // 2. 创建仓储实例（用于本地数据库操作）
                    // 注意：DbHelper 是静态类，方法直接调用，无需实例化
                    var messageRepo = new MessageRepository();  // 消息仓储（保存聊天记录）
                    var friendRepo = new FriendRepository();     // 好友仓储
                    var groupRepo = new GroupRepository();       // 群组仓储

                    // 3. 创建业务服务实例（封装业务逻辑，依赖网络客户端和仓储）
                    var userService = new UserBusinessService(newNetworkClient);
                    var friendService = new FriendBusinessService(newNetworkClient, messageRepo, friendRepo, groupRepo);
                    var messageService = new MessageBusinessService(newNetworkClient, messageRepo, friendRepo, groupRepo, null);
                    var groupService = new GroupBusinessService(newNetworkClient, messageRepo, friendRepo, groupRepo, null);

                    // 4. 注册服务到全局容器，供 UI 层通过 ServiceContainer.Resolve<T>() 获取
                    ServiceContainer.Register<IUserBusinessService>(userService);
                    ServiceContainer.Register<IFriendBusinessService>(friendService);
                    ServiceContainer.Register<IMessageBusinessService>(messageService);
                    ServiceContainer.Register<IGroupBusinessService>(groupService);
                    ServiceContainer.Register<INetworkClient>(newNetworkClient);

                    // 清除旧的用户缓存（新登录时确保干净状态）
                    CurrentUser.Clear();

                    Console.WriteLine("[Program] 新架构服务注册完成");
                }
                else
                {
                    Console.WriteLine("[Program] 新版网络客户端连接失败，新架构服务将不可用");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Program] 初始化新架构失败: {ex.Message}");
                // 新架构初始化失败不影响旧架构运行，继续使用旧版
            }

            // ========== 使用旧版客户端保持现有功能 ==========
            var oldClient = new NetworkClientLegacy();   // 旧版客户端（包含业务方法）
            bool con = oldClient.Connect(ip, port);      // 连接服务器

            if (con)
            {
                // 设置全局客户端（旧版，供现有 UI 通过 GlobalClient.Current 直接调用）
                GlobalClient.Current = oldClient;

                // 启动登录窗体，进入主界面
                Application.Run(new QQClient.UI.login_new());
            }
            else
            {
                MessageBox.Show("连接服务器失败");
            }
        }
    }
}