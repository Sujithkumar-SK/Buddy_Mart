CREATE PROCEDURE sp_GetPaymentByRazorpayOrderId
    @RazorpayOrderId NVARCHAR(100)
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
    WHERE p.TransactionId = @RazorpayOrderId;
END