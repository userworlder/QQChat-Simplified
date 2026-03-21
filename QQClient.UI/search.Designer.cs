namespace QQClient.UI
{
    partial class search
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
            this.pnl_select = new System.Windows.Forms.Panel();
            this.btn_groupmode = new System.Windows.Forms.Button();
            this.btn_friendmode = new System.Windows.Forms.Button();
            this.pnl_search = new System.Windows.Forms.Panel();
            this.pnl_addgroup = new System.Windows.Forms.Panel();
            this.lbl_groupwarn = new System.Windows.Forms.Label();
            this.btn_addgroup = new System.Windows.Forms.Button();
            this.txt_group = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.pnl_addfriend = new System.Windows.Forms.Panel();
            this.lbl_friendwarn = new System.Windows.Forms.Label();
            this.btn_addfriend = new System.Windows.Forms.Button();
            this.txt_friend = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pnl_select.SuspendLayout();
            this.pnl_search.SuspendLayout();
            this.pnl_addgroup.SuspendLayout();
            this.pnl_addfriend.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnl_select
            // 
            this.pnl_select.Controls.Add(this.btn_groupmode);
            this.pnl_select.Controls.Add(this.btn_friendmode);
            this.pnl_select.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnl_select.Location = new System.Drawing.Point(0, 0);
            this.pnl_select.Name = "pnl_select";
            this.pnl_select.Size = new System.Drawing.Size(553, 55);
            this.pnl_select.TabIndex = 0;
            // 
            // btn_groupmode
            // 
            this.btn_groupmode.Location = new System.Drawing.Point(276, 0);
            this.btn_groupmode.Name = "btn_groupmode";
            this.btn_groupmode.Size = new System.Drawing.Size(277, 56);
            this.btn_groupmode.TabIndex = 1;
            this.btn_groupmode.Text = "找群";
            this.btn_groupmode.UseVisualStyleBackColor = true;
            this.btn_groupmode.Click += new System.EventHandler(this.btn_groupmode_Click);
            // 
            // btn_friendmode
            // 
            this.btn_friendmode.Location = new System.Drawing.Point(0, 1);
            this.btn_friendmode.Name = "btn_friendmode";
            this.btn_friendmode.Size = new System.Drawing.Size(278, 55);
            this.btn_friendmode.TabIndex = 0;
            this.btn_friendmode.Text = "找人";
            this.btn_friendmode.UseVisualStyleBackColor = true;
            this.btn_friendmode.Click += new System.EventHandler(this.btn_friendmode_Click);
            // 
            // pnl_search
            // 
            this.pnl_search.Controls.Add(this.pnl_addgroup);
            this.pnl_search.Controls.Add(this.pnl_addfriend);
            this.pnl_search.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_search.Location = new System.Drawing.Point(0, 55);
            this.pnl_search.Name = "pnl_search";
            this.pnl_search.Size = new System.Drawing.Size(553, 231);
            this.pnl_search.TabIndex = 1;
            // 
            // pnl_addgroup
            // 
            this.pnl_addgroup.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.pnl_addgroup.Controls.Add(this.lbl_groupwarn);
            this.pnl_addgroup.Controls.Add(this.btn_addgroup);
            this.pnl_addgroup.Controls.Add(this.txt_group);
            this.pnl_addgroup.Controls.Add(this.label2);
            this.pnl_addgroup.Location = new System.Drawing.Point(22, 107);
            this.pnl_addgroup.Name = "pnl_addgroup";
            this.pnl_addgroup.Size = new System.Drawing.Size(547, 224);
            this.pnl_addgroup.TabIndex = 2;
            // 
            // lbl_groupwarn
            // 
            this.lbl_groupwarn.AutoSize = true;
            this.lbl_groupwarn.ForeColor = System.Drawing.Color.Red;
            this.lbl_groupwarn.Location = new System.Drawing.Point(379, 88);
            this.lbl_groupwarn.Name = "lbl_groupwarn";
            this.lbl_groupwarn.Size = new System.Drawing.Size(62, 18);
            this.lbl_groupwarn.TabIndex = 5;
            this.lbl_groupwarn.Text = "label3";
            this.lbl_groupwarn.Visible = false;
            // 
            // btn_addgroup
            // 
            this.btn_addgroup.Location = new System.Drawing.Point(439, 44);
            this.btn_addgroup.Name = "btn_addgroup";
            this.btn_addgroup.Size = new System.Drawing.Size(75, 28);
            this.btn_addgroup.TabIndex = 4;
            this.btn_addgroup.Text = "加入";
            this.btn_addgroup.UseVisualStyleBackColor = true;
            this.btn_addgroup.Click += new System.EventHandler(this.btn_addgroup_Click);
            // 
            // txt_group
            // 
            this.txt_group.Location = new System.Drawing.Point(149, 44);
            this.txt_group.Name = "txt_group";
            this.txt_group.Size = new System.Drawing.Size(271, 28);
            this.txt_group.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(81, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 18);
            this.label2.TabIndex = 2;
            this.label2.Text = "群组ID";
            // 
            // pnl_addfriend
            // 
            this.pnl_addfriend.BackColor = System.Drawing.SystemColors.Info;
            this.pnl_addfriend.Controls.Add(this.lbl_friendwarn);
            this.pnl_addfriend.Controls.Add(this.btn_addfriend);
            this.pnl_addfriend.Controls.Add(this.txt_friend);
            this.pnl_addfriend.Controls.Add(this.label1);
            this.pnl_addfriend.Location = new System.Drawing.Point(66, 20);
            this.pnl_addfriend.Name = "pnl_addfriend";
            this.pnl_addfriend.Size = new System.Drawing.Size(553, 232);
            this.pnl_addfriend.TabIndex = 0;
            // 
            // lbl_friendwarn
            // 
            this.lbl_friendwarn.AutoSize = true;
            this.lbl_friendwarn.ForeColor = System.Drawing.Color.Red;
            this.lbl_friendwarn.Location = new System.Drawing.Point(379, 88);
            this.lbl_friendwarn.Name = "lbl_friendwarn";
            this.lbl_friendwarn.Size = new System.Drawing.Size(62, 18);
            this.lbl_friendwarn.TabIndex = 4;
            this.lbl_friendwarn.Text = "label3";
            this.lbl_friendwarn.Visible = false;
            // 
            // btn_addfriend
            // 
            this.btn_addfriend.Location = new System.Drawing.Point(439, 44);
            this.btn_addfriend.Name = "btn_addfriend";
            this.btn_addfriend.Size = new System.Drawing.Size(75, 28);
            this.btn_addfriend.TabIndex = 3;
            this.btn_addfriend.Text = "申请";
            this.btn_addfriend.UseVisualStyleBackColor = true;
            this.btn_addfriend.Click += new System.EventHandler(this.btn_addfriend_Click);
            // 
            // txt_friend
            // 
            this.txt_friend.Location = new System.Drawing.Point(149, 44);
            this.txt_friend.Name = "txt_friend";
            this.txt_friend.Size = new System.Drawing.Size(260, 28);
            this.txt_friend.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(99, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "账号";
            // 
            // search
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(553, 286);
            this.Controls.Add(this.pnl_search);
            this.Controls.Add(this.pnl_select);
            this.Name = "search";
            this.Text = "search";
            this.pnl_select.ResumeLayout(false);
            this.pnl_search.ResumeLayout(false);
            this.pnl_addgroup.ResumeLayout(false);
            this.pnl_addgroup.PerformLayout();
            this.pnl_addfriend.ResumeLayout(false);
            this.pnl_addfriend.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnl_select;
        private System.Windows.Forms.Button btn_groupmode;
        private System.Windows.Forms.Button btn_friendmode;
        private System.Windows.Forms.Panel pnl_search;
        private System.Windows.Forms.Panel pnl_addgroup;
        private System.Windows.Forms.Panel pnl_addfriend;
        private System.Windows.Forms.TextBox txt_friend;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_group;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_addfriend;
        private System.Windows.Forms.Button btn_addgroup;
        private System.Windows.Forms.Label lbl_groupwarn;
        private System.Windows.Forms.Label lbl_friendwarn;
    }
}