// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open FSharp.Compiler
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text
open FSharp.Compiler.Symbols
open Microsoft.CodeAnalysis.CodeActions

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = "AddTypeAnnotationToObjectOfIndeterminateType"); Shared>]
type internal FSharpAddTypeAnnotationToObjectOfIndeterminateTypeFixProvider
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider, 
        projectInfoManager: FSharpProjectOptionsManager
    ) =
    inherit CodeFixProvider()

    static let userOpName = "AddTypeAnnotationToObjectOfIndeterminateType"

    let fixableDiagnosticIds = set ["FS0072"; "FS3245"]

    override _.FixableDiagnosticIds = Seq.toImmutableArray fixableDiagnosticIds

    override _.RegisterCodeFixesAsync context : Task =
        asyncMaybe {
            let diagnostics =
                context.Diagnostics
                |> Seq.filter (fun x -> fixableDiagnosticIds |> Set.contains x.Id)
                |> Seq.toImmutableArray

            let document = context.Document
            let position = context.Span.Start
            let checker = checkerProvider.Checker
            let! parsingOptions, projectOptions = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document, context.CancellationToken, userOpName)
            let! sourceText = document.GetTextAsync () |> liftTaskAsync
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing parsingOptions
            let textLine = sourceText.Lines.GetLineFromPosition position
            let textLinePos = sourceText.Lines.GetLinePosition position
            let lineText = textLine.ToString()
            let fcsTextLineNumber = Line.fromZ textLinePos.Line
            let! _, _, checkFileResults = checker.ParseAndCheckDocument (document, projectOptions, sourceText=sourceText, userOpName=userOpName)
            let! lexerSymbol = Tokenizer.getSymbolAtPosition (document.Id, sourceText, position, document.FilePath, defines, SymbolLookupKind.Greedy, false, false)
            let decl = checkFileResults.GetDeclarationLocation (fcsTextLineNumber, lexerSymbol.Ident.idRange.StartColumn, lineText, lexerSymbol.FullIsland, false)

            match decl with
            // Only do this for symbols in the same file. That covers almost all cases anyways.
            // We really shouldn't encourage making values mutable outside of local scopes anyways.
            | FindDeclResult.DeclFound declRange when declRange.FileName = document.FilePath ->
                let! symbolUse = checkFileResults.GetSymbolUseAtLocation(fcsTextLineNumber, lexerSymbol.Ident.idRange.StartColumn, lineText, lexerSymbol.FullIsland)
                let! symbolSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, declRange)
                match symbolUse.Symbol with
                | :? FSharpMemberOrFunctionOrValue as mfv ->
                    let typeString = mfv.FullType.FormatWithConstraints symbolUse.DisplayContext
                    if not mfv.FullType.IsGenericParameter then

                        let alreadyWrappedInParens =
                            let rec leftLoop ch pos =
                                if not (Char.IsWhiteSpace(ch)) then
                                    ch = '('
                                else
                                    leftLoop sourceText.[pos - 1] (pos - 1)

                            let rec rightLoop ch pos =
                                if not (Char.IsWhiteSpace(ch)) then
                                    ch = ')'
                                else
                                    rightLoop sourceText.[pos + 1] (pos + 1)

                            let hasLeftParen = leftLoop sourceText.[symbolSpan.Start - 1] (symbolSpan.Start - 1)
                            let hasRightParen = rightLoop sourceText.[symbolSpan.End] symbolSpan.End
                            hasLeftParen && hasRightParen
                            
                        let getChangedText (sourceText: SourceText) =
                            if alreadyWrappedInParens then
                                sourceText.WithChanges(TextChange(TextSpan(symbolSpan.End, 0), ": " + typeString))
                            else
                                sourceText.WithChanges(TextChange(TextSpan(symbolSpan.Start, 0), "("))
                                            .WithChanges(TextChange(TextSpan(symbolSpan.End + 1, 0), ": " + typeString + ")"))

                        let title = SR.AddTypeAnnotation()
                        let codeAction =
                            CodeAction.Create(
                                title,
                                (fun (cancellationToken: CancellationToken) ->
                                    async {
                                        let! sourceText = context.Document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                                        return context.Document.WithText(getChangedText sourceText)
                                    } |> RoslynHelpers.StartAsyncAsTask(cancellationToken)),
                                title)

                        context.RegisterCodeFix(codeAction, diagnostics)
                |_ -> ()
            | _ -> ()
        }
        |> Async.Ignore
        |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken) 