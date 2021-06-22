namespace WacVisitor
{
    partial class frmAuthenReport
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
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.textBoxRound2 = new WacVisitor.TextBoxRound();
            this.textBoxRound1 = new WacVisitor.TextBoxRound();
            this.roundButton1 = new WacVisitor.RoundButton();
            this.roundButton2 = new WacVisitor.RoundButton();
            this.SuspendLayout();
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.linkLabel1.ForeColor = System.Drawing.Color.White;
            this.linkLabel1.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabel1.LinkColor = System.Drawing.Color.White;
            this.linkLabel1.Location = new System.Drawing.Point(200, 9);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(127, 21);
            this.linkLabel1.TabIndex = 37;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "เปลี่ยนรหัสยืนยัน";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // textBoxRound2
            // 
            this.textBoxRound2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxRound2.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.textBoxRound2.Location = new System.Drawing.Point(21, 97);
            this.textBoxRound2.Name = "textBoxRound2";
            this.textBoxRound2.PasswordChar = '*';
            this.textBoxRound2.Size = new System.Drawing.Size(304, 46);
            this.textBoxRound2.TabIndex = 42;
            this.textBoxRound2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBoxRound2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxRound2_KeyPress);
            // 
            // textBoxRound1
            // 
            this.textBoxRound1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxRound1.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.textBoxRound1.Location = new System.Drawing.Point(21, 45);
            this.textBoxRound1.Name = "textBoxRound1";
            this.textBoxRound1.Size = new System.Drawing.Size(304, 46);
            this.textBoxRound1.TabIndex = 41;
            this.textBoxRound1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // roundButton1
            // 
            this.roundButton1.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.roundButton1.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.roundButton1.Location = new System.Drawing.Point(21, 150);
            this.roundButton1.Name = "roundButton1";
            this.roundButton1.Size = new System.Drawing.Size(122, 50);
            this.roundButton1.TabIndex = 40;
            this.roundButton1.Text = "ยืนยัน";
            this.roundButton1.UseVisualStyleBackColor = true;
            this.roundButton1.Click += new System.EventHandler(this.roundButton1_Click);
            // 
            // roundButton2
            // 
            this.roundButton2.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.roundButton2.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.roundButton2.Location = new System.Drawing.Point(197, 150);
            this.roundButton2.Name = "roundButton2";
            this.roundButton2.Size = new System.Drawing.Size(128, 50);
            this.roundButton2.TabIndex = 43;
            this.roundButton2.Text = "ยกเลิก";
            this.roundButton2.UseVisualStyleBackColor = true;
            this.roundButton2.Click += new System.EventHandler(this.roundButton2_Click);
            // 
            // frmAuthenReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(350, 222);
            this.Controls.Add(this.textBoxRound2);
            this.Controls.Add(this.roundButton1);
            this.Controls.Add(this.textBoxRound1);
            this.Controls.Add(this.roundButton2);
            this.Controls.Add(this.linkLabel1);
            this.Name = "frmAuthenReport";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmAuthenReport";
            this.Load += new System.EventHandler(this.frmAuthenReport_Load);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.frmAuthenReport_MouseClick);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmAuthenReport_MouseMove);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel linkLabel1;
        private TextBoxRound textBoxRound2;
        private TextBoxRound textBoxRound1;
        private RoundButton roundButton1;
        private RoundButton roundButton2;
    }
}