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
    public partial class frmMessageBox : Form
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

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        private static extern bool DeleteObject(System.IntPtr hObject);

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public string strMessage { get; set; }
        public string strStatus { get; set; }

        public frmMessageBox()
        {
            InitializeComponent();

            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;      
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 10, 10));

            this.ActiveControl = button1;
        }

        private void frmMessageBox_Load(object sender, EventArgs e)
        {
            
            if (this.strStatus != "Information")
            {                
                button2.ForeColor = Color.Red;  
            }
            else
            {
                button2.ForeColor = Color.White;  
            }
            button2.Text = this.strMessage;  
           
        }

        private void frmMessageBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

 

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Close(); 
        }

        private void button1_Paint(object sender, PaintEventArgs e)
        {
            //IntPtr ptr = CreateRoundRectRgn(0, 0, button1.Width, button1.Height, 40, 40);
            //button1.Region = Region.FromHrgn(ptr);
            //button1.FlatStyle = FlatStyle.Flat;
            //button1.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(222)));
            //button1.FlatAppearance.BorderSize = 0;
            //button1.ForeColor = System.Drawing.SystemColors.HotTrack;
            //button1.ForeColor = System.Drawing.Color.Blue;
            //button1.BackColor = System.Drawing.SystemColors.ControlDark;
            //DeleteObject(ptr);
        }
    }
}
