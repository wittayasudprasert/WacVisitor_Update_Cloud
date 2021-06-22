using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WacVisitor.FormSetting
{
    public partial class FormPreviewSlip : Form
    {

        string strCharge = "";
        string vCardNumber = "";
        string license_plate = "";
        string checkin = "";
        string checkout = "";
        string workHours = "";


        //string sFreeTimeInfo = "";
        public FormPreviewSlip(string t1,string t2,string t3, string t4, string t5, string t6)
        {
            InitializeComponent();

            strCharge = t1;
            vCardNumber = t2;
            license_plate = t3;
            checkin = t4;   //เข้า   03/12/2562 12:48:55
            checkout = t5;  //ออก   03/12/2562 14:33:34
            workHours = t6; //เวลาจอดทั้งหมด   01:45 ชั่วโมง
        }

        private void FormPreviewSlip_Load(object sender, EventArgs e)
        {
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.WindowState = FormWindowState.Normal;


            label3.Text ="ค่าบริการจอดรถ " + strCharge + " บาท";
            label3.Left = (this.Width - label3.ClientSize.Width) / 2;

            string[] arr = new string[0] { };
            
            arr = checkin.Split('/');
            arr[1] = classGlobal.NUMBER_TO_MONTH(arr[1]);
            checkin = String.Join(" ", arr);

            arr = checkout.Split('/');
            arr[1] = classGlobal.NUMBER_TO_MONTH(arr[1]);
            checkout = String.Join(" ", arr); 

            lb1.Text = "สรุปเวลาการจอดรถของ Visitor No. " + vCardNumber;
            lb2.Text = "ทะเบียนรถ " + license_plate;
            lb3.Text = checkin;
            lb4.Text = checkout;

            lb5.Text = workHours;


            label6.Text = "วันที่พิมพ์ " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            label6.Left = (this.Width - label3.ClientSize.Width);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {        
            bool prnStatus = classGlobal.CheckPOC_ONLINE(classGlobal.printThermalName);
            if (prnStatus == true)
                PRINT_SLIP();

            this.Close();
        }

        private void PRINT_SLIP()
        {

            System.Drawing.Printing.PrintDocument p = new System.Drawing.Printing.PrintDocument();
            p.PrintPage += delegate(object sender1, System.Drawing.Printing.PrintPageEventArgs e1)
            {
                StringFormat AlignmentCenter = new StringFormat() { Alignment = StringAlignment.Center };
                StringFormat AlignmentRight = new StringFormat() { Alignment = StringAlignment.Far };
                StringFormat AlignmentLeft = new StringFormat() { Alignment = StringAlignment.Near };

                //++ โลโก้
                Bitmap bmpLogo = new Bitmap(@"logo.png");
                bmpLogo = classGlobal.ResizeImageRatio(bmpLogo, classGlobal.pubWidth, classGlobal.pubHeight);
                e1.Graphics.DrawImage(bmpLogo, new RectangleF((p.DefaultPageSettings.PrintableArea.Width / 2) - (bmpLogo.Width / 2), 0, bmpLogo.Width, bmpLogo.Height));

                int bottomBmp = Int32.Parse(classGlobal.pubHeight.ToString()) + 5; 
                //-- โลโก้

                var rect = new RectangleF(0, bottomBmp, p.DefaultPageSettings.PrintableArea.Width, p.DefaultPageSettings.PrintableArea.Height);
                //++ ข้อความ
                classGlobal.text += "(เศษของ 1 ชั่วโมงคิดเป็น 1 ชั่วโมง)" + "\n";
                classGlobal.text += "*** ขอบคุณที่ใช้บริการ ***" + "\n";
                e1.Graphics.DrawString(classGlobal.text, new Font("Tahoma", 10), new SolidBrush(Color.Black), rect, AlignmentCenter);
                bottomBmp = Int32.Parse(classGlobal.pubHeight.ToString()) + 250;   //** 250 ประมาณความสูงของข้อความทุกบรรทัดรวมกัน
                //-- ข้อความ

                //++ บรรทัด timestamp
                rect = new RectangleF(0, bottomBmp, p.DefaultPageSettings.PrintableArea.Width, p.DefaultPageSettings.PrintableArea.Height);
                e1.Graphics.DrawString("วันที่พิมพ์ " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), new Font("Tahoma", 10), new SolidBrush(Color.Black), rect, AlignmentRight);
                //-- บรรทัด timestamp
            };

            try
            {
                ////++ โชว์ Dialog 
                //PrintPreviewDialog pPrev = new PrintPreviewDialog();
                //pPrev.Document = p;
                //pPrev.ShowDialog();
                ////--              

                p.Print();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception Occured While Printing", ex);
            }
        }
    }
}
