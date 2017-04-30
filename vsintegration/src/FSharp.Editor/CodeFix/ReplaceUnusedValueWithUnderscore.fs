// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.CodeActions

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.SourceCodeServices

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = "ReplaceUnusedValueWithUnderscore"); Shared>]
type internal FSharpReplaceUnusedValueWithUnderscoreCodeFixProvider
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider, 
        projectInfoManager: ProjectInfoManager
    ) =
    
    inherit CodeFixProvider()
    let fixableDiagnosticIds = ["FS1182"]
        
    let createCodeFix (title: string, context: CodeFixContext, textChange: TextChange) =
        CodeAction.Create(
            title,
            (fun (cancellationToken: CancellationToken) ->
                async {
                    let! sourceText = context.Document.GetTextAsync()
                    return context.Document.WithText(sourceText.WithChanges(textChange))
                } |> RoslynHelpers.StartAsyncAsTask(cancellationToken)),
            title)

    override __.FixableDiagnosticIds = Seq.toImmutableArray fixableDiagnosticIds

    override __.RegisterCodeFixesAsync context : Task =
        asyncMaybe {
            let document = context.Document
            let! sourceText = document.GetTextAsync()
            let ident = sourceText.ToString(context.Span)
            // Prefixing operators and backticked identifiers does not make sense.
            // We have to use the additional check for backtickes because `IsOperatorOrBacktickedName` operates on display names
            // where backtickes are replaced with parens.
            if not (PrettyNaming.IsOperatorOrBacktickedName ident) && not (ident.StartsWith "``") then
                let! options = projectInfoManager.TryGetOptionsForEditingDocumentOrProject document
                let! _, _, checkResults = checkerProvider.Checker.ParseAndCheckDocument(document, options, allowStaleResults = true, sourceText = sourceText)
                let m = RoslynHelpers.TextSpanToFSharpRange(document.FilePath, context.Span, sourceText)
                let defines = CompilerEnvironment.GetCompilationDefinesForEditing (document.FilePath, options.OtherOptions |> Seq.toList)
                let! lexerSymbol = Tokenizer.getSymbolAtPosition (document.Id, sourceText, context.Span.Start, document.FilePath, defines, SymbolLookupKind.Greedy, false)
                let lineText = (sourceText.Lines.GetLineFromPosition context.Span.Start).ToString()  
                let! symbolUse = checkResults.GetSymbolUseAtLocation(m.StartLine, m.EndColumn, lineText, lexerSymbol.FullIsland)
                let diagnostics = context.Diagnostics |> Seq.filter (fun x -> fixableDiagnosticIds |> List.contains x.Id) |> Seq.toImmutableArray
                
                match symbolUse.Symbol with
                | :? FSharpMemberOrFunctionOrValue as func when func.IsMemberThisValue ->
                    context.RegisterCodeFix(createCodeFix(SR.RenameValueToDoubleUnderscore.Value, context, TextChange(context.Span, "__")), diagnostics)
                | _ ->
                    context.RegisterCodeFix(createCodeFix(SR.RenameValueToUnderscore.Value, context, TextChange(context.Span, "_")), diagnostics)
        } 
        |> Async.Ignore
        |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)