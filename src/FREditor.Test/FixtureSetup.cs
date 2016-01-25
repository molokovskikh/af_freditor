using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.MySql;
using NUnit.Framework;
using TestSupport = Test.Support;

namespace FREditor.Test
{
	[SetUpFixture]
	public class FixtureSetup
	{
		[OneTimeSetUp]
		public void Setup()
		{
			TestSupport.Setup.Initialize();
			With.DefaultConnectionStringName = ConnectionHelper.GetConnectionName();
		}
	}
}