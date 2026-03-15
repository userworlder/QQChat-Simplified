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

            // 当气泡面板大小变化时，调整整个控件的高度
            this.panelBubble.Resize += (s, e) => this.Height = panelBubble.Height;
            this.Height = 50; // 临时强制高度，用于测试
        }

        // 消息文本属性
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
                    panelBubble.BackColor = Color.LightGreen;   // 自己消息背景色
                   // panelBubble.Dock = DockStyle.Right;         // 靠右
                }
                else
                {
                    panelBubble.BackColor = Color.LightGray;    // 对方消息背景色
                   // panelBubble.Dock = DockStyle.Left;          // 靠左
                }
            }
        }
    }
}


