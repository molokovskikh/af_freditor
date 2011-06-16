using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
        }

        public void StartMatching(uint priceItemId, uint priceCode)
        {
            try
            {
                string[] res = owner._priceProcessor.FindSynonyms(Convert.ToUInt32(priceItemId));                
                if (res[0] == "Error")
                {
                    MessageBox.Show(res[1], "Ошибка сопоставления синонимов", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if(res[0] == "Success")
                {
                    currentTask = Convert.ToInt64(res[1]);
                    currentPriceCode = priceCode;
                    frmMatchProgr = new frmMatchProgress();                    
                    frmMatchProgr.Owner = owner;
                    frmMatchProgr.SetMatcher(owner.matcher);
                    timer.Start();
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
                    frmMatchProgr.Close();
                    frmMatchProgr = null;
                    frmMatchRes.PriceCode = currentPriceCode;
                    frmMatchRes.Fill(res);
                }
                if (res[0] == "Error")
                {
                    timer.Stop();
                    frmMatchProgr.Close();
                    frmMatchProgr = null;
                    MessageBox.Show(res[1], "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (res[0] == "Canceled")
                {
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
    }
}
