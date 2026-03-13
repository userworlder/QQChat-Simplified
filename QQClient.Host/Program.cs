using QQClient.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using QQCommon.Models;
namespace QQClient.Host
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        /// 全局静态，所有窗体均可访问
        

        [STAThread]

        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string ip = "127.0.0.1";
            int port = 8888;
            // 创建唯一的 NetworkClient 实例
            var client = new NetworkClient();
            bool con = client.Connect(ip, port);
            if (con)
            {
                // 将 client 传给 login 窗体
                GlobalClient.Current = client;
                Application.Run(new QQClient.UI.login());
            }
            else
            {
                MessageBox.Show("连接服务器失败");
            }
            //Application.Run(new QQClient.UI.login());
            // Application.Run(new QQClient.UI.user());
        }
    }
}
