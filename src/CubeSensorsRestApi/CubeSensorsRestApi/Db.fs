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

let PropertyValuesFromDuration(sensorId:string, minutes:int, property:SqlConnection.ServiceTypes.Cubesensors_data -> Nullable<int>) =
    let data = dataForSensor(sensorId, minutes)
    data
        |> Seq.filter (fun x -> (property x).HasValue)
        |> Seq.map (fun x -> x.MeasurementTime, (property x).Value)

let NoiseValuesFromDuration(sensorId:string, minutes:int) =
    PropertyValuesFromDuration(sensorId, minutes, (fun x -> x.Noise))

let TemperatureValuesFromDuration(sensorId:string, minutes:int) =
    PropertyValuesFromDuration(sensorId, minutes, (fun x -> x.Temperature))

let AvgTemperature(sensorId:string, minutes:int) =
    query {
        for r in GetDb().Cubesensors_data do
        where (r.SensorId = sensorId && 
               r.MeasurementTime > getCurrentTime().Subtract(TimeSpan.FromMinutes((float)minutes)) && 
               r.Temperature.HasValue)
        averageBy (float r.Temperature.Value)
        }

//let allData () = query {
//    for r in GetDb().Cubesensors_data do
//    select r
//    }

//let AllSensorData(sensorId:string) =
//    allData ()
//        |> Seq.filter (fun x -> x.SensorId = sensorId)

// Better to let SQL Server handle filtering (use dataForSensor)
//let AllDataFromDuration(sensorId:string, minutes:int) =
//    allData ()
//        |> Seq.filter (fun x -> x.SensorId = sensorId)
//        |> Seq.filter (fun x -> x.MeasurementTime > DateTime.UtcNow.Subtract(TimeSpan.FromMinutes((float)minutes)))

//let PropertyAvg(sensorId:string, minutes:int, property:SqlConnection.ServiceTypes.Cubesensors_data -> Nullable<int>) : float =
//    allData ()
//        |> Seq.filter (fun x -> x.SensorId = sensorId)
//        |> Seq.filter (fun x -> (property x).HasValue)
//        |> Seq.filter (fun x -> x.MeasurementTime > DateTime.UtcNow.Subtract(TimeSpan.FromMinutes((float)minutes)))
//        |> Seq.map (fun x -> (property x).Value)
//        |> Seq.averageBy (fun x -> (float)x)

//let AvgTemperature(sensorId:string, minutes:int) =
//    PropertyAvg(sensorId, minutes, (fun x -> x.Temperature))
//
//let AvgNoise(sensorId:string, minutes:int) =
//    PropertyAvg(sensorId, minutes, (fun x -> x.Noise))
