// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

module FSharp.Editor.Tests.InlineRenameTests

#nowarn "57"

open System
open System.IO
open System.Reflection
open System.Text
open System.Threading

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Editor.FindUsages
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Symbols
open FSharp.Compiler.Text
open FSharp.Editor.Tests.Helpers
open FSharp.Test.Compiler
open FSharp.Test.ProjectGeneration

open Xunit
open CancellableTasks

let private source =
    """module Main

type State = { Count: int }
type Msg = Increment | Decrement
type Cmd = NoOp

let update msg (state: State) =
    match msg with
    | Increment -> { state with Count = state.Count + 1 }, Cmd.NoOp
    | Decrement -> { state with Count = state.Count - 1 }, Cmd.NoOp
"""
    |> fun text -> text.Replace("\r\n", "\n")

let private expected =
    """module Main

type Model = { Count: int }
type Msg = Increment | Decrement
type Cmd = NoOp

let update msg (state: Model) =
    match msg with
    | Increment -> { state with Count = state.Count + 1 }, Cmd.NoOp
    | Decrement -> { state with Count = state.Count - 1 }, Cmd.NoOp
"""
    |> fun text -> text.Replace("\r\n", "\n")

let private coordinates (range: range) =
    range.StartLine, range.StartColumn, range.EndLine, range.EndColumn

let private textSpan (text: SourceText) (line, column, endLine, endColumn) =
    TextSpan.FromBounds(text.Lines[line - 1].Start + column, text.Lines[endLine - 1].Start + endColumn)

let private getDocumentSpan (span: obj) =
    let document = span.GetType().GetProperty("Document").GetValue(span) :?> Document
    let textSpan = span.GetType().GetProperty("SourceSpan").GetValue(span) :?> TextSpan
    document, textSpan

let private getReferenceSpan (reference: obj) =
    let item =
        reference.GetType().GetProperty("RoslynSourceReferenceItem", BindingFlags.Instance ||| BindingFlags.NonPublic).GetValue(reference)

    item.GetType().GetProperty("SourceSpan").GetValue(item) |> getDocumentSpan

let private assertReferences (document: Document) position expectedDefinitions expectedReferences =
    task {
        let context, definitions, references =
            FindReferencesTests.FindReferences.getContext ()

        do! (FSharpFindUsagesService() :> IFSharpFindUsagesService).FindReferencesAsync(document, position, context)

        let assertSpans expected actual =
            task {
                let expectedRows = ResizeArray()
                let actualRows = ResizeArray()

                for (doc: Document), range, snippet in expected do
                    let! text = doc.GetTextAsync()
                    expectedRows.Add(doc.Id, doc.FilePath, textSpan text range, snippet)

                for (doc: Document), span in actual do
                    let! text = doc.GetTextAsync()
                    actualRows.Add(doc.Id, doc.FilePath, span, text.ToString(span))

                let sorted rows =
                    rows
                    |> Seq.sortBy (fun (id: DocumentId, _, span: TextSpan, _) -> id.Id, span.Start)
                    |> Seq.toArray

                Assert.Equal<(DocumentId * string * TextSpan * string) array>(sorted expectedRows, sorted actualRows)
            }

        let definitionSpans =
            definitions
            |> Seq.collect (fun definition ->
                let item =
                    definition
                        .GetType()
                        .GetProperty("RoslynDefinitionItem", BindingFlags.Instance ||| BindingFlags.NonPublic)
                        .GetValue(definition)

                item.GetType().GetProperty("SourceSpans").GetValue(item) :?> System.Collections.IEnumerable
                |> Seq.cast<obj>
                |> Seq.map getDocumentSpan)

        do! assertSpans expectedDefinitions definitionSpans
        do! assertSpans expectedReferences (references |> Seq.map getReferenceSpan)
    }

let private markedSource (marked: string) =
    let parts = marked.Split([| "[|"; "|]" |], StringSplitOptions.None)
    Assert.True(parts.Length % 2 = 1)
    let text = StringBuilder()
    let spans = ResizeArray()

    for i in 0 .. parts.Length - 1 do
        if i % 2 = 1 then
            spans.Add(TextSpan(text.Length, parts[i].Length))

        text.Append(parts[i]) |> ignore

    text.ToString(), spans.ToArray()

