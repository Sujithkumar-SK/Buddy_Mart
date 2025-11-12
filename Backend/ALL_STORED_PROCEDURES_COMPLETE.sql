-- =============================================
-- ALL STORED PROCEDURES FOR ECOMMERCE DATABASE
-- Execute this script on your SQL Server container
-- =============================================

USE Vendor_Ecommerce;
GO

-- Drop existing procedures if they exist
DECLARE @sql NVARCHAR(MAX) = '';
SELECT @sql = @sql + 'DROP PROCEDURE ' + QUOTENAME(name) + ';' + CHAR(13)
FROM sys.procedures 
WHERE name LIKE 'sp_%';
EXEC sp_executesql @sql;
GO

-- =============================================
-- Authentication & User Management
-- =============================================

CREATE PROCEDURE sp_CheckEmailExists
    @Email NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CASE 
        WHEN EXISTS (SELECT 1 FROM Users WHERE Email = @Email) 
        THEN CAST(1 AS BIT) 
        ELSE CAST(0 AS BIT) 
    END AS EmailExists;
END
GO

CREATE PROCEDURE sp_CheckPhoneExists
    @Phone NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CASE 
        WHEN EXISTS (SELECT 1 FROM Users WHERE Phone = @Phone) 
        THEN CAST(1 AS BIT) 
        ELSE CAST(0 AS BIT) 
    END AS PhoneExists;
END
GO

CREATE PROCEDURE sp_GetUserByEmail
    @Email NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        UserId, Email, PasswordHash, Phone, Role, IsEmailVerified, IsActive,
        LastLogin, FailedLoginAttempts, LockoutEnd, TenantId, CreatedBy, CreatedOn, UpdatedBy, UpdatedOn
    FROM Users 
    WHERE Email = @Email AND IsDeleted = 0
    ORDER BY CreatedOn DESC;
END
GO

CREATE PROCEDURE sp_GetUserById
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        UserId, Email, PasswordHash, Phone, Role, IsEmailVerified, IsActive,
        LastLogin, FailedLoginAttempts, LockoutEnd, TenantId, CreatedBy, CreatedOn, UpdatedBy, UpdatedOn
    FROM Users 
    WHERE UserId = @UserId AND IsDeleted = 0;
END
GO

CREATE PROCEDURE sp_GetUserByRefreshToken
    @RefreshToken NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        UserId, Email, PasswordHash, Phone, Role, IsEmailVerified, IsActive,
        LastLogin, FailedLoginAttempts, LockoutEnd, TenantId, CreatedBy, CreatedOn, UpdatedBy, UpdatedOn
    FROM Users 
    WHERE RefreshToken = @RefreshToken AND IsDeleted = 0 AND IsActive = 1;
END
GO

CREATE PROCEDURE sp_ValidateUserCredentials
    @Email NVARCHAR(150),
    @PasswordHash NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @IsValid BIT = 0;
    IF EXISTS (
        SELECT 1 FROM Users 
        WHERE Email = @Email AND PasswordHash = @PasswordHash 
        AND IsDeleted = 0 AND IsActive = 1
    )
    SET @IsValid = 1;
    SELECT @IsValid AS IsValid;
END
GO

-- =============================================
-- Vendor Management
-- =============================================

CREATE PROCEDURE sp_CheckBusinessLicenseExists
    @BusinessLicenseNumber NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CASE 
        WHEN EXISTS (SELECT 1 FROM Vendors WHERE BusinessLicenseNumber = @BusinessLicenseNumber) 
        THEN CAST(1 AS BIT) 
        ELSE CAST(0 AS BIT) 
    END AS BusinessLicenseExists;
END
GO

CREATE PROCEDURE sp_GetVendorById
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Vendors WHERE VendorId = @VendorId AND IsDeleted = 0;
END
GO

CREATE PROCEDURE sp_GetVendorByUserId
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Vendors WHERE UserId = @UserId AND IsDeleted = 0;
END
GO

CREATE PROCEDURE sp_GetPendingVendors
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        v.VendorId, v.BusinessName, v.OwnerName, v.BusinessLicenseNumber,
        v.BusinessAddress, v.City, v.State, v.PinCode, v.TaxRegistrationNumber,
        v.DocumentPath, u.Email, u.Phone, v.CreatedOn, v.Status
    FROM Vendors v
    INNER JOIN Users u ON v.UserId = u.UserId
    WHERE v.Status = 'Pending' AND v.IsDeleted = 0;
