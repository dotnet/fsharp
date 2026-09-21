// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

module InlineRenameServiceTests =

    open System
    open System.Threading
    open Xunit
    open Microsoft.VisualStudio.FSharp.Editor
    open FSharp.Editor.Tests.Helpers

    /// Drives the rename the way Visual Studio does: resolve the symbol under the caret,
    /// collect what would be rewritten, then ask whether the new name is accepted.
    let private rename (fileContents: string) (caretAt: string) (newName: string) =
        let document =
            RoslynTestHelpers.CreateSolution(fileContents)
            |> RoslynTestHelpers.GetSingleDocument

        let position = fileContents.IndexOf(caretAt, StringComparison.Ordinal)
        Assert.True(position >= 0, $"'{caretAt}' is not in the test source")

        let info =
            InlineRenameService().GetRenameInfoAsync(document, position, CancellationToken.None).Result

        Assert.NotNull info

        let locationSet =
            info.FindRenameLocationsAsync(Unchecked.defaultof<_>, Unchecked.defaultof<_>, CancellationToken.None).Result

        let replacements =
            locationSet.GetReplacementsAsync(newName, CancellationToken.None).Result

        locationSet.Locations.Count, replacements.ReplacementTextValid

    let private selfIdentifierWithUse =
        """
type RenameTest() =
    member this.TestMethod() = "Hello, World!"
    member this.TestMethodThis() = this.TestMethod()
"""

    let private selfIdentifierWithoutUse =
        """
type RenameTest() =
    member this.TestMethod() = "Hello, World!"
"""

    [<Theory>]
    [<InlineData("_", false)>]
    [<InlineData("``_``", true)>]
    [<InlineData("self", true)>]
    let ``A self identifier that is used is renamed only to a referenceable name`` (newName: string, isAccepted: bool) =
        let locations, isValid = rename selfIdentifierWithUse "this.TestMethodThis" newName
        Assert.Equal(2, locations)
        Assert.Equal(isAccepted, isValid)

    [<Fact>]
    let ``Renaming a self identifier that is not used to _ is accepted`` () =
        let locations, isValid = rename selfIdentifierWithoutUse "this.TestMethod" "_"
        Assert.Equal(1, locations)
        Assert.True isValid

    [<Theory>]
    [<InlineData "Even|Odd">]
    [<InlineData "Even ->">]
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

        let locations, isValid = rename source caretAt "DivisibleByTwo"
        Assert.True(locations >= 3, $"Expected the declaration and both uses, got {locations} locations")
        Assert.True isValid

    [<Fact>]
    let ``A self identifier named __ renames its declaration together with its uses`` () =
        let source =
            """
type RenameTest() =
    member this.TestMethod() = "Hello, World!"
    member __.TestMethodDoubleUnderscore() = __.TestMethod()
"""

        let fromDeclaration, _ = rename source "__.TestMethodDoubleUnderscore" "this"
        Assert.Equal(2, fromDeclaration)

        let fromUse, _ = rename source "__.TestMethod()" "this"
        Assert.Equal(2, fromUse)
