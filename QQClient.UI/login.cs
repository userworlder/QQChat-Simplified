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

namespace QQClient.UI
{
    public partial class login : Form
    {
        public login()
        {
            InitializeComponent();

            pictureBox1.Image = ImageHelper.Load("login.png");
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            
            register register=new register();
            register.ShowDialog();
        }

     

        private void button1_Click(object sender, EventArgs e)
        {
            QQClient.Communication.NetworkClient client = new QQClient.Communication.NetworkClient();
            string ip = "127.0.0.1";
            int port = 2005;
            bool con = client.Connect(ip, port);
            if (con) {
                //是否为空
                if (textBox1.Text != "" && textBox2.Text != "")
                {
                    string username = textBox1.Text;
                    string password = textBox2.Text;
                    //检验是否是这个人

                    bool x = client.Login(username, password);
                    if (x)
                    {   //打开界面
                        this.Hide();
                        user user = new user();
                        user.ShowDialog();
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
            else
            {
                MessageBox.Show("无法连接到服务器，请重试");
            }           
        }  
    }
}
