CREATE PROCEDURE sp_GetOrderItems
    @OrderId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            oi.OrderItemId,
            oi.OrderId,
            oi.ProductId,
            oi.Quantity,
            oi.UnitPrice,
            oi.TotalPrice,
            oi.CreatedOn,
            p.Name AS ProductName,
            p.SKU AS ProductSKU,
            p.Brand,
            v.BusinessName AS VendorName,
            (SELECT TOP 1 ImagePath FROM ProductImages pi WHERE pi.ProductId = p.ProductId AND pi.IsDeleted = 0 AND pi.IsPrimary = 1) AS ProductImage
        FROM OrderItems oi
        INNER JOIN Products p ON oi.ProductId = p.ProductId
        INNER JOIN Vendors v ON p.VendorId = v.VendorId
        WHERE oi.OrderId = @OrderId 
            AND oi.IsDeleted = 0
            AND p.IsDeleted = 0
            AND v.IsDeleted = 0
        ORDER BY oi.CreatedOn DESC;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END