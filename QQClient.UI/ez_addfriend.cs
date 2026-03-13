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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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
            var client = GlobalClient.Current;
            if (textBox1.Text != "")
            {
                friend_account = textBox1.Text;
                //检验是否是这个人
                bool check = client.SearchId(self_account, friend_account);
                if (check)
                {
                    bool x = client.AddFriend(self_account, friend_account);
                    if (x)
                    {   //打开界面
                        MessageBox.Show("已添加好友");
                    }
                    else
                    {   //返回错误信息
                        MessageBox.Show("添加失败，请检查账号");
                    }
                }
                else
                {
                    label2.Text = "不存在该用户";
                    label2.Visible = true;
                }
            }
            else
            {   //返回错误信息
                MessageBox.Show("账号不可为空");
            }

        }
    }
}
