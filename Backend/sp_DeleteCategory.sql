CREATE PROCEDURE sp_DeleteCategory
    @CategoryId INT,
    @DeletedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if category has products
    IF EXISTS (SELECT 1 FROM Products WHERE CategoryId = @CategoryId AND IsDeleted = 0)
    BEGIN
        SELECT 0 AS CanDelete, 'Category has products and cannot be deleted' AS Message;
        RETURN;
    END
    
    -- Check if category has subcategories
    IF EXISTS (SELECT 1 FROM Categories WHERE ParentCategoryId = @CategoryId AND IsDeleted = 0)
    BEGIN
        SELECT 0 AS CanDelete, 'Category has subcategories and cannot be deleted' AS Message;
        RETURN;
    END
    
    -- Soft delete the category
    UPDATE Categories 
    SET 
        IsDeleted = 1,
        DeletedBy = @DeletedBy,
        DeletedOn = GETUTCDATE()
    WHERE CategoryId = @CategoryId AND IsDeleted = 0;
    
    SELECT 1 AS CanDelete, 'Category deleted successfully' AS Message;
END