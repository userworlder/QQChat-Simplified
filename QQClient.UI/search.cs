using QQClient.Business;
using QQClient.Business.Services;
using QQCommon.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QQClient.UI
{
    public partial class search : Form
    {
        private user _parentForm;
        private string _selfAccount;

        // 业务服务
        private IFriendBusinessService _friendService;
        private IGroupBusinessService _groupService;
        private bool _useNewService = false;

        public search(string selfAccount, user parentForm)
        {
            InitializeComponent();
            Load_Panel();
            _selfAccount = selfAccount;
            _parentForm = parentForm;

            // 初始化服务
            InitializeServices();
        }

        private void InitializeServices()
        {
            try
            {
                if (ServiceContainer.IsRegistered<IFriendBusinessService>())
                    _friendService = ServiceContainer.Resolve<IFriendBusinessService>();
                if (ServiceContainer.IsRegistered<IGroupBusinessService>())
                    _groupService = ServiceContainer.Resolve<IGroupBusinessService>();

                _useNewService = _friendService != null || _groupService != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[search] 初始化服务失败: {ex.Message}");
                _useNewService = false;
            }
        }

        private async void btn_addgroup_Click(object sender, EventArgs e)
        {
            string groupAccount = txt_group.Text.Trim();
            if (string.IsNullOrEmpty(groupAccount))
            {
                lbl_groupwarn.Text = "请输入群组ID";
                lbl_groupwarn.Visible = true;
                return;
            }

            if (_useNewService && _groupService != null)
            {
                await AddGroupByServiceAsync(groupAccount);
            }
            else
            {
                AddGroupLegacy(groupAccount);
            }
        }

        private async Task AddGroupByServiceAsync(string groupAccount)
        {
            try
            {
                var groups = await _groupService.SearchGroupsAsync(groupAccount);
                if (groups != null && groups.Count > 0)
                {
                    foreach (var group in groups)
                    {
                        bool success = await _groupService.JoinGroupAsync(group.GroupId);
                        if (success)
                        {
                            MessageBox.Show("申请已发送");
                            _parentForm?.Load_all();
                        }
                        else
                        {
                            MessageBox.Show("申请发送失败");
                        }
                    }
                }
                else
                {
                    lbl_groupwarn.Visible = true;
                    lbl_groupwarn.Text = "未找到该群组";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"搜索失败: {ex.Message}");
            }
        }

        private void AddGroupLegacy(string groupAccount)
        {
            var client = GlobalClient.Current;
            if (client == null) return;

            var groups = client.SearchGroups(groupAccount);
            if (groups != null && groups.Count > 0)
            {
                foreach (var group in groups)
                {
                    bool success = client.JoinGroup(group.GroupId);
                    if (success)
                    {
                        MessageBox.Show("申请已发送");
                        _parentForm?.Load_all();
                    }
                    else
                    {
                        MessageBox.Show("申请发送失败");
                    }
                }
            }
            else
            {
                lbl_groupwarn.Visible = true;
                lbl_groupwarn.Text = "未找到该群组";
            }
        }

        private async void btn_addfriend_Click(object sender, EventArgs e)
        {
            string friendAccount = txt_friend.Text.Trim();
            if (string.IsNullOrEmpty(friendAccount))
            {
                MessageBox.Show("账号不可为空");
                return;
            }

            if (_useNewService && _friendService != null)
            {
                await AddFriendByServiceAsync(friendAccount);
            }
            else
            {
                AddFriendLegacy(friendAccount);
            }
        }

        private async Task AddFriendByServiceAsync(string friendAccount)
        {
            try
            {
                bool exists = await _friendService.SearchUserAsync(_selfAccount, friendAccount);
                if (exists)
                {
                    bool success = await _friendService.AddFriendAsync(_selfAccount, friendAccount);
                    if (success)
                    {
                        MessageBox.Show("已发送好友申请");
                        lbl_friendwarn.Visible = false;
                        _parentForm?.RefreshFriendList();
                        _parentForm?.RefreshGroupList();
                    }
                    else
                    {
                        MessageBox.Show("发送好友申请失败");
                    }
                }
                else
                {
                    lbl_friendwarn.Text = "不存在该用户";
                    lbl_friendwarn.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"搜索失败: {ex.Message}");
            }
        }

        private void AddFriendLegacy(string friendAccount)
        {
            var client = GlobalClient.Current;
            if (client == null) return;

            bool exists = client.SearchId(_selfAccount, friendAccount);
            if (exists)
            {
                bool success = client.AddFriend(_selfAccount, friendAccount);
                if (success)
                {
                    MessageBox.Show("已发送好友申请");
                    lbl_friendwarn.Visible = false;
                    _parentForm?.RefreshFriendList();
                    _parentForm?.RefreshGroupList();
                }
                else
                {
                    MessageBox.Show("发送好友申请失败");
                }
            }
            else
            {
                lbl_friendwarn.Text = "不存在该用户";
                lbl_friendwarn.Visible = true;
            }
        }

        private void Load_Panel()
        {
            pnl_addfriend.Top = 0;
            pnl_addfriend.Left = 0;
            pnl_addgroup.Top = 0;
            pnl_addgroup.Left = 0;
            pnl_addgroup.Visible = false;
        }

        private void btn_friendmode_Click(object sender, EventArgs e)
        {
            pnl_addfriend.Visible = true;
            pnl_addgroup.Visible = false;
        }

        private void btn_groupmode_Click(object sender, EventArgs e)
        {
            pnl_addgroup.Visible = true;
            pnl_addfriend.Visible = false;
        }
    }
}