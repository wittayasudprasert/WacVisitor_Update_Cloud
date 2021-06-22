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
    public partial class frmLogOff : Form
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

        public bool cancel { get; set; }
        public frmLogOff()
        {
            InitializeComponent();

            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            //Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 10, 10));
            this.cancel = false;
        }
        //protected override void OnLoad(EventArgs e)
        //{
        //    var screen = Screen.FromPoint(this.Location);
        //    this.Location = new Point(screen.WorkingArea.Right - Size.Width, screen.WorkingArea.Bottom - Size.Height);
        //    this.Location = new Point(0, 0);
        //    base.OnLoad(e);
        //}
        private void FrmLogOff_Load(object sender, EventArgs e)
        {
            this.Text = "LOGOFF / EXIT";
        }

        private void Button11_Click(object sender, EventArgs e)
        {
            this.cancel = false;
            classGlobal.bool_Exit = true;
            this.Close();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.cancel = false;
            classGlobal.bool_Exit = false;
            this.Close();
        }

        private void FrmLogOff_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.cancel = true;
            classGlobal.bool_Exit = false;
            this.Close();
        }

        private void Button1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void Button11_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void Button2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}
