using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Npgsql;

namespace WacVisitor
{
    public partial class FormMoreInfo : Form
    {
        public FormMoreInfo()
        {

            InitializeComponent();

            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            ico = Image.FromFile(@"icon\search.png");
            bmp = new Bitmap(ico, button5.Width, button5.Height);

            button5.Text = "";
            button6.Text = "";
            button7.Text = "";
            button8.Text = "";
            button9.Text = "";
            button10.Text = "";
            button11.Text = "";

            button5.Image = bmp;
            button6.Image = bmp;
            button7.Image = bmp;
            button8.Image = bmp;
            button9.Image = bmp;
            button10.Image = bmp;
            button11.Image = bmp;

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

        Bitmap bmp;
        Image ico;
        string queryString = "";
        private void FormMoreInfo_Load(object sender, EventArgs e)
        {
            SET_KB_LANGUAGE();

            panel1.Left = ((this.Width / 2) - (panel1.Width / 2));
            panel1.Top = ((this.Height / 2) - (panel1.Height / 2));

            comboBox1.Items.Clear();
            comboBox1.Items.Add("");
            for (int c1 = 1; c1 <= 15; c1++)
            {
                comboBox1.Items.Add(c1.ToString());
            }

            //GET_COMPANY();
            //GET_LICENSE_PLATE();
            //GET_VEHICLE_TYPE();
            //GET_VISIT_TO();
            //GET_DEPARTMENT();
            //GET_BUSINESS_TOPIC();
            //GET_PLACE();

            if (clsInfo.info_follower == null || clsInfo.info_follower == "" || clsInfo.info_follower == "0")
            {
                clsInfo.info_follower = "1";
            }
            comboBox1.Text = clsInfo.info_follower;
            //comboBox2.Text = clsInfo.info_visitor_company;
            //comboBox3.Text = clsInfo.info_license_plate;
            //comboBox4.Text = clsInfo.info_vehicle_type;
            //comboBox5.Text = clsInfo.info_visit_to;
            //comboBox6.Text = clsInfo.info_department;
            //comboBox7.Text = clsInfo.info_business_topic;
            //comboBox8.Text = clsInfo.info_place;
            textBox1.Text = clsInfo.info_etc;


            textBox2.Text = clsInfo.info_visitor_company;
            textBox3.Text = clsInfo.info_license_plate;
            textBox4.Text = clsInfo.info_vehicle_type;
            textBox5.Text = clsInfo.info_department;
            textBox6.Text = clsInfo.info_visit_to;
            textBox7.Text = clsInfo.info_business_topic;
            textBox8.Text = clsInfo.info_place;

        }


        
        string[] data_company;
        string[] data_license_plate;
        string[] data_vehicle_type;
        string[] data_department;
        string[] data_visitor_to;
        string[] data_topic;
        string[] data_place;      
        private void GET_COMPANY()
        {

            data_company = new string[0];
            Array.Resize(ref data_company, data_company.Length + 1);
            data_company[data_company.Length - 1] = "";

            comboBox2.DataSource = null;
            comboBox2.Items.Clear();

            DataTable dataTable = new DataTable("GET_COMPANY");
            dataTable.Columns.Add("Id");
            dataTable.Columns.Add("Name");

            dataTable.Rows.Add(0, "");

            queryString = "SELECT id, visitor_company FROM tbl_visitor_company";

            if (classGlobal.databaseType == "acc")
            {
                OleDbCommand command = new OleDbCommand(queryString, classGlobal.conn);
                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Application.DoEvents();
                    dataTable.Rows.Add(Int32.Parse(reader[0].ToString()), reader[1].ToString());

                    Array.Resize(ref data_company, data_company.Length + 1);
                    data_company[data_company.Length - 1] = reader[1].ToString();

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
                    Application.DoEvents();
                    dataTable.Rows.Add(Int32.Parse(reader[0].ToString()), reader[1].ToString());

                    Array.Resize(ref data_company, data_company.Length + 1);
                    data_company[data_company.Length - 1] = reader[1].ToString();

                }
                reader.Close();
                command.Dispose();
                command = null;
            }
            else
            {

            }
            

            comboBox2.DataSource = dataTable;
            comboBox2.DisplayMember = "Name";

        }
        private void GET_LICENSE_PLATE()
        {
            data_license_plate = new string[0];
            Array.Resize(ref data_license_plate, data_license_plate.Length + 1);
            data_license_plate[data_license_plate.Length - 1] = "";


            comboBox3.DataSource = null;
            comboBox3.Items.Clear();

            DataTable dataTable = new DataTable("GET_LICENSE_PLATE");
            dataTable.Columns.Add("Id");
            dataTable.Columns.Add("Name");

            dataTable.Rows.Add(0, "");

            queryString = "SELECT id, license_plate FROM tbl_license_plate";

            if (classGlobal.databaseType == "acc")
            {
                OleDbCommand command = new OleDbCommand(queryString, classGlobal.conn);
                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Application.DoEvents();
                    dataTable.Rows.Add(Int32.Parse(reader[0].ToString()), reader[1].ToString());

                    Array.Resize(ref data_license_plate, data_license_plate.Length + 1);
                    data_license_plate[data_license_plate.Length - 1] = reader[1].ToString();

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
                    Application.DoEvents();
                    dataTable.Rows.Add(Int32.Parse(reader[0].ToString()), reader[1].ToString());

                    Array.Resize(ref data_license_plate, data_license_plate.Length + 1);
                    data_license_plate[data_license_plate.Length - 1] = reader[1].ToString();

                }
                reader.Close();
                command.Dispose();
                command = null;
            }
            else
            {

            }
           

