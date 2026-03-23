using QQClient.Business;
using QQClient.Business.Services;
using QQCommon.Models;
using System;
using System.Windows.Forms;

namespace QQClient.UI
{
    public partial class register_new : Form
    {
        #region 字段声明

        private IUserBusinessService _userService;      // 用户业务服务（新架构）
        private IFriendBusinessService _friendService;  // 好友业务服务（用于检查账号是否存在）
        private Form _loginForm;                         // 登录窗体引用，关闭时显示登录界面

        #endregion

        #region 构造函数与初始化

        public register_new(Form login)
        {
            InitializeComponent();
            _loginForm = login;
            // 当注册窗体关闭时，重新显示登录窗体
            this.FormClosed += (sender, e) => login.Show();

            // 从服务容器中获取已注册的业务服务
            if (ServiceContainer.IsRegistered<IUserBusinessService>())
            {
                _userService = ServiceContainer.Resolve<IUserBusinessService>();
            }
            if (ServiceContainer.IsRegistered<IFriendBusinessService>())
            {
                _friendService = ServiceContainer.Resolve<IFriendBusinessService>();
            }
        }

        #endregion

        #region UI事件处理

        /// <summary>
        /// “清空”按钮：清空所有输入框
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";   // 昵称
            textBox2.Text = "";   // 账号
            textBox3.Text = "";   // 密码
        }

        /// <summary>
        /// “注册”按钮：执行注册流程
        /// </summary>
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

            // 如果昵称为空，则默认使用账号作为昵称
            if (string.IsNullOrEmpty(nickname))
            {
                nickname = account;
                textBox1.Text = account;
            }

            try
            {
                // 1. 检查账号是否已存在（通过好友搜索服务）
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

                // 2. 如果用户服务不可用，降级到旧版注册
                if (_userService == null)
                {
                    LegacyRegister(account, password, nickname);
                    return;
                }

                // 3. 使用新版服务注册
                bool success = await _userService.RegisterAsync(account, password, nickname);
                if (success)
                {
                    MessageBox.Show("注册成功");
                    this.Close();   // 关闭注册窗体，自动显示登录窗体
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

        #endregion

        #region 降级注册（旧版客户端）

        /// <summary>
        /// 旧版注册方法（使用 GlobalClient）
        /// </summary>
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

        #endregion
    }
}