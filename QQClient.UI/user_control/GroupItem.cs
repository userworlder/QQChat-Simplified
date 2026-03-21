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
    public partial class GroupItem : UserControl
    {
        public string GroupId { get; set; }
        public string GroupName
        {
            get => lblGroupName.Text;
            set => lblGroupName.Text = value;
        }
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
                    // 如果消息太长，自动截断（AutoEllipsis 会自动处理）
                    lblLastMessage.Text = value;
                    lblLastMessage.ForeColor = Color.FromArgb(100, 100, 100);
                }
            }
        }
        public string Time
        {
            get => lblTime.Text;
            set => lblTime.Text = value;
        }
        private int _unreadCount;
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

        public GroupItem()
        {
            InitializeComponent();
            // 设置控件样式
            this.BackColor = Color.White;
            this.BorderStyle = BorderStyle.FixedSingle;

            // 设置鼠标悬停效果
            this.MouseEnter += (s, e) => this.BackColor = Color.FromArgb(240, 240, 240);
            this.MouseLeave += (s, e) => this.BackColor = Color.White;

            // 确保所有子控件也能触发点击事件
            foreach (Control ctrl in this.Controls)
            {
                ctrl.Click += (s, e) => this.OnClick(e);
                ctrl.MouseEnter += (s, e) => this.OnMouseEnter(e);
                ctrl.MouseLeave += (s, e) => this.OnMouseLeave(e);
            }
        }

        private void lblLastMessage_Click(object sender, EventArgs e)
        {

        }
    }
}
