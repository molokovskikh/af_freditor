using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Test.Support;
using Castle.ActiveRecord;

namespace FREditor.Test
{
	[TestFixture]
	public class DeleteCostColumnFixture
	{
		public static TestPrice CreateTestSupplierWithPrice()
		{
			var priceItem = new TestPriceItem
			{
				Source = new TestPriceSource
				{
					SourceType = PriceSourceType.Email,
				},
				Format = new TestFormat(),
			};

			var priceItem2 = new TestPriceItem
			{
				Source = new TestPriceSource
				{
					SourceType = PriceSourceType.Email,
				},
				Format = new TestFormat(),
			};
			

			var price = new TestPrice
			{
				CostType = CostType.MultiFile,
				Supplier = TestOldClient.CreateTestSupplier(),
				PriceName = "test"
			};
			var cost = price.NewPriceCost(priceItem, "");
			cost.Name = "test base";
			var cost2 = price.NewPriceCost(priceItem2, "");
			cost2.Name = "test";
			
			price.Costs[0].PriceItem.Format.PriceFormat = PriceFormatType.NativeDbf;
			price.Costs[1].PriceItem.Format.PriceFormat = PriceFormatType.NativeDbf;

			price.SaveAndFlush();
			return price;
		} 

		[Test]
		public void Temp()
		{
			using (var scope = new TransactionScope(OnDispose.Rollback))
			{
				TestPrice supplier = CreateTestSupplierWithPrice();

				scope.VoteCommit();
			}


		}

	}
}
