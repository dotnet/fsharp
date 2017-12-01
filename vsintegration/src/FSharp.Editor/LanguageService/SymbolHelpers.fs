// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Generic
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text

open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.VisualStudio.FSharp.Editor.Symbols 


module internal SymbolHelpers =
    open Microsoft.CodeAnalysis.CodeFixes
    open Microsoft.CodeAnalysis.CodeActions

    /// Used for local code fixes in a document, e.g. to rename local parameters
    let getSymbolUsesOfSymbolAtLocationInDocument (document: Document, position: int, projectInfoManager: FSharpProjectOptionsManager, checker: FSharpChecker, userOpName) =
        asyncMaybe {
            let! cancellationToken = Async.CancellationToken |> liftAsync
            let! sourceText = document.GetTextAsync(cancellationToken)
            let! textVersion = document.GetTextVersionAsync(cancellationToken) 
            let textVersionHash = textVersion.GetHashCode()
            let textLine = sourceText.Lines.GetLineFromPosition(position)
            let textLinePos = sourceText.Lines.GetLinePosition(position)
            let fcsTextLineNumber = Line.fromZ textLinePos.Line
            let! parsingOptions, projectOptions = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document) 
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing(document.Name, parsingOptions)
            let! symbol = Tokenizer.getSymbolAtPosition(document.Id, sourceText, position, document.FilePath, defines, SymbolLookupKind.Greedy, false)
            let! _, _, checkFileResults = checker.ParseAndCheckDocument(document.FilePath, textVersionHash, sourceText.ToString(), projectOptions, allowStaleResults = true, userOpName = userOpName) 
            let! symbolUse = checkFileResults.GetSymbolUseAtLocation(fcsTextLineNumber, symbol.Ident.idRange.EndColumn, textLine.ToString(), symbol.FullIsland, userOpName=userOpName)
            let! symbolUses = checkFileResults.GetUsesOfSymbolInFile(symbolUse.Symbol) |> liftAsync
            return symbolUses
        }

    let getSymbolUsesInSolution (symbol: FSharpSymbol, declLoc: SymbolDeclarationLocation, checkFileResults: FSharpCheckFileResults,
                                 projectInfoManager: FSharpProjectOptionsManager, checker: FSharpChecker, solution: Solution, userOpName) =
        async {
            let! symbolUses =
                match declLoc with
                | SymbolDeclarationLocation.CurrentDocument ->
                    checkFileResults.GetUsesOfSymbolInFile(symbol)
                | SymbolDeclarationLocation.Projects (projects, isInternalToProject) -> 
                    let projects =
                        if isInternalToProject then projects
                        else 
                            [ for project in projects do
                                yield project
                                yield! project.GetDependentProjects() ]
                            |> List.distinctBy (fun x -> x.Id)

                    projects
                    |> Seq.map (fun project ->
                        async {
                            match projectInfoManager.TryGetOptionsForProject(project.Id) with
                            | Some (_parsingOptions, _site, projectOptions) ->
                                let! projectCheckResults = checker.ParseAndCheckProject(projectOptions, userOpName = userOpName)
                                return! projectCheckResults.GetUsesOfSymbol(symbol)
                            | None -> return [||]
                        })
                    |> Async.Parallel
                    |> Async.map Array.concat
            
            return
                (symbolUses
                 |> Seq.collect (fun symbolUse -> 
                      solution.GetDocumentIdsWithFilePath(symbolUse.FileName) |> Seq.map (fun id -> id, symbolUse))
                 |> Seq.groupBy fst
                ).ToImmutableDictionary(
                    (fun (id, _) -> id), 
                    fun (_, xs) -> xs |> Seq.map snd |> Seq.toArray)
        }

    type OriginalText = string

    // Note, this function is broken and shouldn't be used because the source text ranges to replace are applied sequentially,
    // breaking the position computations as changes progress, especially if two changes are made on the same line.  
    //
    // However, it is only currently used by ProposeUpperCaseLabel code fix, where the changes to code will rarely be on the same line.
    //
    // A better approach is to use something like createTextChangeCodeFix below, with a delayed function to compute a set of changes to be applied
    // simultaneously.  But that doesn't work for this case, as we want a set of changes to apply acrosss the whole solution.

    let changeAllSymbolReferences (document: Document, symbolSpan: TextSpan, textChanger: string -> string, projectInfoManager: FSharpProjectOptionsManager, checker: FSharpChecker, userOpName)
        : Async<(Func<CancellationToken, Task<Solution>> * OriginalText) option> =
        asyncMaybe {
            do! Option.guard (symbolSpan.Length > 0)
            let! cancellationToken = liftAsync Async.CancellationToken
            let! sourceText = document.GetTextAsync(cancellationToken)
            let originalText = sourceText.ToString(symbolSpan)
            do! Option.guard (originalText.Length > 0)
            let! parsingOptions, projectOptions = projectInfoManager.TryGetOptionsForEditingDocumentOrProject document
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing(document.Name, parsingOptions)
            let! symbol = Tokenizer.getSymbolAtPosition(document.Id, sourceText, symbolSpan.Start, document.FilePath, defines, SymbolLookupKind.Greedy, false)
            let! _, _, checkFileResults = checker.ParseAndCheckDocument(document, projectOptions, allowStaleResults = true, userOpName = userOpName)
            let textLine = sourceText.Lines.GetLineFromPosition(symbolSpan.Start)
            let textLinePos = sourceText.Lines.GetLinePosition(symbolSpan.Start)
            let fcsTextLineNumber = Line.fromZ textLinePos.Line
            let! symbolUse = checkFileResults.GetSymbolUseAtLocation(fcsTextLineNumber, symbol.Ident.idRange.EndColumn, textLine.Text.ToString(), symbol.FullIsland, userOpName=userOpName)
            let! declLoc = symbolUse.GetDeclarationLocation(document)
            let newText = textChanger originalText
            // defer finding all symbol uses throughout the solution
            return 
                Func<_,_>(fun (cancellationToken: CancellationToken) ->
                    async {
                        let! symbolUsesByDocumentId = 
                            getSymbolUsesInSolution(symbolUse.Symbol, declLoc, checkFileResults, projectInfoManager, checker, document.Project.Solution, userOpName)
                    
                        let mutable solution = document.Project.Solution
                        
                        for KeyValue(documentId, symbolUses) in symbolUsesByDocumentId do
                            let document = document.Project.Solution.GetDocument(documentId)
                            let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                            let mutable sourceText = sourceText
                            for symbolUse in symbolUses do
                                match RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, symbolUse.RangeAlternate) with 
                                | None -> ()
                                | Some span -> 
                                    let textSpan = Tokenizer.fixupSpan(sourceText, span)
                                    sourceText <- sourceText.Replace(textSpan, newText)
                                    solution <- solution.WithDocumentText(documentId, sourceText)
                        return solution
                    } |> RoslynHelpers.StartAsyncAsTask cancellationToken),
               originalText
        }

    let createTextChangeCodeFix (title: string, context: CodeFixContext, computeTextChanges: unit -> Async<TextChange[] option>) =
        CodeAction.Create(
            title,
            (fun (cancellationToken: CancellationToken) ->
                async {
                    let! cancellationToken = Async.CancellationToken
                    let! sourceText = context.Document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                    let! changesOpt = computeTextChanges()
                    match changesOpt with
                    | None -> return context.Document
                    | Some textChanges -> return context.Document.WithText(sourceText.WithChanges(textChanges))
                } |> RoslynHelpers.StartAsyncAsTask(cancellationToken)),
            title)

