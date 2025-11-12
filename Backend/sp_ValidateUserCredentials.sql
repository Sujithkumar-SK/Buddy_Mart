CREATE PROCEDURE sp_ValidateUserCredentials
    @Email NVARCHAR(150),
    @PasswordHash NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        DECLARE @IsValid BIT = 0;
        
        IF EXISTS (
            SELECT 1 
            FROM Users 
            WHERE Email = @Email 
                AND PasswordHash = @PasswordHash 
                AND IsDeleted = 0
                AND IsActive = 1
        )
        BEGIN
            SET @IsValid = 1;
        END
        
        SELECT @IsValid AS IsValid;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END