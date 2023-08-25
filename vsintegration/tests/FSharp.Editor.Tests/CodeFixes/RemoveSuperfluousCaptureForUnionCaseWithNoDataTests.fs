// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.RemoveSuperflousCaptureForUnionCaseWithNoDataTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix =
    RemoveSuperfluousCaptureForUnionCaseWithNoDataCodeFixProvider()

[<Theory>]
[<InlineData "_">]
[<InlineData "__">]
[<InlineData "a">]
let ``Fixes FS3548 - DUs`` caseValue =
    let code =
        $"""
type Type = | A | B of int

let f x =
    match x with
    | A {caseValue} -> 42
    | B number -> number
"""

    let expected =
        Some
            {
                Message = "Remove unused binding"
                FixedCode =
                    """
type Type = | A | B of int

let f x =
    match x with
    | A -> 42
    | B number -> number
"""
            }

    let actual = codeFix |> tryFix code (WithOption "--langversion:preview")

    Assert.Equal(expected, actual)

[<Theory>]
[<InlineData "_">]
[<InlineData "__">]
[<InlineData "t">]
let ``Fixes FS3548 - marker types`` caseValue =
    let code =
        $"""
type Type = Type

let f x =
    match x with
    | Type {caseValue} -> ()
"""

    let expected =
        Some
            {
                Message = "Remove unused binding"
                FixedCode =
                    """
type Type = Type

let f x =
    match x with
    | Type -> ()
"""
            }

    let actual = codeFix |> tryFix code (WithOption "--langversion:preview")

    Assert.Equal(expected, actual)

[<Theory>]
[<InlineData "a">]
[<InlineData "__">]
let ``Fixes FS0725 - DUs`` caseValue =
    let code =
        $"""
type Type = | A | B of int

let f x =
    match x with
    | A {caseValue} -> 42
    | B number -> number
"""

    let expected =
        Some
            {
                Message = "Remove unused binding"
                FixedCode =
                    """
type Type = | A | B of int

let f x =
    match x with
    | A -> 42
    | B number -> number
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Theory>]
[<InlineData "t">]
[<InlineData "__">]
let ``Fixes FS0725 - marker types`` caseValue =
    let code =
        $"""
type T = T

let f x =
    match x with
    | T {caseValue} -> ()
"""

    let expected =
        Some
            {
                Message = "Remove unused binding"
                FixedCode =
                    """
type T = T

let f x =
    match x with
    | T -> ()
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)
