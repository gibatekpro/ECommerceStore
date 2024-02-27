-- noinspection SqlDialectInspectionForFile

-- noinspection SqlNoDataSourceInspectionForFile

-- Run this script first because ProductCategoryId in
-- Products table depends on ProductsCategory table

-- -------------------------------------------------------
-- Drop existing data from tables
-- --------------------------------------------------------
DELETE FROM ProductCategories;

-- -------------------------------------------------------
-- Reset the identity seed for a table
-- -------------------------------------------------------
DBCC CHECKIDENT ('ProductCategories', RESEED, 0);


-- -----------------------------------------------------
-- Categories
-- -----------------------------------------------------
INSERT INTO ProductCategories(CategoryName) VALUES ('Books');
INSERT INTO ProductCategories(CategoryName) VALUES ('HouseHold Appliances');
INSERT INTO ProductCategories(CategoryName) VALUES ('Wears');
INSERT INTO ProductCategories(CategoryName) VALUES ('Stationery');