            comboBox3.DataSource = dataTable;
            comboBox3.DisplayMember = "Name";
        }
        private void GET_VEHICLE_TYPE()
        {
            data_vehicle_type = new string[0];
            Array.Resize(ref data_vehicle_type, data_vehicle_type.Length + 1);
            data_vehicle_type[data_vehicle_type.Length - 1] = "";


            comboBox4.DataSource = null;
            comboBox4.Items.Clear();

            DataTable dataTable = new DataTable("GET_VEHICLE_TYPE");
            dataTable.Columns.Add("Id");
            dataTable.Columns.Add("Name");

            dataTable.Rows.Add(0, "");

            queryString = "SELECT id, vehicle_type FROM tbl_vehicle_type";

            if (classGlobal.databaseType == "acc")
            {
                OleDbCommand command = new OleDbCommand(queryString, classGlobal.conn);
                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Application.DoEvents();
                    dataTable.Rows.Add(Int32.Parse(reader[0].ToString()), reader[1].ToString());

                    Array.Resize(ref data_vehicle_type, data_vehicle_type.Length + 1);
                    data_vehicle_type[data_vehicle_type.Length - 1] = reader[1].ToString();
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
                    Application.DoEvents();
                    dataTable.Rows.Add(Int32.Parse(reader[0].ToString()), reader[1].ToString());

                    Array.Resize(ref data_vehicle_type, data_vehicle_type.Length + 1);
                    data_vehicle_type[data_vehicle_type.Length - 1] = reader[1].ToString();
                }
                reader.Close();
                command.Dispose();
                command = null;
            }
            else
            {

            }
           

