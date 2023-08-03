// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.UseTripleQuotedInterpolationTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = UseTripleQuotedInterpolationCodeFixProvider()

[<Fact>]
let ``Fixes FS3373`` () =
    let code =
        """
let pluralize n word = if n = 1 then word else $"{word}s"
let createMsg x = $"Review in {x} {pluralize x "day"}"
"""

    let expected =
        Some
            {
                Message = "Use triple quoted string interpolation."
                FixedCode =
                    "\r\n"
                    + "let pluralize n word = if n = 1 then word else $\"{word}s\"\r\n"
                    + "let createMsg x = $\"\"\"Review in {x} {pluralize x \"day\"}\"\"\""
                    + "\r\n"
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)
