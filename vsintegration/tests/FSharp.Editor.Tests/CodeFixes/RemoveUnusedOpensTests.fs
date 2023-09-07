// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.RemoveUnusedOpensTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = RemoveUnusedOpensCodeFixProvider()

[<Fact>]
let ``Fixes IDE0005`` () =
    let code =
        """
open System
"""

    let expected =
        Some
            {
                Message = "Remove unused open declarations"
                FixedCode =
                    """
"""
            }

    let actual = codeFix |> tryFix code (Manual("open System", "IDE0005"))

    Assert.Equal(expected, actual)
