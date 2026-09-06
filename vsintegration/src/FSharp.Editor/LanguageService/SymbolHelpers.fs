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
    let private groupInstances (currentProject: Project) (projects: Project list) =
        let rank = rankInstances currentProject

        projects
        |> List.groupBy _.FilePath
        |> List.map (fun (_, instances) ->
            let ordered = List.sortBy rank instances
            struct (ordered.Head, ordered.Tail))

    let getSymbolUsesInProjects
        (symbol: FSharpSymbol, currentProject: Project, projects: Project list, onFound: Document -> range -> CancellableTask<unit>)
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
                    [ for project in projects -> struct (project, []) ]

            cancellableTask {
                // TODO: this needs to be a single event with a duration
                TelemetryReporter.ReportSingleEvent(TelemetryEvents.GetSymbolUsesInProjectsStarted, props)

                let! ct = CancellableTask.getCancellationToken ()
                // Not disposed: a search started before a later snapshot fails still has to release it.
                let throttle = new SemaphoreSlim(max 1 Environment.ProcessorCount)
                // Mutated by the checker while a snapshot is built, so snapshots are built one at a time.
                let snapshotAccumulator = Dictionary()
                let searches = ResizeArray<Task>()

                let snapshotFor (project: Project) =
                    if project.UseTransparentCompiler then
                        project.GetFSharpProjectSnapshot snapshotAccumulator
                        |> CancellableTask.map ValueSome
                    else
                        CancellableTask.singleton ValueNone

                let start (project: Project) snapshot searchedInstance =
                    searches.Add(
                        project.FindFSharpReferencesAsync
                            (symbol, snapshot, searchedInstance, throttle, onFound, "getSymbolUsesInProjects")
                            ct
                    )

                for struct (primary, secondaries) in groups do
                    let! snapshot = snapshotFor primary
                    start primary snapshot ValueNone

                    for secondary in secondaries do
                        let! snapshot = snapshotFor secondary
                        start secondary snapshot (ValueSome primary)

                do! Task.WhenAll searches

                TelemetryReporter.ReportSingleEvent(TelemetryEvents.GetSymbolUsesInProjectsFinished, props)
            }

    let findSymbolUses
        (symbolUse: FSharpSymbolUse)
        (currentDocument: Document)
        (checkFileResults: FSharpCheckFileResults)
        (onFound: Document -> range -> CancellableTask<unit>)
        =
        cancellableTask {
            match symbolUse.GetSymbolScope currentDocument with

            | Some SymbolScope.CurrentDocument ->
                let symbolUses =
                    checkFileResults.GetUsesOfSymbolInFile(symbolUse.Symbol, relatedSymbolKinds = RelatedSymbolUseKind.All)

                do!
                    symbolUses
                    |> Seq.map (fun symbolUse -> onFound currentDocument symbolUse.Range)
                    |> CancellableTask.whenAll

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

                let symbolUses =
                    (checkFileResults, currentDocument) :: otherFileCheckResults
                    |> Seq.collect (fun (checkFileResults, doc) ->
                        checkFileResults.GetUsesOfSymbolInFile(symbolUse.Symbol, relatedSymbolKinds = RelatedSymbolUseKind.All)
                        |> Seq.map (fun symbolUse -> (doc, symbolUse.Range)))

                do! symbolUses |> Seq.map ((<||) onFound) |> CancellableTask.whenAll

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

            let onFound =
                fun document range -> cancellableTask { symbolUses.Add(document, range) }

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
