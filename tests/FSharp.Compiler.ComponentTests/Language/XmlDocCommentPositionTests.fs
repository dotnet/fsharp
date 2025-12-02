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
            (Warning 3879, Line 2, Col 29, Line 2, Col 62, "XML documentation comments should be the first non-whitespace text on a line.")
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
            (Warning 3879, Line 2, Col 29, Line 2, Col 45, "XML documentation comments should be the first non-whitespace text on a line.")
        ]

    [<Fact>]
    let ``Regular comment after code should not warn``() =
        FSharp """
let x = 42                  // This is a regular comment, not XML doc
"""
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Regular comment with double slash after code should not warn``() =
        FSharp """
let value = "test"          // Regular comment
let other = value + "more"  // Another regular comment
"""
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Multiple regular comments after code should not warn``() =
        FSharp """
module Test =
    let x = 1  // comment 1
    let y = 2  // comment 2
    let z = x + y  // result
"""
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Four slash comment after code should not warn``() =
        FSharp """
let x = 42  //// This is a four-slash comment, not XML doc
"""
        |> compile
        |> shouldSucceed