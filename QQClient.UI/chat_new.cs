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
using Msg = QQCommon.Models.Message;
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
            //this.Load += chat_new_Load;  // 注意方法名不要写错，且没有括号
            Show_OfflineMessages();
          
        }
        private void Show_OfflineMessages()
        {
            //如果消息缓存中没有对应键（没有好友）则无事发生
            if (!GlobalClient.MessageCache.ContainsKey(_friendAccount))
                return;
            //将未收到信息按时间排序
            var messages = GlobalClient.MessageCache[_friendAccount]
                .OrderBy(m => m.SendTime) // 按时间排序
                .ToList();
            //按序“打印”消息
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
        //测试用消息
        private void chat_new_Load(object sender, EventArgs e)
        {
            flowMessages.Controls.Clear();
            AddReceivedMessage("这是一条对方短消息");
            AddSentMessage("这是我的短消息");
            AddReceivedMessage("这是一条对方非常非常非常非常非常非常非常非常非常非常非常长非常非常非常非常非常非常非常非常非常非常非常非常长的信息消息的。");
            AddSentMessage("这是一条非常非常非常非常非常非常非常非常非常非常非常长非常非常非常非常非常非常非常非常非常非常非常非常长我的信息。");        
        }


        public void AddReceivedMessage(string text)
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

        public void AddSentMessage(string text)
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
            string text = txtInput.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                lbl_warn.Visible = true;
                lbl_warn.Text = "请输入文本";
                return;
            }

            bool x = client.SendMessage(GlobalClient.CurrentUserId, _friendAccount, text);
            if (x)
            {
                // 将发送的消息存入缓存
                var msg = new Msg
                {
                    MessageId = Guid.NewGuid().ToString(), // 如果没有实际ID，可以生成临时ID
                    SenderId = GlobalClient.CurrentUserId,
                    ReceiverId = _friendAccount,
                    Content = text,
                    SendTime = DateTime.Now,
                    IsRead = true,       // 自己发送的消息默认已读
                    MessageType = 1
                };

                if (!GlobalClient.MessageCache.ContainsKey(_friendAccount))
                    GlobalClient.MessageCache[_friendAccount] = new List<Msg>();
                GlobalClient.MessageCache[_friendAccount].Add(msg);

                AddSentMessage(text);
                txtInput.Clear();
                lbl_warn.Visible = false;
            }
            else
            {
                MessageBox.Show("发送失败，请稍后重试");
            }

        }
        //清空消息
        private void btnClear_Click_1(object sender, EventArgs e)
        {
            txtInput.Text = "";
        }
        //快捷测试
        private void btn_test_Click(object sender, EventArgs e)
        {
            string text = $"这是{GlobalClient.CurrentUserId}发给{_friendAccount}的快捷消息";
            txtInput.Text = text;
            //AddSentMessage(text);
        }
    }
}
