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

            // 确保控件初始时不自动调整宽度（宽度由父容器决定），高度手动控制
            this.AutoSize = false;
            this.Height = panelBubble.Height;   // 初始高度设为气泡高度

            // 监听 panelBubble 大小变化，调整整个控件的高度
            panelBubble.Resize += (s, e) => UpdateHeight();

            // 监听文本变化（通过属性设置）或父容器大小变化
            this.Resize += (s, e) => UpdateBubblePosition();
            
        }
        //检测是否是自己来确定出现的方位/颜色
        // 更新控件高度为 panelBubble 的高度
        private void UpdateHeight()
        {
            if (this.Height != panelBubble.Height)
            {
                this.Height = panelBubble.Height;
            }
        }

        // 根据是否自己消息，更新气泡的位置（靠左或靠右）
        private void UpdateBubblePosition()
        {
            // panelBubble 的 Left 需要根据自身宽度和父容器宽度计算
            int newLeft;
            if (_isSelf)
            {
                // 自己消息：靠右，留出右边距（可根据需要调整）
                newLeft = this.ClientSize.Width - panelBubble.Width - 5; // 右边距5像素
            }
            else
            {
                // 对方消息：靠左，留出左边距
                newLeft = 5;
            }

            // 避免无限循环：仅当位置变化时才设置
            if (panelBubble.Left != newLeft)
            {
                panelBubble.Left = newLeft;
            }       
        }


        // 消息文本属性
        public string MessageText
        {
            get => lblMessage.Text;
            set
            {
                lblMessage.Text = value;
                // 文本变化可能导致气泡大小变化，触发重新计算高度和位置
                PerformLayout();
            }
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
                    // 自己消息：蓝底白字
                    panelBubble.BackColor = Color.FromArgb(0, 120, 215); // 一种典型的蓝色（如QQ蓝）
                    lblMessage.ForeColor = Color.White;
                }
                else
                {
                    // 对方消息：灰底黑字
                    panelBubble.BackColor = Color.LightGray;  // 浅灰色
                    lblMessage.ForeColor = Color.Black;
                }
                UpdateBubblePosition();   // 立即更新对齐方向
            }
        }     
    }
}


