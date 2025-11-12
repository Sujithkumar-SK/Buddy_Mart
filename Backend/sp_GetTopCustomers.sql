CREATE PROCEDURE sp_GetTopCustomers
    @StartDate DATETIME,
    @EndDate DATETIME,
    @Limit INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get top customers by total spending, ordered by spending descending (Rule #9: Top spenders first)
    SELECT TOP (@Limit)
        c.CustomerId,
        c.FirstName + ' ' + ISNULL(c.MiddleName + ' ', '') + c.LastName AS CustomerName,
        u.Email,
        COUNT(o.OrderId) AS TotalOrders,
        ISNULL(SUM(o.TotalAmount), 0) AS TotalSpent,
        MAX(o.CreatedOn) AS LastOrderDate
    FROM Customers c
    INNER JOIN Users u ON c.UserId = u.UserId
    LEFT JOIN Orders o ON c.CustomerId = o.CustomerId
    LEFT JOIN Payments p ON o.OrderId = p.OrderId
    WHERE (o.CreatedOn IS NULL OR (o.CreatedOn >= @StartDate AND o.CreatedOn <= @EndDate))
        AND (p.Status IS NULL OR p.Status = 2) -- Only successful payments
        AND c.IsActive = 1
    GROUP BY c.CustomerId, c.FirstName, c.MiddleName, c.LastName, u.Email
    HAVING ISNULL(SUM(o.TotalAmount), 0) > 0
    ORDER BY TotalSpent DESC, TotalOrders DESC; -- Rule #9: Top spenders first, then by order count
END