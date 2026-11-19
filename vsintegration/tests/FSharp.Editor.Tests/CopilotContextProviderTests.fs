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

    let private widgets =
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

    let private solution = RoslynTestHelpers.CreateSolution widgets

    let private cache =
        MefHelpers.createExportProvider().GetExportedValue<FSharpNavigableItemsCache>()

    let private run computation =
        CancellableTask.runSynchronouslyWithoutCancellation computation

    let private manyProject = "C:\\many.fsproj"

    let private solutionOfProjects projects =
        let projects =
            [
                for projectPath, defines, files in projects ->
                    let projectId = ProjectId.CreateNewId()

                    let documents =
                        files
                        |> List.map (fun (path, source) -> RoslynTestHelpers.CreateDocumentInfo projectId path source)

                    struct (projectId, defines, files, RoslynTestHelpers.CreateProjectInfo projectId projectPath documents)
            ]

        let solution =
            RoslynTestHelpers.CreateSolution [ for struct (_, _, _, info) in projects -> info ]

        for struct (projectId, defines, files, _) in projects do
            { RoslynTestHelpers.DefaultProjectOptions with
                SourceFiles = files |> List.map fst |> Array.ofList
                OtherOptions = defines
            }
            |> RoslynTestHelpers.SetProjectOptions projectId solution

        solution

    let private solutionOf files =
        solutionOfProjects [ manyProject, [||], files ]

    let private documentAt (filePath: string) (solution: Solution) =
        solution.Projects
        |> Seq.collect _.Documents
        |> Seq.find (fun document -> document.FilePath = filePath)

    let private parse (document: Document) =
        cache.GetNavigableItems document |> run |> ignore

    let private isParsed (document: Document) =
        (cache.TryGetCachedNavigableItems document).IsSome

    let private hitsIn openDocumentIds focus solution pattern =
        CopilotSymbolQuery.search cache openDocumentIds focus solution [| pattern |]
        |> run
        |> Array.head

    let private namesOf hits =
        hits
        |> Array.map (fun (struct (item, _, _)) -> CopilotSymbolMapping.fullyQualifiedName item)

    let private namesIn openDocumentIds focus solution pattern =
        hitsIn openDocumentIds focus solution pattern |> namesOf

    let private itemNamed fullyQualifiedName =
        hitsIn Seq.empty ValueNone solution fullyQualifiedName
        |> Array.pick (fun (struct (item, _, _)) ->
            if CopilotSymbolMapping.fullyQualifiedName item = fullyQualifiedName then
                Some item
            else
                None)

    let private contextIn solution (name: string) picked =
        CopilotSymbolQuery.symbolContext cache Seq.empty solution name picked
        |> run
        |> ValueOption.defaultWith (fun () -> failwith $"expected a symbol context for {name}")

    let private declarationsPickedIn solution name picked =
        CopilotSymbolQuery.declarationsOf cache Seq.empty solution name picked |> run

    let private lineOf (struct (item: NavigableItem, _: Document)) = item.Range.StartLine

    let private fileOf (struct (_: NavigableItem, document: Document)) = document.FilePath

    let private pickedIn file line =
        ValueSome
            {
                File = file
                ProjectName = manyProject
                Line = line
            }

    let private focusOn filePath firstLine lastLine =
        ValueSome
            {
                FilePath = filePath
                FirstLine = firstLine
                LastLine = lastLine
            }

    let private caretOn filePath line = focusOn filePath line line

    let private manyDeclarations name count =
        let members =
            [ for i in 1..count -> $"    member _.{name}{i} = {i}" ] |> String.concat "\n"

        $"module {name}Module\n\ntype {name}Holder() =\n{members}\n"

    let private coldPath = "C:\\cold.fs"

    let private coldFile = coldPath, "module Cold\n\nlet widgetCounter = 1\n"

    let private openPath = "C:\\open.fs"

    let private multilinePath = "C:\\multiline.fs"

    let private multilineSolution =
        solutionOf
            [
                multilinePath,
                "module Multiline\n\n/// Adds.\nlet add\n    (x: int)\n    (y: int) =\n    x + y\n\ntype Calculator() =\n    member _.Sum\n        (\n            x: int,\n            y: int\n        ) =\n        x * y\n"
            ]

    let private overloadsPath = "C:\\overloads.fs"

    let private overloadsSolution =
        let overloads =
            [ "int"; "string"; "float"; "bool"; "char"; "byte" ]
            |> List.map (fun parameterType ->
                $"    member _.Bump(step: {parameterType}) =\n        ignore step\n        \"{parameterType}\"\n")
            |> String.concat "\n"

        solutionOf [ overloadsPath, $"module Overloads\n\ntype Counter() =\n{overloads}" ]

    let private dottedSolution =
        solutionOf
            [
                "C:\\dotted.fs",
                "module M\n\nlet ``a.b`` = 1\n\nmodule a =\n    let b = 2\n\nmodule ``x.y`` =\n    let z = 3\n\nmodule x =\n    module y =\n        let z = 4\n"
            ]

    [<Theory>]
    [<InlineData("Counter", "Widgets.Counter")>]
    [<InlineData("Bump", "Widgets.Counter.Bump")>]
    [<InlineData("Circle", "Widgets.Shape.Circle")>]
    [<InlineData("describeShape", "Widgets.describeShape")>]
    let ``search finds a declaration by its fully qualified name`` (pattern: string, expected: string) =
        Assert.Contains(expected, namesIn Seq.empty ValueNone solution pattern)

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
        let names = namesIn Seq.empty ValueNone solution "Counter"
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
        let solution = solutionOf [ "C:\\known.fs", manyDeclarations "Widget" 25; coldFile ]
        let known = documentAt "C:\\known.fs" solution

        let openDocumentIds =
            if isOpen then
                [ known.Id ]
            else
                parse known
                []

        let names = namesIn openDocumentIds ValueNone solution "Widget"

        Assert.Equal(20, names.Length)
        Assert.All(names, fun name -> Assert.StartsWith("WidgetModule", name, StringComparison.Ordinal))
        Assert.False(isParsed (documentAt coldPath solution))

    [<Fact>]
    let ``the search stops once it has enough declarations`` () =
        let solution =
            solutionOf
                [
                    for i in 1..150 -> $"C:\\cold{i}.fs", $"module Cold{i}\n\ntype Counter{i}() =\n    member _.Value = {i}\n"
                ]

        Assert.Equal(20, (namesIn Seq.empty ValueNone solution "Counter").Length)
        Assert.NotEmpty(solution.Projects |> Seq.collect _.Documents |> Seq.filter (isParsed >> not))

    [<Theory>]
    [<InlineData(false, false)>]
    [<InlineData(true, false)>]
    [<InlineData(false, true)>]
    let ``an unopened document is visited once whenever the cache learns of it`` (cachedBefore: bool, cachedBetween: bool) =
        let solution = solutionOf [ coldFile ]
        let document = documentAt coldPath solution

        if cachedBefore then
            parse document

        let struct (_, cached, cold) = CopilotSymbolQuery.tiers cache (HashSet()) solution
        let answeredFromCache = Seq.length cached

        if cachedBetween then
            parse document

        Assert.Equal(1, answeredFromCache + Seq.length cold)

    [<Fact>]
    let ``a parse of an earlier text does not answer for a document`` () =
        let solution =
            solutionOf
                [
                    "C:\\edited.fs", "module Edited\n\nlet before = 1\n\n\n\n\nlet removedLater = 2\n"
                ]

        let document = documentAt "C:\\edited.fs" solution
        parse document

        let edited = document.WithText(SourceText.From("module Edited\n\nlet after = 1\n"))
        let solution = edited.Project.Solution

        Assert.False(isParsed edited)
        Assert.Equal<string>([| "Edited.after" |], namesIn Seq.empty ValueNone solution "after")
        Assert.Empty(namesIn Seq.empty ValueNone solution "removedLater")

        Assert.True(
            (CopilotSymbolQuery.symbolContext cache Seq.empty solution "Edited.removedLater" ValueNone
             |> run)
                .IsNone
        )

    /// WidgetHolder loses to Widget on name length, so it can only come first through where its file sits.
    [<Theory>]
    [<InlineData(false, false, "Elsewhere.Widget")>]
    [<InlineData(true, false, "Holder.WidgetHolder")>]
    [<InlineData(true, true, "Holder.WidgetHolder")>]
    [<InlineData(false, true, "Elsewhere.Widget")>]
    let ``the file the user works in answers first`` (holderIsOpen: bool, holderIsFocused: bool, expected: string) =
        let holderPath = "C:\\holder.fs"

        let solution =
            solutionOf
                [
                    "C:\\elsewhere.fs", "module Elsewhere\n\ntype Widget() =\n    member _.Value = 1\n"
                    holderPath, "module Holder\n\ntype WidgetHolder() =\n    member _.Value = 2\n"
                ]

        let openDocumentIds =
            [
                if holderIsOpen then
                    (documentAt holderPath solution).Id
            ]

        let focus = if holderIsFocused then caretOn holderPath 1 else ValueNone
        let names = namesIn openDocumentIds focus solution "Widget"

        Assert.Equal(expected, Array.head names)
        Assert.Equal(2, names.Length)

    [<Fact>]
    let ``no text at all answers with the declarations of the open files`` () =
        let solution = solutionOf [ openPath, manyDeclarations "Widget" 3; coldFile ]

        let names =
            namesIn [ (documentAt openPath solution).Id ] (caretOn openPath 1) solution ""

        Assert.Contains("WidgetModule.WidgetHolder", names)
        Assert.All(names, fun name -> Assert.StartsWith("WidgetModule", name, StringComparison.Ordinal))
        Assert.False(isParsed (documentAt coldPath solution))

    [<Theory>]
    [<InlineData("wi")>]
    [<InlineData("widgetCounter")>]
    let ``a short text is looked up in the open files alone`` (pattern: string) =
        let solution = solutionOf [ openPath, "module Open\n\nlet other = 1\n"; coldFile ]
        let names = namesIn [ (documentAt openPath solution).Id ] ValueNone solution pattern

        Assert.Equal(pattern.Length >= 3, Array.contains "Cold.widgetCounter" names)

    /// AroundTheCaret loses to Short on name length, so it can only come first by holding the caret in its member's body.
    [<Theory>]
    [<InlineData(4, "Selection.Short")>]
    [<InlineData(8, "Selection.AroundTheCaret")>]
    let ``the declaration around the caret answers before the rest of the focused file`` (caretLine: int, expected: string) =
        let path = "C:\\selection.fs"

        let source =
            "module Selection\n\ntype Short() =\n    member _.Value = 1\n\ntype AroundTheCaret() =\n    member _.Compute() =\n        2\n"

        let solution = solutionOf [ path, source ]

        let names =
            namesIn [ (documentAt path solution).Id ] (caretOn path caretLine) solution ""
            |> Array.filter (fun name -> name = "Selection.Short" || name = "Selection.AroundTheCaret")

        Assert.Equal(expected, Array.head names)

    let private overloadsPickedAt picked =
        declarationsPickedIn overloadsSolution "Overloads.Counter.Bump" picked
        |> Array.map lineOf

    [<Fact>]
    let ``navigating a mention goes to the overload it was picked for`` () =
        let lines = overloadsPickedAt ValueNone
        Assert.Equal(6, (Array.distinct lines).Length)

        for line in lines do
            let picked = overloadsPickedAt (pickedIn overloadsPath line)
            Assert.Equal(line, Array.head picked)
            Assert.Equal(6, picked.Length)

        // A stale line leaves the file's own order; a renamed file falls back to the solution-wide answer.
        Assert.Equal<int>(lines, overloadsPickedAt (pickedIn overloadsPath -1))
        Assert.Equal<int>(lines, overloadsPickedAt (pickedIn "C:\\renamed.fs" lines[5]))

    [<Fact>]
    let ``a context starts from the overload its mention was picked for`` () =
        let lastOverload = Array.last (overloadsPickedAt ValueNone)

        let context =
            contextIn overloadsSolution "Overloads.Counter.Bump" (pickedIn overloadsPath lastOverload)

        Assert.StartsWith("member _.Bump(step: byte)", context.Snippet.TrimStart(), StringComparison.Ordinal)
        Assert.Equal(4, Seq.length context.SnippetLocations)

    [<Theory>]
    [<InlineData("C:\\first.fs", 4)>]
    [<InlineData("C:\\second.fs", 4)>]
    [<InlineData("C:\\second.fs", 40)>]
    let ``a mention goes to the file it was picked in when two files declare its name on one line`` (filePath: string, line: int) =
        let twin = "namespace Twins\n\ntype Twin() =\n    member _.Value = 1\n"
        let solution = solutionOf [ "C:\\first.fs", twin; "C:\\second.fs", twin ]

        let declarations =
            declarationsPickedIn solution "Twins.Twin.Value" (pickedIn filePath line)

        Assert.Equal<string>([| filePath |], declarations |> Array.map fileOf)

    [<Theory>]
    [<InlineData("C:\\a.fsproj", 3, "C:\\a.fsproj", 3)>]
    [<InlineData("C:\\b.fsproj", 5, "C:\\b.fsproj", 5)>]
    [<InlineData("C:\\a.fsproj", 40, "C:\\a.fsproj", 3)>]
    [<InlineData("C:\\b.fsproj", 40, "C:\\b.fsproj", 5)>]
    [<InlineData("C:\\gone.fsproj", 5, "C:\\b.fsproj", 5)>]
    let ``a mention goes to the project it was picked in when a file is linked into two``
        (project: string, line: int, expectedProject: string, expectedLine: int)
        =
        let linked =
            "C:\\shared.fs", "module M\n#if A\nlet shared = 1\n#else\nlet shared = \"B\"\n#endif\n"

        let solution =
            solutionOfProjects
                [
                    "C:\\a.fsproj", [| "--define:A" |], [ linked ]
                    "C:\\b.fsproj", [||], [ linked ]
                ]

        let picked =
            ValueSome
                {
                    File = fst linked
                    ProjectName = project
                    Line = line
                }

        let struct (item, document) =
            declarationsPickedIn solution "M.shared" picked |> Array.head

        Assert.Equal(expectedProject, document.Project.Name)
        Assert.Equal(expectedLine, item.Range.StartLine)

    [<Fact>]
    let ``a mention picked in a signature file goes to the implementation`` () =
        let solution =
            solutionOf
                [
                    "C:\\lib.fsi", "module Lib\n\nval twice: int -> int\n"
                    "C:\\lib.fs", "module Lib\n\n\nlet twice x = x * 2\n"
                ]

        let declarations =
            declarationsPickedIn solution "Lib.twice" (pickedIn "C:\\lib.fsi" 3)

        Assert.Equal<string>([| "C:\\lib.fs" |], declarations |> Array.map fileOf)

    [<Theory>]
    [<InlineData("Multiline.add", "/// Adds.", "x + y")>]
    [<InlineData("Multiline.Calculator.Sum", "member _.Sum", "x * y")>]
    let ``a header over several lines keeps its declaration whole`` (name: string, firstLine: string, lastLine: string) =
        let snippet = (contextIn multilineSolution name ValueNone).Snippet.Trim()

        Assert.StartsWith(firstLine, snippet, StringComparison.Ordinal)
        Assert.EndsWith(lastLine, snippet, StringComparison.Ordinal)

    [<Theory>]
    [<InlineData(7, "Multiline.add", "Multiline.Calculator.Sum")>]
    [<InlineData(15, "Multiline.Calculator.Sum", "Multiline.add")>]
    let ``the caret in a body under a header over several lines selects its declaration`` (caretLine: int, around: string, apart: string) =
        let focused = documentAt multilinePath multilineSolution

        let hits =
            hitsIn [ focused.Id ] (caretOn multilinePath caretLine) multilineSolution ""

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

        Assert.Equal<string>(namesIn Seq.empty ValueNone solution "alpha", batched[0])
        Assert.Equal<string>(namesIn Seq.empty ValueNone solution "beta", batched[1])

    [<Fact>]
    let ``a type context carries the whole declaration and its doc comment`` () =
        let context = contextIn solution "Widgets.Counter" ValueNone

        Assert.Equal("Widgets.Counter", context.FullyQualifiedName)
        Assert.Equal("Counter", context.UnqualifiedName)
        Assert.Contains("Counts things that matter.", context.Snippet)
        Assert.Contains("member _.Bump()", context.Snippet)

    [<Fact>]
    let ``a member context carries the member body alone`` () =
        let context = contextIn solution "Widgets.Counter.Bump" ValueNone

        Assert.Contains("value <- value + 1", context.Snippet)
        Assert.DoesNotContain("type Counter", context.Snippet)

    [<Fact>]
    let ``a one-line declaration keeps its doc comment`` () =
        let context = contextIn solution "Widgets.twice" ValueNone

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
        Assert.Equal(expected, (contextIn solution name ValueNone).SymbolType)

    [<Fact>]
    let ``a context points back at the source it was taken from`` () =
        let context = contextIn solution "Widgets.Counter" ValueNone
        let location = Assert.Single<SnippetLocation> context.SnippetLocations
        let document = RoslynTestHelpers.GetSingleDocument solution

        Assert.Equal(document.FilePath, location.FilePath)
        Assert.Equal(context.Snippet.Length, location.Span.Length)

    [<Fact>]
    let ``names that differ only in double backticks answer as two mentions`` () =
        let names = namesIn Seq.empty ValueNone dottedSolution "a.b"

        Assert.Contains("M.``a.b``", names)
        Assert.Contains("M.a.b", names)

    [<Theory>]
    [<InlineData("M.``a.b``", "let ``a.b`` = 1")>]
    [<InlineData("M.a.b", "let b = 2")>]
    [<InlineData("M.``x.y``.z", "let z = 3")>]
    [<InlineData("M.x.y.z", "let z = 4")>]
    let ``a name holding a dot resolves to its own declaration`` (fullyQualifiedName: string, declaration: string) =
        Assert.Equal(declaration, (contextIn dottedSolution fullyQualifiedName ValueNone).Snippet.Trim())

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

    /// Offset 10 starts "let a = 1"; a range over all six lines of the file means "focused, no line known".
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
        Assert.Equal(focusOn focusFilePath expectedFirst expectedLast, documentFocus caretOffset startLine endLine)

    [<Fact>]
    let ``no selection at all is focused with no line known`` () =
        let document = DocumentContext(focusContent, FilePath = focusFilePath)

        Assert.Equal(focusOn focusFilePath 0 0, CopilotSymbolMapping.editorFocusOf document)

    [<Fact>]
    let ``a document with no file path is not focused`` () =
        Assert.True((CopilotSymbolMapping.editorFocusOf (DocumentContext(focusContent))).IsNone)
