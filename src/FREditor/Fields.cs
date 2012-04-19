using System;
using System.Collections.Generic;
using System.Linq;

namespace FREditor
{
	public class Fields
	{
		public static IEnumerable<string> Additional()
		{
			return new[] { "EAN13", "Series", "CodeOKP" };
		}

		public static IEnumerable<string> Columnds()
		{
			return Additional().Select(f => new [] {
				String.Format("F{0}", f),
				String.Format("Txt{0}Begin", f),
				String.Format("Txt{0}End", f),
			})
				.SelectMany(i => i);
		}
	}
}