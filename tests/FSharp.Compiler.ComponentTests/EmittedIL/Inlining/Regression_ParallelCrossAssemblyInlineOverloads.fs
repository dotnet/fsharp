namespace EmittedIL.Inlining

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Regression_ParallelCrossAssemblyInlineOverloads =

    [<Fact>]
    let ``Cross-assembly overloaded inline static members compile and run`` () =
        let library =
            FSharp """
module Library

type InlineOps =
    static member inline Source (x: int) : int = x + 1
    static member inline Source (x: string) : string = x + "!"
"""
            |> withOutputType CompileOutput.Library
            |> withName "Library"
            |> withOptimize

        let consumer =
            FSharp """
module Consumer

open Library

[<EntryPoint>]
let main _ =
    let x = InlineOps.Source 41
    let y = InlineOps.Source "hello"
    if x = 42 && y = "hello!" then 0 else 1
"""
            |> withOutputType CompileOutput.Exe
            |> withReferences [ library ]
            |> withOptimize

        consumer
        |> compileAndRun
        |> shouldSucceed
        |> ignore
