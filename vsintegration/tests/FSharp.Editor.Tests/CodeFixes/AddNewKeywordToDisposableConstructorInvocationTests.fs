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

[<Fact>]
let ``Fixes FS0760 — type app`` () =
    let code =
        """
let _ = System.Threading.Tasks.Task<int>(fun _ -> 3)
"""

    let expected =
        Some
            {
                Message = "Add 'new' keyword"
                FixedCode =
                    """
let _ = new System.Threading.Tasks.Task<int>(fun _ -> 3)
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Fixes FS0760 — keeps space`` () =
    let code =
        """
let stream = System.IO.MemoryStream ()
"""

    let expected =
        Some
            {
                Message = "Add 'new' keyword"
                FixedCode =
                    """
let stream = new System.IO.MemoryStream ()
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Fixes FS0760 — does not add space`` () =
    let code =
        """
let stream = System.IO.MemoryStream()
"""

    let expected =
        Some
            {
                Message = "Add 'new' keyword"
                FixedCode =
                    """
let stream = new System.IO.MemoryStream()
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Fixes FS0760 — adds parentheses when needed`` () =
    let code =
        """
let path = "test.txt"
let sr = System.IO.StreamReader path
"""

    let expected =
        Some
            {
                Message = "Add 'new' keyword"
                FixedCode =
                    """
let path = "test.txt"
let sr = new System.IO.StreamReader(path)
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)

[<Fact>]
let ``Fixes FS0760 — adds parentheses when needed and keeps indentation`` () =
    let code =
        """
let path = "test.txt"
let sr =
    System.IO.StreamReader
        path
"""

    let expected =
        Some
            {
                Message = "Add 'new' keyword"
                FixedCode =
                    """
let path = "test.txt"
let sr =
    new System.IO.StreamReader
        (path)
"""
            }

    let actual = codeFix |> tryFix code Auto

    Assert.Equal(expected, actual)
