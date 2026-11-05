USE AutoFixDB;
GO

IF COL_LENGTH(N'dbo.Products', N'StockQuantity') IS NULL
BEGIN
    ALTER TABLE dbo.Products ADD StockQuantity INT NOT NULL
        CONSTRAINT DF_Products_StockQuantity DEFAULT 0 WITH VALUES;
END;
GO

IF OBJECT_ID(N'dbo.CK_Products_StockQuantity', N'C') IS NULL
BEGIN
    ALTER TABLE dbo.Products ADD CONSTRAINT CK_Products_StockQuantity
        CHECK (StockQuantity >= 0);
END;
GO

IF OBJECT_ID(N'dbo.Reservations', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Reservations (
        Id INT NOT NULL IDENTITY(1, 1) CONSTRAINT PK_Reservations PRIMARY KEY,
        OrderId INT NOT NULL,
        ProductId INT NOT NULL,
        UserId INT NOT NULL,
        Quantity INT NOT NULL,
        CreatedAt DATETIME2 NOT NULL,
        CancelledAt DATETIME2 NULL,
        CONSTRAINT FK_Reservations_Orders FOREIGN KEY (OrderId) REFERENCES dbo.Orders(Id),
        CONSTRAINT FK_Reservations_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products(Id),
        CONSTRAINT FK_Reservations_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(Id),
        CONSTRAINT CK_Reservations_Quantity CHECK (Quantity > 0),
        CONSTRAINT CK_Reservations_Dates CHECK (CancelledAt IS NULL OR CancelledAt >= CreatedAt)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Reservations') AND name = N'IX_Reservations_OrderId')
BEGIN
    CREATE INDEX IX_Reservations_OrderId ON dbo.Reservations(OrderId);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Reservations') AND name = N'IX_Reservations_ProductId')
BEGIN
    CREATE INDEX IX_Reservations_ProductId ON dbo.Reservations(ProductId);
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Reservations') AND name = N'IX_Reservations_UserId')
BEGIN
    CREATE INDEX IX_Reservations_UserId ON dbo.Reservations(UserId);
END;
GO
