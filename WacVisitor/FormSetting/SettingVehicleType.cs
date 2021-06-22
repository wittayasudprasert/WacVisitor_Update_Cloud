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
using Newtonsoft.Json.Linq;

namespace WacVisitor.FormSetting
{
    public partial class SettingVehicleType : Form
    {
        public SettingVehicleType()
        {
            InitializeComponent();
        }

        int lstSelectedIndex = -1;
        private void SettingVehicleType_Load(object sender, EventArgs e)
        {
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            panel1.Left = (this.Width - panel1.ClientSize.Width) / 2;
            //panel1.Top = (this.Height - panel1.ClientSize.Height) / 2;

            //listBox2.Width = 0;
            linkLabel1.Left = (this.Width - linkLabel1.ClientSize.Width) / 2;
            pictureBox1.Left = linkLabel1.Left + linkLabel1.Width;
            if (classGlobal.userId != "")
            {
                linkLabel1.Visible = false;
                pictureBox1.Visible = false;
            }


            GET_LISTS();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void GET_LISTS()
        {
            DataTable dt = new DataTable("dt");

            if (classGlobal.databaseType == "acc")
            {
                OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT id, vehicle_type FROM tbl_vehicle_type ORDER BY id ASC", classGlobal.conn);
                adapter.Fill(dt);
                adapter.Dispose();
                adapter = null;
            }
            else if (classGlobal.databaseType == "psql")
            {
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter("SELECT id, vehicle_type FROM tbl_vehicle_type ORDER BY id ASC", classGlobal.connP);
                adapter.Fill(dt);
                adapter.Dispose();
                adapter = null;
            }
            else
            {
                dt.Columns.Add("id");
                dt.Columns.Add("vehicle_type");

                string jsonString = ClassData.GET_METHODE("vehicleType");
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
            foreach (DataRow reader in dt.Rows)
            {
                listBox1.Items.Add(reader.ItemArray[1].ToString());
            }
            //--
            dt.Dispose();
            dt = null;

        }

        private void MessageBoxSuccess()
        {
            //frmMessageBox f = new frmMessageBox();
            //f.strMessage = "สำเร็จ";
            //f.strStatus = "Information";
            //f.ShowDialog();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                frmMessageBox f = new frmMessageBox();
                f.strMessage = "กรุณากรอกชนิดรถ";
                f.strStatus = "Warning";
                f.ShowDialog();
                return;
            }

            classGlobal.MsgText = "ต้องการเพิ่มชนิดรถ?";
            Msg m = new Msg();
            m.ShowDialog();
            string ret = classGlobal.MsgConfirm;
            //string ret = "YES";
            if (ret == "YES")
            {
                try
                {
                    string query = "INSERT INTO tbl_vehicle_type (vehicle_type) VALUES (@vehicle_type)";
                    if (classGlobal.databaseType == "acc")
                    {
                        OleDbCommand command = new OleDbCommand(query, classGlobal.conn);
                        command.Parameters.Add("@vehicle_type", OleDbType.VarChar).Value = textBox2.Text;
                        command.ExecuteNonQuery();
                        command.Dispose();
                        command = null;
                    }
                    else if (classGlobal.databaseType == "psql")
                    {
                        NpgsqlCommand command = new NpgsqlCommand(query, classGlobal.connP);
                        command.Parameters.AddWithValue("@vehicle_type", textBox2.Text);
                        command.ExecuteNonQuery();
                        command.Dispose();
                        command = null;
                    }
                    else
                    {
                        string s = ClassData.POST_METHODE("vehicleType", textBox2.Text);
                        if (s != "200")
                        {
                            frmMessageBox f = new frmMessageBox();
                            f.strMessage = s;
                            f.strStatus = "Error";
                            f.ShowDialog();
                        }
                    }

                    GET_LISTS();
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                frmMessageBox f = new frmMessageBox();
                f.strMessage = "กรุณาเลือกชนิดรถ";
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
                lstSelectedIndex = GET_ID(listBox1.SelectedItem.ToString());

                string query = "DELETE FROM tbl_vehicle_type WHERE id=" + lstSelectedIndex;

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

                    string s = ClassData.DELETE_METHODE("vehicleType", textBox2.Text);
                    if (s != "200")
                    {
                        frmMessageBox f = new frmMessageBox();
                        f.strMessage = s;
                        f.strStatus = "Error";
                        f.ShowDialog();
                    }
                }

                GET_LISTS();
                textBox2.Text = "";

                MessageBoxSuccess();
                return;
            }
        }

        private int GET_ID(string str)
        {
            int ret = -1;
            try
            {
                DataTable dt = new DataTable("dt");
                if (classGlobal.databaseType == "acc")
                {
                    OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT id FROM tbl_vehicle_type WHERE vehicle_type ='" + str + "'", classGlobal.conn);
                    adapter.Fill(dt);
                    adapter.Dispose();
                    adapter = null;
                    ret = Int32.Parse(dt.Rows[0].ItemArray[0].ToString());
                    dt.Dispose();
                    dt = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter("SELECT id FROM tbl_vehicle_type WHERE vehicle_type ='" + str + "'", classGlobal.connP);
                    adapter.Fill(dt);
                    adapter.Dispose();
                    adapter = null;
                    ret = Int32.Parse(dt.Rows[0].ItemArray[0].ToString());
                    dt.Dispose();
                    dt = null;
                }
                else
                {

                }
            }
            catch
            {
                ret = -1;
            }

            return ret;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                frmMessageBox f = new frmMessageBox();
                f.strMessage = "กรุณาเลือกชนิดรถ";
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
                lstSelectedIndex = GET_ID(listBox1.SelectedItem.ToString());

                string query = "UPDATE tbl_vehicle_type SET vehicle_type = '" + textBox2.Text + "' WHERE id=" + lstSelectedIndex;
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
                    string s = ClassData.PUT_METHODE("vehicleType", listBox1.SelectedItem.ToString(), textBox2.Text);
                    if (s != "200")
                    {
                        frmMessageBox f = new frmMessageBox();
                        f.strMessage = s;
                        f.strStatus = "Error";
                        f.ShowDialog();
                    }
                }

                GET_LISTS();
                textBox2.Text = "";

                MessageBoxSuccess();
                return;
            } 
        }

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

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Excel Files|*.xlsx";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop).ToString();
            dialog.Title = "กรุณาเลือกไฟล์เพื่อนำเข้ารายการ";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = dialog.FileName;
                DataTable dt = classGlobal.GetDataFromXLSX(fileName, false);

