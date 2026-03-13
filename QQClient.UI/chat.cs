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
    public partial class chat : Form
    {
        private string _friendAccount;  // 改为 account
        private string _friendNickname;
        string send_message;
        public chat(string Account,string Name)
        {
            InitializeComponent();
            this._friendAccount = Account;
            this._friendNickname = Name;
            this.Text = $"与 {_friendNickname} 聊天中";  // 窗口标题显示昵称
            label1.Text = _friendNickname;
                                                    
        }
        //打开主页
        private void label1_Click(object sender, EventArgs e)
        {
            //profile profile=new profile();
            MessageBox.Show("打开简介");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            QQClient.Communication.NetworkClient client = new QQClient.Communication.NetworkClient();

            send_message =textBox1.Text;
            if (textBox1.Text.Length > 0)
            {
               // client.SendMessage();
            }
            else
            {

            }
        }
    }
}
