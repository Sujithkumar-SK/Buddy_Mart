CREATE PROCEDURE sp_GetCategoryById
    @CategoryId INT
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
    WHERE c.CategoryId = @CategoryId 
    AND c.IsDeleted = 0;
END