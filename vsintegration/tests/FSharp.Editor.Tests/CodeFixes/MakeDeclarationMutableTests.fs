// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.MakeDeclarationMutableTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = MakeDeclarationMutableCodeFixProvider()

[<Fact>]
let ``Fixes FS0027 for let bindings`` () =
    let code =
        """
let x = 42
x <- 43
"""

    let expected =
        Some
            {
                Message = "Make declaration 'mutable'"
                FixedCode =
                    """
let mutable x = 42
x <- 43
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Doesn't fix FS0027 for parameters`` () =
    let code =
        """
let f x =
    x <- 42
"""

    let expected = None

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)
