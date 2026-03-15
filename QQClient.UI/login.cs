using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QQCommon.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
namespace QQClient.UI
{
    public partial class login : Form
    {
        public login()
        {
            InitializeComponent();

            //  pictureBox1.Image = ImageHelper.Load("login.png");
            // pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.Hide();
            register_new register = new register_new(this);
            register.ShowDialog();
        }



        //private void button1_Click(object sender, EventArgs e)
        //{

        //}

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            //QQClient.Communication.NetworkClient client = new QQClient.Communication.NetworkClient();
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

        private void button1_Click(object sender, EventArgs e)
        {
            user user = new user("1", this);
            user.Show();
        }
    }
}
