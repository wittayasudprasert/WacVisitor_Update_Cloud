using ClassHelper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WacVisitor.FormSetting
{
    public partial class FormSettingEtc : Form
    {
        public FormSettingEtc()
        {
            InitializeComponent();
        }

        clsXML c = new clsXML();
        private void FormSettingEtc_Load(object sender, EventArgs e)
        {
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            panel1.Left = (this.Width - panel1.ClientSize.Width) / 2;


            classGlobal.strPlace = c.GetReadXML("root", "place", classGlobal.config);
            txtPlace.Text = classGlobal.strPlace;

            classGlobal.watermark = c.GetReadXML("root", "watermark", classGlobal.config).ToString();
            txtWatermask.Text = classGlobal.watermark;

        }

        private void button2_Click(object sender, EventArgs e)
        {           
            this.Close();
        }

        private void FormSettingEtc_FormClosing(object sender, FormClosingEventArgs e)
        {
            c = null;
        }

        private void button4_Click(object sender, EventArgs e)
        {            
            if (txtPlace.Text == "" || txtWatermask.Text == "")
            {
                frmMessageBox f = new frmMessageBox();
                f.strMessage = "กรุณากรอกข้อมูล";
                f.strStatus = "Warning";  
                f.ShowDialog();
                return;
            }

            c.ModifyElement("root", "place", txtPlace.Text , classGlobal.config);
            c.ModifyElement("root", "watermark", txtWatermask.Text, classGlobal.config);

            classGlobal.strPlace = txtPlace.Text;
            classGlobal.watermark = txtWatermask.Text;

            frmMessageBox f1 = new frmMessageBox();
            f1 = new frmMessageBox();
            f1.strMessage = "สำเร็จ";
            f1.strStatus = "Information";
            f1.ShowDialog();

        }

    }
}
