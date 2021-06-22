using AForge.Imaging.Filters;
using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using ClassHelper;

namespace WacVisitor
{
    public partial class frmTakePicture : Form
    {

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();


        #region Reduce Memmory
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetProcessWorkingSetSize(IntPtr process,
            UIntPtr minimumWorkingSetSize, UIntPtr maximumWorkingSetSize);
        #endregion

        #region WebCam
        private FilterInfoCollection CaptureDevice; // list of webcam
        public VideoCaptureDevice FinalFrame;      
        static bool WebCamConnect = false;
        static int WebCamIndex = -1;

        private Bitmap TMPLTCAP = null; //defined elsewhere
        private bool RCRDPIC = false;
        private bool COMPON = false;

        //static string strWordingOpenWebcam = "เปิดกล้อง";
        //static string strWordingCloseWebcam = "ปิดกล้อง";
        //static string strWordingTake = "ถ่าย";
        //static string strWordingRetake = "ถ่ายใหม่";
        #endregion

        public frmTakePicture()
        {
            InitializeComponent();


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
                WebCamConnect = true;

                // check webcam connect?  USB 2.0 PC Cam
                string webcamName = "";
                foreach (FilterInfo VideoCaptureDevice in CaptureDevice)
                {
                    WebCamIndex = WebCamIndex + 1;
                    webcamName = VideoCaptureDevice.Name.ToString().Replace(" ", "").ToLower();
                    if (webcamName.Contains("cam"))
                    {                        
                        WebCamConnect = true;
                        break;
                    }

                }

                clsXML c = new clsXML();
                string tem = c.GetReadXML("root", "webcamdevice", classGlobal.config);
                c = null;
                if (tem == "") { tem = "-1"; }
                WebCamIndex = Int32.Parse(tem);

                if (WebCamIndex < 0)
                {
                    MessageBox.Show("ยังไม่ได้เลือก WEB CAM, กรุณาเลือก", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    btnTakePhoto.Enabled = false;
                    btnAccept.Enabled = false;
                    WebCamConnect = false;
                }
                else
                {
                    string nameOfWebcam = "";
                    try
                    {
                        nameOfWebcam = CaptureDevice[WebCamIndex].Name.ToString().ToLower();
                    }
                    catch
                    {
                       nameOfWebcam = CaptureDevice[0].Name.ToString().ToLower();
                    }
                    if (nameOfWebcam.Contains("scan"))
                    {
                        MessageBox.Show("กรุณาเลือกอุปกรณ์ WEB CAM ใหม่", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        btnTakePhoto.Enabled = false;
                        btnAccept.Enabled = false;
                        WebCamConnect = false;
                    }
                }

            }
        }

        private void frmTakePicture_Load(object sender, EventArgs e)
        {

            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;     
            minimizeMemory();    // release memory

            btnAccept.Enabled = false;

            if (WebCamConnect == true)
            {
                if (CaptureDevice.Count < WebCamIndex)
                {
                    WebCamIndex = 0;
                }

                try
                {
                    FinalFrame = new VideoCaptureDevice(CaptureDevice[WebCamIndex].MonikerString);// specified web cam and its filter moniker string   
                }
                catch
                {
                    FinalFrame = new VideoCaptureDevice(CaptureDevice[0].MonikerString);// specified web cam and its filter moniker string   
                }
                             
                FinalFrame.VideoResolution = FinalFrame.VideoCapabilities[2];

                FinalFrame.NewFrame += new NewFrameEventHandler(FinalFrame_NewFrame);// click button event is fired, 
                FinalFrame.Start();
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
                try
                {
                    OldImage = picWebcam.Image;
                    picWebcam.Image = AForge.Imaging.Image.Clone(eventArgs.Frame);
                    OldImage.Dispose();
                }
                catch(Exception ex)
                {
                    //
                }
                //Toggle the flag back to false to show it's safe (i.e., comparisons have stopped)
                //for the save method to copy from the picture box.
                if (RCRDPIC == true)
                {
                    RCRDPIC = false;
                }
            }
        }

        private void TakePhoto()
        {
            if (System.IO.Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + classGlobal.webcam) == false)
            {
                System.IO.Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + classGlobal.webcam);
            }

