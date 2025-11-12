CREATE PROCEDURE sp_CheckStockAvailability
    @ProductId INT,
    @RequiredQuantity INT,
    @IsAvailable BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @CurrentStock INT;
    
    SELECT @CurrentStock = StockQuantity
    FROM Products
    WHERE ProductId = @ProductId
        AND IsDeleted = 0
        AND Status != 0; -- Not Draft status
    
    IF @CurrentStock IS NULL
    BEGIN
        SET @IsAvailable = 0;
        RETURN;
    END
    
    IF @CurrentStock >= @RequiredQuantity
        SET @IsAvailable = 1;
    ELSE
        SET @IsAvailable = 0;
END