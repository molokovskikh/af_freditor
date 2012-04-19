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
using ExcelLibrary.BinaryFileFormat;
using ExcelLibrary.SpreadSheet;

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
				return Dbf.Load(filePath, Encoding.GetEncoding(866), false, true);
			}
			finally
			{
				Application.DoEvents();
			}
		}

		public static string CreateCopyWithoutSpacesAndDots(string filePath)
		{
			string oldpath = Path.GetDirectoryName(filePath);
			string oldfile = Path.GetFileNameWithoutExtension(filePath);
			string ext = Path.GetExtension(filePath);			
			string newfile = oldfile;
			var forbiddenChars = "!#$%^&*()-=+|<>,. \\/@\"`~?{}[]";
			
			if (oldfile != null)
			{
				foreach (var forbiddenChar in forbiddenChars)
				{
					newfile = newfile.Replace(forbiddenChar, '_');
				}
				if (newfile != String.Empty)
				{
					newfile = String.Concat(newfile, ext);					
					if (!File.Exists(Path.Combine(Path.GetDirectoryName(filePath), newfile)))
						File.Copy(filePath, Path.Combine(Path.GetDirectoryName(filePath), newfile));											
					filePath = Path.Combine(Path.GetDirectoryName(filePath), newfile);					
				}
			}
			return filePath;
		}

		private static DataTable OpenTextDelimiterFile(string filePath, PriceFormat? fmt, string delimiter)
		{
			filePath = CreateCopyWithoutSpacesAndDots(filePath);
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
			return ExcelLoader.LoadTables(filePath);
			
		}	
	}
	
	public class ExcelLoader
	{
		public static List<DataTable> LoadTables(string file)
		{
			StringDecoder.DefaultEncoding = Encoding.GetEncoding(1251);
			var workbook = Workbook.Load(file);
			var worksheets = workbook.Worksheets;
			var excelTables = worksheets.Select(worksheet => LoadTable(worksheet)).ToList();
			excelTables.Sort((t1, t2) => t1.TableName.CompareTo(t2.TableName));
			return excelTables;
		}

		private static DataTable LoadTable(Worksheet worksheet, int startLine = 0)
		{			
			var dataTable = new DataTable(worksheet.Name);
			try
			{				
				var cells = worksheet.Cells;
				if (cells.FirstRowIndex != Int32.MaxValue && cells.LastRowIndex != Int32.MaxValue &&
					cells.FirstColIndex != Int32.MaxValue && cells.LastColIndex != Int32.MaxValue)
				{
					var maxColCount = (cells.LastColIndex - cells.FirstColIndex + 1 >= 256)
										? 255
										: cells.LastColIndex - cells.FirstColIndex + 1;

					dataTable.Columns
						.AddRange(Enumerable.Range(cells.FirstColIndex + 1, /*cells.LastColIndex - cells.FirstColIndex + 1*/maxColCount)
									.Select(i => new DataColumn("F" + i))
									.ToArray());

					for (var i = Math.Max(cells.FirstRowIndex, startLine); i <= cells.LastRowIndex; i++)
					{
						var row = dataTable.NewRow();
						for (var j = cells.FirstColIndex; j <= cells.FirstColIndex + maxColCount - 1 /*cells.LastColIndex*/; j++)
						{
							var cell = cells[i, j];

							var columnName = "F" + (j + 1);

							ProcessFormatIfNeeded(columnName, cell, row);
						}
						dataTable.Rows.Add(row);
					}
				}
			}
			finally
			{
				Application.DoEvents();
			}
			return dataTable;
		}

		private static void ProcessFormatIfNeeded(string columnName, Cell cell, DataRow row)
		{
			if (cell.Value is double
				&& cell.Format.FormatType == CellFormatType.Number
				&& (cell.Format.FormatString == "0.00" || cell.Format.FormatString == "#,##0.00"))
			{
				row[columnName] = Math.Round((double)cell.Value, 2, MidpointRounding.AwayFromZero);
				return;
			}

			if (cell.Value is decimal
				&& cell.Format.FormatType == CellFormatType.Custom
				&& cell.Format.FormatString == "#,##0.0")
			{
				row[columnName] = Math.Round((decimal)cell.Value, 1, MidpointRounding.AwayFromZero);
				return;
			}

			if (cell.Value is double
				&& cell.Format.FormatType == CellFormatType.Custom
				&& cell.Format.FormatString == "#,##0.0")
			{
				row[columnName] = Math.Round((double)cell.Value, 1, MidpointRounding.AwayFromZero);
				return;
			}

			if (cell.Value is double
				&& cell.Format.FormatType == CellFormatType.Date
				&& cell.Format.FormatString == "d-mmm")
			{
				var value = cell.TryToGetValueAsDateTime();
				if (value != null)
				{
					row[columnName] = value.Value.ToString("dd.MMM");
					return;
				}
			}

			if (cell.Value is double
				&& cell.Format.FormatType == CellFormatType.Custom
				&& (cell.Format.FormatString == "00000000000"
					|| cell.Format.FormatString == "00000000"
					|| cell.Format.FormatString == "00000"))
			{
				var padCount = cell.FormatString.Length;
				row[columnName] = cell.Value.ToString().PadLeft(padCount, '0');
				return;
			}
			row[columnName] = cell.Value;
		}
	}
}
