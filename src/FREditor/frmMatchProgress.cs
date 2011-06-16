using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FREditor
{
    public partial class frmMatchProgress : Form
    {
        private SynonymMatcher matcher = null;

        public frmMatchProgress()
        {
            InitializeComponent();
            progressMatching.Minimum = 0;
            progressMatching.Maximum = 100;
            progressMatching.Value = 0;
            progressMatching.Step = 1;
        }

        public void SetMatcher(SynonymMatcher _matcher)
        {
            matcher = _matcher;
        }

        public void SetValue(uint val)
        {
            progressMatching.Value = (int)val;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if(matcher != null) matcher.StopMatching();
        }

        private void frmMatchProgress_FormClosed(object sender, FormClosedEventArgs e)
        {
          //  if (matcher != null) matcher.StopMatching();
        }
    }

}
