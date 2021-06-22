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
using Newtonsoft.Json.Linq;
using System.Collections;

namespace WacVisitor.FormSetting
{
    public partial class SettingBlackList : Form
    {
        #region PlaceHold TextBox
        private const int EM_SETCUEBANNER = 0x1501;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)]string lParam);
        #endregion


        string current_BlackWhite_List = "";
        public SettingBlackList()
        {
            InitializeComponent();

            if (classGlobal.strBlackOrWhiteList == "black")
            {
                label1.Text = "Black list";
                linkLabel1.Text = "[ กำหนดวันเวลาให้ Blacklist ]";
                current_BlackWhite_List = "tbl_blacklist";
            }
            else if(classGlobal.strBlackOrWhiteList == "white")
            {
                label1.Text = "White list";
                linkLabel1.Text = "[ กำหนดวันเวลาให้ Whitelist ]";
                current_BlackWhite_List = "tbl_whitelist";
            }
        }

        private void SettingBlackList_Load(object sender, EventArgs e)
        {
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            panel_blacklist.Left = (this.Width - panel_blacklist.ClientSize.Width) / 2;
            SendMessage(textBox4.Handle, EM_SETCUEBANNER, 0, "เลขประจำตัว");
            SendMessage(textBox5.Handle, EM_SETCUEBANNER, 0, "ชื่อ-สกุล");
            SendMessage(textBox6.Handle, EM_SETCUEBANNER, 0, "เริ่มต้น");
            SendMessage(textBox7.Handle, EM_SETCUEBANNER, 0, "สิ้นสุด");

            GET_BLACK_LIST();
        }

        ArrayList arrListId = new ArrayList();
        ArrayList arrListTimeId = new ArrayList();
        private void GET_BLACK_LIST()
        {
            DataTable dt = new DataTable("dt");

            if (classGlobal.databaseType == "acc")
            {
                OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * FROM " + current_BlackWhite_List, classGlobal.conn);
                adapter.Fill(dt);
                adapter.Dispose();
                adapter = null;
            }
            else if (classGlobal.databaseType == "psql")
            {
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter("SELECT * FROM " + current_BlackWhite_List, classGlobal.connP);
                adapter.Fill(dt);
                adapter.Dispose();
                adapter = null;
            }
            else
            {   //*** Get black / white list from server then retrive to datatable "dt"     

                dt.Columns.Add("ID");
                dt.Columns.Add("personal_number");
                dt.Columns.Add("fullname");
                dt.Columns.Add("start");
                dt.Columns.Add("stop");

                string jsonString = "";
                arrListId.Clear();
                arrListTimeId.Clear();
                if (classGlobal.strBlackOrWhiteList == "black")
                {
                    jsonString = ClassData.GET_METHODE_BLACK_WHITE_LIST("blacklist");
                }
                else if (classGlobal.strBlackOrWhiteList == "white")
                {
                    jsonString = ClassData.GET_METHODE_BLACK_WHITE_LIST("whitelist");
                }
                if (jsonString == "")
                {
                    //-- no data
                }
                else
                {
                    string BlackWhiteListName = "";
                    string BlackWhiteTimeListName = "";
                    JArray jsArray = JArray.Parse(jsonString);
                    int j = 1;
                    foreach (var x in jsArray)
                    {

                        if (classGlobal.strBlackOrWhiteList == "black")
                        {
                            arrListId.Add(x["blacklistId"]);

                            BlackWhiteListName = "blacklistId";
                            BlackWhiteTimeListName = "blacklistTimeId";
                        }
                        else if (classGlobal.strBlackOrWhiteList == "white")
                        {
                            arrListId.Add(x["whitelistId"]);

                            BlackWhiteListName = "whitelistId";
                            BlackWhiteTimeListName = "whitelistTimeId";
                        }

                        JArray js = (JArray)x["time"];
                        foreach (var jObj in js)
                        {
                            arrListTimeId.Add(x[BlackWhiteListName] + "," + jObj[BlackWhiteTimeListName]);
                            dt.Rows.Add(j, x["citizenId"], x["name"],
                                            classGlobal.CONVERT_UTC_TO_LOCAL(jObj["timeStart"].ToString()),
                                            classGlobal.CONVERT_UTC_TO_LOCAL(jObj["timeStop"].ToString()));
                            j += 1;
                        }
                    }
                }

            }

            dataGridView1.Rows.Clear();

            //+++ Header Column
            dataGridView1.RowTemplate.Resizable = DataGridViewTriState.True;
            dataGridView1.RowTemplate.Height = 40;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.Columns[0].Width = 0;
            dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[0].Visible = false;

            dataGridView1.Columns[1].Width = dataGridView1.Width / 4;
            dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[2].Width = dataGridView1.Width / 4;
            dataGridView1.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[3].Width = dataGridView1.Width / 4;
            dataGridView1.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[4].Width = dataGridView1.Width / 4;
            dataGridView1.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;

            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.SystemColors.Highlight;
            dataGridView1.EnableHeadersVisualStyles = false;

            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.HeaderCell.Style.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Pixel);
                col.HeaderCell.Style.ForeColor = Color.White;
            }
            //--

            string[] arrSpace = new string[0] { };
            string[] arrSlash = new string[0] { };
            string sDateStart = "";
            string sDateStop = "";
            int year = 0;
            int rowIndex = 0;
            foreach (DataRow dr in dt.Rows)
            {
                try
                {
                    arrSpace = dr.ItemArray[3].ToString().Split(' ');
                    arrSlash = arrSpace[0].ToString().Split('-');
                    year = Int32.Parse(arrSlash[0]);
                    if (year < 2500)
                    { year = year + 543; }
                    sDateStart = arrSlash[2] + "/" + arrSlash[1] + "/" + year.ToString() + " " + arrSpace[1];

                    arrSpace = dr.ItemArray[4].ToString().Split(' ');
                    arrSlash = arrSpace[0].ToString().Split('-');
                    year = Int32.Parse(arrSlash[0]);
                    if (year < 2500)
                    { year = year + 543; }
                    sDateStop = arrSlash[2] + "/" + arrSlash[1] + "/" + year.ToString() + " " + arrSpace[1];
                }
                catch
                {
                    //-----//
                }
                
                this.dataGridView1.Rows.Add(Int32.Parse(dr.ItemArray[0].ToString()),
                                                dr.ItemArray[1].ToString(),
                                                    dr.ItemArray[2].ToString(),
                                                        sDateStart,
                                                            sDateStop);

                rowIndex += 1;
            }

            //++ Set no selected on first record
            dataGridView1.Columns[1].DefaultCellStyle.SelectionForeColor = Color.Black;
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.White;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Blue;
            dataGridView1.BackgroundColor = System.Drawing.SystemColors.HotTrack;
            //--

            dt.Dispose();
            dt = null;

            foreach (DataGridViewColumn c in dataGridView1.Columns)
            {                
                c.DefaultCellStyle.Font = new Font("Segoe UI", 16F, FontStyle.Regular | FontStyle.Regular, GraphicsUnit.Pixel);
                c.DefaultCellStyle.ForeColor = Color.Black; 
            }

            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";

            dataGridView1.ReadOnly = true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Excel Files|*.xlsx";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop).ToString();
            dialog.Title = "กรุณาเลือกไฟล์เพื่อนำเข้ารายการ";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = dialog.FileName;
                DataTable dt = GetDataTableFromExcel(fileName, true);
                INSERT_TO_BLACKLIST_TABLE(dt);
            }   
        }
        private DataTable GetDataTableFromExcel(string path, bool hasHeader = true)
        {
            using (var pck = new OfficeOpenXml.ExcelPackage())
            {
                using (var stream = File.OpenRead(path))
                {
                    pck.Load(stream);
                }
                var ws = pck.Workbook.Worksheets.First();
                DataTable tbl = new DataTable();
                foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                {
                    tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                }
                var startRow = hasHeader ? 2 : 1;
                for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    DataRow row = tbl.Rows.Add();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                }
                return tbl;
            }
        }
        private void INSERT_TO_BLACKLIST_TABLE(DataTable dt)
        {
            if (dt.Rows.Count > 0)  // id    name   start  stop  (22/11/2561 00:00:00)
            {
                try
                {
                    string query = "";
                    OleDbCommand cmd;
                    NpgsqlCommand cmdP;

                    string[] arrSpace = new string[0] { };
                    string[] arrSlash = new string[0] { };
                    string sDateStart = "";
                    string sDateStop = "";
                    int year = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        Application.DoEvents();
                        System.Threading.Thread.Sleep(10);

                        arrSpace = dr.ItemArray[2].ToString().Split(' ');
                        arrSlash = arrSpace[0].ToString().Split('/');
                        year = Int32.Parse(arrSlash[2]);
                        if (year > 2500)
                        { year = year - 543; }
                        sDateStart = year.ToString() + "-" + arrSlash[1] + "-" + arrSlash[0] + " " + arrSpace[1];

                        arrSpace = dr.ItemArray[3].ToString().Split(' ');
                        arrSlash = arrSpace[0].ToString().Split('/');
                        year = Int32.Parse(arrSlash[2]);
                        if (year > 2500)
                        { year = year - 543; }
                        sDateStop = year.ToString() + "-" + arrSlash[1] + "-" + arrSlash[0] + " " + arrSpace[1];

                        query = String.Format("INSERT INTO " + current_BlackWhite_List + " (personal_number, fullname, start, stop) VALUES ('{0}','{1}','{2}','{3}')",
                                                    dr.ItemArray[0].ToString(), dr.ItemArray[1].ToString(), sDateStart, sDateStop);

                        if (classGlobal.databaseType == "acc")
                        {
                            cmd = new OleDbCommand(query, classGlobal.conn);
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                            cmd = null;
                        }
                        else if (classGlobal.databaseType == "psql")
                        {
                            cmdP = new NpgsqlCommand(query, classGlobal.connP);
                            cmdP.ExecuteNonQuery();
                            cmdP.Dispose();
                            cmdP = null;
                        }
                        else
                        {
                            string s = "";
                            if (classGlobal.strBlackOrWhiteList == "black")
                            {
                                sDateStart = classGlobal.CONVERT_LOCAL_TO_UTC(sDateStart);
                                sDateStop = classGlobal.CONVERT_LOCAL_TO_UTC(sDateStop); 
                                s = ClassData.POST_METHODE_BLACK_WHITE_LIST("blacklist", dr.ItemArray[0].ToString(), dr.ItemArray[1].ToString(), sDateStart, sDateStop);
                            }
                            else if (classGlobal.strBlackOrWhiteList == "white")
                            {
                                sDateStart = classGlobal.CONVERT_LOCAL_TO_UTC(sDateStart);
                                sDateStop = classGlobal.CONVERT_LOCAL_TO_UTC(sDateStop); 
                                s = ClassData.POST_METHODE_BLACK_WHITE_LIST("whitelist", dr.ItemArray[0].ToString(), dr.ItemArray[1].ToString(), sDateStart, sDateStop);
                            }
                        }
                    }
                }
                catch
                {

                }

                GET_BLACK_LIST();

            }


        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("th-TH");
            textBox6.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm" + ":" + "00");
            textBox7.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm" + ":" + "59");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox4.Text == "") { return; }
            //if (textBox5.Text == "") { return; }  // ชื่อ-สกุล
            if (textBox6.Text == "") { return; }
            if (textBox7.Text == "") { return; }

            classGlobal.MsgText = "ต้องการเพิ่มรายการ?";
            Msg m = new Msg();
            m.ShowDialog();

            string ret = classGlobal.MsgConfirm;
            if (ret == "YES")
            {
                try
                {
                    string[] arrSpace = new string[0] { };
                    string[] arrSlash = new string[0] { };
                    string sDateStart = "";
                    string sDateStop = "";
                    int year = 0;

                    arrSpace = textBox6.Text.Split(' ');
                    arrSlash = arrSpace[0].ToString().Split('/');
                    year = Int32.Parse(arrSlash[2]);
                    if (year > 2500)
                    { year = year - 543; }
                    sDateStart = year.ToString() + "-" + arrSlash[1] + "-" + arrSlash[0] + " " + arrSpace[1];

                    arrSpace = textBox7.Text.Split(' ');
                    arrSlash = arrSpace[0].ToString().Split('/');
                    year = Int32.Parse(arrSlash[2]);
                    if (year > 2500)
                    { year = year - 543; }
                    sDateStop = year.ToString() + "-" + arrSlash[1] + "-" + arrSlash[0] + " " + arrSpace[1];


                    string query = String.Format("INSERT INTO " + current_BlackWhite_List + " (personal_number, fullname, start, stop) VALUES ('{0}','{1}','{2}','{3}')",
                                                       textBox4.Text, textBox5.Text, sDateStart, sDateStop);

                    if (classGlobal.databaseType == "acc")
                    {
                        OleDbCommand cmd = new OleDbCommand(query, classGlobal.conn);
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        cmd = null;
                    }
                    else if (classGlobal.databaseType == "psql")
                    {
                        NpgsqlCommand cmd = new NpgsqlCommand(query, classGlobal.connP);
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        cmd = null;
                    }
                    else
                    {
                        string s = "";
                        if (classGlobal.strBlackOrWhiteList == "black")
                        {
                            sDateStart = classGlobal.CONVERT_LOCAL_TO_UTC(sDateStart);
                            sDateStop = classGlobal.CONVERT_LOCAL_TO_UTC(sDateStop); 
                            s = ClassData.POST_METHODE_BLACK_WHITE_LIST("blacklist", textBox4.Text, textBox5.Text, sDateStart, sDateStop);           
                        }
                        else if (classGlobal.strBlackOrWhiteList == "white")
                        {
                            sDateStart = classGlobal.CONVERT_LOCAL_TO_UTC(sDateStart);
                            sDateStop = classGlobal.CONVERT_LOCAL_TO_UTC(sDateStop); 
                            s = ClassData.POST_METHODE_BLACK_WHITE_LIST("whitelist", textBox4.Text, textBox5.Text, sDateStart, sDateStop);                            
                        }
                        if (s != "200")
                        {
                            frmMessageBox f1 = new frmMessageBox();
                            f1.strMessage = s;
                            f1.strStatus = "Error";
                            f1.ShowDialog();
                        }
                    }

                    GET_BLACK_LIST();
                    frmMessageBox f = new frmMessageBox();
                    f.strMessage = "สำเร็จ";
                    f.strStatus = "Information";
                    f.ShowDialog();
                }
                catch
                {
                    //
                }
            }

 
        }

        private void button6_Click(object sender, EventArgs e)
        {
            classGlobal.MsgText = "ต้องการลบรายการที่เลือก?";
            Msg m = new Msg();
            m.ShowDialog();


            string ret = classGlobal.MsgConfirm;
            if (ret == "YES")
            {
                try
                {
                    if (int_Blacklist >= 0)
                    {
                        string query = String.Format("DELETE FROM " + current_BlackWhite_List + " WHERE ID = {0}", int_Blacklist);
                        if (classGlobal.databaseType == "acc")
                        {
                            OleDbCommand cmd = new OleDbCommand(query, classGlobal.conn);
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                            cmd = null;
                        }
                        else if (classGlobal.databaseType == "psql")
                        {
                            NpgsqlCommand cmd = new NpgsqlCommand(query, classGlobal.connP);
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                            cmd = null;
                        }
                        else
                        {
                            //string objectId = arrListId[selectedIndexOfObjectId].ToString();
                            string objectId = "";
                            string objectTimeId = "";
                            string[] sub = arrListTimeId[selectedIndexOfObjectId].ToString().Split(',');
                            objectId = sub[0];
                            objectTimeId = sub[1];

                            string s = "";
                            if (classGlobal.strBlackOrWhiteList == "black")
                            {
                                s = ClassData.DELETE_METHODE_BLACK_WHITE_LIST("blacklist", "blacklistId", "blacklistTimeId", objectId, objectTimeId);
                            }
                            else if (classGlobal.strBlackOrWhiteList == "white")
                            {
                                s = ClassData.DELETE_METHODE_BLACK_WHITE_LIST("whitelist", "whitelistId", "whitelistTimeId", objectId, objectTimeId);
                            }
                            if (s != "200")
                            {
                                frmMessageBox f1 = new frmMessageBox();
                                f1.strMessage = s;
                                f1.strStatus = "Error";
                                f1.ShowDialog();
                                return;
                            }
                        }
                        
                        GET_BLACK_LIST();
                        //MessageBox.Show("สำเร็จ");
                        frmMessageBox f = new frmMessageBox();
                        f.strMessage = "สำเร็จ";
                        f.strStatus = "Information";
                        f.ShowDialog();
                    }
                    else
                    {
                        //-- do nothing                       
                    }
                }
                catch
                {
                    //
                }
            }

        }

        int int_Blacklist = -1;
        int selectedIndexOfObjectId = -1;
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedIndexOfObjectId = -1;

            if (e.RowIndex < 0) { return; }
            try
            {
                int_Blacklist = Int32.Parse(dataGridView1.Rows[e.RowIndex].Cells[0].FormattedValue.ToString());
                string personalNo = dataGridView1.Rows[e.RowIndex].Cells[1].FormattedValue.ToString();
                string fullname = dataGridView1.Rows[e.RowIndex].Cells[2].FormattedValue.ToString();
                string start = dataGridView1.Rows[e.RowIndex].Cells[3].FormattedValue.ToString();
                string stop = dataGridView1.Rows[e.RowIndex].Cells[4].FormattedValue.ToString();

                textBox4.Text = personalNo;
                textBox5.Text = fullname;
                textBox6.Text = start;
                textBox7.Text = stop;

                selectedIndexOfObjectId = e.RowIndex;
            }
            catch
            {
                textBox4.Text = "";
                textBox5.Text = "";
                textBox6.Text = "";
                textBox7.Text = "";
                selectedIndexOfObjectId = -1;
            }
            
        }

 
        

    }
}
