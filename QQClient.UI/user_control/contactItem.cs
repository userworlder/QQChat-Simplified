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
    public partial class ContactItem : UserControl
    {
        public string DisplayName
        {
            get { return label_name.Text; }
            set { label_name.Text = value; }
        }
        public string LastMessage
        {
            get { return label_lstchat.Text; }
            set { label_lstchat.Text = value; }
        }
        public string Time
        {
            get { return label_time.Text; }
            set { label_time.Text = value; }
        }
        public string Account { get; set; }
        private int _unreadCount;  // 私有字段

        public int UnreadCount
        {
            get { return _unreadCount; }
            set
            {
                _unreadCount = value;
                // 更新 label_unread 的可见性和文本
                if (_unreadCount > 0)
                {
                    label_unread.Text = _unreadCount.ToString();
                    label_unread.Visible = true;
                }
                else
                {
                    label_unread.Visible = false;
                }
            }
        }
        public ContactItem()
        {
            InitializeComponent();           
        }
        
    }
}
