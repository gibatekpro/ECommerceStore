-- noinspection SqlDialectInspectionForFile

-- noinspection SqlNoDataSourceInspectionForFile

-- Run this script next because ProductCategoryId in
-- Products table depends on ProductsCategory table

-- -------------------------------------------------------
-- Drop existing data from tables
-- --------------------------------------------------------
DELETE
FROM OrderItems;
DELETE
FROM Orders;
DELETE
FROM UserProfiles;
DELETE
FROM Products;
DELETE
FROM ProductCategories;
DELETE
FROM OrderStatus;
DELETE
FROM AspNetUsers;
DELETE
FROM AspNetRoles;
DELETE
FROM Addresses;

-- -------------------------------------------------------
-- Reset the identity seed for a table
-- -------------------------------------------------------
DBCC
CHECKIDENT ('OrderItems', RESEED, 0);
DBCC
CHECKIDENT ('Orders', RESEED, 0);
DBCC
CHECKIDENT ('UserProfiles', RESEED, 0);
DBCC
CHECKIDENT ('Products', RESEED, 0);
DBCC
CHECKIDENT ('OrderStatus', RESEED, 0);
DBCC
CHECKIDENT ('Addresses', RESEED, 0);
DBCC
CHECKIDENT ('ProductCategories', RESEED, 0);

-- -----------------------------------------------------
-- OrderStatus
-- -----------------------------------------------------

INSERT INTO OrderStatus(StatusName)
VALUES ('Ordered');
INSERT INTO OrderStatus(StatusName)
VALUES ('Dispatched');
INSERT INTO OrderStatus(StatusName)
VALUES ('OutForDelivery');
INSERT INTO OrderStatus(StatusName)
VALUES ('Delivered');
INSERT INTO OrderStatus(StatusName)
VALUES ('Cancelled');

-- -----------------------------------------------------
-- Categories
-- -----------------------------------------------------
INSERT INTO ProductCategories(CategoryName)
VALUES ('Books');
INSERT INTO ProductCategories(CategoryName)
VALUES ('HouseHold Appliances');
INSERT INTO ProductCategories(CategoryName)
VALUES ('Wears');
INSERT INTO ProductCategories(CategoryName)
VALUES ('Stationery');

-- -----------------------------------------------------
-- Books
-- -----------------------------------------------------
INSERT INTO Products (ProductName, Description, UnitPrice, ImageUrl, Active, UnitsInStock, DateCreated, LastUpdated,
                      ProductCategoryId)
VALUES ('Python Mastery', 'Master Python programming language with this comprehensive guide.', 19.99, '#', 1, 50,
        GETDATE(), GETDATE(), 1),
       ('JavaScript Ninja', 'Unleash your JavaScript skills and become a coding ninja.', 24.99, '#', 1, 75, GETDATE(),
        GETDATE(), 1),
       ('Vue.js Essentials', 'Essential guide to Vue.js framework for building interactive web interfaces.', 29.99, '#',
        1, 60, GETDATE(), GETDATE(), 1),
       ('Big Data Basics', 'Learn the fundamentals of Big Data technologies and applications.', 17.99, '#', 1, 45,
        GETDATE(), GETDATE(), 1),
       ('C# Crash Course', 'Quick crash course to get you started with C# programming language.', 14.99, '#', 1, 55,
        GETDATE(), GETDATE(), 1),
       ('SQL Made Simple', 'Simplified guide to mastering SQL queries and database management.', 22.99, '#', 1, 65,
        GETDATE(), GETDATE(), 1),
       ('React.js Revolution', 'Revolutionize your web development skills with React.js framework.', 27.99, '#', 1, 70,
        GETDATE(), GETDATE(), 1),
       ('Java Essentials', 'Essential concepts and techniques to become proficient in Java programming.', 21.99, '#', 1,
        80, GETDATE(), GETDATE(), 1),
       ('Machine Learning Basics', 'Basic principles and algorithms of machine learning for beginners.', 18.99, '#', 1,
        40, GETDATE(), GETDATE(), 1),
       ('DevOps Handbook', 'Comprehensive guide to implementing DevOps practices in your organization.', 26.99, '#', 1,
        90, GETDATE(), GETDATE(), 1);

-- -----------------------------------------------------
-- HouseHold Appliances
-- -----------------------------------------------------
INSERT INTO Products (ProductName, Description, UnitPrice, ImageUrl, Active, UnitsInStock, DateCreated, LastUpdated,
                      ProductCategoryId)
