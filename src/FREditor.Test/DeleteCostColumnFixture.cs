using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.MySql;
using NUnit.Framework;
using Test.Support;
using Castle.ActiveRecord;
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;
using Test.Support.Suppliers;

namespace FREditor.Test
{
	[TestFixture]
	public class DeleteCostColumnFixture
	{
		private TestPriceCost costForDelete;

		[SetUp]
		public void Setup()
		{
		}

		private DataTable FillPrices()
		{
			var table = new DataTable();
			With.Connection(c => {
				DbHelper.PricesFill(c, table, "", false, 0);
			});
			return table;
		}

		public TestPrice CreateTestSupplierWithPrice()
		{
			var priceItem = new TestPriceItem {
				Source = new TestPriceSource {
					SourceType = PriceSourceType.Email,
				},
				Format = new TestFormat(),
			};

			var priceItem2 = new TestPriceItem {
				Source = new TestPriceSource {
					SourceType = PriceSourceType.Email,
				},
				Format = new TestFormat(),
			};

			var supplier = TestSupplier.CreateNaked();
			var price = supplier.Prices[0];

			price.CostType = CostType.MultiFile;
			price.PriceName = "test";
			price.Costs.Clear();

			var cost = price.NewPriceCost(priceItem, "");
			cost.Name = "test base";
			price.RegionalData[0].BaseCost = cost;

			costForDelete = price.NewPriceCost(priceItem2, "");
			costForDelete.Name = "test";

			price.Costs[0].PriceItem.Format.PriceFormat = PriceFormatType.NativeDbf;
			price.Costs[1].PriceItem.Format.PriceFormat = PriceFormatType.NativeDbf;

			price.SaveAndFlush();

			return price;
		}

		[Test]
		public void DeleteCostColumn()
		{
			TestPrice price;
			using (var scope = new TransactionScope(OnDispose.Rollback)) {
				price = CreateTestSupplierWithPrice();
				scope.VoteCommit();
			}
			var prices = FillPrices();
			var rows = prices.Select(String.Format("PFirmCode = {0}", price.Supplier.Id));
			Assert.That(rows.Count(), Is.EqualTo(2));
			rows = prices.Select(String.Format("PFirmCode = {0} and PCostType = 1 and PIsParent = 0", price.Supplier.Id));
			Assert.That(rows.Count(), Is.EqualTo(1));
			rows[0].Delete();

			var changes = prices.GetChanges();

			With.Transaction((c, t) => {
				var mcmdDPrice = new MySqlCommand();
				var daPrice = new MySqlDataAdapter();

				mcmdDPrice.CommandText = "usersettings.DeleteCost";
				mcmdDPrice.CommandType = CommandType.StoredProcedure;
				mcmdDPrice.Parameters.Clear();
				mcmdDPrice.Parameters.Add("?inCostCode", MySqlDbType.Int64, 0, "PCostCode");
				mcmdDPrice.Parameters["?inCostCode"].Direction = ParameterDirection.Input;

				mcmdDPrice.Connection = c;
				daPrice.DeleteCommand = mcmdDPrice;
				daPrice.TableMappings.Clear();
				daPrice.TableMappings.Add("Table", prices.TableName);

				daPrice.Update(changes);
				prices.AcceptChanges();
			});


			using (var scope = new TransactionScope(OnDispose.Rollback)) {
				var resprice = TestPrice.Find(price.Id);
				Assert.That(resprice.Costs.Count, Is.EqualTo(1));

				var cost = TestPriceCost.TryFind(costForDelete.Id);
				Assert.That(cost, Is.EqualTo(null));

				var item = TestPriceItem.TryFind(costForDelete.PriceItem.Id);
				Assert.That(item, Is.EqualTo(null));

				var source = TestPriceSource.TryFind(costForDelete.PriceItem.Source.Id);
				Assert.That(source, Is.EqualTo(null));

				var format = TestFormat.TryFind(costForDelete.PriceItem.Format.Id);
				Assert.That(format, Is.EqualTo(null));
			}
		}
	}
}