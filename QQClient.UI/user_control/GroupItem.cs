using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QQClient.UI.user_control
{
    /// <summary>
    /// 群组列表项控件
    /// 用于在主窗体的群聊列表中显示每个群的信息（群名、最后消息、时间、未读数）
    /// </summary>
    public partial class GroupItem : UserControl
    {
        #region 公共属性

        /// <summary>
        /// 群组ID（唯一标识）
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// 群组名称
        /// </summary>
        public string GroupName
        {
            get => lblGroupName.Text;
            set => lblGroupName.Text = value;
        }

        /// <summary>
        /// 最后一条消息内容
        /// 为空时显示“暂无消息”，否则显示消息内容（超长自动截断）
        /// </summary>
        public string LastMessage
        {
            get => lblLastMessage.Text;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    lblLastMessage.Text = "暂无消息";
                    lblLastMessage.ForeColor = Color.Gray;
                }
                else
                {
                    // AutoEllipsis 属性已启用，文本过长时会自动显示省略号
                    lblLastMessage.Text = value;
                    lblLastMessage.ForeColor = Color.FromArgb(100, 100, 100);
                }
            }
        }

        /// <summary>
        /// 最后消息的时间（格式如 HH:mm）
        /// </summary>
        public string Time
        {
            get => lblTime.Text;
            set => lblTime.Text = value;
        }

        private int _unreadCount;

        /// <summary>
        /// 未读消息数量
        /// 大于0时显示红色角标，超过99显示"99+"
        /// </summary>
        public int UnreadCount
        {
            get => _unreadCount;
            set
            {
                _unreadCount = value;
                lblUnread.Visible = value > 0;
                if (value > 0)
                {
                    lblUnread.Text = value > 99 ? "99+" : value.ToString();
                }
            }
        }

        #endregion

        #region 构造函数与初始化

        public GroupItem()
        {
            InitializeComponent();

            // 设置默认样式
            this.BackColor = Color.White;
            this.BorderStyle = BorderStyle.FixedSingle;

            // 鼠标悬停效果：改变背景色
            this.MouseEnter += (s, e) => this.BackColor = Color.FromArgb(240, 240, 240);
            this.MouseLeave += (s, e) => this.BackColor = Color.White;

            // 为了让整个控件区域都能响应点击，将子控件的点击事件转发给控件本身
            foreach (Control ctrl in this.Controls)
            {
                ctrl.Click += (s, e) => this.OnClick(e);
                ctrl.MouseEnter += (s, e) => this.OnMouseEnter(e);
                ctrl.MouseLeave += (s, e) => this.OnMouseLeave(e);
            }
        }

        #endregion

        #region 内部事件处理

        // 保留此方法以兼容设计器生成的代码，实际无逻辑
        private void lblLastMessage_Click(object sender, EventArgs e)
        {
            // 暂无需实现，可由外部统一处理
        }

        #endregion
    }
}