CREATE PROCEDURE sp_GetPaymentsByCustomerId
    @CustomerId INT
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
            o.TotalAmount AS OrderAmount
        FROM Payments p
        INNER JOIN Orders o ON p.OrderId = o.OrderId
        WHERE o.CustomerId = @CustomerId 
            AND p.IsDeleted = 0
            AND o.IsDeleted = 0
        ORDER BY p.CreatedOn DESC;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END