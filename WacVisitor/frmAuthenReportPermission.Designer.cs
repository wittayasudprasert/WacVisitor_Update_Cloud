namespace WacVisitor
{
    partial class frmAuthenReportPermission
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
            this.textBoxRound1 = new WacVisitor.TextBoxRound();
            this.roundButton1 = new WacVisitor.RoundButton();
            this.textBoxRound2 = new WacVisitor.TextBoxRound();
            this.roundButton2 = new WacVisitor.RoundButton();
            this.SuspendLayout();
            // 
            // textBoxRound1
            // 
            this.textBoxRound1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxRound1.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.textBoxRound1.Location = new System.Drawing.Point(35, 27);
            this.textBoxRound1.Name = "textBoxRound1";
            this.textBoxRound1.Size = new System.Drawing.Size(304, 46);
            this.textBoxRound1.TabIndex = 45;
            this.textBoxRound1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // roundButton1
            // 
            this.roundButton1.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.roundButton1.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.roundButton1.Location = new System.Drawing.Point(35, 144);
            this.roundButton1.Name = "roundButton1";
            this.roundButton1.Size = new System.Drawing.Size(129, 50);
            this.roundButton1.TabIndex = 44;
            this.roundButton1.Text = "บันทึก";
            this.roundButton1.UseVisualStyleBackColor = true;
            this.roundButton1.Click += new System.EventHandler(this.roundButton1_Click);
            // 
            // textBoxRound2
            // 
            this.textBoxRound2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxRound2.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.textBoxRound2.Location = new System.Drawing.Point(35, 82);
            this.textBoxRound2.Name = "textBoxRound2";
            this.textBoxRound2.PasswordChar = '*';
            this.textBoxRound2.Size = new System.Drawing.Size(304, 46);
            this.textBoxRound2.TabIndex = 46;
            this.textBoxRound2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // roundButton2
            // 
            this.roundButton2.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.roundButton2.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.roundButton2.Location = new System.Drawing.Point(210, 144);
            this.roundButton2.Name = "roundButton2";
            this.roundButton2.Size = new System.Drawing.Size(128, 50);
            this.roundButton2.TabIndex = 47;
            this.roundButton2.Text = "ยกเลิก";
            this.roundButton2.UseVisualStyleBackColor = true;
            this.roundButton2.Click += new System.EventHandler(this.roundButton2_Click);
            // 
            // frmAuthenReportPermission
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(372, 206);
            this.Controls.Add(this.textBoxRound1);
            this.Controls.Add(this.roundButton1);
            this.Controls.Add(this.textBoxRound2);
            this.Controls.Add(this.roundButton2);
            this.Name = "frmAuthenReportPermission";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmAuthenReportPermission";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmAuthenReportPermission_Load);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.frmAuthenReportPermission_MouseClick);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmAuthenReportPermission_MouseMove);
            this.ResumeLayout(false);

        }

        #endregion

        private TextBoxRound textBoxRound1;
        private RoundButton roundButton1;
        private TextBoxRound textBoxRound2;
        private RoundButton roundButton2;
    }
}