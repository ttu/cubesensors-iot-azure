## ConnectionStrings.fs

ConnectionStrings.fs is not in version control. Add it manually.

```fs
module ConnectionStrings

let LocalDb = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=C:\SRC\GITHUB\CUBESENSORS-IOT-AZURE\SAMPLE_DATA\CUBE_DB.MDF;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
let Azure = @""
```

## SQL type provider
https://msdn.microsoft.com/en-us/library/hh361033.aspx