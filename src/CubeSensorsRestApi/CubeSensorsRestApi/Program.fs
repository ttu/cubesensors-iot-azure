open Suave                 
open Suave.Http
open Suave.Http.Applicatives
open Suave.Http.Successful
open Suave.Web
open Newtonsoft.Json
open Newtonsoft.Json.Serialization

open Db

let JSON content =
  let jsonSerializerSettings = new JsonSerializerSettings()
  jsonSerializerSettings.ContractResolver <- new CamelCasePropertyNamesContractResolver()
  JsonConvert.SerializeObject(content, jsonSerializerSettings)
  |> OK
  >>= Writers.setMimeType "application/json; charset=utf-8"

let app =
  choose
    [ GET >>= choose
        [ path "/api/v1" >>= OK "Hello World!"
          path "/api/v1/goodbye" >>= OK "Good bye GET"
          path "/api/v1/sensor" >>= JSON (Db.GetSensorIds())
          pathScan "/api/v1/sensor/%s" (fun (id) -> JSON (Db.AllSensorData(id)))
          pathScan "/api/v1/sensor/%s/%d" (fun (id, min) -> JSON (Db.AllDataFromDuration(id, min)))
          pathScan "/api/v1/temperature/avg/%s/%d" (fun (id, min) -> JSON (Db.AvgTemperature(id, min)))
          pathScan "/api/v1/temperature/%s/%d" (fun (id, min) -> JSON (Db.TemperatureValuesFromDuration(id, min)))
        ]
      POST >>= choose
        [ path "/api/v1/hello" >>= OK "Hello POST"
          path "/api/v1/goodbye" >>= OK "Good bye POST" ] ]

startWebServer defaultConfig app
    