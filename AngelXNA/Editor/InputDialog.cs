#if WINDOWS

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AngelXNA.Editor
{
    public partial class InputDialog : Form
    {
        public string Value
        {
            get { return txtEntry.Text.Trim(); }
        }

        public InputDialog(string asTitle, string asIniitalText)
        {
            InitializeComponent();
            this.AcceptButton = btnOk;
            btnOk.Click += new EventHandler(btnOk_Click);

            this.Text = asTitle;
            txtEntry.Text = asIniitalText;
        }

        void btnOk_Click(object sender, EventArgs e)
        {
            
        }
    }
}

#endif