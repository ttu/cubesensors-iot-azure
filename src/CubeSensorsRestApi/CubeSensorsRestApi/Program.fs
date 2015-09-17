open Suave                 
open Suave.Http
open Suave.Http.Applicatives
open Suave.Http.Successful
open Suave.Utils
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

let basicAuth =
  Authentication.authenticateBasic (fun (x) -> Seq.exists (fun y -> y = x) Config.Users)
  
let app =
  choose
    [ GET >>= choose
        [ 
          path "/" >>= OK "Hello!"
          path "/login" >>= OK "This should have some login info"
          path "/goodbye" >>= OK "Good bye!"
          path "/api/v1/sensor" >>= JSON (Db.GetSensorIds())
          path "/api/v1/sensor/status" >>= JSON (Db.GetSensorStatus(Db.getCurrentTime()))
          pathScan "/api/v1/sensor/%s" (fun (id) -> JSON (Db.AllSensorData(id)))
          pathScan "/api/v1/sensor/%s/%d" (fun (id, min) -> JSON (Db.AllDataFromDuration(id, min)))
          pathScan "/api/v1/temperature/avg/%s/%d" (fun (id, min) -> JSON (Db.AvgTemperature(id, min)))
          pathScan "/api/v1/temperature/%s/%d" (fun (id, min) -> JSON (Db.TemperatureValuesFromDuration(id, min)))
          pathScan "/api/v1/noise/avg/%s/%d" (fun (id, min) -> JSON (Db.AvgNoise(id, min)))
          pathScan "/api/v1/noise/%s/%d" (fun (id, min) -> JSON (Db.NoiseValuesFromDuration(id, min)))
          pathScan "/api/v1/voc/avg/%s/%d" (fun (id, min) -> JSON (Db.AvgVoc(id, min)))
          pathScan "/api/v1/voc/%s/%d" (fun (id, min) -> JSON (Db.VocValuesFromDuration(id, min)))
          basicAuth // from here on it will require authentication
          path "/api/v1/last" >>= JSON (Db.LastUpdate())
        ]
      POST >>= choose
        [ path "/api/v1/hello" >>= OK "Hello POST"
          path "/api/v1/goodbye" >>= OK "Good bye POST"
       ] 
    ]

[<EntryPoint>]
let main argv =
    startWebServer defaultConfig app
    0    