VALUES ('Smart Coffee Maker', 'Brew coffee with just a tap.', 99.99, '#', 1, 100, GETDATE(), GETDATE(), 2),
       ('Robot Vacuum Cleaner', 'Effortlessly keep your floors clean.', 199.99, '#', 1, 100, GETDATE(), GETDATE(), 2),
       ('Smart Refrigerator', 'Keep your groceries organized and fresh.', 999.99, '#', 1, 100, GETDATE(), GETDATE(), 2),
       ('Air Purifier', 'Breathe cleaner air with advanced filtration.', 149.99, '#', 1, 100, GETDATE(), GETDATE(), 2),
       ('Smart Thermostat', 'Control your homes temperature from anywhere.', 129.99, '#', 1, 100, GETDATE(), GETDATE(),
        2),
       ('Food Processor', 'Effortlessly chop, blend, and mix ingredients.', 79.99, '#', 1, 100, GETDATE(), GETDATE(),
        2),
       ('Smart Light Bulb', 'Adjust lighting with your smartphone.', 19.99, '#', 1, 100, GETDATE(), GETDATE(), 2),
       ('Wireless Speaker', 'Enjoy music anywhere in your home.', 59.99, '#', 1, 100, GETDATE(), GETDATE(), 2),
       ('Coffee Grinder', 'Grind fresh coffee beans for the perfect cup.', 49.99, '#', 1, 100, GETDATE(), GETDATE(), 2),
       ('Smart Blender', 'Blend smoothies and soups with precision.', 89.99, '#', 1, 100, GETDATE(), GETDATE(), 2);

-- -----------------------------------------------------
-- Wears
-- -----------------------------------------------------
INSERT INTO Products (ProductName, Description, UnitPrice, ImageUrl, Active, UnitsInStock, DateCreated, LastUpdated,
                      ProductCategoryId)
VALUES ('Trendy T-Shirt', 'Stay stylish with this trendy t-shirt.', 29.99, '#', 1, 50, GETDATE(), GETDATE(), 3),
       ('Casual Jeans', 'Comfortable and versatile jeans for everyday wear.', 39.99, '#', 1, 75, GETDATE(), GETDATE(),
        3),
       ('Cozy Hoodie', 'Stay warm and cozy with this soft hoodie.', 49.99, '#', 1, 60, GETDATE(), GETDATE(), 3),
       ('Classic Blazer', 'Elevate your look with this classic blazer.', 59.99, '#', 1, 45, GETDATE(), GETDATE(), 3),
       ('Stylish Dress', 'Look elegant and chic in this stylish dress.', 69.99, '#', 1, 55, GETDATE(), GETDATE(), 3),
       ('Comfy Sweatpants', 'Relax in comfort with these cozy sweatpants.', 34.99, '#', 1, 65, GETDATE(), GETDATE(), 3),
       ('Fashionable Skirt', 'Add flair to your wardrobe with this fashionable skirt.', 44.99, '#', 1, 70, GETDATE(),
        GETDATE(), 3),
       ('Sporty Jacket', 'Stay active and stylish with this sporty jacket.', 54.99, '#', 1, 80, GETDATE(), GETDATE(),
        3),
       ('Trendy Sneakers', 'Step out in style with these trendy sneakers.', 79.99, '#', 1, 40, GETDATE(), GETDATE(), 3),
       ('Chic Handbag', 'Complete your look with this chic handbag.', 89.99, '#', 1, 90, GETDATE(), GETDATE(), 3);

-- -----------------------------------------------------
-- Stationery
-- -----------------------------------------------------
INSERT INTO Products (ProductName, Description, UnitPrice, ImageUrl, Active, UnitsInStock, DateCreated, LastUpdated,
                      ProductCategoryId)
VALUES ('Blue Gel Pen', 'Smooth-writing gel pen in blue ink.', 1.99, '#', 1, 100, GETDATE(), GETDATE(), 4),
       ('A4 Notebooks (Pack of 5)', 'Pack of 5 A4 size ruled notebooks.', 6.99, '#', 1, 50, GETDATE(), GETDATE(), 4),
       ('Sticky Notes', 'Assorted color sticky notes for reminders and memos.', 3.49, '#', 1, 200, GETDATE(), GETDATE(),
        4),
       ('Ballpoint Pens (Pack of 10)', 'Pack of 10 ballpoint pens in assorted colors.', 4.99, '#', 1, 80, GETDATE(),
        GETDATE(), 4),
       ('Mechanical Pencils (Pack of 3)', 'Pack of 3 mechanical pencils with erasers.', 2.99, '#', 1, 120, GETDATE(),
        GETDATE(), 4),
       ('Highlighter Markers (Pack of 6)', 'Pack of 6 highlighter markers in neon colors.', 5.99, '#', 1, 60, GETDATE(),
        GETDATE(), 4),
       ('Binder Clips (Pack of 20)', 'Pack of 20 assorted size binder clips.', 3.99, '#', 1, 150, GETDATE(), GETDATE(),
        4),
       ('Correction Tape', 'White correction tape for precise corrections.', 2.49, '#', 1, 100, GETDATE(), GETDATE(),
        4),
       ('Index Cards (Pack of 100)', 'Pack of 100 lined index cards.', 3.99, '#', 1, 80, GETDATE(), GETDATE(), 4),
       ('Fine Point Markers (Pack of 8)', 'Pack of 8 fine point markers in vibrant colors.', 6.99, '#', 1, 70,
        GETDATE(), GETDATE(), 4);

