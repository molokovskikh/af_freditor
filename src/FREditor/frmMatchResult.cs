using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace FREditor
{
    public partial class frmMatchResult : Form
    {
        public SynonymMatcher matcher = null;

        public frmMatchResult()
        {
            InitializeComponent();
        }        

        public void Fill(Dictionary<uint, FirmSummary> firms)
        {
            matchResGV.Rows.Clear();

            int matchCnt = firms.Sum(firm => firm.Value.SynonymCount()); // общее количество найденых совпадений           

            if(matchCnt == 0)
            {
                MessageBox.Show("Не найдено совпадений с имеющимися синонимами", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (var firm in firms)
            {
                int idx = matchResGV.Rows.Add();
                decimal cnt = firm.Value.SynonymCount();
                decimal val = Math.Round(cnt*100/matchCnt, 2);
                matchResGV[0, idx].Value = firm.Value.FullName();
                matchResGV[1, idx].Value = val.ToString();
            }

            Text = String.Format("Результат сопоставления (сопоставлено позиций: {0})", matchCnt);
            ShowDialog();
        }

        private void matchResGV_DoubleClick(object sender, EventArgs e)
        {
            var row = matchResGV.CurrentRow;
            string firmname = row.Cells[0].FormattedValue.ToString();
            uint firmcode;            
            firmcode = matcher.Firms.Where(f => f.Value.FullName() == firmname).FirstOrDefault().Key;
            matcher.CreateSynonyms(firmcode);
            Close();            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnMatchAll_Click(object sender, EventArgs e)
        {            
            if(matcher.Firms.Count > 0)
            {
                matcher.CreateSynonyms(matcher.Firms.First().Key);
                matcher.StartAutoMatching();                
            }
            Close();
        }
    }
}
