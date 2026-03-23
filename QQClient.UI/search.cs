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
        #region 字段声明

        private user _parentForm;               // 主窗体引用，用于刷新界面
        private string _selfAccount;            // 当前登录用户账号

        // 业务服务接口（新架构）
        private IFriendBusinessService _friendService;
        private IGroupBusinessService _groupService;
        private bool _useNewService = false;    // 标记是否使用新架构服务

        #endregion

        #region 构造函数与初始化

        public search(string selfAccount, user parentForm)
        {
            InitializeComponent();
            Load_Panel();                       // 初始化面板位置
            _selfAccount = selfAccount;
            _parentForm = parentForm;

            // 从服务容器获取服务实例
            InitializeServices();
        }

        /// <summary>
        /// 从全局服务容器中获取好友和群组业务服务
        /// </summary>
        private void InitializeServices()
        {
            try
            {
                // 检查并获取已注册的服务
                if (ServiceContainer.IsRegistered<IFriendBusinessService>())
                    _friendService = ServiceContainer.Resolve<IFriendBusinessService>();
                if (ServiceContainer.IsRegistered<IGroupBusinessService>())
                    _groupService = ServiceContainer.Resolve<IGroupBusinessService>();

                // 只要任一服务可用，就使用新架构
                _useNewService = _friendService != null || _groupService != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[search] 初始化服务失败: {ex.Message}");
                _useNewService = false;          // 降级到旧架构
            }
        }

        #endregion

        #region 群组操作（加入群组）

        /// <summary>
        /// “加入群组”按钮点击事件
        /// </summary>
        private async void btn_addgroup_Click(object sender, EventArgs e)
        {
            string groupAccount = txt_group.Text.Trim();
            if (string.IsNullOrEmpty(groupAccount))
            {
                lbl_groupwarn.Text = "请输入群组ID";
                lbl_groupwarn.Visible = true;
                return;
            }

            // 根据是否使用新架构选择不同的实现
            if (_useNewService && _groupService != null)
            {
                await AddGroupByServiceAsync(groupAccount);
            }
            else
            {
                AddGroupLegacy(groupAccount);
            }
        }

        /// <summary>
        /// 使用新版服务加入群组
        /// </summary>
        private async Task AddGroupByServiceAsync(string groupAccount)
        {
            try
            {
                // 1. 搜索群组（支持模糊匹配）
                var groups = await _groupService.SearchGroupsAsync(groupAccount);
                if (groups != null && groups.Count > 0)
                {
                    foreach (var group in groups)
                    {
                        // 2. 对每个搜索结果发送加群申请
                        bool success = await _groupService.JoinGroupAsync(group.GroupId);
                        if (success)
                        {
                            MessageBox.Show("申请已发送");
                            _parentForm?.Load_all();    // 刷新主窗体的列表
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

        /// <summary>
        /// 使用旧版客户端加入群组（降级方案）
        /// </summary>
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

        #endregion

        #region 好友操作（添加好友）

        /// <summary>
        /// “添加好友”按钮点击事件
        /// </summary>
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

        /// <summary>
        /// 使用新版服务添加好友
        /// </summary>
        private async Task AddFriendByServiceAsync(string friendAccount)
        {
            try
            {
                // 1. 检查目标用户是否存在
                bool exists = await _friendService.SearchUserAsync(_selfAccount, friendAccount);
                if (exists)
                {
                    // 2. 发送好友申请
                    bool success = await _friendService.AddFriendAsync(_selfAccount, friendAccount);
                    if (success)
                    {
                        MessageBox.Show("已发送好友申请");
                        lbl_friendwarn.Visible = false;
                        // 刷新主窗体的好友列表和群列表（可能影响验证消息）
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

        /// <summary>
        /// 使用旧版客户端添加好友（降级方案）
        /// </summary>
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

        #endregion

        #region UI面板切换

        /// <summary>
        /// 初始化两个面板的位置，默认显示“添加好友”面板
        /// </summary>
        private void Load_Panel()
        {
            // 将两个面板重叠放置，通过 Visible 控制显示哪个
            pnl_addfriend.Top = 0;
            pnl_addfriend.Left = 0;
            pnl_addgroup.Top = 0;
            pnl_addgroup.Left = 0;
            pnl_addgroup.Visible = false;   // 默认隐藏“加群”面板
        }

        /// <summary>
        /// 切换到“找人”模式（显示添加好友面板）
        /// </summary>
        private void btn_friendmode_Click(object sender, EventArgs e)
        {
            pnl_addfriend.Visible = true;
            pnl_addgroup.Visible = false;
        }

        /// <summary>
        /// 切换到“找群”模式（显示添加群组面板）
        /// </summary>
        private void btn_groupmode_Click(object sender, EventArgs e)
        {
            pnl_addgroup.Visible = true;
            pnl_addfriend.Visible = false;
        }

        #endregion
    }
}