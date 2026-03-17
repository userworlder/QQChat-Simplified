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
            this.Resize += (s, e) => UpdatePosition();
            panelBubble.Resize += (s, e) => UpdatePosition(); // 以防 panel 大小变化
                                                              // 调试：输出控件信息
            lblMessage.TextChanged += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine($"lblMessage 高度: {lblMessage.Height}, 文本长度: {lblMessage.Text.Length}");
            };
           // MessageBox.Show($"lblMessage.MaximumSize = {lblMessage.MaximumSize}");
        }
        //检测是否是自己来确定出现的方位/颜色
        private void UpdatePosition()
        {
            if (_isSelf)
            {
                panelBubble.BackColor = Color.LightGreen;
                panelBubble.Dock = DockStyle.Right;
                // panelBubble.Left = this.ClientSize.Width - panelBubble.Width;
            }
            else
            {
                panelBubble.BackColor = Color.LightGray;
                panelBubble.Dock = DockStyle.Left;
                //panelBubble.Left = 0;
            }
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
                   // panelBubble.Left = this.ClientSize.Width - panelBubble.Width;
                    panelBubble.Dock = DockStyle.Right;         // 靠右
                }
                else
                {
                    panelBubble.BackColor = Color.LightGray;    // 对方消息背景色
                  //  panelBubble.Left = 0;
                    panelBubble.Dock = DockStyle.Left;          // 靠左
                }
            }
        }
    }
}


