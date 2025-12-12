namespace FSharp.Compiler.ComponentTests.Conformance.Inference

open FSharp.Test
open Xunit

module SRTP_NuGet =

    [<FSharp.Test.FactSkipOnSignedBuild>]
    let ``SRTP resolution with curryN and Tuple from FSharpPlus NuGet`` () =
        CompilerAssert.RunScriptWithOptions
            [| "--langversion:preview"; "--source"; "https://api.nuget.org/v3/index.json" |]
            """
#r "nuget: FSharpPlus, 1.6.1"
open FSharpPlus
open System

let f1 (x: Tuple<_>) = [x.Item1]

let test () =
    let _x1 = curryN f1 100
    ()

test()
"""
            []
