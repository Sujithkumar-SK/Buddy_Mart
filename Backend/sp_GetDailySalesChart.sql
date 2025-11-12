CREATE PROCEDURE sp_GetDailySalesChart
    @StartDate DATETIME,
    @EndDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Generate daily sales data for chart, ordered by date ascending for proper chart display
    SELECT 
        CAST(o.CreatedOn AS DATE) AS Date,
        ISNULL(SUM(o.TotalAmount), 0) AS Revenue,
        COUNT(o.OrderId) AS OrderCount
    FROM Orders o
    INNER JOIN Payments p ON o.OrderId = p.OrderId
    WHERE o.CreatedOn >= @StartDate 
        AND o.CreatedOn <= @EndDate
        AND p.Status = 2 -- Only successful payments
    GROUP BY CAST(o.CreatedOn AS DATE)
    ORDER BY Date ASC; -- Rule #9: For charts, chronological order is needed
END