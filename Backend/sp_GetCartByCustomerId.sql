CREATE PROCEDURE sp_GetCartByCustomerId
    @CustomerId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.CartId,
        c.ProductId,
        p.Name AS ProductName,
        p.SKU AS ProductSKU,
        p.Price,
        p.DiscountPrice,
        c.Quantity,
        CASE 
            WHEN p.DiscountPrice IS NOT NULL THEN p.DiscountPrice * c.Quantity
            ELSE p.Price * c.Quantity
        END AS TotalPrice,
        (SELECT TOP 1 ImagePath FROM ProductImages pi WHERE pi.ProductId = p.ProductId AND pi.IsDeleted = 0 AND pi.IsPrimary = 1) AS ProductImage,
        v.VendorId,
        v.BusinessName AS VendorName,
        p.StockQuantity,
        p.IsActive,
        c.CreatedOn AS AddedOn
    FROM 
        Cart c
        INNER JOIN Products p ON c.ProductId = p.ProductId
        INNER JOIN Vendors v ON p.VendorId = v.VendorId
    WHERE 
        c.CustomerId = @CustomerId
        AND c.IsDeleted = 0
        AND p.IsDeleted = 0
        AND v.IsDeleted = 0
    ORDER BY 
        c.CreatedOn DESC;
END