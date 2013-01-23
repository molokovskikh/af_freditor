DROP PROCEDURE usersettings.DeleteCost;
CREATE DEFINER=`RootDBMS`@`127.0.0.1` PROCEDURE usersettings.`DeleteCost`(IN inCostCode INT UNSIGNED)
BEGIN
  declare costType tinyint(1);
  declare baseCost, sourceId, ruleId, itemId, priceId int unsigned;

  select pd.CostType, exists(select * from userSettings.pricesregionaldata prd where prd.PriceCode = pd.PriceCode and prd.BaseCost=inCostCode), f.Id as RuleId, s.Id as SourceId, pi.Id as ItemId, pd.PriceCode
  into
    costType, baseCost, ruleId, sourceId, itemId, priceId
  from usersettings.PricesCosts pc
        join usersettings.PricesData pd on pd.PriceCode = pc.PriceCode
        join usersettings.PriceItems pi on pi.Id = pc.PriceItemId
            join Farm.FormRules f on f.Id = pi.FormRuleId
                        join Farm.sources s on s.Id = pi.SourceId
  where pc.CostCode = inCostCode;

  if (1 <> basecost) then

    update Customers.Intersection i
	join userSettings.pricesregionaldata prd on i.RegionId = prd.RegionCode and prd.pricecode=priceId
    set i.CostId = prd.BaseCost
    where CostId = inCostCode;

    if (costType = 1) then

      delete from Farm.FormRules
      where Id = ruleId;

      delete from Farm.sources
      where Id = sourceId;

      delete from usersettings.PriceItems
      where Id = itemId;

      delete Farm.Core0
      from Farm.Core0
        join Farm.CoreCosts on Farm.Core0.Id = Farm.CoreCosts.Core_Id
      where Farm.CoreCosts.PC_CostCode = inCostCode;
    end if;

    delete from Farm.CoreCosts
    where PC_CostCode = inCostCode;

    delete from Farm.CostFormRules
    where CostCode = inCostCode;

    delete from usersettings.pricescosts where costcode = inCostCode;
  end if;
END;
