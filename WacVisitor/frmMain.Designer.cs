namespace WacVisitor
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.label1 = new System.Windows.Forms.Label();
            this.panel_top = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnSetting = new System.Windows.Forms.Button();
            this.btnView = new System.Windows.Forms.Button();
            this.btnHome = new System.Windows.Forms.Button();
            this.panel_main = new System.Windows.Forms.Panel();
            this.lbTime = new System.Windows.Forms.Label();
            this.panel_bottom = new System.Windows.Forms.Panel();
            this.rbtnOUT = new WacVisitor.RoundButton();
            this.rbtnIN = new WacVisitor.RoundButton();
            this.btnExit1 = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.panel4 = new System.Windows.Forms.Panel();
            this.btnAppointment = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnMoney = new System.Windows.Forms.Button();
            this.btnWEBCAM = new System.Windows.Forms.Button();
            this.pbIcon = new System.Windows.Forms.PictureBox();
            this.panel_top.SuspendLayout();
            this.panel_bottom.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(15, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 45);
            this.label1.TabIndex = 12;
            this.label1.Text = "VISITOR";
            // 
            // panel_top
            // 
            this.panel_top.BackColor = System.Drawing.SystemColors.HotTrack;
            this.panel_top.Controls.Add(this.button1);
            this.panel_top.Controls.Add(this.label1);
            this.panel_top.Controls.Add(this.btnExit);
            this.panel_top.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_top.Location = new System.Drawing.Point(97, 0);
            this.panel_top.Name = "panel_top";
            this.panel_top.Size = new System.Drawing.Size(1149, 86);
            this.panel_top.TabIndex = 9;
            this.panel_top.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel_top_MouseMove);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(518, 30);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 13;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnExit.BackgroundImage = global::WacVisitor.Properties.Resources.poweroff;
            this.btnExit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnExit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExit.FlatAppearance.BorderSize = 0;
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnExit.ForeColor = System.Drawing.Color.White;
            this.btnExit.Location = new System.Drawing.Point(999, 12);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(64, 64);
            this.btnExit.TabIndex = 9;
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnExport.BackgroundImage = global::WacVisitor.Properties.Resources.export;
            this.btnExport.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnExport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExport.FlatAppearance.BorderSize = 0;
            this.btnExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnExport.ForeColor = System.Drawing.Color.White;
            this.btnExport.Location = new System.Drawing.Point(19, 392);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(64, 64);
            this.btnExport.TabIndex = 17;
            this.btnExport.UseVisualStyleBackColor = false;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnSetting
            // 
            this.btnSetting.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnSetting.BackgroundImage = global::WacVisitor.Properties.Resources.setting;
            this.btnSetting.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSetting.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSetting.FlatAppearance.BorderSize = 0;
            this.btnSetting.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnSetting.ForeColor = System.Drawing.Color.White;
            this.btnSetting.Location = new System.Drawing.Point(19, 322);
            this.btnSetting.Name = "btnSetting";
            this.btnSetting.Size = new System.Drawing.Size(64, 64);
            this.btnSetting.TabIndex = 16;
            this.btnSetting.UseVisualStyleBackColor = false;
            this.btnSetting.Click += new System.EventHandler(this.btnSetting_Click);
            // 
            // btnView
            // 
            this.btnView.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnView.BackgroundImage = global::WacVisitor.Properties.Resources.view;
            this.btnView.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnView.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnView.FlatAppearance.BorderSize = 0;
            this.btnView.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnView.ForeColor = System.Drawing.Color.White;
            this.btnView.Location = new System.Drawing.Point(19, 252);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(64, 64);
            this.btnView.TabIndex = 14;
            this.btnView.UseVisualStyleBackColor = false;
            this.btnView.Click += new System.EventHandler(this.btnView_Click);
            // 
            // btnHome
            // 
            this.btnHome.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnHome.BackgroundImage = global::WacVisitor.Properties.Resources.home;
            this.btnHome.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnHome.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnHome.FlatAppearance.BorderSize = 0;
            this.btnHome.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnHome.ForeColor = System.Drawing.Color.White;
            this.btnHome.Location = new System.Drawing.Point(19, 112);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(66, 64);
            this.btnHome.TabIndex = 13;
            this.btnHome.UseVisualStyleBackColor = false;
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);
            // 
            // panel_main
            // 
            this.panel_main.BackColor = System.Drawing.SystemColors.HotTrack;
            this.panel_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_main.Location = new System.Drawing.Point(97, 86);
            this.panel_main.Name = "panel_main";
            this.panel_main.Size = new System.Drawing.Size(1149, 514);
            this.panel_main.TabIndex = 10;
            // 
            // lbTime
            // 
            this.lbTime.AutoSize = true;
            this.lbTime.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lbTime.ForeColor = System.Drawing.Color.White;
            this.lbTime.Location = new System.Drawing.Point(1032, 66);
            this.lbTime.Name = "lbTime";
            this.lbTime.Size = new System.Drawing.Size(84, 32);
            this.lbTime.TabIndex = 7;
            this.lbTime.Text = "label2";
            this.lbTime.Visible = false;
            // 
            // panel_bottom
            // 
            this.panel_bottom.BackColor = System.Drawing.SystemColors.HotTrack;
            this.panel_bottom.Controls.Add(this.rbtnOUT);
            this.panel_bottom.Controls.Add(this.rbtnIN);
            this.panel_bottom.Controls.Add(this.btnExit1);
            this.panel_bottom.Controls.Add(this.progressBar1);
            this.panel_bottom.Controls.Add(this.lbTime);
            this.panel_bottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel_bottom.Location = new System.Drawing.Point(0, 600);
            this.panel_bottom.Name = "panel_bottom";
            this.panel_bottom.Size = new System.Drawing.Size(1246, 107);
            this.panel_bottom.TabIndex = 11;
            // 
            // rbtnOUT
            // 
            this.rbtnOUT.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbtnOUT.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.rbtnOUT.ForeColor = System.Drawing.Color.Blue;
            this.rbtnOUT.Location = new System.Drawing.Point(625, 9);
            this.rbtnOUT.Name = "rbtnOUT";
            this.rbtnOUT.Size = new System.Drawing.Size(191, 89);
            this.rbtnOUT.TabIndex = 15;
            this.rbtnOUT.Text = "ออก";
            this.rbtnOUT.UseVisualStyleBackColor = true;
            this.rbtnOUT.Click += new System.EventHandler(this.rbtnOUT_Click);
            // 
            // rbtnIN
            // 
            this.rbtnIN.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbtnIN.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            this.rbtnIN.ForeColor = System.Drawing.Color.Blue;
            this.rbtnIN.Location = new System.Drawing.Point(428, 9);
            this.rbtnIN.Name = "rbtnIN";
            this.rbtnIN.Size = new System.Drawing.Size(191, 89);
            this.rbtnIN.TabIndex = 14;
            this.rbtnIN.Text = "เข้า";
            this.rbtnIN.UseVisualStyleBackColor = true;
            this.rbtnIN.Click += new System.EventHandler(this.rbtnIN_Click);
            // 
            // btnExit1
            // 
            this.btnExit1.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnExit1.BackgroundImage = global::WacVisitor.Properties.Resources.poweroff;
            this.btnExit1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnExit1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExit1.FlatAppearance.BorderSize = 0;
            this.btnExit1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnExit1.ForeColor = System.Drawing.Color.White;
            this.btnExit1.Location = new System.Drawing.Point(17, 6);
            this.btnExit1.Name = "btnExit1";
            this.btnExit1.Size = new System.Drawing.Size(64, 64);
            this.btnExit1.TabIndex = 13;
            this.btnExit1.UseVisualStyleBackColor = false;
            this.btnExit1.Visible = false;
            this.btnExit1.Click += new System.EventHandler(this.btnExit1_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(14, 80);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(350, 23);
            this.progressBar1.TabIndex = 10;
            this.progressBar1.Visible = false;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.HotTrack;
            this.panel4.Controls.Add(this.btnAppointment);
            this.panel4.Controls.Add(this.pictureBox1);
            this.panel4.Controls.Add(this.btnMoney);
            this.panel4.Controls.Add(this.btnWEBCAM);
            this.panel4.Controls.Add(this.btnExport);
            this.panel4.Controls.Add(this.pbIcon);
            this.panel4.Controls.Add(this.btnHome);
            this.panel4.Controls.Add(this.btnSetting);
            this.panel4.Controls.Add(this.btnView);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(97, 600);
            this.panel4.TabIndex = 12;
            // 
            // btnAppointment
            // 
            this.btnAppointment.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnAppointment.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnAppointment.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAppointment.FlatAppearance.BorderSize = 0;
            this.btnAppointment.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnAppointment.ForeColor = System.Drawing.Color.White;
            this.btnAppointment.Location = new System.Drawing.Point(19, 531);
            this.btnAppointment.Name = "btnAppointment";
            this.btnAppointment.Size = new System.Drawing.Size(64, 64);
            this.btnAppointment.TabIndex = 23;
            this.btnAppointment.UseVisualStyleBackColor = false;
            this.btnAppointment.Click += new System.EventHandler(this.BtnAppointment_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.HighlightText;
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.Location = new System.Drawing.Point(3, 431);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(88, 71);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 22;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // btnMoney
            // 
            this.btnMoney.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnMoney.BackgroundImage = global::WacVisitor.Properties.Resources.money;
            this.btnMoney.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnMoney.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMoney.FlatAppearance.BorderSize = 0;
            this.btnMoney.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnMoney.ForeColor = System.Drawing.Color.White;
            this.btnMoney.Location = new System.Drawing.Point(19, 461);
            this.btnMoney.Name = "btnMoney";
            this.btnMoney.Size = new System.Drawing.Size(64, 64);
            this.btnMoney.TabIndex = 20;
            this.btnMoney.UseVisualStyleBackColor = false;
            this.btnMoney.Click += new System.EventHandler(this.btnMoney_Click);
            // 
            // btnWEBCAM
            // 
            this.btnWEBCAM.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnWEBCAM.BackgroundImage = global::WacVisitor.Properties.Resources.webcam;
            this.btnWEBCAM.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnWEBCAM.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnWEBCAM.FlatAppearance.BorderSize = 0;
            this.btnWEBCAM.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnWEBCAM.ForeColor = System.Drawing.Color.White;
            this.btnWEBCAM.Location = new System.Drawing.Point(19, 182);
            this.btnWEBCAM.Name = "btnWEBCAM";
            this.btnWEBCAM.Size = new System.Drawing.Size(64, 64);
            this.btnWEBCAM.TabIndex = 19;
            this.btnWEBCAM.UseVisualStyleBackColor = false;
            this.btnWEBCAM.Click += new System.EventHandler(this.btnWEBCAM_Click);
            // 
            // pbIcon
            // 
            this.pbIcon.Location = new System.Drawing.Point(20, 15);
            this.pbIcon.Name = "pbIcon";
            this.pbIcon.Size = new System.Drawing.Size(64, 62);
            this.pbIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbIcon.TabIndex = 12;
            this.pbIcon.TabStop = false;
            this.pbIcon.Click += new System.EventHandler(this.pbIcon_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1246, 707);
            this.Controls.Add(this.panel_main);
            this.Controls.Add(this.panel_top);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel_bottom);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMain";
            this.Text = "frmMain";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.panel_top.ResumeLayout(false);
            this.panel_top.PerformLayout();
            this.panel_bottom.ResumeLayout(false);
            this.panel_bottom.PerformLayout();
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pbIcon;
        private System.Windows.Forms.Panel panel_top;
        private System.Windows.Forms.Button btnExit;
        public System.Windows.Forms.Panel panel_main;
        private System.Windows.Forms.Label lbTime;
        private System.Windows.Forms.Panel panel_bottom;
        private System.Windows.Forms.Button btnHome;
        private System.Windows.Forms.Button btnView;
        private System.Windows.Forms.Button btnSetting;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnWEBCAM;
        private System.Windows.Forms.Button btnMoney;
        private System.Windows.Forms.Button btnExit1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private RoundButton rbtnOUT;
        private RoundButton rbtnIN;
        private System.Windows.Forms.Button btnAppointment;
        private System.Windows.Forms.Button button1;
    }
}