            comboBox4.DataSource = dataTable;
            comboBox4.DisplayMember = "Name";
        }
        private void GET_VISIT_TO()
        {
            data_visitor_to = new string[0];
            Array.Resize(ref data_visitor_to, data_visitor_to.Length + 1);
            data_visitor_to[data_visitor_to.Length - 1] = "";

            comboBox5.DataSource = null;
            comboBox5.Items.Clear();

            DataTable dataTable = new DataTable("GET_VISIT_TO");
            dataTable.Columns.Add("Id");
            dataTable.Columns.Add("Name");

            dataTable.Rows.Add(0, "");

            queryString = "SELECT id, visit_to FROM tbl_visit_to";

            if (classGlobal.databaseType == "acc")
            {
                OleDbCommand command = new OleDbCommand(queryString, classGlobal.conn);
                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Application.DoEvents();
                    dataTable.Rows.Add(Int32.Parse(reader[0].ToString()), reader[1].ToString());

                    Array.Resize(ref data_visitor_to, data_visitor_to.Length + 1);
                    data_visitor_to[data_visitor_to.Length - 1] = reader[1].ToString();
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
                    Application.DoEvents();
                    dataTable.Rows.Add(Int32.Parse(reader[0].ToString()), reader[1].ToString());

                    Array.Resize(ref data_visitor_to, data_visitor_to.Length + 1);
                    data_visitor_to[data_visitor_to.Length - 1] = reader[1].ToString();
                }
                reader.Close();
                command.Dispose();
                command = null;
            }
            else
            {

            }
           

            comboBox5.DataSource = dataTable;
            comboBox5.DisplayMember = "Name";
        }
        private void GET_DEPARTMENT()
        {
            data_department = new string[0];
            Array.Resize(ref data_department, data_department.Length + 1);
            data_department[data_department.Length - 1] = "";

            comboBox6.DataSource = null;
            comboBox6.Items.Clear();

            DataTable dataTable = new DataTable("GET_DEPARTMENT");
            dataTable.Columns.Add("Id");
            dataTable.Columns.Add("Name");

            dataTable.Rows.Add(0, "");

            queryString = "SELECT id, department FROM tbl_department";

            if (classGlobal.databaseType == "acc")
            {
                OleDbCommand command = new OleDbCommand(queryString, classGlobal.conn);
                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Application.DoEvents();
                    dataTable.Rows.Add(Int32.Parse(reader[0].ToString()), reader[1].ToString());

                    Array.Resize(ref data_department, data_department.Length + 1);
                    data_department[data_department.Length - 1] = reader[1].ToString();
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
                    Application.DoEvents();
                    dataTable.Rows.Add(Int32.Parse(reader[0].ToString()), reader[1].ToString());

                    Array.Resize(ref data_department, data_department.Length + 1);
                    data_department[data_department.Length - 1] = reader[1].ToString();
                }
                reader.Close();
                command.Dispose();
                command = null;
            }
            else
            {

            }
           

            comboBox6.DataSource = dataTable;
            comboBox6.DisplayMember = "Name";
        }
        private void GET_BUSINESS_TOPIC()
        {
            data_topic = new string[0];
            Array.Resize(ref data_topic, data_topic.Length + 1);
            data_topic[data_topic.Length - 1] = "";

            comboBox7.DataSource = null;
            comboBox7.Items.Clear();

            DataTable dataTable = new DataTable("GET_BUSINESS_TOPIC");
            dataTable.Columns.Add("Id");
            dataTable.Columns.Add("Name");

            dataTable.Rows.Add(0, "");

            queryString = "SELECT id, business_topic FROM tbl_business_topic";

            if (classGlobal.databaseType == "psql")
            {
                NpgsqlCommand command = new NpgsqlCommand(queryString, classGlobal.connP);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Application.DoEvents();
                    dataTable.Rows.Add(Int32.Parse(reader[0].ToString()), reader[1].ToString());

                    Array.Resize(ref data_topic, data_topic.Length + 1);
                    data_topic[data_topic.Length - 1] = reader[1].ToString();
                }
                reader.Close();
                command.Dispose();
                command = null;
            }
            else if (classGlobal.databaseType == "acc")
            {

            }
            else
            {

            }
          

            comboBox7.DataSource = dataTable;
            comboBox7.DisplayMember = "Name";
        }
        private void GET_PLACE()
        {
            data_place = new string[0];
            Array.Resize(ref data_place, data_place.Length + 1);
            data_place[data_place.Length - 1] = "";

            comboBox8.DataSource = null;
            comboBox8.Items.Clear();

            DataTable dataTable = new DataTable("GET_PLACE");
            dataTable.Columns.Add("Id");
            dataTable.Columns.Add("Name");

            dataTable.Rows.Add(0, "");

            queryString = "SELECT id, place FROM tbl_place";

            if (classGlobal.databaseType == "acc")
            {
                OleDbCommand command = new OleDbCommand(queryString, classGlobal.conn);
                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Application.DoEvents();
                    dataTable.Rows.Add(Int32.Parse(reader[0].ToString()), reader[1].ToString());

                    Array.Resize(ref data_place, data_place.Length + 1);
                    data_place[data_place.Length - 1] = reader[1].ToString();
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
                    Application.DoEvents();
                    dataTable.Rows.Add(Int32.Parse(reader[0].ToString()), reader[1].ToString());

                    Array.Resize(ref data_place, data_place.Length + 1);
                    data_place[data_place.Length - 1] = reader[1].ToString();
                }
                reader.Close();
                command.Dispose();
                command = null;
            }
            else
            {

            }
          

            comboBox8.DataSource = dataTable;
            comboBox8.DisplayMember = "Name";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            clsInfo.info_follower = "";
            clsInfo.info_visitor_company = "";
            clsInfo.info_license_plate = "";
            clsInfo.info_vehicle_type = "";
            clsInfo.info_visit_to = "";
            clsInfo.info_department = "";
            clsInfo.info_business_topic = "";
            clsInfo.info_place = "";
            clsInfo.info_etc = ""; 

            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            #region backup
            //DataRow selectedDataRow;
            //int Id;
            //string name = "";

            //if (comboBox1.Text == "")
            //    Id = 0;
            //else
            //    Id = Int32.Parse(comboBox1.Text);
            //clsInfo.info_follower = Id.ToString();

            //try
            //{
            //    selectedDataRow = ((DataRowView)comboBox2.SelectedItem).Row;
            //    Id = Convert.ToInt32(selectedDataRow["Id"]);
            //    name = selectedDataRow["Name"].ToString();
            //    clsInfo.info_visitor_company = name;
            //}
            //catch
            //{
            //    if (comboBox2.SelectedItem.ToString() != "")
            //    {
            //        clsInfo.info_visitor_company = comboBox2.SelectedItem.ToString();
            //    }                
            //}
           

            ////Id = GET_LICENSE_PLATE_ID(comboBox3.Text);
            //clsInfo.info_license_plate = comboBox3.Text;

            //try
            //{
            //    selectedDataRow = ((DataRowView)comboBox4.SelectedItem).Row;
            //    Id = Convert.ToInt32(selectedDataRow["Id"]);
            //    name = selectedDataRow["Name"].ToString();
            //    clsInfo.info_vehicle_type = name;
            //}
            //catch
            //{                
            //    if (comboBox4.SelectedItem.ToString() != "")
            //    {
            //        clsInfo.info_vehicle_type = comboBox4.SelectedItem.ToString();
            //    }  
            //}


            //try
            //{
            //    selectedDataRow = ((DataRowView)comboBox5.SelectedItem).Row;
            //    Id = Convert.ToInt32(selectedDataRow["Id"]);
            //    name = selectedDataRow["Name"].ToString();
            //    clsInfo.info_visit_to = name;
            //}
            //catch
            //{
            //    if (comboBox5.SelectedItem.ToString() != "")
            //    {
            //        clsInfo.info_visit_to = comboBox5.SelectedItem.ToString(); 
            //    }
                
            //}

            //try
            //{
            //    selectedDataRow = ((DataRowView)comboBox6.SelectedItem).Row;
            //    Id = Convert.ToInt32(selectedDataRow["Id"]);
            //    name = selectedDataRow["Name"].ToString();
            //    clsInfo.info_department = name;
            //}
            //catch
            //{
            //    if (comboBox6.SelectedItem.ToString() != "")
            //    {
            //        clsInfo.info_department = comboBox6.SelectedItem.ToString(); 
            //    }
                
            //}


            //try
            //{
            //    selectedDataRow = ((DataRowView)comboBox7.SelectedItem).Row;
            //    Id = Convert.ToInt32(selectedDataRow["Id"]);
            //    name = selectedDataRow["Name"].ToString();
            //    clsInfo.info_business_topic = name;
            //}
            //catch
            //{
            //    if (comboBox7.SelectedItem.ToString() != "")
            //    {
            //        clsInfo.info_business_topic = comboBox7.SelectedItem.ToString(); 
            //    }
                
            //}


            //try
            //{
            //    selectedDataRow = ((DataRowView)comboBox8.SelectedItem).Row;
            //    Id = Convert.ToInt32(selectedDataRow["Id"]);
            //    name = selectedDataRow["Name"].ToString();
            //    clsInfo.info_place = name;
            //}
            //catch
            //{
            //    if (comboBox8.SelectedItem.ToString() != "")
            //    {
            //        clsInfo.info_place = comboBox8.SelectedItem.ToString(); 
            //    }

            //}
            #endregion

            if (comboBox1.Text == "")
            {
                frmMessageBox f = new frmMessageBox();
                f.strMessage = "กรุณาระบุจำนวนคน";
                f.strStatus = "Warning";
                f.ShowDialog();
                return;
            }

            clsInfo.info_follower = comboBox1.Text;

            //++
            clsInfo.info_visitor_company = textBox2.Text;
            clsInfo.info_license_plate = textBox3.Text;
            clsInfo.info_vehicle_type = textBox4.Text;
            clsInfo.info_department = textBox5.Text;
            clsInfo.info_visit_to = textBox6.Text;
            clsInfo.info_business_topic = textBox7.Text;
            clsInfo.info_place = textBox8.Text;
            //--

            clsInfo.info_etc = textBox1.Text;
            
            classGlobal.destinationNotification = clsInfo.info_place;
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string currentComboText = "";
            if (comboBox3.Text == "")
            {
                frmMessageBox f = new frmMessageBox();
                f.strMessage = "กรุณากรอกเลขทะเบียนรถ";
                f.strStatus = "Warning";
                f.ShowDialog();
                return;
            }
            else
            {
                int nExist = -1;
                queryString = "SELECT id FROM tbl_license_plate WHERE license_plate = '" + comboBox3.Text + "'";

                if (classGlobal.databaseType == "acc")
                {
                    OleDbCommand command1 = new OleDbCommand(queryString, classGlobal.conn);
                    OleDbDataReader reader = command1.ExecuteReader();
                    while (reader.Read())
                    {
                        nExist = Int32.Parse(reader[0].ToString());
                    }
                    reader.Close();
                    command1.Dispose();
                    command1 = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    NpgsqlCommand command1 = new NpgsqlCommand(queryString, classGlobal.connP);
                    NpgsqlDataReader reader = command1.ExecuteReader();
                    while (reader.Read())
                    {
                        nExist = Int32.Parse(reader[0].ToString());
                    }
                    reader.Close();
                    command1.Dispose();
                    command1 = null;
                }
                else
                {

                }
                

                if (nExist > 0)
                {
                    frmMessageBox f = new frmMessageBox();
                    f.strMessage = "เลขทะเบียนรถมีอยู่แล้ว";
                    f.strStatus = "Warning";
                    f.ShowDialog();
                    return;
                }
                else
                {
                    string query = "INSERT INTO tbl_license_plate (license_plate) VALUES (@license_plate)";
                    if (classGlobal.databaseType == "acc")
                    {
                        OleDbCommand command = new OleDbCommand(query, classGlobal.conn);
                        command.Parameters.Add("@license_plate", OleDbType.VarChar).Value = comboBox3.Text;
                        command.ExecuteNonQuery();
                        command.Dispose();
                        command = null;
                    }
                    else if (classGlobal.databaseType == "psql")
                    {
                        NpgsqlCommand command = new NpgsqlCommand(query, classGlobal.connP);
                        command.Parameters.AddWithValue("@license_plate", comboBox3.Text);
                        command.ExecuteNonQuery();
                        command.Dispose();
                        command = null;
                    }
                    else
                    {

                    }
                   

                    currentComboText = comboBox3.Text;
                }
            }

            GET_LICENSE_PLATE();
            comboBox3.Text = currentComboText;

            frmMessageBox f1 = new frmMessageBox();
            f1.strMessage = "เพิ่มเลขทะเบียนสำเร็จ";
            f1.strStatus = "Information";
            f1.ShowDialog();
        }

        private int GET_LICENSE_PLATE_ID(string s)
        {
            int ret = 0;
            try
            {
                DataTable _dt = new DataTable("_dt");
                if (classGlobal.databaseType == "acc")
                {
                    OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT id FROM tbl_license_plate WHERE license_plate ='" + s + "'", classGlobal.conn);
                    adapter.Fill(_dt);
                    adapter.Dispose();
                    adapter = null;
                    ret = Int32.Parse(_dt.Rows[0].ItemArray[0].ToString());   
                }
                else if (classGlobal.databaseType == "psql")
                {
                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter("SELECT id FROM tbl_license_plate WHERE license_plate ='" + s + "'", classGlobal.connP);
                    adapter.Fill(_dt);
                    adapter.Dispose();
                    adapter = null;
                    ret = Int32.Parse(_dt.Rows[0].ItemArray[0].ToString());
                }
                else
                {

                }
               
                _dt.Dispose();
                _dt = null;
            }
            catch
            {
                ret = 0;
            }

            return ret;
        }

        private void comboBox3_TextChanged(object sender, EventArgs e)
        {
            HandleTextChanged(comboBox3, data_license_plate);
        }

        private void comboBox3_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {
                int sStart = comboBox3.SelectionStart;
                if (sStart > 0)
                {
                    sStart--;
                    if (sStart == 0)
                    {
                        comboBox3.Text = "";
                    }
                    else
                    {
                        comboBox3.Text = comboBox3.Text.Substring(0, sStart);
                    }
                }
                e.Handled = true;
            }
        }

        

     
        private void comboBox2_TextChanged(object sender, EventArgs e)
        {
            HandleTextChanged(comboBox2, data_company);
        }

        private void comboBox2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {
                int sStart = comboBox2.SelectionStart;
                if (sStart > 0)
                {
                    sStart--;
                    if (sStart == 0)
                    {
                        comboBox2.Text = "";
                    }
                    else
                    {
                        comboBox2.Text = comboBox2.Text.Substring(0, sStart);
                    }
                }
                e.Handled = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string currentComboText = "";
            if (comboBox2.Text == "")
            {
                frmMessageBox f = new frmMessageBox();
                f.strMessage = "กรุณากรอกชื่อบริษัท";
                f.strStatus = "Warning";
                f.ShowDialog();
                return;
            }
            else
            {
                int nExist = -1;
                queryString = "SELECT id FROM tbl_visitor_company WHERE visitor_company = '" + comboBox2.Text + "'";

                if (classGlobal.databaseType == "acc")
                {
                    OleDbCommand command1 = new OleDbCommand(queryString, classGlobal.conn);
                    OleDbDataReader reader = command1.ExecuteReader();
                    while (reader.Read())
                    {
                        nExist = Int32.Parse(reader[0].ToString());
                    }
                    reader.Close();
                    command1.Dispose();
                    command1 = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    NpgsqlCommand command1 = new NpgsqlCommand(queryString, classGlobal.connP);
                    NpgsqlDataReader reader = command1.ExecuteReader();
                    while (reader.Read())
                    {
                        nExist = Int32.Parse(reader[0].ToString());
                    }
                    reader.Close();
                    command1.Dispose();
                    command1 = null;
                }
                else
                {

                }

                if (nExist > 0)
                {
                    frmMessageBox f = new frmMessageBox();
                    f.strMessage = "ชื่อบริษัทมีอยู่แล้ว";
                    f.strStatus = "Warning";
                    f.ShowDialog();
                    return;
                }
                else
                {
                    string query = "INSERT INTO tbl_visitor_company (visitor_company) VALUES (?)";

                    if (classGlobal.databaseType == "acc")
                    {
                        OleDbCommand command = new OleDbCommand(query, classGlobal.conn);
                        command.Parameters.Add("@visitor_company", OleDbType.VarChar).Value = comboBox2.Text;
                        command.ExecuteNonQuery();
                        command.Dispose();
                        command = null;
                    }
                    else if (classGlobal.databaseType == "psql")
                    {
                        query = "INSERT INTO tbl_visitor_company (visitor_company) VALUES (@visitor_company)";
                        NpgsqlCommand command = new NpgsqlCommand(query, classGlobal.connP);
                        command.Parameters.AddWithValue("@visitor_company", comboBox2.Text);
                        command.ExecuteNonQuery();
                        command.Dispose();
                        command = null;
                    }
                    else
                    {

                    }

                    currentComboText = comboBox2.Text;
                }
            }

            GET_COMPANY();
            comboBox2.Text = currentComboText;

            frmMessageBox f1 = new frmMessageBox();
            f1.strMessage = "เพิ่มชื่อบริษัทสำเร็จ";
            f1.strStatus = "Information";
            f1.ShowDialog();
           
        }


        private void HandleTextChanged(ComboBox cb, string[] data)
        {
            //var txt = cb.Text;
            //var list = from d in data
            //           where d.ToUpper().StartsWith(cb.Text.ToUpper())
            //           select d;
            //if (list.Count() > 0)
            //{
            //    cb.DataSource = list.ToList();
            //    var sText = cb.Items[0].ToString();
            //    cb.SelectionStart = txt.Length;
            //    cb.SelectionLength = sText.Length - txt.Length;
            //    //cb.DroppedDown = true;                
            //    return;
            //}
            //else
            //{
            //    cb.DroppedDown = false;
            //    cb.SelectionStart = txt.Length;
            //}
        }

 

        private void SEARCH_CATEGORY_MORE_INFO(string tbl)
        {
            FormSetting.FormSearchCategoryInfo f = new FormSetting.FormSearchCategoryInfo(tbl);
            f.ShowDialog(); 



            comboBox1.Text = clsInfo.info_follower;

            //comboBox2.Text = clsInfo.info_visitor_company;
            //comboBox3.Text = clsInfo.info_license_plate;
            //comboBox4.Text = clsInfo.info_vehicle_type;
            //comboBox5.Text = clsInfo.info_visit_to;
            //comboBox6.Text = clsInfo.info_department;
            //comboBox7.Text = clsInfo.info_business_topic;
            //comboBox8.Text = clsInfo.info_place;

            textBox2.Text = clsInfo.info_visitor_company;
            textBox3.Text = clsInfo.info_license_plate;
            textBox4.Text = clsInfo.info_vehicle_type;
            textBox5.Text = clsInfo.info_department;
            textBox6.Text = clsInfo.info_visit_to;
            textBox7.Text = clsInfo.info_business_topic;
            textBox8.Text = clsInfo.info_place;

            textBox1.Text = clsInfo.info_etc; 
        }
        private void button5_Click(object sender, EventArgs e)
        {
            SEARCH_CATEGORY_MORE_INFO("tbl_visitor_company");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SEARCH_CATEGORY_MORE_INFO("tbl_license_plate");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SEARCH_CATEGORY_MORE_INFO("tbl_vehicle_type");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            SEARCH_CATEGORY_MORE_INFO("tbl_department");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            SEARCH_CATEGORY_MORE_INFO("tbl_visit_to");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            SEARCH_CATEGORY_MORE_INFO("tbl_business_topic");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            SEARCH_CATEGORY_MORE_INFO("tbl_place");
        }

        private void button12_Click(object sender, EventArgs e)
        {
            string currentComboText = "";
            if (comboBox4.Text == "")
            {
                frmMessageBox f = new frmMessageBox();
                f.strMessage = "กรุณากรอกชนิดของรถ";
                f.strStatus = "Warning";
                f.ShowDialog();
                return;
            }
            else
            {
                int nExist = -1;
                queryString = "SELECT id FROM tbl_vehicle_type WHERE vehicle_type = '" + comboBox4.Text + "'";

                if (classGlobal.databaseType == "acc")
                {
                    OleDbCommand command1 = new OleDbCommand(queryString, classGlobal.conn);
                    OleDbDataReader reader = command1.ExecuteReader();
                    while (reader.Read())
                    {
                        nExist = Int32.Parse(reader[0].ToString());
                    }
                    reader.Close();
                    command1.Dispose();
                    command1 = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    NpgsqlCommand command1 = new NpgsqlCommand(queryString, classGlobal.connP);
                    NpgsqlDataReader reader = command1.ExecuteReader();
                    while (reader.Read())
                    {
                        nExist = Int32.Parse(reader[0].ToString());
                    }
                    reader.Close();
                    command1.Dispose();
                    command1 = null;
                }
                else
                {

                }

                if (nExist > 0)
                {
                    frmMessageBox f = new frmMessageBox();
                    f.strMessage = "ชนิดของรถมีอยู่แล้ว";
                    f.strStatus = "Warning";
                    f.ShowDialog();
                    return;
                }
                else
                {
                    string query = "INSERT INTO tbl_vehicle_type (vehicle_type) VALUES (@license_plate)";

                    if (classGlobal.databaseType == "acc")
                    {
                        OleDbCommand command = new OleDbCommand(query, classGlobal.conn);
                        command.Parameters.Add("@license_plate", OleDbType.VarChar).Value = comboBox4.Text;
                        command.ExecuteNonQuery();
                        command.Dispose();
                        command = null;
                    }
                    else if (classGlobal.databaseType == "psql")
                    {
                        NpgsqlCommand command = new NpgsqlCommand(query, classGlobal.connP);
                        command.Parameters.AddWithValue("@license_plate",comboBox4.Text);
                        command.ExecuteNonQuery();
                        command.Dispose();
                        command = null;
                    }
                    else
                    {

                    }

                    currentComboText = comboBox4.Text;
                }
            }

            GET_VEHICLE_TYPE();
            comboBox4.Text = currentComboText;

            frmMessageBox f1 = new frmMessageBox();
            f1.strMessage = "เพิ่มชนิดของรถสำเร็จ";
            f1.strStatus = "Information";
            f1.ShowDialog();
        }


        private void comboBox4_TextChanged(object sender, EventArgs e)
        {
            HandleTextChanged(comboBox4, data_vehicle_type);
        }

        private void comboBox6_TextChanged(object sender, EventArgs e)
        {
            HandleTextChanged(comboBox6, data_department);
        }

        private void comboBox5_TextChanged(object sender, EventArgs e)
        {
            HandleTextChanged(comboBox5, data_visitor_to);
        }

        private void comboBox7_TextChanged(object sender, EventArgs e)
        {
            HandleTextChanged(comboBox7, data_topic);
        }

        private void comboBox8_TextChanged(object sender, EventArgs e)
        {
            HandleTextChanged(comboBox8, data_place); 
        }

        private void button13_Click(object sender, EventArgs e)
        {
            string currentComboText = "";
            if (comboBox6.Text == "")
            {
                frmMessageBox f = new frmMessageBox();
                f.strMessage = "กรุณากรอกแผนกที่ติดต่อ";
                f.strStatus = "Warning";
                f.ShowDialog();
                return;
            }
            else
            {
                int nExist = -1;
                queryString = "SELECT id FROM tbl_department WHERE department = '" + comboBox6.Text + "'";

                if (classGlobal.databaseType == "acc")
                {
                    OleDbCommand command1 = new OleDbCommand(queryString, classGlobal.conn);
                    OleDbDataReader reader = command1.ExecuteReader();
                    while (reader.Read())
                    {
                        nExist = Int32.Parse(reader[0].ToString());
                    }
                    reader.Close();
                    command1.Dispose();
                    command1 = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    NpgsqlCommand command1 = new NpgsqlCommand(queryString, classGlobal.connP);
                    NpgsqlDataReader reader = command1.ExecuteReader();
                    while (reader.Read())
                    {
                        nExist = Int32.Parse(reader[0].ToString());
                    }
                    reader.Close();
                    command1.Dispose();
                    command1 = null;
                }
                else
                {

                }

                if (nExist > 0)
                {
                    frmMessageBox f = new frmMessageBox();
                    f.strMessage = "แผนกที่ติดต่อมีอยู่แล้ว";
                    f.strStatus = "Warning";
                    f.ShowDialog();
                    return;
                }
                else
                {
                    string query = "INSERT INTO tbl_department (department) VALUES (?)";

                    if (classGlobal.databaseType == "acc")
                    {
                        OleDbCommand command = new OleDbCommand(query, classGlobal.conn);
                        command.Parameters.Add("@department", OleDbType.VarChar).Value = comboBox6.Text;
                        command.ExecuteNonQuery();
                        command.Dispose();
                        command = null;
                    }
                    else if (classGlobal.databaseType == "psql")
                    {
                        query = "INSERT INTO tbl_department (department) VALUES (@department)";
                        NpgsqlCommand command = new NpgsqlCommand(query, classGlobal.connP);
                        command.Parameters.AddWithValue("@department", comboBox6.Text);
                        command.ExecuteNonQuery();
                        command.Dispose();
                        command = null;
                    }
                    else
                    {

                    }

                    currentComboText = comboBox6.Text;
                }
            }
            
            GET_DEPARTMENT();
            comboBox6.Text = currentComboText;

            frmMessageBox f1 = new frmMessageBox();
            f1.strMessage = "เพิ่มแผนกที่ติดต่อสำเร็จ";
            f1.strStatus = "Information";
            f1.ShowDialog();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            string currentComboText = "";
            if (comboBox5.Text == "")
            {
                frmMessageBox f = new frmMessageBox();
                f.strMessage = "กรุณากรอกผู้รับการติดต่อ";
                f.strStatus = "Warning";
                f.ShowDialog();
                return;
            }
            else
            {
                int nExist = -1;
                queryString = "SELECT id FROM tbl_visit_to WHERE visit_to = '" + comboBox5.Text + "'";

                if (classGlobal.databaseType == "acc")
                {
                    OleDbCommand command1 = new OleDbCommand(queryString, classGlobal.conn);
                    OleDbDataReader reader = command1.ExecuteReader();
                    while (reader.Read())
                    {
                        nExist = Int32.Parse(reader[0].ToString());
                    }
                    reader.Close();
                    command1.Dispose();
                    command1 = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    NpgsqlCommand command1 = new NpgsqlCommand(queryString, classGlobal.connP);
                    NpgsqlDataReader reader = command1.ExecuteReader();
                    while (reader.Read())
                    {
                        nExist = Int32.Parse(reader[0].ToString());
                    }
                    reader.Close();
                    command1.Dispose();
                    command1 = null;
                }
                else
                {

                }
               

                if (nExist > 0)
                {
                    frmMessageBox f = new frmMessageBox();
                    f.strMessage = "ผู้รับการติดต่อมีอยู่แล้ว";
                    f.strStatus = "Warning";
                    f.ShowDialog();
                    return;
                }
                else
                {
                    string query = "INSERT INTO tbl_visit_to (visit_to) VALUES (@visit_to)";

                    if (classGlobal.databaseType == "acc")
                    {
                        OleDbCommand command = new OleDbCommand(query, classGlobal.conn);
                        command.Parameters.Add("@visit_to", OleDbType.VarChar).Value = comboBox5.Text;
                        command.ExecuteNonQuery();
                        command.Dispose();
                        command = null;
                    }
                    else if (classGlobal.databaseType == "psql")
                    {
                        NpgsqlCommand command = new NpgsqlCommand(query, classGlobal.connP);
                        command.Parameters.AddWithValue("@visit_to", comboBox5.Text);
                        command.ExecuteNonQuery();
                        command.Dispose();
                        command = null;
                    }
                    else
                    {

                    }

                    currentComboText = comboBox5.Text;
                }
            }

            GET_VISIT_TO();
            comboBox5.Text = currentComboText;

            frmMessageBox f1 = new frmMessageBox();
            f1.strMessage = "เพิ่มผู้รับการติดต่อสำเร็จ";
            f1.strStatus = "Information";
            f1.ShowDialog();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            string currentComboText = "";
            if (comboBox7.Text == "")
            {
                frmMessageBox f = new frmMessageBox();
                f.strMessage = "กรุณากรอกติดต่อเรื่อง";
                f.strStatus = "Warning";
                f.ShowDialog();
                return;
            }
            else
            {
                int nExist = -1;
                queryString = "SELECT id FROM tbl_business_topic WHERE business_topic = '" + comboBox7.Text + "'";

                if (classGlobal.databaseType == "acc")
                {
                    OleDbCommand command1 = new OleDbCommand(queryString, classGlobal.conn);
                    OleDbDataReader reader = command1.ExecuteReader();
                    while (reader.Read())
                    {
                        nExist = Int32.Parse(reader[0].ToString());
                    }
                    reader.Close();
                    command1.Dispose();
                    command1 = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    NpgsqlCommand command1 = new NpgsqlCommand(queryString, classGlobal.connP);
                    NpgsqlDataReader reader = command1.ExecuteReader();
                    while (reader.Read())
                    {
                        nExist = Int32.Parse(reader[0].ToString());
                    }
                    reader.Close();
                    command1.Dispose();
                    command1 = null;
                }
                else
                {

                }

                if (nExist > 0)
                {
                    frmMessageBox f = new frmMessageBox();
                    f.strMessage = "ติดต่อเรื่องมีอยู่แล้ว";
                    f.strStatus = "Warning";
                    f.ShowDialog();
                    return;
                }
                else
                {
                    string query = "INSERT INTO tbl_business_topic (business_topic) VALUES (@business_topic)";

                    if (classGlobal.databaseType == "acc")
                    {
                        OleDbCommand command = new OleDbCommand(query, classGlobal.conn);
                        command.Parameters.Add("@business_topic", OleDbType.VarChar).Value = comboBox7.Text;
                        command.ExecuteNonQuery();
                        command.Dispose();
                        command = null;
                    }
                    else if (classGlobal.databaseType == "psql")
                    {
                        NpgsqlCommand command = new NpgsqlCommand(query, classGlobal.connP);
                        command.Parameters.AddWithValue("@business_topic", comboBox7.Text);
                        command.ExecuteNonQuery();
                        command.Dispose();
                        command = null;
                    }
                    else
                    {

                    }

                    currentComboText = comboBox7.Text;
                }
            }

            GET_BUSINESS_TOPIC();
            comboBox7.Text = currentComboText;

            frmMessageBox f1 = new frmMessageBox();
            f1.strMessage = "เพิ่มติดต่อเรื่องสำเร็จ";
            f1.strStatus = "Information";
            f1.ShowDialog();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            string currentComboText = "";
            if (comboBox8.Text == "")
            {
                frmMessageBox f = new frmMessageBox();
                f.strMessage = "กรุณากรอกสถานที่ติดต่อ";
                f.strStatus = "Warning";
                f.ShowDialog();
                return;
            }
            else
            {
                int nExist = -1;
                queryString = "SELECT id FROM tbl_place WHERE place = '" + comboBox8.Text + "'";

                if (classGlobal.databaseType == "acc")
                {
                    OleDbCommand command1 = new OleDbCommand(queryString, classGlobal.conn);
                    OleDbDataReader reader = command1.ExecuteReader();
                    while (reader.Read())
                    {
                        nExist = Int32.Parse(reader[0].ToString());
                    }
                    reader.Close();
                    command1.Dispose();
                    command1 = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    NpgsqlCommand command1 = new NpgsqlCommand(queryString, classGlobal.connP);
                    NpgsqlDataReader reader = command1.ExecuteReader();
                    while (reader.Read())
                    {
                        nExist = Int32.Parse(reader[0].ToString());
                    }
                    reader.Close();
                    command1.Dispose();
                    command1 = null;
                }
                else
                {

                }

                if (nExist > 0)
                {
                    frmMessageBox f = new frmMessageBox();
                    f.strMessage = "สถานที่ติดต่อมีอยู่แล้ว";
                    f.strStatus = "Warning";
                    f.ShowDialog();
                    return;
                }
                else
                {
                    string query = "INSERT INTO tbl_place (place) VALUES (@place)";

                    if (classGlobal.databaseType == "acc")
                    {
                        OleDbCommand command = new OleDbCommand(query, classGlobal.conn);
                        command.Parameters.Add("@place", OleDbType.VarChar).Value = comboBox8.Text;
                        command.ExecuteNonQuery();
                        command.Dispose();
                        command = null;
                    }
                    else if (classGlobal.databaseType == "psql")
                    {
                        NpgsqlCommand command = new NpgsqlCommand(query, classGlobal.connP);
                        command.Parameters.AddWithValue("@place", comboBox8.Text);
                        command.ExecuteNonQuery();
                        command.Dispose();
                        command = null;
                    }
                    else
                    {

                    }

                    currentComboText = comboBox8.Text;
                }
            }

            GET_PLACE();
            comboBox8.Text = currentComboText;

            frmMessageBox f1 = new frmMessageBox();
            f1.strMessage = "เพิ่มสถานที่ติดต่อสำเร็จ";
            f1.strStatus = "Information";
            f1.ShowDialog();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            clsInfo.info_follower = comboBox1.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            clsInfo.info_visitor_company = textBox2.Text; 
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            clsInfo.info_license_plate = textBox3.Text;

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

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            clsInfo.info_vehicle_type = textBox4.Text; 
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            clsInfo.info_department = textBox5.Text;
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            clsInfo.info_visit_to = textBox6.Text; 
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            clsInfo.info_business_topic = textBox7.Text;
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            clsInfo.info_place = textBox8.Text;
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            clsInfo.info_etc = textBox1.Text; 
        }

        private void FormMoreInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            var culture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
            var language = InputLanguage.FromCulture(culture);
            InputLanguage.CurrentInputLanguage = language;
        }

        private void TimerDelay_Tick(object sender, EventArgs e)
        {
            timerDelay.Stop();
            this.Invoke(new Action(() =>
            {
                string place = clsInfo.info_place;
                string licensePlate = clsInfo.info_license_plate;                
                ClassData.CHECK_APPOINTMENT(classGlobal.personID, classGlobal.personName, place, licensePlate);
            }
            ));
        }
    }
}
