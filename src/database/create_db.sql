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
GO

USE cube_db;
CREATE USER cube_producer WITH PASSWORD = 'create new password';
CREATE USER cube_consumer WITH PASSWORD = 'create new password';
CREATE USER cube_admin WITH PASSWORD = 'create new password';

EXEC sp_addrolemember N'db_datareader', N'cube_producer';
EXEC sp_addrolemember N'db_datawriter', N'cube_producer';
EXEC sp_addrolemember N'db_datareader', N'cube_consumer';
EXEC sp_addrolemember N'db_owner', N'cube_admin';
GO

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