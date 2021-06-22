using Newtonsoft.Json.Linq;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WacVisitor
{
    public partial class frmSummaryLogs : Form
    {

        public frmSummaryLogs()
        {
            InitializeComponent();
        }       
        private void SubFormLogs_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            GETLOGS();

            #region โหลดการตั้งค่า ค่าจอดรถ
            if (classGlobal.userId != "" && classGlobal.FactoryVersion == false)
                classGlobal.public_JsonChargePark = ClassData.GET_PAID_CHAGRE_CONFIG();
            #endregion
        }

        DataTable dtLogsComplete;
        DataTable dtLogsStill;
        string typename_for_cloud = "";
        private void GETLOGS()
        {
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
                if (jsonString != "")
                {
                    JArray jsArray = JArray.Parse(jsonString);
                    int j = 0;
                    foreach (var x in jsArray)
                    {
                        dt.Rows.Add(j, x.ToString());
                        j += 1;
                    }
                }
                else
                {
                    //-- do nothing
                }

            }

            dataGridView1.Rows.Clear(); 

            //+++ Header Column
            dataGridView1.RowTemplate.Resizable = DataGridViewTriState.True;
            dataGridView1.RowTemplate.Height = 60;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.Columns[0].Width = 0;
            dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[1].Width = (this.ClientSize.Width / 4) - 15;  //dataGridView1.Width / 3;
            dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[2].Width = (this.ClientSize.Width / 4) - 15;  //dataGridView1.Width / 5;
            dataGridView1.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[3].Width = (this.ClientSize.Width / 4) - 15;  //dataGridView1.Width / 5;
            dataGridView1.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[4].Width = (this.ClientSize.Width / 4) - 15;  //dataGridView1.Width / 5;
            dataGridView1.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;

            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.SystemColors.Highlight; 
            dataGridView1.EnableHeadersVisualStyles = false;            

            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.HeaderCell.Style.Font = new Font("Segoe UI", 40F, FontStyle.Bold, GraphicsUnit.Pixel);
                col.HeaderCell.Style.ForeColor = Color.White;
            }
            //--

            if (classGlobal.userId != "")
            {
                dataGridView1.Visible = false;
                frmTopMost f = new frmTopMost();
                f.Show();


                dtLogsComplete = ClassData.GET_LOGS_COMPLETE(false, "");  // log เข้าออกสำเร้จ
                dtLogsStill = ClassData.GET_LOGS_STILL(true, "");         // log เข้าแต่ยังไม่ออก
                classGlobal.dtAllLogs = dtLogsComplete.Copy();
                classGlobal.dtAllLogs.Merge(dtLogsStill);

                classGlobal.dtAllLogsView = classGlobal.dtAllLogs.Copy();

                f.Close();
                dataGridView1.Visible = true;
            }

            if (dt.Rows.Count > 0 )
                dt = dt.Rows.Cast<DataRow>().Where(row => !row.ItemArray.All(field => field is DBNull || string.IsNullOrWhiteSpace(field as string))).CopyToDataTable();

            int cntIn = 0;
            int cntOut = 0;
            int cntStill = 0;
            foreach (DataRow dr in dt.Rows)
            {
                Application.DoEvents(); 
                typename_for_cloud = dr.ItemArray[1].ToString();
                cntIn = GetCount("SELECT COUNT(*) FROM tbl_visitor WHERE status_in <> '' AND typeid= " + Int32.Parse(dr.ItemArray[0].ToString()), "IN");
                cntOut = GetCount("SELECT COUNT(*) FROM tbl_visitor WHERE status_out <> '' AND typeid= " + Int32.Parse(dr.ItemArray[0].ToString()), "OUT");
                cntStill = GetCount("SELECT COUNT(*) FROM tbl_visitor WHERE status_in <> '' AND (status_out = '' OR status_out is null) AND typeid= " + Int32.Parse(dr.ItemArray[0].ToString()), "STILL");

                this.dataGridView1.Rows.Add(dr.ItemArray[0], dr.ItemArray[1].ToString(), cntIn, cntOut, cntStill);
            }
            GC.Collect();

            //+++ Set no selected on first record
            dataGridView1.Columns[1].DefaultCellStyle.SelectionForeColor = Color.Black;
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.White;   
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Blue; 
            dataGridView1.BackgroundColor = System.Drawing.SystemColors.HotTrack;
            //--- Set no selected on first record

            dt.Dispose();
            dt = null;

            //+++ Add total record
            int sum1 = 0;
            int sum2 = 0;
            int sum3 = 0;
            for (int i = 0; i < dataGridView1.Rows.Count; ++i)
            {
                sum1 += Convert.ToInt32(dataGridView1.Rows[i].Cells[2].Value);
                sum2 += Convert.ToInt32(dataGridView1.Rows[i].Cells[3].Value);
                sum3 += Convert.ToInt32(dataGridView1.Rows[i].Cells[4].Value);
            }
            String[] rowData = { "-1", "รวม", sum1.ToString(), sum2.ToString(), sum3.ToString() };
            this.dataGridView1.Rows.Add(rowData);
            //--- Add total record

            UpdateFont();
            dataGridView1.AllowUserToAddRows = false;  //ไม่ให้ auto add row บรรทัดสุดท้าย

            //++ total row style
            int totalRow = dataGridView1.RowCount - 1;
            dataGridView1.Rows[totalRow].DefaultCellStyle.Font = new Font("Segoe UI", 40F, FontStyle.Bold, GraphicsUnit.Pixel);
            dataGridView1.Rows[totalRow].DefaultCellStyle.ForeColor = Color.White;
            dataGridView1.Rows[totalRow].DefaultCellStyle.BackColor = System.Drawing.SystemColors.Highlight;
            //-- total row style
            
        }


        private int GetCount(string query,string evt)
        {
            try
            {
                if (evt != "STILL")
                {
                    //++filter  2018-10-10 00:00:00
                    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                    string filterDate = DateTime.Now.ToString("yyyy-MM-dd");
                    query = query + " " + "AND status_in LIKE '" + filterDate + "%'";
                    //--
                }

                DataTable dt = new DataTable("dt");
                int cnt = 0;
                if (classGlobal.databaseType == "acc")
                {
                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, classGlobal.conn);
                    adapter.Fill(dt);
                    adapter.Dispose();
                    adapter = null;
                    cnt = Int32.Parse(dt.Rows[0].ItemArray[0].ToString());
                    dt.Dispose();
                    dt = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, classGlobal.connP);
                    adapter.Fill(dt);
                    adapter.Dispose();
                    adapter = null;
                    cnt = Int32.Parse(dt.Rows[0].ItemArray[0].ToString());
                    dt.Dispose();
                    dt = null;
                }
                else
                {
                    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                    string filterDate = DateTime.Now.ToString("yyyy-MM-dd");
                    cnt = 0;
                    DataTable each_dt_count;

                    if (evt == "IN")
                    {
                        if (classGlobal.dtAllLogs.Rows.Count > 0)
                        {
                            DataView dv = new DataView(classGlobal.dtAllLogs);
                            dv.RowFilter = "visitorType = '" + typename_for_cloud + "'" + " AND " + "timestamp='" + filterDate + "'";
                            each_dt_count = dv.ToTable();
                            cnt = each_dt_count.Rows.Count;
                        }

                    }
                    else if (evt == "OUT")
                    {
                        if (classGlobal.dtAllLogs.Rows.Count > 0)
                        {
                            DataView dv = new DataView(classGlobal.dtAllLogs);
                            dv.RowFilter = "visitorType = '" + typename_for_cloud + "'" + " AND " + "timestamp='" + filterDate + "'" + 
                                                        " AND " + "recordTimeOut <>''";
                            each_dt_count = dv.ToTable();
                            cnt = each_dt_count.Rows.Count;
                        }
                    }
                    else
                    {
                        if (dtLogsStill == null)
                        {
                            cnt = 0;
                        }
                        else
                        {
                            DataView dv = new DataView(dtLogsStill);
                            dv.RowFilter = "visitorType = '" + typename_for_cloud + "'";
                            each_dt_count = dv.ToTable();
                            cnt = each_dt_count.Rows.Count;
                        }
                    }
                }
                
                return cnt;
            }
            catch
            {
                return 0;
            }
        }

        private void UpdateFont()
        {
            //Change cell font
            foreach (DataGridViewColumn c in dataGridView1.Columns)    
            {
                if (c.Name != "col_id" && c.Name != "col_type")  
                {
                    c.DefaultCellStyle.Font = new Font("Segoe UI", 35F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Pixel);
                    c.DefaultCellStyle.ForeColor = Color.Blue;
                }
                else 
                {
                    c.DefaultCellStyle.Font = new Font("Segoe UI", 35F, FontStyle.Bold | FontStyle.Regular, GraphicsUnit.Pixel);   // visitor name                    
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)  { return; }

            string indexType = dataGridView1.Rows[e.RowIndex].Cells[0].FormattedValue.ToString();            
            string type_name = dataGridView1.Rows[e.RowIndex].Cells[1].FormattedValue.ToString();
            string selectedValue = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].FormattedValue.ToString();

            if (selectedValue == "0")
            {
                frmMessageBox _f = new frmMessageBox();
                _f.strMessage = "ไม่มีข้อมูล";
                _f.strStatus = "Warning";
                _f.ShowDialog();
                return;
            }


            string query = "";
            if (e.ColumnIndex == 2)  //เข้า
            {
                classGlobal.view_status = "IN"; 
                query = "SELECT  t1.id, t1.object_idcompany, t1.card_number, t1.str_imagedocument, t1.str_imagewebcamera, t1.str_template, " +
                                        "t1.status_in, t1.status_out, t1.typeid, t2.typename " +
                                            "FROM tbl_visitor t1 INNER JOIN tbl_type t2 ON t1.typeid = t2.typeid " +
                                            "WHERE t1.status_in <> '' AND t1.typeid=" + indexType + " " +
                                            "ORDER BY t1.status_in DESC";

            }
            else if (e.ColumnIndex == 3) //ออก
            {
                classGlobal.view_status = "OUT"; 
                query = "SELECT  t1.id, t1.object_idcompany, t1.card_number, t1.str_imagedocument, t1.str_imagewebcamera, t1.str_template, " +
                        "t1.status_in, t1.status_out, t1.typeid, t2.typename " +
                            "FROM tbl_visitor t1 INNER JOIN tbl_type t2 ON t1.typeid = t2.typeid " +
                            "WHERE t1.status_out <> '' AND t1.typeid=" + indexType + " " +
                            "ORDER BY t1.status_in DESC";
            }
            else if (e.ColumnIndex == 4) //คงเหลือ
            {
                classGlobal.view_status = "STILL"; 
                query = "SELECT  t1.id, t1.object_idcompany, t1.card_number, t1.str_imagedocument, t1.str_imagewebcamera, t1.str_template, " +
                        "t1.status_in, t1.status_out, t1.typeid, t2.typename " +
                            "FROM tbl_visitor t1 INNER JOIN tbl_type t2 ON t1.typeid = t2.typeid " +
                            "WHERE t1.status_in <> '' AND (t1.status_out = '' OR t1.status_out is null) AND t1.typeid=" + indexType + " " +
                            "ORDER BY t1.status_in DESC";
            }
            else
            {
                classGlobal.pub_query = "";
                return;
            }

            if (indexType == "-1")  // รวม
            {
                if (e.ColumnIndex == 2)  //เข้า
                {
                    classGlobal.view_status = "IN"; 
                    query = "SELECT  t1.id, t1.object_idcompany, t1.card_number, t1.str_imagedocument, t1.str_imagewebcamera, t1.str_template, " +
                                            "t1.status_in, t1.status_out, t1.typeid, t2.typename " +
                                                "FROM tbl_visitor t1 INNER JOIN tbl_type t2 ON t1.typeid = t2.typeid " +
                                                "WHERE t1.status_in <> '' " +
                                                "ORDER BY t1.status_in DESC";
                }
                else if (e.ColumnIndex == 3) //ออก
                {
                    classGlobal.view_status = "OUT"; 
                    query = "SELECT  t1.id, t1.object_idcompany, t1.card_number, t1.str_imagedocument, t1.str_imagewebcamera, t1.str_template, " +
                                            "t1.status_in, t1.status_out, t1.typeid, t2.typename " +
                                                "FROM tbl_visitor t1 INNER JOIN tbl_type t2 ON t1.typeid = t2.typeid " +
                                                "WHERE t1.status_out <> '' " +
                                                "ORDER BY t1.status_in DESC";
                }
                else if (e.ColumnIndex == 4) //คงเหลือ
                {
                    classGlobal.view_status = "STILL"; 
                    query = "SELECT  t1.id, t1.object_idcompany, t1.card_number, t1.str_imagedocument, t1.str_imagewebcamera, t1.str_template, " +
                                            "t1.status_in, t1.status_out, t1.typeid, t2.typename " +
                                                "FROM tbl_visitor t1 INNER JOIN tbl_type t2 ON t1.typeid = t2.typeid " +
                                                "WHERE t1.status_in <> '' AND (t1.status_out = '' OR t1.status_out is null)  " +
                                                "ORDER BY t1.status_in DESC";
                }
                else
                {
                    classGlobal.pub_query = "";
                    return;
                }
                
            }


            classGlobal.pub_query = query;
            DataTable dt = new DataTable("dt");

            if (classGlobal.databaseType == "acc")
            {
                OleDbDataAdapter adapter = new OleDbDataAdapter(query, classGlobal.conn);
                adapter.Fill(dt);
                adapter.Dispose();
                adapter = null;
            }
            else if (classGlobal.databaseType == "psql")
            {
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, classGlobal.connP);
                adapter.Fill(dt);
                adapter.Dispose();
                adapter = null;
            }
            else
            {
                // ++ โหลดข้อมุลอีกครั้ง เพื่อได้ข้อมูลล่าสุด
                dtLogsComplete = ClassData.GET_LOGS_COMPLETE(false, "");  // log เข้าออกสำเร้จ
                dtLogsStill = ClassData.GET_LOGS_STILL(true, "");         // log เข้าแต่ยังไม่ออก
                classGlobal.dtAllLogs = dtLogsComplete.Copy();
                classGlobal.dtAllLogs.Merge(dtLogsStill);

                classGlobal.dtAllLogsView = classGlobal.dtAllLogs.Copy();
                // -- โหลดข้อมุลอีกครั้ง เพื่อได้ข้อมูลล่าสุด

                dt.Columns.Add("id");
                dt.Columns.Add("object_idcompany");
                dt.Columns.Add("card_number");
                dt.Columns.Add("str_imagedocument");
                dt.Columns.Add("str_imagewebcamera");
                dt.Columns.Add("str_template");
                dt.Columns.Add("status_in");
                dt.Columns.Add("status_out");
                dt.Columns.Add("typeid");
                dt.Columns.Add("typename");
                dt.Columns.Add("visitorId");

                dt.Columns.Add("visitorNumber");
                dt.Columns.Add("image1");
                dt.Columns.Add("image2");
                dt.Columns.Add("image3");
                dt.Columns.Add("image4");
                dt.Columns.Add("recordTimeIn");
                dt.Columns.Add("recordTimeOut");  
                dt.Columns.Add("visiterType");

                dt.Columns.Add("citizenId");
                dt.Columns.Add("vname");
                dt.Columns.Add("place");
                dt.Columns.Add("register");
                dt.Columns.Add("vehicleType");
                dt.Columns.Add("etc"); 

                string base64image1 = "";
                string base64image2 = "";
                string base64image3 = "";
                string base64image4 = "";
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                string filterDate = DateTime.Now.ToString("yyyy-MM-dd");
                DataTable dtFound = null;
                if (classGlobal.view_status == "STILL")
                {
                    DataView dv = new DataView(dtLogsStill);
                    if (type_name != "รวม")
                        dv.RowFilter = "visitorType = '" + type_name + "'";

                    dtFound = dv.ToTable();

                }
                else if (classGlobal.view_status == "IN")
                {
                    DataView dv = new DataView(classGlobal.dtAllLogs);
                    if (type_name != "รวม")
                        dv.RowFilter = "visitorType = '" + type_name + "'" + " AND " + "timestamp='" + filterDate + "'";
                    else
                        dv.RowFilter = "timestamp='" + filterDate + "'";
                    
                    dtFound = dv.ToTable();

                }
                else if (classGlobal.view_status == "OUT")
                {
                    DataView dv = new DataView(classGlobal.dtAllLogs);
                    if (type_name != "รวม")
                        dv.RowFilter = "visitorType = '" + type_name + "'" + " AND " + "timestamp='" + filterDate + "'" + " AND " + "recordTimeOut <>''";
                    else
                        dv.RowFilter = "timestamp='" + filterDate + "'" + " AND " + "recordTimeOut <>''";

                    dtFound = dv.ToTable();
                }
                else
                {
                    //-- do nothing 
                }

                int r = 1;
                foreach (DataRow dr in dtFound.Rows)
                {
                    base64image1 = dr["image1"].ToString();
                    base64image2 = dr["image2"].ToString();
                    base64image3 = dr["image3"].ToString();
                    base64image4 = dr["image4"].ToString();

                    dt.Rows.Add(r.ToString(), "", dr["visitorNumber"], base64image1, base64image2, "", dr["recordTimeIn"], dr["recordTimeOut"], "0", dr["visitorType"], dr["visitorId"],
                                    dr["visitorNumber"], base64image1, base64image2, base64image3, base64image4, dr["recordTimeIn"], dr["recordTimeOut"], dr["visitorType"],
                                    dr["citizenId"], dr["name"], dr["contactPlace"], dr["licensePlate"], dr["vehicleType"], dr["etc"]);

                    r += 1;
                }
            }


            if (e.ColumnIndex != 4)
            {
                if (dt.Rows.Count > 0)
                {
                    if (classGlobal.userId == "")
                    {
                        //++filter  2018-10-10 00:00:00
                        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                        string filterDate = DateTime.Now.ToString("yyyy-MM-dd");
                        DataView dv1 = dt.DefaultView;
                        dv1.RowFilter = "status_in LIKE '" + filterDate + "%'";
                        dt = dv1.ToTable();
                        //--
                    }                    
                }                
            }

            classGlobal.visitor_type_name = type_name;
            classGlobal.dtVisitor = dt.Copy(); 

            dt.Dispose();
            dt = null;

            frmViewVisitor f = new frmViewVisitor();
            f.ShowDialog();

            GETLOGS();
        }

        private void DataGridView1_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {            
            if (e.RowIndex >= 0)
                dataGridView1.Cursor = Cursors.Hand;
        }

        private void DataGridView1_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.Cursor = Cursors.Default;
        }
    }
}
