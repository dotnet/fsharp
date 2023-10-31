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

    [<Fact>]
    let ``#i works with __SOURCE_DIRECTORY__ and string literals`` () =
        Fsx """
#i "nuget:" __SOURCE_DIRECTORY__
        """
        |> compile
        |> shouldSucceed

module ``Test compiler directives in FSI`` =
    [<Fact>]
    let ``r# "" is invalid`` () =
        Fsx"""
#r ""
        """ |> ignoreWarnings
            |> eval
            |> shouldFail
            |> withSingleDiagnostic (Error 2301, Line 2, Col 1, Line 2, Col 6, "'' is not a valid assembly name")

    [<Fact>]
    let ``#i works with __SOURCE_DIRECTORY__ and string literals`` () =
        Fsx """
#i "nuget:" __SOURCE_DIRECTORY__
        """
        |> eval
        |> shouldSucceed