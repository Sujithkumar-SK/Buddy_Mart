CREATE PROCEDURE sp_GetCustomerAnalytics
    @StartDate DATETIME,
    @EndDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @PeriodDays INT = DATEDIFF(DAY, @StartDate, @EndDate);
    DECLARE @PreviousStartDate DATETIME = DATEADD(DAY, -@PeriodDays, @StartDate);
    DECLARE @PreviousEndDate DATETIME = @StartDate;
    
    SELECT 
        -- Total Customers
        (SELECT COUNT(*) FROM Customers WHERE IsActive = 1) AS TotalCustomers,
        
        -- New Customers in period
        (SELECT COUNT(*) FROM Customers 
         WHERE CreatedOn >= @StartDate AND CreatedOn <= @EndDate) AS NewCustomers,
        
        -- Active Customers (placed order in last 30 days)
        (SELECT COUNT(DISTINCT c.CustomerId) 
         FROM Customers c 
         INNER JOIN Orders o ON c.CustomerId = o.CustomerId 
         WHERE o.CreatedOn >= DATEADD(DAY, -30, GETDATE()) 
         AND c.IsActive = 1) AS ActiveCustomers,
        
        -- Inactive Customers
        (SELECT COUNT(*) FROM Customers 
         WHERE IsActive = 1 
         AND CustomerId NOT IN (
             SELECT DISTINCT CustomerId FROM Orders 
             WHERE CreatedOn >= DATEADD(DAY, -30, GETDATE())
         )) AS InactiveCustomers,
        
        -- Customer Growth Rate
        CASE 
            WHEN (SELECT COUNT(*) FROM Customers 
                  WHERE CreatedOn >= @PreviousStartDate AND CreatedOn < @PreviousEndDate) > 0
            THEN ((SELECT COUNT(*) FROM Customers 
                   WHERE CreatedOn >= @StartDate AND CreatedOn <= @EndDate) - 
                  (SELECT COUNT(*) FROM Customers 
                   WHERE CreatedOn >= @PreviousStartDate AND CreatedOn < @PreviousEndDate)) * 100.0 / 
                  (SELECT COUNT(*) FROM Customers 
                   WHERE CreatedOn >= @PreviousStartDate AND CreatedOn < @PreviousEndDate)
            ELSE 0
        END AS CustomerGrowthRate,
        
        -- Customer Retention Rate (customers who made repeat orders)
        CASE 
            WHEN (SELECT COUNT(DISTINCT CustomerId) FROM Orders 
                  WHERE CreatedOn >= @StartDate AND CreatedOn <= @EndDate) > 0
            THEN (SELECT COUNT(DISTINCT o1.CustomerId) 
                  FROM Orders o1 
                  WHERE o1.CreatedOn >= @StartDate AND o1.CreatedOn <= @EndDate
                  AND EXISTS (
                      SELECT 1 FROM Orders o2 
                      WHERE o2.CustomerId = o1.CustomerId 
                      AND o2.OrderId != o1.OrderId
                      AND o2.CreatedOn < o1.CreatedOn
                  )) * 100.0 / 
                  (SELECT COUNT(DISTINCT CustomerId) FROM Orders 
                   WHERE CreatedOn >= @StartDate AND CreatedOn <= @EndDate)
            ELSE 0
        END AS CustomerRetentionRate,
        
        -- Customer Lifetime Value (average revenue per customer)
        CASE 
            WHEN (SELECT COUNT(DISTINCT c.CustomerId) 
                  FROM Customers c 
                  INNER JOIN Orders o ON c.CustomerId = o.CustomerId 
                  INNER JOIN Payments p ON o.OrderId = p.OrderId 
                  WHERE p.Status = 2) > 0
            THEN (SELECT SUM(o.TotalAmount) 
                  FROM Orders o 
                  INNER JOIN Payments p ON o.OrderId = p.OrderId 
                  WHERE p.Status = 2) / 
                 (SELECT COUNT(DISTINCT c.CustomerId) 
                  FROM Customers c 
                  INNER JOIN Orders o ON c.CustomerId = o.CustomerId 
                  INNER JOIN Payments p ON o.OrderId = p.OrderId 
                  WHERE p.Status = 2)
            ELSE 0
        END AS CustomerLifetimeValue,
        
        -- Average Orders Per Customer
        CASE 
            WHEN (SELECT COUNT(*) FROM Customers WHERE IsActive = 1) > 0
            THEN CAST((SELECT COUNT(*) FROM Orders) AS DECIMAL(18,2)) / 
                 (SELECT COUNT(*) FROM Customers WHERE IsActive = 1)
            ELSE 0
        END AS AverageOrdersPerCustomer,
        
        -- Average Revenue Per Customer
        CASE 
            WHEN (SELECT COUNT(*) FROM Customers WHERE IsActive = 1) > 0
            THEN (SELECT ISNULL(SUM(o.TotalAmount), 0) 
                  FROM Orders o 
                  INNER JOIN Payments p ON o.OrderId = p.OrderId 
                  WHERE p.Status = 2) / 
                 (SELECT COUNT(*) FROM Customers WHERE IsActive = 1)
            ELSE 0
        END AS AverageRevenuePerCustomer;
END