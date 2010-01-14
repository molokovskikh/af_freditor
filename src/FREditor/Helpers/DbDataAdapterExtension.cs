using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Windows.Forms;
using System.Threading;

namespace FREditor.Helpers
{
	public static class DbDataAdapterExtension
	{
		private delegate void SyncFillAdapter(DbDataAdapter dataAdapter, DataTable dataTable);

		private static void FillAdapter(DbDataAdapter dataAdapter, DataTable dataTable)
		{
			dataAdapter.Fill(dataTable);
		}

		public static void SyncFill(this DbDataAdapter dataAdapter, DataTable dataTable)
		{
			var syncFill = new SyncFillAdapter(FillAdapter);
			var asyncResult = syncFill.BeginInvoke(dataAdapter, dataTable, null, new object());
			while (!asyncResult.IsCompleted)
			{
				Application.DoEvents();
				Thread.Sleep(1000);
			}
		}
	}
}
