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
let AllSensorData () =
    let t = Db.AllSensorData("000D5F000139280E")
    let itemCount = List.length(Seq.toList t)
    Assert.IsTrue(itemCount > 0)

[<Test>]
let AvgTemperature () =
    let avg = Db.AvgTemperature("000D5F000139280E", 60*24*30)
    Assert.IsTrue(avg > 0.0)