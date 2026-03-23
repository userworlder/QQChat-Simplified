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
    /// 好友列表项控件
    /// 用于在主窗体的私聊列表中显示每个好友的信息（昵称、最后消息、时间、未读数）
    /// </summary>
    public partial class ContactItem : UserControl
    {
        #region 字段声明

        private int _unreadCount;  // 未读消息数量（私有字段，由属性访问）

        #endregion

        #region 公共属性

        /// <summary>
        /// 好友的显示名称（通常是昵称，如无则用账号）
        /// </summary>
        public string DisplayName
        {
            get { return label_name.Text; }
            set { label_name.Text = value; }
        }

        /// <summary>
        /// 最后一条聊天消息的内容（会显示在列表中）
        /// </summary>
        public string LastMessage
        {
            get { return label_lstchat.Text; }
            set { label_lstchat.Text = value; }
        }

        /// <summary>
        /// 最后消息的时间（格式如 HH:mm）
        /// </summary>
        public string Time
        {
            get { return label_time.Text; }
            set { label_time.Text = value; }
        }

        /// <summary>
        /// 好友的账号（唯一标识，用于打开聊天窗口等操作）
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 未读消息数量
        /// 大于0时显示红色角标并显示数字，等于0时隐藏角标
        /// </summary>
        public int UnreadCount
        {
            get { return _unreadCount; }
            set
            {
                _unreadCount = value;
                if (_unreadCount > 0)
                {
                    // 有未读：显示红色角标，并设置数字（最大显示99+由调用方处理）
                    label_unread.Text = _unreadCount.ToString();
                    label_unread.Visible = true;
                }
                else
                {
                    // 无未读：隐藏角标
                    label_unread.Visible = false;
                }
            }
        }

        #endregion

        #region 构造函数

        public ContactItem()
        {
            InitializeComponent();
            // 其他初始化（如果需要）可在此处添加
        }

        #endregion
    }
}