CREATE OR ALTER PROCEDURE sp_GetAllCustomers
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.CustomerId,
        c.FirstName,
        c.MiddleName,
        c.LastName,
        c.DateOfBirth,
        c.Gender,
        c.Address,
        c.City,
        c.State,
        c.PinCode,
        c.IsActive,
        c.UserId,
        u.Email,
        u.Phone
    FROM Customers c
    INNER JOIN Users u ON c.UserId = u.UserId
    WHERE c.IsDeleted = 0
        AND u.IsDeleted = 0
    ORDER BY c.CreatedOn DESC
END
GO
--------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetCustomerById
    @CustomerId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.CustomerId,
        c.FirstName,
        c.MiddleName,
        c.LastName,
        c.DateOfBirth,
        c.Gender,
        c.Address,
        c.City,
        c.State,
        c.PinCode,
        c.IsActive,
        c.UserId,
        u.Email,
        u.Phone
    FROM Customers c
    INNER JOIN Users u ON c.UserId = u.UserId
    WHERE c.CustomerId = @CustomerId 
        AND c.IsDeleted = 0
        AND u.IsDeleted = 0
END
GO
----------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetSubscriptionPlans
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        PlanId,
        PlanName,
        Description,
        Price,
        MaxProducts,
        DurationDays
    FROM SubscriptionPlans
    WHERE IsActive = 1 
        AND IsDeleted = 0
    ORDER BY Price ASC
END
GO
------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetUserByEmail
    @Email NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            UserId,
            Email,
            PasswordHash,
            Phone,
            Role,
            IsEmailVerified,
            IsActive,
            LastLogin,
            FailedLoginAttempts,
            LockoutEnd,
            TenantId,
            CreatedBy,
            CreatedOn,
            UpdatedBy,
            UpdatedOn
        FROM Users 
        WHERE Email = @Email 
            AND IsDeleted = 0
        ORDER BY CreatedOn DESC;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO
-----------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetUserById
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            UserId,
            Email,
            PasswordHash,
            Phone,
            Role,
            IsEmailVerified,
            IsActive,
            LastLogin,
            FailedLoginAttempts,
            LockoutEnd,
            TenantId,
            CreatedBy,
            CreatedOn,
            UpdatedBy,
            UpdatedOn
        FROM Users 
        WHERE UserId = @UserId 
            AND IsDeleted = 0
        ORDER BY CreatedOn DESC;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO
----------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetUserByRefreshToken
    @RefreshToken NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            u.UserId,
            u.Email,
            u.PasswordHash,
            u.Phone,
            u.Role,
            u.IsEmailVerified,
            u.IsActive,
            u.LastLogin,
            u.FailedLoginAttempts,
            u.LockoutEnd,
            u.TenantId,
            u.CreatedBy,
            u.CreatedOn,
            u.UpdatedBy,
            u.UpdatedOn
        FROM Users u
        INNER JOIN RefreshTokens rt ON u.UserId = rt.UserId
        WHERE rt.Token = @RefreshToken 
            AND rt.IsRevoked = 0 
            AND rt.ExpiresAt > GETUTCDATE()
            AND u.IsDeleted = 0
            AND rt.IsDeleted = 0
        ORDER BY rt.CreatedOn DESC;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO
------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetVendorById
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        v.VendorId,
        v.BusinessName,
        v.OwnerName,
        v.BusinessLicenseNumber,
        v.BusinessAddress,
        v.City,
        v.State,
        v.PinCode,
        CASE v.Status
            WHEN 0 THEN 'PendingApproval'
            WHEN 1 THEN 'Active'
            WHEN 2 THEN 'Inactive'
            WHEN 3 THEN 'Suspended'
            WHEN 4 THEN 'Rejected'
        END as Status,
        CASE v.CurrentPlan
            WHEN 1 THEN 'Basic'
            WHEN 2 THEN 'Standard'
            WHEN 3 THEN 'Premium'
        END as CurrentPlan
    FROM Vendors v
    WHERE v.VendorId = @VendorId 
        AND v.IsDeleted = 0
