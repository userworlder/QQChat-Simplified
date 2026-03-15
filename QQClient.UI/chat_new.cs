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
    public partial class chat_new : Form
    {
        private string _friendAccount;
        private string _friendNickname;

        public chat_new(string friendAccount, string friendNickname)
        {
            InitializeComponent();
            _friendAccount = friendAccount;
            _friendNickname = friendNickname;
            lblFriendName.Text = friendNickname;
            this.Text = $"与 {friendNickname} 聊天中";

            // 订阅事件
            //this.Load += ChatNew_Load;
            this.Load += chat_new_Load;  // 注意方法名不要写错，且没有括号

            //flowMessages.Resize += (s, e) => AdjustMessageWidths();
            //btnSend.Click += BtnSend_Click;
            //btnClear.Click += BtnClear_Click;
            //lblFriendName.Click += LblFriendName_Click;
        }
        private void chat_new_Load(object sender, EventArgs e)
        {
            flowMessages.Controls.Clear();

            AddReceivedMessage("你好，这是一条对方的消息，可能比较长，用来测试自动换行和高度自适应。");
            AddSentMessage("这是我的消息，应该显示在右侧。");
            AddReceivedMessage("另一条对方消息。");
            AddSentMessage("再发一条。");

            // 调试：输出每个控件的高度
            //foreach (Control ctrl in flowMessages.Controls)
            //{
            //    if (ctrl is message_bubble msg)
            //    {
            //        MessageBox.Show($"消息: {msg.MessageText}\n控件高度: {msg.Height}\nPanelBubble高度: {msg.Controls[0].Height}");
            //    }
            //}
        }


        private void AddReceivedMessage(string text)
        {
            var msg = new message_bubble
            {
                MessageText = text,
                IsSelf = false,
                Width = flowMessages.ClientSize.Width
            };
            flowMessages.Controls.Add(msg);
            flowMessages.ScrollControlIntoView(msg);
            AdjustMessageWidths();
            System.Diagnostics.Debug.WriteLine($"添加消息: {text}, 宽度: {msg.Width}");
        }

        private void AddSentMessage(string text)
        {
            var msg = new message_bubble
            {
                MessageText = text,
                IsSelf = true,
                Width = flowMessages.ClientSize.Width
            };
            flowMessages.Controls.Add(msg);
            flowMessages.ScrollControlIntoView(msg);
            AdjustMessageWidths();
            System.Diagnostics.Debug.WriteLine($"添加消息: {text}, 宽度: {msg.Width}");
        }

        private void AdjustMessageWidths()
        {
            int newWidth = flowMessages.ClientSize.Width;
            if (flowMessages.VerticalScroll.Visible)
                newWidth -= SystemInformation.VerticalScrollBarWidth;

            foreach (Control ctrl in flowMessages.Controls)
            {
                if (ctrl is message_bubble msg)
                {
                    ctrl.Width = newWidth;
                }
            }
        }
        private void lblFriendName_Click(object sender, EventArgs e)
        {

        }

        private void btnSend_Click_1(object sender, EventArgs e)
        {

        }

        private void btnClear_Click_1(object sender, EventArgs e)
        {

        }
    }
}
