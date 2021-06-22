using ClassHelper;
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

namespace WacVisitor.FormSetting
{
    public partial class SettingChargeCarPark : Form
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


        int typeid = -1;
        string typename = "";

        string current_costId = "";
        string current_classType = "";
        string current_visitorType = "";
        string current_minutes = "";
        string current_rate = "";
        string current_fine = "";
        string current_status = "";
        public SettingChargeCarPark(Int32 tid, string tname)
        {
            InitializeComponent();

            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            //Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 10, 10));

            typeid = tid;
            typename = tname;            
        }

        private void CREATE_DEFAULT_FOR_ALL_CLASS(string classType)
        {
            DataTable dtt;
            if (classGlobal.databaseType == "acc")
            {
                dtt = new DataTable("dtt");
                OleDbDataAdapter adapter = new OleDbDataAdapter(String.Format("SELECT COUNT(ID) FROM tbl_charge_car_park WHERE typeid = {0} AND class = '{1}'", typeid, classType), classGlobal.conn);
                adapter.Fill(dtt);
                adapter.Dispose();
                adapter = null;

                if (dtt.Rows[0][0].ToString() == "0")
                    ADD_DEFAULT_CHARGE(classType);
            }
            else if (classGlobal.databaseType == "psql")
            {
                dtt = new DataTable("dtt");
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(String.Format("SELECT COUNT(ID) FROM tbl_charge_car_park WHERE typeid = {0} AND class = '{1}'", typeid, classType), classGlobal.connP);
                adapter.Fill(dtt);
                adapter.Dispose();
                adapter = null;    

                if (dtt.Rows[0][0].ToString() == "0")
                    ADD_DEFAULT_CHARGE(classType);
            }
            else
            {
                dtt = new DataTable("dtt");
                dtt.Columns.Add("typeId");
                dtt.Columns.Add("typename");
                dtt.Columns.Add("costId");
                dtt.Columns.Add("class");
                dtt.Columns.Add("minutes");
                dtt.Columns.Add("rate");
                dtt.Columns.Add("status");

                string costId = "";
                string typeId = "";
                string visitorType = "";
                string classType_ = "";
                string minutes = "";
                string rate = "";
                string status = ""; // Y N
                JArray jaCost = new JArray();
                List<string> alCostTime = new List<string>();
                List<string> alCostRate = new List<string>();
                string[] temp = new string[0];

                List<string> lstVisitorType = new List<string>();
                JToken ja = classGlobal.public_JsonChargePark["visitorType"];
                foreach (var config in ja)    // main loop "visitorType"
                {
                    typeId = config["typeId"].ToString();
                    visitorType = config["typename"].ToString();

                    JArray jaClassname = (JArray)config["classname"];
                    for (int i = 0; i < jaClassname.Count; i++)
                    {
                        costId = jaClassname[i]["costId"].ToString();
                        classType_ = jaClassname[i]["class"].ToString();
                        minutes = jaClassname[i]["minutes"].ToString();
                        rate = jaClassname[i]["rate"].ToString();
                        status = jaClassname[i]["status"].ToString();
                        dtt.Rows.Add(typeId, visitorType, costId, classType_, minutes, rate, status);
                    }                   
                }

                DataRow[] found = dtt.Select("typename='" + typename + "'" + " AND " + "class='" + classType + "'");
                if (found.Length == 0)
                    ADD_DEFAULT_CHARGE(classType);

            }
        }
        private void SettingChargeCarPark_Load(object sender, EventArgs e)
        {
            
            label2.Text = "ประเภท VISITOR : " + typename;
            panel1.Left = (this.Width - panel1.ClientSize.Width) / 2;
            label2.Left = (this.Width - label2.ClientSize.Width) / 2;

            panel3.Left = (panel2.Width - panel3.ClientSize.Width) / 2;

            cb1.CheckState = CheckState.Checked;
            SET_DATAGRIDVIEW();

            string[] _classType = new string[5] { "A", "B", "C", "D", "E" };
            foreach (var type in _classType)
                CREATE_DEFAULT_FOR_ALL_CLASS(type);
  

        }
      
