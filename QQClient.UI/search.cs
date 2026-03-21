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
    public partial class search : Form
    {
        user _parentForm;
        string _self_account;
        string friend_account;
        public search(string self_account,user parentform)
        {
            InitializeComponent();
            Load_Panel();
            _self_account= self_account;
            _parentForm = parentform;

        }
       
        private async void btn_addgroup_Click(object sender, EventArgs e)
        {
            string group_account = txt_group.Text;
            var client = GlobalClient.Current;
            var groups= await Task.Run(() => client.SearchGroups(group_account));
            if (groups != null)
            {
                foreach (var group in groups)
                {
                    bool success = await Task.Run(() => client.JoinGroup(group.GroupId));
                    if (success)
                    {
                        
                        MessageBox.Show("申请已发送");
                        _parentForm?.RefreshGroupList();
                    }
                    else
                    {
                        MessageBox.Show("申请发送失败");
                    }             
                } 
            }
            else
            {
                lbl_groupwarn.Visible=true;
                lbl_groupwarn.Text = "未找到该群组";
            }  
        }

        private void btn_addfriend_Click(object sender, EventArgs e)
        {
            friend_account = txt_friend.Text;
            var client = GlobalClient.Current;
            if (txt_friend.Text != "")
            {
                friend_account = txt_friend.Text;
                //检验是否是这个人
                bool check = client.SearchId(_self_account, friend_account);
                if (check)
                {
                    bool x = client.AddFriend(_self_account, friend_account);
                    if (x)
                    {   //打开界面
                        MessageBox.Show("已发送好友申请");
                        lbl_friendwarn.Visible=false;
                    }
                    else
                    {   //返回错误信息
                        MessageBox.Show("发送好友申请失败");
                    }
                }
                else
                {
                    lbl_friendwarn.Text = "不存在该用户";
                    lbl_friendwarn.Visible = true;
                }
            }
            else
            {   //返回错误信息
                MessageBox.Show("账号不可为空");
            }

        }
        //载入
        void Load_Panel()
        {
            pnl_addfriend.Top = 0;
            pnl_addfriend.Left = 0;
            pnl_addgroup.Top = 0;
            pnl_addgroup.Left = 0;
            pnl_addgroup.Visible = false;
        }
        //加人模式
        private void btn_friendmode_Click(object sender, EventArgs e)
        {
            pnl_addfriend.Visible = true;
            pnl_addgroup.Visible = false;
        }
        //加群模式
        private void btn_groupmode_Click(object sender, EventArgs e)
        {
            pnl_addgroup.Visible = true;
            pnl_addfriend.Visible = false;
        }

    }
}
