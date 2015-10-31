module Parser

open System
open Db
open Gecko

let ParseSensorName(id) = 
    match id with
        | "000D6F0003141E14" -> "Neukkari"
        | "000D6F0003E4EC19" -> "Pikkuneukkari 2"
        | "000D6F0004476483" -> "Avotila"
        | "000D6F0004491FF6" -> "Pikkuneukkari 1"
        | "000D6F000449280A" -> "Kuntosali"
        | "000D6F000449336B" -> "Pieni työhuone"
        | _ -> id

let NoiseDiff(data : SqlConnection.ServiceTypes.Cubesensors_data) =
    let lastAvg = Db.AvgNoise(data.SensorId, 2)
    let dailyAvgNoise = Db.AvgNoiseDaily(data.SensorId)
    let noiseDiff = lastAvg - dailyAvgNoise
    (System.Math.Round(noiseDiff,3)).ToString() + " db" 

let ParseNoiseAverages(id) =
    let last = System.Math.Round (Db.AvgNoise(id, 3), 3)
    let dailyAvg = System.Math.Round  (Db.AvgNoiseDaily(id), 3)
    let deduct = System.Math.Round (last - dailyAvg, 3)
    Gecko.WrapToNumber (deduct.ToString(), "Current: " + last.ToString() + " | Average: " + dailyAvg.ToString())

let ParseTempeature(id) =
    let last = Db.AvgTemperature(id, 2) / 100.0
    Gecko.WrapToNumber (last.ToString(), DateTime.Now.ToShortTimeString())

let GetSensorEmptyData(id : string, datas : seq<SqlConnection.ServiceTypes.Cubesensors_data>, status : Status) =
    ParseSensorName(id),
    "No connection",
    "-",
    status |> Db.StatusColor

let GetSensorData(id : string, datas : seq<SqlConnection.ServiceTypes.Cubesensors_data>, status : Status) =
    // Now we expect that data is found from the collection
    let x = datas |> Seq.filter (fun (k) -> k.SensorId = id) |> Seq.head

    ParseSensorName(x.SensorId), // +  " (" + NoiseDiff(x) + ")", 
    (float x.Temperature.Value / 100.0).ToString() + " °C | " + 
    (x.Noise.Value).ToString() + " db | " +
    x.Light.Value.ToString() + " lux | " +
    x.Pressure.Value.ToString() + " mBar | " +
    x.Humidity.Value.ToString() + " % | " +
    x.Voc.Value.ToString() + " ppm",
    x.Battery.Value.ToString(),
    status |> Db.StatusColor

let GetParseFunction (id, ids) = 
    match id with
                | id when Seq.exists (fun i -> i = id) ids -> GetSensorData
                | _ -> GetSensorEmptyData

let ParseAll() =
    let ids = Db.GetSensorIds()
    // Just in case take last 3 mins if there is not available for last min
    let datas = ids
                |> Seq.map (fun id -> Db.AllDataFromDuration(id, 3) |> Seq.toList)
                |> Seq.filter (fun i -> i.Length > 0)
                |> Seq.map (fun i -> List.head i)

    let statuses = Db.GetLatestStatuses()

    let ids = datas |> Seq.map (fun m -> m.SensorId)
                                        
    statuses
        |> Seq.map (fun x -> GetParseFunction(fst x, ids)(fst x, datas, snd x))                           
        |> Gecko.WrapToList