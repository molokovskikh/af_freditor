using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace FREditor
{
	public class Encodes
	{
		public static readonly Encoding[] Allow = new[] {
			Encoding.GetEncoding(866),
			Encoding.GetEncoding(1251),
			Encoding.GetEncoding("utf-8")
		};

		public static IList<EncodeSourceType> GetSourceTypes()
		{
			var items = new List<EncodeSourceType> {
				new EncodeSourceType((EncodingInfo)null)
			};
			var encodes = Encoding.GetEncodings()
				.Where(e => Allow.Contains(e.GetEncoding()))
				.Select(e => new EncodeSourceType(e))
				.ToList();
			items.AddRange(encodes);
			return items;
		}
	}

	public class EncodeSourceType
	{
		public EncodeSourceType(Tuple<string, object> item)
		{
			PriceEncode = (int)item.Item2;
			PriceEncodeName = item.Item1;
		}

		public EncodeSourceType(EncodingInfo info)
		{
			if (info != null) {
				PriceEncode = info.CodePage;
				PriceEncodeName = info.DisplayName;
			}
			else {
				PriceEncodeName = "<Не установлена>";
			}
		}

		public int PriceEncode { get; set; }
		public string PriceEncodeName { get; set; }

		public static implicit operator int(EncodeSourceType sourceType)
		{
			return sourceType.PriceEncode;
		}

		public override string ToString()
		{
			return PriceEncode.ToString();
		}
	}

	public static class EnumHelper
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