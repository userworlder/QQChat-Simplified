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
        #region 字段声明

        private string _groupId;                                    // 当前群聊的ID
        private string _groupName;                                  // 当前群聊的名称
        private user _parentForm;                                   // 主窗体引用，用于关闭后刷新群列表
        private Dictionary<string, group_bubble> _messageControls   // 存储已添加的消息气泡控件，用于去重和宽度调整
            = new Dictionary<string, group_bubble>();

        // 业务服务（新架构）
        private IGroupBusinessService _groupService;
        private bool _useNewService = false;                        // 标记是否使用新架构服务

        #endregion

        #region 构造函数与初始化

        public chat_group(string groupId, string groupName, user parentForm)
        {
            InitializeComponent();
            _groupId = groupId;
            _groupName = groupName;
            _parentForm = parentForm;

            lblGroupName.Text = groupName;
            this.Text = $"{groupName} 群聊";

            // 初始化业务服务
            InitializeServices();

            flowMessages.Visible = true;
            LoadHistoryMessages();                   // 加载历史消息

            // 订阅事件（加载时订阅，关闭时取消）
            this.Load += (s, e) => SubscribeEvents();
            this.FormClosed += (s, e) => UnsubscribeEvents();
            // 当窗体大小改变时，调整气泡宽度
            this.Resize += (s, e) => AdjustBubbleWidths();
            this.Shown += (s, e) => AdjustBubbleWidths();

            // 绑定按钮事件
            btnSend.Click += btnSend_Click;
            btnClear.Click += btnClear_Click;
            btn_test.Click += btn_test_Click;
            btnInvite.Click += btnInvite_Click;
            button1.Click += button1_Click;
        }

        /// <summary>
        /// 从服务容器获取群组业务服务
        /// </summary>
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

        /// <summary>
        /// 订阅网络消息事件（根据新旧架构选择订阅方式）
        /// </summary>
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

        /// <summary>
        /// 取消事件订阅（窗体关闭时调用）
        /// </summary>
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
            // 关闭窗口时刷新主窗体的群列表（更新最后消息、未读等）
            _parentForm?.RefreshGroupList();
        }

        #endregion

        #region 历史消息加载（新旧架构）

        /// <summary>
        /// 加载历史消息（根据是否使用新架构选择不同方式）
        /// </summary>
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

        /// <summary>
        /// 使用新版服务加载群历史消息
        /// </summary>
        private async Task LoadHistoryMessagesByServiceAsync()
        {
            try
            {
                var messages = await _groupService.GetGroupHistoryAsync(_groupId, 50);
                flowMessages.Controls.Clear();
                _messageControls.Clear();

                // 按时间顺序添加消息
                foreach (var msg in messages.OrderBy(m => m.SendTime))
                {
                    AddGroupMessage(msg, false);
                }
                ScrollToBottom();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoadHistoryMessagesByServiceAsync] 异常: {ex.Message}");
                // 降级到旧版
                LoadHistoryMessagesLegacy();
            }
        }

        /// <summary>
        /// 使用旧版客户端加载群历史消息（降级方案）
        /// </summary>
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

        #endregion

        #region 添加消息到界面

        /// <summary>
        /// 在聊天界面添加一条群消息
        /// </summary>
        /// <param name="msg">群消息对象</param>
        /// <param name="autoScroll">是否自动滚动到底部</param>
        public void AddGroupMessage(GroupMessage msg, bool autoScroll = true)
        {
            // 防止重复添加同一消息（基于 MessageId）
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

        /// <summary>
        /// 滚动到聊天面板底部（显示最新消息）
        /// </summary>
        private void ScrollToBottom()
        {
            if (flowMessages.Controls.Count > 0)
            {
                // 滚动到最后一条消息可见
                flowMessages.ScrollControlIntoView(flowMessages.Controls[flowMessages.Controls.Count - 1]);
                flowMessages.AutoScrollPosition = new System.Drawing.Point(0, flowMessages.VerticalScroll.Maximum);
                flowMessages.PerformLayout();
            }
        }

        /// <summary>
        /// 调整所有消息气泡的宽度，使其适应 FlowLayoutPanel 的宽度变化
        /// </summary>
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

        #endregion

        #region 消息接收处理（实时推送）

        /// <summary>
        /// 新消息到达事件处理（新版服务）
        /// </summary>
        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            // 只处理当前群组的群聊消息
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
                // 切换到 UI 线程添加消息
                this.Invoke((MethodInvoker)delegate
                {
                    AddGroupMessage(groupMsg, true);
                });
            }
        }

        /// <summary>
        /// 旧版消息接收处理（兼容旧客户端）
        /// </summary>
        private void OnLegacyMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            OnMessageReceived(sender, e);
        }

        #endregion

        #region 发送消息（新旧架构）

        /// <summary>
        /// 发送按钮点击事件
        /// </summary>
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

        /// <summary>
        /// 使用新版服务发送群消息
        /// </summary>
        private async Task SendGroupMessageByServiceAsync(string text)
        {
            bool success = await _groupService.SendGroupMessageAsync(_groupId, text);
            if (success)
            {
                // 乐观更新：立即显示自己发送的消息
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

        /// <summary>
        /// 使用旧版客户端发送群消息（降级方案）
        /// </summary>
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

        #endregion

        #region 其他按钮事件

        /// <summary>
        /// 清空输入框
        /// </summary>
        private void btnClear_Click(object sender, EventArgs e)
        {
            txtInput.Clear();
        }

        /// <summary>
        /// 测试按钮：在输入框填充示例消息
        /// </summary>
        private void btn_test_Click(object sender, EventArgs e)
        {
            txtInput.Text = $"测试消息 {DateTime.Now:HH:mm:ss}";
        }

        /// <summary>
        /// 邀请成员按钮：打开邀请入群窗口
        /// </summary>
        private void btnInvite_Click(object sender, EventArgs e)
        {
            var inviteForm = new InviteToGroup(_groupId);
            inviteForm.ShowDialog();
        }

        private void btnInvite_Click_1(object sender, EventArgs e) { }

        #endregion

        private async void button1_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show($"确定退出群 {_groupName} 吗？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;

            if (_useNewService && _groupService != null)
            {
                bool success = await _groupService.LeaveGroupAsync(_groupId);
                if (success)
                {
                    MessageBox.Show("已退出群组");
                    this.Close();
                    _parentForm?.RefreshGroupList();
                }
                else
                {
                    MessageBox.Show("退出失败");
                }
            }
        }
    }
}