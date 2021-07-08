namespace WacVisitor
{
    partial class frmHWDevices
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
            this.panel_group_2 = new System.Windows.Forms.Panel();
            this.lbFN = new System.Windows.Forms.Label();
            this.txtFullname = new System.Windows.Forms.TextBox();
            this.lbID = new System.Windows.Forms.Label();
            this.txtID = new System.Windows.Forms.TextBox();
            this.lb1 = new System.Windows.Forms.Label();
            this.lb3Text = new System.Windows.Forms.Label();
            this.lb3 = new System.Windows.Forms.Label();
            this.lb4 = new System.Windows.Forms.Label();
            this.lb2Text = new System.Windows.Forms.Label();
            this.lb2 = new System.Windows.Forms.Label();
            this.lb1Text = new System.Windows.Forms.Label();
            this.lb4Text = new System.Windows.Forms.Label();
            this.btnLicense = new System.Windows.Forms.Button();
            this.btnMoreInfo = new System.Windows.Forms.Button();
            this.txtVisitorNumber = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lbMessage = new System.Windows.Forms.Label();
            this.panel_group_1 = new System.Windows.Forms.Panel();
            this.picWebcam = new System.Windows.Forms.PictureBox();
            this.picDocument = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.btnIPCAM = new System.Windows.Forms.Button();
            this.timerDelay = new System.Windows.Forms.Timer(this.components);
            this.btnIDCard = new System.Windows.Forms.Button();
            this.lbKeyboardLayout = new System.Windows.Forms.LinkLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rbtnReprint = new WacVisitor.RoundButton();
            this.rbtnOK = new WacVisitor.RoundButton();
            this.panel_group_2.SuspendLayout();
            this.panel_group_1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picWebcam)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDocument)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_group_2
            // 
            this.panel_group_2.Controls.Add(this.lbFN);
            this.panel_group_2.Controls.Add(this.txtFullname);
            this.panel_group_2.Controls.Add(this.lbID);
            this.panel_group_2.Controls.Add(this.txtID);
            this.panel_group_2.Controls.Add(this.lb1);
            this.panel_group_2.Controls.Add(this.lb3Text);
            this.panel_group_2.Controls.Add(this.lb3);
            this.panel_group_2.Controls.Add(this.lb4);
            this.panel_group_2.Controls.Add(this.lb2Text);
            this.panel_group_2.Controls.Add(this.lb2);
            this.panel_group_2.Controls.Add(this.lb1Text);
            this.panel_group_2.Controls.Add(this.lb4Text);
            this.panel_group_2.Location = new System.Drawing.Point(254, 331);
            this.panel_group_2.Name = "panel_group_2";
            this.panel_group_2.Size = new System.Drawing.Size(606, 206);
            this.panel_group_2.TabIndex = 31;
            // 
            // lbFN
            // 
            this.lbFN.AutoSize = true;
            this.lbFN.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lbFN.ForeColor = System.Drawing.Color.White;
            this.lbFN.Location = new System.Drawing.Point(15, 45);
            this.lbFN.Name = "lbFN";
            this.lbFN.Size = new System.Drawing.Size(127, 25);
            this.lbFN.TabIndex = 35;
            this.lbFN.Text = "ชื่อ-นามสกุล :";
            // 
            // txtFullname
            // 
            this.txtFullname.BackColor = System.Drawing.Color.Moccasin;
            this.txtFullname.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.txtFullname.Location = new System.Drawing.Point(144, 45);
            this.txtFullname.Name = "txtFullname";
            this.txtFullname.Size = new System.Drawing.Size(247, 29);
            this.txtFullname.TabIndex = 43;
            this.txtFullname.TextChanged += new System.EventHandler(this.TxtFullname_TextChanged);
            // 
            // lbID
            // 
            this.lbID.AutoSize = true;
            this.lbID.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lbID.ForeColor = System.Drawing.Color.White;
            this.lbID.Location = new System.Drawing.Point(15, 14);
            this.lbID.Name = "lbID";
            this.lbID.Size = new System.Drawing.Size(126, 25);
            this.lbID.TabIndex = 34;
            this.lbID.Text = "เลขประจำตัว :";
            // 
            // txtID
            // 
            this.txtID.BackColor = System.Drawing.Color.Moccasin;
            this.txtID.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.txtID.Location = new System.Drawing.Point(144, 13);
            this.txtID.Name = "txtID";
            this.txtID.Size = new System.Drawing.Size(247, 29);
            this.txtID.TabIndex = 33;
            this.txtID.TextChanged += new System.EventHandler(this.TxtID_TextChanged);
            // 
            // lb1
            // 
            this.lb1.AutoSize = true;
            this.lb1.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lb1.ForeColor = System.Drawing.Color.White;
            this.lb1.Location = new System.Drawing.Point(12, 74);
            this.lb1.Name = "lb1";
            this.lb1.Size = new System.Drawing.Size(131, 25);
            this.lb1.TabIndex = 22;
            this.lb1.Text = "เลข VISITOR :";
            // 
            // lb3Text
            // 
            this.lb3Text.AutoSize = true;
            this.lb3Text.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lb3Text.ForeColor = System.Drawing.Color.White;
            this.lb3Text.Location = new System.Drawing.Point(145, 162);
            this.lb3Text.Name = "lb3Text";
            this.lb3Text.Size = new System.Drawing.Size(20, 25);
            this.lb3Text.TabIndex = 42;
            this.lb3Text.Text = "-";
            // 
            // lb3
            // 
            this.lb3.AutoSize = true;
            this.lb3.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lb3.ForeColor = System.Drawing.Color.White;
            this.lb3.Location = new System.Drawing.Point(49, 163);
            this.lb3.Name = "lb3";
            this.lb3.Size = new System.Drawing.Size(94, 25);
            this.lb3.TabIndex = 24;
            this.lb3.Text = "เวลาออก :";
            // 
            // lb4
            // 
            this.lb4.AutoSize = true;
            this.lb4.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lb4.ForeColor = System.Drawing.Color.White;
            this.lb4.Location = new System.Drawing.Point(61, 105);
            this.lb4.Name = "lb4";
            this.lb4.Size = new System.Drawing.Size(82, 25);
            this.lb4.TabIndex = 25;
            this.lb4.Text = "ประเภท :";
            // 
            // lb2Text
            // 
            this.lb2Text.AutoSize = true;
            this.lb2Text.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lb2Text.ForeColor = System.Drawing.Color.White;
            this.lb2Text.Location = new System.Drawing.Point(145, 133);
            this.lb2Text.Name = "lb2Text";
            this.lb2Text.Size = new System.Drawing.Size(20, 25);
            this.lb2Text.TabIndex = 41;
            this.lb2Text.Text = "-";
            // 
            // lb2
            // 
            this.lb2.AutoSize = true;
            this.lb2.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lb2.ForeColor = System.Drawing.Color.White;
            this.lb2.Location = new System.Drawing.Point(53, 134);
            this.lb2.Name = "lb2";
            this.lb2.Size = new System.Drawing.Size(90, 25);
            this.lb2.TabIndex = 23;
            this.lb2.Text = "เวลาเข้า :";
            // 
            // lb1Text
            // 
            this.lb1Text.AutoSize = true;
            this.lb1Text.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lb1Text.ForeColor = System.Drawing.Color.White;
            this.lb1Text.Location = new System.Drawing.Point(145, 73);
            this.lb1Text.Name = "lb1Text";
            this.lb1Text.Size = new System.Drawing.Size(20, 25);
            this.lb1Text.TabIndex = 39;
            this.lb1Text.Text = "-";
            // 
            // lb4Text
            // 
            this.lb4Text.AutoSize = true;
            this.lb4Text.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lb4Text.ForeColor = System.Drawing.Color.White;
            this.lb4Text.Location = new System.Drawing.Point(145, 104);
            this.lb4Text.Name = "lb4Text";
            this.lb4Text.Size = new System.Drawing.Size(20, 25);
            this.lb4Text.TabIndex = 40;
            this.lb4Text.Text = "-";
            // 
            // btnLicense
            // 
            this.btnLicense.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnLicense.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnLicense.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLicense.FlatAppearance.BorderSize = 0;
            this.btnLicense.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnLicense.ForeColor = System.Drawing.Color.White;
            this.btnLicense.Location = new System.Drawing.Point(3, 87);
            this.btnLicense.Name = "btnLicense";
            this.btnLicense.Size = new System.Drawing.Size(153, 80);
            this.btnLicense.TabIndex = 45;
            this.btnLicense.UseVisualStyleBackColor = false;
            this.btnLicense.Click += new System.EventHandler(this.btnLicense_Click);
            // 
            // btnMoreInfo
            // 
            this.btnMoreInfo.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnMoreInfo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnMoreInfo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMoreInfo.FlatAppearance.BorderSize = 0;
            this.btnMoreInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnMoreInfo.ForeColor = System.Drawing.Color.White;
            this.btnMoreInfo.Location = new System.Drawing.Point(3, 171);
            this.btnMoreInfo.Name = "btnMoreInfo";
            this.btnMoreInfo.Size = new System.Drawing.Size(153, 83);
            this.btnMoreInfo.TabIndex = 47;
            this.btnMoreInfo.UseVisualStyleBackColor = false;
            this.btnMoreInfo.Click += new System.EventHandler(this.btnMoreInfo_Click);
            // 
            // txtVisitorNumber
            // 
            this.txtVisitorNumber.BackColor = System.Drawing.Color.Moccasin;
            this.txtVisitorNumber.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.txtVisitorNumber.Location = new System.Drawing.Point(254, 282);
            this.txtVisitorNumber.Name = "txtVisitorNumber";
            this.txtVisitorNumber.Size = new System.Drawing.Size(422, 43);
            this.txtVisitorNumber.TabIndex = 3;
            this.txtVisitorNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtVisitorNumber.TextChanged += new System.EventHandler(this.txtVisitorNumber_TextChanged);
            this.txtVisitorNumber.Enter += new System.EventHandler(this.txtVisitorNumber_Enter);
            this.txtVisitorNumber.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVisitorNumber_KeyDown);
            this.txtVisitorNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtVisitorNumber_KeyPress);
            this.txtVisitorNumber.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtVisitorNumber_KeyUp);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(9, 11);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(101, 10);
            this.progressBar1.TabIndex = 32;
            this.progressBar1.Visible = false;
            // 
            // lbMessage
            // 
            this.lbMessage.AutoSize = true;
            this.lbMessage.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lbMessage.ForeColor = System.Drawing.Color.White;
            this.lbMessage.Location = new System.Drawing.Point(866, 280);
            this.lbMessage.Name = "lbMessage";
            this.lbMessage.Size = new System.Drawing.Size(62, 30);
            this.lbMessage.TabIndex = 26;
            this.lbMessage.Text = "MSG";
            this.lbMessage.Visible = false;
            // 
            // panel_group_1
            // 
            this.panel_group_1.Controls.Add(this.progressBar1);
            this.panel_group_1.Controls.Add(this.picWebcam);
            this.panel_group_1.Controls.Add(this.picDocument);
            this.panel_group_1.Location = new System.Drawing.Point(70, 9);
            this.panel_group_1.Name = "panel_group_1";
            this.panel_group_1.Size = new System.Drawing.Size(790, 267);
            this.panel_group_1.TabIndex = 30;
            // 
            // picWebcam
            // 
            this.picWebcam.BackColor = System.Drawing.Color.White;
            this.picWebcam.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picWebcam.Location = new System.Drawing.Point(398, 7);
            this.picWebcam.Name = "picWebcam";
            this.picWebcam.Size = new System.Drawing.Size(387, 250);
            this.picWebcam.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picWebcam.TabIndex = 16;
            this.picWebcam.TabStop = false;
            this.picWebcam.Click += new System.EventHandler(this.picWebcam_Click);
            // 
            // picDocument
            // 
            this.picDocument.BackColor = System.Drawing.Color.White;
            this.picDocument.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.picDocument.Location = new System.Drawing.Point(6, 7);
            this.picDocument.Name = "picDocument";
            this.picDocument.Size = new System.Drawing.Size(387, 250);
            this.picDocument.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picDocument.TabIndex = 1;
            this.picDocument.TabStop = false;
            this.picDocument.Click += new System.EventHandler(this.picDocument_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // listBox1
            // 
            this.listBox1.BackColor = System.Drawing.Color.White;
            this.listBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBox1.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 25;
            this.listBox1.Location = new System.Drawing.Point(861, 16);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(231, 250);
            this.listBox1.TabIndex = 32;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            this.listBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBox1_KeyDown);
            // 
            // btnIPCAM
            // 
            this.btnIPCAM.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnIPCAM.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnIPCAM.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.btnIPCAM.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnIPCAM.ForeColor = System.Drawing.Color.Blue;
            this.btnIPCAM.Location = new System.Drawing.Point(-39, 15);
            this.btnIPCAM.Name = "btnIPCAM";
            this.btnIPCAM.Size = new System.Drawing.Size(103, 92);
            this.btnIPCAM.TabIndex = 64;
            this.btnIPCAM.Text = "IP CAM";
            this.btnIPCAM.UseVisualStyleBackColor = true;
            this.btnIPCAM.Click += new System.EventHandler(this.btnIPCAM_Click);
            // 
            // timerDelay
            // 
            this.timerDelay.Interval = 1500;
            this.timerDelay.Tick += new System.EventHandler(this.TimerDelay_Tick);
            // 
            // btnIDCard
            // 
            this.btnIDCard.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnIDCard.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnIDCard.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnIDCard.FlatAppearance.BorderSize = 0;
            this.btnIDCard.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnIDCard.ForeColor = System.Drawing.Color.White;
            this.btnIDCard.Location = new System.Drawing.Point(3, 3);
            this.btnIDCard.Name = "btnIDCard";
            this.btnIDCard.Size = new System.Drawing.Size(153, 81);
            this.btnIDCard.TabIndex = 45;
            this.btnIDCard.UseVisualStyleBackColor = false;
            // 
            // lbKeyboardLayout
            // 
            this.lbKeyboardLayout.AutoSize = true;
            this.lbKeyboardLayout.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lbKeyboardLayout.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lbKeyboardLayout.LinkColor = System.Drawing.Color.White;
            this.lbKeyboardLayout.Location = new System.Drawing.Point(691, 290);
            this.lbKeyboardLayout.Name = "lbKeyboardLayout";
            this.lbKeyboardLayout.Size = new System.Drawing.Size(92, 29);
            this.lbKeyboardLayout.TabIndex = 63;
            this.lbKeyboardLayout.TabStop = true;
            this.lbKeyboardLayout.Text = "TH/EN";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnIDCard);
            this.panel1.Controls.Add(this.btnLicense);
            this.panel1.Controls.Add(this.btnMoreInfo);
            this.panel1.Location = new System.Drawing.Point(70, 278);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(163, 267);
            this.panel1.TabIndex = 67;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rbtnReprint);
            this.panel2.Controls.Add(this.rbtnOK);
            this.panel2.Location = new System.Drawing.Point(861, 331);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(242, 207);
            this.panel2.TabIndex = 68;
            // 
            // rbtnReprint
            // 
            this.rbtnReprint.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbtnReprint.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.rbtnReprint.ForeColor = System.Drawing.Color.Blue;
            this.rbtnReprint.Location = new System.Drawing.Point(3, 3);
            this.rbtnReprint.Name = "rbtnReprint";
            this.rbtnReprint.Size = new System.Drawing.Size(226, 65);
            this.rbtnReprint.TabIndex = 66;
            this.rbtnReprint.Text = "Reprint";
            this.rbtnReprint.UseVisualStyleBackColor = true;
            this.rbtnReprint.Click += new System.EventHandler(this.rbtnReprint_Click);
            // 
            // rbtnOK
            // 
            this.rbtnOK.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbtnOK.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.rbtnOK.ForeColor = System.Drawing.Color.Blue;
            this.rbtnOK.Location = new System.Drawing.Point(3, 78);
            this.rbtnOK.Name = "rbtnOK";
            this.rbtnOK.Size = new System.Drawing.Size(226, 110);
            this.rbtnOK.TabIndex = 65;
            this.rbtnOK.Text = "OK";
            this.rbtnOK.UseVisualStyleBackColor = true;
            this.rbtnOK.Click += new System.EventHandler(this.rbtnOK_Click);
            // 
            // frmHWDevices
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.HotTrack;
            this.ClientSize = new System.Drawing.Size(1230, 706);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lbKeyboardLayout);
            this.Controls.Add(this.txtVisitorNumber);
            this.Controls.Add(this.lbMessage);
            this.Controls.Add(this.btnIPCAM);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.panel_group_1);
            this.Controls.Add(this.panel_group_2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmHWDevices";
            this.Text = "Loading...";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SubFormDevice_FormClosing);
            this.Load += new System.EventHandler(this.SubFormDevice_Load);
            this.Resize += new System.EventHandler(this.SubFormDevice_Resize);
            this.panel_group_2.ResumeLayout(false);
            this.panel_group_2.PerformLayout();
            this.panel_group_1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picWebcam)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDocument)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel_group_2;
        public System.Windows.Forms.TextBox txtVisitorNumber;
        public System.Windows.Forms.Label lb1;
        public System.Windows.Forms.Label lb3;
        public System.Windows.Forms.Label lb2;
        private System.Windows.Forms.Panel panel_group_1;
        public System.Windows.Forms.PictureBox picWebcam;
        public System.Windows.Forms.PictureBox picDocument;
        public System.Windows.Forms.Label lbMessage;
        private System.Windows.Forms.ProgressBar progressBar1;
        public System.Windows.Forms.Label lb4;
        private System.Windows.Forms.Timer timer1;
        public System.Windows.Forms.Label lbFN;
        public System.Windows.Forms.Label lbID;
        public System.Windows.Forms.Label lb3Text;
        public System.Windows.Forms.Label lb2Text;
        public System.Windows.Forms.Label lb4Text;
        public System.Windows.Forms.Label lb1Text;
        public System.Windows.Forms.ListBox listBox1;
        public System.Windows.Forms.TextBox txtFullname;
        public System.Windows.Forms.TextBox txtID;
        public System.Windows.Forms.Button btnLicense;
        public System.Windows.Forms.Button btnMoreInfo;
        private System.Windows.Forms.Button btnIPCAM;
        private RoundButton rbtnReprint;
        private System.Windows.Forms.Timer timerDelay;
        public System.Windows.Forms.Button btnIDCard;
        private System.Windows.Forms.LinkLabel lbKeyboardLayout;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private RoundButton rbtnOK;
    }
}