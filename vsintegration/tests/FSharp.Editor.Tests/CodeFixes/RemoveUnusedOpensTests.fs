// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.RemoveUnusedOpensTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = RemoveUnusedOpensCodeFixProvider()

[<Fact>]
let ``Fixes IDE0005 - one unused declaration`` () =
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

[<Theory>]
[<InlineData "open System.Buffers">]
[<InlineData "open System.IO">]
[<InlineData "open System.Text">]
let ``Fixes IDE0005 - multiple unused declarations`` focus =
    let code =
        """
open System.Buffers
open System.IO
open System.Text
"""

    let expected =
        Some
            {
                Message = "Remove unused open declarations"
                FixedCode =
                    """
"""
            }

    let actual = codeFix |> tryFix code (Manual(focus, "IDE0005"))

    Assert.Equal(expected, actual)
