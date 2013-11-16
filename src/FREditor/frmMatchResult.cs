using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace FREditor
{
	public partial class frmMatchResult : Form
	{
		public SynonymMatcher Matcher { get; set; }

		public frmMatchResult()
		{
			InitializeComponent();
		}

		public void Fill(Dictionary<uint, FirmSummary> firms)
		{
			btnMatchAll.Enabled = true;
			matchResGV.Rows.Clear();

			var matchCnt = firms.Sum(firm => firm.Value.SynonymCount()); // общее количество найденых совпадений

			if (matchCnt == 0) {
				MessageBox.Show("Не найдено совпадений с имеющимися синонимами", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			foreach (var firm in firms) {
				var idx = matchResGV.Rows.Add();
				decimal cnt = firm.Value.SynonymCount();
				var val = Math.Round(cnt * 100 / matchCnt, 2);
				matchResGV[0, idx].Value = firm.Value.FullName();
				matchResGV[1, idx].Value = val.ToString();
			}

			Text = String.Format("Результат сопоставления (сопоставлено позиций: {0})", matchCnt);
			if (!Modal)
				ShowDialog();
		}

		private void matchResGV_DoubleClick(object sender, EventArgs e)
		{
			var row = matchResGV.CurrentRow;
			var firmname = row.Cells[0].FormattedValue.ToString();
			var firmcode = Matcher.Firms.FirstOrDefault(f => f.Value.FullName() == firmname).Key;
			Matcher.CreateSynonyms(firmcode);
			Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btnMatchAll_Click(object sender, EventArgs e)
		{
			if (Matcher.Firms.Count <= 0)
				return;
			btnMatchAll.Enabled = false;
			Close();
			Matcher.StartAutoMatching();
		}
	}
}