        private void SET_DATAGRIDVIEW()
        {
            MTextBox1.Text = "00:00";
            textBox1.Text = "";
            textBox2.Text = "";

            txtFree.Text = "0";
            txtFine.Text = "0";
            int u = 72; //24;
            foreach (Control x in this.paneltextbox.Controls)
            {                
                if (x is UserControl)
                {
                    ((uControl)x).AppendText1(u.ToString());
                    ((uControl)x).AppendText2("0");
                    u = u - 1;
                }
            }

            string query = String.Format("SELECT minutes, rate, status FROM tbl_charge_car_park WHERE typeid = {0} AND class = '{1}' ORDER BY ID ASC", typeid, classGlobal.class_charge);
            DataTable dtt = new DataTable("dtt");
           
            if (classGlobal.databaseType == "acc")
            {
                OleDbDataAdapter adapter = new OleDbDataAdapter(query, classGlobal.conn);                
                adapter.Fill(dtt);
                adapter.Dispose();
                adapter = null;

            }
            else if (classGlobal.databaseType == "psql")
            {
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(query, classGlobal.connP);
                adapter.Fill(dtt);
                adapter.Dispose();
                adapter = null;
            }
            else
            {  //SELECT minutes, rate FROM tbl_charge_car_park WHERE typeid = 0 AND class = 'A' ORDER BY ID ASC
                // ข้อมูล minutes, rate  คั่นด้วย ;

                dtt.Columns.Add("minutes");
                dtt.Columns.Add("rate");
                dtt.Columns.Add("status");

                JToken jMessage = classGlobal.public_JsonChargePark["visitorType"];
                if(jMessage != null)
                {
                    foreach (var node in jMessage)
                    {
                        JArray ja = (JArray)node["classname"];
                        foreach (var nodeSub in ja)
                        {
                            if (node["typename"].ToString() == typename && nodeSub["class"].ToString() == classGlobal.class_charge)
                            {
                                current_costId = nodeSub["costId"].ToString();
                                current_classType = nodeSub["class"].ToString();
                                current_visitorType = node["typename"].ToString();

                                current_minutes = nodeSub["minutes"].ToString().Replace(Environment.NewLine, "").Replace("minutes", "");
                                current_rate = nodeSub["rate"].ToString().Replace(Environment.NewLine, "").Replace("rate", "");
                                string[] _minute = current_minutes.Split(';');
                                Array.Resize(ref _minute, _minute.Length - 1);
                                current_minutes = String.Join(";", _minute); 

                                string[] _rate = current_rate.Split(';');
                                Array.Resize(ref _rate, _rate.Length - 1);
                                current_rate = String.Join(";", _rate);

                                current_status = nodeSub["status"].ToString().ToUpper().Replace("Y","true").Replace("N", "false");  

                                dtt.Rows.Add(nodeSub["minutes"].ToString().Replace(Environment.NewLine, "").Replace("minutes", ""),
                                    nodeSub["rate"].ToString().Replace(Environment.NewLine, "").Replace("rate", ""),
                                    nodeSub["status"].ToString());
                            }

                        }
                    }
                }

            }

            // fix 72 Hrs
            if (dtt.Rows.Count > 0)
            {
                string[] stringsMinute = dtt.Rows[0][0].ToString().Split(';');   // 26
                string[] stringsCharge = dtt.Rows[0][1].ToString().Split(';');


                if (stringsMinute.Length < 74)   // หมายถึงเป็นการกำหนดแบบเดิมคือ 24 ชั่วโมง ต้องแปลงให้เป็น 72 ชั่วโมง
                {                    
                    stringsMinute = new string[74];
                    stringsMinute[0] = "0";
                    for (int n = 1; n <= 72; n++)
                    {
                        stringsMinute[n] = (n * 60).ToString();
                    }
                    stringsMinute[stringsMinute.Length - 1] = (Int32.Parse(stringsMinute[stringsMinute.Length - 2]) + 1).ToString();

                    string temp_Fine = "";
                    List<string> list = new List<string>();
                    foreach (String data in stringsCharge)
                    {
                        list.Add(data);
                    }
                    temp_Fine = list[stringsCharge.Length - 1];                   
                    int limit = 74 - list.Count;
                    for (int m = 0; m < limit; m++)
                    {
                        list.Add(temp_Fine);
                    }
                    stringsCharge = list.ToArray();
                    
                }


                MTextBox1.Text = "00" + ":" + stringsMinute[0].ToString().PadLeft(2, '0');
                txtFree.Text = stringsCharge[0].ToString();

                string tmpName = "";
                foreach (Control x in this.paneltextbox.Controls)
                {
                    if (x is UserControl)
                    {
                        tmpName = x.Name.ToString().Replace("uControl", "");  //uControl1

                        ((uControl)x).AppendText2(stringsCharge[int.Parse(tmpName)]); 
                        
                    }
                }
                textBox52.Text = "ปรับ";
                txtFine.Text = stringsCharge[stringsCharge.Length-1].ToString();

                bool _status = false;
                try
                {
                    string sTextYN = dtt.Rows[0][2].ToString().ToUpper();
                    if (sTextYN == "Y")
                        _status = true;
                    else
                        _status = false;
                }
                catch
                {
                    _status = false;
                }

                myCheckBox1.Checked = _status;
            }
            else
            {
                myCheckBox1.Checked = false;
            }
 
        }
 
