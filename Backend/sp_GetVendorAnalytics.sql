CREATE PROCEDURE sp_GetVendorAnalytics
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ThisMonthStart DATE = DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1);
    
    SELECT 
        -- Product Analytics
        (SELECT COUNT(*) FROM Products WHERE VendorId = @VendorId AND IsDeleted = 0) AS TotalProducts,
        (SELECT COUNT(*) FROM Products WHERE VendorId = @VendorId AND Status = 1 AND IsDeleted = 0) AS ActiveProducts,
        (SELECT COUNT(*) FROM Products 
         WHERE VendorId = @VendorId AND IsDeleted = 0 AND MinStockLevel IS NOT NULL 
         AND StockQuantity <= MinStockLevel) AS LowStockProducts,
        
        -- Order Analytics
        (SELECT COUNT(*) FROM Orders WHERE VendorId = @VendorId AND IsDeleted = 0) AS TotalOrders,
        (SELECT COUNT(*) FROM Orders WHERE VendorId = @VendorId AND Status = 1 AND IsDeleted = 0) AS PendingOrders,
        (SELECT COUNT(*) FROM Orders WHERE VendorId = @VendorId AND Status = 3 AND IsDeleted = 0) AS ProcessingOrders,
        (SELECT COUNT(*) FROM Orders WHERE VendorId = @VendorId AND Status = 5 AND IsDeleted = 0) AS CompletedOrders,
        
        -- Shipping Analytics
        (SELECT COUNT(*) FROM Shipping s 
         INNER JOIN Orders o ON s.OrderId = o.OrderId 
         WHERE o.VendorId = @VendorId AND s.IsDeleted = 0) AS TotalShipments,
        (SELECT COUNT(*) FROM Shipping s 
         INNER JOIN Orders o ON s.OrderId = o.OrderId 
         WHERE o.VendorId = @VendorId AND s.Status = 1 AND s.IsDeleted = 0) AS PendingShipments,
        (SELECT COUNT(*) FROM Shipping s 
         INNER JOIN Orders o ON s.OrderId = o.OrderId 
         WHERE o.VendorId = @VendorId AND s.Status = 2 AND s.IsDeleted = 0) AS ShippedOrders,
        (SELECT COUNT(*) FROM Shipping s 
         INNER JOIN Orders o ON s.OrderId = o.OrderId 
         WHERE o.VendorId = @VendorId AND s.Status = 4 AND s.IsDeleted = 0) AS DeliveredOrders,
        
        -- Revenue Analytics
        ISNULL((SELECT SUM(o.TotalAmount) 
                FROM Orders o 
                INNER JOIN Payments p ON o.OrderId = p.OrderId 
                WHERE o.VendorId = @VendorId AND p.Status = 2 AND o.IsDeleted = 0), 0) AS TotalRevenue,
        ISNULL((SELECT SUM(o.TotalAmount) 
                FROM Orders o 
                INNER JOIN Payments p ON o.OrderId = p.OrderId 
                WHERE o.VendorId = @VendorId AND o.CreatedOn >= @ThisMonthStart 
                AND p.Status = 2 AND o.IsDeleted = 0), 0) AS MonthlyRevenue,
        
        -- Average Order Value
        CASE 
            WHEN (SELECT COUNT(*) FROM Orders o 
                  INNER JOIN Payments p ON o.OrderId = p.OrderId 
                  WHERE o.VendorId = @VendorId AND p.Status = 2 AND o.IsDeleted = 0) > 0
            THEN (SELECT SUM(o.TotalAmount) FROM Orders o 
                  INNER JOIN Payments p ON o.OrderId = p.OrderId 
                  WHERE o.VendorId = @VendorId AND p.Status = 2 AND o.IsDeleted = 0) / 
                 (SELECT COUNT(*) FROM Orders o 
                  INNER JOIN Payments p ON o.OrderId = p.OrderId 
                  WHERE o.VendorId = @VendorId AND p.Status = 2 AND o.IsDeleted = 0)
            ELSE 0
        END AS AverageOrderValue;
END