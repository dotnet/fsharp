// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.AddMissingSeqTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit
open CodeFixTestFramework

let private codeFix = AddMissingSeqCodeFixProvider()

// This can be changed to Auto when featureDeprecatePlacesWhereSeqCanBeOmitted is out of preview.
let mode = WithOption "--langversion:preview"

[<Fact>]
let ``FS3873 — Adds missing seq before { start..finish }`` () =
    let code = "let xs = { 1..10 }"

    let expected =
        Some
            {
                Message = "Add missing 'seq'"
                FixedCode = "let xs = seq { 1..10 }"
            }

    let actual = codeFix |> tryFix code mode

    Assert.Equal(expected, actual)

[<Fact>]
let ``FS0740 — Adds missing seq before { x; y }`` () =
    let code = "let xs = { 1; 10 }"

    let expected =
        Some
            {
                Message = "Add missing 'seq'"
                FixedCode = "let xs = seq { 1; 10 }"
            }

    let actual = codeFix |> tryFix code mode

    Assert.Equal(expected, actual)

[<Fact>]
let ``FS3873 — Adds parens when needed`` () =
    let code = "let xs = id { 1..10 }"

    let expected =
        Some
            {
                Message = "Add missing 'seq'"
                FixedCode = "let xs = id (seq { 1..10 })"
            }

    let actual = codeFix |> tryFix code mode

    Assert.Equal(expected, actual)

[<Fact>]
let ``FS0740 — Adds parens when needed`` () =
    let code = "let xs = id { 1; 10 }"

    let expected =
        Some
            {
                Message = "Add missing 'seq'"
                FixedCode = "let xs = id (seq { 1; 10 })"
            }

    let actual = codeFix |> tryFix code mode

    Assert.Equal(expected, actual)

[<Fact>]
let ``FS3873 — Adds parens when needed — multiline`` () =
    let code =
        """
let xs =
    id {
        1..10
    }
"""

    let expected =
        Some
            {
                Message = "Add missing 'seq'"
                FixedCode =
                    """
let xs =
    id (seq {
        1..10
    })
"""
            }

    let actual = codeFix |> tryFix code mode

    Assert.Equal(expected, actual)

[<Fact>]
let ``FS0740 — Adds parens when needed — multiline`` () =
    let code =
        """
let xs =
    id {
        1; 10
    }
"""

    let expected =
        Some
            {
                Message = "Add missing 'seq'"
                FixedCode =
                    """
let xs =
    id (seq {
        1; 10
    })
"""
            }

    let actual = codeFix |> tryFix code mode

    Assert.Equal(expected, actual)
