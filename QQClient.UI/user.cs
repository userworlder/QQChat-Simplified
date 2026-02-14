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
    public partial class user : Form
    {
        public user()
        {
            InitializeComponent();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void public_chat_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {//私聊模式
            public_chat.Visible = false;
            private_chat.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {//群聊
            public_chat.Visible = true;
            private_chat.Visible = false;
        }
    }
}
