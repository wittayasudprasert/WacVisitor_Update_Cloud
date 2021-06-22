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
    public partial class Msg : Form
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
        public Msg()
        {
            InitializeComponent();

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 10, 10));
        }

        private void Msg_Load(object sender, EventArgs e)
        {
            

            label1.Text = classGlobal.MsgText;  
            label1.Left = (this.Width / 2) - (label1.Width / 2); 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            classGlobal.MsgConfirm = "NO";
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            classGlobal.MsgConfirm = "YES";
            this.Close();
        }
    }
}
