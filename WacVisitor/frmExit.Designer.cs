namespace WacVisitor
{
    partial class frmExit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmExit));
            this.button1 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.myButton1 = new WacVisitor.MyButton();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Transparent;
            this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button1.FlatAppearance.BorderColor = System.Drawing.Color.CornflowerBlue;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(182, 95);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(112, 70);
            this.button1.TabIndex = 12;
            this.button1.Text = "ยกเลิก";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button11
            // 
            this.button11.BackColor = System.Drawing.Color.Transparent;
            this.button11.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button11.FlatAppearance.BorderColor = System.Drawing.Color.CornflowerBlue;
            this.button11.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button11.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.button11.ForeColor = System.Drawing.Color.White;
            this.button11.Location = new System.Drawing.Point(43, 95);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(112, 70);
            this.button11.TabIndex = 11;
            this.button11.Text = "ออก";
            this.button11.UseVisualStyleBackColor = false;
            this.button11.Click += new System.EventHandler(this.button11_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(55, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(239, 26);
            this.label2.TabIndex = 10;
            this.label2.Text = "ต้องการออกจากโปรแกรม?";
            // 
            // myButton1
            // 
            this.myButton1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.myButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.myButton1.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.myButton1.ForeColor = System.Drawing.Color.White;
            this.myButton1.Location = new System.Drawing.Point(249, 171);
            this.myButton1.Name = "myButton1";
            this.myButton1.Size = new System.Drawing.Size(73, 38);
            this.myButton1.TabIndex = 13;
            this.myButton1.Text = "ออก";
            this.myButton1.UseVisualStyleBackColor = false;
            this.myButton1.Visible = false;
            this.myButton1.MouseLeave += new System.EventHandler(this.myButton1_MouseLeave);
            this.myButton1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.myButton1_MouseMove);
            // 
            // frmExit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(334, 202);
            this.Controls.Add(this.myButton1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button11);
            this.Controls.Add(this.label2);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmExit";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmExit";
            this.Load += new System.EventHandler(this.frmExit_Load);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmExit_MouseMove);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Label label2;
        private MyButton myButton1;
    }
}