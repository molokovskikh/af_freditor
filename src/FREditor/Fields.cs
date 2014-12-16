using System;
using System.Collections.Generic;
using System.Linq;

namespace FREditor
{
	/// <summary>
	/// Класс необходим для того, чтобы создавать новые поля формализации в редакторе прайсов.
	/// Поначалу кажется, что он не нужен и лучше пользоваться дизайнером в студии.
	/// Но в последствии выяснится, что на самом деле этот самый класс - вселенское благо, а
	/// в дизайнере огромное количество дублирования кода и прочих неудобств.
	///
	/// Если надо добавить поле - делать лучше это здесь.
	/// </summary>
	public class Fields
	{
		public static IEnumerable<Tuple<string, string>> fields
			= new List<Tuple<string, string>> {
				new Tuple<string, string>("EAN13", "Штрих-код"),
				new Tuple<string, string>("Series", "Серия"),
				new Tuple<string, string>("OptimizationSkip", "Флаг исключений оптимизации"),
				new Tuple<string, string>("CodeOKP", "Код ОКП")
			};

		public static IEnumerable<Tuple<string, string>> AdditionalFields()
		{
			return fields;
		}

		public static IEnumerable<string> Additional()
		{
			return fields.Select(f => f.Item1);
		}

		public static IEnumerable<string> Columnds()
		{
			return Additional().Select(f => new[] {
				GeneralColumn(f),
				BeginColumn(f),
				EndColumn(f),
			})
				.SelectMany(i => i);
		}

		public static string BeginColumn(string name)
		{
			return String.Format("Txt{0}Begin", name);
		}

		public static string EndColumn(string name)
		{
			return String.Format("Txt{0}End", name);
		}

		public static string GeneralColumn(string name)
		{
			return String.Format("F{0}", name);
		}
	}
}