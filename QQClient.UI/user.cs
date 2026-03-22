using QQClient.Business;
using QQClient.Business.Services;
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
    public partial class user : Form
    {
        private string self_account;
        private Form _loginForm;

        // 业务服务
        private IUserBusinessService _userService;
        private IFriendBusinessService _friendService;
        private IMessageBusinessService _messageService;
        private IGroupBusinessService _groupService;

        // 存储已打开的聊天窗口
        private Dictionary<string, chat_private> _openChatWindows = new Dictionary<string, chat_private>();
        private Dictionary<string, GroupItem> _groupItems = new Dictionary<string, GroupItem>();
        private Dictionary<string, chat_group> _openGroupWindows = new Dictionary<string, chat_group>();

        public user(string user_account, Form login)
        {
            InitializeComponent();
            self_account = user_account;
            _loginForm = login;
            this.Text = user_account;

            // 初始化服务
            InitializeServices();

            // 订阅消息事件
            SubscribeEvents();

            // 窗体加载事件
            this.Load += async (s, e) =>
            {
                Load_Panel();
                await LoadAllDataAsync();
            };

            // 窗体关闭事件
            this.FormClosed += (sender, e) =>
            {
                UnsubscribeEvents();
                _loginForm?.Show();
            };
        }

        #region 初始化与事件订阅

        private void InitializeServices()
        {
            try
            {
                if (ServiceContainer.IsRegistered<IUserBusinessService>())
                    _userService = ServiceContainer.Resolve<IUserBusinessService>();
                if (ServiceContainer.IsRegistered<IFriendBusinessService>())
                    _friendService = ServiceContainer.Resolve<IFriendBusinessService>();
                if (ServiceContainer.IsRegistered<IMessageBusinessService>())
                    _messageService = ServiceContainer.Resolve<IMessageBusinessService>();
                if (ServiceContainer.IsRegistered<IGroupBusinessService>())
                    _groupService = ServiceContainer.Resolve<IGroupBusinessService>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[user] 初始化服务失败: {ex.Message}");
            }
        }

        private void SubscribeEvents()
        {
            // 订阅新版服务的消息事件
            if (_messageService != null)
            {
                _messageService.MessageReceived += OnNewMessageReceived;
            }
            if (_friendService != null)
            {
                _friendService.MessageReceived += OnFriendRequestReceived;
            }
            if (_groupService != null)
            {
                _groupService.MessageReceived += OnGroupMessageReceived;
            }

            // 同时订阅旧版事件（兼容）
            if (GlobalClient.Current != null)
            {
                GlobalClient.Current.MessageReceived += OnLegacyMessageReceived;
            }
        }

        private void UnsubscribeEvents()
        {
            if (_messageService != null)
                _messageService.MessageReceived -= OnNewMessageReceived;
            if (_friendService != null)
                _friendService.MessageReceived -= OnFriendRequestReceived;
            if (_groupService != null)
                _groupService.MessageReceived -= OnGroupMessageReceived;
            if (GlobalClient.Current != null)
                GlobalClient.Current.MessageReceived -= OnLegacyMessageReceived;
        }

        #endregion

        #region 消息接收处理

        // 新版私聊消息接收
        private void OnNewMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (e.Packet.Type == MessageType.ChatMessage)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    string senderId = e.Packet.Sender;
                    string receiverId = e.Packet.Receiver;
                    string otherId = senderId == self_account ? receiverId : senderId;

                    // 更新最后消息
                    UpdateFriendLastMessage(otherId, e.Packet.Content);

                    // 如果聊天窗口未打开，增加未读计数
                    if (!_openChatWindows.ContainsKey(otherId))
                    {
                        UpdateFriendUnreadCount(otherId);
                    }
                });
            }
        }

        // 新版好友请求接收
        private void OnFriendRequestReceived(object sender, MessageReceivedEventArgs e)
        {
            if (e.Packet.Type == MessageType.AddFriendRequest)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    AddFriendRequest(e.Packet.Sender);
                });
            }
        }

        // 新版群消息接收
        private void OnGroupMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (e.Packet.Type == MessageType.GroupChatMessage)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    string groupId = e.Packet.Receiver;
                    if (_groupItems.TryGetValue(groupId, out var item))
                    {
                        item.LastMessage = e.Packet.Content;
                        item.Time = DateTime.Now.ToString("HH:mm");
                        item.UnreadCount++;
                    }
                });
            }
            else if (e.Packet.Type == MessageType.GroupJoinRequestNotification)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    LoadGroupListAsync();
                });
            }
        }

        // 旧版消息接收处理（兼容）
        private void OnLegacyMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (e.Packet.Type == MessageType.ChatMessage)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    string senderId = e.Packet.Sender;
                    string receiverId = e.Packet.Receiver;
                    string otherId = senderId == self_account ? receiverId : senderId;
                    UpdateFriendLastMessage(otherId, e.Packet.Content);

                    if (!_openChatWindows.ContainsKey(otherId))
                    {
                        UpdateFriendUnreadCount(otherId);
                    }
                });
            }
            else if (e.Packet.Type == MessageType.AddFriendRequest)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    AddFriendRequest(e.Packet.Sender);
                });
            }
            else if (e.Packet.Type == MessageType.GroupChatMessage)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    string groupId = e.Packet.Receiver;
                    if (_groupItems.TryGetValue(groupId, out var item))
                    {
                        item.LastMessage = e.Packet.Content;
                        item.Time = DateTime.Now.ToString("HH:mm");
                        item.UnreadCount++;
                    }
                });
            }
        }
        public async void Load_all()
        {
            await LoadOfflineMessagesAsync();
            await LoadFriendListAsync();
            await LoadGroupListAsync();
        }
        #endregion

        #region 数据加载

        // 异步加载所有数据
        private async Task LoadAllDataAsync()
        {
            await LoadOfflineMessagesAsync();
            await LoadFriendListAsync();
            await LoadGroupListAsync();
        }

        // 加载好友列表（新版优先）
        private async Task LoadFriendListAsync()
        {
            if (_friendService != null)
            {
                await LoadFriendListByServiceAsync();
            }
            else
            {
                LoadFriendListLegacy();
            }
        }

        private async Task LoadFriendListByServiceAsync()
        {
            private_chat.Controls.Clear();

            try
            {
                List<Friend> friends = await _friendService.GetFriendListAsync(self_account);

                foreach (var friend in friends)
                {
                    var item = new ContactItem();
                    string displayName = friend.FriendNickName ?? friend.FriendUserName;
                    item.DisplayName = displayName;
                    item.Account = friend.FriendUserName;

                    // 从缓存获取最后消息和未读数
                    var lastMessage = CacheManager.GetLastMessage(friend.FriendUserName);
                    if (lastMessage != null)
                    {
                        item.LastMessage = lastMessage.Content;
                        item.Time = lastMessage.SendTime.ToString("HH:mm");
                    }
                    else
                    {
                        item.LastMessage = "暂无消息";
                        item.Time = "";
                    }

                    item.UnreadCount = CacheManager.GetFriendUnreadCount(friend.FriendUserName);

                    item.Width = private_chat.ClientSize.Width -
                        (private_chat.VerticalScroll.Visible ? SystemInformation.VerticalScrollBarWidth : 0);
                    private_chat.Controls.Add(item);

                    item.Click += (s, e) =>
                    {
                        var clickedItem = (ContactItem)s;
                        OpenChatWindow(clickedItem.Account, clickedItem.DisplayName);
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoadFriendListAsync] 异常: {ex.Message}");
                LoadFriendListLegacy();
            }
        }

        private void LoadFriendListLegacy()
        {
            private_chat.Controls.Clear();
            var client = GlobalClient.Current;
            if (client == null) return;

            List<Friend> friends = client.SearchAllFriends(self_account);
            foreach (var friend in friends)
            {
                var item = new ContactItem();
                string displayName = friend.FriendNickName?.ToString() ?? friend.FriendUserName.ToString();
                item.DisplayName = displayName;
                item.Account = friend.FriendUserName.ToString();
                item.LastMessage = "暂无消息";
                item.Time = "";
                item.UnreadCount = 0;
                item.Width = private_chat.ClientSize.Width -
                    (private_chat.VerticalScroll.Visible ? SystemInformation.VerticalScrollBarWidth : 0);
                private_chat.Controls.Add(item);
                item.Click += (s, e) =>
                {
                    var clickedItem = (ContactItem)s;
                    OpenChatWindow(clickedItem.Account, clickedItem.DisplayName);
                };
            }
        }

        // 加载群列表
        private async Task LoadGroupListAsync()
        {
            if (_groupService != null)
            {
                await LoadGroupListByServiceAsync();
            }
            else
            {
                LoadGroupListLegacy();
            }
        }

        private async Task LoadGroupListByServiceAsync()
        {
            public_chat.Controls.Clear();
            _groupItems.Clear();

            try
            {
                var groups = await _groupService.GetGroupListAsync();

                foreach (var group in groups)
                {
                    var item = new GroupItem
                    {
                        GroupId = group.GroupId,
                        GroupName = group.GroupName,
                        Width = public_chat.ClientSize.Width -
                            (public_chat.VerticalScroll.Visible ? SystemInformation.VerticalScrollBarWidth : 0)
                    };

                    // 从缓存获取最后消息和未读数
                    var lastMessage = CacheManager.GetLastGroupMessage(group.GroupId);
                    if (lastMessage != null)
                    {
                        item.LastMessage = lastMessage.Content;
                        item.Time = lastMessage.SendTime.ToString("HH:mm");
                    }
                    else
                    {
                        item.LastMessage = "暂无消息";
                        item.Time = "";
                    }

                    item.UnreadCount = CacheManager.GetGroupUnreadCount(group.GroupId);

                    public_chat.Controls.Add(item);
                    item.Click += (s, e) => OpenGroupChat(item.GroupId, item.GroupName);
                    _groupItems[group.GroupId] = item;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoadGroupListAsync] 异常: {ex.Message}");
                LoadGroupListLegacy();
            }
        }

        private void LoadGroupListLegacy()
        {
            public_chat.Controls.Clear();
            _groupItems.Clear();
            var client = GlobalClient.Current;
            if (client == null) return;

            var groups = client.GetGroupList();
            foreach (var group in groups)
            {
                var item = new GroupItem
                {
                    GroupId = group.GroupId,
                    GroupName = group.GroupName,
                    Width = public_chat.ClientSize.Width -
                        (public_chat.VerticalScroll.Visible ? SystemInformation.VerticalScrollBarWidth : 0)
                };
                item.LastMessage = "暂无消息";
                item.Time = "";
                item.UnreadCount = 0;
                public_chat.Controls.Add(item);
                item.Click += (s, e) => OpenGroupChat(item.GroupId, item.GroupName);
                _groupItems[group.GroupId] = item;
            }
        }

        // 加载离线消息
        private async Task LoadOfflineMessagesAsync()
        {
            if (_messageService != null)
            {
                await LoadOfflineMessagesByServiceAsync();
            }
            else
            {
                LoadOfflineMessagesLegacy();
            }
        }

        private async Task LoadOfflineMessagesByServiceAsync()
        {
            try
            {
                var (messages, friendRequests) = await _messageService.GetOfflineMessagesAsync();

                // 处理好友请求
                if (friendRequests != null && friendRequests.Count > 0)
                {
                    request.Controls.Clear();
                    foreach (var fromUserId in friendRequests)
                    {
                        AddFriendRequest(fromUserId);
                    }
                }

                // 处理离线消息（更新未读计数）
                if (messages != null && messages.Count > 0)
                {
                    var unreadCounts = new Dictionary<string, int>();
                    foreach (var msg in messages)
                    {
                        string otherId = msg.SenderId == self_account ? msg.ReceiverId : msg.SenderId;
                        if (unreadCounts.ContainsKey(otherId))
                            unreadCounts[otherId]++;
                        else
                            unreadCounts[otherId] = 1;
                    }

                    foreach (var kvp in unreadCounts)
                    {
                        UpdateFriendUnreadCount(kvp.Key);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoadOfflineMessagesAsync] 异常: {ex.Message}");
                LoadOfflineMessagesLegacy();
            }
        }

        private void LoadOfflineMessagesLegacy()
        {
            var client = GlobalClient.Current;
            if (client == null) return;

            List<string> friendRequests;
            var offlineMessages = client.GetOfflineMessages(out friendRequests);

            if (friendRequests != null && friendRequests.Count > 0)
            {
                request.Controls.Clear();
                foreach (var fromUserId in friendRequests)
                {
                    AddFriendRequest(fromUserId);
                }
            }

            if (offlineMessages != null && offlineMessages.Count > 0)
            {
                var unreadCounts = new Dictionary<string, int>();
                foreach (var msg in offlineMessages)
                {
                    string otherId = msg.SenderId == self_account ? msg.ReceiverId : msg.SenderId;
                    if (unreadCounts.ContainsKey(otherId))
                        unreadCounts[otherId]++;
                    else
                        unreadCounts[otherId] = 1;
                }

                foreach (var kvp in unreadCounts)
                {
                    UpdateFriendUnreadCount(kvp.Key);
                }
            }
        }

        #endregion

        #region 好友请求处理

        private void AddFriendRequest(string fromUserId)
        {
            foreach (Control ctrl in request.Controls)
            {
                if (ctrl is FriendItem exist_item && exist_item.FromUserId == fromUserId)
                    return;
            }

            var item = new FriendItem(fromUserId);
            item.AcceptClicked += OnAcceptRequest;
            item.RejectClicked += OnRejectRequest;
            request.Controls.Add(item);
        }

        private async void OnAcceptRequest(object sender, string fromUserId)
        {
            if (_friendService != null)
            {
                await AcceptFriendRequestByServiceAsync(sender, fromUserId);
            }
            else
            {
                AcceptFriendRequestLegacy(sender, fromUserId);
            }
        }

        private async Task AcceptFriendRequestByServiceAsync(object sender, string fromUserId)
        {
            try
            {
                bool success = await _friendService.AcceptFriendRequestAsync(fromUserId);
                if (success)
                {
                    request.Controls.Remove((UserControl)sender);
                    MessageBox.Show($"已同意 {fromUserId} 的好友请求");
                    await LoadFriendListAsync();
                }
                else
                {
                    MessageBox.Show("同意失败，请稍后重试");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"同意失败: {ex.Message}");
            }
        }

        private void AcceptFriendRequestLegacy(object sender, string fromUserId)
        {
            var client = GlobalClient.Current;
            if (client == null) return;

            bool success = client.AcceptFriendRequest(fromUserId);
            if (success)
            {
                request.Controls.Remove((UserControl)sender);
                MessageBox.Show($"已同意 {fromUserId} 的好友请求");
                LoadFriendListLegacy();
            }
            else
            {
                MessageBox.Show("同意失败，请稍后重试");
            }
        }

        private async void OnRejectRequest(object sender, string fromUserId)
        {
            if (_friendService != null)
            {
                await RejectFriendRequestByServiceAsync(sender, fromUserId);
            }
            else
            {
                RejectFriendRequestLegacy(sender, fromUserId);
            }
        }

        private async Task RejectFriendRequestByServiceAsync(object sender, string fromUserId)
        {
            try
            {
                bool success = await _friendService.RejectFriendRequestAsync(fromUserId);
                if (success)
                {
                    request.Controls.Remove((UserControl)sender);
                    MessageBox.Show($"已拒绝 {fromUserId} 的好友请求");
                }
                else
                {
                    MessageBox.Show("拒绝失败，请稍后重试");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"拒绝失败: {ex.Message}");
            }
        }

        private void RejectFriendRequestLegacy(object sender, string fromUserId)
        {
            var client = GlobalClient.Current;
            if (client == null) return;

            bool success = client.RejectFriendRequest(fromUserId);
            if (success)
            {
                request.Controls.Remove((UserControl)sender);
                MessageBox.Show($"已拒绝 {fromUserId} 的好友请求");
            }
            else
            {
                MessageBox.Show("拒绝失败，请稍后重试");
            }
        }

        #endregion

        #region 聊天窗口管理

        private void OpenChatWindow(string friendUserName, string friendDisplayName)
        {
            if (_openChatWindows.ContainsKey(friendUserName))
            {
                var existing = _openChatWindows[friendUserName];
                if (!existing.IsDisposed)
                {
                    existing.Activate();
                    return;
                }
                else
                {
                    _openChatWindows.Remove(friendUserName);
                }
            }

            var chatForm = new chat_private(friendUserName, friendDisplayName, this);
            chatForm.FormClosed += (s, e) => _openChatWindows.Remove(friendUserName);
            _openChatWindows[friendUserName] = chatForm;
            chatForm.Show();
        }

        private void OpenGroupChat(string groupId, string groupName)
        {
            if (_openGroupWindows.ContainsKey(groupId))
            {
                var existing = _openGroupWindows[groupId];
                if (!existing.IsDisposed)
                {
                    existing.Activate();
                    return;
                }
                else
                {
                    _openGroupWindows.Remove(groupId);
                }
            }

            var groupChat = new chat_group(groupId, groupName, this);
            groupChat.FormClosed += (s, e) => _openGroupWindows.Remove(groupId);
            _openGroupWindows[groupId] = groupChat;
            groupChat.Show();
        }

        #endregion

        #region UI更新辅助方法

        private void UpdateFriendUnreadCount(string friendUserName)
        {
            foreach (Control ctrl in private_chat.Controls)
            {
                if (ctrl is ContactItem item && item.Account == friendUserName)
                {
                    item.UnreadCount++;
                    break;
                }
            }
        }

        private void UpdateFriendLastMessage(string friendUserName, string lastMessage)
        {
            foreach (Control ctrl in private_chat.Controls)
            {
                if (ctrl is ContactItem item && item.Account == friendUserName)
                {
                    item.LastMessage = lastMessage;
                    item.Time = DateTime.Now.ToString("HH:mm");
                    break;
                }
            }
        }

        #endregion

        #region 面板切换

        private void Load_Panel()
        {
            private_chat.Left = 0;
            private_chat.Top = 0;
            public_chat.Left = 0;
            public_chat.Top = 0;
            request.Left = 0;
            request.Top = 0;
        }

        private void btn_privatemode(object sender, EventArgs e)
        {
            public_chat.Visible = false;
            private_chat.Visible = true;
            request.Visible = false;
        }

        private void btn_publicmode(object sender, EventArgs e)
        {
            public_chat.Visible = true;
            private_chat.Visible = false;
            request.Visible = false;
        }

        private void btn_requestmode(object sender, EventArgs e)
        {
            public_chat.Visible = false;
            private_chat.Visible = false;
            request.Visible = true;
        }

        #endregion

        #region 按钮事件

        private void btn_profile_Click(object sender, EventArgs e)
        {
            profile profileForm = new profile(self_account, self_account);
            profileForm.ShowDialog();
        }

        private async void btn_creatgroup_Click(object sender, EventArgs e)
        {
            var form = new CreateGroupForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                if (_groupService != null)
                {
                    await CreateGroupByServiceAsync(form);
                }
                else
                {
                    CreateGroupLegacy(form);
                }
            }
        }

        private async Task CreateGroupByServiceAsync(CreateGroupForm form)
        {
            try
            {
                string groupId = await _groupService.CreateGroupAsync(form.GroupName, form.Description);
                if (!string.IsNullOrEmpty(groupId))
                {
                    MessageBox.Show("群组创建成功");
                    await LoadGroupListAsync();
                }
                else
                {
                    MessageBox.Show("创建失败");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"创建失败: {ex.Message}");
            }
        }

        private void CreateGroupLegacy(CreateGroupForm form)
        {
            var client = GlobalClient.Current;
            if (client == null) return;

            string groupId = client.CreateGroup(form.GroupName, form.Description);
            if (!string.IsNullOrEmpty(groupId))
            {
                MessageBox.Show("群组创建成功");
                LoadGroupListLegacy();
            }
            else
            {
                MessageBox.Show("创建失败");
            }
        }

        private void btn_search_Click(object sender, EventArgs e)
        {
            search searchForm = new search(self_account, this);
            searchForm.ShowDialog();
        }

        #endregion

        #region 公共刷新方法（供子窗体调用）

        public async void RefreshFriendList()
        {
            await LoadOfflineMessagesAsync();
            await LoadFriendListAsync();
        }

        public async void RefreshGroupList()
        {
            await LoadGroupListAsync();
        }

        #endregion
    }
}