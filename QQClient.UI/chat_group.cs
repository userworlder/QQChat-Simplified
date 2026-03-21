using QQClient.UI.user_control;
using QQCommon.Interfaces;
using QQCommon.Models;
using QQCommon.Protocols;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QQClient.UI
{
    public partial class chat_group : Form
    {
        private string _groupId;
        private string _groupName;
        private Dictionary<string, group_bubble> _messageControls = new Dictionary<string, group_bubble>();
        private user _parentForm;

        public chat_group(string groupId, string groupName, user parentForm)
        {
            InitializeComponent();
            _groupId = groupId;
            _groupName = groupName;
            _parentForm = parentForm;
            lblGroupName.Text = groupName;
            this.Text = $"{groupName} 群聊";

            LoadHistoryMessages();
            GlobalClient.Current.MessageReceived += OnMessageReceived;
            this.FormClosed += (s, e) =>
            {
                GlobalClient.Current.MessageReceived -= OnMessageReceived;
                _parentForm?.RefreshGroupList();
            };
            this.Resize += (s, e) => AdjustBubbleWidths();
            btnSend.Click += btnSend_Click;
            btnClear.Click += btnClear_Click;
            btn_test.Click += btn_test_Click;
            btnInvite.Click += btnInvite_Click;
        }

        private async void LoadHistoryMessages()
        {
            var client = GlobalClient.Current;
            if (client == null) return;

            var messages = await Task.Run(() => client.GetGroupHistory(_groupId, 50));
            flowMessages.Controls.Clear();
            _messageControls.Clear();

            foreach (var msg in messages.OrderBy(m => m.SendTime))
            {
                AddGroupMessage(msg, false);
            }
            ScrollToBottom();
        }

        public void AddGroupMessage(GroupMessage msg, bool autoScroll = true)
        {
            if (_messageControls.ContainsKey(msg.MessageId)) return;

            bool isSelf = msg.SenderId == GlobalClient.CurrentUserId;
            string displayName = isSelf ? "我" : msg.SenderId; // 可改为从缓存获取昵称
            var bubble = new group_bubble
            {
                IsSelf = isSelf,
                Nickname = displayName,
                MessageText = msg.Content,
                Width = flowMessages.ClientSize.Width
            };
            flowMessages.Controls.Add(bubble);
            _messageControls[msg.MessageId] = bubble;
            if (autoScroll) ScrollToBottom();
            AdjustBubbleWidths();
        }

        private void ScrollToBottom()
        {
            if (flowMessages.Controls.Count > 0)
            {
                var last = flowMessages.Controls[flowMessages.Controls.Count - 1];
                flowMessages.ScrollControlIntoView(last);
            }
        }

        private void AdjustBubbleWidths()
        {
            int newWidth = flowMessages.ClientSize.Width;
            if (flowMessages.VerticalScroll.Visible)
                newWidth -= SystemInformation.VerticalScrollBarWidth;

            foreach (var bubble in _messageControls.Values)
            {
                bubble.Width = newWidth;
            }
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (e.Packet.Type == MessageType.GroupChatMessage && e.Packet.Receiver == _groupId)
            {
                var groupMsg = new GroupMessage
                {
                    MessageId = e.Packet.MessageId,
                    GroupId = e.Packet.Receiver,
                    SenderId = e.Packet.Sender,
                    Content = e.Packet.Content,
                    SendTime = e.Packet.Timestamp,
                    MessageType = 1
                };
                this.Invoke((MethodInvoker)delegate
                {
                    AddGroupMessage(groupMsg, true);
                });
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string text = txtInput.Text.Trim();
            if (string.IsNullOrEmpty(text))
            {
                lbl_warn.Text = "消息不能为空";
                lbl_warn.Visible = true;
                return;
            }
            lbl_warn.Visible = false;

            var client = GlobalClient.Current;
            bool success = client.SendGroupMessage(_groupId, text);
            if (success)
            {
                var selfMsg = new GroupMessage
                {
                    MessageId = Guid.NewGuid().ToString(),
                    GroupId = _groupId,
                    SenderId = GlobalClient.CurrentUserId,
                    Content = text,
                    SendTime = DateTime.Now,
                    MessageType = 1
                };
                AddGroupMessage(selfMsg, true);
                txtInput.Clear();
            }
            else
            {
                MessageBox.Show("发送失败");
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtInput.Clear();
        }

        private void btn_test_Click(object sender, EventArgs e)
        {
            txtInput.Text = $"测试消息 {DateTime.Now:HH:mm:ss}";
        }

        private void btnInvite_Click(object sender, EventArgs e)
        {
            var inviteForm = new InviteToGroup(_groupId);
            inviteForm.ShowDialog();
        }
    }
}