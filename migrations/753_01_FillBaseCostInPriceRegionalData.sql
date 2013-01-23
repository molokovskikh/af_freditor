update userSettings.pricesregionaldata p
join usersettings.pricesdata pd on p.pricecode = pd.pricecode
join usersettings.pricescosts pc on pd.pricecode=pc.pricecode and pc.BaseCost=1
set p.BaseCost = pc.CostCode
where p.BaseCost is null;