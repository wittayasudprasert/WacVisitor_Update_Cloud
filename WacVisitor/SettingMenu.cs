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

namespace WacVisitor
{
    public partial class SettingMenu : Form
    {
        public SettingMenu()
        {
            InitializeComponent();
        }

        private void SettingMenu_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            panel2.Left = ((this.Width / 2) - (panel2.Width / 2));
            panel2.Top = ((this.Height / 2) - (panel2.Height / 2));

            if (classGlobal.userId != "")
            {
                btnImport.Visible = false;
            }
            else
            {
                if (classGlobal.FactoryVersion == false)
                    btnImport.Visible = false;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
           this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormSetting.SettingVisitorType f = new FormSetting.SettingVisitorType();
            f.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            classGlobal.strBlackOrWhiteList = "black"; 
            FormSetting.SettingBlackList f = new FormSetting.SettingBlackList();
            f.ShowDialog();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            classGlobal.strBlackOrWhiteList = "white";
            FormSetting.SettingBlackList f = new FormSetting.SettingBlackList();
            f.ShowDialog();
        }
        
        private void button13_Click(object sender, EventArgs e)
        {
            int WebCamIndex = -1;
            WebcamDevice wc = new WebcamDevice();
            wc.ShowDialog();
            WebCamIndex = classGlobal.SelectedWebcamDevice;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            FormSetting.SettingCompany f = new FormSetting.SettingCompany();
            f.ShowDialog();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            FormSetting.SettingLicense f = new FormSetting.SettingLicense();
            f.ShowDialog();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            FormSetting.SettingVehicleType f = new FormSetting.SettingVehicleType();
            f.ShowDialog();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            FormSetting.SettingVisitTo f = new FormSetting.SettingVisitTo();
            f.ShowDialog();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            FormSetting.SettingDepartment f = new FormSetting.SettingDepartment();
            f.ShowDialog();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            FormSetting.SettingBusinessTopic f = new FormSetting.SettingBusinessTopic();
            f.ShowDialog();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            FormSetting.SettingPlace f = new FormSetting.SettingPlace();
            f.ShowDialog();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            //classGlobal.DELETE_FOR_BACKUP();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FormSetting.FormSettingEtc f = new FormSetting.FormSettingEtc();
            f.ShowDialog();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Excel Files|*.xlsx";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop).ToString();
            dialog.Title = "กรุณาเลือกไฟล์เพื่อนำเข้ารายการ";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = dialog.FileName;
                DataTable dt = classGlobal.GetDataFromXLSX(fileName, true);

                for (int c = 0; c < dt.Columns.Count; c++)
                    dt.Columns[c].ColumnName = dt.Columns[c].ColumnName.ToString().ToLower().Replace(" ", "");

                string tableName = "";
                string strFiled = "";
                string colName = "";
                foreach (DataColumn dc in dt.Columns)
                {
                    colName = dc.ToString();
                    colName = colName.Replace("ทะเบียนรถ", "tbl_license_plate");
                    colName = colName.Replace("บริษัท", "tbl_visitor_company");
                    colName = colName.Replace("ผู้รับการติดต่อ", "tbl_visit_to");
                    colName = colName.Replace("ชนิดรถ", "tbl_vehicle_type");
                    colName = colName.Replace("ติดต่อเรื่อง", "tbl_business_topic");
                    colName = colName.Replace("แผนกที่ติดต่อ", "tbl_department");
                    colName = colName.Replace("สถานที่ติดต่อ", "tbl_place");
                    tableName = colName;

                    if (tableName == "tbl_license_plate") strFiled = "license_plate";
                    if (tableName == "tbl_visitor_company") strFiled = "visitor_company";
                    if (tableName == "tbl_visit_to") strFiled = "visit_to";
                    if (tableName == "tbl_vehicle_type") strFiled = "vehicle_type";
                    if (tableName == "tbl_business_topic") strFiled = "business_topic";
                    if (tableName == "tbl_department") strFiled = "department";
                    if (tableName == "tbl_place") strFiled = "place";

                    OleDbDataAdapter da;
                    OleDbCommand cmd;

                    NpgsqlDataAdapter nda;
                    NpgsqlCommand ncmd;

                    DataTable dtCount;
                    foreach (DataRow dtRow in dt.Rows)
                    {
                        var values = dtRow[dc].ToString();
                        if (values != "")
                        {
                            string query = "SELECT COUNT(*) FROM " + tableName + " WHERE " + strFiled + " = '" + values + "'";
                            if (classGlobal.databaseType == "acc")
                            {
                                da = new OleDbDataAdapter(query, classGlobal.conn);
                                dtCount = new DataTable("count");
                                da.Fill(dtCount);
                                da.Dispose();
                                if (int.Parse(dtCount.Rows[0][0].ToString()) == 0)
                                {
                                    query = "INSERT INTO " + tableName + "(" + strFiled + ") VALUES (@" + strFiled + ")";
                                    cmd = new OleDbCommand(query, classGlobal.conn);
                                    cmd.Parameters.Add("@" + strFiled, OleDbType.VarChar).Value = values;
                                    cmd.ExecuteNonQuery();
                                    cmd.Dispose();
                                    cmd = null;
                                }
                            }
                            else if (classGlobal.databaseType == "psql")
                            {
                                nda = new NpgsqlDataAdapter(query, classGlobal.connP);
                                dtCount = new DataTable("count");
                                nda.Fill(dtCount);
                                nda.Dispose();
                                if (int.Parse(dtCount.Rows[0][0].ToString()) == 0)
                                {
                                    query = "INSERT INTO " + tableName + "(" + strFiled + ") VALUES (@" + strFiled + ")";
                                    ncmd = new NpgsqlCommand(query, classGlobal.connP);
                                    ncmd.Parameters.AddWithValue("@" + strFiled, OleDbType.VarChar).Value = values;
                                    ncmd.ExecuteNonQuery();
                                    ncmd.Dispose();
                                    ncmd = null;
                                }
                            }
                        }
                    }
                }


                frmMessageBox f1 = new frmMessageBox();
                f1.strMessage = "สำเร็จ";
                f1.strStatus = "Information";
                f1.ShowDialog();

            }
        }
    }
}
