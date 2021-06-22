using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace WacVisitor
{
    public partial class frmExit : Form
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

        public frmExit()
        {
            InitializeComponent();

            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 10, 10));
        }

        private void frmExit_Load(object sender, EventArgs e)
        {            
            this.Text = "EXIT";
        }

        private void button11_Click(object sender, EventArgs e)
        {
            classGlobal.bool_Exit = true;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            classGlobal.bool_Exit = false; 
            this.Close();
        }

        private void frmExit_MouseMove(object sender, MouseEventArgs e)
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
            if (keyData == Keys.Escape)
            {
                classGlobal.bool_Exit = false;
                this.Close();
            }

            if (keyData == Keys.Enter)
            {
                classGlobal.bool_Exit = true;
                this.Close();
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void myButton1_MouseLeave(object sender, EventArgs e)
        {
            myButton1.BackColor = Color.FromArgb(0, 0, 192); 
        }

        private void myButton1_MouseMove(object sender, MouseEventArgs e)
        {
            myButton1.BackColor = Color.LightGray;
        }
    }
}
