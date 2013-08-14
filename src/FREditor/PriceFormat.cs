﻿namespace FREditor
{
	public enum PriceEncode
	{
		CoNo = 0,
		Cp866 = 866,
		Cp1251 = 1251
	}

	public enum PriceFormat
	{
		DelimWIN = 1,
		DelimDOS,
		XLS,
		DBF,
		XML,
		FixedWIN,
		FixedDOS,
		NativeDbf,
		Sudakov,
		NativeXls,
		/*NativeDelimWIN,
		NativeDelimDOS,
		NativeFixedWIN,
		NativeFixedDOS,*/
		UniversalFormalizer = 16,
		FarmaimpeksOKPFormalizer = 17,
		NativeDelim = 18,
		NativeFixed = 19
	}
}