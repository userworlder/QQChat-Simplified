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
            this.lblGroupName.AutoSize = true;
            this.lblGroupName.Location = new System.Drawing.Point(57, 8);
            this.lblGroupName.Name = "lblGroupName";
            this.lblGroupName.Size = new System.Drawing.Size(37, 15);
            this.lblGroupName.TabIndex = 0;
            this.lblGroupName.Text = "群名";
            // 
            // lblLastMessage
            // 
            this.lblLastMessage.AutoSize = true;
            this.lblLastMessage.Location = new System.Drawing.Point(47, 42);
            this.lblLastMessage.Name = "lblLastMessage";
            this.lblLastMessage.Size = new System.Drawing.Size(67, 15);
            this.lblLastMessage.TabIndex = 1;
            this.lblLastMessage.Text = "最后消息";
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Location = new System.Drawing.Point(240, 8);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(37, 15);
            this.lblTime.TabIndex = 2;
            this.lblTime.Text = "时间";
            // 
            // lblUnread
            // 
            this.lblUnread.AutoSize = true;
            this.lblUnread.BackColor = System.Drawing.SystemColors.Control;
            this.lblUnread.ForeColor = System.Drawing.Color.Red;
            this.lblUnread.Location = new System.Drawing.Point(240, 42);
            this.lblUnread.Name = "lblUnread";
            this.lblUnread.Size = new System.Drawing.Size(37, 15);
            this.lblUnread.TabIndex = 3;
            this.lblUnread.Text = "未读";
            this.lblUnread.Visible = false;
            // 
            // GroupItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblUnread);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.lblLastMessage);
            this.Controls.Add(this.lblGroupName);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "GroupItem";
            this.Size = new System.Drawing.Size(306, 64);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label lblGroupName;
        private System.Windows.Forms.Label lblLastMessage;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Label lblUnread;
    }

    
}
