// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.CodeActions
open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Text.Tagging
open Microsoft.VisualStudio.Text.Formatting
open Microsoft.VisualStudio.Shell.Interop

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Parser
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices

open System.Windows.Documents

[<ExportCodeFixProvider(FSharpCommonConstants.FSharpLanguageName, Name = "ProposeUpperCaseLabel"); Shared>]
type internal FSharpProposeUpperCaseLabelCodeFixProvider
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider, 
        projectInfoManager: ProjectInfoManager
    ) =
    inherit CodeFixProvider()
    let fixableDiagnosticIds = ["FS0053"]
        
    override __.FixableDiagnosticIds = fixableDiagnosticIds.ToImmutableArray()

    override __.RegisterCodeFixesAsync context : Task =
        asyncMaybe {
            do! Option.guard (context.Span.Length > 0)
            let document = context.Document
            let! sourceText = document.GetTextAsync(context.CancellationToken)
            let originalText = sourceText.ToString(context.Span)
            do! Option.guard (originalText.Length > 0)
            let! options = projectInfoManager.TryGetOptionsForEditingDocumentOrProject document
            let! sourceText = document.GetTextAsync(context.CancellationToken)
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing(document.Name, options.OtherOptions |> Seq.toList)
            let! symbol = CommonHelpers.getSymbolAtPosition(document.Id, sourceText, context.Span.Start, document.FilePath, defines, SymbolLookupKind.Fuzzy)
            let checker = checkerProvider.Checker
            let! _, checkFileResults = checker.ParseAndCheckDocument(document, options)
            let textLine = sourceText.Lines.GetLineFromPosition(context.Span.Start)
            let textLinePos = sourceText.Lines.GetLinePosition(context.Span.Start)
            let fcsTextLineNumber = textLinePos.Line + 1
            let! symbolUse = checkFileResults.GetSymbolUseAtLocation(fcsTextLineNumber, symbol.RightColumn, textLine.Text.ToString(), [symbol.Text])
            let! declLoc = symbolUse.GetDeclarationLocation(document)
            let newText = originalText.[0].ToString().ToUpper() + originalText.Substring(1)
            let title = FSComp.SR.replaceWithSuggestion newText
            // defer finding all symbol uses throughout the solution until the code fix action is executed
            let codeFix = 
                CodeAction.Create(
                    title,
                    (fun (cancellationToken: CancellationToken) ->
                        async {
                            let! symbolUsesByDocumentId = 
                                SymbolHelpers.getSymbolUsesInSolution(symbolUse.Symbol, declLoc, checkFileResults, projectInfoManager, checker, document.Project.Solution)
            
                            let mutable solution = document.Project.Solution
                            
                            for KeyValue(documentId, symbolUses) in symbolUsesByDocumentId do
                                let document = document.Project.Solution.GetDocument(documentId)
                                let! sourceText = document.GetTextAsync(cancellationToken)
                                let mutable sourceText = sourceText
                                for symbolUse in symbolUses do
                                    let textSpan = CommonHelpers.fixupSpan(sourceText, CommonRoslynHelpers.FSharpRangeToTextSpan(sourceText, symbolUse.RangeAlternate))
                                    sourceText <- sourceText.Replace(textSpan, newText)
                                    solution <- solution.WithDocumentText(documentId, sourceText)
                            return solution
                        } |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)),
                    title)
            let diagnostics = (context.Diagnostics |> Seq.filter (fun x -> fixableDiagnosticIds |> List.contains x.Id)).ToImmutableArray()
            context.RegisterCodeFix(codeFix, diagnostics)
        } |> Async.ignore |> CommonRoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)