END
GO
---------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetVendorByUserId
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        v.VendorId,
        v.BusinessName,
        v.OwnerName,
        v.BusinessLicenseNumber,
        v.BusinessAddress,
        v.City,
        v.State,
        v.PinCode,
        CASE v.Status
            WHEN 0 THEN 'PendingApproval'
            WHEN 1 THEN 'Active'
            WHEN 2 THEN 'Inactive'
            WHEN 3 THEN 'Suspended'
            WHEN 4 THEN 'Rejected'
        END as Status,
        CASE v.CurrentPlan
            WHEN 1 THEN 'Basic'
            WHEN 2 THEN 'Standard'
            WHEN 3 THEN 'Premium'
        END as CurrentPlan
    FROM Vendors v
    WHERE v.UserId = @UserId 
        AND v.IsDeleted = 0
END
GO
----------------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_ValidateUserCredentials
    @Email NVARCHAR(150),
    @PasswordHash NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        DECLARE @IsValid BIT = 0;
        
        IF EXISTS (
            SELECT 1 
            FROM Users 
            WHERE Email = @Email 
                AND PasswordHash = @PasswordHash 
                AND IsDeleted = 0
                AND IsActive = 1
        )
        BEGIN
            SET @IsValid = 1;
        END
        
        SELECT @IsValid AS IsValid;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO
----------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_CheckBusinessLicenseExists
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
----------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_CheckEmailExists
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
----------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_CheckPhoneExists
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
----------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetPendingVendors
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        v.VendorId,
        v.BusinessName,
        v.OwnerName,
        v.BusinessLicenseNumber,
        v.BusinessAddress,
        v.City,
        v.State,
        v.PinCode,
        v.TaxRegistrationNumber,
        v.DocumentPath,
        u.Email,
        u.Phone,
        v.CreatedOn,
        v.Status,
        CASE v.Status
            WHEN 0 THEN 'PendingApproval'
            WHEN 1 THEN 'Active'
            WHEN 2 THEN 'Inactive'
            WHEN 3 THEN 'Suspended'
            WHEN 4 THEN 'Rejected'
        END as StatusText
    FROM Vendors v
    INNER JOIN Users u ON v.UserId = u.UserId
    WHERE v.Status = 0 -- PendingApproval
        AND v.IsDeleted = 0
        AND u.IsDeleted = 0
    ORDER BY v.CreatedOn DESC
END
GO
----------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetVendorForApproval
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        v.VendorId,
        v.BusinessName,
        v.OwnerName,
        v.BusinessLicenseNumber,
        v.BusinessAddress,
        v.City,
        v.State,
        v.PinCode,
        v.TaxRegistrationNumber,
        v.DocumentPath,
        u.Email,
        u.Phone,
        v.CreatedOn,
        v.Status,
        CASE v.Status
            WHEN 0 THEN 'PendingApproval'
            WHEN 1 THEN 'Active'
            WHEN 2 THEN 'Inactive'
            WHEN 3 THEN 'Suspended'
            WHEN 4 THEN 'Rejected'
        END as StatusText
    FROM Vendors v
    INNER JOIN Users u ON v.UserId = u.UserId
    WHERE v.VendorId = @VendorId
        AND v.IsDeleted = 0
        AND u.IsDeleted = 0
END
GO
----------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetVendorForApprovalWithDetails
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        -- Vendor Information
        v.VendorId,
        v.BusinessName,
        v.OwnerName,
        v.BusinessLicenseNumber,
        v.BusinessAddress,
        v.City,
        v.State,
        v.PinCode,
        v.TaxRegistrationNumber,
        v.DocumentPath,
        v.DocumentStatus,
        v.VerifiedOn,
        v.VerifiedBy,
        v.RejectionReason,
        v.CurrentPlan,
        v.Status,
        v.IsActive,
        v.CreatedOn AS VendorCreatedOn,
        v.UpdatedOn AS VendorUpdatedOn,
        v.CreatedBy AS VendorCreatedBy,
        v.UpdatedBy AS VendorUpdatedBy,
        
        -- User Information
        u.UserId,
        u.Email,
        u.Phone,
        u.Role,
        u.IsEmailVerified,
        u.CreatedOn AS UserCreatedOn
    FROM 
        Vendors v
        INNER JOIN Users u ON v.UserId = u.UserId
    WHERE 
        v.VendorId = @VendorId
        AND v.IsDeleted = 0
        AND u.IsDeleted = 0
    ORDER BY 
        v.CreatedOn DESC;
