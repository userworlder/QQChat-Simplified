using QQClient.Business;
using QQClient.Business.Services;
using QQCommon.Models;
using System;
using System.Windows.Forms;

namespace QQClient.UI
{
    public partial class login_new : Form
    {
        #region 字段声明

        private IUserBusinessService _userService;   // 用户业务服务（新架构）

        #endregion

        #region 构造函数

        public login_new()
        {
            InitializeComponent();

            // 从服务容器中获取用户业务服务（如果已注册）
            if (ServiceContainer.IsRegistered<IUserBusinessService>())
            {
                _userService = ServiceContainer.Resolve<IUserBusinessService>();
            }
        }

        #endregion

        #region 测试按钮（快速填充账号密码）

        /// <summary>
        /// 测试按钮1：填充账号为"1"，密码为"1"
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "1";
            textBox2.Text = "1";
        }

        /// <summary>
        /// 测试按钮2：填充账号为"wxb"，密码为"wxb"
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "wxb";
            textBox2.Text = "wxb";
        }

        #endregion

        #region 登录逻辑（新版服务优先）

        /// <summary>
        /// 登录按钮点击事件（图片框）
        /// </summary>
        private async void pictureBox2_Click(object sender, EventArgs e)
        {
            // 如果新版服务未初始化，则降级使用旧版客户端
            if (_userService == null)
            {
                MessageBox.Show("服务未初始化，请使用旧版登录");
                LegacyLogin();
                return;
            }

            // 验证账号和密码是否为空
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("账户或密码不可为空");
                return;
            }

            string username = textBox1.Text;
            string password = textBox2.Text;

            try
            {
                // 调用新版服务进行登录
                bool success = await _userService.LoginAsync(username, password);

                if (success)
                {
                    // 登录成功，清除旧缓存
                    CurrentUser.Clear();

                    // 设置当前用户信息（全局静态）
                    CurrentUser.UserId = username;
                    CurrentUser.Username = username;

                    // 更新消息服务和群组服务的当前用户ID（以便它们能正确发送和接收消息）
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

                    // 隐藏登录窗体，打开主窗体
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

        #endregion

        #region 旧版登录（降级方案）

        /// <summary>
        /// 使用旧版 GlobalClient 进行登录（当新服务不可用时）
        /// </summary>
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

            bool success = client.Login(username, password);
            if (success)
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

        #endregion

        #region 注册链接

        /// <summary>
        /// 点击注册标签，打开注册窗体
        /// </summary>
        private void label1_Click(object sender, EventArgs e)
        {
            this.Hide();
            register_new register = new register_new(this);
            register.ShowDialog();   // 注册窗体关闭后，登录窗体自动显示（注册窗体构造函数中已处理 FormClosed 事件）
        }

        #endregion
    }
}