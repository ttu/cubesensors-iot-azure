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


## From nuget to Paket
http://fsprojects.github.io/Paket/getting-started.html

https://fsprojects.github.io/Paket/paket-convert-from-nuget.html

```sh
# get paket.exe
.paket\paket.bootstrapper.exe
# install dependencies
.paket\paket.exe install
```

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
# Get all data for sensor from last 60 min
http 127.0.0.1:8083/api/v1/sensor/000D4F0004E764C3/60
# Get all value points (time, value) from X min
http 127.0.0.1:8083/api/v1/temperature/000D4F0004E764C3/1440
http 127.0.0.1:8083/api/v1/noise/000D4F0004E764C3/30
# Get average temperature from last day
http 127.0.0.1:8083/api/v1/temperature/avg/000D4F0004E764C3/1440
# Get last update time
http 127.0.0.1:8083/api/v1/last -a foo:bar
```
