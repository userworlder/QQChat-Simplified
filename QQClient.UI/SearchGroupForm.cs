using QQCommon.Models;
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
    public partial class SearchGroupForm : Form
    {
        public SearchGroupForm()
        {
            InitializeComponent();
        }

        private async void BtnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtKeyword.Text.Trim();
            if (string.IsNullOrEmpty(keyword)) return;

            var client = GlobalClient.Current;
            var groups = await Task.Run(() => client.SearchGroups(keyword));
            flowResults.Controls.Clear();

            foreach (var group in groups)
            {
                var panel = new Panel { Width = flowResults.Width - 20, Height = 80, BorderStyle = BorderStyle.FixedSingle };
                var lblName = new Label { Text = group.GroupName, Location = new System.Drawing.Point(10, 10), AutoSize = true };
                var lblDesc = new Label { Text = group.Description, Location = new System.Drawing.Point(10, 35), AutoSize = true };
                var btnJoin = new Button { Text = "加入", Location = new System.Drawing.Point(panel.Width - 80, 25), Size = new System.Drawing.Size(70, 30), Tag = group.GroupId };
                btnJoin.Click += async (s, ev) =>
                {
                    bool success = await Task.Run(() => client.JoinGroup(group.GroupId));
                    MessageBox.Show(success ? "申请已发送" : "加入失败");
                };
                panel.Controls.Add(lblName);
                panel.Controls.Add(lblDesc);
                panel.Controls.Add(btnJoin);
                flowResults.Controls.Add(panel);
            }

            if (flowResults.Controls.Count == 0)
            {
                flowResults.Controls.Add(new Label { Text = "未找到相关群组", AutoSize = true });
            }
        }
    }
}
