CREATE PROCEDURE sp_GetPaymentMethodSales
    @StartDate DATETIME,
    @EndDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get total revenue for percentage calculation
    DECLARE @TotalRevenue DECIMAL(18,2) = (
        SELECT ISNULL(SUM(p.Amount), 0)
        FROM Payments p
        INNER JOIN Orders o ON p.OrderId = o.OrderId
        WHERE o.CreatedOn >= @StartDate 
            AND o.CreatedOn <= @EndDate
            AND p.Status = 2
    );
    
    -- Get payment method wise sales data, ordered by revenue descending (Rule #9: Top performers first)
    SELECT 
        CASE p.PaymentMethod
            WHEN 1 THEN 'Mock Payment'
            WHEN 2 THEN 'UPI'
            WHEN 3 THEN 'Credit/Debit Card'
            WHEN 4 THEN 'Net Banking'
            WHEN 5 THEN 'Digital Wallet'
            ELSE 'Other'
        END AS PaymentMethod,
        ISNULL(SUM(p.Amount), 0) AS Revenue,
        COUNT(p.PaymentId) AS TransactionCount,
        CASE 
            WHEN @TotalRevenue > 0 
            THEN (ISNULL(SUM(p.Amount), 0) * 100.0 / @TotalRevenue)
            ELSE 0 
        END AS Percentage
    FROM Payments p
    INNER JOIN Orders o ON p.OrderId = o.OrderId
    WHERE o.CreatedOn >= @StartDate 
        AND o.CreatedOn <= @EndDate
        AND p.Status = 2 -- Only successful payments
    GROUP BY p.PaymentMethod
    ORDER BY Revenue DESC; -- Rule #9: Top revenue payment methods first
END