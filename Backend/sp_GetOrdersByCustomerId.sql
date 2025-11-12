CREATE PROCEDURE sp_GetOrdersByCustomerId
    @CustomerId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        o.OrderId,
        o.CustomerId,
        o.TotalAmount,
        o.ShippingAddress,
        o.Status,
        o.CreatedOn
    FROM Orders o
    WHERE o.CustomerId = @CustomerId AND o.IsDeleted = 0
    ORDER BY o.CreatedOn DESC;
END