USE AutoFixDB;
GO
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF EXISTS (SELECT Login FROM dbo.Users GROUP BY Login HAVING COUNT(*) > 1)
    THROW 50001, 'Duplicate logins must be resolved before adding the unique index.', 1;

IF EXISTS (SELECT Name FROM dbo.Roles GROUP BY Name HAVING COUNT(*) > 1)
    THROW 50002, 'Duplicate role names must be resolved before adding the unique index.', 1;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Users') AND name = N'IX_Users_Login')
    CREATE UNIQUE INDEX IX_Users_Login ON dbo.Users(Login);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Roles') AND name = N'IX_Roles_Name')
    CREATE UNIQUE INDEX IX_Roles_Name ON dbo.Roles(Name);

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Name = N'Admin')
    INSERT INTO dbo.Roles(Name) VALUES (N'Admin');
IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Name = N'User')
    INSERT INTO dbo.Roles(Name) VALUES (N'User');

UPDATE u
SET RoleId = canonical.Id
FROM dbo.Users u
JOIN dbo.Roles legacy ON legacy.Id = u.RoleId
JOIN dbo.Roles canonical ON canonical.Name =
    CASE legacy.Name WHEN N'Админ' THEN N'Admin' WHEN N'Пользователь' THEN N'User' END
WHERE legacy.Name IN (N'Админ', N'Пользователь');

COMMIT;
GO
