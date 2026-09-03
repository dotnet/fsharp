// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open System.Threading

open Xunit

open Microsoft.VisualStudio.Copilot
open Microsoft.VisualStudio.FSharp.Editor

open FSharp.Editor.Tests.Helpers
open CancellableTasks

module CopilotContextProviderTests =

    let fileContents =
        """
module Widgets

/// Counts things that matter.
type Counter(start: int) =
    let mutable value = start

    member _.Value = value

    member _.Bump() =
        value <- value + 1
        value

type Shape =
    | Circle of radius: float
    | Square of side: float

let describeShape shape =
    match shape with
    | Circle r -> $"circle {r}"
    | Square s -> $"square {s}"

/// Twice the value.
let twice x = x * 2
"""

    let solution = RoslynTestHelpers.CreateSolution fileContents

    let private cache =
        MefHelpers.createExportProvider().GetExportedValue<FSharpNavigableItemsCache>()

    let private run computation =
        computation |> CancellableTask.start CancellationToken.None |> _.Result

    let private search pattern =
        CopilotSymbolQuery.search cache solution pattern
        |> run
        |> Array.map (fun (struct (item, _)) -> CopilotSymbolMapping.fullyQualifiedName item)

    let private symbolContext name =
        CopilotSymbolQuery.symbolContext cache solution name |> run

    let private contextOf name =
        match symbolContext name with
        | ValueSome context -> context
        | ValueNone -> failwith $"expected a symbol context for {name}"

    [<Theory>]
    [<InlineData("Counter", "Widgets.Counter")>]
    [<InlineData("Bump", "Widgets.Counter.Bump")>]
    [<InlineData("Circle", "Widgets.Shape.Circle")>]
    [<InlineData("describeShape", "Widgets.describeShape")>]
    let ``search finds a declaration by its fully qualified name`` (pattern: string, expected: string) =
        Assert.Contains(expected, search pattern)

    [<Fact>]
    let ``search reports each declaration once`` () =
        let names = search "Counter"
        Assert.Equal((Array.distinct names).Length, names.Length)

    [<Fact>]
    let ``an unknown name has no context`` () =
        Assert.True((symbolContext "Widgets.NoSuchThing").IsNone)

    [<Fact>]
    let ``a type context carries the whole declaration and its doc comment`` () =
        let context = contextOf "Widgets.Counter"

        Assert.Equal("Widgets.Counter", context.FullyQualifiedName)
        Assert.Equal("Counter", context.UnqualifiedName)
        Assert.Contains("Counts things that matter.", context.Snippet)
        Assert.Contains("member _.Bump()", context.Snippet)

    [<Fact>]
    let ``a member context carries the member body alone`` () =
        let context = contextOf "Widgets.Counter.Bump"

        Assert.Contains("value <- value + 1", context.Snippet)
        Assert.DoesNotContain("type Counter", context.Snippet)

    [<Fact>]
    let ``a one-line declaration keeps its doc comment`` () =
        let context = contextOf "Widgets.twice"

        Assert.Contains("Twice the value.", context.Snippet)
        Assert.Contains("let twice x", context.Snippet)
        Assert.DoesNotContain("describeShape", context.Snippet)

    [<Theory>]
    [<InlineData("Widgets.Counter", CopilotSymbolContextType.Class)>]
    [<InlineData("Widgets.Counter.Bump", CopilotSymbolContextType.Method)>]
    [<InlineData("Widgets.Counter.Value", CopilotSymbolContextType.Method)>]
    [<InlineData("Widgets.Shape.Circle", CopilotSymbolContextType.Union)>]
    [<InlineData("Widgets.describeShape", CopilotSymbolContextType.Function)>]
    let ``declaration kinds map onto Copilot symbol types`` (name: string, expected: CopilotSymbolContextType) =
        Assert.Equal(expected, (contextOf name).SymbolType)

    [<Fact>]
    let ``a context points back at the source it was taken from`` () =
        let context = contextOf "Widgets.Counter"
        let location = Assert.Single<SnippetLocation> context.SnippetLocations
        let document = solution.Projects |> Seq.exactlyOne |> _.Documents |> Seq.exactlyOne

        Assert.Equal(document.FilePath, location.FilePath)
        Assert.Equal(context.Snippet.Length, location.Span.Length)
