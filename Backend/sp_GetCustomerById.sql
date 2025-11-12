CREATE PROCEDURE sp_GetCustomerById
    @CustomerId INT
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
    WHERE c.CustomerId = @CustomerId 
        AND c.IsDeleted = 0
        AND u.IsDeleted = 0
END