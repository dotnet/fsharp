// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.AddTypeAnnotationToObjectOfIndeterminateTypeTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = AddTypeAnnotationToObjectOfIndeterminateTypeFixProvider()

[<Theory>]
[<InlineData("", "")>]
[<InlineData("(", ")")>]
let ``Fixes FS0072`` leftParen rightParen =
    let code =
        $"""
let db = 
    [
        {{| Name = "Liam"; Id = 2 |}}
        {{| Name = "Noel"; Id = 3 |}}
    ]

let f = List.filter (fun {leftParen}x{rightParen} -> x.Id = 7) db
"""

    let expected =
        Some
            {
                Message = "Add type annotation"
                FixedCode =
                    """
let db = 
    [
        {| Name = "Liam"; Id = 2 |}
        {| Name = "Noel"; Id = 3 |}
    ]

let f = List.filter (fun (x: {| Id: int; Name: string |}) -> x.Id = 7) db
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Doesn't fix FS0072 for generic inferred types`` () =
    let code =
        """
let f x =
    x.IsSpecial
"""

    let expected = None

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Theory>]
[<InlineData("", "")>]
[<InlineData("(", ")")>]
let ``Fixes FS3245`` leftParen rightParen =
    let code =
        $"""
let count numbers =
    numbers
    |> List.fold 
        (fun {leftParen}s{rightParen} _ -> {{| s with Count = s.Count + 1 |}})
        {{| Count = 0 |}}
"""

    let expected =
        Some
            {
                Message = "Add type annotation"
                FixedCode =
                    """
let count numbers =
    numbers
    |> List.fold 
        (fun (s: {| Count: int |}) _ -> {| s with Count = s.Count + 1 |})
        {| Count = 0 |}
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)
