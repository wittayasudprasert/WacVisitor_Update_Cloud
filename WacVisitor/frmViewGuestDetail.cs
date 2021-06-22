using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using Npgsql;

namespace WacVisitor
{
    public partial class frmViewGuestDetail : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public frmViewGuestDetail()
        {
            InitializeComponent();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        }

        private void frmViewGuestDetail_Load(object sender, EventArgs e)
        {
            int id = classGlobal.pub_id;

            string query = "";
            DataRow[] drSelectedLogs = null;

            #region ++ ข้อมูล visitor การเข้าออก
            DataTable dtID = new DataTable("info");

            query = "SELECT  t1.card_number, t1.str_imagedocument, t1.str_imagewebcamera, t1.status_in, t1.status_out, t2.typename " +
                                            "FROM tbl_visitor t1 INNER JOIN tbl_type t2 ON t1.typeid = t2.typeid " +
                                            "WHERE id=" + id;

            if (classGlobal.databaseType == "acc")
            {
                OleDbDataAdapter ad = new OleDbDataAdapter(query, classGlobal.conn);
                ad.Fill(dtID);
                ad.Dispose();
                ad = null;
            }
            else if (classGlobal.databaseType == "psql")
            {
                NpgsqlDataAdapter ad = new NpgsqlDataAdapter(query, classGlobal.connP);
                ad.Fill(dtID);
                ad.Dispose();
                ad = null;
            }
            else
            {
                if (id == 0)  // เกิดจากการกดดูแต่ละประเภท
                {                    
                    drSelectedLogs = classGlobal.dtVisitor.Select("visitorId='" + classGlobal.public_visitorId + "'"); 
                }
                else  // เกิดจากการกดเมนูดูรายการ
                {
                    //drSelectedLogs = classGlobal.dtAllLogs.Select("visitorId='" + classGlobal.public_visitorId + "'");
                    drSelectedLogs = classGlobal.dtAllLogsView.Select("id='" + id + "'");
                }


                string image1 = ClassData.DOWNLOAD_IMAGE(drSelectedLogs[0]["image1"].ToString(), 260, 160);
                string image2 = ClassData.DOWNLOAD_IMAGE(drSelectedLogs[0]["image2"].ToString(), 260, 160);

                dtID.Columns.Add("card_number");
                dtID.Columns.Add("str_imagedocument");
                dtID.Columns.Add("str_imagewebcamera");
                dtID.Columns.Add("status_in");
                dtID.Columns.Add("status_out");
                dtID.Columns.Add("typename");

                string _cardNumber = "";
                string _status_in = "";
                string _status_out = "";
                string _typename = "";
                _cardNumber = drSelectedLogs[0]["card_number"].ToString();
                _status_in = drSelectedLogs[0]["status_in"].ToString();
                _status_out = drSelectedLogs[0]["status_out"].ToString();
                _typename = drSelectedLogs[0]["typename"].ToString();

                dtID.Rows.Add(_cardNumber, image1, image2, _status_in, _status_out, _typename);  
            }

            string card_no = dtID.Rows[0][0].ToString();
            string baseCardImage = dtID.Rows[0][1].ToString();
            string baseCamera = dtID.Rows[0][2].ToString();

            if (baseCardImage != "")  
            {
                pictureBox1.Image = Base64ToImage(baseCardImage);
            }
            else
            {
                pictureBox1.Image = Image.FromFile(@"icon\unknown.png");
            }

            if (baseCamera != "")
            {
                pictureBox2.Image = Base64ToImage(baseCamera);
            }
            else
            {
                pictureBox2.Image = Image.FromFile(@"icon\unknown.png");
            }
           
            

            label1.Text = "หมายเลข VISITOR : " + dtID.Rows[0][0].ToString();
            label1.Left = (this.ClientSize.Width - label1.Width) / 2;

            label4.Text = "ประเภท VISITOR : " + dtID.Rows[0][5].ToString();
            label4.Left = (this.ClientSize.Width - label4.Width) / 2;

            string[] arr1 = new string[0];
            string[] arr2 = new string[0];
            int beYearFormat = 0;

            string strIN = dtID.Rows[0][3].ToString();
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

            string strOUT = dtID.Rows[0][4].ToString();
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

            label2.Text = "เวลาเข้า : " + strIN;
            label3.Text = "เวลาออก : " + strOUT;

            dtID.Dispose();
            #endregion

            if (classGlobal.DisplayHashTag == true)
            {
                if (baseCardImage != "")
                {
                    pictureBox1.Image = Base64ToImage(classGlobal.PLACEWATERMARK_FROM_BASE64(baseCardImage));
                }
                else
                {
                    byte[] bytes = System.IO.File.ReadAllBytes(@"icon\unknown.png");
                    bytes = classGlobal.PLACEWATERMARK_FROM_BYTE(bytes);
                    pictureBox1.Image = (Image)((new ImageConverter()).ConvertFrom(bytes));
                }

                if (baseCamera != "")
                {
                    pictureBox2.Image = Base64ToImage(classGlobal.PLACEWATERMARK_FROM_BASE64(baseCamera));
                }
                else
                {
                    byte[] bytes = System.IO.File.ReadAllBytes(@"icon\unknown.png");
                    bytes = classGlobal.PLACEWATERMARK_FROM_BYTE(bytes);
                    pictureBox2.Image = (Image)((new ImageConverter()).ConvertFrom(bytes));
                }
            }
            else
            {
                if (baseCardImage != "")
                {
                    pictureBox1.Image = Base64ToImage(classGlobal.PLACEWATERMARK_FROM_BASE64(baseCardImage));
                }
                else
                {
                    byte[] bytes = System.IO.File.ReadAllBytes(@"icon\unknown.png");
                    bytes = classGlobal.PLACEWATERMARK_FROM_BYTE(bytes);
                    pictureBox1.Image = (Image)((new ImageConverter()).ConvertFrom(bytes));
                }

                if (baseCamera != "")
                {
                    pictureBox2.Image = Base64ToImage(classGlobal.PLACEWATERMARK_FROM_BASE64(baseCamera));
                }
                else
                {
                    byte[] bytes = System.IO.File.ReadAllBytes(@"icon\unknown.png");
                    bytes = classGlobal.PLACEWATERMARK_FROM_BYTE(bytes);
                    pictureBox2.Image = (Image)((new ImageConverter()).ConvertFrom(bytes));
                }
            }



            #region ++ ข้อมูลส่วนบุคคล
            query = "SELECT  t2.id_number, t2.th_title, t2.th_firstname, t2.th_lastname " + 
                                        "FROM tbl_visitor t1 INNER JOIN tbl_idcard t2 ON t1.id = t2.id " +
                                            "WHERE t1.id=" + id;

            if (classGlobal.databaseType == "acc")
            {
                OleDbDataAdapter ad = new OleDbDataAdapter(query, classGlobal.conn);
                try
                {
                    dtID = new DataTable("info");
                    ad.Fill(dtID);
                }
                catch
                {
                    //
                }
                ad.Dispose();
                ad = null;
            }
            else if (classGlobal.databaseType == "psql")
            {
                NpgsqlDataAdapter ad = new NpgsqlDataAdapter(query, classGlobal.connP);
                try
                {
                    dtID = new DataTable("info");
                    ad.Fill(dtID);
                }
                catch
                {
                    //
                }
                ad.Dispose();
                ad = null;
            }
            else
            {
                // do nothing (อ่านเลขบัตรประจำตัว + ชื่อ-สกุล)
            }

            string personalId = "";
            string fullanme = "";

            if (dtID.Rows.Count > 0)
            {
                if (classGlobal.userId == "")
                {
                    personalId = dtID.Rows[0][0].ToString();
                    fullanme = dtID.Rows[0][1].ToString() + "" + dtID.Rows[0][2].ToString() + " " + dtID.Rows[0][3].ToString();
                }
                else
                {
                    personalId = drSelectedLogs[0]["citizenId"].ToString();
                    fullanme = drSelectedLogs[0]["vname"].ToString(); 
                }
                
            }
            else
            {
                query = "SELECT  t2.DocumentNo, t2.Familyname, t2.Givenname, t2.Nationality " +
                                        "FROM tbl_visitor t1 INNER JOIN tbl_passport t2 ON t1.id = t2.id " +
                                            "WHERE t1.id=" + id;

                if (classGlobal.databaseType == "acc")
                {
                    OleDbDataAdapter ad = new OleDbDataAdapter(query, classGlobal.conn);
                    try
                    {
                        dtID = new DataTable("info");
                        ad.Fill(dtID);
                    }
                    catch
                    {
                        //
                    }
                    ad.Dispose();
                    ad = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    NpgsqlDataAdapter ad = new NpgsqlDataAdapter(query, classGlobal.connP);
                    try
                    {
                        dtID = new DataTable("info");
                        ad.Fill(dtID);
                    }
                    catch
                    {
                        //
                    }
                    ad.Dispose();
                    ad = null;
                }
                else
                {
                    // do nothing (อ่านเลขบัตรประจำตัว + ชื่อ-สกุล)
                }

                if (dtID.Rows.Count > 0)
                {
                    if (classGlobal.userId == "")
                    {
                        personalId = dtID.Rows[0][0].ToString();
                        fullanme = dtID.Rows[0][1].ToString() + " " + dtID.Rows[0][2].ToString();
                    }
                    else
                    {
                        personalId = drSelectedLogs[0]["citizenId"].ToString();
                        fullanme = drSelectedLogs[0]["vname"].ToString();
                    }
                }
                else
                {
                    query = "SELECT  t2.id_number, t2.fullname " +
                                        "FROM tbl_visitor t1 INNER JOIN tbl_personal t2 ON t1.id = t2.id " +
                                            "WHERE t1.id=" + id;

                    if (classGlobal.databaseType == "acc")
                    {
                        OleDbDataAdapter ad = new OleDbDataAdapter(query, classGlobal.conn);
                        try
                        {
                            dtID = new DataTable("info");
                            ad.Fill(dtID);
                        }
                        catch
                        {
                            //
                        }
                        ad.Dispose();
                        ad = null;
                    }
                    else if (classGlobal.databaseType == "psql")
                    {
                        NpgsqlDataAdapter ad = new NpgsqlDataAdapter(query, classGlobal.connP);
                        try
                        {
                            dtID = new DataTable("info");
                            ad.Fill(dtID);
                        }
                        catch
                        {
                            //
                        }
                        ad.Dispose();
                        ad = null;
                    }
                    else
                    {

                    }

                    if (dtID.Rows.Count > 0)
                    {
                        if (classGlobal.userId == "")
                        {
                            personalId = dtID.Rows[0][0].ToString();
                            fullanme = dtID.Rows[0][1].ToString();
                        }
                        else
                        {
                            personalId = drSelectedLogs[0]["citizenId"].ToString();
                            fullanme = drSelectedLogs[0]["vname"].ToString();
                        }
                        
                    }
                }

            }

            personalId = "#############";
            if (classGlobal.DisplayHashTag == true)
            {
                //personalId = classGlobal.REPLACE_IDCARD(personalId);
                fullanme = classGlobal.REPLACE_NAME(fullanme);
            }

            label5.Text = "เลขประจำตัว : " + personalId;
            label5.Left = (this.ClientSize.Width - label5.Width) / 2;

            label6.Text = "ชื่อ-สกุล : " + fullanme;
            label6.Left = (this.ClientSize.Width - label6.Width) / 2;
            #endregion

            #region ++ ข้อมูลอื่นๆ 
            string place = "";
            string license = "";
            string vehicleType = "";
            string etc = "";

            query = "SELECT * FROM tbl_moreinfo WHERE id=" + id;

            if (classGlobal.databaseType == "acc")
            {
                OleDbDataAdapter ad = new OleDbDataAdapter(query, classGlobal.conn);
                try
                {
                    dtID = new DataTable("info");
                    ad.Fill(dtID);
                }
                catch
                {
                    //
                }
                ad.Dispose();
                ad = null;
            }
            else if (classGlobal.databaseType == "psql")
            {
                NpgsqlDataAdapter ad = new NpgsqlDataAdapter(query, classGlobal.connP);
                try
                {
                    dtID = new DataTable("info");
                    ad.Fill(dtID);
                }
                catch
                {
                    //
                }
                ad.Dispose();
                ad = null;
            }
            else
            {

            }

            panel1.Visible = false;
            if (dtID.Rows.Count > 0)
            {
                place = dtID.Rows[0][1].ToString();
                license = dtID.Rows[0][2].ToString();
                vehicleType = dtID.Rows[0][3].ToString();
                etc = dtID.Rows[0][4].ToString();

                if (classGlobal.userId != "")
                {
                    place = drSelectedLogs[0]["place"].ToString();
                    license = drSelectedLogs[0]["register"].ToString();
                    vehicleType = drSelectedLogs[0]["vehicleType"].ToString();
                    etc = drSelectedLogs[0]["etc"].ToString();
                }

                panel1.Visible = true;

                label7.Text = "สถานที่ติดต่อ : " + place;
                label8.Text = "เลขทะเบียนรถ : " + license;
                label9.Text = "ชนิดของรถ : " + vehicleType;
                label10.Text = "อื่นๆ : " + etc;


                label7.Left = (this.ClientSize.Width - label7.Width) / 2;
                label8.Left = (this.ClientSize.Width - label8.Width) / 2;
                label9.Left = (this.ClientSize.Width - label9.Width) / 2;
                label10.Left = (this.ClientSize.Width - label10.Width) / 2;
            }

            #endregion

        }



        public Image Base64ToImage(string base64String)
        {
            try
            {
                // Convert base 64 string to byte[]
                byte[] imageBytes = Convert.FromBase64String(base64String);
                // Convert byte[] to Image
                using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                {
                    Image image = Image.FromStream(ms, true);
                    return image;
                }
            }
            catch
            {
                return null;
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }


        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Control c = Control.FromHandle(msg.HWnd);
            if (keyData == Keys.F11)
            {
                if (classGlobal.fastCheckOut == true)
                {
                    if (label3.Text == "เวลาออก : -")
                    {
                        string visitorNumber = label1.Text.Replace("หมายเลข VISITOR : ", "").Replace(" ", "");

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
