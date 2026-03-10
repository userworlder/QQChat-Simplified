using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QQClient.UI
{
    public partial class ez_addfriend : Form
    {
        string self_account;
        string friend_account;
        public ez_addfriend(string self_account)
        {
            InitializeComponent();
            this.self_account = self_account;
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            friend_account=textBox1.Text;
            QQClient.Communication.NetworkClient client = new QQClient.Communication.NetworkClient();
            string ip = "127.0.0.1";
            int port = 8888;
            bool con = client.Connect(ip, port);
            if (con)
            {
                bool x=client.AddFriend(self_account, friend_account);
                if (x)
                {

                }
                else
                {

                }
            }
        }
    }
}