END
GO

CREATE PROCEDURE sp_GetVendorForApproval
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        v.*, u.Email, u.Phone, u.IsEmailVerified, u.CreatedOn as RegistrationDate
    FROM Vendors v
    INNER JOIN Users u ON v.UserId = u.UserId
    WHERE v.VendorId = @VendorId AND v.IsDeleted = 0;
END
GO

CREATE PROCEDURE sp_ValidateVendor
    @VendorId INT,
    @BusinessLicenseNumber NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CASE 
        WHEN EXISTS (
            SELECT 1 FROM Vendors 
            WHERE VendorId = @VendorId 
            AND BusinessLicenseNumber = @BusinessLicenseNumber 
            AND IsDeleted = 0
        ) 
        THEN CAST(1 AS BIT) 
        ELSE CAST(0 AS BIT) 
    END AS IsValid;
END
GO

-- =============================================
-- Product Management
-- =============================================

CREATE PROCEDURE sp_CheckSKUExists
    @SKU NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CASE 
        WHEN EXISTS (SELECT 1 FROM Products WHERE SKU = @SKU AND IsDeleted = 0) 
        THEN CAST(1 AS BIT)
        ELSE CAST(0 AS BIT)
    END AS SKUExists;
END
GO

CREATE PROCEDURE sp_GetAllProducts
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        p.*, c.Name as CategoryName, v.BusinessName as VendorName
    FROM Products p
    LEFT JOIN Categories c ON p.CategoryId = c.CategoryId
    LEFT JOIN Vendors v ON p.VendorId = v.VendorId
    WHERE p.IsDeleted = 0
    ORDER BY p.CreatedOn DESC;
END
GO

CREATE PROCEDURE sp_GetProductById
    @ProductId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        p.*, c.Name as CategoryName, v.BusinessName as VendorName
    FROM Products p
    LEFT JOIN Categories c ON p.CategoryId = c.CategoryId
    LEFT JOIN Vendors v ON p.VendorId = v.VendorId
    WHERE p.ProductId = @ProductId AND p.IsDeleted = 0;
END
GO

CREATE PROCEDURE sp_GetProductsByVendor
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        p.*, c.Name as CategoryName
    FROM Products p
    LEFT JOIN Categories c ON p.CategoryId = c.CategoryId
    WHERE p.VendorId = @VendorId AND p.IsDeleted = 0
    ORDER BY p.CreatedOn DESC;
END
GO

CREATE PROCEDURE sp_GetProductsByCategory
    @CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        p.*, c.Name as CategoryName, v.BusinessName as VendorName
    FROM Products p
    LEFT JOIN Categories c ON p.CategoryId = c.CategoryId
    LEFT JOIN Vendors v ON p.VendorId = v.VendorId
    WHERE p.CategoryId = @CategoryId AND p.IsDeleted = 0
    ORDER BY p.CreatedOn DESC;
END
GO

CREATE PROCEDURE sp_CheckStockAvailability
    @ProductId INT,
    @RequiredQuantity INT,
    @IsAvailable BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @CurrentStock INT;
    SELECT @CurrentStock = StockQuantity
    FROM Products
    WHERE ProductId = @ProductId AND IsDeleted = 0 AND Status != 0;
    
    IF @CurrentStock IS NULL
    BEGIN
        SET @IsAvailable = 0;
        RETURN;
    END
    
    IF @CurrentStock >= @RequiredQuantity
        SET @IsAvailable = 1;
    ELSE
        SET @IsAvailable = 0;
END
GO

-- =============================================
-- Category Management
-- =============================================

CREATE PROCEDURE sp_CheckCategoryNameExists
    @Name NVARCHAR(100),
    @CategoryId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CASE 
        WHEN EXISTS (
            SELECT 1 FROM Categories 
            WHERE Name = @Name AND IsDeleted = 0 
            AND (@CategoryId IS NULL OR CategoryId != @CategoryId)
        ) 
        THEN CAST(1 AS BIT)
        ELSE CAST(0 AS BIT)
    END AS NameExists;
END
GO

CREATE PROCEDURE sp_CreateCategory
    @Name NVARCHAR(100),
    @Description NVARCHAR(500) = NULL,
    @ImagePath NVARCHAR(500) = NULL,
    @ParentCategoryId INT = NULL,
    @CreatedBy NVARCHAR(100),
    @TenantId NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Categories (Name, Description, ImagePath, ParentCategoryId, IsActive, CreatedBy, CreatedOn, TenantId)
    VALUES (@Name, @Description, @ImagePath, @ParentCategoryId, 1, @CreatedBy, GETUTCDATE(), @TenantId);
    SELECT SCOPE_IDENTITY() AS CategoryId;
