CREATE PROCEDURE sp_CheckCartItemExists
    @CustomerId INT,
    @ProductId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        CASE 
            WHEN EXISTS (
                SELECT 1 FROM Cart 
                WHERE CustomerId = @CustomerId 
                AND ProductId = @ProductId 
                AND IsDeleted = 0
            ) 
            THEN CAST(1 AS BIT)
            ELSE CAST(0 AS BIT)
        END AS ItemExists;
END