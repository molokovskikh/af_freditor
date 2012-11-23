using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using NHibernate.Linq;
using NUnit.Framework;
using Test.Support;
using Test.Support.Suppliers;

namespace FREditor.Test
{
	[TestFixture]
	public class PriceDateWithBiasFixture : IntegrationFixture
	{
		private frmFREMain form;

		[SetUp]
		public void Setup()
		{
			var supplier = TestSupplier.Create();
			supplier.Prices[0].CostType = CostType.MultiFile;
			supplier.Save();

			form = new frmFREMain();
			form.Form1_Load(form, null);
		}

		[Test, STAThread]
		public void PriceDateWithBiasTest()
		{
			form.dtClientsFill("", false);
			form.dtPricesFill("", false);

			int i = 0;
			foreach (DataRow dataRow in form.DTPrices.Rows) {
				var supplier = session.Query<TestSupplier>().SingleOrDefault(s => s.Id == Convert.ToUInt32(dataRow["PFirmCode"]));
				if(!String.IsNullOrEmpty(dataRow["PPriceDate"].ToString())) {
					var time = DateTime.Parse(dataRow["PPriceDate"].ToString());
					var biasTime = DateTime.Parse(dataRow["PPriceDateWithBias"].ToString());
					Assert.That(time.AddHours(supplier.HomeRegion.MoscowBias), Is.EqualTo(biasTime));
				}
				if(i++ > 30)
					break;
			}
		}
	}
}