END
GO

CREATE PROCEDURE sp_UpdateCategory
    @CategoryId INT,
    @Name NVARCHAR(100),
    @Description NVARCHAR(500) = NULL,
    @ImagePath NVARCHAR(500) = NULL,
    @ParentCategoryId INT = NULL,
    @IsActive BIT,
    @UpdatedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Categories 
    SET Name = @Name, Description = @Description, ImagePath = @ImagePath,
        ParentCategoryId = @ParentCategoryId, IsActive = @IsActive,
        UpdatedBy = @UpdatedBy, UpdatedOn = GETUTCDATE()
    WHERE CategoryId = @CategoryId AND IsDeleted = 0;
END
GO

CREATE PROCEDURE sp_DeleteCategory
    @CategoryId INT,
    @DeletedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM Products WHERE CategoryId = @CategoryId AND IsDeleted = 0)
    BEGIN
        SELECT 0 AS CanDelete, 'Category has products and cannot be deleted' AS Message;
        RETURN;
    END
    
    UPDATE Categories 
    SET IsDeleted = 1, DeletedBy = @DeletedBy, DeletedOn = GETUTCDATE()
    WHERE CategoryId = @CategoryId AND IsDeleted = 0;
    
    SELECT 1 AS CanDelete, 'Category deleted successfully' AS Message;
END
GO

CREATE PROCEDURE sp_GetAllCategories
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        c.CategoryId, c.Name, c.Description, c.ImagePath, c.IsActive,
        c.ParentCategoryId, pc.Name AS ParentCategoryName, c.CreatedOn
    FROM Categories c
    LEFT JOIN Categories pc ON c.ParentCategoryId = pc.CategoryId
    WHERE c.IsDeleted = 0
    ORDER BY c.CreatedOn DESC;
END
GO

CREATE PROCEDURE sp_GetCategoryById
    @CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        c.CategoryId, c.Name, c.Description, c.ImagePath, c.IsActive,
        c.ParentCategoryId, pc.Name AS ParentCategoryName, c.CreatedOn
    FROM Categories c
    LEFT JOIN Categories pc ON c.ParentCategoryId = pc.CategoryId
    WHERE c.CategoryId = @CategoryId AND c.IsDeleted = 0;
END
GO

CREATE PROCEDURE sp_ValidateCategory
    @CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CASE 
        WHEN EXISTS (SELECT 1 FROM Categories WHERE CategoryId = @CategoryId AND IsDeleted = 0 AND IsActive = 1) 
        THEN CAST(1 AS BIT)
        ELSE CAST(0 AS BIT)
    END AS IsValid;
END
GO

-- =============================================
-- Cart Management
-- =============================================

CREATE PROCEDURE sp_CheckCartItemExists
    @CustomerId INT,
    @ProductId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CASE 
        WHEN EXISTS (
            SELECT 1 FROM Cart 
            WHERE CustomerId = @CustomerId AND ProductId = @ProductId AND IsDeleted = 0
        ) 
        THEN CAST(1 AS BIT)
        ELSE CAST(0 AS BIT)
    END AS ItemExists;
END
GO

CREATE PROCEDURE sp_GetCartByCustomerId
    @CustomerId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        c.CartId, c.CustomerId, c.ProductId, c.Quantity, c.Price,
        p.Name as ProductName, p.ImagePath, p.StockQuantity
    FROM Cart c
    INNER JOIN Products p ON c.ProductId = p.ProductId
    WHERE c.CustomerId = @CustomerId AND c.IsDeleted = 0 AND p.IsDeleted = 0
    ORDER BY c.CreatedOn DESC;
END
GO

-- =============================================
-- Customer Management
-- =============================================

CREATE PROCEDURE sp_GetAllCustomers
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        c.CustomerId, c.FirstName, c.MiddleName, c.LastName,
        c.DateOfBirth, c.Gender, c.Address, c.City, c.State, c.PinCode,
        c.IsActive, c.UserId, u.Email, u.Phone
    FROM Customers c
    INNER JOIN Users u ON c.UserId = u.UserId
    WHERE c.IsDeleted = 0 AND u.IsDeleted = 0
    ORDER BY c.CreatedOn DESC;
END
GO

