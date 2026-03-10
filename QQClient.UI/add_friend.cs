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
    public partial class add_friend : Form
    {
        public add_friend()
        {
            InitializeComponent();
        }
        private void user_Load(object sender, EventArgs e)
        {
            Table();
        }

        //public void Table()
        //{
        //    dataGridView1.Rows.Clear();
        //    // 使用返回 DataTable 的方法
        //   // var users = bookManager.GetAllBooksAsDataTable();

        //    foreach (System.Data.DataRow row in users.Rows)
        //    {
        //        dataGridView1.Rows.Add(          
        //            row["publisher"].ToString(),
        //            row["number"]);
        //    }
        //}
        //账号查询
        private void search_account_Click(object sender, EventArgs e)
        {
            string keyword=textBox1.Text;
            if (blurd_mode.Checked)
            {
                //模糊账号查询

            }
            else
            {

            }
        }
        //昵称查询
        private void search_nickname_Click(object sender, EventArgs e)
        {       
            string keyword=textBox2.Text;
            if (blurd_mode.Checked)
            {
                //模糊昵称查询

            }
            else
            {

            }
        }
        

    }
}
