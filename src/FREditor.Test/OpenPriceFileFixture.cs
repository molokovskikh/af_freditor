using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FREditor.Helpers;
using NUnit.Framework;
using FREditor.Test.Properties;
using System.IO;

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
			var countsTables = new int[3] { 1, 3, 1 };
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
	}
}
