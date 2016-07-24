// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Unittests

open System
open NUnit.Framework

type A(s: string) = 
    member val Y = s

type B = {
    X : string
    Y : int
}


[<TestFixture>]
type StringInterpolationTests() =
    [<Test>]
    member this.``should interpolate local variable``() =
        let bar = "!!!abc!!!"
        Assert.AreEqual(bar,sprintf "%(bar)")

//    [<Test>]
//    member this.``should interpolate record property``() =
//        let b = { X = "world"; Y = 42 }
//        Assert.AreEqual("hello world and 42.",sprintf "hello %(b.X) and %(b.Y).")

    [<Test>]
    member this.``should interpolate local variable and class``() =
        let bar = "!!!abc!!!"
        let baz = new A("100500")
        Assert.AreEqual("start foo!!!abc!!! + 100500abc 999",sprintf "%s foo%(bar) + %(baz.Y + bar.[3..5]) %d" "start" 999)

//    [<Test>]
//    member this.``interpolate %(number) is consistent to %d`` () =
//        let prefix = "blab"
//        let suffix = "blub"
//        [-10..10]
//        |> List.iter (fun number -> Assert.AreEqual(sprintf "%s%d%s" prefix number suffix,sprintf "%s%(number)%s" prefix suffix))

