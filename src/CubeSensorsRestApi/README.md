## Config.fs

Config.fs is not in version control. Add it manually.

``` csharp
module Config

let LocalDb = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=C:\SRC\GITHUB\CUBESENSORS-IOT-AZURE\SAMPLE_DATA\CUBE_DB.MDF;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
let Azure = @""

let Users = [("foo", "bar"); ("bill", "temple")]
```

## SQL type provider
https://msdn.microsoft.com/en-us/library/hh361033.aspx

## Example requests

Use HTTPie

```sh
pip install httpie
```

```sh
# List all sensors
http 127.0.0.1:8083/api/v1/sensor
# Get all data for sensor
http 127.0.0.1:8083/api/v1/sensor/000D4F0004E764C3
# Get all data for sensor
http 127.0.0.1:8083/api/v1/sensor/000D4F0004E764C3/100000
# Get all temperature points (time, temperature)
http 127.0.0.1:8083/api/v1/temperature/000D4F0004E764C3/100000
# Get average temperature
http 127.0.0.1:8083/api/v1/temperature/avg/000D4F0004E764C3/100000
# Get last update time
http 127.0.0.1:8083/api/v1/last -a foo:bar
```