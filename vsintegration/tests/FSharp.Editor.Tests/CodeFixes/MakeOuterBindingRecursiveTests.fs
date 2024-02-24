// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.MakeOuterBindingRecursiveTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = MakeOuterBindingRecursiveCodeFixProvider()

[<Fact>]
let ``Fixes FS0039 for recursive functions`` () =
    let code =
        """
let factorial n =
    match n with
    | 0 -> 1
    | _ -> n * factorial (n - 1)
"""

    let expected =
        Some
            {
                Message = "Make 'factorial' recursive"
                FixedCode =
                    """
let rec factorial n =
    match n with
    | 0 -> 1
    | _ -> n * factorial (n - 1)
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Doesn't fix FS0039 for random undefined stuff`` () =
    let code =
        """
let f = g
"""

    let expected = None

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)
