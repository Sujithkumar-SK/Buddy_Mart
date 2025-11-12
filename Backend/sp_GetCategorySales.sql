CREATE PROCEDURE sp_GetCategorySales
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
    
    -- Get category-wise sales data, ordered by revenue descending (Rule #9: Top performers first)
    SELECT 
        c.CategoryId,
        c.Name AS CategoryName,
        ISNULL(SUM(o.TotalAmount), 0) AS Revenue,
        COUNT(DISTINCT o.OrderId) AS OrderCount,
        CASE 
            WHEN @TotalRevenue > 0 
            THEN (ISNULL(SUM(o.TotalAmount), 0) * 100.0 / @TotalRevenue)
            ELSE 0 
        END AS Percentage
    FROM Categories c
    LEFT JOIN Products pr ON c.CategoryId = pr.CategoryId
    LEFT JOIN OrderItems oi ON pr.ProductId = oi.ProductId
    LEFT JOIN Orders o ON oi.OrderId = o.OrderId
    LEFT JOIN Payments p ON o.OrderId = p.OrderId
    WHERE (o.CreatedOn IS NULL OR (o.CreatedOn >= @StartDate AND o.CreatedOn <= @EndDate))
        AND (p.Status IS NULL OR p.Status = 2)
        AND c.IsActive = 1
    GROUP BY c.CategoryId, c.Name
    HAVING ISNULL(SUM(o.TotalAmount), 0) > 0
    ORDER BY Revenue DESC; -- Rule #9: Top revenue categories first
END