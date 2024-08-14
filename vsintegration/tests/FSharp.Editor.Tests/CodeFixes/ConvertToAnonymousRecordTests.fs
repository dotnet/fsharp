// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.ConvertToAnonymousRecordTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = ConvertToAnonymousRecordCodeFixProvider()
let private diagnostic = 0039 // ... is not defined...

[<Fact>]
let ``Fixes FS0039 for records`` () =
    let code =
        """
let band = { Name = "The Velvet Underground" }
"""

    let expected =
        Some
            {
                Message = "Convert to Anonymous Record"
                FixedCode =
                    """
let band = {| Name = "The Velvet Underground" |}
"""
            }

    let actual = codeFix |> tryFix code diagnostic

    Assert.Equal(expected, actual)

[<Fact>]
let ``Doesn't fix FS0039 for random undefined identifiers`` () =
    let code =
        """
let x = someUndefinedFunction 42
"""

    let expected = None

    let actual = codeFix |> tryFix code diagnostic

    Assert.Equal(expected, actual)
