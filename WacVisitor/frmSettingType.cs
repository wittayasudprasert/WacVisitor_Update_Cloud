using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Npgsql;
using Newtonsoft.Json.Linq;
using System.Collections;
using static WacVisitor.classAnimate;

namespace WacVisitor
{
    public partial class frmSettingType : Form
    {
        #region PlaceHold TextBox
        private const int EM_SETCUEBANNER = 0x1501;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)]string lParam);
        #endregion

        public frmSettingType()
        {
            InitializeComponent();
        }

        string selected_type = "";
        string currentBlackWhite = "";
        private void frmSettingType_Load(object sender, EventArgs e)
        {
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;

            panel5.Left = (panel1.ClientSize .Width - panel5.ClientSize.Width  ) / 2;

            SendMessage(textBox4.Handle, EM_SETCUEBANNER, 0, "เลขประจำตัว");
            SendMessage(textBox5.Handle, EM_SETCUEBANNER, 0, "ชื่อ-สกุล");
            SendMessage(textBox6.Handle, EM_SETCUEBANNER, 0, "เริ่มต้น");
            SendMessage(textBox7.Handle, EM_SETCUEBANNER, 0, "สิ้นสุด");

            panel_visitor_type.Visible = false;
            panel_blacklist.Visible = false;

            AnimateWindow(this.Handle, 100, AnimateWindowFlags.AW_CENTER);

        }

        //++++ SET VISITOR TYPE ++++//
        int type_id = -1;
        ArrayList arrListId = new ArrayList();
        ArrayList arrListTimeId = new ArrayList();
        private void GET_VISITOR_VEHICLE_TYPE(string selected_type) 
        {
            DataTable dt = new DataTable("dt");

            if (classGlobal.databaseType == "acc")
            {
                OleDbDataAdapter adapter;
                if (selected_type == "tbl_type")
                    adapter = new OleDbDataAdapter("SELECT typeid AS ID, typename AS Name FROM tbl_type WHERE typeid <> 0 ORDER BY typeid ASC", classGlobal.conn);
                else
                    adapter = new OleDbDataAdapter("SELECT v_id AS ID, v_nametype AS Name FROM tbl_vehicle ORDER BY v_id ASC", classGlobal.conn);

                adapter.Fill(dt);
                adapter.Dispose();
                adapter = null;
            }
            else if (classGlobal.databaseType == "psql")
            {
                NpgsqlDataAdapter adapter;
                if (selected_type == "tbl_type")
                    adapter = new NpgsqlDataAdapter("SELECT typeid AS ID, typename AS Name FROM tbl_type WHERE typeid <> 0 ORDER BY typeid ASC", classGlobal.connP);
                else
                    adapter = new NpgsqlDataAdapter("SELECT v_id AS ID, v_nametype AS Name FROM tbl_vehicle WHERE v_id <> 0 ORDER BY v_id ASC", classGlobal.connP);

                adapter.Fill(dt);
                adapter.Dispose();
                adapter = null;
            }
            else
            {
                dt.Columns.Add("ID");
                dt.Columns.Add("Name");

                string jsonString;
                if (selected_type == "tbl_type")
                    jsonString = ClassData.GET_METHODE("visitorType");
                else
                    jsonString = ClassData.GET_METHODE("vehicleType");

                if (jsonString != "")
                {
                    JArray jsArray = JArray.Parse(jsonString);
                    int j = 0;
                    foreach (var x in jsArray)
                    {
                        dt.Rows.Add(j, x.ToString());
                        j += 1;
                    }
                }
                else
                {
                    // no data
                }
            }

            if (selected_type == "tbl_type")
                dt.Columns.Add("Charge");

            dvVisitorType.Columns.Clear();
            dvVisitorType.Columns.Add("ID", "ID");

            if (selected_type == "tbl_type")
            {
                dvVisitorType.Columns.Add("VisitorType", "ประเภท Visitor");
                dvVisitorType.Columns.Add("Charge", "คิดเงิน");
            }
            else
            {
                dvVisitorType.Columns.Add("VisitorType", "ชนิดของรถ");
            }


            //++ datagridview header
            dvVisitorType.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 120, 215);
            dvVisitorType.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dvVisitorType.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 24.0F, FontStyle.Bold, GraphicsUnit.Pixel);
            dvVisitorType.EnableHeadersVisualStyles = false;
            dvVisitorType.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dvVisitorType.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            if (selected_type == "tbl_type")
            {
                dvVisitorType.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dvVisitorType.Columns[2].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
 
            //-- datagridview header
            dvVisitorType.RowHeadersVisible = false;
            dvVisitorType.Columns[0].Visible = false;
            dvVisitorType.Rows.Clear();
            dvVisitorType.Refresh();
            dvVisitorType.RowTemplate.MinimumHeight = 35;

            dvVisitorType.Columns[1].Width = (dvVisitorType.Width * 80) / 100;

            if (selected_type == "tbl_type")
            {
                dvVisitorType.Columns[2].HeaderText = "คิดเงิน";
                dvVisitorType.Columns[2].Width = (dvVisitorType.Width * 20) / 100;
            }

            if (selected_type == "tbl_type")
            {
                if (classGlobal.boolCharge == true)
                    dvVisitorType.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                else
                    dvVisitorType.Columns[2].Width = 0;
            }

            if (selected_type == "tbl_type")
            {
                foreach (DataRow reader in dt.Rows)
                {
                    Application.DoEvents();
                    dvVisitorType.Rows.Add(reader.ItemArray[0].ToString(),
                                                reader.ItemArray[1].ToString(),
                                                    CHECK_VISITOR_TYPE_CHARGE(Int32.Parse(reader.ItemArray[0].ToString()), reader.ItemArray[1].ToString()));
                }
            }
            else
            {
                foreach (DataRow reader in dt.Rows)
                {
                    Application.DoEvents();
                    dvVisitorType.Rows.Add(reader.ItemArray[0].ToString(),
                                                reader.ItemArray[1].ToString());
                }
            }
            //---

            dt.Dispose();
            dt = null;

            foreach (DataGridViewColumn c in dvVisitorType.Columns)
            {
                Application.DoEvents();
                c.DefaultCellStyle.Font = new Font("Segoe UI", 22.5F, GraphicsUnit.Pixel);
            }

            if (selected_type == "tbl_type")
            {
                if (classGlobal.boolCharge == false)
                {
                    dvVisitorType.Columns[2].Visible = false;
                }
            }

            dataGridView1.ReadOnly = true;

        }

        private string CHECK_VISITOR_TYPE_CHARGE(int typeid, string typeName)
        {
            string s = "ไม่คิด";
            try
            {
                DataTable dt = new DataTable("dt");

                //tbl_charge_car_park
                if (classGlobal.databaseType == "acc")
                {
                    OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT ID, status FROM tbl_charge_car_park WHERE typeid =" + typeid, classGlobal.conn);
                    adapter.Fill(dt);
                    adapter.Dispose();
                    adapter = null;

                    if (dt.Rows.Count > 0)
                    {
                        string _temp = "";
                        foreach (DataRow dr in dt.Rows)
                        {
                            _temp = _temp + dr["status"].ToString(); 
                        }
                        if (_temp.Replace("N","") == "")
                            dt.Clear(); 
                    }
                }
                else if (classGlobal.databaseType == "psql")
                {
                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter("SELECT ID, status FROM tbl_charge_car_park WHERE typeid =" + typeid, classGlobal.connP);
                    adapter.Fill(dt);
                    adapter.Dispose();
                    adapter = null;

                    if (dt.Rows.Count > 0)
                    {
                        string _temp = "";
                        foreach (DataRow dr in dt.Rows)
                        {
                            _temp = _temp + dr["status"].ToString();
                        }
                        if (_temp.Replace("N", "") == "")
                            dt.Clear();
                    }
                }
                else
                {
                    dt.Columns.Add("ID");
                    int _foundId = 1;
                    JToken jMessage = classGlobal.public_JsonChargePark["visitorType"];
                    foreach (var node in jMessage)
                    {
                        JArray ja = (JArray)node["classname"];
                        foreach (var nodeSub in ja)
                        {
                            if (node["typename"].ToString() == typeName)
                            {
                                if (nodeSub["status"].ToString() == "Y")
                                {
                                    dt.Rows.Add(_foundId);
                                    _foundId += 1;
                                }
                                else
                                {
                                    //dt.Clear();
                                    //break;
                                }
                            }                            
                        }
                    }

                }
              
                if (dt.Rows.Count > 0)
                    s = "คิด";
                else
                    s = "ไม่คิด";

                dt.Dispose();
                dt = null;
            }
            catch
            {
                s = "ไม่คิด";
            }

            return s;
        }
        private void button10_Click(object sender, EventArgs e)
        {
           this.Close();  
        }
       
        private int GetTypeID(string typename)
        {
            try
            {
                DataTable dt = new DataTable("dt");

                if (classGlobal.databaseType == "acc")
                {
                    OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT typeid FROM tbl_type WHERE typename='" + typename + "'", classGlobal.conn);
                    adapter.Fill(dt);
                    adapter.Dispose();
                    adapter = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter("SELECT typeid FROM tbl_type WHERE typename='" + typename + "'", classGlobal.connP);
                    adapter.Fill(dt);
                    adapter.Dispose();
                    adapter = null;
                }
                else
                {

                }

                int n = Int32.Parse(dt.Rows[0][0].ToString());
                dt.Dispose();
                dt = null;
                return n;
            }
            catch
            {
                return -1;
            }
           

        }
        private void button1_Click(object sender, EventArgs e)
        {

            try
            {
                string query = "";
                if (selected_type == "tbl_type")
                    query = "UPDATE tbl_type SET typename = ? WHERE typeid=?";
                else
                    query = "UPDATE tbl_vehicle SET v_nametype = ? WHERE v_id=?";

                if (classGlobal.databaseType == "acc")
                {
                    OleDbCommand command = new OleDbCommand(query, classGlobal.conn);
                    if (selected_type == "tbl_type")
                    {
                        command.Parameters.Add("@typename", OleDbType.VarChar).Value = textBox1.Text;
                        command.Parameters.Add("@typeid", OleDbType.Integer).Value = type_id;
                    }
                    else
                    {
                        command.Parameters.Add("@v_nametype", OleDbType.VarChar).Value = textBox1.Text;
                        command.Parameters.Add("@v_id", OleDbType.Integer).Value = type_id;
                    }
                    
                    command.ExecuteNonQuery();
                    command.Dispose();
                    command = null;

                    if (selected_type == "tbl_type")
                    {
                        query = "UPDATE tbl_charge_car_park SET typename = ? WHERE typeid=?";
                        command = new OleDbCommand(query, classGlobal.conn);
                        command.Parameters.Add("@typename", OleDbType.VarChar).Value = textBox1.Text;
                        command.Parameters.Add("@typeid", OleDbType.Integer).Value = type_id;
                        command.ExecuteNonQuery();
                        command.Dispose();
                        command = null;
                    }
                        
                }
                else if (classGlobal.databaseType == "psql")
                {
                    NpgsqlCommand command = new NpgsqlCommand(query, classGlobal.connP);
                    if (selected_type == "tbl_type")
                    {
                        command.Parameters.AddWithValue("@typename", textBox1.Text);
                        command.Parameters.AddWithValue("@typeid", type_id);
                        command.ExecuteNonQuery();
                        command.Dispose();
                        command = null;
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@v_nametype", textBox1.Text);
                        command.Parameters.AddWithValue("@v_id", type_id);
                        command.ExecuteNonQuery();
                        command.Dispose();
                        command = null;
                    }

                    if (selected_type == "tbl_type")
                    {
                        query = "UPDATE tbl_charge_car_park SET typename = ? WHERE typeid=?";
                        command = new NpgsqlCommand(query, classGlobal.connP);
                        command.Parameters.AddWithValue("@typename", textBox1.Text);
                        command.Parameters.AddWithValue("@typeid", type_id);
                        command.ExecuteNonQuery();
                        command.Dispose();
                        command = null;
                    }
                        
                }
                else
                {
                    if (recentVisitorType.Equals(textBox1.Text))
                    {
                        frmMessageBox f1 = new frmMessageBox();
                        f1.strMessage = "ประเภท visitor มีอยู่แล้ว!";
                        f1.strStatus = "Error";
                        f1.ShowDialog();
                        return;
                    }

                    string s = "";
                    if (selected_type == "tbl_type")
                        s = ClassData.PUT_METHODE("visitorType", recentVisitorType, textBox1.Text);
                    else
                        s = ClassData.PUT_METHODE("vehicleType", recentVisitorType, textBox1.Text);

                    if (s != "200")
                    {
                        frmMessageBox f1 = new frmMessageBox();
                        f1.strMessage = s;
                        f1.strStatus = "Error";
                        f1.ShowDialog();
                    }
                }

                GET_VISITOR_VEHICLE_TYPE(selected_type);
                //MessageBox.Show("สำเร็จ");
                frmMessageBox f = new frmMessageBox();
                f.strMessage = "สำเร็จ";
                f.strStatus = "Information";
                f.ShowDialog(); 
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
           
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (type_id == -1)
                return;

            classGlobal.MsgText = "ต้องการลบประเภท Visitor ที่เลือก?";
            Msg m = new Msg();
            m.ShowDialog();

            string ret = classGlobal.MsgConfirm;
            if (ret == "YES")
            {
                try
                {
                    string query = "";
                    if (selected_type == "tbl_type")
                        query = "DELETE FROM tbl_type WHERE typeid=?";
                    else
                        query = "DELETE FROM tbl_vehicle WHERE v_id=?";

                    if (classGlobal.databaseType == "acc")
                    {
                        OleDbCommand command = new OleDbCommand(query, classGlobal.conn);

                        if (selected_type == "tbl_type")
                            command.Parameters.Add("@typeid", OleDbType.Integer).Value = type_id;
                        else
                            command.Parameters.Add("@v_id", OleDbType.Integer).Value = type_id;

                        command.ExecuteNonQuery();
                        command.Dispose();
                        command = null;

                        GET_VISITOR_VEHICLE_TYPE(selected_type);
                        textBox1.Text = "";

                        if (selected_type == "tbl_type")
                        {
                            //++ เคลียร๋ typeid ที่ถูกลบใน tbl_visitor ให้เป็น 0 (ไม่ระบุ) เพราะ typeid ที่เคยใช้ถูกลบไป
                            query = "UPDATE tbl_visitor SET typeid=0 WHERE typeid=?";
                            command = new OleDbCommand(query, classGlobal.conn);
                            command.Parameters.Add("@typeid", OleDbType.Integer).Value = type_id;
                            command.ExecuteNonQuery();
                            command.Dispose();
                            command = null;

                            //++ ลบรายการใน tbl_charge_car_park ที่ type id ที่เลือกถูกลบ
                            query = "DELETE FROM tbl_charge_car_park WHERE typeid=?";
                            command = new OleDbCommand(query, classGlobal.conn);
                            command.Parameters.Add("@typeid", OleDbType.Integer).Value = type_id;
                            command.ExecuteNonQuery();
                            command.Dispose();
                            command = null;
                        }
                        
                    }
                    else if (classGlobal.databaseType == "psql")
                    {
                        NpgsqlCommand command = new NpgsqlCommand(query, classGlobal.connP);
                        if (selected_type == "tbl_type")
                            command.Parameters.AddWithValue("@typeid", type_id);
                        else
                            command.Parameters.AddWithValue("@v_id", type_id);
                        command.ExecuteNonQuery();
                        command.Dispose();
                        command = null;

                        GET_VISITOR_VEHICLE_TYPE(selected_type);
                        textBox1.Text = "";

                        if (selected_type == "tbl_type")
                        {
                            //++ เคลียร๋ typeid ที่ถูกลบใน tbl_visitor ให้เป็น 0 (ไม่ระบุ) เพราะ typeid ที่เคยใช้ถูกลบไป
                            query = "UPDATE tbl_visitor SET typeid=0 WHERE typeid=?";
                            command = new NpgsqlCommand(query, classGlobal.connP);
                            command.Parameters.AddWithValue("@typeid", type_id);
                            command.ExecuteNonQuery();
                            command.Dispose();
                            command = null;

                            //++ ลบรายการใน tbl_charge_car_park ที่ type id ที่เลือกถูกลบ
                            query = "DELETE FROM tbl_charge_car_park WHERE typeid=?";
                            command = new NpgsqlCommand(query, classGlobal.connP);
                            command.Parameters.AddWithValue("@typeid", type_id);
                            command.ExecuteNonQuery();
                            command.Dispose();
                            command = null;
                        }
                        
                    }
                    else
                    {
                        string s = "";
                        if (selected_type == "tbl_type")
                            s = ClassData.DELETE_METHODE("visitorType", textBox1.Text);
                        else
                            s = ClassData.DELETE_METHODE("vehicleType", textBox1.Text);

                        if (s != "200")
                        {
                            frmMessageBox f1 = new frmMessageBox();
                            f1.strMessage = s;
                            f1.strStatus = "Error";
                            f1.ShowDialog();
                        }

                        GET_VISITOR_VEHICLE_TYPE(selected_type);
                        textBox1.Text = "";
                    }

                    frmMessageBox f = new frmMessageBox();
                    f.strMessage = "สำเร็จ";
                    f.strStatus = "Information";
                    f.ShowDialog(); 

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }

            }

            #region old code
            //DialogResult dialogResult = MessageBox.Show("ต้องการลบประเภท Visitor ที่เลือก?", "ลบประเภท Visitor", MessageBoxButtons.YesNo);
            //if (dialogResult == DialogResult.Yes)
            //{
            //    try
            //    {
            //        string query = "DELETE FROM tbl_type WHERE typeid=?";
            //        OleDbCommand command = new OleDbCommand(query, classGlobal.conn);
            //        command.Parameters.Add("@typeid", OleDbType.Integer).Value = type_id;
            //        command.ExecuteNonQuery();
            //        command.Dispose();
            //        command = null;

            //        GET_VISITOR_TYPE();
            //        MessageBox.Show("สำเร็จ");
            //        textBox1.Text = "";

            //        query = "UPDATE tbl_visitor SET typeid=0 WHERE typeid=?";
            //        command = new OleDbCommand(query, classGlobal.conn);
            //        command.Parameters.Add("@typeid", OleDbType.Integer).Value = type_id;
            //        command.ExecuteNonQuery();
            //        command.Dispose();
            //        command = null;


            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message.ToString());
            //    }
            //}
            //else if (dialogResult == DialogResult.No)
            //{
            //    //do something else
            //}
            #endregion

        }
        private void button3_Click(object sender, EventArgs e)
        {
            string s_Type = textBox1.Text;
            if (s_Type.Replace(" ","") == "")
            {
                frmMessageBox f = new frmMessageBox();
                f.strMessage = "กรุณากรอกข้อมูล";
                f.strStatus = "Error";
                f.ShowDialog();
                return;
            }

            foreach (DataGridViewRow row in dvVisitorType.Rows)
            {
                string curTypeText = row.Cells["VisitorType"].Value.ToString().Replace(" ", "");
                if (s_Type.Replace(" ", "") == curTypeText)
                {
                    frmMessageBox f = new frmMessageBox();
                    f.strMessage = "มีข้อมูลอยู่แล้ว";
                    f.strStatus = "Error";
                    f.ShowDialog();     
                    return;
                }
                
            }


            int new_id = GetMaxID();
            try
            {
                string query = "";

                if (selected_type == "tbl_type")
                    query = "INSERT INTO tbl_type (typeid, typename) VALUES (?, ?)";
                else
                    query = "INSERT INTO tbl_vehicle (v_id, v_nametype) VALUES (?, ?)";

                if (classGlobal.databaseType == "acc")
                {
                    OleDbCommand command = new OleDbCommand(query, classGlobal.conn);
                    if (selected_type == "tbl_type")
                    {
                        command.Parameters.Add("@typeid", OleDbType.Integer).Value = new_id;
                        command.Parameters.Add("@typename", OleDbType.VarChar).Value = textBox1.Text;
                    }
                    else
                    {
                        command.Parameters.Add("@v_id", OleDbType.Integer).Value = new_id;
                        command.Parameters.Add("@v_nametype", OleDbType.VarChar).Value = textBox1.Text;
                    }                                       
                    command.ExecuteNonQuery();
                    command.Dispose();
                    command = null;
                }
                else if (classGlobal.databaseType == "psql")
                {                   
                    if (selected_type == "tbl_type")
                        query = "INSERT INTO tbl_type (typeid, typename) VALUES (@typeid,@typename)";
                    else
                        query = "INSERT INTO tbl_type (v_id, v_nametype) VALUES (@v_id,@v_nametype)";

                    NpgsqlCommand command = new NpgsqlCommand(query, classGlobal.connP);                    
                    if (selected_type == "tbl_type")
                    {
                        command.Parameters.AddWithValue("@typeid", new_id);
                        command.Parameters.AddWithValue("@typename", textBox1.Text);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@v_id", new_id);
                        command.Parameters.AddWithValue("@v_nametype", textBox1.Text);
                    }
                    command.ExecuteNonQuery();
                    command.Dispose();
                    command = null;
                }
                else
                {
                    string s = "";
                    if (selected_type == "tbl_type")
                        s = ClassData.POST_METHODE("visitorType", textBox1.Text);
                    else
                        s = ClassData.POST_METHODE("vehicleType", textBox1.Text);

                    if (s != "200")
                    {
                        frmMessageBox f1 = new frmMessageBox();
                        f1.strMessage = s;
                        f1.strStatus = "Error";
                        f1.ShowDialog();
                    }
                }

                GET_VISITOR_VEHICLE_TYPE(selected_type);
                //MessageBox.Show("สำเร็จ");
                textBox1.Text = "";

                frmMessageBox f = new frmMessageBox();
                f.strMessage = "สำเร็จ";
                f.strStatus = "Information";
                f.ShowDialog();               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        private int GetMaxID()
        {
            int id = 0;
            try
            {
                if (classGlobal.databaseType == "acc")
                {
                    OleDbCommand command;
                    if (selected_type == "tbl_type")
                        command = new OleDbCommand("Select MAX(typeid) From tbl_type", classGlobal.conn);
                    else
                        command = new OleDbCommand("Select MAX(v_id) From tbl_vehicle", classGlobal.conn);

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
                    NpgsqlCommand command;
                    if (selected_type == "tbl_type")
                        command = new NpgsqlCommand("Select MAX(typeid) From tbl_type", classGlobal.connP);
                    else
                        command = new NpgsqlCommand("Select MAX(v_id) From tbl_vehicle", classGlobal.connP);
                    
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

                }
               
                return id;
            }
            catch
            {
                return id;
            }
        }
        //--------------------------//

        //++++ IMPORT BLACKLIST ++++//

        private void GET_BLACK_LIST(string tbName)
        {
            DataTable dt = new DataTable("dt");

            if (classGlobal.databaseType == "acc")
            {
                OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT * FROM " + tbName, classGlobal.conn);
                adapter.Fill(dt);
                adapter.Dispose();
                adapter = null;
            }
            else if (classGlobal.databaseType == "psql")
            {
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter("SELECT * FROM " + tbName, classGlobal.connP);
                adapter.Fill(dt);
                adapter.Dispose();
                adapter = null;
            }
            else
            {

                dt.Columns.Add("ID");
                dt.Columns.Add("CID");
                dt.Columns.Add("Name");
                dt.Columns.Add("startTime");
                dt.Columns.Add("stopTime");

                string jsonString = "";
                arrListId.Clear();
                arrListTimeId.Clear();
                if (classGlobal.strBlackOrWhiteList == "black")
                {
                    jsonString = ClassData.GET_METHODE_BLACK_WHITE_LIST("blacklist");
                }
                else if (classGlobal.strBlackOrWhiteList == "white")
                {
                    jsonString = ClassData.GET_METHODE_BLACK_WHITE_LIST("whitelist");
                }
                if (jsonString == "")
                {
                    //-- no data
                }
                else
                {
                    string BlackWhiteListName = "";
                    string BlackWhiteTimeListName = "";
                    JArray jsArray = JArray.Parse(jsonString);
                    int j = 1;


                    foreach (var x in jsArray)
                    {

                        if (classGlobal.strBlackOrWhiteList == "black")
                        {
                            arrListId.Add(x["blacklistId"]);

                            BlackWhiteListName = "blacklistId";
                            BlackWhiteTimeListName = "blacklistTimeId";

                        }
                        else if (classGlobal.strBlackOrWhiteList == "white")
                        {
                            arrListId.Add(x["whitelistId"]);

                            BlackWhiteListName = "whitelistId";
                            BlackWhiteTimeListName = "whitelistTimeId";
                        }

                        JArray js = (JArray)x["time"];
                        foreach (var jObj in js)
                        {
                            arrListTimeId.Add(x[BlackWhiteListName] + "," + jObj[BlackWhiteTimeListName]);
                            dt.Rows.Add(j, x["citizenId"], x["name"],
                                            classGlobal.CONVERT_UTC_TO_LOCAL(jObj["timeStart"].ToString()), 
                                            classGlobal.CONVERT_UTC_TO_LOCAL(jObj["timeStop"].ToString()));
                            j += 1;
                        } 
                    }
                }
            }

            dataGridView1.Rows.Clear();

            //+++ Header Column
            dataGridView1.RowTemplate.Resizable = DataGridViewTriState.True;
            dataGridView1.RowTemplate.Height = 40;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.Columns[0].Width = 0;
            dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[0].Visible = false;

            dataGridView1.Columns[1].Width = dataGridView1.Width / 4;
            dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[2].Width = dataGridView1.Width / 4;
            dataGridView1.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[3].Width = dataGridView1.Width / 4;
            dataGridView1.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[4].Width = dataGridView1.Width / 4;
            dataGridView1.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;

            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.SystemColors.Highlight;
            dataGridView1.EnableHeadersVisualStyles = false;

            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.HeaderCell.Style.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Pixel);
                col.HeaderCell.Style.ForeColor = Color.White;
            }
            //--

            string[] arrSpace = new string[0] { };
            string[] arrSlash = new string[0] { };
            string sDateStart = "";
            string sDateStop = "";
            int year = 0;
            foreach (DataRow dr in dt.Rows)
            {
                try
                {
                    arrSpace = dr.ItemArray[3].ToString().Split(' ');
                    arrSlash = arrSpace[0].ToString().Split('-');
                    year = Int32.Parse(arrSlash[0]);
                    if (year < 2500)
                    { year = year + 543; }
                    sDateStart = arrSlash[2] + "/" + arrSlash[1] + "/" + year.ToString() + " " + arrSpace[1];
                   
                }
                catch
                {
                    sDateStart = "";
                }

                try
                {
                    arrSpace = dr.ItemArray[4].ToString().Split(' ');
                    arrSlash = arrSpace[0].ToString().Split('-');
                    year = Int32.Parse(arrSlash[0]);
                    if (year < 2500)
                    { year = year + 543; }
                    sDateStop = arrSlash[2] + "/" + arrSlash[1] + "/" + year.ToString() + " " + arrSpace[1];
                }
                catch
                {
                    sDateStop = "";
                }
 
                this.dataGridView1.Rows.Add(Int32.Parse(dr.ItemArray[0].ToString()),
                                                dr.ItemArray[1].ToString(),
                                                    dr.ItemArray[2].ToString(),
                                                        sDateStart,
                                                            sDateStop);
            }

            //++ Set no selected on first record
            dataGridView1.Columns[1].DefaultCellStyle.SelectionForeColor = Color.Black;
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.White;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Blue;
            dataGridView1.BackgroundColor = System.Drawing.SystemColors.HotTrack;
            //--

            dt.Dispose();
            dt = null;

            foreach (DataGridViewColumn c in dataGridView1.Columns)
            {
                c.DefaultCellStyle.Font = new Font("Segoe UI", 16F, FontStyle.Regular | FontStyle.Regular, GraphicsUnit.Pixel);       
            }

            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
        }
        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Excel Files|*.xlsx";
            //dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop).ToString();
            dialog.RestoreDirectory = true;
            dialog.Title = "กรุณาเลือกไฟล์เพื่อนำเข้ารายการ " + currentBlackWhite.Replace("tbl_","").ToUpper();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = dialog.FileName;
                DataTable dt = GetDataTableFromExcel(fileName, true);
                INSERT_TO_BLACKLIST_TABLE(dt);
            }   
        }

        private DataTable GetDataTableFromExcel(string path, bool hasHeader = true)
        {
            using (var pck = new OfficeOpenXml.ExcelPackage())
            {
                using (var stream = File.OpenRead(path))
                {
                    pck.Load(stream);
                }
                var ws = pck.Workbook.Worksheets.First();
                DataTable tbl = new DataTable();
                foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                {
                    tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {0}", firstRowCell.Start.Column));
                }
                var startRow = hasHeader ? 2 : 1;
                for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    DataRow row = tbl.Rows.Add();
                    foreach (var cell in wsRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                }
                return tbl;
            }
        }
        private void INSERT_TO_BLACKLIST_TABLE(DataTable dt)
        {
            if (dt.Rows.Count > 0)  // id    name   start  stop  (22/11/2561 00:00:00)
            {
                try
                {
                    string query = "";                    

                    string[] arrSpace = new string[0] { };
                    string[] arrSlash = new string[0] { };
                    string sDateStart = "";
                    string sDateStop = "";
                    int year = 0;
                    foreach (DataRow dr in dt.Rows)
                    {
                        Application.DoEvents();
                        System.Threading.Thread.Sleep(10);
   
                        arrSpace = dr.ItemArray[2].ToString().Split(' ');
                        arrSlash = arrSpace[0].ToString().Split('/');
                        year = Int32.Parse(arrSlash[2]);
                        if (year > 2500)
                        { year = year - 543;  }
                        sDateStart = year.ToString() + "-" + arrSlash[1] + "-" + arrSlash[0] + " " + arrSpace[1];

                        arrSpace = dr.ItemArray[3].ToString().Split(' ');
                        arrSlash = arrSpace[0].ToString().Split('/');
                        year = Int32.Parse(arrSlash[2]);
                        if (year > 2500)
                        { year = year - 543; }
                        sDateStop = year.ToString() + "-" + arrSlash[1] + "-" + arrSlash[0] + " " + arrSpace[1];

                        query = String.Format ("INSERT INTO " + currentBlackWhite + " (personal_number, fullname, start, stop) VALUES ('{0}','{1}','{2}','{3}')",
                                                    dr.ItemArray[0].ToString(), dr.ItemArray[1].ToString(), sDateStart, sDateStop);

                        if (classGlobal.databaseType == "acc")
                        {
                            OleDbCommand cmd = new OleDbCommand(query, classGlobal.conn);
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                            cmd = null;
                        }
                        else if (classGlobal.databaseType == "psql")
                        {
                            NpgsqlCommand cmd = new NpgsqlCommand(query, classGlobal.connP);
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                            cmd = null;
                        }
                        else
                        {
                            //string filename = currentBlackWhite.Replace("tbl_","") + ".txt";
                            //using (StreamWriter sw = File.AppendText(@"json/" + filename))
                            //{
                            //    sDateStart = classGlobal.CONVERT_LOCAL_TO_UTC(sDateStart);
                            //    sDateStop = classGlobal.CONVERT_LOCAL_TO_UTC(sDateStop); 
                            //    sw.WriteLine(dr.ItemArray[0].ToString() + "," + dr.ItemArray[1].ToString() + "," + sDateStart + "," + sDateStop);
                            //}	

                            string s = "";
                            if (classGlobal.strBlackOrWhiteList == "black")
                            {
                                sDateStart = classGlobal.CONVERT_LOCAL_TO_UTC(sDateStart);
                                sDateStop = classGlobal.CONVERT_LOCAL_TO_UTC(sDateStop);
                                s = ClassData.POST_METHODE_BLACK_WHITE_LIST("blacklist", dr.ItemArray[0].ToString(), dr.ItemArray[1].ToString(), sDateStart, sDateStop);
                            }
                            else if (classGlobal.strBlackOrWhiteList == "white")
                            {
                                sDateStart = classGlobal.CONVERT_LOCAL_TO_UTC(sDateStart);
                                sDateStop = classGlobal.CONVERT_LOCAL_TO_UTC(sDateStop);
                                s = ClassData.POST_METHODE_BLACK_WHITE_LIST("whitelist", dr.ItemArray[0].ToString(), dr.ItemArray[1].ToString(), sDateStart, sDateStop);
                            }
                        }
                        
                    }
                }
                catch
                {

                }

                GET_BLACK_LIST(currentBlackWhite);

            }


        }


        int int_Blacklist = -1;
        int selectedIndexOfObjectId = -1;
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedIndexOfObjectId = -1;

            if (e.RowIndex < 0) { return; }
            try
            {
                int_Blacklist = Int32.Parse(dataGridView1.Rows[e.RowIndex].Cells[0].FormattedValue.ToString());
                string personalNo = dataGridView1.Rows[e.RowIndex].Cells[1].FormattedValue.ToString();
                string fullname = dataGridView1.Rows[e.RowIndex].Cells[2].FormattedValue.ToString();
                string start = dataGridView1.Rows[e.RowIndex].Cells[3].FormattedValue.ToString();
                string stop = dataGridView1.Rows[e.RowIndex].Cells[4].FormattedValue.ToString();

                textBox4.Text = personalNo;
                textBox5.Text = fullname;
                textBox6.Text = start;
                textBox7.Text = stop;

                selectedIndexOfObjectId = e.RowIndex;
            }
            catch
            {
                textBox4.Text = "";
                textBox5.Text = "";
                textBox6.Text = "";
                textBox7.Text = "";
                selectedIndexOfObjectId = -1;
            }
            

        }

        private void button6_Click(object sender, EventArgs e)
        {
             classGlobal.MsgText = "ต้องการลบรายการที่เลือก?";
             Msg m = new Msg();
             m.ShowDialog();
             string ret = classGlobal.MsgConfirm;

             //DialogResult dialogResult = MessageBox.Show("ต้องการลบรายการที่เลือก?", "ลบรายการ", MessageBoxButtons.YesNo);
             if (ret == "YES") //if (dialogResult == DialogResult.Yes)
             {
                 try
                 {
                     if (int_Blacklist > 0)
                     {
                         string query = String.Format("DELETE FROM " + currentBlackWhite + " WHERE ID = {0}", int_Blacklist);

                         if (classGlobal.databaseType == "acc")
                         {
                             OleDbCommand cmd = new OleDbCommand(query, classGlobal.conn);
                             cmd.ExecuteNonQuery();
                             cmd.Dispose();
                             cmd = null;
                         }
                         else if (classGlobal.databaseType == "psql")
                         {
                             NpgsqlCommand cmd = new NpgsqlCommand(query, classGlobal.connP);
                             cmd.ExecuteNonQuery();
                             cmd.Dispose();
                             cmd = null;
                         }
                         else
                         {
                             //string objectId = arrListId[selectedIndexOfObjectId].ToString();
                             string objectId = "";
                             string objectTimeId = "";
                             string[] sub = arrListTimeId[selectedIndexOfObjectId].ToString().Split(',') ;
                             objectId = sub[0];
                             objectTimeId = sub[1];

                             string s = "";
                             if (classGlobal.strBlackOrWhiteList == "black")
                             {
                                 s = ClassData.DELETE_METHODE_BLACK_WHITE_LIST("blacklist", "blacklistId", "blacklistTimeId", objectId, objectTimeId);
                             }
                             else if (classGlobal.strBlackOrWhiteList == "white")
                             {
                                 s = ClassData.DELETE_METHODE_BLACK_WHITE_LIST("whitelist", "whitelistId", "whitelistTimeId", objectId, objectTimeId);
                             }
                             if (s != "200")
                             {
                                 frmMessageBox f1 = new frmMessageBox();
                                 f1.strMessage = s;
                                 f1.strStatus = "Error";
                                 f1.ShowDialog();
                                 return;
                             }
                         }

                         GET_BLACK_LIST(currentBlackWhite);
                         //MessageBox.Show("สำเร็จ");
                         frmMessageBox f = new frmMessageBox();
                         f.strMessage = "สำเร็จ ";
                         f.strStatus = "Information";
                         f.ShowDialog();
                     }
                 }
                 catch
                 {
                     //
                 }
             }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox4.Text == "") { return; }
            //if (textBox5.Text == "") { return; }  // ชื่อ-สกุล
            if (textBox6.Text == "") { return; }
            if (textBox7.Text == "") { return; }

            try
            {
                string[] arrSpace = new string[0] { };
                string[] arrSlash = new string[0] { };
                string sDateStart = "";
                string sDateStop = "";
                int year = 0;

                arrSpace = textBox6.Text.Split(' ');
                arrSlash = arrSpace[0].ToString().Split('/');
                year = Int32.Parse(arrSlash[2]);
                if (year > 2500)
                { year = year - 543; }
                sDateStart = year.ToString() + "-" + arrSlash[1] + "-" + arrSlash[0] + " " + arrSpace[1];

                arrSpace = textBox7.Text.Split(' ');
                arrSlash = arrSpace[0].ToString().Split('/');
                year = Int32.Parse(arrSlash[2]);
                if (year > 2500)
                { year = year - 543; }
                sDateStop = year.ToString() + "-" + arrSlash[1] + "-" + arrSlash[0] + " " + arrSpace[1];


                string query = String.Format("INSERT INTO " + currentBlackWhite + " (personal_number, fullname, start, stop) VALUES ('{0}','{1}','{2}','{3}')",
                                                   textBox4.Text, textBox5.Text, sDateStart, sDateStop);

                if (classGlobal.databaseType == "acc")
                {
                    OleDbCommand cmd = new OleDbCommand(query, classGlobal.conn);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    cmd = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    NpgsqlCommand cmd = new NpgsqlCommand(query, classGlobal.connP);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    cmd = null;
                }
                else
                {

                    string s = "";
                    if (classGlobal.strBlackOrWhiteList == "black")
                    {
                        sDateStart = classGlobal.CONVERT_LOCAL_TO_UTC(sDateStart);
                        sDateStop = classGlobal.CONVERT_LOCAL_TO_UTC(sDateStop); 
                        s = ClassData.POST_METHODE_BLACK_WHITE_LIST("blacklist", textBox4.Text, textBox5.Text, sDateStart, sDateStop);
                    }
                    else if (classGlobal.strBlackOrWhiteList == "white")
                    {
                        sDateStart = classGlobal.CONVERT_LOCAL_TO_UTC(sDateStart);
                        sDateStop = classGlobal.CONVERT_LOCAL_TO_UTC(sDateStop); 
                        s = ClassData.POST_METHODE_BLACK_WHITE_LIST("whitelist", textBox4.Text, textBox5.Text, sDateStart, sDateStop);
                    }
                    if (s != "200")
                    {
                        frmMessageBox f1 = new frmMessageBox();
                        f1.strMessage = s;
                        f1.strStatus = "Error";
                        f1.ShowDialog();
                    }
                }
                
                GET_BLACK_LIST(currentBlackWhite);
                //MessageBox.Show("สำเร็จ");
                frmMessageBox f = new frmMessageBox();
                f.strMessage = "สำเร็จ ";
                f.strStatus = "Information";
                f.ShowDialog();
            }
            catch
            {
                //
            }
            
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("th-TH");
            textBox6.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm" + ":" + "00");
            textBox7.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm" + ":" + "59");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            panel_visitor_type.Visible = true;
            panel_blacklist.Visible = false;

            panel_visitor_type.Width = this.ClientSize.Width;  
            //dvVisitorType.Width = panel_visitor_type.Width - 100;  //--
            dvVisitorType.Left = (this.ClientSize.Width - dvVisitorType.ClientSize.Width) / 2;  
            panel_visitor_type.Left = (this.ClientSize.Width - panel_visitor_type.ClientSize.Width) / 2;
            panelTextBoxButton.Left = (this.ClientSize.Width - panelTextBoxButton.ClientSize.Width) / 2;


            panel_visitor_type.Visible = false;
            panel_blacklist.Visible = false;
            frm_visitor_vehicle_type f = new frm_visitor_vehicle_type();
            f.ShowDialog();

            if (f.selected_type == "")
            {
                selected_type = "";
                panel_visitor_type.Visible = false;
                panel_blacklist.Visible = false;
                return;
            }

            panel_visitor_type.Visible = true;
            panel_blacklist.Visible = false;

            selected_type = f.selected_type;
            textBox1.Text = ""; 
            GET_VISITOR_VEHICLE_TYPE(selected_type);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            currentBlackWhite = "tbl_blacklist";
            classGlobal.strBlackOrWhiteList = "black";

            linkLabel1.Text = "[ กำหนดวันเวลาให้ Blacklist ]"; 

            panel_visitor_type.Visible = false;
            panel_blacklist.Visible = true;
            panel_blacklist.Left = (this.ClientSize.Width - panel_blacklist.ClientSize.Width) / 2;

            GET_BLACK_LIST(currentBlackWhite);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //
        }


        private void frmSettingType_Activated(object sender, EventArgs e)
        {
            //
        }

        string recentVisitorType = "";
        private void dvVisitorType_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            try
            {
                type_id = Int32.Parse(dvVisitorType.Rows[e.RowIndex].Cells[0].FormattedValue.ToString());
                textBox1.Text = dvVisitorType.Rows[e.RowIndex].Cells[1].FormattedValue.ToString();
                recentVisitorType = dvVisitorType.Rows[e.RowIndex].Cells[1].FormattedValue.ToString();
            }
            catch
            {
              //
            }
        }

        private void dvVisitorType_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            try
            {
                type_id = Int32.Parse(dvVisitorType.Rows[e.RowIndex].Cells[0].FormattedValue.ToString());
                textBox1.Text = dvVisitorType.Rows[e.RowIndex].Cells[1].FormattedValue.ToString();

                if (classGlobal.boolCharge == true)
                {
                    FormSetting.SettingChargeCarPark f = new FormSetting.SettingChargeCarPark(type_id, textBox1.Text);
                    f.ShowDialog();

                    GET_VISITOR_VEHICLE_TYPE(selected_type);
                }
               
            }
            catch(Exception ex)
            {
                Console.Write(ex.Message.ToString()); 
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            currentBlackWhite = "tbl_whitelist";
            classGlobal.strBlackOrWhiteList = "white";

            linkLabel1.Text = "[ กำหนดวันเวลาให้ Whitelist ]"; 

            panel_visitor_type.Visible = false;
            panel_blacklist.Visible = true;
            panel_blacklist.Left = (this.ClientSize.Width - panel_blacklist.ClientSize.Width) / 2;

            GET_BLACK_LIST("tbl_whitelist");
        }



    }
}
