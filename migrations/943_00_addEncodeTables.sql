update `farm`.`FormRules` set `PriceEncode` = 866;

ALTER TABLE `farm`.`FormRules` MODIFY COLUMN `PriceEncode` INT(3) UNSIGNED NOT NULL DEFAULT 866;
