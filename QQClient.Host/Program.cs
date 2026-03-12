using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string ip = "127.0.0.1";
            int port = 8888;
            QQClient.Communication.NetworkClient client = new QQClient.Communication.NetworkClient();
            bool con = client.Connect(ip, port);
            if (con)
            {
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
