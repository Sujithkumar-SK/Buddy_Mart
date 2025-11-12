CREATE PROCEDURE sp_GetUserById
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            UserId,
            Email,
            PasswordHash,
            Phone,
            Role,
            IsEmailVerified,
            IsActive,
            LastLogin,
            FailedLoginAttempts,
            LockoutEnd,
            TenantId,
            CreatedBy,
            CreatedOn,
            UpdatedBy,
            UpdatedOn
        FROM Users 
        WHERE UserId = @UserId 
            AND IsDeleted = 0
        ORDER BY CreatedOn DESC;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END