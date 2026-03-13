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
            this.Load += Chat_Load;                                       
        }

        private void Chat_Load(object sender, EventArgs e)
        {
            // 模拟几条历史消息
            AddReceivedMessage("你好！我是 " + _friendNickname);
            AddSentMessage("嗨，最近怎么样？");
            AddReceivedMessage("还不错，你呢？");
            AddSentMessage("我也挺好，正在测试聊天界面。");
            AddReceivedMessage("看起来不错，消息气泡正常显示了！");
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
            QQClient.Communication.NetworkClient client = new QQClient.Communication.NetworkClient();

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
