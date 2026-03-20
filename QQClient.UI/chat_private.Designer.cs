namespace QQClient.UI
{
    partial class chat_private
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlTop = new System.Windows.Forms.Panel();
            this.lblFriendName = new System.Windows.Forms.Label();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.btn_test = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.pnlInput = new System.Windows.Forms.Panel();
            this.lbl_warn = new System.Windows.Forms.Label();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.pnlMessages = new System.Windows.Forms.Panel();
            this.flowMessages = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlTop.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.pnlInput.SuspendLayout();
            this.pnlMessages.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.DarkGray;
            this.pnlTop.Controls.Add(this.lblFriendName);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(800, 49);
            this.pnlTop.TabIndex = 0;
            // 
            // lblFriendName
            // 
            this.lblFriendName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblFriendName.AutoSize = true;
            this.lblFriendName.Location = new System.Drawing.Point(371, 9);
            this.lblFriendName.Name = "lblFriendName";
            this.lblFriendName.Size = new System.Drawing.Size(62, 18);
            this.lblFriendName.TabIndex = 0;
            this.lblFriendName.Text = "好友名";
            this.lblFriendName.Click += new System.EventHandler(this.lblFriendName_Click);
            // 
            // pnlBottom
            // 
            this.pnlBottom.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pnlBottom.Controls.Add(this.btn_test);
            this.pnlBottom.Controls.Add(this.btnSend);
            this.pnlBottom.Controls.Add(this.btnClear);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 402);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(800, 48);
            this.pnlBottom.TabIndex = 2;
            // 
            // btn_test
            // 
            this.btn_test.Location = new System.Drawing.Point(498, 13);
            this.btn_test.Name = "btn_test";
            this.btn_test.Size = new System.Drawing.Size(91, 32);
            this.btn_test.TabIndex = 5;
            this.btn_test.Text = "测试发送";
            this.btn_test.UseVisualStyleBackColor = true;
            this.btn_test.Click += new System.EventHandler(this.btn_test_Click);
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(711, 13);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 32);
            this.btnSend.TabIndex = 4;
            this.btnSend.Text = "发送";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click_1);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(610, 13);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 32);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "清空";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click_1);
            // 
            // pnlInput
            // 
            this.pnlInput.Controls.Add(this.lbl_warn);
            this.pnlInput.Controls.Add(this.txtInput);
            this.pnlInput.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlInput.Location = new System.Drawing.Point(0, 299);
            this.pnlInput.Name = "pnlInput";
            this.pnlInput.Size = new System.Drawing.Size(800, 103);
            this.pnlInput.TabIndex = 3;
            // 
            // lbl_warn
            // 
            this.lbl_warn.AutoSize = true;
            this.lbl_warn.ForeColor = System.Drawing.Color.Red;
            this.lbl_warn.Location = new System.Drawing.Point(607, 73);
            this.lbl_warn.Name = "lbl_warn";
            this.lbl_warn.Size = new System.Drawing.Size(80, 18);
            this.lbl_warn.TabIndex = 1;
            this.lbl_warn.Text = "警告文本";
            this.lbl_warn.Visible = false;
            // 
            // txtInput
            // 
            this.txtInput.AcceptsReturn = true;
            this.txtInput.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInput.Location = new System.Drawing.Point(0, 0);
            this.txtInput.Multiline = true;
            this.txtInput.Name = "txtInput";
            this.txtInput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtInput.Size = new System.Drawing.Size(800, 103);
            this.txtInput.TabIndex = 0;
            // 
            // pnlMessages
            // 
            this.pnlMessages.Controls.Add(this.flowMessages);
            this.pnlMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMessages.Location = new System.Drawing.Point(0, 49);
            this.pnlMessages.Name = "pnlMessages";
            this.pnlMessages.Size = new System.Drawing.Size(800, 250);
            this.pnlMessages.TabIndex = 4;
            // 
            // flowMessages
            // 
            this.flowMessages.AutoScroll = true;
            this.flowMessages.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.flowMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowMessages.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowMessages.Location = new System.Drawing.Point(0, 0);
            this.flowMessages.Name = "flowMessages";
            this.flowMessages.Size = new System.Drawing.Size(800, 250);
            this.flowMessages.TabIndex = 0;
            this.flowMessages.WrapContents = false;
            // 
            // chat_new
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.pnlMessages);
            this.Controls.Add(this.pnlInput);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlTop);
            this.Name = "chat_new";
            this.Text = "chat_new";
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlBottom.ResumeLayout(false);
            this.pnlInput.ResumeLayout(false);
            this.pnlInput.PerformLayout();
            this.pnlMessages.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblFriendName;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Panel pnlInput;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.Panel pnlMessages;
        private System.Windows.Forms.FlowLayoutPanel flowMessages;
        private System.Windows.Forms.Label lbl_warn;
        private System.Windows.Forms.Button btn_test;
    }
}