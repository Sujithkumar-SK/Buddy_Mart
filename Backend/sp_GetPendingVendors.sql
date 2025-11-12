CREATE PROCEDURE sp_GetPendingVendors
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        v.VendorId,
        v.BusinessName,
        v.OwnerName,
        v.BusinessLicenseNumber,
        v.BusinessAddress,
        v.City,
        v.State,
        v.PinCode,
        v.TaxRegistrationNumber,
        v.DocumentPath,
        u.Email,
        u.Phone,
        v.CreatedOn,
        v.Status,
        CASE v.Status
            WHEN 0 THEN 'PendingApproval'
            WHEN 1 THEN 'Active'
            WHEN 2 THEN 'Inactive'
            WHEN 3 THEN 'Suspended'
            WHEN 4 THEN 'Rejected'
        END as StatusText
    FROM Vendors v
    INNER JOIN Users u ON v.UserId = u.UserId
    WHERE v.Status = 0 -- PendingApproval
        AND v.IsDeleted = 0
        AND u.IsDeleted = 0
    ORDER BY v.CreatedOn DESC
END