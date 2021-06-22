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
    public partial class frmAgeForPub : Form
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

        public string strStatus { get; set; }
        public string citizenId { get; set; }
        public string citizenAge { get; set; }
        public string citizenName { get; set; }
        public string citizenCheckIn { get; set; }
        public string base64PhotoImage { get; set; }
        public string base64CameraImage { get; set; }

        public frmAgeForPub()
        {
            InitializeComponent();

            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
 
            this.Location = new Point(((Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2) + 0, (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2);
            AnimateWindow(this.Handle, 100, AnimateWindowFlags.AW_CENTER);

            pictureBox1.Image = Image.FromFile(@"icon\unknown.png");
            pictureBox2.Image = Image.FromFile(@"icon\unknown.png");

        }

        private void FrmAgeForPub_Load(object sender, EventArgs e)
        {
            ClassHelper.clsImageProcessing c = new ClassHelper.clsImageProcessing();

            byte[] bytesPhoto = Convert.FromBase64String(base64PhotoImage);
            byte[] bytesCamera = Convert.FromBase64String(base64CameraImage);

            if (bytesPhoto.Length > 0)
            {
                bytesPhoto = classGlobal.PLACEWATERMARK_FROM_BYTE(bytesPhoto);
                pictureBox1.Image = c.ByteToImage(bytesPhoto);
            }                

            if (bytesCamera.Length > 0)
            {
                bytesCamera = classGlobal.PLACEWATERMARK_FROM_BYTE(bytesCamera);
                pictureBox2.Image = c.ByteToImage(bytesCamera);
            }


            label1.Text = citizenName; //+ "   " + "อายุ " + citizenAge + " ปี";
            label3.Text = "อายุ " + citizenAge + " ปี";

            if (this.strStatus != "Information")   // เข้าซ้ำ
            {
                label2.Text = "เข้าเมื่อเวลา : " + citizenCheckIn;
                this.Size = new Size(700, 660);
            }                
            else   // ไม่เข้าซ้ำ
            {
                this.Size = new Size(700, 330);
                pictureBox2.Visible = false;
                label2.Text = "";
            }

            this.Location = new Point(((Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2) + 0, (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2);

        }

        private void FrmAgeForPub_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmAgeForPub_Resize(object sender, EventArgs e)
        {
            //
        }

        private void Label1_Click(object sender, EventArgs e)
        {
            //
        }
    }
}