END
GO
----------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetAllProducts
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        p.ProductId,
        p.Name,
        p.Description,
        p.SKU,
        p.Price,
        p.DiscountPrice,
        p.StockQuantity,
        p.MinStockLevel,
        p.Brand,
        p.Weight,
        p.Dimensions,
        p.Status,
        p.IsActive,
        p.CreatedOn,
        p.UpdatedOn,
        p.CreatedBy,
        p.UpdatedBy,
        p.VendorId,
        v.BusinessName AS VendorName,
        p.CategoryId,
        c.Name AS CategoryName,
        (SELECT TOP 1 ImagePath FROM ProductImages pi WHERE pi.ProductId = p.ProductId AND pi.IsDeleted = 0 AND pi.IsPrimary = 1) AS PrimaryImagePath
    FROM 
        Products p
        INNER JOIN Vendors v ON p.VendorId = v.VendorId
        INNER JOIN Categories c ON p.CategoryId = c.CategoryId
    WHERE 
        p.IsDeleted = 0
        AND v.IsDeleted = 0
        AND c.IsDeleted = 0
        AND p.Status = 1 -- Active products only
    ORDER BY 
        p.CreatedOn DESC;
END
GO
----------------------------------------------------------
CREATE PROCEDURE sp_GetProductById
    @ProductId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get product details with vendor and category information
    SELECT 
        p.ProductId,
        p.Name,
        p.Description,
        p.SKU,
        p.Price,
        p.DiscountPrice,
        p.StockQuantity,
        p.MinStockLevel,
        p.Brand,
        p.Weight,
        p.Dimensions,
        p.Status,
        p.IsActive,
        p.CreatedOn,
        p.UpdatedOn,
        p.CreatedBy,
        p.UpdatedBy,
        p.VendorId,
        v.BusinessName AS VendorName,
        p.CategoryId,
        c.Name AS CategoryName
    FROM 
        Products p
        INNER JOIN Vendors v ON p.VendorId = v.VendorId
        INNER JOIN Categories c ON p.CategoryId = c.CategoryId
    WHERE 
        p.ProductId = @ProductId
        AND p.IsDeleted = 0
        AND v.IsDeleted = 0
        AND c.IsDeleted = 0;
    
    -- Get product images
    SELECT 
        ImagePath
    FROM 
        ProductImages
    WHERE 
        ProductId = @ProductId
        AND IsDeleted = 0
    ORDER BY 
        CreatedOn ASC;
END
GO
----------------------------------------------------------
CREATE PROCEDURE sp_GetProductsByVendor
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        p.ProductId,
        p.Name,
        p.Description,
        p.SKU,
        p.Price,
        p.DiscountPrice,
        p.StockQuantity,
        p.MinStockLevel,
        p.Brand,
        p.Weight,
        p.Dimensions,
        p.Status,
        p.IsActive,
        p.CreatedOn,
        p.UpdatedOn,
        p.CreatedBy,
        p.UpdatedBy,
        p.VendorId,
        v.BusinessName AS VendorName,
        p.CategoryId,
        c.Name AS CategoryName,
        (SELECT TOP 1 ImagePath FROM ProductImages pi WHERE pi.ProductId = p.ProductId AND pi.IsDeleted = 0 ORDER BY pi.CreatedOn ASC) AS PrimaryImagePath
    FROM 
        Products p
        INNER JOIN Vendors v ON p.VendorId = v.VendorId
        INNER JOIN Categories c ON p.CategoryId = c.CategoryId
    WHERE 
        p.VendorId = @VendorId
        AND p.IsDeleted = 0
        AND v.IsDeleted = 0
        AND c.IsDeleted = 0
    ORDER BY 
        p.CreatedOn DESC;
END
GO
----------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetProductsByCategory
    @CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        p.ProductId,
        p.Name,
        p.Description,
        p.SKU,
        p.Price,
        p.DiscountPrice,
        p.StockQuantity,
        p.MinStockLevel,
        p.Brand,
        p.Weight,
        p.Dimensions,
        p.Status,
        p.IsActive,
        p.CreatedOn,
        p.UpdatedOn,
        p.CreatedBy,
        p.UpdatedBy,
        p.VendorId,
        v.BusinessName AS VendorName,
        p.CategoryId,
        c.Name AS CategoryName,
        (SELECT TOP 1 ImagePath FROM ProductImages pi WHERE pi.ProductId = p.ProductId AND pi.IsDeleted = 0 ORDER BY pi.CreatedOn ASC) AS PrimaryImagePath
    FROM 
        Products p
        INNER JOIN Vendors v ON p.VendorId = v.VendorId
        INNER JOIN Categories c ON p.CategoryId = c.CategoryId
    WHERE 
        p.CategoryId = @CategoryId
        AND p.IsDeleted = 0
        AND v.IsDeleted = 0
        AND c.IsDeleted = 0
        AND p.Status = 1 -- Active products only
    ORDER BY 
        p.CreatedOn DESC;
