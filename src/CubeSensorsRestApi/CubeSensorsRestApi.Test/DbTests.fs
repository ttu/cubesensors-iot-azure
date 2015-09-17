module CubeSensorRestApiTests

open System
open NUnit.Framework
open Db

[<Test>]
let GetSensorIds () =
    let t = Db.GetSensorIds()
    let itemCount = List.length(Seq.toList t)
    Assert.AreEqual(itemCount, 6)

[<Test>]
let AllDataFromDuration () =
    let time = 30
    let t = Db.AllDataFromDuration("000D5F000139280E", time)
    let itemCount = List.length(Seq.toList t)
    Assert.AreEqual(itemCount, time - 1)

[<Test>]
let AllSensorData () =
    let t = Db.AllSensorData("000D5F000139280E")
    let itemCount = List.length(Seq.toList t)
    Assert.IsTrue(itemCount > 0)

[<Test>]
let TemperatureValuesFromDuration() =
    let time = 30
    let values = Db.TemperatureValuesFromDuration("000D5F000139280E", time)
    let itemCount = List.length(Seq.toList values)
    Assert.AreEqual(itemCount, (time - 1))

[<Test>]
let AvgTemperature () =
    let avg = Db.AvgTemperature("000D5F000139280E", 60*24*30)
    Assert.IsTrue(avg > 0.0)

[<Test>]
let AvgNoise () =
    let avg = Db.AvgNoise("000D5F000139280E", 60*24)
    Assert.IsTrue(avg > 0.0)

[<Test>]
let LastUpdate () =
    let latest = Db.LastUpdate()
    Assert.IsTrue(latest < DateTime.UtcNow)

[<Test>]
let GetSensorStatus () =
    let statusList = Db.GetSensorStatus(Db.getCurrentTime())
    Assert.AreEqual(snd (Seq.head statusList), Db.Status.Ok)