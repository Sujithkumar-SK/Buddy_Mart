CREATE PROCEDURE sp_GetProductsByVendor
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        p.ProductId,
        p.Name,
        p.Description,
        p.SKU,
        p.Price,
        p.DiscountPrice,
        p.StockQuantity,
        p.MinStockLevel,
        p.Brand,
        p.Weight,
        p.Dimensions,
        p.Status,
        p.IsActive,
        p.CreatedOn,
        p.UpdatedOn,
        p.CreatedBy,
        p.UpdatedBy,
        p.VendorId,
        v.BusinessName AS VendorName,
        p.CategoryId,
        c.Name AS CategoryName,
        (SELECT TOP 1 ImagePath FROM ProductImages pi WHERE pi.ProductId = p.ProductId AND pi.IsDeleted = 0 ORDER BY pi.CreatedOn ASC) AS PrimaryImagePath
    FROM 
        Products p
        INNER JOIN Vendors v ON p.VendorId = v.VendorId
        INNER JOIN Categories c ON p.CategoryId = c.CategoryId
    WHERE 
        p.VendorId = @VendorId
        AND p.IsDeleted = 0
        AND v.IsDeleted = 0
        AND c.IsDeleted = 0
    ORDER BY 
        p.CreatedOn DESC;
END