END
GO
----------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_CheckSKUExists
    @SKU NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        CASE 
            WHEN EXISTS (
                SELECT 1 
                FROM Products 
                WHERE SKU = @SKU 
                AND IsDeleted = 0
            ) 
            THEN CAST(1 AS BIT)
            ELSE CAST(0 AS BIT)
        END AS SKUExists;
END
GO
----------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_ValidateCategory
    @CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        CASE 
            WHEN EXISTS (
                SELECT 1 
                FROM Categories 
                WHERE CategoryId = @CategoryId 
                AND IsDeleted = 0 
                AND IsActive = 1
            ) 
            THEN CAST(1 AS BIT)
            ELSE CAST(0 AS BIT)
        END AS CategoryValid;
END
GO
----------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_ValidateVendor
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        CASE 
            WHEN EXISTS (
                SELECT 1 
                FROM Vendors 
                WHERE VendorId = @VendorId 
                AND IsDeleted = 0 
                AND IsActive = 1
                AND Status = 1 -- Active status
            ) 
            THEN CAST(1 AS BIT)
            ELSE CAST(0 AS BIT)
        END AS VendorValid;
END
GO
----------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_CheckCartItemExists
    @CustomerId INT,
    @ProductId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        CASE 
            WHEN EXISTS (
                SELECT 1 FROM Cart 
                WHERE CustomerId = @CustomerId 
                AND ProductId = @ProductId 
                AND IsDeleted = 0
            ) 
            THEN CAST(1 AS BIT)
            ELSE CAST(0 AS BIT)
        END AS ItemExists;
END
GO
----------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetCartByCustomerId
    @CustomerId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.CartId,
        c.ProductId,
        p.Name AS ProductName,
        p.SKU AS ProductSKU,
        p.Price,
        p.DiscountPrice,
        c.Quantity,
        CASE 
            WHEN p.DiscountPrice IS NOT NULL THEN p.DiscountPrice * c.Quantity
            ELSE p.Price * c.Quantity
        END AS TotalPrice,
        (SELECT TOP 1 ImagePath FROM ProductImages pi WHERE pi.ProductId = p.ProductId AND pi.IsDeleted = 0 AND pi.IsPrimary = 1) AS ProductImage,
        v.VendorId,
        v.BusinessName AS VendorName,
        p.StockQuantity,
        p.IsActive,
        c.CreatedOn AS AddedOn
    FROM 
        Cart c
        INNER JOIN Products p ON c.ProductId = p.ProductId
        INNER JOIN Vendors v ON p.VendorId = v.VendorId
    WHERE 
        c.CustomerId = @CustomerId
        AND c.IsDeleted = 0
        AND p.IsDeleted = 0
        AND v.IsDeleted = 0
    ORDER BY 
        c.CreatedOn DESC;
END
GO
----------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetAllCategories
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.CategoryId,
        c.Name,
        c.Description,
        c.ImagePath,
        c.IsActive,
        c.ParentCategoryId,
        pc.Name AS ParentCategoryName,
        c.CreatedOn
    FROM Categories c
    LEFT JOIN Categories pc ON c.ParentCategoryId = pc.CategoryId
    WHERE c.IsDeleted = 0
    ORDER BY c.CreatedOn DESC;
END
GO
----------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetOrderById
    @OrderId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            o.OrderId,
            o.CustomerId,
            o.TotalAmount,
            o.ShippingAddress,
            o.Status,
            o.CreatedOn,
            c.FirstName + ' ' + c.LastName AS CustomerName,
            u.Phone AS CustomerPhone,
            u.Email AS CustomerEmail
        FROM Orders o
        INNER JOIN Customers c ON o.CustomerId = c.CustomerId
        INNER JOIN Users u ON c.UserId = u.UserId
        WHERE o.OrderId = @OrderId 
            AND o.IsDeleted = 0
            AND c.IsDeleted = 0
            AND u.IsDeleted = 0;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO
