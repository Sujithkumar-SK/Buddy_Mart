CREATE PROCEDURE sp_GetOrderById
    @OrderId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            o.OrderId,
            o.CustomerId,
            o.TotalAmount,
            o.ShippingAddress,
            o.Status,
            o.CreatedOn,
            c.FirstName + ' ' + c.LastName AS CustomerName,
            u.Phone AS CustomerPhone,
            u.Email AS CustomerEmail
        FROM Orders o
        INNER JOIN Customers c ON o.CustomerId = c.CustomerId
        INNER JOIN Users u ON c.UserId = u.UserId
        WHERE o.OrderId = @OrderId 
            AND o.IsDeleted = 0
            AND c.IsDeleted = 0
            AND u.IsDeleted = 0;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END