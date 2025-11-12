CREATE PROCEDURE sp_GetPaymentsByOrderId
    @OrderId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            p.PaymentId,
            p.OrderId,
            p.PaymentMethod,
            p.PaymentStatus,
            p.Amount,
            p.TransactionId,
            p.PaymentGateway,
            p.PaymentDate,
            p.CreatedOn,
            o.CustomerId,
            c.FirstName + ' ' + c.LastName AS CustomerName
        FROM Payments p
        INNER JOIN Orders o ON p.OrderId = o.OrderId
        INNER JOIN Customers c ON o.CustomerId = c.CustomerId
        WHERE p.OrderId = @OrderId 
            AND p.IsDeleted = 0
            AND o.IsDeleted = 0
            AND c.IsDeleted = 0
        ORDER BY p.CreatedOn DESC;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END