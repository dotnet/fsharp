// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Concurrent
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols
open FSharp.Compiler.Text
open Microsoft.VisualStudio.FSharp.Editor.Telemetry

module internal SymbolHelpers =
    /// Used for local code fixes in a document, e.g. to rename local parameters
    let getSymbolUsesOfSymbolAtLocationInDocument (document: Document, position: int) =
        asyncMaybe {
            let userOpName = "getSymbolUsesOfSymbolAtLocationInDocument"
            let! _, checkFileResults = document.GetFSharpParseAndCheckResultsAsync(userOpName) |> liftAsync
            let! defines, langVersion = document.GetFSharpCompilationDefinesAndLangVersionAsync(userOpName) |> liftAsync

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

    let getSymbolUsesInProjects
        (
            symbol: FSharpSymbol,
            projects: Project list,
            onFound: Document -> range -> Async<unit>,
            ct: CancellationToken
        ) =
        match projects with
        | [] -> Task.CompletedTask
        | firstProject :: _ ->
            let isFastFindReferencesEnabled = firstProject.IsFastFindReferencesEnabled

            let props =
                [ nameof isFastFindReferencesEnabled, isFastFindReferencesEnabled :> obj ]

            backgroundTask {
                TelemetryReporter.reportEvent "getSymbolUsesInProjectsStarted" props

                do!
                    projects
                    |> Seq.map (fun project ->
                        Task.Run(fun () -> project.FindFSharpReferencesAsync(symbol, onFound, "getSymbolUsesInProjects", ct)))
                    |> Task.WhenAll

                TelemetryReporter.reportEvent "getSymbolUsesInProjectsFinished" props
            }

    let findSymbolUses (symbolUse: FSharpSymbolUse) (currentDocument: Document) (checkFileResults: FSharpCheckFileResults) onFound =
        async {
            match symbolUse.GetSymbolScope currentDocument with

            | Some SymbolScope.CurrentDocument ->
                let symbolUses = checkFileResults.GetUsesOfSymbolInFile(symbolUse.Symbol)

                for symbolUse in symbolUses do
                    do! onFound currentDocument symbolUse.Range

            | Some SymbolScope.SignatureAndImplementation ->
                let otherFile = getOtherFile currentDocument.FilePath

                let! otherFileCheckResults =
                    match currentDocument.Project.Solution.TryGetDocumentFromPath otherFile with
                    | Some doc ->
                        async {
                            let! _, checkFileResults = doc.GetFSharpParseAndCheckResultsAsync("findReferencedSymbolsAsync")
                            return [ checkFileResults, doc ]
                        }
                    | None -> async.Return []

                let symbolUses =
                    (checkFileResults, currentDocument) :: otherFileCheckResults
                    |> Seq.collect (fun (checkFileResults, doc) ->
                        checkFileResults.GetUsesOfSymbolInFile(symbolUse.Symbol)
                        |> Seq.map (fun symbolUse -> (doc, symbolUse.Range)))

                for document, range in symbolUses do
                    do! onFound document range

            | scope ->
                let projectsToCheck =
                    match scope with
                    | Some (SymbolScope.Projects (scopeProjects, false)) ->
                        [
                            for scopeProject in scopeProjects do
                                yield scopeProject
                                yield! scopeProject.GetDependentProjects()
                        ]
                        |> List.distinct
                    | Some (SymbolScope.Projects (scopeProjects, true)) -> scopeProjects
                    // The symbol is declared in .NET framework, an external assembly or in a C# project within the solution.
                    // In order to find all its usages we have to check all F# projects.
                    | _ -> Seq.toList currentDocument.Project.Solution.Projects

                let! ct = Async.CancellationToken

                do!
                    getSymbolUsesInProjects (symbolUse.Symbol, projectsToCheck, onFound, ct)
                    |> Async.AwaitTask
        }

    let getSymbolUses (symbolUse: FSharpSymbolUse) (currentDocument: Document) (checkFileResults: FSharpCheckFileResults) =
        async {
            let symbolUses = ConcurrentBag()
            let onFound = fun document range -> async { symbolUses.Add(document, range) }

            do! findSymbolUses symbolUse currentDocument checkFileResults onFound

            return symbolUses |> seq
        }

    let getSymbolUsesInSolution (symbolUse: FSharpSymbolUse, checkFileResults: FSharpCheckFileResults, document: Document) =
        async {
            let! symbolUses = getSymbolUses symbolUse document checkFileResults

            let symbolUsesWithDocumentId =
                symbolUses |> Seq.map (fun (doc, range) -> doc.Id, range)

            let usesByDocumentId = symbolUsesWithDocumentId |> Seq.groupBy fst
            return usesByDocumentId.ToImmutableDictionary(fst, snd >> Seq.map snd >> Seq.toArray)
        }

    type OriginalText = string

    // Note, this function is broken and shouldn't be used because the source text ranges to replace are applied sequentially,
    // breaking the position computations as changes progress, especially if two changes are made on the same line.
    //
    // However, it is only currently used by ProposeUpperCaseLabel code fix, where the changes to code will rarely be on the same line.
    //
    // A better approach is to use something like createTextChangeCodeFix below, with a delayed function to compute a set of changes to be applied
    // simultaneously.  But that doesn't work for this case, as we want a set of changes to apply acrosss the whole solution.

    let changeAllSymbolReferences
        (
            document: Document,
            symbolSpan: TextSpan,
            textChanger: string -> string
        ) : Async<(Func<CancellationToken, Task<Solution>> * OriginalText) option> =
        asyncMaybe {
            let userOpName = "changeAllSymbolReferences"
            do! Option.guard (symbolSpan.Length > 0)
            let! cancellationToken = liftAsync Async.CancellationToken
            let! sourceText = document.GetTextAsync(cancellationToken)
            let originalText = sourceText.ToString(symbolSpan)
            do! Option.guard (originalText.Length > 0)

            let! symbol = document.TryFindFSharpLexerSymbolAsync(symbolSpan.Start, SymbolLookupKind.Greedy, false, false, userOpName)
            let textLine = sourceText.Lines.GetLineFromPosition(symbolSpan.Start)
            let textLinePos = sourceText.Lines.GetLinePosition(symbolSpan.Start)
            let fcsTextLineNumber = Line.fromZ textLinePos.Line

            let! _, checkFileResults = document.GetFSharpParseAndCheckResultsAsync(userOpName) |> liftAsync

            let! symbolUse =
                checkFileResults.GetSymbolUseAtLocation(
                    fcsTextLineNumber,
                    symbol.Ident.idRange.EndColumn,
                    textLine.ToString(),
                    symbol.FullIsland
                )

            let newText = textChanger originalText
            // defer finding all symbol uses throughout the solution
            return
                Func<_, _>(fun (cancellationToken: CancellationToken) ->
                    async {
                        let! symbolUsesByDocumentId = getSymbolUsesInSolution (symbolUse, checkFileResults, document)

                        let mutable solution = document.Project.Solution

                        for KeyValue (documentId, symbolUses) in symbolUsesByDocumentId do
                            let document = document.Project.Solution.GetDocument(documentId)
                            let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                            let mutable sourceText = sourceText

                            for symbolUse in symbolUses do
                                match RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, symbolUse) with
                                | None -> ()
                                | Some span ->
                                    let textSpan = Tokenizer.fixupSpan (sourceText, span)
                                    sourceText <- sourceText.Replace(textSpan, newText)
                                    solution <- solution.WithDocumentText(documentId, sourceText)

                        return solution
                    }
                    |> RoslynHelpers.StartAsyncAsTask cancellationToken),
                originalText
        }
