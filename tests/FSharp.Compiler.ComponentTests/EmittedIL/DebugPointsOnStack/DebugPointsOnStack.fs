namespace EmittedIL

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module DebugPointsOnStack =

    // A debug point must be emitted at an empty evaluation stack so a debugger can bind a breakpoint to it.
    // The baseline shows the operand pending across 'System.Console.WriteLine()' spilled to a local for that.
    [<Theory; FileInlineData("DebugPointInOperandPosition.fs")>]
    let ``DebugPointInOperandPosition_fs`` compilation =
        compilation
        |> getCompilation
        |> withNoOptimize
        |> withEmbeddedPdb
        |> ignoreWarnings
        |> compile
        |> verifyILBaseline
