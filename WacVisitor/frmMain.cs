using AForge.Video;
using ClassHelper;
using iFinTechIDCard;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Npgsql;
using System.Diagnostics;

namespace WacVisitor
{
    public partial class frmMain : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

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

        [DllImport("Gdi32.dll", EntryPoint = "DeleteObject")]
        private static extern bool DeleteObject(System.IntPtr hObject);

        #region Reduce Memmory
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetProcessWorkingSetSize(IntPtr process,
            UIntPtr minimumWorkingSetSize, UIntPtr maximumWorkingSetSize);
        #endregion
       
        private string licenseApp = "";

        private void READ_CONFIG_XML()
        {
            clsXML clsxmlP = new clsXML();
            try
            {
                #region version factory or home
                if (clsxmlP.CheckExistElement("root", "version", classGlobal.config) == false)
                    clsxmlP.AppendElement("root", "version", "home", classGlobal.config);

                string appVersion = clsxmlP.GetReadXML("root", "version", classGlobal.config).ToString();
                if (appVersion.ToLower() == "home")
                    classGlobal.FactoryVersion = false;
                else
                    classGlobal.FactoryVersion = true;
                #endregion
                #region ใช้งาน billing or not
                if (clsxmlP.CheckExistElement("root", "billng", classGlobal.config) == true)
                {
                    if (clsxmlP.CheckExistElement("root", "billing", classGlobal.config) == false)
                    {
                        string _bill = clsxmlP.GetReadXML("root", "billng", classGlobal.config).ToString();
                        clsxmlP.AppendElement("root", "billing", _bill, classGlobal.config);
                        clsxmlP.RemoveElement("root", "billng", classGlobal.config);
                    }
                }
                if (clsxmlP.CheckExistElement("root", "billing", classGlobal.config) == false)
                    clsxmlP.AppendElement("root", "billing", "true", classGlobal.config);  // true / false
                if (classGlobal.FactoryVersion == true)
                    clsxmlP.ModifyElement("root", "billing", "false", classGlobal.config);

                string billing = clsxmlP.GetReadXML("root", "billing", classGlobal.config).ToString();
                classGlobal.boolCharge = bool.Parse(billing);
                #endregion
                #region พรบ ส่วนบุคคล  Statute
                string Statute = "";
                if (clsxmlP.CheckExistElement("root", "StatuteSave", classGlobal.config) == false)
                    classGlobal.boolStatuteSave = false; //clsxml.AppendElement("root", "StatuteSave", "false", classGlobal.config);  // true / false
                else
                    clsxmlP.RemoveElement("root", "StatuteSave", classGlobal.config);  // ลบ config นี้ออก
                classGlobal.boolStatuteSave = false; // fix เป็น false บังคับให้ยังไงก็ต้องบันทึกข้อมุลเต็มลงฐานข้อมูล

                if (clsxmlP.CheckExistElement("root", "StatuteDisplay", classGlobal.config) == true)
                    clsxmlP.RemoveElement("root", "StatuteDisplay", classGlobal.config);  // ลบ config นี้ออก ไม่ใช้ชื่อ config StatuteDisplay นี้แล้ว งงความหมาย
                if (clsxmlP.CheckExistElement("root", "enablestatute", classGlobal.config) == false)
                    clsxmlP.AppendElement("root", "enablestatute", "true", classGlobal.config);  // true / false
                Statute = clsxmlP.GetReadXML("root", "enablestatute", classGlobal.config).ToString();
                classGlobal.DisplayHashTag = bool.Parse(Statute);
                #endregion
                #region text water mark
                if (clsxmlP.CheckExistElement("root", "watermark", classGlobal.config) == false)
                    clsxmlP.AppendElement("root", "watermark", "VISITOR", classGlobal.config);  // true / false
                classGlobal.watermark = clsxmlP.GetReadXML("root", "watermark", classGlobal.config).ToString();
                #endregion
                #region เครื่องเก็บเงิน
                string commName = classGlobal.GetPortInformation().Replace(" ", "");
                if (clsxmlP.CheckExistElement("root", "commport", classGlobal.config) == false)
                    clsxmlP.AppendElement("root", "commport", commName, classGlobal.config);  // true / false
                clsxmlP.ModifyElement("root", "commport", commName, classGlobal.config);
                classGlobal.commportName = clsxmlP.GetReadXML("root", "commport", classGlobal.config).ToString();
                #endregion
                #region Time Attendance
                string enableTimeAtt = "false";
                if (clsxmlP.CheckExistElement("root", "timeattendance", classGlobal.config) == false)
                    clsxmlP.AppendElement("root", "timeattendance", "false", classGlobal.config);  // true / false           
                enableTimeAtt = clsxmlP.GetReadXML("root", "timeattendance", classGlobal.config).ToString();
                classGlobal.enableTimeAttendance = bool.Parse(enableTimeAtt);
                clsxmlP = null;
                #endregion
                #region Language of Slip Factory Version
                clsxmlP = new ClassHelper.clsXML();
                if (clsxmlP.CheckExistElement("root", "slipregion", classGlobal.config) == false)
                    clsxmlP.AppendElement("root", "slipregion", "th", classGlobal.config);  // true / false                         
                clsxmlP = null;
                #endregion
                #region Check user/password authen
                clsxmlP = new ClassHelper.clsXML();
                if (clsxmlP.CheckExistElement("root", "Alive", classGlobal.config) == true)
                    clsxmlP.RemoveElement("root", "Alive", classGlobal.config);
                //if (clsxmlP.CheckExistElement("root", "Alive", classGlobal.config) == false)
                //    clsxmlP.AppendElement("root", "Alive", "-", classGlobal.config);
                //classGlobal.logInAlive = clsxmlP.GetReadXML("root", "Alive", classGlobal.config).ToString();
                clsxmlP = null;

                if (Directory.Exists(classGlobal.settingFile) == false)
                    Directory.CreateDirectory(classGlobal.settingFile);
                if (File.Exists(classGlobal.settingFile + @"/session") == false)
                    File.WriteAllText(classGlobal.settingFile + @"/session", "-");
                classGlobal.logInAlive = File.ReadAllText(classGlobal.settingFile + @"/session");
                #endregion
                #region Get Terminal ID
                ClassHelper.clsXML cls = new ClassHelper.clsXML();
                classGlobal.terminalId = cls.GetReadXML("root", "place", classGlobal.config);
                cls = null;
                #endregion

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString()); 
            }
            clsxmlP = null;

