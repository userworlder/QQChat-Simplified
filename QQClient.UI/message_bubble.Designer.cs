namespace QQClient.UI
{
    partial class message_bubble
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
            this.panelBubble = new System.Windows.Forms.Panel();
            this.lblMessage = new System.Windows.Forms.Label();
            this.panelBubble.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelBubble
            // 
            this.panelBubble.Controls.Add(this.lblMessage);
            this.panelBubble.Location = new System.Drawing.Point(17, 3);
            this.panelBubble.MaximumSize = new System.Drawing.Size(200, 0);
            this.panelBubble.Name = "panelBubble";
            this.panelBubble.Size = new System.Drawing.Size(200, 50);
            this.panelBubble.TabIndex = 0;
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(19, 16);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(80, 18);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "对话文本";
            // 
            // message_bubble
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelBubble);
            this.Name = "message_bubble";
            this.Size = new System.Drawing.Size(244, 56);
            this.panelBubble.ResumeLayout(false);
            this.panelBubble.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelBubble;
        private System.Windows.Forms.Label lblMessage;
    }
}
