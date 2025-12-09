CREATE DATABASE CafeDB;
GO

USE CafeDB;
GO

-- 1. Users Table
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    PhoneNumber NVARCHAR(20),
    UserRole NVARCHAR(20) CHECK (UserRole IN ('Administrator', 'Staff', 'Customer')),
    CreatedAt DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(20) DEFAULT 'Active'
);

-- 2. Menu Items Table
CREATE TABLE MenuItems (
    ItemID INT IDENTITY(1,1) PRIMARY KEY,
    ItemName NVARCHAR(100) NOT NULL,
    Category NVARCHAR(50),
    Price DECIMAL(10, 2) NOT NULL,
    StockQuantity INT DEFAULT 0,
    LowStockLimit INT DEFAULT 10,
    Description NVARCHAR(255),
    Status NVARCHAR(20) DEFAULT 'Available'
);

-- 3. Orders Table
CREATE TABLE Orders (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID INT FOREIGN KEY REFERENCES Users(UserID),
    StaffID INT FOREIGN KEY REFERENCES Users(UserID),
    OrderDate DATETIME DEFAULT GETDATE(),
    TotalAmount DECIMAL(10, 2),
    Status NVARCHAR(20) DEFAULT 'Pending', -- Pending, Confirmed, Completed, Cancelled
    PaymentMethod NVARCHAR(50),
    Remarks NVARCHAR(255),
    ConfirmedAt DATETIME
);

-- 4. Order Details Table
CREATE TABLE OrderDetails (
    OrderDetailID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT FOREIGN KEY REFERENCES Orders(OrderID),
    ItemID INT FOREIGN KEY REFERENCES MenuItems(ItemID),
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10, 2) NOT NULL,
    TotalPrice AS (Quantity * UnitPrice)
);

-- 5. Suppliers Table
CREATE TABLE Suppliers (
    SupplierID INT IDENTITY(1,1) PRIMARY KEY,
    SupplierName NVARCHAR(100) NOT NULL,
    ContactPerson NVARCHAR(100),
    PhoneNumber NVARCHAR(20),
    Email NVARCHAR(100),
    Address NVARCHAR(255),
    Rating DECIMAL(3, 2),
    Status NVARCHAR(20) DEFAULT 'Active'
);

-- 6. Purchase Orders Table
CREATE TABLE PurchaseOrders (
    PurchaseOrderID INT IDENTITY(1,1) PRIMARY KEY,
    SupplierID INT FOREIGN KEY REFERENCES Suppliers(SupplierID),
    CreatedBy INT FOREIGN KEY REFERENCES Users(UserID),
    OrderDate DATETIME DEFAULT GETDATE(),
    Status NVARCHAR(20) DEFAULT 'Pending',
    TotalCost DECIMAL(10, 2)
);

-- 7. Purchase Order Details Table
CREATE TABLE PurchaseOrderDetails (
    PODetailID INT IDENTITY(1,1) PRIMARY KEY,
    PurchaseOrderID INT FOREIGN KEY REFERENCES PurchaseOrders(PurchaseOrderID),
    ItemID INT FOREIGN KEY REFERENCES MenuItems(ItemID),
    QuantityOrdered INT NOT NULL,
    UnitCost DECIMAL(10, 2) NOT NULL,
    Status NVARCHAR(20) DEFAULT 'Pending'
);

-- 8. Notifications Table
CREATE TABLE Notifications (
    NotificationID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    Message NVARCHAR(255),
    CreatedAt DATETIME DEFAULT GETDATE(),
    IsRead BIT DEFAULT 0
);

-- 9. Audit Logs Table
CREATE TABLE AuditLogs (
    LogID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    Action NVARCHAR(255),
    ActionDate DATETIME DEFAULT GETDATE()
);

-- Insert Default Admin User
INSERT INTO Users (FullName, Email, PasswordHash, PhoneNumber, UserRole)
VALUES ('System Admin', 'admin@cafe.com', 'admin123', '1234567890', 'Administrator');
select *from Users
INSERT INTO Users (FullName, Email, PasswordHash, PhoneNumber, UserRole)
VALUES ('staff', 'staff@cafe.com', 'staff123', '1234567890', 'staff');
INSERT INTO Users (FullName, Email, PasswordHash, PhoneNumber, UserRole)
VALUES ('customer', 'customer@cafe.com', 'cust123', '1234567890', 'Administrator');

update Users set UserRole='Customer' where UserID=3 

select * from Users
select * from MenuItems
select* from Orders
select * from PurchaseOrderDetails