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
    public partial class chat : Form
    {
        private string _friendAccount;  // 改为 account
        private string _friendNickname;
        string send_message;
        public chat(string Account,string Name)
        {
            InitializeComponent();
            this._friendAccount = Account;
            this._friendNickname = Name;
            this.Text = $"与 {_friendNickname} 聊天中";  // 窗口标题显示昵称
            label1.Text = _friendNickname;
            this.Resize += (s, e) => AdjustMessageWidths();
            
            this.Shown+=(s,e)=> chat_Load(s,e);
            this.Shown += (s, e) =>
            {
                // 添加一条对方消息
                var msg1 = new message_bubble
                {
                    MessageText = "这是对方的消息",
                    IsSelf = false,
                    Width = flowLayoutPanel1.ClientSize.Width
                };
                flowLayoutPanel1.Controls.Add(msg1);

                // 添加一条自己消息
                var msg2 = new message_bubble
                {
                    MessageText = "这是自己的消息",
                    IsSelf = true,
                    Width = flowLayoutPanel1.ClientSize.Width
                };
                flowLayoutPanel1.Controls.Add(msg2);
            };
            this.Shown += (s, e) =>
            {
                AddReceivedMessage("你好！我是 " + _friendNickname);
                AddSentMessage("嗨，最近怎么样？");
                AddReceivedMessage("还不错，你呢？");
                AddSentMessage("我也挺好，正在测试聊天界面。");
            };
        }
        private void chat_Load(object sender, EventArgs e)
        {
            var testMsg = new message_bubble
            {
                MessageText = "测试",
                IsSelf = false,
                Width = flowLayoutPanel1.ClientSize.Width,
                BackColor = Color.Red   // 临时设置，看控件是否出现
            };
            flowLayoutPanel1.Controls.Add(testMsg);
        }
        private void AdjustMessageWidths()
        {
            int newWidth = flowLayoutPanel1.ClientSize.Width;
            // 如果有垂直滚动条，减去滚动条宽度
            if (flowLayoutPanel1.VerticalScroll.Visible)
                newWidth -= SystemInformation.VerticalScrollBarWidth;

            foreach (Control ctrl in flowLayoutPanel1.Controls)
            {
                if (ctrl is message_bubble msg)
                {
                    ctrl.Width = newWidth;
                }
            }
        }

        // 添加一条对方的消息
        private void AddReceivedMessage(string text)
        {
            var msg = new message_bubble
            {
                MessageText = text,
                IsSelf = false,
                Width = flowLayoutPanel1.ClientSize.Width  // 设置宽度
            };
            flowLayoutPanel1.Controls.Add(msg);
            flowLayoutPanel1.ScrollControlIntoView(msg);  // 滚动到底部
            AdjustMessageWidths();                    // 确保宽度正确
        }

        // 添加一条自己发送的消息
        private void AddSentMessage(string text)
        {
            var msg = new message_bubble
            {
                MessageText = text,
                IsSelf = true,
                Width = flowLayoutPanel1.ClientSize.Width
            };
            flowLayoutPanel1.Controls.Add(msg);
            flowLayoutPanel1.ScrollControlIntoView(msg);
            AdjustMessageWidths();
        }

        //打开主页
        private void label1_Click(object sender, EventArgs e)
        {
            //profile profile=new profile();
            MessageBox.Show("打开简介");
        }
        //清空
        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }
        //发送
        private void button2_Click(object sender, EventArgs e)
        {
            QQClient.Communication.NetworkClientLegacy client = new QQClient.Communication.NetworkClientLegacy();

            send_message =textBox1.Text;
            if (textBox1.Text.Length > 0)
            {
               // client.SendMessage();
            }
            else
            {

            }
        }
    }
}
