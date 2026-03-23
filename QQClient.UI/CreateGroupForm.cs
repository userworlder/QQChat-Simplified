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
        #region 公开属性

        /// <summary>
        /// 获取用户输入的群名称（已去除首尾空格）
        /// </summary>
        public string GroupName => txtGroupName.Text.Trim();

        /// <summary>
        /// 获取用户输入的群简介（已去除首尾空格）
        /// </summary>
        public string Description => txtDescription.Text.Trim();

        #endregion

        #region 构造函数

        public CreateGroupForm()
        {
            InitializeComponent();
        }

        #endregion

        #region 按钮事件处理

        /// <summary>
        /// “确定”按钮点击事件
        /// 验证群名称不为空，然后设置 DialogResult 为 OK 并关闭窗体
        /// </summary>
        private void btnOK_Click(object sender, EventArgs e)
        {
            // 检查群名称是否为空（包括空白字符）
            if (string.IsNullOrWhiteSpace(txtGroupName.Text))
            {
                MessageBox.Show("群名称不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // 设置对话框结果为 OK，调用方可通过 ShowDialog 的返回值判断用户是否确认
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// “取消”按钮点击事件
        /// 设置 DialogResult 为 Cancel 并关闭窗体
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        #endregion
    }
}