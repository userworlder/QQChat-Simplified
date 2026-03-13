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
    public partial class message_bubble : UserControl
    {
        public message_bubble()
        {
            InitializeComponent();
        }
        public string MessageText
        {
            get => lblMessage.Text;
            set => lblMessage.Text = value;
        }

        // 是否为自己发送的消息
        private bool _isSelf;
        public bool IsSelf
        {
            get => _isSelf;
            set
            {
                _isSelf = value;
                if (value)
                {
                    panel1.BackColor = Color.LightGreen;   // 自己消息背景色
                    panel1.Dock = DockStyle.Right;         // 靠右
                }
                else
                {
                    panel1.BackColor = Color.LightGray;    // 对方消息背景色
                    panel1.Dock = DockStyle.Left;          // 靠左
                }
            }
        }
    }
}
