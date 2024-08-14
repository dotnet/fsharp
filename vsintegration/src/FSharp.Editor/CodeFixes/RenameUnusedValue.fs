// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Threading
open System.Threading.Tasks
open System.Collections.Immutable

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open FSharp.Compiler.Symbols
open FSharp.Compiler.Syntax

module UnusedCodeFixHelper =
    let getUnusedSymbol (sourceText: SourceText) (textSpan: TextSpan) (document: Document) =

        let ident = sourceText.ToString(textSpan)

        // Prefixing operators and backticked identifiers does not make sense.
        // We have to use the additional check for backtickes
        if PrettyNaming.IsIdentifierName ident then
            asyncMaybe {
                let! lexerSymbol =
                    document.TryFindFSharpLexerSymbolAsync(textSpan.Start, SymbolLookupKind.Greedy, false, false, CodeFix.RenameUnusedValue)

                let m = RoslynHelpers.TextSpanToFSharpRange(document.FilePath, textSpan, sourceText)

                let lineText = (sourceText.Lines.GetLineFromPosition textSpan.Start).ToString()

                let! _, checkResults =
                    document.GetFSharpParseAndCheckResultsAsync(CodeFix.RenameUnusedValue)
                    |> liftAsync

                return! checkResults.GetSymbolUseAtLocation(m.StartLine, m.EndColumn, lineText, lexerSymbol.FullIsland)

            }
        else
            async { return None }

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.PrefixUnusedValue); Shared>]
type internal PrefixUnusedValueWithUnderscoreCodeFixProvider [<ImportingConstructor>] () =

    inherit CodeFixProvider()

    static let title (symbolName: string) =
        String.Format(SR.PrefixValueNameWithUnderscore(), symbolName)

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS1182")

    member this.GetChanges(document: Document, diagnostics: ImmutableArray<Diagnostic>, ct: CancellationToken) =
        backgroundTask {
            let! sourceText = document.GetTextAsync(ct)

            let! changes =
                seq {
                    for d in diagnostics do
                        let textSpan = d.Location.SourceSpan

                        yield
                            async {
                                let! symbolUse = UnusedCodeFixHelper.getUnusedSymbol sourceText textSpan document

                                return
                                    seq {
                                        match symbolUse with
                                        | None -> ()
                                        | Some symbolUse ->
                                            match symbolUse.Symbol with
                                            | :? FSharpMemberOrFunctionOrValue -> yield TextChange(TextSpan(textSpan.Start, 0), "_")
                                            | _ -> ()
                                    }
                            }
                }
                |> Async.Parallel

            return (changes |> Seq.concat)
        }

    override this.RegisterCodeFixesAsync ctx : Task =
        backgroundTask {
            if ctx.Document.Project.IsFSharpCodeFixesUnusedDeclarationsEnabled then
                let! sourceText = ctx.Document.GetTextAsync(ctx.CancellationToken)
                let! unusedSymbol = UnusedCodeFixHelper.getUnusedSymbol sourceText ctx.Span ctx.Document

                match unusedSymbol with
                | None -> ()
                | Some symbolUse ->
                    match symbolUse.Symbol with
                    | :? FSharpMemberOrFunctionOrValue ->
                        let prefixTitle = title symbolUse.Symbol.DisplayName
                        let! changes = this.GetChanges(ctx.Document, ctx.Diagnostics, ctx.CancellationToken)
                        ctx.RegisterFsharpFix(CodeFix.PrefixUnusedValue, prefixTitle, changes)
                    | _ -> ()
        }

    override this.GetFixAllProvider() =
        CodeFixHelpers.createFixAllProvider CodeFix.PrefixUnusedValue this.GetChanges

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.RenameUnusedValue); Shared>]
type internal FSharpRenameUnusedValueWithUnderscoreCodeFixProvider [<ImportingConstructor>] () =

    inherit CodeFixProvider()

    static let title (symbolName: string) =
        String.Format(SR.RenameValueToUnderscore(), symbolName)

    override _.FixableDiagnosticIds = ImmutableArray.Create("FS1182")

    member this.GetChanges(document: Document, diagnostics: ImmutableArray<Diagnostic>, ct: CancellationToken) =
        backgroundTask {
            let! sourceText = document.GetTextAsync(ct)

            let! changes =
                seq {
                    for d in diagnostics do
                        let textSpan = d.Location.SourceSpan

                        yield
                            async {
                                let! symbolUse = UnusedCodeFixHelper.getUnusedSymbol sourceText textSpan document

                                return
                                    seq {
                                        match symbolUse with
                                        | None -> ()
                                        | Some symbolUse ->
                                            match symbolUse.Symbol with
                                            | :? FSharpMemberOrFunctionOrValue as func when func.IsValue -> yield TextChange(textSpan, "_")
                                            | _ -> ()
                                    }
                            }
                }
                |> Async.Parallel

            return (changes |> Seq.concat)
        }

    override this.RegisterCodeFixesAsync ctx : Task =
        backgroundTask {
            if ctx.Document.Project.IsFSharpCodeFixesUnusedDeclarationsEnabled then
                let! sourceText = ctx.Document.GetTextAsync(ctx.CancellationToken)
                let! unusedSymbol = UnusedCodeFixHelper.getUnusedSymbol sourceText ctx.Span ctx.Document

                match unusedSymbol with
                | None -> ()
                | Some symbolUse ->
                    match symbolUse.Symbol with
                    | :? FSharpMemberOrFunctionOrValue as func when func.IsValue ->
                        let prefixTitle = title symbolUse.Symbol.DisplayName

                        let! changes = this.GetChanges(ctx.Document, ctx.Diagnostics, ctx.CancellationToken)
                        ctx.RegisterFsharpFix(CodeFix.RenameUnusedValue, prefixTitle, changes)
                    | _ -> ()
        }

    override this.GetFixAllProvider() =
        CodeFixHelpers.createFixAllProvider CodeFix.RenameUnusedValue this.GetChanges
