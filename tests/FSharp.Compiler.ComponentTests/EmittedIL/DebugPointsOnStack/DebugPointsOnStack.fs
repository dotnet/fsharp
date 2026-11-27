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

    // https://github.com/dotnet/fsharp/issues/20910
    [<Theory; InlineData(false, true); InlineData(true, true); InlineData(false, false)>]
    let ``Decision tree target bindings use the incoming stack`` optimize debug =
        FSharp """
module DecisionTreeTargetStack

let (|Add|) (k: int) (x: int) = x + k

let emptyStack x =
    Some (match x with Add 1 0 -> 0 | Add 2 y -> y)

let pendingOperands x =
    struct ("pending", 42L, match x with Add 1 0 -> 0 | Add 2 y -> y)

let sharedTarget tag x =
    Some (
        match tag, x with
        | (0, Add 1 y)
        | (1, Add 2 y) -> y
        | (_, Add 3 y) -> y)

[<EntryPoint>]
let main _ =
    if emptyStack -1 <> Some 0 then failwith "first target"
    if emptyStack 3 <> Some 5 then failwith "last target"
    if pendingOperands -1 <> struct ("pending", 42L, 0) then failwith "first target with pending operands"
    if pendingOperands 3 <> struct ("pending", 42L, 5) then failwith "last target with pending operands"
    if sharedTarget 0 3 <> Some 4 then failwith "shared target before bindings"
    if sharedTarget 1 3 <> Some 5 then failwith "shared target after bindings"
    if sharedTarget 2 3 <> Some 6 then failwith "last target after shared target"
    0
"""
        |> asExe
        |> (if optimize then withOptimize else withNoOptimize)
        |> (if debug then withDebug else withNoDebug)
        |> compileAndRun
        |> shouldSucceed
