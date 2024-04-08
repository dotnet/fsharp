// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Threading.Tasks
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text
open FSharp.Compiler.Symbols

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.AddTypeAnnotationToObjectOfIndeterminateType); Shared>]
type internal AddTypeAnnotationToObjectOfIndeterminateTypeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    static let title = SR.AddTypeAnnotation()

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS0072", "FS3245")

    override _.RegisterCodeFixesAsync context : Task =
        asyncMaybe {

            let document = context.Document
            let position = context.Span.Start

            let! sourceText = document.GetTextAsync(context.CancellationToken)
            let textLine = sourceText.Lines.GetLineFromPosition position
            let textLinePos = sourceText.Lines.GetLinePosition position
            let fcsTextLineNumber = Line.fromZ textLinePos.Line

            let! lexerSymbol =
                document.TryFindFSharpLexerSymbolAsync(
                    position,
                    SymbolLookupKind.Greedy,
                    false,
                    false,
                    nameof (AddTypeAnnotationToObjectOfIndeterminateTypeFixProvider)
                )

            let! _, checkFileResults =
                document.GetFSharpParseAndCheckResultsAsync(nameof (AddTypeAnnotationToObjectOfIndeterminateTypeFixProvider))
                |> liftAsync

            let decl =
                checkFileResults.GetDeclarationLocation(
                    fcsTextLineNumber,
                    lexerSymbol.Ident.idRange.EndColumn,
                    textLine.ToString(),
                    lexerSymbol.FullIsland,
                    false
                )

            match decl with
            | FindDeclResult.DeclFound declRange when declRange.FileName = document.FilePath ->
                let! declSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, declRange)
                let declTextLine = sourceText.Lines.GetLineFromPosition declSpan.Start

                let! symbolUse =
                    checkFileResults.GetSymbolUseAtLocation(
                        declRange.StartLine,
                        declRange.EndColumn,
                        declTextLine.ToString(),
                        lexerSymbol.FullIsland
                    )

                match symbolUse.Symbol with
                | :? FSharpMemberOrFunctionOrValue as mfv ->
                    let typeString = mfv.FullType.FormatWithConstraints symbolUse.DisplayContext

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

                        let hasLeftParen = leftLoop sourceText.[declSpan.Start - 1] (declSpan.Start - 1)
                        let hasRightParen = rightLoop sourceText.[declSpan.End] declSpan.End
                        hasLeftParen && hasRightParen

                    let changes =
                        [
                            if alreadyWrappedInParens then
                                TextChange(TextSpan(declSpan.End, 0), ": " + typeString)
                            else
                                TextChange(TextSpan(declSpan.Start, 0), "(")
                                TextChange(TextSpan(declSpan.End + 1, 0), ": " + typeString + ")")
                        ]

                    context.RegisterFsharpFix(CodeFix.AddTypeAnnotationToObjectOfIndeterminateType, title, changes)

                | _ -> ()
            | _ -> ()
        }
        |> Async.Ignore
        |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
