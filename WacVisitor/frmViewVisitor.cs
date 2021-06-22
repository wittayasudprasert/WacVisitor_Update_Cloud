using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace WacVisitor
{
    public partial class frmViewVisitor : Form
    {

        #region Reduce Memmory
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetProcessWorkingSetSize(IntPtr process,
            UIntPtr minimumWorkingSetSize, UIntPtr maximumWorkingSetSize);
        #endregion

        
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        int pageCurrent = 1;
        int pageSize = 4;  
        double pageTotal = 0;
        int pageAll = 0;

        public frmViewVisitor()
        {
            InitializeComponent();
          
            if (classGlobal.dtVisitor.Rows.Count % pageSize == 0)
            {
                pageAll = classGlobal.dtVisitor.Rows.Count / pageSize;
            }
            else
            {
                pageTotal = classGlobal.dtVisitor.Rows.Count / pageSize;
                pageAll = (int)Math.Floor(pageTotal) + 1;
            }

        }
        
        private void frmViewVisitor_Load(object sender, EventArgs e)
        {
            cmbPage.Visible = false;
            label1.Visible = false;

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            panelPage.Left = (panel2.Width - panelPage.Size.Width) / 2;
            button3.Left = panelPage.Left + panelPage.Size.Width + 15;


            button1.Enabled = false;
            if (pageAll == 1)
            {
                button1.Enabled = false;
                button2.Enabled = false;
            }


            dataGridView1.Visible = false;
            frmTopMost f = new frmTopMost();
            f.Show();

            DataTable dt = classGlobal.dtVisitor;

            DataView dv = dt.DefaultView;
            if (classGlobal.userId != "")
                dv.Sort = "recordTimeIn " + classGlobal.sortOrderBy;
            else
                dv.Sort = "status_in " + classGlobal.sortOrderBy;

            dt = dv.ToTable();

            dt = Populate(pageCurrent);
            DisplayDataGridView(dt);

            f.Close();
            dataGridView1.Visible = true;


            cmbPage.Visible = true;
            label1.Visible = true;
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
                c.DefaultCellStyle.Font = new Font("Segoe UI", 20F, FontStyle.Bold, GraphicsUnit.Pixel);                
            }
        }


        private DataTable Populate(int pgNo)
        {
            DataSet ds = null;
            DataTable dt = null;

            string query = classGlobal.pub_query;

            if (classGlobal.databaseType == "acc")
            {
                OleDbDataAdapter adapter = new OleDbDataAdapter(query, classGlobal.conn);
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
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, classGlobal.connP);
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
                dt = classGlobal.dtVisitor.Copy();

                DataView dv = dt.DefaultView;
                if (classGlobal.userId != "")
                    dv.Sort = "recordTimeIn " + classGlobal.sortOrderBy;
                else
                    dv.Sort = "status_in " + classGlobal.sortOrderBy;
                dt = dv.ToTable();

                DataView dataView = new DataView(dt);
                dt = dataView.ToTable();
                while (dt.Rows.Count > pageSize)
                {
                    dt = dt.AsEnumerable().Skip((pgNo - 1) * pageSize).Take(pageSize).CopyToDataTable();
                }

            }


            if (classGlobal.view_status != "STILL")
            {
                if (dt != null && dt.Rows.Count > 0)
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

            return dt;
        }
        private void DisplayDataGridView(DataTable dt)
        {
            if (dt == null)
                dt = new DataTable("dt");

            dataGridView1.Rows.Clear(); 

            //+++ Header Column
            dataGridView1.RowTemplate.Resizable = DataGridViewTriState.True;
            dataGridView1.RowTemplate.Height = 50;
            dataGridView1.RowHeadersVisible = false;

            dataGridView1.Columns[0].Width = (this.ClientSize.Width / 6) - 8; 
            dataGridView1.Columns[1].Width = (this.ClientSize.Width / 6) - 8; 
            dataGridView1.Columns[2].Width = (this.ClientSize.Width / 6) - 8; 
            dataGridView1.Columns[3].Width = (this.ClientSize.Width / 6) - 8; 
            dataGridView1.Columns[4].Width = (this.ClientSize.Width / 6) - 8; 
            dataGridView1.Columns[5].Width = (this.ClientSize.Width / 6) - 8; 

            dataGridView1.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;

            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.SystemColors.Highlight;
            dataGridView1.EnableHeadersVisualStyles = false;

            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.HeaderCell.Style.Font = new Font("Segoe UI", 30F, FontStyle.Bold, GraphicsUnit.Pixel);

                col.HeaderCell.Style.ForeColor = Color.White;
            }
            //--

            string strIN;
            string strOUT;
            int beYearFormat = 0;
            string[] arr1;
            string[] arr2;
            int rows = 0;
            DataGridViewRow row;
            Bitmap bmp;
            foreach (DataRow dr in dt.Rows)
            {
                strIN = dr.ItemArray[6].ToString();
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

                strOUT = dr.ItemArray[7].ToString();
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

                row = new DataGridViewRow();

                row.Cells.Add(new DataGridViewTextBoxCell { Value = dr.ItemArray[9].ToString() });   
                row.Cells.Add(new DataGridViewTextBoxCell { Value = dr.ItemArray[2].ToString() });
                row.Cells.Add(new DataGridViewTextBoxCell { Value = strIN });
                row.Cells.Add(new DataGridViewTextBoxCell { Value = strOUT });

                if (dr.ItemArray[4].ToString() != "")
                {
                    if (classGlobal.userId != "")
                    {
                        try
                        {
                            string base64image = ClassData.DOWNLOAD_IMAGE(dr.ItemArray[4].ToString(), 130, 80);   
                            bmp = new Bitmap((Bitmap)Base64ToImage(base64image), 130, 80); 
                        }  
                        catch
                        {
                            bmp = new Bitmap(classGlobal.unknown);
                            bmp = new Bitmap(bmp, 100, 80);
                        }
                       
                    }
                    else
                    {
                        bmp = new Bitmap((Bitmap)Base64ToImage(dr.ItemArray[4].ToString()), 130, 80);  
                    }
                }
                else
                {
                    bmp = new Bitmap(classGlobal.unknown);
                    bmp = new Bitmap(bmp, 100, 80);
                }

                if (classGlobal.DisplayHashTag == true)
                {
                    row.Cells.Add(new DataGridViewImageCell { Value = classGlobal.PLACEWATERMARK_FROM_BITMAP(bmp) });
                }
                else
                {
                    //row.Cells.Add(new DataGridViewImageCell { Value = bmp });
                    row.Cells.Add(new DataGridViewImageCell { Value = classGlobal.PLACEWATERMARK_FROM_BITMAP(bmp) });
                }

                //++ card image
                if (dr.ItemArray[3].ToString() != "")
                {
                    Bitmap b = null;
                    if (classGlobal.userId != "")
                    {
                        try
                        {
                            string base64image = ClassData.DOWNLOAD_IMAGE(dr.ItemArray[3].ToString(), 130, 80);
                            b = new Bitmap((Bitmap)Base64ToImage(base64image), 130, 80);
                        }
                        catch
                        {
                            bmp = new Bitmap(classGlobal.unknown);
                            b = new Bitmap(bmp, 100, 80);
                        }                        
                    }
                    else
                    {
                        b = (Bitmap)Base64ToImage(dr.ItemArray[3].ToString());
                    }

                    
                    if (b.Width < b.Height)
                    {
                        bmp = new Bitmap((Bitmap)Base64ToImage(dr.ItemArray[3].ToString()), 100, 120);
                    }
                    else
                    {
                        if (classGlobal.userId != "")
                        {
                            try
                            {
                                string base64image = ClassData.DOWNLOAD_IMAGE(dr.ItemArray[3].ToString(), 130, 80);
                                bmp = new Bitmap((Bitmap)Base64ToImage(base64image), 130, 80);
                            }
                            catch
                            {
                                bmp = new Bitmap(classGlobal.unknown);
                                bmp = new Bitmap(bmp, 100, 80);
                            }                            
                        }
                        else
                        {
                            bmp = new Bitmap((Bitmap)Base64ToImage(dr.ItemArray[3].ToString()), 130, 80);
                        }
                        
                    }
                    b.Dispose();
                    b = null;

                }
                else
                {
                    bmp = new Bitmap(classGlobal.unknown);
                    bmp = new Bitmap(bmp, 100, 80);
                }

                if (classGlobal.DisplayHashTag == true)
                {
                    row.Cells.Add(new DataGridViewImageCell { Value = classGlobal.PLACEWATERMARK_FROM_BITMAP(bmp) });
                }
                else
                {
                    //row.Cells.Add(new DataGridViewImageCell { Value = bmp });
                    row.Cells.Add(new DataGridViewImageCell { Value = classGlobal.PLACEWATERMARK_FROM_BITMAP(bmp) });
                }
                //-- card image

                dataGridView1.Rows.Add(row);

                rows = rows + 1;
            }

            //++ Set no selected on first record
            dataGridView1.Columns[1].DefaultCellStyle.SelectionForeColor = Color.Black;
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.White;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;
            dataGridView1.BackgroundColor = System.Drawing.SystemColors.Highlight;
            //--

            UpdateFont();
            dataGridView1.AllowUserToAddRows = false;

            lbPage.Text = pageCurrent.ToString() + "/" + pageAll.ToString();

            if (dt.Rows.Count == 0)
            {
                lbPage.Text = "0/0";
                button1.Enabled = false;
                button2.Enabled = false;                
            }

            minimizeMemory();   
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

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            dataGridView1.Rows[e.RowIndex].Height = 120;
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {
            //
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

            DataTable dt = classGlobal.dtVisitor;
            dt = Populate(pageCurrent);
            DisplayDataGridView(dt);
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

            DataTable dt = classGlobal.dtVisitor;
            dt = Populate(pageCurrent);
            DisplayDataGridView(dt);
        }

        private static void minimizeMemory()
        {
            GC.Collect(GC.MaxGeneration);
            GC.WaitForPendingFinalizers();
            SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, (UIntPtr)0xFFFFFFFF, (UIntPtr)0xFFFFFFFF);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DataTable dt = classGlobal.dtVisitor;
            dt = Populate(0);
            DisplayDataGridView(dt);
        }

        private string GetVisitorTypeName(int typeid)
        {
            try
            {


                return "";
            }
            catch
            {
                return "";
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                string[] spl = new string[0];
                string[] spl_slash = new string[0];

                string visitorNumber = dataGridView1.Rows[e.RowIndex].Cells[1].FormattedValue.ToString();
                string checkIn =  dataGridView1.Rows[e.RowIndex].Cells[2].FormattedValue.ToString().Replace("-","");
                string checkOut = dataGridView1.Rows[e.RowIndex].Cells[3].FormattedValue.ToString().Replace("-", "");

                if (checkIn != "")
                {
                    spl = checkIn.Split(' ');
                    spl_slash = spl[0].Split('/');
                    checkIn = (Int32.Parse(spl_slash[2].ToString()) - 543).ToString() + "-" + spl_slash[1].ToString() + "-" + spl_slash[0].ToString() + " " + spl[1];
                }
                else
                {
                    checkIn = "";
                }

                if (checkOut != "")
                {
                    spl = checkOut.Split(' ');
                    spl_slash = spl[0].Split('/');
                    checkOut = (Int32.Parse(spl_slash[2].ToString()) - 543).ToString() + "-" + spl_slash[1].ToString() + "-" + spl_slash[0].ToString() + " " + spl[1];
                }
                else
                {
                    checkOut = "";
                }

                DataTable dtID = new DataTable("ID");

                if (classGlobal.databaseType == "acc")
                {
                    OleDbDataAdapter adapter = new OleDbDataAdapter(String.Format("SELECT id FROM tbl_visitor WHERE status_in='{0}' AND status_out='{1}'", checkIn, checkOut), classGlobal.conn);
                    adapter.Fill(dtID);
                    adapter.Dispose();
                    adapter = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(String.Format("SELECT id FROM tbl_visitor WHERE status_in='{0}' AND status_out='{1}'", checkIn, checkOut), classGlobal.connP);
                    adapter.Fill(dtID);
                    adapter.Dispose();
                    adapter = null;
                }
                else
                {
                    classGlobal.dtVisitor = classGlobal.dtVisitor;

                    dtID.Columns.Add("id");
                    dtID.Rows.Add("0");

                    DataRow[] getVisitorId = classGlobal.dtVisitor.Select("card_number='" + visitorNumber + "'" + " AND " +
                                                                                "status_in='" + checkIn + "'" + " AND " +
                                                                                    "status_out='" + checkOut + "'");

                    classGlobal.public_visitorId = getVisitorId[0]["visitorId"].ToString(); 
                }

                int id = Int32.Parse (dtID.Rows[0][0].ToString());
                classGlobal.pub_id = id;

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

        private string CHECKOUT(string card_number, string str_datetime)
        {
            try
            {

                OleDbCommand command;
                OleDbDataReader reader;

                NpgsqlCommand commandP;
                NpgsqlDataReader readerP;

                string query;
                int id = 0;
                query = String.Format("SELECT id FROM tbl_visitor WHERE card_number ='{0}' AND status_in <> '' AND (status_out ='' OR status_out IS NULL)",
                                card_number);

                if (classGlobal.databaseType == "acc")
                {
                    command = new OleDbCommand(query, classGlobal.conn);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        id = Int32.Parse("0" + reader.GetValue(0).ToString());
                    }
                    reader.Close();
                    command.Dispose();
                    command = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    commandP = new NpgsqlCommand(query, classGlobal.connP);
                    readerP = commandP.ExecuteReader();
                    while (readerP.Read())
                    {
                        id = Int32.Parse("0" + readerP.GetValue(0).ToString());
                    }
                    readerP.Close();
                    commandP.Dispose();
                    commandP = null;
                }
                else
                {

                }

                if (id == 0) { return "false"; }

                query = String.Format("UPDATE tbl_visitor SET status_out ='{0}' WHERE id={1}", str_datetime, id);

                if (classGlobal.databaseType == "acc")
                {
                    command = new OleDbCommand(query, classGlobal.conn);
                    command.ExecuteNonQuery();
                    command.Dispose();
                    command = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    commandP = new NpgsqlCommand(query, classGlobal.connP);
                    commandP.ExecuteNonQuery();
                    commandP.Dispose();
                    commandP = null;
                }
                else
                {

                }

                try
                {
                    //++ UPDATE MACADDRESS FOR OUT STATUS
                    //query = String.Format("UPDATE tbl_visitor SET mac_checkout='{0}' WHERE id={1}", classGlobal.MachineAddress, id);
                    query = String.Format("UPDATE tbl_visitor SET mac_checkout='{0}' WHERE id={1}", classGlobal.strPlace, id);
                    if (classGlobal.databaseType == "acc")
                    {
                        command = new OleDbCommand(query, classGlobal.conn);
                        command.ExecuteNonQuery();
                        command.Dispose();
                        command = null;
                    }
                    else if (classGlobal.databaseType == "psql")
                    {
                        commandP = new NpgsqlCommand(query, classGlobal.connP);
                        commandP.ExecuteNonQuery();
                        commandP.Dispose();
                        commandP = null;
                    }
                    else
                    {

                    }
                    //--
                }
                catch
                {
                    //--
                }

                try
                {
                    //++ UPDATE สถานะการ Upload ให้กลับเป็น 0 
                    query = String.Format("UPDATE tbl_visitor SET upload ='{0}' WHERE id={1}", "0", id);
                    if (classGlobal.databaseType == "acc")
                    {
                        command = new OleDbCommand(query, classGlobal.conn);
                        command.ExecuteNonQuery();
                        command.Dispose();
                        command = null;
                    }
                    else if (classGlobal.databaseType == "psql")
                    {
                        commandP = new NpgsqlCommand(query, classGlobal.connP);
                        commandP.ExecuteNonQuery();
                        commandP.Dispose();
                        commandP = null;
                    }
                    else
                    {

                    }
                    //--
                }
                catch
                {
                    //--
                }

                DataTable dtResult = new DataTable("dtResult");
                string strReturn = "";
                string para = "";
                query = "SELECT  t1.card_number, t1.str_imagedocument, t1.str_imagewebcamera, t1.status_in, t1.status_out, t2.typename " +
                                            "FROM tbl_visitor t1 INNER JOIN tbl_type t2 ON t1.typeid = t2.typeid " +
                                            "WHERE id=" + id;

                if (classGlobal.databaseType == "acc")
                {
                    command = new OleDbCommand(query, classGlobal.conn);
                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, classGlobal.conn);
                    adapter.Fill(dtResult);
                    adapter.Dispose();
                    adapter = null;
                    command.Dispose();
                    command = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    commandP = new NpgsqlCommand(query, classGlobal.connP);
                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, classGlobal.connP);
                    adapter.Fill(dtResult);
                    adapter.Dispose();
                    adapter = null;
                    commandP.Dispose();
                    commandP = null;
                }
                else
                {

                }


                foreach (DataRow dr in dtResult.Rows)
                {
                    para = dr.ItemArray[0].ToString() + "@" +
                            dr.ItemArray[1].ToString() + "@" +
                              dr.ItemArray[2].ToString() + "@" +
                                dr.ItemArray[3].ToString() + "@" +
                                  dr.ItemArray[4].ToString() + "@" +
                                    dr.ItemArray[5].ToString();
                }
                dtResult.Dispose();
                dtResult = null;

                strReturn = para;

                return strReturn;
            }
            catch
            {
                return "false";
            }
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

        private void CmbPage_SelectedIndexChanged(object sender, EventArgs e)
        {

            pageCurrent = Int32.Parse(cmbPage.SelectedItem.ToString());
            DataTable dt = classGlobal.dtVisitor;
            dt = Populate(pageCurrent);
            DisplayDataGridView(dt);
        }
    }
}
