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
            string ip = "192.168.24.81";
            int port = 8888;

            //创建唯一的 NetworkClient 实例
            var client = new NetworkClient();
            bool con = client.Connect(ip, port);
            if (con)
            {
                //
                GlobalClient.Current = client;

                Application.Run(new QQClient.UI.login_new());
            }
            else
            {
                MessageBox.Show("连接服务器失败");
            }



        }
    }
}
