using QQClient.Business;
using QQClient.Business.Services;
using QQCommon.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QQClient.UI
{
    public partial class profile : Form
    {
        #region 字段声明

        private string _selfAccount;           // 当前登录用户的账号
        private string _friendAccount;          // 正在查看的用户账号（可能是自己或他人）
        private User _user;                     // 从服务器获取的原始用户信息（用于取消修改时恢复）
        private User _newUser = new User();     // 修改后的用户信息（用于提交更新）

        // 业务服务
        private IUserBusinessService _userService;
        private bool _useNewService = false;    // 标记是否使用新架构服务

        #endregion

        #region 构造函数与初始化

        public profile(string selfAccount, string friendAccount)
        {
            InitializeComponent();
            _selfAccount = selfAccount;
            _friendAccount = friendAccount;

            // 初始化业务服务
            InitializeServices();

            // 默认进入只读模式
            ReadOnlyMode();

            // 加载用户信息（异步，但UI会等待）
            LoadUserInfo();

            // 如果是查看他人资料，隐藏修改按钮和密码字段
            if (_selfAccount != _friendAccount)
            {
                lbl_update.Visible = false;   // 编辑资料按钮
                label4.Visible = false;       // 密码标签
                textBox4.Visible = false;     // 密码输入框
            }
        }

        /// <summary>
        /// 从服务容器获取用户业务服务
        /// </summary>
        private void InitializeServices()
        {
            try
            {
                if (ServiceContainer.IsRegistered<IUserBusinessService>())
                {
                    _userService = ServiceContainer.Resolve<IUserBusinessService>();
                    _useNewService = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[profile] 初始化服务失败: {ex.Message}");
                _useNewService = false;
            }
        }

        #endregion

        #region 加载用户信息（新旧架构）

        /// <summary>
        /// 异步加载用户信息（根据是否使用新架构选择不同方式）
        /// </summary>
        private async void LoadUserInfo()
        {
            if (_useNewService && _userService != null)
            {
                await LoadUserInfoByServiceAsync();
            }
            else
            {
                LoadUserInfoLegacy();
            }
        }

        /// <summary>
        /// 使用新版服务从服务器获取用户信息
        /// </summary>
        private async Task LoadUserInfoByServiceAsync()
        {
            try
            {
                _user = await _userService.GetUserInfoAsync(_friendAccount);
                if (_user != null)
                {
                    DisplayUserInfo(_user);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoadUserInfoByServiceAsync] 异常: {ex.Message}");
                // 降级到旧版
                LoadUserInfoLegacy();
            }
        }

        /// <summary>
        /// 使用旧版客户端获取用户信息（降级方案）
        /// </summary>
        private void LoadUserInfoLegacy()
        {
            var client = GlobalClient.Current;
            if (client == null) return;

            _user = client.GetUserInfo(_friendAccount);
            if (_user != null)
            {
                DisplayUserInfo(_user);
            }
        }

        #endregion

        #region 显示用户信息

        /// <summary>
        /// 将用户对象的数据显示到界面控件
        /// </summary>
        private void DisplayUserInfo(User user)
        {
            textBox1.Text = user.Username;      // 账号（只读）
            textBox2.Text = user.Nickname;      // 昵称
            textBox3.Text = user.Signature;     // 个性签名
            textBox4.Text = user.Password;      // 密码（查看他人时隐藏）
        }

        #endregion

        #region 模式切换（只读/编辑）

        /// <summary>
        /// 设置为只读模式（所有文本框不可编辑，确认/取消按钮隐藏）
        /// </summary>
        private void ReadOnlyMode()
        {
            textBox1.ReadOnly = true;
            textBox2.ReadOnly = true;
            textBox3.ReadOnly = true;
            textBox4.ReadOnly = true;
            btn_accept.Visible = false;
            btn_cancel.Visible = false;
        }

        /// <summary>
        /// 设置为编辑模式（昵称、签名、密码可编辑，显示确认/取消按钮）
        /// </summary>
        private void EditMode()
        {
            btn_accept.Visible = true;
            btn_cancel.Visible = true;
            textBox2.ReadOnly = false;
            textBox3.ReadOnly = false;
            textBox4.ReadOnly = false;
        }

        #endregion

        #region 保存修改（新旧架构）

        /// <summary>
        /// “确认”按钮点击事件：提交修改
        /// </summary>
        private async void btn_accept_Click(object sender, EventArgs e)
        {
            string newNickname = textBox2.Text;
            string newSignature = textBox3.Text;
            string newPassword = textBox4.Text;

            // 将用户输入的修改填充到 _newUser 对象
            _newUser.Username = _selfAccount;
            _newUser.Password = newPassword;
            _newUser.Nickname = newNickname;
            _newUser.Signature = newSignature;

            if (_useNewService && _userService != null)
            {
                await UpdateUserInfoByServiceAsync(newNickname);
            }
            else
            {
                UpdateUserInfoLegacy();
            }
        }

        /// <summary>
        /// 使用新版服务更新用户信息
        /// </summary>
        private async Task UpdateUserInfoByServiceAsync(string newNickname)
        {
            try
            {
                bool success = await _userService.UpdateUserInfoAsync(_newUser);
                if (success)
                {
                    MessageBox.Show("修改成功");
                    ReadOnlyMode();
                    // 更新全局当前用户的昵称
                    CurrentUser.Nickname = newNickname;
                    // 同时更新本地缓存的 _user 对象，以便取消修改时恢复
                    if (_user != null)
                    {
                        _user.Nickname = newNickname;
                        _user.Signature = textBox3.Text;
                        _user.Password = textBox4.Text;
                    }
                }
                else
                {
                    MessageBox.Show("修改失败");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"修改失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 使用旧版客户端更新用户信息（降级方案）
        /// </summary>
        private void UpdateUserInfoLegacy()
        {
            var client = GlobalClient.Current;
            if (client == null)
            {
                MessageBox.Show("网络客户端未初始化");
                return;
            }

            bool success = client.UpdateUserInfo(_newUser);
            if (success)
            {
                MessageBox.Show("修改成功");
                ReadOnlyMode();
            }
            else
            {
                MessageBox.Show("修改失败");
            }
        }

        #endregion

        #region 取消修改

        /// <summary>
        /// “取消”按钮点击事件：放弃修改，恢复原始数据
        /// </summary>
        private void btn_cancel_Click(object sender, EventArgs e)
        {
            // 恢复原始数据
            if (_user != null)
            {
                textBox2.Text = _user.Nickname;
                textBox3.Text = _user.Signature;
                textBox4.Text = _user.Password;
            }
            ReadOnlyMode();
        }

        #endregion

        #region 编辑资料

        /// <summary>
        /// “编辑资料”标签点击：进入编辑模式，并保存当前数据副本（用于取消）
        /// </summary>
        private void lbl_update_Click(object sender, EventArgs e)
        {
            EditMode();
            // 备份当前显示的数据到 _user（以便取消时恢复）
            if (_user != null)
            {
                _user.Nickname = textBox2.Text;
                _user.Signature = textBox3.Text;
                _user.Password = textBox4.Text;
            }
        }

        #endregion
    }
}