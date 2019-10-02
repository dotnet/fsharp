// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.CodeActions

open FSharp.Compiler
open FSharp.Compiler.SourceCodeServices

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = "RenameUnusedValue"); Shared>]
type internal FSharpRenameUnusedValueCodeFixProvider
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider, 
        projectInfoManager: FSharpProjectOptionsManager
    ) =
    
    inherit CodeFixProvider()
    static let userOpName = "RenameUnusedValueCodeFix"
    let fixableDiagnosticIds = ["FS1182"]
    let checker = checkerProvider.Checker
        
    let createCodeFix (context: CodeFixContext, symbolName: string, titleFormat: string, textChange: TextChange) =
        let title = String.Format(titleFormat, symbolName)
        let codeAction =
            CodeAction.Create(
                title,
                (fun (cancellationToken: CancellationToken) ->
                    async {
                        let! cancellationToken = Async.CancellationToken
                        let! sourceText = context.Document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                        return context.Document.WithText(sourceText.WithChanges(textChange))
                    } |> RoslynHelpers.StartAsyncAsTask(cancellationToken)),
                title)
        let diagnostics = context.Diagnostics |> Seq.filter (fun x -> fixableDiagnosticIds |> List.contains x.Id) |> Seq.toImmutableArray
        context.RegisterCodeFix(codeAction, diagnostics)

    override __.FixableDiagnosticIds = Seq.toImmutableArray fixableDiagnosticIds

    override __.RegisterCodeFixesAsync context : Task =
        asyncMaybe {
            // Don't show code fixes for unused values, even if they are compiler-generated.
            do! Option.guard context.Document.FSharpOptions.CodeFixes.UnusedDeclarations

            let document = context.Document
            let! sourceText = document.GetTextAsync()
            let ident = sourceText.ToString(context.Span)
            // Prefixing operators and backticked identifiers does not make sense.
            // We have to use the additional check for backtickes because `IsOperatorOrBacktickedName` operates on display names
            // where backtickes are replaced with parens.
            if not (PrettyNaming.IsOperatorOrBacktickedName ident) && not (ident.StartsWith "``") then
                let! parsingOptions, projectOptions = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document, context.CancellationToken)
                let! _, _, checkResults = checker.ParseAndCheckDocument(document, projectOptions, sourceText = sourceText, userOpName=userOpName)
                let m = RoslynHelpers.TextSpanToFSharpRange(document.FilePath, context.Span, sourceText)
                let defines = CompilerEnvironment.GetCompilationDefinesForEditing parsingOptions
                let! lexerSymbol = Tokenizer.getSymbolAtPosition (document.Id, sourceText, context.Span.Start, document.FilePath, defines, SymbolLookupKind.Greedy, false)
                let lineText = (sourceText.Lines.GetLineFromPosition context.Span.Start).ToString()  
                let! symbolUse = checkResults.GetSymbolUseAtLocation(m.StartLine, m.EndColumn, lineText, lexerSymbol.FullIsland, userOpName=userOpName)
                let symbolName = symbolUse.Symbol.DisplayName

                match symbolUse.Symbol with
                | :? FSharpMemberOrFunctionOrValue as func ->
                    createCodeFix(context, symbolName, SR.PrefixValueNameWithUnderscore(), TextChange(TextSpan(context.Span.Start, 0), "_"))
                    if func.IsValue then createCodeFix(context, symbolName, SR.RenameValueToUnderscore(), TextChange(context.Span, "_"))
                | _ -> ()
        } 
        |> Async.Ignore
        |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
