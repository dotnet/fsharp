// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.CompilerOptions
module ReflectionFree = 

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
    let ``Records and DUs don't have generated ToString`` () =
        someCode
        |> withOptions [ "--reflectionfree" ]
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "Thing says: Test+MyRecord"
        |> withStdOutContains "Thing says: Test+MyUnion+B"
        |> withStdOutContains "Thing says: Test+MyClass"

    [<Fact>]
    let ``No debug display attribute`` () =
        someCode
        |> withOptions [ "--reflectionfree" ]
        |> compile
        |> shouldSucceed
        |> verifyILNotPresent [ "[runtime]System.Diagnostics.DebuggerDisplayAttribute" ]
