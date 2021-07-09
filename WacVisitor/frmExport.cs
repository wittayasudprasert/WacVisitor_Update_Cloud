using Npgsql;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
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

using NpgsqlTypes;

namespace WacVisitor
{
    public partial class frmExport : Form
    {
        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
        );

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        bool boolTerminateJob = false;
        DataTable dtAudit = new DataTable("dtAudit");
        public frmExport()
        {
            InitializeComponent();

            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 10, 10));
        }
        private void frmExport_Load(object sender, EventArgs e)
        {
            progressBar1.Visible = false;
            button2.Enabled = false;

            textBox1.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            button2.Enabled = true;

            string dateTime = "";

            dateTime = DateTime.Now.ToString("yyyy-MM-dd") + " " + "00:00:00";
            dateTimePicker1.Value = DateTime.Parse(dateTime, System.Globalization.CultureInfo.InvariantCulture);
            dateTime = DateTime.Now.ToString("yyyy-MM-dd") + " " + "23:59:59";
            dateTimePicker2.Value = DateTime.Parse(dateTime, System.Globalization.CultureInfo.InvariantCulture);
        }
        private void button10_Click(object sender, EventArgs e)
        {
            boolTerminateJob = true;
            this.Close();
        }
        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && fbd.SelectedPath != "")
                {
                    textBox1.Text = fbd.SelectedPath;
                    button2.Enabled = true;
                }
                else
                {
                    button2.Enabled = false;
                }
            }
        }



        //+++ Export 
        int iRandomPicture = 0;
        private void button2_Click(object sender, EventArgs e)
        {

            DataTable dt = QUERY_EXPORT();

            if (dt != null)
            {
                if (dt.Rows.Count == 0)
                {
                    frmMessageBox f = new frmMessageBox();
                    f.strMessage = "ไม่มีข้อมูลสำหรับส่งออก";
                    f.strStatus = "Warning";
                    f.ShowDialog();
                    return;
                }

                string startText = dateTimePicker1.Value.ToString("dd-MM-yyyy_HH_mm_ss");
                string stopText = dateTimePicker2.Value.ToString("dd-MM-yyyy_HH_mm_ss");

                string outputfilename = textBox1.Text + @"\" + startText + "-to-" + stopText + ".xlsx";

                //++ ส่งออกรายงาน excell สำหรับ audit การเข้าออก รายวัน request ของลูกค้าหนุ่มใน version โรงงาน 07/07/2564
                if (classGlobal.FactoryVersion == true)
                    CreateExcelPackageForAudit(dt, textBox1.Text, dateTimePicker1.Value.ToString("yyyy-MM-dd"), dateTimePicker2.Value.ToString("yyyy-MM-dd"));
                //-- ส่งออกรายงาน excell สำหรับ audit การเข้าออก รายวัน request ของลูกค้าหนุ่มใน version โรงงาน 07/07/2564

                if (classGlobal.FactoryVersion == true)
                    CreateExcelPackage_Factory(dt, outputfilename);
                else
                    CreateExcelPackage(dt, outputfilename);
            }
            else
            {
                frmMessageBox f = new frmMessageBox();
                f.strMessage = "ไม่มีข้อมูลสำหรับส่งออก";
                f.strStatus = "Warning";
                f.ShowDialog();
            }
            
        }
        private DataTable QUERY_EXPORT()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            try
            {
                string query = "";
                query = "SELECT tbl_visitor.*,  tbl_personal.*, tbl_moreinfo.place, tbl_moreinfo.register " +
                            "FROM (tbl_visitor LEFT JOIN tbl_personal ON tbl_visitor.id = tbl_personal.id) " +
                            "LEFT JOIN tbl_moreinfo ON tbl_personal.id = tbl_moreinfo.id " +
                            "WHERE tbl_visitor.status_in BETWEEN @startDate AND @endDate";

                if (classGlobal.FactoryVersion == true)
                {
                    query = "SELECT tbl_visitor.*,  tbl_personal.*, tbl_moreinfo_factory.* " +
                            "FROM (tbl_visitor LEFT JOIN tbl_personal ON tbl_visitor.id = tbl_personal.id) " +
                            "LEFT JOIN tbl_moreinfo_factory ON tbl_personal.id = tbl_moreinfo_factory.vid " +
                            "WHERE tbl_visitor.status_in BETWEEN @startDate AND @endDate";
                }


                DataTable dt = new DataTable("dt");
                if (classGlobal.databaseType == "acc")
                {
                    OleDbCommand cmd = new OleDbCommand(query, classGlobal.conn);
                    cmd.Parameters.AddWithValue("@startDate", DbType.DateTime).Value =
                                                                dateTimePicker1.Value.ToString("yyyy-MM-dd HH:mm:ss");
                    cmd.Parameters.AddWithValue("@endDate", DbType.DateTime).Value =
                                                                dateTimePicker2.Value.ToString("yyyy-MM-dd HH:mm:ss");
                    OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                    adapter.Fill(dt);
                    adapter.Dispose();
                    adapter = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    query = query.Replace("@startDate", "'" + dateTimePicker1.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                    query = query.Replace("@endDate", "'" + dateTimePicker2.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                    NpgsqlCommand cmd = new NpgsqlCommand(query, classGlobal.connP);
                    //cmd.Parameters.AddWithValue("@startDate", Char).Value =
                    //                                            dateTimePicker1.Value.ToString("yyyy-MM-dd HH:mm:ss");
                    //cmd.Parameters.AddWithValue("@endDate", NpgsqlDbType.Char).Value =
                    //                                            dateTimePicker2.Value.ToString("yyyy-MM-dd HH:mm:ss");

                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(cmd);
                    adapter.Fill(dt);
                    adapter.Dispose();
                    adapter = null;
                }
                else
                {

                    string startTime = dateTimePicker1.Value.ToString("yyyy-MM-dd HH:mm:ss");
                    string stopTime = dateTimePicker2.Value.ToString("yyyy-MM-dd HH:mm:ss");

                    ////++ LOADING....
                    //DataTable dtLogsComplete = ClassData.GET_LOGS_COMPLETE(true);  // log เข้าออกสำเร้จ
                    //DataTable dtLogsStill = ClassData.GET_LOGS_STILL(true);        // log เข้าแต่ยังไม่ออก

                    //DataTable dtExport = dtLogsComplete.Copy();
                    //dtExport.Merge(dtLogsStill);

                    //DataView dv = dtExport.DefaultView;
                    //if (classGlobal.userId != "")
                    //    dv.Sort = "recordTimeIn " + classGlobal.sortOrderBy;
                    //else
                    //    dv.Sort = "status_in " + classGlobal.sortOrderBy;
                    //dtExport = dv.ToTable();
                    ////-- LOADING....

                    //++ LOADING....
                    List<String> lstDate = classGlobal.GET_LIST_OF_DATE_BETWEEN(dateTimePicker1.Value.Date, dateTimePicker2.Value.Date);
                    Cursor = Cursors.WaitCursor;
                    DataTable dtLogsComplete;
                    DataTable dtLogsStill;
                    classGlobal.dtAllLogsView = new DataTable();
                    for (int d = 0; d < lstDate.Count; d++)
                    {
                        if (boolTerminateJob == true)
                            return null;

                        dtLogsComplete = ClassData.GET_LOGS_COMPLETE(false, lstDate[d]);  // log เข้าออกสำเร้จ
                        classGlobal.dtAllLogsView.Merge(dtLogsComplete);
                        dtLogsStill = ClassData.GET_LOGS_STILL(false, lstDate[d]);        // log เข้าแต่ยังไม่ออก
                        classGlobal.dtAllLogsView.Merge(dtLogsStill);
                    }

                    DataView dv = classGlobal.dtAllLogsView.DefaultView;
                    if (classGlobal.userId != "")
                        dv.Sort = "recordTimeIn " + classGlobal.sortOrderBy;
                    else
                        dv.Sort = "status_in " + classGlobal.sortOrderBy;
                    DataTable dtExport = dv.ToTable();
                    Cursor = Cursors.Default;
                    //-- LOADING....

                    try
                    {
                        
                        DataTable dtFound = dtExport.Select().Where(p => (Convert.ToDateTime(p["recordTimeIn"]) >= Convert.ToDateTime(startTime)) && (Convert.ToDateTime(p["recordTimeIn"]) <= Convert.ToDateTime(stopTime))).CopyToDataTable();
                        if (classGlobal.FactoryVersion == true)
                        {  // id_number   fullname  card_number    typename   status_in   status_out     
                            // follower   company    license_plate   vehicle_type   contact_to  department  topic  place    etc    
                            // str_imagedocument    str_imagewebcamera    str_imagewebcamera1        str_imagewebcamera2
                            dt.Columns.Add("id_number");
                            dt.Columns.Add("fullname");
                            dt.Columns.Add("card_number");
                            dt.Columns.Add("typename");
                            dt.Columns.Add("status_in");
                            dt.Columns.Add("status_out");

                            dt.Columns.Add("follower");
                            dt.Columns.Add("company");
                            dt.Columns.Add("license_plate");
                            dt.Columns.Add("vehicle_type");
                            dt.Columns.Add("contact_to");
                            dt.Columns.Add("department");
                            dt.Columns.Add("topic");
                            dt.Columns.Add("place");
                            dt.Columns.Add("etc");

                            dt.Columns.Add("str_imagedocument");
                            dt.Columns.Add("str_imagewebcamera");
                            dt.Columns.Add("str_imagewebcamera1");
                            dt.Columns.Add("str_imagewebcamera2");


                            string status_in = "";
                            string status_out = "";
                            foreach (DataRow dr in dtFound.Rows)
                            {
                                status_in = dr["recordTimeIn"].ToString();
                                status_out = dr["recordTimeOut"].ToString();
                                dt.Rows.Add(dr["citizenId"], dr["name"], dr["visitorNumber"], dr["visitorType"], status_in, status_out,
                                                 dr["follower"], dr["visitorFrom"], dr["licensePlate"], dr["vehicleType"], dr["visitPerson"],
                                                 dr["department"], dr["contactTopic"], dr["contactPlace"], dr["etc"],
                                                 dr["image1"], dr["image2"], dr["image3"], dr["image4"]);
                            } 
                        }
                        else
                        {  // id_number   fullname   card_number    typename     status_in    status_out    place    register    
                            // str_imagedocument    str_imagewebcamera
                            dt.Columns.Add("id_number");
                            dt.Columns.Add("fullname");
                            dt.Columns.Add("card_number");
                            dt.Columns.Add("typename");
                            dt.Columns.Add("status_in");
                            dt.Columns.Add("status_out");
                            dt.Columns.Add("place");
                            dt.Columns.Add("register");

                            dt.Columns.Add("str_imagedocument");
                            dt.Columns.Add("str_imagewebcamera");

                            string status_in = "";
                            string status_out = "";
                            foreach (DataRow dr in dtFound.Rows)
                            {
                                status_in = dr["recordTimeIn"].ToString();
                                status_out = dr["recordTimeOut"].ToString();

                                dt.Rows.Add(dr["citizenId"], dr["name"], dr["visitorNumber"], dr["visitorType"], status_in, status_out,
                                                 dr["contactPlace"], dr["licensePlate"], dr["image1"], dr["image2"]);  
                            } 
                        }
                        return dt;
                    }
                    catch
                    {
                        return null;
                    }                    

                }

                dt.Columns.Add("typename", typeof(String));

                //dt.Columns.Add("place", typeof(String));
                //dt.Columns.Add("register", typeof(String));

                foreach (DataRow dr in dt.Rows)
                {
                    Application.DoEvents();
                    System.Threading.Thread.Sleep(5);

                    dr[6] = ConvertDateTime(dr.ItemArray[6].ToString());  //status_in
                    dr[7] = ConvertDateTime(dr.ItemArray[7].ToString());  //status_out

                    dr["typename"] = GetTypeVisitorText(dr.ItemArray[8].ToString());
                }

                return dt;
            }
            catch
            {
                return null;
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
        private void CreateExcelPackage(DataTable dt, string xlsxfiles)
        {
            using (var package = new ExcelPackage())
            {
                var workbook = package.Workbook;
                //*** Sheet 1
                var worksheet = workbook.Worksheets.Add("Sheet1");
                worksheet.Row(1).Height = 75;   // set row height

                if (System.IO.File.Exists(xlsxfiles) == false)   //กรณีไม่มีไฟล์ xlsx ให้สร้าง header column 
                {
                    worksheet.Cells[1, 1].Value = "เลข VISITOR";
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[1, 2].Value = "ประเภท VISITOR";
                    worksheet.Cells[1, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[1, 3].Value = "เลขประจำตัว";
                    worksheet.Cells[1, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[1, 4].Value = "ชื่อ-สกุล";
                    worksheet.Cells[1, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[1, 5].Value = "เวลาเข้า";
                    worksheet.Cells[1, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[1, 6].Value = "เวลาออก";
                    worksheet.Cells[1, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                    worksheet.Cells[1, 7].Value = "สถานที่ติดต่อ";
                    worksheet.Cells[1, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[1, 8].Value = "เลขทะเบียนรถ";
                    worksheet.Cells[1, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                    worksheet.Cells[1, 9].Value = "ภาพบัตร";
                    worksheet.Cells[1, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[1, 10].Value = "ภาพถ่าย";
                    worksheet.Cells[1, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    package.SaveAs(new FileInfo(xlsxfiles));   //save file

                }
                else
                {
                    //
                }

                //Append  เขียนข้อมูลต่อจากไฟล์เดิม          
                //create a fileinfo object of an excel file on the disk                
                FileInfo file = new FileInfo(xlsxfiles);
                ExcelPicture picture1;
                ExcelPicture picture2;
                Image bmp;

                string randomPictureName;
                //create a new Excel package from the file
                using (ExcelPackage excelPackage = new ExcelPackage(file))
                {

                    progressBar1.Visible = true;
                    progressBar1.Minimum = 0;
                    progressBar1.Maximum = dt.Rows.Count;
                    progressBar1.Value = 0;

                    //create an instance of the the first sheet in the loaded file
                    ExcelWorksheet worksheetA = excelPackage.Workbook.Worksheets[1];

                    int rowCnt, colCnt;
                    string idcard = "";
                    string fullname = "";
                    byte[] byte1 = null;
                    ImageConverter ic;

                    foreach (DataRow dr in dt.Rows)
                    {

                        rowCnt = worksheetA.Dimension.End.Row;
                        colCnt = worksheetA.Dimension.End.Column;

                        worksheetA.Row(rowCnt + 1).Height = 75;  // set row height

                        idcard = dr["id_number"].ToString();
                        fullname = dr["fullname"].ToString();
                        if (myCheckBox1.Checked == true)
                        {
                            idcard = classGlobal.REPLACE_IDCARD(idcard);
                            fullname = classGlobal.REPLACE_NAME(fullname);
                        }

                        //++ write values
                        worksheetA.Cells[rowCnt + 1, 1].Value = dr["card_number"];
                        worksheetA.Cells[rowCnt + 1, 2].Value = dr["typename"];
                        worksheetA.Cells[rowCnt + 1, 3].Value = idcard;
                        worksheetA.Cells[rowCnt + 1, 4].Value = fullname;
                        worksheetA.Cells[rowCnt + 1, 5].Value = dr["status_in"];
                        worksheetA.Cells[rowCnt + 1, 6].Value = dr["status_out"];
                        //--

                        worksheetA.Cells[rowCnt + 1, 7].Value = dr["place"];
                        worksheetA.Cells[rowCnt + 1, 8].Value = dr["register"];

                        //++ กรณีจะ insert ภาพ                       
                        if (dr["str_imagedocument"].ToString() == "")
                        {
                            bmp = Image.FromFile(@"icon\unknown.png");
                        }
                        else
                        {
                            if (classGlobal.userId == "")
                            {
                                Base64ToImage(dr["str_imagedocument"].ToString());
                            }
                            else
                            {
                                Base64ToImage(ClassData.DOWNLOAD_IMAGE(dr["str_imagedocument"].ToString(), 130, 60)); 
                            }
                            
                            //bmp = new Bitmap(@"Webcam\temp.png");
                            byte1 = System.IO.File.ReadAllBytes(@"Webcam\temp.png");
                            ic = new ImageConverter();
                            bmp = (Image)ic.ConvertFrom(byte1);
                        }
                        System.Threading.Thread.Sleep(10);

                        if (myCheckBox1.Checked == true)
                        {
                            bmp = classGlobal.PLACEWATERMARK_FROM_IMAGE((Image)bmp);
                        }                      

                        iRandomPicture += 1;
                        randomPictureName = DateTime.Now.ToString("ddMMyyyyHHmmssfff");
                        picture1 = worksheetA.Drawings.AddPicture(randomPictureName, bmp);
                        picture1.From.Row = rowCnt;
                        picture1.From.Column = 8;
                        if (bmp.Width < bmp.Height)
                        {
                            picture1.SetSize(110, 100);
                        }
                        else
                        {
                            picture1.SetSize(160, 100);
                        }
                        //picture1.SetSize(160, 100);  // picture width * hegiht
                        bmp.Dispose();
                        bmp = null;
                        System.IO.File.Delete(@"Webcam\temp.png");
                        picture1.Dispose();
                        picture1 = null;
                        //--

                        //++ กรณีจะ insert ภาพ
                        if (dr["str_imagewebcamera"].ToString() == "")
                        {
                            bmp = Image.FromFile(@"icon\unknown.png");
                        }
                        else
                        {
                            if (classGlobal.userId == "")
                            {
                                Base64ToImage(dr["str_imagewebcamera"].ToString());
                            }
                            else
                            {
                                Base64ToImage(ClassData.DOWNLOAD_IMAGE(dr["str_imagewebcamera"].ToString(), 130, 60)); 
                            }
                            
                            //bmp = new Bitmap(@"Webcam\temp.png");
                            byte1 = System.IO.File.ReadAllBytes(@"Webcam\temp.png");
                            ic = new ImageConverter();
                            bmp = (Image)ic.ConvertFrom(byte1);
                        }
                        System.Threading.Thread.Sleep(10);

                        if (myCheckBox1.Checked == true)
                        {
                            bmp = classGlobal.PLACEWATERMARK_FROM_IMAGE((Image)bmp);
                        }

                        iRandomPicture += 1;
                        randomPictureName = iRandomPicture.ToString();
                        picture2 = worksheetA.Drawings.AddPicture(randomPictureName, bmp);
                        picture2.From.Row = rowCnt;
                        picture2.From.Column = 9;
                        if (bmp.Width < bmp.Height)
                        {
                            picture2.SetSize(110, 100);
                        }
                        else
                        {
                            picture2.SetSize(160, 100);
                        }
                        //picture2.SetSize(160, 100);  // picture width * hegiht
                        bmp.Dispose();
                        bmp = null;
                        System.IO.File.Delete(@"Webcam\temp.png");
                        picture2.Dispose();
                        picture2 = null;
                        //--

                        progressBar1.Value += 1;
                    }

                    //'++ set cell width             
                    worksheetA.Column(1).Width = 20;
                    worksheetA.Column(2).Width = 20;
                    worksheetA.Column(3).Width = 25;
                    worksheetA.Column(4).Width = 25;
                    worksheetA.Column(5).Width = 35;
                    worksheetA.Column(6).Width = 35;
                    worksheetA.Column(7).Width = 35;
                    worksheetA.Column(8).Width = 35;
                    worksheetA.Column(9).Width = 30;
                    worksheetA.Column(10).Width = 30;

                    //save the changes
                    excelPackage.Save();
                    excelPackage.Dispose();

                    progressBar1.Value = dt.Rows.Count;
                    //MessageBox.Show("สำเร็จ " + dt.Rows.Count + " รายการ");
                    frmMessageBox f = new frmMessageBox();
                    f.strMessage = "สำเร็จ " + dt.Rows.Count + " รายการ";
                    f.strStatus = "Information";
                    f.ShowDialog();

                    progressBar1.Value = 0;
                    progressBar1.Visible = false;
                }

            }
        }
        private void CreateExcelPackage_Factory(DataTable dt, string xlsxfiles)
        {
            using (var package = new ExcelPackage())
            {
                var workbook = package.Workbook;
                //*** Sheet 1
                var worksheet = workbook.Worksheets.Add("Sheet1");
                worksheet.Row(1).Height = 75;   // set row height

                if (System.IO.File.Exists(xlsxfiles) == false)   //กรณีไม่มีไฟล์ xlsx ให้สร้าง header column 
                {
                    worksheet.Cells[1, 1].Value = "เลข VISITOR";
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[1, 2].Value = "ประเภท VISITOR";
                    worksheet.Cells[1, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[1, 3].Value = "เลขประจำตัว";
                    worksheet.Cells[1, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[1, 4].Value = "ชื่อ-สกุล";
                    worksheet.Cells[1, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[1, 5].Value = "เวลาเข้า";
                    worksheet.Cells[1, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[1, 6].Value = "เวลาออก";
                    worksheet.Cells[1, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[1, 7].Value = "จำนวนคน";
                    worksheet.Cells[1, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[1, 8].Value = "บริษัท";
                    worksheet.Cells[1, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[1, 9].Value = "ทะเบียนรถ";
                    worksheet.Cells[1, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[1, 10].Value = "ชนิดของรถ";
                    worksheet.Cells[1, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[1, 11].Value = "ผู้รับการติดต่อ";
                    worksheet.Cells[1, 11].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 11].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[1, 12].Value = "แผนกที่ติดต่อ";
                    worksheet.Cells[1, 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 12].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[1, 13].Value = "ติดต่อเรื่อง";
                    worksheet.Cells[1, 13].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 13].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[1, 14].Value = "สถานที่ติดต่อ";
                    worksheet.Cells[1, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 14].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[1, 15].Value = "ข้อมูลอื่นๆ";
                    worksheet.Cells[1, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 15].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[1, 16].Value = "ภาพบัตร";
                    worksheet.Cells[1, 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 16].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[1, 17].Value = "ภาพถ่าย1";
                    worksheet.Cells[1, 17].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 17].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[1, 18].Value = "ภาพถ่าย2";
                    worksheet.Cells[1, 18].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 18].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[1, 19].Value = "ภาพถ่าย3";
                    worksheet.Cells[1, 19].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 19].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    package.SaveAs(new FileInfo(xlsxfiles));   //save file

                }
                else
                {
                    //
                }

                //Append  เขียนข้อมูลต่อจากไฟล์เดิม          
                //create a fileinfo object of an excel file on the disk
                int i = 0;
                FileInfo file = new FileInfo(xlsxfiles);
                ExcelPicture picID;
                ExcelPicture picCam1;
                ExcelPicture picCam2;
                ExcelPicture picCam3;
                Image bmp;

                string randomPictureName;
                //create a new Excel package from the file
                using (ExcelPackage excelPackage = new ExcelPackage(file))
                {

                    progressBar1.Visible = true;
                    progressBar1.Minimum = 0;
                    progressBar1.Maximum = dt.Rows.Count;
                    progressBar1.Value = 0;

                    //create an instance of the the first sheet in the loaded file
                    ExcelWorksheet worksheetA = excelPackage.Workbook.Worksheets[1];

                    int rowCnt, colCnt;
                    string idcard = "";
                    string fullname = "";
                    byte[] byte1 = null;
                    ImageConverter ic;

                    int sumRow = 0;
                    int follow = 0;
                    int followCount = 0;
                    foreach (DataRow dr in dt.Rows)
                    {

                        rowCnt = worksheetA.Dimension.End.Row;
                        colCnt = worksheetA.Dimension.End.Column;

                        worksheetA.Row(rowCnt + 1).Height = 75;  // set row height

                        idcard = dr["id_number"].ToString();
                        fullname = dr["fullname"].ToString();

                        if (myCheckBox1.Checked == true)
                        {
                            idcard = classGlobal.REPLACE_IDCARD(idcard);
                            fullname = classGlobal.REPLACE_NAME(fullname);
                        }

                        //++ write values
                        worksheetA.Cells[rowCnt + 1, 1].Value = dr["card_number"];
                        worksheetA.Cells[rowCnt + 1, 2].Value = dr["typename"];
                        worksheetA.Cells[rowCnt + 1, 3].Value = idcard;
                        worksheetA.Cells[rowCnt + 1, 4].Value = fullname;
                        worksheetA.Cells[rowCnt + 1, 5].Value = dr["status_in"];
                        worksheetA.Cells[rowCnt + 1, 6].Value = dr["status_out"];
                        //--

                        try
                        {
                            follow = (Int32.Parse(dr["follower"].ToString()));
                        }
                        catch
                        {
                            follow = 0;
                        }
                        follow += 1; // ของกองทหารเรือ งานของหนุ่ม

                        worksheetA.Cells[rowCnt + 1, 7].Value = follow.ToString();
                        worksheetA.Cells[rowCnt + 1, 8].Value = dr["company"];
                        worksheetA.Cells[rowCnt + 1, 9].Value = dr["license_plate"];
                        worksheetA.Cells[rowCnt + 1, 10].Value = dr["vehicle_type"];
                        worksheetA.Cells[rowCnt + 1, 11].Value = dr["contact_to"];
                        worksheetA.Cells[rowCnt + 1, 12].Value = dr["department"];
                        worksheetA.Cells[rowCnt + 1, 13].Value = dr["topic"];
                        worksheetA.Cells[rowCnt + 1, 14].Value = dr["place"];
                        worksheetA.Cells[rowCnt + 1, 15].Value = dr["etc"];

                        //++ กรณีจะ insert ภาพ IDCARD                      
                        if (dr["str_imagedocument"].ToString() == "")
                        {
                            bmp = Image.FromFile(@"icon\unknown.png");
                        }
                        else
                        {
                            if (classGlobal.userId == "")
                            {
                                Base64ToImage(dr["str_imagedocument"].ToString());
                            }
                            else
                            {
                                Base64ToImage(ClassData.DOWNLOAD_IMAGE(dr["str_imagedocument"].ToString(), 130, 60));
                            }                            
                            //bmp = new Bitmap(@"Webcam\temp.png");
                            byte1 = System.IO.File.ReadAllBytes(@"Webcam\temp.png");
                            ic = new ImageConverter();
                            bmp = (Image)ic.ConvertFrom(byte1);
                        }
                        System.Threading.Thread.Sleep(10);

                        if (myCheckBox1.Checked == true)
                        {
                            bmp = classGlobal.PLACEWATERMARK_FROM_IMAGE((Image)bmp);
                        }                         

                        i += 1;
                        randomPictureName = DateTime.Now.ToString("ddMMyyyyHHmmssfff");
                        picID = worksheetA.Drawings.AddPicture(randomPictureName, bmp);
                        picID.From.Row = rowCnt;
                        picID.From.Column = 15;
                        if (bmp.Width < bmp.Height)
                        {
                            picID.SetSize(110, 100);
                        }
                        else
                        {
                            picID.SetSize(160, 100);
                        }
                        //picture1.SetSize(160, 100);  // picture width * hegiht
                        bmp.Dispose();
                        bmp = null;
                        System.IO.File.Delete(@"Webcam\temp.png");
                        picID.Dispose();
                        picID = null;
                        //--

                        //++ กรณีจะ insert ภาพ WEBCAM1
                        if (dr["str_imagewebcamera"].ToString() == "")
                        {
                            bmp = Image.FromFile(@"icon\unknown.png");
                        }
                        else
                        {
                            if (classGlobal.userId == "")
                            {
                                Base64ToImage(dr["str_imagewebcamera"].ToString());
                            }
                            else
                            {
                                Base64ToImage(ClassData.DOWNLOAD_IMAGE(dr["str_imagewebcamera"].ToString(), 130, 60));
                            }                            
                            //bmp = new Bitmap(@"Webcam\temp.png");
                            byte1 = System.IO.File.ReadAllBytes(@"Webcam\temp.png");
                            ic = new ImageConverter();
                            bmp = (Image)ic.ConvertFrom(byte1);
                        }
                        System.Threading.Thread.Sleep(10);

                        if (myCheckBox1.Checked == true)
                        {
                            bmp = classGlobal.PLACEWATERMARK_FROM_IMAGE((Image)bmp);
                        }                         

                        i += 1;
                        randomPictureName = i.ToString();
                        picCam1 = worksheetA.Drawings.AddPicture(randomPictureName, bmp);
                        picCam1.From.Row = rowCnt;
                        picCam1.From.Column = 16;
                        if (bmp.Width < bmp.Height)
                        {
                            picCam1.SetSize(110, 100);
                        }
                        else
                        {
                            picCam1.SetSize(160, 100);
                        }
                        bmp.Dispose();
                        bmp = null;
                        System.IO.File.Delete(@"Webcam\temp.png");
                        picCam1.Dispose();
                        picCam1 = null;
                        //--

                        //++ กรณีจะ insert ภาพ WEBCAM2
                        if (dr["str_imagewebcamera1"].ToString() == "")
                        {
                            bmp = Image.FromFile(@"icon\unknown.png");
                        }
                        else
                        {
                            if (classGlobal.userId == "")
                            {
                                Base64ToImage(dr["str_imagewebcamera1"].ToString());
                            }
                            else
                            {
                                Base64ToImage(ClassData.DOWNLOAD_IMAGE(dr["str_imagewebcamera1"].ToString(), 130, 60));
                            }                              
                            //bmp = new Bitmap(@"Webcam\temp.png");
                            byte1 = System.IO.File.ReadAllBytes(@"Webcam\temp.png");
                            ic = new ImageConverter();
                            bmp = (Image)ic.ConvertFrom(byte1);
                        }
                        System.Threading.Thread.Sleep(10);

                        if (myCheckBox1.Checked == true)
                        {
                            bmp = classGlobal.PLACEWATERMARK_FROM_IMAGE((Image)bmp);
                        }

                        i += 1;
                        randomPictureName = i.ToString();
                        picCam2 = worksheetA.Drawings.AddPicture(randomPictureName, bmp);
                        picCam2.From.Row = rowCnt;
                        picCam2.From.Column = 17;
                        if (bmp.Width < bmp.Height)
                        {
                            picCam2.SetSize(110, 100);
                        }
                        else
                        {
                            picCam2.SetSize(160, 100);
                        }
                        bmp.Dispose();
                        bmp = null;
                        System.IO.File.Delete(@"Webcam\temp.png");
                        picCam2.Dispose();
                        picCam2 = null;
                        //--

                        //++ กรณีจะ insert ภาพ WEBCAM3
                        if (dr["str_imagewebcamera2"].ToString() == "")
                        {
                            bmp = Image.FromFile(@"icon\unknown.png");
                        }
                        else
                        {
                            if (classGlobal.userId == "")
                            {
                                Base64ToImage(dr["str_imagewebcamera2"].ToString());
                            }
                            else
                            {
                                Base64ToImage(ClassData.DOWNLOAD_IMAGE(dr["str_imagewebcamera2"].ToString(), 130, 60));
                            }  
                            //bmp = new Bitmap(@"Webcam\temp.png");
                            byte1 = System.IO.File.ReadAllBytes(@"Webcam\temp.png");
                            ic = new ImageConverter();
                            bmp = (Image)ic.ConvertFrom(byte1);
                        }
                        System.Threading.Thread.Sleep(10);

                        if (myCheckBox1.Checked == true)
                        {
                            bmp = classGlobal.PLACEWATERMARK_FROM_IMAGE((Image)bmp);
                        }

                        i += 1;
                        randomPictureName = i.ToString();
                        picCam3 = worksheetA.Drawings.AddPicture(randomPictureName, bmp);
                        picCam3.From.Row = rowCnt;
                        picCam3.From.Column = 18;
                        if (bmp.Width < bmp.Height)
                        {
                            picCam3.SetSize(110, 100);
                        }
                        else
                        {
                            picCam3.SetSize(160, 100);
                        }
                        bmp.Dispose();
                        bmp = null;
                        System.IO.File.Delete(@"Webcam\temp.png");
                        picCam3.Dispose();
                        picCam3 = null;
                        //--

                        followCount += follow;
                        sumRow = rowCnt;

                        progressBar1.Value += 1;
                    }

                    //'++ set cell width             
                    worksheetA.Column(1).Width = 20;
                    worksheetA.Column(2).Width = 20;
                    worksheetA.Column(3).Width = 25;
                    worksheetA.Column(4).Width = 25;
                    worksheetA.Column(5).Width = 35;
                    worksheetA.Column(6).Width = 35;
                    worksheetA.Column(7).Width = 35;
                    worksheetA.Column(8).Width = 35;
                    worksheetA.Column(9).Width = 30;
                    worksheetA.Column(10).Width = 30;

                    worksheetA.Column(11).Width = 35;
                    worksheetA.Column(12).Width = 35;
                    worksheetA.Column(13).Width = 35;
                    worksheetA.Column(14).Width = 35;
                    worksheetA.Column(15).Width = 35;

                    //++ ภาพ
                    worksheetA.Column(16).Width = 16;
                    worksheetA.Column(17).Width = 23;
                    worksheetA.Column(18).Width = 23;
                    worksheetA.Column(19).Width = 23;

                    worksheetA.Cells[sumRow + 2, 7].Value = followCount.ToString();  // ของกองทหารเรือ งานของหนุ่ม

                    //save the changes
                    excelPackage.Save();
                    excelPackage.Dispose();

                    progressBar1.Value = dt.Rows.Count;
                    //MessageBox.Show("สำเร็จ " + dt.Rows.Count + " รายการ");
                    frmMessageBox f = new frmMessageBox();
                    f.strMessage = "สำเร็จ " + dt.Rows.Count + " รายการ";
                    f.strStatus = "Information";
                    f.ShowDialog();

                    progressBar1.Value = 0;
                    progressBar1.Visible = false;
                }

            }
        }
        private static void Base64ToImage(string base64String)
        {
            try
            {
                // Convert base 64 string to byte[]
                byte[] imageBytes = Convert.FromBase64String(base64String);
                // Convert byte[] to Image
                using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                {
                    Image image = Image.FromStream(ms, true);
                    image.Save(@"Webcam\temp.png");
                }
            }
            catch
            {
                //return null;
            }

        }
        //--- Export 

        private void CreateExcelPackageForAudit(DataTable dt, string output, string start, string stop)
        {

            DateTime dtStart = DateTime.ParseExact(start, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime dtStop = DateTime.ParseExact(stop, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            TimeSpan diffResult = dtStop.Subtract(dtStart);

            dtStart = dtStart.AddDays(-1);
            for (int d = 1; d <= diffResult.TotalDays + 2; d++)  // main loop วันที่
            {
                
                dtStart = dtStart.AddDays(1);
                //++ check if equal date 
                string loopDateString = dtStart.Day.ToString().PadLeft(2,'0') + "/" + dtStart.Month.ToString().PadLeft(2, '0') + "/" + (int.Parse(dtStart.Year.ToString()) + 543).ToString();
                DataRow[] foundDateFocus= dt.Select("status_in LIKE '" + loopDateString + "%'");
                if (foundDateFocus.Count() == 0)
                    goto JUMP_LOOP;
                //-- check if equal date 

                string fileName = dtStart.Day.ToString().PadLeft(2, '0') + "_" + dtStart.Month.ToString().PadLeft(2, '0') + "_" + dtStart.Year.ToString();
                fileName = output + @"\" + fileName + ".xlsx";
                string captionText = "สรุปยอดรถ เข้า-ออกประเภทต่างๆ.. วันที่" + " " + dtStart.Day.ToString() + " " + classGlobal.monthTextLong[dtStart.Month] + " " + (dtStart.Year + 543).ToString();
                using (var package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var worksheet = workbook.Worksheets.Add("Sheet1");

                    worksheet.Cells[1, 1].Value = captionText;
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    worksheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    for (int r = 1; r <= 1; r++)
                    {
                        for (int c = 2; c <= 6; c++)
                        {
                            worksheet.Cells[r, c].Value = " ";
                            worksheet.Cells[r, c].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[r, c].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        }
                    }
                    worksheet.Cells["A1:F1"].Merge = true;  // Merge first row  สรุปยอดรถ เข้า-ออกประเภทต่างๆ..
                    var drVisitorTypeGroup = (from Rows in dt.AsEnumerable() select Rows["typename"]).Distinct().ToList();

                    int currentRow = 0;
                    DataTable dtClone = foundDateFocus.CopyToDataTable();
                    drVisitorTypeGroup = (from Rows in dtClone.AsEnumerable() select Rows["typename"]).Distinct().ToList();
                    currentRow = currentRow;

                    foreach (var typeName in drVisitorTypeGroup)
                    {
                        currentRow = worksheet.Dimension.End.Row + 1;
                        worksheet.Cells[currentRow, 1].Value = "ประเภท VISITOR - " + typeName;
                        worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        worksheet.Cells[currentRow, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[currentRow, 1].Style.Font.Bold = true;
                        worksheet.Cells[currentRow, 1].Style.Font.UnderLine = true;

                        for (int r = currentRow; r <= currentRow; r++)
                        {
                            for (int c = 2; c <= 6; c++)
                            {
                                worksheet.Cells[r, c].Value = " ";
                                worksheet.Cells[r, c].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[r, c].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            }
                        }
                        worksheet.Cells["A" + currentRow + ":" + "F" + currentRow].Merge = true;  // Merge row แต่ละประเภท

                        //--------------------------------------------------------------------//

                        currentRow = worksheet.Dimension.End.Row + 1;//// บรรทัดใหม่ (บริษัท เลขปรพจำตัว....)
                        worksheet.Cells[currentRow, 1].Value = "บริษัท";
                        worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[currentRow, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Column(1).Width = 40;

                        worksheet.Cells[currentRow, 2].Value = "เลขประจำตัว";
                        worksheet.Cells[currentRow, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[currentRow, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Column(2).Width = 20;

                        worksheet.Cells[currentRow, 3].Value = "ชื่อ-สกุล";
                        worksheet.Cells[currentRow, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[currentRow, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Column(3).Width = 20;

                        worksheet.Cells[currentRow, 4].Value = "เวลาเข้า";
                        worksheet.Cells[currentRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[currentRow, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Column(4).Width = 20;

                        worksheet.Cells[currentRow, 5].Value = "เวลาออก";
                        worksheet.Cells[currentRow, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[currentRow, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Column(5).Width = 20;

                        worksheet.Cells[currentRow, 6].Value = "จำนวนคน";
                        worksheet.Cells[currentRow, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[currentRow, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Column(6).Width = 15;

                        //--------------------------------------------------------------------//   
                        //DataRow[] dr = dt.Select("typename='" + typeName + "'");
                        DataRow[] dr = dtClone.Select("typename='" + typeName + "'" + " AND " + "status_in LIKE '" + loopDateString + "%'");
                        if (dr.Count() > 0)
                        {
                            string company = "";
                            string citizenId = "";
                            string fullName = "";
                            string In = "";
                            string Out = "";
                            string personCount = "";
                            int numPerson = 0;
                            for (int r = 0; r < dr.Count(); r++)
                            {
                                company = dr[r][21].ToString();
                                citizenId = dr[r][16].ToString();
                                fullName = dr[r][17].ToString();
                                In = dr[r][6].ToString();
                                Out = dr[r][7].ToString();
                                personCount = dr[r][20].ToString();
                                if (personCount == "")
                                    personCount = "1";
                                else
                                    personCount = (int.Parse(personCount) + 1).ToString();

                                numPerson = numPerson + int.Parse(personCount);


                                currentRow = worksheet.Dimension.End.Row + 1;
                                worksheet.Cells[currentRow, 1].Value = company;
                                worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells[currentRow, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                worksheet.Column(1).Width = 40;

                                worksheet.Cells[currentRow, 2].Value = citizenId;
                                worksheet.Cells[currentRow, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells[currentRow, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                worksheet.Column(2).Width = 20;

                                worksheet.Cells[currentRow, 3].Value = fullName;
                                worksheet.Cells[currentRow, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells[currentRow, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                worksheet.Column(3).Width = 20;

                                worksheet.Cells[currentRow, 4].Value = In;
                                worksheet.Cells[currentRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells[currentRow, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                worksheet.Column(4).Width = 20;

                                worksheet.Cells[currentRow, 5].Value = Out;
                                worksheet.Cells[currentRow, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                worksheet.Cells[currentRow, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                worksheet.Column(5).Width = 20;

                                worksheet.Cells[currentRow, 6].Value = personCount;
                                worksheet.Cells[currentRow, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                worksheet.Cells[currentRow, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                                worksheet.Column(6).Width = 15;

                            }
                            currentRow = worksheet.Dimension.End.Row + 1;
                            worksheet.Cells[currentRow, 1].Value = dr.Count().ToString();
                            worksheet.Cells[currentRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[currentRow, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Column(1).Width = 40;

                            worksheet.Cells[currentRow, 2].Value = " ";
                            worksheet.Cells[currentRow, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[currentRow, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Column(2).Width = 20;

                            worksheet.Cells[currentRow, 3].Value = " ";
                            worksheet.Cells[currentRow, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[currentRow, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Column(3).Width = 20;

                            worksheet.Cells[currentRow, 4].Value = " ";
                            worksheet.Cells[currentRow, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[currentRow, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Column(4).Width = 20;

                            worksheet.Cells[currentRow, 5].Value = " ";
                            worksheet.Cells[currentRow, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[currentRow, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Column(5).Width = 20;

                            worksheet.Cells[currentRow, 6].Value = numPerson.ToString();
                            worksheet.Cells[currentRow, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[currentRow, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            worksheet.Column(6).Width = 15;
                        }
                    }

                    package.SaveAs(new FileInfo(fileName));
                }

            JUMP_LOOP:
                Console.Write("JUMP_LOOP");
                System.Threading.Thread.Sleep(10);
            }

        }
    }
}

