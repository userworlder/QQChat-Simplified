namespace QQClient.UI.user_control
{
    partial class group_bubble
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.panelNickname = new System.Windows.Forms.Panel();
            this.lblNickname = new System.Windows.Forms.Label();
            this.panelBubble = new System.Windows.Forms.Panel();
            this.lblMessage = new System.Windows.Forms.Label();
            this.panelNickname.SuspendLayout();
            this.panelBubble.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelNickname
            // 
            this.panelNickname.Controls.Add(this.lblNickname);
            this.panelNickname.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelNickname.Location = new System.Drawing.Point(0, 0);
            this.panelNickname.Name = "panelNickname";
            this.panelNickname.Size = new System.Drawing.Size(530, 30);
            this.panelNickname.TabIndex = 0;
            // 
            // lblNickname
            // 
            this.lblNickname.AutoSize = true;
            this.lblNickname.Location = new System.Drawing.Point(4, 4);
            this.lblNickname.Name = "lblNickname";
            this.lblNickname.Size = new System.Drawing.Size(44, 18);
            this.lblNickname.TabIndex = 0;
            this.lblNickname.Text = "昵称";
            // 
            // panelBubble
            // 
            this.panelBubble.Controls.Add(this.lblMessage);
            this.panelBubble.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBubble.Location = new System.Drawing.Point(0, 30);
            this.panelBubble.Name = "panelBubble";
            this.panelBubble.Size = new System.Drawing.Size(530, 54);
            this.panelBubble.TabIndex = 1;
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(3, 3);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(44, 18);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "文本";
            // 
            // group_bubble
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelBubble);
            this.Controls.Add(this.panelNickname);
            this.Name = "group_bubble";
            this.Size = new System.Drawing.Size(530, 84);
            this.panelNickname.ResumeLayout(false);
            this.panelNickname.PerformLayout();
            this.panelBubble.ResumeLayout(false);
            this.panelBubble.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelNickname;
        private System.Windows.Forms.Panel panelBubble;
        private System.Windows.Forms.Label lblNickname;
        private System.Windows.Forms.Label lblMessage;
    }
}
