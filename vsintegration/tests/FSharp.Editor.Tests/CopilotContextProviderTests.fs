// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open System
open System.Collections.Generic

open Xunit

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.Copilot
open Microsoft.VisualStudio.FSharp.Editor

open FSharp.Compiler.EditorServices
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

    /// One matcher and one parse cache per test, so what a test leaves parsed cannot answer the next one.
    let private freshCache () =
        MefHelpers.createExportProvider().GetExportedValue<FSharpNavigableItemsCache>()

    let private cache = freshCache ()

    let private run computation =
        CancellableTask.runSynchronouslyWithoutCancellation computation

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

    let private hitsIn cache openDocumentIds focus solution pattern =
        CopilotSymbolQuery.search cache openDocumentIds focus solution [| pattern |]
        |> run
        |> Array.head

    let private namesOf hits =
        hits
        |> Array.map (fun (struct (item, _, _)) -> CopilotSymbolMapping.fullyQualifiedName item)

    let private searchFocused cache openDocumentIds focus solution pattern =
        hitsIn cache openDocumentIds focus solution pattern |> namesOf

    let private searchIn cache openDocumentIds solution pattern =
        searchFocused cache openDocumentIds ValueNone solution pattern

    let private search pattern =
        searchIn cache Seq.empty solution pattern

    let private itemNamed (fullyQualifiedName: string) =
        hitsIn cache Seq.empty ValueNone solution fullyQualifiedName
        |> Array.tryPick (fun (struct (item, _, _)) ->
            if CopilotSymbolMapping.fullyQualifiedName item = fullyQualifiedName then
                Some item
            else
                None)
        |> Option.defaultWith (fun () -> failwith $"no declaration named {fullyQualifiedName}")

    let private contextPickedIn cache solution (name: string) picked =
        CopilotSymbolQuery.symbolContext cache Seq.empty solution name picked
        |> run
        |> ValueOption.defaultWith (fun () -> failwith $"expected a symbol context for {name}")

    let private contextIn cache solution name =
        contextPickedIn cache solution name ValueNone

    let private declarationsPickedIn cache solution name picked =
        CopilotSymbolQuery.declarationsOf cache Seq.empty solution name picked |> run

    let private lineOf (struct (item: NavigableItem, _: Document)) = item.Range.StartLine

    let private fileOf (struct (_: NavigableItem, document: Document)) = document.FilePath

    let private contextOf name = contextIn cache solution name

    /// A file holding more declarations matching `name` than one query reports.
    let private manyDeclarations name count =
        let members =
            [ for i in 1..count -> $"    member _.{name}{i} = {i}" ] |> String.concat "\n"

        $"module {name}Module\n\ntype {name}Holder() =\n{members}\n"

    let private coldFile = "C:\\cold.fs", "module Cold\n\nlet widgetCounter = 1\n"

    /// The holder's declaration loses on every other part of the ordering - the name it is matched
    /// against is longer - so it can only come first through where its file sits.
    let private twoWidgets =
        [
            "C:\\elsewhere.fs", "module Elsewhere\n\ntype Widget() =\n    member _.Value = 1\n"
            "C:\\holder.fs", "module Holder\n\ntype WidgetHolder() =\n    member _.Value = 2\n"
        ]

    /// A value and a module whose names hold a dot, beside the nested paths that would spell the same
    /// without the double backticks.
    let private dottedSolution =
        solutionOf
            [
                "C:\\dotted.fs",
                "module M\n\nlet ``a.b`` = 1\n\nmodule a =\n    let b = 2\n\nmodule ``x.y`` =\n    let z = 3\n\nmodule x =\n    module y =\n        let z = 4\n"
            ]

    let private caretOn filePath line =
        ValueSome
            {
                FilePath = filePath
                FirstLine = line
                LastLine = line
            }

    /// A function and a member whose parameters each take a line, so their headers end lines below their names.
    let private multilineHeaders =
        "module Multiline\n\n/// Adds.\nlet add\n    (x: int)\n    (y: int) =\n    x + y\n\ntype Calculator() =\n    member _.Sum\n        (\n            x: int,\n            y: int\n        ) =\n        x * y\n"

    let private multilineSolution = solutionOf [ "C:\\multiline.fs", multilineHeaders ]

    /// More overloads of one member than a context carries, each naming its parameter type in its body.
    let private overloadsSolution =
        let overloads =
            [ "int"; "string"; "float"; "bool"; "char"; "byte" ]
            |> List.map (fun parameterType ->
                $"    member _.Bump(step: {parameterType}) =\n        ignore step\n        \"{parameterType}\"\n")
            |> String.concat "\n"

        solutionOf [ "C:\\overloads.fs", $"module Overloads\n\ntype Counter() =\n{overloads}" ]

    /// One name on one line of two files, as when two projects declare the same type or share a linked file.
    let private twinsSolution =
        let twin = "namespace Twins\n\ntype Twin() =\n    member _.Value = 1\n"
        solutionOf [ "C:\\first.fs", twin; "C:\\second.fs", twin ]

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

    /// The inputs of a query are split on ':', so "#fsharpSymbol:Ns.Type:15" arrives as three of them.
    [<Theory>]
    [<InlineData("Widget", "Widget")>]
    [<InlineData("", "")>]
    [<InlineData("fsharpSymbol", null)>]
    [<InlineData("fsharpSymbol: Ns.Type ", "Ns.Type")>]
    [<InlineData("fsharpSymbol:Ns.Type:15", "Ns.Type")>]
    [<InlineData("fsharpSymbol::15", "")>]
    let ``the search text is the input after the member name`` (inputs: string, expected: string) =
        let query = CopilotMentionQuery(CopilotMentionType.Unknown, inputs.Split ':')

        Assert.Equal(ValueOption.ofObj expected, CopilotSymbolMapping.searchTextOf query)

    [<Fact>]
    let ``only context mentions are searched for`` () =
        let query = CopilotMentionQuery(CopilotMentionType.Command, [| "Widget" |])

        Assert.True((CopilotSymbolMapping.searchTextOf query).IsNone)

    [<Fact>]
    let ``search reports each declaration once`` () =
        let names = search "Counter"
        Assert.Equal((Array.distinct names).Length, names.Length)

    [<Fact>]
    let ``an unknown name has no context`` () =
        Assert.True(
            (CopilotSymbolQuery.symbolContext cache Seq.empty solution "Widgets.NoSuchThing" ValueNone
             |> run)
                .IsNone
        )

    [<Theory>]
    [<InlineData(true)>]
    [<InlineData(false)>]
    let ``a known file answers without parsing the rest of the solution`` (isOpen: bool) =
        let cache = freshCache ()
        let solution = solutionOf [ "C:\\known.fs", manyDeclarations "Widget" 25; coldFile ]
        let known = documentNamed "known.fs" solution

        let openDocumentIds =
            if isOpen then
                [ known.Id ]
            else
                cache.GetNavigableItems known |> run |> ignore
                []

        let names = searchIn cache openDocumentIds solution "Widget"

        Assert.Equal(20, names.Length)
        Assert.All(names, fun name -> Assert.StartsWith("WidgetModule", name, StringComparison.Ordinal))
        Assert.True((cache.TryGetCachedNavigableItems(documentNamed "cold.fs" solution)).IsNone)

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
            |> Array.filter (fun document -> (cache.TryGetCachedNavigableItems document).IsNone)
        )

    /// Navigate To and other queries fill the same cache, so it can learn of a document between a
    /// query's walk of the cached documents and its walk of the rest.
    [<Theory>]
    [<InlineData(false, false)>]
    [<InlineData(true, false)>]
    [<InlineData(false, true)>]
    let ``an unopened document is visited once whenever the cache learns of it`` (cachedBefore: bool, cachedBetween: bool) =
        let cache = freshCache ()
        let solution = solutionOf [ coldFile ]
        let document = documentNamed "cold.fs" solution

        if cachedBefore then
            cache.GetNavigableItems document |> run |> ignore

        let struct (_, cached, cold) = CopilotSymbolQuery.tiers cache (HashSet()) solution
        let answeredFromCache = Seq.length cached

        if cachedBetween then
            cache.GetNavigableItems document |> run |> ignore

        Assert.Equal(1, answeredFromCache + Seq.length cold)

    /// A file nobody has open still changes - on disk, or by closing its tab without saving - and what
    /// was parsed of it before then names declarations and lines its text no longer has.
    [<Fact>]
    let ``a parse of an earlier text does not answer for a document`` () =
        let cache = freshCache ()

        let solution =
            solutionOf
                [
                    "C:\\edited.fs", "module Edited\n\nlet before = 1\n\n\n\n\nlet removedLater = 2\n"
                ]

        let document = documentNamed "edited.fs" solution
        cache.GetNavigableItems document |> run |> ignore

        let edited = document.WithText(SourceText.From("module Edited\n\nlet after = 1\n"))
        let solution = edited.Project.Solution

        Assert.True((cache.TryGetCachedNavigableItems edited).IsNone)
        Assert.Equal<string>([| "Edited.after" |], searchIn cache Seq.empty solution "after")
        Assert.Empty(searchIn cache Seq.empty solution "removedLater")

        Assert.True(
            (CopilotSymbolQuery.symbolContext cache Seq.empty solution "Edited.removedLater" ValueNone
             |> run)
                .IsNone
        )

    /// Focus outranks being open, which outranks the rest; a focused file that is no longer open counts
    /// for nothing, since nothing tells the tracker that its tab has closed.
    [<Theory>]
    [<InlineData(false, false, "Elsewhere.Widget")>]
    [<InlineData(true, false, "Holder.WidgetHolder")>]
    [<InlineData(true, true, "Holder.WidgetHolder")>]
    [<InlineData(false, true, "Elsewhere.Widget")>]
    let ``the file the user works in answers first`` (holderIsOpen: bool, holderIsFocused: bool, expected: string) =
        let cache = freshCache ()
        let solution = solutionOf twoWidgets
        let holder = documentNamed "holder.fs" solution

        let openDocumentIds =
            [
                if holderIsOpen then
                    holder.Id
            ]

        let focus =
            if holderIsFocused then
                caretOn "C:\\holder.fs" 1
            else
                ValueNone

        let names = searchFocused cache openDocumentIds focus solution "Widget"

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
        Assert.True((cache.TryGetCachedNavigableItems(documentNamed "cold.fs" solution)).IsNone)

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

    /// The type around the caret loses on name length to the other one, so it can only come first by
    /// holding the caret - on a line of its member's body, not of its own name.
    [<Theory>]
    [<InlineData(4, "Selection.Short")>]
    [<InlineData(8, "Selection.AroundTheCaret")>]
    let ``the declaration around the caret answers before the rest of the focused file`` (caretLine: int, expected: string) =
        let cache = freshCache ()

        let source =
            "module Selection\n\ntype Short() =\n    member _.Value = 1\n\ntype AroundTheCaret() =\n    member _.Compute() =\n        2\n"

        let solution = solutionOf [ "C:\\selection.fs", source ]
        let focused = documentNamed "selection.fs" solution

        let names =
            searchFocused cache [ focused.Id ] (caretOn "C:\\selection.fs" caretLine) solution ""
            |> Array.filter (fun name -> name = "Selection.Short" || name = "Selection.AroundTheCaret")

        Assert.Equal(expected, Array.head names)

    let private overloadsPickedAt picked =
        declarationsPickedIn cache overloadsSolution "Overloads.Counter.Bump" picked
        |> Array.map lineOf

    /// Overloads answer to the same fully qualified name, so navigating one after the picker has
    /// closed - when there is no caret to consult any more - has to go by where it was picked,
    /// not by whichever overload a solution-wide scan happens to reach first.
    [<Fact>]
    let ``navigating a mention goes to the overload it was picked for`` () =
        let lines = overloadsPickedAt ValueNone
        Assert.Equal(6, (Array.distinct lines).Length)

        for line in lines do
            let picked = overloadsPickedAt (ValueSome(struct ("C:\\overloads.fs", line)))
            Assert.Equal(line, Array.head picked)
            Assert.Equal(6, picked.Length)

        // An edit above an overload moves its line, and a renamed file no longer answers for it.
        Assert.Equal<int>(lines, overloadsPickedAt (ValueSome(struct ("C:\\overloads.fs", -1))))
        Assert.Equal<int>(lines, overloadsPickedAt (ValueSome(struct ("C:\\renamed.fs", lines[5]))))

    [<Fact>]
    let ``a context starts from the overload its mention was picked for`` () =
        let lastOverload = Array.last (overloadsPickedAt ValueNone)

        let context =
            contextPickedIn cache overloadsSolution "Overloads.Counter.Bump" (ValueSome(struct ("C:\\overloads.fs", lastOverload)))

        Assert.StartsWith("member _.Bump(step: byte)", context.Snippet.TrimStart(), StringComparison.Ordinal)
        Assert.Equal(4, Seq.length context.SnippetLocations)

    /// A line the picked file no longer declares the name on still leaves the file to go by.
    [<Theory>]
    [<InlineData("C:\\first.fs", 4)>]
    [<InlineData("C:\\second.fs", 4)>]
    [<InlineData("C:\\second.fs", 40)>]
    let ``a mention goes to the file it was picked in when two files declare its name on one line`` (filePath: string, line: int) =
        let declarations =
            declarationsPickedIn cache twinsSolution "Twins.Twin.Value" (ValueSome(struct (filePath, line)))

        Assert.Equal<string>([| filePath |], declarations |> Array.map fileOf)

    /// A signature answers only when no implementation declares the name, wherever the mention was picked.
    [<Fact>]
    let ``a mention picked in a signature file goes to the implementation`` () =
        let solution =
            solutionOf
                [
                    "C:\\lib.fsi", "module Lib\n\nval twice: int -> int\n"
                    "C:\\lib.fs", "module Lib\n\n\nlet twice x = x * 2\n"
                ]

        let declarations =
            declarationsPickedIn (freshCache ()) solution "Lib.twice" (ValueSome(struct ("C:\\lib.fsi", 3)))

        Assert.Equal<string>([| "C:\\lib.fs" |], declarations |> Array.map fileOf)

    [<Theory>]
    [<InlineData("Multiline.add", "/// Adds.", "x + y")>]
    [<InlineData("Multiline.Calculator.Sum", "member _.Sum", "x * y")>]
    let ``a header over several lines keeps its declaration whole`` (name: string, firstLine: string, lastLine: string) =
        let snippet = (contextIn cache multilineSolution name).Snippet.Trim()

        Assert.StartsWith(firstLine, snippet, StringComparison.Ordinal)
        Assert.EndsWith(lastLine, snippet, StringComparison.Ordinal)

    [<Theory>]
    [<InlineData(7, "Multiline.add", "Multiline.Calculator.Sum")>]
    [<InlineData(15, "Multiline.Calculator.Sum", "Multiline.add")>]
    let ``the caret in a body under a header over several lines selects its declaration`` (caretLine: int, around: string, apart: string) =
        let focused = documentNamed "multiline.fs" multilineSolution

        let hits =
            hitsIn (freshCache ()) [ focused.Id ] (caretOn "C:\\multiline.fs" caretLine) multilineSolution ""

        let focusOf name =
            hits
            |> Array.pick (fun (struct (item, _, focus)) ->
                if CopilotSymbolMapping.fullyQualifiedName item = name then
                    Some focus
                else
                    None)

        Assert.Equal(DocumentFocus.Selected, focusOf around)
        Assert.Equal(DocumentFocus.Focused, focusOf apart)

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
        let document = RoslynTestHelpers.GetSingleDocument solution

        Assert.Equal(document.FilePath, location.FilePath)
        Assert.Equal(context.Snippet.Length, location.Span.Length)

    [<Fact>]
    let ``names that differ only in double backticks answer as two mentions`` () =
        let names = searchIn cache Seq.empty dottedSolution "a.b"

        Assert.Contains("M.``a.b``", names)
        Assert.Contains("M.a.b", names)

    [<Theory>]
    [<InlineData("M.``a.b``", "let ``a.b`` = 1")>]
    [<InlineData("M.a.b", "let b = 2")>]
    [<InlineData("M.``x.y``.z", "let z = 3")>]
    [<InlineData("M.x.y.z", "let z = 4")>]
    let ``a name holding a dot resolves to its own declaration`` (fullyQualifiedName: string, declaration: string) =
        Assert.Equal(declaration, (contextIn cache dottedSolution fullyQualifiedName).Snippet.Trim())

    /// Six lines, counting newlines: "module M", "", "let a = 1", "", "let b = 2", and the blank line a
    /// trailing newline leaves after it.
    let private focusContent = "module M\n\nlet a = 1\n\nlet b = 2\n"

    let private focusFilePath = "C:\\focus.fs"

    let private documentFocus caretOffset startLine endLine =
        let caret = if caretOffset < 0 then Nullable() else Nullable caretOffset

        let lineRange =
            if startLine < 0 then
                Nullable()
            else
                Nullable(Microsoft.VisualStudio.RpcContracts.Utilities.Range(startLine, -1, endLine, -1))

        let selection =
            DocumentSelection(0, focusContent.Length, Caret = caret, LineRange = lineRange)

        DocumentContext(focusContent, FilePath = focusFilePath, TotalLinesInFile = 6, Selections = [| selection |])
        |> CopilotSymbolMapping.editorFocusOf

    /// Copilot reports the caret as the offset of its line's start (10, the start of "let a = 1"), a
    /// line range as 1-based line numbers, and the whole file as a range covering every line - which no
    /// declaration can be "around", so it answers "focused, no line known", the same as no selection at all.
    [<Theory>]
    [<InlineData(10, -1, -1, 3, 3)>]
    [<InlineData(-1, 2, 5, 2, 5)>]
    [<InlineData(-1, 1, 6, 0, 0)>]
    let ``the caret or selection maps onto a focused line range``
        (caretOffset: int)
        (startLine: int)
        (endLine: int)
        (expectedFirst: int)
        (expectedLast: int)
        =
        let expected =
            ValueSome
                {
                    FilePath = focusFilePath
                    FirstLine = expectedFirst
                    LastLine = expectedLast
                }

        Assert.Equal(expected, documentFocus caretOffset startLine endLine)

    [<Fact>]
    let ``no selection at all is focused with no line known`` () =
        let document = DocumentContext(focusContent, FilePath = focusFilePath)

        let expected =
            ValueSome
                {
                    FilePath = focusFilePath
                    FirstLine = 0
                    LastLine = 0
                }

        Assert.Equal(expected, CopilotSymbolMapping.editorFocusOf document)

    [<Fact>]
    let ``a document with no file path is not focused`` () =
        let document = DocumentContext(focusContent)
        Assert.True((CopilotSymbolMapping.editorFocusOf document).IsNone)