            if (picWebcam.Image != null)
            {
                minimizeMemory();

                try
                {
                    //Save First
                    Bitmap varBmp = new Bitmap(picWebcam.Image);
                    Bitmap newBitmap = new Bitmap(varBmp);
                    string filename = DateTime.Now.ToString("yyyyddMMHHmmss") + ".jpg";

                    SaveBitmapWithQuality(newBitmap, classGlobal.webcam + @"\" + filename, 80L);

                    if (classGlobal.cam == "cam0")
                    {
                        classGlobal.PlustekBase64String = PictureToBase64String(classGlobal.webcam + @"\" + filename);
                    }
                    if (classGlobal.cam == "cam1")
                    {
                        classGlobal.WebcamBase64String = PictureToBase64String(classGlobal.webcam + @"\" + filename);
                    }
                    if (classGlobal.cam == "cam2")
                    {
                        classGlobal.WebcamBase64String1 = PictureToBase64String(classGlobal.webcam + @"\" + filename);
                    }
                    if (classGlobal.cam == "cam3")
                    {
                        classGlobal.WebcamBase64String2 = PictureToBase64String(classGlobal.webcam + @"\" + filename);
                    }
                    
                    Image img = Image.FromFile(classGlobal.webcam + @"\" + filename);
                    byte[] byteArrayImg;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        byteArrayImg = ms.ToArray();
                        ms.Dispose();
                    }
                    img.Dispose();
                    img = null;
                    System.IO.File.Delete(classGlobal.webcam + @"\" + filename);

                    using (var ms = new MemoryStream(byteArrayImg))
                    {
                        picWebcam.Image = Image.FromStream(ms);
                    }

                }
                catch
                {
                    //
                }
                finally
                {
                    FinalFrame.Stop();
                }

            }
            else
            {
                //MessageBox.Show("null exception"); 
            }
        }
        private void RetakePhoto()
        {
            if (WebCamConnect == true)
                FinalFrame.Start();
        }

        private static Bitmap ResizeImage(Bitmap mg, Size newSize)
        {
            double ratio = 0d;
            double myThumbWidth = 0d;
            double myThumbHeight = 0d;
            int x = 0;
            int y = 0;

            Bitmap bp;

            if ((mg.Width / Convert.ToDouble(newSize.Width)) > (mg.Height /
            Convert.ToDouble(newSize.Height)))
                ratio = Convert.ToDouble(mg.Width) / Convert.ToDouble(newSize.Width);
            else
                ratio = Convert.ToDouble(mg.Height) / Convert.ToDouble(newSize.Height);
            myThumbHeight = Math.Ceiling(mg.Height / ratio);
            myThumbWidth = Math.Ceiling(mg.Width / ratio);

            Size thumbSize = new Size((int)myThumbWidth, (int)myThumbHeight);
            bp = new Bitmap(newSize.Width, newSize.Height);
            x = (newSize.Width - thumbSize.Width) / 2;
            y = (newSize.Height - thumbSize.Height);

            System.Drawing.Graphics g = Graphics.FromImage(bp);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            Rectangle rect = new Rectangle(x, y, thumbSize.Width, thumbSize.Height);
            g.DrawImage(mg, rect, 0, 0, mg.Width, mg.Height, GraphicsUnit.Pixel);

            return bp;
        }
        public void SaveBitmapWithQuality(Bitmap bmp, string filename, Int64 quality)
        {
            // Get a bitmap.
            Bitmap bmp1 = new Bitmap(bmp);
            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);

            // Create an Encoder object based on the GUID
            // for the Quality parameter category.
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;

            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, quality);  //0L - 10L  less value as bad quality (low size)
            myEncoderParameters.Param[0] = myEncoderParameter;
            bmp1.Save(filename, jgpEncoder,
                myEncoderParameters);

            bmp1.Dispose();
        }
        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        private string PictureToBase64String(string imgPath)
        {
            try
            {
                using (Image image = Image.FromFile(imgPath))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        byte[] imageBytes = m.ToArray();

                        // Convert byte[] to Base64 String
                        string base64String = Convert.ToBase64String(imageBytes);
                        return base64String;
                    }
                }
            }
            catch
            {
                return "";
            }

        }

        private static void minimizeMemory()
        {
            GC.Collect(GC.MaxGeneration);
            GC.WaitForPendingFinalizers();
            SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, (UIntPtr)0xFFFFFFFF, (UIntPtr)0xFFFFFFFF);
        }

        private void btnTakePhoto_Click(object sender, EventArgs e)
        {
            if (btnTakePhoto.Text == "ถ่าย")
            {
                btnTakePhoto.Text = "ถ่ายใหม่";
                TakePhoto();
            }
            else
            {
                btnTakePhoto.Text = "ถ่าย";
                RetakePhoto();
            }

            btnAccept.Enabled = true;

            if (FinalFrame != null)
            {
                if (FinalFrame.IsRunning == true)
                {
                    //Signal the camera to stop, then remove the event handler and camera.
                    FinalFrame.Stop();
                    FinalFrame.NewFrame -= new NewFrameEventHandler(FinalFrame_NewFrame);
                }
                FinalFrame = null;
            }
            this.Close();
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            if (FinalFrame != null)
            {
                if (FinalFrame.IsRunning == true)
                {
                    //Signal the camera to stop, then remove the event handler and camera.
                    FinalFrame.Stop();
                    FinalFrame.NewFrame -= new NewFrameEventHandler(FinalFrame_NewFrame);                    
                }
                FinalFrame = null;
            }
            this.Close();
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (FinalFrame != null)
            {
                if (FinalFrame.IsRunning == true)
                {
                    //Signal the camera to stop, then remove the event handler and camera.
                    FinalFrame.Stop();
                    FinalFrame.NewFrame -= new NewFrameEventHandler(FinalFrame_NewFrame);
                }
                FinalFrame = null;
            }

            this.Close();
        }

      

    }
}
