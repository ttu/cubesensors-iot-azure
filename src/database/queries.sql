-- All data from last 60min
SELECT * FROM cubesensors_data
WHERE SensorId = '000D6F0004476483' AND MeasurementTime > DATEADD(minute, -61, GETDATE())

-- Select Averages from last 60min
SELECT (AVG(Temperature) / 100.0) as Temp, AVG(Noise) as Noise FROM cubesensors_data
WHERE SensorId = '000D6F0004476483' AND MeasurementTime > DATEADD(minute, -61, GETDATE())

-- Add time to average
SELECT MAX(MeasurementTime) AS MeasurementTime, (AVG(Temperature) / 100.0) as Temp, AVG(Noise) as Noise FROM cubesensors_data
WHERE SensorId = '000D6F0004476483' AND MeasurementTime > DATEADD(minute, -61, GETDATE())
GROUP BY (DATEPART(MINUTE, MeasurementTime) % 1)

-- Grouped averages every 10min
SELECT MAX(MeasurementTime) AS MeasurementTime, (AVG(Temperature) / 100.0) as Temp, AVG(Noise) as Noise FROM cubesensors_data
WHERE SensorId = '000D6F0004476483' AND MeasurementTime > DATEADD(minute, -61, GETDATE())
GROUP BY (DATEPART(MINUTE, MeasurementTime) / 10)

-- Create heatmap
SELECT MAX(c1.MeasurementTime) AS M1, AVG(c1.Noise) AS 'Avotila', AVG(c2.Noise) AS 'Neukkari' FROM cubesensors_data AS c1
LEFT JOIN cubesensors_data AS c2 ON c2.MeasurementTime = c1.MeasurementTime
WHERE c1.SensorId = '000D6F0004476483'
AND c2.SensorId = '000D6F0003141E14'
AND c1.MeasurementTime > DATEADD(minute, -1441, GETDATE())
GROUP BY 
(DATEPART(HOUR, c1.MeasurementTime)),
(DATEPART(MINUTE, c1.MeasurementTime) / 30)
ORDER BY M1 ASC

-- Select last rows for each sensor (just check from last 10h beacause this is pretty slow)
SELECT d1.*
FROM cubesensors_data d1 LEFT JOIN cubesensors_data d2
ON (d1.SensorId = d2.SensorId AND d1.MeasurementTime < d2.MeasurementTime)
WHERE d2.MeasurementTime IS NULL
AND d1.MeasurementTime > DATEADD(hour, -10, GETDATE())