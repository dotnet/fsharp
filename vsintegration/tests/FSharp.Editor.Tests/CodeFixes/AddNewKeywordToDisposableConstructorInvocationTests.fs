// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.AddNewKeywordToDisposableConstructorInvocationTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = AddNewKeywordCodeFixProvider()

[<Fact>]
let ``Fixes FS0760`` () =
    let code =
        """
let sr = System.IO.StreamReader "test.txt"
"""

    let expected =
        Some
            {
                Message = "Add 'new' keyword"
                FixedCode =
                    """
let sr = new System.IO.StreamReader "test.txt"
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)
