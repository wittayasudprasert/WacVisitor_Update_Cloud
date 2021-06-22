using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Reflection;
using System.IO;
using ClassHelper;

namespace WacVisitor
{
    public partial class frmAuthen : Form
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

        private const int EM_SETCUEBANNER = 0x1501;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)]string lParam);

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public frmAuthen()
        {

            InitializeComponent();

            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 10, 10));

            CheckForIllegalCrossThreadCalls = false;

            lbTime.Visible = true;
            trd = new Thread(ThreadTask);
            trd.IsBackground = true;
            trd.Start();

            if (classGlobal.databaseType != "acc" && classGlobal.databaseType != "psql")
                roundButton2.Visible = false;

            btnExit.BackgroundImage = Image.FromFile(@"icon\poweroff.png");
            btnExit.BackgroundImageLayout = ImageLayout.Stretch;
            btnExit.FlatStyle = FlatStyle.Flat;

        }

        #region Show Time Clock
        private Thread trd;
        bool condition = true;
        string strCurrentDateTime = "";
        DateTime dt;
        int year;
        private void ThreadTask()
        {

            do
            {
                dt = DateTime.Now;  ////DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                if (dt.Year > 2500) { year = dt.Year; } else { year = dt.Year + 543; }

                strCurrentDateTime = dt.Day.ToString().PadLeft(2, '0').ToString() + "/" + dt.Month.ToString().PadLeft(2, '0').ToString() + "/" + year.ToString() + " " +
                                        dt.Hour.ToString().PadLeft(2, '0').ToString() + ":" + dt.Minute.ToString().PadLeft(2, '0').ToString() + ":" + dt.Second.ToString().PadLeft(2, '0').ToString();

                strCurrentDateTime = DateTime.Now.ToString("dddd dd MMM yyyy เวลา HH:mm:ss");

                Application.DoEvents();
                if (lbTime.InvokeRequired)
                {
                    lbTime.BeginInvoke(new MethodInvoker(delegate { lbTime.Text = strCurrentDateTime; }));
                    lbTime.BeginInvoke(new MethodInvoker(delegate { lbTime.Left = (this.Width - lbTime.Width) / 2; }));
                }
                else
                {
                    lbTime.Text = strCurrentDateTime;
                    lbTime.Left = (this.Width - lbTime.Width) / 2;
                }
                System.Threading.Thread.Sleep(1000);

            }
            while (condition == true);

            trd.Abort();

        }
        #endregion
               
        private void frmAuthen_Load(object sender, EventArgs e)
        {
             
            SendMessage(textBoxRound1.Handle, EM_SETCUEBANNER, 0, "ชื่อผู้ใช้");
            SendMessage(textBoxRound2.Handle, EM_SETCUEBANNER, 0, "รหัสผ่าน");
            this.ActiveControl = roundButton2;

 
            if (classGlobal.databaseType != "cloud")
            {
                roundButton2.PerformClick();
                return;
            }


            if (classGlobal.logInAlive != "-")
            {
                string[] _strEnc = classGlobal.logInAlive.Split('@'); 
                string usrDec = ClassEncryptDecrypt.Decrypt(_strEnc[0], "wacinfotech");
                string pwdDec = ClassEncryptDecrypt.Decrypt(_strEnc[1], "wacinfotech");
                textBoxRound1.Text = usrDec;
                textBoxRound2.Text = pwdDec;
                roundButton1.PerformClick();
            }

            label1.Text = classGlobal.API_URL;

        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                roundButton1.PerformClick(); 
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Control c = Control.FromHandle(msg.HWnd);
            if (keyData == Keys.Z)
            {
                textBoxRound1.Text = classGlobal.userName;
                textBoxRound2.Text = classGlobal.passWord;
            }

            //if (keyData == Keys.F1)
            //    classGlobal.API_URL = "http://10.0.0.205:4004/";
            //if (keyData == Keys.F2)
            //    classGlobal.API_URL = "https://api.visitors.wacappcloud.com/";
            if (keyData == Keys.Enter)
                roundButton1.PerformClick();

            label1.Text = classGlobal.API_URL;

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void frmAuthen_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void roundButton1_Click(object sender, EventArgs e)
        {
            if (textBoxRound1.Text == "")
            {
                frmMessageBox f = new frmMessageBox();
                f.strMessage = "กรุณาใส่ชื่อผู้ใช้";
                f.strStatus = "Warning";
                f.ShowDialog();
                return;
            }
            if (textBoxRound2.Text == "")
            {
                frmMessageBox f = new frmMessageBox();
                f.strMessage = "กรุณาใส่รหัสผ่าน";
                f.strStatus = "Warning";
                f.ShowDialog();
                return;
            }

            ClassData.POST_LOGIN(textBoxRound1.Text, textBoxRound2.Text);
            if (classGlobal.refresh_token == null || classGlobal.refresh_token == "")
            {
                classGlobal.userId = "";
                frmMessageBox f = new frmMessageBox();
                f.strMessage = "ชื่อผู้ใช้/รหัสผ่าน ไม่ถูกต้อง!";
                f.strStatus = "Warning";
                f.ShowDialog();
                return;
            }
            ClassData.GET_ACCESS_TOKEN(classGlobal.refresh_token);

            string currentRule = classGlobal.rule.ToString().ToLower().Replace(" ", "");   //masteradmin admin user member

            //if (currentRule == "masteradmin")      
            //{
            //    classGlobal.userId = "";
            //    frmMessageBox f = new frmMessageBox();
            //    f.strMessage = "Master Admin, ไม่สามารถเข้าใช้งานได้";
            //    f.strStatus = "Warning";
            //    f.ShowDialog();
            //    return;
            //}
            
            if (currentRule == "user")
            {
                classGlobal.userId = "";
                frmMessageBox f = new frmMessageBox();
                f.strMessage = "ไม่มีสิทธิ์เข้าใช้งาน";
                f.strStatus = "Warning";
                f.ShowDialog();
                return;
            }

            #region get user lists (ไม่ได้ใช้แล้ว)
            //if (classGlobal.userId != "")
            //{
            //    classGlobal.userName = textBoxRound1.Text;
            //    classGlobal.uId = ClassData.POST_FOR_GET_UID(classGlobal.userName);
            //}               
            #endregion

            clsXML cc = new clsXML();
            string usrEnc = ClassEncryptDecrypt.Encrypt(textBoxRound1.Text, "wacinfotech");
            string pwdEnc = ClassEncryptDecrypt.Encrypt(textBoxRound2.Text, "wacinfotech");
            //cc.ModifyElement("root", "Alive", usrEnc + "@" + pwdEnc, classGlobal.config);
            cc = null;

            File.WriteAllText(classGlobal.settingFile + @"/session", usrEnc + "@" + pwdEnc);

            condition = true;
            trd.Abort();
            this.Close();            
        }

        private void roundButton2_Click(object sender, EventArgs e)
        {
            classGlobal.userId = "";
            condition = true;
            trd.Abort();
            this.Close();
        }

        private void textBoxRound1_TextChanged(object sender, EventArgs e)
        {
            if (textBoxRound1.Text != "" && textBoxRound1.Text != "")
            {
                roundButton2.Enabled = false;
            }
            else
            {
                roundButton2.Enabled = true;
            }
        }

        private void textBoxRound2_TextChanged(object sender, EventArgs e)
        {
            if (textBoxRound1.Text != "" && textBoxRound1.Text != "")
            {
                roundButton2.Enabled = false;
            }
            else
            {
                roundButton2.Enabled = true;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            classGlobal.accountUsr = "";
            classGlobal.accountPwd = "";

            base.Dispose(true);
            Environment.Exit(0); 
        }

        private void TextBoxRound2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                roundButton1.PerformClick();
            }
        }
    }
}
