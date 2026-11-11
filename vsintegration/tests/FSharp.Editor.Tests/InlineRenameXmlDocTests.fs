// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.InlineRenameXmlDocTests

open System
open System.Threading
open Xunit
open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Editor.Tests.Helpers

let private renamedSource (fileContents: string) (caretAt: string) (newName: string) =
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

    Assert.True replacements.ReplacementTextValid

    replacements.NewSolution.GetDocument(document.Id).GetTextAsync(CancellationToken.None).Result.ToString()

[<Theory>]
[<InlineData("""
/// <summary>Adds <paramref name="x"/> to one.</summary>
/// <param name="x">The addend.</param>
let addOne x = x + 1
""",
             "x = x + 1",
             "count",
             """
/// <summary>Adds <paramref name="count"/> to one.</summary>
/// <param name="count">The addend.</param>
let addOne count = count + 1
""",
             Label = "param and paramref of a function follow the parameter")>]
[<InlineData("""
/// <summary>Wraps a <typeparamref name="T"/>.</summary>
/// <typeparam name="T">The element type.</typeparam>
let wrap<'T> (x: 'T) = [ x ]
""",
             "T> (x",
             "'U",
             """
/// <summary>Wraps a <typeparamref name="U"/>.</summary>
/// <typeparam name="U">The element type.</typeparam>
let wrap<'U> (x: 'U) = [ x ]
""",
             Label = "typeparam and typeparamref get the name without its tick")>]
[<InlineData("""
/// <param name="x">The addend.</param>
let addOne x = x + 1
""",
             "x = x + 1",
             "``the addend``",
             """
/// <param name="the addend">The addend.</param>
let addOne ``the addend`` = ``the addend`` + 1
""",
             Label = "a backticked name goes into the tag without its backticks")>]
[<InlineData("""
type C() =
    /// <param name="x">The input.</param>
    member this.M x = this.GetHashCode() + x
""",
             "x = this",
             "input",
             """
type C() =
    /// <param name="input">The input.</param>
    member this.M input = this.GetHashCode() + input
""",
             Label = "param of a member follows the parameter and leaves this alone")>]
[<InlineData("""
module M

type Shape =
    /// <summary>A circle.</summary>
    /// <param name="radius">The radius.</param>
    | Circle of radius: float

let unitCircle = Circle(radius = 1.0)
""",
             "radius: float",
             "size",
             """
module M

type Shape =
    /// <summary>A circle.</summary>
    /// <param name="size">The radius.</param>
    | Circle of size: float

let unitCircle = Circle(size = 1.0)
""",
             Label = "param of a public union case field follows the field")>]
[<InlineData("""
/// <summary>Holds a value.</summary>
/// <param name="value">The held value.</param>
type Holder(value: int) =
    member _.Value = value
""",
             "value: int",
             "item",
             """
/// <summary>Holds a value.</summary>
/// <param name="item">The held value.</param>
type Holder(item: int) =
    member _.Value = item
""",
             Label = "param of a primary constructor documented on the type follows the parameter")>]
let ``Renaming a symbol renames the names its doc tags hold`` (source: string, caretAt: string, newName: string, expected: string) =
    Assert.Equal(expected, renamedSource source caretAt newName)
