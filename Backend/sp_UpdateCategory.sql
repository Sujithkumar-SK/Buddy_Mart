CREATE PROCEDURE sp_UpdateCategory
    @CategoryId INT,
    @Name NVARCHAR(100),
    @Description NVARCHAR(500) = NULL,
    @ImagePath NVARCHAR(500) = NULL,
    @ParentCategoryId INT = NULL,
    @IsActive BIT,
    @UpdatedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Categories 
    SET 
        Name = @Name,
        Description = @Description,
        ImagePath = @ImagePath,
        ParentCategoryId = @ParentCategoryId,
        IsActive = @IsActive,
        UpdatedBy = @UpdatedBy,
        UpdatedOn = GETUTCDATE()
    WHERE CategoryId = @CategoryId AND IsDeleted = 0;
    
    SELECT @@ROWCOUNT AS RowsAffected;
END