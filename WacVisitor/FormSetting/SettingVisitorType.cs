using Newtonsoft.Json.Linq;
using Npgsql;
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

namespace WacVisitor.FormSetting
{
    public partial class SettingVisitorType : Form
    {
        public SettingVisitorType()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SettingVisitorType_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;


            panel2.Left = ((this.Width / 2) - (panel2.Width / 2));

            listBox1.Left = ((panel2.Width / 2) - (listBox1.Width / 2));

            listBox2.Width = 0;

            GET_VISITOR_TYPE();
        }

        private void GET_VISITOR_TYPE()
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
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            foreach (DataRow reader in dt.Rows)
            {
                listBox1.Items.Add(reader.ItemArray[1].ToString());
                listBox2.Items.Add(reader.ItemArray[0].ToString());   
            }
            //--
            dt.Dispose();
            dt = null;

        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                frmMessageBox f = new frmMessageBox();
                f.strMessage = "กรุณาเลือกข้อมูลประเภท Visitor";
                f.strStatus = "Warning";
                f.ShowDialog();
                return;
            }

            classGlobal.MsgText = "ต้องการลบรายการที่เลือก?";
            Msg m = new Msg();
            m.ShowDialog();


            string ret = classGlobal.MsgConfirm;
            if (ret == "YES")
            {
                int _typeID = Int32.Parse(listBox2.Items[lstSelectedIndex].ToString());

                string query = "DELETE FROM tbl_type WHERE typeid=" + _typeID;
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
                    string s = ClassData.DELETE_METHODE("visitorType", listBox1.SelectedItem.ToString());
                    if (s != "200")
                    {
                        frmMessageBox f = new frmMessageBox();
                        f.strMessage = s;
                        f.strStatus = "Error";
                        f.ShowDialog();
                    }
                }

                GET_VISITOR_TYPE();
                textBox2.Text = "";

                //++ เคลียร๋ typeid ที่ถูกลบใน tbl_visitor ให้เป็น 0 (ไม่ระบุ) เพราะ typeid ที่เคยใช้ถูกลบไป
                query = "UPDATE tbl_visitor SET typeid=0 WHERE typeid=?";
                if (classGlobal.databaseType == "acc")
                {
                    OleDbCommand command = new OleDbCommand(query, classGlobal.conn);
                    command = new OleDbCommand(query, classGlobal.conn);
                    command.Parameters.Add("@typeid", OleDbType.Integer).Value = _typeID;
                    command.ExecuteNonQuery();
                    command.Dispose();
                    command = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    NpgsqlCommand command = new NpgsqlCommand(query, classGlobal.connP);
                    command = new NpgsqlCommand(query, classGlobal.connP);
                    //command.Parameters.Add("@typeid", OleDbType.Integer).Value = _typeID;
                    command.Parameters.AddWithValue("@typeid", _typeID);
                    command.ExecuteNonQuery();
                    command.Dispose();
                    command = null;
                }
                else
                {
                    //-- do nothing
                }

                //++ ลบรายการใน tbl_charge_car_park ที่ type id ที่เลือกถูกลบ
                query = "DELETE FROM tbl_charge_car_park WHERE typeid=?";
                if (classGlobal.databaseType == "acc")
                {
                    OleDbCommand command = new OleDbCommand(query, classGlobal.conn);
                    command = new OleDbCommand(query, classGlobal.conn);
                    command.Parameters.Add("@typeid", OleDbType.Integer).Value = _typeID;
                    command.ExecuteNonQuery();
                    command.Dispose();
                    command = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    NpgsqlCommand command = new NpgsqlCommand(query, classGlobal.connP);
                    command = new NpgsqlCommand(query, classGlobal.connP);
                    //command.Parameters.Add("@typeid", OleDbType.Integer).Value = _typeID;
                    command.Parameters.AddWithValue("@typeid", _typeID);  
                    command.ExecuteNonQuery();
                    command.Dispose();
                    command = null;
                }
                else
                {
                    //-- do nothing
                }

                MessageBoxSuccess();
                return;
            }
        }

        private void MessageBoxSuccess()
        {
            //frmMessageBox f = new frmMessageBox();
            //f.strMessage = "สำเร็จ";
            //f.strStatus = "Information";
            //f.ShowDialog();
        }
        private void button1_Click(object sender, EventArgs e)
        {

            string s_Type = textBox2.Text;
            if (s_Type.Replace(" ", "") == "")
            {
                frmMessageBox f = new frmMessageBox();
                f.strMessage = "กรุณากรอกข้อมูล";
                f.strStatus = "Error";
                f.ShowDialog();
                return;
            }

            for (int i = 0; i < listBox1.Items.Count; i++ )
            {
                string curTypeText = listBox1.Items[i].ToString().Replace(" ", "");
                if (s_Type.Replace(" ", "") == curTypeText)
                {
                    frmMessageBox f = new frmMessageBox();
                    f.strMessage = "มีข้อมูลอยู่แล้ว";
                    f.strStatus = "Error";
                    f.ShowDialog();
                    return;
                }
            }

            if (textBox2.Text == "")
            {
                frmMessageBox f = new frmMessageBox();
                f.strMessage = "กรุณากรอกข้อมูลประเภท Visitor";
                f.strStatus = "Warning";
                f.ShowDialog();
                return;
            }

            classGlobal.MsgText = "ต้องการเพิ่มประเภท Visitor?";
            Msg m = new Msg();
            m.ShowDialog();
            string ret = classGlobal.MsgConfirm;
            //string ret = "YES";
            if (ret == "YES")
            {
                int new_id = GetMaxID();
                try
                {
                    string query = "INSERT INTO tbl_type (typeid, typename) VALUES (@typeid,@typename)";

                    if (classGlobal.databaseType == "acc")
                    {
                        OleDbCommand command = new OleDbCommand(query, classGlobal.conn);
                        command.Parameters.Add("@typeid", OleDbType.Integer).Value = new_id;
                        command.Parameters.Add("@typename", OleDbType.VarChar).Value = textBox2.Text;
                        command.ExecuteNonQuery();
                        command.Dispose();
                        command = null;
                    }
                    else if (classGlobal.databaseType == "psql")
                    {
                        query = "INSERT INTO tbl_type (typeid, typename) VALUES (@typeid,@typename)";
                        NpgsqlCommand command = new NpgsqlCommand(query, classGlobal.connP);
                        command.Parameters.AddWithValue("@typeid",new_id);
                        command.Parameters.AddWithValue("@typename",textBox2.Text);
                        command.ExecuteNonQuery();
                        command.Dispose();
                        command = null;
                    }
                    else
                    {
                        //using (StreamWriter sw = File.AppendText(@"json/visitortype.txt"))
                        //{
                        //    sw.WriteLine(textBox2.Text);
                        //}
                        string s = ClassData.POST_METHODE("visitorType", textBox2.Text);
                        if (s != "200")
                        {
                            frmMessageBox f = new frmMessageBox();
                            f.strMessage = s;
                            f.strStatus = "Error";
                            f.ShowDialog();
                        }
                    }
                   

                    GET_VISITOR_TYPE();
                    textBox2.Text = "";

                    MessageBoxSuccess();
                    return;


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }

        }
        private int GetMaxID()
        {
            int id = 0;
            try
            {
                if (classGlobal.databaseType == "acc")
                {
                    OleDbCommand command = new OleDbCommand("Select MAX(typeid) from tbl_type", classGlobal.conn);
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
                    NpgsqlCommand command = new NpgsqlCommand("Select MAX(typeid) from tbl_type", classGlobal.connP);
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

        int lstSelectedIndex = -1;
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                lstSelectedIndex = listBox1.SelectedIndex;
                textBox2.Text = listBox1.SelectedItem.ToString();
            }
            catch
            {
                lstSelectedIndex = -1;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                frmMessageBox f = new frmMessageBox();
                f.strMessage = "กรุณาเลือกข้อมูลประเภท Visitor";
                f.strStatus = "Warning";
                f.ShowDialog();
                return;
            }

            classGlobal.MsgText = "ต้องการแก้ไขรายการที่เลือก?";
            Msg m = new Msg();
            m.ShowDialog();

            string ret = classGlobal.MsgConfirm;
            if (ret == "YES")
            {
                int _typeID = Int32.Parse(listBox2.Items[lstSelectedIndex].ToString());

                string query = "UPDATE tbl_type SET typename = '" + textBox2.Text + "' WHERE typeid=" + _typeID;
                
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

                    if (listBox1.SelectedItem.ToString().Equals(textBox2.Text))
                    {
                        frmMessageBox f1 = new frmMessageBox();
                        f1.strMessage = "ประเภท visitor มีอยู่แล้ว!";
                        f1.strStatus = "Error";
                        f1.ShowDialog();
                        return;
                    }

                    string s = ClassData.PUT_METHODE("visitorType", listBox1.SelectedItem.ToString(), textBox2.Text);
                    if (s != "200")
                    {
                        frmMessageBox f = new frmMessageBox();
                        f.strMessage = s;
                        f.strStatus = "Error";
                        f.ShowDialog();
                    }
                }

                GET_VISITOR_TYPE();
                textBox2.Text = "";

                MessageBoxSuccess();
                return;
            }           
        }
    }
}
