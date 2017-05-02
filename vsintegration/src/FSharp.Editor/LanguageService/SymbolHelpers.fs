// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text

open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.VisualStudio.FSharp.Editor.Symbols 


module internal SymbolHelpers =
    let getSymbolUsesInSolution (symbol: FSharpSymbol, declLoc: SymbolDeclarationLocation, checkFileResults: FSharpCheckFileResults,
                                 projectInfoManager: ProjectInfoManager, checker: FSharpChecker, solution: Solution) =
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
                            | Some options ->
                                let! projectCheckResults = checker.ParseAndCheckProject(options)
                                return! projectCheckResults.GetUsesOfSymbol(symbol)
                            | None -> return [||]
                        })
                    |> Async.Parallel
                    |> Async.map Array.concat
            
            let declarationLength = 
                symbol.DeclarationLocation
                |> Option.map (fun m -> m.EndColumn - m.StartColumn)

            return
                (symbolUses
                 |> Seq.filter (fun su -> 
                     match declarationLength with
                     | Some declLength -> su.RangeAlternate.EndColumn - su.RangeAlternate.StartColumn = declLength
                     | None -> true)
                 |> Seq.collect (fun symbolUse -> 
                      solution.GetDocumentIdsWithFilePath(symbolUse.FileName) |> Seq.map (fun id -> id, symbolUse))
                 |> Seq.groupBy fst
                ).ToImmutableDictionary(
                    (fun (id, _) -> id), 
                    fun (_, xs) -> xs |> Seq.map snd |> Seq.toArray)
        }

    type OriginalText = string

    let changeAllSymbolReferences (document: Document, symbolSpan: TextSpan, textChanger: string -> string, projectInfoManager: ProjectInfoManager, checker: FSharpChecker)
        : Async<(Func<CancellationToken, Task<Solution>> * OriginalText) option> =
        asyncMaybe {
            do! Option.guard (symbolSpan.Length > 0)
            let! cancellationToken = liftAsync Async.CancellationToken
            let! sourceText = document.GetTextAsync(cancellationToken)
            let originalText = sourceText.ToString(symbolSpan)
            do! Option.guard (originalText.Length > 0)
            let! options = projectInfoManager.TryGetOptionsForEditingDocumentOrProject document
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing(document.Name, options.OtherOptions |> Seq.toList)
            let! symbol = Tokenizer.getSymbolAtPosition(document.Id, sourceText, symbolSpan.Start, document.FilePath, defines, SymbolLookupKind.Greedy, false)
            let! _, _, checkFileResults = checker.ParseAndCheckDocument(document, options, allowStaleResults = true)
            let textLine = sourceText.Lines.GetLineFromPosition(symbolSpan.Start)
            let textLinePos = sourceText.Lines.GetLinePosition(symbolSpan.Start)
            let fcsTextLineNumber = textLinePos.Line + 1
            let! symbolUse = checkFileResults.GetSymbolUseAtLocation(fcsTextLineNumber, symbol.Ident.idRange.EndColumn, textLine.Text.ToString(), symbol.FullIsland)
            let! declLoc = symbolUse.GetDeclarationLocation(document)
            let newText = textChanger originalText
            // defer finding all symbol uses throughout the solution
            return 
                Func<_,_>(fun (cancellationToken: CancellationToken) ->
                    async {
                        let! symbolUsesByDocumentId = 
                            getSymbolUsesInSolution(symbolUse.Symbol, declLoc, checkFileResults, projectInfoManager, checker, document.Project.Solution)
                    
                        let mutable solution = document.Project.Solution
                        
                        for KeyValue(documentId, symbolUses) in symbolUsesByDocumentId do
                            let document = document.Project.Solution.GetDocument(documentId)
                            let! sourceText = document.GetTextAsync(cancellationToken)
                            let mutable sourceText = sourceText
                            for symbolUse in symbolUses do
                                let textSpan = Tokenizer.fixupSpan(sourceText, RoslynHelpers.FSharpRangeToTextSpan(sourceText, symbolUse.RangeAlternate))
                                sourceText <- sourceText.Replace(textSpan, newText)
                                solution <- solution.WithDocumentText(documentId, sourceText)
                        return solution
                    } |> RoslynHelpers.StartAsyncAsTask cancellationToken),
               originalText
        }
