﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using log4net;
using RemotePriceProcessor;

namespace FREditor
{
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
		private readonly MySqlConnection connection = new MySqlConnection(Literals.ConnectionString());

		private int iterCount;
		private bool matching;

		private ILog _logger = LogManager.GetLogger(typeof(SynonymMatcher));

		public Dictionary<uint, FirmSummary> Firms
		{
			get { return firms; }
		}

		public SynonymMatcher(frmFREMain _owner)
		{
			owner = _owner;
			timer = new Timer();
			timer.Tick += OnTimedEvent;
			timer.Interval = 3000;
			timer.Stop();
			frmMatchRes = new frmMatchResult { Owner = owner, Matcher = this };
			frmMatchProgr = new frmMatchProgress { Owner = owner, Matcher = this };
			firms = new Dictionary<uint, FirmSummary>();
		}

		public void CloseProgressBar()
		{
			frmMatchProgr.SetValue(0);
			frmMatchProgr.Close();
		}

		public void Start(uint priceItemId, uint priceCode)
		{
			currentPriceCode = priceCode;
			currentPriceItemId = priceItemId;
			matching = true;
			timer.Start();
		}

		public void StartAutoMatching()
		{
			CreateSynonyms(Firms.First().Key);
			iterCount = 5;
			matching = true;
			timer.Start();
		}

		private bool StartMatching(uint priceItemId, uint priceCode)
		{
			bool ret = false;
			try {
				string[] res = owner._priceProcessor.FindSynonyms(Convert.ToUInt32(priceItemId));
				if (res[0] == "Error") {
					iterCount = 0;
					timer.Stop();
					MessageBox.Show(res[1], "Ошибка сопоставления синонимов", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				if (res[0] == "Success") {
					currentTask = Convert.ToInt64(res[1]);
					timer.Start();
					ret = true;
				}
			}
			catch (EndpointNotFoundException ex) {
				iterCount = 0;
				timer.Stop();
				ErrorOnConnectToPriceProcessor(_logger, ex);
			}
			catch (Exception ex) {
				_logger.Error("Ошибка при старте сопоставления", ex);
				iterCount = 0;
				timer.Stop();
				MessageBox.Show("Не удалось произвести сопоставление синонимов. Сообщение об ошибке отправлено разработчику",
					"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			return ret;
		}

		public void StopMatching()
		{
			try {
				owner._priceProcessor.StopFindSynonyms(currentTask.ToString());
			}
			catch (Exception ex) {
				_logger.Error("Ошибка при остановке задачи сопоставления", ex);
			}
		}

		public void AppendToIndex(IList<int> ids)
		{
			try {
				owner._priceProcessor.AppendToIndex(ids.Select(id => id.ToString()).ToArray());
			}
			catch (Exception ex) {
				_logger.Error("Ошибка при добавлении индексов", ex);
			}
		}

		private void OnTimedEvent(object sender, System.EventArgs e)
		{
			try {
				if (matching) {
					if (StartMatching(currentPriceItemId, currentPriceCode)) {
						matching = false;
						if (!frmMatchProgr.Modal)
							frmMatchProgr.ShowDialog();
					}
					else
						timer.Stop();
					return;
				}

				WcfSynonymBox res = owner._priceProcessor.FindSynonymsResult(currentTask.ToString());
				if (res.Status == TaskState.Running) {
					frmMatchProgr.SetValue(Convert.ToUInt32(res.Message));
					owner.EnableMatchBtn();
					return;
				}
				if (res.Status == TaskState.Success) {
					timer.Stop();
					FillSummary(res); // заполняем информацию о совпадениях по поставщикам
					if (iterCount == 0) {
						frmMatchRes.Fill(firms); // выводим окно со списком совпадений
						CloseProgressBar();
					}
					else if (firms.Count > 0 && iterCount > 0) {
						iterCount--;
						CreateSynonyms(firms.First().Key);
						if (iterCount == 0) {
							CloseProgressBar();
							return;
						}
						StartMatching(currentPriceItemId, currentPriceCode);
					}
					else {
						iterCount = 0;
						CloseProgressBar();
					}

					return;
				}
				if (res.Status == TaskState.Error) {
					iterCount = 0;
					timer.Stop();
					CloseProgressBar();
					MessageBox.Show(res.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				if (res.Status == TaskState.Canceled) {
					iterCount = 0;
					timer.Stop();
					CloseProgressBar();
				}
			}
			catch (EndpointNotFoundException ex) {
				timer.Stop();
				CloseProgressBar();
				ErrorOnConnectToPriceProcessor(_logger, ex);
			}
			catch (Exception ex) {
				_logger.Error("Ошибка при сопоставлении", ex);
				timer.Stop();
				CloseProgressBar();
				MessageBox.Show("Ошибка в процессе сопоставления синонимов. Сообщение об ошибке отправлено разработчику",
					"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void FillSummary(WcfSynonymBox result)
		{
			firms.Clear();
			int matchCnt = 0;
			foreach (var res in result.SynonymBox) {
				var info = res.SynonymList;
				foreach (var item in info) {
					if (!firms.ContainsKey(item.FirmCode)) {
						firms[item.FirmCode] = new FirmSummary(item.FirmName);
						firms[item.FirmCode].AddInfo(res.OriginalName, item.ProductId, item.Junk);
					}
				}
			}
			firms = firms.OrderByDescending(f => f.Value.SynonymCount()).ToDictionary(pair => pair.Key, pair => pair.Value);
		}

		public void CreateSynonyms(uint firmcode)
		{
			if (firmcode != 0) {
				var infoList = firms[firmcode].Summary();
				connection.Open();
				try {
					IList<int> ids = new List<int>();
					foreach (var firmInfo in infoList) {
						try {
							MySqlHelper.ExecuteNonQuery(
								connection,
								"insert into farm.synonym (PriceCode, Synonym, ProductId, Junk) values (?PriceCode, ?Synonym, ?ProductId, ?Junk)",
								new MySqlParameter("?PriceCode", currentPriceCode),
								new MySqlParameter("?Synonym", firmInfo.Synonym),
								new MySqlParameter("?ProductId", firmInfo.ProductId),
								new MySqlParameter("?Junk", Convert.ToUInt32(firmInfo.Junk)));
							var lastId = Convert.ToInt32(MySqlHelper.ExecuteScalar(connection, "select last_insert_id();"));
							ids.Add(lastId);
						}
						catch (MySqlException ex) {
							if (ex.Number == 1062)
								continue;
							throw;
						}
					}
					AppendToIndex(ids);
				}
				finally {
					connection.Close();
				}
			}
		}

		public static void ErrorOnConnectToPriceProcessor(ILog logger, Exception ex)
		{
			logger.Error("Ошибка при подключении к PriceProcessor", ex);
			MessageBox.Show("Не удалось подключиться к PriceProcessor, возможно он остановлен. Повторите операцию позднее.",
				"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
}