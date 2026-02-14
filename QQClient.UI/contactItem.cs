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
    public partial class ContactItem : UserControl
    {
        public string DisplayName
        {
            get { return label_name.Text; }
            set { label_name.Text = value; }
        }

        public string LastMessage
        {
            get { return label_lstchat.Text; }
            set { label_lstchat.Text = value; }
        }

        public string Time
        {
            get { return label_time.Text; }
            set { label_time.Text = value; }
        }
        public ContactItem()
        {
            InitializeComponent();
        }

        private void contactItem_Click(object sender, EventArgs e)
        {

        }
    }
}