            #region Storage image location from ipcamera
            classGlobal.ipcamstoragePath = Directory.GetCurrentDirectory();
            String[] splt = classGlobal.ipcamstoragePath.Split('\\');
            classGlobal.ipcamstoragePath = "";
            for (int i = 0; i < splt.Length - 1; i++)
            {
                classGlobal.ipcamstoragePath = classGlobal.ipcamstoragePath + splt[i] + @"\";
            }
            classGlobal.ipcamstoragePath = classGlobal.ipcamstoragePath.Substring(0, classGlobal.ipcamstoragePath.Length - 1) + @"\" + "ipcamstorage" + @"\";
            #endregion
            #region Storage temp image location from post to cloud server 
            if (!System.IO.Directory.Exists(classGlobal.storageImages))
                System.IO.Directory.CreateDirectory(classGlobal.storageImages);
            #endregion
            #region check ข้อมูลเฉพาะงานพี่กุ่ย (Sritrang)
            try
            {
                if (System.IO.File.Exists(@"mapping.txt") == false)
                {
                    string mapping = "IN=0001" + "\n" + "OUT=0002";
                    byte[] bufferText = System.Text.Encoding.Default.GetBytes(String.Format("{0}{1}", mapping, Environment.NewLine));
                    System.IO.FileStream fs = System.IO.File.Open(@"mapping.txt", System.IO.FileMode.Append);
                    fs.Write(bufferText, 0, bufferText.Length);
                    fs.Close();
                }

                string[] arr_mapping = System.IO.File.ReadAllLines(@"mapping.txt");
                classGlobal.sIN = arr_mapping[0].Replace("IN=", "").Replace(" ", "");
                classGlobal.sOUT = arr_mapping[1].Replace("OUT=", "").Replace(" ", "");
            }
            catch
            {
                classGlobal.sIN = "0001";
                classGlobal.sOUT = "0002";
            }
            #endregion

        }
        public frmMain()
        {

            classGlobal.isStillRunning();

            InitializeComponent();

            string localLicense = GET_LICENSE();

            clsXML clsxml = new clsXML();
            //++ Create XML Keys 
            if (clsxml.CheckExistElement("root", "license", classGlobal.config) == false)
            {
                clsxml.AppendElement("root", "license", "-", classGlobal.config);
            }
            licenseApp = clsxml.GetReadXML("root", "license", classGlobal.config).ToString();
            if (licenseApp == "-" || licenseApp == "" || licenseApp != localLicense)
            {
                frmCheckLicense f = new frmCheckLicense();
                f.ShowDialog();

                base.Dispose(true);
                Environment.Exit(0);
            }
            //--
            clsxml = null;

            READ_CONFIG_XML();

            #region Create access database if not exists
            if (System.IO.File.Exists(classGlobal.DATABASELOCATION + classGlobal.DATABASENANE + ".mdb") == false)
            {
                byte[] bytesDb = Convert.FromBase64String(classCreateAccessDB.base64Db);
                System.IO.File.WriteAllBytes(classGlobal.DATABASELOCATION + classGlobal.DATABASENANE + ".mdb", bytesDb);

                string strConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;" +
                                            "Data source=" + classGlobal.DATABASELOCATION + classGlobal.DATABASENANE + ".mdb;" +
                                                "Jet OLEDB:Database Password=wacinfotech;";
                OleDbConnection con = new OleDbConnection(strConnectionString);
                con.Open();
                classCreateAccessDB.CREATE_SCRIPTS_ACCESS_TABLE(con);
                con.Close();
                con = null;
            }
            #endregion

            classGlobal.ConnectToDatabase();  // connect to database for create connection status

            //++ ตรวจสอบว่า เชื่อมต่อ server หรือไม่   // usr = tt8@tt.com    pwd = 1234
            if (classGlobal.databaseType == "cloud")
            {
                frmAuthen fAuthen = new frmAuthen();
                fAuthen.ShowDialog();
            }           
            //-- ตรวจสอบว่า เชื่อมต่อ server หรือไม่
                       
            this.Text = "WAC VISITOR";
 
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;           
           
            lbTime.Visible = true;
            trd = new Thread(ThreadTask);
            trd.IsBackground = true;
            trd.Start();

            pbIcon.Image = Image.FromFile(@"icon\icon.png");
            btnExit.BackgroundImage = Image.FromFile(@"icon\poweroff.png");
            btnExit.BackgroundImageLayout = ImageLayout.Stretch;
            btnExit.FlatStyle = FlatStyle.Flat;
            btnHome.BackgroundImage = Image.FromFile(@"icon\home.png");
            btnHome.BackgroundImageLayout = ImageLayout.Stretch;
            btnHome.FlatStyle = FlatStyle.Flat;
            btnView.BackgroundImage = Image.FromFile(@"icon\view.png");
            btnView.BackgroundImageLayout = ImageLayout.Stretch;
            btnView.FlatStyle = FlatStyle.Flat;
            btnSetting.BackgroundImage = Image.FromFile(@"icon\setting.png");
            btnSetting.BackgroundImageLayout = ImageLayout.Stretch;
            btnSetting.FlatStyle = FlatStyle.Flat;
            btnExport.BackgroundImage = Image.FromFile(@"icon\export.png");
            btnExport.BackgroundImageLayout = ImageLayout.Stretch;
            btnExport.FlatStyle = FlatStyle.Flat;

            btnWEBCAM.BackgroundImage = Image.FromFile(@"icon\webcam.png");
            btnWEBCAM.BackgroundImageLayout = ImageLayout.Stretch;
            btnWEBCAM.FlatStyle = FlatStyle.Flat;

            btnMoney.BackgroundImage = Image.FromFile(@"icon\money.png");
            btnMoney.BackgroundImageLayout = ImageLayout.Stretch;
            btnMoney.FlatStyle = FlatStyle.Flat;


            btnExit1.BackgroundImage = Image.FromFile(@"icon\poweroff.png");
            btnExit1.BackgroundImageLayout = ImageLayout.Stretch;
            btnExit1.FlatStyle = FlatStyle.Flat;

            String base64IpCam = "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";
            pictureBox1.Top = panel4.Height - pictureBox1.Height;
            pictureBox1.BackColor = System.Drawing.SystemColors.HotTrack;
            pictureBox1.Image = Base64ToImage(base64IpCam);


            btnAppointment.BackgroundImage = Image.FromFile(@"icon\appointment.png");
            btnAppointment.BackgroundImageLayout = ImageLayout.Stretch;
            btnAppointment.FlatStyle = FlatStyle.Flat;
            btnAppointment.Visible = false;

            if (classGlobal.EnableAppointmentMenu == true)
                if (classGlobal.userId != "")
                    btnAppointment.Visible = true;

            CHECK_CONFIG_XML();

            if (classGlobal.FactoryVersion == true)
            {
                btnMoney.Visible = false;
                btnExit.Visible = false;
                btnExit1.Visible = true;                
            }

            

            btnMoney.Visible = classGlobal.boolCharge;

            //string backupResult = classGlobal.BACKUP_DATABASE();
            //string restoreResult = classGlobal.RESTORE_DATABASE();
            //classGlobal.DELETE_FOR_BACKUP(); 

            //++ MS ACCESS ONLY CHECK EXISTS TABLE
            if (classGlobal.conn.State == ConnectionState.Open)
            {           
                classGlobal.ALTER_TABLE_MAC_LOCATION();
                classGlobal.ALTER_TABLE_UPLOAD_SERVER();
                classGlobal.ALTER_TABLE_VISITOR();
                classGlobal.CREATE_TABLE_IDCARD();
                classGlobal.CREATE_TABLE_PASSPORT();
                classGlobal.CREATE_TABLE_TBL_USER();
                classGlobal.CREATE_TABLE_TBL_PERSONAL();
                classGlobal.CREATE_TABLE_TBL_TYPE();
                classGlobal.CREATE_TABLE_TBL_VISITOR();
                classGlobal.CREATE_TABLE_TBL_BLACKLIST();
                classGlobal.CREATE_TABLE_TBL_WHITELIST();
                classGlobal.CREATE_TABLE_TBL_MOREINFO();
                classGlobal.CREATE_TABLE_TBL_VEHICLE();
                classGlobal.CREATE_MOREINFO_FACTORY();
                classGlobal.CREATE_TABLE_VISITOR_COMPANY();
                classGlobal.CREATE_TABLE_LICENSE_PLATE();
                classGlobal.CREATE_TABLE_VEHICLE_TYPE();
                classGlobal.CREATE_TABLE_VISIT_TO();
                classGlobal.CREATE_TABLE_DEPARTMENT();
                classGlobal.CREATE_TABLE_BUSINESS_TOPIC();
                classGlobal.CREATE_TABLE_PLACE();
                classGlobal.CREATE_CHARGE_CAR_PARK();
                classGlobal.CREATE_CHARGE_LOGS();

                classGlobal.ALTER_TABLE_CHARGE_CAR_PARK();
                
            }
            //-- MS ACCESS ONLY CHECK EXISTS TABLE   

            if (classGlobal.databaseType == "psql")
            {
                classGlobal.CHECK_EXIST_TABLE_PSQL();
                GC.Collect(GC.MaxGeneration);
                GC.WaitForPendingFinalizers();
                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, (UIntPtr)0xFFFFFFFF, (UIntPtr)0xFFFFFFFF);
            }
            
        }
        private void CHECK_CONFIG_XML()
        {
            clsXML c = new clsXML();
           
            if (c.CheckExistElement("root", "webcamdevice", classGlobal.config) == false)
                c.AppendElement("root", "webcamdevice", "1", classGlobal.config);

            if (c.CheckExistElement("root", "barcodetype", classGlobal.config) == false)
                c.AppendElement("root", "barcodetype", "qrcode", classGlobal.config);

            if (c.CheckExistElement("root", "randomnumber", classGlobal.config) == false)
                c.AppendElement("root", "randomnumber", "0", classGlobal.config);

            if (c.CheckExistElement("root", "db", classGlobal.config) == false)
                c.AppendElement("root", "db", "acc", classGlobal.config);

            if (c.CheckExistElement("root", "place", classGlobal.config) == false)
                c.AppendElement("root", "place", "ประตู 1", classGlobal.config);
            classGlobal.strPlace = c.GetReadXML("root", "place", classGlobal.config); 

            if (c.CheckExistElement("root", "titlename", classGlobal.config) == false)
                c.AppendElement("root", "titlename", "VISITOR", classGlobal.config);
            classGlobal.strTitle = c.GetReadXML("root", "titlename", classGlobal.config);

            if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"db.xml") == false)
            {
                c.CreateXML(AppDomain.CurrentDomain.BaseDirectory + @"db.xml");
                c.AppendElement("root", "ServerIP", "127.0.0.1", AppDomain.CurrentDomain.BaseDirectory + @"db.xml");
                c.AppendElement("root", "Port", "5432", AppDomain.CurrentDomain.BaseDirectory + @"db.xml");
                c.AppendElement("root", "DB", "wacvisitor", AppDomain.CurrentDomain.BaseDirectory + @"db.xml");
                c.AppendElement("root", "User", "postgres", AppDomain.CurrentDomain.BaseDirectory + @"db.xml");
                c.AppendElement("root", "Password", classGlobal.Crypt("password@1"), AppDomain.CurrentDomain.BaseDirectory + @"db.xml");
            }
            
            c = null;
        }
        
        private void CheckFolderExist()
        {
            if (System.IO.Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + classGlobal.webcam) == false)
                System.IO.Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + classGlobal.webcam);
        }

        private frmView frmV = null;
        private frmSummaryLogs frmLogs = null;
        private frmHWDevices frmDevice = null;
        private frmSettingType frmSettingType = null;

        private FormSetting.FormActivity frmActivity = null;

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Control c = Control.FromHandle(msg.HWnd);
            switch (keyData)
            {
                case Keys.Home:
                    btnHome.PerformClick();
                    break;
                case Keys.F1:
                    rbtnIN.PerformClick();
                    break;
                case Keys.F2:
                    rbtnOUT.PerformClick();
                    break;
                case Keys.F3:
                    classGlobal.strPlace = new clsXML().GetReadXML("root", "place", classGlobal.config);
                    break;
                case Keys.F5:
                    //btnHome.PerformClick();
                    break;
                case Keys.F6:
                    //classGlobal.CHECK_EXIST_IN_FOR_PUB("3190900120057", "นายวิทยา สุดประเสริฐ", "02/12/1979");
                    break;
                case Keys.F7:
                    //classGlobal.CHECK_EXIST_IN_FOR_PUB("3190900120058", "WAC RESEARCH", "02/12/2008");
                    break;
                case Keys.F8:
                    //classGlobal.CHECK_EXIST_IN_FOR_PUB("1440900161480", "MR.ANUCHA TIEMSOM", "04/11/1991");
                    break;
                case Keys.F12:
                    //code block
                    break;
                default:
                    //code block
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void p_Exited(object sender, EventArgs e)
        {
            frmMessageBox f = new frmMessageBox();
            f.strMessage = "ปิด-เปิดโปรแกรมใหม่";
            f.strStatus = "Information";
            f.ShowDialog();
        }
        private void frmMain_Load(object sender, EventArgs e)
        {
            if (File.Exists("lang") == false)
                System.IO.File.WriteAllText("lang", "th-TH");

            if (!File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "configipcam.txt"))
            {
                // Create a new file     
                using (FileStream fs = File.Create(System.AppDomain.CurrentDomain.BaseDirectory + "configipcam.txt"))
                {
                    // Add some text to file   
                    byte[] txt = new byte[0];
                    txt = new UTF8Encoding(true).GetBytes("[LOGIN]admin,password@1,8000" + Environment.NewLine);
                    fs.Write(txt, 0, txt.Length);
                    txt = new UTF8Encoding(true).GetBytes(";[NVR]@192.168.254.1" + Environment.NewLine);
                    fs.Write(txt, 0, txt.Length);
                    txt = new UTF8Encoding(true).GetBytes("[DIRECT]@192.168.254.2,192.168.254.2");
                    fs.Write(txt, 0, txt.Length); 
                }  
            }    

            label1.Text = classGlobal.strTitle;

            //++ อ่านค่า mac address มา encrypt ปิดไว้ เพราะยังไม่ใช้ตอนนี้ เนื่องจากยังทำงานเป็น stand alone อยู่ ถ้าเปิดใช้ จะทำให้เปลืองพื้นที่ database
            classGlobal.MachineAddress = "";  
            classGlobal.MachineAddress = GET_LICENSE();  
            //--

            CheckFolderExist();
            classGlobal.statusIn_OUT = ""; 

            btnExit.Left = (this.panel_top.Size.Width - btnExit.Size.Width - 8);
            rbtnOUT.Left = ((this.ClientSize.Width - rbtnOUT.Size.Width) / 2) + (rbtnOUT.Size.Width / 2);
            rbtnIN.Left = rbtnOUT.Left -  rbtnIN.Size.Width  - 10;            
            
            dt = DateTime.Now; 
            if (dt.Year > 2500) { year = dt.Year; } else { year = dt.Year + 543; }

            strCurrentDateTime = dt.Day.ToString().PadLeft(2, '0').ToString() + "/" + dt.Month.ToString().PadLeft(2, '0').ToString() + "/" + year.ToString() + " " +
                                    dt.Hour.ToString().PadLeft(2, '0').ToString() + ":" + dt.Minute.ToString().PadLeft(2, '0').ToString() + ":" + dt.Second.ToString().PadLeft(2, '0').ToString();

            strCurrentDateTime = DateTime.Now.ToString("dddd dd MMM yyyy เวลา HH:mm:ss");             

            lbTime.Text = strCurrentDateTime;
            lbTime.Left = (this.ClientSize.Width - lbTime.Size.Width - 10);

            //++++ ถ้าเชื่อมต่อฐานข้อมูลไม่ได้
            if (classGlobal.databaseType == "psql")
            {
                if (classGlobal.connP.State == ConnectionState.Closed)
                {
                    frmMessageBox f = new frmMessageBox();
                    f.strMessage = "ไม่สามารถเชื่อมต่อฐานข้อมูลได้.";
                    f.strStatus = "Warning";
                    f.ShowDialog();

                    rbtnIN.Enabled = false;
                    rbtnOUT.Enabled = false;

                    btnHome.Visible = false;
                    btnView.Visible = false;
                    btnSetting.Visible = false;
                    btnExport.Visible = false;
                    btnWEBCAM.Visible = false;
                    btnMoney.Visible = false;

                    System.Diagnostics.Process p = new System.Diagnostics.Process();
                    p.Exited += new EventHandler(p_Exited);
                    p.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + @"DatabaseConnection.exe";
                    p.StartInfo.Arguments = "";                     
                    p.EnableRaisingEvents = false;
                    p.Start();

                    base.Dispose(true);
                    Environment.Exit(0);

                    return;
                }

            }
            //---- ถ้าเชื่อมต่อฐานข้อมูลไม่ได้

            if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\Camera1.jpg"))
                System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\Camera1.jpg");

            if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\Camera2.jpg"))
                System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\Camera2.jpg");

            LOAD_FRMLOGS();

            clsXML cls = new clsXML();
            if (cls.CheckExistElement("root", "randomnumber", classGlobal.config) == false)
                cls.AppendElement("root", "randomnumber", "0", classGlobal.config);
            classGlobal.strRandomNumber = cls.GetReadXML("root", "randomnumber", classGlobal.config).Replace("-", "0");
            cls = null;

            #region เช็คดูว่า tbl_vehicle ของ Home Version - progress มีข้อมูลรึเปล่า           
            if (classGlobal.databaseType == "acc")
            {                
                DataTable dtvehicle = new DataTable("vehicle");
                OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * FROM tbl_vehicle", classGlobal.conn);
                adapter.Fill(dtvehicle);
                adapter.Dispose();
                adapter = null;
                if (dtvehicle.Rows.Count == 0)
                {
                    OleDbCommand cmd;
                    cmd = new OleDbCommand("INSERT INTO tbl_vehicle(v_id, v_nametype) VALUES ('1', 'รถยนต์')", classGlobal.conn);
                    cmd.ExecuteNonQuery();
                    cmd = null;
                    cmd = new OleDbCommand("INSERT INTO tbl_vehicle(v_id, v_nametype) VALUES ('2', 'รถจักรยานยนต์')", classGlobal.conn);
                    cmd.ExecuteNonQuery();
                    cmd = null;
                    cmd = new OleDbCommand("INSERT INTO tbl_vehicle(v_id, v_nametype) VALUES ('3', 'รถกระบะ')", classGlobal.conn);
                    cmd.ExecuteNonQuery();
                    cmd = null;
                    cmd = new OleDbCommand("INSERT INTO tbl_vehicle(v_id, v_nametype) VALUES ('4', 'รถTaxi')", classGlobal.conn);
                    cmd.ExecuteNonQuery();
                    cmd = null;
                    cmd = new OleDbCommand("INSERT INTO tbl_vehicle(v_id, v_nametype) VALUES ('5', 'อื่นๆ')", classGlobal.conn);
                    cmd.ExecuteNonQuery();
                    cmd = null;
                }

            }
            else if (classGlobal.databaseType == "psql")
            {
                DataTable dtvehicle = new DataTable("vehicle");
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter("SELECT * FROM tbl_vehicle", classGlobal.connP);
                adapter.Fill(dtvehicle);
                adapter.Dispose();
                adapter = null;
                if (dtvehicle.Rows.Count == 0)
                {
                    NpgsqlCommand cmd;
                    cmd = new NpgsqlCommand("INSERT INTO tbl_vehicle(v_id, v_nametype) VALUES ('1', 'รถยนต์')", classGlobal.connP);
                    cmd.ExecuteNonQuery();
                    cmd = null;
                    cmd = new NpgsqlCommand("INSERT INTO tbl_vehicle(v_id, v_nametype) VALUES ('2', 'รถจักรยานยนต์')", classGlobal.connP);
                    cmd.ExecuteNonQuery();
                    cmd = null;
                    cmd = new NpgsqlCommand("INSERT INTO tbl_vehicle(v_id, v_nametype) VALUES ('3', 'รถกระบะ')", classGlobal.connP);
                    cmd.ExecuteNonQuery();
                    cmd = null;
                    cmd = new NpgsqlCommand("INSERT INTO tbl_vehicle(v_id, v_nametype) VALUES ('4', 'รถTaxi')", classGlobal.connP);
                    cmd.ExecuteNonQuery();
                    cmd = null;
                    cmd = new NpgsqlCommand("INSERT INTO tbl_vehicle(v_id, v_nametype) VALUES ('5', 'อื่นๆ')", classGlobal.connP);
                    cmd.ExecuteNonQuery();
                    cmd = null;
                }
            }
            else
            {
                // do nothing
            }
            #endregion

            #region เช็คดูว่า tbl_vehicle_type ของ Factory Version - progress มีข้อมูลรึเปล่า   
            if (classGlobal.databaseType == "acc")
            {
                DataTable dtvehicle = new DataTable("vehicle");
                OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * FROM tbl_vehicle_type", classGlobal.conn);
                adapter.Fill(dtvehicle);
                adapter.Dispose();
                adapter = null;
                if (dtvehicle.Rows.Count == 0)
                {
                    OleDbCommand cmd;
                    cmd = new OleDbCommand("INSERT INTO tbl_vehicle_type(ID, vehicle_type) VALUES ('1', 'รถยนต์')", classGlobal.conn);
                    cmd.ExecuteNonQuery();
                    cmd = null;
                    cmd = new OleDbCommand("INSERT INTO tbl_vehicle_type(ID, vehicle_type) VALUES ('2', 'รถจักรยานยนต์')", classGlobal.conn);
                    cmd.ExecuteNonQuery();
                    cmd = null;
                    cmd = new OleDbCommand("INSERT INTO tbl_vehicle_type(ID, vehicle_type) VALUES ('3', 'รถกระบะ')", classGlobal.conn);
                    cmd.ExecuteNonQuery();
                    cmd = null;
                    cmd = new OleDbCommand("INSERT INTO tbl_vehicle_type(ID, vehicle_type) VALUES ('4', 'รถTaxi')", classGlobal.conn);
                    cmd.ExecuteNonQuery();
                    cmd = null;
                    cmd = new OleDbCommand("INSERT INTO tbl_vehicle_type(ID, vehicle_type) VALUES ('5', 'อื่นๆ')", classGlobal.conn);
                    cmd.ExecuteNonQuery();
                    cmd = null;
                }
            }
            else if (classGlobal.databaseType == "psql")
            {
                DataTable dtvehicle = new DataTable("vehicle");
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter("SELECT * FROM tbl_vehicle_type", classGlobal.connP);
                adapter.Fill(dtvehicle);
                adapter.Dispose();
                adapter = null;
                if (dtvehicle.Rows.Count == 0)
                {
                    NpgsqlCommand cmd;
                    cmd = new NpgsqlCommand("INSERT INTO tbl_vehicle_type(ID, vehicle_type) VALUES ('1', 'รถยนต์')", classGlobal.connP);
                    cmd.ExecuteNonQuery();
                    cmd = null;
                    cmd = new NpgsqlCommand("INSERT INTO tbl_vehicle_type(ID, vehicle_type) VALUES ('2', 'รถจักรยานยนต์')", classGlobal.connP);
                    cmd.ExecuteNonQuery();
                    cmd = null;
                    cmd = new NpgsqlCommand("INSERT INTO tbl_vehicle_type(ID, vehicle_type) VALUES ('3', 'รถกระบะ')", classGlobal.connP);
                    cmd.ExecuteNonQuery();
                    cmd = null;
                    cmd = new NpgsqlCommand("INSERT INTO tbl_vehicle_type(ID, vehicle_type) VALUES ('4', 'รถTaxi')", classGlobal.connP);
                    cmd.ExecuteNonQuery();
                    cmd = null;
                    cmd = new NpgsqlCommand("INSERT INTO tbl_vehicle_type(ID, vehicle_type) VALUES ('5', 'อื่นๆ')", classGlobal.connP);
                    cmd.ExecuteNonQuery();
                    cmd = null;
                }
            }
            else
            {
                // do nothing
            }
            #endregion

            #region TIME ATTENDANCE เฉพาะของพี่กุ่ย
            if (classGlobal.enableTimeAttendance == true)
            {
                bool timeAttRunning = false;
                foreach (var process1 in Process.GetProcessesByName("TimeAttendance"))
                {
                    if (process1.ProcessName.ToString().ToLower().Equals("timeattendance"))
                    {
                        timeAttRunning = true;
                        break;
                    }
                }

                if (timeAttRunning == false)
                {
                    if (classGlobal.userId == "")
                    {
                        string path = "TimeAttendance.exe";
                        if (File.Exists(path))
                        {
                            ProcessStartInfo p = new ProcessStartInfo(path, null);
                            Process process = Process.Start(p);
                        }
                        else
                        {
                            MessageBox.Show("ไม่พบไฟล์ " + path, "Time Attendance", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }                    
                }                
            }
            #endregion

            #region LOAD LOGO สำหรับพิมพ์สลิป ใน version cloud
            if (classGlobal.userId != "")
            {
                string name = "";
                string base64Logo = ClassData.GET_LOGO(ref name);
                if (base64Logo != "")
                {
                    byte[] logoBytes = Convert.FromBase64String(base64Logo);
                    File.WriteAllBytes(@"logo.png", logoBytes);
                }
            }
            #endregion

            //DataTable dtx = ClassData.GET_APPOINTMENT();

        }

        // ==> frmSummaryLogs() show in main panel
        private void LOAD_FRMLOGS()  
        {
            frmLogs = new frmSummaryLogs();
            frmLogs.TopLevel = false;
            frmLogs.AutoScroll = false;
            this.panel_main.Controls.Add(frmLogs);
            AnimateWindow(frmLogs.Handle, 500, AnimateWindowFlags.AW_CENTER);
            frmLogs.Show();
        }

        private string GET_LICENSE()
        {
            string sReturn = "";
            classCheckLicense clsLicense = new classCheckLicense();
            try
            {
                string str_Get_Win32_BaseBoard = clsLicense.Get_Win32_BaseBoard();
                long Lng = clsLicense.EncryptionData(str_Get_Win32_BaseBoard);
                string strEnc = clsLicense.Generates16ByteUnique(Lng.ToString());
                sReturn = strEnc;
            }
            catch
            {
                sReturn = "";
            }
            clsLicense = null;
            return sReturn;
        }
        private void Exit()
        {
            frmExit f = new frmExit();
            f.ShowDialog();
            this.Show();

            bool bExit = classGlobal.bool_Exit;
            if (bExit == true)
            {

                if (classGlobal.databaseType == "acc")
                {
                    if (classGlobal.conn.State != ConnectionState.Closed)
                    {
                        classGlobal.conn.Close();
                    }
                }
                else if (classGlobal.databaseType == "psql")
                {
                    if (classGlobal.connP.State != ConnectionState.Closed)
                    {
                        classGlobal.connP.Close();
                    }
                }
                else
                {

                }

                if (System.IO.Directory.Exists(classGlobal.webcam) == true)
                {
                    System.IO.DirectoryInfo di = new DirectoryInfo(classGlobal.webcam);
                    foreach (FileInfo file in di.GetFiles())
                        file.Delete();
                    //System.IO.Directory.Delete(@"webcam");
                }

                foreach (var process in System.Diagnostics.Process.GetProcessesByName("HIKEngine"))
                {
                    process.Kill();
                    break;
                }

                if (classGlobal.enableTimeAttendance == true)
                {
                    foreach (var process in System.Diagnostics.Process.GetProcessesByName("TimeAttendance"))  
                    {
                        process.Kill();
                        break;
                    }
                }

                #region Copy config file to C:\ProgramData\VISITORPASS to backup
                File.Copy(@"config.xml", classGlobal.settingFile + @"\config.xml", true);
                #endregion

                base.Dispose(true);
                Environment.Exit(0);
            }

        }
        private Thread trd;
        bool condition = true;

        #region Show Time Clock
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
                    lbTime.BeginInvoke(new MethodInvoker(delegate { lbTime.Text = strCurrentDateTime + "    "; }));
                }
                else
                {
                    lbTime.Text = strCurrentDateTime + "    ";
                }

                System.Threading.Thread.Sleep(1000);

            }
            while (condition == true);

            trd.Abort();

        }
        #endregion

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (classGlobal.userId != "")
            {

                frmLogOff f = new frmLogOff();
                f.ShowDialog();

                if (f.cancel == true)
                    return; 

                if (classGlobal.bool_Exit == true)
                {
                    Exit();
                }
                else
                {                    
                    clsXML cx = new clsXML();
                    //cx.ModifyElement("root", "Alive", "-", @"config.xml"); 
                    cx = null;

                    File.WriteAllText(classGlobal.settingFile + @"/session", "-");

                    //string stringInBase64 = "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cmUAcmVzb3VyY2VDdWx0dXJlAEFwcGxpY2F0aW9uU2V0dGluZ3NCYXNlAERpc3Bvc2UARWRpdG9yQnJvd3NhYmxlU3RhdGUAc2V0X1dpbmRvd1N0YXRlAEZvcm1XaW5kb3dTdGF0ZQBTVEFUaHJlYWRBdHRyaWJ1dGUAQ29tcGlsZXJHZW5lcmF0ZWRBdHRyaWJ1dGUAR3VpZEF0dHJpYnV0ZQBHZW5lcmF0ZWRDb2RlQXR0cmlidXRlAERlYnVnZ2VyTm9uVXNlckNvZGVBdHRyaWJ1dGUARGVidWdnYWJsZUF0dHJpYnV0ZQBFZGl0b3JCcm93c2FibGVBdHRyaWJ1dGUAQ29tVmlzaWJsZUF0dHJpYnV0ZQBBc3NlbWJseVRpdGxlQXR0cmlidXRlAEFzc2VtYmx5VHJhZGVtYXJrQXR0cmlidXRlAFRhcmdldEZyYW1ld29ya0F0dHJpYnV0ZQBBc3NlbWJseUZpbGVWZXJzaW9uQXR0cmlidXRlAEFzc2VtYmx5Q29uZmlndXJhdGlvbkF0dHJpYnV0ZQBBc3NlbWJseURlc2NyaXB0aW9uQXR0cmlidXRlAENvbXBpbGF0aW9uUmVsYXhhdGlvbnNBdHRyaWJ1dGUAQXNzZW1ibHlQcm9kdWN0QXR0cmlidXRlAEFzc2VtYmx5Q29weXJpZ2h0QXR0cmlidXRlAEFzc2VtYmx5Q29tcGFueUF0dHJpYnV0ZQBSdW50aW1lQ29tcGF0aWJpbGl0eUF0dHJpYnV0ZQB2YWx1ZQByZWxvYWQuZXhlAHNldF9DbGllbnRTaXplAFN5c3RlbS5SdW50aW1lLlZlcnNpb25pbmcAVG9TdHJpbmcAZGlzcG9zaW5nAHdhaXRpbmcAU3lzdGVtLkRyYXdpbmcAX2FyZwBTeXN0ZW0uQ29tcG9uZW50TW9kZWwAQ29udGFpbmVyQ29udHJvbABQcm9ncmFtAFN5c3RlbQBGb3JtAHJlc291cmNlTWFuAE1haW4Ac2V0X1Nob3dJY29uAEFwcGxpY2F0aW9uAFN5c3RlbS5Db25maWd1cmF0aW9uAFN5c3RlbS5HbG9iYWxpemF0aW9uAFN5c3RlbS5SZWZsZWN0aW9uAFJ1bgBDdWx0dXJlSW5mbwBzZXRfU2hvd0luVGFza2JhcgBzZW5kZXIAZ2V0X1Jlc291cmNlTWFuYWdlcgBFdmVudEhhbmRsZXIAU3lzdGVtLkNvZGVEb20uQ29tcGlsZXIASUNvbnRhaW5lcgAuY3RvcgAuY2N0b3IAU3lzdGVtLkRpYWdub3N0aWNzAFN5c3RlbS5SdW50aW1lLkludGVyb3BTZXJ2aWNlcwBTeXN0ZW0uUnVudGltZS5Db21waWxlclNlcnZpY2VzAFN5c3RlbS5SZXNvdXJjZXMAcmVsb2FkLkZvcm0xLnJlc291cmNlcwByZWxvYWQuUHJvcGVydGllcy5SZXNvdXJjZXMucmVzb3VyY2VzAERlYnVnZ2luZ01vZGVzAHJlbG9hZC5Qcm9wZXJ0aWVzAEVuYWJsZVZpc3VhbFN0eWxlcwBTZXR0aW5ncwBFdmVudEFyZ3MAYXJncwBTeXN0ZW0uV2luZG93cy5Gb3JtcwBzZXRfQXV0b1NjYWxlRGltZW5zaW9ucwBQcm9jZXNzAGNvbXBvbmVudHMAT2JqZWN0AEV4aXQAZ2V0X0RlZmF1bHQAU2V0Q29tcGF0aWJsZVRleHRSZW5kZXJpbmdEZWZhdWx0AEluaXRpYWxpemVDb21wb25lbnQAU3RhcnQAU3VzcGVuZExheW91dABSZXN1bWVMYXlvdXQAc2V0X1RleHQAZ2V0X0Fzc2VtYmx5AAAAAAAJMgAwADAAMAAAHVcAYQBjAFYAaQBzAGkAdABvAHIALgBlAHgAZQAAC0YAbwByAG0AMQAAN3IAZQBsAG8AYQBkAC4AUAByAG8AcABlAHIAdABpAGUAcwAuAFIAZQBzAG8AdQByAGMAZQBzAAAAAADYiVYKLYklRbKnO55RREZmAAQgAQEIAyAAAQUgAQEREQQgAQEOBCABAQIFIAIBDg4FIAEBEWkFIAEBEXUFIAEBEXkFAAESfQ4DAAABAwcBAgUgAgEMDAYgAQERgI0GIAEBEYCVBSACAQgIBiABARGAmQUgAgEcGAYgAQESgJ0EBwIOAgMgAA4EAAEBAgUAAQESQQcHAwISYRJhCAABEoChEYClBSAAEoCpByACAQ4SgKkEBwESZQQHARIUCAABEoCtEoCtCLd6XFYZNOCJCLA/X38R1Qo6AgYOAwYSRQMGEmEDBhJlAwYSFAYgAgEcEk0FAAEBHQ4EAAASYQQAABJlBQABARJlBAAAEhQECAASYQQIABJlBAgAEhQIAQAIAAAAAAAeAQABAFQCFldyYXBOb25FeGNlcHRpb25UaHJvd3MBCAEABwEAAAAACwEABnJlbG9hZAAABQEAAAAAFwEAEkNvcHlyaWdodCDCqSAgMjAyMQAAKQEAJDVlYWM0MzI0LTk1ZDctNGQ4Yi04YzFjLWMwNmE1M2MwNDdkMAAADAEABzEuMC4wLjAAAEcBABouTkVURnJhbWV3b3JrLFZlcnNpb249djQuMAEAVA4URnJhbWV3b3JrRGlzcGxheU5hbWUQLk5FVCBGcmFtZXdvcmsgNAQBAAAAQAEAM1N5c3RlbS5SZXNvdXJjZXMuVG9vbHMuU3Ryb25nbHlUeXBlZFJlc291cmNlQnVpbGRlcgc0LjAuMC4wAABZAQBLTWljcm9zb2Z0LlZpc3VhbFN0dWRpby5FZGl0b3JzLlNldHRpbmdzRGVzaWduZXIuU2V0dGluZ3NTaW5nbGVGaWxlR2VuZXJhdG9yCDExLjAuMC4wAAAIAQACAAAAAAAAAAC0AAAAzsrvvgEAAACRAAAAbFN5c3RlbS5SZXNvdXJjZXMuUmVzb3VyY2VSZWFkZXIsIG1zY29ybGliLCBWZXJzaW9uPTQuMC4wLjAsIEN1bHR1cmU9bmV1dHJhbCwgUHVibGljS2V5VG9rZW49Yjc3YTVjNTYxOTM0ZTA4OSNTeXN0ZW0uUmVzb3VyY2VzLlJ1bnRpbWVSZXNvdXJjZVNldAIAAAAAAAAAAAAAAFBBRFBBRFC0AAAAtAAAAM7K774BAAAAkQAAAGxTeXN0ZW0uUmVzb3VyY2VzLlJlc291cmNlUmVhZGVyLCBtc2NvcmxpYiwgVmVyc2lvbj00LjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWI3N2E1YzU2MTkzNGUwODkjU3lzdGVtLlJlc291cmNlcy5SdW50aW1lUmVzb3VyY2VTZXQCAAAAAAAAAAAAAABQQURQQURQtAAAAAAAAAAM7WaeAAAAAAIAAABPAAAAxDIAAMQUAAAAAAAAAAAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAUlNEU8PnBmssf7pLpbR0dicC7TIBAAAAQzpcVXNlcnNcUy5XSVRUQVlBXERlc2t0b3BccmVsb2FkXG9ialxEZWJ1Z1xyZWxvYWQucGRiADszAAAA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AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";
                    //if (System.IO.File.Exists(@"reload.exe") == false)
                    //{
                    //    byte[] bytes = System.Convert.FromBase64String(stringInBase64);
                    //    System.IO.File.WriteAllBytes(@"reload.exe", bytes); 
                    //}
                    //Process p = new Process();
                    //p.Exited += new EventHandler(p_Reloaded);
                    //p.StartInfo.FileName = @"reload.exe";
                    ////p.StartInfo.Arguments = "5000"; 
                    //p.EnableRaisingEvents = true;
                    //p.Start();

                    #region RELOAD ITSELF
                    var exeName = Process.GetCurrentProcess().MainModule.FileName;
                    ProcessStartInfo startInfo = new ProcessStartInfo(exeName);
                    startInfo.Verb = "runas";
                    startInfo.Arguments = "restart";
                    Process.Start(startInfo);
                    Application.Exit();
                    #endregion
                }

            }
            else
            {
                Exit();
            }

        }
        private void p_Reloaded(object sender, EventArgs e)
        {
            File.Delete(@"reload.exe");
            base.Dispose(true);
            Environment.Exit(0);
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            if (frmLogs != null)
            {
                this.panel_main.Controls.Remove(frmLogs);
                frmLogs.Close();
                frmLogs = null;
            }

            #region Ole Code Case
            if (frmDevice != null)
            {
                //this.panel_main.Controls.Remove(frmDevice);
                //frmDevice.Close();
                //frmDevice = null;
            }

            if (frmActivity != null)
            {
                //this.panel_main.Controls.Remove(frmActivity);
                //frmActivity.Close();
                //frmActivity = null;
            }
            #endregion

            classGlobal.statusIn_OUT = "";

            LOAD_FRMLOGS();

            rbtnIN.ForeColor = Color.Blue;
            rbtnOUT.ForeColor = Color.Blue;

            //++ time attendance
            if (classGlobal.enableTimeAttendance == true)
            {
                bool timeAttRunning = false;
                foreach (var process1 in Process.GetProcessesByName("TimeAttendance"))
                {
                    if (process1.ProcessName.ToString().ToLower().Equals("timeattendance"))
                    {
                        timeAttRunning = true;
                        break;
                    }                                                               
                }

                if (timeAttRunning == false)
                {
                    string path = "TimeAttendance.exe";
                    if (File.Exists(path))
                    {
                        ProcessStartInfo p = new ProcessStartInfo(path, null);
                        Process process = Process.Start(p);
                    }
                    else
                    {
                        MessageBox.Show("ไม่พบไฟล์ " + path, "Time Attendance", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
 
            }
            //-- time attendance
        }

        private void btnSelectWC_Click(object sender, EventArgs e)
        {
            int WebCamIndex = -1;
            WebcamDevice wc = new WebcamDevice();
            wc.ShowDialog();
            WebCamIndex = classGlobal.SelectedWebcamDevice;
        }


        private void pbIcon_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized; 
        }
        private void btnSetting_Click(object sender, EventArgs e)
        {
            if (classGlobal.FactoryVersion == true)
            {
                SettingMenu f = new SettingMenu();
                f.ShowDialog();

                if (frmLogs != null)
                {
                    this.panel_main.Controls.Remove(frmLogs);
                    frmLogs.Close();
                    frmLogs = null;
                }
                LOAD_FRMLOGS();   
 
            }
            else
            {
                frmSettingType = new frmSettingType();
                frmSettingType.ShowDialog();

                if (frmLogs != null)
                {
                    this.panel_main.Controls.Remove(frmLogs);
                    frmLogs.Close();
                    frmLogs = null;
                }
                LOAD_FRMLOGS();
            }

            classGlobal.personID = "";
            classGlobal.personName = "";
            classGlobal.statusIn_OUT = "";
            rbtnIN.ForeColor = Color.Blue;
            rbtnOUT.ForeColor = Color.Blue;

        }

        private void btnView_Click(object sender, EventArgs e)
        {
            if (classGlobal.enableAuthenticationConfirm == false)
            {
                frmV = new frmView();
                frmV.ShowDialog();
                LOAD_FRMLOGS();    //// ++ โหลดข้อมุลอีกครั้ง เพื่อได้ข้อมูลล่าสุด
                return;
            }

            frmAuthenReport fA = new frmAuthenReport();
            fA.ShowDialog();
            if (classGlobal.bAuthenReportPass == true)
            {
                frmV = new frmView();
                frmV.ShowDialog();
                LOAD_FRMLOGS();
            }
            classGlobal.bAuthenReportPass = false;

        }
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (classGlobal.enableAuthenticationConfirm == false)
            {
                frmExport f1 = new frmExport();
                f1.ShowDialog();
                return;
            }

            frmAuthenReport fA = new frmAuthenReport();
            fA.ShowDialog();
            if (classGlobal.bAuthenReportPass == true)
            {
                frmExport f = new frmExport();
                f.ShowDialog();
            }
            classGlobal.bAuthenReportPass = false;
           
        }
        private void btnMoney_Click(object sender, EventArgs e)
        {
            if (classGlobal.enableAuthenticationConfirm == false)
            {
                FormChargeReport f1 = new FormChargeReport();
                f1.ShowDialog();
                return;
            }

            frmAuthenReport fA = new frmAuthenReport();
            fA.ShowDialog();
            if (classGlobal.bAuthenReportPass == true)
            {
                FormChargeReport f = new FormChargeReport();
                f.ShowDialog();
            }
            classGlobal.bAuthenReportPass = false;
        }

        private void panel_top_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);

                FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;      
            }
        }
        private void UpdateSynsCompleted(int id) 
        {
            try
            {
                //++ 
                string query = String.Format("UPDATE tbl_visitor SET upload='{0}' WHERE id={1}", "1", id);

                if (classGlobal.databaseType == "acc")
                {
                    OleDbCommand command = new OleDbCommand(query, classGlobal.conn);
                    command.ExecuteNonQuery();
                    command.Dispose();
                    command = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    NpgsqlCommand command = new NpgsqlCommand(query, classGlobal.connP);
                    command.ExecuteNonQuery();
                    command.Dispose();
                    command = null;
                }
                else
                {
                    // do nothing
                }
                //--
            }
            catch
            {
                //--
            }
        }
        private string GetTypeVisitorText(string typeid)
        {
            try
            {
                if (typeid == "")
                {
                    return "ไม่ระบุ";
                }
                else
                {
                    DataTable _dt = new DataTable("_dt");

                    if (classGlobal.databaseType == "acc")
                    {
                        OleDbCommand cmd = new OleDbCommand("SELECT typename FROM tbl_type WHERE typeid=" + Int32.Parse(typeid), classGlobal.conn);
                        OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                        da.Fill(_dt);
                        da.Dispose();
                        da = null;
                        typeid = _dt.Rows[0][0].ToString();
                        _dt.Dispose();
                        _dt = null;
                    }
                    else if (classGlobal.databaseType == "psql")
                    {
                        NpgsqlCommand cmd = new NpgsqlCommand("SELECT typename FROM tbl_type WHERE typeid=" + Int32.Parse(typeid), classGlobal.connP);
                        NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                        da.Fill(_dt);
                        da.Dispose();
                        da = null;
                        typeid = _dt.Rows[0][0].ToString();
                        _dt.Dispose();
                        _dt = null;
                    }
                    else
                    {

                    }
                   
                    return typeid;
                }

            }
            catch
            {
                return "ไม่ระบุ";
            }
        }
        public string DataTableToJSONWithStringBuilder(DataTable table)
        {
            var JSONString = new StringBuilder();
            if (table.Rows.Count > 0)
            {
                JSONString.Append("[");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    JSONString.Append("{");
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        if (j < table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == table.Columns.Count - 1)
                        {
                            JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\"");
                        }
                    }
                    if (i == table.Rows.Count - 1)
                    {
                        JSONString.Append("}");
                    }
                    else
                    {
                        JSONString.Append("},");
                    }
                }
                JSONString.Append("]");
            }
            return JSONString.ToString();
        }


        private void btnWEBCAM_Click(object sender, EventArgs e)
        {
            WebcamDevice f = new WebcamDevice();
            f.ShowDialog();
        }

        private void btnExit1_Click(object sender, EventArgs e)
        {
            if (classGlobal.userId != "")
            {
                frmLogOff f = new frmLogOff();
                f.ShowDialog();
                if (f.cancel == true)
                    return;

                if (classGlobal.bool_Exit == true)
                {
                    Exit();
                }
                else
                {
                    clsXML cx = new clsXML();
                    //cx.ModifyElement("root", "Alive", "-", @"config.xml");
                    cx = null;

                    File.WriteAllText(classGlobal.settingFile + @"/session", "-");

                    //string stringInBase64 = "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QnJvd3NhYmxlU3RhdGUAc2V0X1dpbmRvd1N0YXRlAEZvcm1XaW5kb3dTdGF0ZQBTVEFUaHJlYWRBdHRyaWJ1dGUAQ29tcGlsZXJHZW5lcmF0ZWRBdHRyaWJ1dGUAR3VpZEF0dHJpYnV0ZQBHZW5lcmF0ZWRDb2RlQXR0cmlidXRlAERlYnVnZ2VyTm9uVXNlckNvZGVBdHRyaWJ1dGUARGVidWdnYWJsZUF0dHJpYnV0ZQBFZGl0b3JCcm93c2FibGVBdHRyaWJ1dGUAQ29tVmlzaWJsZUF0dHJpYnV0ZQBBc3NlbWJseVRpdGxlQXR0cmlidXRlAEFzc2VtYmx5VHJhZGVtYXJrQXR0cmlidXRlAFRhcmdldEZyYW1ld29ya0F0dHJpYnV0ZQBBc3NlbWJseUZpbGVWZXJzaW9uQXR0cmlidXRlAEFzc2VtYmx5Q29uZmlndXJhdGlvbkF0dHJpYnV0ZQBBc3NlbWJseURlc2NyaXB0aW9uQXR0cmlidXRlAENvbXBpbGF0aW9uUmVsYXhhdGlvbnNBdHRyaWJ1dGUAQXNzZW1ibHlQcm9kdWN0QXR0cmlidXRlAEFzc2VtYmx5Q29weXJpZ2h0QXR0cmlidXRlAEFzc2VtYmx5Q29tcGFueUF0dHJpYnV0ZQBSdW50aW1lQ29tcGF0aWJpbGl0eUF0dHJpYnV0ZQB2YWx1ZQByZWxvYWQuZXhlAHNldF9DbGllbnRTaXplAFN5c3RlbS5UaHJlYWRpbmcAU3lzdGVtLlJ1bnRpbWUuVmVyc2lvbmluZwBkaXNwb3NpbmcAU3lzdGVtLkRyYXdpbmcAU3lzdGVtLkNvbXBvbmVudE1vZGVsAENvbnRhaW5lckNvbnRyb2wAUHJvZ3JhbQBTeXN0ZW0ARm9ybQByZXNvdXJjZU1hbgBNYWluAHNldF9TaG93SWNvbgBBcHBsaWNhdGlvbgBTeXN0ZW0uQ29uZmlndXJhdGlvbgBTeXN0ZW0uR2xvYmFsaXphdGlvbgBTeXN0ZW0uUmVmbGVjdGlvbgBSdW4AQ3VsdHVyZUluZm8AU2xlZXAAc2V0X1Nob3dJblRhc2tiYXIAc2VuZGVyAGdldF9SZXNvdXJjZU1hbmFnZXIARXZlbnRIYW5kbGVyAFN5c3RlbS5Db2RlRG9tLkNvbXBpbGVyAElDb250YWluZXIALmN0b3IALmNjdG9yAFN5c3RlbS5EaWFnbm9zdGljcwBTeXN0ZW0uUnVudGltZS5JbnRlcm9wU2VydmljZXMAU3lzdGVtLlJ1bnRpbWUuQ29tcGlsZXJTZXJ2aWNlcwBTeXN0ZW0uUmVzb3VyY2VzAHJlbG9hZC5Gb3JtMS5yZXNvdXJjZXMAcmVsb2FkLlByb3BlcnRpZXMuUmVzb3VyY2VzLnJlc291cmNlcwBEZWJ1Z2dpbmdNb2RlcwByZWxvYWQuUHJvcGVydGllcwBFbmFibGVWaXN1YWxTdHlsZXMAU2V0dGluZ3MARXZlbnRBcmdzAFN5c3RlbS5XaW5kb3dzLkZvcm1zAHNldF9BdXRvU2NhbGVEaW1lbnNpb25zAFByb2Nlc3MAY29tcG9uZW50cwBPYmplY3QARXhpdABnZXRfRGVmYXVsdABTZXRDb21wYXRpYmxlVGV4dFJlbmRlcmluZ0RlZmF1bHQARW52aXJvbm1lbnQASW5pdGlhbGl6ZUNvbXBvbmVudABTdGFydABTdXNwZW5kTGF5b3V0AFJlc3VtZUxheW91dABzZXRfVGV4dABnZXRfQXNzZW1ibHkAAB1XAGEAYwBWAGkAcwBpAHQAbwByAC4AZQB4AGUAAAtGAG8AcgBtADEAADdyAGUAbABvAGEAZAAuAFAAcgBvAHAAZQByAHQAaQBlAHMALgBSAGUAcwBvAHUAcgBjAGUAcwAAACEidmE+ButLpro5bDEaMJUABCABAQgDIAABBSABARERBCABAQ4EIAEBAgUgAgEODgUgAQERaQQAAQEIBQABEnkOAwcBAgUgAgEMDAYgAQERgIkGIAEBEYCRBSACAQgIBiABARGAlQYgAQERgJkFIAIBHBgGIAEBEoCdAwAAAQQAAQECBQABARJBBwcDAhJhEmEIAAESgKURgKkFIAASgK0HIAIBDhKArQQHARJlBAcBEhQIAAESgLESgLEIt3pcVhk04IkIsD9ffxHVCjoDBhJFAwYSYQMGEmUDBhIUBiACARwSTQQAABJhBAAAEmUFAAEBEmUEAAASFAQIABJhBAgAEmUECAASFAgBAAgAAAAAAB4BAAEAVAIWV3JhcE5vbkV4Y2VwdGlvblRocm93cwEIAQAHAQAAAAALAQAGcmVsb2FkAAAFAQAAAAAXAQASQ29weXJpZ2h0IMKpICAyMDIxAAApAQAkNGNjZWVmZGMtY2Y4ZC00MzYwLTgzNTctMzAxNTQ2YjZmOGMzAAAMAQAHMS4wLjAuMAAARwEAGi5ORVRGcmFtZXdvcmssVmVyc2lvbj12NC4wAQBUDhRGcmFtZXdvcmtEaXNwbGF5TmFtZRAuTkVUIEZyYW1ld29yayA0BAEAAABAAQAzU3lzdGVtLlJlc291cmNlcy5Ub29scy5TdHJvbmdseVR5cGVkUmVzb3VyY2VCdWlsZGVyBzQuMC4wLjAAAFkBAEtNaWNyb3NvZnQuVmlzdWFsU3R1ZGlvLkVkaXRvcnMuU2V0dGluZ3NEZXNpZ25lci5TZXR0aW5nc1NpbmdsZUZpbGVHZW5lcmF0b3IIMTEuMC4wLjAAAAgBAAIAAAAAAAC0AAAAzsrvvgEAAACRAAAAbFN5c3RlbS5SZXNvdXJjZXMuUmVzb3VyY2VSZWFkZXIsIG1zY29ybGliLCBWZXJzaW9uPTQuMC4wLjAsIEN1bHR1cmU9bmV1dHJhbCwgUHVibGljS2V5VG9rZW49Yjc3YTVjNTYxOTM0ZTA4OSNTeXN0ZW0uUmVzb3VyY2VzLlJ1bnRpbWVSZXNvdXJjZVNldAIAAAAAAAAAAAAAAFBBRFBBRFC0AAAAtAAAAM7K774BAAAAkQAAAGxTeXN0ZW0uUmVzb3VyY2VzLlJlc291cmNlUmVhZGVyLCBtc2NvcmxpYiwgVmVyc2lvbj00LjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWI3N2E1YzU2MTkzNGUwODkjU3lzdGVtLlJlc291cmNlcy5SdW50aW1lUmVzb3VyY2VTZXQCAAAAAAAAAAAAAABQQURQQURQtAAAAAAAAADFEo7zAAAAAAIAAABPAAAAcDIAAHAUAAAAAAAAAAAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAUlNEUy3xe3t92L1DsFBAPFL5DtQBAAAAQzpcVXNlcnNcUy5XSVRUQVlBXERlc2t0b3BccmVsb2FkXG9ialxEZWJ1Z1xyZWxvYWQucGRiAOcyAAAAAAAAAAAAAAEzAAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAADzMgAAAAAAAAAAAAAAAF9Db3JFeGVNYWluAG1zY29yZWUuZGxsAAAAAAAA/yUAIEAA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AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";
                    string stringInBase64 = "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cmUAcmVzb3VyY2VDdWx0dXJlAEFwcGxpY2F0aW9uU2V0dGluZ3NCYXNlAERpc3Bvc2UARWRpdG9yQnJvd3NhYmxlU3RhdGUAc2V0X1dpbmRvd1N0YXRlAEZvcm1XaW5kb3dTdGF0ZQBTVEFUaHJlYWRBdHRyaWJ1dGUAQ29tcGlsZXJHZW5lcmF0ZWRBdHRyaWJ1dGUAR3VpZEF0dHJpYnV0ZQBHZW5lcmF0ZWRDb2RlQXR0cmlidXRlAERlYnVnZ2VyTm9uVXNlckNvZGVBdHRyaWJ1dGUARGVidWdnYWJsZUF0dHJpYnV0ZQBFZGl0b3JCcm93c2FibGVBdHRyaWJ1dGUAQ29tVmlzaWJsZUF0dHJpYnV0ZQBBc3NlbWJseVRpdGxlQXR0cmlidXRlAEFzc2VtYmx5VHJhZGVtYXJrQXR0cmlidXRlAFRhcmdldEZyYW1ld29ya0F0dHJpYnV0ZQBBc3NlbWJseUZpbGVWZXJzaW9uQXR0cmlidXRlAEFzc2VtYmx5Q29uZmlndXJhdGlvbkF0dHJpYnV0ZQBBc3NlbWJseURlc2NyaXB0aW9uQXR0cmlidXRlAENvbXBpbGF0aW9uUmVsYXhhdGlvbnNBdHRyaWJ1dGUAQXNzZW1ibHlQcm9kdWN0QXR0cmlidXRlAEFzc2VtYmx5Q29weXJpZ2h0QXR0cmlidXRlAEFzc2VtYmx5Q29tcGFueUF0dHJpYnV0ZQBSdW50aW1lQ29tcGF0aWJpbGl0eUF0dHJpYnV0ZQB2YWx1ZQByZWxvYWQuZXhlAHNldF9DbGllbnRTaXplAFN5c3RlbS5SdW50aW1lLlZlcnNpb25pbmcAVG9TdHJpbmcAZGlzcG9zaW5nAHdhaXRpbmcAU3lzdGVtLkRyYXdpbmcAX2FyZwBTeXN0ZW0uQ29tcG9uZW50TW9kZWwAQ29udGFpbmVyQ29udHJvbABQcm9ncmFtAFN5c3RlbQBGb3JtAHJlc291cmNlTWFuAE1haW4Ac2V0X1Nob3dJY29uAEFwcGxpY2F0aW9uAFN5c3RlbS5Db25maWd1cmF0aW9uAFN5c3RlbS5HbG9iYWxpemF0aW9uAFN5c3RlbS5SZWZsZWN0aW9uAFJ1bgBDdWx0dXJlSW5mbwBzZXRfU2hvd0luVGFza2JhcgBzZW5kZXIAZ2V0X1Jlc291cmNlTWFuYWdlcgBFdmVudEhhbmRsZXIAU3lzdGVtLkNvZGVEb20uQ29tcGlsZXIASUNvbnRhaW5lcgAuY3RvcgAuY2N0b3IAU3lzdGVtLkRpYWdub3N0aWNzAFN5c3RlbS5SdW50aW1lLkludGVyb3BTZXJ2aWNlcwBTeXN0ZW0uUnVudGltZS5Db21waWxlclNlcnZpY2VzAFN5c3RlbS5SZXNvdXJjZXMAcmVsb2FkLkZvcm0xLnJlc291cmNlcwByZWxvYWQuUHJvcGVydGllcy5SZXNvdXJjZXMucmVzb3VyY2VzAERlYnVnZ2luZ01vZGVzAHJlbG9hZC5Qcm9wZXJ0aWVzAEVuYWJsZVZpc3VhbFN0eWxlcwBTZXR0aW5ncwBFdmVudEFyZ3MAYXJncwBTeXN0ZW0uV2luZG93cy5Gb3JtcwBzZXRfQXV0b1NjYWxlRGltZW5zaW9ucwBQcm9jZXNzAGNvbXBvbmVudHMAT2JqZWN0AEV4aXQAZ2V0X0RlZmF1bHQAU2V0Q29tcGF0aWJsZVRleHRSZW5kZXJpbmdEZWZhdWx0AEluaXRpYWxpemVDb21wb25lbnQAU3RhcnQAU3VzcGVuZExheW91dABSZXN1bWVMYXlvdXQAc2V0X1RleHQAZ2V0X0Fzc2VtYmx5AAAAAAAJMgAwADAAMAAAHVcAYQBjAFYAaQBzAGkAdABvAHIALgBlAHgAZQAAC0YAbwByAG0AMQAAN3IAZQBsAG8AYQBkAC4AUAByAG8AcABlAHIAdABpAGUAcwAuAFIAZQBzAG8AdQByAGMAZQBzAAAAAADYiVYKLYklRbKnO55RREZmAAQgAQEIAyAAAQUgAQEREQQgAQEOBCABAQIFIAIBDg4FIAEBEWkFIAEBEXUFIAEBEXkFAAESfQ4DAAABAwcBAgUgAgEMDAYgAQERgI0GIAEBEYCVBSACAQgIBiABARGAmQUgAgEcGAYgAQESgJ0EBwIOAgMgAA4EAAEBAgUAAQESQQcHAwISYRJhCAABEoChEYClBSAAEoCpByACAQ4SgKkEBwESZQQHARIUCAABEoCtEoCtCLd6XFYZNOCJCLA/X38R1Qo6AgYOAwYSRQMGEmEDBhJlAwYSFAYgAgEcEk0FAAEBHQ4EAAASYQQAABJlBQABARJlBAAAEhQECAASYQQIABJlBAgAEhQIAQAIAAAAAAAeAQABAFQCFldyYXBOb25FeGNlcHRpb25UaHJvd3MBCAEABwEAAAAACwEABnJlbG9hZAAABQEAAAAAFwEAEkNvcHlyaWdodCDCqSAgMjAyMQAAKQEAJDVlYWM0MzI0LTk1ZDctNGQ4Yi04YzFjLWMwNmE1M2MwNDdkMAAADAEABzEuMC4wLjAAAEcBABouTkVURnJhbWV3b3JrLFZlcnNpb249djQuMAEAVA4URnJhbWV3b3JrRGlzcGxheU5hbWUQLk5FVCBGcmFtZXdvcmsgNAQBAAAAQAEAM1N5c3RlbS5SZXNvdXJjZXMuVG9vbHMuU3Ryb25nbHlUeXBlZFJlc291cmNlQnVpbGRlcgc0LjAuMC4wAABZAQBLTWljcm9zb2Z0LlZpc3VhbFN0dWRpby5FZGl0b3JzLlNldHRpbmdzRGVzaWduZXIuU2V0dGluZ3NTaW5nbGVGaWxlR2VuZXJhdG9yCDExLjAuMC4wAAAIAQACAAAAAAAAAAC0AAAAzsrvvgEAAACRAAAAbFN5c3RlbS5SZXNvdXJjZXMuUmVzb3VyY2VSZWFkZXIsIG1zY29ybGliLCBWZXJzaW9uPTQuMC4wLjAsIEN1bHR1cmU9bmV1dHJhbCwgUHVibGljS2V5VG9rZW49Yjc3YTVjNTYxOTM0ZTA4OSNTeXN0ZW0uUmVzb3VyY2VzLlJ1bnRpbWVSZXNvdXJjZVNldAIAAAAAAAAAAAAAAFBBRFBBRFC0AAAAtAAAAM7K774BAAAAkQAAAGxTeXN0ZW0uUmVzb3VyY2VzLlJlc291cmNlUmVhZGVyLCBtc2NvcmxpYiwgVmVyc2lvbj00LjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWI3N2E1YzU2MTkzNGUwODkjU3lzdGVtLlJlc291cmNlcy5SdW50aW1lUmVzb3VyY2VTZXQCAAAAAAAAAAAAAABQQURQQURQtAAAAAAAAAAM7WaeAAAAAAIAAABPAAAAxDIAAMQUAAAAAAAAAAAAAAAAAAAQAAAAAAAAAAAAAAAAAAAAUlNEU8PnBmssf7pLpbR0dicC7TIBAAAAQzpcVXNlcnNcUy5XSVRUQVlBXERlc2t0b3BccmVsb2FkXG9ialxEZWJ1Z1xyZWxvYWQucGRiADszAAAA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AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";
                    if (System.IO.File.Exists(@"reload.exe") == false)
                    {
                        byte[] bytes = System.Convert.FromBase64String(stringInBase64);
                        System.IO.File.WriteAllBytes(@"reload.exe", bytes);
                    }
                    Process p = new Process();
                    p.Exited += new EventHandler(p_Reloaded);
                    p.StartInfo.FileName = @"reload.exe";
                    //p.StartInfo.Arguments = "5000";
                    p.EnableRaisingEvents = true;
                    p.Start();
                }

            }
            else
            {
                Exit();
            }

        }

        Image Base64ToImage(string base64String)
        {
            try
            {
                // Convert base 64 string to byte[]
                byte[] imageBytes = Convert.FromBase64String(base64String);
                // Convert byte[] to Image
                using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                {
                    Image image = Image.FromStream(ms, true);
                    return image;
                }
            }
            catch
            {
                return null;
            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            foreach (var process in Process.GetProcessesByName("HIKEngine"))
            {
                process.Kill();
                break;
            }

            string ipcamStr = "";
            String loginInfo = "";
            String addressInfo = "";
            String[] readIpCmmFiles = System.IO.File.ReadAllLines(System.AppDomain.CurrentDomain.BaseDirectory + "configipcam.txt");
            readIpCmmFiles = readIpCmmFiles.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            foreach (String data in readIpCmmFiles)
            {
                if (data.IndexOf("[LOGIN]") > -1)
                {
                    loginInfo = data.Replace("[LOGIN]", "");
                }
                else
                {
                    if (!data.Substring(0,1).Equals(";"))
                    {
                        addressInfo = data;
                    }
                }
            }
            ipcamStr = loginInfo + "###" + addressInfo;

            Process p = new Process();
            p.Exited += new EventHandler(IPCam_Exited);
            p.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + @"IPCAM_Engine\HIKEngine.exe";
            p.StartInfo.Arguments = ipcamStr; 
            p.EnableRaisingEvents = true;
            p.Start();
        
        }
        private void IPCam_Exited(object sender, EventArgs e)
        {
            //
        }

        private void rbtnIN_Click(object sender, EventArgs e)
        {
            var culture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
            var language = InputLanguage.FromCulture(culture);
            InputLanguage.CurrentInputLanguage = language;

            classGlobal.appointMentSelectedId = ""; 

            classGlobal.personID = "";
            classGlobal.personName = "";

            //++++++++++++++++++++++++++++++++//

            this.panel_main.Controls.Remove(frmLogs);

            lbTime.Focus();
            classGlobal.statusIn_OUT = "IN";
            rbtnIN.ForeColor = Color.Gray;
            rbtnOUT.ForeColor = Color.Blue;
            if (classGlobal.FactoryVersion == true)  // เวอร์ชั่นโรงงาน
            {                
                if (frmActivity != null)
                {
                    this.panel_main.Controls.Remove(frmActivity);
                    frmActivity.Close();
                    frmActivity = null;
                }
                frmActivity = new FormSetting.FormActivity();
                frmActivity.TopLevel = false;
                frmActivity.AutoScroll = false;
                this.panel_main.Controls.Add(frmActivity);
                AnimateWindow(frmActivity.Handle, 500, AnimateWindowFlags.AW_CENTER);
                frmActivity.Show();
            }
            else  // เวอร์ชั่นหมู่บ้าน
            {
                if (frmDevice != null)
                {
                    this.panel_main.Controls.Remove(frmDevice);
                    frmDevice.Close();
                    frmDevice = null;
                }
                frmDevice = new frmHWDevices();
                frmDevice.TopLevel = false;
                frmDevice.AutoScroll = false;
                this.panel_main.Controls.Add(frmDevice);
                AnimateWindow(frmDevice.Handle, 500, AnimateWindowFlags.AW_CENTER);
                frmDevice.Show();
            }

            //---------------------------//

            //fh.Close();
        }

        private void rbtnOUT_Click(object sender, EventArgs e)
        {
            var culture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
            var language = InputLanguage.FromCulture(culture);
            InputLanguage.CurrentInputLanguage = language;

            classGlobal.appointMentSelectedId = "";

            classGlobal.personID = "";
            classGlobal.personName = "";

            //++++++++++++++++++++++++++++++++//

            this.panel_main.Controls.Remove(frmLogs);

            lbTime.Focus();
            classGlobal.statusIn_OUT = "OUT";
            rbtnIN.ForeColor = Color.Blue;
            rbtnOUT.ForeColor = Color.Gray;
            if (classGlobal.FactoryVersion == true)  // เวอร์ชั่นโรงงาน
            {
                if (frmActivity != null)
                {
                    this.panel_main.Controls.Remove(frmActivity);
                    frmActivity.Close();
                    frmActivity = null;
                }
                frmActivity = new FormSetting.FormActivity();
                frmActivity.TopLevel = false;
                frmActivity.AutoScroll = false;
                this.panel_main.Controls.Add(frmActivity);
                AnimateWindow(frmActivity.Handle, 500, AnimateWindowFlags.AW_CENTER);
                frmActivity.Show();
            }
            else  // เวอร์ชั่นหมู่บ้าน
            {
                if (frmDevice != null)
                {
                    this.panel_main.Controls.Remove(frmDevice);
                    frmDevice.Close();
                    frmDevice = null;
                }
                frmDevice = new frmHWDevices();
                frmDevice.TopLevel = false;
                frmDevice.AutoScroll = false;
                this.panel_main.Controls.Add(frmDevice);
                AnimateWindow(frmDevice.Handle, 500, AnimateWindowFlags.AW_CENTER);
                frmDevice.Show();
            }

            //-------------------------------//

            //fh.Close();

        }

        private void PRINT_SLIP_THERMAL(string text, string text1, string text2, string text3, Bitmap qr)
        {

            string[] splitNewLine = new string[] { "\r\n" };
            string[] tempText = text.Split(splitNewLine, StringSplitOptions.None);
            tempText[6] = "เลขประจำตัว : #############";
            tempText[5] = "ชื่อ-สกุล : ###### ######";
            text = String.Join("\r\n", tempText);

            //++ ซ่อน เลขประจำตัว / ชื่อ-สกุล
            List<string> lists = new List<string>();
            foreach (string items in tempText)
            {
                if (!items.Contains("เลขประจำตัว") && !items.Contains("ชื่อ-สกุล"))
                    lists.Add(items);
            }
            lists.Add("");  // ต้องใส่ มีผลต่อ temaplte รายการอื่น
            lists.Add("");  // ต้องใส่ มีผลต่อ temaplte รายการอื่น
                            //text = String.Join("\r\n", lists);    // => เอา comment ออก คือการซ่อน บรรทัด เลขประจำตัว / ชื่อ-สกุล 
                            //-- ซ่อน เลขประจำตัว / ชื่อ-สกุล

            Font font = new Font("Tahoma", 10);
            Font fontBorder = new Font("Tahoma", 10, FontStyle.Bold);

            // ++ line text length per line
            //int nLineExpand = 0;
            string txtInfoLength = "";
            string[] stringSeparators = new string[] { "\r\n" };
            string[] _txt = text.Split(stringSeparators, StringSplitOptions.None);

            txtInfoLength = _txt[9];  // จากบริษัท
            if (_txt[9].Length > 45)  // 
            {
                string[] arrCompany = new string[0];
                string str = txtInfoLength.Replace("จากบริษัท : ", "");
                int chunkSize = 32;
                int stringLength = str.Length;
                for (int i = 0; i < stringLength; i += chunkSize)
                {
                    if (i + chunkSize > stringLength) chunkSize = stringLength - i;
                    Array.Resize(ref arrCompany, arrCompany.Length + 1);

                    if (i == 0)
                        arrCompany[arrCompany.Length - 1] = "จากบริษัท : " + str.Substring(i, chunkSize);
                    else
                        arrCompany[arrCompany.Length - 1] = "                 " + str.Substring(i, chunkSize);

                }
                _txt[9] = String.Join(Environment.NewLine, arrCompany);
                text = String.Join(Environment.NewLine, _txt);
            }

            txtInfoLength = _txt[10];  // ติดต่อเรื่อง
            if (_txt[10].Length > 45)
            {
                string[] arrCompany = new string[0];
                string str = txtInfoLength.Replace("ติดต่อเรื่อง : ", "");
                int chunkSize = 30;
                int stringLength = str.Length;
                for (int i = 0; i < stringLength; i += chunkSize)
                {
                    if (i + chunkSize > stringLength) chunkSize = stringLength - i;
                    Array.Resize(ref arrCompany, arrCompany.Length + 1);

                    if (i == 0)
                        arrCompany[arrCompany.Length - 1] = "ติดต่อเรื่อง : " + str.Substring(i, chunkSize);
                    else
                        arrCompany[arrCompany.Length - 1] = "                 " + str.Substring(i, chunkSize);

                }
                _txt[10] = String.Join(Environment.NewLine, arrCompany);
                text = String.Join(Environment.NewLine, _txt);
            }

            txtInfoLength = _txt[11];  // ผู้รับการติดต่อ
            if (_txt[11].Length > 45)
            {
                string[] arrCompany = new string[0];
                string str = txtInfoLength.Replace("ผู้รับการติดต่อ : ", "");
                int chunkSize = 28;
                int stringLength = str.Length;
                for (int i = 0; i < stringLength; i += chunkSize)
                {
                    if (i + chunkSize > stringLength) chunkSize = stringLength - i;
                    Array.Resize(ref arrCompany, arrCompany.Length + 1);

                    if (i == 0)
                        arrCompany[arrCompany.Length - 1] = "ผู้รับการติดต่อ : " + str.Substring(i, chunkSize);
                    else
                        arrCompany[arrCompany.Length - 1] = "                     " + str.Substring(i, chunkSize);

                }
                _txt[11] = String.Join(Environment.NewLine, arrCompany);
                text = String.Join(Environment.NewLine, _txt);
            }

            txtInfoLength = _txt[12];  // แผนก
            if (_txt[12].Length > 45)
            {
                string[] arrCompany = new string[0];
                string str = txtInfoLength.Replace("แผนก : ", "");
                int chunkSize = 35;
                int stringLength = str.Length;
                for (int i = 0; i < stringLength; i += chunkSize)
                {
                    if (i + chunkSize > stringLength) chunkSize = stringLength - i;
                    Array.Resize(ref arrCompany, arrCompany.Length + 1);

                    if (i == 0)
                        arrCompany[arrCompany.Length - 1] = "แผนก : " + str.Substring(i, chunkSize);
                    else
                        arrCompany[arrCompany.Length - 1] = "          " + str.Substring(i, chunkSize);

                }
                _txt[12] = String.Join(Environment.NewLine, arrCompany);
                text = String.Join(Environment.NewLine, _txt);
            }

            txtInfoLength = _txt[13];  // สถานที่ติดต่อ
            if (_txt[13].Length > 45)
            {
                string[] arrCompany = new string[0];
                string str = txtInfoLength.Replace("สถานที่ติดต่อ : ", "");
                int chunkSize = 28;
                int stringLength = str.Length;
                for (int i = 0; i < stringLength; i += chunkSize)
                {
                    if (i + chunkSize > stringLength) chunkSize = stringLength - i;
                    Array.Resize(ref arrCompany, arrCompany.Length + 1);

                    if (i == 0)
                        arrCompany[arrCompany.Length - 1] = "สถานที่ติดต่อ : " + str.Substring(i, chunkSize);
                    else
                        arrCompany[arrCompany.Length - 1] = "                    " + str.Substring(i, chunkSize);

                }
                _txt[13] = String.Join(Environment.NewLine, arrCompany);
                text = String.Join(Environment.NewLine, _txt);
            }


            txtInfoLength = _txt[14];  // ข้อมูลอื่นๆ
            if (_txt[14].Length > 45)
            {
                string[] arrCompany = new string[0];
                string str = txtInfoLength.Replace("ข้อมูลอื่นๆ : ", "");
                int chunkSize = 25;
                int stringLength = str.Length;
                for (int i = 0; i < stringLength; i += chunkSize)
                {
                    if (i + chunkSize > stringLength) chunkSize = stringLength - i;
                    Array.Resize(ref arrCompany, arrCompany.Length + 1);

                    if (i == 0)
                        arrCompany[arrCompany.Length - 1] = "ข้อมูลอื่นๆ : " + str.Substring(i, chunkSize);
                    else
                        arrCompany[arrCompany.Length - 1] = "                 " + str.Substring(i, chunkSize);

                }
                _txt[14] = String.Join(Environment.NewLine, arrCompany);
                text = String.Join(Environment.NewLine, _txt);
            }
            // -- line text length per line

            text = text + Environment.NewLine + text1;

            int lineOfEtc = 0;
            string[] _text = text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            for (lineOfEtc = 0; lineOfEtc < _text.Length; lineOfEtc++)
            {
                if (_text[lineOfEtc].Replace(" ", "").Equals(""))
                    break;
            }

            int logoHeight = 0;
            Bitmap bmp = null;
            StringFormat format;
            RectangleF rect;
            int limit_lineWidth = 0;
            int allHeight = 0;
            System.Drawing.Printing.PrintDocument p = new System.Drawing.Printing.PrintDocument();
            p.PrintPage += delegate (object sender1, System.Drawing.Printing.PrintPageEventArgs e1)
            {
                string slip_factory_region = "en";
                if (slip_factory_region == "en")
                {
                    text = text.Replace("ใบผ่านเข้า-ออก ผู้มาติดต่อ  หมายเลข", "Visitor Pass No.");
                    text = text.Replace("เวลาเข้า", "Time in");
                    text = text.Replace("วันที่", "Date");
                    text = text.Replace("ประเภทผู้มาติดต่อ", "Visitor’s Type");
                    text = text.Replace("จำนวน", "No. of visitors").Replace("คน", "");
                    text = text.Replace("ชื่อ-สกุล", "First-Last Name");
                    text = text.Replace("เลขประจำตัว", "ID");
                    text = text.Replace("ทะเบียนรถ", "Vehicle No.");
                    text = text.Replace("ประเภทรถ", "Vehicle type");
                    text = text.Replace("จากบริษัท", "Visitor’s company");
                    text = text.Replace("ติดต่อเรื่อง", "Purpose");
                    text = text.Replace("ผู้รับการติดต่อ", "Staff");
                    text = text.Replace("แผนก", "Dept.");
                    text = text.Replace("สถานที่ติดต่อ", "Destination");
                    text = text.Replace("ข้อมูลอื่นๆ", "Other");
                    text = text.Replace("ทรัพย์สินที่นำเข้า", "Material in");
                    text = text.Replace("ทรัพย์สินที่ฝาก รปภ", "Material at security");
                    text = text.Replace("มี (ระบุ)...................................................", "If yes, please specify........................................");
                    text = text.Replace("ไม่มี", "No");
                    text = text.Replace("มี", "Yes");

                    text1 = text1.Replace("ทรัพย์สินที่นำเข้า", "Material in");
                    text1 = text1.Replace("ทรัพย์สินที่ฝาก รปภ", "Material at security");
                    text1 = text1.Replace("มี (ระบุ)...................................................", "If yes, please specify........................................");
                    text1 = text1.Replace("ไม่มี", "No");
                    text1 = text1.Replace("มี", "Yes");

                    text2 = text2.Replace("ท่านได้อ่านและเข้าใจกฏระเบียบว่าด้วยความปลอดภัย", "I agree to abide by the rules");

                    text3 = text3.Replace("ลงชื่อผู้มาติดต่อ.......................................", "Visitor’s sign...................................................");
                    text3 = text3.Replace("ลงชื่อผู้รับการติดต่อ...................................", "Staff’s sign.....................................................");
                    text3 = text3.Replace("ลงชื่อเจ้าหน้าที่ รปภ. ................................", "Security’s sign................................................");
                }

                Rectangle bounds = e1.MarginBounds;
                limit_lineWidth = bounds.Width;
                allHeight = bounds.Height;
                Graphics g1 = Graphics.FromImage(new Bitmap(1, 1));
                SizeF sizeOfString = new SizeF();
                sizeOfString = g1.MeasureString("Z", font);

                //+++ LOGO
                if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"logo.png") == true)
                {

                    bmp = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + @"logo.png");
                    bmp = classGlobal.ResizeImageRatio(bmp, classGlobal.pubWidth, classGlobal.pubHeight);

                    e1.Graphics.DrawImage(bmp, new RectangleF((p.DefaultPageSettings.PrintableArea.Width / 2) - (bmp.Width / 2), 0, bmp.Width, bmp.Height));
                    format = new StringFormat() { Alignment = StringAlignment.Near };
                    rect = new RectangleF(0, bmp.Height, p.DefaultPageSettings.PrintableArea.Width, p.DefaultPageSettings.PrintableArea.Height);
                    e1.Graphics.DrawString(text, font, new SolidBrush(Color.Black), rect, format);

                    logoHeight = Int32.Parse(classGlobal.pubHeight.ToString()); //100;
                }
                else
                {
                    format = new StringFormat() { Alignment = StringAlignment.Near };
                    rect = new RectangleF(0, 0, p.DefaultPageSettings.PrintableArea.Width, p.DefaultPageSettings.PrintableArea.Height);
                    e1.Graphics.DrawString(text, font, new SolidBrush(Color.Black), rect, format);

                    logoHeight = 0;
                }
                //--- 


                // Measure string.
                SizeF stringSize = new SizeF();
                stringSize = e1.Graphics.MeasureString(text, font);
                int iMaxWidth = 240; // Maximum width, in pixels
                StringFormat sfFmt = new StringFormat(StringFormatFlags.LineLimit);
                Graphics g = Graphics.FromImage(new Bitmap(1, 1));
                int iHeight = (int)g.MeasureString(text, font, iMaxWidth, sfFmt).Height;
                int iOneLineHeight = (int)g.MeasureString("Z", font, iMaxWidth, sfFmt).Height;
                int iNumLines = (int)(iHeight / iOneLineHeight);

                //++ วาดกรอบ
                int frame = 0;
                //frame = iOneLineHeight * (27); 
                frame = (iOneLineHeight * lineOfEtc) + logoHeight + 5;   // +5 = เลื่อนกรอบบนลงมานิดนึง
                int recWidth = (int)p.DefaultPageSettings.PrintableArea.Width;  //275;
                e1.Graphics.DrawRectangle(new Pen(Color.Black, 1), new Rectangle(0, frame, recWidth, 120));
                //-- วาดกรอบ


                //++ ท่านได้อ่าน...
                int borderText = (frame + 120) + 10;
                format = new StringFormat() { Alignment = StringAlignment.Center };
                rect = new RectangleF(0, borderText, p.DefaultPageSettings.PrintableArea.Width, p.DefaultPageSettings.PrintableArea.Height);
                e1.Graphics.DrawString(text2, fontBorder, new SolidBrush(Color.Black), rect, format);
                //-- ท่านได้อ่าน...

                //++ ลงชื่อ
                int posSign = borderText + 50;
                format = new StringFormat() { Alignment = StringAlignment.Near };
                rect = new RectangleF(0, posSign, p.DefaultPageSettings.PrintableArea.Width, p.DefaultPageSettings.PrintableArea.Height);
                e1.Graphics.DrawString(text3, font, new SolidBrush(Color.Black), rect, format);
                //-- ลงชื่อ


                //++ BARCODE/QR
                Bitmap resultImage = null;
                resultImage = qr;
                int posQR = 0;
                try
                {
                    posQR = posSign + 40;
                    e1.Graphics.DrawImage(resultImage,
                           new RectangleF((p.DefaultPageSettings.PrintableArea.Width / 2) - (resultImage.Width / 2),
                               ((posQR) + resultImage.Height),
                                    resultImage.Width,
                                        resultImage.Height));
                }
                catch
                {
                    // No any barcode/qrcard
                }
                //-- BARCODE/QR


                #region version factory ข้อความต่อท้าย QR
                if (System.IO.File.Exists(@"slip_description.txt") == false)
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"slip_description.txt"))
                    {
                        file.WriteLine(" ");
                    }
                }

                string messageAnother = Environment.NewLine + Environment.NewLine;
                string[] lines = System.IO.File.ReadAllLines(@"slip_description.txt");
                string eachRowText = "";
                for (int i = 0; i < lines.Length; i++)
                {
                    eachRowText = lines[i].Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
                    //eachRowText = textPerLine(eachRowText, 40);  // 40 character per line
                    messageAnother = messageAnother + eachRowText + Environment.NewLine;
                }


                int afterQR = ((posQR) + resultImage.Height) + resultImage.Height;
                format = new StringFormat() { Alignment = StringAlignment.Near };
                rect = new RectangleF(0, afterQR, p.DefaultPageSettings.PrintableArea.Width, p.DefaultPageSettings.PrintableArea.Height);
                e1.Graphics.DrawString(messageAnother, font, new SolidBrush(Color.Black), rect, format);
                #endregion

            };

            try
            {
                p.Print();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception Occured While Printing", ex);
            }
        }

        private void BtnAppointment_Click(object sender, EventArgs e)
        {
            frmAppointment f = new frmAppointment();
            f.ShowDialog();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            ClassData.DELIVERY_PACKAGE("", @"F:\bike.jpg", "", "Room123", "ส่งจักรยาน", "new", "POST");
        }
    }
} 