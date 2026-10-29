// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.InlineRenameServiceTests

open System
open System.Threading
open Xunit
open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Editor.Tests.Helpers

let private renameUnderCaret (fileContents: string) (caretAt: string) (newName: string) =
    let document =
        RoslynTestHelpers.CreateSolution(fileContents)
        |> RoslynTestHelpers.GetSingleDocument

    let position = fileContents.IndexOf(caretAt, StringComparison.Ordinal)
    Assert.True(position >= 0, $"'{caretAt}' is not in the test source")

    let info =
        InlineRenameService().GetRenameInfoAsync(document, position, CancellationToken.None).Result

    Assert.NotNull info

    let locationSet =
        info.FindRenameLocationsAsync(false, false, CancellationToken.None).Result

    let replacements =
        locationSet.GetReplacementsAsync(newName, CancellationToken.None).Result

    locationSet.Locations.Count, replacements.ReplacementTextValid

[<Theory>]
[<InlineData("this.TestMethodThis", "_", 2, false, Label = "used, renamed to _")>]
[<InlineData("this.TestMethodThis", "``_``", 2, true, Label = "used, renamed to escaped _")>]
[<InlineData("this.TestMethodThis", "self", 2, true, Label = "used, renamed to another name")>]
[<InlineData("this.TestMethod", "_", 1, true, Label = "unused, renamed to _")>]
[<InlineData("__.TestMethodDoubleUnderscore", "this", 2, true, Label = "__, caret on its declaration")>]
[<InlineData("__.TestMethod()", "this", 2, true, Label = "__, caret on a use")>]
let ``Renaming a self identifier renames its uses and accepts _ only when there are none``
    (caretAt: string, newName: string, expectedLocations: int, isAccepted: bool)
    =
    let source =
        """
type RenameTest() =
    member this.TestMethod() = "Hello, World!"
    member this.TestMethodThis() = this.TestMethod()
    member __.TestMethodDoubleUnderscore() = __.TestMethod()
"""

    Assert.Equal((expectedLocations, isAccepted), renameUnderCaret source caretAt newName)

[<Theory>]
[<InlineData("Even|Odd", Label = "caret on the declaration")>]
[<InlineData("Even ->", Label = "caret on a use")>]
let ``An active pattern case is renamed from its declaration and from its uses`` (caretAt: string) =
    let source =
        """
module M

let (|Even|Odd|) n = if n % 2 = 0 then Even else Odd

let f n =
    match n with
    | Even -> true
    | Odd -> false
"""

    let locations, isValid = renameUnderCaret source caretAt "DivisibleByTwo"
    Assert.True(locations >= 3, $"Expected the declaration and both uses, got {locations} locations")
    Assert.True isValid
