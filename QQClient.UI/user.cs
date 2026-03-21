using QQClient.UI.user_control;
using QQCommon.Interfaces;
using QQCommon.Models;
using QQCommon.Protocols;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//防止关键字的冲突
using Msg = QQCommon.Models.Message;
namespace QQClient.UI
{
    public partial class user : Form
    {
        string self_account;
        string fromUserId;
        //panel的坐标
        int panel_x;
        int panel_y;
        // 存储已打开的聊天窗口，键为好友用户名
        private Dictionary<string, chat_private> _openChatWindows = new Dictionary<string, chat_private>();
        private Dictionary<string, GroupItem> _groupItems = new Dictionary<string, GroupItem>();
        private Dictionary<string, chat_group> _openGroupWindows = new Dictionary<string, chat_group>();
        public user(string user_account, Form login)
        {
            InitializeComponent();
            self_account = user_account;
            panel_x = panel1.Left;
            panel_y = panel1.Top;
            this.Text = user_account;
            GlobalClient.Current.MessageReceived += OnMessageReceived;

            this.Load += (s, e) =>
            {
                Load_Panel();
                Load_OfflineMessages();
                Load_Friend();
                LoadGroupList();
            };
            // Load_PendingRequests();
            this.FormClosed += (sender, e) => login.Show();
        }
        //刷新所有列表（关闭聊天窗口的事件所触发）
        public void RefreshFriendList()
        {
            // 先获取最新的离线消息（包括好友请求和未读消息），更新缓存
            Load_OfflineMessages();
            // 重新加载好友列表（会清空原有控件，从缓存读取最后消息、未读计数，并显示新好友）
            Load_Friend();
            //重新加载群聊列表
            RefreshGroupList();
        }
        // 打开或激活聊天窗口的方法
        private void OpenChatWindow(string friendUserName, string friendDisplayName)
        {
            if (_openChatWindows.ContainsKey(friendUserName))
            {
                // 如果窗口已存在，激活它
                var existing = _openChatWindows[friendUserName];
                if (!existing.IsDisposed)
                {
                    existing.Activate();
                    return;
                }
                else
                {
                    // 如果窗口已关闭，从字典移除
                    _openChatWindows.Remove(friendUserName);
                }
            }

            // 创建新窗口
            var chatForm = new chat_private(friendUserName, friendDisplayName, this);
            // 订阅窗口关闭事件，以便从字典中移除
            chatForm.FormClosed += (s, e) =>
            {
                _openChatWindows.Remove(friendUserName);
            };
            _openChatWindows[friendUserName] = chatForm;
            chatForm.Show();
        }
        private void LoadGroupList1()
        {
            var client = GlobalClient.Current;
            var groups = client.GetGroupList();
            public_chat.Controls.Clear();
            _groupItems.Clear();
            foreach (var group in groups)
            {
                var item = new GroupItem
                {
                    GroupId = group.GroupId,
                    GroupName = group.GroupName,
                    Width = public_chat.ClientSize.Width - (public_chat.VerticalScroll.Visible ? SystemInformation.VerticalScrollBarWidth : 0)
                };
                // 从缓存中获取最后消息和未读（如果有群消息缓存）
                // 暂未实现群消息缓存，可以简单显示“暂无消息”
                item.LastMessage = "暂无消息";
                item.Time = "";
                item.UnreadCount = 0; // 从缓存读取

                public_chat.Controls.Add(item);
                item.Click += (s, e) =>
                {
                    OpenGroupChat(item.GroupId, item.GroupName);
                };
                _groupItems[group.GroupId] = item;
            }
        }
        private void LoadGroupList()
        {
            var client = GlobalClient.Current;
            var groups = client.GetGroupList();
            public_chat.Controls.Clear();
            _groupItems.Clear();

            foreach (var group in groups)
            {
                var item = new GroupItem
                {
                    GroupId = group.GroupId,
                    GroupName = group.GroupName,
                    Width = public_chat.ClientSize.Width - (public_chat.VerticalScroll.Visible ? SystemInformation.VerticalScrollBarWidth : 0)
                };

                // 1. 先从缓存读取最后消息
                string lastMessage = "暂无消息";
                string lastTime = "";

                if (GlobalClient.GroupMessageCache.TryGetValue(group.GroupId, out var cachedMessages))
                {
                    var latest = cachedMessages.OrderByDescending(m => m.SendTime).FirstOrDefault();
                    if (latest != null)
                    {
                        lastMessage = latest.Content;
                        lastTime = latest.SendTime.ToString("HH:mm");
                    }
                }

                // 2. 未读计数从 GroupUnreadCount 中获取，没有则默认为 0
                int unread = GlobalClient.GroupUnreadCount.ContainsKey(group.GroupId) ? GlobalClient.GroupUnreadCount[group.GroupId] : 0;

                item.LastMessage = lastMessage;
                item.Time = lastTime;
                item.UnreadCount = unread;

                public_chat.Controls.Add(item);
                item.Click += (s, e) => OpenGroupChat(item.GroupId, item.GroupName);
                _groupItems[group.GroupId] = item;

                // 3. 异步获取历史消息，用最新消息更新界面，并存入缓存（不修改未读计数）
                Task.Run(() =>
                {
                    var history = client.GetGroupHistory(group.GroupId, 50);
                    if (history != null && history.Any())
                    {
                        lock (GlobalClient.GroupMessageCache)
                        {
                            if (!GlobalClient.GroupMessageCache.ContainsKey(group.GroupId))
                                GlobalClient.GroupMessageCache[group.GroupId] = new List<GroupMessage>();

                            var existingIds = new HashSet<string>(GlobalClient.GroupMessageCache[group.GroupId].Select(m => m.MessageId));
                            foreach (var msg in history)
                            {
                                if (!existingIds.Contains(msg.MessageId))
                                    GlobalClient.GroupMessageCache[group.GroupId].Add(msg);
                            }
                        }

                        var latest = history.OrderByDescending(m => m.SendTime).First();
                        this.Invoke((MethodInvoker)delegate
                        {
                            item.LastMessage = latest.Content;
                            item.Time = latest.SendTime.ToString("HH:mm");
                        });
                    }
                });
            }
        }
        private void OpenGroupChat(string groupId, string groupName)
        {
            // 检查是否已打开，可类似私聊做窗口管理
            if (_openGroupWindows.ContainsKey(groupId))
            {
                _openGroupWindows[groupId].Activate();
                return;
            }
            var groupChat = new chat_group(groupId, groupName, this);
            groupChat.FormClosed += (s, e) => _openGroupWindows.Remove(groupId);
            _openGroupWindows[groupId] = groupChat;
            groupChat.Show();
        }
        //在线消息/请求的接收
        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            void SafeInvoke(Action action)
            {
                if (this.IsHandleCreated)
                {
                    this.Invoke(action);
                }
                else
                {
                    EventHandler handler = null;
                    handler = (s, ev) =>
                    {
                        this.HandleCreated -= handler;
                        this.Invoke(action);
                    };
                    this.HandleCreated += handler;
                }
            }
            // 根据包的类型判断是否为好友请求
            if (e.Packet.Type == MessageType.AddFriendRequest)
            {
                string fromUserId = e.Packet.Sender; // 发起者账号
                // 切换到 UI 线程添加请求项
                this.Invoke((MethodInvoker)delegate
                {
                    AddFriendRequest(fromUserId);
                });
            }
            //处理在线聊天消息
            else if (e.Packet.Type == MessageType.ChatMessage)
            {
                string senderId = e.Packet.Sender;
                string receiverId = e.Packet.Receiver;
                string content = e.Packet.Content;

                // 确定消息对方：如果自己是接收者，对方是发送者；否则是接收者（一般不会收到自己发的）
                string otherId = (receiverId == GlobalClient.CurrentUserId) ? senderId : receiverId;

                // 将消息存入缓存
                var msg = new Msg  // Msg 是别名，指向 QQCommon.Models.Message
                {
                    MessageId = e.Packet.MessageId,
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    Content = content,
                    SendTime = e.Packet.Timestamp,
                    IsRead = false,          // 初始为未读
                    MessageType = 1           // 1=文本
                };

                this.Invoke((MethodInvoker)delegate
                {
                    // 存入缓存
                    if (!GlobalClient.MessageCache.ContainsKey(otherId))
                        GlobalClient.MessageCache[otherId] = new List<Msg>();
                    GlobalClient.MessageCache[otherId].Add(msg);

                    if (_openChatWindows.ContainsKey(otherId))
                    {
                        // 窗口已打开，直接添加消息
                        _openChatWindows[otherId].AddReceivedMessage(content);
                    }
                    else
                    {
                        // 窗口未打开：更新好友列表的未读计数
                        UpdateFriendUnreadCount(otherId);
                        UpdateFriendLastMessage(otherId, content);
                    }
                });
            }
            else if (e.Packet.Type == MessageType.GroupChatMessage)
            {
                string groupId = e.Packet.Receiver;
                string content = e.Packet.Content;
                var groupMsg = new GroupMessage
                {
                    MessageId = e.Packet.MessageId,
                    GroupId = groupId,
                    SenderId = e.Packet.Sender,
                    Content = content,
                    SendTime = e.Packet.Timestamp,
                    MessageType = 1
                };
                this.Invoke((MethodInvoker)delegate
                {
                    if (_openGroupWindows.ContainsKey(groupId))
                    {
                        _openGroupWindows[groupId].AddGroupMessage(groupMsg, true);
                    }
                    else
                    {
                        if (_groupItems.TryGetValue(groupId, out var item))
                        {
                            item.LastMessage = content;
                            item.Time = groupMsg.SendTime.ToString("HH:mm");
                            item.UnreadCount++;
                        }
                    }
                });
            }
            else if (e.Packet.Type == MessageType.GroupJoinRequestNotification)
            {
                // 收到邀请或加入群的通知，刷新群列表
                this.Invoke((MethodInvoker)delegate
                {
                    LoadGroupList();
                });
            }
            else if (e.Packet.Type == MessageType.JoinGroupResponse && e.Packet.Content == "SUCCESS")
            {
                this.Invoke((MethodInvoker)delegate
                {
                    LoadGroupList();
                });
            }
        }
       public void Load_all()
        {
            Load_OfflineMessages();
            Load_Friend();
            LoadGroupList();

        }
        //加载好友请求
        private void AddFriendRequest(string fromUserId)
        {
            //MessageBox.Show(fromUserId);
            // 检查是否已经存在相同请求
            foreach (Control ctrl in request.Controls)
            {
                if (ctrl is FriendItem exist_item && exist_item.FromUserId == fromUserId)
                    return;
            }
            //MessageBox.Show(fromUserId);
            var item = new FriendItem(fromUserId);
            item.AcceptClicked += OnAcceptRequest;
            item.RejectClicked += OnRejectRequest;
            request.Controls.Add(item);
        }
        //加载离线消息/请求
        private void Load_OfflineMessages()
        {
            List<string> friendRequests;
            var client = GlobalClient.Current;
            var offlineMessages = client.GetOfflineMessages(out friendRequests);

            // 处理好友请求
            if (friendRequests != null && friendRequests.Count > 0)
            {
                request.Controls.Clear();
                foreach (var fromUserId in friendRequests)
                {
                    var item = new FriendItem(fromUserId);
                    item.AcceptClicked += OnAcceptRequest;
                    item.RejectClicked += OnRejectRequest;
                    request.Controls.Add(item);
                }
            }

            // 处理离线消息：存入缓存（供好友列表显示最后消息和时间）并更新未读计数
            if (offlineMessages != null && offlineMessages.Count > 0)
            {
                //MessageBox.Show($"你有 {offlineMessages.Count}条离线消息");  // 调试用

                // 先存入缓存
                foreach (var msg in offlineMessages)
                {
                    string otherId = msg.SenderId == GlobalClient.CurrentUserId ? msg.ReceiverId : msg.SenderId;
                    if (!GlobalClient.MessageCache.ContainsKey(otherId))
                        GlobalClient.MessageCache[otherId] = new List<Msg>();
                    GlobalClient.MessageCache[otherId].Add(msg);
                }

                // 统计未读计数（用于小红点，可选）
                var unreadCounts = new Dictionary<string, int>();
                foreach (var msg in offlineMessages)
                {
                    string otherId = msg.SenderId == GlobalClient.CurrentUserId ? msg.ReceiverId : msg.SenderId;
                    if (unreadCounts.ContainsKey(otherId))
                        unreadCounts[otherId]++;
                    else
                        unreadCounts[otherId] = 1;
                }
            }

        }
        //加载界面的位置
        void Load_Panel()
        {
            private_chat.Left = 0;
            private_chat.Top = 0;
            public_chat.Left = 0;
            public_chat.Top = 0;
            request.Left = 0;
            request.Top = 0;
        }
        //加载好友栏
        void Load_Friend()
        {
            private_chat.Controls.Clear();  // 清空现有控件，防止重复
            var client = GlobalClient.Current;
            List<Friend> friends = client.SearchAllFriends(GlobalClient.CurrentUserId);
            foreach (var friend in friends)
            {
                var item = new ContactItem();
                string displayName = friend.FriendNickName?.ToString() ?? friend.FriendUserName.ToString();
                item.DisplayName = displayName;
                string account = friend.FriendUserName.ToString();
                item.Account = account;

                // 先用缓存中的未读消息作为临时数据
                string lastMessage = "暂无消息";
                string lastTime = "";
                int unread = 0;
                if (GlobalClient.MessageCache.ContainsKey(account))
                {
                    var messages = GlobalClient.MessageCache[account];
                    unread = messages.Count(m => !m.IsRead);
                    var latestUnread = messages.OrderByDescending(m => m.SendTime).FirstOrDefault();
                    if (latestUnread != null)
                    {
                        lastMessage = latestUnread.Content;
                        lastTime = latestUnread.SendTime.ToString("HH:mm");
                    }
                }
                item.LastMessage = lastMessage;
                item.Time = lastTime;
                item.UnreadCount = unread;

                // 设置宽度和点击事件
                item.Width = private_chat.ClientSize.Width - (private_chat.VerticalScroll.Visible ? SystemInformation.VerticalScrollBarWidth : 0);
                private_chat.Controls.Add(item);
                item.Click += (s, e) =>
                {
                    var clickedItem = (ContactItem)s;
                    OpenChatWindow(clickedItem.Account, clickedItem.DisplayName);
                };

                // 异步获取完整历史消息，用最新消息更新界面（不阻塞UI）
                Task.Run(() =>
                {
                    var messages = client.GetHistoryMessages(account);
                    if (messages != null && messages.Any())
                    {
                        var latest = messages.OrderByDescending(m => m.SendTime).First();
                        // 回到UI线程更新
                        this.Invoke((MethodInvoker)delegate
                        {
                            item.LastMessage = latest.Content;
                            item.Time = latest.SendTime.ToString("HH:mm");
                        });
                    }
                });
            }
        }
        //更新未读消息小红点
        private void UpdateFriendUnreadCount(string friendUserName)
        {
            foreach (Control ctrl in private_chat.Controls)
            {
                if (ctrl is ContactItem item && item.Account == friendUserName)
                {
                    // UnreadCount 属性，用来显示小红点
                    item.UnreadCount++;
                    break;
                }
            }
        }
        //更新最后消息
        private void UpdateFriendLastMessage(string friendUserName, string lastMessage)
        {
            foreach (Control ctrl in private_chat.Controls)
            {
                if (ctrl is ContactItem item && item.Account == friendUserName)
                {
                    //LastMessage 属性，用来显示聊天的最后一条消息
                    item.LastMessage = lastMessage;
                    break;
                }
            }
        }
        //同意请求的事件
        private void OnAcceptRequest(object sender, string fromUserId)
        {
            var client = GlobalClient.Current;
            //MessageBox.Show($"CurrentUserId: {GlobalClient.CurrentUserId}, fromUserId: {fromUserId}");
            bool success = client.AcceptFriendRequest(fromUserId);
            if (success)
            {
                // 从 FlowLayoutPanel 中移除该项
                request.Controls.Remove((UserControl)sender);
                MessageBox.Show($"已同意 {fromUserId} 的好友请求");
                Load_Friend();
            }
            else
            {
                MessageBox.Show("同意失败，请稍后重试");
            }
        }
        //拒绝请求的事件
        private void OnRejectRequest(object sender, string fromUserId)
        {
            var client = GlobalClient.Current;
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
        //私聊模式
        private void btn_privatemode(object sender, EventArgs e)
        {
            public_chat.Visible = false;
            private_chat.Visible = true;
            request.Visible = false;
        }
        //群聊模式
        private void btn_publicmode(object sender, EventArgs e)
        {
            public_chat.Visible = true;
            private_chat.Visible = false;
            request.Visible = false;
        }
        //验证消息
        private void btn_requestmode(object sender, EventArgs e)
        {
            public_chat.Visible = false;
            private_chat.Visible = false;
            request.Visible = true;
        }
        //打开个人简历
        private void btn_profile_Click(object sender, EventArgs e)
        {
            profile profile = new profile(self_account, self_account);
            profile.ShowDialog();
        }
        // 刷新群列表
        public void RefreshGroupList()
        {
            LoadGroupList();
        }
        //创建群聊
        private void btn_creatgroup_Click(object sender, EventArgs e)
        {
            var form = new CreateGroupForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                var client = GlobalClient.Current;
                string groupId = client.CreateGroup(form.GroupName, form.Description);
                if (!string.IsNullOrEmpty(groupId))
                {
                    MessageBox.Show("群组创建成功");
                    LoadGroupList(); // 刷新群列表
                }
                else
                {
                    MessageBox.Show("创建失败");
                }
            }
        }
        //加人/群
        private void btn_search_Click(object sender, EventArgs e)
        {
            search search = new search(GlobalClient.CurrentUserId, this);
            search.ShowDialog();
        }

        //废弃代码
        //搜索群聊
        //private void button6_Click(object sender, EventArgs e)
        //{
        //    var form = new SearchGroupForm(this);
        //    form.ShowDialog();
        //}
        //添加好友
        //private void btn_addfriend(object sender, EventArgs e)
        //{
        //    ez_addfriend add = new ez_addfriend(self_account);
        //    add.ShowDialog();
        //}
    }
}

