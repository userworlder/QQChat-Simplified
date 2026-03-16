using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QQCommon.Models;
namespace QQClient.UI
{
    public partial class user : Form
    {
        string self_account;
        //panel的坐标
        int panel_x;
        int panel_y;
        public user()
        {
            InitializeComponent();
            panel_x = panel1.Left;
            panel_y = panel1.Top;
            Load_Panel();
        }
        public user(string user_account, Form login)
        {
            InitializeComponent();
            self_account = user_account;
            panel_x = panel1.Left;
            panel_y = panel1.Top;
            Load_Panel();
            //MessageBox.Show(GlobalClient.CurrentUserId);
            Load_Friend();
            LoadPendingRequests();
            /*            var testFriends = new List<QQCommon.Models.Message>
            ////测试案例
            //    {
            //        new QQCommon.Models.Message
            //        {
            //            Content = "I am zhangsan",
            //            SenderId = "张三",
            //            SendTime = DateTime.Now.AddHours(-1)
            //        },

            //    };

            //foreach (var friend in testFriends)
            //{
            //    // 创建 ContactItem 实例a
            //    var item = new ContactItem();

            //    // 设置显示名称：如果有备注就用备注，否则用 FriendUserId
            //    item.DisplayName =friend.SenderId.ToString();

            //    // 设置最后消息：这里用固定文本模拟，你可以用其他内容
            //    item.LastMessage = friend.Content.ToString();

            //    // 设置时间：用 AddTime 格式化为简短时间
            //    item.Time = friend.SendTime.ToString("HH:mm");

            //    // （可选）如果有头像，可以设置
            //    // item.Avatar = Properties.Resources.default_avatar;

            //    // 设置宽度适应 FlowLayoutPanel（考虑滚动条）
            //    item.Width = private_chat.ClientSize.Width - (private_chat.VerticalScroll.Visible ? 20 : 0);

            //    // 添加到 FlowLayoutPanel
            //    private_chat.Controls.Add(item);
            //}
            */
            this.FormClosed += (sender, e) => login.Show();
        }
        //加载界面的位置
        void Load_Panel()
        {
            private_chat.Left = 0;
            private_chat.Top = 0;
            public_chat.Left = 0;
            public_chat.Top = 0;
            request.Left = 0;
            request.Top = 0;
        }
        //加载好友列表
        void Load_Friend()
        {
            //获取Friend
            var client = GlobalClient.Current;
            //List<Friend> friends=new List<Friend>();
            MessageBox.Show(GlobalClient.CurrentUserId);
            List<Friend> friends = client.SearchAllFriends(GlobalClient.CurrentUserId);
            foreach (var friend in friends)
            {
                // 创建 ContactItem 实例
                var item = new ContactItem();            
                //string displayName = friend.FriendNickName.ToString();
                //item.DisplayName = displayName;
                //string account= friend.FriendUserName.ToString();
                //item.Account = account;
                //    // 设置最后一条消息（可从 Messages 表查询最近的一条消息）
                //    // 这里暂时留空或设置默认文本，你可以单独写一个方法获取最后消息
                  //  item.LastMessage = GetLatestMessage(friend.FriendUserId, currentUserId);
                    item.LastMessage = "!!!";
                //    // 设置时间（例如最后消息的时间或添加好友的时间）
                //    // 这里先用 AddTime 格式化
                //    item.Time = friend.AddTime.ToString("HH:mm");

                // 存储好友的唯一标识（FriendUserId）到 Tag 中，方便点击时识别
                //item.Tag = friend.FriendUserId;


                // 设置宽度适应 FlowLayoutPanel
                item.Width = private_chat.ClientSize.Width - (private_chat.VerticalScroll.Visible ? SystemInformation.VerticalScrollBarWidth : 0);

            }


        }
        private void LoadPendingRequests()
        {
            var client = GlobalClient.Current;
            if (client == null) return;

            // 调用方法，获取好友请求列表
            var offlineMessages = client.GetOfflineMessages(out List<string> requestUsers);

            // 如果请求列表不为空，则填充界面
            if (requestUsers != null && requestUsers.Count > 0)
            {
                request.Controls.Clear();
                foreach (var fromUserId in requestUsers)
                {
                    var item = new FriendItem(fromUserId);
                    //AcceptClicked的对应事件
                    item.AcceptClicked += OnAcceptRequest;
                    item.RejectClicked += OnRejectRequest;
                    request.Controls.Add(item);
                }
            }
            else
            {
                // 没有请求时显示提示（可选）
                Label lblEmpty = new Label { Text = "暂无好友请求", AutoSize = true };
                request.Controls.Add(lblEmpty);
            }
        }


        private void OnAcceptRequest(object sender, string fromUserId)
        {
            var client = GlobalClient.Current;
            bool success = client.AcceptFriendRequest(fromUserId);
            if (success)
            {
                // 从 FlowLayoutPanel 中移除该项
                request.Controls.Remove((UserControl)sender);
                MessageBox.Show($"已同意 {fromUserId} 的好友请求");
            }
            else
            {
                MessageBox.Show("同意失败，请稍后重试");
            }
        }

        private void OnRejectRequest(object sender, string fromUserId)
        {
            var client = GlobalClient.Current;
            bool success = client.RejectFriendRequest(fromUserId);
            if (success)
            {
                request.Controls.Remove((UserControl)sender);
                MessageBox.Show($"已拒绝 {fromUserId} 的好友请求");
            }
            else
            {
                MessageBox.Show("拒绝失败，请稍后重试");
            }
        }
      
        //私聊模式
        private void btn_privatemode(object sender, EventArgs e)
        {
            public_chat.Visible = false;
            private_chat.Visible = true;
            request.Visible = false;
        }
        //群聊模式
        private void btn_publicmode(object sender, EventArgs e)
        {
            public_chat.Visible = true;
            private_chat.Visible = false;
            request.Visible = false;
        }
        //验证消息
        private void btn_requestmode(object sender, EventArgs e)
        {
            public_chat.Visible = false;
            private_chat.Visible = false;
            request.Visible = true;
        }
        //添加好友
        private void btn_addfriend(object sender, EventArgs e)
        {
            ez_addfriend add = new ez_addfriend(self_account);
            add.ShowDialog();
        }



  
    }
}

