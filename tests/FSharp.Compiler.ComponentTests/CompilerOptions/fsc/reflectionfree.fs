// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module CompilerOptions.Fsc.ReflectionFree

open Xunit
open FSharp.Test.Compiler

[<Fact>]
let ``Gives error when %A is used`` () =
    FSharp """printfn "Hello, %A" "world" """
    |> asExe
    |> withOptions [ "--reflectionfree" ]
    |> compile
    |> shouldFail
    |> withDiagnostics [
        Error 741, Line 1, Col 9, Line 1, Col 20,
        "Unable to parse format string 'The '%A' format specifier may not be used in an assembly being compiled with option '--reflectionfree'. This construct implicitly uses reflection.'" ]

let someCode =
    FSharp """
    module Test

    type MyRecord = { A: int }
    type MyUnion = A of int | B of string
    type MyClass() = member val A = 42

    let poke thing = $"Thing says: {thing}"

    [<EntryPoint>]
    let doStuff _ =
        poke { A = 3 } |> printfn "%s"
        poke <| B "foo" |> printfn "%s"
        poke <| MyClass() |> printfn "%s"
        0
    """

[<Fact>]
let ``Records and classes don't have generated ToString`` () =
    someCode
    |> withOptions [ "--reflectionfree" ]
    |> compileExeAndRun
    |> shouldSucceed
    |> withStdOutContains "Thing says: Test+MyRecord"
    |> withStdOutContains "Thing says: Test+MyClass"

[<Fact>]
let ``Unions have a generated ToString that matches on the case`` () =
    someCode
    |> withOptions [ "--reflectionfree" ]
    |> compileExeAndRun
    |> shouldSucceed
    |> withStdOutContains "Thing says: B(foo)"

[<Fact>]
let ``Generic unions get a correct generated ToString`` () =
    FSharp """
module Test
type Box<'T> = Box of 'T | Empty            // single nullary case -> UseNullAsTrueValue representation
type Single<'T> = Just of 'T

[<EntryPoint>]
let main _ =
    Box 42 |> string |> printfn "%s"
    Box (Box 7) |> string |> printfn "%s"   // nested generic
    (Empty: Box<int>) |> string |> printfn "%s"
    Just 5 |> string |> printfn "%s"         // single-case generic union
    0
    """
    |> asExe
    |> withOptions [ "--reflectionfree" ]
    |> compileExeAndRun
    |> shouldSucceed
    |> withStdOutContains "Box(42)"
    |> withStdOutContains "Box(Box(7))"
    |> withStdOutContains "Empty"
    |> withStdOutContains "Just(5)"

[<Fact>]
let ``Generated ToString renders a null field as "null" like option does`` () =
    FSharp """
module Test
type W = W of string

[<EntryPoint>]
let main _ =
    W null |> string |> printfn "%s"                 // null field -> "null"
    (Some (null: string)).ToString() |> printfn "%s" // option renders it the same way
    0
    """
    |> asExe
    |> withOptions [ "--reflectionfree" ]
    |> compileExeAndRun
    |> shouldSucceed
    |> withStdOutContains "W(null)"
    |> withStdOutContains "Some(null)"

[<Fact>]
let ``No debug display attribute`` () =
    someCode
    |> withOptions [ "--reflectionfree" ]
    |> compile
    |> shouldSucceed
    |> verifyILNotPresent [ "[runtime]System.Diagnostics.DebuggerDisplayAttribute" ]
