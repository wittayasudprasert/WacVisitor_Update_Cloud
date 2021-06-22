using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using Npgsql;

namespace WacVisitor
{
    public partial class FormViewVisitorFactory : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        public FormViewVisitorFactory()
        {
            InitializeComponent();

            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.WindowState = FormWindowState.Normal;

        }

        Image imgUnknows = Image.FromFile(@"icon\unknown.png");

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormViewVisitorFactory_Load(object sender, EventArgs e)
        {
            int id = classGlobal.pub_id;   // 
            DataRow[] drSelectedLogs = null;

            DataTable dt = new DataTable("allDetail");

            string query = "SELECT tbl_visitor.*,  tbl_personal.*, tbl_moreinfo_factory.* " +
                            "FROM (tbl_visitor LEFT JOIN tbl_personal ON tbl_visitor.id = tbl_personal.id) " +
                            "LEFT JOIN tbl_moreinfo_factory ON tbl_personal.id = tbl_moreinfo_factory.vid " +
                            "WHERE tbl_visitor.id = " + classGlobal.pub_id;

            if (classGlobal.databaseType == "acc")
            {
                OleDbCommand cmd = new OleDbCommand(query, classGlobal.conn);
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                da.Fill(dt);
                da.Dispose();
                da = null;
            }
            else if (classGlobal.databaseType == "psql")
            {
                NpgsqlCommand cmd = new NpgsqlCommand(query, classGlobal.connP);
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                da.Fill(dt);
                da.Dispose();
                da = null;
            }
            else
            { 

                if (classGlobal.public_visitorId == ""){
                    drSelectedLogs = classGlobal.dtAllLogsView.Select("id='" + id + "'");
                }
                else
                {
                    if (classGlobal.dtLogsPaging == null || classGlobal.dtLogsPaging.Rows.Count == 0)
                    {
                        drSelectedLogs = classGlobal.dtAllLogs.Select("visitorId='" + classGlobal.public_visitorId + "'");
                        if (drSelectedLogs.Length == 0)
                        {
                            drSelectedLogs = classGlobal.dtAllLogsView.Select("visitorId='" + classGlobal.public_visitorId + "'");
                        }
                    }
                    else
                    {
                        drSelectedLogs = classGlobal.dtLogsPaging.Select("visitorId='" + classGlobal.public_visitorId + "'");
                        if (drSelectedLogs.Length == 0)
                        {
                            drSelectedLogs = classGlobal.dtAllLogsView.Select("visitorId='" + classGlobal.public_visitorId + "'");
                        }
                    }
                    
                }
                    
                DataRow dr = drSelectedLogs[0];

                pictureBox1.Image = Base64ToImage(ClassData.DOWNLOAD_IMAGE(dr["image1"].ToString(), 260, 160));
                pictureBox2.Image = Base64ToImage(ClassData.DOWNLOAD_IMAGE(dr["image2"].ToString(), 260, 160));
                pictureBox3.Image = Base64ToImage(ClassData.DOWNLOAD_IMAGE(dr["image3"].ToString(), 260, 160));
                pictureBox4.Image = Base64ToImage(ClassData.DOWNLOAD_IMAGE(dr["image4"].ToString(), 260, 160));

                string[] arr1 = new string[0];
                string[] arr2 = new string[0];
                int beYearFormat = 0;
                string strIN = dr["recordTimeIn"].ToString();
                if (strIN != "")
                {
                    arr1 = strIN.Split(' ');
                    arr2 = arr1[0].Split('-');
                    beYearFormat = Int32.Parse(arr2[0]) + 543;
                    strIN = arr2[2] + "/" + arr2[1] + "/" + beYearFormat.ToString() + " " + arr1[1];
                }
                else
                {
                    strIN = "-";
                }

                string strOUT = dr["recordTimeOut"].ToString();
                if (strOUT != "")
                {
                    arr1 = strOUT.Split(' ');
                    arr2 = arr1[0].Split('-');
                    beYearFormat = Int32.Parse(arr2[0]) + 543;
                    strOUT = arr2[2] + "/" + arr2[1] + "/" + beYearFormat.ToString() + " " + arr1[1];
                }
                else
                {
                    strOUT = "-";
                }

                label1.Text = "หมายเลข Visitor : " + dr["visitorNumber"].ToString();
                label2.Text = "ประเภท Visitor : " + dr["visitorType"].ToString();
                label3.Text = "หมายเลขประจำตัว : " + dr["citizenId"].ToString();
                label4.Text = "ชื่อ - สกุล : " + dr["name"].ToString();
                label5.Text = "เวลาเข้า : " + strIN;
                label6.Text = "เวลาออก : " + strOUT;


                label7.Text = label7.Text.Replace(": -", ": ") + dr["follower"].ToString() + " คน";
                label8.Text = label8.Text.Replace(": -", ": ") + dr["visitorFrom"].ToString();
                label9.Text = label9.Text.Replace(": -", ": ") + dr["licensePlate"].ToString();
                label10.Text = label10.Text.Replace(": -", ": ") + dr["vehicleType"].ToString();
                label11.Text = label11.Text.Replace(": -", ": ") + dr["visitPerson"].ToString();
                label12.Text = label12.Text.Replace(": -", ": ") + dr["department"].ToString();
                label13.Text = label13.Text.Replace(": -", ": ") + dr["contactTopic"].ToString();
                label14.Text = label14.Text.Replace(": -", ": ") + dr["contactPlace"].ToString();
                label15.Text = label15.Text.Replace(": -", ": ") + dr["etc"].ToString();

                label3.Text = "หมายเลขประจำตัว : " + "#############";
                if (classGlobal.DisplayHashTag == true)
                {
                    //label3.Text = "หมายเลขประจำตัว : " + classGlobal.REPLACE_IDCARD(dr["citizenId"].ToString());
                    label4.Text = "ชื่อ - สกุล : " + classGlobal.REPLACE_NAME(dr["name"].ToString());
                }

                goto SHOW_RESULT;
            }

            dt.Columns.Add("typename", typeof(String));

            foreach (DataRow dr in dt.Rows)
            {
                Application.DoEvents();
                System.Threading.Thread.Sleep(5);

                dr[6] = ConvertDateTime(dr.ItemArray[6].ToString());  //status_in
                dr[7] = ConvertDateTime(dr.ItemArray[7].ToString());  //status_out

                dr["typename"] = GetTypeVisitorText(dr.ItemArray[8].ToString());

                pictureBox1.Image = Base64ToImage(dr["str_imagedocument"].ToString());
                pictureBox2.Image = Base64ToImage(dr["str_imagewebcamera"].ToString());
                pictureBox3.Image = Base64ToImage(dr["str_imagewebcamera1"].ToString());
                pictureBox4.Image = Base64ToImage(dr["str_imagewebcamera2"].ToString());

                label1.Text = "หมายเลข Visitor : "+ dr["card_number"].ToString();
                label2.Text = "ประเภท Visitor : " + dr["typename"].ToString();
                label3.Text = "หมายเลขประจำตัว : " + dr["id_number"].ToString();
                label4.Text = "ชื่อ - สกุล : " + dr["fullname"].ToString();
                label5.Text = "เวลาเข้า : " + dr["status_in"].ToString();
                label6.Text = "เวลาออก : " + dr["status_out"].ToString();


                label7.Text = label7.Text.Replace(": -", ": ") + dr["follower"].ToString() + " คน";
                label8.Text = label8.Text.Replace(": -", ": ") + dr["company"].ToString();
                label9.Text = label9.Text.Replace(": -", ": ") + dr["license_plate"].ToString();
                label10.Text = label10.Text.Replace(": -", ": ") + dr["vehicle_type"].ToString();
                label11.Text = label11.Text.Replace(": -", ": ") + dr["contact_to"].ToString();
                label12.Text = label12.Text.Replace(": -", ": ") + dr["department"].ToString();
                label13.Text = label13.Text.Replace(": -", ": ") + dr["topic"].ToString();
                label14.Text = label14.Text.Replace(": -", ": ") + dr["place"].ToString();
                label15.Text = label15.Text.Replace(": -", ": ") + dr["etc"].ToString();


                label3.Text = "หมายเลขประจำตัว : " + "#############";
                if (classGlobal.DisplayHashTag == true)
                {
                    pictureBox1.Image = Base64ToImage(dr["str_imagedocument"].ToString());
                    pictureBox2.Image = Base64ToImage(dr["str_imagewebcamera"].ToString());
                    pictureBox3.Image = Base64ToImage(dr["str_imagewebcamera1"].ToString());
                    pictureBox4.Image = Base64ToImage(dr["str_imagewebcamera2"].ToString());

                    //label3.Text = "หมายเลขประจำตัว : " + classGlobal.REPLACE_IDCARD(dr["id_number"].ToString());
                    label4.Text = "ชื่อ - สกุล : " + classGlobal.REPLACE_NAME(dr["fullname"].ToString());
                }

            }

            SHOW_RESULT:
            label1.Left = panel1.Width - label1.Width;
            label2.Left = panel1.Width - label2.Width;
            label3.Left = panel1.Width - label3.Width;
            label4.Left = panel1.Width - label4.Width;
            label5.Left = panel1.Width - label5.Width;
            label6.Left = panel1.Width - label6.Width;

        }

