namespace WacVisitor
{
    partial class FormChargeReport
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.btnReport = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.panelPaging = new System.Windows.Forms.Panel();
            this.next = new System.Windows.Forms.LinkLabel();
            this.back = new System.Windows.Forms.LinkLabel();
            this.lbPaging = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.panelPaging.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnClose);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1229, 69);
            this.panel2.TabIndex = 28;
            this.panel2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel2_MouseMove);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(1132, 0);
            this.btnClose.Margin = new System.Windows.Forms.Padding(6);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(97, 69);
            this.btnClose.TabIndex = 15;
            this.btnClose.Text = "X";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.label1.ForeColor = System.Drawing.Color.Yellow;
            this.label1.Location = new System.Drawing.Point(37, 22);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(261, 25);
            this.label1.TabIndex = 14;
            this.label1.Text = "รายงาน สรุปรายการคิดเงิน";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Yellow;
            this.label2.Location = new System.Drawing.Point(97, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 23);
            this.label2.TabIndex = 32;
            this.label2.Text = "วันสิ้นสุด";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Yellow;
            this.label4.Location = new System.Drawing.Point(97, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 23);
            this.label4.TabIndex = 31;
            this.label4.Text = "วันเริ่มต้น";
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.dateTimePicker2.CustomFormat = "dd MMMM yyyy";
            this.dateTimePicker2.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker2.Location = new System.Drawing.Point(192, 44);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(252, 30);
            this.dateTimePicker2.TabIndex = 30;
            this.dateTimePicker2.Value = new System.DateTime(2021, 2, 16, 0, 0, 0, 0);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.dateTimePicker1.CustomFormat = "dd MMMM yyyy";
            this.dateTimePicker1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker1.Location = new System.Drawing.Point(192, 8);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(252, 30);
            this.dateTimePicker1.TabIndex = 29;
            this.dateTimePicker1.Value = new System.DateTime(2021, 2, 16, 0, 0, 0, 0);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.btnReport);
            this.panel1.Controls.Add(this.dateTimePicker2);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.dateTimePicker1);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Location = new System.Drawing.Point(149, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(603, 169);
            this.panel1.TabIndex = 33;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.button1.ForeColor = System.Drawing.Color.Blue;
            this.button1.Location = new System.Drawing.Point(192, 123);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(252, 40);
            this.button1.TabIndex = 35;
            this.button1.Text = "ส่งออก";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnReport
            // 
            this.btnReport.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.btnReport.ForeColor = System.Drawing.Color.Blue;
            this.btnReport.Location = new System.Drawing.Point(192, 80);
            this.btnReport.Name = "btnReport";
            this.btnReport.Size = new System.Drawing.Size(252, 41);
            this.btnReport.TabIndex = 34;
            this.btnReport.Text = "ดูรายงาน";
            this.btnReport.UseVisualStyleBackColor = true;
            this.btnReport.Click += new System.EventHandler(this.btnReport_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.dataGridView2);
            this.panel3.Controls.Add(this.panelPaging);
            this.panel3.Controls.Add(this.label5);
            this.panel3.Controls.Add(this.panel1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 69);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1229, 177);
            this.panel3.TabIndex = 34;
            // 
            // dataGridView2
            // 
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Location = new System.Drawing.Point(1083, 19);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.Size = new System.Drawing.Size(111, 64);
            this.dataGridView2.TabIndex = 49;
            this.dataGridView2.Visible = false;
            // 
            // panelPaging
            // 
            this.panelPaging.Controls.Add(this.next);
            this.panelPaging.Controls.Add(this.back);
            this.panelPaging.Controls.Add(this.lbPaging);
            this.panelPaging.Location = new System.Drawing.Point(830, 50);
            this.panelPaging.Name = "panelPaging";
            this.panelPaging.Size = new System.Drawing.Size(169, 33);
            this.panelPaging.TabIndex = 48;
            // 
            // next
            // 
            this.next.AutoSize = true;
            this.next.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold);
            this.next.ForeColor = System.Drawing.Color.Yellow;
            this.next.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.next.LinkColor = System.Drawing.Color.Yellow;
            this.next.Location = new System.Drawing.Point(115, 4);
            this.next.Name = "next";
            this.next.Size = new System.Drawing.Size(26, 23);
            this.next.TabIndex = 47;
            this.next.TabStop = true;
            this.next.Text = ">";
            this.next.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Next_LinkClicked);
            // 
            // back
            // 
            this.back.AutoSize = true;
            this.back.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold);
            this.back.ForeColor = System.Drawing.Color.Yellow;
            this.back.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.back.LinkColor = System.Drawing.Color.Yellow;
            this.back.Location = new System.Drawing.Point(35, 4);
            this.back.Name = "back";
            this.back.Size = new System.Drawing.Size(26, 23);
            this.back.TabIndex = 46;
            this.back.TabStop = true;
            this.back.Text = "<";
            this.back.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.Back_LinkClicked);
            // 
            // lbPaging
            // 
            this.lbPaging.AutoSize = true;
            this.lbPaging.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.lbPaging.ForeColor = System.Drawing.Color.Yellow;
            this.lbPaging.Location = new System.Drawing.Point(58, 5);
            this.lbPaging.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lbPaging.Name = "lbPaging";
            this.lbPaging.Size = new System.Drawing.Size(57, 23);
            this.lbPaging.TabIndex = 45;
            this.lbPaging.Text = "1/10";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.label5.ForeColor = System.Drawing.Color.Yellow;
            this.label5.Location = new System.Drawing.Point(5, 153);
            this.label5.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 23);
            this.label5.TabIndex = 44;
            this.label5.Text = "รายการ";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.dataGridView1);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 246);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1229, 458);
            this.panel4.TabIndex = 35;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(1229, 458);
            this.dataGridView1.TabIndex = 0;
            // 
            // FormChargeReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.HotTrack;
            this.ClientSize = new System.Drawing.Size(1229, 704);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Name = "FormChargeReport";
            this.Text = "FormChargeReport";
            this.Load += new System.EventHandler(this.FormChargeReport_Load);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.panelPaging.ResumeLayout(false);
            this.panelPaging.PerformLayout();
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnReport;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lbPaging;
        private System.Windows.Forms.LinkLabel next;
        private System.Windows.Forms.LinkLabel back;
        private System.Windows.Forms.Panel panelPaging;
        private System.Windows.Forms.DataGridView dataGridView2;
    }
}