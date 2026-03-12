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

            string nickname = textBox1.Text;
            string account = textBox2.Text;
            string password = textBox3.Text;
            if (account != "" && nickname == "")
            {
                textBox1.Text = account;
                nickname = account;
            }
            else if (password == "" || account == "")
            {
                label_warn.Text = "账号或密码不可为空";
                label_warn.Visible = true;
            }
            else if (client.SearchId(account, account))
            {
                label_warn.Text = "已存在该账号,请尝试新的账号";
                label_warn.Visible = true;
            }
            else
            {
                bool x = client.Register(account, password, nickname);
                if (x)
                {
                    MessageBox.Show("注册成功");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("注册失败");
                    label_warn.Text = "请尝试其他账号密码";
                    label_warn.Visible = true;
                }

            }

        }





    }
}
