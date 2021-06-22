using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace WacVisitor.FormSetting
{
    public partial class FormPreviewIN : Form
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

        string t1 = "";
        string[] t2 = new string[0];
        string t3 = "";
        string t4 = "";
        string t5 = "";
        public FormPreviewIN(string txt1, string[] txt2, string txt3, string txt4, string txt5)
        {
            InitializeComponent();

            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 10, 10));

            t1 = txt1;
            t2 = txt2;
            t3 = txt3;
            t4 = txt4;
            t5 = txt5;
        }

        private void FormPreviewIN_Load(object sender, EventArgs e)
        {
            classGlobal.PreviewPrintIN = "";

            if (clsInfo.info_follower == null || clsInfo.info_follower == "")
                clsInfo.info_follower = "1";

            label1.Text = classGlobal.strPlace.ToString();
            label2.Text = "ใบผ่านเข้า-ออก  ผู้มาติดต่อ  หมายเลข : " + t1;
            label3.Text = "เวลาเข้า : " + t2[1] + "  วันที่ : " + t2[0];
            label4.Text = "ประเภทผู้มาติดต่อ : " + t3 + "     " + "จำนวน  " + clsInfo.info_follower + "   คน";
            label5.Text = "ชื่อ-สกุล : " + t4;
            label6.Text = "เลขประจําตัว : " + t5;
            label7.Text = "ทะเบียนรถ : " + clsInfo.info_license_plate.ToString() + "     " + "ประเภทรถ : " + clsInfo.info_vehicle_type;
            label8.Text = "จากบริษัท : " + clsInfo.info_visitor_company.ToString();            
            label9.Text = "ติดต่อเรื่อง : " + clsInfo.info_business_topic.ToString();         
            label10.Text = "ผู้รับการติดต่อ : " + clsInfo.info_visit_to;
            label11.Text = "แผนกที่ติดต่อ : " + clsInfo.info_department;
            label12.Text = "สถานที่ติดต่อ : " + clsInfo.info_place.ToString(); 
            label13.Text = "ข้อมูลอื่นๆ : " + clsInfo.info_etc;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            classGlobal.PreviewPrintIN = "Y";
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            classGlobal.PreviewPrintIN = "N";
            this.Close();
        }
    }
}
