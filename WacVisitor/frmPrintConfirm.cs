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
    public partial class frmPrintConfirm : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private string strInformation = "";
        public bool boolConfirm { get; set; }
        public frmPrintConfirm(string data)
        {
            InitializeComponent();

            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.WindowState = FormWindowState.Normal;

            //if (classGlobal.userId != "")
            //{
            //    strInformation = data;
            //    string[] arrInfo = strInformation.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            //    List<String> lst = new List<string>();
            //    lst.Add(arrInfo[3]);
            //    lst.Add(arrInfo[4]);
            //    lst.Add(arrInfo[5]);
            //    lst.Add(arrInfo[6]);
            //    lst.Add(arrInfo[7]);
            //    lst.Add(arrInfo[8] + " " + clsInfo.info_place);
            //    lst.Add(arrInfo[9] + " " + clsInfo.info_license_plate);
            //    lst.Add(arrInfo[10] + " " + clsInfo.info_vehicle_type);
            //    lst.Add(arrInfo[11] + " " + clsInfo.info_etc);
            //    arrInfo = lst.ToArray();
            //    strInformation = String.Join(Environment.NewLine, arrInfo);
            //}
            //else
            //{
            //    strInformation = data;
            //    string[] arrInfo = strInformation.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            //    List<String> lst = new List<string>();
            //    for (int i = 3; i < arrInfo.Length; i++)
            //        lst.Add(arrInfo[i]);

            //    arrInfo = lst.ToArray();
            //    strInformation = String.Join(Environment.NewLine, arrInfo);
            //}

            strInformation = data;
            string[] arrInfo = strInformation.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            List<String> lst = new List<string>();
            for (int i = 3; i < arrInfo.Length; i++)
                lst.Add(arrInfo[i]);

            arrInfo = lst.ToArray();
            strInformation = String.Join(Environment.NewLine, arrInfo);
        }

        private void FrmPrintConfirm_Load(object sender, EventArgs e)
        {
            boolConfirm = false;
            label2.Text = strInformation;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            boolConfirm = true;
            this.Close();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            boolConfirm = false;
            this.Close();
        }

        private void Button10_Click(object sender, EventArgs e)
        {
            boolConfirm = false;
            this.Close();
        }

        private void Label1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}
