CREATE PROCEDURE sp_GetShippingsByVendor
    @VendorId INT
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
    INNER JOIN Orders o ON s.OrderId = o.OrderId
    WHERE o.VendorId = @VendorId
        AND s.IsDeleted = 0
        AND o.IsDeleted = 0
    ORDER BY s.CreatedOn DESC;
END