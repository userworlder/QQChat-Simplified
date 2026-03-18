using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QQCommon.Interfaces;
using QQCommon.Models;
using QQCommon.Protocols;
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
        private Dictionary<string, chat_new> _openChatWindows = new Dictionary<string, chat_new>();
        public user()
        {
            InitializeComponent();
            panel_x = panel1.Left;
            panel_y = panel1.Top;
            Load_Panel();
        }
        public user(string user_account, Form login)
        {
            InitializeComponent();
            self_account = user_account;
            panel_x = panel1.Left;
            panel_y = panel1.Top;
            this.Text = user_account;
            GlobalClient.Current.MessageReceived += OnMessageReceived;
            Load_Panel();
            Load_Friend();
            Load_PendingRequests();
            Load_OfflineMessages();
            this.FormClosed += (sender, e) => login.Show();
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
            var chatForm = new chat_new(friendUserName, friendDisplayName);
            // 订阅窗口关闭事件，以便从字典中移除
            chatForm.FormClosed += (s, e) =>
            {
                _openChatWindows.Remove(friendUserName);
            };
            _openChatWindows[friendUserName] = chatForm;
            chatForm.Show();
        }
        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            // 根据包的类型判断是否为好友请求
            if (e.Packet.Type == MessageType.AddFriendRequest) // 假设有这样一个类型
            {
                string fromUserId = e.Packet.Sender; // 发起者账号
                //MessageBox.Show(fromUserId);
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
                    MessageType = 1           // 假设1=文本
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
                        // 窗口未打开：更新好友列表的未读计数（可选）
                        // UpdateFriendUnreadCount(otherId);
                    }
                });
            }
        }
        private void UpdateFriendUnreadCount(string friendUserName)
        {
            foreach (Control ctrl in private_chat.Controls)
            {
                if (ctrl is ContactItem item && item.Account == friendUserName)
                {
                    // 假设 ContactItem 有一个 UnreadCount 属性，用来显示小红点
                    // 如果你还没有这个属性，可以添加一个，或者用其他方式（如修改背景色）
                    // item.UnreadCount++; // 需要先在 ContactItem 中定义 UnreadCount 属性
                    break;
                }
            }
        }
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
        //加载好友列表
        void Load_Friend()
        {
            //获取Friend
            var client = GlobalClient.Current;
            //List<Friend> friends=new List<Friend>();           
            List<Friend> friends = client.SearchAllFriends(GlobalClient.CurrentUserId);

            foreach (var friend in friends)
            {
                // 创建 ContactItem 实例
                var item = new ContactItem();
                string displayName = friend.FriendNickName.ToString();
                item.DisplayName = displayName;
                string account= friend.FriendUserName.ToString();
                item.Account = account;
                //    // 设置最后一条消息（可从 Messages 表查询最近的一条消息）
                //    // 这里暂时留空或设置默认文本，你可以单独写一个方法获取最后消息
                //  item.LastMessage = GetLatestMessage(friend.FriendUserId, currentUserId);
                item.LastMessage = "!!!";
                //    // 设置时间（例如最后消息的时间或添加好友的时间）
                //    // 这里先用 AddTime 格式化
                //    item.Time = friend.AddTime.ToString("HH:mm");
                // 设置宽度适应 FlowLayoutPanel
                item.Width = private_chat.ClientSize.Width - (private_chat.VerticalScroll.Visible ? SystemInformation.VerticalScrollBarWidth : 0);
                private_chat.Controls.Add(item);
                item.Click += (s, e) =>
                {
                    var clickedItem = (ContactItem)s;
                    string friendUserName = clickedItem.Account;
                    string FriendDisplayName = clickedItem.DisplayName;
                    OpenChatWindow(friendUserName, FriendDisplayName);
                };
            }


        }
        private void Load_PendingRequests()
        {
            var client = GlobalClient.Current;
            if (client == null) return;

            // 调用方法，获取好友请求列表
            var offlineMessages = client.GetOfflineMessages(out List<string> requestUsers);

            // 如果请求列表不为空，则填充界面
            if (requestUsers != null && requestUsers.Count > 0)
            {
                request.Controls.Clear();
                foreach (var fromUserId in requestUsers)
                {
                    var item = new FriendItem(fromUserId);
                    //AcceptClicked的对应事件
                    item.AcceptClicked += OnAcceptRequest;
                    item.RejectClicked += OnRejectRequest;
                    request.Controls.Add(item);
                }
            }
            else
            {
                // 没有请求时显示提示（可选）
                Label lblEmpty = new Label { Text = "暂无好友请求", AutoSize = true };
                request.Controls.Add(lblEmpty);
            }
        }

        private void Load_OfflineMessages()
        {
            List<string> friendRequests;
            var client= GlobalClient.Current;
            var offlineMessages = client.GetOfflineMessages(out friendRequests);

            // 将离线消息存入缓存
            if (offlineMessages != null)
            {
                MessageBox.Show($"收到了 {offlineMessages.Count} 条消息");
                foreach (var msg in offlineMessages)
                {
                    // 确定对话对方：如果我是发送者，对方是接收者；否则对方是发送者
                    string otherId = msg.SenderId == GlobalClient.CurrentUserId ? msg.ReceiverId : msg.SenderId;

                    if (!GlobalClient.MessageCache.ContainsKey(otherId))
                        GlobalClient.MessageCache[otherId] = new List<Msg>();

                    GlobalClient.MessageCache[otherId].Add(msg);
                }
            }
        }
        //同意请求的事件所执行的
        private void OnAcceptRequest(object sender, string fromUserId)
        {
            var client = GlobalClient.Current;
            MessageBox.Show($"CurrentUserId: {GlobalClient.CurrentUserId}, fromUserId: {fromUserId}");
            bool success = client.AcceptFriendRequest(fromUserId);
            if (success)
            {
                // 从 FlowLayoutPanel 中移除该项
                request.Controls.Remove((UserControl)sender);
                MessageBox.Show($"已同意 {fromUserId} 的好友请求");
            }
            else
            {
                MessageBox.Show("同意失败，请稍后重试");
            }
        }
        //拒绝请求的事件所执行的
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
        //添加好友
        private void btn_addfriend(object sender, EventArgs e)
        {
            ez_addfriend add = new ez_addfriend(self_account);
            add.ShowDialog();
        }




    }
}

