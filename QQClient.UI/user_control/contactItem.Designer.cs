namespace QQClient.UI
{
    partial class ContactItem
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
            this.label_name = new System.Windows.Forms.Label();
            this.label_lstchat = new System.Windows.Forms.Label();
            this.label_time = new System.Windows.Forms.Label();
            this.label_unread = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label_name
            // 
            this.label_name.AutoSize = true;
            this.label_name.Location = new System.Drawing.Point(64, 10);
            this.label_name.Name = "label_name";
            this.label_name.Size = new System.Drawing.Size(44, 18);
            this.label_name.TabIndex = 0;
            this.label_name.Text = "名字";
            // 
            // label_lstchat
            // 
            this.label_lstchat.AutoSize = true;
            this.label_lstchat.Location = new System.Drawing.Point(64, 51);
            this.label_lstchat.Name = "label_lstchat";
            this.label_lstchat.Size = new System.Drawing.Size(80, 18);
            this.label_lstchat.TabIndex = 1;
            this.label_lstchat.Text = "最后聊天";
            // 
            // label_time
            // 
            this.label_time.AutoSize = true;
            this.label_time.Location = new System.Drawing.Point(270, 10);
            this.label_time.Name = "label_time";
            this.label_time.Size = new System.Drawing.Size(44, 18);
            this.label_time.TabIndex = 2;
            this.label_time.Text = "时间";
            // 
            // label_unread
            // 
            this.label_unread.AutoSize = true;
            this.label_unread.BackColor = System.Drawing.SystemColors.Control;
            this.label_unread.ForeColor = System.Drawing.Color.Red;
            this.label_unread.Location = new System.Drawing.Point(273, 50);
            this.label_unread.Name = "label_unread";
            this.label_unread.Size = new System.Drawing.Size(44, 18);
            this.label_unread.TabIndex = 3;
            this.label_unread.Text = "未读";
            this.label_unread.Visible = false;
            // 
            // ContactItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.Controls.Add(this.label_unread);
            this.Controls.Add(this.label_time);
            this.Controls.Add(this.label_lstchat);
            this.Controls.Add(this.label_name);
            this.Name = "ContactItem";
            this.Size = new System.Drawing.Size(344, 77);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_name;
        private System.Windows.Forms.Label label_lstchat;
        private System.Windows.Forms.Label label_time;
        private System.Windows.Forms.Label label_unread;
    }
}
