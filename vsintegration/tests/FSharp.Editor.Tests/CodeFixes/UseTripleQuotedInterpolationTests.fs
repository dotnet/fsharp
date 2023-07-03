// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.UseTripleQuotedInterpolationTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = UseTripleQuotedInterpolationCodeFixProvider()
let private diagnostic = 3373 // ... invalid interpolated string ...

[<Fact>]
let ``Fixes FS3373`` () =
    let code =
        """
let answer = $"{"42"}"
"""

    let expected =
        Some
            {
                Message = "Use triple quoted string interpolation."
                // yeah... because I can't understand how to
                // put a triple-quoted string inside a triple-quoted string
                // but this is what we need
                FixedCode =
                    """
let answer = $"""
                    + "\"\"\""
                    + """{"42"}"""
                    + "\"\"\""
                    + """
"""
            }

    let actual = codeFix |> tryFix code diagnostic

    Assert.Equal(expected, actual)
