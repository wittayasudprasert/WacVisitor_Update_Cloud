namespace WacVisitor
{
    partial class frmView
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmView));
            this.panel1 = new System.Windows.Forms.Panel();
            this.button10 = new System.Windows.Forms.Button();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel_search = new System.Windows.Forms.Panel();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.panelPage = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.lbPage = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.col_type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_in = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_out = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_typeid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.cmbPage = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel_search.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panelPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.HotTrack;
            this.panel1.Controls.Add(this.button10);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1132, 48);
            this.panel1.TabIndex = 0;
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
            this.button10.Location = new System.Drawing.Point(1062, 0);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(70, 48);
            this.button10.TabIndex = 10;
            this.button10.Text = "X";
            this.button10.UseVisualStyleBackColor = false;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.SystemColors.HotTrack;
            this.panel5.Controls.Add(this.panel_search);
            this.panel5.Controls.Add(this.button3);
            this.panel5.Controls.Add(this.panelPage);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 48);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(1132, 175);
            this.panel5.TabIndex = 4;
            // 
            // panel_search
            // 
            this.panel_search.Controls.Add(this.dateTimePicker2);
            this.panel_search.Controls.Add(this.dateTimePicker1);
            this.panel_search.Controls.Add(this.comboBox1);
            this.panel_search.Controls.Add(this.pictureBox1);
            this.panel_search.Controls.Add(this.textBox1);
            this.panel_search.Location = new System.Drawing.Point(91, 73);
            this.panel_search.Name = "panel_search";
            this.panel_search.Size = new System.Drawing.Size(576, 90);
            this.panel_search.TabIndex = 2;
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.dateTimePicker2.CustomFormat = "dd MMMM yyyy";
            this.dateTimePicker2.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker2.Location = new System.Drawing.Point(278, 50);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(249, 34);
            this.dateTimePicker2.TabIndex = 20;
            this.dateTimePicker2.ValueChanged += new System.EventHandler(this.DateTimePicker2_ValueChanged);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.dateTimePicker1.CustomFormat = "dd MMMM yyyy";
            this.dateTimePicker1.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker1.Location = new System.Drawing.Point(5, 50);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(255, 34);
            this.dateTimePicker1.TabIndex = 19;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(306, 5);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(221, 36);
            this.comboBox1.TabIndex = 8;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.Location = new System.Drawing.Point(532, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(39, 38);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(5, 5);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(295, 36);
            this.textBox1.TabIndex = 4;
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // button3
            // 
            this.button3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button3.ForeColor = System.Drawing.Color.Blue;
            this.button3.Location = new System.Drawing.Point(870, 18);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(131, 37);
            this.button3.TabIndex = 5;
            this.button3.Text = "แสดงทั้งหมด";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Visible = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // panelPage
            // 
            this.panelPage.Controls.Add(this.cmbPage);
            this.panelPage.Controls.Add(this.label1);
            this.panelPage.Controls.Add(this.button1);
            this.panelPage.Controls.Add(this.lbPage);
            this.panelPage.Controls.Add(this.button2);
            this.panelPage.Location = new System.Drawing.Point(91, 6);
            this.panelPage.Name = "panelPage";
            this.panelPage.Size = new System.Drawing.Size(360, 64);
            this.panelPage.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button1.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.button1.ForeColor = System.Drawing.Color.Blue;
            this.button1.Location = new System.Drawing.Point(36, 15);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(84, 36);
            this.button1.TabIndex = 0;
            this.button1.Text = "<";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lbPage
            // 
            this.lbPage.AutoSize = true;
            this.lbPage.ForeColor = System.Drawing.Color.White;
            this.lbPage.Location = new System.Drawing.Point(151, 19);
            this.lbPage.Name = "lbPage";
            this.lbPage.Size = new System.Drawing.Size(58, 30);
            this.lbPage.TabIndex = 2;
            this.lbPage.Text = "1/10";
            this.lbPage.Visible = false;
            // 
            // button2
            // 
            this.button2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button2.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.button2.ForeColor = System.Drawing.Color.Blue;
            this.button2.Location = new System.Drawing.Point(242, 15);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(84, 36);
            this.button2.TabIndex = 1;
            this.button2.Text = ">";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.HotTrack;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_type,
            this.col_number,
            this.col_in,
            this.col_out,
            this.col_typeid});
            this.dataGridView1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(8, 9, 8, 9);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(1074, 427);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.dataGridView1.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView1_CellMouseEnter);
            this.dataGridView1.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView1_CellMouseLeave);
            this.dataGridView1.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dataGridView1_RowPostPaint);
            // 
            // col_type
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.col_type.DefaultCellStyle = dataGridViewCellStyle1;
            this.col_type.HeaderText = "ประเภท";
            this.col_type.Name = "col_type";
            this.col_type.ReadOnly = true;
            // 
            // col_number
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.col_number.DefaultCellStyle = dataGridViewCellStyle2;
            this.col_number.FillWeight = 80F;
            this.col_number.HeaderText = "เลข VISITOR";
            this.col_number.Name = "col_number";
            this.col_number.ReadOnly = true;
            this.col_number.Width = 80;
            // 
            // col_in
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.col_in.DefaultCellStyle = dataGridViewCellStyle3;
            this.col_in.FillWeight = 150F;
            this.col_in.HeaderText = "เข้า";
            this.col_in.Name = "col_in";
            this.col_in.ReadOnly = true;
            this.col_in.Width = 150;
            // 
            // col_out
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.col_out.DefaultCellStyle = dataGridViewCellStyle4;
            this.col_out.FillWeight = 150F;
            this.col_out.HeaderText = "ออก";
            this.col_out.Name = "col_out";
            this.col_out.ReadOnly = true;
            this.col_out.Width = 150;
            // 
            // col_typeid
            // 
            this.col_typeid.HeaderText = "Type_ID";
            this.col_typeid.Name = "col_typeid";
            this.col_typeid.ReadOnly = true;
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 223);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(31, 427);
            this.panel2.TabIndex = 5;
            // 
            // panel3
            // 
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(1105, 223);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(27, 427);
            this.panel3.TabIndex = 6;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.dataGridView1);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(31, 223);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1074, 427);
            this.panel4.TabIndex = 7;
            // 
            // cmbPage
            // 
            this.cmbPage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPage.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbPage.FormattingEnabled = true;
            this.cmbPage.Location = new System.Drawing.Point(117, 11);
            this.cmbPage.Name = "cmbPage";
            this.cmbPage.Size = new System.Drawing.Size(67, 40);
            this.cmbPage.TabIndex = 12;
            this.cmbPage.SelectedIndexChanged += new System.EventHandler(this.CmbPage_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(186, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 30);
            this.label1.TabIndex = 11;
            this.label1.Text = "/1";
            // 
            // frmView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 30F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.HotTrack;
            this.ClientSize = new System.Drawing.Size(1132, 650);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.Name = "frmView";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Loading...";
            this.Load += new System.EventHandler(this.frmView_Load);
            this.panel1.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel_search.ResumeLayout(false);
            this.panel_search.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panelPage.ResumeLayout(false);
            this.panelPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Panel panelPage;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lbPage;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_type;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_number;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_in;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_out;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_typeid;
        private System.Windows.Forms.Panel panel_search;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.ComboBox cmbPage;
        private System.Windows.Forms.Label label1;
    }
}