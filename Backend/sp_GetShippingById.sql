CREATE PROCEDURE sp_GetShippingById
    @ShippingId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        ShippingId,
        OrderId,
        TrackingNumber,
        CourierService,
        Status,
        ShippedDate,
        EstimatedDeliveryDate,
        ActualDeliveryDate,
        DeliveryNotes,
        CreatedBy,
        CreatedOn,
        TenantId
    FROM Shipping
    WHERE ShippingId = @ShippingId 
        AND IsDeleted = 0;
END