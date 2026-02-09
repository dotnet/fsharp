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
