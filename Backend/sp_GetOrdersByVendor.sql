CREATE PROCEDURE sp_GetOrdersByVendor
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        o.OrderId,
        o.OrderNumber,
        o.TotalAmount,
        o.ShippingAddress,
        o.ShippingCity,
        o.ShippingState,
        o.ShippingPinCode,
        o.ShippingPhone,
        o.Status,
        o.CreatedOn as OrderDate,
        c.FirstName + ' ' + c.LastName as CustomerName,
        u.Phone as CustomerPhone,
        u.Email as CustomerEmail,
        (SELECT COUNT(*) FROM OrderItems oi WHERE oi.OrderId = o.OrderId) as ItemCount,
        s.ShippingId,
        s.TrackingNumber,
        s.CourierService,
        s.Status as ShippingStatus,
        s.ShippedDate,
        s.EstimatedDeliveryDate,
        s.ActualDeliveryDate,
        s.DeliveryNotes,
        s.CreatedOn as ShippingCreatedOn
    FROM Orders o
    INNER JOIN Customers c ON o.CustomerId = c.CustomerId
    INNER JOIN Users u ON c.UserId = u.UserId
    LEFT JOIN Shipping s ON o.OrderId = s.OrderId
    WHERE o.VendorId = @VendorId 
        AND o.IsDeleted = 0
    ORDER BY o.CreatedOn DESC;
    
    -- Get Order Items for all orders
    SELECT 
        oi.OrderItemId,
        oi.OrderId,
        oi.ProductId,
        oi.Quantity,
        oi.UnitPrice,
        oi.TotalPrice,
        p.Name as ProductName,
        p.SKU as ProductSKU,
        (SELECT TOP 1 ImagePath FROM ProductImages pi WHERE pi.ProductId = p.ProductId) as ProductImage
    FROM OrderItems oi
    INNER JOIN Products p ON oi.ProductId = p.ProductId
    INNER JOIN Orders o ON oi.OrderId = o.OrderId
    WHERE o.VendorId = @VendorId 
        AND o.IsDeleted = 0
    ORDER BY oi.OrderId, oi.OrderItemId;
END