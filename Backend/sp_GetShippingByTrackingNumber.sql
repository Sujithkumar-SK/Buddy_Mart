CREATE PROCEDURE sp_GetShippingByTrackingNumber
    @TrackingNumber NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        s.ShippingId,
        s.OrderId,
        s.TrackingNumber,
        s.CourierService,
        s.Status,
        s.ShippedDate,
        s.EstimatedDeliveryDate,
        s.ActualDeliveryDate,
        s.DeliveryNotes,
        s.CreatedBy,
        s.CreatedOn,
        s.TenantId
    FROM Shipping s
    WHERE s.TrackingNumber = @TrackingNumber
        AND s.IsDeleted = 0
    ORDER BY s.CreatedOn DESC;
END