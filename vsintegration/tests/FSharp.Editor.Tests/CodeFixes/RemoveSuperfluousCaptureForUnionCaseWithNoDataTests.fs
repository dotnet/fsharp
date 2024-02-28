// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.RemoveSuperfluousCaptureForUnionCaseWithNoDataTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix =
    RemoveSuperfluousCaptureForUnionCaseWithNoDataCodeFixProvider()

[<Fact>]
let ``Fixes FS3548 - DUs`` () =
    let code =
        """
type Type = | A | B of int

let f x =
    match x with
    | A _ -> 42
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

[<Fact>]
let ``Fixes FS3548 - discarded argument in function`` () =
    let code =
        """
type C = | C
        
let myDiscardedArgFunc(C _) = ()
"""

    let expected =
        Some
            {
                Message = "Remove unused binding"
                FixedCode =
                    """
type C = | C
        
let myDiscardedArgFunc(C) = ()
"""
            }

    let actual = codeFix |> tryFix code (WithOption "--langversion:preview")

    Assert.Equal(expected, actual)

[<Fact>]
let ``Fixes FS3548 - marker types`` () =
    let code =
        """
type Type = Type

let f x =
    match x with
    | Type _ -> ()
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
