CREATE PROCEDURE sp_CheckSKUExists
    @SKU NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        CASE 
            WHEN EXISTS (
                SELECT 1 
                FROM Products 
                WHERE SKU = @SKU 
                AND IsDeleted = 0
            ) 
            THEN CAST(1 AS BIT)
            ELSE CAST(0 AS BIT)
        END AS SKUExists;
END