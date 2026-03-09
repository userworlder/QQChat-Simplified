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
    public partial class register_new : Form
    {
        public register_new(Form login)
        {
            InitializeComponent();
            this.FormClosed += (sender, e) => login.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            QQClient.Communication.NetworkClient client = new QQClient.Communication.NetworkClient();
            string ip = "127.0.0.1";
            int port = 8888;
            bool con = client.Connect(ip, port);
            if (con) 
            {
                string nickname = textBox1.Text;
                string account = textBox2.Text;
                string password = textBox3.Text;
                if (account != "" && nickname == "")
                {
                    nickname = account;
                }
                if (password == "" || account == "")
                {
                    label_warn.Text = "账号或密码不可为空";
                    label_warn.Visible = true;
                }
                else
                {
                    client.Register( account, password,nickname);
                    this.Close();
                    
                }

            }
            else
            {
                MessageBox.Show("无法连接到服务器，请重试");
            }
           
        }

        
    }
}
