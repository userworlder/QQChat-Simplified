using QQClient.Business;
using QQClient.Business.Services;
using QQCommon.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QQClient.UI
{
    public partial class InviteToGroup : Form
    {
        #region 字段声明

        private string _groupId;                     // 要邀请入群的群组ID

        // 业务服务（新架构）
        private IFriendBusinessService _friendService;
        private IGroupBusinessService _groupService;
        private bool _useNewService = false;         // 标记是否使用新架构

        #endregion

        #region 构造函数与初始化

        public InviteToGroup(string groupId)
        {
            InitializeComponent();
            _groupId = groupId;

            // 从服务容器获取业务服务
            InitializeServices();

            // 加载好友列表
            LoadFriends();
        }

        /// <summary>
        /// 初始化业务服务（从服务容器中获取）
        /// </summary>
        private void InitializeServices()
        {
            try
            {
                if (ServiceContainer.IsRegistered<IFriendBusinessService>())
                    _friendService = ServiceContainer.Resolve<IFriendBusinessService>();
                if (ServiceContainer.IsRegistered<IGroupBusinessService>())
                    _groupService = ServiceContainer.Resolve<IGroupBusinessService>();

                _useNewService = _friendService != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[InviteToGroup] 初始化服务失败: {ex.Message}");
                _useNewService = false;
            }
        }

        #endregion

        #region 加载好友列表（新旧架构）

        /// <summary>
        /// 加载好友列表，显示为按钮
        /// </summary>
        private async void LoadFriends()
        {
            if (_useNewService && _friendService != null)
            {
                await LoadFriendsByServiceAsync();
            }
            else
            {
                LoadFriendsLegacy();
            }
        }

        /// <summary>
        /// 使用新版服务加载好友列表
        /// </summary>
        private async Task LoadFriendsByServiceAsync()
        {
            try
            {
                // 获取当前登录用户ID
                string currentUserId = CurrentUser.UserId ?? GlobalClient.CurrentUserId;
                // 从服务器获取好友列表
                var friends = await _friendService.GetFriendListAsync(currentUserId);

                flowFriends.Controls.Clear();

                foreach (var friend in friends)
                {
                    // 优先显示昵称，没有则显示账号
                    string displayName = friend.FriendNickName ?? friend.FriendUserName;
                    // 为每个好友创建一个邀请按钮
                    var btnInvite = new Button
                    {
                        Text = displayName,
                        Tag = friend.FriendUserName,      // 存储好友账号，用于邀请
                        Width = 120,
                        Height = 30,
                        Margin = new Padding(5)
                    };
                    btnInvite.Click += async (s, e) => await InviteFriendAsync(btnInvite);
                    flowFriends.Controls.Add(btnInvite);
                }

                // 如果没有好友，显示提示文本
                if (flowFriends.Controls.Count == 0)
                {
                    flowFriends.Controls.Add(new Label { Text = "暂无好友，请先添加好友", AutoSize = true });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoadFriendsByServiceAsync] 异常: {ex.Message}");
                // 降级到旧版
                LoadFriendsLegacy();
            }
        }

        /// <summary>
        /// 使用旧版客户端加载好友列表（降级方案）
        /// </summary>
        private void LoadFriendsLegacy()
        {
            var client = GlobalClient.Current;
            if (client == null) return;

            var friends = client.SearchAllFriends(GlobalClient.CurrentUserId);
            flowFriends.Controls.Clear();

            foreach (var friend in friends)
            {
                string displayName = friend.FriendNickName ?? friend.FriendUserName;
                var btnInvite = new Button
                {
                    Text = displayName,
                    Tag = friend.FriendUserName,
                    Width = 120,
                    Height = 30,
                    Margin = new Padding(5)
                };
                // 旧版邀请方法（使用 Task.Run 避免阻塞UI）
                btnInvite.Click += async (s, e) =>
                {
                    var btn = (Button)s;
                    bool success = await Task.Run(() => client.InviteToGroup(_groupId, btn.Tag.ToString()));
                    MessageBox.Show(success ? "邀请已发送" : "邀请失败，请重试");
                };
                flowFriends.Controls.Add(btnInvite);
            }

            if (flowFriends.Controls.Count == 0)
            {
                flowFriends.Controls.Add(new Label { Text = "暂无好友，请先添加好友", AutoSize = true });
            }
        }

        #endregion

        #region 邀请好友入群

        /// <summary>
        /// 邀请指定的好友加入群组
        /// </summary>
        /// <param name="btnInvite">被点击的按钮（包含好友账号）</param>
        private async Task InviteFriendAsync(Button btnInvite)
        {
            string friendId = btnInvite.Tag.ToString();

            if (_useNewService && _groupService != null)
            {
                // 使用新版群组服务发送邀请
                bool success = await _groupService.InviteToGroupAsync(_groupId, friendId);
                MessageBox.Show(success ? "邀请已发送" : "邀请失败，请重试");
            }
            else
            {
                // 使用旧版客户端发送邀请
                var client = GlobalClient.Current;
                bool success = await Task.Run(() => client.InviteToGroup(_groupId, friendId));
                MessageBox.Show(success ? "邀请已发送" : "邀请失败，请重试");
            }
        }

        #endregion
    }
}