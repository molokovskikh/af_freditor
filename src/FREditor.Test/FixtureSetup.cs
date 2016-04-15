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
			Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
			With.DefaultConnectionStringName = ConnectionHelper.GetConnectionName();
		}
	}
}