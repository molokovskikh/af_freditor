using System;
using System.Windows.Forms;

namespace FREditor
{
    public partial class frmMatchProgress : Form
    {        
		public SynonymMatcher Matcher { get; set; }

        public frmMatchProgress()
        {
            InitializeComponent();
            progressMatching.Minimum = 0;
            progressMatching.Maximum = 100;
            progressMatching.Value = 0;
            progressMatching.Step = 1;
        }

        public void SetValue(uint val)
        {
            progressMatching.Value = (int)val;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if(Matcher != null) Matcher.StopMatching();
        }

	/*	public void ShowProgress()
		{
			if (Modal) Visible = true;
			else ShowDialog();
		}*/
    }

}
