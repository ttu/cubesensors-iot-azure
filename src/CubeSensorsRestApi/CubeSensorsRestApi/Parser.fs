module Parser

open System
open Db
open Gecko

let ParseSensorName(id) = 
    match id with
        | "000D6F0003141E14" -> "Room 1"
        | "000D6F0003E4EC19" -> "Room 2"
        | "000D6F0004476483" -> "Room 3"
        | "000D6F0004491FF6" -> "Room 4"
        | "000D6F000449280A" -> "Room 5"
        | "000D6F000449336B" -> "Room 6"
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
    Gecko.WrapToNumber (deduct.ToString(), last.ToString() + " | " + dailyAvg.ToString() + " | " + id)

let ParseTempeature(id) =
    let last = Db.AvgTemperature(id, 1) / 100.0
    Gecko.WrapToNumber (last.ToString(), DateTime.Now.ToShortTimeString() + " | " + id)

let ParseAll() =
    let ids = Db.GetSensorIds()
    let data = ids
                |> Seq.map (fun id -> Db.AllDataFromDuration(id, 1) |> Seq.toList |> List.head)

    let statuses = Db.GetLatestStatuses()

    data
        |> Seq.map (fun x -> ParseSensorName(x.SensorId), // +  " (" + NoiseDiff(x) + ")", 
                                (float x.Temperature.Value / 100.0).ToString() + " °C | " + 
                                (x.Noise.Value).ToString() + " db | " +
                                x.Light.Value.ToString() + " lux | " +
                                x.Pressure.Value.ToString() + " mBar | " +
                                x.Humidity.Value.ToString() + " % | " +
                                x.Voc.Value.ToString() + " ppm",
                                x.Battery.Value.ToString(),
                                statuses |> Seq.filter (fun (k,v) -> k = x.SensorId) |> Seq.head |> snd |> Db.StatusColor)
        |> Gecko.WrapToList