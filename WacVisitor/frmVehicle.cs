using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Npgsql;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using System.IO;

namespace WacVisitor
{
    public partial class frmVehicle : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public frmVehicle()
        {
            InitializeComponent();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            var culture = System.Globalization.CultureInfo.GetCultureInfo("th-TH");
            var language = InputLanguage.FromCulture(culture);
            InputLanguage.CurrentInputLanguage = language;
        }

        void SET_KB_LANGUAGE()
        {
            string kbLayout = InputLanguage.CurrentInputLanguage.Culture.Name;
            string new_kbLayout = System.IO.File.ReadAllText("lang").Replace(" ", "").Replace(Environment.NewLine, "");
            var culture = System.Globalization.CultureInfo.GetCultureInfo(new_kbLayout);
            var language = InputLanguage.FromCulture(culture);
            InputLanguage.CurrentInputLanguage = language;
            kbLayout = InputLanguage.CurrentInputLanguage.Culture.Name;
        }

        private void frmVehicle_Load(object sender, EventArgs e)
        {
            SET_KB_LANGUAGE();

            classGlobal.destinationNotification = "";

            DataTable dataTable = new DataTable("vehicle_list");
            dataTable.Columns.Add("Id");
            dataTable.Columns.Add("Name");

            DataTable vehicle = new DataTable("vehicle");

            if (classGlobal.databaseType == "acc")
            {
                OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * FROM tbl_vehicle", classGlobal.conn);
                try
                {
                    adapter.Fill(vehicle);
                }
                catch
                {
                    //
                }
                adapter.Dispose();
                adapter = null;
            }
            else if (classGlobal.databaseType == "psql")
            {
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter("SELECT * FROM tbl_vehicle", classGlobal.connP);
                try
                {
                    adapter.Fill(vehicle);
                }
                catch
                {
                    //
                }
                adapter.Dispose();
                adapter = null;
            }
            else
            {

                vehicle.Columns.Add("v_id");
                vehicle.Columns.Add("v_nametype");

                string jsonString = ClassData.GET_METHODE("vehicleType");
                if (jsonString == "")
                {
                    //-- no data
                }
                else
                {
                    JArray jsArray = JArray.Parse(jsonString);
                    int j = 1;
                    foreach (var x in jsArray)
                    {
                        vehicle.Rows.Add(j, x.ToString());
                        j += 1;
                    }
                }
            }

            comboBox1.Items.Clear();

            dataTable.Rows.Add("0", "--กรุณาระบุ--");
            foreach (DataRow dr in vehicle.Rows)
            {
                dataTable.Rows.Add(dr["v_id"], dr["v_nametype"]);
            }

            comboBox1.DataSource = dataTable;
            comboBox1.DisplayMember = "Name";
            comboBox1.SelectedIndex = 0;
            comboBox1.Refresh();


            if (classGlobal.arrVehicleInfo.Count > 0)
            {
                int v = 0;
                foreach (DataRow dr in dataTable.Rows)
                {
                    if (dr.ItemArray[1].ToString().Equals(classGlobal.arrVehicleInfo[2].ToString()))
                    {
                        v = Int32.Parse(dr.ItemArray[0].ToString());
                        break; 
                    }
                }

                textBox1.Text = classGlobal.arrVehicleInfo[0].ToString();
                textBox2.Text = classGlobal.arrVehicleInfo[1].ToString();
                comboBox1.SelectedIndex = v;
                textBox3.Text = classGlobal.arrVehicleInfo[3].ToString();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            classGlobal.arrVehicleInfo.Clear();

            DataRow selectedDataRow = ((DataRowView)comboBox1.SelectedItem).Row;
            int vId = Convert.ToInt32(selectedDataRow["Id"]);
            string vName = selectedDataRow["Name"].ToString();
            vName = vName.Replace("--กรุณาระบุ--", "ไม่ระบุ"); 

            classGlobal.arrVehicleInfo.Add(textBox1.Text); //สถานที่ติดต่อ
            classGlobal.arrVehicleInfo.Add(textBox2.Text); //ทะเบียนรถ
            classGlobal.arrVehicleInfo.Add(vName); //ชนิดของรถ
            classGlobal.arrVehicleInfo.Add(textBox3.Text);  //ข้อมูลอื่นๆ

            classGlobal.destinationNotification = textBox1.Text;


            clsInfo.info_place = textBox1.Text;
            clsInfo.info_license_plate = textBox2.Text;
            clsInfo.info_vehicle_type = vName;
            clsInfo.info_etc = textBox3.Text;

            Console.Write(classGlobal.appointMentSelectedId);
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

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Console.Write(classGlobal.arrVehicleInfo);
            this.Close();
        }

        private void frmVehicle_FormClosing(object sender, FormClosingEventArgs e)
        {
            var culture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
            var language = InputLanguage.FromCulture(culture);
            InputLanguage.CurrentInputLanguage = language;
        }








        #region จับเวลาการพิมพ์ข้อมูลบน textbox
        static int VALIDATION_DELAY = 1500;
        System.Threading.Timer timer = null;
   
        private void TimerElapsed(Object obj)
        {
            CheckSyntaxAndReport();
            DisposeTimer();
        }
        private void DisposeTimer()
        {
            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }
        }
        private void CheckSyntaxAndReport()
        {
            this.Invoke(new Action(() =>
            {
                string s = textBox2.Text.ToUpper();
                s = s.Replace(" ", "");
                if (s != "")
                {
                    MessageBox.Show(s);                    
                }
            }
            ));
        }
        #endregion

        private void TextBox2_TextChanged(object sender, EventArgs e)
        {
            if (classGlobal.userId != "")
            {
                //TextBox origin = sender as TextBox;
                //if (!origin.ContainsFocus)
                //    return;
                //DisposeTimer();
                //timer = new System.Threading.Timer(TimerElapsed, null, VALIDATION_DELAY, VALIDATION_DELAY);

                timerDelay.Stop();
                timerDelay.Start();
            }
        }

        private void TimerDelay_Tick(object sender, EventArgs e)
        {
            timerDelay.Stop();
            this.Invoke(new Action(() =>
            {
                string place = textBox1.Text;
                string licensePlate = textBox2.Text;                
                ClassData.CHECK_APPOINTMENT(classGlobal.personID, classGlobal.personName, place, licensePlate);                
            }
            ));
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            if (classGlobal.userId != "")
            {
                //TextBox origin = sender as TextBox;
                //if (!origin.ContainsFocus)
                //    return;
                //DisposeTimer();
                //timer = new System.Threading.Timer(TimerElapsed, null, VALIDATION_DELAY, VALIDATION_DELAY);

                timerDelay.Stop();
                timerDelay.Start();
            }
        }
    }
}


//DataTable dataTable = new DataTable("Country");
//dataTable.Columns.Add("Id");
//dataTable.Columns.Add("Name");
//dataTable.Rows.Add(45, "Denmark");
//dataTable.Rows.Add(63, "Philippines");   
//comboBox1.DataSource = dataTable;
//comboBox1.DisplayMember = "Name";

//comboBox1.SelectedIndex = 1;
//comboBox1.Refresh();  

//DataRow selectedDataRow = ((DataRowView)comboBox1.SelectedItem).Row;
//int countryId = Convert.ToInt32(selectedDataRow["Id"]);
//string countryName = selectedDataRow["Name"].ToString();