CREATE PROCEDURE sp_GetCustomerById
    @CustomerId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        c.CustomerId, c.FirstName, c.MiddleName, c.LastName,
        c.DateOfBirth, c.Gender, c.Address, c.City, c.State, c.PinCode,
        c.IsActive, c.UserId, u.Email, u.Phone
    FROM Customers c
    INNER JOIN Users u ON c.UserId = u.UserId
    WHERE c.CustomerId = @CustomerId AND c.IsDeleted = 0;
END
GO

-- =============================================
-- Order Management
-- =============================================

CREATE PROCEDURE sp_GetOrderById
    @OrderId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Orders WHERE OrderId = @OrderId AND IsDeleted = 0;
END
GO

CREATE PROCEDURE sp_GetOrdersByCustomerId
    @CustomerId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Orders 
    WHERE CustomerId = @CustomerId AND IsDeleted = 0
    ORDER BY CreatedOn DESC;
END
GO

CREATE PROCEDURE sp_GetOrdersByVendor
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT DISTINCT o.*
    FROM Orders o
    INNER JOIN OrderItems oi ON o.OrderId = oi.OrderId
    INNER JOIN Products p ON oi.ProductId = p.ProductId
    WHERE p.VendorId = @VendorId AND o.IsDeleted = 0
    ORDER BY o.CreatedOn DESC;
END
GO

CREATE PROCEDURE sp_GetOrderItems
    @OrderId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        oi.*, p.Name as ProductName, p.ImagePath
    FROM OrderItems oi
    INNER JOIN Products p ON oi.ProductId = p.ProductId
    WHERE oi.OrderId = @OrderId AND oi.IsDeleted = 0;
END
GO

-- =============================================
-- Payment Management
-- =============================================

CREATE PROCEDURE sp_GetPaymentById
    @PaymentId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Payments WHERE PaymentId = @PaymentId AND IsDeleted = 0;
END
GO

CREATE PROCEDURE sp_GetPaymentByRazorpayOrderId
    @RazorpayOrderId NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Payments 
    WHERE RazorpayOrderId = @RazorpayOrderId AND IsDeleted = 0;
END
GO

CREATE PROCEDURE sp_GetPaymentsByCustomerId
    @CustomerId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Payments 
    WHERE CustomerId = @CustomerId AND IsDeleted = 0
    ORDER BY CreatedOn DESC;
END
GO

CREATE PROCEDURE sp_GetPaymentsByOrderId
    @OrderId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Payments 
    WHERE OrderId = @OrderId AND IsDeleted = 0
    ORDER BY CreatedOn DESC;
END
GO

-- =============================================
-- Shipping Management
-- =============================================

CREATE PROCEDURE sp_GetShippingById
    @ShippingId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Shipping WHERE ShippingId = @ShippingId AND IsDeleted = 0;
END
GO

CREATE PROCEDURE sp_GetShippingByOrderId
    @OrderId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Shipping WHERE OrderId = @OrderId AND IsDeleted = 0;
END
GO

CREATE PROCEDURE sp_GetShippingByTrackingNumber
    @TrackingNumber NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Shipping 
    WHERE TrackingNumber = @TrackingNumber AND IsDeleted = 0;
END
GO

CREATE PROCEDURE sp_GetShippingsByVendor
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT s.*
    FROM Shipping s
    INNER JOIN Orders o ON s.OrderId = o.OrderId
    INNER JOIN OrderItems oi ON o.OrderId = oi.OrderId
    INNER JOIN Products p ON oi.ProductId = p.ProductId
    WHERE p.VendorId = @VendorId AND s.IsDeleted = 0
    ORDER BY s.CreatedOn DESC;
END
GO

-- =============================================
-- Analytics & Reports
-- =============================================

CREATE PROCEDURE sp_GetDashboardAnalytics
    @StartDate DATE,
    @EndDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        COUNT(DISTINCT o.OrderId) as TotalOrders,
        SUM(o.TotalAmount) as TotalRevenue,
        COUNT(DISTINCT o.CustomerId) as TotalCustomers,
        COUNT(DISTINCT p.VendorId) as ActiveVendors
    FROM Orders o
    LEFT JOIN OrderItems oi ON o.OrderId = oi.OrderId
    LEFT JOIN Products p ON oi.ProductId = p.ProductId
    WHERE o.CreatedOn BETWEEN @StartDate AND @EndDate
    AND o.IsDeleted = 0;
END
GO

PRINT 'All stored procedures created successfully!';