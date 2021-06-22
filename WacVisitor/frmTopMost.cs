using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WacVisitor
{
    public partial class frmTopMost : Form
    {
        public frmTopMost()
        {
            InitializeComponent();
        }

        private void frmTopMost_Load(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.DoEvents();

            label1.Left = (this.ClientSize.Width / 2) - (label1.Width / 2);
            label1.Top = (this.ClientSize.Height / 2) - label1.Height;

            BackColor = Color.Lime;
            TransparencyKey = Color.Lime;

            timer1.Interval = 100;
            timer1.Enabled = true; 
        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        int[] rgb = new int[9] { 255, 182, 128, 64, 0, 64, 128, 182, 255 };
        int n = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            label1.ForeColor = Color.FromArgb(rgb[n], rgb[n], rgb[n]);
            timer1.Enabled = true;
            n += 1;

            if (n == 9)
                n = 0;
        }
    }
}
