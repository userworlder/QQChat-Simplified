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
            panelBubble.Resize += (s, e) => UpdateTotalHeight();
            lblNickname.TextChanged += (s, e) => UpdateTotalHeight();
            this.Resize += (s, e) => UpdatePositions();
            UpdateTotalHeight();
        }

        private void UpdateTotalHeight()
        {
            int nicknameHeight = lblNickname.Visible ? lblNickname.Height : 0;
            int spacing = 2;
            int newHeight = nicknameHeight + spacing + panelBubble.Height;
            if (this.Height != newHeight)
                this.Height = newHeight;
        }

        private void UpdatePositions()
        {
            int newNicknameLeft, newBubbleLeft;
            if (_isSelf)
            {
                newNicknameLeft = this.ClientSize.Width - lblNickname.Width - 5;
                newBubbleLeft = this.ClientSize.Width - panelBubble.Width - 5;
            }
            else
            {
                newNicknameLeft = 5;
                newBubbleLeft = 5;
            }
            if (lblNickname.Left != newNicknameLeft) lblNickname.Left = newNicknameLeft;
            if (panelBubble.Left != newBubbleLeft) panelBubble.Left = newBubbleLeft;
        }

        public string Nickname
        {
            get => lblNickname.Text;
            set => lblNickname.Text = value;
        }

        public string MessageText
        {
            get => lblMessage.Text;
            set => lblMessage.Text = value;
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
                    // 可选：自己消息隐藏昵称
                    lblNickname.Visible = false;
                }
                else
                {
                    panelBubble.BackColor = Color.LightGray;
                    lblMessage.ForeColor = Color.Black;
                    lblNickname.Visible = true;
                }
                UpdatePositions();
                UpdateTotalHeight();
            }
        }
    }
}
