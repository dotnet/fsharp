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
        async {
            if context.Span.Length > 0 then
                let document = context.Document
                let! sourceText = document.GetTextAsync(context.CancellationToken)
                let originalText = sourceText.ToString(context.Span)
                if originalText.Length > 0 then
                    match projectInfoManager.TryGetOptionsForEditingDocumentOrProject context.Document with 
                    | Some options ->
                        let! sourceText = context.Document.GetTextAsync(context.CancellationToken)
                        let defines = CompilerEnvironment.GetCompilationDefinesForEditing(document.Name, options.OtherOptions |> Seq.toList)
                        match CommonHelpers.getSymbolAtPosition(document.Id, sourceText, context.Span.Start, document.FilePath, defines, SymbolLookupKind.Fuzzy) with 
                        | Some symbol -> 
                            let! textVersion = document.GetTextVersionAsync(context.CancellationToken)
                            let checker = checkerProvider.Checker
                            let! _, checkFileAnswer = checker.ParseAndCheckFileInProject(context.Document.FilePath, textVersion.GetHashCode(), sourceText.ToString(), options)
                            match checkFileAnswer with
                            | FSharpCheckFileAnswer.Aborted -> ()
                            | FSharpCheckFileAnswer.Succeeded checkFileResults ->
                                let textLine = sourceText.Lines.GetLineFromPosition(context.Span.Start)
                                let textLinePos = sourceText.Lines.GetLinePosition(context.Span.Start)
                                let fcsTextLineNumber = textLinePos.Line + 1
                                let! symbolUse = checkFileResults.GetSymbolUseAtLocation(fcsTextLineNumber, symbol.RightColumn, textLine.Text.ToString(), [symbol.Text])
                                match symbolUse with
                                | Some symbolUse ->
                                    match symbolUse.GetDeclarationLocation(document) with
                                    | None -> ()
                                    | Some declLoc ->
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
                                | None -> ()
                        | None -> ()
                    | _ -> ()
        } |> CommonRoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)