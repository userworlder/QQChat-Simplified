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
            set => lblLastMessage.Text = value;
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
                lblUnread.Text = value.ToString();
            }
        }

        public GroupItem()
        {
            InitializeComponent();
        }
    }
}