        private void button8_Click(object sender, EventArgs e)
        {

            if (classGlobal.class_charge == "")
            {
                frmMessageBox fe = new frmMessageBox();
                fe.strMessage = "กรุณาเลือก แบบการคิดเงิน";
                fe.strStatus = "Error";
                fe.ShowDialog();
                return;
            }

            string[] strMinute = new string[0];
            string[] strCharge = new string[0];

            //++ เวลาฟรี 
            try
            {
                if (txtFree.Text.ToString().Replace(" ", "") == "")
                    txtFree.Text = "0";

                string[] arrMark = MTextBox1.Text.ToString().Split(':');
                int hr = int.Parse(arrMark[0].ToString()) * 60;
                int min = int.Parse(arrMark[1].ToString());
                MTextBox1.Text = hr.ToString().PadLeft(2, '0') + ":" + min.ToString().PadLeft(2, '0');


                Array.Resize(ref strMinute, strMinute.Length + 1);
                strMinute[strMinute.Length - 1] = min.ToString();

                Array.Resize(ref strCharge, strCharge.Length + 1);
                strCharge[strCharge.Length - 1] = txtFree.Text;
            }
            catch
            {
                frmMessageBox fe = new frmMessageBox();
                fe.strMessage = "กรุณาป้อนเวลาในรูปแบบ 00:00 (ชั่วโมง:นาที)";
                fe.strStatus = "Error";
                fe.ShowDialog();
                MTextBox1.Text = "00:00";
                return;
            }
            //-- เวลาฟรี

            for (int i = 1; i <= 72; i++)
            {
                Array.Resize(ref strMinute, strMinute.Length + 1);
                strMinute[strMinute.Length - 1] = i.ToString();

                Array.Resize(ref strCharge, strCharge.Length + 1);
                strCharge[strCharge.Length - 1] = "0";
            }

            string tmpName = "";
            string sCharge = "";
            foreach (Control x in this.paneltextbox.Controls)
            {
                if (x is UserControl)
                {
                    tmpName = x.Name.ToString().Replace("uControl", "");  //uControl1
                    strMinute[int.Parse(tmpName)] = (int.Parse(((uControl)x).GetText1()) * 60).ToString();
                    sCharge = ((uControl)x).GetText2();
                    if (sCharge == "")
                    {
                        sCharge = "0";
                        ((uControl)x).AppendText2(sCharge); 
                    }
                    strCharge[int.Parse(tmpName)] = sCharge;
                }
            }

            //++ ค่าปรับ
            if (txtFine.Text.ToString().Replace(" ", "") == "")
                txtFine.Text = "0";

            Array.Resize(ref strMinute, strMinute.Length + 1); // เกินวันที่กำหนด
            strMinute[strMinute.Length - 1] = (Int32.Parse (strMinute[strMinute.Length - 2]) + 1).ToString();

            Array.Resize(ref strCharge, strCharge.Length + 1);
            strCharge[strCharge.Length - 1] = txtFine.Text;
            //-- ค่าปรับ

            string query = "";
            OleDbCommand command;
            NpgsqlCommand commandP;

            #region backup old code
            //if (classGlobal.databaseType == "acc")
            //{
            //    command = new OleDbCommand("DELETE FROM tbl_charge_car_park WHERE typeid=" + typeid + " AND class = '" + classGlobal.class_charge + "'", classGlobal.conn);
            //    command.ExecuteNonQuery();
            //    command.Dispose();
            //    command = null;
            //}
            //else if (classGlobal.databaseType == "psql")
            //{
            //    commandP = new NpgsqlCommand("DELETE FROM tbl_charge_car_park WHERE typeid=" + typeid + " AND class = '" + classGlobal.class_charge + "'", classGlobal.connP);
            //    commandP.ExecuteNonQuery();
            //    commandP.Dispose();
            //    commandP = null;
            //}
            //else
            //{

            //}
            #endregion

            if (strMinute.Length > 0)
            {
                string stringsMinutes = String.Join(";", strMinute);
                string stringsCharge = String.Join(";", strCharge); 
                try
                {
                    //query = String.Format("INSERT INTO tbl_charge_car_park(typeid, class, typename, minutes, rate, status) VALUES ({0}, '{1}', '{2}', '{3}', '{4}', '{5}')",
                    //                        typeid, classGlobal.class_charge, typename, stringsMinutes, stringsCharge, "Y");

                    query = String.Format("UPDATE tbl_charge_car_park SET minutes ='{0}', rate = '{1}' WHERE typeid={2} AND class='{3}' AND typename='{4}'",
                                           stringsMinutes, stringsCharge, typeid, classGlobal.class_charge, typename);

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

                        current_minutes = stringsMinutes;
                        current_rate = stringsCharge;
                        string[] _minute = current_minutes.Split(';');
                        Array.Resize(ref _minute, _minute.Length - 1);
                        current_minutes = String.Join(";", _minute);

                        string[] _rate = current_rate.Split(';');
                        Array.Resize(ref _rate, _rate.Length - 1);
                        current_rate = String.Join(";", _rate);

                        current_fine = txtFine.Text;

                        string _status = myCheckBox1.Checked.ToString().ToLower();

                        if (current_classType == "")
                            current_classType = classGlobal.class_charge;

                        if (current_visitorType == "")
                            current_visitorType = typename;

                        ClassData.POST_PUT_PAID_CHARGE_CONFIG(classGlobal.access_token, 
                                                                 classGlobal.userId, 
                                                                    current_classType, 
                                                                        current_visitorType, 
                                                                            current_minutes, 
                                                                                current_rate, 
                                                                                    _status.ToString().ToLower(), 
                                                                                        current_costId, 
                                                                                            "PUT", 
                                                                                                current_fine);

                        current_status = _status.ToString().ToLower();
                        classGlobal.public_JsonChargePark = ClassData.GET_PAID_CHAGRE_CONFIG();
                    }

                }
                catch
                {

                }
            }

