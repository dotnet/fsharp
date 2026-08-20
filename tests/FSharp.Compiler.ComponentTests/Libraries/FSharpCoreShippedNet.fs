// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Libraries

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module FSharpCoreShippedNet =

    // Dogfood guard: a ComponentTest can compile+run against the shipped .NETCoreApp FSharp.Core
    // (e.g. net10.0) via withFSharpCoreShippedNet, instead of the default netstandard2.1 reference.
    let private runsAgainstShippedNetCore expected source =
        FSharp source
        |> withFSharpCoreShippedNet
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains expected

    // The expected framework name tracks FSharpCoreShippedNetTargetFramework in eng/TargetFrameworks.props - bump both together.
    [<FactForNETCOREAPP>]
    let ``compiles and runs against the shipped net FSharp.Core`` () =
        runsAgainstShippedNetCore "FSharpCoreFrameworkName=.NETCoreApp,Version=v10.0" """
module Test

open System.Runtime.Versioning

[<EntryPoint>]
let main _ =
    typeof<int list>.Assembly.GetCustomAttributes(typeof<TargetFrameworkAttribute>, false)
    |> Array.map (fun a -> (a :?> TargetFrameworkAttribute).FrameworkName)
    |> Array.tryHead
    |> Option.defaultValue "<none>"
    |> printfn "FSharpCoreFrameworkName=%s"
    0
"""

    // Sanity: ordinary FSharp.Core code still compiles and runs under the shipped net asset.
    [<FactForNETCOREAPP>]
    let ``basic FSharp.Core code runs against the shipped net FSharp.Core`` () =
        runsAgainstShippedNetCore "sum=385" """
module Test

[<EntryPoint>]
let main _ =
    [ 1..10 ] |> List.sumBy (fun x -> x * x) |> printfn "sum=%d"
    0
"""
