CREATE PROCEDURE sp_GetRecentActivities
    @Limit INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get recent activities from multiple sources, ordered by most recent first (Rule #9)
    SELECT TOP (@Limit)
        ActivityType,
        Description,
        CreatedOn,
        UserName
    FROM (
        -- New Orders
        SELECT 
            'Order' AS ActivityType,
            'New order #' + o.OrderNumber + ' placed by ' + c.FirstName + ' ' + c.LastName AS Description,
            o.CreatedOn,
            c.FirstName + ' ' + c.LastName AS UserName
        FROM Orders o
        INNER JOIN Customers c ON o.CustomerId = c.CustomerId
        WHERE o.CreatedOn >= DATEADD(DAY, -7, GETDATE())
        
        UNION ALL
        
        -- New Customer Registrations
        SELECT 
            'Registration' AS ActivityType,
            'New customer registered: ' + c.FirstName + ' ' + c.LastName AS Description,
            c.CreatedOn,
            c.FirstName + ' ' + c.LastName AS UserName
        FROM Customers c
        WHERE c.CreatedOn >= DATEADD(DAY, -7, GETDATE())
        
        UNION ALL
        
        -- Vendor Applications
        SELECT 
            'Vendor Application' AS ActivityType,
            'New vendor application: ' + v.BusinessName AS Description,
            v.CreatedOn,
            v.OwnerName AS UserName
        FROM Vendors v
        WHERE v.CreatedOn >= DATEADD(DAY, -7, GETDATE())
        
        UNION ALL
        
        -- Product Additions
        SELECT 
            'Product' AS ActivityType,
            'New product added: ' + p.Name + ' by ' + v.BusinessName AS Description,
            p.CreatedOn,
            v.OwnerName AS UserName
        FROM Products p
        INNER JOIN Vendors v ON p.VendorId = v.VendorId
        WHERE p.CreatedOn >= DATEADD(DAY, -7, GETDATE())
        
        UNION ALL
        
        -- Successful Payments
        SELECT 
            'Payment' AS ActivityType,
            'Payment completed for order #' + o.OrderNumber + ' - ₹' + CAST(p.Amount AS VARCHAR(20)) AS Description,
            p.PaymentDate,
            c.FirstName + ' ' + c.LastName AS UserName
        FROM Payments p
        INNER JOIN Orders o ON p.OrderId = o.OrderId
        INNER JOIN Customers c ON o.CustomerId = c.CustomerId
        WHERE p.Status = 2 AND p.PaymentDate >= DATEADD(DAY, -7, GETDATE())
    ) AS Activities
    ORDER BY CreatedOn DESC; -- Rule #9: Recently added records should be top
END