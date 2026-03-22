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
        private string _groupId;

        // 业务服务
        private IFriendBusinessService _friendService;
        private IGroupBusinessService _groupService;
        private bool _useNewService = false;

        public InviteToGroup(string groupId)
        {
            InitializeComponent();
            _groupId = groupId;

            // 初始化服务
            InitializeServices();

            LoadFriends();
        }

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

        private async Task LoadFriendsByServiceAsync()
        {
            try
            {
                string currentUserId = CurrentUser.UserId ?? GlobalClient.CurrentUserId;
                var friends = await _friendService.GetFriendListAsync(currentUserId);

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
                    btnInvite.Click += async (s, e) => await InviteFriendAsync(btnInvite);
                    flowFriends.Controls.Add(btnInvite);
                }

                if (flowFriends.Controls.Count == 0)
                {
                    flowFriends.Controls.Add(new Label { Text = "暂无好友，请先添加好友", AutoSize = true });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoadFriendsByServiceAsync] 异常: {ex.Message}");
                LoadFriendsLegacy();
            }
        }

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

        private async Task InviteFriendAsync(Button btnInvite)
        {
            if (_useNewService && _groupService != null)
            {
                bool success = await _groupService.InviteToGroupAsync(_groupId, btnInvite.Tag.ToString());
                MessageBox.Show(success ? "邀请已发送" : "邀请失败，请重试");
            }
            else
            {
                var client = GlobalClient.Current;
                bool success = await Task.Run(() => client.InviteToGroup(_groupId, btnInvite.Tag.ToString()));
                MessageBox.Show(success ? "邀请已发送" : "邀请失败，请重试");
            }
        }
    }
}