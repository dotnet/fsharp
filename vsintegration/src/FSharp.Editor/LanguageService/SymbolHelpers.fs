// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.Collections.Immutable
open System.IO
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.NameResolution
open FSharp.Compiler.Symbols
open FSharp.Compiler.Text
open Microsoft.VisualStudio.FSharp.Editor.Telemetry
open CancellableTasks

module internal SymbolHelpers =

    /// Used for local code fixes in a document, e.g. to rename local parameters
    let getSymbolUsesOfSymbolAtLocationInDocument (document: Document, position: int) =
        asyncMaybe {
            let userOpName = "getSymbolUsesOfSymbolAtLocationInDocument"
            let! ct = Async.CancellationToken |> liftAsync

            let! _, checkFileResults =
                document.GetFSharpParseAndCheckResultsAsync(userOpName)
                |> CancellableTask.start ct
                |> Async.AwaitTask
                |> liftAsync

            let! defines, langVersion = document.GetFsharpParsingOptionsAsync(userOpName) |> liftAsync

            let! cancellationToken = Async.CancellationToken |> liftAsync
            let! sourceText = document.GetTextAsync(cancellationToken)
            let textLine = sourceText.Lines.GetLineFromPosition(position)
            let textLinePos = sourceText.Lines.GetLinePosition(position)
            let fcsTextLineNumber = Line.fromZ textLinePos.Line

            let! symbol =
                Tokenizer.getSymbolAtPosition (
                    document.Id,
                    sourceText,
                    position,
                    document.FilePath,
                    defines,
                    SymbolLookupKind.Greedy,
                    false,
                    false,
                    Some langVersion,
                    cancellationToken
                )

            let! symbolUse =
                checkFileResults.GetSymbolUseAtLocation(
                    fcsTextLineNumber,
                    symbol.Ident.idRange.EndColumn,
                    textLine.ToString(),
                    symbol.FullIsland
                )

            let! ct = Async.CancellationToken |> liftAsync

            let symbolUses =
                checkFileResults.GetUsesOfSymbolInFile(symbolUse.Symbol, cancellationToken = ct)

            return symbolUses
        }

    /// Ranks the target-framework instances of a project file: the one the current document lives in,
    /// then those in its dependency closure, whose references resolve the symbol the same way.
    let private rankInstances (currentProject: Project) =
        let graph = currentProject.Solution.GetProjectDependencyGraph()

        let related =
            HashSet
                [
                    yield! graph.GetProjectsThatThisProjectTransitivelyDependsOn currentProject.Id
                    yield! graph.GetProjectsThatTransitivelyDependOnThisProject currentProject.Id
                ]

        fun (project: Project) ->
            if project.Id = currentProject.Id then 0
            elif related.Contains project.Id then 1
            else 2

    /// One search per project file: the best-ranked target-framework instance is searched in full,
    /// the others only where their sources can differ from it.
    let private groupInstances (currentProject: Project) (projects: Project seq) =
        let rank = rankInstances currentProject

        seq {
            for _, instances in projects |> Seq.groupBy _.FilePath do
                let instances = Seq.toArray instances
                // Ordering the rest buys nothing, and ranking every instance of the solution at once
                // measures slower than one pass per group.
                let primary = Array.minBy rank instances
                struct (primary, instances |> Seq.filter (fun instance -> instance.Id <> primary.Id))
        }

    /// One core is left to the thread that has to stay responsive, and every search in the editor
    /// shares what remains.
    let searchThrottle = new SemaphoreSlim(max 1 (Environment.ProcessorCount - 1))

    let getSymbolUsesInProjects
        (symbol: FSharpSymbol, currentProject: Project, projects: Project list, onFound: Document -> range seq -> CancellableTask<unit>)
        =
        match projects |> List.filter _.IsFSharp with
        | [] -> CancellableTask.singleton ()
        | firstProject :: _ as projects ->
            let isFastFindReferencesEnabled = firstProject.IsFastFindReferencesEnabled

            // TODO: this needs to use already boxed boolean instead of boxing it every time.
            let props =
                [| nameof isFastFindReferencesEnabled, isFastFindReferencesEnabled :> obj |]

            let groups =
                if isFastFindReferencesEnabled then
                    groupInstances currentProject projects
                else
                    seq { for project in projects -> struct (project, Seq.empty) }

            cancellableTask {
                // TODO: this needs to be a single event with a duration
                TelemetryReporter.ReportSingleEvent(TelemetryEvents.GetSymbolUsesInProjectsStarted, props)

                let! ct = CancellableTask.getCancellationToken ()

                // A file that several projects compile - the target-framework instances of one project
                // file, or two project files sharing a source file - is searched in each of them and
                // reports the same range every time. The range carries its file, so the first project
                // to report a use keeps it and the rest are dropped.
                let reported = ConcurrentDictionary<range, unit>()

                let onFound document ranges =
                    let fresh =
                        ranges |> Seq.filter (fun range -> reported.TryAdd(range, ())) |> Seq.toArray

                    if fresh.Length = 0 then
                        CancellableTask.singleton ()
                    else
                        onFound document fresh
                // Mutated by the checker while a snapshot is built, so snapshots are built one at a time.
                let snapshotAccumulator = Dictionary()
                let searches = ResizeArray<Task>()

                let snapshotFor (project: Project) =
                    if project.UseTransparentCompiler then
                        project.GetFSharpProjectSnapshot snapshotAccumulator
                        |> CancellableTask.map ValueSome
                    else
                        CancellableTask.singleton ValueNone

                // Started, not awaited: the next project's snapshot is built while this one searches.
                let startSearching (project: Project) snapshot searchedInstance =
                    searches.Add(
                        project.FindFSharpReferencesAsync
                            (symbol, snapshot, searchedInstance, searchThrottle, onFound, "getSymbolUsesInProjects")
                            ct
                    )

                for struct (primary, secondaries) in groups do
                    let! snapshot = snapshotFor primary
                    startSearching primary snapshot ValueNone

                    for secondary in secondaries do
                        let! snapshot = snapshotFor secondary
                        startSearching secondary snapshot (ValueSome primary)

                do! Task.WhenAll searches

                TelemetryReporter.ReportSingleEvent(TelemetryEvents.GetSymbolUsesInProjectsFinished, props)
            }

    let findSymbolUses
        (symbolUse: FSharpSymbolUse)
        (currentDocument: Document)
        (checkFileResults: FSharpCheckFileResults)
        (onFound: Document -> range seq -> CancellableTask<unit>)
        =
        cancellableTask {
            match symbolUse.GetSymbolScope currentDocument with

            | Some SymbolScope.CurrentDocument ->
                let symbolUses =
                    checkFileResults.GetUsesOfSymbolInFile(symbolUse.Symbol, relatedSymbolKinds = RelatedSymbolUseKind.All)

                do! onFound currentDocument (symbolUses |> Seq.map _.Range)

            | Some SymbolScope.SignatureAndImplementation ->
                let otherFile = getOtherFile currentDocument.FilePath

                let! otherFileCheckResults =
                    match currentDocument.Project.Solution.TryGetDocumentFromPath otherFile with
                    | ValueSome doc ->
                        cancellableTask {
                            let! _, checkFileResults = doc.GetFSharpParseAndCheckResultsAsync("findReferencedSymbolsAsync")
                            return [ checkFileResults, doc ]
                        }
                    | ValueNone -> CancellableTask.singleton []

                for checkFileResults, doc in (checkFileResults, currentDocument) :: otherFileCheckResults do
                    let symbolUses =
                        checkFileResults.GetUsesOfSymbolInFile(symbolUse.Symbol, relatedSymbolKinds = RelatedSymbolUseKind.All)

                    do! onFound doc (symbolUses |> Seq.map _.Range)

            | Some(SymbolScope.Projects(scopeProjects, isLocalForProject)) ->
                let projectsToCheck =
                    if isLocalForProject then
                        scopeProjects
                    else
                        [
                            for scopeProject in scopeProjects do
                                yield scopeProject
                                yield! scopeProject.GetDependentProjects()
                        ]
                        |> List.distinct

                do! getSymbolUsesInProjects (symbolUse.Symbol, currentDocument.Project, projectsToCheck, onFound)

            // The symbol is declared in .NET framework, an external assembly or in a C# project within the solution.
            // Optimization: Only search projects that reference the specific assembly
            | None ->
                let projectsToCheck =
                    match symbolUse.Symbol.Assembly.FileName with
                    | Some assemblyPath ->
                        match ProjectFiltering.getProjectsReferencingAssembly assemblyPath currentDocument.Project.Solution with
                        | [] -> Seq.toList currentDocument.Project.Solution.Projects
                        | referencingProjects -> referencingProjects
                    | None -> Seq.toList currentDocument.Project.Solution.Projects

                do! getSymbolUsesInProjects (symbolUse.Symbol, currentDocument.Project, projectsToCheck, onFound)
        }

    let getSymbolUses (symbolUse: FSharpSymbolUse) (currentDocument: Document) (checkFileResults: FSharpCheckFileResults) =
        cancellableTask {
            let symbolUses = ConcurrentBag()

            let onFound document (ranges: range seq) =
                cancellableTask {
                    for range in ranges do
                        symbolUses.Add(document, range)
                }

            do! findSymbolUses symbolUse currentDocument checkFileResults onFound

            return symbolUses |> seq
        }

    let getSymbolUsesInSolution (symbolUse: FSharpSymbolUse, checkFileResults: FSharpCheckFileResults, document: Document) =
        cancellableTask {
            let! symbolUses = getSymbolUses symbolUse document checkFileResults

            let symbolUsesWithDocumentId =
                symbolUses |> Seq.map (fun (doc, range) -> doc.Id, range)

            let usesByDocumentId = symbolUsesWithDocumentId |> Seq.groupBy fst
            return usesByDocumentId.ToImmutableDictionary(fst, snd >> Seq.map snd >> Seq.toArray)
        }
