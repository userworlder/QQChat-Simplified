using QQClient.Business;
using QQClient.Business.Services;
using QQCommon.Models;
using System;
using System.Windows.Forms;

namespace QQClient.UI
{
    public partial class login_new : Form
    {
        private IUserBusinessService _userService;

        public login_new()
        {
            InitializeComponent();

            // 从服务容器获取用户服务
            if (ServiceContainer.IsRegistered<IUserBusinessService>())
            {
                _userService = ServiceContainer.Resolve<IUserBusinessService>();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "1";
            textBox2.Text = "1";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "wxb";
            textBox2.Text = "wxb";
        }

        // 登录键
        private async void pictureBox2_Click(object sender, EventArgs e)
        {
            // 检查服务是否可用
            if (_userService == null)
            {
                MessageBox.Show("服务未初始化，请使用旧版登录");
                // 降级到旧版登录
                LegacyLogin();
                return;
            }

            // 验证输入
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("账户或密码不可为空");
                return;
            }

            string username = textBox1.Text;
            string password = textBox2.Text;

            try
            {
                // 使用新版服务登录
                bool success = await _userService.LoginAsync(username, password);

                if (success)
                {
                    // 清除之前的缓存
                    CurrentUser.Clear();

                    // 设置当前用户
                    CurrentUser.UserId = username;
                    CurrentUser.Username = username;

                    // 更新消息服务和群组服务的当前用户ID
                    if (ServiceContainer.IsRegistered<IMessageBusinessService>())
                    {
                        var messageService = ServiceContainer.Resolve<IMessageBusinessService>();
                        if (messageService is MessageBusinessService msgService)
                        {
                            msgService.SetCurrentUserId(username);
                        }
                    }

                    if (ServiceContainer.IsRegistered<IGroupBusinessService>())
                    {
                        var groupService = ServiceContainer.Resolve<IGroupBusinessService>();
                        if (groupService is GroupBusinessService grpService)
                        {
                            grpService.SetCurrentUserId(username);
                        }
                    }

                    // 打开主界面
                    this.Hide();
                    user mainForm = new user(username, this);
                    mainForm.Show();
                }
                else
                {
                    MessageBox.Show("不存在该用户，请检查账号或密码");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"登录失败: {ex.Message}");
            }
        }

        // 旧版登录（降级方案）
        private void LegacyLogin()
        {
            var client = GlobalClient.Current;
            if (client == null)
            {
                MessageBox.Show("网络客户端未初始化");
                return;
            }

            string username = textBox1.Text;
            string password = textBox2.Text;

            bool x = client.Login(username, password);
            if (x)
            {
                this.Hide();
                GlobalClient.CurrentUserId = username;
                user userForm = new user(username, this);
                userForm.Show();
            }
            else
            {
                MessageBox.Show("不存在该用户，请检查账号或密码");
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.Hide();
            register_new register = new register_new(this);
            register.ShowDialog();
        }
    }
}