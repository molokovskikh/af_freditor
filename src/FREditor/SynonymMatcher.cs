using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace FREditor
{
    public class SynonymMatcher
    {
        private frmFREMain owner;
        private frmMatchProgress frmMatchProgr;        
        private frmMatchResult frmMatchRes;        
        private Timer timer;
        private long currentTask;
        private uint currentPriceCode;
        private uint currentPriceItemId;

        private Dictionary<uint, FirmSummary> firms;
        private MySqlConnection connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["DB"].ConnectionString);

        private bool auto_matching = false;

        public Dictionary<uint, FirmSummary> Firms
        {
            get { return firms; }
        }

        private int iterCount = 0;

        public SynonymMatcher(frmFREMain _owner)
        {
            owner = _owner;
            timer = new Timer();
            timer.Tick += OnTimedEvent;            
            timer.Interval = 3000;
            timer.Stop();
            frmMatchRes = new frmMatchResult();
            frmMatchRes.Owner = owner;
            frmMatchRes.matcher = this;
            firms = new Dictionary<uint, FirmSummary>();

           // frmMatchProgr = new frmMatchProgress();                    
           // frmMatchProgr.Owner = owner;
            
           // frmMatchProgr.Hide();

        }

        public void StartAutoMatching()
        {            
            iterCount = 5;
            StartMatching(currentPriceItemId, currentPriceCode);            
        }

        public bool StartMatching(uint priceItemId, uint priceCode)
        {
            bool ret = false;
            try
            {
                string[] res = owner._priceProcessor.FindSynonyms(Convert.ToUInt32(priceItemId));                
                if (res[0] == "Error")
                {
                    iterCount = 0;
                    MessageBox.Show(res[1], "Ошибка сопоставления синонимов", MessageBoxButtons.OK, MessageBoxIcon.Error);                    
                }
                if(res[0] == "Success")
                {
                    currentTask = Convert.ToInt64(res[1]);
                    currentPriceCode = priceCode;
                    currentPriceItemId = priceItemId;
                    //frmMatchProgr.ShowDialog();
                    frmMatchProgr = new frmMatchProgress();                    
                    frmMatchProgr.Owner = owner;
                    frmMatchProgr.SetMatcher(owner.matcher);
                    timer.Start();
                    ret = true;                    
                    frmMatchProgr.ShowDialog();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Не удалось произвести сопоставление синонимов. Сообщение об ошибке отправлено разработчику",
                                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
#if !DEBUG
					Mailer.SendErrorMessageToService("Ошибка при старте сопоставления", ex);
#endif                
            }
            return ret;
        }

        public void StopMatching()
        {
            try
            {                
                owner._priceProcessor.StopFindSynonyms(currentTask.ToString());
            }
            catch(Exception ex)
            {
                Mailer.SendErrorMessageToService("Ошибка при остановке задачи сопоставления", ex);
            }
        }

        public void AppendToIndex(IList<int> ids)
        {
            try
            {
                IList<string> sids = new List<string>();
                foreach (var id in ids)
                {
                    sids.Add(id.ToString());
                }
                owner._priceProcessor.AppendToIndex(sids.ToArray());
            }
            catch(Exception ex)
            {
                Mailer.SendErrorMessageToService("Ошибка при добавлении индексов", ex);
            }
        }
        
        private void OnTimedEvent(object sender, System.EventArgs e)
        {
            try
            {
                string[] res = owner._priceProcessor.FindSynonymsResult(currentTask.ToString());
                if (res[0] == "Running")
                {
                    frmMatchProgr.SetValue(Convert.ToUInt32(res[1]));
                    return;
                }
                if (res[0] == "Success")
                {
                    timer.Stop();
                 //   frmMatchProgr.Owner = null;
                    frmMatchProgr.Close();
                    frmMatchProgr = null;                                                      
                    FillSummary(res);  // заполняем информацию о совпадениях по поставщикам
                    if(iterCount == 0)
                        frmMatchRes.Fill(firms); // выводим окно со списком совпадений
                    else
                    {
                        if (firms.Count > 0 && iterCount > 0)
                        {
                            iterCount--;
                            CreateSynonyms(firms.First().Key);
                            if (iterCount == 0) return;                            
                            StartMatching(currentPriceItemId, currentPriceCode);
                        }
                        else
                            iterCount = 0;
                    }
                    return;
                }
                if (res[0] == "Error")
                {
                    iterCount = 0;
                    timer.Stop();
                    frmMatchProgr.Close();
                    frmMatchProgr = null;                    
                    MessageBox.Show(res[1], "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (res[0] == "Canceled")
                {
                    iterCount = 0;
                    timer.Stop();
                    frmMatchProgr.Close();
                    frmMatchProgr = null;                                     
                    return;
                }
            }
            catch(Exception ex)
            {
                timer.Stop();
                frmMatchProgr.Close();
                frmMatchProgr = null;                
                MessageBox.Show("Ошибка в процессе сопоставления синонимов. Сообщение об ошибке отправлено разработчику",
                                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
#if !DEBUG
					Mailer.SendErrorMessageToService("Ошибка при сопоставлении", ex);
#endif                
            }            
        }

        private void FillSummary(string[] result)
        {
            firms.Clear();
            int matchCnt = 0;
            foreach (var res in result)
            {
                string[] info = res.Split(';');
                if (info.Length <= 1) continue;
                int firmCnt = Convert.ToInt32(info[0]);
                string synonym = info[info.Length - 1];
                for (int i = 0; i < firmCnt; i++)
                {
                    uint firmCode = Convert.ToUInt32(info[i * 4 + 1]);
                    string firmName = info[i * 4 + 2];
                    uint productId = Convert.ToUInt32(info[i * 4 + 3]);
                    bool junk = Convert.ToBoolean(info[i * 4 + 4]);
                    if (!firms.ContainsKey(firmCode))
                        firms[firmCode] = new FirmSummary(firmName);
                    firms[firmCode].AddInfo(synonym, productId, junk);
                    matchCnt++;
                }
            }
            firms = firms.OrderByDescending(f => f.Value.SynonymCount()).ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        public void CreateSynonyms(uint firmcode)
        {
            if (firmcode != 0)
            {
                IList<SynonymInfo> infoList = firms[firmcode].Summary();
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
                                new MySqlParameter("?PriceCode", currentPriceCode),
                                new MySqlParameter("?Synonym", firmInfo.Synonym),
                                new MySqlParameter("?ProductId", firmInfo.ProductId),
                                new MySqlParameter("?Junk", Convert.ToUInt32(firmInfo.Junk)));
                            int lastId = Convert.ToInt32(MySqlHelper.ExecuteScalar(connection, "select last_insert_id();"));
                            ids.Add(lastId);
                        }
                        catch (MySqlException ex)
                        {
                            if (ex.Number == 1062) continue;
#if !DEBUG
					Mailer.SendErrorMessageToService("Ошибка при создании синонимов", ex);
#endif
                            throw;
                        }
                    }
                    AppendToIndex(ids);
                }
                finally
                {
                    connection.Close();                    
                }
            }
        }
    }


    public struct SynonymInfo
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
            _summary = new List<SynonymInfo>();
        }

        public void AddInfo(string synonym, uint productid, bool junk)
        {
            var info = new SynonymInfo() { Synonym = synonym, ProductId = productid, Junk = junk };
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

        public IList<SynonymInfo> Summary()
        {
            return _summary;
        }

        private readonly string _fullName;
        private readonly IList<SynonymInfo> _summary;
    }
}
