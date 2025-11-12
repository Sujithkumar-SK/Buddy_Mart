CREATE PROCEDURE sp_CheckBusinessLicenseExists
    @BusinessLicenseNumber NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT CASE 
        WHEN EXISTS (SELECT 1 FROM Vendors WHERE BusinessLicenseNumber = @BusinessLicenseNumber) 
        THEN CAST(1 AS BIT) 
        ELSE CAST(0 AS BIT) 
    END AS BusinessLicenseExists;
END