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
using System.Text;
using System.Windows.Forms;

using Npgsql;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace WacVisitor
{
    public partial class FormChargeReport : Form
    {

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();


        public FormChargeReport()
        {
            InitializeComponent();
        }

        DataTable dtClone;
        DataTable dtCloneAll;
        bool boolTerminateJob = false;

        private void FormChargeReport_Load(object sender, EventArgs e)
        {
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            panel1.Left = (panel3.Width - panel1.ClientSize.Width) / 2;

            dateTimePicker1.Value = DateTime.Now;
            dateTimePicker1.Value = Convert.ToDateTime("0:00 AM");  //"8:00 AM"

            dateTimePicker2.Value = DateTime.Now.AddHours(1.0);

            
            dataGridView1.Left = (panel4.Width - dataGridView1.ClientSize.Width) / 2;
            dataGridView1.Visible = false;
            label5.Visible = false;

            dataGridView1.BackgroundColor = System.Drawing.SystemColors.HotTrack;

            button1.Enabled = false;


            panelPaging.Visible = false;
            panelPaging.Left = ClientSize.Width - panelPaging.Width - 50;
            panelPaging.Top = label5.Top - 5;

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            boolTerminateJob = true;
            this.Close();
        }


        DataTable dtInit = new DataTable("dt");
        private void btnReport_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable("dt");
            //ดูรายงาน
            string query = "";
            query = "SELECT tbl_charge_logs.*,  tbl_visitor.status_in, Mid(tbl_visitor.status_in, 1, 10) AS status_in_ex, tbl_visitor.status_out, Mid(tbl_visitor.status_out, 1, 10) AS status_out_ex, tbl_visitor.card_number, tbl_visitor.typeid " +
                            "FROM (tbl_charge_logs LEFT JOIN tbl_visitor ON tbl_visitor.id = tbl_charge_logs.id) " +
                            "WHERE tbl_visitor.status_in BETWEEN @startDate AND @endDate ORDER BY tbl_charge_logs.AID ASC";

            //++ เพิ่มสถานที่
            query = "SELECT tbl_charge_logs.*,  tbl_visitor.status_in, Mid(tbl_visitor.status_in, 1, 10) AS status_in_ex, tbl_visitor.status_out, Mid(tbl_visitor.status_out, 1, 10) AS status_out_ex, tbl_visitor.card_number, tbl_visitor.typeid, tbl_moreinfo.place " +
                            "FROM (tbl_charge_logs LEFT JOIN tbl_visitor ON tbl_visitor.id = tbl_charge_logs.id) " +
                            "LEFT JOIN tbl_moreinfo ON tbl_visitor.id = tbl_moreinfo.id " +
                            "WHERE tbl_visitor.status_in BETWEEN @startDate AND @endDate ORDER BY tbl_charge_logs.AID ASC";
            //-- เพิ่มสถานที่

            if (classGlobal.databaseType == "acc")
            {
                OleDbCommand cmd = new OleDbCommand(query, classGlobal.conn);
                cmd.Parameters.AddWithValue("@startDate", DbType.DateTime).Value = dateTimePicker1.Value.ToString("yyyy-MM-dd") + " " + "00:00:00" + "'";
                cmd.Parameters.AddWithValue("@endDate", DbType.DateTime).Value = dateTimePicker2.Value.ToString("yyyy-MM-dd") + " " + "23:59:59" + "'";
                OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);                
                adapter.Fill(dt);
                adapter.Dispose();
                adapter = null;
            }
            else if (classGlobal.databaseType == "psql")
            {
                query = query.Replace("Mid", "Substring");
                query = query.Replace("@startDate", "'" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + " " + "00:00:00" + "'");
                query = query.Replace("@endDate", "'" + dateTimePicker2.Value.ToString("yyyy-MM-dd") + " " + "23:59:59" + "'");
                NpgsqlCommand cmd = new NpgsqlCommand(query, classGlobal.connP);
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(cmd);
                adapter.Fill(dt);
                adapter.Dispose();
                adapter = null;
            }
            else
            {
                string startTime = dateTimePicker1.Value.ToString("yyyy-MM-dd") + " " + "00:00:00";
                string stopTime = dateTimePicker2.Value.ToString("yyyy-MM-dd") + " " + "23:59:59";

                Cursor.Current = Cursors.WaitCursor;
                ////++ LOADING....
                //DataTable dtLogsComplete = ClassData.GET_LOGS_COMPLETE(true);  // log เข้าออกสำเร้จ
                //DataTable dtExport = dtLogsComplete.Copy();
                //DataView dv = dtExport.DefaultView;
                //if (classGlobal.userId != "")
                //    dv.Sort = "recordTimeIn " + classGlobal.sortOrderBy;
                //else
                //    dv.Sort = "status_in " + classGlobal.sortOrderBy;
                //dtLogsComplete = dv.ToTable();
                ////-- LOADING....

                ////++ LOADING....
                //DataTable dtLogsPaid = ClassData.GET_CHARGE_LOGS();  // log การจ่ายเงิน
                //DataTable dtPaid = dtLogsPaid.Copy();
                //DataView dvPaid = dtPaid.DefaultView;
                //if (classGlobal.userId != "")
                //    dvPaid.Sort = "recordTimeIn " + classGlobal.sortOrderBy;
                //else
                //    dvPaid.Sort = "status_in " + classGlobal.sortOrderBy;
                //dtLogsPaid = dvPaid.ToTable();
                ////-- LOADING....


                //++ LOADING....
                List<String> lstDate = classGlobal.GET_LIST_OF_DATE_BETWEEN(dateTimePicker1.Value.Date, dateTimePicker2.Value.Date);
                DataTable dtTemp = new DataTable();
                DataTable dtLogsPaid = new DataTable();
                for (int d = 0; d < lstDate.Count; d++)
                {
                    if (boolTerminateJob == true)
                        return;

                    dtTemp = ClassData.GET_CHARGE_LOGS(false, lstDate[d]);  // log เข้าออกสำเร้จ
                    dtLogsPaid.Merge(dtTemp);
                }
                DataView dv1 = dtLogsPaid.DefaultView;
                if (classGlobal.userId != "")
                    dv1.Sort = "recordTimeIn " + classGlobal.sortOrderBy;
                else
                    dv1.Sort = "status_in " + classGlobal.sortOrderBy;
                dtLogsPaid = dv1.ToTable();
                //-- LOADING....

                DataTable dtVisitorLogs = dtLogsPaid.Copy();
                if (dtVisitorLogs.Rows.Count > 0)
                {
                    dtVisitorLogs = dtVisitorLogs.Select().Where(p => (Convert.ToDateTime(p["recordTimeIn"]) >= Convert.ToDateTime(startTime)) && 
                                                                        (Convert.ToDateTime(p["recordTimeIn"]) <= Convert.ToDateTime(stopTime))).CopyToDataTable();

                }
                else
                {
                    Console.Write(dtVisitorLogs);
                }
                    

                string[] columnName = new string[14] { "AID", "id", "minutes", "charge", "extra",
                                                          "paid", "charge_type", "status_in", "status_in_ex", "status_out", "status_out_ex",
                                                             "card_number", "typeid", "place" };  // status_in = 2020-12-07 15:00:00 ; status_in_ex = 2020-12-07
                foreach (var col in columnName)
                    dt.Columns.Add(col);

                int rows = 1;
                string visitorId = "";
                string visitor_Number = "";
                string visitor_Type = "";
                string parkMinutes = "0";
                string chargeBaht = "0";
                string paidExtraBaht = "0";
                string paid_or_not_paid = "N";
                string log_charge_type = "-";

                foreach (DataRow dr in dtLogsPaid.Rows)
                {
                    visitorId = dr["visitorId"].ToString();

                    DataTable dtMatchLogs = new DataTable("dtMatchLogs");
                    try { dtMatchLogs = dtVisitorLogs.Select("visitorId = '" + visitorId + "'").CopyToDataTable(); }
                    catch { /*****/ }

                    if (dtMatchLogs.Rows.Count > 0)
                    {
                        visitor_Number = dtMatchLogs.Rows[0]["visitorNumber"].ToString();
                        visitor_Type = dtMatchLogs.Rows[0]["visitorType"].ToString();

                        parkMinutes = classGlobal.COMPARE_BETWEEN_TIME_TO_MINUTES(dtMatchLogs.Rows[0]["recordTimeIn"].ToString(),
                                                                                    dtMatchLogs.Rows[0]["recordTimeOut"].ToString()).ToString();

                        //++ *** loop หาข้อมุลการจ่ายค่าจอด ใน log การจ่ายเงิน
                        parkMinutes = dr["minutes"].ToString();
                        chargeBaht = dr["charge"].ToString();
                        paidExtraBaht = dr["extra"].ToString();
                        paid_or_not_paid = dr["paid"].ToString();        // Y / N
                        log_charge_type = dr["charge_type"].ToString();  // A / B / C / D / E

                        dt.Rows.Add(rows, dtMatchLogs.Rows[0]["visitorId"].ToString(), parkMinutes, chargeBaht, paidExtraBaht,
                                        paid_or_not_paid, log_charge_type,
                                            dtMatchLogs.Rows[0]["recordTimeIn"].ToString(),
                                            dtMatchLogs.Rows[0]["recordTimeIn"].ToString().Substring(0, 10),
                                                dtMatchLogs.Rows[0]["recordTimeOut"].ToString(),
                                                dtMatchLogs.Rows[0]["recordTimeOut"].ToString().Substring(0, 10),
                                                    visitor_Number, visitor_Type,
                                                        dtMatchLogs.Rows[0]["contactPlace"].ToString());
                        rows += 1;
                        //-- *** loop หาข้อมุลการจ่ายค่าจอด ใน log การจ่ายเงิน
                    }

                }

                Cursor.Current = Cursors.Arrow;  

            }

            //++ paging
            currentPage = 0;
            int cntRow = dt.Rows.Count;
            if (cntRow % pageSize == 0)
            {
                totalPages = cntRow / pageSize;
            }
            else
            {
                pageTotal = cntRow / pageSize;
                totalPages = (int)Math.Floor(pageTotal) + 1;
            }
            currentPage++;
            offset = 0;
            //-- paging


            //DISPLAY_LOGS(dt);  // โค้ดเดิม แสดงทั้งหมดทุกรายการ

            DISPLAY_LOGS_ALL(dt);
            dtInit = dt.Copy();
            DISPLAY_LOGS(dtInit);

            dataGridView1.Visible = true;
            label5.Visible = true;

            //label5.Text = "จำนวน " + (dtClone.Rows.Count - 1) + " รายการ";    // โค้ดเดิม แสดงทั้งหมดทุกรายการ

            label5.Text = "จำนวน " + (dt.Rows.Count) + " รายการ";

            panelPaging.Left = this.Width - panelPaging.Width;
            panelPaging.Visible = true;
           
        }


        //++ paging
        int offset = 0;
        float pageTotal = 0.0f;
        int currentPage = 0;
        int totalPages = 0;
        int pageSize = 12;
        //-- paging

        private void DISPLAY_LOGS_ALL(DataTable dt)
        {
            DataTable dt2 = new DataTable();
            dt2.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("เวลาออก"),
                new DataColumn("เวลาเข้า"),
                new DataColumn("สถานที่ติดต่อ"),
                new DataColumn("เลข Visitor"),
                new DataColumn("ประเภท Visitor"),
                new DataColumn("ประเภทการคิดเงิน"),
                new DataColumn("ระยะเวลาจอด(ชั่วโมง)"),
                new DataColumn("ระยะเวลาคิดเงิน(ชั่วโมง)"),
                new DataColumn("จำนวนเงิน"),
                new DataColumn("สถานะ")
            });

            string[] strings = new string[0];
            string[] splt = new string[0];
            string status_in = "";
            string status_out = "";
            string visitor = "";
            string visitor_type = "";
            string charge_type = "";
            string workHours = "";
            TimeSpan spWorkMin;
            string charge = "";
            int baht = 0;
            int extra = 0;
            int sum = 0;
            string statusLog = "";
            string s_place = "";

            int nFreeMinutes = 0;
            int nRemainPark = 0;
            string RemainParkTime = "";

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                nFreeMinutes = 0;

                splt = dt.Rows[i].ItemArray[9].ToString().Split(' ');
                strings = splt[0].ToString().Split('-');
                try
                {
                    status_out = strings[2] + " " + classGlobal.NUMBER_TO_MONTH(strings[1]) + " " + (int.Parse(strings[0].ToString()) + 543) + " " + splt[1];
                }
                catch
                {
                    status_out = "";
                }
                splt = dt.Rows[i].ItemArray[7].ToString().Split(' ');
                strings = splt[0].ToString().Split('-');
                status_in = strings[2] + " " + classGlobal.NUMBER_TO_MONTH(strings[1]) + " " + (int.Parse(strings[0].ToString()) + 543) + " " + splt[1];

                visitor = dt.Rows[i].ItemArray[11].ToString();

                spWorkMin = TimeSpan.FromMinutes(int.Parse(dt.Rows[i].ItemArray[2].ToString()));
                workHours = string.Format("{0}:{1}", ((int)spWorkMin.TotalHours).ToString().PadLeft(2, '0'), spWorkMin.Minutes.ToString().PadLeft(2, '0'));

                charge = dt.Rows[i].ItemArray[3].ToString();
                if (charge == "")
                    charge = "0";
                baht = int.Parse(charge);

                charge = dt.Rows[i].ItemArray[4].ToString();
                if (charge == "")
                    charge = "0";
                extra = int.Parse(charge);

                visitor_type = GET_TYPE_VISITOR(dt.Rows[i].ItemArray[12].ToString());

                statusLog = dt.Rows[i].ItemArray[5].ToString().Replace("Y", "จ่าย").Replace("N", "ไม่จ่าย").Replace("C", "แก้ไขเงิน");

                charge_type = dt.Rows[i].ItemArray[6].ToString();
                if (charge_type == "")
                    charge_type = "-";

                if (charge_type != "-")
                {
                    nFreeMinutes = GET_INFO_OF_CHARGE(visitor_type, charge_type);
                }

                nRemainPark = int.Parse(dt.Rows[i].ItemArray[2].ToString()) - nFreeMinutes;
                if (nRemainPark <= 0)
                {
                    nRemainPark = 0;
                    RemainParkTime = "00:00";
                }
                else
                {
                    spWorkMin = TimeSpan.FromMinutes(nRemainPark);
                    RemainParkTime = string.Format("{0}:{1}", ((int)spWorkMin.TotalHours).ToString().PadLeft(2, '0'), spWorkMin.Minutes.ToString().PadLeft(2, '0'));
                }


                //++ สถานที่ติดต่อ
                s_place = dt.Rows[i].ItemArray[13].ToString();
                if (s_place.Equals(""))
                    s_place = "-";
                //-- สถานที่ติดต่อ

                dt2.Rows.Add(status_out, status_in, s_place, visitor, visitor_type, charge_type, workHours, RemainParkTime, (baht + extra).ToString(), statusLog);

                sum = sum + (baht + extra);
            }

            dt2.Rows.Add("", "", "", "", "", "", "", "รวม", sum.ToString(), "");   //summary last row

            //******************************************//

            dataGridView2.RowTemplate.MinimumHeight = 30;
            dataGridView2.DataSource = dt2;
            //++ datagridview header
            dataGridView2.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 120, 215);
            dataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView2.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 16.0F, FontStyle.Bold, GraphicsUnit.Pixel);
            dataGridView2.RowHeadersVisible = false;
            dataGridView2.EnableHeadersVisualStyles = false;

            foreach (DataGridViewColumn dgvc in dataGridView2.Columns)
            {
                dgvc.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            for (int c = 0; c < dataGridView2.Columns.Count; c++)
            {
                dataGridView2.Columns[c].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView2.Columns[c].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            dataGridView2.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //-- datagridview header

            foreach (DataGridViewColumn c in dataGridView2.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Segoe UI", 14.0F, GraphicsUnit.Pixel);
            }

            dataGridView2.DefaultCellStyle.SelectionBackColor = Color.Blue;
            dataGridView2.DefaultCellStyle.SelectionForeColor = Color.White;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;


            if (dataGridView2.Rows.Count > 0)  // total last record
            {
                dataGridView2.Rows[dataGridView2.Rows.Count - 1].DefaultCellStyle.BackColor = Color.FromArgb(0, 120, 215);
                dataGridView2.Rows[dataGridView2.Rows.Count - 1].DefaultCellStyle.ForeColor = Color.White;
                dataGridView2.Rows[dataGridView2.Rows.Count - 1].DefaultCellStyle.Font = new Font("Segoe UI", 16.0F, FontStyle.Bold, GraphicsUnit.Pixel);
            }

            dtCloneAll = dt2;


            int ro = 0;
            int inxPayStatus = 9; //8;
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                try
                {
                    string RowType = row.Cells[inxPayStatus].Value.ToString();

                    if (RowType == "จ่าย")
                    {
                        row.Cells[inxPayStatus].Style.BackColor = Color.Green;
                        row.Cells[inxPayStatus].Style.ForeColor = Color.White;
                    }
                    else if (RowType == "ไม่จ่าย")
                    {
                        row.Cells[inxPayStatus].Style.BackColor = Color.Red;
                        row.Cells[inxPayStatus].Style.ForeColor = Color.White;
                    }
                    else if (RowType == "แก้ไขเงิน")
                    {
                        row.Cells[inxPayStatus].Style.BackColor = Color.Yellow;
                        row.Cells[inxPayStatus].Style.ForeColor = Color.Black;
                    }

                    ro = ro + 1;
                }
                catch
                {
                    //--
                }               
            }

            dataGridView2.Columns[0].Width = 180;
            dataGridView2.Columns[1].Width = 180;


            dataGridView2.ClearSelection();
        }
        private void DISPLAY_LOGS(DataTable dt)
        {
            DataTable dt2 = new DataTable();
            dt2.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("เวลาออก"),
                new DataColumn("เวลาเข้า"),
                new DataColumn("สถานที่ติดต่อ"),
                new DataColumn("เลข Visitor"),
                new DataColumn("ประเภท Visitor"),
                new DataColumn("ประเภทการคิดเงิน"),
                new DataColumn("ระยะเวลาจอด(ชั่วโมง)"),
                new DataColumn("ระยะเวลาคิดเงิน(ชั่วโมง)"),
                new DataColumn("จำนวนเงิน"),
                new DataColumn("สถานะ")
            });

            string[] strings = new string[0];
            string[] splt = new string[0];
            string status_in = "";
            string status_out = "";
            string visitor = "";
            string visitor_type = "";
            string charge_type = "";
            string workHours = "";
            TimeSpan spWorkMin;
            string charge = "";
            int baht = 0;
            int extra = 0;
            int sum = 0;
            string statusLog = "";
            string s_place = "";

            int nFreeMinutes = 0;
            int nRemainPark = 0;
            string RemainParkTime = "";

            //++ paging
            if (offset >= dtInit.Rows.Count)
                offset = (int)pageTotal * pageSize;

            if (dt.Rows.Count > 0)
            {
                var rows = (from r in dt.AsEnumerable() select r).Skip(offset).Take(pageSize);
                dt = rows.CopyToDataTable();
            }
            else
            {
                totalPages = 1;
            }            
            lbPaging.Text = " " + currentPage.ToString() + "/" + totalPages.ToString() + " ";
            next.Left = lbPaging.Left + lbPaging.Width;
            //-- paging

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                nFreeMinutes = 0;

                splt = dt.Rows[i].ItemArray[9].ToString().Split(' ');
                strings = splt[0].ToString().Split('-');
                try
                {
                    status_out = strings[2] + " " + classGlobal.NUMBER_TO_MONTH(strings[1]) + " " + (int.Parse(strings[0].ToString()) + 543) + " " + splt[1];
                }
                catch
                {
                    status_out = "";
                }
                splt = dt.Rows[i].ItemArray[7].ToString().Split(' ');
                strings = splt[0].ToString().Split('-');
                status_in = strings[2] + " " + classGlobal.NUMBER_TO_MONTH(strings[1]) + " " + (int.Parse(strings[0].ToString()) + 543) + " " + splt[1];

                visitor = dt.Rows[i].ItemArray[11].ToString();

                spWorkMin = TimeSpan.FromMinutes(int.Parse(dt.Rows[i].ItemArray[2].ToString()));
                workHours = string.Format("{0}:{1}", ((int)spWorkMin.TotalHours).ToString().PadLeft(2, '0'), spWorkMin.Minutes.ToString().PadLeft(2, '0'));

                charge = dt.Rows[i].ItemArray[3].ToString();
                if (charge == "")
                    charge = "0";
                baht = int.Parse(charge);

                charge = dt.Rows[i].ItemArray[4].ToString();
                if (charge == "")
                    charge = "0";
                extra = int.Parse(charge);

                visitor_type = GET_TYPE_VISITOR(dt.Rows[i].ItemArray[12].ToString());

                statusLog = dt.Rows[i].ItemArray[5].ToString().Replace("Y", "จ่าย").Replace("N", "ไม่จ่าย").Replace("C", "แก้ไขเงิน");

                charge_type = dt.Rows[i].ItemArray[6].ToString();
                if (charge_type == "")
                    charge_type = "-";

                if (charge_type != "-")
                {
                    nFreeMinutes = GET_INFO_OF_CHARGE(visitor_type, charge_type);
                }

                nRemainPark = int.Parse(dt.Rows[i].ItemArray[2].ToString()) - nFreeMinutes;
                if (nRemainPark <= 0)
                {
                    nRemainPark = 0;
                    RemainParkTime = "00:00";
                }
                else
                {
                    spWorkMin = TimeSpan.FromMinutes(nRemainPark);
                    RemainParkTime = string.Format("{0}:{1}", ((int)spWorkMin.TotalHours).ToString().PadLeft(2, '0'), spWorkMin.Minutes.ToString().PadLeft(2, '0'));
                }


                //++ สถานที่ติดต่อ
                s_place = dt.Rows[i].ItemArray[13].ToString();
                if (s_place.Equals(""))
                    s_place = "-";
                //-- สถานที่ติดต่อ

                dt2.Rows.Add(status_out, status_in, s_place, visitor, visitor_type, charge_type, workHours, RemainParkTime, (baht + extra).ToString(), statusLog);

                sum = sum + (baht + extra);
            }

            dt2.Rows.Add("", "", "", "", "", "", "", "รวม", sum.ToString(), "");   //summary last row

            button1.Enabled = true;
            if (dt.Rows.Count == 0)
                button1.Enabled = false;

            //******************************************//

            dataGridView1.RowTemplate.MinimumHeight = 30;
            dataGridView1.DataSource = dt2;
            //++ datagridview header
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 120, 215);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 16.0F, FontStyle.Bold, GraphicsUnit.Pixel);
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.EnableHeadersVisualStyles = false;

            foreach (DataGridViewColumn dgvc in dataGridView1.Columns)
            {
                dgvc.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            for (int c = 0; c < dataGridView1.Columns.Count; c++)
            {
                dataGridView1.Columns[c].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[c].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //-- datagridview header

            foreach (DataGridViewColumn c in dataGridView1.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Segoe UI", 14.0F, GraphicsUnit.Pixel);
            }

            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.Blue;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;


            if (dataGridView1.Rows.Count > 0)  // total last record
            {
                dataGridView1.Rows[dataGridView1.Rows.Count - 1].DefaultCellStyle.BackColor = Color.FromArgb(0, 120, 215);
                dataGridView1.Rows[dataGridView1.Rows.Count - 1].DefaultCellStyle.ForeColor = Color.White;
                dataGridView1.Rows[dataGridView1.Rows.Count - 1].DefaultCellStyle.Font = new Font("Segoe UI", 16.0F, FontStyle.Bold, GraphicsUnit.Pixel);
            }

            dtClone = dt2;


            int ro = 0;
            int inxPayStatus = 9; //8;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string RowType = row.Cells[inxPayStatus].Value.ToString();

                if (RowType == "จ่าย")
                {
                    row.Cells[inxPayStatus].Style.BackColor = Color.Green;
                    row.Cells[inxPayStatus].Style.ForeColor = Color.White;
                }
                else if (RowType == "ไม่จ่าย")
                {
                    row.Cells[inxPayStatus].Style.BackColor = Color.Red;
                    row.Cells[inxPayStatus].Style.ForeColor = Color.White;
                }
                else if (RowType == "แก้ไขเงิน")
                {
                    row.Cells[inxPayStatus].Style.BackColor = Color.Yellow;
                    row.Cells[inxPayStatus].Style.ForeColor = Color.Black;
                }

                ro = ro + 1;
            }

            dataGridView1.Columns[0].Width = 180;
            dataGridView1.Columns[1].Width = 180;


            dataGridView1.ClearSelection();
        }


        private DataTable FILTER_CHECKIN_GROUP(DataTable dt)
        {
            DataTable dt2 = new DataTable();
            dt2.Columns.AddRange(new DataColumn[] 
            {
                new DataColumn("วันที่"),
                new DataColumn("จำนวนเงิน")
            });

            var groups = dt.AsEnumerable().GroupBy(row => row["status_in_ex"]);
            foreach (var group in groups)
            {
                Console.WriteLine(group.Key);
                int money = 0;
                var groups2 = group.OrderBy(row => row["status_in"]).GroupBy(row => Convert.ToDateTime(row["status_in"]).ToString("yyyy-MM-dd"));
                foreach (var group2 in groups2)
                {
                    Console.WriteLine("\t" + group2.Key);
                    var list = group2.ToList();                    
                    for (int l = 0; l < list.Count;l++ )
                    {
                        money = money + int.Parse(list[l]["charge"].ToString());
                    }

                   dt2.Rows.Add(group2.Key, money.ToString());
                   money = 0;
                }
            }
            return dt2;
        }
        private void DISPLAY_GRIDVIEW(DataTable dt)
        {
            string[] strings = new string[0]; 
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                strings = dt.Rows[i][0].ToString().Split('-');
                dt.Rows[i][0] = strings[2] + " " + classGlobal.NUMBER_TO_MONTH(strings[1]) + " " + (int.Parse(strings[0].ToString()) + 543);
            }

            dataGridView1.RowTemplate.MinimumHeight = 30;
            dataGridView1.DataSource = dt;
            //++ datagridview header
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 120, 215);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 24.0F, FontStyle.Bold, GraphicsUnit.Pixel);
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.EnableHeadersVisualStyles = false;

            for (int c = 0; c < dataGridView1.Columns.Count; c++)
            {
                //dataGridView1.Columns[c].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;  //fit

                dataGridView1.Columns[c].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns[c].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //-- datagridview header

            foreach (DataGridViewColumn c in dataGridView1.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Segoe UI", 22.5F, GraphicsUnit.Pixel);
            }

            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.White;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;

            dataGridView1.Columns[0].Width = (dataGridView1.Width * 60) / 100;
            dataGridView1.Columns[1].Width = (dataGridView1.Width * 40) / 100;
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dtClone != null)
            {
                if (dtClone.Rows.Count != dtCloneAll.Rows.Count)
                    dtClone = dtCloneAll.Copy();


                string startText = dateTimePicker1.Value.ToString("dd-MM-yyyy_HH_mm_ss");
                string stopText = dateTimePicker2.Value.ToString("dd-MM-yyyy_HH_mm_ss");

                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Excel files (*.xlsx)|*.xlsx";
                saveFileDialog1.RestoreDirectory = true;
                saveFileDialog1.FileName = DateTime.Now.ToString("yyyyMMdd_HHmmss");

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string outputfilename = saveFileDialog1.FileName;

                    CreateExcelPackage(dtClone, outputfilename);
                }

               
            }
            else
            {
                //MessageBox.Show("ไม่พบข้อมูล");
                frmMessageBox f = new frmMessageBox();
                f.strMessage = "ไม่พบข้อมูล";
                f.strStatus = "Error";
                f.ShowDialog();
            }
        }

        private void CreateExcelPackage(DataTable dt, string xlsxfiles)
        {
            using (var package = new ExcelPackage())
            {
                var workbook = package.Workbook;
                //*** Sheet 1
                var worksheet = workbook.Worksheets.Add("Sheet1");
                //worksheet.Row(1).Height = 75;   // set row height

                if (System.IO.File.Exists(xlsxfiles) == false)   //กรณีไม่มีไฟล์ xlsx ให้สร้าง header column 
                {

                    worksheet.Cells[1, 1].Value = "เวลาออก";
                    worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[1, 2].Value = "เวลาเข้า";
                    worksheet.Cells[1, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[1, 3].Value = "สถานที่ติดต่อ";
                    worksheet.Cells[1, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[1, 4].Value = "เลข Visitor";
                    worksheet.Cells[1, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[1, 5].Value = "ประเภท Visitor";
                    worksheet.Cells[1, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[1, 6].Value = "ประเภทการคิดเงิน";
                    worksheet.Cells[1, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[1, 7].Value = "ระยะเวลาจอด(ชั่วโมง)";
                    worksheet.Cells[1, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[1, 8].Value = "ระยะเวลาคิดเงิน(ชั่วโมง)";
                    worksheet.Cells[1, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 8].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[1, 9].Value = "จำนวนเงิน";
                    worksheet.Cells[1, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[1, 9].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    worksheet.Cells[1, 10].Value = "สถานะ";
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
                //int i = 0;
                FileInfo file = new FileInfo(xlsxfiles);
                //create a new Excel package from the file
                using (ExcelPackage excelPackage = new ExcelPackage(file))
                {

                    //create an instance of the the first sheet in the loaded file
                    ExcelWorksheet worksheetA = excelPackage.Workbook.Worksheets[1];

                    int rowCnt, colCnt;

                    foreach (DataRow dr in dt.Rows)
                    {

                        rowCnt = worksheetA.Dimension.End.Row;
                        colCnt = worksheetA.Dimension.End.Column;

                        //worksheetA.Row(rowCnt + 1).Height = 30;  // set row height

                        //++ write values
                        worksheetA.Cells[rowCnt + 1, 1].Value = dr[0];
                        worksheetA.Cells[rowCnt + 1, 2].Value = dr[1];
                        worksheetA.Cells[rowCnt + 1, 3].Value = dr[2];
                        worksheetA.Cells[rowCnt + 1, 4].Value = dr[3];
                        worksheetA.Cells[rowCnt + 1, 5].Value = dr[4];
                        worksheetA.Cells[rowCnt + 1, 6].Value = dr[5];
                        worksheetA.Cells[rowCnt + 1, 7].Value = dr[6];
                        worksheetA.Cells[rowCnt + 1, 8].Value = dr[7];
                        worksheetA.Cells[rowCnt + 1, 9].Value = dr[8];
                        worksheetA.Cells[rowCnt + 1, 10].Value = dr[9];
  
                    }

                    //'++ set cell width             
                    worksheetA.Column(1).Width = 30;
                    worksheetA.Column(2).Width = 30;
                    worksheetA.Column(3).Width = 20;
                    worksheetA.Column(4).Width = 20;
                    worksheetA.Column(5).Width = 20;
                    worksheetA.Column(6).Width = 20;
                    worksheetA.Column(7).Width = 20;
                    worksheetA.Column(8).Width = 20;
                    worksheetA.Column(9).Width = 20;
                    worksheetA.Column(10).Width = 20;

                    //save the changes
                    excelPackage.Save();
                    excelPackage.Dispose();

                    frmMessageBox f = new frmMessageBox();
                    f.strMessage = "สำเร็จ " + (dt.Rows.Count - 1).ToString() + " รายการ";
                    f.strStatus = "Information";
                    f.ShowDialog();
                }

            }
        }

        private string GET_TYPE_VISITOR(string typeid)
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
                        Console.Write(typeid);  
                    }
                    

                    return typeid;
                }

            }
            catch
            {
                return "ไม่ระบุ";
            }
        }

        private int GET_INFO_OF_CHARGE(string typename, string class_type)
        {
            DataTable dtID = new DataTable("typename");

            int intFreeChargeMinute = 0;
            try
            {
                string query = "SELECT minutes, rate FROM tbl_charge_car_park WHERE typename='" + typename + "'" + " AND " + "class ='" + class_type + "'";

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
                    dtID.Columns.Add("minutes");
                    dtID.Columns.Add("rate");

                    JArray resources = (JArray)classGlobal.public_JsonChargePark["visitorType"];
                    foreach (var type3Resource in resources)
                    {                        
                        JToken jt = (JToken)type3Resource;
                        if (jt["typename"].ToString() == typename)
                        {
                            JToken _class = (JToken)jt["classname"].ToString();
                            var objs = JsonConvert.DeserializeObject(_class.ToString());
                            JArray subClass = (JArray)objs;
                            foreach (var _subClass in subClass)
                            {
                                if (_subClass["class"].ToString() == class_type)
                                    dtID.Rows.Add(_subClass["minutes"].ToString(), _subClass["rate"].ToString());
                            }
                        }
                    }
                }             

                string[] aMinutes = dtID.Rows[0][0].ToString().Split(';');
                string[] aRate = dtID.Rows[0][1].ToString().Split(';');
                int j = 0;
                for (int i = 0; i < aRate.Length; i++)
                {
                    if (int.Parse(aRate[i].ToString()) > 0)
                    {
                        j = i - 1;
                        break;
                    }
                }

                string strFreeTime = "";
                intFreeChargeMinute = int.Parse(aMinutes[j]);
                if (intFreeChargeMinute < 60)
                    strFreeTime = "จอดฟรี " + intFreeChargeMinute.ToString() + " นาที ";
                else
                    strFreeTime = "จอดฟรี " + (int.Parse(aMinutes[j]) / 60).ToString() + " ชั่วโมง ";

                return intFreeChargeMinute;
            }
            catch
            {
                intFreeChargeMinute = 0;
                return intFreeChargeMinute;
            }

        }

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //ReleaseCapture();
                //SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void Back_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (currentPage > 1)
                currentPage--;

            offset = (currentPage - 1) * pageSize;
            DISPLAY_LOGS(dtInit);
        }

        private void Next_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (currentPage < totalPages)
                currentPage++;

            offset = (currentPage - 1) * pageSize;
            DISPLAY_LOGS(dtInit);
        }
    }
}
