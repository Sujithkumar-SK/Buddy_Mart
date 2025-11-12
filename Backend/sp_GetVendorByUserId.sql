CREATE PROCEDURE sp_GetVendorByUserId
    @UserId INT
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
        CASE v.Status
            WHEN 0 THEN 'PendingApproval'
            WHEN 1 THEN 'Active'
            WHEN 2 THEN 'Inactive'
            WHEN 3 THEN 'Suspended'
            WHEN 4 THEN 'Rejected'
        END as Status,
        CASE v.CurrentPlan
            WHEN 1 THEN 'Basic'
            WHEN 2 THEN 'Standard'
            WHEN 3 THEN 'Premium'
        END as CurrentPlan
    FROM Vendors v
    WHERE v.UserId = @UserId 
        AND v.IsDeleted = 0
END