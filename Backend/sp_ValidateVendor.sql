CREATE PROCEDURE sp_ValidateVendor
    @VendorId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        CASE 
            WHEN EXISTS (
                SELECT 1 
                FROM Vendors 
                WHERE VendorId = @VendorId 
                AND IsDeleted = 0 
                AND IsActive = 1
                AND Status = 1 -- Active status
            ) 
            THEN CAST(1 AS BIT)
            ELSE CAST(0 AS BIT)
        END AS VendorValid;
END