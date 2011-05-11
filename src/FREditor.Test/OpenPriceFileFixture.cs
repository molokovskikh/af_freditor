using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FREditor.Helpers;
using NUnit.Framework;
using FREditor.Test.Properties;
using System.IO;
using System.Data;

namespace FREditor.Test
{
	[TestFixture]
	public class OpenPriceFileFixture
	{
		[Test]
		public void Open_price_file()
		{
			var pricesDirectory = "prices";
			var invalidPricesDirectory = "invalid_prices";
			var pricesFiles = new string[3] {"1.txt", "2.xls", "3.dbf"};
			var countsTables = new int[3] { 1, 2, 1 };
			var pricesFormats = new PriceFormat[3] { PriceFormat.DelimWIN, PriceFormat.XLS, PriceFormat.NativeDbf };
			var indvalidPricesFiles = new string[3] {"1.xls", "2.xls", "3.txt"};
			var index = 0;
			
			foreach (var filename in pricesFiles)
			{
				var path = Settings.Default.TestDirectoryPath + pricesDirectory + Path.DirectorySeparatorChar + filename;
				var tables = PriceFileHelper.OpenPriceFile(Path.GetFullPath(path), pricesFormats[index], "tab", null);
				Assert.That(tables.Count, Is.EqualTo(countsTables[index++]));
				Assert.That(tables[0].Rows.Count, Is.GreaterThan(0));
			}
			index = 0;
			foreach (var filename in indvalidPricesFiles)
			{
				var path = Settings.Default.TestDirectoryPath + invalidPricesDirectory + Path.DirectorySeparatorChar + filename;
				var tables = PriceFileHelper.OpenPriceFile(Path.GetFullPath(path), pricesFormats[index++], "tab", null);
				if (tables != null)
				{
					Assert.That(tables.Count, Is.EqualTo(1));
					Assert.That(tables[0].Rows.Count, Is.EqualTo(0));
				}
			}
		}

		[Test]
		public void OpenXlsFile()
		{
			var pricesDirectory = "prices";
			var filename = "1236.xls";
			var path = Settings.Default.TestDirectoryPath + pricesDirectory + Path.DirectorySeparatorChar + filename;
			List<DataTable> tables = ExcelLoader.LoadTables(path);
			Assert.That(tables.Count, Is.EqualTo(18));
			var table = tables.Where(t => t.TableName == "Лист1").FirstOrDefault();
			var rows = table.Rows;
			Assert.That(rows.Count, Is.EqualTo(2));
			var row = rows[0];
			string value = Convert.ToString(row[0]);
			Assert.That(value, Is.EqualTo("Абилифай 15мг №28 табл."));
			value = Convert.ToString(row[1]);
			Assert.That(value, Is.EqualTo("6330"));
			row = rows[1];
			value = Convert.ToString(row[0]);
			Assert.That(value, Is.EqualTo("Абитаксел - Тева д/инф. 100мг/16.7мл фл.№1"));
			value = Convert.ToString(row[1]);
			Assert.That(value, Is.EqualTo("11000="));		
		}

		[Test]
		public void CreateCopyWithoutSpaces()
		{
			var path = Settings.Default.TestDirectoryPath + "Тестовый файл";
			File.Delete(path);
			using (var w = new StreamWriter(path, false, Encoding.GetEncoding(1251)))
			{
				w.WriteLine("Тестовый файл");
			}
			string path2 = PriceFileHelper.CreateCopyWithoutSpaces(path);

			Assert.That(File.Exists(path), Is.True);
			Assert.That(Path.GetFileName(path2), Is.EqualTo("Тестовый_файл"));
			Assert.That(File.Exists(path2), Is.True);

			string path3 = PriceFileHelper.CreateCopyWithoutSpaces(path);

			File.Delete(path);
			File.Delete(path2);
		}
	}

}
