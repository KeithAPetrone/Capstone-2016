CREATE TABLE [dbo].[Users]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Name] NCHAR(30) NOT NULL,
	[Email] nvarchar(50) NOT NULL
)

INSERT INTO Users (Id,Name,Email)
(
	VALUES(1, "Keith Petrone", "kpetrone@student.neumont.edu"),\
)
