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
    public partial class FriendItem : UserControl
    {
        public string FromUserId { get; set; }
        public event EventHandler<string> AcceptClicked;
        public event EventHandler<string> RejectClicked;
        public FriendItem(string fromUserId)
        {
            InitializeComponent();
            label1.Text = fromUserId;
            //订阅点击事件
            btn_accept.Click += btn_accept_Click;
            btn_reject.Click += btn_reject_Click;
        }

        private void btn_accept_Click(object sender, EventArgs e)
        {
            //触发点击事件后触发自定义事件AcceptClicked，其对应事件在对应窗体中编写
            // 非空则执行invoke
            AcceptClicked?.Invoke(this, FromUserId);
        }

        private void btn_reject_Click(object sender, EventArgs e)
        {
            RejectClicked?.Invoke(this, FromUserId);
        }
    }
}
