CREATE TABLE `farm`.`PriceEncodes` (
  `Id` INTEGER UNSIGNED NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`Id`)
)
ENGINE = InnoDB;

insert into farm.PriceEncodes (Name)
values
("Cp866"),("Cp1251");

update farm.FormRules
set PriceEncode = (select id from farm.PriceEncodes  where
Name = 'Cp866');

ALTER TABLE `farm`.`FormRules` ADD CONSTRAINT `PriceEncode` FOREIGN KEY `PriceEncode` (`PriceEncode`)
    REFERENCES `PriceEncodes` (`Id`)
    ON DELETE RESTRICT
    ON UPDATE RESTRICT;

ALTER TABLE `farm`.`FormRules` MODIFY COLUMN `PriceEncode` INT(3) UNSIGNED NOT NULL DEFAULT 1;
