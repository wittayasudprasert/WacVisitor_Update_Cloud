using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using AForge.Video;
using AForge.Video.DirectShow;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

using System.Data.OleDb;
using System.Web.Services.Description;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Web.Services.Protocols;
using System.Reflection;
using System.Diagnostics;
using AForge.Imaging.Filters;
using ClassHelper;

namespace WacVisitor
{
    public partial class WebcamDevice : Form
    {
        private FilterInfoCollection CaptureDevice; // list of webcam
        private VideoCaptureDevice FinalFrame;

        public WebcamDevice()
        {
            InitializeComponent();
        }


        private void WebcamDevice_Load(object sender, EventArgs e)
        {

            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            panel1.Left = (this.Width - panel1.ClientSize.Width) / 2;
            panel1.Top = (this.Height - panel1.ClientSize.Height) / 2;


            CaptureDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice);//constructor
            FinalFrame = new VideoCaptureDevice();

            comboBox1.Items.Clear();  
            if (CaptureDevice.Count > 0)
            {
                // check webcam connect?  USB 2.0 PC Cam
                string webcamName = "";
                foreach (FilterInfo VideoCaptureDevice in CaptureDevice)
                {
                    webcamName = VideoCaptureDevice.Name.ToString();
                    if (webcamName.Contains("Scan"))
                    {
                        comboBox1.Items.Add(webcamName + " [Reserved]");
                    }
                    else
                    {
                        comboBox1.Items.Add(webcamName);
                    }

                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveWebcamDeviceConfig();
            classGlobal.SelectedWebcamDevice = comboBox1.SelectedIndex;
            this.Close();
        }

        private void SaveWebcamDeviceConfig()
        {

            try
            {
                ClassHelper.clsXML cls = new clsXML();
                if (System.IO.File.Exists(classGlobal.config) == false)
                {
                    cls.CreateXML(classGlobal.config);
                    cls.SetWriteXML("root", "webcamdevice", "-1", classGlobal.config);
                    
                }
                else
                {                    
                    cls.ModifyElement("root", "webcamdevice", comboBox1.SelectedIndex.ToString() , classGlobal.config);
                }
                cls = null;
               
            }
            catch
            {
              //
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
           this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text.Contains("Reserved"))
            {
                MessageBox.Show("รายการที่เลือก ไม่ใช่อุปกรณ์กล้อง WEB CAM!", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);  
                button1.Enabled = false;
                return; 
            }
            else
            {
                button1.Enabled = true;
            }
        }
    }
}
