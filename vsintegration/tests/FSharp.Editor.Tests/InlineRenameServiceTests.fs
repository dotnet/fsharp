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
    [<InlineData "_">]
    [<InlineData "``_``">]
    let ``Renaming a self identifier that is used to _ is rejected`` (newName: string) =
        let locations, isValid = rename selfIdentifierWithUse "this.TestMethodThis" newName
        Assert.Equal(2, locations)
        Assert.False isValid

    [<Fact>]
    let ``Renaming a self identifier that is not used to _ is accepted`` () =
        let locations, isValid = rename selfIdentifierWithoutUse "this.TestMethod" "_"
        Assert.Equal(1, locations)
        Assert.True isValid

    [<Fact>]
    let ``Renaming a self identifier that is used to an ordinary name is accepted`` () =
        let locations, isValid = rename selfIdentifierWithUse "this.TestMethodThis" "self"
        Assert.Equal(2, locations)
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
