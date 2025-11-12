CREATE PROCEDURE sp_GetAllCustomers
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.CustomerId,
        c.FirstName,
        c.MiddleName,
        c.LastName,
        c.DateOfBirth,
        c.Gender,
        c.Address,
        c.City,
        c.State,
        c.PinCode,
        c.IsActive,
        c.UserId,
        u.Email,
        u.Phone
    FROM Customers c
    INNER JOIN Users u ON c.UserId = u.UserId
    WHERE c.IsDeleted = 0
        AND u.IsDeleted = 0
    ORDER BY c.CreatedOn DESC
END