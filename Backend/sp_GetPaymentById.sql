CREATE PROCEDURE sp_GetPaymentById
    @PaymentId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        p.PaymentId,
        p.OrderId,
        p.PaymentMethod,
        p.Status AS PaymentStatus,
        p.Amount,
        p.TransactionId,
        p.PaymentDate,
        p.CreatedOn
    FROM Payments p
    WHERE p.PaymentId = @PaymentId;
END