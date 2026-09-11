// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open System
open System.Threading

open Xunit

open Microsoft.CodeAnalysis
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

    let private namesOf hits =
        hits
        |> Array.map (fun (struct (item, _, _)) -> CopilotSymbolMapping.fullyQualifiedName item)

    let private searchFocused cache openDocumentIds focus solution pattern =
        CopilotSymbolQuery.search cache openDocumentIds focus solution [| pattern |]
        |> run
        |> Array.head
        |> namesOf

    let private searchIn cache openDocumentIds solution pattern =
        searchFocused cache openDocumentIds ValueNone solution pattern

    let private search pattern =
        searchIn cache Seq.empty solution pattern

    let private itemNamed (fullyQualifiedName: string) =
        CopilotSymbolQuery.search cache Seq.empty ValueNone solution [| fullyQualifiedName |]
        |> run
        |> Array.head
        |> Array.pick (fun (struct (item, _, _)) ->
            if CopilotSymbolMapping.fullyQualifiedName item = fullyQualifiedName then
                Some item
            else
                None)

    let private symbolContext name =
        CopilotSymbolQuery.symbolContext cache Seq.empty solution name |> run

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

    [<Theory>]
    [<InlineData("Widgets.Counter", true)>]
    [<InlineData("Widgets.Counte", false)>]
    [<InlineData("Widgets.CounterX", false)>]
    [<InlineData("WidgetsXCounter", false)>]
    [<InlineData("Counter", false)>]
    [<InlineData("", false)>]
    let ``a name matches only the declaration it spells out`` (candidate: string, expected: bool) =
        Assert.Equal(expected, CopilotSymbolMapping.hasFullyQualifiedName candidate (itemNamed "Widgets.Counter"))

    [<Theory>]
    [<InlineData("Widgets.Counter", "Counter")>]
    [<InlineData("Widgets.Counter.Bump", "Counter.Bump")>]
    [<InlineData("Widgets.Shape.Circle", "Shape.Circle")>]
    [<InlineData("Widgets.describeShape", "Widgets.describeShape")>]
    let ``a tooltip names a member by its container and a type by itself`` (fullyQualifiedName: string, expected: string) =
        Assert.Equal(expected, CopilotSymbolMapping.tooltipName (itemNamed fullyQualifiedName))

    [<Fact>]
    let ``search reports each declaration once`` () =
        let names = search "Counter"
        Assert.Equal((Array.distinct names).Length, names.Length)

    [<Fact>]
    let ``an unknown name has no context`` () =
        Assert.True((symbolContext "Widgets.NoSuchThing").IsNone)

    /// One matcher and one parse cache per test, so what a test leaves parsed cannot answer the next one.
    let private freshCache () =
        MefHelpers.createExportProvider().GetExportedValue<FSharpNavigableItemsCache>()

    let private solutionOf files =
        let projectId = ProjectId.CreateNewId()

        let documents =
            files
            |> List.map (fun (path, source) -> RoslynTestHelpers.CreateDocumentInfo projectId path source)

        let solution =
            RoslynTestHelpers.CreateSolution [ RoslynTestHelpers.CreateProjectInfo projectId "C:\\many.fsproj" documents ]

        { RoslynTestHelpers.DefaultProjectOptions with
            SourceFiles = files |> List.map fst |> Array.ofList
        }
        |> RoslynTestHelpers.SetProjectOptions projectId solution

        solution

    let private documentsOf (solution: Solution) =
        solution.Projects |> Seq.exactlyOne |> _.Documents |> Seq.toArray

    let private documentNamed (name: string) solution =
        documentsOf solution
        |> Array.find _.FilePath.EndsWith(name, StringComparison.Ordinal)

    /// A file holding more declarations matching `name` than one query reports.
    let private manyDeclarations name count =
        let members =
            [ for i in 1..count -> $"    member _.{name}{i} = {i}" ] |> String.concat "\n"

        $"module {name}Module\n\ntype {name}Holder() =\n{members}\n"

    let private coldFile = "C:\\cold.fs", "module Cold\n\nlet widgetCounter = 1\n"

    let private caretOn filePath line =
        ValueSome
            {
                FilePath = filePath
                FirstLine = line
                LastLine = line
            }

    [<Fact>]
    let ``an open document answers without parsing the rest of the solution`` () =
        let cache = freshCache ()
        let solution = solutionOf [ "C:\\open.fs", manyDeclarations "Widget" 25; coldFile ]
        let opened = documentNamed "open.fs" solution

        let names = searchIn cache [ opened.Id ] solution "Widget"

        Assert.Equal(20, names.Length)
        Assert.All(names, fun name -> Assert.StartsWith("WidgetModule", name, StringComparison.Ordinal))
        Assert.True((cache.TryGetCachedNavigableItems (documentNamed "cold.fs" solution).Id).IsNone)

    [<Fact>]
    let ``documents already parsed answer without parsing the rest`` () =
        let cache = freshCache ()
        let solution = solutionOf [ "C:\\warm.fs", manyDeclarations "Widget" 25; coldFile ]
        cache.GetNavigableItems(documentNamed "warm.fs" solution) |> run |> ignore

        let names = searchIn cache Seq.empty solution "Widget"

        Assert.Equal(20, names.Length)
        Assert.True((cache.TryGetCachedNavigableItems (documentNamed "cold.fs" solution).Id).IsNone)

    [<Fact>]
    let ``the search stops once it has enough declarations`` () =
        let cache = freshCache ()

        let solution =
            solutionOf
                [
                    for i in 1..150 -> $"C:\\cold{i}.fs", $"module Cold{i}\n\ntype Counter{i}() =\n    member _.Value = {i}\n"
                ]

        let names = searchIn cache Seq.empty solution "Counter"

        Assert.Equal(20, names.Length)

        Assert.NotEmpty(
            documentsOf solution
            |> Array.filter (fun document -> (cache.TryGetCachedNavigableItems document.Id).IsNone)
        )

    /// The declaration in the open file loses on every other part of the ordering - the name it is
    /// matched against is longer - so it can only come first by being the file the user has open.
    [<Theory>]
    [<InlineData(false, "Elsewhere.Widget")>]
    [<InlineData(true, "Holder.WidgetHolder")>]
    let ``an open file answers before the rest`` (holderIsOpen: bool) (expected: string) =
        let cache = freshCache ()

        let solution =
            solutionOf
                [
                    "C:\\elsewhere.fs", "module Elsewhere\n\ntype Widget() =\n    member _.Value = 1\n"
                    "C:\\holder.fs", "module Holder\n\ntype WidgetHolder() =\n    member _.Value = 2\n"
                ]

        let openDocumentIds =
            if holderIsOpen then
                [ (documentNamed "holder.fs" solution).Id ]
            else
                []

        let names = searchIn cache openDocumentIds solution "Widget"

        Assert.Equal(expected, Array.head names)
        Assert.Equal(2, names.Length)

    /// A bare "#" is the first thing the picker asks, and Copilot's own provider fills it from the open files.
    [<Fact>]
    let ``no text at all answers with the declarations of the open files`` () =
        let cache = freshCache ()
        let solution = solutionOf [ "C:\\open.fs", manyDeclarations "Widget" 3; coldFile ]

        let names =
            searchFocused cache [ (documentNamed "open.fs" solution).Id ] (caretOn "C:\\open.fs" 1) solution ""

        Assert.Contains("WidgetModule.WidgetHolder", names)
        Assert.All(names, fun name -> Assert.StartsWith("WidgetModule", name, StringComparison.Ordinal))
        Assert.True((cache.TryGetCachedNavigableItems (documentNamed "cold.fs" solution).Id).IsNone)

    [<Theory>]
    [<InlineData("wi")>]
    [<InlineData("widgetCounter")>]
    let ``a short text is looked up in the open files alone`` (pattern: string) =
        let cache = freshCache ()

        let solution =
            solutionOf [ "C:\\open.fs", "module Open\n\nlet other = 1\n"; coldFile ]

        let names =
            searchIn cache [ (documentNamed "open.fs" solution).Id ] solution pattern

        Assert.Equal(pattern.Length >= 3, Array.contains "Cold.widgetCounter" names)

    /// The focused file outranks the merely open one, which is how Copilot's own provider separates
    /// the tab being edited from the rest of the tabs.
    [<Fact>]
    let ``the focused file answers before the other open ones`` () =
        let cache = freshCache ()

        let solution =
            solutionOf
                [
                    "C:\\elsewhere.fs", "module Elsewhere\n\ntype Widget() =\n    member _.Value = 1\n"
                    "C:\\holder.fs", "module Holder\n\ntype WidgetHolder() =\n    member _.Value = 2\n"
                ]

        let openDocumentIds = documentsOf solution |> Array.map _.Id

        let names =
            searchFocused cache openDocumentIds (caretOn "C:\\holder.fs" 1) solution "Widget"

        Assert.Equal("Holder.WidgetHolder", Array.head names)

    /// The type around the caret loses on name length to the other one, so it can only come first by
    /// holding the caret - on a line of its member's body, not of its own name.
    [<Theory>]
    [<InlineData(4, "Selection.Short")>]
    [<InlineData(8, "Selection.AroundTheCaret")>]
    let ``the declaration around the caret answers before the rest of the focused file`` (caretLine: int) (expected: string) =
        let cache = freshCache ()

        let source =
            "module Selection\n\ntype Short() =\n    member _.Value = 1\n\ntype AroundTheCaret() =\n    member _.Compute() =\n        2\n"

        let solution = solutionOf [ "C:\\selection.fs", source ]
        let focused = documentNamed "selection.fs" solution

        let names =
            searchFocused cache [ focused.Id ] (caretOn "C:\\selection.fs" caretLine) solution ""
            |> Array.filter (fun name -> name = "Selection.Short" || name = "Selection.AroundTheCaret")

        Assert.Equal(expected, Array.head names)

    [<Fact>]
    let ``a batch of texts answers like the same texts one by one`` () =
        let cache = freshCache ()

        let solution =
            solutionOf
                [
                    "C:\\a.fs", "module A\n\nlet alpha = 1\n"
                    "C:\\b.fs", "module B\n\nlet beta = 2\n"
                ]

        let batched =
            CopilotSymbolQuery.search cache Seq.empty ValueNone solution [| "alpha"; "beta" |]
            |> run
            |> Array.map namesOf

        Assert.Equal<string>(searchIn cache Seq.empty solution "alpha", batched[0])
        Assert.Equal<string>(searchIn cache Seq.empty solution "beta", batched[1])

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
