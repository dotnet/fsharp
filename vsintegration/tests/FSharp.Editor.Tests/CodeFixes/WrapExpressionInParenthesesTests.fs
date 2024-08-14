// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.FSharpWrapExpressionInParenthesesFixProviderTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = WrapExpressionInParenthesesCodeFixProvider()
let private diagnostic = 0597 // ... arguments involving function or method applications should be parenthesized

// Test case is taken from the original PR:
// https://github.com/dotnet/fsharp/pull/10460

[<Fact>]
let ``Fixes FS0597`` () =
    let code =
        """
let rng = System.Random()

printfn "Hello %d" rng.Next(5)
"""

    let expected =
        Some
            {
                Message = "Wrap expression in parentheses"
                FixedCode =
                    """
let rng = System.Random()

printfn "Hello %d" (rng.Next(5))
"""
            }

    let actual = codeFix |> tryFix code diagnostic

    Assert.Equal(expected, actual)
