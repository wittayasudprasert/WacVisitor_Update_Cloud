using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WacVisitor
{
    public partial class MsgLongInfo : Form
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

        DataTable dt;
        bool onload = false;
        public MsgLongInfo(DataTable _dt)
        {
            InitializeComponent();
            this.TopMost = true;                       
            this.FormBorderStyle = FormBorderStyle.None;
            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 10, 10));

            //this.Location = new Point(((Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2) + 10, 0);
            this.Location = new Point(((Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2) + 0, (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2);
            AnimateWindow(this.Handle, 100, AnimateWindowFlags.AW_CENTER);

            dt = _dt.Copy();
            classGlobal.appointMentSelectedId = "";            
        }
        
        private void MsgLongInfo_Load(object sender, EventArgs e)
        {
            onload = true;

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
            // column name
            columnId.Text = "_id";
            columnFullName.Text = "ชื่อ-นามสกุล";
            columnLicensePlate.Text = "ทะเบียนรถ";
            columnAppointTime.Text = "เวลานัดหมาย";
            columnAppointPlace.Text = "สถานที่นัดหมาย";
            // column width
            columnId.Width = 0;
            columnFullName.Width = 220;
            columnLicensePlate.Width = 120;
            columnAppointTime.Width = 170;
            columnAppointPlace.Width = 150;
            // title line setting
            ColumnHeader[] colHeaderRegValue = { columnId, columnFullName, columnLicensePlate, columnAppointTime, columnAppointPlace };
            listView1.Columns.AddRange(colHeaderRegValue);
            // No sort function
            listView1.Sorting = SortOrder.None;

            foreach (DataRow dr in dt.Rows)
            {
                listView1.Items.Add(new ListViewItem(new string[] {
                    dr[0].ToString(),
                    dr[1].ToString(),
                    dr[2].ToString(),
                    dr[3].ToString(),
                    dr[4].ToString()
                }));
            }

            //if (listView1.Items.Count == 1)
            //    listView1.Items[0].Selected = true;

            onload = false;

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
            Console.Write(classGlobal.appointMentSelectedId);
            this.Close();
        }

        private void MsgLongInfo_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void ListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            classGlobal.appointMentSelectedId = "";

            if (listView1.SelectedItems.Count > 0)
            {
                classGlobal.appointMentSelectedId = listView1.SelectedItems[0].SubItems[0].Text;
            }
                

            if (onload==false)
                this.Close();
        }
    }
}
