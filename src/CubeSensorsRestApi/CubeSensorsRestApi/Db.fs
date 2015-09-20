module Db

open System
open System.Data
open System.Data.Linq
open Microsoft.FSharp.Data.TypeProviders

type SqlConnection = Microsoft.FSharp.Data.TypeProviders.SqlDataConnection<ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=C:\SRC\GITHUB\CUBESENSORS-IOT-AZURE\SAMPLE_DATA\CUBE_DB.MDF;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False">

let runtimeConnStr = Config.LocalDb

// If connection string is LocalDb, we will assume this is for development use
let isDevTime = runtimeConnStr = Config.LocalDb

let GetDb () = SqlConnection.GetDataContext(runtimeConnStr)

let LastUpdate() = 
    query {
        for r in GetDb().Cubesensors_data do
        sortByDescending r.MeasurementTime 
        select r.MeasurementTime
        head
    }

let getCurrentTime() =
    match isDevTime with
    | true -> LastUpdate()
    | false -> DateTime.UtcNow

let allDataForSensor (sensorId:string) = 
    query {
        for r in GetDb().Cubesensors_data do
        where (r.SensorId = sensorId)
        select r
    }

let dataForSensor (sensorId:string, minutes:int) = 
    query {
        for r in GetDb().Cubesensors_data do
        where (r.SensorId = sensorId && r.MeasurementTime > getCurrentTime().Subtract(TimeSpan.FromMinutes((float)minutes)))
        select r
    }

let GetSensorIds() = 
    query {
        for r in GetDb().Cubesensors_data do
        select r.SensorId
        distinct
    }

let AllSensorData(sensorId:string) =
    allDataForSensor (sensorId)

let AllDataFromDuration(sensorId:string, minutes:int) =
    dataForSensor(sensorId, minutes)

// Use record type to return values. This way JSON won't have item1 and item2.
type Record = {
    Time : DateTime;
    Value : int;
}

type Status = Ok | Charge | Charging | Unplug | Unknown

// Better way would be to define field in query, but don't know how to use (property x) in query
let PropertyValuesFromDuration(sensorId:string, minutes:int, property:SqlConnection.ServiceTypes.Cubesensors_data -> Nullable<int>) =
    let data = dataForSensor(sensorId, minutes)
    data
        |> Seq.filter (fun x -> (property x).HasValue)
        |> Seq.map (fun x -> { Record.Time = x.MeasurementTime; Record.Value = (property x).Value})

let NoiseValuesFromDuration(sensorId:string, minutes:int) =
    PropertyValuesFromDuration(sensorId, minutes, (fun x -> x.Noise))

let TemperatureValuesFromDuration(sensorId:string, minutes:int) =
    PropertyValuesFromDuration(sensorId, minutes, (fun x -> x.Temperature))

let VocValuesFromDuration(sensorId:string, minutes:int) =
    PropertyValuesFromDuration(sensorId, minutes, (fun x -> x.Voc))

let AvgTemperature(sensorId:string, minutes:int) =
    query {
        for r in GetDb().Cubesensors_data do
        where (r.SensorId = sensorId && 
               r.MeasurementTime > getCurrentTime().Subtract(TimeSpan.FromMinutes((float)minutes)) && 
               r.Temperature.HasValue)
        averageBy (float r.Temperature.Value)
        }

let AvgNoise(sensorId:string, minutes:int) =
    query {
        for r in GetDb().Cubesensors_data do
        where (r.SensorId = sensorId && 
               r.MeasurementTime > getCurrentTime().Subtract(TimeSpan.FromMinutes((float)minutes)) && 
               r.Noise.HasValue)
        averageBy (float r.Noise.Value)
        }

let AvgVoc(sensorId:string, minutes:int) =
    query {
        for r in GetDb().Cubesensors_data do
        where (r.SensorId = sensorId && 
               r.MeasurementTime > getCurrentTime().Subtract(TimeSpan.FromMinutes((float)minutes)) && 
               r.Voc.HasValue)
        averageBy (float r.Voc.Value)
        }

let GetSensorStatus(time:DateTime) =
    let data = query {
        for r in GetDb().Cubesensors_data do
        where (r.MeasurementTime = time && r.Battery.HasValue && r.Cable.HasValue)
        select (r.SensorId, r.Battery.Value, r.Cable.Value)
        }
    data
        |> Seq.map (fun x -> 
            match x with 
            | (id, bat, cbl) when bat < 10 && cbl = 0 -> id, Status.Charge
            | (id, bat, cbl) when bat > 96 && cbl = 1 -> id, Status.Unplug
            | (id, bat, cbl) when cbl = 1 -> id, Status.Charging
            | (id, bat, cbl) -> id, Status.Ok)

let createAllIdStatusList(current, allIds) =
     allIds
        |> Seq.map(fun x -> 
            match x with
                | id when (Seq.exists (fun y -> fst y = id) current) -> id, snd (Seq.head (Seq.filter (fun i -> fst i = x) current))
                | _ -> x, Status.Unknown
            )

let GetLeatestStatuses() =
    let current = GetSensorStatus(getCurrentTime())
    let allIds = GetSensorIds()
    createAllIdStatusList(current, allIds)

