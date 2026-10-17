// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.AddOpenOnTopSharedTests

open Microsoft.VisualStudio.FSharp.Editor
open Xunit

open CodeFixTestFramework

let private codeFix = AddOpenCodeFixProvider(AssemblyContentProvider())

let scenarios (cases: (string * string * string) list) : obj[] seq =
    cases
    |> Seq.map (fun (name, code, fixedCode) -> [| box name; box code; box fixedCode |])

let expectOpenSystem mode code fixedCode =
    let expected =
        Some
            {
                Message = "open System"
                FixedCode = fixedCode
            }

    Assert.Equal(expected, codeFix |> tryFix code mode)

[<Theory>]
[<InlineData(true)>]
[<InlineData(false)>]
let ``Fixes FS0039 for missing opens - module has an attribute on the same line`` (alwaysPlaceOpensAtTopLevel: bool) =
    let mode =
        WithSettings
            { CodeFixesOptions.Default with
                AlwaysPlaceOpensAtTopLevel = alwaysPlaceOpensAtTopLevel
            }

    expectOpenSystem
        mode
        "[<AutoOpen>] module Module1\n\nConsole.WriteLine 42\n"
        "[<AutoOpen>] module Module1\n\nopen System\n\nConsole.WriteLine 42\n"
