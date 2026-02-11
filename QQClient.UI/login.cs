using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QQClient.UI
{
    public partial class login : Form
    {
        public login()
        {
            InitializeComponent();

            pictureBox1.Image = ImageHelper.Load("login.png");
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            
            register register=new register();
            register.ShowDialog();
        }

     

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            user user= new user();
            user.ShowDialog();
        }
    }
}
