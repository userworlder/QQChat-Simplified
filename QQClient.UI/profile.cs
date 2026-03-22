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
        private string _selfAccount;
        private string _friendAccount;
        private User _user;
        private User _newUser = new User();

        // 业务服务
        private IUserBusinessService _userService;
        private bool _useNewService = false;

        public profile(string selfAccount, string friendAccount)
        {
            InitializeComponent();
            _selfAccount = selfAccount;
            _friendAccount = friendAccount;

            // 初始化服务
            InitializeServices();

            ReadOnlyMode();

            // 加载用户信息
            LoadUserInfo();

            // 如果是查看他人资料，隐藏修改按钮和密码
            if (_selfAccount != _friendAccount)
            {
                lbl_update.Visible = false;
                label4.Visible = false;
                textBox4.Visible = false;
            }
        }

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
                LoadUserInfoLegacy();
            }
        }

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

        private void DisplayUserInfo(User user)
        {
            textBox1.Text = user.Username;
            textBox2.Text = user.Nickname;
            textBox3.Text = user.Signature;
            textBox4.Text = user.Password;
        }

        private void ReadOnlyMode()
        {
            textBox1.ReadOnly = true;
            textBox2.ReadOnly = true;
            textBox3.ReadOnly = true;
            textBox4.ReadOnly = true;
            btn_accept.Visible = false;
            btn_cancel.Visible = false;
        }

        private void EditMode()
        {
            btn_accept.Visible = true;
            btn_cancel.Visible = true;
            textBox2.ReadOnly = false;
            textBox3.ReadOnly = false;
            textBox4.ReadOnly = false;
        }

        private async void btn_accept_Click(object sender, EventArgs e)
        {
            string newNickname = textBox2.Text;
            string newSignature = textBox3.Text;
            string newPassword = textBox4.Text;

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

        private async Task UpdateUserInfoByServiceAsync(string newNickname)
        {
            try
            {
                bool success = await _userService.UpdateUserInfoAsync(_newUser);
                if (success)
                {
                    MessageBox.Show("修改成功");
                    ReadOnlyMode();
                    // 更新当前用户昵称
                    CurrentUser.Nickname = newNickname;
                    // 同时更新 _user 对象
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

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            // 恢复数据
            if (_user != null)
            {
                textBox2.Text = _user.Nickname;
                textBox3.Text = _user.Signature;
                textBox4.Text = _user.Password;
            }
            ReadOnlyMode();
        }

        private void lbl_update_Click(object sender, EventArgs e)
        {
            EditMode();
            // 保存原始数据
            if (_user != null)
            {
                _user.Nickname = textBox2.Text;
                _user.Signature = textBox3.Text;
                _user.Password = textBox4.Text;
            }
        }
    }
}