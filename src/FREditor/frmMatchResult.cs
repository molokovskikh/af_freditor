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
        private readonly Dictionary<uint, FirmSummary> firms;
        private int matchCnt = 0;

        private MySqlConnection connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);

        public SynonymMatcher matcher = null;

        public frmMatchResult()
        {
            InitializeComponent();
            firms = new Dictionary<uint, FirmSummary>();
        }

        public uint PriceCode { get; set; }

        public void Fill(string[] result)
        {
            matchResGV.Rows.Clear();
            firms.Clear();
            matchCnt = 0;
            foreach (var res in result)
            {
                string[] info = res.Split(';');
                if (info.Length <= 1) continue;                
                int firmCnt = Convert.ToInt32(info[0]);
                string synonym = info[info.Length - 1];
                for (int i = 0; i < firmCnt; i++)
                {
                    uint firmCode = Convert.ToUInt32(info[i*4 + 1]);
                    string firmName = info[i*4 + 2];
                    uint productId = Convert.ToUInt32(info[i*4 + 3]);
                    bool junk = Convert.ToBoolean(info[i*4 + 4]);
                    if(!firms.ContainsKey(firmCode))
                        firms[firmCode] = new FirmSummary(firmName);
                    firms[firmCode].AddInfo(synonym, productId, junk);
                    matchCnt++;
                }
            }
            
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
            uint firmcode = 0;
            foreach(var firm in firms)
            {
                if (firm.Value.FullName() == firmname)
                    firmcode = firm.Key;
            }
            if(firmcode != 0)
            {
                IList<FirmInfo> infoList = firms[firmcode].Summary();
                connection.Open();
                try
                {
                    IList<int> ids = new List<int>();
                    foreach (var firmInfo in infoList)
                    {
                        try
                        {
                            MySqlHelper.ExecuteNonQuery(
                                connection,
                                "insert into farm.synonym (PriceCode, Synonym, ProductId, Junk) values (?PriceCode, ?Synonym, ?ProductId, ?Junk)",
                                new MySqlParameter("?PriceCode", PriceCode),
                                new MySqlParameter("?Synonym", firmInfo.Synonym),
                                new MySqlParameter("?ProductId", firmInfo.ProductId),
                                new MySqlParameter("?Junk", Convert.ToUInt32(firmInfo.Junk)));
                            int lastId = Convert.ToInt32(MySqlHelper.ExecuteScalar(connection, "select last_insert_id();"));
                            ids.Add(lastId);
                        }
                        catch(MySqlException ex)
                        {
                            if (ex.Number == 1062) continue;
#if !DEBUG
					Mailer.SendErrorMessageToService("Ошибка при создании синонимов", ex);
#endif
                            throw;
                        }
                    }
                    matcher.AppendToIndex(ids);
                }
                finally
                {
                    connection.Close();
                    Close();
                }
            }
        }
    }

    public struct FirmInfo
    {
        public string Synonym;
        public uint ProductId;
        public bool Junk;
    }

    public class FirmSummary
    {
        public FirmSummary(string name)
        {
            _fullName = name;
            _summary = new List<FirmInfo>();
        }

        public void AddInfo(string synonym, uint productid, bool junk)
        {
            var info = new FirmInfo() { Synonym = synonym, ProductId = productid, Junk = junk };
            _summary.Add(info);
        }

        public string FullName()
        {
            return _fullName;
        }

        public int SynonymCount()
        {
            return _summary.Count();
        }

        public IList<FirmInfo> Summary()
        {
            return _summary;
        }

        private readonly string _fullName;
        private readonly IList<FirmInfo> _summary;
    }
}
