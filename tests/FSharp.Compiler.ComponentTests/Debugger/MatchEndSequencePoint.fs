// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Debugger

open Xunit
open FSharp.Test.Compiler
open Debugger.DebuggerTestHelpers

/// https://github.com/dotnet/fsharp/issues/12052
module MatchEndSequencePoint =

    [<Fact>]
    let ``Match expression does not produce extra sequence point at end`` () =
        verifyMethodDebugPoints
            "
module TestModule

let funcA (x: int) =
    let result =
        match x with
        | 1 -> \"one\"
        | 2 -> \"two\"
        | _ -> \"other\"
    System.Console.WriteLine(result)
            "
            "funcA"
            [ (Line 10, Col 5, Line 10, Col 37)
              (Line 6, Col 9, Line 6, Col 21)
              (Line 7, Col 16, Line 7, Col 21)
              (Line 8, Col 16, Line 8, Col 21)
              (Line 9, Col 16, Line 9, Col 23) ]

    [<Fact>]
    let ``Match in statement position does not produce extra sequence point`` () =
        verifyMethodDebugPoints
            "
module TestModule2

let funcB (x: int) =
    match x with
    | 1 -> System.Console.WriteLine(\"one\")
    | 2 -> System.Console.WriteLine(\"two\")
    | _ -> System.Console.WriteLine(\"other\")
    System.Console.WriteLine(\"done\")
            "
            "funcB"
            [ (Line 5, Col 5, Line 5, Col 17)
              (Line 6, Col 12, Line 6, Col 43)
              (Line 7, Col 12, Line 7, Col 43)
              (Line 8, Col 12, Line 8, Col 45)
              (Line 9, Col 5, Line 9, Col 37) ]

    [<Fact>]
    let ``Match with when guards has sequence points for all branches`` () =
        verifyMethodDebugPoints
            "
module TestModule3

let classify (x: int) (y: string) =
    match x with
    | n when n < 0 -> \"negative\"
    | 0 -> \"zero\"
    | n when n > 100 && y.Length > 0 -> \"big with text\"
    | n when n > 100 -> \"big\"
    | n when n > 50 -> \"medium\"
    | _ -> \"small\"
            "
            "classify"
            [ (Line 5, Col 5, Line 5, Col 17)
              (Line 6, Col 14, Line 6, Col 19)
              (Line 8, Col 14, Line 8, Col 21)
              (Line 8, Col 25, Line 8, Col 37)
              (Line 9, Col 14, Line 9, Col 21)
              (Line 10, Col 14, Line 10, Col 20)
              (Line 6, Col 23, Line 6, Col 33)
              (Line 7, Col 12, Line 7, Col 18)
              (Line 8, Col 41, Line 8, Col 56)
              (Line 9, Col 25, Line 9, Col 30)
              (Line 10, Col 24, Line 10, Col 32)
              (Line 11, Col 12, Line 11, Col 19) ]

    [<Fact>]
    let ``Match with when guards followed by code has no sequence point bleed`` () =
        verifyMethodDebugPoints
            "
module TestModule4

let processAndLog (x: int) =
    let label =
        match x with
        | n when n < 0 -> \"negative\"
        | n when n > 0 -> \"positive\"
        | _ -> \"zero\"
    System.Console.WriteLine(label)
            "
            "processAndLog"
            [ (Line 10, Col 5, Line 10, Col 36)
              (Line 6, Col 9, Line 6, Col 21)
              (Line 7, Col 18, Line 7, Col 23)
              (Line 8, Col 18, Line 8, Col 23)
              (Line 7, Col 27, Line 7, Col 37)
              (Line 8, Col 27, Line 8, Col 37)
              (Line 9, Col 16, Line 9, Col 22) ]

    /// https://github.com/dotnet/fsharp/issues/12052#issuecomment-974695340
    [<Fact>]
    let ``If-then-else does not produce extra sequence point at end`` () =
        verifyMethodDebugPoints
            "
module TestModuleIf

let funcC (x: int) =
    let result =
        if x > 0 then \"positive\"
        elif x = 0 then \"zero\"
        else \"negative\"
    System.Console.WriteLine(result)
            "
            "funcC"
            [ (Line 9, Col 5, Line 9, Col 37)
              (Line 6, Col 9, Line 6, Col 22)
              (Line 6, Col 23, Line 6, Col 33)
              (Line 7, Col 9, Line 7, Col 24)
              (Line 7, Col 25, Line 7, Col 31)
              (Line 8, Col 14, Line 8, Col 24) ]

    [<Fact>]
    let ``If-then-else in statement position does not produce extra sequence point`` () =
        verifyMethodDebugPoints
            "
module TestModuleIfStmt

let funcD (x: int) =
    if x > 0 then
        System.Console.WriteLine(\"positive\")
    else
        System.Console.WriteLine(\"non-positive\")
    System.Console.WriteLine(\"done\")
            "
            "funcD"
            [ (Line 5, Col 5, Line 5, Col 18)
              (Line 6, Col 9, Line 6, Col 45)
              (Line 8, Col 9, Line 8, Col 49)
              (Line 9, Col 5, Line 9, Col 37) ]

    [<Fact>]
    let ``Complex match with nested when guards has all sequence points within method range`` () =
        verifyMethodDebugPointsInRange
            "
module TestModule5

type Cmd = Start of int | Stop | Pause of string | Resume

let dispatch (cmd: Cmd) (active: bool) (count: int) =
    match cmd with
    | Start n when n > 0 && active ->
        let msg = sprintf \"Starting %d\" n
        msg
    | Start n when n > 0 ->
        \"start-inactive\"
    | Start _ ->
        \"start-invalid\"
    | Stop when active && count > 0 ->
        let status = sprintf \"Stopping after %d\" count
        status
    | Stop ->
        \"stop-noop\"
    | Pause reason when active && reason.Length > 0 ->
        sprintf \"Pausing: %s\" reason
    | Pause _ when active ->
        \"pause-no-reason\"
    | Pause _ ->
        \"pause-ignored\"
    | Resume when not active ->
        \"resuming\"
    | Resume ->
        \"already-active\"
            "
            "dispatch"
            (Line 6)
            (Line 29)

    // ---- Instrumentation: verify FeeFee IS emitted at match join points ----

    [<Fact>]
    let ``Match in statement position emits hidden point at join`` () =
        // #12052: The fix emits EmitStartOfHiddenCode unconditionally.
        // Verify hidden (FeeFee) points are present — proving the fix emits them.
        verifyAllSequencePoints
            "
module TestModuleHidden

let funcE (x: int) =
    match x with
    | 1 -> System.Console.WriteLine(\"one\")
    | _ -> System.Console.WriteLine(\"other\")
    System.Console.WriteLine(\"done\")
            " [
                (Line 5, Col 5, Line 5, Col 17)
                (Line 6, Col 12, Line 6, Col 43)
                (Line 7, Col 12, Line 7, Col 45)
                (Line 8, Col 5, Line 8, Col 37)
                (Line 16707566, Col 0, Line 16707566, Col 0)
                (Line 16707566, Col 0, Line 16707566, Col 0)
                (Line 16707566, Col 0, Line 16707566, Col 0)
            ]
