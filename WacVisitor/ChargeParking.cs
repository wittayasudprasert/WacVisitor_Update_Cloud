using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Npgsql;

namespace WacVisitor
{
    public partial class ChargeParking : Form
    {
        string typename;
        string vCardNumber;
        int chargeMinutes;
        int chargeBaht = 0;
        string checkin = "";
        string checkout = "";

        int diffChargeBaht = 0;

        string license_plate = "";

        public ChargeParking(string _typename, int _chargeMinutes, string card_number, string dtIN, string dtOut)
        {
            InitializeComponent();

            typename = _typename;
            chargeMinutes = _chargeMinutes;
            vCardNumber = card_number;

            checkin = dtIN;
            checkout = dtOut;

            license_plate = GET_MOREINFO(classGlobal.pub_id);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        string workHours = "";
        string sFreeTimeInfo = "";
        int intFreeChargeMinute = 0;
        private void ChargeParking_Load(object sender, EventArgs e)
        {
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.WindowState = FormWindowState.Normal;

 
            TimeSpan spWorkMin = TimeSpan.FromMinutes(chargeMinutes);
            workHours = string.Format("{0}:{1}", ((int)spWorkMin.TotalHours).ToString().PadLeft(2, '0'), spWorkMin.Minutes.ToString().PadLeft(2, '0'));
           

            //++++
            lbVisitorNo.Text = "Visitor No. " + vCardNumber;
            lbLicensePlate.Text = "ทะเบียนรถ " + license_plate;
            lbIN.Text = "เข้า   " + checkin;
            lbOUT.Text = "ออก   " + checkout;
            lbParkTime.Text = "เวลาจอดทั้งหมด   " + workHours + " ชั่วโมง";
            lbFreePark.Text = "จอดฟรี (ชม.)  " + "0";
            lbSelectType.Text = "เลือกประเภทเก็บเงิน";
            //lbSelectType1.Text = "สำหรับ ..ประเภท Visitor..";  
            lbSelectType1.Text = "สำหรับ  " + typename; 

            txtCharge.Text = "0"; 
            //-----

            //panelButton.Enabled = false; 
            classGlobal.class_charge = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {   // ไม่จ่าย
            if (classGlobal.class_charge == "")
            {
                frmMsgWhiteBG fx = new frmMsgWhiteBG();
                fx.strMessage = "กรุณาเลือกประเภทเก็บเงิน A,B,C,D หรือ E ก่อน";
                fx.strStatus = "Error";
                fx.ShowDialog();
                return;
            }

            INSERT_CHARGE_LOGS("N");
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {   // จ่าย
            if (classGlobal.class_charge == "")
            {
                frmMsgWhiteBG fx = new frmMsgWhiteBG();
                fx.strMessage = "กรุณาเลือกประเภทเก็บเงิน A,B,C,D หรือ E ก่อน";
                fx.strStatus = "Error";
                fx.ShowDialog();
                return;
            }

            INSERT_CHARGE_LOGS("Y");

            classGlobal.text = "";
            PRINT_SLIP_TEXT();

            classGlobal.OPEN_CASH_DRAWER();

            FormSetting.FormPreviewSlip f = new FormSetting.FormPreviewSlip(txtCharge.Text, vCardNumber, license_plate, lbIN.Text, lbOUT.Text, lbParkTime.Text);
            f.ShowDialog();

            this.Close();
        }
        private void PRINT_SLIP_TEXT()
        {
            string[] arr = new string[0];
            string[] arr1 = new string[0];
            arr = checkin.Split(' ');

            //++ START                
            string[] arrworkHours = workHours.Split(':');
            workHours = (int.Parse(arrworkHours[0].ToString())).ToString() + ":" + arrworkHours[1].ToString();

            string[] splt = new string[0];
            string[] arrDateTime = new string[0];

            arrDateTime = checkin.Split(' ');
            splt = arrDateTime[0].ToString().Split('/');
            splt[1] = classGlobal.NUMBER_TO_MONTH_SHORT(splt[1]);
            checkin = String.Join(" ", splt) + " " + arrDateTime[1];

            arrDateTime = checkout.Split(' ');
            splt = arrDateTime[0].ToString().Split('/');
            splt[1] = classGlobal.NUMBER_TO_MONTH_SHORT(splt[1]);
            checkout = String.Join(" ", splt) + " " + arrDateTime[1];

            ////++ new line for logo
            //int emptyline = 7;
            //emptyline = ((Int32.Parse(classGlobal.pubHeight.ToString()) * 7) / 100) - 1;
            //for (int l = 0; l < emptyline; l++)
            //{
            //    classGlobal.text += "\t" + Environment.NewLine;
            //}
            ////-- new line for logo

            if (license_plate == "")
                license_plate = "-";

            classGlobal.text += "***********************************" + Environment.NewLine;
            //text += "VISITOR" + Environment.NewLine;
            classGlobal.text += "ใบเสร็จรับเงิน" + Environment.NewLine;
            classGlobal.text += "***********************************" + Environment.NewLine;
            classGlobal.text += "หมายเลข VISITOR   " + vCardNumber + Environment.NewLine;
            classGlobal.text += "ทะเบียนรถ   " + license_plate + Environment.NewLine;
            classGlobal.text += "เวลาเข้า   " + checkin + Environment.NewLine;
            classGlobal.text += "เวลาออก   " + checkout + Environment.NewLine;
            //text += "เวลาคิดค่าบริการ   " + workHours + " ชั่วโมง" + Environment.NewLine;
            classGlobal.text += sFreeTimeInfo + "เวลารวม  " + workHours + " ชั่วโมง" + Environment.NewLine;
            classGlobal.text += "ค่าบริการจอดรถ  " + txtCharge.Text + " บาท" + Environment.NewLine;

            classGlobal.text += "\t" + Environment.NewLine;

            //++ STOP
        }

        private void INSERT_CHARGE_LOGS(string paid)
        {
            //chargeBaht จากตารางการคิดเงินค่าจอด

            int ChangeChargeBaht = int.Parse(txtCharge.Text);   //ChangeChargeBaht จาก textbox จำนวนเงิน อาจมีการเปลี่ยนแปลงเองด้วยมือ
            diffChargeBaht = ChangeChargeBaht - chargeBaht;     //ผลต่างหากมีการแก้ไขจำนวนเงิน
            if (chargeBaht != ChangeChargeBaht) 
            {
                paid = "C";
            }

            string query = String.Format("INSERT INTO tbl_charge_logs(id, minutes, charge, extra, paid, charge_type) VALUES ({0}, '{1}', '{2}', '{3}', '{4}', '{5}')",
                                            classGlobal.pub_id,
                                            chargeMinutes.ToString(),
                                            chargeBaht.ToString(),
                                            diffChargeBaht.ToString(), 
                                            paid,
                                            classGlobal.class_charge);

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
                string jsonString = ClassData.POST_CHARGE_LOGS(chargeMinutes.ToString(), 
                                                                    chargeBaht.ToString(), 
                                                                            diffChargeBaht.ToString(), 
                                                                                    paid, 
                                                                                          classGlobal.class_charge);

                Console.WriteLine(jsonString);

            }
            
        }


        private string GET_INFO_OF_CHARGE()
        {
            try
            {
                string query = "SELECT minutes, rate FROM tbl_charge_car_park WHERE typename='" + typename + "'" + " AND " + "class ='" + classGlobal.class_charge + "'";

                DataTable dtID = new DataTable("typename");

                if (classGlobal.databaseType == "acc")
                {
                    OleDbDataAdapter ad = new OleDbDataAdapter(query, classGlobal.conn);
                    ad.Fill(dtID);
                    ad.Dispose();
                    ad = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    NpgsqlDataAdapter ad = new NpgsqlDataAdapter(query, classGlobal.connP);
                    ad.Fill(dtID);
                    ad.Dispose();
                    ad = null;
                }
                else
                {
                    dtID.Columns.Add("minutes");
                    dtID.Columns.Add("rate");

                    JToken jMessage = classGlobal.public_JsonChargePark["visitorType"];
                    foreach (var node in jMessage)
                    {
                        JArray ja = (JArray)node["classname"];
                        foreach (var nodeSub in ja)
                        {
                            if (node["typename"].ToString() == typename)
                            {
                                if (nodeSub["class"].ToString() == classGlobal.class_charge)
                                {
                                    // dt.Rows.Add(_foundId);
                                    if (nodeSub["status"].ToString() == "Y")
                                    {
                                        dtID.Rows.Add(nodeSub["minutes"].ToString(), nodeSub["rate"].ToString());
                                    }
                                }
                            }
                        }
                    }
                }


                string[] aMinutes = dtID.Rows[0][0].ToString().Split(';');
                string[] aRate = dtID.Rows[0][1].ToString().Split(';');
                int j = 0;
                for (int i = 0; i < aRate.Length; i++)
                {
                    if (int.Parse(aRate[i].ToString()) > 0)
                    {
                        j = i - 1;
                        break;
                    }
                }

                string strFreeTime = "";
                intFreeChargeMinute = int.Parse(aMinutes[j]);
                if (intFreeChargeMinute < 60)
                {
                    strFreeTime = "จอดฟรี " + intFreeChargeMinute.ToString() + " นาที ";
                    //strFreeTime = "00:" + intFreeChargeMinute.ToString();
                }
                else
                {
                    strFreeTime = "จอดฟรี " + (int.Parse(aMinutes[j]) / 60).ToString() + " ชั่วโมง ";
                    //strFreeTime = (int.Parse(aMinutes[j]) / 60).ToString();
                }
                
                return strFreeTime;
            }
            catch
            {
                intFreeChargeMinute = 0;
                return "";
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
                if (_dt.Rows.Count == 0)
                {
                    text = "";
                }
                else
                {
                    text = _dt.Rows[0][2].ToString();
                }
            }
            else if (classGlobal.databaseType == "psql")
            {
                NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM tbl_moreinfo WHERE id=" + id, classGlobal.connP);
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                da.Fill(_dt);
                da.Dispose();
                da = null;
                if (_dt.Rows.Count == 0)
                {
                    text = "";
                }
                else
                {
                    text = _dt.Rows[0][2].ToString();
                }
            }
            else
            {

            }
          

            return text;

        }

        private void cb1_CheckedChanged(object sender, EventArgs e)
        {
            classGlobal.class_charge = "";
            cb1.ForeColor = Color.Blue;
            cb2.ForeColor = Color.Blue;
            cb3.ForeColor = Color.Blue;
            cb4.ForeColor = Color.Blue;
            cb5.ForeColor = Color.Blue;
            if (cb1.Checked == true)
            {
                cb1.ForeColor = Color.Yellow;
                classGlobal.class_charge = "A";
                RECALCULATE_CHARGE();
            }
            else
            {
                cb1.CheckState = CheckState.Checked;
            }

             
        }

        private void cb2_CheckedChanged(object sender, EventArgs e)
        {
            classGlobal.class_charge = "";
            cb1.ForeColor = Color.Blue;
            cb2.ForeColor = Color.Blue;
            cb3.ForeColor = Color.Blue;
            cb4.ForeColor = Color.Blue;
            cb5.ForeColor = Color.Blue;
            if (cb2.Checked == true)
            {
                cb2.ForeColor = Color.Yellow;
                classGlobal.class_charge = "B";
                RECALCULATE_CHARGE();
            }
            else
            {
                cb2.CheckState = CheckState.Checked;
            }
        }

        private void cb3_CheckedChanged(object sender, EventArgs e)
        {
            classGlobal.class_charge = "";
            cb1.ForeColor = Color.Blue;
            cb2.ForeColor = Color.Blue;
            cb3.ForeColor = Color.Blue;
            cb4.ForeColor = Color.Blue;
            cb5.ForeColor = Color.Blue;
            if (cb3.Checked == true)
            {
                cb3.ForeColor = Color.Yellow;
                classGlobal.class_charge = "C";
                RECALCULATE_CHARGE();
            }
            else
            {
                cb3.CheckState = CheckState.Checked;
            }
        }

        private void RECALCULATE_CHARGE()
        {
            panelButton.Enabled = true;

            sFreeTimeInfo = GET_INFO_OF_CHARGE();

            lbFreePark.Text = "จอดฟรี (ชม.)  " + sFreeTimeInfo;


            int[] minutes = new int[0];  //นาที
            int[] rates = new int[0];  //ราคา
            try
            {
                DataTable dtID = new DataTable("typename");
                string query = "";
                query = "SELECT minutes, rate FROM tbl_charge_car_park WHERE typename='" + typename + "'" + 
                    " AND class = '" + classGlobal.class_charge + "'" + 
                    " AND status = 'Y'";

                if (classGlobal.databaseType == "acc")
                {
                    OleDbDataAdapter ad = new OleDbDataAdapter(query, classGlobal.conn);
                    ad.Fill(dtID);
                    ad.Dispose();
                    ad = null;
                }
                else if (classGlobal.databaseType == "psql")
                {
                    NpgsqlDataAdapter ad = new NpgsqlDataAdapter(query, classGlobal.connP);
                    ad.Fill(dtID);
                    ad.Dispose();
                    ad = null;
                }
                else
                {
                    dtID.Columns.Add("minutes");
                    dtID.Columns.Add("rate");

                    JToken jMessage = classGlobal.public_JsonChargePark["visitorType"];
                    foreach (var node in jMessage)
                    {
                        JArray ja = (JArray)node["classname"];
                        foreach (var nodeSub in ja)
                        {
                            if (node["typename"].ToString() == typename)
                            {
                                if (nodeSub["class"].ToString() == classGlobal.class_charge)
                                {
                                    // dt.Rows.Add(_foundId);
                                    if (nodeSub["status"].ToString() == "Y")
                                    {
                                        dtID.Rows.Add(nodeSub["minutes"].ToString(), nodeSub["rate"].ToString()); 
                                    }
                                }                                
                            }
                        }
                    }
                }

                int HrsToMins = 1;

                string[] stringsMinute = dtID.Rows[0][0].ToString().Split(';');
                string[] stringsRate = dtID.Rows[0][1].ToString().Split(';');
                for (int i = 0; i < stringsMinute.Length; i++)
                {
                    Array.Resize(ref minutes, minutes.Length + 1);
                    minutes[minutes.Length - 1] = Int32.Parse(stringsMinute[i].ToString()) * HrsToMins;

                    Array.Resize(ref rates, rates.Length + 1);
                    rates[rates.Length - 1] = Int32.Parse(stringsRate[i].ToString());
                }

                dtID.Dispose();
                dtID = null;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString()); 
                chargeBaht = 0;
                txtCharge.Text = chargeBaht.ToString();
                frmMessageBox f = new frmMessageBox();
                f.strMessage = "ไม่พบอัตราค่าบริการประเภทนี้!";
                f.strStatus = "Error";
                f.ShowDialog();
                classGlobal.class_charge = "";
                return;
            }

            //chargeMinutes = chargeMinutes - intFreeChargeMinute;
            //sFreeTimeInfo = GET_INFO_OF_CHARGE();

            int result = classGlobal.findClosest(minutes, chargeMinutes);
            int posOfArray = Array.IndexOf(minutes, result);
            posOfArray = classGlobal.MINUTES_BETWEEN_RANGE(minutes, chargeMinutes);
            chargeBaht = rates[posOfArray];

            txtCharge.Text = chargeBaht.ToString();

        }

        private void cb4_CheckedChanged(object sender, EventArgs e)
        {
            classGlobal.class_charge = "";
            cb1.ForeColor = Color.Blue;
            cb2.ForeColor = Color.Blue;
            cb3.ForeColor = Color.Blue;
            cb4.ForeColor = Color.Blue;
            cb5.ForeColor = Color.Blue;
            if (cb4.Checked == true)
            {
                cb4.ForeColor = Color.Yellow;
                classGlobal.class_charge = "D";
                RECALCULATE_CHARGE();
            }
            else
            {
                cb4.CheckState = CheckState.Checked;
            }
        }

        private void cb5_CheckedChanged(object sender, EventArgs e)
        {
            classGlobal.class_charge = "";
            cb1.ForeColor = Color.Blue;
            cb2.ForeColor = Color.Blue;
            cb3.ForeColor = Color.Blue;
            cb4.ForeColor = Color.Blue;
            cb5.ForeColor = Color.Blue;
            if (cb5.Checked == true)
            {
                cb5.ForeColor = Color.Yellow;
                classGlobal.class_charge = "E";
                RECALCULATE_CHARGE();
            }
            else
            {
                cb5.CheckState = CheckState.Checked;
            }
        }
    }
}
