CREATE PROCEDURE sp_ValidateCategory
    @CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        CASE 
            WHEN EXISTS (
                SELECT 1 
                FROM Categories 
                WHERE CategoryId = @CategoryId 
                AND IsDeleted = 0 
                AND IsActive = 1
            ) 
            THEN CAST(1 AS BIT)
            ELSE CAST(0 AS BIT)
        END AS CategoryValid;
END