        private string GetTypeVisitorText(string typeid)
        {
            try
            {
                if (typeid == "")
                {
                    return "ไม่ระบุ";
                }
                else
                {

                    DataTable _dt = new DataTable("_dt");
                    if (classGlobal.databaseType == "acc")
                    {
                        OleDbCommand cmd = new OleDbCommand("SELECT typename FROM tbl_type WHERE typeid=" + Int32.Parse(typeid), classGlobal.conn);
                        OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                        da.Fill(_dt);
                        da.Dispose();
                        da = null;
                        typeid = _dt.Rows[0][0].ToString();
                        _dt.Dispose();
                        _dt = null;
                    }
                    else if (classGlobal.databaseType == "psql")
                    {
                        NpgsqlCommand cmd = new NpgsqlCommand("SELECT typename FROM tbl_type WHERE typeid=" + Int32.Parse(typeid), classGlobal.connP);
                        NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);                        
                        da.Fill(_dt);
                        da.Dispose();
                        da = null;
                        typeid = _dt.Rows[0][0].ToString();
                        _dt.Dispose();
                        _dt = null;
                    }
                    else
                    {

                    }
                    
                    return typeid;
                }

            }
            catch
            {
                return "ไม่ระบุ";
            }
        }
        private string ConvertDateTime(string strInput)
        {
            try    //// 2018-11-20 16:35:05 
            {
                string[] a = strInput.Split(' ');
                string[] b = a[0].ToString().Split('-');

                int year = Int32.Parse(b[0].ToString());
                if (year < 2500)
                {
                    year = year + 543;
                }

                strInput = b[2] + "/" + b[1] + "/" + year.ToString() + " " + a[1].ToString();
                return strInput;
            }
            catch
            {
                return strInput;
            }
        }
        private Image Base64ToImage(string base64String)
        {
            try
            {
                if (classGlobal.DisplayHashTag == true)
                {
                    base64String = classGlobal.PLACEWATERMARK_FROM_BASE64(base64String);
                }
                else
                {
                    base64String = classGlobal.PLACEWATERMARK_FROM_BASE64(base64String);
                }


                // Convert base 64 string to byte[]
                byte[] imageBytes = Convert.FromBase64String(base64String);
                // Convert byte[] to Image
                using (var ms = new System.IO.MemoryStream(imageBytes, 0, imageBytes.Length))
                {
                    Image image = Image.FromStream(ms, true);
                    //image.Save(@"Webcam\temp.png");
                    return image;
                }
            }
            catch
            {
                if (classGlobal.DisplayHashTag == true)
                    imgUnknows = classGlobal.PLACEWATERMARK_FROM_BITMAP((Bitmap)imgUnknows);
                else
                    imgUnknows = classGlobal.PLACEWATERMARK_FROM_BITMAP((Bitmap)imgUnknows);

                return imgUnknows;
            }

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Control c = Control.FromHandle(msg.HWnd);
            if (keyData == Keys.F11)
            {
                if (classGlobal.fastCheckOut == true)
                {
                    if (label6.Text == "เวลาออก : -")
                    {
                        string visitorNumber = label1.Text.Replace("หมายเลข Visitor : ", "").Replace(" ", "");

                        string jsonString = ClassData.POST_CHECK_EXIST_VISITOR_NUMBER(classGlobal.access_token, classGlobal.userId, visitorNumber, "out", true);
                        Newtonsoft.Json.Linq.JObject js = Newtonsoft.Json.Linq.JObject.Parse(jsonString);
                        Newtonsoft.Json.Linq.JToken jMessage = js["message"];
                        string visitorId = (String)jMessage[0]["visitorId"];
                        string result = ClassData.POST_VISITOR_OUT(classGlobal.access_token, classGlobal.userId, visitorId, classGlobal.terminalId);
                    }
                }                
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
