namespace WacVisitor
{
    partial class frmAuthen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAuthen));
            this.btnExit = new System.Windows.Forms.Button();
            this.lbTime = new System.Windows.Forms.Label();
            this.textBoxRound2 = new WacVisitor.TextBoxRound();
            this.textBoxRound1 = new WacVisitor.TextBoxRound();
            this.roundButton2 = new WacVisitor.RoundButton();
            this.roundButton1 = new WacVisitor.RoundButton();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnExit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnExit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExit.FlatAppearance.BorderSize = 0;
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnExit.ForeColor = System.Drawing.Color.White;
            this.btnExit.Location = new System.Drawing.Point(377, 10);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(64, 64);
            this.btnExit.TabIndex = 11;
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lbTime
            // 
            this.lbTime.AutoSize = true;
            this.lbTime.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lbTime.ForeColor = System.Drawing.Color.White;
            this.lbTime.Location = new System.Drawing.Point(196, 76);
            this.lbTime.Name = "lbTime";
            this.lbTime.Size = new System.Drawing.Size(76, 30);
            this.lbTime.TabIndex = 35;
            this.lbTime.Text = "label2";
            this.lbTime.Visible = false;
            // 
            // textBoxRound2
            // 
            this.textBoxRound2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxRound2.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.textBoxRound2.Location = new System.Drawing.Point(109, 172);
            this.textBoxRound2.Name = "textBoxRound2";
            this.textBoxRound2.PasswordChar = '*';
            this.textBoxRound2.Size = new System.Drawing.Size(239, 46);
            this.textBoxRound2.TabIndex = 39;
            this.textBoxRound2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBoxRound2.TextChanged += new System.EventHandler(this.textBoxRound2_TextChanged);
            this.textBoxRound2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxRound2_KeyDown);
            // 
            // textBoxRound1
            // 
            this.textBoxRound1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxRound1.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.textBoxRound1.Location = new System.Drawing.Point(109, 118);
            this.textBoxRound1.Name = "textBoxRound1";
            this.textBoxRound1.Size = new System.Drawing.Size(239, 46);
            this.textBoxRound1.TabIndex = 38;
            this.textBoxRound1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBoxRound1.TextChanged += new System.EventHandler(this.textBoxRound1_TextChanged);
            // 
            // roundButton2
            // 
            this.roundButton2.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.roundButton2.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.roundButton2.Location = new System.Drawing.Point(10, 289);
            this.roundButton2.Name = "roundButton2";
            this.roundButton2.Size = new System.Drawing.Size(185, 50);
            this.roundButton2.TabIndex = 37;
            this.roundButton2.Text = "ใช้แบบ Offline";
            this.roundButton2.UseVisualStyleBackColor = true;
            this.roundButton2.Click += new System.EventHandler(this.roundButton2_Click);
            // 
            // roundButton1
            // 
            this.roundButton1.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.roundButton1.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.roundButton1.Location = new System.Drawing.Point(142, 229);
            this.roundButton1.Name = "roundButton1";
            this.roundButton1.Size = new System.Drawing.Size(173, 50);
            this.roundButton1.TabIndex = 36;
            this.roundButton1.Text = "ยืนยัน";
            this.roundButton1.UseVisualStyleBackColor = true;
            this.roundButton1.Click += new System.EventHandler(this.roundButton1_Click);
            // 
            // panel3
            // 
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(440, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(10, 342);
            this.panel3.TabIndex = 41;
            // 
            // panel4
            // 
            this.panel4.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(10, 342);
            this.panel4.TabIndex = 42;
            // 
            // panel5
            // 
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(10, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(430, 10);
            this.panel5.TabIndex = 43;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(17, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 15);
            this.label1.TabIndex = 44;
            this.label1.Text = "label1";
            this.label1.Visible = false;
            // 
            // frmAuthen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.HotTrack;
            this.ClientSize = new System.Drawing.Size(450, 342);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.textBoxRound2);
            this.Controls.Add(this.textBoxRound1);
            this.Controls.Add(this.roundButton2);
            this.Controls.Add(this.roundButton1);
            this.Controls.Add(this.lbTime);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "frmAuthen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmAuthen";
            this.Load += new System.EventHandler(this.frmAuthen_Load);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmAuthen_MouseMove);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbTime;
        private RoundButton roundButton1;
        private RoundButton roundButton2;
        private TextBoxRound textBoxRound1;
        private TextBoxRound textBoxRound2;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label1;
    }
}