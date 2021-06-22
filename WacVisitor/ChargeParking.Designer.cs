namespace WacVisitor
{
    partial class ChargeParking
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.button10 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.lbLicensePlate = new System.Windows.Forms.Label();
            this.lbFreePark = new System.Windows.Forms.Label();
            this.lbChargeBaht = new System.Windows.Forms.Label();
            this.txtCharge = new System.Windows.Forms.TextBox();
            this.lbVisitorNo = new System.Windows.Forms.Label();
            this.lbParkTime = new System.Windows.Forms.Label();
            this.lbIN = new System.Windows.Forms.Label();
            this.lbOUT = new System.Windows.Forms.Label();
            this.lbSelectType = new System.Windows.Forms.Label();
            this.lbSelectType1 = new System.Windows.Forms.Label();
            this.cb3 = new System.Windows.Forms.CheckBox();
            this.cb2 = new System.Windows.Forms.CheckBox();
            this.cb1 = new System.Windows.Forms.CheckBox();
            this.panelButton = new System.Windows.Forms.Panel();
            this.cb4 = new System.Windows.Forms.CheckBox();
            this.cb5 = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.panelButton.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Blue;
            this.panel1.Controls.Add(this.button10);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(9);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(966, 64);
            this.panel1.TabIndex = 31;
            // 
            // button10
            // 
            this.button10.BackColor = System.Drawing.Color.Transparent;
            this.button10.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button10.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button10.Dock = System.Windows.Forms.DockStyle.Right;
            this.button10.FlatAppearance.BorderSize = 0;
            this.button10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button10.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.button10.ForeColor = System.Drawing.Color.White;
            this.button10.Location = new System.Drawing.Point(878, 0);
            this.button10.Margin = new System.Windows.Forms.Padding(9);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(88, 64);
            this.button10.TabIndex = 10;
            this.button10.Text = "X";
            this.button10.UseVisualStyleBackColor = false;
            this.button10.Visible = false;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.label1.ForeColor = System.Drawing.Color.Yellow;
            this.label1.Location = new System.Drawing.Point(14, 19);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(190, 29);
            this.label1.TabIndex = 34;
            this.label1.Text = "คิดเงินค่าจอดรถ";
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.button2.ForeColor = System.Drawing.Color.Blue;
            this.button2.Location = new System.Drawing.Point(51, 8);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(176, 55);
            this.button2.TabIndex = 32;
            this.button2.Text = "จ่าย";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.button1.ForeColor = System.Drawing.Color.Blue;
            this.button1.Location = new System.Drawing.Point(435, 8);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(176, 55);
            this.button1.TabIndex = 33;
            this.button1.Text = "ไม่จ่าย";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lbLicensePlate
            // 
            this.lbLicensePlate.AutoSize = true;
            this.lbLicensePlate.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lbLicensePlate.ForeColor = System.Drawing.Color.Yellow;
            this.lbLicensePlate.Location = new System.Drawing.Point(438, 77);
            this.lbLicensePlate.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lbLicensePlate.Name = "lbLicensePlate";
            this.lbLicensePlate.Size = new System.Drawing.Size(118, 27);
            this.lbLicensePlate.TabIndex = 35;
            this.lbLicensePlate.Text = "ทะเบียนรถ";
            // 
            // lbFreePark
            // 
            this.lbFreePark.AutoSize = true;
            this.lbFreePark.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lbFreePark.ForeColor = System.Drawing.Color.Yellow;
            this.lbFreePark.Location = new System.Drawing.Point(18, 246);
            this.lbFreePark.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lbFreePark.Name = "lbFreePark";
            this.lbFreePark.Size = new System.Drawing.Size(149, 27);
            this.lbFreePark.TabIndex = 36;
            this.lbFreePark.Text = "จอดฟรี (ชม.)";
            // 
            // lbChargeBaht
            // 
            this.lbChargeBaht.AutoSize = true;
            this.lbChargeBaht.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lbChargeBaht.ForeColor = System.Drawing.Color.Yellow;
            this.lbChargeBaht.Location = new System.Drawing.Point(215, 393);
            this.lbChargeBaht.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lbChargeBaht.Name = "lbChargeBaht";
            this.lbChargeBaht.Size = new System.Drawing.Size(539, 39);
            this.lbChargeBaht.TabIndex = 37;
            this.lbChargeBaht.Text = "ค่าบริการจอดรถ                         บาท";
            // 
            // txtCharge
            // 
            this.txtCharge.BackColor = System.Drawing.SystemColors.Info;
            this.txtCharge.Font = new System.Drawing.Font("Tahoma", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.txtCharge.Location = new System.Drawing.Point(468, 390);
            this.txtCharge.Name = "txtCharge";
            this.txtCharge.Size = new System.Drawing.Size(197, 46);
            this.txtCharge.TabIndex = 38;
            this.txtCharge.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbVisitorNo
            // 
            this.lbVisitorNo.AutoSize = true;
            this.lbVisitorNo.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lbVisitorNo.ForeColor = System.Drawing.Color.Yellow;
            this.lbVisitorNo.Location = new System.Drawing.Point(18, 77);
            this.lbVisitorNo.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lbVisitorNo.Name = "lbVisitorNo";
            this.lbVisitorNo.Size = new System.Drawing.Size(129, 27);
            this.lbVisitorNo.TabIndex = 39;
            this.lbVisitorNo.Text = "Visitor No.";
            // 
            // lbParkTime
            // 
            this.lbParkTime.AutoSize = true;
            this.lbParkTime.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lbParkTime.ForeColor = System.Drawing.Color.Yellow;
            this.lbParkTime.Location = new System.Drawing.Point(258, 194);
            this.lbParkTime.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lbParkTime.Name = "lbParkTime";
            this.lbParkTime.Size = new System.Drawing.Size(175, 27);
            this.lbParkTime.TabIndex = 40;
            this.lbParkTime.Text = "เวลาจอดทั้งหมด";
            // 
            // lbIN
            // 
            this.lbIN.AutoSize = true;
            this.lbIN.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lbIN.ForeColor = System.Drawing.Color.Yellow;
            this.lbIN.Location = new System.Drawing.Point(18, 131);
            this.lbIN.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lbIN.Name = "lbIN";
            this.lbIN.Size = new System.Drawing.Size(47, 27);
            this.lbIN.TabIndex = 41;
            this.lbIN.Text = "เข้า";
            // 
            // lbOUT
            // 
            this.lbOUT.AutoSize = true;
            this.lbOUT.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lbOUT.ForeColor = System.Drawing.Color.Yellow;
            this.lbOUT.Location = new System.Drawing.Point(438, 131);
            this.lbOUT.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lbOUT.Name = "lbOUT";
            this.lbOUT.Size = new System.Drawing.Size(55, 27);
            this.lbOUT.TabIndex = 42;
            this.lbOUT.Text = "ออก";
            // 
            // lbSelectType
            // 
            this.lbSelectType.AutoSize = true;
            this.lbSelectType.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lbSelectType.ForeColor = System.Drawing.Color.Yellow;
            this.lbSelectType.Location = new System.Drawing.Point(18, 290);
            this.lbSelectType.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lbSelectType.Name = "lbSelectType";
            this.lbSelectType.Size = new System.Drawing.Size(212, 27);
            this.lbSelectType.TabIndex = 43;
            this.lbSelectType.Text = "เลือกประเภทเก็บเงิน";
            // 
            // lbSelectType1
            // 
            this.lbSelectType1.AutoSize = true;
            this.lbSelectType1.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lbSelectType1.ForeColor = System.Drawing.Color.Yellow;
            this.lbSelectType1.Location = new System.Drawing.Point(18, 332);
            this.lbSelectType1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lbSelectType1.Name = "lbSelectType1";
            this.lbSelectType1.Size = new System.Drawing.Size(270, 27);
            this.lbSelectType1.TabIndex = 44;
            this.lbSelectType1.Text = "สำหรับ ..ประเภท Visitor..";
            // 
            // cb3
            // 
            this.cb3.Appearance = System.Windows.Forms.Appearance.Button;
            this.cb3.AutoSize = true;
            this.cb3.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.cb3.ForeColor = System.Drawing.Color.Blue;
            this.cb3.Location = new System.Drawing.Point(563, 315);
            this.cb3.Name = "cb3";
            this.cb3.Size = new System.Drawing.Size(119, 46);
            this.cb3.TabIndex = 47;
            this.cb3.Text = "    C    ";
            this.cb3.UseVisualStyleBackColor = true;
            this.cb3.CheckedChanged += new System.EventHandler(this.cb3_CheckedChanged);
            // 
            // cb2
            // 
            this.cb2.Appearance = System.Windows.Forms.Appearance.Button;
            this.cb2.AutoSize = true;
            this.cb2.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.cb2.ForeColor = System.Drawing.Color.Blue;
            this.cb2.Location = new System.Drawing.Point(439, 315);
            this.cb2.Name = "cb2";
            this.cb2.Size = new System.Drawing.Size(118, 46);
            this.cb2.TabIndex = 46;
            this.cb2.Text = "    B    ";
            this.cb2.UseVisualStyleBackColor = true;
            this.cb2.CheckedChanged += new System.EventHandler(this.cb2_CheckedChanged);
            // 
            // cb1
            // 
            this.cb1.Appearance = System.Windows.Forms.Appearance.Button;
            this.cb1.AutoSize = true;
            this.cb1.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.cb1.ForeColor = System.Drawing.Color.Blue;
            this.cb1.Location = new System.Drawing.Point(314, 315);
            this.cb1.Name = "cb1";
            this.cb1.Size = new System.Drawing.Size(119, 46);
            this.cb1.TabIndex = 45;
            this.cb1.Text = "    A    ";
            this.cb1.UseVisualStyleBackColor = true;
            this.cb1.CheckedChanged += new System.EventHandler(this.cb1_CheckedChanged);
            // 
            // panelButton
            // 
            this.panelButton.Controls.Add(this.button2);
            this.panelButton.Controls.Add(this.button1);
            this.panelButton.Location = new System.Drawing.Point(171, 456);
            this.panelButton.Name = "panelButton";
            this.panelButton.Size = new System.Drawing.Size(662, 72);
            this.panelButton.TabIndex = 48;
            // 
            // cb4
            // 
            this.cb4.Appearance = System.Windows.Forms.Appearance.Button;
            this.cb4.AutoSize = true;
            this.cb4.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.cb4.ForeColor = System.Drawing.Color.Blue;
            this.cb4.Location = new System.Drawing.Point(688, 315);
            this.cb4.Name = "cb4";
            this.cb4.Size = new System.Drawing.Size(119, 46);
            this.cb4.TabIndex = 49;
            this.cb4.Text = "    D    ";
            this.cb4.UseVisualStyleBackColor = true;
            this.cb4.CheckedChanged += new System.EventHandler(this.cb4_CheckedChanged);
            // 
            // cb5
            // 
            this.cb5.Appearance = System.Windows.Forms.Appearance.Button;
            this.cb5.AutoSize = true;
            this.cb5.Font = new System.Drawing.Font("Microsoft Sans Serif", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.cb5.ForeColor = System.Drawing.Color.Blue;
            this.cb5.Location = new System.Drawing.Point(812, 315);
            this.cb5.Name = "cb5";
            this.cb5.Size = new System.Drawing.Size(118, 46);
            this.cb5.TabIndex = 50;
            this.cb5.Text = "    E    ";
            this.cb5.UseVisualStyleBackColor = true;
            this.cb5.CheckedChanged += new System.EventHandler(this.cb5_CheckedChanged);
            // 
            // ChargeParking
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 29F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Blue;
            this.ClientSize = new System.Drawing.Size(966, 540);
            this.Controls.Add(this.cb5);
            this.Controls.Add(this.cb4);
            this.Controls.Add(this.panelButton);
            this.Controls.Add(this.cb3);
            this.Controls.Add(this.cb2);
            this.Controls.Add(this.cb1);
            this.Controls.Add(this.lbSelectType1);
            this.Controls.Add(this.lbSelectType);
            this.Controls.Add(this.lbOUT);
            this.Controls.Add(this.lbIN);
            this.Controls.Add(this.lbParkTime);
            this.Controls.Add(this.lbVisitorNo);
            this.Controls.Add(this.txtCharge);
            this.Controls.Add(this.lbChargeBaht);
            this.Controls.Add(this.lbFreePark);
            this.Controls.Add(this.lbLicensePlate);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.Name = "ChargeParking";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ChargeParking";
            this.Load += new System.EventHandler(this.ChargeParking_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panelButton.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbLicensePlate;
        private System.Windows.Forms.Label lbFreePark;
        private System.Windows.Forms.Label lbChargeBaht;
        private System.Windows.Forms.TextBox txtCharge;
        private System.Windows.Forms.Label lbVisitorNo;
        private System.Windows.Forms.Label lbParkTime;
        private System.Windows.Forms.Label lbIN;
        private System.Windows.Forms.Label lbOUT;
        private System.Windows.Forms.Label lbSelectType;
        private System.Windows.Forms.Label lbSelectType1;
        private System.Windows.Forms.CheckBox cb3;
        private System.Windows.Forms.CheckBox cb2;
        private System.Windows.Forms.CheckBox cb1;
        private System.Windows.Forms.Panel panelButton;
        private System.Windows.Forms.CheckBox cb4;
        private System.Windows.Forms.CheckBox cb5;
    }
}