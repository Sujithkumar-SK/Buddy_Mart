CREATE PROCEDURE sp_CheckStockAvailability
    @ProductId INT,
    @RequiredQuantity INT,
    @IsAvailable BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @CurrentStock INT;
    
    -- Get current stock quantity
    SELECT @CurrentStock = StockQuantity 
    FROM Products 
    WHERE ProductId = @ProductId 
      AND IsDeleted = 0 
      AND IsActive = 1
      AND Status IN (1, 2); -- Active or OutOfStock status
    
    -- Check if we have enough stock
    IF @CurrentStock IS NULL
        SET @IsAvailable = 0; -- Product not found
    ELSE IF @CurrentStock >= @RequiredQuantity
        SET @IsAvailable = 1; -- Stock available
    ELSE
        SET @IsAvailable = 0; -- Insufficient stock
END