let private setSearchOptions fast transparent solution =
    RoslynTestHelpers.SetEditorOptions
        solution
        { LanguageServicePerformanceOptions.Default with
            EnableFastFindReferencesAndRename = fast
        }

    RoslynTestHelpers.SetEditorOptions
        solution
        { AdvancedOptions.Default with
            UseTransparentCompiler = transparent
        }

let private initializeSearchOptions fast transparent solution =
    // Snapshot lookup requires identifier capture, including when fast search is subsequently switched off.
    setSearchOptions (fast || transparent) transparent solution

let private assertRename (document: Document) replacementText (expectedDocuments: (Document * string * TextSpan array) list) =
    task {
        let ct = CancellationToken.None
        let solution = document.Project.Solution

        let _, _, triggerSpans =
            expectedDocuments |> List.find (fun (doc, _, _) -> doc.Id = document.Id)

        let! info = InlineRenameService().GetRenameInfoAsync(document, triggerSpans[0].Start, ct)
        Assert.NotNull(info)
        Assert.True(info.CanRename)
        Assert.Equal(triggerSpans[0], info.TriggerSpan)
        let! locations = info.FindRenameLocationsAsync(false, false, ct)

        let expectedSpans =
            expectedDocuments
            |> Seq.collect (fun (doc, _, spans) -> spans |> Seq.map (fun span -> doc.Id, span))
            |> Seq.sortBy (fun (id, span) -> id.Id, span.Start)
            |> Seq.toArray

        let actualSpans =
            locations.Locations
            |> Seq.map (fun location ->
                Assert.Equal(location.TextSpan, info.GetReferenceEditSpan(location, ct))
                location.Document.Id, location.TextSpan)
            |> Seq.sortBy (fun (id, span) -> id.Id, span.Start)
            |> Seq.toArray

        Assert.Equal<(DocumentId * TextSpan) array>(expectedSpans, actualSpans)
        let! replacement = locations.GetReplacementsAsync(info.GetFinalSymbolName replacementText, ct)
        Assert.True(replacement.ReplacementTextValid)

        for project in solution.Projects do
            for original in project.Documents do
                let! oldText = original.GetTextAsync(ct)

                let expected, spans =
                    expectedDocuments
                    |> List.tryFind (fun (doc, _, _) -> doc.Id = original.Id)
                    |> Option.map (fun (_, expected, spans) -> expected, spans)
                    |> Option.defaultValue (oldText.ToString(), [||])

                let proposed = replacement.NewSolution.GetDocument(original.Id)
                let! proposedText = proposed.GetTextAsync(ct)
                Assert.Equal(expected, proposedText.ToString())
                let! changes = proposed.GetTextChangesAsync(original, ct)
                Assert.Equal<TextSpan array>(spans, changes |> Seq.map _.Span |> Seq.sortBy _.Start |> Seq.toArray)

                for previous, next in spans |> Array.pairwise do
                    Assert.True(previous.End <= next.Start)

        Assert.True(solution.Workspace.TryApplyChanges replacement.NewSolution)

        for project in solution.Projects do
            for original in project.Documents do
                let! proposed = replacement.NewSolution.GetDocument(original.Id).GetTextAsync(ct)
                let! applied = solution.Workspace.CurrentSolution.GetDocument(original.Id).GetTextAsync(ct)
                Assert.Equal(proposed.ToString(), applied.ToString())

        return solution.Workspace.CurrentSolution
    }

let private compileSolution (solution: Solution) (synthetic: SyntheticProject) (checker: FSharpChecker) =
    task {
        for id in solution.GetProjectDependencyGraph().GetTopologicallySortedProjects() do
            let project = solution.GetProject(id)

            for document in project.Documents do
                let! text = document.GetTextAsync()
                File.WriteAllText(document.FilePath, text.ToString())

            let syntheticProject =
                synthetic.GetAllProjects()
                |> Seq.find (fun p -> p.ProjectFileName = project.FilePath)

            let options = syntheticProject.GetProjectOptions checker

            let! diagnostics, error =
                checker.Compile
                    [|
                        "fsc.exe"
                        yield! options.OtherOptions
                        "--target:library"
                        $"-o:{syntheticProject.OutputFilename}"
                        yield! options.SourceFiles
                    |]

            Assert.True(error.IsNone, $"{error}")

            Assert.Empty(
                diagnostics
                |> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
            )

            Assert.True(File.Exists syntheticProject.OutputFilename)
    }

