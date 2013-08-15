ALTER TABLE `farm`.`pricefmts` ADD COLUMN `Hidden` TINYINT(1) UNSIGNED NOT NULL AFTER `ParserClassName`;


update farm.pricefmts
set hidden = true
where id in (11,12,13,14);