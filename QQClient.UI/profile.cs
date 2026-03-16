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
        public profile(string self_account, string friend_account)
        {
            //所有文本框初始均为只读状态
            InitializeComponent();

            //打开自己的界面
            if (self_account == friend_account)
            {
                //Load();
            }
            //打开他人的界面
            else
            {   //无法修改字段，隐藏修改按钮和密码
                //Load();
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
        void Load(string id)
        {
            //textBox1.Text =getAccount(id);
            //textBox2.Text =getNickname(id);
            //textBox3.Text =getSinature(id);
            //textBox4.Text =getPassword(id);
        }
        private void btn_accept_Click(object sender, EventArgs e)
        {
            string new_nickname = textBox2.Text;
            string new_signature = textBox3.Text;
            string new_password = textBox4.Text;
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            //恢复数据
            textBox2.Text = origin_nickname;
            textBox3.Text = origin_signature;
            textBox4.Text = origin_password;
            // 恢复只读状态
            textBox2.ReadOnly = true;
            textBox3.ReadOnly = true;
            textBox4.ReadOnly = true;
            //隐藏按钮
            btn_accept.Visible = false;
            btn_cancel.Visible = false;
        }

        private void lbl_update_Click(object sender, EventArgs e)
        {
            //允许修改除账号外的所有数据
            textBox2.ReadOnly = false;
            textBox3.ReadOnly = false;
            textBox4.ReadOnly = false;
            //显示按钮
            btn_accept.Visible = true;
            btn_cancel.Visible = true;
            //记忆原有的资料
            origin_account = textBox1.Text;
            origin_nickname = textBox2.Text;
            origin_signature = textBox3.Text;
            origin_password = textBox4.Text;
        }
    }
}