let renameCases =
    [
        "qualified trivia",
        "Model",
        """module Main
module Records =
    type [|State|] = { Count: int }
let a: Records. [|State|] = { Count = 0 }
let b: Records.
           [|State|] = a
let c: Records.(*comment*)[|State|] = a
let update (state: Records. [|State|]) = { state with Count = state.Count + 1 }
""",
        """module Main
module Records =
    type Model = { Count: int }
let a: Records. Model = { Count = 0 }
let b: Records.
           Model = a
let c: Records.(*comment*)Model = a
let update (state: Records. Model) = { state with Count = state.Count + 1 }
"""
        "qualified backticks",
        "``New Model``",
        """module Main
module Records =
    type [|``Old State``|] = { Count: int }
let update (state: Records.[|``Old State``|]) =
    { state with Count = state.Count + 1 }, "Old State" // Old State
""",
        """module Main
module Records =
    type ``New Model`` = { Count: int }
let update (state: Records.``New Model``) =
    { state with Count = state.Count + 1 }, "Old State" // Old State
"""
        "generic nested shadowed",
        "Model",
        """module Main
type [|State|]<'T> = { Count: int; Value: 'T }
type Outer<'T> = { Inner: [|State|]<'T>; Tag: int }
let State = "State"
type Other = { State: int }
let seed: [|State|]<string> = { Count = 0; Value = State }
let outer: Outer<string> = { Inner = seed; Tag = 1 }
let update (state: [|State|]<string>) =
    { state with
        Count = state.Count + 1
        Value = State }, "State" // State
let nested =
    { outer with
        Inner.Count = 2
        Tag = 3 }
let explicitNested = { outer with Inner = { seed with Count = 3 } }
module Shadow =
    type State = { X: int }
    let x: State = { X = 1 }
""",
        """module Main
type Model<'T> = { Count: int; Value: 'T }
type Outer<'T> = { Inner: Model<'T>; Tag: int }
let State = "State"
type Other = { State: int }
let seed: Model<string> = { Count = 0; Value = State }
let outer: Outer<string> = { Inner = seed; Tag = 1 }
let update (state: Model<string>) =
    { state with
        Count = state.Count + 1
        Value = State }, "State" // State
let nested =
    { outer with
        Inner.Count = 2
        Tag = 3 }
let explicitNested = { outer with Inner = { seed with Count = 3 } }
module Shadow =
    type State = { X: int }
    let x: State = { X = 1 }
"""
        "private record",
        "Model",
        """module Main
type private [|State|] = { Count: int }
let private update (state: [|State|]) = { state with Count = state.Count - 1 } // State
""",
        """module Main
type private Model = { Count: int }
let private update (state: Model) = { state with Count = state.Count - 1 } // State
"""
        "dot operator",
        "+.+",
        """module Main
module Operators =
    let ([|-.-|]) a b = a - b
open Operators
let result = 3 [|-.-|] 1
""",
        """module Main
module Operators =
    let (+.+) a b = a - b
open Operators
let result = 3 +.+ 1
"""
        "property",
        "Contents",
        """module Main
type Box() =
    let mutable value = 0
    member _.[|Value|]
        with get () = value
        and set v = value <- v
let box = Box()
box.[|Value|] <- box.[|Value|] + 1
""",
        """module Main
type Box() =
    let mutable value = 0
    member _.Contents
        with get () = value
        and set v = value <- v
let box = Box()
box.Contents <- box.Contents + 1
"""
        "get identifier",
        "read",
        "module Main\nlet [|get|] x = x + 1\nlet result = [|get|] 42\n",
        "module Main\nlet read x = x + 1\nlet result = read 42\n"
        "set identifier",
        "write",
        "module Main\nlet [|set|] x = x + 1\nlet result = [|set|] 42\n",
        "module Main\nlet write x = x + 1\nlet result = write 42\n"
    ]

let cases =
    seq {
        for name, replacement, original, expected in renameCases do
            for newline in [ "\n"; "\r\n" ] do
                yield
                    [|
                        box name
                        box replacement
                        box (original.Replace("\r\n", "\n").Replace("\n", newline))
                        box (expected.Replace("\r\n", "\n").Replace("\n", newline))
                    |]
    }

