// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading.Tasks
open System.Collections.Immutable

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.MakeDeclarationMutable); Shared>]
type internal MakeDeclarationMutableFixProvider [<ImportingConstructor>] () =
    inherit CodeFixProvider()

    static let title = SR.MakeDeclarationMutable()

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS0027")

    override _.RegisterCodeFixesAsync context : Task =
        asyncMaybe {

            let document = context.Document
            do! Option.guard (not (isSignatureFile document.FilePath))
            let position = context.Span.Start

            let! lexerSymbol =
                document.TryFindFSharpLexerSymbolAsync(
                    position,
                    SymbolLookupKind.Greedy,
                    false,
                    false,
                    nameof (MakeDeclarationMutableFixProvider)
                )

            let! sourceText = document.GetTextAsync() |> liftTaskAsync
            let textLine = sourceText.Lines.GetLineFromPosition position
            let textLinePos = sourceText.Lines.GetLinePosition position
            let fcsTextLineNumber = Line.fromZ textLinePos.Line

            let! parseFileResults, checkFileResults =
                document.GetFSharpParseAndCheckResultsAsync(nameof (MakeDeclarationMutableFixProvider))
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
            // Only do this for symbols in the same file. That covers almost all cases anyways.
            // We really shouldn't encourage making values mutable outside of local scopes anyways.
            | FindDeclResult.DeclFound declRange when declRange.FileName = document.FilePath ->
                let! span = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, declRange)

                // Bail if it's a parameter, because like, that ain't allowed
                do! Option.guard (not (parseFileResults.IsPositionContainedInACurriedParameter declRange.Start))
                do context.RegisterFsharpFix(CodeFix.MakeDeclarationMutable, title, [| TextChange(TextSpan(span.Start, 0), "mutable ") |])
            | _ -> ()
        }
        |> Async.Ignore
        |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
