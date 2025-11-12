CREATE PROCEDURE sp_CheckPhoneExists
    @Phone NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT CASE 
        WHEN EXISTS (SELECT 1 FROM Users WHERE Phone = @Phone) 
        THEN CAST(1 AS BIT) 
        ELSE CAST(0 AS BIT) 
    END AS PhoneExists;
END