using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using  TestSupport = Test.Support;

namespace FREditor.Test
{
	[SetUpFixture]
	public class FixtureSetup
	{
		[SetUp]
		public void Setup()
		{	
			TestSupport.Setup.Initialize();
		}
	}
}