----------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_CheckStockAvailability
    @ProductId INT,
    @RequiredQuantity INT,
    @IsAvailable BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @CurrentStock INT;
    
    -- Get current stock quantity
    SELECT @CurrentStock = StockQuantity 
    FROM Products 
    WHERE ProductId = @ProductId 
      AND IsDeleted = 0 
      AND IsActive = 1
      AND Status IN (1, 2); -- Active or OutOfStock status
    
    -- Check if we have enough stock
    IF @CurrentStock IS NULL
        SET @IsAvailable = 0; -- Product not found
    ELSE IF @CurrentStock >= @RequiredQuantity
        SET @IsAvailable = 1; -- Stock available
    ELSE
        SET @IsAvailable = 0; -- Insufficient stock
END
GO
----------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetOrderItems
    @OrderId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            oi.OrderItemId,
            oi.OrderId,
            oi.ProductId,
            oi.Quantity,
            oi.UnitPrice,
            oi.TotalPrice,
            oi.CreatedOn,
            p.Name AS ProductName,
            p.SKU AS ProductSKU,
            p.Brand,
            v.BusinessName AS VendorName,
            (SELECT TOP 1 ImagePath FROM ProductImages pi WHERE pi.ProductId = p.ProductId AND pi.IsDeleted = 0 AND pi.IsPrimary = 1) AS ProductImage
        FROM OrderItems oi
        INNER JOIN Products p ON oi.ProductId = p.ProductId
        INNER JOIN Vendors v ON p.VendorId = v.VendorId
        WHERE oi.OrderId = @OrderId 
            AND oi.IsDeleted = 0
            AND p.IsDeleted = 0
            AND v.IsDeleted = 0
        ORDER BY oi.CreatedOn DESC;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO
----------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetPaymentById
    @PaymentId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        p.PaymentId,
        p.OrderId,
        p.PaymentMethod,
        p.Status AS PaymentStatus,
        p.Amount,
        p.TransactionId,
        p.PaymentDate,
        p.CreatedOn
    FROM Payments p
    WHERE p.PaymentId = @PaymentId;
END
GO
----------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetPaymentByRazorpayOrderId
    @RazorpayOrderId NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        p.PaymentId,
        p.OrderId,
        p.PaymentMethod,
        p.Status AS PaymentStatus,
        p.Amount,
        p.TransactionId,
        p.PaymentDate,
        p.CreatedOn
    FROM Payments p
    WHERE p.TransactionId = @RazorpayOrderId;
END
GO
----------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetOrdersByCustomerId
    @CustomerId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        o.OrderId,
        o.CustomerId,
        o.TotalAmount,
        o.ShippingAddress,
        o.Status,
        o.CreatedOn
    FROM Orders o
    WHERE o.CustomerId = @CustomerId AND o.IsDeleted = 0
    ORDER BY o.CreatedOn DESC;
END
GO
----------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetOrdersByVendor
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        o.OrderId,
        o.OrderNumber,
        o.TotalAmount,
        o.ShippingAddress,
        o.ShippingCity,
        o.ShippingState,
        o.ShippingPinCode,
        o.ShippingPhone,
        o.Status,
        o.CreatedOn as OrderDate,
        c.FirstName + ' ' + c.LastName as CustomerName,
        u.Phone as CustomerPhone,
        u.Email as CustomerEmail,
        (SELECT COUNT(*) FROM OrderItems oi WHERE oi.OrderId = o.OrderId) as ItemCount,
        s.ShippingId,
        s.TrackingNumber,
        s.CourierService,
        s.Status as ShippingStatus,
        s.ShippedDate,
        s.EstimatedDeliveryDate,
        s.ActualDeliveryDate,
        s.DeliveryNotes,
        s.CreatedOn as ShippingCreatedOn
    FROM Orders o
    INNER JOIN Customers c ON o.CustomerId = c.CustomerId
    INNER JOIN Users u ON c.UserId = u.UserId
    LEFT JOIN Shipping s ON o.OrderId = s.OrderId
    WHERE o.VendorId = @VendorId 
        AND o.IsDeleted = 0
    ORDER BY o.CreatedOn DESC;
    
    -- Get Order Items for all orders
    SELECT 
        oi.OrderItemId,
        oi.OrderId,
        oi.ProductId,
        oi.Quantity,
        oi.UnitPrice,
        oi.TotalPrice,
        p.Name as ProductName,
        p.SKU as ProductSKU,
        (SELECT TOP 1 ImagePath FROM ProductImages pi WHERE pi.ProductId = p.ProductId) as ProductImage
    FROM OrderItems oi
    INNER JOIN Products p ON oi.ProductId = p.ProductId
    INNER JOIN Orders o ON oi.OrderId = o.OrderId
    WHERE o.VendorId = @VendorId 
        AND o.IsDeleted = 0
    ORDER BY oi.OrderId, oi.OrderItemId;
