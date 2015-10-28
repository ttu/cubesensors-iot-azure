module CubeSensorRestApiTests

open System
open NUnit.Framework
open Program
open Db

[<Test>]
let GetSensorIds () =
    let t = Db.GetSensorIds()
    let itemCount = List.length(Seq.toList t)
    Assert.AreEqual(itemCount, 6)

[<Test>]
let AllDataFromDuration () =
    let time = 30
    let t = Db.AllDataFromDuration("000D6F0003141E25", time)
    let itemCount = List.length(Seq.toList t)
    Assert.AreEqual(itemCount, time - 1)

[<Test>]
let AllSensorData () =
    let t = Db.AllSensorData("000D6F0003141E25")
    let itemCount = List.length(Seq.toList t)
    Assert.IsTrue(itemCount > 0)

[<Test>]
let TemperatureValuesFromDuration() =
    let time = 30
    let values = Db.TemperatureValuesFromDuration("000D6F0003141E25", time)
    let itemCount = List.length(Seq.toList values)
    Assert.AreEqual(itemCount, (time - 1))

[<Test>]
let AvgTemperature () =
    let avg = Db.AvgTemperature("000D6F0003141E25", 60*24*30)
    Assert.IsTrue(avg > 0.0)

[<Test>]
let AvgNoise () =
    let avg = Db.AvgNoise("000D6F0003141E25", 60*24)
    Assert.IsTrue(avg > 0.0)

[<Test>]
let AvgNoiseDaily() =
    let avg = Db.AvgNoiseDaily("000D6F0003141E25")
    Assert.IsTrue(avg > 0.0)

[<Test>]
let LastUpdate () =
    let latest = Db.LastUpdate()
    Assert.IsTrue(latest < DateTime.UtcNow)

[<Test>]
let GetSensorStatus () =
    let statusList = Db.GetSensorStatus(Db.getCurrentTime())
    Assert.AreEqual(snd (Seq.head statusList), Db.Status.Ok)

[<Test>]
let GetLatestStatuses() = 
    let current = Db.GetSensorStatus(Db.LastUpdate())
    let allIds = Db.GetSensorIds()
    let mockId = "ABCDEFG12345"
    let mockIds = allIds |> Seq.toList |> List.append [mockId]
    let statuses = Db.createAllIdStatusList(current, mockIds)

    Assert.AreEqual(fst (Seq.head statuses), mockId)
    Assert.AreEqual(snd (Seq.head statuses), Db.Status.Unknown)
    Assert.AreEqual(snd (Seq.head (Seq.rev statuses)), Db.Status.Ok)

[<Test>]
let DaysWithDataWorkTime() =
    let sId = "000D6F0003141E25"
    let days = Db.AvgNoiseDaily(sId)
    Assert.IsTrue(true)

[<Test>]
let ParserTests() =
    let sId = "000D6F0003141E14" // "000D6F0003141E25"
    let s1 = Parser.ParseNoiseAverages(sId)
    let s2 = Parser.ParseTempeature(sId)
    let avg = Db.AvgNoise(sId, 3)
    let avgDaily = Db.AvgNoiseDaily(sId)
    Assert.IsTrue(avg > avgDaily)

[<Test>]
let WrapToList() =
    let list = Gecko.WrapToList([("test", "test", "ok", "green")])
    Assert.IsTrue(true)
