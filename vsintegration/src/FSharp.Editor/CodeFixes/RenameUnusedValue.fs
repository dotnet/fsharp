// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Threading.Tasks
open System.Collections.Immutable

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

open FSharp.Compiler.Symbols
open FSharp.Compiler.Syntax

open CancellableTasks

module UnusedCodeFixHelper =
    let getUnusedSymbol textSpan (document: Document) codeFixName =
        cancellableTask {
            let! token = CancellableTask.getCurrentCancellationToken ()
            let! sourceText = document.GetTextAsync token

            let ident = sourceText.ToString textSpan

            // Prefixing operators and backticked identifiers does not make sense.
            // We have to use the additional check for backtickes
            if PrettyNaming.IsIdentifierName ident then
                let! lexerSymbol =
                    document.TryFindFSharpLexerSymbolAsync(textSpan.Start, SymbolLookupKind.Greedy, false, false, CodeFix.RenameUnusedValue)

                let m = RoslynHelpers.TextSpanToFSharpRange(document.FilePath, textSpan, sourceText)

                let lineText = (sourceText.Lines.GetLineFromPosition textSpan.Start).ToString()

                let! _, checkResults = document.GetFSharpParseAndCheckResultsAsync codeFixName

                return
                    lexerSymbol
                    |> Option.bind (fun symbol -> checkResults.GetSymbolUseAtLocation(m.StartLine, m.EndColumn, lineText, symbol.FullIsland))
                    |> Option.bind (fun symbolUse ->
                        match symbolUse.Symbol with
                        | :? FSharpMemberOrFunctionOrValue as func when func.IsValue -> Some symbolUse.Symbol
                        | _ -> None)
            else
                return None
        }

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.PrefixUnusedValue); Shared>]
type internal PrefixUnusedValueWithUnderscoreCodeFixProvider [<ImportingConstructor>] () =

    inherit CodeFixProvider()

    static let getTitle (symbolName: string) =
        String.Format(SR.PrefixValueNameWithUnderscore(), symbolName)

    override _.FixableDiagnosticIds = ImmutableArray.Create "FS1182"

    override this.RegisterCodeFixesAsync context =
        if context.Document.Project.IsFSharpCodeFixesUnusedDeclarationsEnabled then
            context.RegisterFsharpFix this
        else
            Task.CompletedTask

    override this.GetFixAllProvider() = this.RegisterFsharpFixAll()

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync context =
            cancellableTask {
                let! symbol = UnusedCodeFixHelper.getUnusedSymbol context.Span context.Document CodeFix.PrefixUnusedValue

                return
                    symbol
                    |> Option.map (fun symbol ->
                        {
                            Name = CodeFix.PrefixUnusedValue
                            Message = getTitle symbol.DisplayName
                            Changes = [ TextChange(TextSpan(context.Span.Start, 0), "_") ]
                        })
            }

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = CodeFix.RenameUnusedValue); Shared>]
type internal RenameUnusedValueWithUnderscoreCodeFixProvider [<ImportingConstructor>] () =

    inherit CodeFixProvider()

    static let getTitle (symbolName: string) =
        String.Format(SR.RenameValueToUnderscore(), symbolName)

    override _.FixableDiagnosticIds = ImmutableArray.Create "FS1182"

    override this.RegisterCodeFixesAsync context =
        if context.Document.Project.IsFSharpCodeFixesUnusedDeclarationsEnabled then
            context.RegisterFsharpFix this
        else
            Task.CompletedTask

    override this.GetFixAllProvider() = this.RegisterFsharpFixAll()

    interface IFSharpCodeFixProvider with
        member _.GetCodeFixIfAppliesAsync context =
            cancellableTask {
                let! symbol = UnusedCodeFixHelper.getUnusedSymbol context.Span context.Document CodeFix.RenameUnusedValue

                return
                    symbol
                    |> Option.filter (fun symbol ->
                        match symbol with
                        | :? FSharpMemberOrFunctionOrValue as x when x.IsConstructorThisValue -> false
                        | _ -> true)
                    |> Option.map (fun symbol ->
                        {
                            Name = CodeFix.RenameUnusedValue
                            Message = getTitle symbol.DisplayName
                            Changes = [ TextChange(context.Span, "_") ]
                        })
            }
