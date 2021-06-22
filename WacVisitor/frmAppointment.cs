using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WacVisitor
{
    public partial class frmAppointment : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
     (
         int nLeftRect,     // x-coordinate of upper-left corner
         int nTopRect,      // y-coordinate of upper-left corner
         int nRightRect,    // x-coordinate of lower-right corner
         int nBottomRect,   // y-coordinate of lower-right corner
         int nWidthEllipse, // height of ellipse
         int nHeightEllipse // width of ellipse
     );

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        private static extern bool DeleteObject(System.IntPtr hObject);

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        static extern bool AnimateWindow(IntPtr hWnd, int time, AnimateWindowFlags flags);
        [Flags]
        enum AnimateWindowFlags
        {
            AW_HOR_POSITIVE = 0x00000001,
            AW_HOR_NEGATIVE = 0x00000002,
            AW_VER_POSITIVE = 0x00000004,
            AW_VER_NEGATIVE = 0x00000008,
            AW_CENTER = 0x00000010,
            AW_HIDE = 0x00010000,
            AW_ACTIVATE = 0x00020000,
            AW_SLIDE = 0x00040000,
            AW_BLEND = 0x00080000
        }
        public frmAppointment()
        {
            InitializeComponent();

            //this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.None;
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 10, 10));

            //this.Location = new Point(((Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2) + 10, 0);
            this.Location = new Point(((Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2) + 0, (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2);
            AnimateWindow(this.Handle, 100, AnimateWindowFlags.AW_CENTER);
        }

        DataTable dt;
        private void FrmAppointment_Load(object sender, EventArgs e)
        {
            classGlobal.appointMentSelectedId = "";
            dateTimePicker1.Value = new DateTime(
                                                        DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                                                        DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            comboBox1.SelectedIndex = 0;

            dt = ClassData.GET_APPOINTMENT();
            LOAD_LISTVIEW();
        }

        private void LOAD_LISTVIEW()
        {
            listView1.Items.Clear();
            listView1.View = View.Details;
            listView1.FullRowSelect = true;

            listView1.GridLines = true;
            listView1.OwnerDraw = true;

            listView1.DrawColumnHeader += new DrawListViewColumnHeaderEventHandler(listView1_DrawColumnHeader);
            listView1.DrawItem += new DrawListViewItemEventHandler(listView1_DrawItem);
            listView1.DrawSubItem += new DrawListViewSubItemEventHandler(listView1_DrawSubItem);
            // create column header
            ColumnHeader columnId = new ColumnHeader();
            ColumnHeader columnFullName = new ColumnHeader();
            ColumnHeader columnLicensePlate = new ColumnHeader();
            ColumnHeader columnAppointTime = new ColumnHeader();
            ColumnHeader columnAppointPlace = new ColumnHeader();
            ColumnHeader columnAppointStatus = new ColumnHeader();
            // column name
            columnId.Text = "_id";
            columnFullName.Text = "ชื่อ-นามสกุล";
            columnLicensePlate.Text = "ทะเบียนรถ";
            columnAppointTime.Text = "เวลานัดหมาย";
            columnAppointPlace.Text = "สถานที่นัดหมาย";
            columnAppointStatus.Text = "สถานะ";
            // column width
            columnId.Width = 0;
            columnFullName.Width = 220;
            columnLicensePlate.Width = 120;
            columnAppointTime.Width = 170;
            columnAppointPlace.Width = 150;
            columnAppointStatus.Width = 80;
            // title line setting
            ColumnHeader[] colHeaderRegValue = { columnId, columnFullName, columnLicensePlate, columnAppointTime, columnAppointPlace, columnAppointStatus };
            listView1.Columns.AddRange(colHeaderRegValue);
            // No sort function
            listView1.Sorting = SortOrder.None;
           
            if (dt != null)
            {
                string localTime = "";
                foreach (DataRow dr in dt.Rows)
                {
                    localTime = classGlobal.CONVERT_UTC_TO_LOCAL(dr[4].ToString());
                    string appStamp = localTime.Substring(0, 10);
                    string[] _date = appStamp.Split('-');
                    string dateSlash = _date[2] + "/" + _date[1] + "/" + (int.Parse(_date[0]) + 543).ToString();
                    string[] _time = localTime.Split(' ');
                    //appStamp = dateSlash + " " + _time[1];
                    appStamp = dateSlash + " " + _time[1].Substring(0, 5);

                    listView1.Items.Add(new ListViewItem(new string[] {
                    dr[0].ToString(),
                    dr[1].ToString(),
                    dr[3].ToString(),
                    appStamp,
                    dr[2].ToString(),
                    dr[5].ToString(),
                }));
                }
            }

            foreach (ListViewItem lvw in listView1.Items)
            {
                if (lvw.SubItems[5].Text == "นัด")
                    lvw.BackColor = Color.LightGreen;

                if (lvw.SubItems[5].Text == "มา")
                {
                    lvw.BackColor = Color.Blue;
                    lvw.ForeColor = Color.White;
                }

                if (lvw.SubItems[5].Text == "พบ")
                    lvw.BackColor = Color.Cyan;

                if (lvw.SubItems[5].Text == "ไม่พบ")
                    lvw.BackColor = Color.Orange;

                if (lvw.SubItems[5].Text == "ยกเลิก")
                {
                    lvw.BackColor = Color.Red;
                    lvw.ForeColor = Color.White;
                }

            }
        }
        private void listView1_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }
        private void listView1_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
        }
        private void listView1_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawBackground();
            switch (e.ColumnIndex)
            {
                case 0: // 0 column
                case 1: // 1 column
                case 2: // 2 columns
                case 3: // 3 columns
                case 4: // 4 columns
                case 5: // 5 columns
                        // Draw a square in the column header.
                    e.Graphics.FillRectangle(Brushes.LightSteelBlue, e.Bounds);
                    StringFormat sf = new StringFormat();
                    sf.LineAlignment = StringAlignment.Center;
                    sf.Trimming = StringTrimming.EllipsisCharacter;
                    sf.FormatFlags |= StringFormatFlags.NoWrap;
                    // Draw the column header string.
                    e.Graphics.DrawString(e.Header.Text, listView1.Font, Brushes.Black, e.Bounds, sf);
                    break;
                default:
                    e.DrawDefault = true;
                    break;
            }
        }


        private void Button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmAppointment_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void RbtnIN_Click(object sender, EventArgs e)
        {
            
            string name = textBox1.Text;
            string licensePlate = textBox2.Text;
            string appPlace = textBox3.Text;
            string appTime = dateTimePicker1.Value.Year + "-" + dateTimePicker1.Value.Month.ToString().PadLeft(2, '0') + "-" + dateTimePicker1.Value.Day.ToString().PadLeft(2, '0') + " " +
                                dateTimePicker1.Value.Hour.ToString().PadLeft(2, '0') + ":" + dateTimePicker1.Value.Minute.ToString().PadLeft(2, '0') + ":" + "00";

            appTime = classGlobal.CONVERT_LOCAL_TO_UTC(appTime);
            string appStatus = comboBox1.Text;

            JObject jo = new JObject();
            jo.Add("name", name);
            jo.Add("meetUpLocal", appPlace);
            jo.Add("licensePlate", licensePlate);
            jo.Add("daysToCome", appTime);
            jo.Add("status", appStatus);

            //string method = "";
            //if (classGlobal.appointMentSelectedId != "")   // update - put
            //    method = "PUT";
            //else   // insert - post
            //    method = "POST";

            JArray jaData = new JArray();
            jaData.Add(jo);           
            ClassData.POST_PUT_APPOINTMENT(jaData, classGlobal.appointMentSelectedId == "" ? "POST" : "PUT");  //===> POST SAVE TO SERVER 

            dt = ClassData.GET_APPOINTMENT();
            LOAD_LISTVIEW();

            classGlobal.appointMentSelectedId = "";

            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            comboBox1.SelectedIndex = 0;
            dateTimePicker1.Value = new DateTime(
                                            DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                                            DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            frmMessageBox f = new frmMessageBox();
            f.strMessage = "บันทึกสำเร็จ";
            f.strStatus = "Information";
            f.ShowDialog();
            
        }

        private void ListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {

                classGlobal.appointMentSelectedId = listView1.SelectedItems[0].SubItems[0].Text;

                textBox1.Text = listView1.SelectedItems[0].SubItems[1].Text;
                textBox2.Text = listView1.SelectedItems[0].SubItems[2].Text;
                textBox3.Text = listView1.SelectedItems[0].SubItems[4].Text;
                comboBox1.Text = listView1.SelectedItems[0].SubItems[5].Text;
                string[] spltSpace = listView1.SelectedItems[0].SubItems[3].Text.Split(' ');
                string[] spltDate = spltSpace[0].Split('/');
                string[] spltTime = spltSpace[1].Split(':');

                int dd = int.Parse(spltDate[0]);
                int MM = int.Parse(spltDate[1]);
                int yyyy = int.Parse(spltDate[2]) - 543;

                int HH = int.Parse(spltTime[0]);
                int mm = int.Parse(spltTime[1]);
                int ss = 0; 

                dateTimePicker1.Value = new DateTime(yyyy, MM, dd, HH, mm, ss);

            }

                
        }

        private void RoundButton1_Click(object sender, EventArgs e)
        {
            classGlobal.MsgText = "ต้องการลบรายการที่เลือก?";
            Msg m = new Msg();
            m.ShowDialog();
            string ret = classGlobal.MsgConfirm;
            if (ret.ToLower() == "yes")
            {
                Console.Write(classGlobal.appointMentSelectedId);

                foreach (JObject js in ClassData.jaAppointment)
                {
                    if (js["meetingId"].ToString() == classGlobal.appointMentSelectedId)
                    {
                        ClassData.jaAppointment.Remove(js);
                        break;
                    }
                }

                ClassData.DELETE_APPONITMENT(classGlobal.appointMentSelectedId);

                dt = ClassData.GET_APPOINTMENT();
                LOAD_LISTVIEW();

            }
            else
            { /* */ }
        }

        private void RoundButton2_Click(object sender, EventArgs e)
        {
            classGlobal.appointMentSelectedId = "";
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            comboBox1.SelectedIndex = 0;
            dateTimePicker1.Value = new DateTime(
                                            DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                                            DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);



        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Control c = Control.FromHandle(msg.HWnd);
            if (keyData == Keys.F5)
            {
                dt = ClassData.GET_APPOINTMENT();
                LOAD_LISTVIEW();
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
