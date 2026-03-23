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
        #region 字段声明

        private string _friendAccount;          // 聊天对象的账号
        private string _friendNickname;         // 聊天对象的昵称
        private user _parentForm;               // 主窗体引用，用于关闭后刷新好友列表

        // 业务服务（新架构）
        private IUserBusinessService _userService;
        private IFriendBusinessService _friendService;
        private IMessageBusinessService _messageService;

        private bool _useNewService = false;    // 标记是否使用新架构服务

        #endregion

        #region 构造函数与初始化

        public chat_private(string friendAccount, string friendNickname, user parentForm)
        {
            InitializeComponent();
            _friendAccount = friendAccount;
            _friendNickname = friendNickname;
            _parentForm = parentForm;

            // 设置窗体标题和好友名标签
            lblFriendName.Text = friendNickname;
            this.Text = $"与 {friendNickname} 聊天中";

            // 初始化业务服务
            InitializeServices();

            // 加载历史消息（异步）
            LoadHistoryMessages();

            // 窗体关闭时取消事件订阅并刷新主窗体的好友列表
            this.FormClosed += (s, e) =>
            {
                UnsubscribeEvents();
                _parentForm?.RefreshFriendList();
            };
        }

        /// <summary>
        /// 从服务容器获取需要的业务服务
        /// </summary>
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

            // 如果新服务可用，则订阅消息接收事件
            if (_useNewService && _messageService != null)
            {
                _messageService.MessageReceived += OnNewMessageReceived;
            }
        }

        /// <summary>
        /// 取消事件订阅（窗体关闭时调用）
        /// </summary>
        private void UnsubscribeEvents()
        {
            if (_useNewService && _messageService != null)
            {
                _messageService.MessageReceived -= OnNewMessageReceived;
            }
        }

        #endregion

        #region 新消息接收（实时推送）

        /// <summary>
        /// 新消息到达事件处理（新版服务）
        /// </summary>
        private void OnNewMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            // 只处理来自当前聊天对象的消息
            if (e.Packet.Type == MessageType.ChatMessage && e.Packet.Sender == _friendAccount)
            {
                // 由于事件在后台线程触发，需 Invoke 到 UI 线程更新界面
                this.Invoke((MethodInvoker)delegate
                {
                    AddReceivedMessage(e.Packet.Content);
                    // 收到消息后立即标记为已读（通知服务器和本地）
                    MarkMessagesAsRead();
                });
            }
        }

        #endregion

        #region 历史消息加载（新旧架构）

        /// <summary>
        /// 加载历史消息（根据是否使用新架构选择不同方式）
        /// </summary>
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

        /// <summary>
        /// 使用新版服务加载历史消息
        /// </summary>
        private async Task LoadHistoryMessagesByServiceAsync()
        {
            try
            {
                // 从服务获取与 _friendAccount 的历史消息
                var messages = await _messageService.GetHistoryMessagesAsync(_friendAccount);

                flowMessages.Controls.Clear();

                // 按时间顺序显示消息
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

                // 如果有未读消息，标记为已读（服务器和本地）
                var unreadMessages = messages.Where(m => !m.IsRead && m.ReceiverId == CurrentUser.UserId).ToList();
                if (unreadMessages.Any())
                {
                    await _messageService.MarkMessagesAsReadAsync(_friendAccount);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoadHistoryMessagesByServiceAsync] 异常: {ex.Message}");
                // 降级到旧版
                LoadHistoryMessagesLegacy();
            }
        }

        /// <summary>
        /// 使用旧版客户端加载历史消息（降级方案）
        /// </summary>
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

            // 标记未读消息为已读（仅本地数据库）
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

        #endregion

        #region 标记消息已读

        /// <summary>
        /// 标记与当前好友的所有消息为已读
        /// </summary>
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

        #endregion

        #region 添加消息到界面

        /// <summary>
        /// 在聊天界面添加一条接收到的消息（对方发送）
        /// </summary>
        public void AddReceivedMessage(string text)
        {
            var msg = new message_bubble
            {
                MessageText = text,
                IsSelf = false,                // 对方消息，气泡靠左显示
                Width = flowMessages.ClientSize.Width
            };
            flowMessages.Controls.Add(msg);
            flowMessages.ScrollControlIntoView(msg);   // 滚动到最新消息
            AdjustMessageWidths();                      // 调整所有消息气泡宽度
        }

        /// <summary>
        /// 在聊天界面添加一条发送的消息（自己发送）
        /// </summary>
        public void AddSentMessage(string text)
        {
            var msg = new message_bubble
            {
                MessageText = text,
                IsSelf = true,                 // 自己消息，气泡靠右显示
                Width = flowMessages.ClientSize.Width
            };
            flowMessages.Controls.Add(msg);
            flowMessages.ScrollControlIntoView(msg);
            AdjustMessageWidths();
        }

        /// <summary>
        /// 调整所有消息气泡的宽度，使其适应 FlowLayoutPanel 的宽度变化
        /// 如果出现垂直滚动条，需要减去滚动条宽度
        /// </summary>
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

        #endregion

        #region UI 事件处理

        /// <summary>
        /// 点击好友昵称，打开好友资料窗体
        /// </summary>
        private void lblFriendName_Click(object sender, EventArgs e)
        {
            string currentUserId = CurrentUser.UserId ?? GlobalClient.CurrentUserId;
            var profileForm = new profile(currentUserId, _friendAccount);
            profileForm.Show();
        }

        /// <summary>
        /// 发送按钮点击事件
        /// </summary>
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

        /// <summary>
        /// 清空输入框
        /// </summary>
        private void btnClear_Click_1(object sender, EventArgs e)
        {
            txtInput.Text = "";
        }

        /// <summary>
        /// 测试按钮：在输入框填充示例消息
        /// </summary>
        private void btn_test_Click(object sender, EventArgs e)
        {
            string currentUser = CurrentUser.UserId ?? GlobalClient.CurrentUserId;
            txtInput.Text = $"这是{currentUser}发给{_friendAccount}的快捷消息";
        }

        #endregion

        #region 消息发送（新旧架构）

        /// <summary>
        /// 使用新版服务发送消息
        /// </summary>
        private async Task SendMessageByServiceAsync(string text)
        {
            bool success = await _messageService.SendMessageAsync(_friendAccount, text);
            if (success)
            {
                AddSentMessage(text);   // 立即显示发送的消息（乐观更新）
                txtInput.Clear();
                lbl_warn.Visible = false;
            }
            else
            {
                MessageBox.Show("发送失败，请稍后重试");
            }
        }

        /// <summary>
        /// 使用旧版客户端发送消息（降级方案）
        /// </summary>
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

        #endregion
    }
}