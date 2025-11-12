CREATE PROCEDURE sp_GetSubscriptionPlans
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        PlanId,
        PlanName,
        Description,
        Price,
        MaxProducts,
        DurationDays
    FROM SubscriptionPlans
    WHERE IsActive = 1 
        AND IsDeleted = 0
    ORDER BY Price ASC
END