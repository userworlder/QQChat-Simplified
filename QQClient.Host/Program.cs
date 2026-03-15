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
            DialogResult result = MessageBox.Show(
               "是否进入调试模式？\n\n是：跳过连接，直接打开测试窗口\n否：正常连接服务器并登录",
               "启动选项",
               MessageBoxButtons.YesNo,
               MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                //Application.Run(new QQClient.UI.chat_new("1", "1"));
               Application.Run(new QQClient.UI.user());
            }
            else
            {
                //创建唯一的 NetworkClient 实例
                var client = new NetworkClient();
                bool con = client.Connect(ip, port);
                if (con)
                {
                    //
                    GlobalClient.Current = client;
                    Application.Run(new QQClient.UI.login());
                }
                else
                {
                    MessageBox.Show("连接服务器失败");
                }
            }


        }
    }
}
