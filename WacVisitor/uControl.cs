using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WacVisitor
{
    public partial class uControl : UserControl
    {
        


        public uControl()
        {
            InitializeComponent();
        }


        public void AppendText1(string str)
        {
            textBox1.Text = str;
        }
        public void AppendText2(string str)
        {
            textBox2.Text = str;
        }

        public string GetText1()
        {
            return textBox1.Text; 
        }
        public string GetText2()
        {
            return textBox2.Text;
        }

        
    }
}
