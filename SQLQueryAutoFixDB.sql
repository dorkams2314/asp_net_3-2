USE master;
GO

IF DB_ID(N'AutoFixDB') IS NOT NULL
BEGIN
    ALTER DATABASE AutoFixDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE AutoFixDB;
END;
GO

CREATE DATABASE AutoFixDB;
GO

USE AutoFixDB;
GO

DROP TABLE IF EXISTS OrderItems;
DROP TABLE IF EXISTS Orders;
DROP TABLE IF EXISTS CartItems;
DROP TABLE IF EXISTS Carts;
DROP TABLE IF EXISTS Products;
DROP TABLE IF EXISTS Categories;
DROP TABLE IF EXISTS Users;
DROP TABLE IF EXISTS Roles;

CREATE TABLE Roles (
    Id INT PRIMARY KEY IDENTITY(1, 1),
    Name NVARCHAR(50) NOT NULL
);

CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1, 1),
    Login NVARCHAR(100) NOT NULL,
    UserName NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(256) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    RoleId INT NOT NULL,
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

CREATE TABLE Categories (
    Id INT PRIMARY KEY IDENTITY(1, 1),
    Name NVARCHAR(100) NOT NULL
);

CREATE TABLE Products (
    Id INT PRIMARY KEY IDENTITY(1, 1),
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    ImagePath NVARCHAR(300) NOT NULL,
    Price DECIMAL(10, 2) NOT NULL,
    CategoryId INT NOT NULL,
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
);

CREATE TABLE Carts (
    Id INT PRIMARY KEY IDENTITY(1, 1),
    UserId INT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

CREATE TABLE CartItems (
    Id INT PRIMARY KEY IDENTITY(1, 1),
    CartId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    FOREIGN KEY (CartId) REFERENCES Carts(Id),
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

CREATE TABLE Orders (
    Id INT PRIMARY KEY IDENTITY(1, 1),
    UserId INT NOT NULL,
    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

CREATE TABLE OrderItems (
    Id INT PRIMARY KEY IDENTITY(1, 1),
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    Price DECIMAL(10, 2) NOT NULL,
    FOREIGN KEY (OrderId) REFERENCES Orders(Id),
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

INSERT INTO Roles (Name)
VALUES
	('Админ'),
	('Пользователь');

INSERT INTO Users (Login, UserName, PasswordHash, IsActive, RoleId)
VALUES
	('admin', 'Администратор', '$2a$11$tKZ0QRVGMsM47FTJ2O719O9ec2eKHz07osNtPWTOfXF13nXM.Ax5m', 1, 1),
	('ivan', 'Иван', '$2a$11$cuA1CB84ECW49Nqy9QXsLenBl1m2qvlIt7cMR5aoW3Zup.fZrHMOC', 1, 2),
	('petr', 'Пётр', '$2a$11$cuA1CB84ECW49Nqy9QXsLenBl1m2qvlIt7cMR5aoW3Zup.fZrHMOC', 1, 2);

INSERT INTO Categories (Name)
VALUES
	('Техобслуживание'),
	('Диагностика'),
	('Ремонт'),
	('Шиномонтаж');

INSERT INTO Products (Name, Description, ImagePath, Price, CategoryId)
VALUES
	('Замена масла', 'Замена моторного масла и базовая проверка уровня технических жидкостей.', '/images/products/service.png', 2500.00, 1),
	('Замена фильтров', 'Замена воздушного, салонного или масляного фильтра по выбору клиента.', '/images/products/service.png', 1200.00, 1),
	('Замена тормозных колодок', 'Снятие старых колодок и установка нового комплекта тормозных колодок.', '/images/products/service.png', 3500.00, 1),
	('Замена свечей зажигания', 'Подбор и установка свечей зажигания с проверкой работы двигателя.', '/images/products/service.png', 1800.00, 1),
	('Компьютерная диагностика двигателя', 'Подключение сканера и чтение ошибок электронных систем двигателя.', '/images/products/diagnostics.png', 2000.00, 2),
	('Диагностика ходовой', 'Проверка подвески, рулевого управления и основных узлов ходовой части.', '/images/products/diagnostics.png', 1500.00, 2),
	('Проверка электрики', 'Диагностика освещения, аккумулятора, генератора и проводки автомобиля.', '/images/products/diagnostics.png', 1000.00, 2),
	('Ремонт подвески', 'Замена поврежденных элементов подвески и восстановление ее работы.', '/images/products/repair.png', 8000.00, 3),
	('Ремонт тормозной системы', 'Ремонт тормозных механизмов и проверка исправности системы.', '/images/products/repair.png', 6000.00, 3),
	('Замена ремня ГРМ', 'Снятие старого ремня ГРМ и установка нового комплекта.', '/images/products/repair.png', 7000.00, 3),
	('Сезонная смена шин', 'Снятие, установка и затяжка колес при сезонной смене резины.', '/images/products/tires.png', 2000.00, 4),
	('Балансировка колес', 'Балансировка колес для более ровного и безопасного движения.', '/images/products/tires.png', 800.00,  4),
	('Хранение шин', 'Хранение комплекта шин на складе сервиса в течение сезона.', '/images/products/tires.png', 3000.00, 4);

INSERT INTO Carts (UserId)
VALUES
	(2),
	(3);

INSERT INTO CartItems (CartId, ProductId, Quantity)
VALUES
	(1, 1,  1),
	(1, 5,  1),
	(2, 11, 1),
	(2, 12, 4);

INSERT INTO Orders (UserId)
VALUES
	(2);

INSERT INTO OrderItems (OrderId, ProductId, Quantity, Price) VALUES
	(1, 1, 1, 2500.00),
	(1, 3, 1, 3500.00);
