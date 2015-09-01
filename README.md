# CubeSensors IoT Azure

* Python parser
  * Gets data from CubesensorsAPI, parses data to selected format and sends it to Event Hub.
* Azure
  * Stream Analytics fetches data from Event Hub and sends it to SQL Server.


## Azure Deployment

0. Create ServiceBus
  1. Scale -> Basic
  1. Create Event Hub
    1. Configure -> Shared access policies
      * cube_sender (Permissions: send)
      * cube_receiver (Permissions: listen)
      * Update sender private key to keys.py
0. Create SQL Database
  1. Create database
0. Create Stream Analytics
  1. Add input
    * Event Hub Policy Name: cube_receiver
  1. Add output
  1. Add query
0. Create WebApp
  1. Scale -> Basic
  1. Configuration -> Always on
  1. Add Python script (data_parser) as a new Web Job
    1. Add as a zip file. Pack whole module including env folder.

### JSON

Sample files in sample_data folder.

### Stream Analytics Query

Input: cubesensors-event-hub

Output: cubesensors-sql-data (SQL), cubesensors-alert-blob (BLOB)

```sql
WITH data_query AS (
  SELECT
      id AS sensorid,
      [time] AS measurementtime,
      temp AS temperature,
      pressure,
      humidity,
      voc,
      light,
      noisedba AS noise,
      battery,
      cable,
      voc_resistance AS vocresistance,
      rssi
  FROM
      [cubesensors-event-hub]
)

-- Addl all data to database
SELECT * INTO [cubesensors-sql-data] FROM data_query

-- Add alert data when battery is running low and cube is not plugged in
SELECT sensorid, measurementtime, battery INTO [cubesensors-alert-blob]
FROM data_query
WHERE cable = false AND battery < 10
```

### Database

Prepare database (some of these are required by localdb).

```sql
sp_configure 'show advanced options',1
GO
RECONFIGURE WITH OVERRIDE
GO
sp_configure 'contained database authentication', 1
GO
RECONFIGURE WITH OVERRIDE
GO

CREATE DATABASE cube_db
CONTAINMENT = PARTIAL;

USE cube_db;
CREATE USER cube_producer WITH PASSWORD = 'create new password';
CREATE USER cube_consumer WITH PASSWORD = 'create new password';
CREATE USER cube_admin WITH PASSWORD = 'create new password';

EXEC sp_addrolemember N'db_datareader', N'cube_producer';
EXEC sp_addrolemember N'db_datawriter', N'cube_producer';
EXEC sp_addrolemember N'db_datareader', N'cube_consumer';
EXEC sp_addrolemember N'db_owner', N'cube_admin';
```

##### Table

```sql
CREATE TABLE dbo.cubesensors_data (
  SensorId NVARCHAR(20) NOT NULL,
  MeasurementTime DATETIME2 NOT NULL,
  Temperature INT,
  Pressure INT,
  Humidity INT,
  Voc INT,
  Light INT,
  Noise INT,
  Battery INT,
  Cable INT,
  VocResistance INT,
  Rssi INT,
  PRIMARY KEY (SensorId, MeasurementTime)
);
```
