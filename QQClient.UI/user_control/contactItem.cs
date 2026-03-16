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
        public string Nickname { get; set; }
        public ContactItem()
        {
            InitializeComponent();           
            //this.Click += contactItem_Click;
            //所有的控件也会被视为点击
            //foreach (Control ctrl in this.Controls)
            //{
            //    ctrl.Click += contactItem_Click;
            //}
        }
        //点击好友时触发的事件
        private void contactItem_Click(object sender, EventArgs e)
        {
            // 1. 获取被点击的 ContactItem 对象
            ContactItem clickedItem = sender as ContactItem;
            if (clickedItem == null) return;

            // 2. 获取联系人信息
            string account=clickedItem.Account;
            string displayname = clickedItem.DisplayName;  // 联系人昵称

            // 3. 创建聊天窗口实例
            chat_new chat = new chat_new(account,displayname);

            // 4. 显示聊天窗口（允许同时打开多个聊天窗口）
            chat.Show();

           
        }
    }
}
