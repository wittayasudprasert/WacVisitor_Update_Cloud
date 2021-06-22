namespace WacVisitor
{
    partial class frmAppointment
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
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.rbtnIN = new WacVisitor.RoundButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.listView1 = new System.Windows.Forms.ListView();
            this.roundButton1 = new WacVisitor.RoundButton();
            this.roundButton2 = new WacVisitor.RoundButton();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Consolas", 32F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.White;
            this.button1.Location = new System.Drawing.Point(1050, -15);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(78, 71);
            this.button1.TabIndex = 0;
            this.button1.Text = "x";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(199, 37);
            this.label1.TabIndex = 1;
            this.label1.Text = "สร้างการนัดหมาย";
            // 
            // rbtnIN
            // 
            this.rbtnIN.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbtnIN.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.rbtnIN.ForeColor = System.Drawing.Color.Blue;
            this.rbtnIN.Location = new System.Drawing.Point(125, 391);
            this.rbtnIN.Name = "rbtnIN";
            this.rbtnIN.Size = new System.Drawing.Size(91, 46);
            this.rbtnIN.TabIndex = 15;
            this.rbtnIN.Text = "บันทึก";
            this.rbtnIN.UseVisualStyleBackColor = true;
            this.rbtnIN.Click += new System.EventHandler(this.RbtnIN_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Consolas", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(15, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 22);
            this.label2.TabIndex = 16;
            this.label2.Text = "ผู้เข้าพบ :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Consolas", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(15, 117);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 22);
            this.label3.TabIndex = 17;
            this.label3.Text = "ทะเบียนรถ :";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Consolas", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(15, 180);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(123, 22);
            this.label4.TabIndex = 18;
            this.label4.Text = "สถานที่นัดหมาย :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Consolas", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(15, 245);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(106, 22);
            this.label5.TabIndex = 19;
            this.label5.Text = "เวลานัดหมาย :";
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(19, 82);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(302, 30);
            this.textBox1.TabIndex = 20;
            // 
            // textBox2
            // 
            this.textBox2.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.Location = new System.Drawing.Point(19, 142);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(302, 30);
            this.textBox2.TabIndex = 21;
            // 
            // textBox3
            // 
            this.textBox3.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox3.Location = new System.Drawing.Point(19, 205);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(302, 30);
            this.textBox3.TabIndex = 22;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.dateTimePicker1.CustomFormat = "dd MMMM yyyy  HH:mm";
            this.dateTimePicker1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker1.Location = new System.Drawing.Point(19, 270);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(302, 32);
            this.dateTimePicker1.TabIndex = 23;
            this.dateTimePicker1.Value = new System.DateTime(2021, 3, 5, 11, 27, 0, 0);
            // 
            // listView1
            // 
            this.listView1.BackColor = System.Drawing.Color.White;
            this.listView1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            this.listView1.ForeColor = System.Drawing.Color.Black;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(358, 57);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(745, 380);
            this.listView1.TabIndex = 24;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.ListView1_SelectedIndexChanged);
            // 
            // roundButton1
            // 
            this.roundButton1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.roundButton1.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.roundButton1.ForeColor = System.Drawing.Color.Blue;
            this.roundButton1.Location = new System.Drawing.Point(231, 391);
            this.roundButton1.Name = "roundButton1";
            this.roundButton1.Size = new System.Drawing.Size(92, 46);
            this.roundButton1.TabIndex = 25;
            this.roundButton1.Text = "ลบ";
            this.roundButton1.UseVisualStyleBackColor = true;
            this.roundButton1.Click += new System.EventHandler(this.RoundButton1_Click);
            // 
            // roundButton2
            // 
            this.roundButton2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.roundButton2.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.roundButton2.ForeColor = System.Drawing.Color.Blue;
            this.roundButton2.Location = new System.Drawing.Point(19, 391);
            this.roundButton2.Name = "roundButton2";
            this.roundButton2.Size = new System.Drawing.Size(91, 46);
            this.roundButton2.TabIndex = 26;
            this.roundButton2.Text = "เพิ่ม";
            this.roundButton2.UseVisualStyleBackColor = true;
            this.roundButton2.Click += new System.EventHandler(this.RoundButton2_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Consolas", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(15, 315);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 22);
            this.label6.TabIndex = 27;
            this.label6.Text = "สถานะ :";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.Font = new System.Drawing.Font("Consolas", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "นัด",
            "มา",
            "พบ",
            "ไม่พบ",
            "ยกเลิก"});
            this.comboBox1.Location = new System.Drawing.Point(19, 340);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(302, 36);
            this.comboBox1.TabIndex = 28;
            // 
            // frmAppointment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(1126, 454);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.roundButton2);
            this.Controls.Add(this.roundButton1);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.rbtnIN);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Name = "frmAppointment";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmAppointment";
            this.Load += new System.EventHandler(this.FrmAppointment_Load);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FrmAppointment_MouseMove);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private RoundButton rbtnIN;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.ListView listView1;
        private RoundButton roundButton1;
        private RoundButton roundButton2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBox1;
    }
}