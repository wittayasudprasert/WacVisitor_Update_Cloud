using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WacVisitor
{
    public partial class frmLicense : Form
    {
        public frmLicense()
        {
            InitializeComponent();
        }

        bool eventClickOK = false;
        private void frmLicense_Load(object sender, EventArgs e)
        {
            string kbLayout = InputLanguage.CurrentInputLanguage.Culture.Name;
            string new_kbLayout = "en-US";

            if (kbLayout == "th-TH")
            {
                var culture = System.Globalization.CultureInfo.GetCultureInfo(new_kbLayout);
                var language = InputLanguage.FromCulture(culture);
                InputLanguage.CurrentInputLanguage = language;
                kbLayout = InputLanguage.CurrentInputLanguage.Culture.Name;
            }
            
            textBox3.Width = 0;

            this.ActiveControl = textBox3;

            button2.Width = 0;
        }
       
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Control c = Control.FromHandle(msg.HWnd);
            if (keyData == Keys.F1)
            {
                button2.PerformClick(); 
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                string licenseText = textBox3.Text; 
                string rawText = textBox3.Text;
                licenseText = licenseText.ToString().Replace(" ", "").Replace("%", "").Replace("^", "").Replace("=", "$").Replace("?", "").Replace(".", "$").Replace(";", "");
                string[] spl = licenseText.Split('$');
                string _tmp = Reverse(spl[3].ToString());
                string fullname = "";
                try
                {
                    _tmp = _tmp.Substring(0, 13);
                    _tmp = Reverse(_tmp);
                    fullname = (spl[2].ToString() + ".").Replace("..", ".") + "" + spl[1].ToString() + " " + spl[0].ToString();
                }
                catch
                {
                    _tmp = Reverse(spl[2].ToString());
                    _tmp = _tmp.Substring(0, 13);
                    _tmp = Reverse(_tmp);
                    int posName = rawText.IndexOf("?", 0, rawText.Length);
                    rawText = rawText.Substring(0, posName);
                    rawText = rawText.ToString().Replace(" ", "").Replace("%", "").Replace("^", "").Replace("=", "$").Replace("?", "").Replace(".", "$").Replace(";", "").Replace("$", " ");
                    string[] arrRaw = rawText.Split(' ');
                    fullname = arrRaw[2] + "" + arrRaw[1] + " " + arrRaw[0];
                }


                classGlobal.personID = _tmp;
                classGlobal.personName = fullname;

                //+++++++++++++++++++++++++
                string _tempLicnseText = textBox3.Text;
                System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("[%*'\",_&#^@.?$;=]");
                _tempLicnseText = reg.Replace(_tempLicnseText, string.Empty);
                _tempLicnseText = _tempLicnseText.Replace(" ", "");
                string _tmpBD = _tempLicnseText.Substring(_tempLicnseText.Length - 8);
                string d = _tmpBD.Substring(_tmpBD.Length - 2);
                string m = _tmpBD.Substring(_tmpBD.Length - 4, 2);
                string y = _tmpBD.Substring(0, 4);
                classGlobal.strBirthDateInLicensePlate = d + "/" + m + "/" + y;
                //-------------------------

                textBox1.Text = classGlobal.personID;
                textBox2.Text = classGlobal.personName;

                textBox1.Text = "#############";
                if (classGlobal.DisplayHashTag == true)
                {
                    //textBox1.Text = classGlobal.REPLACE_IDCARD(classGlobal.personID);
                    textBox2.Text = classGlobal.REPLACE_NAME(classGlobal.personName);
                }

                textBox3.Text = "";
            }
            catch
            {
                //
            }
            timer1.Stop();

            button1.PerformClick();
        }


        private string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            timer1.Stop();
            timer1.Interval = 500;
            timer1.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            eventClickOK = true;

            if (classGlobal.personID == "")
                classGlobal.personID = textBox1.Text;

            if (classGlobal.personName == "")
                classGlobal.personName = textBox2.Text;

            this.Close();
        }

        #region TEST
        private void button2_Click(object sender, EventArgs e)
        {
            string licenseText = "";
            licenseText = "%  ^SUDPRASERT$WITTAYA$MR.^^?;6007643190900120057=201219791202=?"; 
            //licenseText = "%  ^KUMSUWAN$SUPAPORN$MISS^^?;6007641841600045694=241019871007=?";
            string rawText = licenseText;
            licenseText = licenseText.ToString().Replace(" ", "").Replace("%", "").Replace("^", "").Replace("=", "$").Replace("?", "").Replace(".", "$").Replace(";", "");
            string[] spl = licenseText.Split('$');
            string _tmp = Reverse(spl[3].ToString());
            string fullname = "";
            try
            {
                _tmp = _tmp.Substring(0, 13);
                _tmp = Reverse(_tmp);
                fullname = (spl[2].ToString() + ".").Replace("..", ".") + "" + spl[1].ToString() + " " + spl[0].ToString();
            }
            catch
            {
                _tmp = Reverse(spl[2].ToString());
                _tmp = _tmp.Substring(0, 13);
                _tmp = Reverse(_tmp);
                int posName = rawText.IndexOf("?", 0, rawText.Length);
                rawText = rawText.Substring(0, posName);
                rawText = rawText.ToString().Replace(" ", "").Replace("%", "").Replace("^", "").Replace("=", "$").Replace("?", "").Replace(".", "$").Replace(";", "").Replace("$", " ");
                string[] arrRaw = rawText.Split(' ');
                fullname = arrRaw[2] + "" + arrRaw[1] + " " + arrRaw[0];
            }
           

            classGlobal.personID = _tmp;
            classGlobal.personName = fullname;

            textBox1.Text = classGlobal.personID;
            textBox2.Text = classGlobal.personName;

            textBox3.Text = "";

            textBox1.Text = "#############";
            if (classGlobal.DisplayHashTag == true)
            {
                //textBox1.Text = classGlobal.REPLACE_IDCARD(classGlobal.personID);
                textBox2.Text = classGlobal.REPLACE_NAME(classGlobal.personName);
            }
        }
        #endregion

        private void frmLicense_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (eventClickOK == false)
            {
               classGlobal.personID = "";
               classGlobal.personName = "";
            }
        }

    }
}
