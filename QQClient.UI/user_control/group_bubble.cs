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
    /// <summary>
    /// 群聊消息气泡控件
    /// 用于显示群聊中的单条消息，包含发送者昵称和消息内容
    /// 支持左右对齐（自己/对方）、不同颜色背景、自动调整高度
    /// </summary>
    public partial class group_bubble : UserControl
    {
        #region 字段声明

        private bool _isSelf;   // 标记是否为自己发送的消息

        #endregion

        #region 构造函数与初始化

        public group_bubble()
        {
            InitializeComponent();

            // 设置背景透明，使控件融入父容器背景
            this.BackColor = Color.Transparent;
            this.Visible = true;
            this.AutoSize = false;          // 关闭自动大小，由代码控制高度

            // 取消默认的 Dock 设置，改为手动布局
            panelNickname.Dock = DockStyle.None;
            panelBubble.Dock = DockStyle.None;

            // 设置昵称样式
            lblNickname.Font = new Font("微软雅黑", 9F, FontStyle.Regular);
            lblNickname.ForeColor = Color.Gray;

            // 设置消息样式
            lblMessage.Font = new Font("微软雅黑", 10F, FontStyle.Regular);
            lblMessage.BackColor = Color.Transparent;
            lblMessage.MaximumSize = new Size(400, 0);  // 限制消息最大宽度，避免过宽
            lblMessage.AutoSize = true;                 // 消息文本自动换行

            // 设置气泡面板：自动调整大小，内边距营造气泡效果
            panelBubble.AutoSize = true;
            panelBubble.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelBubble.Padding = new Padding(10, 8, 10, 8);

            // 整个控件也允许自动调整大小（但实际由代码控制）
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            // 订阅事件，当子控件大小变化时更新整体高度和位置
            panelBubble.Resize += (s, e) => UpdateTotalHeight();
            lblNickname.TextChanged += (s, e) => UpdateTotalHeight();
            lblMessage.TextChanged += (s, e) =>
            {
                // 如果控件句柄已创建，通过 Invoke 安全更新（避免跨线程问题）
                if (this.IsHandleCreated)
                {
                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        UpdateTotalHeight();
                        this.Refresh();
                    });
                }
                else
                {
                    UpdateTotalHeight();
                }
            };
            this.Resize += (s, e) => UpdatePositions();

            // 句柄创建后再执行一次初始布局，确保位置和高度正确
            this.HandleCreated += (s, e) =>
            {
                UpdateTotalHeight();
                UpdatePositions();
            };

            // 最终设置默认颜色
            lblMessage.ForeColor = Color.Black;
            lblNickname.ForeColor = Color.Blue;
        }

        #endregion

        #region 布局计算与更新

        /// <summary>
        /// 计算整个控件的高度
        /// 高度 = 昵称高度 + 间距 + 气泡高度（至少30px）
        /// </summary>
        private void UpdateTotalHeight()
        {
            try
            {
                int nicknameHeight = lblNickname.Visible ? lblNickname.Height : 0;
                int spacing = 2;

                // 获取气泡面板高度
                int bubbleHeight = panelBubble.Height;

                // 如果气泡高度为0，根据消息标签的高度推算（容错）
                if (bubbleHeight <= 0 && lblMessage.Height > 0)
                {
                    bubbleHeight = lblMessage.Height + (panelBubble.Padding.Top + panelBubble.Padding.Bottom);
                    Console.WriteLine($"[group_bubble] 从lblMessage计算高度: {bubbleHeight}");
                }

                // 最小高度30
                int newHeight = nicknameHeight + spacing + Math.Max(bubbleHeight, 30);
                if (newHeight < 40) newHeight = 40;

                // 仅当高度变化时才更新，避免重复设置
                if (this.Height != newHeight && this.IsHandleCreated)
                {
                    this.Height = newHeight;
                    Console.WriteLine($"[group_bubble] 更新高度: {newHeight}, 气泡高度={bubbleHeight}, 昵称高度={nicknameHeight}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[group_bubble] UpdateTotalHeight异常: {ex.Message}");
            }
        }

        /// <summary>
        /// 根据是否为自己消息，计算昵称和气泡面板的 X 坐标（靠左或靠右）
        /// </summary>
        private void UpdatePositions()
        {
            try
            {
                // 获取父容器的宽度，用于计算右对齐位置
                int containerWidth = this.Parent?.ClientSize.Width ?? 500;
                if (!this.IsHandleCreated) return;

                int newNicknameLeft, newBubbleLeft;
                if (_isSelf)
                {
                    // 自己消息：靠右，距右边10像素
                    newBubbleLeft = containerWidth - panelBubble.Width - 10;
                    newNicknameLeft = containerWidth - lblNickname.Width - 10;
                }
                else
                {
                    // 对方消息：靠左，距左边5像素
                    newNicknameLeft = 5;
                    newBubbleLeft = 5;
                }

                // 防止负数位置（当宽度不足时）
                if (newNicknameLeft < 0) newNicknameLeft = 5;
                if (newBubbleLeft < 0) newBubbleLeft = 5;

                if (lblNickname.Left != newNicknameLeft) lblNickname.Left = newNicknameLeft;
                if (panelBubble.Left != newBubbleLeft) panelBubble.Left = newBubbleLeft;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[group_bubble] UpdatePositions异常: {ex.Message}");
            }
        }

        #endregion

        #region 公共属性

        /// <summary>
        /// 发送者昵称
        /// </summary>
        public string Nickname
        {
            get => lblNickname.Text;
            set => lblNickname.Text = value;
        }

        /// <summary>
        /// 消息文本内容
        /// 设置后会重新计算布局（高度和位置）
        /// </summary>
        public string MessageText
        {
            get => lblMessage.Text;
            set
            {
                lblMessage.Text = value;
                Console.WriteLine($"[group_bubble] 设置消息文本: {value}");

                // 强制重新计算布局（安全地跨线程调用）
                if (this.IsHandleCreated)
                {
                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        UpdateTotalHeight();
                        UpdatePositions();
                        this.Refresh();
                    });
                }
                else
                {
                    UpdateTotalHeight();
                    UpdatePositions();
                }
            }
        }

        /// <summary>
        /// 是否为自己发送的消息
        /// true：气泡靠右、蓝色背景、白色文字，昵称显示为“我”
        /// false：气泡靠左、灰色背景、黑色文字，昵称显示发送者原昵称
        /// </summary>
        public bool IsSelf
        {
            get => _isSelf;
            set
            {
                _isSelf = value;
                if (value)
                {
                    // 自己：蓝底白字，昵称“我”并右对齐
                    panelBubble.BackColor = Color.FromArgb(0, 120, 215);
                    lblMessage.ForeColor = Color.White;
                    lblNickname.Visible = true;
                    lblNickname.ForeColor = Color.FromArgb(0, 120, 215);
                    lblNickname.Text = "我";
                    lblNickname.TextAlign = ContentAlignment.TopRight;
                }
                else
                {
                    // 对方：灰底黑字，昵称正常显示
                    panelBubble.BackColor = Color.LightGray;
                    lblMessage.ForeColor = Color.Black;
                    lblNickname.Visible = true;
                    lblNickname.ForeColor = Color.Gray;
                }

                // 立即更新位置和高度
                UpdatePositions();
                UpdateTotalHeight();
            }
        }

        #endregion
    }
}