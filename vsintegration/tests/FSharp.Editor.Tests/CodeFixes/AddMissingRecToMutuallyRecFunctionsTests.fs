// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.AddMissingRecToMutuallyRecFunctionsTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = FSharpAddMissingRecToMutuallyRecFunctionsCodeFixProvider()
let private diagnostic = 0576 // The declaration form 'let ... and ...' for non-recursive bindings is not used in F# code...

// TODO: write some negative test cases here

[<Fact>]
let ``Fixes FS0576`` () =
    let code =
        """
let isEven n =
    match n with
    | 0 -> true
    | _ -> isOdd (n - 1)

and isOdd n =
    match n with
    | 0 -> false
    | _ -> isEven (n - 1)
"""

    let expected =
        Some
            {
                Message = "Make 'isEven' recursive"
                FixedCode =
                    """
let rec isEven n =
    match n with
    | 0 -> true
    | _ -> isOdd (n - 1)

and isOdd n =
    match n with
    | 0 -> false
    | _ -> isEven (n - 1)
"""
            }

    let actual = codeFix |> tryFix code diagnostic

    Assert.Equal(expected, actual)
