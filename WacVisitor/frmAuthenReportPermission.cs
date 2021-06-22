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
    public partial class frmAuthenReportPermission : Form
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
        public frmAuthenReportPermission()
        {
            InitializeComponent();
        }

        private void frmAuthenReportPermission_Load(object sender, EventArgs e)
        {
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 10, 10));

            SendMessage(textBoxRound1.Handle, EM_SETCUEBANNER, 0, "รหัสยืนยันเดิม");
            SendMessage(textBoxRound2.Handle, EM_SETCUEBANNER, 0, "รหัสยืนยันใหม่");

            this.ActiveControl = roundButton2;

            AnimateWindow(this.Handle, 100, AnimateWindowFlags.AW_CENTER);
        }


        private void frmAuthenReportPermission_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void frmAuthenReportPermission_MouseClick(object sender, MouseEventArgs e)
        {
            this.ActiveControl = roundButton2;   
        }

     

        private void roundButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void roundButton1_Click(object sender, EventArgs e)
        {
            if (textBoxRound1.Text.Replace(" ", "").Equals(""))
                return;

            if (textBoxRound2.Text.Replace(" ", "").Equals(""))
                return;


            int found = 0;
            int id = 0;
            DataTable dt = new DataTable("dt");
            string query = "";
            if (classGlobal.databaseType == "acc")
            {
                query = "SELECT ID FROM tbl_user_permission WHERE USR ='" + textBoxRound1.Text + "'";
                OleDbCommand cmd = new OleDbCommand(query, classGlobal.conn);
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                da.Fill(dt);
                da.Dispose();
                da = null;
                found = dt.Rows.Count;
                try { id = Int32.Parse(dt.Rows[0][0].ToString()); }
                catch { id = 0; }
                dt.Dispose();
                dt = null;
                cmd.Dispose();
                cmd = null;

                if ((found == 0) && textBoxRound1.Text.Equals("wacinfotech"))
                {
                    id = 1;
                    found = 1;
                }

                if (found > 0)
                {
                    query = String.Format("UPDATE tbl_user_permission SET USR ='{0}'" + ", PWD = '{0}'" + " WHERE ID = {1}", textBoxRound2.Text, id);
                    cmd = new OleDbCommand(query, classGlobal.conn);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    cmd = null;
                    this.Close();
                }
                else
                {
                    frmMessageBox f = new frmMessageBox();
                    f.strMessage = "รหัสยืนยันเดิม ไม่ถูกต้อง!";
                    f.strStatus = "Warning";
                    f.ShowDialog();
                    textBoxRound1.Text = "";
                    textBoxRound2.Text = "";
                }

            }
            else if (classGlobal.databaseType == "psql")
            {
                query = "SELECT ID FROM tbl_user_permission WHERE USR ='" + textBoxRound1.Text + "'";
                NpgsqlCommand cmd = new NpgsqlCommand(query, classGlobal.connP);
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                da.Fill(dt);
                da.Dispose();
                da = null;
                found = dt.Rows.Count;
                try { id = Int32.Parse(dt.Rows[0][0].ToString()); }
                catch { id = 0; }
                dt.Dispose();
                dt = null;
                cmd.Dispose();
                cmd = null;

                if ((found == 0) && textBoxRound1.Text.Equals("wacinfotech"))
                {
                    id = 1;
                    found = 1;
                }

                if (found > 0)
                {
                    query = String.Format("UPDATE tbl_user_permission SET USR ='{0}'" + ", PWD = '{0}'" + " WHERE ID = {1}", textBoxRound2.Text, id);
                    cmd = new NpgsqlCommand(query, classGlobal.connP);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    cmd = null;
                    this.Close();
                }
                else
                {
                    frmMessageBox f = new frmMessageBox();
                    f.strMessage = "รหัสยืนยันเดิม ไม่ถูกต้อง!";
                    f.strStatus = "Warning";
                    f.ShowDialog();
                    textBoxRound1.Text = "";
                    textBoxRound2.Text = "";
                }
            }
            else
            {

            }
        }
    }
}
