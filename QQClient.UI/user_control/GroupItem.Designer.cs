namespace QQClient.UI.user_control
{
    partial class GroupItem
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblGroupName = new System.Windows.Forms.Label();
            this.lblLastMessage = new System.Windows.Forms.Label();
            this.lblTime = new System.Windows.Forms.Label();
            this.lblUnread = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblGroupName
            // 
            this.lblGroupName.AutoSize = false;
            this.lblGroupName.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold);
            this.lblGroupName.Location = new System.Drawing.Point(12, 10);
            this.lblGroupName.Name = "lblGroupName";
            this.lblGroupName.Size = new System.Drawing.Size(180, 23);
            this.lblGroupName.TabIndex = 0;
            this.lblGroupName.Text = "群名";
            this.lblGroupName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblLastMessage
            // 
            this.lblLastMessage.AutoEllipsis = true;
            this.lblLastMessage.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lblLastMessage.ForeColor = System.Drawing.Color.Gray;
            this.lblLastMessage.Location = new System.Drawing.Point(12, 38);
            this.lblLastMessage.Name = "lblLastMessage";
            this.lblLastMessage.Size = new System.Drawing.Size(220, 20);
            this.lblLastMessage.TabIndex = 1;
            this.lblLastMessage.Text = "最后消息";
            this.lblLastMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblLastMessage.Click += new System.EventHandler(this.lblLastMessage_Click);
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Font = new System.Drawing.Font("微软雅黑", 8F);
            this.lblTime.ForeColor = System.Drawing.Color.Gray;
            this.lblTime.Location = new System.Drawing.Point(240, 12);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(32, 16);
            this.lblTime.TabIndex = 2;
            this.lblTime.Text = "时间";
            // 
            // lblUnread
            // 
            this.lblUnread.BackColor = System.Drawing.Color.Red;
            this.lblUnread.Font = new System.Drawing.Font("微软雅黑", 8F, System.Drawing.FontStyle.Bold);
            this.lblUnread.ForeColor = System.Drawing.Color.White;
            this.lblUnread.Location = new System.Drawing.Point(260, 38);
            this.lblUnread.Name = "lblUnread";
            this.lblUnread.Size = new System.Drawing.Size(28, 24);
            this.lblUnread.TabIndex = 3;
            this.lblUnread.Text = "0";
            this.lblUnread.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblUnread.Visible = false;
            // 
            // GroupItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.lblUnread);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.lblLastMessage);
            this.Controls.Add(this.lblGroupName);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(306, 70);
            this.Name = "GroupItem";
            this.Size = new System.Drawing.Size(306, 70);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblGroupName;
        private System.Windows.Forms.Label lblLastMessage;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Label lblUnread;
    }
}
