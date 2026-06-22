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
let ``Classes don't have a generated ToString`` () =
    someCode
    |> withOptions [ "--reflectionfree" ]
    |> compileExeAndRun
    |> shouldSucceed
    |> withStdOutContains "Thing says: Test+MyClass"

[<Fact>]
let ``Records get a generated single-line ToString`` () =
    FSharp """
module Test
type Point = { X: int; Y: int }
type Nested = { P: Point; S: string }

[<EntryPoint>]
let main _ =
    { X = 1; Y = 2 } |> string |> System.Console.WriteLine
    { P = { X = 1; Y = 2 }; S = null } |> string |> System.Console.WriteLine // nested record + null field
    0
    """
    |> asExe
    |> withOptions [ "--reflectionfree" ]
    |> compileExeAndRun
    |> shouldSucceed
    |> withStdOutContains "{ X = 1; Y = 2 }"
    |> withStdOutContains "{ P = { X = 1; Y = 2 }; S = null }"

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
type Box<'T> =
    | Box of 'T
    | Empty
type Single<'T> = | Just of 'T

[<EntryPoint>]
let main _ =
    Box 42 |> string |> System.Console.WriteLine
    Box (Box 7) |> string |> System.Console.WriteLine // nested generic
    (Empty: Box<int>) |> string |> System.Console.WriteLine
    Just 5 |> string |> System.Console.WriteLine // single-case generic union
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
let ``Generated ToString renders a field the same way option does`` () =
    FSharp """
module Test
type Wrapper = | Wrap of string

[<EntryPoint>]
let main _ =
    let value: string = null
    // A union field should render its content the same way option does. Compare the two directly rather
    // than asserting a fixed rendering. "Wrap" and "Some" are both 4 chars, so dropping them leaves the
    // field rendering to compare.
    let fromUnion = (Wrap value |> string).Substring 4
    let fromOption = ((Some value).ToString()).Substring 4
    if fromUnion = fromOption then System.Console.WriteLine "fields-render-alike"
    else System.Console.WriteLine("DIFFER: " + fromUnion + " vs " + fromOption)
    0
    """
    |> asExe
    |> withOptions [ "--reflectionfree" ]
    |> compileExeAndRun
    |> shouldSucceed
    |> withStdOutContains "fields-render-alike"

[<Fact>]
let ``A hand-written ToString override is kept, not replaced by the generated one`` () =
    FSharp """
module Test
type MyDU =
    | A of int
    override _.ToString() = "custom-du"

type MyRecord =
    { X: int }
    override _.ToString() = "custom-record"

[<EntryPoint>]
let main _ =
    A 1 |> string |> System.Console.WriteLine
    { X = 1 } |> string |> System.Console.WriteLine
    0
    """
    |> asExe
    |> withOptions [ "--reflectionfree" ]
    |> compileExeAndRun
    |> shouldSucceed
    |> withStdOutContains "custom-du"
    |> withStdOutContains "custom-record"

[<Fact>]
let ``No debug display attribute`` () =
    someCode
    |> withOptions [ "--reflectionfree" ]
    |> compile
    |> shouldSucceed
    |> verifyILNotPresent [ "[runtime]System.Diagnostics.DebuggerDisplayAttribute" ]
