// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.ConvertToAnonymousRecordTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = ConvertToAnonymousRecordCodeFixProvider()

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

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Fixes FS3578 for anon records`` () =
    let code =
        """
let t3 (t1: {| gu: string; ff: int |}) = { t1 with ff = 3 }
"""

    let expected =
        Some
            {
                Message = "Convert to Anonymous Record"
                FixedCode =
                    """
let t3 (t1: {| gu: string; ff: int |}) = {| t1 with ff = 3 |}
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Fixes FS3578 for struct anon records`` () =
    let code =
        """
let t3 (t1: struct {| gu: string; ff: int |}) = { t1 with ff = 3 }
"""

    let expected =
        Some
            {
                Message = "Convert to Anonymous Record"
                FixedCode =
                    """
let t3 (t1: struct {| gu: string; ff: int |}) = {| t1 with ff = 3 |}
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Fixes FS3578 for anon records with multiple fields`` () =
    let code =
        """
let f (r: {| A: int; C: int |}) = { r with A = 1; B = 2; C = 3 }
"""

    let expected =
        Some
            {
                Message = "Convert to Anonymous Record"
                FixedCode =
                    """
let f (r: {| A: int; C: int |}) = {| r with A = 1; B = 2; C = 3 |}
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Doesn't fix FS0039 for random undefined identifiers`` () =
    let code =
        """
let x = someUndefinedFunction 42
"""

    let expected = None

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)
