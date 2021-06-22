using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using Npgsql;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace WacVisitor.FormSetting
{
    public partial class FormSearchCategoryInfo : Form
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


        #region PlaceHold TextBox
        private const int EM_SETCUEBANNER = 0x1501;
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)]string lParam);
        #endregion


        ArrayList arrList = new ArrayList(); 
        string tblName = "";
        public FormSearchCategoryInfo(string tbl)
        {
            InitializeComponent();

            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 10, 10));

            tblName = tbl;
        }

        private void FormSearchCategoryInfo_Load(object sender, EventArgs e)
        {
            SendMessage(textBox1.Handle, EM_SETCUEBANNER, 0, "ค้นหารายการ");

            listBox1.Items.Clear();  
            string queryString = "SELECT * FROM " + tblName;

            if (classGlobal.databaseType == "acc")
            {
                OleDbCommand command = new OleDbCommand(queryString, classGlobal.conn);
                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    listBox1.Items.Add(reader[1].ToString());
                }
                reader.Close();
                command.Dispose();
                command = null;
            }
            else if (classGlobal.databaseType == "psql")
            {
                NpgsqlCommand command = new NpgsqlCommand(queryString, classGlobal.connP);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    listBox1.Items.Add(reader[1].ToString());
                }
                reader.Close();
                command.Dispose();
                command = null;
            }
            else
            {
                arrList.Clear();
                string apiName = "";
                switch (tblName)
                {
                    case "tbl_visitor_company":
                        apiName = "visitFrom";
                        break;
                    case "tbl_license_plate":
                        apiName = "licensePlate";
                        break;
                    case "tbl_vehicle_type":
                        apiName = "vehicleType";
                        break;
                    case "tbl_department":
                        apiName = "department";
                        break;
                    case "tbl_visit_to":
                        apiName = "visitPerson";
                        break;
                    case "tbl_business_topic":
                        apiName = "contactTopic";
                        break;
                    case "tbl_place":
                        apiName = "visitPlace";
                        break;
                    default:
                        break;
                }

                string jsonString = ClassData.GET_METHODE(apiName);
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
                        listBox1.Items.Add(x.ToString());                        
                        arrList.Add(x.ToString());
                        j += 1;

                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (tblName == "tbl_visitor_company")
            {
                if (listBox1.SelectedItem == null)
                {
                    clsInfo.info_visitor_company = textBox1.Text;
                }
                else
                {
                    clsInfo.info_visitor_company = listBox1.SelectedItem.ToString();
                }
                
            }
            if (tblName == "tbl_license_plate")
            {
                if (listBox1.SelectedItem == null)
                {
                    clsInfo.info_license_plate = textBox1.Text;
                }
                else
                {
                    clsInfo.info_license_plate = listBox1.SelectedItem.ToString();
                }                
            }
            if (tblName == "tbl_vehicle_type")
            {
                if (listBox1.SelectedItem == null)
                {
                    clsInfo.info_vehicle_type = textBox1.Text;
                }
                else
                {
                    clsInfo.info_vehicle_type = listBox1.SelectedItem.ToString();
                }                
            }
            if (tblName == "tbl_department")
            {
                if (listBox1.SelectedItem == null)
                {
                    clsInfo.info_department = textBox1.Text;
                }
                else
                {
                    clsInfo.info_department = listBox1.SelectedItem.ToString();
                }                
            }
            if (tblName == "tbl_visit_to")
            {
                if (listBox1.SelectedItem == null)
                {
                    clsInfo.info_visit_to = textBox1.Text;
                }
                else
                {
                    clsInfo.info_visit_to = listBox1.SelectedItem.ToString();
                }
                
            }
            if (tblName == "tbl_business_topic")
            {
                if (listBox1.SelectedItem == null)
                {
                    clsInfo.info_business_topic = textBox1.Text;
                }
                else
                {
                    clsInfo.info_business_topic = listBox1.SelectedItem.ToString();
                }
                
            }
            if (tblName == "tbl_place")
            {
                if (listBox1.SelectedItem == null)
                {
                    clsInfo.info_place = textBox1.Text;
                }
                else
                {
                    clsInfo.info_place = listBox1.SelectedItem.ToString();
                }
               
            }

            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            button1.PerformClick();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            string queryString = "";
            if (textBox1.Text == "")
            {
                queryString = "SELECT * FROM " + tblName;
            }
            else
            {
                queryString = "SELECT * FROM " + tblName + " WHERE " + tblName.Replace("tbl_", "") + " LIKE '%" + textBox1.Text + "%'";
            }

            if (classGlobal.databaseType == "acc")
            {
                OleDbCommand command = new OleDbCommand(queryString, classGlobal.conn);
                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    listBox1.Items.Add(reader[1].ToString());
                }
                reader.Close();
                command.Dispose();
                command = null;
            }
            else if (classGlobal.databaseType == "psql")
            {
                NpgsqlCommand command = new NpgsqlCommand(queryString, classGlobal.connP);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    listBox1.Items.Add(reader[1].ToString());
                }
                reader.Close();
                command.Dispose();
                command = null;
            }
            else
            {
                foreach (string str in arrList.ToArray())
                {
                    if (str.Contains(textBox1.Text))
                    {
                        listBox1.Items.Add(str);
                    }
                } 
            }
            
        }
    }
}
