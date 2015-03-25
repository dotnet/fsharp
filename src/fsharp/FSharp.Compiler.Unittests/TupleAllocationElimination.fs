// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Unittests

open System
open System.Text

open NUnit.Framework
open System.Collections.Generic
open System.Diagnostics

module TupleElimination = 
    let lookUp (d: IDictionary<int, string>) key =
        // tuple is allocated here, unless the https://github.com/Microsoft/visualfsharp/pull/331 is applied
        let r, v = d.TryGetValue(key)
        if r then v else ""


[<TestFixture>]
type TupleEliminationTest() =
    [<Test>]
    member this.RunLookup1() = 
        let dict = new Dictionary<_,_>()
        
        Assert.AreEqual("", TupleElimination.lookUp dict 42)

    [<Test>]
    member this.StressTestTupleEliminationWithDict() = 
        let cache = Dictionary<int, int64>()

        let rec square n =
            match cache.TryGetValue(n) with
            | true, v -> v
            | _ ->
                let v =
                    if n < 1 then
                        0L
                    else
                        square (n - 1) + (int64 n * 2L - 1L)
                cache.Add(n, v) |> ignore
                v

        let sw = Stopwatch.StartNew()

        let mutable sum = 0L
        for i=1 to 100 do
            cache.Clear()
            let res = square 10000
            sum <- sum + res

        let time = sw.Elapsed

        printfn "%d in %A" sum time