END
GO
----------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetShippingsByVendor
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        s.ShippingId,
        s.OrderId,
        s.TrackingNumber,
        s.CourierService,
        s.Status,
        s.ShippedDate,
        s.EstimatedDeliveryDate,
        s.ActualDeliveryDate,
        s.DeliveryNotes,
        s.CreatedBy,
        s.CreatedOn,
        s.TenantId
    FROM Shipping s
    INNER JOIN Orders o ON s.OrderId = o.OrderId
    WHERE o.VendorId = @VendorId
        AND s.IsDeleted = 0
        AND o.IsDeleted = 0
    ORDER BY s.CreatedOn DESC;
END
GO
----------------------------------------------------------
CREATE PROCEDURE sp_GetShippingById
    @ShippingId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        ShippingId,
        OrderId,
        TrackingNumber,
        CourierService,
        Status,
        ShippedDate,
        EstimatedDeliveryDate,
        ActualDeliveryDate,
        DeliveryNotes,
        CreatedBy,
        CreatedOn,
        TenantId
    FROM Shipping
    WHERE ShippingId = @ShippingId 
        AND IsDeleted = 0;
END
GO
----------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetDashboardAnalytics
    @StartDate DATETIME,
    @EndDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Today DATE = CAST(GETDATE() AS DATE);
    DECLARE @ThisMonthStart DATE = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1);
    DECLARE @ThisYearStart DATE = DATEFROMPARTS(YEAR(GETDATE()), 1, 1);
    DECLARE @LastMonthStart DATE = DATEADD(MONTH, -1, @ThisMonthStart);
    DECLARE @LastMonthEnd DATE = DATEADD(DAY, -1, @ThisMonthStart);
    
    SELECT 
        -- Revenue Analytics
        ISNULL((SELECT SUM(o.TotalAmount) 
                FROM Orders o 
                INNER JOIN Payments p ON o.OrderId = p.OrderId 
                WHERE CAST(o.CreatedOn AS DATE) = @Today 
                AND p.Status = 2), 0) AS TotalRevenueToday,
                
        ISNULL((SELECT SUM(o.TotalAmount) 
                FROM Orders o 
                INNER JOIN Payments p ON o.OrderId = p.OrderId 
                WHERE o.CreatedOn >= @ThisMonthStart 
                AND p.Status = 2), 0) AS TotalRevenueThisMonth,
                
        ISNULL((SELECT SUM(o.TotalAmount) 
                FROM Orders o 
                INNER JOIN Payments p ON o.OrderId = p.OrderId 
                WHERE o.CreatedOn >= @ThisYearStart 
                AND p.Status = 2), 0) AS TotalRevenueThisYear,
                
        -- Revenue Growth Rate (This Month vs Last Month)
        CASE 
            WHEN (SELECT SUM(o.TotalAmount) 
                  FROM Orders o 
                  INNER JOIN Payments p ON o.OrderId = p.OrderId 
                  WHERE o.CreatedOn >= @LastMonthStart AND o.CreatedOn <= @LastMonthEnd 
                  AND p.Status = 2) > 0
            THEN ((SELECT SUM(o.TotalAmount) 
                   FROM Orders o 
                   INNER JOIN Payments p ON o.OrderId = p.OrderId 
                   WHERE o.CreatedOn >= @ThisMonthStart 
                   AND p.Status = 2) - 
                  (SELECT SUM(o.TotalAmount) 
                   FROM Orders o 
                   INNER JOIN Payments p ON o.OrderId = p.OrderId 
                   WHERE o.CreatedOn >= @LastMonthStart AND o.CreatedOn <= @LastMonthEnd 
                   AND p.Status = 2)) * 100.0 / 
                  (SELECT SUM(o.TotalAmount) 
                   FROM Orders o 
                   INNER JOIN Payments p ON o.OrderId = p.OrderId 
                   WHERE o.CreatedOn >= @LastMonthStart AND o.CreatedOn <= @LastMonthEnd 
                   AND p.Status = 2)
            ELSE 0
        END AS RevenueGrowthRate,
        
        -- Order Analytics
        (SELECT COUNT(*) FROM Orders WHERE CAST(CreatedOn AS DATE) = @Today) AS TotalOrdersToday,
        (SELECT COUNT(*) FROM Orders WHERE CreatedOn >= @ThisMonthStart) AS TotalOrdersThisMonth,
        (SELECT COUNT(*) FROM Orders WHERE Status = 1) AS PendingOrders,
        (SELECT COUNT(*) FROM Orders WHERE Status = 3) AS ProcessingOrders,
        (SELECT COUNT(*) FROM Orders WHERE Status = 5) AS CompletedOrders,
        
        -- Customer Analytics
        (SELECT COUNT(*) FROM Customers WHERE CAST(CreatedOn AS DATE) = @Today) AS TotalCustomersToday,
        (SELECT COUNT(*) FROM Customers WHERE CreatedOn >= @ThisMonthStart) AS TotalCustomersThisMonth,
        (SELECT COUNT(DISTINCT c.CustomerId) 
         FROM Customers c 
         INNER JOIN Orders o ON c.CustomerId = o.CustomerId 
         WHERE o.CreatedOn >= DATEADD(DAY, -30, GETDATE())) AS ActiveCustomers,
        
        -- Vendor Analytics
        (SELECT COUNT(*) FROM Vendors WHERE IsActive = 1) AS TotalVendors,
        (SELECT COUNT(*) FROM Vendors WHERE Status = 1 AND IsActive = 1) AS ActiveVendors,
        (SELECT COUNT(*) FROM Vendors WHERE Status = 0) AS PendingVendorApplications,
        
        -- Product Analytics
        (SELECT COUNT(*) FROM Products WHERE IsActive = 1) AS TotalProducts,
        (SELECT COUNT(*) FROM Products 
         WHERE IsActive = 1 AND MinStockLevel IS NOT NULL 
         AND StockQuantity <= MinStockLevel) AS LowStockProductsCount,
        (SELECT COUNT(*) FROM Products WHERE IsActive = 1 AND StockQuantity = 0) AS OutOfStockProductsCount,
        
        -- Business Metrics
        CASE 
            WHEN (SELECT COUNT(*) FROM Orders WHERE CreatedOn >= @ThisMonthStart) > 0
            THEN (SELECT SUM(o.TotalAmount) FROM Orders o 
                  INNER JOIN Payments p ON o.OrderId = p.OrderId 
                  WHERE o.CreatedOn >= @ThisMonthStart AND p.Status = 2) / 
                 (SELECT COUNT(*) FROM Orders WHERE CreatedOn >= @ThisMonthStart)
            ELSE 0
        END AS AverageOrderValue,
        
        -- Conversion Rate (Orders with successful payments / Total Orders)
        CASE 
            WHEN (SELECT COUNT(*) FROM Orders WHERE CreatedOn >= @ThisMonthStart) > 0
            THEN (SELECT COUNT(*) FROM Orders o 
                  INNER JOIN Payments p ON o.OrderId = p.OrderId 
                  WHERE o.CreatedOn >= @ThisMonthStart AND p.Status = 2) * 100.0 / 
                 (SELECT COUNT(*) FROM Orders WHERE CreatedOn >= @ThisMonthStart)
            ELSE 0
        END AS ConversionRate;
