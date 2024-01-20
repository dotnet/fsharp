// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Collections.Concurrent
open System.Collections.Generic
open System.Collections.Immutable
open System.Threading.Tasks

open Microsoft.CodeAnalysis

open FSharp.Compiler.CodeAnalysis
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

            let! defines, langVersion, strictIndentation = document.GetFsharpParsingOptionsAsync(userOpName) |> liftAsync

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
                    strictIndentation,
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

    let getSymbolUsesInProjects (symbol: FSharpSymbol, projects: Project list, onFound: Document -> range -> CancellableTask<unit>) =
        match projects with
        | [] -> CancellableTask.singleton ()
        | firstProject :: _ ->
            let isFastFindReferencesEnabled = firstProject.IsFastFindReferencesEnabled

            // TODO: this needs to use already boxed boolean instead of boxing it every time.
            let props =
                [| nameof isFastFindReferencesEnabled, isFastFindReferencesEnabled :> obj |]

            cancellableTask {
                // TODO: this needs to be a single event with a duration
                TelemetryReporter.ReportSingleEvent(TelemetryEvents.GetSymbolUsesInProjectsStarted, props)

                let snapshotAccumulator = Dictionary()

                let! projects =
                    projects
                    |> Seq.map (fun project ->
                        project.GetFSharpProjectSnapshot(snapshotAccumulator)
                        |> CancellableTask.map (fun s -> project, s))
                    |> CancellableTask.sequential

                do!
                    projects
                    |> Seq.map (fun (project, snapshot) ->
                        project.FindFSharpReferencesAsync(symbol, snapshot, onFound, "getSymbolUsesInProjects"))
                    |> CancellableTask.whenAll

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
                let symbolUses = checkFileResults.GetUsesOfSymbolInFile(symbolUse.Symbol)

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
                        checkFileResults.GetUsesOfSymbolInFile(symbolUse.Symbol)
                        |> Seq.map (fun symbolUse -> (doc, symbolUse.Range)))

                do! symbolUses |> Seq.map ((<||) onFound) |> CancellableTask.whenAll

            | scope ->
                let projectsToCheck =
                    match scope with
                    | Some(SymbolScope.Projects(scopeProjects, false)) ->
                        [
                            for scopeProject in scopeProjects do
                                yield scopeProject
                                yield! scopeProject.GetDependentProjects()
                        ]
                        |> List.distinct
                    | Some(SymbolScope.Projects(scopeProjects, true)) -> scopeProjects
                    // The symbol is declared in .NET framework, an external assembly or in a C# project within the solution.
                    // In order to find all its usages we have to check all F# projects.
                    | _ -> Seq.toList currentDocument.Project.Solution.Projects

                do! getSymbolUsesInProjects (symbolUse.Symbol, projectsToCheck, onFound)
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
