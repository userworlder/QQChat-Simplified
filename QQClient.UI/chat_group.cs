using QQClient.Business;
using QQClient.Business.Services;
using QQClient.UI.user_control;
using QQCommon.Interfaces;
using QQCommon.Models;
using QQCommon.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QQClient.UI
{
    public partial class chat_group : Form
    {
        private string _groupId;
        private string _groupName;
        private user _parentForm;
        private Dictionary<string, group_bubble> _messageControls = new Dictionary<string, group_bubble>();

        // 业务服务
        private IGroupBusinessService _groupService;
        private bool _useNewService = false;

        public chat_group(string groupId, string groupName, user parentForm)
        {
            InitializeComponent();
            _groupId = groupId;
            _groupName = groupName;
            _parentForm = parentForm;

            lblGroupName.Text = groupName;
            this.Text = $"{groupName} 群聊";

            // 初始化服务
            InitializeServices();

            flowMessages.Visible = true;
            LoadHistoryMessages();

            this.Load += (s, e) => SubscribeEvents();
            this.FormClosed += (s, e) => UnsubscribeEvents();
            this.Resize += (s, e) => AdjustBubbleWidths();
            this.Shown += (s, e) => AdjustBubbleWidths();

            btnSend.Click += btnSend_Click;
            btnClear.Click += btnClear_Click;
            btn_test.Click += btn_test_Click;
            btnInvite.Click += btnInvite_Click;
        }

        private void InitializeServices()
        {
            try
            {
                if (ServiceContainer.IsRegistered<IGroupBusinessService>())
                {
                    _groupService = ServiceContainer.Resolve<IGroupBusinessService>();
                    _useNewService = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[chat_group] 初始化服务失败: {ex.Message}");
                _useNewService = false;
            }
        }

        private void SubscribeEvents()
        {
            if (_useNewService && _groupService != null)
            {
                _groupService.MessageReceived += OnMessageReceived;
            }
            else if (GlobalClient.Current != null)
            {
                GlobalClient.Current.MessageReceived += OnLegacyMessageReceived;
            }
        }

        private void UnsubscribeEvents()
        {
            if (_useNewService && _groupService != null)
            {
                _groupService.MessageReceived -= OnMessageReceived;
            }
            else if (GlobalClient.Current != null)
            {
                GlobalClient.Current.MessageReceived -= OnLegacyMessageReceived;
            }
            _parentForm?.RefreshGroupList();
        }

        private async void LoadHistoryMessages()
        {
            if (_useNewService && _groupService != null)
            {
                await LoadHistoryMessagesByServiceAsync();
            }
            else
            {
                LoadHistoryMessagesLegacy();
            }
        }

        private async Task LoadHistoryMessagesByServiceAsync()
        {
            try
            {
                var messages = await _groupService.GetGroupHistoryAsync(_groupId, 50);
                flowMessages.Controls.Clear();
                _messageControls.Clear();

                foreach (var msg in messages.OrderBy(m => m.SendTime))
                {
                    AddGroupMessage(msg, false);
                }
                ScrollToBottom();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoadHistoryMessagesByServiceAsync] 异常: {ex.Message}");
                LoadHistoryMessagesLegacy();
            }
        }

        private void LoadHistoryMessagesLegacy()
        {
            var client = GlobalClient.Current;
            if (client == null) return;

            var messages = client.GetGroupHistory(_groupId, 50);
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

            string currentUserId = CurrentUser.UserId ?? GlobalClient.CurrentUserId;
            bool isSelf = msg.SenderId == currentUserId;
            string displayName = isSelf ? "我" : msg.SenderId;

            var bubble = new group_bubble
            {
                IsSelf = isSelf,
                Nickname = displayName,
                MessageText = msg.Content,
                Width = flowMessages.ClientSize.Width
            };

            flowMessages.Controls.Add(bubble);
            _messageControls[msg.MessageId] = bubble;
            AdjustBubbleWidths();

            if (autoScroll) ScrollToBottom();
        }

        private void ScrollToBottom()
        {
            if (flowMessages.Controls.Count > 0)
            {
                flowMessages.ScrollControlIntoView(flowMessages.Controls[flowMessages.Controls.Count - 1]);
                flowMessages.AutoScrollPosition = new System.Drawing.Point(0, flowMessages.VerticalScroll.Maximum);
                flowMessages.PerformLayout();
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

        private void OnLegacyMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            OnMessageReceived(sender, e);
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            string text = txtInput.Text.Trim();
            if (string.IsNullOrEmpty(text))
            {
                lbl_warn.Text = "消息不能为空";
                lbl_warn.Visible = true;
                return;
            }
            lbl_warn.Visible = false;

            if (_useNewService && _groupService != null)
            {
                await SendGroupMessageByServiceAsync(text);
            }
            else
            {
                SendGroupMessageLegacy(text);
            }
        }

        private async Task SendGroupMessageByServiceAsync(string text)
        {
            bool success = await _groupService.SendGroupMessageAsync(_groupId, text);
            if (success)
            {
                var selfMsg = new GroupMessage
                {
                    MessageId = Guid.NewGuid().ToString(),
                    GroupId = _groupId,
                    SenderId = CurrentUser.UserId ?? GlobalClient.CurrentUserId,
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

        private void SendGroupMessageLegacy(string text)
        {
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

        private void btnInvite_Click_1(object sender, EventArgs e) { }
    }
}