            frmMessageBox f = new frmMessageBox();
            f.strMessage = "สำเร็จ";
            f.strStatus = "Information";
            f.ShowDialog();
            
        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)  // ปุ่ม "ไม่คิดค่าบริการ"  ซ่อนไว้ ไม่ได้ใช้งานแล้ว
        {
            if (classGlobal.class_charge == "")
            {
                frmMessageBox fe = new frmMessageBox();
                fe.strMessage = "กรุณาเลือก แบบการคิดเงิน";
                fe.strStatus = "Error";
                fe.ShowDialog();
                return;
            }

            OleDbCommand command;
            NpgsqlCommand commandP;
            if (classGlobal.databaseType == "acc")
            {
                //command = new OleDbCommand("DELETE FROM tbl_charge_car_park WHERE typeid=" + typeid + " AND class = '" + classGlobal.class_charge + "'", classGlobal.conn);
                //command.ExecuteNonQuery();
                //command.Dispose();
                //command = null;

                string query = "UPDATE tbl_charge_car_park SET status = 'N' WHERE typeid=" + typeid + " AND class = '" + classGlobal.class_charge + "'";
                command = new OleDbCommand(query, classGlobal.conn);
                command.ExecuteNonQuery();
                command.Dispose();
                command = null;
            }
            else if (classGlobal.databaseType == "psql")
            {
                //commandP = new NpgsqlCommand("DELETE FROM tbl_charge_car_park WHERE typeid=" + typeid + " AND class = '" + classGlobal.class_charge + "'", classGlobal.connP);
                //commandP.ExecuteNonQuery();
                //commandP.Dispose();
                //commandP = null;

                string query = "UPDATE tbl_charge_car_park SET status = 'N' WHERE typeid=" + typeid + " AND class = '" + classGlobal.class_charge + "'";
                commandP = new NpgsqlCommand(query, classGlobal.connP);
                commandP.ExecuteNonQuery();
                commandP.Dispose();
                commandP = null;
            }
            else
            {

            }
          
            this.Close();
        }

        private void MTextBox1_Leave(object sender, EventArgs e)
        {
            //++ เวลาฟรี 
            try
            {
                string[] arrMark = MTextBox1.Text.ToString().Split(':');
                int hr = int.Parse(arrMark[0].ToString()) * 60;
                int min = int.Parse(arrMark[1].ToString());

                MTextBox1.Text = hr.ToString().PadLeft(2, '0') + ":" + min.ToString().PadLeft(2, '0');
            }
            catch
            {
                frmMessageBox fe = new frmMessageBox();
                fe.strMessage = "กรุณาป้อนเวลาในรูปแบบ 00:00 (ชั่วโมง:นาที)";
                fe.strStatus = "Error";
                fe.ShowDialog();
                MTextBox1.Text = "00:00";

                return;
            }
            //-- เวลาฟรี
        }

        private void cb1_CheckedChanged(object sender, EventArgs e)
        {
            classGlobal.class_charge = "";
            cb1.ForeColor = Color.Black;
            cb2.ForeColor = Color.Black;
            cb3.ForeColor = Color.Black;
            cb4.ForeColor = Color.Black;
            cb5.ForeColor = Color.Black;
            if (cb1.Checked == true)
            {
                cb1.ForeColor = Color.Yellow;
                classGlobal.class_charge = "A";
                SET_DATAGRIDVIEW();
            }
            else
            {
                cb1.CheckState = CheckState.Checked;
            }
        }
        private void cb2_CheckedChanged(object sender, EventArgs e)
        {
            classGlobal.class_charge = "";
            cb1.ForeColor = Color.Black;
            cb2.ForeColor = Color.Black;
            cb3.ForeColor = Color.Black;
            cb4.ForeColor = Color.Black;
            cb5.ForeColor = Color.Black;
            if (cb2.Checked == true)
            {
                cb2.ForeColor = Color.Yellow;
                classGlobal.class_charge = "B";
                SET_DATAGRIDVIEW();
            }
            else
            {
                cb2.CheckState = CheckState.Checked;
            }
        }
        private void cb3_CheckedChanged(object sender, EventArgs e)
        {
            classGlobal.class_charge = "";
            cb1.ForeColor = Color.Black;
            cb2.ForeColor = Color.Black;
            cb3.ForeColor = Color.Black;
            cb4.ForeColor = Color.Black;
            cb5.ForeColor = Color.Black;
            if (cb3.Checked == true)
            {
                cb3.ForeColor = Color.Yellow;
                classGlobal.class_charge = "C";
                SET_DATAGRIDVIEW();
            }
            else
            {
                cb3.CheckState = CheckState.Checked;
            }
        }
        private void cb4_CheckedChanged(object sender, EventArgs e)
        {
            classGlobal.class_charge = "";
            cb1.ForeColor = Color.Black;
            cb2.ForeColor = Color.Black;
            cb3.ForeColor = Color.Black;
            cb4.ForeColor = Color.Black;
            cb5.ForeColor = Color.Black;
            if (cb4.Checked == true)
            {
                cb4.ForeColor = Color.Yellow;
                classGlobal.class_charge = "D";
                SET_DATAGRIDVIEW();
            }
            else
            {
                cb4.CheckState = CheckState.Checked;
            }
        }
        private void cb5_CheckedChanged(object sender, EventArgs e)
        {
            classGlobal.class_charge = "";
            cb1.ForeColor = Color.Black;
            cb2.ForeColor = Color.Black;
            cb3.ForeColor = Color.Black;
            cb4.ForeColor = Color.Black;
            cb5.ForeColor = Color.Black;
            if (cb5.Checked == true)
            {
                cb5.ForeColor = Color.Yellow;
                classGlobal.class_charge = "E";
                SET_DATAGRIDVIEW();
            }
            else
            {
                cb5.CheckState = CheckState.Checked;
            }
        }

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Replace(" ", "").Equals(""))
                return;

            if (textBox2.Text.Replace(" ", "").Equals(""))
                return;

            if (textBox3.Text.Replace(" ", "").Equals(""))
                return;


            try
            {
                if (textBox3.Text.IndexOf("+") > -1)  // คิดเงินแบบตรงๆ หรือ บวกเพิ่ม
                {
                    string start = textBox1.Text.Replace(" ", "");
                    string stop = textBox2.Text.Replace(" ", "");
                    string plus = textBox3.Text.Replace(" ", "").Replace("+", "");
                    string currentValueStart = "";

                    string tmpName = "";
                    foreach (Control x in this.paneltextbox.Controls)
                    {
                        if (x is UserControl)
                        {
                            tmpName = x.Name.ToString().Replace("uControl", "");
                            if (tmpName.Equals(start))
                            {
                                //((uControl)x).AppendText2(plus);
                                currentValueStart = ((uControl)x).GetText2().ToString();
                                break;
                            }
                        }
                    }

                    int updateValues = Int32.Parse(currentValueStart);
                    for (int i = (Int32.Parse(start) + 1); i <= Int32.Parse(stop); i++)
                    {
                        foreach (Control x in this.paneltextbox.Controls)
                        {
                            if (x is UserControl)
                            {
                                tmpName = x.Name.ToString().Replace("uControl", "");
                                if (tmpName.Equals(i.ToString()))
                                {
                                    updateValues = updateValues + Int32.Parse(plus);
                                    ((uControl)x).AppendText2(updateValues.ToString());
                                }
                            }
                        }
                    }

                }
                else
                {
                    string start = textBox1.Text.Replace(" ", "");
                    string stop = textBox2.Text.Replace(" ", "");
                    string value = textBox3.Text.Replace(" ", "");

                    string tmpName = "";
                    for (int i = Int32.Parse(start); i <= Int32.Parse(stop); i++)
                    {
                        foreach (Control x in this.paneltextbox.Controls)
                        {
                            if (x is UserControl)
                            {
                                tmpName = x.Name.ToString().Replace("uControl", "");
                                if (tmpName.Equals(i.ToString()))
                                {
                                    ((uControl)x).AppendText2(value);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
           
        }

        private void myCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            bool _status = myCheckBox1.Checked;

            string strStatus = "";
            if (_status == true)
                strStatus = "Y";
            else
                strStatus = "N";

            OleDbCommand command;
            NpgsqlCommand commandP;
            if (classGlobal.databaseType == "acc")
            {

                DataTable dtt = new DataTable("dtt");
                OleDbDataAdapter adapter = new OleDbDataAdapter(String.Format("SELECT COUNT(ID) FROM tbl_charge_car_park WHERE typeid = {0} AND class = '{1}'", typeid, classGlobal.class_charge), classGlobal.conn);
                adapter.Fill(dtt);
                adapter.Dispose();
                adapter = null;

                if (dtt.Rows[0][0].ToString() == "0")
                {
                    return;
                }

                string query = "UPDATE tbl_charge_car_park SET status = '" + strStatus + "' WHERE typeid=" + typeid + " AND class = '" + classGlobal.class_charge + "'";
                command = new OleDbCommand(query, classGlobal.conn);
                command.ExecuteNonQuery();
                command.Dispose();
                command = null;
            }
            else if (classGlobal.databaseType == "psql")
            {

                DataTable dtt = new DataTable("dtt");
                OleDbDataAdapter adapter = new OleDbDataAdapter(String.Format("SELECT COUNT(ID) FROM tbl_charge_car_park WHERE typeid = {0} AND class = '{1}'", typeid, classGlobal.class_charge), classGlobal.conn);
                adapter.Fill(dtt);
                adapter.Dispose();
                adapter = null;

                if (dtt.Rows[0][0].ToString() == "0")
                {
                    return;
                }

                string query = "UPDATE tbl_charge_car_park SET status = '" + strStatus + "' WHERE typeid=" + typeid + " AND class = '" + classGlobal.class_charge + "'";
                commandP = new NpgsqlCommand(query, classGlobal.connP);
                commandP.ExecuteNonQuery();
                commandP.Dispose();
                commandP = null;
            }
            else
            {
                Console.WriteLine(current_costId);
                Console.WriteLine(current_visitorType);
                Console.WriteLine(current_classType);
                Console.WriteLine(current_minutes);
                Console.WriteLine(current_rate);

                current_fine = txtFine.Text;

                if (current_status != _status.ToString().ToLower())
                {
                    ClassData.POST_PUT_PAID_CHARGE_CONFIG(classGlobal.access_token,
                                                             classGlobal.userId, 
                                                                current_classType, 
                                                                    current_visitorType, 
                                                                        current_minutes, 
                                                                            current_rate, 
                                                                                _status.ToString().ToLower(), 
                                                                                    current_costId, 
                                                                                        "PUT", 
                                                                                            current_fine);

                    current_status = _status.ToString().ToLower();
                    classGlobal.public_JsonChargePark = ClassData.GET_PAID_CHAGRE_CONFIG();
                }

                
            }

        }

        private void ADD_DEFAULT_CHARGE(string _className)
        {
            string[] strMinute = new string[0];
            string[] strCharge = new string[0];

            //++ เวลาฟรี 
            try
            {
                if (txtFree.Text.ToString().Replace(" ", "") == "")
                    txtFree.Text = "0";

                string[] arrMark = MTextBox1.Text.ToString().Split(':');
                int hr = int.Parse(arrMark[0].ToString()) * 60;
                int min = int.Parse(arrMark[1].ToString());
                MTextBox1.Text = hr.ToString().PadLeft(2, '0') + ":" + min.ToString().PadLeft(2, '0');


                Array.Resize(ref strMinute, strMinute.Length + 1);
                strMinute[strMinute.Length - 1] = min.ToString();

                Array.Resize(ref strCharge, strCharge.Length + 1);
                strCharge[strCharge.Length - 1] = txtFree.Text;
            }
            catch
            {
                frmMessageBox fe = new frmMessageBox();
                fe.strMessage = "กรุณาป้อนเวลาในรูปแบบ 00:00 (ชั่วโมง:นาที)";
                fe.strStatus = "Error";
                fe.ShowDialog();
                MTextBox1.Text = "00:00";
                return;
            }
            //-- เวลาฟรี

            for (int i = 1; i <= 72; i++)
            {
                Array.Resize(ref strMinute, strMinute.Length + 1);
                strMinute[strMinute.Length - 1] = i.ToString();

                Array.Resize(ref strCharge, strCharge.Length + 1);
                strCharge[strCharge.Length - 1] = "0";
            }

            string tmpName = "";
            string sCharge = "";
            foreach (Control x in this.paneltextbox.Controls)
            {
                if (x is UserControl)
                {
                    tmpName = x.Name.ToString().Replace("uControl", "");  //uControl1
                    strMinute[int.Parse(tmpName)] = (int.Parse(((uControl)x).GetText1()) * 60).ToString();
                    sCharge = ((uControl)x).GetText2();
                    if (sCharge == "")
                    {
                        sCharge = "0";
                        ((uControl)x).AppendText2(sCharge);
                    }
                    strCharge[int.Parse(tmpName)] = sCharge;
                }
            }

            //++ ค่าปรับ
            if (txtFine.Text.ToString().Replace(" ", "") == "")
                txtFine.Text = "0";

            Array.Resize(ref strMinute, strMinute.Length + 1); // เกินวันที่กำหนด
            strMinute[strMinute.Length - 1] = (Int32.Parse(strMinute[strMinute.Length - 2]) + 1).ToString();

            Array.Resize(ref strCharge, strCharge.Length + 1);
            strCharge[strCharge.Length - 1] = txtFine.Text;
            //-- ค่าปรับ


            if (strMinute.Length > 0)
            {
                string query = "";
                string stringsMinutes = String.Join(";", strMinute);
                string stringsCharge = String.Join(";", strCharge);
                try
                {
                    query = String.Format("INSERT INTO tbl_charge_car_park(typeid, class, typename, minutes, rate, status) VALUES ({0}, '{1}', '{2}', '{3}', '{4}', '{5}')",
                                            typeid, _className, typename, stringsMinutes, stringsCharge, "N");

                    if (classGlobal.databaseType == "acc")
                    {
                        OleDbCommand command = new OleDbCommand(query, classGlobal.conn);
                        command.ExecuteNonQuery();
                        command.Dispose();
                        command = null;
                    }
                    else if (classGlobal.databaseType == "psql")
                    {
                        NpgsqlCommand commandP = new NpgsqlCommand(query, classGlobal.connP);
                        commandP.ExecuteNonQuery();
                        commandP.Dispose();
                        commandP = null;
                    }
                    else
                    {
                        string[] minutes = stringsMinutes.Split(';');
                        string[] rate = stringsCharge.Split(';');

                        Array.Resize(ref minutes, minutes.Length-1);
                        Array.Resize(ref rate, rate.Length - 1);

                        stringsMinutes = String.Join(";", minutes);
                        stringsCharge = String.Join(";", rate);

                        ClassData.POST_PUT_PAID_CHARGE_CONFIG(classGlobal.access_token, 
                                                                 classGlobal.userId, 
                                                                    _className, 
                                                                        typename, 
                                                                            stringsMinutes, 
                                                                                stringsCharge, 
                                                                                    "false", 
                                                                                        "", 
                                                                                            "POST", 
                                                                                                "0");
                    }

                }
                catch
                {

                }
            }
        }
    }
}
