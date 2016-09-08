// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Unittests

open System
open NUnit.Framework
open System.Collections.Generic

[<TestFixture>]
type SeqFusionTestsModule() =

    [<Test>]
    member this.FuseTwoMapsWithSameType() =
        let data = [3; 1; 2]
        let result = Seq.map (fun x -> x * 2) (Seq.map (fun x -> x + 2) data)
        Assert.areEqual [10; 6; 8] (Seq.toList result)

    [<Test>]
    member this.FuseTwoMapsWithSameType_String() =
        let data = ["hello"; "world"; "!"]
        let result = Seq.map (fun x -> "hello" + x) (Seq.map (fun (y:string) -> " " + y) data)
        Assert.areEqual ["hello hello"; "hello world"; "hello !"] (Seq.toList result)
        
    [<Test>]
    member this.FuseTwoMapsWithDifferentType() =
        let data = ["hello"; "world"; "!"]
        let result = Seq.map (fun x -> x * 3) (Seq.map (fun (y:string) -> y.Length) data)
        Assert.areEqual [15; 15; 3] (Seq.toList result)

    [<Test>]
    member this.FusisonOfTwoMapsKeepsSideEffectOrder() =
        let list = List<string>()
        let data = ["hello"; "world"; "!"]
        let result = Seq.map (fun x -> x * 3) (Seq.map (fun y -> list.Add y; y.Length) data)
        
        // seq is not evaluated yet
        Assert.areEqual 0 list.Count

        // evaluate it
        Assert.areEqual [15; 15; 3] (Seq.toList result)

        Assert.areEqual data (Seq.toList list)