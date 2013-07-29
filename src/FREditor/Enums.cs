using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FREditor
{
	public enum Encodes
	{
		[Description("CP 866")]
		Cp866 = 0,
		[Description("CP 1251")]
		Cp1251 = 1
	}

	public class EncodeSourceType
	{
		public EncodeSourceType(Tuple<string, object> item)
		{
			PriceEncode = (int)item.Item2;
			PriceEncodeName = item.Item1;
		}

		public int PriceEncode { get; set; }
		public string PriceEncodeName { get; set; }
	}

	public static class EnulHelper
	{
		public static object[] AllElements<T>(Func<Tuple<string, object>, object> constructor)
		{
			var result = new List<Tuple<string, object>>();
			foreach (var value in Enum.GetValues(typeof(T)).Cast<Enum>()) {
				result.Add(new Tuple<string, object>(value.GetDescription(), value));
			}
			return result.Select(i => constructor(i)).ToArray();
		}
	}
}
