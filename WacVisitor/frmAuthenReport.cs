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
using static WacVisitor.classAnimate;

namespace WacVisitor
{
    public partial class frmAuthenReport : Form
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

        private const int EM_SETCUEBANNER = 0x1501;
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)]string lParam);

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        public frmAuthenReport()
        {
            InitializeComponent();

            string encryptMD5Hash = classGlobal.MD5Hash("wacinfotech");
        }

        private void CHECK_USER_PERMISSION_TABLE()
        {
            if (classGlobal.databaseType == "acc")
            {
                string query = "";
                query = "CREATE TABLE tbl_user_permission " +
                        "(" +
                            "[ID] AUTOINCREMENT PRIMARY KEY," +
                            "[USR] TEXT(50), " +
                            "[PWD] TEXT(50) " +
                        ");";
                OleDbCommand cmd = new OleDbCommand(query, classGlobal.conn);
                try
                {
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    cmd = null;

                    cmd = new OleDbCommand("INSERT INTO tbl_user_permission (USR, PWD) VALUES ('wacinfotech','wacinfotech')", classGlobal.conn);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    cmd = null;
                }
                catch
                {
                    // Table 'tbl_user_permission' already exists.
                }
            }
            else if (classGlobal.databaseType == "psql")   // pgsql
            {
                string query = "";
                query = "CREATE TABLE tbl_user_permission " +
                        "(" +
                            "ID SERIAL PRIMARY KEY," +
                            "USR VARCHAR (50) NULL, " +
                            "PWD VARCHAR (50) NULL " +
                        ");";
                NpgsqlCommand cmd = new NpgsqlCommand(query, classGlobal.connP);
                try
                {
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    cmd = null;
                    cmd = new NpgsqlCommand("INSERT INTO tbl_user_permission (USR, PWD) VALUES ('wacinfotech','wacinfotech')", classGlobal.connP);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    cmd = null;
                }
                catch
                {
                    // Table 'tbl_user_permission' already exists.
                }
            }
            else
            {

            }
        }
        private void frmAuthenReport_Load(object sender, EventArgs e)
        {
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 10, 10));

            SendMessage(textBoxRound1.Handle, EM_SETCUEBANNER, 0, "ชื่อผู้ใช้");
            SendMessage(textBoxRound2.Handle, EM_SETCUEBANNER, 0, "รหัสผ่าน");

            this.ActiveControl = roundButton2;

            CHECK_USER_PERMISSION_TABLE();

            if (classGlobal.userId == "")
            {
                textBoxRound2.Location = textBoxRound1.Location;
                SendMessage(textBoxRound2.Handle, EM_SETCUEBANNER, 0, "รหัสยืนยัน");
            }
            else
            {
                linkLabel1.Visible = false;                
            }

            AnimateWindow(this.Handle, 100, AnimateWindowFlags.AW_CENTER);
        }


        private void frmAuthenReport_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

    
        private void frmAuthenReport_MouseClick(object sender, MouseEventArgs e)
        {
            this.ActiveControl = roundButton2;   
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmAuthenReportPermission f = new frmAuthenReportPermission();
            f.ShowDialog();
        }

        private void roundButton1_Click(object sender, EventArgs e)
        {
            if (classGlobal.userId != "" ){
                if (textBoxRound1.Text.Replace(" ", "").Equals(""))
                    return;
            }          

            if (textBoxRound2.Text.Replace(" ", "").Equals(""))
                return;

            int found = 0;
            DataTable dt = new DataTable("dt");
            string query = "";
            if (classGlobal.databaseType == "acc")
            {
                query = "SELECT ID FROM tbl_user_permission WHERE USR ='" + textBoxRound2.Text + "'";
                OleDbCommand cmd = new OleDbCommand(query, classGlobal.conn);
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                da.Fill(dt);
                da.Dispose();
                da = null;
                found = dt.Rows.Count;
                dt.Dispose();
                dt = null;

                if (found > 0)
                {
                    classGlobal.bAuthenReportPass = true;
                    this.Close();
                }
                else
                {
                    frmMessageBox f = new frmMessageBox();
                    f.strMessage = "รหัสยืนยัน ไม่ถูกต้อง!";
                    f.strStatus = "Warning";
                    f.ShowDialog();
                    textBoxRound1.Text = "";
                }

            }
            else if (classGlobal.databaseType == "psql")
            {
                query = "SELECT ID FROM tbl_user_permission WHERE USR ='" + textBoxRound2.Text + "'";
                NpgsqlCommand cmd = new NpgsqlCommand(query, classGlobal.connP);
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                da.Fill(dt);
                da.Dispose();
                da = null;
                found = dt.Rows.Count;
                dt.Dispose();
                dt = null;

                if (found > 0)
                {
                    classGlobal.bAuthenReportPass = true;
                    this.Close();
                }
                else
                {
                    frmMessageBox f = new frmMessageBox();
                    f.strMessage = "รหัสยืนยัน ไม่ถูกต้อง!";
                    f.strStatus = "Warning";
                    f.ShowDialog();
                    textBoxRound1.Text = "";
                }
            }
            else
            {
                string _UID = ClassData.POST_LOGIN(textBoxRound1.Text, textBoxRound2.Text);
                if (_UID == null || _UID == "")
                {
                    frmMessageBox f = new frmMessageBox();
                    f.strMessage = "ชื่อผู้ใช้/รหัสผ่าน ไม่ถูกต้อง!";
                    f.strStatus = "Warning";
                    f.ShowDialog();
                    textBoxRound2.Text = "";
                    return;
                }
                classGlobal.bAuthenReportPass = true;
                this.Close();
            }

        }

        private void roundButton2_Click(object sender, EventArgs e)
        {
            classGlobal.bAuthenReportPass = false;
            this.Close();
        }


        private void textBoxRound2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
                roundButton1.PerformClick();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Control c = Control.FromHandle(msg.HWnd);
            if (keyData == Keys.Z)
            {
                textBoxRound1.Text = classGlobal.userName;
                textBoxRound2.Text = classGlobal.passWord;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
