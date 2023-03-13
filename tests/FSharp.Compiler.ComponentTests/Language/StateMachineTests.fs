// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Language

open Xunit
open FSharp.Test.Compiler

module StateMachineTests =

    [<Fact>] // https://github.com/dotnet/fsharp/issues/13067
    let ``Declaring a local function with a flexible type``() = 
        let code = """
task {
    let m1 f s = Seq.map f s
    do! Async.Sleep 1
    do! System.Threading.Tasks.Task.Delay 1

    let m2 f (s: #seq<_>) = Seq.map f s
    do! Async.Sleep 1
    do! System.Threading.Tasks.Task.Delay 1

    return 1
}
|> fun f -> f.Wait()
"""

        Fsx code
        |> withNoOptimize
        |> compile
        |> shouldFail
        |> withWarningCode 3511
        |> ignore

        Fsx code
        |> withNoOptimize
        |> withOptions ["--nowarn:3511"]
        |> compileExeAndRun
        |> shouldSucceed
