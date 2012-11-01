using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FREditor
{
	public class CustomConstraintException : Exception
	{
		public CustomConstraintException(string message)
			: base(message)
		{
		}

		public CustomConstraintException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}