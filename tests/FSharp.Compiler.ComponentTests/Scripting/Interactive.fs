// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Scripting

open Xunit
open System
open FSharp.Test.Compiler

module ``Interactive tests`` =
    [<Fact>]
    let ``Eval object value``() =
        Fsx "1+1"
        |> eval
        |> shouldSucceed
        |> withEvalTypeEquals typeof<int>
        |> withEvalValueEquals 2

    [<Fact>]
    let ``Pretty print void pointer``() =
        Fsx "System.IntPtr.Zero.ToPointer()"
        |> runFsi
        |> shouldSucceed
        |> withStdOutContains "val it: voidptr = 0n"

    [<Fact>]
    let ``EntryPoint attribute in FSI should produce a compiler warning`` () =
        Fsx "[<EntryPoint>] let myFunc _ = 0"
        |> eval
        |> shouldFail
        |> withDiagnostics [
            (Warning 2304, Line 1, Col 3, Line 1, Col 13, "Functions with [<EntryPoint>] are not invoked in FSI. 'myFunc' was not invoked. Execute 'myFunc <args>' in order to invoke 'myFunc' with the appropriate string array of command line arguments.")
        ]

module ``External FSI tests`` =
    [<Fact>]
    let ``Eval object value``() =
        Fsx "1+1"
        |> runFsi
        |> shouldSucceed

    [<Fact>]
    let ``Invalid expression should fail``() =
        Fsx "1+a"
        |> runFsi
        |> shouldFail


    [<Fact>]
    let ``Internals visible over a large number of submissions``() =
        let submission =
            let lines = [|
                yield """let internal original_submission = "From the first submission";;""" + Environment.NewLine
                for _ in 1 .. 200 do yield """if original_submission <> "From the first submission" then failwith $"Failed to read an internal at line: {__LINE__}";;""" + Environment.NewLine
                |]
            lines |> Array.fold(fun acc line -> acc + line) ""
        Fsx submission
        |> runFsi
        |> shouldSucceed

