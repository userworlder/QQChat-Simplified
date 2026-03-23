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
    /// 私聊消息气泡控件
    /// 用于显示单条聊天消息，支持左右对齐和不同颜色（自己/对方）
    /// </summary>
    public partial class message_bubble : UserControl
    {
        #region 字段声明

        private bool _isSelf;   // 标记是否为自己发送的消息

        #endregion

        #region 构造函数与初始化

        public message_bubble()
        {
            InitializeComponent();

            // 确保控件不自动调整宽度（宽度由父容器决定），高度由内容动态调整
            this.AutoSize = false;
            this.Height = panelBubble.Height;   // 初始高度设为气泡面板的高度

            // 当气泡面板大小变化时，同步更新整个控件的高度
            panelBubble.Resize += (s, e) => UpdateHeight();

            // 当控件大小变化时，重新计算气泡位置（父容器宽度变化时触发）
            this.Resize += (s, e) => UpdateBubblePosition();
        }

        #endregion

        #region 布局更新方法

        /// <summary>
        /// 更新控件高度为气泡面板的高度（使控件高度刚好包裹内容）
        /// </summary>
        private void UpdateHeight()
        {
            if (this.Height != panelBubble.Height)
            {
                this.Height = panelBubble.Height;
            }
        }

        /// <summary>
        /// 根据是否为自己消息，更新气泡面板的位置（靠左或靠右）
        /// </summary>
        private void UpdateBubblePosition()
        {
            int newLeft;
            if (_isSelf)
            {
                // 自己消息：靠右，留出右边距5像素
                newLeft = this.ClientSize.Width - panelBubble.Width - 5;
            }
            else
            {
                // 对方消息：靠左，留出左边距5像素
                newLeft = 5;
            }

            // 仅当位置改变时才设置，避免无限循环
            if (panelBubble.Left != newLeft)
            {
                panelBubble.Left = newLeft;
            }
        }

        #endregion

        #region 公共属性

        /// <summary>
        /// 消息文本内容
        /// 设置文本后，气泡大小会重新计算，布局自动更新
        /// </summary>
        public string MessageText
        {
            get => lblMessage.Text;
            set
            {
                lblMessage.Text = value;
                // 文本变化可能导致气泡大小改变，触发重新布局（会间接调用 UpdateHeight 和 UpdateBubblePosition）
                PerformLayout();
            }
        }

        /// <summary>
        /// 是否为自己发送的消息
        /// 设置为 true 时：气泡靠右、蓝色背景、白色字体
        /// 设置为 false 时：气泡靠左、灰色背景、黑色字体
        /// </summary>
        public bool IsSelf
        {
            get => _isSelf;
            set
            {
                _isSelf = value;
                if (value)
                {
                    // 自己消息：蓝底白字（QQ风格）
                    panelBubble.BackColor = Color.FromArgb(0, 120, 215);
                    lblMessage.ForeColor = Color.White;
                }
                else
                {
                    // 对方消息：灰底黑字
                    panelBubble.BackColor = Color.LightGray;
                    lblMessage.ForeColor = Color.Black;
                }
                // 立即更新对齐方向（位置）
                UpdateBubblePosition();
            }
        }

        #endregion
    }
}