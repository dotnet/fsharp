// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.Text

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = "UseMutationWhenValueIsMutable"); Shared>]
type internal FSharpUseMutationWhenValueIsMutableFixProvider
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider, 
        projectInfoManager: FSharpProjectOptionsManager
    ) =
    inherit CodeFixProvider()

    static let userOpName = "UseMutationWhenValueIsMutable"

    let fixableDiagnosticIds = set ["FS0020"]

    override _.FixableDiagnosticIds = Seq.toImmutableArray fixableDiagnosticIds

    override _.RegisterCodeFixesAsync context : Task =
        asyncMaybe {
            let diagnostics =
                context.Diagnostics
                |> Seq.filter (fun x -> fixableDiagnosticIds |> Set.contains x.Id)
                |> Seq.toImmutableArray

            let document = context.Document
            do! Option.guard (not(isSignatureFile document.FilePath))
            let position = context.Span.Start
            let checker = checkerProvider.Checker
            let! parsingOptions, projectOptions = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document, CancellationToken.None, userOpName)
            let! sourceText = document.GetTextAsync () |> liftTaskAsync
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing parsingOptions
            let textLine = sourceText.Lines.GetLineFromPosition position
            let textLinePos = sourceText.Lines.GetLinePosition position
            let fcsTextLineNumber = Line.fromZ textLinePos.Line
            let! _, _, checkFileResults = checker.ParseAndCheckDocument (document, projectOptions, sourceText=sourceText, userOpName=userOpName)
            let! lexerSymbol = Tokenizer.getSymbolAtPosition (document.Id, sourceText, position, document.FilePath, defines, SymbolLookupKind.Greedy, false, false)
            let! symbolUse = checkFileResults.GetSymbolUseAtLocation(fcsTextLineNumber, lexerSymbol.Ident.idRange.EndColumn, textLine.ToString(), lexerSymbol.FullIsland)

            match symbolUse.Symbol with
            | :? FSharpMemberOrFunctionOrValue as mfv when mfv.IsValue && mfv.IsMutable ->
                let title = SR.UseMutationWhenValueIsMutable()
                let! symbolSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, symbolUse.RangeAlternate)
                let mutable pos = symbolSpan.End
                let mutable ch = sourceText.GetSubText(pos).ToString()

                // We're looking for the possibly erroneous '='
                while pos <= context.Span.Length && ch <> "=" do
                    pos <- pos + 1
                    ch <- sourceText.GetSubText(pos).ToString()

                let codeFix =
                    CodeFixHelpers.createTextChangeCodeFix(
                        title,
                        context,
                        (fun () -> asyncMaybe.Return [| TextChange(TextSpan(pos + 1, 1), "<-") |]))

                context.RegisterCodeFix(codeFix, diagnostics)
            | _ -> ()
        }
        |> Async.Ignore
        |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)  