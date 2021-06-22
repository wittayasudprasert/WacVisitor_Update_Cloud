using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Windows.Forms;

namespace WacVisitor
{
    public partial class frmView : Form
    {
        #region Reduce Memmory
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetProcessWorkingSetSize(IntPtr process,
            UIntPtr minimumWorkingSetSize, UIntPtr maximumWorkingSetSize);
        #endregion
        #region PlaceHold TextBox
        private const int EM_SETCUEBANNER = 0x1501;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)]string lParam);
        #endregion

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public string UrlWebService_Methode = "Time_IN_OUT";

        bool boolTerminateJob = false;

        int pageCurrent = 1;
        int pageSize = 12;
        double pageTotal = 0;
        int pageAll = 0;

        public frmView()
        {
            InitializeComponent();

            //SendMessage(textBox1.Handle, EM_SETCUEBANNER, 0, "เลข VISITOR");
            if (classGlobal.FactoryVersion == true)
                SendMessage(textBox1.Handle, EM_SETCUEBANNER, 0, "เลข VISITOR/ทะเบียนรถ");
            else
                SendMessage(textBox1.Handle, EM_SETCUEBANNER, 0, "เลข VISITOR/ทะเบียนรถ/สถานที่");
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //ReleaseCapture();
                //SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void GetVisitorTypeList()
        {
            comboBox1.Items.Clear();

            DataTable dt = new DataTable("dt");
            if (classGlobal.databaseType == "acc")
            {
                OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT typeid, typename FROM tbl_type ORDER BY typeid ASC", classGlobal.conn);
                adapter.Fill(dt);
                adapter.Dispose();
                adapter = null;
            }
            else if (classGlobal.databaseType == "psql")
            {
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter("SELECT typeid, typename FROM tbl_type ORDER BY typeid ASC", classGlobal.connP);
                adapter.Fill(dt);
                adapter.Dispose();
                adapter = null;
            }
            else
            {
                dt.Columns.Add("typeid");
                dt.Columns.Add("typename");

                string jsonString = ClassData.GET_METHODE("visitorType");
                if (jsonString == "")
                {
                    //-- no data
                }
                else
                {
                    JArray jsArray = JArray.Parse(jsonString);
                    int j = 0;
                    foreach (var x in jsArray)
                    {
                        dt.Rows.Add(j, x.ToString());
                        j += 1;
                    }
                }
            }

            Dictionary<string, string> list = new Dictionary<string, string>();
            comboBox1.Items.Add("ประเภทที่ค้นหา");
            foreach (DataRow reader in dt.Rows)
            {
                comboBox1.Items.Add(reader.ItemArray[1].ToString());
            }

            dt.Dispose();
            dt = null;

            comboBox1.SelectedIndex = 0;
        }
        private void frmView_Load(object sender, EventArgs e)
        {
            cmbPage.Visible = false;
            label1.Visible = false;

            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            button10.Left = (this.ClientSize.Width - button10.Size.Width - 10);
            panelPage.Left = (panel5.Width - panelPage.Size.Width) / 2;
            panel_search.Left = (panel5.Width - panel_search.Size.Width) / 2;
            pictureBox1.Image = Image.FromFile(@"icon\search.png");

            if (classGlobal.userId != "")
            {
                dataGridView1.Visible = false;
                panelPage.Visible = false;
                panel_search.Visible = false;

                frmTopMost f = new frmTopMost();
                f.Show();
                GetVisitorTypeList();
                ListLogs();
                f.Close();

                dataGridView1.Visible = true;
                panelPage.Visible = true;
                panel_search.Visible = true;
            }
            else
            {
                GetVisitorTypeList();
                ListLogs();
            }


            cmbPage.Visible = true;
            label1.Visible = true;
            cmbPage.Items.Clear();
            for (int n = 1; n <= pageAll; n++)
                cmbPage.Items.Add(n.ToString());
            if (cmbPage.Items.Count > 0)
                cmbPage.SelectedIndex = 0;

            label1.Text = "/" + pageAll.ToString();

        }

        private string QueryString()
        {

            string query = "";
            string visitorType = comboBox1.Text.ToString().Replace("ประเภทที่ค้นหา", "");

            if (classGlobal.FactoryVersion == true)
            {   // factory version
                query = String.Format("SELECT  t1.id, t1.card_number, t1.status_in, t1.status_out, t2.typename, t1.typeid, " +
                                        "(t4.th_title + '' + t4.th_firstname + ' ' + t4.th_lastname) AS vname, " +
                                        "t3.company, t3.license_plate, t5.fullname, t1.mac_checkin, t1.mac_checkout " +
                                        "FROM " +
                                        "((((tbl_visitor t1 LEFT JOIN tbl_type t2 ON t1.typeid = t2.typeid) " +
                                        "LEFT JOIN tbl_moreinfo_factory t3 ON t1.id = t3.vid) " +
                                        "LEFT JOIN tbl_idcard t4 ON t1.id = t4.id) " +
                                        "LEFT JOIN tbl_personal t5 ON t1.id = t5.id) " +
                                        "WHERE (t1.card_number LIKE '%{0}%' " +
                                        "OR t3.license_plate LIKE '%{1}%') AND t2.typename LIKE '%{2}%' " +
                                        "AND (t1.status_in BETWEEN @startDate AND @endDate) " +
                                        "GROUP BY t1.id, t1.card_number, t1.status_in, t1.status_out, t2.typename, t1.typeid, " +
                                        "t4.th_title, t4.th_firstname, t4.th_lastname, t3.company, t3.license_plate, t5.fullname, t1.mac_checkin, t1.mac_checkout " +
                                        "ORDER BY t1.status_in DESC", textBox1.Text, textBox1.Text, visitorType);


                if (textBox1.Text == "" && visitorType != "")
                {
                    query = String.Format("SELECT  t1.id, t1.card_number, t1.status_in, t1.status_out, t2.typename, t1.typeid, " +
                                        "(t4.th_title + '' + t4.th_firstname + ' ' + t4.th_lastname) AS vname, " +
                                        "t3.company, t3.license_plate, t5.fullname, t1.mac_checkin, t1.mac_checkout " +
                                        "FROM " +
                                        "((((tbl_visitor t1 LEFT JOIN tbl_type t2 ON t1.typeid = t2.typeid) " +
                                        "LEFT JOIN tbl_moreinfo_factory t3 ON t1.id = t3.vid) " +
                                        "LEFT JOIN tbl_idcard t4 ON t1.id = t4.id) " +
                                        "LEFT JOIN tbl_personal t5 ON t1.id = t5.id) " +
                                        "WHERE t2.typename = '{0}' " +
                                        "AND (t1.status_in BETWEEN @startDate AND @endDate) " +
                                        "GROUP BY t1.id, t1.card_number, t1.status_in, t1.status_out, t2.typename, t1.typeid, " +
                                        "t4.th_title, t4.th_firstname, t4.th_lastname, t3.company, t3.license_plate, t5.fullname, t1.mac_checkin, t1.mac_checkout " +
                                        "ORDER BY t1.status_in DESC", visitorType);
                }
                else if (textBox1.Text != "" && visitorType == "")
                {
                    query = String.Format("SELECT  t1.id, t1.card_number, t1.status_in, t1.status_out, t2.typename, t1.typeid, " +
                                        "(t4.th_title + '' + t4.th_firstname + ' ' + t4.th_lastname) AS vname, " +
                                        "t3.company, t3.license_plate, t5.fullname, t1.mac_checkin, t1.mac_checkout " +
                                        "FROM " +
                                        "((((tbl_visitor t1 LEFT JOIN tbl_type t2 ON t1.typeid = t2.typeid) " +
                                        "LEFT JOIN tbl_moreinfo_factory t3 ON t1.id = t3.vid) " +
                                        "LEFT JOIN tbl_idcard t4 ON t1.id = t4.id) " +
                                        "LEFT JOIN tbl_personal t5 ON t1.id = t5.id) " +
                                        "WHERE (t1.card_number LIKE '%{0}%' " +
                                        "OR t3.license_plate LIKE '%{1}%') " +
                                        "AND (t1.status_in BETWEEN @startDate AND @endDate) " +
                                        "GROUP BY t1.id, t1.card_number, t1.status_in, t1.status_out, t2.typename, t1.typeid, " +
                                        "t4.th_title, t4.th_firstname, t4.th_lastname, t3.company, t3.license_plate, t5.fullname, t1.mac_checkin, t1.mac_checkout " +
                                        "ORDER BY t1.status_in DESC", textBox1.Text, textBox1.Text);
                }
            }
            else
            {   // home version                
                query = String.Format("SELECT  t1.id, t1.card_number, t1.status_in, t1.status_out, t2.typename, t1.typeid, " +
                                                        "(t4.th_title + '' + t4.th_firstname + ' ' + t4.th_lastname) AS vname, " +
                                                        "t3.register, t3.place " +
                                                        "FROM " +
                                                        "(((tbl_visitor t1 LEFT JOIN tbl_type t2 ON t1.typeid = t2.typeid) " +
                                                        "LEFT JOIN tbl_moreinfo t3 ON t1.id = t3.id) " +
                                                        "LEFT JOIN tbl_idcard t4 ON t1.id = t4.id) " +
                                                        "WHERE (t1.card_number LIKE '%{0}%' " +
                                                        "OR t3.register LIKE '%{1}%' OR t3.place LIKE '%{2}%') AND (t2.typename LIKE '%{3}%') " +
                                                        "AND (t1.status_in BETWEEN @startDate AND @endDate) " +
                                                        "ORDER BY t1.status_in DESC", textBox1.Text, textBox1.Text, textBox1.Text, visitorType);   


                if (textBox1.Text == "" && visitorType != "")
                {
                    query = String.Format("SELECT  t1.id, t1.card_number, t1.status_in, t1.status_out, t2.typename, t1.typeid, " +
                                        "(t4.th_title + '' + t4.th_firstname + ' ' + t4.th_lastname) AS vname, " +
                                        "t3.register, t3.place " +
                                        "FROM " +
                                        "(((tbl_visitor t1 LEFT JOIN tbl_type t2 ON t1.typeid = t2.typeid) " +
                                        "LEFT JOIN tbl_moreinfo t3 ON t1.id = t3.id) " +
                                        "LEFT JOIN tbl_idcard t4 ON t1.id = t4.id) " +
                                        "WHERE t2.typename = '{0}' " +
                                        "AND (t1.status_in BETWEEN @startDate AND @endDate) " +
                                        "ORDER BY t1.status_in DESC", visitorType);
                }
                else if (textBox1.Text != "" && visitorType == "")
                {
                    query = String.Format("SELECT  t1.id, t1.card_number, t1.status_in, t1.status_out, t2.typename, t1.typeid, " +
                                        "(t4.th_title + '' + t4.th_firstname + ' ' + t4.th_lastname) AS vname, " +
                                        "t3.register, t3.place " +
                                        "FROM " +
                                        "(((tbl_visitor t1 LEFT JOIN tbl_type t2 ON t1.typeid = t2.typeid) " +
                                        "LEFT JOIN tbl_moreinfo t3 ON t1.id = t3.id) " +
                                        "LEFT JOIN tbl_idcard t4 ON t1.id = t4.id) " +
                                        "WHERE (t1.card_number LIKE '%{0}%' " +
                                        "OR t3.register LIKE '%{1}%' OR t3.place LIKE '%{2}%') " +
                                        "AND (t1.status_in BETWEEN @startDate AND @endDate) " +
                                        "ORDER BY t1.status_in DESC", textBox1.Text, textBox1.Text, textBox1.Text);
                }
            }

            if (classGlobal.databaseType == "acc")
            {
                //-- do nothing  --//
            }
            else if (classGlobal.databaseType == "psql")
            {
                query = query.Replace("(t4.th_title + '' + t4.th_firstname + ' ' + t4.th_lastname)", 
                                        "CONCAT(t4.th_title , '' , t4.th_firstname , ' ' , t4.th_lastname)");
            }
            else
            {
                //-- do nothing  --//
            }

            return query;
        }

        private void ListLogs()
        {
            string query = "";
            query = QueryString();

            DataTable dtCount = new DataTable("dtCount");
            if (classGlobal.databaseType == "acc")
            {
                OleDbCommand cmd = new OleDbCommand(query, classGlobal.conn);
                cmd.Parameters.AddWithValue("@startDate", DbType.DateTime).Value =
                                                            dateTimePicker1.Value.ToString("yyyy-MM-dd") + " " + "00:00:00";
                cmd.Parameters.AddWithValue("@endDate", DbType.DateTime).Value =
                                                            dateTimePicker2.Value.ToString("yyyy-MM-dd") + " " + "23:59:59";

                OleDbDataAdapter ad = new OleDbDataAdapter(cmd);
                ad.Fill(dtCount);
                ad.Dispose();
                ad = null;
            }
            else if (classGlobal.databaseType == "psql")
            {
                query = query.Replace("@startDate", "'" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + " " + "00:00:00" + "'");
                query = query.Replace("@endDate", "'" + dateTimePicker2.Value.ToString("yyyy-MM-dd") + " " + "00:00:00" + "'");
                NpgsqlCommand cmd = new NpgsqlCommand(query, classGlobal.connP);
                NpgsqlDataAdapter ad = new NpgsqlDataAdapter(cmd);
                ad.Fill(dtCount);
                ad.Dispose();
                ad = null;
            }
            else
            {
                classGlobal.arrayListVisitorId.Clear();

                ////++ LOADING....
                //Cursor = Cursors.WaitCursor;                
                //DataTable dtLogsComplete = ClassData.GET_LOGS_COMPLETE(true);  // log เข้าออกสำเร้จ
                //DataTable dtLogsStill = ClassData.GET_LOGS_STILL(true);        // log เข้าแต่ยังไม่ออก

                //classGlobal.dtAllLogsView = dtLogsComplete.Copy();
                //classGlobal.dtAllLogsView.Merge(dtLogsStill);

                //DataView dv = classGlobal.dtAllLogsView.DefaultView;
                //if (classGlobal.userId != "")
                //    dv.Sort = "recordTimeIn " + classGlobal.sortOrderBy;
                //else
                //    dv.Sort = "status_in " + classGlobal.sortOrderBy;
                //classGlobal.dtAllLogsView = dv.ToTable();
                //Cursor = Cursors.Default;
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
                        break;
                 
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
                classGlobal.dtAllLogsView = dv.ToTable();
                Cursor = Cursors.Default;
                //-- LOADING....

                string searchAnother = textBox1.Text;
                string serachVisitorType = comboBox1.Text.Replace("ประเภทที่ค้นหา","");

                DataRow[] foundRows;
                if (searchAnother.Replace(" ", "") != "")
                {
                    if (serachVisitorType != "")  // หาทั้ง 2 option
                    {
                        foundRows = classGlobal.dtAllLogsView.Select("(visitorNumber LIKE '%" + searchAnother + "%'" + " OR " +
                                                                                "licensePlate LIKE '%" + searchAnother + "%'" + " OR " +
                                                                                    "contactPlace LIKE '%" + searchAnother + "%')" + " AND " +
                                                                                        "(visitorType = '" + serachVisitorType + "')");
                        classGlobal.dtAllLogsView = foundRows.CopyToDataTable();
                    }
                    else  // หาเฉพาะ searchAnother
                    {
                        foundRows = classGlobal.dtAllLogsView.Select("visitorNumber LIKE '%" + searchAnother + "%'" + " OR " +
                                                                                "licensePlate LIKE '%" + searchAnother + "%'" + " OR " +
                                                                                    "contactPlace LIKE '%" + searchAnother + "%'");
                        classGlobal.dtAllLogsView = foundRows.CopyToDataTable();
                    }
                }
                else
                {
                    if (serachVisitorType != "") // หาเฉพาะ serachVisitorType
                    {
                       foundRows = classGlobal.dtAllLogsView.Select("visitorType = '" + serachVisitorType + "'");
                       if (foundRows.Length > 0)
                           classGlobal.dtAllLogsView = foundRows.CopyToDataTable();
                       else
                           classGlobal.dtAllLogsView = new DataTable("dt");
                    }
                }

                if (classGlobal.FactoryVersion == true)  
                {
                    dtCount.Columns.Add("id");
                    dtCount.Columns.Add("card_number");
                    dtCount.Columns.Add("status_in");
                    dtCount.Columns.Add("status_out");
                    dtCount.Columns.Add("typename");
                    dtCount.Columns.Add("typeid");
                    dtCount.Columns.Add("vname");
                    dtCount.Columns.Add("company");
                    dtCount.Columns.Add("license_plate");
                    dtCount.Columns.Add("fullname");
                    dtCount.Columns.Add("mac_checkin");
                    dtCount.Columns.Add("mac_checkout");

                    dtCount.Columns.Add("image1");
                    dtCount.Columns.Add("image2");
                    dtCount.Columns.Add("image3");
                    dtCount.Columns.Add("image4");

                    dtCount.Columns.Add("citizenId");
                    dtCount.Columns.Add("etc");
                    dtCount.Columns.Add("vehicleType");
                    dtCount.Columns.Add("visitorId");

                    dtCount.Columns.Add("visitorNumber");
                    dtCount.Columns.Add("visitorType");
                    dtCount.Columns.Add("name");
                    dtCount.Columns.Add("recordTimeIn");
                    dtCount.Columns.Add("recordTimeOut");

                    dtCount.Columns.Add("follower");
                    dtCount.Columns.Add("visitorFrom");
                    dtCount.Columns.Add("licensePlate");
                    dtCount.Columns.Add("visitPerson");
                    dtCount.Columns.Add("department");
                    dtCount.Columns.Add("contactTopic");
                    dtCount.Columns.Add("contactPlace");

                    int r = 1;
                    foreach (DataRow dr in classGlobal.dtAllLogsView.Rows)
                    {
                        dtCount.Rows.Add(r, dr["visitorNumber"], dr["recordTimeIn"], dr["recordTimeOut"], dr["visitorType"], 0,
                                                dr["name"], dr["visitorFrom"], dr["licensePlate"], dr["name"], "", "",
                                                    dr["image1"], dr["image2"], dr["image3"], dr["image4"], dr["citizenId"], dr["etc"], dr["vehicleType"], dr["visitorId"],
                                                    dr["visitorNumber"], dr["visitorType"], dr["name"], dr["recordTimeIn"], dr["recordTimeOut"],
                                                    dr["follower"], dr["visitorFrom"], dr["licensePlate"], dr["visitPerson"], dr["department"], dr["contactTopic"], dr["etc"]);
                        r += 1;
                    }
                }
                else 
                {
                    dtCount.Columns.Add("id");
                    dtCount.Columns.Add("card_number");
                    dtCount.Columns.Add("status_in");
                    dtCount.Columns.Add("status_out");
                    dtCount.Columns.Add("typename");
                    dtCount.Columns.Add("typeid");
                    dtCount.Columns.Add("vname");
                    dtCount.Columns.Add("register"); // "license_plate"
                    dtCount.Columns.Add("place");

                    dtCount.Columns.Add("image1");
                    dtCount.Columns.Add("image2");
                    dtCount.Columns.Add("image3");
                    dtCount.Columns.Add("image4");

                    dtCount.Columns.Add("citizenId");
                    dtCount.Columns.Add("etc");
                    dtCount.Columns.Add("vehicleType");
                    dtCount.Columns.Add("visitorId");

                    int r = 1;
                    foreach (DataRow dr in classGlobal.dtAllLogsView.Rows)
                    {
                        dtCount.Rows.Add(r, dr["visitorNumber"], dr["recordTimeIn"], dr["recordTimeOut"], dr["visitorType"], 0,
                                                dr["name"], dr["licensePlate"], dr["contactPlace"],
                                                    dr["image1"], dr["image2"], dr["image3"], dr["image4"], dr["citizenId"], dr["etc"], dr["vehicleType"], dr["visitorId"]); 
                        r += 1;
                    }

                }

                classGlobal.dtAllLogsView = dtCount.Copy();
            }

            int cntRow = dtCount.Rows.Count;
            if (cntRow % pageSize == 0)
            {
                pageAll = cntRow / pageSize;
            }
            else
            {
                pageTotal = cntRow / pageSize;
                pageAll = (int)Math.Floor(pageTotal) + 1;
            }

            dtCount.Dispose();
            dtCount = null;

            button1.Enabled = false;

            if (pageAll <= 1)
            {
                button1.Enabled = false;
                button2.Enabled = false;
            }
            else if (pageAll > 1)
            {
                button1.Enabled = false;
                button2.Enabled = true;
            }

            DataTable dt1 = Populate(pageCurrent);
            DisplayDataGridView(dt1);
            classGlobal.dtLogsPaging = dt1.Copy();


            cmbPage.Items.Clear();
            for (int n = 1; n <= pageAll; n++)
                cmbPage.Items.Add(n.ToString());
            if (cmbPage.Items.Count > 0)
                cmbPage.SelectedIndex = 0;

            label1.Text = "/" + pageAll.ToString();
        }

        private void UpdateFont()
        {
            //Change cell font
            foreach (DataGridViewColumn c in dataGridView1.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Pixel);
            }
        }

        bool showIndicator = false;
        private void DisplayDataGridView(DataTable dt)
        {

            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();

            //+++ Header Column
            dataGridView1.RowTemplate.Resizable = DataGridViewTriState.True;
            dataGridView1.RowTemplate.Height = 80;
            dataGridView1.RowHeadersVisible = showIndicator;  // false = ซ่อน indicator

            if (classGlobal.FactoryVersion == true)
            {   // factory version
                dataGridView1.Columns.Add("col_type", "ประเภท");
                dataGridView1.Columns.Add("col_number", "เลข VISITOR");
                dataGridView1.Columns.Add("col_visitorname", "ผู้มาติดต่อ");
                dataGridView1.Columns.Add("col_company", "บริษัท");
                dataGridView1.Columns.Add("col_licenseplate", "ทะเบียนรถ");
                dataGridView1.Columns.Add("col_in", "เข้า");
                dataGridView1.Columns.Add("col_out", "ออก");
                dataGridView1.Columns.Add("col_typeid", "Type_ID");

                dataGridView1.Columns[0].Width = (this.ClientSize.Width / 7) - 10;
                dataGridView1.Columns[1].Width = (this.ClientSize.Width / 7) - 0;
                dataGridView1.Columns[2].Width = (this.ClientSize.Width / 7) - 10;
                dataGridView1.Columns[3].Width = (this.ClientSize.Width / 7) - 10;
                dataGridView1.Columns[4].Width = (this.ClientSize.Width / 7) - 10;
                dataGridView1.Columns[5].Width = (this.ClientSize.Width / 7) - 10;
                dataGridView1.Columns[6].Width = (this.ClientSize.Width / 7) - 10;
                dataGridView1.Columns[7].Width = 0;  //typeid
                dataGridView1.Columns[7].Visible = false;

                dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.SystemColors.Highlight;
                dataGridView1.EnableHeadersVisualStyles = false;

                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    col.HeaderCell.Style.Font = new Font("Segoe UI", 22F, FontStyle.Bold, GraphicsUnit.Pixel);

                    col.HeaderCell.Style.ForeColor = Color.White;
                }
                //--

                if (dt == null)
                    dt = new DataTable("dt");

                if (dt.Rows.Count == 0)
                {
                    lbPage.Text = "0/0";
                    button1.Enabled = false;
                    button2.Enabled = false;
                    return;
                }


                string vName = "";
                string vLicense = "";
                string vCompany = "";

                string vType = "";
                string card_num = "";
                string status_in = "";
                string status_out = "";
                string strIN = "";
                string strOUT = "";
                int beYearFormat = 0;
                string[] arr1;
                string[] arr2;

                DataGridViewRow row;
                foreach (DataRow reader in dt.Rows)
                {
                    card_num = reader.ItemArray[1].ToString();
                    char chr = ' ';
                    card_num = card_num.PadRight(10, chr);

                    status_in = reader.ItemArray[2].ToString();
                    status_out = reader.ItemArray[3].ToString();

                    strIN = status_in;
                    strOUT = status_out;
                    beYearFormat = 0;

                    if (status_in != "")
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

                    if (status_out != "")
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


                    vType = reader.ItemArray[4].ToString();

                    vName = reader.ItemArray[6].ToString();
                    if (vName == "")
                        vName = reader.ItemArray[9].ToString();

                    if (classGlobal.DisplayHashTag == true)
                        vName = classGlobal.REPLACE_NAME(vName);

                    vCompany = reader.ItemArray[7].ToString();
                    vLicense = reader.ItemArray[8].ToString();

                    row = new DataGridViewRow();

                    row.Cells.Add(new DataGridViewTextBoxCell { Value = vType });
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = card_num });
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = vName });
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = vCompany });
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = vLicense });
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = strIN });
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = strOUT });

                    row.Cells.Add(new DataGridViewTextBoxCell { Value = reader.ItemArray[0].ToString() });

                    dataGridView1.Rows.Add(row);

                    row.Height = 40;  //<-- set each row height

                }

                dt.Dispose();
                dt = null;


            }
            else
            {    // home version
                dataGridView1.Columns.Add("col_type", "ประเภท");
                dataGridView1.Columns.Add("col_number", "เลข VISITOR");
                dataGridView1.Columns.Add("col_licenseplate", "ทะเบียนรถ");
                dataGridView1.Columns.Add("col_place", "สถานที่ติดต่อ");
                dataGridView1.Columns.Add("col_in", "เข้า");
                dataGridView1.Columns.Add("col_out", "ออก");
                dataGridView1.Columns.Add("col_typeid", "Type_ID");

                dataGridView1.Columns[0].Width = (this.ClientSize.Width / 6) - 12;
                dataGridView1.Columns[1].Width = (this.ClientSize.Width / 6) - 0;
                dataGridView1.Columns[2].Width = (this.ClientSize.Width / 6) - 12;
                dataGridView1.Columns[3].Width = (this.ClientSize.Width / 6) - 12;
                dataGridView1.Columns[4].Width = (this.ClientSize.Width / 6) - 12;
                dataGridView1.Columns[5].Width = (this.ClientSize.Width / 6) - 12;
                dataGridView1.Columns[6].Width = 0;  //typeid
                dataGridView1.Columns[6].Visible = false;

                dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.SystemColors.Highlight;
                dataGridView1.EnableHeadersVisualStyles = false;

                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    col.HeaderCell.Style.Font = new Font("Segoe UI", 22F, FontStyle.Bold, GraphicsUnit.Pixel);

                    col.HeaderCell.Style.ForeColor = Color.White;
                }
                //--
                if (dt == null)
                    dt = new DataTable("dt");

                if (dt.Rows.Count == 0)
                {
                    lbPage.Text = "0/0";
                    button1.Enabled = false;
                    button2.Enabled = false;
                    return;
                }


                string vLicense = "";
                string vPlace = "";

                string vType = "";
                string card_num = "";
                string status_in = "";
                string status_out = "";
                string strIN = "";
                string strOUT = "";
                int beYearFormat = 0;
                string[] arr1;
                string[] arr2;

                DataGridViewRow row;
                foreach (DataRow reader in dt.Rows)
                {
                    card_num = reader.ItemArray[1].ToString();
                    char chr = ' ';
                    card_num = card_num.PadRight(10, chr);

                    status_in = reader.ItemArray[2].ToString();
                    status_out = reader.ItemArray[3].ToString();

                    strIN = status_in;
                    strOUT = status_out;
                    beYearFormat = 0;

                    if (status_in != "")
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

                    if (status_out != "")
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


                    vType = reader.ItemArray[4].ToString();
                    vLicense = reader.ItemArray[7].ToString();
                    vPlace = reader.ItemArray[8].ToString();

                    row = new DataGridViewRow();

                    row.Cells.Add(new DataGridViewTextBoxCell { Value = vType });
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = card_num });
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = vLicense });
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = vPlace });
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = strIN });
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = strOUT });

                    row.Cells.Add(new DataGridViewTextBoxCell { Value = reader.ItemArray[0].ToString() });

                    dataGridView1.Rows.Add(row);

                    row.Height = 40;  //<-- set each row height

                }

                dt.Dispose();
                dt = null;


            }

            dataGridView1.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;


            //++ Set no selected on first record
            dataGridView1.Columns[1].DefaultCellStyle.SelectionForeColor = Color.Black;
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.White;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;
            dataGridView1.BackgroundColor = System.Drawing.SystemColors.HotTrack;
            //--

            UpdateFont();
            dataGridView1.AllowUserToAddRows = false;

            //++ set form height up to datagridview height
            DataGridViewRow row1 = dataGridView1.Rows[0];
            this.Height = (row1.Height * (dataGridView1.RowCount + 5));
            this.StartPosition = FormStartPosition.CenterScreen;
            //--

            lbPage.Text = pageCurrent.ToString() + "/" + pageAll.ToString();

            minimizeMemory();

        }

        private static void minimizeMemory()
        {
            GC.Collect(GC.MaxGeneration);
            GC.WaitForPendingFinalizers();
            SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, (UIntPtr)0xFFFFFFFF, (UIntPtr)0xFFFFFFFF);
        }
        private void button10_Click(object sender, EventArgs e)
        {
            boolTerminateJob = true;
            this.Close();  
        }

        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            if (showIndicator == true)
            {
                var grid = sender as DataGridView;
                var rowIdx = (e.RowIndex + 1).ToString();

                var centerFormat = new StringFormat()
                {
                    // right alignment might actually make more sense for numbers
                    Alignment = StringAlignment.Center,

                    LineAlignment = StringAlignment.Center
                };
                //get the size of the string
                Size textSize = TextRenderer.MeasureText(rowIdx, this.Font);
                //if header width lower then string width then resize
                if (grid.RowHeadersWidth < textSize.Width + 40)
                {
                    grid.RowHeadersWidth = textSize.Width + 40;
                }
                var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
                e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
            }

        }


        private DataTable Populate(int pgNo)
        {
            DataSet ds = null;
            DataTable dt = null;

            string query = "";
            query = QueryString();

            if (classGlobal.databaseType == "acc")
            {
                OleDbCommand cmd = new OleDbCommand(query, classGlobal.conn);
                cmd.Parameters.AddWithValue("@startDate", DbType.DateTime).Value =
                                                            dateTimePicker1.Value.ToString("yyyy-MM-dd") + " " + "00:00:00";
                cmd.Parameters.AddWithValue("@endDate", DbType.DateTime).Value =
                                                            dateTimePicker2.Value.ToString("yyyy-MM-dd") + " " + "23:59:59";
                OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                dt = new DataTable("dt");
                ds = new DataSet();
                if (pgNo == 0)
                { adapter.Fill(ds, "Table"); }
                else
                { adapter.Fill(ds, (pgNo - 1) * pageSize, pageSize, "Table"); }
                dt = ds.Tables[0];

                adapter.Dispose();
                adapter = null;
            }
            else if (classGlobal.databaseType == "psql")
            {
                query = query.Replace("@startDate", "'" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + " " + "00:00:00" + "'");
                query = query.Replace("@endDate", "'" + dateTimePicker2.Value.ToString("yyyy-MM-dd") + " " + "00:00:00" + "'");
                NpgsqlCommand cmd = new NpgsqlCommand(query, classGlobal.connP);
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(cmd);
                dt = new DataTable("dt");
                ds = new DataSet();
                if (pgNo == 0)
                { adapter.Fill(ds, "Table"); }
                else
                { adapter.Fill(ds, (pgNo - 1) * pageSize, pageSize, "Table"); }
                dt = ds.Tables[0];

                adapter.Dispose();
                adapter = null;
            }
            else
            {

                dt = classGlobal.dtAllLogsView.Copy();

                DataView dataView = new DataView(dt);
                dt = dataView.ToTable();
                
                classGlobal.arrayListVisitorId.Clear(); 
                while (dt.Rows.Count > pageSize)
                {
                    dt = dt.AsEnumerable().Skip((pgNo - 1) * pageSize).Take(pageSize).CopyToDataTable();                   
                }

                int row = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    classGlobal.arrayListVisitorId.Add(dr["visitorId"]);
                    row += 1;
                } 
            }

            return dt;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            button2.Enabled = true;
            pageCurrent = pageCurrent - 1;

            if (pageCurrent == 1)
            {
                button1.Enabled = false;
            }
            else
            {
                button1.Enabled = true;
            }

            DataTable dt = Populate(pageCurrent);
            DisplayDataGridView(dt);
            classGlobal.dtLogsPaging = dt.Copy();

        }
        private void button2_Click(object sender, EventArgs e)
        {

            button1.Enabled = true;
            pageCurrent = pageCurrent + 1;

            if (pageCurrent == pageAll)
            {
                button2.Enabled = false;
            }
            else
            {
                button2.Enabled = true;
            }

            DataTable dt = Populate(pageCurrent);
            DisplayDataGridView(dt);
            classGlobal.dtLogsPaging = dt.Copy();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //SearchFromVisitorNumber(textBox1.Text);
            }           
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                pageCurrent = 1;
                ListLogs();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DataTable dt = Populate(0);
            DisplayDataGridView(dt); 
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                pageCurrent = 1;
                ListLogs();
                return;
            }


            string query = "";
            query = QueryString();

            
            DataTable dtCount = null;

            if (classGlobal.databaseType == "acc")
            {
                OleDbDataAdapter adapter = new OleDbDataAdapter(query, classGlobal.conn);
                dtCount = new DataTable("dtCount");
                adapter.Fill(dtCount);

                int cntRow = dtCount.Rows.Count;
                if (cntRow % pageSize == 0)
                {
                    pageAll = cntRow / pageSize;
                }
                else
                {
                    pageTotal = cntRow / pageSize;
                    pageAll = (int)Math.Floor(pageTotal) + 1;
                }

                adapter.Dispose();
                adapter = null;
                dtCount.Dispose();
                dtCount = null;
            }
            else if (classGlobal.databaseType == "psql")
            {
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, classGlobal.connP);
                dtCount = new DataTable("dtCount");
                adapter.Fill(dtCount);

                int cntRow = dtCount.Rows.Count;
                if (cntRow % pageSize == 0)
                {
                    pageAll = cntRow / pageSize;
                }
                else
                {
                    pageTotal = cntRow / pageSize;
                    pageAll = (int)Math.Floor(pageTotal) + 1;
                }

                adapter.Dispose();
                adapter = null;
                dtCount.Dispose();
                dtCount = null;
            }
            else
            {
                pageCurrent = 1;
                ListLogs();
                return;
            }

            button1.Enabled = false;

            if (pageAll <= 1)
            {
                button1.Enabled = false;
                button2.Enabled = false;
            }
            else if (pageAll > 1)
            {
                button1.Enabled = false;
                button2.Enabled = true;
            }

            pageCurrent = 1;
            DataTable dt1 = Populate(pageCurrent);
            DisplayDataGridView(dt1);
           
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            int filedID = 0;
            if (classGlobal.FactoryVersion == true) // factory version
                filedID = 7;
            else // home version    
                filedID = 6;

            if (e.RowIndex > -1)
            {
                string ID = dataGridView1.Rows[e.RowIndex].Cells[filedID].FormattedValue.ToString().Trim();
                classGlobal.pub_id = Int32.Parse(ID);

                if (classGlobal.userId != "")
                    classGlobal.public_visitorId = classGlobal.arrayListVisitorId[e.RowIndex].ToString();

                if (classGlobal.FactoryVersion == true)
                {
                    FormViewVisitorFactory f = new FormViewVisitorFactory();
                    f.ShowDialog();
                }
                else
                {
                    frmViewGuestDetail f = new frmViewGuestDetail();
                    f.ShowDialog();
                }


            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //
        }

        private void DataGridView1_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >=0)
                dataGridView1.Cursor = Cursors.Hand;
        }

        private void DataGridView1_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.Cursor = Cursors.Default;
        }

        private void DateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            //List<String> lstDate = classGlobal.GET_LIST_OF_DATE_BETWEEN(dateTimePicker1.Value.Date, dateTimePicker2.Value.Date);
            //if (lstDate.Count == 0)
            //{
            //    frmMessageBox f = new frmMessageBox();
            //    f.strMessage = "เลือกวันที่ไม่ถูกต้อง!";
            //    f.strStatus = "Error";
            //    f.ShowDialog();
            //    dateTimePicker2.Value = DateTime.Now;
            //}
        }

        private void CmbPage_SelectedIndexChanged(object sender, EventArgs e)
        {
            pageCurrent = Int32.Parse(cmbPage.SelectedItem.ToString());
            DataTable dt = classGlobal.dtVisitor;
            dt = Populate(pageCurrent);
            DisplayDataGridView(dt);
        }

        //***************************************//

    }
}
