// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.ChangeEqualsInFieldTypeToColonTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix : IFSharpCodeFixProvider = failwith "" // AddInstanceMemberParameterCodeFixProvider()
let private diagnostic = 0010 // Unexpected symbol...

[<Fact>]
let ``Fixes FS0010`` () =
    let code = 
        """
type Band = { Name = string }
"""

    let expected = 
        Some 
            {
                Message = "Use ':' for type in field declaration"
                FixedCode = """
type Band = { Name : string }
"""
            }

    let actual = codeFix |> tryFix code diagnostic

    Assert.Equal(expected, actual)
