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
    public partial class CreateGroupForm : Form
    {
        // 公开属性，供调用者获取输入值
        public string GroupName => txtGroupName.Text.Trim();
        public string Description => txtDescription.Text.Trim();

        public CreateGroupForm()
        {
            InitializeComponent();
        }

        // 确定按钮点击事件
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtGroupName.Text))
            {
                MessageBox.Show("群名称不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        // 取消按钮点击事件
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
