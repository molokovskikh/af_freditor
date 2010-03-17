using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Tools;
using Inforoom.WinForms;
using System.Threading;

namespace FREditor.Helpers
{
	public class OpenPriceFileThread
	{
		private string _filePath;
		private PriceFormat? _priceFormat;
		private string _delimiter;
		private DataTableMarking _dataTableMarking;
		private Thread _handle;
		private frmWait _formWait;
		private List<DataTable> _returnValue = null;

		private delegate DialogResult ShowForm();
		private delegate void CloseForm();

		public OpenPriceFileThread(string filePath, PriceFormat? priceFormat, string delimiter, DataTableMarking dataTableMarking)
		{
				_filePath = filePath;
				_priceFormat = priceFormat;
				_delimiter = delimiter;
				_dataTableMarking = dataTableMarking;
				_handle = new Thread(ThreadProc);
				_formWait = new frmWait();
		}

		private void ThreadProc()
		{
			_returnValue = PriceFileHelper.OpenPriceFile(_filePath, _priceFormat, _delimiter, _dataTableMarking);
		}

		public void Start()
		{
				_handle.Start();
				var show = new ShowForm(_formWait.ShowDialog);
				show.BeginInvoke(null, new object());
		}

		public List<DataTable> WaitStop()
		{
			_handle.Join();
			_formWait.Invoke(new CloseForm(_formWait.Close));
			return _returnValue;
		}
	}

	public static class PriceFileHelper
	{
		public static List<DataTable> AsyncOpenPriceFile(string filePath, PriceFormat? priceFormat, string delimiter, DataTableMarking dataTableMarking)
		{
			var openPriceThread = new OpenPriceFileThread(filePath, priceFormat, delimiter, dataTableMarking);
			openPriceThread.Start();
			return openPriceThread.WaitStop();
		}

		public static List<DataTable> OpenPriceFile(string filePath, PriceFormat? priceFormat, string delimiter, DataTableMarking dataTableMarking)
		{
			try
			{
				if (!priceFormat.HasValue)
					throw new Exception(String.Format("Неизвестный формат {0}", priceFormat));
				var tables = new List<DataTable>();
				if ((priceFormat.Value == PriceFormat.DBF) || (priceFormat.Value == PriceFormat.NativeDbf))
				{					
					tables.Add(OpenDbfFile(filePath));
					return tables;
				}
				if ((priceFormat.Value == PriceFormat.XLS) || (priceFormat.Value == PriceFormat.NativeXls))
				{					
					return OpenXlsFile(filePath);
				}
				if ((priceFormat.Value == PriceFormat.DelimDOS) || (priceFormat.Value == PriceFormat.DelimWIN) ||
                    (priceFormat.Value == PriceFormat.NativeDelimWIN) || (priceFormat.Value == PriceFormat.NativeDelimDOS))
				{
                    tables.Add(OpenTextDelimiterFile(filePath, priceFormat, delimiter));
					return tables;
				}
				if ((priceFormat.Value == PriceFormat.FixedDOS) || (priceFormat.Value == PriceFormat.FixedWIN) ||
                    (priceFormat.Value == PriceFormat.NativeFixedDOS) || (priceFormat.Value == PriceFormat.NativeFixedWIN))
				{
					tables.Add(OpenTextFixedFile(filePath, priceFormat, dataTableMarking));
					return tables;
				}
				throw new Exception(String.Format("Неизвестный формат {0}", priceFormat));
			}
			catch (Exception ex)
			{
				Mailer.SendErrorMessageToService(String.Format(
					"Ошибка при открытии файла\nЛокальный путь:{0}\nФормат:{1}\nРазделитель:{2}", filePath, priceFormat, delimiter), ex);
				return null;
			}
		}

		private static DataTable OpenDbfFile(string filePath)
		{
			Application.DoEvents();
			try
			{
				return Dbf.Load(filePath);
			}
			finally
			{
				Application.DoEvents();
			}
		}

