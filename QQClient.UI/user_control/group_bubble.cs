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
    public partial class group_bubble : UserControl
    {
        public group_bubble()
        {
            InitializeComponent();
            // 设置样式
            this.BackColor = Color.Transparent;
            // 确保控件可见
            this.Visible = true;
            this.AutoSize = false;

            panelNickname.Dock = DockStyle.None;
            panelBubble.Dock = DockStyle.None;

            // 设置昵称样式
            lblNickname.Font = new Font("微软雅黑", 9F, FontStyle.Regular);
            lblNickname.ForeColor = Color.Gray;

            // 设置消息样式
            lblMessage.Font = new Font("微软雅黑", 10F, FontStyle.Regular);
            lblMessage.BackColor = Color.Transparent;
            lblMessage.MaximumSize = new Size(400, 0);  // 限制最大宽度为400像素
            lblMessage.AutoSize = true;  // 自动调整大小

            // 设置气泡面板
            panelBubble.AutoSize = true;
            panelBubble.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panelBubble.Padding = new Padding(10, 8, 10, 8);

            // 重要：不设置固定宽度，让控件自动调整
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            panelBubble.Resize += (s, e) => UpdateTotalHeight();
            lblNickname.TextChanged += (s, e) => UpdateTotalHeight();
            lblMessage.TextChanged += (s, e) =>
            {
                // 检查控件句柄是否已创建
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
                    // 句柄未创建，直接更新
                    UpdateTotalHeight();
                }
            };
            this.Resize += (s, e) => UpdatePositions();

            // 在句柄创建后再执行初始化
            this.HandleCreated += (s, e) =>
            {
                UpdateTotalHeight();
                UpdatePositions();
            };
            lblMessage.ForeColor = Color.Black;
            lblNickname.ForeColor = Color.Blue;
        }
        private void UpdateTotalHeight()
        {
            try
            {
                int nicknameHeight = lblNickname.Visible ? lblNickname.Height : 0;
                int spacing = 2;

                // 计算气泡实际高度
                int bubbleHeight = panelBubble.Height;

                // 如果气泡高度为0，尝试从lblMessage获取高度
                if (bubbleHeight <= 0 && lblMessage.Height > 0)
                {
                    bubbleHeight = lblMessage.Height + (panelBubble.Padding.Top + panelBubble.Padding.Bottom);
                    Console.WriteLine($"[group_bubble] 从lblMessage计算高度: {bubbleHeight}");
                }

                // 确保最小高度
                int newHeight = nicknameHeight + spacing + Math.Max(bubbleHeight, 30);

                if (newHeight < 40) newHeight = 40;

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

        private void UpdatePositions()
        {
            try
            {
                int containerWidth = this.Parent?.ClientSize.Width ?? 500;
                if (!this.IsHandleCreated) return;

                int newNicknameLeft, newBubbleLeft;
                if (_isSelf)
                {
                    // 自己的消息靠右
                    newBubbleLeft = containerWidth - panelBubble.Width - 10;
                    newNicknameLeft = containerWidth - lblNickname.Width - 10;
                }
                else
                {
                    newNicknameLeft = 5;
                    newBubbleLeft = 5;
                }

                // 确保位置不为负数
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

        public string Nickname
        {
            get => lblNickname.Text;
            set => lblNickname.Text = value;
        }

        public string MessageText
        {
            get => lblMessage.Text;
            set
            {
                lblMessage.Text = value;
                Console.WriteLine($"[group_bubble] 设置消息文本: {value}");

                // 强制重新计算布局
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

        private bool _isSelf;
        public bool IsSelf
        {
            get => _isSelf;
            set
            {
                _isSelf = value;
                if (value)
                {
                    panelBubble.BackColor = Color.FromArgb(0, 120, 215);
                    lblMessage.ForeColor = Color.White;
                    lblNickname.Visible = true;
                    lblNickname.ForeColor = Color.FromArgb(0, 120, 215);
                    lblNickname.Text = "我";
                    lblNickname.TextAlign = ContentAlignment.TopRight;
                }
                else
                {
                    panelBubble.BackColor = Color.LightGray;
                    lblMessage.ForeColor = Color.Black;
                    lblNickname.Visible = true;
                    lblNickname.ForeColor = Color.Gray;
                }

                UpdatePositions();
                UpdateTotalHeight();
            }
        }
    }
}
