-- =============================================
-- ALL STORED PROCEDURES FOR ECOMMERCE DATABASE
-- Execute this script on your SQL Server container
-- =============================================

USE Vendor_Ecommerce;
GO

-- Drop existing procedures if they exist
IF OBJECT_ID('sp_CheckBusinessLicenseExists', 'P') IS NOT NULL DROP PROCEDURE sp_CheckBusinessLicenseExists;
IF OBJECT_ID('sp_CheckCartItemExists', 'P') IS NOT NULL DROP PROCEDURE sp_CheckCartItemExists;
IF OBJECT_ID('sp_CheckCategoryNameExists', 'P') IS NOT NULL DROP PROCEDURE sp_CheckCategoryNameExists;
IF OBJECT_ID('sp_CheckEmailExists', 'P') IS NOT NULL DROP PROCEDURE sp_CheckEmailExists;
IF OBJECT_ID('sp_CheckPhoneExists', 'P') IS NOT NULL DROP PROCEDURE sp_CheckPhoneExists;
IF OBJECT_ID('sp_CheckSKUExists', 'P') IS NOT NULL DROP PROCEDURE sp_CheckSKUExists;
IF OBJECT_ID('sp_CheckStockAvailability', 'P') IS NOT NULL DROP PROCEDURE sp_CheckStockAvailability;
IF OBJECT_ID('sp_CreateCategory', 'P') IS NOT NULL DROP PROCEDURE sp_CreateCategory;
IF OBJECT_ID('sp_DeleteCategory', 'P') IS NOT NULL DROP PROCEDURE sp_DeleteCategory;
IF OBJECT_ID('sp_GetAllCategories', 'P') IS NOT NULL DROP PROCEDURE sp_GetAllCategories;
IF OBJECT_ID('sp_GetAllCustomers', 'P') IS NOT NULL DROP PROCEDURE sp_GetAllCustomers;
IF OBJECT_ID('sp_GetAllProducts', 'P') IS NOT NULL DROP PROCEDURE sp_GetAllProducts;
IF OBJECT_ID('sp_GetCartByCustomerId', 'P') IS NOT NULL DROP PROCEDURE sp_GetCartByCustomerId;
IF OBJECT_ID('sp_GetCategoryById', 'P') IS NOT NULL DROP PROCEDURE sp_GetCategoryById;
IF OBJECT_ID('sp_GetUserByEmail', 'P') IS NOT NULL DROP PROCEDURE sp_GetUserByEmail;
IF OBJECT_ID('sp_ValidateUserCredentials', 'P') IS NOT NULL DROP PROCEDURE sp_ValidateUserCredentials;
GO

-- Create all stored procedures

-- =============================================
-- Authentication & User Management
-- =============================================

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
GO

CREATE PROCEDURE sp_GetUserByEmail
    @Email NVARCHAR(150)
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
        WHERE Email = @Email 
            AND IsDeleted = 0
        ORDER BY CreatedOn DESC;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END
GO

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
GO