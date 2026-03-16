using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QQCommon.Interfaces;
using QQCommon.Models;
using QQCommon.Protocols;
namespace QQClient.UI
{
    public partial class user : Form
    {
        string self_account;
        string fromUserId;
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
            this.Text = user_account;
            GlobalClient.Current.MessageReceived += OnMessageReceived;
            Load_Panel();
            Load_Friend();
            LoadPendingRequests();
            this.FormClosed += (sender, e) => login.Show();
        }
        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            // 根据包的类型判断是否为好友请求
            if (e.Packet.Type == MessageType.AddFriendRequest) // 假设有这样一个类型
            {
                string fromUserId = e.Packet.Sender; // 发起者账号
                //MessageBox.Show(fromUserId);
                // 切换到 UI 线程添加请求项
                this.Invoke((MethodInvoker)delegate
                {
                    AddFriendRequest(fromUserId);
                });
            }
            // 可能还需要处理其他消息类型，比如新消息等
        }

        private void AddFriendRequest(string fromUserId)
        {
            //MessageBox.Show(fromUserId);
            // 检查是否已经存在相同请求
            foreach (Control ctrl in request.Controls)
            {
                if (ctrl is FriendItem exist_item && exist_item.FromUserId == fromUserId)
                    return;
            }
            //MessageBox.Show(fromUserId);
            var item = new FriendItem(fromUserId);
            item.AcceptClicked += OnAcceptRequest;
            item.RejectClicked += OnRejectRequest;
            request.Controls.Add(item);
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
            List<Friend> friends = client.SearchAllFriends(GlobalClient.CurrentUserId);

            foreach (var friend in friends)
            {
                // 创建 ContactItem 实例
                var item = new ContactItem();
                string displayName = friend.FriendNickName.ToString();
                item.DisplayName = displayName;
                string account= friend.FriendUserName.ToString();
                item.Account = account;
                //    // 设置最后一条消息（可从 Messages 表查询最近的一条消息）
                //    // 这里暂时留空或设置默认文本，你可以单独写一个方法获取最后消息
                //  item.LastMessage = GetLatestMessage(friend.FriendUserId, currentUserId);
                item.LastMessage = "!!!";
                //    // 设置时间（例如最后消息的时间或添加好友的时间）
                //    // 这里先用 AddTime 格式化
                //    item.Time = friend.AddTime.ToString("HH:mm");
                // 设置宽度适应 FlowLayoutPanel
                item.Width = private_chat.ClientSize.Width - (private_chat.VerticalScroll.Visible ? SystemInformation.VerticalScrollBarWidth : 0);
                private_chat.Controls.Add(item);
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
            MessageBox.Show($"CurrentUserId: {GlobalClient.CurrentUserId}, fromUserId: {fromUserId}");
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

