// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.ConvertToAnonymousRecordTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = FSharpConvertToAnonymousRecordCodeFixProvider()
let private diagnostic = 0039 // The record label is not defined...

[<Fact>]
let ``Fixes FS0039`` () =
    let code =
        """
let band = { Name = "The Velvet Underground" }
"""

    let expected =
        {
            Title = "Convert to Anonymous Record"
            FixedCode =
                """
let band = {| Name = "The Velvet Underground" |}
"""
        }

    let actual = codeFix |> fix code diagnostic

    Assert.Equal(expected, actual)
