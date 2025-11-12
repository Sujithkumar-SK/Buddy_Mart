CREATE PROCEDURE sp_GetCustomerRegistrationTrends
    @StartDate DATETIME,
    @EndDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Generate daily customer registration trends, ordered by date ascending for chart display
    WITH DateSeries AS (
        SELECT CAST(@StartDate AS DATE) AS Date
        UNION ALL
        SELECT DATEADD(DAY, 1, Date)
        FROM DateSeries
        WHERE Date < CAST(@EndDate AS DATE)
    ),
    DailyRegistrations AS (
        SELECT 
            CAST(c.CreatedOn AS DATE) AS Date,
            COUNT(c.CustomerId) AS NewRegistrations
        FROM Customers c
        WHERE c.CreatedOn >= @StartDate 
            AND c.CreatedOn <= @EndDate
        GROUP BY CAST(c.CreatedOn AS DATE)
    ),
    RunningTotals AS (
        SELECT 
            ds.Date,
            ISNULL(dr.NewRegistrations, 0) AS NewRegistrations,
            (SELECT COUNT(*) FROM Customers 
             WHERE CAST(CreatedOn AS DATE) <= ds.Date) AS TotalCustomers
        FROM DateSeries ds
        LEFT JOIN DailyRegistrations dr ON ds.Date = dr.Date
    )
    SELECT 
        Date,
        NewRegistrations,
        TotalCustomers
    FROM RunningTotals
    ORDER BY Date ASC -- Rule #9: Chronological order for chart display
    OPTION (MAXRECURSION 400); -- Allow up to 400 days
END