CREATE PROCEDURE sp_GetCustomerSegments
    @StartDate DATETIME,
    @EndDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get total revenue for percentage calculation
    DECLARE @TotalRevenue DECIMAL(18,2) = (
        SELECT ISNULL(SUM(o.TotalAmount), 0)
        FROM Orders o
        INNER JOIN Payments p ON o.OrderId = p.OrderId
        WHERE o.CreatedOn >= @StartDate 
            AND o.CreatedOn <= @EndDate
            AND p.Status = 2
    );
    
    -- Customer segmentation based on spending behavior, ordered by revenue descending (Rule #9: Top segments first)
    WITH CustomerSpending AS (
        SELECT 
            c.CustomerId,
            ISNULL(SUM(o.TotalAmount), 0) AS TotalSpent,
            COUNT(o.OrderId) AS OrderCount
        FROM Customers c
        LEFT JOIN Orders o ON c.CustomerId = o.CustomerId
        LEFT JOIN Payments p ON o.OrderId = p.OrderId
        WHERE (o.CreatedOn IS NULL OR (o.CreatedOn >= @StartDate AND o.CreatedOn <= @EndDate))
            AND (p.Status IS NULL OR p.Status = 2)
            AND c.IsActive = 1
        GROUP BY c.CustomerId
    ),
    CustomerSegments AS (
        SELECT 
            CustomerId,
            TotalSpent,
            OrderCount,
            CASE 
                WHEN TotalSpent >= 50000 THEN 'VIP Customers'
                WHEN TotalSpent >= 20000 THEN 'High Value Customers'
                WHEN TotalSpent >= 5000 THEN 'Regular Customers'
                WHEN TotalSpent > 0 THEN 'New Customers'
                ELSE 'Inactive Customers'
            END AS SegmentName
        FROM CustomerSpending
    )
    SELECT 
        SegmentName,
        COUNT(CustomerId) AS CustomerCount,
        CASE 
            WHEN COUNT(CustomerId) > 0 
            THEN AVG(TotalSpent)
            ELSE 0 
        END AS AverageOrderValue,
        SUM(TotalSpent) AS TotalRevenue,
        CASE 
            WHEN @TotalRevenue > 0 
            THEN (SUM(TotalSpent) * 100.0 / @TotalRevenue)
            ELSE 0 
        END AS Percentage
    FROM CustomerSegments
    GROUP BY SegmentName
    ORDER BY TotalRevenue DESC; -- Rule #9: Top revenue segments first
END