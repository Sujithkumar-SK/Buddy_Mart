CREATE PROCEDURE sp_GetAllCategories
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.CategoryId,
        c.Name,
        c.Description,
        c.ImagePath,
        c.IsActive,
        c.ParentCategoryId,
        pc.Name AS ParentCategoryName,
        c.CreatedOn
    FROM Categories c
    LEFT JOIN Categories pc ON c.ParentCategoryId = pc.CategoryId
    WHERE c.IsDeleted = 0
    ORDER BY c.CreatedOn DESC;
END