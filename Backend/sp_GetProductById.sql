CREATE PROCEDURE sp_GetProductById
    @ProductId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get product details with vendor and category information
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
        c.Name AS CategoryName
    FROM 
        Products p
        INNER JOIN Vendors v ON p.VendorId = v.VendorId
        INNER JOIN Categories c ON p.CategoryId = c.CategoryId
    WHERE 
        p.ProductId = @ProductId
        AND p.IsDeleted = 0
        AND v.IsDeleted = 0
        AND c.IsDeleted = 0;
    
    -- Get product images
    SELECT 
        ImagePath
    FROM 
        ProductImages
    WHERE 
        ProductId = @ProductId
        AND IsDeleted = 0
    ORDER BY 
        CreatedOn ASC;
END