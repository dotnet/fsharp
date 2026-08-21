// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

open Xunit
open FSharp.Test.Compiler

module ``Test Compiler Directives`` =

    [<Fact>]
    let ``r# "" is invalid`` () =
        Fsx"""
#r ""
        """ |> ignoreWarnings
            |> compile
            |> shouldSucceed
            |> withSingleDiagnostic (Warning 213, Line 2, Col 1, Line 2, Col 6, "'' is not a valid assembly name")

    [<Fact>]
    let ``#r "    " is invalid`` () =
        Fsx"""
#r "    "
        """ |> compile
            |> shouldFail
            |> withSingleDiagnostic (Warning 213, Line 2, Col 1, Line 2, Col 10, "'' is not a valid assembly name")

    // https://github.com/dotnet/fsharp/issues/3841
    [<Fact>]
    let ``Hash directive inside nested module produces indentation error`` () =
        Fsx
            """
let x = 42

module Nested =
    let foo = 123

#r "SomeAssembly"

    let bar = 1

let y = x
            """
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withSingleDiagnostic
            (Error 58, Line 11, Col 1, Line 11, Col 4, "Unexpected syntax or possible incorrect indentation: this token is offside of context started at position (9:5). Try indenting this further.\nTo continue using non-conforming indentation, pass the '--strict-indentation-' flag to the compiler, or set the language version to F# 7.")

module ``Test compiler directives in FSI`` =
    [<Fact>]
    let ``r# "" is invalid`` () =
        Fsx"""
#r ""
        """ |> ignoreWarnings
            |> eval
            |> shouldFail
            |> withSingleDiagnostic (Error 2301, Line 2, Col 1, Line 2, Col 6, "'' is not a valid assembly name")