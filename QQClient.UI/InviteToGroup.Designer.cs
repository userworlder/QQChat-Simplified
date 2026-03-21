namespace QQClient.UI
{
    partial class InviteToGroup
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.FlowLayoutPanel flowFriends;
        private System.Windows.Forms.Button btnClose;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.flowFriends = new System.Windows.Forms.FlowLayoutPanel();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // flowFriends
            // 
            this.flowFriends.AutoScroll = true;
            this.flowFriends.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowFriends.Location = new System.Drawing.Point(0, 0);
            this.flowFriends.Name = "flowFriends";
            this.flowFriends.Size = new System.Drawing.Size(400, 300);
            this.flowFriends.TabIndex = 0;
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(162, 310);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 30);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "关闭";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // InviteToGroup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(400, 350);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.flowFriends);
            this.Name = "InviteToGroup";
            this.Text = "邀请好友入群";
            this.ResumeLayout(false);
        }
    }
}