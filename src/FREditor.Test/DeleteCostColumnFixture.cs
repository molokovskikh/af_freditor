using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
		private MySqlConnection connection;
		private MySqlCommand command;
		private MySqlDataAdapter dataAdapter;
		private DataTable dtPrices;
		private DataColumn PPriceCode;
		private DataColumn PFirmCode;
		private DataColumn PPriceName;
		private DataColumn PDateCurPrice;
		private DataColumn PDateLastForm;
		private DataColumn PMaxOld;
		private DataColumn PPriceType;
		private DataColumn PCostType;
		private DataColumn PWaitingDownloadInterval;
		private DataColumn PIsParent;
		private DataColumn PBaseCost;
		private DataColumn PCostCode;
		private DataColumn PPriceDate;
		private DataColumn PPriceItemId;
		private DataColumn PDeleted;
		private DataSet dtSet;

		private TestPriceCost costForDelete;


		[SetUp]
		public void Setup()
		{
			connection = new MySqlConnection(Literals.ConnectionString());
			command = new MySqlCommand();
			command.Connection = connection;
			dataAdapter = new MySqlDataAdapter(command);
			dtPrices = new System.Data.DataTable();
			dtPrices.Columns.AddRange(new System.Data.DataColumn[] {
				PPriceCode = new System.Data.DataColumn() { AllowDBNull = false, ColumnName = "PPriceCode", DataType = typeof(long) },
				PFirmCode = new System.Data.DataColumn() { ColumnName = "PFirmCode", DataType = typeof(long) },
				PPriceName = new System.Data.DataColumn() { ColumnName = "PPriceName" },
				PDateCurPrice = new System.Data.DataColumn() { ColumnName = "PDateCurPrice", DataType = typeof(System.DateTime) },
				PDateLastForm = new System.Data.DataColumn() { ColumnName = "PDateLastForm", DataType = typeof(System.DateTime) },
				PMaxOld = new System.Data.DataColumn() { ColumnName = "PMaxOld", DataType = typeof(int) },
				PPriceType = new System.Data.DataColumn() { ColumnName = "PPriceType", DataType = typeof(int) },
				PCostType = new System.Data.DataColumn() { ColumnName = "PCostType", DataType = typeof(int) },
				PWaitingDownloadInterval = new System.Data.DataColumn() { ColumnName = "PWaitingDownloadInterval", DataType = typeof(int) },
				PIsParent = new System.Data.DataColumn() { ColumnName = "PIsParent", DataType = typeof(byte) },
				PBaseCost = new System.Data.DataColumn() { ColumnName = "PBaseCost", DataType = typeof(byte) },
				PCostCode = new System.Data.DataColumn() { ColumnName = "PCostCode", DataType = typeof(int) },
				PPriceDate = new System.Data.DataColumn() { ColumnName = "PPriceDate", DataType = typeof(System.DateTime) },
				PPriceItemId = new System.Data.DataColumn() { AllowDBNull = false, ColumnName = "PPriceItemId", DataType = typeof(long) },
				PDeleted = new System.Data.DataColumn() { AllowDBNull = false, ColumnName = "PDeleted", DataType = typeof(bool), DefaultValue = false }
			});
			dtPrices.PrimaryKey = new System.Data.DataColumn[] { PPriceItemId };
			dtPrices.TableName = "Prices";
			dtSet = new DataSet();
			dtSet.Tables.Add(dtPrices);
		}

		private void FillPrices()
		{
			connection.Open();
			command.CommandText =
				@"
SELECT
  pd.FirmCode as PFirmCode,
  pim.Id as PPriceItemId,
  pd.PriceCode as PPriceCode,
  if(pd.CostType = 1, concat(pd.PriceName, ' [Колонка] ', pc.CostName), pd.PriceName) as PPriceName,
  pim.PriceDate as PPriceDate,
  pim.LastFormalization as PDateLastForm,
  fr.MaxOld as PMaxOld,
  pd.PriceType as PPriceType,
  pd.CostType as PCostType,
  pim.WaitingDownloadInterval as PWaitingDownloadInterval,
  if(pc.BaseCost = 1 or (exists(select * from userSettings.pricesregionaldata prd where prd.BaseCost=pc.CostCode and prd.PriceCode=pd.PriceCode limit 1)), 1, 0) PIsParent,
  pc.BaseCost as PBaseCost,
  pc.CostCode as PCostCode	
FROM
  usersettings.pricesdata pd
  inner join usersettings.pricescosts pc on pc.pricecode = pd.pricecode
  inner join usersettings.PriceItems pim on (pim.Id = pc.PriceItemId) 
  inner join Customers.suppliers s on s.Id = pd.FirmCode
  inner join farm.formrules fr on fr.Id = pim.FormRuleId
  inner join farm.regions r on r.regioncode=s.HomeRegion
where 
(pd.CostType = 1) or (exists(select * from userSettings.pricesregionaldata prd where prd.PriceCode = PD.PriceCode and prd.BaseCost=pc.CostCode limit 1))";
			command.CommandText += @" 
group by pim.Id
order by PPriceName";

			dataAdapter.Fill(dtPrices);
			connection.Close();
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
			FillPrices();
			DataRow[] rows = dtPrices.Select(String.Format("PFirmCode = {0}", price.Supplier.Id));
			Assert.That(rows.Count(), Is.EqualTo(2));
			rows = dtPrices.Select(String.Format("PFirmCode = {0} and PCostType = 1 and PIsParent = 0", price.Supplier.Id));
			Assert.That(rows.Count(), Is.EqualTo(1));
			rows[0].Delete();
			DataSet chg = dtSet.GetChanges();
			Assert.That(chg != null, "Запись не удалена");
			if (connection.State == ConnectionState.Closed) connection.Open();
			MySqlCommand mcmdDPrice = new MySqlCommand();
			MySqlDataAdapter daPrice = new MySqlDataAdapter();

			mcmdDPrice.CommandText = "usersettings.DeleteCost";
			mcmdDPrice.CommandType = CommandType.StoredProcedure;
			mcmdDPrice.Parameters.Clear();
			mcmdDPrice.Parameters.Add("?inCostCode", MySqlDbType.Int64, 0, "PCostCode");
			mcmdDPrice.Parameters["?inCostCode"].Direction = ParameterDirection.Input;

			mcmdDPrice.Connection = connection;
			daPrice.DeleteCommand = mcmdDPrice;
			daPrice.TableMappings.Clear();
			daPrice.TableMappings.Add("Table", dtPrices.TableName);

			MySqlTransaction tr = connection.BeginTransaction();
			daPrice.Update(chg.Tables[dtPrices.TableName]);
			dtSet.AcceptChanges();
			tr.Commit();
			connection.Close();

			using (var scope = new TransactionScope(OnDispose.Rollback)) {
				TestPrice resprice = TestPrice.Find(price.Id);
				Assert.That(resprice.Costs.Count, Is.EqualTo(1));

				TestPriceCost cost = TestPriceCost.TryFind(costForDelete.Id);
				Assert.That(cost, Is.EqualTo(null));

				TestPriceItem item = TestPriceItem.TryFind(costForDelete.PriceItem.Id);
				Assert.That(item, Is.EqualTo(null));

				TestPriceSource source = TestPriceSource.TryFind(costForDelete.PriceItem.Source.Id);
				Assert.That(source, Is.EqualTo(null));

				TestFormat format = TestFormat.TryFind(costForDelete.PriceItem.Format.Id);
				Assert.That(format, Is.EqualTo(null));
			}
		}
	}
}