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
    public partial class chat : Form
    {
        private string self_account=GlobalClient.CurrentUserId;
        private string _friendAccount;  
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
            profile profile=new profile(self_account,_friendAccount);
           // MessageBox.Show("打开简介");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var client = GlobalClient.Current;

            send_message =textBox1.Text;
            if (send_message.Length > 0)
            {
               client.SendMessage(GlobalClient.CurrentUserId,_friendAccount,send_message);
            }
            else
            {
                MessageBox.Show("请输入文本");
            }
        }
    }
}
