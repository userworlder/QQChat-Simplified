using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QQClient.UI
{
    /// <summary>
    /// 好友请求条目控件
    /// 用于在主窗体的“验证消息”面板中显示好友申请，提供接受/拒绝按钮
    /// </summary>
    public partial class FriendItem : UserControl
    {
        #region 公共属性与事件

        /// <summary>
        /// 发起好友申请的用户ID
        /// </summary>
        public string FromUserId { get; set; }

        /// <summary>
        /// 接受按钮点击事件，参数为发起者ID
        /// </summary>
        public event EventHandler<string> AcceptClicked;

        /// <summary>
        /// 拒绝按钮点击事件，参数为发起者ID
        /// </summary>
        public event EventHandler<string> RejectClicked;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数，传入发起申请的用户ID，显示在标签上
        /// </summary>
        /// <param name="fromUserId">申请发送方账号</param>
        public FriendItem(string fromUserId)
        {
            InitializeComponent();
            label1.Text = fromUserId;      // 在标签上显示申请者账号
            this.FromUserId = fromUserId;  // 保存ID供事件传递
        }

        #endregion

        #region 按钮点击处理

        /// <summary>
        /// 接受按钮点击：触发 AcceptClicked 事件
        /// </summary>
        private void btn_accept_Click(object sender, EventArgs e)
        {
            // 触发自定义事件，通知父窗体（如 user 窗体）执行接受好友申请的逻辑
            AcceptClicked?.Invoke(this, FromUserId);
        }

        /// <summary>
        /// 拒绝按钮点击：触发 RejectClicked 事件
        /// </summary>
        private void btn_reject_Click(object sender, EventArgs e)
        {
            RejectClicked?.Invoke(this, FromUserId);
        }

        #endregion
    }
}