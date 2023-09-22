// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open FSharp.Compiler.EditorServices
open FSharp.Compiler.Symbols

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.AddTypeAnnotationToObjectOfIndeterminateType); Shared>]
type internal AddTypeAnnotationToObjectOfIndeterminateTypeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    static let title = SR.AddTypeAnnotation()

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS0072", "FS3245")

    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFix this

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync context =
            cancellableTask {
                let document = context.Document
                let position = context.Span.Start

                let! lexerSymbolOpt =
                    document.TryFindFSharpLexerSymbolAsync(
                        position,
                        SymbolLookupKind.Greedy,
                        false,
                        false,
                        nameof AddTypeAnnotationToObjectOfIndeterminateTypeFixProvider
                    )

                match lexerSymbolOpt with
                | None -> return ValueNone
                | Some lexerSymbol ->
                    let! fcsTextLineNumber, textLine = context.GetLineNumberAndText position

                    let! _, checkFileResults =
                        document.GetFSharpParseAndCheckResultsAsync(nameof AddTypeAnnotationToObjectOfIndeterminateTypeFixProvider)

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
                        let! sourceText = context.GetSourceTextAsync()
                        let declSpan = RoslynHelpers.FSharpRangeToTextSpan(sourceText, declRange)
                        let declTextLine = sourceText.Lines.GetLineFromPosition declSpan.Start

                        let symbolUseOpt =
                            checkFileResults.GetSymbolUseAtLocation(
                                declRange.StartLine,
                                declRange.EndColumn,
                                declTextLine.ToString(),
                                lexerSymbol.FullIsland
                            )

                        match symbolUseOpt with
                        | None -> return ValueNone
                        | Some symbolUse ->
                            match symbolUse.Symbol with
                            | :? FSharpMemberOrFunctionOrValue as mfv when not mfv.FullType.IsGenericParameter ->
                                let typeString = mfv.FullType.FormatWithConstraints symbolUse.DisplayContext

                                let alreadyWrappedInParens =
                                    let rec leftLoop ch pos =
                                        if not (Char.IsWhiteSpace(ch)) then
                                            ch = '('
                                        else
                                            leftLoop sourceText[pos - 1] (pos - 1)

                                    let rec rightLoop ch pos =
                                        if not (Char.IsWhiteSpace(ch)) then
                                            ch = ')'
                                        else
                                            rightLoop sourceText[pos + 1] (pos + 1)

                                    let hasLeftParen = leftLoop sourceText[declSpan.Start - 1] (declSpan.Start - 1)
                                    let hasRightParen = rightLoop sourceText[declSpan.End] declSpan.End
                                    hasLeftParen && hasRightParen

                                let changes =
                                    [
                                        if alreadyWrappedInParens then
                                            TextChange(TextSpan(declSpan.End, 0), ": " + typeString)
                                        else
                                            TextChange(TextSpan(declSpan.Start, 0), "(")
                                            TextChange(TextSpan(declSpan.End, 0), ": " + typeString + ")")
                                    ]

                                return
                                    ValueSome
                                        {
                                            Name = CodeFix.AddTypeAnnotationToObjectOfIndeterminateType
                                            Message = title
                                            Changes = changes
                                        }

                            | _ -> return ValueNone

                    | _ -> return ValueNone
            }
