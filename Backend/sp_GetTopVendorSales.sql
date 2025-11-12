CREATE PROCEDURE sp_GetTopVendorSales
    @StartDate DATETIME,
    @EndDate DATETIME,
    @Limit INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get top vendor sales data, ordered by revenue descending (Rule #9: Top performers first)
    SELECT TOP (@Limit)
        v.VendorId,
        v.BusinessName AS VendorName,
        ISNULL(SUM(o.TotalAmount), 0) AS Revenue,
        COUNT(DISTINCT o.OrderId) AS OrderCount,
        -- Calculate commission (assuming 5% commission rate)
        ISNULL(SUM(o.TotalAmount), 0) * 0.05 AS Commission
    FROM Vendors v
    LEFT JOIN Orders o ON v.VendorId = o.VendorId
    LEFT JOIN Payments p ON o.OrderId = p.OrderId
    WHERE (o.CreatedOn IS NULL OR (o.CreatedOn >= @StartDate AND o.CreatedOn <= @EndDate))
        AND (p.Status IS NULL OR p.Status = 2) -- Only successful payments
        AND v.IsActive = 1
        AND v.Status = 1 -- Only active vendors
    GROUP BY v.VendorId, v.BusinessName
    HAVING ISNULL(SUM(o.TotalAmount), 0) > 0
    ORDER BY Revenue DESC; -- Rule #9: Top revenue vendors first
END