END
GO
----------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetRecentActivities
    @Limit INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get recent activities from multiple sources, ordered by most recent first (Rule #9)
    SELECT TOP (@Limit)
        ActivityType,
        Description,
        CreatedOn,
        UserName
    FROM (
        -- New Orders
        SELECT 
            'Order' AS ActivityType,
            'New order #' + o.OrderNumber + ' placed by ' + c.FirstName + ' ' + c.LastName AS Description,
            o.CreatedOn,
            c.FirstName + ' ' + c.LastName AS UserName
        FROM Orders o
        INNER JOIN Customers c ON o.CustomerId = c.CustomerId
        WHERE o.CreatedOn >= DATEADD(DAY, -7, GETDATE())
        
        UNION ALL
        
        -- New Customer Registrations
        SELECT 
            'Registration' AS ActivityType,
            'New customer registered: ' + c.FirstName + ' ' + c.LastName AS Description,
            c.CreatedOn,
            c.FirstName + ' ' + c.LastName AS UserName
        FROM Customers c
        WHERE c.CreatedOn >= DATEADD(DAY, -7, GETDATE())
        
        UNION ALL
        
        -- Vendor Applications
        SELECT 
            'Vendor Application' AS ActivityType,
            'New vendor application: ' + v.BusinessName AS Description,
            v.CreatedOn,
            v.OwnerName AS UserName
        FROM Vendors v
        WHERE v.CreatedOn >= DATEADD(DAY, -7, GETDATE())
        
        UNION ALL
        
        -- Product Additions
        SELECT 
            'Product' AS ActivityType,
            'New product added: ' + p.Name + ' by ' + v.BusinessName AS Description,
            p.CreatedOn,
            v.OwnerName AS UserName
        FROM Products p
        INNER JOIN Vendors v ON p.VendorId = v.VendorId
        WHERE p.CreatedOn >= DATEADD(DAY, -7, GETDATE())
        
        UNION ALL
        
        -- Successful Payments
        SELECT 
            'Payment' AS ActivityType,
            'Payment completed for order #' + o.OrderNumber + ' - ₹' + CAST(p.Amount AS VARCHAR(20)) AS Description,
            p.PaymentDate,
            c.FirstName + ' ' + c.LastName AS UserName
        FROM Payments p
        INNER JOIN Orders o ON p.OrderId = o.OrderId
        INNER JOIN Customers c ON o.CustomerId = c.CustomerId
        WHERE p.Status = 2 AND p.PaymentDate >= DATEADD(DAY, -7, GETDATE())
    ) AS Activities
    ORDER BY CreatedOn DESC; -- Rule #9: Recently added records should be top
END
GO
----------------------------------------------------------
CREATE OR ALTER PROCEDURE sp_GetTopSellingProducts
    @StartDate DATETIME,
    @EndDate DATETIME,
    @Limit INT = 5
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get top selling products within date range, ordered by total sold descending (Rule #9)
    SELECT TOP (@Limit)
        p.ProductId,
        p.Name AS ProductName,
        SUM(oi.Quantity) AS TotalSold,
        SUM(oi.Quantity * oi.UnitPrice) AS Revenue
    FROM Products p
    INNER JOIN OrderItems oi ON p.ProductId = oi.ProductId
    INNER JOIN Orders o ON oi.OrderId = o.OrderId
    INNER JOIN Payments pay ON o.OrderId = pay.OrderId
    WHERE o.CreatedOn >= @StartDate 
        AND o.CreatedOn <= @EndDate
        AND pay.Status = 2 -- Only successful payments
        AND p.IsActive = 1
    GROUP BY p.ProductId, p.Name
    ORDER BY TotalSold DESC, Revenue DESC; -- Rule #9: Top performers first
END
GO
----------------------------------------------------------
select * from users
select * from customers
select * from Vendors
select * from Categories
select * from RefreshTokens
select * from Products
select * from ProductImages
select * from Cart


DELETE FROM Users WHERE Email = '';
delete from Vendors where VendorId=5
delete from users where UserId=5
-- Insert new admin with properly hashed password for "Admin123"
INSERT INTO Users (Email, PasswordHash, Phone, Role, IsActive, IsEmailVerified, FailedLoginAttempts, IsDeleted, CreatedBy, CreatedOn, TenantId)
VALUES (
    'sujithkumar.kanini@outlook.com',
    '$2a$11$8gF7YQvnAb8rKtXKjxeOUeQxQxQxQxQxQxQxQxQxQxQxQxQxQxQxQx',
    '9385562091',
    3, -- Admin role
    1, -- IsActive
    1, -- IsEmailVerified
    0, -- FailedLoginAttempts
    0, -- IsDeleted
    'System',
    GETUTCDATE(),
    'admin'
);

UPDATE Users 
SET PasswordHash = 'O2Esdae1BIpDX7bsgeUv+S1teVqLWpwXBw9qY8l6U7I='
WHERE Email = 'sujithkumar.kanini@outlook.com';

Update Users set IsActive =1 where UserId=6
UPDATE Products SET Status = 1 WHERE ProductId = 1;

ALTER DATABASE SCOPED CONFIGURATION SET IDENTITY_CACHE = OFF;

Truncate table customers
