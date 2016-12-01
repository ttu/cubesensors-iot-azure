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
	
	-- Average from the last 24h
	SELECT @AvgVoc = AVG(Cast(Voc AS float)), @AvgLight = AVG(Cast(Light AS float)), @AvgNoise = AVG(Cast(Noise AS float)) FROM cubesensors_data
	WHERE MeasurementTime > DATEADD(minute, -(60 * 24 + 1), GETDATE()) AND SensorId = @SensorId
	
	INSERT INTO @latestData
	SELECT Noise, Light, Voc FROM cubesensors_data
	WHERE MeasurementTime > DATEADD(minute, -16, GETDATE()) AND SensorId = @SensorId
		
	-- Select top 3 data points from last 15 minutes and averages for those
	SELECT @CurrentNoise = AVG(Cast(Noise AS float))
	FROM 
	(
		SELECT TOP 3 Noise FROM @latestData
		ORDER BY Noise DESC
	) a
	
	SELECT @CurrentLight = AVG(Cast(Light AS float))
	FROM 
	(
		SELECT TOP 3 Light FROM @latestData
		ORDER BY Light DESC
	) b
	
	SELECT @CurrentVoc = AVG(Cast(Voc AS float))
	FROM 
	(
		SELECT TOP 3 Voc FROM @latestData
		ORDER BY Voc DESC
	) c
	
	-- TODO: Calculta return value based on probablity of having people on sensors proximity
	-- TODO Use also Voc (NOTE: Voc levels in the morning are below average)
	IF @CurrentNoise > @AvgNoise AND @CurrentLight > @AvgLight
		RETURN(100)
	
	RETURN(0)	
END