let private report typeName fieldName localName expanded newline =
    let messages =
        if expanded then
            "type Msg =\n    | Increment\n    | Decrement\n\ntype Cmd = | NoOp"
        else
            "type Msg = Increment | Decrement\ntype Cmd = NoOp"

    let code =
        $"""module Main

type {typeName} = {{ {fieldName}: int }}
{messages}

let update msg ({localName}: {typeName}) =
    match msg with
    | Increment -> {{ {localName} with {fieldName} = {localName}.{fieldName} + 1 }}, Cmd.NoOp
    | Decrement -> {{ {localName} with {fieldName} = {localName}.{fieldName} - 1 }}, Cmd.NoOp
"""

    code.Replace("\r\n", "\n").Replace("\n", newline)

let reportCases =
    seq {
        for symbol, replacement, oldNames, newNames in
            [
                "type", "Model", ("[|State|]", "Count", "state"), ("Model", "Count", "state")
                "field", "Tally", ("State", "[|Count|]", "state"), ("State", "Tally", "state")
                "local", "current", ("State", "Count", "[|state|]"), ("State", "Count", "current")
            ] do
            for expanded, newline, fast, transparent in [ false, "\n", false, false; true, "\n", true, false; true, "\r\n", true, true ] do
                let ty, field, local = oldNames
                let newTy, newField, newLocal = newNames

                yield
                    [|
                        box symbol
                        box (report ty field local expanded newline)
                        box replacement
                        box (report newTy newField newLocal expanded newline)
                        box fast
                        box transparent
                    |]
    }

