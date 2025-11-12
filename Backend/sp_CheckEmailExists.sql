CREATE PROCEDURE sp_CheckEmailExists
    @Email NVARCHAR(150)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT CASE 
        WHEN EXISTS (SELECT 1 FROM Users WHERE Email = @Email) 
        THEN CAST(1 AS BIT) 
        ELSE CAST(0 AS BIT) 
    END AS EmailExists;
END