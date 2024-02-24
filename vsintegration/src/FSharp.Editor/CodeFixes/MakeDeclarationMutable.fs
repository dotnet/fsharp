// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open FSharp.Compiler.EditorServices

open CancellableTasks

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.MakeDeclarationMutable); Shared>]
type internal MakeDeclarationMutableCodeFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    static let title = SR.MakeDeclarationMutable()

    override _.FixableDiagnosticIds = ImmutableArray.Create "FS0027"

    override this.RegisterCodeFixesAsync context = context.RegisterFsharpFix this

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync context =
            cancellableTask {
                let document = context.Document

                if isSignatureFile document.FilePath then
                    return ValueNone
                else
                    let position = context.Span.Start

                    let! lexerSymbolOpt =
                        document.TryFindFSharpLexerSymbolAsync(
                            position,
                            SymbolLookupKind.Greedy,
                            false,
                            false,
                            nameof MakeDeclarationMutableCodeFixProvider
                        )

                    match lexerSymbolOpt with
                    | None -> return ValueNone
                    | Some lexerSymbol ->
                        let! sourceText = context.GetSourceTextAsync()

                        let! fcsTextLineNumber, textLine = context.GetLineNumberAndText position

                        let! parseFileResults, checkFileResults =
                            document.GetFSharpParseAndCheckResultsAsync(nameof MakeDeclarationMutableCodeFixProvider)

                        let decl =
                            checkFileResults.GetDeclarationLocation(
                                fcsTextLineNumber,
                                lexerSymbol.Ident.idRange.EndColumn,
                                textLine,
                                lexerSymbol.FullIsland,
                                false
                            )

                        match decl with
                        | FindDeclResult.DeclFound declRange when
                            // Only do this for symbols in the same file. That covers almost all cases anyways.
                            // We really shouldn't encourage making values mutable outside of local scopes anyways.
                            declRange.FileName = document.FilePath
                            // Bail if it's a parameter, because like, that ain't allowed
                            && not <| parseFileResults.IsPositionContainedInACurriedParameter declRange.Start
                            ->
                            let span = RoslynHelpers.FSharpRangeToTextSpan(sourceText, declRange)

                            return
                                ValueSome
                                    {
                                        Name = CodeFix.MakeDeclarationMutable
                                        Message = title
                                        Changes = [ TextChange(TextSpan(span.Start, 0), "mutable ") ]
                                    }
                        | _ -> return ValueNone
            }
