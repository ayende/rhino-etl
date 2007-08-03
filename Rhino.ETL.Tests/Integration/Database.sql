-- DISCLAIMER - this is a DB that is meant to challange an ETL tool, not a good DB

if object_id('Users_Source') is not null drop table Users_Source
if object_id('Users_Destination') is not null drop table Users_Destination
if object_id('Users2Org') is not null drop table Users2Org

CREATE TABLE Users_Source
(
	[Id] int identity primary key,
	[Name] nvarchar(50) not null,
	[Email] nvarchar(255) not null
)

CREATE TABLE Users2Org
(
	[UserId] nvarchar(25),
	[organization id] int
)

CREATE TABLE Users_Destination
(
	UserId nvarchar(20) not null primary key,
	[First Name] nvarchar(25),
	[Last Name] nvarchar(25),
	Email nvarchar(255),
	Orgnaization int 
)

INSERT INTO Users_Source
SELECT 'Nancy Davolio', 'Seattle@USA.com' UNION ALL
SELECT 'Andrew Fuller', 'Tacoma@USA.com' UNION ALL
SELECT 'Janet Leverling', 'Kirkland@USA.com' UNION ALL
SELECT 'Margaret Peacock', 'Bad Email' UNION ALL
SELECT 'Steven Buchanan', 'London@UK.com' UNION ALL
SELECT 'Michael Suyama', 'London@UK.com' UNION ALL
SELECT 'Robert King', 'London@UK.com' UNION ALL
SELECT 'Laura Callahan', 'Seattle@USA.com' UNION ALL
SELECT 'Anne Dodsworth', 'London@UK.com'

INSERT INTO Users2Org
SELECT '1', 432 UNION ALL
SELECT '2', 332 UNION ALL
SELECT '3', 232 UNION ALL
SELECT '4', 132 