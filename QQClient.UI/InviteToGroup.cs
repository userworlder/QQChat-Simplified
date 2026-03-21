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
    public partial class InviteToGroup : Form
    {
        private string _groupId;

        public InviteToGroup(string groupId)
        {
            InitializeComponent();
            _groupId = groupId;
            LoadFriends();
        }

        private async void LoadFriends()
        {
            var client = GlobalClient.Current;
            if (client == null) return;

            var friends = await Task.Run(() => client.SearchAllFriends(GlobalClient.CurrentUserId));
            flowFriends.Controls.Clear();

            foreach (var friend in friends)
            {
                string displayName = friend.FriendNickName ?? friend.FriendUserName;
                var btnInvite = new Button
                {
                    Text = displayName,
                    Tag = friend.FriendUserName,
                    Width = 120,
                    Height = 30,
                    Margin = new Padding(5)
                };
                btnInvite.Click += async (s, e) =>
                {
                    var btn = (Button)s;
                    bool success = await Task.Run(() => client.InviteToGroup(_groupId, btn.Tag.ToString()));
                    MessageBox.Show(success ? "邀请已发送" : "邀请失败，请重试");
                };
                flowFriends.Controls.Add(btnInvite);
            }

            if (flowFriends.Controls.Count == 0)
            {
                flowFriends.Controls.Add(new Label { Text = "暂无好友，请先添加好友", AutoSize = true });
            }
        }
    }
}
