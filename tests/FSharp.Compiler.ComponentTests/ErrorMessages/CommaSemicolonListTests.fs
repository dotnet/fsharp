// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace ErrorMessages

open Xunit
open FSharp.Test.Compiler

module ``Comma vs semicolon in list`` =

    [<Fact>]
    let ``Warn on single tuple in list literal``() =
        FSharp """
let x = [1, 2, 3]
        """
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Information 3886, Line 2, Col 9, Line 2, Col 18,
                                 "This list expression contains a single tuple element. Did you mean to use ';' instead of ',' to separate list elements?")

    [<Fact>]
    let ``No warning on semicolon-separated list``() =
        FSharp """
let x = [1; 2; 3]
        """
        |> typecheck
        |> shouldSucceed
        |> withDiagnostics []

    [<Fact>]
    let ``No warning on parenthesized tuple in list``() =
        FSharp """
let x = [(1, 2, 3)]
        """
        |> typecheck
        |> shouldSucceed
        |> withDiagnostics []

    [<Fact>]
    let ``No warning on multi-element list of tuples``() =
        FSharp """
let x = [1, 2; 3, 4]
        """
        |> typecheck
        |> shouldSucceed
        |> withDiagnostics []

    [<Fact>]
    let ``No warning on array with tuple``() =
        FSharp """
let x = [| 1, 2, 3 |]
        """
        |> typecheck
        |> shouldSucceed
        |> withDiagnostics []

    [<Fact>]
    let ``Warn on two-element tuple in list``() =
        FSharp """
let x = [1, 2]
        """
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Information 3886, Line 2, Col 9, Line 2, Col 15,
                                 "This list expression contains a single tuple element. Did you mean to use ';' instead of ',' to separate list elements?")

    [<Fact>]
    let ``Warning is suppressible via nowarn``() =
        FSharp """
#nowarn "3886"
let x = [1, 2, 3]
        """
        |> typecheck
        |> shouldSucceed
        |> withDiagnostics []

    [<Fact>]
    let ``No warning on struct tuple in list``() =
        FSharp """
let x = [struct(1, 2, 3)]
        """
        |> typecheck
        |> shouldSucceed
        |> withDiagnostics []

    [<Fact>]
    let ``No warning on empty list``() =
        FSharp """
let x: int list = []
        """
        |> typecheck
        |> shouldSucceed
        |> withDiagnostics []
