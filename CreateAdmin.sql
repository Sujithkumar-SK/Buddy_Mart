-- Create Admin User with hashed password for "Admin123"
INSERT INTO Users (Email, PasswordHash, Phone, Role, IsActive, IsEmailVerified, FailedLoginAttempts, IsDeleted, CreatedBy, CreatedOn, TenantId)
VALUES (
    'sujithkumar.kanini@outlook.com',
    'QNHhzjkUTVY7ZvRzqx3m5w==', -- This is SHA256 hash of "Admin123"
    '9385562091',
    3, -- Admin role
    1, -- IsActive
    1, -- IsEmailVerified
    0, -- FailedLoginAttempts
    0, -- IsDeleted
    'System',
    GETUTCDATE(),
    'admin'
);

-- Alternative: If user already exists, just update the password
UPDATE Users 
SET PasswordHash = 'QNHhzjkUTVY7ZvRzqx3m5w==', -- SHA256 hash of "Admin123"
    IsActive = 1,
    IsEmailVerified = 1
WHERE Email = 'sujithkumar.kanini@outlook.com';