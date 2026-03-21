using QQCommon.Models;
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
    public partial class login_new : Form
    {
        public login_new()
        {
            InitializeComponent();
        }

        //private void label3_Click(object sender, EventArgs e)
        //{
            
        //}
        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "1";
            textBox2.Text = "1";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "wxb";
            textBox2.Text = "wxb";
        }
        //登录键
        private void pictureBox2_Click(object sender, EventArgs e)
        {       
            //是否为空
            var client = GlobalClient.Current;
            if (client == null)
            {
                MessageBox.Show("网络客户端未初始化");
                return;
            }
            if (textBox1.Text != "" && textBox2.Text != "")
            {
                string username = textBox1.Text;
                string password = textBox2.Text;
                //检验是否是这个人

                bool x = client.Login(username, password);
                if (x)
                {   //打开界面
                    this.Hide();
                    GlobalClient.CurrentUserId = username;
                    user user = new user(username, this);
                    user.Show();
                }
                else
                {   //返回错误信息
                    MessageBox.Show("不存在该用户，请检查账号或密码");
                }
            }
            else
            {   //返回错误信息
                MessageBox.Show("账户或密码不可为空");
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.Hide();
            register_new register = new register_new(this);
            register.ShowDialog();
        }
    }
}