		private static DataTable OpenTextDelimiterFile(string filePath, PriceFormat? fmt, string delimiter)
		{
			var fileName = Path.GetDirectoryName(filePath) + Path.DirectorySeparatorChar + "Schema.ini";
			using (var w = new StreamWriter(fileName, false, Encoding.GetEncoding(1251)))
			{
				w.WriteLine("[" + Path.GetFileName(filePath) + "]");
				w.WriteLine(((fmt == PriceFormat.DelimWIN) || ((fmt == PriceFormat.NativeDelimWIN)))
				            	? "CharacterSet=ANSI"
				            	: "CharacterSet=OEM");
				w.WriteLine(("TAB" == delimiter.ToUpper()) ? "Format=TabDelimited" : "Format=Delimited(" + delimiter + ")");
				w.WriteLine("ColNameHeader=False");
				w.WriteLine("MaxScanRows=300");
			}

			string replaceFile;
			using (var reader = new StreamReader(filePath, Encoding.GetEncoding(1251)))
			{
				replaceFile = reader.ReadToEnd();
			}
			replaceFile = replaceFile.Replace("\"", "");

			using (var sw = new StreamWriter(filePath, false, Encoding.GetEncoding(1251)))
			{
				sw.Write(replaceFile);
			}

			int maxColCount = 0;
			var tableName = Path.GetFileName(filePath).Replace(".", "#");

			var connectionFormatString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Text\"";
			var connectionString = String.Format(connectionFormatString, Path.GetDirectoryName(filePath));
			var connection = new OleDbConnection(connectionString);
			Application.DoEvents();
			connection.Open();
			DataTable ColumnNames = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns,
			                                                       new object[] {null, null, tableName, null});
			maxColCount = (ColumnNames.Rows.Count >= 256) ? 255 : ColumnNames.Rows.Count;
			connection.Close();
			Application.DoEvents();

			using (var w = new StreamWriter(Path.GetDirectoryName(filePath) + Path.DirectorySeparatorChar + "Schema.ini",
			                                false, Encoding.GetEncoding(1251)))
			{
				w.WriteLine("[" + Path.GetFileName(filePath) + "]");
				w.WriteLine(((fmt == PriceFormat.DelimWIN) || ((fmt == PriceFormat.NativeDelimWIN)))
				            	? "CharacterSet=ANSI"
				            	: "CharacterSet=OEM");
				w.WriteLine(("TAB" == delimiter.ToUpper()) ? "Format=TabDelimited" : "Format=Delimited(" + delimiter + ")");
				w.WriteLine("ColNameHeader=False");
				w.WriteLine("MaxScanRows=300");
				for (var i = 0; i <= maxColCount; i++)
				{
					w.WriteLine("Col{0}=F{0} Text", i);
				}
			}

			Application.DoEvents();
			connection.ConnectionString = String.Format(connectionFormatString, Path.GetDirectoryName(filePath));
			connection.Open();
			var dataAdapter = new OleDbDataAdapter(String.Format(
			                                       	"select * from {0}", Path.GetFileName(filePath).Replace(".", "#")),
			                                       connection);
			var	dataTablePrice = new DataTable();
			dataAdapter.SyncFill(dataTablePrice);
			if (dataTablePrice.Rows.Count == 0)
				throw new Exception("При открытии файла таблица с полями прайс-листа оказалась пуста. Предположительно неверный формат или файл поврежден");
			connection.Close();
			Application.DoEvents();
			return dataTablePrice;
		}

		private static DataTable OpenTextFixedFile(string filePath, PriceFormat? priceFormat, DataTableMarking dataTableMarking)
		{
			return OpenPriceTable(filePath, dataTableMarking, priceFormat);
		}

