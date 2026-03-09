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
    public partial class user : Form
    {
        public user(Form login)
        {
            InitializeComponent();
            var testFriends = new List<QQCommon.Models.Message>
    {
        new QQCommon.Models.Message
        {
            Content = "I am zhangsan",
            SenderId = "张三",
            SendTime = DateTime.Now.AddHours(-1)
        },
        
    };

            foreach (var friend in testFriends)
            {
                // 创建 ContactItem 实例
                var item = new ContactItem();

                // 设置显示名称：如果有备注就用备注，否则用 FriendUserId
                item.DisplayName =friend.SenderId.ToString();

                // 设置最后消息：这里用固定文本模拟，你可以用其他内容
                item.LastMessage = friend.Content.ToString();

                // 设置时间：用 AddTime 格式化为简短时间
                item.Time = friend.SendTime.ToString("HH:mm");

                // （可选）如果有头像，可以设置
                // item.Avatar = Properties.Resources.default_avatar;

                // 设置宽度适应 FlowLayoutPanel（考虑滚动条）
                item.Width = private_chat.ClientSize.Width - (private_chat.VerticalScroll.Visible ? 20 : 0);

                // 添加到 FlowLayoutPanel
                private_chat.Controls.Add(item);
            }

            this.FormClosed += (sender, e) => login.Show();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

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
