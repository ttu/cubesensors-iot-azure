-- Simple procedure that checks if there are people in the same room with the sensor
-- Return 100 when has people and 0 when does not
CREATE PROCEDURE dbo.get_has_people
   @SensorId AS NVARCHAR(MAX)
AS
BEGIN  
	DECLARE @CurrentNoise float
	DECLARE @CurrentLight float
	DECLARE @CurrentVoc float
	DECLARE @AvgNoise float
	DECLARE @AvgLight float
	DECLARE @AvgVoc float
	
	DECLARE @latestData TABLE(
		Noise int,
		Light int,
		Voc int
	)

	-- Try to get averages from saved data
	SELECT @AvgNoise = AvgNoise, @AvgLight = AvgLight, @AvgVoc = AvgVoc 
	FROM cubesensors_averages
	WHERE QueryDate = CONVERT(date, GETDATE())
	AND SensorId = @SensorId

	IF @AvgNoise IS NULL
	BEGIN
		-- Averages from the last 4 weeks for the same weekday
		SELECT @AvgVoc = AVG(Cast(Voc AS float)), @AvgLight = AVG(Cast(Light AS float)), @AvgNoise = AVG(Cast(Noise AS float))
		FROM cubesensors_data
		WHERE SensorId = @SensorId
		AND 
		(
			(
				MeasurementTime > DATEADD(day, DATEDIFF(day, 7, GETDATE()), 0) AND 
				MeasurementTime < DATEADD(day, DATEDIFF(day, 6, GETDATE()), 0)
			)
			OR (
				MeasurementTime > DATEADD(day, DATEDIFF(day, 14, GETDATE()), 0) AND 
				MeasurementTime < DATEADD(day, DATEDIFF(day, 13, GETDATE()), 0)
			)
			OR (
				MeasurementTime > DATEADD(day, DATEDIFF(day, 21, GETDATE()), 0) AND 
				MeasurementTime < DATEADD(day, DATEDIFF(day, 20, GETDATE()), 0)
			)
			OR (
				MeasurementTime > DATEADD(day, DATEDIFF(day, 28, GETDATE()), 0) AND 
				MeasurementTime < DATEADD(day, DATEDIFF(day, 27, GETDATE()), 0)
			)
		)

		INSERT INTO cubesensors_averages 
		VALUES (CONVERT(date, GETDATE()), @SensorId, @AvgNoise, @AvgLight, @AvgVoc) 
	END
	
	INSERT INTO @latestData
	SELECT Noise, Light, Voc FROM cubesensors_data
	WHERE MeasurementTime > DATEADD(minute, -16, GETDATE()) AND SensorId = @SensorId
		
	-- Select top 3 data points from last 15 minutes and averages for those
	-- Drop highest value as there might be some mistake with single value
	SELECT @CurrentNoise = AVG(Cast(Noise AS float))
	FROM 
	(
		SELECT Noise FROM @latestData
		ORDER BY Noise DESC
		OFFSET 1 ROWS
		FETCH NEXT 3 ROWS ONLY
	) a
	
	SELECT @CurrentLight = AVG(Cast(Light AS float))
	FROM 
	(
		SELECT Light FROM @latestData
		ORDER BY Light DESC
		OFFSET 1 ROWS
		FETCH NEXT 3 ROWS ONLY
	) b
	
	SELECT @CurrentVoc = AVG(Cast(Voc AS float))
	FROM 
	(
		SELECT Voc FROM @latestData
		ORDER BY Voc DESC
		OFFSET 1 ROWS
		FETCH NEXT 3 ROWS ONLY
	) c
	
	/*
	TODO: Find better way than to compare against lats 4 weeks averages
	TODO: Calculate return value based on probablity of having people on sensor's proximity
	TODO: Use also Voc (NOTE: Voc levels in the morning are below average)
	*/ 

	PRINT 'Current Noise: ' + CAST(@CurrentNoise AS VARCHAR)
	PRINT 'Average Noise: ' + CAST(@AvgNoise AS VARCHAR)
	PRINT 'Current Light: ' + CAST(@CurrentLight AS VARCHAR)
	PRINT 'Average Light: ' + CAST(@AvgLight AS VARCHAR)

	IF @CurrentNoise > @AvgNoise AND @CurrentLight > @AvgLight
		RETURN(100)
	
	RETURN(0)	
END