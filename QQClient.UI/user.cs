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
        public user(string user_account, Form login)
        {
            InitializeComponent();
            self_account = user_account;
            Load_Friend();
            //            var testFriends = new List<QQCommon.Models.Message>
            ////测试案例
            //    {
            //        new QQCommon.Models.Message
            //        {
            //            Content = "I am zhangsan",
            //            SenderId = "张三",
            //            SendTime = DateTime.Now.AddHours(-1)
            //        },

            //    };

            foreach (var friend in testFriends)
            {
                // 创建 ContactItem 实例a
                var item = new ContactItem();

            //                // 设置显示名称：如果有备注就用备注，否则用 FriendUserId
            //                item.DisplayName =friend.SenderId.ToString();

            //                // 设置最后消息：这里用固定文本模拟，你可以用其他内容
            //                item.LastMessage = friend.Content.ToString();

            //                // 设置时间：用 AddTime 格式化为简短时间
            //                item.Time = friend.SendTime.ToString("HH:mm");

            //                // （可选）如果有头像，可以设置
            //                // item.Avatar = Properties.Resources.default_avatar;

            //                // 设置宽度适应 FlowLayoutPanel（考虑滚动条）
            //                item.Width = private_chat.ClientSize.Width - (private_chat.VerticalScroll.Visible ? 20 : 0);

            //                // 添加到 FlowLayoutPanel
            //                private_chat.Controls.Add(item);
            //            }

            this.FormClosed += (sender, e) => login.Show();
        }


        private void Load_Friend()
        {
            // 清空现有好友控件（防止重复加载）
            private_chat.Controls.Clear();

            // 获取当前登录用户ID（示例）
            //string currentUserId = Global.CurrentUser.UserId;  // 假设你有一个全局变量

            // 调用数据访问方法获取好友列表
            List<Friend> friends = GetFriendsByUserId(self_account);

            foreach (var friend in friends)
            {
                // 创建 ContactItem 实例
                var item = new ContactItem();

                // 设置显示名称：优先使用备注，否则使用好友昵称，再否则使用好友账号
                //string displayName = !string.IsNullOrEmpty(friend.Remark) ? friend.Remark
                //                   : (!string.IsNullOrEmpty(friend.FriendNickname) ? friend.FriendNickname
                //                   : friend.FriendUserId);
                string displayName = friend.FriendId.ToString();
                item.DisplayName = displayName;

                // 设置最后一条消息（可从 Messages 表查询最近的一条消息）
                // 这里暂时留空或设置默认文本，你可以单独写一个方法获取最后消息
                //item.LastMessage = GetLatestMessage(friend.FriendUserId, currentUserId);
                item.LastMessage = "!!!";
                // 设置时间（例如最后消息的时间或添加好友的时间）
                // 这里先用 AddTime 格式化
                item.Time = friend.AddTime.ToString("HH:mm");

                // 存储好友的唯一标识（FriendUserId）到 Tag 中，方便点击时识别
                item.Tag = friend.FriendUserId;

                // （可选）如果有头像，设置 item.Avatar = ...;

                // 设置宽度适应 FlowLayoutPanel
                item.Width = private_chat.ClientSize.Width - (private_chat.VerticalScroll.Visible ? SystemInformation.VerticalScrollBarWidth : 0);

                // 订阅点击事件（如果需要打开聊天窗口）
                // item.Click += ContactItem_Click;

                // 添加到 FlowLayoutPanel
                private_chat.Controls.Add(item);
            }
        }

        private void public_chat_Paint(object sender, PaintEventArgs e)
        {

        }
        //私聊模式
        private void button1_Click(object sender, EventArgs e)
        {
            public_chat.Visible = false;
            private_chat.Visible = true;
        }
        //群聊模式
        private void button2_Click(object sender, EventArgs e)
        {
            public_chat.Visible = true;
            private_chat.Visible = false;
        }
        //添加好友
        private void button3_Click(object sender, EventArgs e)
        {
            //this.Hide();
            //add_friend add = new add_friend(self_account);  
            ez_addfriend add = new ez_addfriend(self_account);
            add.ShowDialog();
        }

        // 加载私聊列表
        //    private void LoadPrivateChats()
        //    {
        //        private_chat.Controls.Clear();

        //        // 示例数据（实际应从数据源获取）
        //        var privateChats = new[]
        //        {
        //    new { Name = "张三", LastMsg = "你好", Time = "10:30", Unread = 3 },
        //    new { Name = "李四", LastMsg = "在吗？", Time = "昨天", Unread = 0 }
        //};

        //        foreach (var chat in privateChats)
        //        {
        //            // 创建 contactItem 实例
        //            var item = new ContactItem
        //            {
        //                DisplayName = chat.Name,
        //                LastMessage = chat.LastMsg,
        //                Time = chat.Time,
        //               // UnreadCount = chat.Unread,
        //                //Avatar = Properties.Resources.default_avatar // 头像资源
        //            };

        //            // 设置宽度适应 FlowLayoutPanel（考虑滚动条）
        //            item.Width = private_chat.ClientSize.Width - (private_chat.VerticalScroll.Visible ? 20 : 0);

        //            // 订阅点击事件（假设 contactItem 有 Clicked 事件）
        //            //item.Clicked += (s, e) =>
        //            //{
        //            //    var clickedItem = (contactItem)s;
        //            //    MessageBox.Show($"打开与 {clickedItem.DisplayName} 的聊天");
        //            //    // 这里打开聊天窗口
        //            //};

        //            // 添加到 FlowLayoutPanel
        //            private_chat.Controls.Add(item);
        //        }
        //    }

        // 加载群聊列表（类似）
        //private void LoadGroupChats()
        //{
        //    flowGroup.Controls.Clear();
        //    // 群聊数据...
        //    foreach (var chat in groupChats)
        //    {
        //        var item = new contactItem
        //        {
        //            DisplayName = chat.Name,
        //            LastMessage = chat.LastMsg,
        //            Time = chat.Time,
        //            UnreadCount = chat.Unread,
        //            Avatar = Properties.Resources.group_avatar
        //        };
        //        item.Width = flowGroup.ClientSize.Width - (flowGroup.VerticalScroll.Visible ? 20 : 0);
        //        item.Clicked += (s, e) => MessageBox.Show($"打开群聊 {((contactItem)s).DisplayName}");
        //        flowGroup.Controls.Add(item);
        //    }
        //}
    }
}
