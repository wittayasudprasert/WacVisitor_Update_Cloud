using ClassHelper;
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
    public partial class frmCheckLicense : Form
    {
        public frmCheckLicense()
        {
            InitializeComponent();
        }

        string filename = classGlobal.config; 
        private void frmCheckLicense_Load(object sender, EventArgs e)
        {
            clsXML clsxml = new clsXML();

            string strLocal_Lic = clsxml.GetReadXML("root", "license", filename);
            if (strLocal_Lic.Trim() == "-")
            {
                textBox1.Text = "";
            }
            else
            {
                textBox1.Text = strLocal_Lic;
            }
            //textBox1.Text = strLocal_Lic;

            try
            {
                classCheckLicense cls = new classCheckLicense();
                string str_Get_Win32_BaseBoard = cls.Get_Win32_BaseBoard();
                long Lng = cls.EncryptionData(str_Get_Win32_BaseBoard);
                textBox2.Text = Lng.ToString();
                textBox2.ForeColor = Color.Blue;
                cls = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            clsxml = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string strLocal_Lic = textBox1.Text;
            if (strLocal_Lic.Replace(" ", "") != "")
            {
                if (strLocal_Lic.Length == 39)
                {

                    classCheckLicense cls = new classCheckLicense();
                    string str_Get_Win32_BaseBoard = cls.Get_Win32_BaseBoard();
                    long Lng = cls.EncryptionData(str_Get_Win32_BaseBoard);
                    string strEnc = cls.Generates16ByteUnique(Lng.ToString());
                    cls = null;

                    if ((strLocal_Lic.Replace(" ", "").ToUpper()) == (strEnc.Replace(" ", "").ToUpper()))
                    {
                        clsXML clsxml = new clsXML();
                        clsxml.ModifyElement("root", "license", strLocal_Lic, filename);
                        clsxml = null;
                        MessageBox.Show("สำเร็จ, กรุณาปิด-เปิด โปรแกรมใหม่อีกครั้ง", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
            }

            MessageBox.Show("License Key ไม่ถูกต้อง กรุณาตรวจสอบ!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);


        }
    }
}
