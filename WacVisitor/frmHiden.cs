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
    public partial class frmHiden : Form
    {
        public frmHiden()
        {
            InitializeComponent();
        }

        private void FrmHiden_Load(object sender, EventArgs e)
        {
            label1.Left = (this.ClientSize.Width / 2) - (label1.Width / 2);
            label1.Top = (this.ClientSize.Height / 2) - label1.Height;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Control c = Control.FromHandle(msg.HWnd);
            if (keyData == Keys.Z)
            {
                this.Close();
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
