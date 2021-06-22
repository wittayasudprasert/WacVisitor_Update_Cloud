namespace WacVisitor
{
    partial class frmViewVisitor
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel5 = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.col_type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_in = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_out = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_photo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_card = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button3 = new System.Windows.Forms.Button();
            this.panelPage = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.lbPage = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button10 = new System.Windows.Forms.Button();
            this.cmbPage = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel2.SuspendLayout();
            this.panelPage.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.SystemColors.Highlight;
            this.panel5.Controls.Add(this.dataGridView1);
            this.panel5.Controls.Add(this.panel4);
            this.panel5.Controls.Add(this.panel3);
            this.panel5.Controls.Add(this.panel2);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 63);
            this.panel5.Margin = new System.Windows.Forms.Padding(8, 9, 8, 9);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(1201, 543);
            this.panel5.TabIndex = 9;
            this.panel5.Paint += new System.Windows.Forms.PaintEventHandler(this.panel5_Paint);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Highlight;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_type,
            this.col_number,
            this.col_in,
            this.col_out,
            this.col_photo,
            this.col_card});
            this.dataGridView1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(26, 85);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(8, 9, 8, 9);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(1150, 458);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.dataGridView1.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView1_CellMouseEnter);
            this.dataGridView1.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridView1_CellMouseLeave);
            this.dataGridView1.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridView1_RowsAdded);
            // 
            // col_type
            // 
            dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.col_type.DefaultCellStyle = dataGridViewCellStyle13;
            this.col_type.HeaderText = "ประเภท";
            this.col_type.Name = "col_type";
            this.col_type.ReadOnly = true;
            // 
            // col_number
            // 
            dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.col_number.DefaultCellStyle = dataGridViewCellStyle14;
            this.col_number.FillWeight = 80F;
            this.col_number.HeaderText = "เลข VISITOR";
            this.col_number.Name = "col_number";
            this.col_number.ReadOnly = true;
            this.col_number.Width = 80;
            // 
            // col_in
            // 
            dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.col_in.DefaultCellStyle = dataGridViewCellStyle15;
            this.col_in.FillWeight = 150F;
            this.col_in.HeaderText = "เข้า";
            this.col_in.Name = "col_in";
            this.col_in.ReadOnly = true;
            this.col_in.Width = 150;
            // 
            // col_out
            // 
            dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.col_out.DefaultCellStyle = dataGridViewCellStyle16;
            this.col_out.FillWeight = 150F;
            this.col_out.HeaderText = "ออก";
            this.col_out.Name = "col_out";
            this.col_out.ReadOnly = true;
            this.col_out.Width = 150;
            // 
            // col_photo
            // 
            dataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.col_photo.DefaultCellStyle = dataGridViewCellStyle17;
            this.col_photo.HeaderText = "ภาพถ่าย";
            this.col_photo.Name = "col_photo";
            this.col_photo.ReadOnly = true;
            // 
            // col_card
            // 
            dataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.col_card.DefaultCellStyle = dataGridViewCellStyle18;
            this.col_card.HeaderText = "ภาพบัตร";
            this.col_card.Name = "col_card";
            this.col_card.ReadOnly = true;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.Highlight;
            this.panel4.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel4.Location = new System.Drawing.Point(1176, 85);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(25, 458);
            this.panel4.TabIndex = 3;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.Highlight;
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(0, 85);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(26, 458);
            this.panel3.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Highlight;
            this.panel2.Controls.Add(this.button3);
            this.panel2.Controls.Add(this.panelPage);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1201, 85);
            this.panel2.TabIndex = 1;
            // 
            // button3
            // 
            this.button3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button3.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.ForeColor = System.Drawing.Color.Blue;
            this.button3.Location = new System.Drawing.Point(766, 23);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(160, 36);
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
            this.panelPage.Location = new System.Drawing.Point(440, 8);
            this.panelPage.Name = "panelPage";
            this.panelPage.Size = new System.Drawing.Size(320, 68);
            this.panelPage.TabIndex = 4;
            // 
            // button1
            // 
            this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button1.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.button1.ForeColor = System.Drawing.Color.Blue;
            this.button1.Location = new System.Drawing.Point(25, 15);
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
            this.lbPage.Location = new System.Drawing.Point(130, 13);
            this.lbPage.Name = "lbPage";
            this.lbPage.Size = new System.Drawing.Size(58, 37);
            this.lbPage.TabIndex = 2;
            this.lbPage.Text = "0/0";
            this.lbPage.Visible = false;
            // 
            // button2
            // 
            this.button2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button2.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.button2.ForeColor = System.Drawing.Color.Blue;
            this.button2.Location = new System.Drawing.Point(213, 15);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(84, 36);
            this.button2.TabIndex = 1;
            this.button2.Text = ">";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Highlight;
            this.panel1.Controls.Add(this.button10);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(8, 9, 8, 9);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1201, 63);
            this.panel1.TabIndex = 5;
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
            this.button10.Location = new System.Drawing.Point(1117, 0);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(84, 63);
            this.button10.TabIndex = 11;
            this.button10.Text = "X";
            this.button10.UseVisualStyleBackColor = false;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // cmbPage
            // 
            this.cmbPage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPage.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbPage.FormattingEnabled = true;
            this.cmbPage.Location = new System.Drawing.Point(95, 12);
            this.cmbPage.Name = "cmbPage";
            this.cmbPage.Size = new System.Drawing.Size(67, 40);
            this.cmbPage.TabIndex = 10;
            this.cmbPage.SelectedIndexChanged += new System.EventHandler(this.CmbPage_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(164, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 37);
            this.label1.TabIndex = 3;
            this.label1.Text = "/1";
            // 
            // frmViewVisitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(15F, 37F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Highlight;
            this.ClientSize = new System.Drawing.Size(1201, 606);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(8, 9, 8, 9);
            this.Name = "frmViewVisitor";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = ":: VIEW ::";
            this.Load += new System.EventHandler(this.frmViewVisitor_Load);
            this.panel5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panelPage.ResumeLayout(false);
            this.panelPage.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panelPage;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lbPage;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_type;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_number;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_in;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_out;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_photo;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_card;
        private System.Windows.Forms.ComboBox cmbPage;
        private System.Windows.Forms.Label label1;
    }
}