type RecordRename() =

    static member Cases = cases
    static member ReportCases = reportCases

    [<Theory; MemberData(nameof RecordRename.ReportCases)>]
    member _.``Issue 20791 - report type field and local rename``(symbol: string, marked, replacement, expected, fast, transparent) =
        task {
            let source, spans = markedSource marked

            let project =
                { SyntheticProject.Create(
                      { sourceFile "Main" [] with
                          Source = source
                      }
                  ) with
                    AutoAddModules = false
                }

            let solution, checker =
                RoslynTestHelpers.CreateSolution(project, configureWorkspace = initializeSearchOptions fast transparent)

            use _workspace = solution.Workspace
            setSearchOptions fast transparent solution

            let document = RoslynTestHelpers.GetSingleDocument solution

            if symbol = "local" then
                let! text = document.GetTextAsync()
                let pos = text.Lines.GetLinePosition(spans[0].Start)

                let! _, check =
                    document.GetFSharpParseAndCheckResultsAsync("Issue 20791")
                    |> CancellableTask.start CancellationToken.None

                let use' =
                    check
                        .GetSymbolUseAtLocation(pos.Line + 1, pos.Character + spans[0].Length, text.Lines[pos.Line].ToString(), [ "state" ])
                        .Value

                Assert.Equal(Some SymbolScope.CurrentDocument, use'.GetSymbolScope document)

            let! applied = assertRename document replacement [ document, expected, spans ]
            do! compileSolution applied project checker
        }

    [<Theory>]
    [<InlineData("record", false, false)>]
    [<InlineData("record", true, false)>]
    [<InlineData("record", true, true)>]
    [<InlineData("parameter", false, false)>]
    [<InlineData("parameter", true, true)>]
    [<InlineData("record signature", false, false)>]
    [<InlineData("opaque signature", false, false)>]
    [<InlineData("opaque signature", true, false)>]
    [<InlineData("opaque signature", false, true)>]
    [<InlineData("opaque signature", true, true)>]
    member _.``Issue 20791 - signature ordered files and dependent project``(target: string, fast, transparent) =
        task {
            let parameter = target = "parameter"
            let opaque = target = "opaque signature"

            let signature, implementation, expectedSignature, expectedImplementation, replacement =
                if parameter then
                    "module Library\ntype State = { Count: int }\nval update: [|state|]: State -> State\n",
                    "module Library\ntype State = { Count: int }\nlet update ([|state|]: State) = { [|state|] with Count = [|state|].Count + 1 }\n",
                    "module Library\ntype State = { Count: int }\nval update: current: State -> State\n",
                    "module Library\ntype State = { Count: int }\nlet update (current: State) = { current with Count = current.Count + 1 }\n",
                    "current"
                else
                    "module Library\ntype [|State|] = { Count: int }\nval update: state: [|State|] -> [|State|]\n",
                    "module Library\ntype [|State|] = { Count: int }\nlet update (state: [|State|]) = { state with Count = state.Count + 1 }\n",
                    "module Library\ntype Model = { Count: int }\nval update: state: Model -> Model\n",
                    "module Library\ntype Model = { Count: int }\nlet update (state: Model) = { state with Count = state.Count + 1 }\n",
                    "Model"

            let signature, expectedSignature =
                if opaque then
                    "module Library\ntype [|State|]\nval update: state: [|State|] -> [|State|]\n",
                    "module Library\ntype Model\nval update: state: Model -> Model\n"
                else
                    signature, expectedSignature

            let signature, signatureSpans = markedSource signature
            let implementation, implementationSpans = markedSource implementation

            let usage, expectedUsage =
                if parameter then
                    "module Usage\nlet update (state: Library.State) = Library.update state\n",
                    "module Usage\nlet update (state: Library.State) = Library.update state\n"
                else
                    "module Usage\nlet update (state: Library.[|State|]) = Library.update state\n",
                    "module Usage\nlet update (state: Library.Model) = Library.update state\n"

            let usage, usageSpans = markedSource usage

            let dependent, expectedDependent =
                if parameter then
                    "module App\nlet update (state: Library.State) = { state with Count = state.Count - 1 }, \"State\"\n",
                    "module App\nlet update (state: Library.State) = { state with Count = state.Count - 1 }, \"State\"\n"
                elif opaque then
                    "module App\nlet update (state: Library.[|State|]) = Library.update state, \"State\"\n",
                    "module App\nlet update (state: Library.Model) = Library.update state, \"State\"\n"
                else
                    "module App\nlet update (state: Library.[|State|]) = { state with Count = state.Count - 1 }, \"State\"\n",
                    "module App\nlet update (state: Library.Model) = { state with Count = state.Count - 1 }, \"State\"\n"

            let dependent, dependentSpans = markedSource dependent

            let library =
                { SyntheticProject.Create(
                      { sourceFile "Library" [] with
                          Source = implementation
                          SignatureFile = Custom signature
                      },
                      { sourceFile "Usage" [] with
                          Source = usage
                      }
                  ) with
                    AutoAddModules = false
                }

            let app =
                { SyntheticProject.Create(
                      { sourceFile "App" [] with
                          Source = dependent
                      },
                      { sourceFile "Untouched" [] with
                          Source = "module Untouched\nlet text = \"State\"\n"
                      }
                  ) with
                    AutoAddModules = false
                    DependsOn = [ library ]
                }

            let struct (solution, checker) =
                RoslynTestHelpers.CreateMultiProjectSolution(app, configureWorkspace = initializeSearchOptions fast transparent)

            use _workspace = solution.Workspace
            setSearchOptions fast transparent solution

            let document path =
                solution.TryGetDocumentFromPath path |> ValueOption.get

            let implementationDoc = document (library.GetFilePath "Library")
            let signatureDoc = document (library.GetSignatureFilePath "Library")

            let triggerDoc, triggerSpan =
                if target = "record signature" || opaque then
                    signatureDoc, signatureSpans[0]
                else
                    implementationDoc, implementationSpans[0]

            let! text = triggerDoc.GetTextAsync()
            let position = text.Lines.GetLinePosition(triggerSpan.Start)

            let! _, check =
                triggerDoc.GetFSharpParseAndCheckResultsAsync("Issue 20791")
                |> CancellableTask.start CancellationToken.None

            let use' =
                check
                    .GetSymbolUseAtLocation(
                        position.Line + 1,
                        position.Character + triggerSpan.Length,
                        text.Lines[position.Line].ToString(),
                        [
                            if parameter then "state" else "State"
                        ]
                    )
                    .Value

            if parameter then
                Assert.Equal(Some SymbolScope.SignatureAndImplementation, use'.GetSymbolScope triggerDoc)
            else
                let entity = Assert.IsType<FSharpEntity>(use'.Symbol)
                Assert.Equal(opaque, entity.IsOpaque)
                Assert.Equal(not opaque, entity.IsFSharpRecord)

                let! _, implementationCheck =
                    implementationDoc.GetFSharpParseAndCheckResultsAsync("Issue 20791")
                    |> CancellableTask.start CancellationToken.None

                let implementationEntity =
                    implementationCheck.GetSymbolUseAtLocation(2, 10, "type State = { Count: int }", [ "State" ]).Value.Symbol

                Assert.True(entity.IsEffectivelySameAs implementationEntity)

                Assert.Equal<(int * int * int * int) array>(
                    [| 2, 5, 2, 10; 3, 19, 3, 24 |],
                    implementationCheck.GetUsesOfSymbolInFile(entity)
                    |> Array.map (fun use' -> coordinates use'.Range)
                    |> Array.sort
                )

                Assert.Equal<(int * int * int * int) array>(
                    [| 2, 5, 2, 10; 3, 19, 3, 24; 3, 28, 3, 66 |],
                    implementationCheck.GetUsesOfSymbolInFile(entity, relatedSymbolKinds = RelatedSymbolUseKind.All)
                    |> Array.map (fun use' -> coordinates use'.Range)
                    |> Array.sort
                )

                match use'.GetSymbolScope triggerDoc with
                | Some(SymbolScope.Projects([ p ], false)) -> Assert.Equal(triggerDoc.Project.Id, p.Id)
                | scope -> Assert.Fail($"Unexpected scope: {scope}")

                let definitionDoc = if transparent then implementationDoc else triggerDoc

                do!
                    assertReferences
                        triggerDoc
                        triggerSpan.Start
                        [ definitionDoc, (2, 5, 2, 10), "State" ]
                        [
                            (if definitionDoc.Id = signatureDoc.Id then
                                 implementationDoc
                             else
                                 signatureDoc),
                            (2, 5, 2, 10),
                            "State"
                            signatureDoc, (3, 19, 3, 24), "State"
                            signatureDoc, (3, 28, 3, 33), "State"
                            implementationDoc, (3, 19, 3, 24), "State"
                            implementationDoc, (3, 55, 3, 66), "Count + 1 }"
                            document (library.GetFilePath "Usage"), (2, 27, 2, 32), "State"
                            document (app.GetFilePath "App"), (2, 27, 2, 32), "State"
                            if not opaque then
                                document (app.GetFilePath "App"), (2, 63, 2, 74), "Count - 1 }"
                        ]

            let! applied =
                assertRename
                    triggerDoc
                    replacement
                    [
                        signatureDoc, expectedSignature, signatureSpans
                        implementationDoc, expectedImplementation, implementationSpans
                        document (library.GetFilePath "Usage"), expectedUsage, usageSpans
                        document (app.GetFilePath "App"), expectedDependent, dependentSpans
                    ]

            do! compileSolution applied app checker
        }

    [<Theory; MemberData(nameof RecordRename.Cases)>]
    member _.``Issue 20791 - lexical locations preserve syntax and symbol identity``(name: string, replacement, marked, expected) =
        task {
            let source, spans = markedSource marked

            let project =
                { SyntheticProject.Create(
                      { sourceFile "Main" [] with
                          Source = source
                      }
                  ) with
                    AutoAddModules = false
                }

            let solution, checker = RoslynTestHelpers.CreateSolution project
            use _workspace = solution.Workspace
            let document = RoslynTestHelpers.GetSingleDocument solution

            if name = "generic nested shadowed" then
                let! text = document.GetTextAsync()

                let! _, check =
                    document.GetFSharpParseAndCheckResultsAsync("Issue 20791")
                    |> CancellableTask.start CancellationToken.None

                let use' =
                    check.GetSymbolUseAtLocation(2, 10, text.Lines[1].ToString(), [ "State" ]).Value

                let all =
                    check.GetUsesOfSymbolInFile(use'.Symbol, relatedSymbolKinds = RelatedSymbolUseKind.All)

                let! shared =
                    SymbolHelpers.getSymbolUses use' document check
                    |> CancellableTask.start CancellationToken.None

                Assert.Equal<(int * int * int * int) array>(
                    all |> Array.map (fun use' -> coordinates use'.Range) |> Array.sort,
                    shared |> Seq.map (snd >> coordinates) |> Seq.sort |> Seq.toArray
                )

                let newline =
                    if source.IndexOf("\r\n", StringComparison.Ordinal) >= 0 then
                        "\r\n"
                    else
                        "\n"

                do!
                    assertReferences
                        document
                        spans[0].Start
                        [ document, (2, 5, 2, 10), "State" ]
                        [
                            document, (3, 26, 3, 31), "State"
                            document, (6, 10, 6, 15), "State"
                            document, (8, 19, 8, 24), "State"
                            document, (10, 22, 11, 23), $"Count + 1{newline}        Value = State }}"
                            document, (16, 42, 16, 65), "{ seed with Count = 3 }"
                        ]

                do!
                    assertReferences
                        document
                        (text.Lines[2].Start + 5)
                        [ document, (3, 5, 3, 10), "Outer" ]
                        [
                            document, (7, 11, 7, 16), "Outer"
                            document, (14, 14, 15, 17), $"Count = 2{newline}        Tag = 3 }}"
                            document, (16, 21, 16, 67), "{ outer with Inner = { seed with Count = 3 } }"
                        ]

            if name = "private record" then
                let! _, check =
                    document.GetFSharpParseAndCheckResultsAsync("Issue 20791")
                    |> CancellableTask.start CancellationToken.None

                let use' =
                    check.GetSymbolUseAtLocation(2, 18, "type private State = { Count: int }", [ "State" ]).Value

                Assert.Equal(Some SymbolScope.CurrentDocument, use'.GetSymbolScope document)

                do!
                    assertReferences
                        document
                        spans[0].Start
                        [ document, (2, 13, 2, 18), "State" ]
                        [ document, (3, 27, 3, 32), "State"; document, (3, 63, 3, 74), "Count - 1 }" ]

            let! applied = assertRename document replacement [ document, expected, spans ]
            do! compileSolution applied project checker
        }

    [<Theory>]
    [<InlineData(false, false)>]
    [<InlineData(true, false)>]
    [<InlineData(false, true)>]
    [<InlineData(true, true)>]
    member _.``Issue 20791 - record type rename preserves update expressions``(fast, transparent) =
        task {
            let ct = CancellationToken.None

            let project =
                { SyntheticProject.Create(
                      { sourceFile "Main" [] with
                          Source = source
                      }
                  ) with
                    AutoAddModules = false
                }

            let solution, _ =
                RoslynTestHelpers.CreateSolution(project, configureWorkspace = initializeSearchOptions fast transparent)

            use _workspace = solution.Workspace
            setSearchOptions fast transparent solution

            let document = RoslynTestHelpers.GetSingleDocument solution
            let! text = document.GetTextAsync(ct)
            Assert.DoesNotContain("\r", source)
            Assert.Equal(source, text.ToString())

            let! _, check =
                document.GetFSharpParseAndCheckResultsAsync("Issue 20791")
                |> CancellableTask.start ct

            let symbolUse =
                check.GetSymbolUseAtLocation(3, 10, text.Lines[2].ToString(), [ "State" ]).Value

            let entity = Assert.IsType<FSharpEntity>(symbolUse.Symbol)
            Assert.True(entity.IsFSharpRecord)

            let declaredEntity =
                check.PartialAssemblySignature.Entities
                |> Seq.exactlyOne
                |> _.NestedEntities
                |> Seq.find (fun entity -> entity.DisplayName = "State")

            Assert.True(entity.IsEffectivelySameAs declaredEntity)
            Assert.Equal((3, 5, 3, 10), coordinates entity.DeclarationLocation)

            match symbolUse.GetSymbolScope document with
            | Some(SymbolScope.Projects([ p ], false)) -> Assert.Equal(document.Project.Id, p.Id)
            | scope -> Assert.Fail($"Unexpected scope: {scope}")

            for kinds in
                [
                    ValueNone
                    ValueSome RelatedSymbolUseKind.None
                    ValueSome RelatedSymbolUseKind.CopyAndUpdateRecord
                    ValueSome RelatedSymbolUseKind.All
                ] do
                let uses =
                    check.GetUsesOfSymbolInFile(entity, ?relatedSymbolKinds = ValueOption.toOption kinds)

                let expectedRanges =
                    [|
                        3, 5, 3, 10
                        7, 23, 7, 28
                        if
                            kinds = ValueSome RelatedSymbolUseKind.All
                            || kinds = ValueSome RelatedSymbolUseKind.CopyAndUpdateRecord
                        then
                            9, 19, 9, 57
                            10, 19, 10, 57
                    |]

                Assert.Equal<(int * int * int * int) array>(
                    expectedRanges,
                    uses |> Array.map (fun use' -> coordinates use'.Range) |> Array.sort
                )

                for use' in uses |> Array.sortBy (fun use' -> coordinates use'.Range) do
                    Assert.True(entity.IsEffectivelySameAs use'.Symbol)
                    let span = textSpan text (coordinates use'.Range)

                    let expectedText =
                        match use'.Range.StartLine with
                        | 9 -> "{ state with Count = state.Count + 1 }"
                        | 10 -> "{ state with Count = state.Count - 1 }"
                        | _ -> "State"

                    Assert.Equal(expectedText, text.ToString(span))
                    Assert.Equal(use'.Range.StartLine = 3, use'.IsFromDefinition)
                    Assert.Equal(use'.Range.StartLine = 7, use'.IsFromType)
                    Assert.Equal(use'.Range.StartLine >= 9, use'.IsFromUse)

            let ordinary = check.GetUsesOfSymbolInFile(entity)

            Assert.Equal<(int * int * int * int) array>(
                [| 3, 5, 3, 10; 7, 23, 7, 28 |],
                ordinary |> Array.map (fun use' -> coordinates use'.Range) |> Array.sort
            )

            let! shared = SymbolHelpers.getSymbolUses symbolUse document check |> CancellableTask.start ct
            let! snapshot = document.Project.GetFSharpProjectSnapshot() |> CancellableTask.start ct

            let! checker, _, _, options =
                document.GetFSharpCompilationOptionsAsync("Issue 20791")
                |> CancellableTask.start ct

            let! background =
                if transparent then
                    checker.FindBackgroundReferencesInFile(document.FilePath, snapshot, entity)
                else
                    checker.FindBackgroundReferencesInFile(
                        document.FilePath,
                        options,
                        entity,
                        canInvalidateProject = false,
                        fastCheck = fast
                    )

            Assert.Equal<(int * int * int * int) array>(
                [| 3, 5, 3, 10; 7, 23, 7, 28; 9, 19, 9, 57; 10, 19, 10, 57 |],
                background |> Seq.map coordinates |> Seq.sort |> Seq.toArray
            )

            Assert.Equal<(int * int * int * int) array>(
                background |> Seq.map coordinates |> Seq.sort |> Seq.toArray,
                shared |> Seq.map (snd >> coordinates) |> Seq.sort |> Seq.toArray
            )

            Assert.Equal(transparent, document.Project.UseTransparentCompiler)
            Assert.Equal(transparent, document.GetFSharpChecker().UsesTransparentCompiler)
            Assert.Equal(fast, document.Project.IsFastFindReferencesEnabled)

            for _, range in shared |> Seq.sortBy (snd >> coordinates) do
                let raw = RoslynHelpers.TryFSharpRangeToTextSpan(text, range) |> ValueOption.get

                let converted =
                    Tokenizer.TryFSharpRangeToTextSpanForEditor(text, range, entity.DisplayName)

                Assert.Equal(textSpan text (coordinates range), raw)

                let expectedSpan =
                    if range.StartLine >= 9 then
                        textSpan text (range.StartLine, 46, range.EndLine, 57)
                    else
                        raw

                Assert.Equal(ValueSome expectedSpan, converted)

            do!
                assertReferences
                    document
                    (text.Lines[2].Start + 5)
                    [ document, (3, 5, 3, 10), "State" ]
                    [
                        document, (7, 23, 7, 28), "State"
                        document, (9, 46, 9, 57), "Count + 1 }"
                        document, (10, 46, 10, 57), "Count - 1 }"
                    ]

            let service = InlineRenameService()
            let! info = service.GetRenameInfoAsync(document, text.Lines[2].Start + 5, ct)
            Assert.NotNull(info)
            Assert.True(info.CanRename)
            Assert.Equal(entity.FullName, info.FullDisplayName)
            Assert.Equal(textSpan text (3, 5, 3, 10), info.TriggerSpan)

            let! applied =
                assertRename
                    document
                    "Model"
                    [
                        document, expected, [| textSpan text (3, 5, 3, 10); textSpan text (7, 23, 7, 28) |]
                    ]

            let! appliedText = applied.GetDocument(document.Id).GetTextAsync(ct)

            FSharp(appliedText.ToString())
            |> asLibrary
            |> compile
            |> shouldSucceed
            |> ignore
        }
