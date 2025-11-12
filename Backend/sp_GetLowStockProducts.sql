CREATE PROCEDURE sp_GetLowStockProducts
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        p.ProductId,
        p.Name,
        p.SKU,
        p.StockQuantity,
        p.MinStockLevel,
        p.Status,
        p.VendorId,
        p.CategoryId,
        p.CreatedBy,
        p.CreatedOn,
        p.TenantId
    FROM Products p
    WHERE p.VendorId = @VendorId
        AND p.IsDeleted = 0
        AND p.MinStockLevel IS NOT NULL
        AND p.StockQuantity <= p.MinStockLevel
    ORDER BY p.CreatedOn DESC;
END