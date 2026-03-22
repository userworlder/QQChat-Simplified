using QQClient.Business;
using QQClient.Business.Services;
using QQCommon.Interfaces;
using QQCommon.Models;
using QQCommon.Protocols;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QQClient.UI
{
    public partial class chat_private : Form
    {
        private string _friendAccount;
        private string _friendNickname;
        private user _parentForm;

        // 业务服务
        private IUserBusinessService _userService;
        private IFriendBusinessService _friendService;
        private IMessageBusinessService _messageService;

        private bool _useNewService = false;

        public chat_private(string friendAccount, string friendNickname, user parentForm)
        {
            InitializeComponent();
            _friendAccount = friendAccount;
            _friendNickname = friendNickname;
            _parentForm = parentForm;

            lblFriendName.Text = friendNickname;
            this.Text = $"与 {friendNickname} 聊天中";

            // 初始化服务
            InitializeServices();

            // 加载历史消息
            LoadHistoryMessages();

            this.FormClosed += (s, e) =>
            {
                UnsubscribeEvents();
                _parentForm?.RefreshFriendList();
            };
        }

        private void InitializeServices()
        {
            try
            {
                if (ServiceContainer.IsRegistered<IUserBusinessService>())
                    _userService = ServiceContainer.Resolve<IUserBusinessService>();
                if (ServiceContainer.IsRegistered<IFriendBusinessService>())
                    _friendService = ServiceContainer.Resolve<IFriendBusinessService>();
                if (ServiceContainer.IsRegistered<IMessageBusinessService>())
                {
                    _messageService = ServiceContainer.Resolve<IMessageBusinessService>();
                    _useNewService = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[chat_private] 初始化服务失败: {ex.Message}");
                _useNewService = false;
            }

            // 订阅新消息事件
            if (_useNewService && _messageService != null)
            {
                _messageService.MessageReceived += OnNewMessageReceived;
            }
        }

        private void UnsubscribeEvents()
        {
            if (_useNewService && _messageService != null)
            {
                _messageService.MessageReceived -= OnNewMessageReceived;
            }
        }

        // 新版消息接收
        private void OnNewMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (e.Packet.Type == MessageType.ChatMessage && e.Packet.Sender == _friendAccount)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    AddReceivedMessage(e.Packet.Content);
                    // 标记已读
                    MarkMessagesAsRead();
                });
            }
        }

        private async void LoadHistoryMessages()
        {
            if (_useNewService && _messageService != null)
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
                var messages = await _messageService.GetHistoryMessagesAsync(_friendAccount);

                flowMessages.Controls.Clear();

                foreach (var msg in messages.OrderBy(m => m.SendTime))
                {
                    if (msg.SenderId == _friendAccount)
                    {
                        AddReceivedMessage(msg.Content);
                    }
                    else
                    {
                        AddSentMessage(msg.Content);
                    }
                }

                // 标记未读消息为已读
                var unreadMessages = messages.Where(m => !m.IsRead && m.ReceiverId == CurrentUser.UserId).ToList();
                if (unreadMessages.Any())
                {
                    await _messageService.MarkMessagesAsReadAsync(_friendAccount);
                }
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

            var messages = client.GetHistoryMessages(_friendAccount);

            flowMessages.Controls.Clear();

            foreach (var msg in messages.OrderBy(m => m.SendTime))
            {
                if (msg.SenderId == _friendAccount)
                {
                    AddReceivedMessage(msg.Content);
                }
                else
                {
                    AddSentMessage(msg.Content);
                }
            }

            var unreadMessages = messages.Where(m => !m.IsRead && m.ReceiverId == GlobalClient.CurrentUserId).ToList();
            if (unreadMessages.Any())
            {
                bool success = client.MarkMessagesAsRead(_friendAccount);
                if (success)
                {
                    Console.WriteLine("消息已标记为已读");
                }
                else
                {
                    Console.WriteLine("标记已读失败");
                }
            }
        }

        private async void MarkMessagesAsRead()
        {
            if (_useNewService && _messageService != null)
            {
                await _messageService.MarkMessagesAsReadAsync(_friendAccount);
            }
            else
            {
                var client = GlobalClient.Current;
                client?.MarkMessagesAsRead(_friendAccount);
            }
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

        private void lblFriendName_Click(object sender, EventArgs e)
        {
            string currentUserId = CurrentUser.UserId ?? GlobalClient.CurrentUserId;
            var profileForm = new profile(currentUserId, _friendAccount);
            profileForm.Show();
        }

        private async void btnSend_Click_1(object sender, EventArgs e)
        {
            string text = txtInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                lbl_warn.Visible = true;
                lbl_warn.Text = "请输入文本";
                return;
            }

            lbl_warn.Visible = false;

            if (_useNewService && _messageService != null)
            {
                await SendMessageByServiceAsync(text);
            }
            else
            {
                SendMessageLegacy(text);
            }
        }

        private async Task SendMessageByServiceAsync(string text)
        {
            bool success = await _messageService.SendMessageAsync(_friendAccount, text);
            if (success)
            {
                AddSentMessage(text);
                txtInput.Clear();
                lbl_warn.Visible = false;
            }
            else
            {
                MessageBox.Show("发送失败，请稍后重试");
            }
        }

        private void SendMessageLegacy(string text)
        {
            var client = GlobalClient.Current;
            if (client == null) return;

            bool success = client.SendMessage(GlobalClient.CurrentUserId, _friendAccount, text);
            if (success)
            {
                AddSentMessage(text);
                txtInput.Clear();
                lbl_warn.Visible = false;
            }
            else
            {
                MessageBox.Show("发送失败，请稍后重试");
            }
        }

        private void btnClear_Click_1(object sender, EventArgs e)
        {
            txtInput.Text = "";
        }

        private void btn_test_Click(object sender, EventArgs e)
        {
            string currentUser = CurrentUser.UserId ?? GlobalClient.CurrentUserId;
            txtInput.Text = $"这是{currentUser}发给{_friendAccount}的快捷消息";
        }
    }
}