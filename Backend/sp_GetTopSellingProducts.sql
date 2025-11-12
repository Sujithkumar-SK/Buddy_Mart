CREATE PROCEDURE sp_GetTopSellingProducts
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