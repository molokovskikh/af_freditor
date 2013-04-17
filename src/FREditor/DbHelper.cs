using System;
using System.Data;
using System.Data.Common;
using Common.MySql;
using Common.Tools;
using MySql.Data.MySqlClient;

namespace FREditor
{
	public class DbHelper
	{
		public static void SetLogParameters(MySqlConnection connection)
		{
			var command = new MySqlCommand(
				"set @INHost = ?Host; set @INUser = ?User;", connection);
			command.Parameters.AddWithValue("?Host", Environment.MachineName);
			command.Parameters.AddWithValue("?User", Environment.UserName);
			command.ExecuteNonQuery();
		}

		public static void PricesFill(MySqlConnection connection, DataTable prices, string param, bool showOnlyEnabled, int supplierIndex, params MySqlParameter[] parameters)
		{
			var sqlPart = String.Empty;
			if (showOnlyEnabled)
				sqlPart += @" and (datediff(curdate(), date(pim.pricedate)) < 200) 
  inner join Customers.suppliers s on s.Id = pd.FirmCode";
			else {
				sqlPart += "inner join Customers.suppliers s on s.Id = pd.FirmCode";
			}
			// Выбираем прайс-листы с мультиколоночными ценами
			var joinSynonymPart = string.Empty;
			var selectPart = string.Empty;
			if (supplierIndex > 0) {
				joinSynonymPart += @"join usersettings.pricesdata PD2 on pd.FirmCode = ?synonymSupplier and PD2.ParentSynonym = PD.PriceCode
inner join usersettings.pricescosts pc on pc.pricecode = pd2.pricecode";
				selectPart += "pd2.FirmCode as PFirmCode,";
				param = param.Replace("pd", "pd2");
				sqlPart = sqlPart.Replace("pd", "pd2");
			}
			else {
				joinSynonymPart += "inner join usersettings.pricescosts pc on pc.pricecode = pd.pricecode";
				selectPart += "pd.FirmCode as PFirmCode,";
			}

			var sql =
				@"
SELECT
  " + selectPart + @"
  pim.Id as PPriceItemId,
  pd.PriceCode as PPriceCode,
  if(pd.CostType = 1, concat(pd.PriceName, ' [Колонка] ', pc.CostName), pd.PriceName) as PPriceName,
  pim.LastDownload as PPriceDate,
	DATE_ADD(pim.LastDownload, INTERVAL r.MoscowBias HOUR) as PPriceDateWithBias,
  pim.LastFormalization as PDateLastForm,
  fr.MaxOld as PMaxOld,
  pd.PriceType as PPriceType,
  pd.CostType as PCostType,
  pim.WaitingDownloadInterval as PWaitingDownloadInterval,
  -- редактировать тип ценовой колонки и тип прайс-листа можно только относительно базовой ценовой колонки
  exists(select * from userSettings.pricesregionaldata prd where prd.BaseCost=pc.CostCode and prd.PriceCode=pd.PriceCode) as PIsParent,
  exists(select * from userSettings.pricesregionaldata prd where prd.BaseCost=pc.CostCode and prd.PriceCode=pd.PriceCode) as PBaseCost,
  pc.CostCode as PCostCode
FROM
  usersettings.pricesdata pd " + joinSynonymPart + @"
  inner join usersettings.PriceItems pim on (pim.Id = pc.PriceItemId)
"
					+ sqlPart +
					@"
  inner join farm.formrules fr on fr.Id = pim.FormRuleId
  inner join farm.regions r on r.regioncode=s.HomeRegion
	join Farm.Sources so on so.id = pim.SourceId
join farm.sourcetypes st on st.id = so.SourceTypeId
where 1 = 1 ";
			sql += param;
			sql += @"
group by pim.Id
order by PPriceName";

			var dataAdapter = new MySqlDataAdapter(sql, connection);
			dataAdapter.SelectCommand.Parameters.AddRange(parameters);
			dataAdapter.Fill(prices);
		}
	}
}