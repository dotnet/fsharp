// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Threading

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.CodeActions
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.Range

[<RequireQualifiedAccess>]
module internal CodeFixHelpers =
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
     
    let getSymbolUse (context: CodeFixContext) (projectInfoManager: FSharpProjectOptionsManager) (checker: FSharpChecker) userOpName =
        asyncMaybe {
            let document = context.Document
            let! parsingOptions, projectOptions = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document, context.CancellationToken)
            let! sourceText = document.GetTextAsync(context.CancellationToken)
            let! _, _, checkResults = checker.ParseAndCheckDocument(document, projectOptions, sourceText = sourceText, userOpName=userOpName)
            let line = sourceText.Lines.GetLineFromPosition(context.Span.End)
            let linePos = sourceText.Lines.GetLinePosition(context.Span.End)
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing parsingOptions
            let! lexerSymbol = Tokenizer.getSymbolAtPosition(document.Id, sourceText, context.Span.End, document.FilePath, defines, SymbolLookupKind.Greedy, false)
            return! checkResults.GetSymbolUseAtLocation(Line.fromZ linePos.Line, lexerSymbol.Ident.idRange.EndColumn, line.ToString(), lexerSymbol.FullIsland, userOpName=userOpName)
        } |> liftAsync