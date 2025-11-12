CREATE PROCEDURE sp_GetDashboardAnalytics
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