                if (dt.Rows.Count > 0)
                {
                    DataTable dtExist = new DataTable("dtExist");

                    if (classGlobal.databaseType == "acc")
                    {
                        OleDbDataAdapter adapter = new OleDbDataAdapter("SELECT id, vehicle_type FROM tbl_vehicle_type ORDER BY id ASC", classGlobal.conn);
                        adapter.Fill(dtExist);
                        adapter.Dispose();
                        adapter = null;
                    }
                    else if (classGlobal.databaseType == "psql")
                    {
                        NpgsqlDataAdapter adapter = new NpgsqlDataAdapter("SELECT id, vehicle_type FROM tbl_vehicle_type ORDER BY id ASC", classGlobal.connP);
                        adapter.Fill(dtExist);
                        adapter.Dispose();
                        adapter = null;
                    }
                    else
                    {
                        //-
                    }

                    foreach (DataRow dr1 in dtExist.Rows)
                    {
                        for (int i = dt.Rows.Count - 1; i >= 0; i--)
                        {
                            DataRow dr = dt.Rows[i];
                            if (dr1[1].ToString() == dr[0].ToString())
                                dr.Delete();
                        }
                    }
                    dt.AcceptChanges();

                    try
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            string query = "INSERT INTO tbl_vehicle_type (vehicle_type) VALUES (@vehicle_type)";
                            if (classGlobal.databaseType == "acc")
                            {
                                OleDbCommand command = new OleDbCommand(query, classGlobal.conn);
                                command.Parameters.Add("@vehicle_type", OleDbType.VarChar).Value = dr[0].ToString();
                                command.ExecuteNonQuery();
                                command.Dispose();
                                command = null;
                            }
                            else if (classGlobal.databaseType == "psql")
                            {
                                NpgsqlCommand command = new NpgsqlCommand(query, classGlobal.connP);
                                command.Parameters.AddWithValue("@vehicle_type", dr[0].ToString());
                                command.ExecuteNonQuery();
                                command.Dispose();
                                command = null;
                            }
                            else
                            {
                                //string s = ClassData.POST_METHODE("vehicleType", textBox2.Text);
                                //if (s != "200")
                                //{
                                //    frmMessageBox f = new frmMessageBox();
                                //    f.strMessage = s;
                                //    f.strStatus = "Error";
                                //    f.ShowDialog();
                                //}
                            }
                        }

                        GET_LISTS();
                        textBox2.Text = "";
                        MessageBoxSuccess();
                        return;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
                else
                {
                    frmMessageBox f = new frmMessageBox();
                    f.strMessage = "ไม่พบข้อมูล";
                    f.strStatus = "Warning";
                    f.ShowDialog();
                    return;
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            LinkLabelLinkClickedEventArgs ex = new LinkLabelLinkClickedEventArgs(linkLabel1.Links[0]);
            linkLabel1_LinkClicked(sender, ex);
        }
    }
}
