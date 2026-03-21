namespace QQClient.UI
{
    partial class user
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
            this.components = new System.ComponentModel.Container();
            this.paneltab = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.private_chat = new System.Windows.Forms.FlowLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.public_chat = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.request = new System.Windows.Forms.FlowLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.btn_profile = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.paneltab.SuspendLayout();
            this.panel1.SuspendLayout();
            this.private_chat.SuspendLayout();
            this.public_chat.SuspendLayout();
            this.request.SuspendLayout();
            this.SuspendLayout();
            // 
            // paneltab
            // 
            this.paneltab.Controls.Add(this.button2);
            this.paneltab.Controls.Add(this.button1);
            this.paneltab.Location = new System.Drawing.Point(0, 103);
            this.paneltab.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.paneltab.Name = "paneltab";
            this.paneltab.Size = new System.Drawing.Size(308, 32);
            this.paneltab.TabIndex = 0;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(153, 0);
            this.button2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(156, 32);
            this.button2.TabIndex = 1;
            this.button2.Text = "群聊";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.btn_publicmode);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(0, 0);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(153, 32);
            this.button1.TabIndex = 0;
            this.button1.Text = "私聊";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btn_privatemode);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.private_chat);
            this.panel1.Controls.Add(this.public_chat);
            this.panel1.Controls.Add(this.request);
            this.panel1.Location = new System.Drawing.Point(0, 141);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(308, 383);
            this.panel1.TabIndex = 1;
            // 
            // private_chat
            // 
            this.private_chat.AutoScroll = true;
            this.private_chat.BackColor = System.Drawing.SystemColors.Info;
            this.private_chat.Controls.Add(this.label2);
            this.private_chat.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.private_chat.Location = new System.Drawing.Point(22, 53);
            this.private_chat.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.private_chat.Name = "private_chat";
            this.private_chat.Size = new System.Drawing.Size(305, 380);
            this.private_chat.TabIndex = 0;
            this.private_chat.WrapContents = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "私聊模式";
            // 
            // public_chat
            // 
            this.public_chat.AutoScroll = true;
            this.public_chat.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.public_chat.Controls.Add(this.label1);
            this.public_chat.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.public_chat.Location = new System.Drawing.Point(47, 31);
            this.public_chat.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.public_chat.Name = "public_chat";
            this.public_chat.Size = new System.Drawing.Size(306, 381);
            this.public_chat.TabIndex = 0;
            this.public_chat.Visible = false;
            this.public_chat.WrapContents = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "群聊模式";
            // 
            // request
            // 
            this.request.BackColor = System.Drawing.Color.Tan;
            this.request.Controls.Add(this.label3);
            this.request.Location = new System.Drawing.Point(75, 3);
            this.request.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.request.Name = "request";
            this.request.Size = new System.Drawing.Size(305, 380);
            this.request.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "验证消息";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(214, 69);
            this.button3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(94, 29);
            this.button3.TabIndex = 2;
            this.button3.Text = "添加好友";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.btn_addfriend);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(214, 35);
            this.button4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(94, 29);
            this.button4.TabIndex = 3;
            this.button4.Text = "验证消息";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.btn_requestmode);
            // 
            // btn_profile
            // 
            this.btn_profile.Location = new System.Drawing.Point(214, 6);
            this.btn_profile.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btn_profile.Name = "btn_profile";
            this.btn_profile.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btn_profile.Size = new System.Drawing.Size(94, 24);
            this.btn_profile.TabIndex = 4;
            this.btn_profile.Text = "个人简介";
            this.btn_profile.UseVisualStyleBackColor = true;
            this.btn_profile.Click += new System.EventHandler(this.btn_profile_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(116, 69);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(92, 29);
            this.button5.TabIndex = 5;
            this.button5.Text = "创建群聊";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(116, 35);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(92, 28);
            this.button6.TabIndex = 6;
            this.button6.Text = "搜索群组";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // user
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(310, 523);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.btn_profile);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.paneltab);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "user";
            this.Text = "user";
            this.paneltab.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.private_chat.ResumeLayout(false);
            this.private_chat.PerformLayout();
            this.public_chat.ResumeLayout(false);
            this.public_chat.PerformLayout();
            this.request.ResumeLayout(false);
            this.request.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel paneltab;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.FlowLayoutPanel private_chat;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.FlowLayoutPanel public_chat;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.FlowLayoutPanel request;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button btn_profile;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
    }
}