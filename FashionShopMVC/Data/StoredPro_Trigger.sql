--use FashionShop;


-- PHAN STORED PROCEDURED
USE [FashionShop]
GO
/****** Object:  StoredProcedure [dbo].[GetRevenueStatistic]    Script Date: 11/11/2024 3:01:56 CH ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[GetRevenueStatistic]
                @fromDate DATE,
                @toDate DATE,
				@period varchar(10)
            AS
			SET nocount ON;
			IF @period = 'day'
            BEGIN
                SELECT
                    o.OrderDate AS Date,
                    SUM(od.Quantity * od.Price) AS Revenues,
                    SUM((od.Quantity * od.Price) - (od.Quantity * p.PurchasePrice)) AS Benefit
                FROM Orders o
                INNER JOIN OrderDetails od ON o.ID = od.OrderId
                INNER JOIN Products p ON od.ProductID = p.ID
                WHERE o.OrderDate <= @toDate AND o.OrderDate >= @fromDate
                GROUP BY o.OrderDate;
            END
			ELSE IF @period = 'month'
			BEGIN
				-- Thống kê theo tháng
				SELECT
					DATEFROMPARTS(YEAR(o.OrderDate), MONTH(o.OrderDate), 1) AS Date,
					SUM(od.Quantity * od.Price) AS Revenues,
					SUM((od.Quantity * od.Price) - (od.Quantity * p.PurchasePrice)) AS Benefit
				FROM Orders o
				INNER JOIN OrderDetails od ON o.ID = od.OrderId
				INNER JOIN Products p ON od.ProductID = p.ID
				WHERE o.OrderDate BETWEEN @fromDate AND @toDate
				GROUP BY YEAR(o.OrderDate), MONTH(o.OrderDate);
			END
			ELSE IF @period = 'quarter'
			BEGIN
				-- Thống kê theo quý
				SELECT
					CONCAT(YEAR(o.OrderDate), '-Q', DATEPART(QUARTER, o.OrderDate)) AS Date,
					SUM(od.Quantity * od.Price) AS Revenues,
					SUM((od.Quantity * od.Price) - (od.Quantity * p.PurchasePrice)) AS Benefit
				FROM Orders o
				INNER JOIN OrderDetails od ON o.ID = od.OrderId
				INNER JOIN Products p ON od.ProductID = p.ID
				WHERE o.OrderDate BETWEEN @fromDate AND @toDate
				GROUP BY YEAR(o.OrderDate), DATEPART(QUARTER, o.OrderDate);
			END
			ELSE IF @period = 'year'
			BEGIN
				-- Thống kê theo năm
				SELECT
					YEAR(o.OrderDate) AS Date,
					SUM(od.Quantity * od.Price) AS Revenues,
					SUM((od.Quantity * od.Price) - (od.Quantity * p.PurchasePrice)) AS Benefit
				FROM Orders o
				INNER JOIN OrderDetails od ON o.ID = od.OrderId
				INNER JOIN Products p ON od.ProductID = p.ID
				WHERE o.OrderDate BETWEEN @fromDate AND @toDate
				GROUP BY YEAR(o.OrderDate);
			END
			ELSE
			BEGIN
				-- Nếu @period không hợp lệ, trả về thông báo lỗi
				RAISERROR('Invalid period specified. Valid options are day, month, quarter, or year.', 16, 1);
			END
					


--PHAN TRIGGER
/*
GO
CREATE TRIGGER trg_AddRevenueOnOrderComplete
ON Orders
AFTER INSERT, UPDATE
AS
BEGIN
    INSERT INTO Revenue (Date, OrderID, CustomerID, Revenues, Cost)
    SELECT 
        GETDATE(),                -- Ngày hiện tại
        inserted.OrderID,         -- Mã đơn hàng
        inserted.CustomerID,      -- Mã khách hàng
        inserted.TotalAmount,     -- Doanh thu từ tổng giá trị đơn hàng
        inserted.CostAmount       -- Chi phí của đơn hàng
    FROM 
        inserted
    WHERE 
        inserted.Status = 4; -- Chỉ thêm nếu trạng thái là 'Hoàn thành'
END;

GO
CREATE TRIGGER trg_UpdateAggregatedRevenue
ON Revenue
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    -- Tổng hợp dữ liệu theo ngày
    MERGE AggregatedRevenue AS target
    USING (
        SELECT
            'day' AS AggregationType,
            Date AS PeriodStartDate,
            Date AS PeriodEndDate,
            SUM(Revenues) AS TotalRevenues,
            SUM(Benefit) AS TotalBenefit
        FROM Revenue
        GROUP BY Date
    ) AS source
    ON target.AggregationType = source.AggregationType 
       AND target.PeriodStartDate = source.PeriodStartDate
    WHEN MATCHED THEN
        UPDATE SET
            target.TotalRevenues = source.TotalRevenues,
            target.TotalBenefit = source.TotalBenefit
    WHEN NOT MATCHED THEN
        INSERT (AggregationType, PeriodStartDate, PeriodEndDate, TotalRevenues, TotalBenefit)
        VALUES (source.AggregationType, source.PeriodStartDate, source.PeriodEndDate, source.TotalRevenues, source.TotalBenefit);

    -- Tổng hợp dữ liệu theo tháng
    MERGE AggregatedRevenue AS target
    USING (
        SELECT
            'month' AS AggregationType,
            DATEFROMPARTS(YEAR(Date), MONTH(Date), 1) AS PeriodStartDate,
            EOMONTH(Date) AS PeriodEndDate,
            SUM(Revenues) AS TotalRevenues,
            SUM(Benefit) AS TotalBenefit
        FROM Revenue
        GROUP BY YEAR(Date), MONTH(Date)
    ) AS source
    ON target.AggregationType = source.AggregationType 
       AND target.PeriodStartDate = source.PeriodStartDate
    WHEN MATCHED THEN
        UPDATE SET
            target.TotalRevenues = source.TotalRevenues,
            target.TotalBenefit = source.TotalBenefit
    WHEN NOT MATCHED THEN
        INSERT (AggregationType, PeriodStartDate, PeriodEndDate, TotalRevenues, TotalBenefit)
        VALUES (source.AggregationType, source.PeriodStartDate, source.PeriodEndDate, source.TotalRevenues, source.TotalBenefit);

    -- Tổng hợp dữ liệu theo quý
    MERGE AggregatedRevenue AS target
    USING (
        SELECT
            'quarter' AS AggregationType,
            DATEFROMPARTS(YEAR(Date), ((MONTH(Date) - 1) / 3) * 3 + 1, 1) AS PeriodStartDate,
            DATEADD(DAY, -1, DATEFROMPARTS(YEAR(Date), ((MONTH(Date) - 1) / 3) * 3 + 4, 1)) AS PeriodEndDate,
            SUM(Revenues) AS TotalRevenues,
            SUM(Benefit) AS TotalBenefit
        FROM Revenue
        GROUP BY YEAR(Date), (MONTH(Date) - 1) / 3
    ) AS source
    ON target.AggregationType = source.AggregationType 
       AND target.PeriodStartDate = source.PeriodStartDate
    WHEN MATCHED THEN
        UPDATE SET
            target.TotalRevenues = source.TotalRevenues,
            target.TotalBenefit = source.TotalBenefit
    WHEN NOT MATCHED THEN
        INSERT (AggregationType, PeriodStartDate, PeriodEndDate, TotalRevenues, TotalBenefit)
        VALUES (source.AggregationType, source.PeriodStartDate, source.PeriodEndDate, source.TotalRevenues, source.TotalBenefit);

    -- Tổng hợp dữ liệu theo năm
    MERGE AggregatedRevenue AS target
    USING (
        SELECT
            'year' AS AggregationType,
            DATEFROMPARTS(YEAR(Date), 1, 1) AS PeriodStartDate,
            DATEFROMPARTS(YEAR(Date), 12, 31) AS PeriodEndDate,
            SUM(Revenues) AS TotalRevenues,
            SUM(Benefit) AS TotalBenefit
        FROM Revenue
        GROUP BY YEAR(Date)
    ) AS source
    ON target.AggregationType = source.AggregationType 
       AND target.PeriodStartDate = source.PeriodStartDate
    WHEN MATCHED THEN
        UPDATE SET
            target.TotalRevenues = source.TotalRevenues,
            target.TotalBenefit = source.TotalBenefit
    WHEN NOT MATCHED THEN
        INSERT (AggregationType, PeriodStartDate, PeriodEndDate, TotalRevenues, TotalBenefit)
        VALUES (source.AggregationType, source.PeriodStartDate, source.PeriodEndDate, source.TotalRevenues, source.TotalBenefit);

END;
*/