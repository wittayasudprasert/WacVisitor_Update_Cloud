
namespace WacVisitor
{
    partial class ThaiIDReader
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
            this.label1 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.comboReader = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.linkLabel1.ForeColor = System.Drawing.Color.White;
            this.linkLabel1.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabel1.LinkColor = System.Drawing.Color.White;
            this.linkLabel1.Location = new System.Drawing.Point(461, -2);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(70, 41);
            this.linkLabel1.TabIndex = 38;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "  X  ";
            this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(22, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(474, 33);
            this.label1.TabIndex = 39;
            this.label1.Text = "เสียบบัตรประชาชน แล้วกรุณารอสักครู่";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(28, 102);
            this.progressBar1.Maximum = 20;
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(468, 29);
            this.progressBar1.TabIndex = 40;
            // 
            // comboReader
            // 
            this.comboReader.FormattingEnabled = true;
            this.comboReader.Location = new System.Drawing.Point(552, 102);
            this.comboReader.Name = "comboReader";
            this.comboReader.Size = new System.Drawing.Size(319, 21);
            this.comboReader.TabIndex = 41;
            this.comboReader.SelectedIndexChanged += new System.EventHandler(this.comboReader_SelectedIndexChanged);
            // 
            // ThaiIDReader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(523, 150);
            this.Controls.Add(this.comboReader);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.linkLabel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ThaiIDReader";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ThaiIDReader";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ThaiIDReader_FormClosing);
            this.Load += new System.EventHandler(this.ThaiIDReader_Load);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ThaiIDReader_MouseMove);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ComboBox comboReader;
    }
}