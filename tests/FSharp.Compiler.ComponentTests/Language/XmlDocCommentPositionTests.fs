// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Language

open Xunit
open FSharp.Test

module XmlDocCommentPositionTests =

    [<Fact>]
    let ``XML doc comment after code should warn``() =
        FSharp """
let x = 42                  /// This should trigger warning
"""
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 3537, Line 2, Col 25, Line 2, Col 28, "XML documentation comments should be the first non-whitespace text on a line.")
        ]

    [<Fact>]
    let ``XML doc comment at start of line should not warn``() =
        FSharp """
/// This is proper documentation
let x = 42
"""
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``XML doc comment with indentation should not warn``() =
        FSharp """
module Test =
    /// This is properly indented
    let x = 42
"""
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``XML doc comment after let binding should warn``() =
        FSharp """
let value = "test"          /// Bad position
"""
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 3537, Line 2, Col 25, Line 2, Col 28, "XML documentation comments should be the first non-whitespace text on a line.")
        ]