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
    public partial class chat_private : Form
    {
        
        private string _friendAccount;
        private string _friendNickname;

        public chat_private(string friendAccount, string friendNickname,user parentForm)
        {
            InitializeComponent();
            _friendAccount = friendAccount;
            _friendNickname = friendNickname;
            lblFriendName.Text = friendNickname;
            this.Text = $"与 {friendNickname} 聊天中";        
            LoadHistoryMessages();
            this.FormClosed += (s, e) => parentForm.RefreshFriendList();
        }
        private async void LoadHistoryMessages()
        {
            var client = GlobalClient.Current;
            if (client == null) return;

            // 异步获取历史消息
            var messages = await Task.Run(() => client.GetHistoryMessages(_friendAccount));

            // 清空现有消息（如果有测试消息）
            flowMessages.Controls.Clear();

            // 按时间顺序显示
            foreach (var msg in messages.OrderBy(m => m.SendTime))
            {
                if (msg.SenderId == _friendAccount) // 对方发送
                {
                    AddReceivedMessage(msg.Content);
                }
                else // 自己发送
                {
                    AddSentMessage(msg.Content);
                }
            }

            // 找出所有未读消息（接收者是当前用户，且 IsRead == false）
            var unreadMessages = messages.Where(m => !m.IsRead && m.ReceiverId == GlobalClient.CurrentUserId).ToList();
            if (unreadMessages.Any())
            {
                // 调用服务器标记已读
                bool success = await Task.Run(() => client.MarkMessagesAsRead(_friendAccount));
                if (success)
                {
                    // 可选：更新本地缓存中的 IsRead 状态
                    if (GlobalClient.MessageCache.ContainsKey(_friendAccount))
                    {
                        foreach (var msg in GlobalClient.MessageCache[_friendAccount])
                            msg.IsRead = true;
                    }
                }
                else
                {
                    Console.WriteLine("标记已读失败");
                }
            }
        }       
        //添加对方的消息
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
        //添加自己的消息
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
        }
        //聊天气泡自适应宽度
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
            profile.Show();
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
