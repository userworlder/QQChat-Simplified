using QQCommon.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QQClient.UI
{
    public partial class profile : Form
    {
        string origin_account;
        string origin_nickname;
        string origin_signature;
        string origin_password;
        private User User;
        private User new_User=new User();
        public profile(string self_account, string friend_account)
        {
            //所有文本框初始均为只读状态
            InitializeComponent();
            ReadOnlyMode();
            var client = GlobalClient.Current;
            User = client.GetUserInfo(friend_account);
            origin_account= self_account;
            //打开自己的界面
            if (self_account == friend_account)
            {
                Load(User);
            }
            //打开他人的界面
            else
            {   //无法修改字段，隐藏修改按钮和密码
                Load(User);
                lbl_update.Visible = false;
                label4.Visible = false;
                textBox4.Visible = false;
            }

        }
        //读取个人资料并获取
        void Load(User user)
        {
            textBox1.Text = user.Username;
            textBox2.Text = user.Nickname;
            textBox3.Text = user.Signature;
            textBox4.Text = user.Password;
        }
        //只读模式
        void ReadOnlyMode()
        {
            textBox1.ReadOnly = true;
            textBox2.ReadOnly = true;
            textBox3.ReadOnly = true;
            textBox4.ReadOnly = true;
            btn_accept.Visible = false;
            btn_cancel.Visible = false;
        }
        //编辑模式
        void EditMode()
        {
            btn_accept.Visible = true;
            btn_cancel.Visible = true;
            textBox2.ReadOnly = false;
            textBox3.ReadOnly = false;
            textBox4.ReadOnly = false;
        }
        private void btn_accept_Click(object sender, EventArgs e)
        {
            string new_nickname = textBox2.Text;
            string new_signature = textBox3.Text;
            string new_password = textBox4.Text;
            new_User.Username = GlobalClient.CurrentUserId;
            new_User.Password = new_password;
            new_User.Nickname = new_nickname;
            new_User.Signature = new_signature;
            var client = GlobalClient.Current;        
            bool x = client.UpdateUserInfo(new_User);
            if (client == null)
            {
                MessageBox.Show("网络客户端未初始化");
                return;
            }
            else
            {
                if (x)
                {
                    MessageBox.Show("修改成功");
                    ReadOnlyMode();
                }
                else
                {
                    MessageBox.Show("修改失败");
                }
            }

        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            //恢复数据
            textBox2.Text = origin_nickname;
            textBox3.Text = origin_signature;
            textBox4.Text = origin_password;
            // 恢复只读状态
            ReadOnlyMode();
            
        }

        private void lbl_update_Click(object sender, EventArgs e)
        {
            //允许修改除账号外的所有数据
            EditMode();
            //记忆原有的资料
            origin_account = textBox1.Text;
            origin_nickname = textBox2.Text;
            origin_signature = textBox3.Text;
            origin_password = textBox4.Text;
        }
    }
}
