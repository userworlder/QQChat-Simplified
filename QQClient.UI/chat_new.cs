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
    public partial class chat_new : Form
    {
        //供外部窗体使用的账号，用于识别唯一窗口
        public string FriendAccount => _friendAccount;

        private string _friendAccount;
        private string _friendNickname;

        public chat_new(string friendAccount, string friendNickname)
        {
            InitializeComponent();
            _friendAccount = friendAccount;
            _friendNickname = friendNickname;
            lblFriendName.Text = friendNickname;
            this.Text = $"与 {friendNickname} 聊天中";
            // 订阅事件       
            this.Load += chat_new_Load;  // 注意方法名不要写错，且没有括号
            Show_OfflineMessages();
          
        }
        private void Show_OfflineMessages()
        {
            if (!GlobalClient.MessageCache.ContainsKey(_friendAccount))
                return;

            var messages = GlobalClient.MessageCache[_friendAccount]
                .OrderBy(m => m.SendTime) // 按时间排序
                .ToList();

            foreach (var msg in messages)
            {
                if (msg.SenderId == _friendAccount) // 对方发送的消息
                {
                    AddReceivedMessage(msg.Content); // 使用对方气泡样式
                }
                else if (msg.SenderId == GlobalClient.CurrentUserId) // 自己发送的消息
                {
                    AddSentMessage(msg.Content); // 使用自己气泡样式
                }
            }
            MarkMessagesAsRead();

        }
        private void MarkMessagesAsRead()
        {
            if (GlobalClient.MessageCache.ContainsKey(_friendAccount))
            {
                foreach (var msg in GlobalClient.MessageCache[_friendAccount])
                {
                    msg.IsRead = true;
                }
            }
        }
        private void chat_new_Load(object sender, EventArgs e)
        {
            flowMessages.Controls.Clear();

            AddReceivedMessage("这是一条对方短消息");
            AddSentMessage("这是我的短消息");
            AddReceivedMessage("这是一条对方非常非常非常非常非常非常非常非常非常非常非常长非常非常非常非常非常非常非常非常非常非常非常非常长的信息消息的。");
            AddSentMessage("这是一条非常非常非常非常非常非常非常非常非常非常非常长非常非常非常非常非常非常非常非常非常非常非常非常长我的信息。");

            // 调试：输出每个控件的高度
            //foreach (Control ctrl in flowMessages.Controls)
            //{
            //    if (ctrl is message_bubble msg)
            //    {
            //        MessageBox.Show($"消息: {msg.MessageText}\n控件高度: {msg.Height}\nPanelBubble高度: {msg.Controls[0].Height}");
            //    }
            //}
        }


        private void AddReceivedMessage(string text)
        {
            var msg = new message_bubble
            {
                MessageText = text,
                IsSelf = false,
                Width = flowMessages.ClientSize.Width
            };
            flowMessages.Controls.Add(msg);
            flowMessages.ScrollControlIntoView(msg);
            AdjustMessageWidths();
            // System.Diagnostics.Debug.WriteLine($"添加消息: {text}, 宽度: {msg.Width}");
        }

        private void AddSentMessage(string text)
        {
            var msg = new message_bubble
            {
                MessageText = text,
                IsSelf = true,
                Width = flowMessages.ClientSize.Width
            };
            flowMessages.Controls.Add(msg);
            flowMessages.ScrollControlIntoView(msg);
            AdjustMessageWidths();
            //System.Diagnostics.Debug.WriteLine($"添加消息: {text}, 宽度: {msg.Width}");
        }

        private void AdjustMessageWidths()
        {
            int newWidth = flowMessages.ClientSize.Width;
            if (flowMessages.VerticalScroll.Visible)
                newWidth -= SystemInformation.VerticalScrollBarWidth;

            foreach (Control ctrl in flowMessages.Controls)
            {
                if (ctrl is message_bubble msg)
                {
                    ctrl.Width = newWidth;
                }
            }
        }
        //打开简介
        private void lblFriendName_Click(object sender, EventArgs e)
        {
            profile profile = new profile(GlobalClient.CurrentUserId, _friendAccount);
        }
        //发送消息
        private void btnSend_Click_1(object sender, EventArgs e)
        {
            var client = GlobalClient.Current;
            //MessageBox.Show("发送消息");
            string text = txtInput.Text;
            //检测空白消息
            if (string.IsNullOrWhiteSpace(text) || text == "")
            {
                lbl_warn.Visible = true;
                lbl_warn.Text = "请输入文本";
            }
            else
            {
                bool x = client.SendMessage(GlobalClient.CurrentUserId, _friendAccount, text);
                if (x)
                {
                    AddSentMessage(text);
                    txtInput.Clear(); // 清空输入框
                    lbl_warn.Visible = false; // 清除警告
                }
                else
                {
                    MessageBox.Show("发送失败，请稍后重试给");
                }
            }

        }
        //清空消息
        private void btnClear_Click_1(object sender, EventArgs e)
        {
            txtInput.Text = "";
        }

        private void btn_test_Click(object sender, EventArgs e)
        {
            string text = $"这是{GlobalClient.CurrentUserId}发给{_friendAccount}的快捷消息";
            txtInput.Text = text;
            AddSentMessage(text);
        }
    }
}
