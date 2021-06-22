namespace WacVisitor
{
    partial class frmTakePicture
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
            this.picWebcam = new System.Windows.Forms.PictureBox();
            this.btnTakePhoto = new System.Windows.Forms.Button();
            this.btnAccept = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button10 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picWebcam)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // picWebcam
            // 
            this.picWebcam.BackColor = System.Drawing.Color.Black;
            this.picWebcam.Location = new System.Drawing.Point(38, 73);
            this.picWebcam.Name = "picWebcam";
            this.picWebcam.Size = new System.Drawing.Size(501, 323);
            this.picWebcam.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picWebcam.TabIndex = 0;
            this.picWebcam.TabStop = false;
            // 
            // btnTakePhoto
            // 
            this.btnTakePhoto.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnTakePhoto.ForeColor = System.Drawing.Color.Blue;
            this.btnTakePhoto.Location = new System.Drawing.Point(179, 402);
            this.btnTakePhoto.Name = "btnTakePhoto";
            this.btnTakePhoto.Size = new System.Drawing.Size(217, 62);
            this.btnTakePhoto.TabIndex = 1;
            this.btnTakePhoto.Text = "ถ่าย";
            this.btnTakePhoto.UseVisualStyleBackColor = true;
            this.btnTakePhoto.Click += new System.EventHandler(this.btnTakePhoto_Click);
            // 
            // btnAccept
            // 
            this.btnAccept.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnAccept.ForeColor = System.Drawing.Color.Blue;
            this.btnAccept.Location = new System.Drawing.Point(426, 411);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(142, 62);
            this.btnAccept.TabIndex = 3;
            this.btnAccept.Text = "ตกลง";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Visible = false;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Highlight;
            this.panel1.Controls.Add(this.button10);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(568, 67);
            this.panel1.TabIndex = 4;
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
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
            this.button10.Location = new System.Drawing.Point(484, 0);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(84, 67);
            this.button10.TabIndex = 10;
            this.button10.Text = "X";
            this.button10.UseVisualStyleBackColor = false;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // frmTakePicture
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Highlight;
            this.ClientSize = new System.Drawing.Size(568, 476);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.btnTakePhoto);
            this.Controls.Add(this.picWebcam);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "frmTakePicture";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = ":: TAKE PICTURE ::";
            this.Load += new System.EventHandler(this.frmTakePicture_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picWebcam)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picWebcam;
        private System.Windows.Forms.Button btnTakePhoto;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button10;
    }
}