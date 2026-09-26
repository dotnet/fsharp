// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

module InlineRenameXmlDocTests =

    open System
    open System.Threading
    open Xunit
    open Microsoft.VisualStudio.FSharp.Editor
    open FSharp.Editor.Tests.Helpers

    /// Renames the symbol under the caret the way Visual Studio does and returns the rewritten source.
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

        Assert.True replacements.ReplacementTextValid

        replacements.NewSolution.GetDocument(document.Id).GetTextAsync(CancellationToken.None).Result.ToString()

    [<Fact>]
    let ``param and paramref of a function follow the parameter`` () =
        let source =
            """
/// <summary>Adds <paramref name="x"/> to one.</summary>
/// <param name="x">The addend.</param>
let addOne x = x + 1
"""

        let expected =
            """
/// <summary>Adds <paramref name="count"/> to one.</summary>
/// <param name="count">The addend.</param>
let addOne count = count + 1
"""

        Assert.Equal(expected, rename source "x = x + 1" "count")

    [<Fact>]
    let ``typeparam and typeparamref get the name without its tick`` () =
        let source =
            """
/// <summary>Wraps a <typeparamref name="T"/>.</summary>
/// <typeparam name="T">The element type.</typeparam>
let wrap<'T> (x: 'T) = [ x ]
"""

        let expected =
            """
/// <summary>Wraps a <typeparamref name="U"/>.</summary>
/// <typeparam name="U">The element type.</typeparam>
let wrap<'U> (x: 'U) = [ x ]
"""

        Assert.Equal(expected, rename source "T> (x" "'U")

    [<Fact>]
    let ``a backticked name goes into the tag without its backticks`` () =
        let source =
            """
/// <param name="x">The addend.</param>
let addOne x = x + 1
"""

        let expected =
            """
/// <param name="the addend">The addend.</param>
let addOne ``the addend`` = ``the addend`` + 1
"""

        Assert.Equal(expected, rename source "x = x + 1" "``the addend``")

    [<Fact>]
    let ``param of a member follows the parameter and leaves this alone`` () =
        let source =
            """
type C() =
    /// <param name="x">The input.</param>
    member this.M x = this.GetHashCode() + x
"""

        let expected =
            """
type C() =
    /// <param name="input">The input.</param>
    member this.M input = this.GetHashCode() + input
"""

        Assert.Equal(expected, rename source "x = this" "input")

    [<Fact>]
    let ``param of a primary constructor documented on the type follows the parameter`` () =
        let source =
            """
/// <summary>Holds a value.</summary>
/// <param name="value">The held value.</param>
type Holder(value: int) =
    member _.Value = value
"""

        let expected =
            """
/// <summary>Holds a value.</summary>
/// <param name="item">The held value.</param>
type Holder(item: int) =
    member _.Value = item
"""

        Assert.Equal(expected, rename source "value: int" "item")
