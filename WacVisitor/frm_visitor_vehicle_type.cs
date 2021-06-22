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
    public partial class frm_visitor_vehicle_type : Form
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


        public string selected_type { get; set; }
        public frm_visitor_vehicle_type()
        {
            InitializeComponent();
        }

        private void Frm_visitor_vehicle_type_Load(object sender, EventArgs e)
        {
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 10, 10));
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            selected_type = "";
            this.Close();

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            selected_type = "tbl_type";
            this.Close();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            selected_type = "tbl_vehicle";
            this.Close();
        }
    }
}
