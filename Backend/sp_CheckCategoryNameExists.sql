CREATE PROCEDURE sp_CheckCategoryNameExists
    @Name NVARCHAR(100),
    @CategoryId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        CASE 
            WHEN EXISTS (
                SELECT 1 
                FROM Categories 
                WHERE Name = @Name 
                AND IsDeleted = 0 
                AND (@CategoryId IS NULL OR CategoryId != @CategoryId)
            ) 
            THEN CAST(1 AS BIT)
            ELSE CAST(0 AS BIT)
        END AS NameExists;
END