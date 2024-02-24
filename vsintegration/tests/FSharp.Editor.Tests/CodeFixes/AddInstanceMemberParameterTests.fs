// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.AddInstanceMemberParameterTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = AddInstanceMemberParameterCodeFixProvider()

[<Fact>]
let ``Fixes FS0673`` () =
    let code =
        """
type UsefulTestHarness() =
    member FortyTwo = 42
"""

    let expected =
        Some
            {
                Message = "Add missing instance member parameter"
                FixedCode =
                    """
type UsefulTestHarness() =
    member x.FortyTwo = 42
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)
