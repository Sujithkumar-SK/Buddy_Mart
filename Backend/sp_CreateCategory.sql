CREATE PROCEDURE sp_CreateCategory
    @Name NVARCHAR(100),
    @Description NVARCHAR(500) = NULL,
    @ImagePath NVARCHAR(500) = NULL,
    @ParentCategoryId INT = NULL,
    @CreatedBy NVARCHAR(100),
    @TenantId NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO Categories (Name, Description, ImagePath, ParentCategoryId, IsActive, CreatedBy, CreatedOn, TenantId)
    VALUES (@Name, @Description, @ImagePath, @ParentCategoryId, 1, @CreatedBy, GETUTCDATE(), @TenantId);
    
    SELECT SCOPE_IDENTITY() AS CategoryId;
END