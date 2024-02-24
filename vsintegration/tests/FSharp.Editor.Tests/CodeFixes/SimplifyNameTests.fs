// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.SimplifyNameTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = SimplifyNameCodeFixProvider()

[<Fact>]
let ``Fixes IDE0002`` () =
    let code =
        """
open System

let now = System.DateTime.Now
"""

    let expected =
        Some
            {
                // In reality, the message would be "Simplify name: 'System.DateTime.Now'".
                // This diag is produced by an analyzer hence we have to create it manually.
                // To simplify things, the test framework does not currently
                // fill the property bag of the diag (which the code fix looks at).
                // If we find more similar cases, this can be improved.
                Message = "Simplify name"
                FixedCode =
                    """
open System

let now = DateTime.Now
"""
            }

    let actual = codeFix |> tryFix code (Manual("System.", "IDE0002"))

    Assert.Equal(expected, actual)
