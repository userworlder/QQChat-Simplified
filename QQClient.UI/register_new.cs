using QQClient.Business;
using QQClient.Business.Services;
using QQCommon.Models;
using System;
using System.Windows.Forms;

namespace QQClient.UI
{
    public partial class register_new : Form
    {
        private IUserBusinessService _userService;
        private IFriendBusinessService _friendService;
        private Form _loginForm;

        public register_new(Form login)
        {
            InitializeComponent();
            _loginForm = login;
            this.FormClosed += (sender, e) => login.Show();

            // 从服务容器获取服务
            if (ServiceContainer.IsRegistered<IUserBusinessService>())
            {
                _userService = ServiceContainer.Resolve<IUserBusinessService>();
            }
            if (ServiceContainer.IsRegistered<IFriendBusinessService>())
            {
                _friendService = ServiceContainer.Resolve<IFriendBusinessService>();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string nickname = textBox1.Text;
            string account = textBox2.Text;
            string password = textBox3.Text;

            // 输入验证
            if (string.IsNullOrEmpty(account))
            {
                label_warn.Text = "账号不可为空";
                label_warn.Visible = true;
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                label_warn.Text = "密码不可为空";
                label_warn.Visible = true;
                return;
            }

            // 设置昵称（如果为空则使用账号）
            if (string.IsNullOrEmpty(nickname))
            {
                nickname = account;
                textBox1.Text = account;
            }

            try
            {
                // 使用新版服务检查账号是否已存在
                bool exists = false;
                if (_friendService != null)
                {
                    exists = await _friendService.SearchUserAsync(account, account);
                }

                if (exists)
                {
                    label_warn.Text = "已存在该账号，请尝试新的账号";
                    label_warn.Visible = true;
                    return;
                }

                // 使用新版服务注册
                if (_userService == null)
                {
                    // 降级到旧版注册
                    LegacyRegister(account, password, nickname);
                    return;
                }

                bool success = await _userService.RegisterAsync(account, password, nickname);
                if (success)
                {
                    MessageBox.Show("注册成功");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("注册失败");
                    label_warn.Text = "请尝试其他账号密码";
                    label_warn.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"注册失败: {ex.Message}");
                label_warn.Text = "注册失败，请重试";
                label_warn.Visible = true;
            }
        }

        // 旧版注册（降级方案）
        private void LegacyRegister(string account, string password, string nickname)
        {
            var client = GlobalClient.Current;
            if (client == null)
            {
                MessageBox.Show("网络客户端未初始化");
                return;
            }

            bool x = client.Register(account, password, nickname);
            if (x)
            {
                MessageBox.Show("注册成功");
                this.Close();
            }
            else
            {
                MessageBox.Show("注册失败");
                label_warn.Text = "请尝试其他账号密码";
                label_warn.Visible = true;
            }
        }
    }
}