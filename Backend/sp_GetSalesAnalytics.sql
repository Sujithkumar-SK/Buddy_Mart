CREATE PROCEDURE sp_GetSalesAnalytics
    @StartDate DATETIME,
    @EndDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @PeriodDays INT = DATEDIFF(DAY, @StartDate, @EndDate);
    DECLARE @PreviousStartDate DATETIME = DATEADD(DAY, -@PeriodDays, @StartDate);
    DECLARE @PreviousEndDate DATETIME = @StartDate;
    
    SELECT 
        -- Current Period Revenue
        ISNULL((SELECT SUM(o.TotalAmount) 
                FROM Orders o 
                INNER JOIN Payments p ON o.OrderId = p.OrderId 
                WHERE o.CreatedOn >= @StartDate AND o.CreatedOn <= @EndDate 
                AND p.Status = 2), 0) AS TotalRevenue,
                
        -- Previous Period Revenue
        ISNULL((SELECT SUM(o.TotalAmount) 
                FROM Orders o 
                INNER JOIN Payments p ON o.OrderId = p.OrderId 
                WHERE o.CreatedOn >= @PreviousStartDate AND o.CreatedOn < @PreviousEndDate 
                AND p.Status = 2), 0) AS PreviousPeriodRevenue,
                
        -- Revenue Growth Rate
        CASE 
            WHEN (SELECT SUM(o.TotalAmount) 
                  FROM Orders o 
                  INNER JOIN Payments p ON o.OrderId = p.OrderId 
                  WHERE o.CreatedOn >= @PreviousStartDate AND o.CreatedOn < @PreviousEndDate 
                  AND p.Status = 2) > 0
            THEN ((SELECT SUM(o.TotalAmount) 
                   FROM Orders o 
                   INNER JOIN Payments p ON o.OrderId = p.OrderId 
                   WHERE o.CreatedOn >= @StartDate AND o.CreatedOn <= @EndDate 
                   AND p.Status = 2) - 
                  (SELECT SUM(o.TotalAmount) 
                   FROM Orders o 
                   INNER JOIN Payments p ON o.OrderId = p.OrderId 
                   WHERE o.CreatedOn >= @PreviousStartDate AND o.CreatedOn < @PreviousEndDate 
                   AND p.Status = 2)) * 100.0 / 
                  (SELECT SUM(o.TotalAmount) 
                   FROM Orders o 
                   INNER JOIN Payments p ON o.OrderId = p.OrderId 
                   WHERE o.CreatedOn >= @PreviousStartDate AND o.CreatedOn < @PreviousEndDate 
                   AND p.Status = 2)
            ELSE 0
        END AS GrowthRate,
        
        -- Current Period Orders
        (SELECT COUNT(*) 
         FROM Orders o 
         INNER JOIN Payments p ON o.OrderId = p.OrderId 
         WHERE o.CreatedOn >= @StartDate AND o.CreatedOn <= @EndDate 
         AND p.Status = 2) AS TotalOrders,
         
        -- Previous Period Orders
        (SELECT COUNT(*) 
         FROM Orders o 
         INNER JOIN Payments p ON o.OrderId = p.OrderId 
         WHERE o.CreatedOn >= @PreviousStartDate AND o.CreatedOn < @PreviousEndDate 
         AND p.Status = 2) AS PreviousPeriodOrders,
         
        -- Order Growth Rate
        CASE 
            WHEN (SELECT COUNT(*) 
                  FROM Orders o 
                  INNER JOIN Payments p ON o.OrderId = p.OrderId 
                  WHERE o.CreatedOn >= @PreviousStartDate AND o.CreatedOn < @PreviousEndDate 
                  AND p.Status = 2) > 0
            THEN ((SELECT COUNT(*) 
                   FROM Orders o 
                   INNER JOIN Payments p ON o.OrderId = p.OrderId 
                   WHERE o.CreatedOn >= @StartDate AND o.CreatedOn <= @EndDate 
                   AND p.Status = 2) - 
                  (SELECT COUNT(*) 
                   FROM Orders o 
                   INNER JOIN Payments p ON o.OrderId = p.OrderId 
                   WHERE o.CreatedOn >= @PreviousStartDate AND o.CreatedOn < @PreviousEndDate 
                   AND p.Status = 2)) * 100.0 / 
                  (SELECT COUNT(*) 
                   FROM Orders o 
                   INNER JOIN Payments p ON o.OrderId = p.OrderId 
                   WHERE o.CreatedOn >= @PreviousStartDate AND o.CreatedOn < @PreviousEndDate 
                   AND p.Status = 2)
            ELSE 0
        END AS OrderGrowthRate,
        
        -- Current Average Order Value
        CASE 
            WHEN (SELECT COUNT(*) 
                  FROM Orders o 
                  INNER JOIN Payments p ON o.OrderId = p.OrderId 
                  WHERE o.CreatedOn >= @StartDate AND o.CreatedOn <= @EndDate 
                  AND p.Status = 2) > 0
            THEN (SELECT SUM(o.TotalAmount) 
                  FROM Orders o 
                  INNER JOIN Payments p ON o.OrderId = p.OrderId 
                  WHERE o.CreatedOn >= @StartDate AND o.CreatedOn <= @EndDate 
                  AND p.Status = 2) / 
                 (SELECT COUNT(*) 
                  FROM Orders o 
                  INNER JOIN Payments p ON o.OrderId = p.OrderId 
                  WHERE o.CreatedOn >= @StartDate AND o.CreatedOn <= @EndDate 
                  AND p.Status = 2)
            ELSE 0
        END AS AverageOrderValue,
        
        -- Previous Average Order Value
        CASE 
            WHEN (SELECT COUNT(*) 
                  FROM Orders o 
                  INNER JOIN Payments p ON o.OrderId = p.OrderId 
                  WHERE o.CreatedOn >= @PreviousStartDate AND o.CreatedOn < @PreviousEndDate 
                  AND p.Status = 2) > 0
            THEN (SELECT SUM(o.TotalAmount) 
                  FROM Orders o 
                  INNER JOIN Payments p ON o.OrderId = p.OrderId 
                  WHERE o.CreatedOn >= @PreviousStartDate AND o.CreatedOn < @PreviousEndDate 
                  AND p.Status = 2) / 
                 (SELECT COUNT(*) 
                  FROM Orders o 
                  INNER JOIN Payments p ON o.OrderId = p.OrderId 
                  WHERE o.CreatedOn >= @PreviousStartDate AND o.CreatedOn < @PreviousEndDate 
                  AND p.Status = 2)
            ELSE 0
        END AS PreviousAverageOrderValue;
END