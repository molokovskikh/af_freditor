using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;
using NUnit.Framework;
using Test.Support;
using Test.Support.Suppliers;

namespace FREditor.Test
{
	[TestFixture]
	public class DeleteCostIntegrationFixture : IntegrationFixture
	{
		[Test]
		public void DeleteCostTest()
		{
			// создаем поставщика
			var supplier = TestSupplier.CreateNaked();
			supplier.Prices[0].Costs[0].Name = "Базовая";
			supplier.Prices[0].CostType = 0;
			Save(supplier);
			var price = supplier.Prices[0];
			// добавляем ценовую колонку к прайсу
			price.Costs.Add(new TestPriceCost {
				Price = price,
				PriceItem = price.Costs[0].PriceItem,
				Name = "Новая базовая"
			});

			price.Costs[1].FormRule = new TestCostFormRule { Cost = price.Costs[1], FieldName = "" };
			Save(price);
			// для региональных настроек задаем новую базовую колонку
			var regionalData = price.RegionalData.First();
			regionalData.BaseCost = price.Costs[1];
			Save(regionalData);
			// создаем клиента и проверяем, что ему назначена верная ценовая колонка
			var client = TestClient.CreateNaked();
			var intersection = session.Query<TestIntersection>().Where(i => i.Price.Id == price.Id && i.Client == client);
			foreach (var intersectionItem in intersection)
				Assert.That(intersectionItem.Cost == price.Costs[1]);
			// назначаем базовой ценовой колонкой для региона другую
			regionalData.BaseCost = price.Costs[0];
			Save(regionalData);
			Flush();
			// удаляем предыдущую базовую колонку,используя хранимую процедуру
			var query = session.CreateSQLQuery("call usersettings.deletecost(:deleteCost);");
			query.SetParameter("deleteCost", supplier.Prices[0].Costs[1].Id);
			query.ExecuteUpdate();
			// проверяем, что вместо удаленной колонки клиенту назначилась новая базовая
			intersection = session.Query<TestIntersection>().Where(i => i.Price.Id == price.Id
				&& i.Client == client
				&& i.Cost.Id == price.Costs[0].Id);
			Assert.That(intersection.Count() == 1);
		}
	}
}