		public static DataTable OpenPriceTable(string filePath, DataTableMarking dataTableMarking, PriceFormat? priceFormat)
		{
			DataTable dataTablePrice = null;
			if (dataTableMarking.Rows.Count > 1)
			{
				var fields = new ArrayList();
				for (var i = 0; i < dataTableMarking.Rows.Count; i++)
				{
					var row = dataTableMarking.Rows[i];
					var fieldName = row["MNameField"].ToString();
					var beginPosition = Convert.ToInt32(row["MBeginField"]);
					var endPosition = Convert.ToInt32(row["MEndField"]);
					fields.Add(new TxtFieldDef(fieldName, beginPosition, endPosition));
				}
				fields.Sort(new TxtFieldDef());

				var schemaFile = Path.GetDirectoryName(filePath) + Path.DirectorySeparatorChar + "Schema.ini";
				using (var w = new StreamWriter(schemaFile, false, Encoding.GetEncoding(1251)))
				{
					w.WriteLine("[" + Path.GetFileName(filePath) + "]");
					w.WriteLine(((priceFormat == PriceFormat.FixedWIN) || (priceFormat == PriceFormat.NativeFixedWIN)) ? 
						"CharacterSet=ANSI" : "CharacterSet=OEM");
					w.WriteLine("Format=FixedLength");
					w.WriteLine("ColNameHeader=False");
					w.WriteLine("MaxScanRows=300");

					if (fields.Count > 0)
					{
						int j = 1;
						TxtFieldDef prevTFD, currTFD = (TxtFieldDef)fields[0];

						if (1 == currTFD.posBegin)
						{
							w.WriteLine(String.Format("Col{0}={1} Text Width {2}", j, currTFD.fieldName, currTFD.posEnd));
							j++;
						}
						else
						{
							w.WriteLine(String.Format("Col{0}={1} Text Width {2}", j, "x", currTFD.posBegin - 1));
							j++;
							w.WriteLine(String.Format("Col{0}={1} Text Width {2}", j, currTFD.fieldName, currTFD.posEnd - currTFD.posBegin + 1));
							j++;
						}

						for (var i = 1; i <= fields.Count - 1; i++)
						{
							prevTFD = (TxtFieldDef)fields[i - 1];
							currTFD = (TxtFieldDef)fields[i];
							if (currTFD.posBegin == prevTFD.posEnd + 1)
							{
								w.WriteLine(String.Format("Col{0}={1} Text Width {2}", j, currTFD.fieldName, currTFD.posEnd - currTFD.posBegin + 1));
								j++;
							}
							else
							{
								w.WriteLine(String.Format("Col{0}={1} Text Width {2}", j, "x", currTFD.posBegin - prevTFD.posEnd - 1));
								j++;
								w.WriteLine(String.Format("Col{0}={1} Text Width {2}", j, currTFD.fieldName, currTFD.posEnd - currTFD.posBegin + 1));
								j++;
							}
						}
					}
					else
					{
						w.WriteLine(String.Format("Col{0}=x1 Text Width {1}", 1, 255));
					}
				}

				var connectionString = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Text\"", 
					Path.GetDirectoryName(filePath));
				using (var connection = new OleDbConnection(connectionString))
				{
					connection.Open();
					var dataAdapter = new OleDbDataAdapter(String.Format("select * from {0}",
						Path.GetFileName(filePath).Replace(".", "#")), connection);
					dataTablePrice = new DataTable();
					dataAdapter.SyncFill(dataTablePrice);
					if (dataTablePrice.Rows.Count == 0)
						throw new Exception("При открытии файла таблица с полями прайс-листа оказалась пуста. Предположительно неверный формат или файл поврежден");
				}
			}
			return dataTablePrice;
		}

		private static List<DataTable> OpenXlsFile(string filePath)
		{
			Application.DoEvents();
			var excelTables = new List<DataTable>();
			var connectionString = String.Format(
				"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 5.0;HDR=No;IMEX=1\"", filePath);
			using (var connection = new OleDbConnection(connectionString))
			{
					connection.Open();
					try
					{
						Application.DoEvents();
						var tableNames = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] {null, null, null, "TABLE"});						
						for (var i = 0; i < tableNames.Rows.Count; i++)
						{
							var row = tableNames.Rows[i];
							if (!(row["TABLE_NAME"] is DBNull))
							{
								var sheetName = (string) row["TABLE_NAME"];
								var table = new DataTable(sheetName);
								excelTables.Add(table);
								FillXlsSheetTable(connection, table, sheetName);
							}
							Application.DoEvents();
						}
					}
					finally
					{
						Application.DoEvents();
					}
			}
			return excelTables;
		}

		private static void FillXlsSheetTable(OleDbConnection connection, DataTable table, string sheetName)
		{
			try
			{
				var columnNames = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns,
					new object[] { null, null, sheetName, null });
				var fieldNames = "F1";
				var maxColCount = (columnNames.Rows.Count >= 256) ? 255 : columnNames.Rows.Count;
				for (var i = 1; i < maxColCount; i++)
				{
					fieldNames = fieldNames + ", F" + Convert.ToString(i + 1);
				}
				var dataAdapter = new OleDbDataAdapter(String.Format("select {0} from [{1}]", fieldNames, sheetName), connection);
				dataAdapter.SyncFill(table);
			}
			catch {}
		}
		
		/**/
	}
}
