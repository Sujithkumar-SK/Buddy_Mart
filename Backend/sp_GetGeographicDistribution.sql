CREATE PROCEDURE sp_GetGeographicDistribution
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
    
    -- Geographic distribution by state and city, ordered by revenue descending (Rule #9: Top locations first)
    SELECT 
        ISNULL(c.State, 'Unknown') AS State,
        ISNULL(c.City, 'Unknown') AS City,
        COUNT(DISTINCT c.CustomerId) AS CustomerCount,
        ISNULL(SUM(o.TotalAmount), 0) AS Revenue,
        CASE 
            WHEN @TotalRevenue > 0 
            THEN (ISNULL(SUM(o.TotalAmount), 0) * 100.0 / @TotalRevenue)
            ELSE 0 
        END AS Percentage
    FROM Customers c
    LEFT JOIN Orders o ON c.CustomerId = o.CustomerId
    LEFT JOIN Payments p ON o.OrderId = p.OrderId
    WHERE (o.CreatedOn IS NULL OR (o.CreatedOn >= @StartDate AND o.CreatedOn <= @EndDate))
        AND (p.Status IS NULL OR p.Status = 2)
        AND c.IsActive = 1
    GROUP BY c.State, c.City
    HAVING COUNT(DISTINCT c.CustomerId) > 0
    ORDER BY Revenue DESC, CustomerCount DESC; -- Rule #9: Top revenue locations first
END