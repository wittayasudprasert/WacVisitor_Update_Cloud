using AForge.Imaging.Filters;
using AForge.Video;
using AForge.Video.DirectShow;
using ClassHelper;
using iFinTechIDCard;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
//using ThaiNationalIDCard;

using Npgsql;
using Newtonsoft.Json;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Web.Script.Serialization;
using System.Management;

namespace WacVisitor.FormSetting
{
    public partial class FormActivity : Form
    {
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

        #region Reduce Memmory
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetProcessWorkingSetSize(IntPtr process,
            UIntPtr minimumWorkingSetSize, UIntPtr maximumWorkingSetSize);
        #endregion

        #region PlaceHold TextBox
        private const int EM_SETCUEBANNER = 0x1501;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)]string lParam);
        #endregion

        #region For TOPMOST
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        #endregion

        #region Mouse Move Form
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        #endregion

  
        #region WebCam
        private FilterInfoCollection CaptureDevice; // list of webcam
        public VideoCaptureDevice FinalFrame;
        //static string WebcamBase64String = "";       
        //static bool Captured = false;
        static int WebCamIndex = -1;

        private Bitmap TMPLTCAP = null; //defined elsewhere
        private bool RCRDPIC = false;
        private bool COMPON = false;

        #endregion

        #region Smartcard
        private IDCard mIdCard;
        #endregion

        private string slip_factory_region = "th";  //th  en

        public bool b_enableTopMost = false;
        public bool b_debug = false;

        public string companyName = "";

        private static string strINOUT = "";

        private static int tmrCountClearText = 0;
        private static int tmrCountMax = 3;  // รอ 3 วินาที แล้วล้างข้อมูล

        private static bool btnOKClick = true;

        private static string strID = "";
        private static string strFullname = "";

        private static string strAgreementSlip = "ท่านได้อ่านและเข้าใจกฏระเบียบว่าด้วยความปลอดภัย";  // default
        public FormActivity()
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;

            minimizeMemory();  // release memory usage

            //++ Web Cam
            if (FinalFrame != null)
            {
                if (FinalFrame.IsRunning == true)
                {
                    //Signal the camera to stop, then remove the event handler and camera.
                    FinalFrame.Stop();
                    FinalFrame.NewFrame -= new NewFrameEventHandler(FinalFrame_NewFrame);
                    FinalFrame = null;
                }
                else
                {
                    FinalFrame = null;
                }
            }

            CaptureDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice);//constructor
            FinalFrame = new VideoCaptureDevice();

            if (CaptureDevice.Count > 0)
            {
                classGlobal.WebCamConnect = true;

                // check webcam connect?  USB 2.0 PC Cam
                string webcamName = "";
                foreach (FilterInfo VideoCaptureDevice in CaptureDevice)
                {
                    WebCamIndex = WebCamIndex + 1;
                    webcamName = VideoCaptureDevice.Name.ToString().Replace(" ", "").ToLower();
                    if (webcamName.Contains("cam"))
                    {
                        classGlobal.WebCamConnect = true;
                        break;
                    }

                }
            }

            SendMessage(txtVisitorNumber.Handle, EM_SETCUEBANNER, 0, "เลข VISITOR");

            #region Before Form_Load
            picDocument.Image = Image.FromFile(classGlobal.notfoundscanner);
            picDocument.Image = null;

            //++ WebCam
            if (classGlobal.WebCamConnect == true)
            {
                picWebcam.Image = null;
                picWebcam.Image = Image.FromFile(classGlobal.click4capture);

                picWebcam1.Image = null;
                picWebcam1.Image = Image.FromFile(classGlobal.click4capture);

                picWebcam2.Image = null;
                picWebcam2.Image = Image.FromFile(classGlobal.click4capture);

            }
            else
            {
                picWebcam.Image = Image.FromFile(classGlobal.notfoundwebcam);
                picWebcam1.Image = Image.FromFile(classGlobal.notfoundwebcam);
                picWebcam2.Image = Image.FromFile(classGlobal.notfoundwebcam);
            }
            //--
            #endregion

            strINOUT = "";
            strINOUT = classGlobal.statusIn_OUT;

            txtVisitorNumber.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, txtVisitorNumber.Width, txtVisitorNumber.Height, 15, 15));

        }

        private void GetVisitorTypeList()
        {
            DataTable dt = new DataTable("dt");

            if (classGlobal.databaseType == "acc") 
            { 
                OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT typeid, typename FROM tbl_type ORDER BY typeid ASC", classGlobal.conn);            
                adapter.Fill(dt);
                adapter.Dispose();
                adapter = null;
            }
            else if (classGlobal.databaseType == "psql")
            {
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter("SELECT typeid, typename FROM tbl_type ORDER BY typeid ASC", classGlobal.connP);
                adapter.Fill(dt);
                adapter.Dispose();
                adapter = null;
            }
            else
            {
                dt.Columns.Add("typeid");
                dt.Columns.Add("typename");

                string jsonString = ClassData.GET_METHODE("visitorType");
                if (jsonString == "")
                {
                    //-- no data
                }
                else
                {
                    JArray jsArray = JArray.Parse(jsonString);
                    int j = 0;
                    foreach (var x in jsArray)
                    {
                        dt.Rows.Add(j, x.ToString());
                        j += 1;
                    }
                }
            }
            //+++
            //listBox1.Items.Clear();
            Dictionary<string, string> list = new Dictionary<string, string>();
            list.Add("ประเภท VISITOR", "-1");
            foreach (DataRow reader in dt.Rows)
            {
                try
                {
                    list.Add(reader.ItemArray[1].ToString(), reader.ItemArray[0].ToString());
                }
                catch
                {
                    // exits key 
                }              
            }

            listBox1.DataSource = new BindingSource(list, null);
            listBox1.DisplayMember = "Key";
            listBox1.ValueMember = "Value";
            //--

            dt.Dispose();
            dt = null;
        }

        void SET_KB_LANGUAGE()
        {
            string kbLayout = InputLanguage.CurrentInputLanguage.Culture.Name;
            string new_kbLayout = "th-TH";
            
            if (File.Exists("lang") == true)
            {
                new_kbLayout = System.IO.File.ReadAllText("lang").Replace(" ", "").Replace(Environment.NewLine, "");          
            }
            else
            {
                System.IO.File.WriteAllText("lang", "th-TH");
                new_kbLayout = "th-TH";                
            }

            var culture = System.Globalization.CultureInfo.GetCultureInfo(new_kbLayout);
            var language = InputLanguage.FromCulture(culture);
            InputLanguage.CurrentInputLanguage = language;
            kbLayout = InputLanguage.CurrentInputLanguage.Culture.Name;
            lbKeyboardLayout.Text = kbLayout.Replace("en-US", "EN").Replace("th-TH", "TH");

            this.ActiveControl = txtVisitorNumber;
        }
        private void FormActivity_Load(object sender, EventArgs e)
        {
            SET_KB_LANGUAGE();

            rbtnReprint.Enabled = false;

            if (classGlobal.boolX50 == true)
            {
                picDocument.Cursor = Cursors.Hand; 
            }


            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.WindowState = FormWindowState.Normal;

            string kbLayout = InputLanguage.CurrentInputLanguage.Culture.Name;
            lbKeyboardLayout.Text = kbLayout.Replace("en-US", "EN").Replace("th-TH", "TH");

            GetVisitorTypeList();

            btnIDCard.BackgroundImage = Image.FromFile(@"icon\idcard.png");
            btnIDCard.BackgroundImageLayout = ImageLayout.Stretch;
            btnIDCard.FlatStyle = FlatStyle.Flat;

            btnLicense.BackgroundImage = Image.FromFile(@"icon\license.png");
            btnLicense.BackgroundImageLayout = ImageLayout.Stretch;
            btnLicense.FlatStyle = FlatStyle.Flat;

            btnMoreInfo.BackgroundImage = Image.FromFile(@"icon\moreinfo.png");
            btnMoreInfo.BackgroundImageLayout = ImageLayout.Stretch;
            btnMoreInfo.FlatStyle = FlatStyle.Flat;
            ////--    

            FileSystemWatcher watcher = new FileSystemWatcher();
            btnIPCAM.Visible = false;
            if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"IPCAM_Engine\HIKEngine.exe") == true)
            {
                if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"IPCAM_Engine\cam1.jpg"))
                    System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"IPCAM_Engine\cam1.jpg");
                if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"IPCAM_Engine\cam2.jpg"))
                    System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"IPCAM_Engine\cam2.jpg");

                if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"cam1.jpg"))
                    System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"cam1.jpg");
                if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"cam2.jpg"))
                    System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"cam2.jpg");


                String base64IpCam = "iVBORw0KGgoAAAANSUhEUgAAAHMAAABdCAYAAACW5VmgAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAIGNIUk0AAHolAACAgwAA+f8AAIDpAAB1MAAA6mAAADqYAAAXb5JfxUYAAAxRSURBVHja7J1/TFP3FsBPb39RfkmhLSgRUWC4ClVmNmdRhMF0IDhFNuebTtyy548lvIfazo3NiOA2f21DUKNOBJUxh9skBUZg4wkUrXkJxkyyKTGMZzKIUNzeiP1Bb8/7482GUtBCf9229yQngfZ7v/f2fnK+3/M993zPZSAi0OIZQtC3gIZJCw2TFhomLVYJi6oXVlpa+uzt27djBwYGRCRJEkFBQb/Hxsbelslk7TS2CQQRKaM1NTXhaWlpRVwutw0AVACAY1TFZrOVixYtKikuLk6l0rVTQSlzIS+++GLRBAAnUhVBEFcXL1782f79+1NomBSAWVFREcvn8xsnCXJci5VKpUcOHDiwlIbpAt2zZ0+6jRDHBcvlctukUumRQ4cOJdIwnaAFBQUrHQDSAqyvr2/L0qVLD3kDWJec9OjRo886AaQFWB6PdyUlJeXjI0eOLKZh2kGVSiXB4/GuOBmkBVgOh9OWlpZW9Omnny6mYU5RxWLxGReDHHeOTU1N3V9SUvIsDdNKlclk2S4YXicF1s/PryUlJeXjY8eOJdAwH6N+fn4tFAZpATYwMLApNTV1f1lZ2UIa5ijdtGnTVopb5WPBTps2rWnFihWFVLZYhrMeTvv6+l7RaDTLPCACej0gIGB42bJlV9LT0+u3b99+w6tis05aU7psjs3Kynr/5MmT8V4xzM6dO/esB4K0ABsQEPBDRkbGnjNnzjztkTBbW1tZTCbzqhfANAMbFBTUmJWV9X5FRUWsx8AsKipK9TKQFmAFAkFddna2vLKyMsatYb7wwgv7vRymGdi5c+eevXz5ssgtYUZHR5+nIVpCfffdd1e7HUwOh9PmjcDYbDay2ezHAt21a1eOPe+1Q3OAmpub/fV6PcdbUnDmzJkDOTk58Nxzz0FERAQAANy7dw+uXr0K586dg4GBgdHNFx0+fHiXRCK5uXHjxm7KrzMvXboU7g1WGB4ejiUlJahWq3Ei+fXXXzEnJ8fiWKFQWOcWw+y5c+diPB1kWloa9vT0ICLiyMgIajSacRUR0WAw4Lp16yyG2/z8/FcpD7OystKjYa5fv94EaiKIo5UkSRwaGsKnnnrKrJ+QkJAGe9xvhyZB+/j4aD11fszKyoKKigrgcDig1WrH/m7w8fEBLpdr9rlerwc+nw+FhYVmn6vV6uBjx44tpHRGO5/PfwAA1z0NpEQigcrKSmCz2aDX602fMxgMIAgCWlpaQKFQQE9PD/j4+ACTyTQDunr1apg3b56ZM6RQKFZRPtD+V0KzxwytXC4Xr127NuHQioj4zjvvIAAgn89HuVyODx8+RL1eb9bmww8/NOs3LCzsO8qvM2NiYjwqaLB79+7HzpGIiHV1dRZzK0mSqNPpTG2ampqQIAhTGwaDoWpqavKn7JwJABAbG3vbU4bXGTNmQH5+PpAk+fgNPCzz5Xt1dTV8+eWXwOH8f8lNkiSIxWIQCoWjR8hFvb29syi9CywzM1PhKfPmtm3bQCQSwcjICDAYjAnb/fTTTxaflZSUgE6nA4IggCRJ4PP5EBgYaNbm/v37IkrD3LJly82goKDf3R1kYGAgbNiwwWSVLBYLCML89hEEAYgICoXC4vgbN25Ad3c3sNnsR74EsNnssV6tkNIwAQDWrFnzrbtbZ2pqKkRGRoLBYAAulwuXLl0CtVptNqRyOByoqakBpVJpcTxJknDz5k2TRRMEYWHdOp2OQ3mY5eXlp9zdOtPT083AFBcXw4kTJ4DFYpnWlVeuXIG8vDwwGo3j9tHb22v622AwWLRjs9kGW67RaZttCwoKimQyWRAALHI3kFwuF5YsWQKICGw2G+7evQs9PT1QXFwMd+7cAalUCp2dnXDx4kWLAMK4FkQQoNFoQKfTjV2Xqym9zhytb7zxxnZww8SumJgY/PPPP1Gv1yMiYnV19ZT6KS0tRUREkiSxp6cHQ0JCzL4/fvx4AmUfgY2VysrK4yRJElVVVeBOFiqRSMDf399kSX19fVPqJz4+3mSZd+7cAbXazBCvh4WF9dlynU4vUHHhwoWyw4cP7wwJCfneXZwikUhkGsUAAIaHh6cS2oSYmBiTN9zZ2Wn2fUBAwPCaNWv63QomAMDOnTs7BgcHM/bs2VMoFovLqQ7V39/f7H8ejzclb3j69OlgMBgAEaGxsdHs+/Dw8HuUDrQ/SQoLC7/v6up6q7W1dYlcLl8rkUhOURHs2CVEWFjYpPvIzc0FBoMBHA4Huru7LSxz6dKltldRodp+iba2NpZMJsueP3/+Sao4S3l5eaZ4rNFoxK6uLuTxeFYfn56ejgaDAbVaLSIi7t271+IBdXV19SyPqTYynra3txO7du3KkUgkJxkMhsvAvv7664iIqNVqUavVosFgwOXLl1t1rEAgwF9++QWNRiOOjIzggwcPMCIiYmzayTceVTrGGrAymSw7Li7utLPBzp8/H7VardlTj46Ojidl32FAQAA2NjaarBoR8ZNPPnFYlp5b7hBubW1l7d69e/Vfu7AdDpbH42F3dzcajUazR13l5eXIYrHGPSYqKgpbWlrMhue7d++iQCAwa+fv7/+DxxV1ojrY06dPmz3HfDT/tbe349q1azEqKgojIiJQIpHgvn37sL+/39Rep9MhIuLLL7/ssGQuj4A51nmSy+UOcZ7S0tLQaDSiVqu1eBiNiKhWq7G/v98EzmAwmEHft2+fRZ+hoaG1HllujepeMUEQ2NzcjIiIDx8+tMgw0Ov1ODIyYgb7EcizZ88ig8GwsMrz589H0TCnUK5GJpNl2+oVx8fH44MHD6xKrSRJEhERjx49ikwm0wLkm2+++XePLYToTK9YLpdnx8XFnSYIYtL7RletWoV//PEHIqLJux1tiY8g9vf349tvvz3uHpOkpKQDHl3V0lXO03vvvbdq3rx5k3KeFixYgI2NjaanKKPl3r17WFZWhpGRkeOCFIvFZ9y+QIUzpL6+PnjlypVDUzm2vb2d1dDQkFlXV7fy1q1b8dY81Xn++echOjoaIiMjYWBgAHp7e6GzsxPu378/XvPriYmJHUqlcqfDboCnWFltba0A/trMmpeX9zdb0hYfDcULFiw4YSevWJWbm7vVa4oH26r19fVBY29gdHT0+fz8/FdtAfvIeZqiV6yKjo4+X1VVNdurKkE7AKbZTa2trRXYyyuOj49/UkhRFRcXd/rgwYNOLWTMAu+QRRqNxg8ABm3pJDEx0ZiYmPjtwYMHv21ra2M1Nzev+Pnnn58eHBwUcrlcfWhoaN+cOXPuJicn/ys5OVnv7B/pLTCBIAijPftLSkoyJCUl1QNAPWV+oytPXltbK3rppZcKRSJRHYPBUE2bNq0pMTHxSElJyaTzg5hMphG8XVw1x+Xm5j6uMOKkF9ZP2nL/9ddfz6TLejtAU1JSPrbCM1TNnDmzxpr+Ll++LJoxY8Y3NEwnn3Dr1q25k3DxVUuWLDn0pApgVoTlVBcvXqRh2lMbGhqCprJWm+glNOvXr8+zpj8Wi6WkX4VhZ33mmWeOTSWCMl4Bh0nEU1UbN27cTsO0oyoUimAbQmOqoqKiVESEr776atYk3lCkeuWVV3bQL6mxs65bt+6ftsQ3ExISTsjlcqsL9jOZzKsffPDBSvqNQw7Q0NDQWidl06mEQmGdoypHUlmdEgH68ccffQcGBoROONV1qVTa0dHRsdMbYwZOiQDdunUr3mg0LnY0yM2bN5d7K0inxWZ/++23cEf27+Pj0/r555//Y8uWLTe9OZrnFJh9fX1hjrLGmJiY7i+++GJzUlKSAbxcnAJTo9H4OgJkRkZGQ319/T6gxXlzpp+f37C9QRYUFBTRIF1gmYGBgf+1F8SQkJChsrKyba+99lovjc8FMAUCwYA9+hGLxV1dXV1v0dhcOMzOmjXrP2CHHdEbNmy4QCNzMcxNmzbd5nK5tubEXF+4cOG/aWQuhgkAEBkZ2WPL8UKhcHD58uXDNDIKwLS1umVycnILjesJ4tRAMIulhCkGzxUKRbA374uhRPHg0TLV6pZSqbQjMzNziDY9ClnmVN47zWQyryqVSoK2PAomdJ06dSoeJpHQ9dFHH6XQoCicarl3794VVgBV7dix41UakhtsHKqqqpo9e/bs6nGgqqZPn/5daWnpQhqQm222PX78eIJKpVo8NDQUHBwcPJSQkHAjPz//Gu3NTF48auc0HTSghYZJCw2TFhomLdbI/wYAW2VRfK5sPW0AAAAASUVORK5CYII=";
                btnIPCAM.Visible = true;
                btnIPCAM.Image = clsImage.Base64ToImage(base64IpCam);
                btnIPCAM.BackgroundImageLayout = ImageLayout.Stretch;
                btnIPCAM.BackColor = System.Drawing.SystemColors.HotTrack;
                btnIPCAM.Text = "";
                btnIPCAM.FlatStyle = FlatStyle.Flat;
                btnIPCAM.FlatAppearance.BorderSize = 0;

                if (classGlobal.boolWatcherActive == false)
                {
                    String savePicturePath = "";
                    savePicturePath = classGlobal.ipcamstoragePath;
                    if (!System.IO.Directory.Exists(savePicturePath))
                        System.IO.Directory.CreateDirectory(savePicturePath);

                    watcher.Path = savePicturePath;  //IPCAM
                    watcher.Filter = "*.jpg";
                    watcher.Created -= FswCreated;
                    watcher.Created += FswCreated;
                    watcher.EnableRaisingEvents = true;
                    classGlobal.boolWatcherActive = true;
                }
            }


            if (strINOUT == "IN")
            {
                lb1Text.Text = "";
                lb2Text.Text = "";
                lb3Text.Text = "";
                lb4Text.Text = "";

                if (int.Parse(classGlobal.strRandomNumber) > 0)
                    txtVisitorNumber.Text = classGlobal.RandomVisitorNumber();

            }
            else if (strINOUT == "OUT")  // OUT ไม่ต้องใช้กล้องถ่ายรูป / สแกนเนอร์
            {
                picDocument.Image = null;
                picWebcam.Image = null;
                picWebcam1.Image = null;
                picWebcam2.Image = null;

                txtID.ReadOnly = true;
                txtFullname.ReadOnly = true;

                minimizeMemory();
            }

            //++ Smartcard
            mIdCard = new IDCard();
            mIdCard.LicenseCompany = "WAC RESEARCH CO., LTD.";
            mIdCard.LicenseTel = "025303809";
            mIdCard.LicenseSerial = "A3967B";

            string[] readers = new string[0] { };
            try
            {
                readers = mIdCard.GetReaders();
            }
            catch
            {
                //
            }
            if (readers == null)
            {
                readers = new string[0] { };
            }

            if (readers.Length > 0)
            {
                foreach (string sRead in readers)
                {
                    if (sRead.IndexOf("Generic EMV Smartcard Reader") == 0)
                    {
                        classGlobal.smartCardReaderName = sRead.Replace(" ", "@");
                        break;
                    }
                    if (sRead.IndexOf("ACS ACR1281 1S Dual Reader ICC") == 0)
                    {
                        classGlobal.smartCardReaderName = sRead.Replace(" ", "@");
                        break;
                    }
                    if (sRead.IndexOf("ACS") == 0)
                    {
                        classGlobal.smartCardReaderName = sRead.Replace(" ", "@");
                        break;
                    }

                    classGlobal.smartCardReaderName = sRead.Replace(" ", "@");
                    break;
                }

                classGlobal.SmartcardDeviceConnect = true;
                btnIDCard.Visible = true;
                btnLicense.Visible = true;
                btnMoreInfo.Visible = true;

                mIdCard.Dispose();
                mIdCard = null;

                #region Smartcard Event iFintech
                //if (mIdCard.SetAutoRead() == 0)
                //{
                //    mIdCard.eventPhotoProgressBar -= new handlePhotoProgressBar(setProgressBar);
                //    mIdCard.eventCardInsertedWithPhoto -= new handleCardInserted(onCardInsertedWithPhoto);
                //    mIdCard.eventCardRemoved -= new handleCardRemoved(onCardRemoved);
                //    mIdCard.eventReaderStatus -= new handleReaderStatus(LogLine);

                //    mIdCard.eventPhotoProgressBar += new handlePhotoProgressBar(setProgressBar);
                //    mIdCard.eventCardInsertedWithPhoto += new handleCardInserted(onCardInsertedWithPhoto);
                //    mIdCard.eventCardRemoved += new handleCardRemoved(onCardRemoved);
                //    mIdCard.eventReaderStatus += new handleReaderStatus(LogLine);
                //}
                #endregion

            }
            else
            {

                btnIDCard.BackgroundImage = Image.FromFile(@"icon\idcardno.png");
                btnIDCard.BackgroundImageLayout = ImageLayout.Stretch;
                btnIDCard.FlatStyle = FlatStyle.Flat;

            }
            //--

            GetVisitorTypeList();  //Visitor Type

            if (strINOUT == "OUT")
            {
                //ถ้าออก ไม่ต้องเปิดใช้งานอ่านบัตรประชาชน
                btnIDCard.Visible = false;
                btnLicense.Visible = false;
                btnMoreInfo.Visible = false;

                this.ActiveControl = txtVisitorNumber;
            }

            if (classGlobal.FactoryVersion == true)
                this.ActiveControl = txtVisitorNumber;

            //lbID.Focus();

            if (classGlobal.textBox_CardId_Name_Readonly == true)
            {
                txtID.ReadOnly = true;
                txtID.BackColor = System.Drawing.SystemColors.HotTrack;
                txtID.ForeColor = Color.White;
                txtID.BorderStyle = BorderStyle.None;
                txtFullname.ReadOnly = true;
                txtFullname.BackColor = System.Drawing.SystemColors.HotTrack;
                txtFullname.ForeColor = Color.White;
                txtFullname.BorderStyle = BorderStyle.None;
            }

            this.ActiveControl = txtVisitorNumber;
        }

        //************************
        #region Smartcard Event iFintech
        public void setProgressBar(int value, int maximum)
        {
            if (progressBar1.InvokeRequired)
            {
                progressBar1.BeginInvoke(new MethodInvoker(delegate { progressBar1.Visible = true; }));

                if (progressBar1.Maximum != maximum)
                    progressBar1.BeginInvoke(new MethodInvoker(delegate { progressBar1.Maximum = maximum; }));

                // fix progress bar sync.
                if (progressBar1.Maximum > value)
                    progressBar1.BeginInvoke(new MethodInvoker(delegate { progressBar1.Value = value + 1; }));
            }
            else
            {
                progressBar1.Visible = true;

                if (progressBar1.Maximum != maximum)
                    progressBar1.Maximum = maximum;

                // fix progress bar sync.
                if (progressBar1.Maximum > value)
                    progressBar1.Value = value + 1;

                progressBar1.Value = value;
            }

        }
        public void onCardInsertedWithPhoto(IDCardProfile personal)
        {
            if (personal != null)
            {
                if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"cid.ini") == true)
                { System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"cid.ini"); }

                string fileName;
                StreamWriter objWriter;

                fileName = "cid.ini";
                objWriter = new StreamWriter(fileName);
                objWriter.WriteLine(personal.CitizenID);
                objWriter.WriteLine(personal.ThPreName);
                objWriter.WriteLine(personal.ThFirstName);
                objWriter.WriteLine(personal.ThLastName);
                objWriter.WriteLine(personal.EnPreName);
                objWriter.WriteLine(personal.EnFirstName);
                objWriter.WriteLine(personal.EnLastName);
                objWriter.WriteLine(personal.ThBirthDate);
                objWriter.WriteLine(personal.Sex);
                objWriter.WriteLine(personal.AddressHouseNo);
                objWriter.WriteLine(personal.AddressVillageNo);
                objWriter.WriteLine(personal.AddressLane);
                objWriter.WriteLine(personal.AddressRoad);
                objWriter.WriteLine(personal.AddressSubDistrict);
                objWriter.WriteLine(personal.AddressDistrict);
                objWriter.WriteLine(personal.AddressProvince);
                objWriter.WriteLine(personal.ThIssueDate);
                objWriter.WriteLine(personal.ThExpiryDate);
                objWriter.WriteLine(Convert.ToBase64String(personal.PhotoByte));
                objWriter.Close();


                if (progressBar1.InvokeRequired)
                {
                    progressBar1.BeginInvoke(new MethodInvoker(delegate { progressBar1.Visible = false; })); 
                }
                else
                {
                    progressBar1.Visible = false;
                }

                classGlobal.pub_personal = new IDCardProfile();
                string[] t = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"cid.ini");

                t[7] = classGlobal.YearEngToTha(t[7]);
                t[16] = classGlobal.YearEngToTha(t[16]);
                t[17] = classGlobal.YearEngToTha(t[17]);

                classGlobal.pub_personal.CitizenID = t[0];
                classGlobal.pub_personal.ThPreName = t[1];
                classGlobal.pub_personal.ThFirstName = t[2];
                classGlobal.pub_personal.ThLastName = t[3];
                classGlobal.pub_personal.EnPreName = t[4];
                classGlobal.pub_personal.EnFirstName = t[5];
                classGlobal.pub_personal.EnLastName = t[6];
                classGlobal.pub_personal.ThBirthDate = t[7];  //"02/12/1979"
                classGlobal.pub_personal.Sex = t[8];
                classGlobal.pub_personal.AddressHouseNo = t[9];
                classGlobal.pub_personal.AddressVillageNo = t[10];
                classGlobal.pub_personal.AddressLane = t[11];
                classGlobal.pub_personal.AddressRoad = t[12];
                classGlobal.pub_personal.AddressSubDistrict = t[13];
                classGlobal.pub_personal.AddressDistrict = t[14];
                classGlobal.pub_personal.AddressProvince = t[15];
                classGlobal.pub_personal.ThIssueDate = t[16];  //"02/12/2013"
                classGlobal.pub_personal.ThExpiryDate = t[17];  //"01/12/2021"
                string stringInBase64 = t[18];
                byte[] bytes = System.Convert.FromBase64String(stringInBase64);
                classGlobal.pub_personal.PhotoByte = bytes;

                classGlobal.PlustekBase64String = stringInBase64;

                System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"cid.ini");

                classGlobal.personID = classGlobal.pub_personal.CitizenID;
                classGlobal.personName = classGlobal.pub_personal.ThPreName + "" +
                                                classGlobal.pub_personal.ThFirstName + " " + classGlobal.pub_personal.ThMidName + " " +
                                                    classGlobal.pub_personal.ThLastName;

                classGlobal.personName = classGlobal.personName.Replace("  ", " ");
                personal = classGlobal.pub_personal;

                if (personal != null)
                {
                    if (txtVisitorNumber.InvokeRequired)
                    {
                        txtID.BeginInvoke(new MethodInvoker(delegate { txtID.Text = personal.CitizenID; }));
                        txtFullname.BeginInvoke(new MethodInvoker(delegate { txtFullname.Text = personal.ThPreName + personal.ThFirstName + " " + personal.ThLastName; }));

                        using (MemoryStream memstr = new MemoryStream(classGlobal.pub_personal.PhotoByte))
                        {
                            Image img = Image.FromStream(memstr);
                            picDocument.Image = img;
                            picDocument.SizeMode = PictureBoxSizeMode.Zoom;
                        }

                        txtID.BeginInvoke(new MethodInvoker(delegate { txtID.Text = "#############"; }));
                        if (classGlobal.DisplayHashTag == true)
                        {
                            //txtID.BeginInvoke(new MethodInvoker(delegate { txtID.Text = classGlobal.REPLACE_IDCARD(personal.CitizenID); }));
                            txtFullname.BeginInvoke(new MethodInvoker(delegate { txtFullname.Text = (classGlobal.REPLACE_NAME(personal.ThPreName + personal.ThFirstName + " " + personal.ThLastName)); }));

                            using (MemoryStream memstr = new MemoryStream(classGlobal.PLACEWATERMARK_FROM_BYTE(classGlobal.pub_personal.PhotoByte)))
                            {
                                Image img = Image.FromStream(memstr);
                                picDocument.Image = img;
                                picDocument.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                        }
                        else
                        {
                            using (MemoryStream memstr = new MemoryStream(classGlobal.PLACEWATERMARK_FROM_BYTE(classGlobal.pub_personal.PhotoByte)))
                            {
                                Image img = Image.FromStream(memstr);
                                picDocument.Image = img;
                                picDocument.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                        }
                    }
                    else
                    {
                        txtID.Text = personal.CitizenID.ToString();
                        txtFullname.Text = personal.ThPreName + personal.ThFirstName + " " + personal.ThLastName;

                        using (MemoryStream memstr = new MemoryStream(classGlobal.pub_personal.PhotoByte))
                        {
                            Image img = Image.FromStream(memstr);
                            try
                            {
                                picDocument.BeginInvoke(new MethodInvoker(delegate { picDocument.Image = img; }));
                            }
                            catch
                            {
                                picDocument.Image = img; 
                            }                            
                            picDocument.SizeMode = PictureBoxSizeMode.Zoom;
                        }

                        txtID.Text = "#############";
                        if (classGlobal.DisplayHashTag == true)
                        {
                            //txtID.Text = classGlobal.REPLACE_IDCARD(personal.CitizenID);
                            txtFullname.Text = (classGlobal.REPLACE_NAME(personal.ThPreName + personal.ThFirstName + " " + personal.ThLastName));

                            using (MemoryStream memstr = new MemoryStream(classGlobal.PLACEWATERMARK_FROM_BYTE(classGlobal.pub_personal.PhotoByte)))
                            {
                                Image img = Image.FromStream(memstr);
                                picDocument.BeginInvoke(new MethodInvoker(delegate { picDocument.Image = img; }));
                                picDocument.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                        }
                        else
                        {
                            using (MemoryStream memstr = new MemoryStream(classGlobal.PLACEWATERMARK_FROM_BYTE(classGlobal.pub_personal.PhotoByte)))
                            {
                                Image img = Image.FromStream(memstr);
                                picDocument.BeginInvoke(new MethodInvoker(delegate { picDocument.Image = img; }));
                                picDocument.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                        }
                    }
                }
                else
                {
                    if (txtVisitorNumber.InvokeRequired)
                    {
                        txtID.BeginInvoke(new MethodInvoker(delegate { txtID.Text = ""; }));
                        txtFullname.BeginInvoke(new MethodInvoker(delegate { txtFullname.Text = ""; }));

                        picDocument.BeginInvoke(new MethodInvoker(delegate { picDocument.Image = null; }));
                        picDocument.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                    else
                    {
                        txtID.Text = "";
                        txtFullname.Text = "";
                        picDocument.Image = null;
                    }

                }

                AUTO_CHECK_BLACKLIST();
                AUTO_CHECK_WHITELIST();
                if (classGlobal.visitorStatus != "blacklist")
                    ClassData.CHECK_APPOINTMENT(classGlobal.personID, classGlobal.personName);

            }

        }
        public void onCardRemoved()
        {
            try
            {
                //AddLogs("Event CardRemoved...");
                if (progressBar1.InvokeRequired)
                {
                    progressBar1.BeginInvoke(new MethodInvoker(delegate { progressBar1.Value = 0; }));
                }
                else
                {
                    progressBar1.Value = 0;
                }

            }
            catch (Exception e)
            {
                string sErr = "Event CardRemoved... " + e.ToString();
                //AddLogs("Event CardRemoved... " + ex.ToString());
            }
        }
        public void LogLine(string text = "")
        {
            //
        }
        #endregion
        //************************

        private static void minimizeMemory()
        {
            GC.Collect(GC.MaxGeneration);
            GC.WaitForPendingFinalizers();
            SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, (UIntPtr)0xFFFFFFFF, (UIntPtr)0xFFFFFFFF);
        }
        private void FormActivity_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (FinalFrame != null)
            {
                if (FinalFrame.IsRunning == true)
                {
                    //Signal the camera to stop, then remove the event handler and camera.
                    FinalFrame.Stop();
                    FinalFrame.NewFrame -= new NewFrameEventHandler(FinalFrame_NewFrame);
                    FinalFrame = null;
                }
                else
                {
                    FinalFrame = null;
                }
            }
        }
        public void FinalFrame_NewFrame(object sender, NewFrameEventArgs eventArgs) // must be void so that it can be accessed everywhere.
        {
            //Image to point to existing image and dispose of it
            System.Drawing.Image OldImage;
            //Call comparison method if flag is set.
            if (COMPON == true)
            {
                Difference TmpltFilter = new Difference(TMPLTCAP);
                TmpltFilter.ApplyInPlace(eventArgs.Frame);
                //Point to the existing image, change what the picturebox points to, then dispose of the old image.
                OldImage = picWebcam.Image;
                picWebcam.Image = AForge.Imaging.Image.Clone(eventArgs.Frame);
                OldImage.Dispose();
            }
            else
            {
                //Point to the existing image, change what the picturebox points to, then dispose of the old image.
                OldImage = picWebcam.Image;
                picWebcam.Image = AForge.Imaging.Image.Clone(eventArgs.Frame);
                OldImage.Dispose();
                //Toggle the flag back to false to show it's safe (i.e., comparisons have stopped)
                //for the save method to copy from the picture box.
                if (RCRDPIC == true)
                {
                    RCRDPIC = false;
                }
            }
        }

        private void txtVisitorNumber_TextChanged(object sender, EventArgs e)
        {
            if (txtVisitorNumber.Text == "")
            {
                if (btnOKClick == false)
                {
                    CLEARCONTROLS();
                    btnOKClick = true;
                }
            }
           
        }
        private void CLEARCONTROLS()
        {
            tmrCountClearText = 0;
            timer1.Enabled = false;

            listBox1.SelectedIndex = 0;
            txtVisitorNumber.Text = "";

            lb1Text.Text = "";
            lb2Text.Text = "";
            lb3Text.Text = "";
            lb4Text.Text = "";
            txtID.Text = "";
            txtFullname.Text = "";

            picDocument.Image = Image.FromFile(classGlobal.click4scan);
            picDocument.Image = null;

            picWebcam.Image = Image.FromFile(classGlobal.click4capture);
            picWebcam1.Image = Image.FromFile(classGlobal.click4capture);
            picWebcam2.Image = Image.FromFile(classGlobal.click4capture);
           

            //++ WebCam
            if (classGlobal.WebCamConnect == true)
            {
                try
                {
                    FinalFrame.Stop();
                    FinalFrame.Start();
                }
                catch
                {
                    //
                }

            }
            else
            {
                picWebcam.Image = Image.FromFile(classGlobal.notfoundwebcam);
                picWebcam1.Image = Image.FromFile(classGlobal.notfoundwebcam);
                picWebcam2.Image = Image.FromFile(classGlobal.notfoundwebcam);
            }

            if (classGlobal.statusIn_OUT == "OUT")
            {
                picDocument.Image = null;
                picWebcam.Image = null;
                picWebcam1.Image = null;
                picWebcam2.Image = null;
            }


            this.ActiveControl = txtVisitorNumber;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (classGlobal.statusIn_OUT == "IN")
            {
                tmrCountMax = classGlobal.timerClearIn;
            }
            else if (classGlobal.statusIn_OUT == "OUT")
            {
                tmrCountMax = classGlobal.timerClearOut;
            }

            tmrCountClearText += 1;
            if (tmrCountClearText == tmrCountMax)
            {
                CLEARCONTROLS();
            }
        }

        private void p_Exited(object sender, EventArgs e)
        {

            // Read the file and display it line by line.               
            classGlobal.pub_personal = null;
            if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"cid.ini") == true)
            {

                classGlobal.pub_personal = new IDCardProfile();
                string[] t = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"cid.ini");

                t[7] = classGlobal.YearEngToTha(t[7]);
                t[16] = classGlobal.YearEngToTha(t[16]);
                t[17] = classGlobal.YearEngToTha(t[17]);

                classGlobal.pub_personal.CitizenID = t[0];
                classGlobal.pub_personal.ThPreName = t[1];
                classGlobal.pub_personal.ThFirstName = t[2];
                classGlobal.pub_personal.ThLastName = t[3];
                classGlobal.pub_personal.EnPreName = t[4];
                classGlobal.pub_personal.EnFirstName = t[5];
                classGlobal.pub_personal.EnLastName = t[6];
                classGlobal.pub_personal.ThBirthDate = t[7];  //"02/12/1979"
                classGlobal.pub_personal.Sex = t[8];
                classGlobal.pub_personal.AddressHouseNo = t[9];
                classGlobal.pub_personal.AddressVillageNo = t[10];
                classGlobal.pub_personal.AddressLane = t[11];
                classGlobal.pub_personal.AddressRoad = t[12];
                classGlobal.pub_personal.AddressSubDistrict = t[13];
                classGlobal.pub_personal.AddressDistrict = t[14];
                classGlobal.pub_personal.AddressProvince = t[15];
                classGlobal.pub_personal.ThIssueDate = t[16];  //"02/12/2013"
                classGlobal.pub_personal.ThExpiryDate = t[17];  //"01/12/2021"
                string stringInBase64 = t[18];
                byte[] bytes = System.Convert.FromBase64String(stringInBase64);
                classGlobal.pub_personal.PhotoByte = bytes;

                classGlobal.PlustekBase64String = stringInBase64;

                System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"cid.ini");

                classGlobal.personID = classGlobal.pub_personal.CitizenID;
                classGlobal.personName = classGlobal.pub_personal.ThPreName + "" + 
                                                classGlobal.pub_personal.ThFirstName + " " + classGlobal.pub_personal.ThMidName + " " +
                                                    classGlobal.pub_personal.ThLastName;

                classGlobal.personName = classGlobal.personName.Replace("  ", " "); 
            }

            IDCardProfile personal = classGlobal.pub_personal;

            if (personal != null)
            {
                if (txtVisitorNumber.InvokeRequired)
                {
                    txtID.BeginInvoke(new MethodInvoker(delegate { txtID.Text = personal.CitizenID; }));
                    txtFullname.BeginInvoke(new MethodInvoker(delegate { txtFullname.Text = personal.ThPreName + personal.ThFirstName + " " + personal.ThLastName; }));

                    using (MemoryStream memstr = new MemoryStream(classGlobal.pub_personal.PhotoByte))
                    {
                        Image img = Image.FromStream(memstr);
                        picDocument.Image = img;
                        picDocument.SizeMode = PictureBoxSizeMode.Zoom;
                    }

                    txtID.BeginInvoke(new MethodInvoker(delegate { txtID.Text = "#############"; }));
                    if (classGlobal.DisplayHashTag == true)
                    {
                        //txtID.BeginInvoke(new MethodInvoker(delegate { txtID.Text = classGlobal.REPLACE_IDCARD(personal.CitizenID); }));                        
                        txtFullname.BeginInvoke(new MethodInvoker(delegate { txtFullname.Text = (classGlobal.REPLACE_NAME(personal.ThFirstName + " " + personal.ThLastName)); }));

                        using (MemoryStream memstr = new MemoryStream(classGlobal.PLACEWATERMARK_FROM_BYTE(classGlobal.pub_personal.PhotoByte)))
                        {
                            Image img = Image.FromStream(memstr);
                            picDocument.Image = img;
                            picDocument.SizeMode = PictureBoxSizeMode.Zoom;
                        }
                    }
                    else
                    {
                        using (MemoryStream memstr = new MemoryStream(classGlobal.PLACEWATERMARK_FROM_BYTE(classGlobal.pub_personal.PhotoByte)))
                        {
                            Image img = Image.FromStream(memstr);
                            picDocument.Image = img;
                            picDocument.SizeMode = PictureBoxSizeMode.Zoom;
                        }
                    }
                }
                else
                {
                    txtID.Text = personal.CitizenID.ToString();
                    txtFullname.Text = personal.ThPreName + personal.ThFirstName + " " + personal.ThLastName;

                    using (MemoryStream memstr = new MemoryStream(classGlobal.pub_personal.PhotoByte))
                    {
                        Image img = Image.FromStream(memstr);
                        picDocument.BeginInvoke(new MethodInvoker(delegate { picDocument.Image = img; }));
                        picDocument.SizeMode = PictureBoxSizeMode.Zoom;
                    }

                    txtID.Text = "#############";
                    if (classGlobal.DisplayHashTag == true)
                    {
                        //txtID.Text = classGlobal.REPLACE_IDCARD(personal.CitizenID);                        
                        txtFullname.Text = (classGlobal.REPLACE_NAME(personal.ThFirstName) + " " + classGlobal.REPLACE_NAME(personal.ThLastName));

                        using (MemoryStream memstr = new MemoryStream(classGlobal.PLACEWATERMARK_FROM_BYTE(classGlobal.pub_personal.PhotoByte)))
                        {
                            Image img = Image.FromStream(memstr);
                            picDocument.BeginInvoke(new MethodInvoker(delegate { picDocument.Image = img; }));
                            picDocument.SizeMode = PictureBoxSizeMode.Zoom;
                        }
                    }
                    else
                    {
                        using (MemoryStream memstr = new MemoryStream(classGlobal.PLACEWATERMARK_FROM_BYTE(classGlobal.pub_personal.PhotoByte)))
                        {
                            Image img = Image.FromStream(memstr);
                            picDocument.BeginInvoke(new MethodInvoker(delegate { picDocument.Image = img; }));
                            picDocument.SizeMode = PictureBoxSizeMode.Zoom;
                        }
                    }
                }
            }
            else
            {
                if (txtVisitorNumber.InvokeRequired)
                {
                    txtID.BeginInvoke(new MethodInvoker(delegate { txtID.Text = ""; }));
                    txtFullname.BeginInvoke(new MethodInvoker(delegate { txtFullname.Text = ""; }));

                    picDocument.BeginInvoke(new MethodInvoker(delegate { picDocument.Image = null; }));
                    picDocument.SizeMode = PictureBoxSizeMode.Zoom;
                }
                else
                {
                    txtID.Text = "";
                    txtFullname.Text = "";
                    picDocument.Image = null;
                }

            }

            AUTO_CHECK_BLACKLIST();
            AUTO_CHECK_WHITELIST();
            if (classGlobal.visitorStatus != "blacklist")
                ClassData.CHECK_APPOINTMENT(classGlobal.personID, classGlobal.personName);

            try
            {
                if (classGlobal.pub_personal != null)
                {
                    string[] txtBD = classGlobal.pub_personal.ThBirthDate.Split('/');
                    txtBD[2] = (Int32.Parse(txtBD[2]) - 543).ToString();
                    classGlobal.CHECK_EXIST_IN_FOR_PUB(classGlobal.personID, classGlobal.personName, string.Join(@"/", txtBD));
                }                
            }catch
            {
                //--
            }
              
        }

        private void btnIDCard_Click_1(object sender, EventArgs e)
        {
            bool b = classGlobal.ProgramIsRunning(AppDomain.CurrentDomain.BaseDirectory + @"iFinTechIDCard-Engine.exe");
            if (b == true)
                return;

            //++ โค้ดแก้ปัญหาเฉพาะหน้า โดยการ shell exe  Engine.exe ขึ้นมาเพื่ออ่านบัตร แล้วเขียนลง textfile แล้วอ่านขึ้นมาเก็บในตัวแปล classGlobal.pub_personal
            if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"cid.ini") == true)
            { System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"cid.ini"); }


            ThaiIDReader frmThaiID = new ThaiIDReader();
            frmThaiID.ShowDialog();
            p_Exited(sender, e);
            return;

            Process p = new Process();
            p.Exited += new EventHandler(p_Exited);
            p.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + @"iFinTechIDCard-Engine.exe";
            p.StartInfo.Arguments = ""; // classGlobal.smartCardReaderName;
            p.EnableRaisingEvents = true;
            p.Start();
            //--

        }

        private void btnLicense_Click(object sender, EventArgs e)
        {
            txtFullname.Text = "";
            txtID.Text = "";

            using (var frm = new frmLicense())
            {
                frm.ShowDialog();

                txtID.Text = classGlobal.personID;
                txtFullname.Text = classGlobal.personName;

                if (classGlobal.personID != "")
                    txtID.Text = "#############";

                if (classGlobal.DisplayHashTag == true)
                {
                    //txtID.Text = classGlobal.REPLACE_IDCARD(classGlobal.personID);
                    txtFullname.Text = classGlobal.REPLACE_NAME(classGlobal.personName);
                }
            };

            AUTO_CHECK_BLACKLIST();
            AUTO_CHECK_WHITELIST();
            if (classGlobal.visitorStatus != "blacklist")
                ClassData.CHECK_APPOINTMENT(classGlobal.personID, classGlobal.personName);


            string kbLayout = InputLanguage.CurrentInputLanguage.Culture.Name;
            lbKeyboardLayout.Text = kbLayout.Replace("en-US", "EN").Replace("th-TH", "TH");

            try
            {
                string[] txtBD = classGlobal.strBirthDateInLicensePlate.Split('/');
                txtBD[2] = (Int32.Parse(txtBD[2]) - 0).ToString();
                classGlobal.CHECK_EXIST_IN_FOR_PUB(classGlobal.personID, classGlobal.personName, string.Join(@"/", txtBD));
            }
            catch
            {
                //--
            }
                
        }

        private void btnMoreInfo_Click(object sender, EventArgs e)
        {
            if (classGlobal.FactoryVersion == true)
            {
                FormMoreInfo f = new FormMoreInfo();
                f.ShowDialog();
            }
            else
            {
                using (var frm = new frmVehicle())
                {
                    frm.ShowDialog();
                };
            }

        }

        private void picWebcam_Click(object sender, EventArgs e)
        {
            if (classGlobal.WebCamConnect == false)
            {
                return;
            }

            if (classGlobal.statusIn_OUT == "IN")
            {
                classGlobal.cam = "cam1";
                frmTakePicture f = new frmTakePicture();
                f.ShowDialog();

                Image imgWebcam = clsImage.Base64ToImage(classGlobal.WebcamBase64String);
                if (imgWebcam == null)
                {
                    imgWebcam = new Bitmap(classGlobal.click4capture);
                }
                picWebcam.Image = imgWebcam;

                if (classGlobal.DisplayHashTag == true)
                {
                    imgWebcam = clsImage.Base64ToImage(classGlobal.PLACEWATERMARK_FROM_BASE64(classGlobal.WebcamBase64String));
                    if (imgWebcam == null)
                    {
                        imgWebcam = new Bitmap(classGlobal.click4capture);
                    }
                    picWebcam.Image = imgWebcam;
                }
                else
                {
                    imgWebcam = clsImage.Base64ToImage(classGlobal.PLACEWATERMARK_FROM_BASE64(classGlobal.WebcamBase64String));
                    if (imgWebcam == null)
                    {
                        imgWebcam = new Bitmap(classGlobal.click4capture);
                    }
                    picWebcam.Image = imgWebcam;
                }
            }
            else
            {
                //
            }

        }

        private void picWebcam1_Click(object sender, EventArgs e)
        {
            if (classGlobal.WebCamConnect == false)
            {
                return;
            }

            if (classGlobal.statusIn_OUT == "IN")
            {
                classGlobal.cam = "cam2";
                frmTakePicture f = new frmTakePicture();
                f.ShowDialog();

                Image imgWebcam = clsImage.Base64ToImage(classGlobal.WebcamBase64String1);
                if (imgWebcam == null)
                {
                    imgWebcam = new Bitmap(classGlobal.click4capture);
                }
                picWebcam1.Image = imgWebcam;

                if (classGlobal.DisplayHashTag == true)
                {
                    imgWebcam = clsImage.Base64ToImage(classGlobal.PLACEWATERMARK_FROM_BASE64(classGlobal.WebcamBase64String1));
                    if (imgWebcam == null)
                    {
                        imgWebcam = new Bitmap(classGlobal.click4capture);
                    }
                    picWebcam1.Image = imgWebcam;
                }
                else
                {
                    imgWebcam = clsImage.Base64ToImage(classGlobal.PLACEWATERMARK_FROM_BASE64(classGlobal.WebcamBase64String1));
                    if (imgWebcam == null)
                    {
                        imgWebcam = new Bitmap(classGlobal.click4capture);
                    }
                    picWebcam1.Image = imgWebcam;
                }
            }
            else
            {
                //
            }

        }

        private void picWebcam2_Click(object sender, EventArgs e)
        {
            if (classGlobal.WebCamConnect == false)
            {
                return;
            }

            if (classGlobal.statusIn_OUT == "IN")
            {
                classGlobal.cam = "cam3";
                frmTakePicture f = new frmTakePicture();
                f.ShowDialog();

                Image imgWebcam = clsImage.Base64ToImage(classGlobal.WebcamBase64String2);
                if (imgWebcam == null)
                {
                    imgWebcam = new Bitmap(classGlobal.click4capture);
                }
                picWebcam2.Image = imgWebcam;

                if (classGlobal.DisplayHashTag == true)
                {
                    imgWebcam = clsImage.Base64ToImage(classGlobal.PLACEWATERMARK_FROM_BASE64(classGlobal.WebcamBase64String2));
                    if (imgWebcam == null)
                    {
                        imgWebcam = new Bitmap(classGlobal.click4capture);
                    }
                    picWebcam2.Image = imgWebcam;
                }
                else
                {
                    imgWebcam = clsImage.Base64ToImage(classGlobal.PLACEWATERMARK_FROM_BASE64(classGlobal.WebcamBase64String2));
                    if (imgWebcam == null)
                    {
                        imgWebcam = new Bitmap(classGlobal.click4capture);
                    }
                    picWebcam2.Image = imgWebcam;
                }
            }
            else
            {
                //
            }

        }

        private void PRESS_OK_EVENT()
        {
            strAgreementSlip = System.IO.File.ReadAllText(@"slip_agreement.txt");

            //*** check values before insert 
            string ppString = classGlobal.PlustekBase64String;
            string wcString = classGlobal.WebcamBase64String;
            string wcString1 = classGlobal.WebcamBase64String1;
            string wcString2 = classGlobal.WebcamBase64String2;

            IDCardProfile idd = classGlobal.pub_personal;
            JToken jtPassport = classGlobal.pub_passport;
            //---

            strID = txtID.Text.Replace("-", "");
            strFullname = txtFullname.Text.Replace("-", "");

            if (txtID.Text.ToString().Replace(" ", "") == "")
                classGlobal.personID = "";
            if (txtFullname.Text.ToString().Replace(" ", "") == "")
                classGlobal.personName = "";

            if (classGlobal.personID == "")
                classGlobal.personID = strID;
            if (classGlobal.personName == "")
                classGlobal.personName = strFullname;

            if (txtVisitorNumber.Text == "")
            {
                //LabelMessageDisplay("กรุณากรอกเลข VISITOR", Color.Red);
                frmMessageBox f = new frmMessageBox();
                f.strMessage = "กรุณากรอกเลข VISITOR";
                f.strStatus = "Warning";
                f.ShowDialog();

                return;
            }

            if (classGlobal.statusIn_OUT == "IN")
            {
                if (classGlobal.WebCamConnect == true)
                {
                    if (classGlobal.WebcamBase64String == "")
                    {
                        //
                    }
                }

                string keyType = ((KeyValuePair<string, string>)listBox1.SelectedItem).Value;
                if (keyType == "-1")
                {
                    //LabelMessageDisplay("กรุณาระบุ ประเภทผู้มาติดต่อ", Color.Red);
                    frmMessageBox f = new frmMessageBox();
                    f.strMessage = "กรุณาระบุ ประเภทผู้มาติดต่อ";
                    f.strStatus = "Warning";
                    f.ShowDialog();
                    return;
                }

                classGlobal.visitorStatus = "normal";

                CHECK_WHITELIST(classGlobal.personID);

                if (CHECK_BLACKLIST(classGlobal.personID) == true)  
                {

                    frmMessageBox f = new frmMessageBox();
                    f.strMessage = "บุคคลต้องห้าม (Blacklist)!!";
                    f.strStatus = "Warning";
                    f.ShowDialog();

                    classGlobal.personID = "";
                    classGlobal.personName = "";

                    //++ test ++//
                    clsInfo.info_follower = "";
                    clsInfo.info_visitor_company = "";
                    clsInfo.info_license_plate = "";
                    clsInfo.info_vehicle_type = "";
                    clsInfo.info_visit_to = "";
                    clsInfo.info_department = "";
                    clsInfo.info_business_topic = "";
                    clsInfo.info_place = "";
                    clsInfo.info_etc = "";
                    classGlobal.destinationNotification = "";
                    picDocument.Image = null;
                    picWebcam.Image = null;
                    picWebcam1.Image = null;
                    picWebcam2.Image = null;
                    txtID.Text = "";
                    txtFullname.Text = "";
                    //-- test --//


                    return;
                }

                bool boolCheckIN = VISITORIN();
                classGlobal.statusCheckIN = boolCheckIN;
                if (boolCheckIN == false)
                {
                    #region clear
                    //clsInfo.info_follower = "";
                    //clsInfo.info_visitor_company = "";
                    //clsInfo.info_license_plate = "";
                    //clsInfo.info_vehicle_type = "";
                    //clsInfo.info_visit_to = "";
                    //clsInfo.info_department = "";
                    //clsInfo.info_business_topic = "";
                    //clsInfo.info_place = "";
                    //clsInfo.info_etc = "";
                    ////++ clear
                    #endregion
                    return;
                }

                if (classGlobal.FactoryVersion == true)
                {
                    classGlobal.INSERT_MOREIN_FACTORY(classGlobal.pub_id);


                    // ++ แสดง Preview ว่าจะพิมพ์สลิปหรือไม่
                    //int line = 0;
                    bool prnStatus = classGlobal.CheckPOC_ONLINE(classGlobal.printThermalName);
                    if (prnStatus == true)   
                    {
                        if (clsInfo.info_follower == null || clsInfo.info_follower == "")  //ผู้ติดตาม
                            clsInfo.info_follower = "1";                       
                        if (clsInfo.info_license_plate == null || clsInfo.info_license_plate =="")  //ทะเบียนรถ
                            clsInfo.info_license_plate = "         ";
                        if (clsInfo.info_vehicle_type == null || clsInfo.info_vehicle_type == "")   //ประเภทรถ
                            clsInfo.info_vehicle_type = "          ";
                        if (clsInfo.info_visitor_company == null || clsInfo.info_visitor_company == "")  //จากบริษัท
                            clsInfo.info_visitor_company = "          ";
                        if (clsInfo.info_business_topic == null || clsInfo.info_business_topic == "")  //ติดต่อเรื่อง
                            clsInfo.info_business_topic = "          ";
                        if (clsInfo.info_visit_to == null || clsInfo.info_visit_to == "")  //ผู้รับการติดต่อ
                            clsInfo.info_visit_to = "            ";
                        if (clsInfo.info_department == null || clsInfo.info_department == "")   //แผนกที่ติดต่อ
                            clsInfo.info_department = "          ";
                        if (clsInfo.info_place == null || clsInfo.info_place == "")   //สถานที่ติดต่อ
                            clsInfo.info_place = "          ";
                        if (clsInfo.info_etc == null || clsInfo.info_etc == "")   //ข้อมูลอื่นๆ
                            clsInfo.info_etc = "          ";


                        string strVisitorNumber = txtVisitorNumber.Text;

                        string text = "";
                        string[] arrIN = pub_in.Split(' '); 
                        text += classGlobal.strPlace + Environment.NewLine;
                        text += "ใบผ่านเข้า-ออก ผู้มาติดต่อ  หมายเลข " + strVisitorNumber + Environment.NewLine;
                        text += "เวลาเข้า : " + arrIN[1] + "   วันที่ : " + arrIN[0]  + Environment.NewLine;
                        text += "ประเภทผู้มาติดต่อ : " + lb4Text.Text + Environment.NewLine;
                        text += "จำนวน : " + clsInfo.info_follower.ToString() + " คน" + Environment.NewLine;
                        text += "ชื่อ-สกุล : " + txtFullname.Text + Environment.NewLine;
                        text += "เลขประจำตัว : " + txtID.Text + Environment.NewLine;
                        text += "ทะเบียนรถ : " + clsInfo.info_license_plate.ToString() + Environment.NewLine;
                        text += "ประเภทรถ : " + clsInfo.info_vehicle_type + Environment.NewLine;
                        text += "จากบริษัท : " + clsInfo.info_visitor_company.ToString() + Environment.NewLine;
                        text += "ติดต่อเรื่อง : " + clsInfo.info_business_topic.ToString() + Environment.NewLine;
                        text += "ผู้รับการติดต่อ : " + clsInfo.info_visit_to.ToString() + Environment.NewLine;
                        text += "แผนก : " + clsInfo.info_department + Environment.NewLine;
                        text += "สถานที่ติดต่อ : " + clsInfo.info_place.ToString() + Environment.NewLine;
                        text += "ข้อมูลอื่นๆ : " + clsInfo.info_etc.ToString() + Environment.NewLine + Environment.NewLine + Environment.NewLine;


                        string text1 = "ทรัพย์สินที่นำเข้า [ ] มี  [ ] ไม่มี" + Environment.NewLine +
                                    "	" + Environment.NewLine +
                                    "มี (ระบุ)..................................................." + Environment.NewLine +
                                    "ทรัพย์สินที่ฝาก รปภ. [ ] มี  [ ] ไม่มี" + Environment.NewLine +
                                    "	" + Environment.NewLine +
                                    "มี (ระบุ)...................................................";                   
                        
                        string text2 = Environment.NewLine + strAgreementSlip; //"ท่านได้อ่านและเข้าใจกฏระเบียบว่าด้วยความปลอดภัย";

                        string text3 = Environment.NewLine + "                               " + Environment.NewLine + Environment.NewLine + 
                                        "ลงชื่อผู้มาติดต่อ......................................." + Environment.NewLine + Environment.NewLine +
                                        "                               " + Environment.NewLine + Environment.NewLine +
                                        "ลงชื่อผู้รับการติดต่อ..................................." + Environment.NewLine + Environment.NewLine +
                                        "                               " + Environment.NewLine + Environment.NewLine +
                                        "ลงชื่อเจ้าหน้าที่ รปภ. ................................";

                        FormSetting.FormPreviewIN fpIN = new FormPreviewIN(strVisitorNumber, arrIN, lb4Text.Text, txtFullname.Text, txtID.Text);
                        fpIN.ShowDialog();
                        if (classGlobal.PreviewPrintIN == "Y")
                        {
                            rbtnReprint.Enabled = true;

                            string codeType = ""; //  barcode / qrcode   
                            ClassHelper.clsXML cls = new clsXML();
                            try
                            {
                                if (cls.CheckExistElement("root", "barcodetype", classGlobal.config) == false)
                                {
                                    cls.AppendElement("root", "barcodetype", "-", classGlobal.config);
                                }
                                codeType = cls.GetReadXML("root", "barcodetype", classGlobal.config).Replace("-", "");
                            }
                            catch
                            {
                                codeType = "";
                            }
                            cls = null;
                            //--

                            string cardID = strVisitorNumber;
                            Bitmap qrbc = null; 
                            if (codeType != "")
                            {
                                //+++ BARCODE/QR
                                Bitmap QR_BARCODE = null;
                                Bitmap resultImage = null;

                                Zen.Barcode.Code128BarcodeDraw barcode = Zen.Barcode.BarcodeDrawFactory.Code128WithChecksum;
                                Zen.Barcode.CodeQrBarcodeDraw qrcode = Zen.Barcode.BarcodeDrawFactory.CodeQr;

                                if (codeType == "barcode")
                                { QR_BARCODE = (Bitmap)barcode.Draw(cardID, 80, 1); }
                                if (codeType == "qrcode")
                                { QR_BARCODE = (Bitmap)qrcode.Draw(cardID, 5, 2); }

                                resultImage = new Bitmap(QR_BARCODE.Width, QR_BARCODE.Height + 20); // 20 is bottom padding, adjust to your text

                                using (var graphics = Graphics.FromImage(resultImage))
                                using (var font = new Font("Tahoma", 10))
                                using (var brush = new SolidBrush(Color.Black))
                                using (var format = new StringFormat()
                                {
                                    Alignment = StringAlignment.Center, // Also, horizontally centered text, as in your example of the expected output
                                    LineAlignment = StringAlignment.Far
                                })
                                {
                                    graphics.Clear(Color.White);
                                    graphics.DrawImage(QR_BARCODE, 0, 0);
                                }

                                qrbc = QR_BARCODE;
                            }
                            else
                            {

                            }

                            PRINT_SLIP_THERMAL(text, text1, text2, text3, qrbc);
                        }
                        else
                        {
                            rbtnReprint.Enabled = false;
                        }

                    }
                    // -- แสดง Preview ว่าจะพิมพ์สลิปหรือไม่


                    //++ clear
                    clsInfo.info_follower = "";
                    clsInfo.info_visitor_company = "";
                    clsInfo.info_license_plate = "";
                    clsInfo.info_vehicle_type = "";
                    clsInfo.info_visit_to = "";
                    clsInfo.info_department = "";
                    clsInfo.info_business_topic = "";
                    clsInfo.info_place = "";
                    clsInfo.info_etc = "";
                    classGlobal.destinationNotification = "";
                    //++ clear
                    
                }

                frmMessageBox fsuccess = new frmMessageBox();
                fsuccess.strMessage = "สำเร็จ";
                fsuccess.strStatus = "Information";
                fsuccess.ShowDialog();
                
            }
            else
            {
                VISITOROUT(txtVisitorNumber.Text);
            }

            classGlobal.WebcamBase64String = "";
            classGlobal.WebcamBase64String1 = "";
            classGlobal.WebcamBase64String2 = "";
            classGlobal.PlustekBase64String = "";
            classGlobal.pub_personal = null;
            classGlobal.pub_passport = null;

            classGlobal.personID = "";
            classGlobal.personName = "";
            
        }

        private static bool CHECK_BLACKLIST(string personID)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            CultureInfo MyCultureInfo = new CultureInfo("en-US");
            DateTime MyDateTimeStart;
            DateTime MyDateTimeStop;
            long CurrentTicks = DateTime.Now.Ticks;
            DateTime MyDateTime;

            bool result = false;
            try
            {
                DataTable dt = new DataTable("dt");
                string query = String.Format("SELECT * FROM tbl_blacklist WHERE personal_number='{0}'", personID);

                if (classGlobal.databaseType == "acc")
                {
                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, classGlobal.conn);
                    adapter.Fill(dt);
                    adapter.Dispose();
                    adapter = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, classGlobal.connP);
                    adapter.Fill(dt);
                    adapter.Dispose();
                    adapter = null;
                }
                else
                {
                    dt.Columns.Add("ID");
                    dt.Columns.Add("personal_number");
                    dt.Columns.Add("fullname");
                    dt.Columns.Add("start");
                    dt.Columns.Add("stop");

                    string jsonString = ClassData.GET_METHODE_BLACK_WHITE_LIST("blacklist");
                    JArray jsArray = JArray.Parse(jsonString);
                    int j = 0;
                    foreach (var x in jsArray)
                    {
                        if (personID == x["citizenId"].ToString())
                        {
                            JToken jt = x["time"];
                            foreach (var ja in jt)
                            {
                                dt.Rows.Add(j, x["citizenId"], x["name"], ja["timeStart"], ja["timeStop"]);
                                j += 1;
                            }                            
                        }    
                    }
                }

                if (dt.Rows.Count > 0)
                {
                    long startTick = 0;
                    long stopTick = 0;

                    if (classGlobal.userId != "")
                    {
                        string currentLocalDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        foreach (DataRow dr in dt.Rows)
                        {

                            MyDateTimeStart = DateTime.Parse(dr.ItemArray[3].ToString(), MyCultureInfo);
                            startTick = MyDateTimeStart.Ticks;

                            MyDateTimeStop = DateTime.Parse(dr.ItemArray[4].ToString(), MyCultureInfo);
                            stopTick = MyDateTimeStop.Ticks;


                            //++ new revise time utc to local
                            string startLocalDateTime = "";
                            startLocalDateTime = MyDateTimeStart.ToString("yyyy-MM-dd HH:mm:ss");
                            startLocalDateTime = classGlobal.CONVERT_UTC_TO_LOCAL(startLocalDateTime);

                            string stopLocalDateTime = "";
                            stopLocalDateTime = MyDateTimeStop.ToString("yyyy-MM-dd HH:mm:ss");
                            stopLocalDateTime = classGlobal.CONVERT_UTC_TO_LOCAL(stopLocalDateTime);

                            CurrentTicks = DateTime.Parse(currentLocalDateTime).Ticks;
                            startTick = DateTime.Parse(startLocalDateTime).Ticks;
                            stopTick = DateTime.Parse(stopLocalDateTime).Ticks;
                            //-- new revise time utc to local

                            if (CurrentTicks > startTick && CurrentTicks < stopTick)
                            {
                                result = true;
                                classGlobal.visitorStatus = "blacklist";
                                break;
                            }

                        }
                    }
                    else
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            MyDateTime = DateTime.Parse(dr.ItemArray[3].ToString(), MyCultureInfo);
                            startTick = MyDateTime.Ticks;

                            MyDateTime = DateTime.Parse(dr.ItemArray[4].ToString(), MyCultureInfo);
                            stopTick = MyDateTime.Ticks;

                            if (CurrentTicks > startTick && CurrentTicks < stopTick)
                            {
                                result = true;
                                classGlobal.visitorStatus = "blacklist";
                                break;
                            }

                        }
                    }

                }
                else
                {
                    result = false;
                    classGlobal.visitorStatus = "normal";
                }

                dt.Dispose();
                dt = null;

                return result;
            }
            catch
            {
                classGlobal.visitorStatus = "normal";
                return result;
            }
        }

        private static void CHECK_WHITELIST(string personID)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            CultureInfo MyCultureInfo = new CultureInfo("en-US");
            DateTime MyDateTime;
            DateTime MyDateTimeStart;
            DateTime MyDateTimeStop;
            long CurrentTicks = DateTime.Now.Ticks;

            //bool result = false;
            try
            {
                DataTable dt = new DataTable("dt");
                string query = String.Format("SELECT * FROM tbl_whitelist WHERE personal_number='{0}'", personID);

                if (classGlobal.databaseType == "acc")
                {
                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, classGlobal.conn);
                    adapter.Fill(dt);
                    adapter.Dispose();
                    adapter = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, classGlobal.connP);
                    adapter.Fill(dt);
                    adapter.Dispose();
                    adapter = null;
                }
                else
                {
                    dt.Columns.Add("ID");
                    dt.Columns.Add("personal_number");
                    dt.Columns.Add("fullname");
                    dt.Columns.Add("start");
                    dt.Columns.Add("stop");

                    string jsonString = ClassData.GET_METHODE_BLACK_WHITE_LIST("whitelist");
                    JArray jsArray = JArray.Parse(jsonString);
                    int j = 0;
                    foreach (var x in jsArray)
                    {
                        if (personID == x["citizenId"].ToString())
                        {
                            JToken jt = x["time"];
                            foreach (var ja in jt)
                            {
                                dt.Rows.Add(j, x["citizenId"], x["name"], ja["timeStart"], ja["timeStop"]);
                                j += 1;
                            }     
                            //dt.Rows.Add(j, x["citizenId"], x["name"], x["timeStart"], x["timeStop"]);
                            //j += 1;
                        }  
                    }
                }

                if (dt.Rows.Count > 0)
                {
                    long startTick = 0;
                    long stopTick = 0;

                    if (classGlobal.userId != "")
                    {
                        string currentLocalDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        foreach (DataRow dr in dt.Rows)
                        {

                            MyDateTimeStart = DateTime.Parse(dr.ItemArray[3].ToString(), MyCultureInfo);
                            startTick = MyDateTimeStart.Ticks;

                            MyDateTimeStop = DateTime.Parse(dr.ItemArray[4].ToString(), MyCultureInfo);
                            stopTick = MyDateTimeStop.Ticks;


                            //++ new revise time utc to local
                            string startLocalDateTime = "";
                            startLocalDateTime = MyDateTimeStart.ToString("yyyy-MM-dd HH:mm:ss");
                            startLocalDateTime = classGlobal.CONVERT_UTC_TO_LOCAL(startLocalDateTime);

                            string stopLocalDateTime = "";
                            stopLocalDateTime = MyDateTimeStop.ToString("yyyy-MM-dd HH:mm:ss");
                            stopLocalDateTime = classGlobal.CONVERT_UTC_TO_LOCAL(stopLocalDateTime);

                            CurrentTicks = DateTime.Parse(currentLocalDateTime).Ticks;
                            startTick = DateTime.Parse(startLocalDateTime).Ticks;
                            stopTick = DateTime.Parse(stopLocalDateTime).Ticks;
                            //-- new revise time utc to local

                            if (CurrentTicks > startTick && CurrentTicks < stopTick)
                            {
                                //result = true;
                                classGlobal.visitorStatus = "whitelist";
                                break;
                            }

                        }
                    }
                    else
                    {

                        foreach (DataRow dr in dt.Rows)
                        {
                            MyDateTime = DateTime.Parse(dr.ItemArray[3].ToString(), MyCultureInfo);
                            startTick = MyDateTime.Ticks;

                            MyDateTime = DateTime.Parse(dr.ItemArray[4].ToString(), MyCultureInfo);
                            stopTick = MyDateTime.Ticks;

                            if (CurrentTicks > startTick && CurrentTicks < stopTick)
                            {
                                //result = true;
                                classGlobal.visitorStatus = "whitelist";
                                break;
                            }

                        }
                    }
                }
                else
                {
                    //result = false;
                    classGlobal.visitorStatus = "normal";
                }

                dt.Dispose();
                dt = null;

                
            }
            catch
            {
                classGlobal.visitorStatus = "normal";
            }
        }

        //int pub_qr_location_y = 0;
        string pub_in = "";
        private bool VISITORIN()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            string idcompany = companyName;   // companyid
            string card_number = txtVisitorNumber.Text;  // visitor number 
            string str_imagedocument = classGlobal.PlustekBase64String;  // ภาพจาก plutek
            string str_imagewebcamera = classGlobal.WebcamBase64String;   // ภาพจาก webcam
            string str_imagewebcamera1 = classGlobal.WebcamBase64String1;   // ภาพจาก webcam
            string str_imagewebcamera2 = classGlobal.WebcamBase64String2;   // ภาพจาก webcam
            string str_template = "";   // ภาพ template ลายนิ้วมือ --- ยังไม่ใช้ตอนนี้
            string str_datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");   //2018-09-18 10:10:10
            string keyType = ((KeyValuePair<string, string>)listBox1.SelectedItem).Value;
            string vType = ((KeyValuePair<string, string>)listBox1.SelectedItem).Key;

            bool result = FUNCTION_INSERT(idcompany, card_number, str_imagedocument, str_imagewebcamera, str_template, str_datetime, Int32.Parse(keyType), str_imagewebcamera1, str_imagewebcamera2);

            if (result == true)
            {
                // show detail
                string strIN = str_datetime;
                int beYearFormat = 0;
                string[] arr1 = strIN.Split(' ');
                string[] arr2 = arr1[0].Split('-');
                beYearFormat = Int32.Parse(arr2[0]) + 543;
                strIN = arr2[2] + "/" + arr2[1] + "/" + beYearFormat.ToString() + " " + arr1[1];
                string strOUT = "-";
                lb1Text.Text = card_number;
                lb2Text.Text = strIN;
                lb3Text.Text = strOUT;
                lb4Text.Text = listBox1.Text;

                pub_in = strIN;

                classGlobal.public_strIN = strIN;
                classGlobal.public_GuestName = txtFullname.Text;     

                INSERT_IDCARD_INFO(classGlobal.pub_id);
                INSERT_PASSPORT_INFO(classGlobal.pub_id);

                timer1.Enabled = true;
                btnOKClick = false;

                //++ clear ข้อมูลเป็นค่าว่าง
                classGlobal.arrVehicleInfo.Clear();
                //--
            
            }
            else
            {
                frmMessageBox f = new frmMessageBox();
                f.strMessage = "กรุณาตรวจสอบเลข VISITOR";
                f.strStatus = "Warning";
                f.ShowDialog();
            }

            return result;
        }
        public void VISITOROUT(string visitorNumber)
        {
            rbtnReprint.Enabled = false;

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            string idcompany = companyName;   // companyid
            string card_number = txtVisitorNumber.Text;  // visitor number 
            string str_datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");   //2018-09-18 10:10:10

            if (FinalFrame != null)
            {
                FinalFrame.Stop();
            }

            string result = FUNCTION_UPDATE(idcompany, card_number, str_datetime);

            if (result == "false")
            {
                frmMessageBox f = new frmMessageBox();
                f.strMessage = "กรุณาตรวจสอบเลข VISITOR";
                f.strStatus = "Warning";
                f.ShowDialog();
            }
            else
            {

                #region ส่งค่าไปยัง SERVER
                if (classGlobal.userId != "")
                {
                    string visitorId = "";
                    string image1 = "";
                    string image2 = "";
                    string image3 = "";
                    string image4 = "";
                    string visiterNumber = "";
                    string citizenId = "";
                    string name = "";
                    string visitorType = "";
                    string recordTimeIn = "";
                    string recordTimeOut = "";
                    string visitPlace = "";
                    string licensePlate = "";
                    string vehicleType = "";
                    string terminalName = "";
                    string follower = "";
                    string visitorFrom = "";
                    string department = "";
                    string visitPerson = "";
                    string contactTopic = "";
                    string contactPlace = "";
                    string etc = "";
                    string timestamp = "";
                                      
                    JObject json = JObject.Parse(result);
                    JToken jMessage = json["message"];
                    JArray jsArray = (JArray)jMessage;
                    JArray jsArrImage;
                    string strImage1 = "";
                    string strImage2 = "";
                    string strImage3 = "";
                    string strImage4 = "";
                    foreach (var x in jsArray)
                    {
                        jsArrImage = (JArray)x["image1"];
                        foreach (var y in jsArrImage)
                            strImage1 = y.ToString();

                        jsArrImage = (JArray)x["image2"];
                        foreach (var y in jsArrImage)
                            strImage2 = y.ToString();

                        jsArrImage = (JArray)x["image3"];
                        foreach (var y in jsArrImage)
                            strImage3 = y.ToString();

                        jsArrImage = (JArray)x["image4"];
                        foreach (var y in jsArrImage)
                            strImage4 = y.ToString();

                        visitorId = x["visitorId"].ToString();
                        classGlobal.visitorId_for_out_event = visitorId;
                        image1 = ClassData.DOWNLOAD_IMAGE(strImage1, 130, 80);
                        image2 = ClassData.DOWNLOAD_IMAGE(strImage2, 130, 80);
                        image3 = ClassData.DOWNLOAD_IMAGE(strImage3, 130, 80);
                        image4 = ClassData.DOWNLOAD_IMAGE(strImage4, 130, 80);
                        visiterNumber = x["visitorNumber"].ToString();
                        citizenId = x["citizenId"].ToString();
                        name = x["name"].ToString();
                        visitorType = x["visitorType"].ToString();
                        recordTimeIn = classGlobal.CONVERT_UTC_TO_LOCAL(x["recordTimeIn"].ToString());
                        recordTimeOut = classGlobal.CONVERT_UTC_TO_LOCAL(x["recordTimeOut"].ToString());
                        visitPlace = x["contactPlace"].ToString();
                        licensePlate = x["licensePlate"].ToString();
                        vehicleType = x["vehicleType"].ToString();
                        terminalName = x["terminalIn"].ToString();
                        follower = x["follower"].ToString();
                        visitorFrom = x["visitorFrom"].ToString();
                        department = x["department"].ToString();
                        visitPerson = x["visitPerson"].ToString();
                        contactTopic = x["contactTopic"].ToString();
                        contactPlace = x["contactPlace"].ToString();
                        etc = x["etc"].ToString();
                        timestamp = classGlobal.CONVERT_UTC_TO_LOCAL(x["timestamp"].ToString()); 

                    }
                    
                    string stringReturn = ClassData.POST_VISITOR_OUT(classGlobal.access_token, classGlobal.userId, visitorId, classGlobal.strPlace);
                    json = JObject.Parse(stringReturn);
                    stringReturn = json["status"].ToString();
                    jMessage = json["message"];
                    if (stringReturn == "200")
                    {
                        recordTimeOut = jMessage["recordTimeOut"].ToString();
                        recordTimeOut = classGlobal.CONVERT_UTC_TO_LOCAL(recordTimeOut); 
                        result = visiterNumber + "@" +
                                    image1 + "@" +
                                    image2 + "@" +
                                    recordTimeIn + "@" +
                                    recordTimeOut + "@" +
                                    visitorType + "@" +
                                    image3 + "@" +
                                    image4;
                    }
                    else
                    {
                        timer1.Enabled = true;
                        btnOKClick = false;
                        MessageBox.Show(stringReturn, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); 
                        return;
                    }
                    
                }
                #endregion

                // show detail
                string[] strDetail = result.Split('@');
                card_number = strDetail[0];
                string base64Document = strDetail[1].ToString();
                string base64Webcam = strDetail[2].ToString();
                string base64Webcam1 = strDetail[6].ToString();
                string base64Webcam2 = strDetail[7].ToString();
                string strIN = strDetail[3];  //2018-11-01 HH:mm:ss
                string strOUT = strDetail[4];
                string visiterType = strDetail[5];

                //++ เช็คว่าเป็น visitor type ที่มีค่าใช้จ่ายในการจอดรถรึเปล่า
                Int32 typesInt = classGlobal.VISITOR_TYPE_CHARGE_STATUS(visiterType);
                if (typesInt > 0)
                {
                    classGlobal.pub_MinuteCharge = classGlobal.COMPARE_BETWEEN_TIME_TO_MINUTES(strIN, strOUT); // เปรียบเทียบเวลาเข้า - ออก เป็นนาที
                }
                //-- เช็คว่าเป็น visitor type ที่มีค่าใช้จ่ายในการจอดรถรึเปล่า

                try
                {
                    int beYearFormat = 0;
                    string[] arr1 = strIN.Split(' ');
                    string[] arr2 = arr1[0].Split('-');
                    beYearFormat = Int32.Parse(arr2[0]) + 543;
                    strIN = arr2[2] + "/" + arr2[1] + "/" + beYearFormat.ToString() + " " + arr1[1];

                    arr1 = strOUT.Split(' ');
                    arr2 = arr1[0].Split('-');
                    beYearFormat = Int32.Parse(arr2[0]) + 543;
                    strOUT = arr2[2] + "/" + arr2[1] + "/" + beYearFormat.ToString() + " " + arr1[1];
                }
                catch
                {
                    //
                }

                if (base64Document == "")
                {
                    base64Document = clsImage.ImageToBase64(classGlobal.unknown);
                }
                if (base64Webcam == "")
                {
                    base64Webcam = clsImage.ImageToBase64(classGlobal.unknown);
                }
                if (base64Webcam1 == "")
                {
                    base64Webcam1 = clsImage.ImageToBase64(classGlobal.unknown);
                }
                if (base64Webcam2 == "")
                {
                    base64Webcam2 = clsImage.ImageToBase64(classGlobal.unknown);
                }

                lb1Text.Text = card_number;
                lb2Text.Text = strIN;
                lb3Text.Text = strOUT;
                lb4Text.Text = visiterType;
                picDocument.Image = clsImage.Base64ToImage(base64Document);
                picWebcam.Image = clsImage.Base64ToImage(base64Webcam);
                picWebcam1.Image = clsImage.Base64ToImage(base64Webcam1);
                picWebcam2.Image = clsImage.Base64ToImage(base64Webcam2);

                //++ get more information from thaiid / passport
                string[] info = Retrive_PersonalID_And_Name(classGlobal.pub_id);
                txtID.Text = info[0].ToString().Replace("-", "");
                txtFullname.Text = info[1].ToString().Replace("-", "");

                txtID.ReadOnly = true;
                txtFullname.ReadOnly = true;
                //--

                txtID.Text = "############";
                if (classGlobal.DisplayHashTag == true)
                {
                    lb1Text.Text = card_number;
                    lb2Text.Text = strIN;
                    lb3Text.Text = strOUT;
                    lb4Text.Text = visiterType;
                    picDocument.Image = clsImage.Base64ToImage(classGlobal.PLACEWATERMARK_FROM_BASE64(base64Document));
                    picWebcam.Image = clsImage.Base64ToImage(classGlobal.PLACEWATERMARK_FROM_BASE64(base64Webcam));
                    picWebcam1.Image = clsImage.Base64ToImage(classGlobal.PLACEWATERMARK_FROM_BASE64(base64Webcam1));
                    picWebcam2.Image = clsImage.Base64ToImage(classGlobal.PLACEWATERMARK_FROM_BASE64(base64Webcam2));

                    //++ get more information from thaiid / passport
                    info = Retrive_PersonalID_And_Name(classGlobal.pub_id);
                    //txtID.Text = classGlobal.REPLACE_IDCARD(info[0].ToString().Replace("-", ""));
                    txtFullname.Text = classGlobal.REPLACE_NAME(info[1].ToString().Replace("-", ""));

                    txtID.ReadOnly = true;
                    txtFullname.ReadOnly = true;
                    //--
                }
                else
                {
                    picDocument.Image = clsImage.Base64ToImage(classGlobal.PLACEWATERMARK_FROM_BASE64(base64Document));
                    picWebcam.Image = clsImage.Base64ToImage(classGlobal.PLACEWATERMARK_FROM_BASE64(base64Webcam));
                    picWebcam1.Image = clsImage.Base64ToImage(classGlobal.PLACEWATERMARK_FROM_BASE64(base64Webcam1));
                    picWebcam2.Image = clsImage.Base64ToImage(classGlobal.PLACEWATERMARK_FROM_BASE64(base64Webcam2));
                }

                listBox1.Text = visiterType;
                //LabelMessageDisplay("สำเร็จ", Color.Yellow);
                frmMessageBox f = new frmMessageBox();
                f.strMessage = "สำเร็จ";
                f.strStatus = "Information";
                f.ShowDialog();
               
                timer1.Enabled = true;
                btnOKClick = false;

                if (classGlobal.boolCharge == false)
                {
                    typesInt = 0;
                }

                if (typesInt > 0)
                {
                    ChargeParking f1 = new ChargeParking(visiterType, classGlobal.pub_MinuteCharge, card_number, strIN, strOUT);
                    f1.ShowDialog();
                }
            }

        }

        private string GET_MOREINFO(int id)
        {
            string text = "";
            DataTable _dt = new DataTable("_dt");

            if (classGlobal.databaseType == "acc")
            {
                OleDbCommand cmd = new OleDbCommand("SELECT * FROM tbl_moreinfo WHERE id=" + id, classGlobal.conn);
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                da.Fill(_dt);
                da.Dispose();
                da = null;
            }
            else if (classGlobal.databaseType == "psql")
            {
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM tbl_moreinfo WHERE id=" + id, classGlobal.connP);
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                da.Fill(_dt);
                da.Dispose();
                da = null;
            }
            else
            {

            }

            if (_dt.Rows.Count == 0)
            {
                text = "";
            }
            else
            {
                text += "สถานที่ติดต่อ : " + _dt.Rows[0][1].ToString() + Environment.NewLine;
                text += "เลขทะเบียนรถ : " + _dt.Rows[0][2].ToString() + Environment.NewLine;
                text += "ชนิดของรถ : " + _dt.Rows[0][3].ToString() + Environment.NewLine;
                text += "อื่นๆ : " + _dt.Rows[0][4].ToString() + Environment.NewLine;
            }

            return text;

        }
        private void INSERT_IDCARD_INFO(int id)
        {
            if (classGlobal.pub_personal != null)
            {
                IDCardProfile personal = classGlobal.pub_personal;

                string gendar = "ชาย";
                if (personal.Sex == "F")
                { gendar = "หญิง"; }

                string strBirthDate = personal.ThBirthDate.ToString();
                strBirthDate = strBirthDate.Replace(".", "/").Replace("-", "/").Replace(",", "/").Replace(" ", "/").Replace("//", "/");
                strBirthDate = strBirthDate.Substring(0, 6) + classGlobal.CheckYear(strBirthDate);
                string strIssueDate = personal.ThIssueDate.ToString();
                strIssueDate = strIssueDate.Replace(".", "/").Replace("-", "/").Replace(",", "/").Replace(" ", "/").Replace("//", "/");
                strIssueDate = strIssueDate.Substring(0, 6) + classGlobal.CheckYear(strIssueDate);
                string strExpireDate = personal.ThExpiryDate.ToString();
                strExpireDate = strExpireDate.Replace(".", "/").Replace("-", "/").Replace(",", "/").Replace(" ", "/").Replace("//", "/");
                strExpireDate = strExpireDate.Substring(0, 6) + classGlobal.CheckYear(strExpireDate);

                if (classGlobal.databaseType == "acc")
                {
                    string query = "INSERT INTO tbl_idcard (id, id_number, birthdate, gendar, th_title, th_firstname, th_lastname, " +
                                                  "en_title, en_firstname, en_lastname, issuedate, expiredate, " +
                                                  "house_no, village_no, lane, road, subdistrict, district, province, photo) " +
                                                  "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                    OleDbCommand command = new OleDbCommand(query, classGlobal.conn);
                    command.Parameters.Add("@id", OleDbType.Integer).Value = id;
                    command.Parameters.Add("@id_number", OleDbType.VarChar).Value = personal.CitizenID.ToString();
                    command.Parameters.Add("@birthdate", OleDbType.VarChar).Value = strBirthDate;
                    command.Parameters.Add("@gendar", OleDbType.VarChar).Value = gendar;
                    command.Parameters.Add("@th_title", OleDbType.VarChar).Value = personal.ThPreName.ToString();
                    command.Parameters.Add("@th_firstname", OleDbType.VarChar).Value = personal.ThFirstName.ToString();
                    command.Parameters.Add("@th_lastname", OleDbType.VarChar).Value = personal.ThLastName.ToString();
                    command.Parameters.Add("@en_title", OleDbType.VarChar).Value = personal.EnPreName.ToString();
                    command.Parameters.Add("@en_firstname", OleDbType.VarChar).Value = personal.EnFirstName.ToString();
                    command.Parameters.Add("@en_lastname", OleDbType.VarChar).Value = personal.EnLastName.ToString();
                    command.Parameters.Add("@issuedate", OleDbType.VarChar).Value = strIssueDate;
                    command.Parameters.Add("@expiredate", OleDbType.VarChar).Value = strExpireDate;
                    command.Parameters.Add("@house_no", OleDbType.VarChar).Value = personal.AddressHouseNo.ToString();
                    command.Parameters.Add("@village_no", OleDbType.VarChar).Value = personal.AddressVillageNo.ToString();
                    command.Parameters.Add("@lane", OleDbType.VarChar).Value = personal.AddressLane.ToString();
                    command.Parameters.Add("@road", OleDbType.VarChar).Value = personal.AddressRoad.ToString();
                    command.Parameters.Add("@subdistrict", OleDbType.VarChar).Value = personal.AddressSubDistrict.ToString();
                    command.Parameters.Add("@district", OleDbType.VarChar).Value = personal.AddressDistrict.ToString();
                    command.Parameters.Add("@province", OleDbType.VarChar).Value = personal.AddressProvince.ToString();
                    command.Parameters.Add("@photo", OleDbType.VarWChar).Value = Convert.ToBase64String(personal.PhotoByte);
                    command.ExecuteNonQuery();
                    command.Dispose();
                    command = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    string query = "INSERT INTO tbl_idcard (id, id_number, birthdate, gendar, th_title, th_firstname, th_lastname, " +
                                                 "en_title, en_firstname, en_lastname, issuedate, expiredate, " +
                                                 "house_no, village_no, lane, road, subdistrict, district, province, photo) " +
                                                 "VALUES " +
                                                 "(" +
                                                 "@id, @id_number, @birthdate, @gendar, @th_title, @th_firstname, @th_lastname, " +
                                                 "@en_title, @en_firstname, @en_lastname, @issuedate, @expiredate, " +
                                                 "@house_no, @village_no, @lane, @road, @subdistrict, @district, @province, @photo" +
                                                 ")";

                    NpgsqlCommand command = new NpgsqlCommand(query, classGlobal.connP);
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@id_number", personal.CitizenID.ToString());
                    command.Parameters.AddWithValue("@birthdate", strBirthDate);
                    command.Parameters.AddWithValue("@gendar", gendar);
                    command.Parameters.AddWithValue("@th_title", personal.ThPreName.ToString());
                    command.Parameters.AddWithValue("@th_firstname",personal.ThFirstName.ToString());
                    command.Parameters.AddWithValue("@th_lastname", personal.ThLastName.ToString());
                    command.Parameters.AddWithValue("@en_title", personal.EnPreName.ToString());
                    command.Parameters.AddWithValue("@en_firstname", personal.EnFirstName.ToString());
                    command.Parameters.AddWithValue("@en_lastname", personal.EnLastName.ToString());
                    command.Parameters.AddWithValue("@issuedate", strIssueDate);
                    command.Parameters.AddWithValue("@expiredate", strExpireDate);
                    command.Parameters.AddWithValue("@house_no", personal.AddressHouseNo.ToString());
                    command.Parameters.AddWithValue("@village_no", personal.AddressVillageNo.ToString());
                    command.Parameters.AddWithValue("@lane", personal.AddressLane.ToString());
                    command.Parameters.AddWithValue("@road",personal.AddressRoad.ToString());
                    command.Parameters.AddWithValue("@subdistrict", personal.AddressSubDistrict.ToString());
                    command.Parameters.AddWithValue("@district", personal.AddressDistrict.ToString());
                    command.Parameters.AddWithValue("@province", personal.AddressProvince.ToString());
                    command.Parameters.AddWithValue("@photo", Convert.ToBase64String(personal.PhotoByte));

                    command.ExecuteNonQuery();
                    command.Dispose();
                    command = null;
                }
                else
                {
                    // do nothing
                }

                classGlobal.pub_personal = null;
            }
        }

        private void INSERT_PASSPORT_INFO(int id)
        {

            if (classGlobal.pub_passport != null)
            {
                JToken token = classGlobal.pub_passport;

                string DocumentNo = (string)token.SelectToken("DocumentNo");
                string Familyname = (string)token.SelectToken("Familyname");
                string Givenname = (string)token.SelectToken("Givenname");
                string Birthday = (string)token.SelectToken("Birthday");
                string PersonalNo = (string)token.SelectToken("PersonalNo");
                string Nationality = (string)token.SelectToken("Nationality");
                string Sex = (string)token.SelectToken("Sex");
                string Dateofexpiry = (string)token.SelectToken("Dateofexpiry");
                string IssueState = (string)token.SelectToken("IssueState");
                string NativeName = (string)token.SelectToken("NativeName");
                string MRTDs = (string)token.SelectToken("MRTDs");
                string Photo = classGlobal.PlustekBase64String;

                if (NativeName == "")
                { NativeName = "-"; }

                int sYear;
                sYear = Int32.Parse(Birthday.Substring(0, 2));
                Birthday = Birthday.Substring(4, 2) + "/" +
                                Birthday.Substring(2, 2) + "/" +
                                   classGlobal.YearTwoToFour(sYear.ToString());

                sYear = Int32.Parse(Dateofexpiry.Substring(0, 2));
                Dateofexpiry = Dateofexpiry.Substring(4, 2) + "/" +
                                Dateofexpiry.Substring(2, 2) + "/" +
                                    classGlobal.YearTwoToFour(sYear.ToString());

                if ((Sex == "M") || (Sex == "m"))
                { Sex = "Male"; }
                else
                { Sex = "Female"; }

                if (classGlobal.databaseType == "acc")
                {
                    string query = "INSERT INTO tbl_passport (id, DocumentNo, Familyname, Givenname, Birthday, PersonalNo, Nationality, " +
                                                       "Sex, Dateofexpiry, IssueState, NativeName, MRTDs, Photo) " +
                                                        "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                    Birthday = "";
                    Dateofexpiry = "";

                    OleDbCommand command = new OleDbCommand(query, classGlobal.conn);
                    command.Parameters.Add("@id", OleDbType.Integer).Value = id;
                    command.Parameters.Add("@DocumentNo", OleDbType.VarChar).Value = DocumentNo;
                    command.Parameters.Add("@Familyname", OleDbType.VarChar).Value = Familyname;
                    command.Parameters.Add("@Givenname", OleDbType.VarChar).Value = Givenname;
                    command.Parameters.Add("@Birthday", OleDbType.VarChar).Value = Birthday;
                    command.Parameters.Add("@PersonalNo", OleDbType.VarChar).Value = PersonalNo;
                    command.Parameters.Add("@Nationality", OleDbType.VarChar).Value = Nationality;
                    command.Parameters.Add("@Sex", OleDbType.VarChar).Value = Sex;
                    command.Parameters.Add("@Dateofexpiry", OleDbType.VarChar).Value = Dateofexpiry;
                    command.Parameters.Add("@IssueState", OleDbType.VarChar).Value = IssueState;
                    command.Parameters.Add("@NativeName", OleDbType.VarChar).Value = NativeName;
                    command.Parameters.Add("@MRTDs", OleDbType.VarChar).Value = MRTDs;
                    command.Parameters.Add("@Photo", OleDbType.VarWChar).Value = Photo;
                    command.ExecuteNonQuery();
                    command.Dispose();
                    command = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    string query = "INSERT INTO tbl_passport (id, DocumentNo, Familyname, Givenname, Birthday, PersonalNo, Nationality, " +
                                                       "Sex, Dateofexpiry, IssueState, NativeName, MRTDs, Photo) " +
                                                        "VALUES (@id, @DocumentNo, @Familyname, @Givenname, @Birthday, @PersonalNo, @Nationality,@Sex, @Dateofexpiry, @IssueState, @NativeName, @MRTDs, @Photo)";

                    Birthday = "";
                    Dateofexpiry = "";

                    NpgsqlCommand command = new NpgsqlCommand(query, classGlobal.connP);
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@DocumentNo",DocumentNo);
                    command.Parameters.AddWithValue("@Familyname", Familyname);
                    command.Parameters.AddWithValue("@Givenname",  Givenname);
                    command.Parameters.AddWithValue("@Birthday",  Birthday);
                    command.Parameters.AddWithValue("@PersonalNo",  PersonalNo);
                    command.Parameters.AddWithValue("@Nationality",  Nationality);
                    command.Parameters.AddWithValue("@Sex", Sex);
                    command.Parameters.AddWithValue("@Dateofexpiry", Dateofexpiry);
                    command.Parameters.AddWithValue("@IssueState", IssueState);
                    command.Parameters.AddWithValue("@NativeName",  NativeName);
                    command.Parameters.AddWithValue("@MRTDs",  MRTDs);
                    command.Parameters.AddWithValue("@Photo", Photo);
                    command.ExecuteNonQuery();
                    command.Dispose();
                    command = null;
                }
                else
                {
                    // do nothing
                }

                classGlobal.pub_passport = null;
            }
        }

        private string[] Retrive_PersonalID_And_Name(int _id)
        {
            string[] result = new string[2] { "-", "-" };

            DataTable dt;
            OleDbDataAdapter adapter;
            NpgsqlDataAdapter adapterP;
            string query = "";
            try
            {
                query = String.Format("SELECT (th_title + th_firstname) AS firstname, th_lastname " +
                                        "FROM tbl_idcard WHERE id = {0}", _id);

                if (classGlobal.databaseType == "acc")
                {
                    adapter = new OleDbDataAdapter(query, classGlobal.conn);
                    dt = new DataTable("dt_Display");
                    adapter.Fill(dt);
                    adapter.Dispose();
                    adapter = null;

                    if (dt == null)
                        dt = new DataTable("dt_Display");

                    if (dt.Rows.Count == 0)  // ไม่เจอใน idcard 
                    {
                        query = String.Format("SELECT Familyname, Givenname " +
                                                "FROM tbl_passport WHERE id = {0}", _id);
                        adapter = new OleDbDataAdapter(query, classGlobal.conn);
                        dt = new DataTable("dt");
                        adapter.Fill(dt);
                        adapter.Dispose();
                        adapter = null;
                        if (dt.Rows.Count > 0)
                        {
                            //เป็นต่างชาติที่ได้ข้อมูลจากการอ่านพาสพอร์ต
                            result[0] = dt.Rows[0][0].ToString();
                            result[1] = dt.Rows[0][1].ToString();
                        }
                    }
                    else
                    {
                        //เป็นคนไทยที่ได้ข้อมูลจากการอ่านบัตรประชาชน
                        result[0] = dt.Rows[0][0].ToString();
                        result[1] = dt.Rows[0][1].ToString();
                    }
                }
                else if (classGlobal.databaseType == "psql")
                {
                    adapterP = new NpgsqlDataAdapter(query, classGlobal.connP);
                    dt = new DataTable("dt_Display");
                    adapterP.Fill(dt);
                    adapterP.Dispose();
                    adapterP = null;

                    if (dt == null)
                        dt = new DataTable("dt_Display");

                    if (dt.Rows.Count == 0)  // ไม่เจอใน idcard 
                    {
                        query = String.Format("SELECT Familyname, Givenname " +
                                                "FROM tbl_passport WHERE id = {0}", _id);
                        adapterP = new NpgsqlDataAdapter(query, classGlobal.connP);
                        dt = new DataTable("dt");
                        adapterP.Fill(dt);
                        adapterP.Dispose();
                        adapterP = null;
                        if (dt.Rows.Count > 0)
                        {
                            //เป็นต่างชาติที่ได้ข้อมูลจากการอ่านพาสพอร์ต
                            result[0] = dt.Rows[0][0].ToString();
                            result[1] = dt.Rows[0][1].ToString();
                        }
                    }
                    else
                    {
                        //เป็นคนไทยที่ได้ข้อมูลจากการอ่านบัตรประชาชน
                        result[0] = dt.Rows[0][0].ToString();
                        result[1] = dt.Rows[0][1].ToString();
                    }
                }
                else
                {
                    // do nothing
                }
            }
            catch
            {
                result[0] = "-";
                result[1] = "-";
            }

            //++ หาชื่อจากตาราง tbl_personal อีกครั้ง
            if (classGlobal.databaseType == "acc")
            {
                query = String.Format("SELECT id_number, fullname " +
                                            "FROM tbl_personal WHERE id = {0}", _id);
                adapter = new OleDbDataAdapter(query, classGlobal.conn);
                dt = new DataTable("dt");
                adapter.Fill(dt);
                adapter.Dispose();
                adapter = null;

                if (dt == null)
                    dt = new DataTable("dt");

                if (dt.Rows.Count > 0)
                {
                    //มีข้อมูลในตาราง tbl_personal
                    result[0] = dt.Rows[0][0].ToString();
                    result[1] = dt.Rows[0][1].ToString();
                }
            }
            else if (classGlobal.databaseType == "psql")
            {
                query = String.Format("SELECT id_number, fullname " +
                                            "FROM tbl_personal WHERE id = {0}", _id);
                adapterP = new NpgsqlDataAdapter(query, classGlobal.connP);
                dt = new DataTable("dt");
                adapterP.Fill(dt);
                adapterP.Dispose();
                adapterP = null;

                if (dt == null)
                    dt = new DataTable("dt");

                if (dt.Rows.Count > 0)
                {
                    //มีข้อมูลในตาราง tbl_personal
                    result[0] = dt.Rows[0][0].ToString();
                    result[1] = dt.Rows[0][1].ToString();
                }
            }
            else
            {
                // do nothing
            }
            
            //--

            return result;
        }

        private bool FUNCTION_INSERT(string idcompany, string card_number, string str_imagedocument, string str_imagewebcamera, string str_template, string str_datetime, int visitorType, string str_imagewebcamera1, string str_imagewebcamera2)
        {

            if (str_imagedocument == null)
                str_imagedocument = "";

            if (str_imagewebcamera == null)
                str_imagewebcamera = "";

            if (str_template == null)
                str_template = "";

            try
            {

                OleDbCommand command;
                OleDbDataReader reader;

                NpgsqlCommand commandP;
                NpgsqlDataReader readerP;

                string query;

                int id = 0;
                query = String.Format("SELECT id FROM tbl_visitor WHERE object_idcompany ='{0}' AND " +
                                                "card_number ='{1}' AND status_in <> '' AND (status_out ='' OR status_out IS NULL)",
                                                                idcompany, card_number);

                if (classGlobal.databaseType == "acc")
                {
                    command = new OleDbCommand(query, classGlobal.conn);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        id = Int32.Parse("0" + reader.GetValue(0).ToString());
                    }

                    reader.Close();
                    command.Dispose();
                    command = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    commandP = new NpgsqlCommand(query, classGlobal.connP);
                    readerP = commandP.ExecuteReader();
                    while (readerP.Read())
                    {
                        id = Int32.Parse("0" + readerP.GetValue(0).ToString());
                    }

                    readerP.Close();
                    commandP.Dispose();
                    commandP = null;
                }
                else
                {
                    string stringReturn = ClassData.POST_CHECK_EXIST_VISITOR_NUMBER(classGlobal.access_token, classGlobal.userId, card_number, "in", true);
                    if (stringReturn == "false")  // false หมายถึง ไม่มีข้อมูล แสดงว่า card_number นี้ยังไม่ถูกเอาไปใช้
                    {
                        // do nothing
                    }
                    else
                    {
                        return false;  // card_number มีคนใช้ไปแล้ว return false
                    }
                }


                if (id > 0) { return false; }

                id = GetMaxID();
                query = String.Format("INSERT INTO tbl_visitor (id, object_idcompany, card_number, str_imagedocument, str_imagewebcamera, str_template, status_in, status_out, typeid, str_imagewebcamera1, str_imagewebcamera2) " +
                                                            "VALUES ({0}, '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', {8}, '{9}', '{10}')", id, idcompany, card_number, str_imagedocument, str_imagewebcamera, str_template, str_datetime, "", visitorType, str_imagewebcamera1, str_imagewebcamera2);

                if (classGlobal.databaseType == "acc")
                {
                    command = new OleDbCommand(query, classGlobal.conn);
                    command.ExecuteNonQuery();
                    command.Dispose();
                    command = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    commandP = new NpgsqlCommand(query, classGlobal.connP);
                    commandP.ExecuteNonQuery();
                    commandP.Dispose();
                    commandP = null;
                }
                else
                {
                    #region ส่งค่าไปยัง SERVER
                    if (classGlobal.userId != "")
                    {
                        #region ส่งข้อมุลหน้าผู้ใช้งานมาในรูปแบบ Dictionary
                        Array.ForEach(Directory.GetFiles(classGlobal.storageImages), delegate(string path) { File.Delete(path); });
                        classGlobal.personID = classGlobal.personID ?? "";
                        classGlobal.personName = classGlobal.personName ?? "";
                        clsInfo.info_place = clsInfo.info_place ?? "";
                        clsInfo.info_license_plate = clsInfo.info_license_plate ?? "";
                        clsInfo.info_vehicle_type = clsInfo.info_vehicle_type ?? "";
                        clsInfo.info_etc = clsInfo.info_etc ?? "";
                        clsInfo.info_follower = clsInfo.info_follower ?? "";
                        clsInfo.info_visitor_company = clsInfo.info_visitor_company ?? "";
                        clsInfo.info_department = clsInfo.info_department ?? "";
                        clsInfo.info_visit_to = clsInfo.info_visit_to ?? "";
                        clsInfo.info_business_topic = clsInfo.info_business_topic ?? "";
                        classGlobal.strPlace = classGlobal.strPlace ?? "";
                        classGlobal.PlustekBase64String = classGlobal.PlustekBase64String ?? "";
                        classGlobal.WebcamBase64String = classGlobal.WebcamBase64String ?? "";
                        classGlobal.WebcamBase64String1 = classGlobal.WebcamBase64String1 ?? "";
                        classGlobal.WebcamBase64String2 = classGlobal.WebcamBase64String2 ?? "";

                        Dictionary<string, string> myDicInfo = new Dictionary<string, string>();
                        myDicInfo.Add("visitorNumber", card_number);
                        myDicInfo.Add("citizenId", classGlobal.personID);
                        myDicInfo.Add("name", classGlobal.personName);
                        myDicInfo.Add("visitorType", listBoxVisitorTypeName);  
                        myDicInfo.Add("recordStatus", "in");
                        myDicInfo.Add("licensePlate", clsInfo.info_license_plate);
                        myDicInfo.Add("vehicleType", clsInfo.info_vehicle_type);
                        myDicInfo.Add("terminalIn", classGlobal.strPlace);
                        myDicInfo.Add("terminalOut", "");
                        myDicInfo.Add("follower", clsInfo.info_follower);
                        myDicInfo.Add("visitorFrom", clsInfo.info_visitor_company);
                        myDicInfo.Add("department", clsInfo.info_department);
                        myDicInfo.Add("visitPerson", clsInfo.info_visit_to);
                        myDicInfo.Add("contactTopic", clsInfo.info_business_topic);
                        myDicInfo.Add("contactPlace", clsInfo.info_place);
                        myDicInfo.Add("etc", clsInfo.info_etc);
                        myDicInfo.Add("visitorId", "");
                        myDicInfo.Add("meetingId", classGlobal.appointMentSelectedId);

                        string image1 = classGlobal.PlustekBase64String;   // ภาพจาก plutek or document or webcam
                        string image2 = classGlobal.WebcamBase64String;    // ภาพจาก webcam
                        string image3 = classGlobal.WebcamBase64String1;   // ภาพจาก webcam
                        string image4 = classGlobal.WebcamBase64String2;   // ภาพจาก webcam 
                        if (image1 != "")
                        {
                            System.IO.File.WriteAllBytes(classGlobal.storageImages + @"\image1.png", Convert.FromBase64String(image1));
                            image1 = Directory.GetCurrentDirectory() + @"\" + classGlobal.storageImages + @"\image1.png";
                        }
                        if (image2 != "")
                        {
                            System.IO.File.WriteAllBytes(classGlobal.storageImages + @"\image2.png", Convert.FromBase64String(image2));
                            image2 = Directory.GetCurrentDirectory() + @"\" + classGlobal.storageImages + @"\image2.png";
                        }
                        if (image3 != "")
                        {
                            System.IO.File.WriteAllBytes(classGlobal.storageImages + @"\image3.png", Convert.FromBase64String(image3));
                            image3 = Directory.GetCurrentDirectory() + @"\" + classGlobal.storageImages + @"\image3.png";
                        }
                        if (image4 != "")
                        {
                            System.IO.File.WriteAllBytes(classGlobal.storageImages + @"\image4.png", Convert.FromBase64String(image4));
                            image4 = Directory.GetCurrentDirectory() + @"\" + classGlobal.storageImages + @"\image4.png";
                        }

                        string[] imageFiles = new string[4] { image1, image2, image3, image4 };
                        string stringResturn = ClassData.POST_VISITOR_IN(myDicInfo, classGlobal.access_token, classGlobal.userId, imageFiles);
                        if (stringResturn == "200")
                        {
                            Array.ForEach(Directory.GetFiles(classGlobal.storageImages), delegate(string path) { File.Delete(path); });
                        }
                        #endregion
                    }
                    #endregion
                }

                classGlobal.pub_id = id;

                if (classGlobal.pub_personal == null)
                {
                    strID = classGlobal.personID;
                    strFullname = classGlobal.personName;
                }
                else
                {
                    strID = classGlobal.pub_personal.CitizenID;
                    strFullname = classGlobal.pub_personal.ThPreName + classGlobal.pub_personal.ThFirstName + " " + classGlobal.pub_personal.ThLastName;
                }

                if (strID == "")
                    strID = txtID.Text;
                if (strFullname == "")
                    strFullname = txtFullname.Text; 
                
                //++ INSERT INTO tbl_personal
                query = String.Format("INSERT INTO tbl_personal (id, id_number, fullname) VALUES ({0}, '{1}', '{2}')", id, strID, strFullname);

                if (classGlobal.databaseType == "acc")
                {
                    command = new OleDbCommand(query, classGlobal.conn);
                    command.ExecuteNonQuery();
                    command.Dispose();
                    command = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    commandP = new NpgsqlCommand(query, classGlobal.connP);
                    commandP.ExecuteNonQuery();
                    commandP.Dispose();
                    commandP = null;
                }
                else
                {
                    // do nothing
                }
                //--

                try
                {
                    //++ UPDATE MACADDRESS FOR IN STATUS
                    //query = String.Format("UPDATE tbl_visitor SET mac_checkin='{0}' WHERE id={1}", classGlobal.MachineAddress, id);
                    query = String.Format("UPDATE tbl_visitor SET mac_checkin='{0}' WHERE id={1}", classGlobal.strPlace, id);
                    if (classGlobal.databaseType == "acc")
                    {
                        command = new OleDbCommand(query, classGlobal.conn);
                        command.ExecuteNonQuery();
                        command.Dispose();
                        command = null;
                    }
                    else if (classGlobal.databaseType == "psql")
                    {
                        commandP = new NpgsqlCommand(query, classGlobal.connP);
                        commandP.ExecuteNonQuery();
                        commandP.Dispose();
                        commandP = null;
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

                //++ INSERT INTO tbl_moreinfo นำเข้ารายการเพิ่มเติม เช่น สถานที่ติดต่อ/ทะเบียนรถ/ชนิด/อื่นๆ (งานของพี่กุ่ย 19-08-2562)
                string _vehicleText = "";
                for (int i = 0; i < classGlobal.arrVehicleInfo.Count; i++)
                {
                    _vehicleText += classGlobal.arrVehicleInfo[i].ToString();
                }
                if (_vehicleText != "")
                {
                    query = String.Format("INSERT INTO tbl_moreinfo (id, place, register, v_nametype, etc) " +
                                                                   "VALUES ({0}, '{1}', '{2}', '{3}', '{4}')",
                                                                   id,
                                                                   classGlobal.arrVehicleInfo[0], classGlobal.arrVehicleInfo[1],
                                                                   classGlobal.arrVehicleInfo[2], classGlobal.arrVehicleInfo[3]);

                    if (classGlobal.databaseType == "acc")
                    {
                        command = new OleDbCommand(query, classGlobal.conn);
                        command.ExecuteNonQuery();
                        command.Dispose();
                        command = null;
                    }
                    else if (classGlobal.databaseType == "psql")
                    {
                        commandP = new NpgsqlCommand(query, classGlobal.connP);
                        commandP.ExecuteNonQuery();
                        commandP.Dispose();
                        commandP = null;
                    }
                    else
                    {
                        // do nothing
                    }
                }
                //-- INSERT INTO tbl_moreinfo นำเข้ารายการเพิ่มเติม เช่น สถานที่ติดต่อ/ทะเบียนรถ/ชนิด/อื่นๆ (งานของพี่กุ่ย 19-08-2562)

                return true;
            }
            catch
            {
                return false;
            }
        }

        private string FUNCTION_UPDATE(string idcompany, string card_number, string str_datetime)
        {

            try
            {

                OleDbCommand command;
                OleDbDataReader reader;

                NpgsqlCommand commandP;
                NpgsqlDataReader readerP;

                string query;
                int id = 0;
                query = String.Format("SELECT id FROM tbl_visitor WHERE object_idcompany ='{0}' AND card_number ='{1}' AND status_in <> '' AND (status_out ='' OR status_out IS NULL)",
                                idcompany, card_number);

                if (classGlobal.databaseType == "acc")
                {
                    command = new OleDbCommand(query, classGlobal.conn);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        id = Int32.Parse("0" + reader.GetValue(0).ToString());
                    }
                    reader.Close();
                    command.Dispose();
                    command = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    commandP = new NpgsqlCommand(query, classGlobal.connP);
                    readerP = commandP.ExecuteReader();
                    while (readerP.Read())
                    {
                        id = Int32.Parse("0" + readerP.GetValue(0).ToString());
                    }
                    readerP.Close();
                    commandP.Dispose();
                    commandP = null;
                }
                else
                {
                    //*** เรียก api server ถ้ามี json object ตอบกลับมา แสดงว่าเลขตอนออกถูกต้อง  ถ้าเลข visitor ไม่ถูก จะreturn "false"                  
                    string stringResturn = ClassData.POST_CHECK_EXIST_VISITOR_NUMBER(classGlobal.access_token, classGlobal.userId, card_number, "out", true);
                    return stringResturn;
                }

                if (id == 0) { return "false"; }

                classGlobal.pub_id = id;

                query = String.Format("UPDATE tbl_visitor SET status_out ='{0}' WHERE id={1}", str_datetime, id);

                if (classGlobal.databaseType == "acc")
                {
                    command = new OleDbCommand(query, classGlobal.conn);
                    command.ExecuteNonQuery();
                    command.Dispose();
                    command = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    commandP = new NpgsqlCommand(query, classGlobal.connP);
                    commandP.ExecuteNonQuery();
                    commandP.Dispose();
                    commandP = null;
                }
                else
                {
                    // do nothing
                }             

                try
                {
                    //++ UPDATE MACADDRESS FOR OUT STATUS
                    //query = String.Format("UPDATE tbl_visitor SET mac_checkout='{0}' WHERE id={1}", classGlobal.MachineAddress, id);
                    query = String.Format("UPDATE tbl_visitor SET mac_checkout='{0}' WHERE id={1}", classGlobal.strPlace, id);
                    if (classGlobal.databaseType == "acc")
                    {
                        command = new OleDbCommand(query, classGlobal.conn);
                        command.ExecuteNonQuery();
                        command.Dispose();
                        command = null;
                    }
                    else if (classGlobal.databaseType == "psql")
                    {
                        commandP = new NpgsqlCommand(query, classGlobal.connP);
                        commandP.ExecuteNonQuery();
                        commandP.Dispose();
                        commandP = null;
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

                try
                {
                    //++ UPDATE สถานะการ Upload ให้กลับเป็น 0 
                    query = String.Format("UPDATE tbl_visitor SET upload ='{0}' WHERE id={1}", "0", id);
                    if (classGlobal.databaseType == "acc")
                    {
                        command = new OleDbCommand(query, classGlobal.conn);
                        command.ExecuteNonQuery();
                        command.Dispose();
                        command = null;
                    }
                    else if (classGlobal.databaseType == "psql")
                    {
                        commandP = new NpgsqlCommand(query, classGlobal.connP);
                        commandP.ExecuteNonQuery();
                        commandP.Dispose();
                        commandP = null;
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


                string strReturn = "";
                string para = "";
                query = "SELECT  t1.card_number, t1.str_imagedocument, t1.str_imagewebcamera, t1.status_in, t1.status_out, t2.typename, t1.str_imagewebcamera1, t1.str_imagewebcamera2 " +
                                            "FROM tbl_visitor t1 INNER JOIN tbl_type t2 ON t1.typeid = t2.typeid " +
                                            "WHERE id=" + id;

                DataTable dtResult = new DataTable("dtResult");

                if (classGlobal.databaseType == "acc")
                {
                    command = new OleDbCommand(query, classGlobal.conn);
                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, classGlobal.conn);
                    adapter.Fill(dtResult);
                    adapter.Dispose();
                    adapter = null;
                    command.Dispose();
                    command = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    commandP = new NpgsqlCommand(query, classGlobal.connP);
                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, classGlobal.connP);                    
                    adapter.Fill(dtResult);
                    adapter.Dispose();
                    adapter = null;
                    commandP.Dispose();
                    commandP = null;
                }
                else
                {
                    // do nothing
                }

                foreach (DataRow dr in dtResult.Rows)
                {
                    para = dr.ItemArray[0].ToString() + "@" +
                            dr.ItemArray[1].ToString() + "@" +
                              dr.ItemArray[2].ToString() + "@" +
                                dr.ItemArray[3].ToString() + "@" +
                                  dr.ItemArray[4].ToString() + "@" +
                                    dr.ItemArray[5].ToString() + "@" +
                                      dr.ItemArray[6].ToString() + "@" +
                                        dr.ItemArray[7].ToString();
                }
                dtResult.Dispose();
                dtResult = null;

                strReturn = para;
                return strReturn;
            }
            catch
            {
                return "false";
            }
        }

        private int GetMaxID()
        {
            int id = 0;
            try
            {
                if (classGlobal.databaseType == "acc")
                {
                    OleDbCommand command = new OleDbCommand("Select MAX(id) from tbl_visitor", classGlobal.conn);
                    OleDbDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        id = Int32.Parse("0" + reader.GetValue(0).ToString()) + 1;
                    }
                    reader.Close();
                    command.Dispose();
                    command = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    NpgsqlCommand command = new NpgsqlCommand("Select MAX(id) from tbl_visitor", classGlobal.connP);
                    NpgsqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        id = Int32.Parse("0" + reader.GetValue(0).ToString()) + 1;
                    }
                    reader.Close();
                    command.Dispose();
                    command = null;
                }
                else
                {
                    // do nothing
                }

                return id;
            }
            catch
            {
                return id;
            }
        }

        private void PRINT_SLIP_THERMAL(string text, string text1, string text2, string text3, Bitmap qr)
        {
            clsXML clsxmlP = new clsXML();
            classGlobal.DisplayHashTag = bool.Parse(clsxmlP.GetReadXML("root", "enablestatute", classGlobal.config).ToString());
            clsxmlP = null;

            #region fix citizenId = #############
            string[] _spltRn = text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            for (int r = 0; r < _spltRn.Length; r++)
            {
                if (_spltRn[r].Contains("เลขประจำตัว"))
                {
                    _spltRn[r] = "เลขประจำตัว : #############";
                    break;
                }
            }
            text = String.Join(Environment.NewLine, _spltRn);
            #endregion

            if (classGlobal.DisplayHashTag == true)
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
            }

            classGlobal.Reprint_text = text;
            classGlobal.Reprint_text1 = text1;
            classGlobal.Reprint_text2 = text2;
            classGlobal.Reprint_text3 = text3;
            classGlobal.Reprint_Bitmap = qr;


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

            int lineOfEtc = 0;
            string[] _text = text.Split(new[] { Environment.NewLine },StringSplitOptions.None);
            for (lineOfEtc = 0; lineOfEtc < _text.Length; lineOfEtc++)
            {
                if (_text[lineOfEtc].Replace(" ", "").Equals(""))
                    break;
            }

            //int logoHeight = 0;
            Bitmap bmp = null;
            //StringFormat format;
            //RectangleF rect;
            //int limit_lineWidth = 0;
            //int allHeight = 0;
            PrintDocument p = new PrintDocument();
            p.PrintPage += delegate(object sender1, PrintPageEventArgs e1)
            {

                clsXML c_region = new clsXML();
                slip_factory_region = c_region.GetReadXML("root", "slipregion", classGlobal.config);
                slip_factory_region = slip_factory_region.ToLower();
                c_region = null;

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
                    text = text.Replace("มี (ระบุ)...................................................", "If yes, please specify....................................");
                    text = text.Replace("ไม่มี", "No");
                    text = text.Replace("มี", "Yes");

                    text1 = text1.Replace("ทรัพย์สินที่นำเข้า", "Material in");
                    text1 = text1.Replace("ทรัพย์สินที่ฝาก รปภ", "Material at security");
                    text1 = text1.Replace("มี (ระบุ)...................................................", "If yes, please specify....................................");
                    text1 = text1.Replace("ไม่มี", "No");
                    text1 = text1.Replace("มี", "Yes");

                    text2 = text2.Replace("ท่านได้อ่านและเข้าใจกฏระเบียบว่าด้วยความปลอดภัย", "I agree to abide by the rules");

                    text3 = text3.Replace("ลงชื่อผู้มาติดต่อ.......................................", "Visitor’s sign...............................................");
                    text3 = text3.Replace("ลงชื่อผู้รับการติดต่อ...................................", "Staff’s sign.................................................");
                    text3 = text3.Replace("ลงชื่อเจ้าหน้าที่ รปภ. ................................", "Security’s sign............................................");
                }


                string[] _linePrint = new string[0] { };

                Graphics graphics = e1.Graphics;
                float fontHeight = font.GetHeight();
                //int startX = 0;
                //int startY = 0;
                int Offset = 0;

                //+++ LOGO
                bmp = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + @"logo.png");
                bmp = classGlobal.ResizeImageRatio(bmp, classGlobal.pubWidth, classGlobal.pubHeight);
                graphics.DrawImage(bmp, new RectangleF((p.DefaultPageSettings.PrintableArea.Width / 2) - (bmp.Width / 2), 0, bmp.Width, bmp.Height));
                // -- LOGO
                Offset = Offset + bmp.Height;


                float x = 0;
                float y = Offset;
                float width = 300.0F; // max width I found through trial and error
                float height = 0F;

                SolidBrush drawBrush = new SolidBrush(Color.Black);

                // Set format of string.
                StringFormat drawFormatCenter = new StringFormat();
                drawFormatCenter.Alignment = StringAlignment.Center;
                StringFormat drawFormatLeft = new StringFormat();
                drawFormatLeft.Alignment = StringAlignment.Near;
                StringFormat drawFormatRight = new StringFormat();
                drawFormatRight.Alignment = StringAlignment.Far;


                int nOver = 0;
                _linePrint = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                for (int l = 0; l < _linePrint.Length; l++)
                {
                    SizeF stringSize = new SizeF();
                    stringSize = e1.Graphics.MeasureString(_linePrint[l], font);
                    int iMaxWidth = 300; // Maximum width, in pixels
                    StringFormat sfFmt = new StringFormat(StringFormatFlags.LineLimit);
                    Graphics g = Graphics.FromImage(new Bitmap(1, 1));
                    int iHeight = (int)g.MeasureString(_linePrint[l], font, iMaxWidth, sfFmt).Height;
                    int iOneLineHeight = (int)g.MeasureString("Z", font, iMaxWidth, sfFmt).Height;

                    if (iHeight > iOneLineHeight)
                    {
                        nOver += 1;
                        iHeight = iHeight + nOver;
                    }

                    int iNumLines = (int)(iHeight / iOneLineHeight);
                    e1.Graphics.DrawString(_linePrint[l], font, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
                    y += (iOneLineHeight * iNumLines);

                }

                nOver = 0;
                y += (int)graphics.MeasureString("Z", font, 300, new StringFormat(StringFormatFlags.LineLimit)).Height;
                _linePrint = text1.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                for (int l = 0; l < _linePrint.Length; l++)
                {
                    SizeF stringSize = new SizeF();
                    stringSize = e1.Graphics.MeasureString(_linePrint[l], font);
                    int iMaxWidth = 300; // Maximum width, in pixels
                    StringFormat sfFmt = new StringFormat(StringFormatFlags.LineLimit);
                    Graphics g = Graphics.FromImage(new Bitmap(1, 1));
                    int iHeight = (int)g.MeasureString(_linePrint[l], font, iMaxWidth, sfFmt).Height;
                    int iOneLineHeight = (int)g.MeasureString("Z", font, iMaxWidth, sfFmt).Height;

                    if (iHeight > iOneLineHeight)
                    {
                        nOver += 1;
                        iHeight = iHeight + nOver;
                    }

                    int iNumLines = (int)(iHeight / iOneLineHeight);
                    e1.Graphics.DrawString(_linePrint[l], font, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
                    y += (iOneLineHeight * iNumLines);

                }

                #region วาดกรอบ "ทรัพย์สินที่นำเข้า..."
                int frame = 0;
                int iOneLineHeightFrame = (int)e1.Graphics.MeasureString("Z", font, 300, new StringFormat(StringFormatFlags.LineLimit)).Height;
                frame = (iOneLineHeightFrame * lineOfEtc) + Offset - 5;   //   -5 = เลื่อนกรอบบนลงมานิดนึง
                int recWidth = (int)p.DefaultPageSettings.PrintableArea.Width;  //275;
                e1.Graphics.DrawRectangle(new Pen(Color.Black, 1), new Rectangle(0, frame, recWidth, 120));  // 120 = ประเมินขนาดความสูงของหัวข้อ ทรัพย์สิน....
                #endregion

                nOver = 0;
                y += (int)graphics.MeasureString("Z", font, 300, new StringFormat(StringFormatFlags.LineLimit)).Height;
                _linePrint = text2.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                for (int l = 0; l < _linePrint.Length; l++)
                {
                    SizeF stringSize = new SizeF();
                    stringSize = e1.Graphics.MeasureString(_linePrint[l], font);
                    int iMaxWidth = 300; // Maximum width, in pixels
                    StringFormat sfFmt = new StringFormat(StringFormatFlags.LineLimit);
                    Graphics g = Graphics.FromImage(new Bitmap(1, 1));
                    int iHeight = (int)g.MeasureString(_linePrint[l], font, iMaxWidth, sfFmt).Height;
                    int iOneLineHeight = (int)g.MeasureString("Z", font, iMaxWidth, sfFmt).Height;

                    if (iHeight > iOneLineHeight)
                    {
                        nOver += 1;
                        iHeight = iHeight + nOver;
                    }

                    int iNumLines = (int)(iHeight / iOneLineHeight);
                    e1.Graphics.DrawString(_linePrint[l], fontBorder, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
                    y += (iOneLineHeight * iNumLines);

                }

                nOver = 0;
                y += (int)graphics.MeasureString("Z", font, 300, new StringFormat(StringFormatFlags.LineLimit)).Height;
                _linePrint = text3.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                for (int l = 0; l < _linePrint.Length; l++)
                {
                    SizeF stringSize = new SizeF();
                    stringSize = e1.Graphics.MeasureString(_linePrint[l], font);
                    int iMaxWidth = 300; // Maximum width, in pixels
                    StringFormat sfFmt = new StringFormat(StringFormatFlags.LineLimit);
                    Graphics g = Graphics.FromImage(new Bitmap(1, 1));
                    int iHeight = (int)g.MeasureString(_linePrint[l], font, iMaxWidth, sfFmt).Height;
                    int iOneLineHeight = (int)g.MeasureString("Z", font, iMaxWidth, sfFmt).Height;

                    if (iHeight > iOneLineHeight)
                    {
                        nOver += 1;
                        iHeight = iHeight + nOver;
                    }

                    int iNumLines = (int)(iHeight / iOneLineHeight);
                    e1.Graphics.DrawString(_linePrint[l], font, drawBrush, new RectangleF(x, y, width, height), drawFormatLeft);
                    y += (iOneLineHeight * iNumLines);

                }


                //++ QR IMAGE
                nOver = 0;
                y += (int)graphics.MeasureString("Z", font, 300, new StringFormat(StringFormatFlags.LineLimit)).Height;
                graphics.DrawImage(qr, new RectangleF((p.DefaultPageSettings.PrintableArea.Width / 2) - (qr.Width / 2), y, qr.Width, qr.Height));
                y += qr.Height + 20;

                #region version factory ข้อความต่อท้าย QR
                string messageAnother = Environment.NewLine + Environment.NewLine;
                string[] lines = System.IO.File.ReadAllLines(@"slip_description.txt");
                string eachRowText = "";
                for (int i = 0; i < lines.Length; i++)
                {
                    eachRowText = lines[i].Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
                    messageAnother = messageAnother + eachRowText + Environment.NewLine;
                }


                nOver = 0;
                _linePrint = messageAnother.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                for (int l = 0; l < _linePrint.Length; l++)
                {
                    SizeF stringSize = new SizeF();
                    stringSize = e1.Graphics.MeasureString(_linePrint[l], font);
                    int iMaxWidth = 300; // Maximum width, in pixels
                    StringFormat sfFmt = new StringFormat(StringFormatFlags.LineLimit);
                    Graphics g = Graphics.FromImage(new Bitmap(1, 1));
                    int iHeight = (int)g.MeasureString(_linePrint[l], font, iMaxWidth, sfFmt).Height;
                    int iOneLineHeight = (int)g.MeasureString("Z", font, iMaxWidth, sfFmt).Height;

                    if (iHeight > iOneLineHeight)
                    {
                        nOver += 1;
                        iHeight = iHeight + nOver;
                    }

                    int iNumLines = (int)(iHeight / iOneLineHeight);
                    e1.Graphics.DrawString(_linePrint[l], font, drawBrush, new RectangleF(x, y, width, height), drawFormatCenter);
                    y += (iOneLineHeight * iNumLines);

                }
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
        private string textPerLine(string yourtext, int perLine)
        {
            char[] chars = yourtext.ToCharArray();
            string b = "";
            int i = 1;
            foreach (char s in chars)
            {
                if (b.Length < i * perLine)
                    b += s + "";
                else
                {
                    b += Environment.NewLine;
                    i++;
                }
            }
            return b;
        }
       
        private void txtVisitorNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (classGlobal.statusIn_OUT == "OUT")
                {
                    rbtnOK_Click(this, new EventArgs());  // Auto Enter
                }
                //btnOK_Click(this, new EventArgs());
            }
        }

        private void txtVisitorNumber_KeyUp(object sender, KeyEventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(delegate { lbKeyboardLayout.Text = InputLanguage.CurrentInputLanguage.Culture.Name.Replace("en-US", "EN").Replace("th-TH", "TH"); }));

            if (e.Control && e.KeyCode == Keys.V)
            {
                e.SuppressKeyPress = true;
                rbtnOK_Click(this, new EventArgs());
            }
        }

        private void lbKeyboardLayout_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string kbLayout = InputLanguage.CurrentInputLanguage.Culture.Name;
            string new_kbLayout = "en-US";

            if (kbLayout == "en-US")
                new_kbLayout = "th-TH";
            else
                new_kbLayout = "en-US";

            var culture = System.Globalization.CultureInfo.GetCultureInfo(new_kbLayout);
            var language = InputLanguage.FromCulture(culture);
            InputLanguage.CurrentInputLanguage = language;
            kbLayout = InputLanguage.CurrentInputLanguage.Culture.Name;
            lbKeyboardLayout.Text = kbLayout.Replace("en-US", "EN").Replace("th-TH", "TH");

            this.ActiveControl = txtVisitorNumber;

            Console.Write(new_kbLayout);
            File.WriteAllText("lang", kbLayout);
        }

        private void picDocument_Click(object sender, EventArgs e)
        {
            if (classGlobal.statusIn_OUT == "OUT")
                return;

            if (classGlobal.boolX50 == true)
            {
                //++ โค้ดแก้ปัญหาเฉพาะหน้า โดยการ shell exe  Engine.exe ขึ้นมาเพื่ออ่านบัตร แล้วเขียนลง textfile แล้วอ่านขึ้นมาเก็บในตัวแปล classGlobal.pub_personal
                if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"PASSPORT_Engine\passport.ini") == true)
                { System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"PASSPORT_Engine\passport.ini"); }

                Process p = new Process();
                p.Exited += new EventHandler(passport_EventHandler);
                p.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + @"PASSPORT_Engine\X50_Engine.exe";
                p.StartInfo.Arguments = "";
                p.EnableRaisingEvents = true;
                p.Start();
                //-- 
                return;
            }

            if (classGlobal.WebCamConnect == false)
                return;

            if (classGlobal.statusIn_OUT == "IN")
            {
                classGlobal.cam = "cam0";
                frmTakePicture f = new frmTakePicture();
                f.ShowDialog();

                Image imgWebcam = clsImage.Base64ToImage(classGlobal.PlustekBase64String);
                if (imgWebcam == null)
                {
                    imgWebcam = new Bitmap(classGlobal.click4capture);
                }
                picDocument.Image = imgWebcam;

                if (classGlobal.DisplayHashTag == true)
                {
                    if (classGlobal.PlustekBase64String != null || classGlobal.PlustekBase64String != "")
                    {
                        imgWebcam = clsImage.Base64ToImage(classGlobal.PLACEWATERMARK_FROM_BASE64(classGlobal.PlustekBase64String));
                    }
                    else
                    {
                        byte[] bytes = System.IO.File.ReadAllBytes(classGlobal.click4capture);
                        bytes = classGlobal.PLACEWATERMARK_FROM_BYTE(bytes);
                        imgWebcam = (Bitmap)((new ImageConverter()).ConvertFrom(bytes));
                    }
                    picDocument.Image = imgWebcam;
                }
                else
                {
                    if (classGlobal.PlustekBase64String != null || classGlobal.PlustekBase64String != "")
                    {
                        imgWebcam = clsImage.Base64ToImage(classGlobal.PLACEWATERMARK_FROM_BASE64(classGlobal.PlustekBase64String));
                    }
                    else
                    {
                        byte[] bytes = System.IO.File.ReadAllBytes(classGlobal.click4capture);
                        bytes = classGlobal.PLACEWATERMARK_FROM_BYTE(bytes);
                        imgWebcam = (Bitmap)((new ImageConverter()).ConvertFrom(bytes));
                    }
                    picDocument.Image = imgWebcam;
                }
            }
            else
            {
                //
            }

        }

        private void passport_EventHandler(object sender, EventArgs e)
        {
            // Read the file and display it line by line.               
            classGlobal.pub_personal = null;
            if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"PASSPORT_Engine\passport.ini") == true)
            {
                string[] t = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"PASSPORT_Engine\passport.ini");
                string stringInBase64 = t[2];

                classGlobal.personID = t[0];
                classGlobal.personName = t[1].Replace("  ", " ");
                classGlobal.PlustekBase64String = stringInBase64;
                byte[] PhotoByte = new byte[0];
                PhotoByte = System.Convert.FromBase64String(stringInBase64);
                System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"PASSPORT_Engine\passport.ini");
                using (MemoryStream memstr = new MemoryStream(PhotoByte))
                {
                    Image img = Image.FromStream(memstr);
                    picDocument.Image = img;
                    picDocument.SizeMode = PictureBoxSizeMode.Zoom;
                }

                txtID.BeginInvoke(new MethodInvoker(delegate { txtID.Text = classGlobal.personID; }));
                txtFullname.BeginInvoke(new MethodInvoker(delegate { txtFullname.Text = classGlobal.personName; }));
                txtID.BeginInvoke(new MethodInvoker(delegate { txtID.Text = "############"; }));
                if (classGlobal.DisplayHashTag == true)
                {
                    //txtID.BeginInvoke(new MethodInvoker(delegate { txtID.Text = classGlobal.REPLACE_IDCARD(classGlobal.personID); }));
                    txtFullname.BeginInvoke(new MethodInvoker(delegate { txtFullname.Text = (classGlobal.REPLACE_NAME(classGlobal.personName)); }));
                    using (MemoryStream memstr = new MemoryStream(classGlobal.PLACEWATERMARK_FROM_BYTE(PhotoByte)))
                    {
                        Image img = Image.FromStream(memstr);
                        picDocument.Image = img;
                        picDocument.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                }
                else
                {
                    using (MemoryStream memstr = new MemoryStream(classGlobal.PLACEWATERMARK_FROM_BYTE(PhotoByte)))
                    {
                        Image img = Image.FromStream(memstr);
                        picDocument.Image = img;
                        picDocument.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                }
            }
        }

        private void AUTO_CHECK_BLACKLIST()
        {
            if (CHECK_BLACKLIST(classGlobal.personID) == true)  //if (CHECK_BLACKLIST(classGlobal.pub_personal.CitizenID) == true) 
            {

                frmMessageBox f = new frmMessageBox();
                f.strMessage = "บุคคลต้องห้าม (Blacklist)!!";
                f.strStatus = "Warning";
                f.ShowDialog();

                classGlobal.personID = "";
                classGlobal.personName = "";
                return;
            }
        }


        private void AUTO_CHECK_WHITELIST()
        {
            if (classGlobal.visitorStatus == "blacklist")
                return;

            CHECK_WHITELIST(classGlobal.personID);
            if (classGlobal.visitorStatus == "whitelist")
            {
                frmMessageBox f = new frmMessageBox();
                f.strMessage = "Whitelist";
                f.strStatus = "Information";
                f.ShowDialog();

                string str_datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");   //2018-09-18 10:10:10

                string strIN = str_datetime;
                int beYearFormat = 0;
                string[] arr1 = strIN.Split(' ');
                string[] arr2 = arr1[0].Split('-');
                beYearFormat = Int32.Parse(arr2[0]) + 543;
                strIN = arr2[2] + "/" + arr2[1] + "/" + beYearFormat.ToString() + " " + arr1[1];

                int keyType = 0;
                string[] whitelistType = new string[5] { "ลูกบ้าน", "เจ้าหน้าที่", "พนักงาน", "VIP", "WHITELIST" };
                for (int w = 0; w < whitelistType.Length; w++)
                {
                    keyType = listBox1.FindString(whitelistType[w]);
                    if (keyType > -1)
                    {
                        listBox1.SelectedIndex = keyType;                        
                        string kPair = ((KeyValuePair<string, string>)listBox1.SelectedItem).Value;
                        try
                        { keyType = Int32.Parse(kPair);  }
                        catch
                        {  keyType = 0; } 
                        
                        break;
                    }
                        
                }
                if (keyType < 0)
                    keyType = 0;

                

                string base64IDCard = "";

                if (classGlobal.PlustekBase64String != null || classGlobal.PlustekBase64String != "")
                    base64IDCard = classGlobal.PlustekBase64String;

                bool status = FUNCTION_INSERT("", classGlobal.personID, base64IDCard, "", "", str_datetime, keyType, "", "");
                if (status == true)
                {
                    INSERT_IDCARD_INFO(classGlobal.pub_id);
                    INSERT_PASSPORT_INFO(classGlobal.pub_id);

                    //++ auto visit out
                    str_datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");   //2018-09-18 10:10:10
                    string statusStr = FUNCTION_UPDATE("", classGlobal.personID, str_datetime);
                    //-- auto visit out
                }
                
                System.Threading.Thread.Sleep(3000);
                CLEARCONTROLS();

                
            }
        }

        private void txtVisitorNumber_Enter(object sender, EventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(delegate { lbKeyboardLayout.Text = InputLanguage.CurrentInputLanguage.Culture.Name.Replace("en-US", "EN").Replace("th-TH", "TH"); }));
        }

        private void txtVisitorNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((sender as TextBox).SelectionStart == 0)
                e.Handled = (e.KeyChar == (char)Keys.Space);
            else
                e.Handled = false; 
        }


        private void btnIPCAM_Click(object sender, EventArgs e)
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
                    if (!data.Substring(0, 1).Equals(";"))
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
            //**//
        }

        private bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;
            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            //file is not locked
            return false;
        }
        private void FswCreated(object sender, FileSystemEventArgs e)
        {
            System.Threading.Thread.Sleep(500);

            if (e.Name.ToString() == @"cam1.jpg")
            {
                    string sFile = e.FullPath;
                    Console.WriteLine("processing file : " + sFile);
                    // Wait if file is still open
                    FileInfo fileInfo = new FileInfo(sFile);
                    while (IsFileLocked(fileInfo))
                    {
                        Thread.Sleep(500);
                    }

                    byte[] bytes = System.IO.File.ReadAllBytes(sFile);                
                    string stringInBase64 = Convert.ToBase64String(bytes, 0, bytes.Length);
                    classGlobal.WebcamBase64String = stringInBase64;

                    Image imgWebcam = clsImage.Base64ToImage(classGlobal.WebcamBase64String);
                    if (imgWebcam == null)
                        imgWebcam = new Bitmap(classGlobal.click4capture);
                    picWebcam.Image = imgWebcam;

                    imgWebcam = clsImage.Base64ToImage(classGlobal.PLACEWATERMARK_FROM_BASE64(classGlobal.WebcamBase64String));
                    if (imgWebcam == null)
                        imgWebcam = new Bitmap(classGlobal.click4capture);
                    picWebcam.Image = imgWebcam;

                    System.IO.File.Delete(sFile);
                    if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"cam1.jpg"))
                        System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"cam1.jpg");

                }

            if (e.Name.ToString() == @"cam2.jpg")
            {
                string sFile = e.FullPath;
                Console.WriteLine("processing file : " + sFile);
                // Wait if file is still open
                FileInfo fileInfo = new FileInfo(sFile);
                while (IsFileLocked(fileInfo))
                {
                    Thread.Sleep(500);
                }

                byte[] bytes = System.IO.File.ReadAllBytes(sFile);
                string stringInBase64 = Convert.ToBase64String(bytes, 0, bytes.Length);
                classGlobal.WebcamBase64String1 = stringInBase64;

                Image imgWebcam = clsImage.Base64ToImage(classGlobal.WebcamBase64String1);
                if (imgWebcam == null)
                    imgWebcam = new Bitmap(classGlobal.click4capture);
                picWebcam1.Image = imgWebcam;

                imgWebcam = clsImage.Base64ToImage(classGlobal.PLACEWATERMARK_FROM_BASE64(classGlobal.WebcamBase64String1));
                if (imgWebcam == null)
                    imgWebcam = new Bitmap(classGlobal.click4capture);
                picWebcam1.Image = imgWebcam;

                System.IO.File.Delete(sFile);
                if (System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"cam2.jpg"))
                    System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"cam2.jpg");

            }

            Thread.Sleep(500);
        }

        private void rbtnReprint_Click(object sender, EventArgs e)
        {
            PRINT_SLIP_THERMAL(classGlobal.Reprint_text, classGlobal.Reprint_text1, classGlobal.Reprint_text2, classGlobal.Reprint_text3, classGlobal.Reprint_Bitmap);
        }

        private void rbtnOK_Click(object sender, EventArgs e)
        {
            txtVisitorNumber.Text = txtVisitorNumber.Text.Replace(" ", "");
            PRESS_OK_EVENT();
            if (classGlobal.statusCheckIN == true && classGlobal.userId != "" && classGlobal.appointMentSelectedId != "")
            {
                ClassData.UPDATE_APPOINTMENT_STATUS(classGlobal.appointMentSelectedId, "มา");
            }

            classGlobal.CHECK_DATABASE_SIZE();
        }


        string listBoxVisitorTypeName = "";
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBoxVisitorTypeName = ((KeyValuePair<string, string>)listBox1.SelectedItem).Key;
        }





        #region จับเวลาการพิมพ์ข้อมูลบน textbox
        static int VALIDATION_DELAY = 1500;
        System.Threading.Timer timerID = null;
        System.Threading.Timer timerName = null;

        private void TimerElapsedID(Object obj)
        {
            CheckSyntaxAndReportID();
            DisposeTimerID();
        }
        private void DisposeTimerID()
        {
            if (timerID != null)
            {
                timerID.Dispose();
                timerID = null;
            }
        }
        private void CheckSyntaxAndReportID()
        {
            this.Invoke(new Action(() =>
            {
                string s = txtID.Text.ToUpper();
                s = s.Replace(" ", "");
                if (s != "")
                {
                    MessageBox.Show(s);
                }
            }
            ));
        }


        private void TimerElapsedName(Object obj)
        {
            CheckSyntaxAndReportName();
            DisposeTimerName();
        }
        private void DisposeTimerName()
        {
            if (timerName != null)
            {
                timerName.Dispose();
                timerName = null;
            }
        }
        private void CheckSyntaxAndReportName()
        {
            this.Invoke(new Action(() =>
            {
                string s = txtFullname.Text.ToUpper();
                s = s.Replace(" ", "");
                if (s != "")
                {
                    MessageBox.Show(s);
                }
            }
            ));
        }
        #endregion

        private void TxtID_TextChanged(object sender, EventArgs e)
        {
            if (classGlobal.userId != "")
            {
                //TextBox origin = sender as TextBox;
                //if (!origin.ContainsFocus)
                //    return;

                //DisposeTimerID();
                //timerID = new System.Threading.Timer(TimerElapsedID, null, VALIDATION_DELAY, VALIDATION_DELAY);
                timerDelay.Stop();
                timerDelay.Start();
            }
        }
        private void TxtFullname_TextChanged(object sender, EventArgs e)
        {
            if (classGlobal.userId != "")
            {
                //TextBox origin = sender as TextBox;
                //if (!origin.ContainsFocus)
                //    return;

                //DisposeTimerName();
                //timerName = new System.Threading.Timer(TimerElapsedName, null, VALIDATION_DELAY, VALIDATION_DELAY);
                timerDelay.Stop();
                timerDelay.Start();
            }
        }

        private void TimerDelay_Tick(object sender, EventArgs e)
        {
            timerDelay.Stop();
            this.Invoke(new Action(() =>
            {
                string cID = txtID.Text;
                string cName = txtFullname.Text;
                ClassData.CHECK_APPOINTMENT(cID, cName, "");
            }
            ));
        }

    }
}
