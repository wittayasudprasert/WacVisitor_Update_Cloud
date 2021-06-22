using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WacVisitor
{
    public class TextBoxRound : TextBox
    {
        public TextBoxRound()
        {
            BorderStyle = System.Windows.Forms.BorderStyle.None;
            AutoSize = false; //Allows you to change height to have bottom padding
            Controls.Add(new Label() { Height = 5, Dock = DockStyle.Bottom, BackColor = Color.Black });
        }
    }
}
