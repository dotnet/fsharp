// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.RenameParamToMatchSignatureTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = RenameParamToMatchSignatureCodeFixProvider()

[<Fact>]
let ``Fixes FS3218`` () =
    let fsCode =
        """
module Library

let id y = y
"""

    let fsiCode =
        """
module Library

val id: x: 'a -> 'a
"""

    let expected =
        Some
            {
                Message = "Replace with 'x'"
                FixedCode =
                    """
module Library

let id x = x
"""
            }

    let actual = codeFix |> tryFix fsCode (WithSignature fsiCode)

    Assert.Equal(expected, actual)
