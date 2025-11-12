CREATE PROCEDURE sp_GetUserByRefreshToken
    @RefreshToken NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            u.UserId,
            u.Email,
            u.PasswordHash,
            u.Phone,
            u.Role,
            u.IsEmailVerified,
            u.IsActive,
            u.LastLogin,
            u.FailedLoginAttempts,
            u.LockoutEnd,
            u.TenantId,
            u.CreatedBy,
            u.CreatedOn,
            u.UpdatedBy,
            u.UpdatedOn
        FROM Users u
        INNER JOIN RefreshTokens rt ON u.UserId = rt.UserId
        WHERE rt.Token = @RefreshToken 
            AND rt.IsRevoked = 0 
            AND rt.ExpiresAt > GETUTCDATE()
            AND u.IsDeleted = 0
            AND rt.IsDeleted = 0
        ORDER BY rt.CreatedOn DESC;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END