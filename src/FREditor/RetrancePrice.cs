﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;
using Inforoom.WinForms;
using MySql.Data.MySqlClient;
using RemotePriceProcessor;
using log4net;
using log4net.Core;


namespace FREditor
{
	public class RetrancePrice
	{
		private static ILog _log = LogManager.GetLogger(typeof(RetrancePrice));

		public static void Go(MySqlConnection connection, PriceProcessorWcfHelper priceProcessor, uint priceItemId)
		{
			try {
				if (!priceProcessor.RetransPrice(priceItemId, true)) {
					MessageBox.Show(priceProcessor.LastErrorMessage, "Ошибка",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				if (connection.State == ConnectionState.Closed)
					connection.Open();
				try {
					MySqlHelper.ExecuteNonQuery(
						connection,
						"insert into logs.pricesretrans (LogTime, OperatorName, OperatorHost, PriceItemId) values (now(), ?UserName, ?UserHost, ?PriceItemId)",
						new MySqlParameter("?UserName", Environment.UserName),
						new MySqlParameter("?UserHost", Environment.MachineName),
						new MySqlParameter("?PriceItemId", priceItemId));
				}
				finally {
					connection.Close();
				}
#if !DEBUG
				MessageBox.Show("Прайс-лист успешно переподложен.");
#endif
			}
			catch (EndpointNotFoundException ex) {
				SynonymMatcher.ErrorOnConnectToPriceProcessor(_log, ex);
			}
			catch (Exception ex) {
				_log.Error("Ошибка при попытке переподложить прайс-лист", ex);
				MessageBox.Show("Не удалось переподложить прайс-лист. Сообщение об ошибке отправлено разработчику", "Ошибка",
					MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}