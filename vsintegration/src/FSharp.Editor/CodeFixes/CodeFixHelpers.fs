// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Immutable
open System.Diagnostics

open Microsoft
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.CodeActions
open Microsoft.VisualStudio.FSharp.Editor.Telemetry

open FSharp.Compiler.Symbols
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text

open CancellableTasks

module internal UnusedCodeFixHelper =
    let getUnusedSymbol textSpan (document: Document) (sourceText: SourceText) codeFixName =
        let ident = sourceText.ToString textSpan

        // Prefixing operators and backticked identifiers does not make sense.
        // We have to use the additional check for backticks
        if PrettyNaming.IsIdentifierName ident then
            cancellableTask {
                let! lexerSymbol =
                    document.TryFindFSharpLexerSymbolAsync(textSpan.Start, SymbolLookupKind.Greedy, false, false, CodeFix.RenameUnusedValue)

                let m = RoslynHelpers.TextSpanToFSharpRange(document.FilePath, textSpan, sourceText)

                let lineText = (sourceText.Lines.GetLineFromPosition textSpan.Start).ToString()

                let! _, checkResults = document.GetFSharpParseAndCheckResultsAsync codeFixName

                return
                    lexerSymbol
                    |> Option.bind (fun symbol ->
                        checkResults.GetSymbolUseAtLocation(m.StartLine, m.EndColumn, lineText, symbol.FullIsland))
                    |> ValueOption.ofOption
                    |> ValueOption.bind (fun symbolUse ->
                        match symbolUse.Symbol with
                        | :? FSharpMemberOrFunctionOrValue as func when func.IsValue -> ValueSome symbolUse.Symbol
                        | _ -> ValueNone)
            }
        else
            CancellableTask.singleton ValueNone

[<RequireQualifiedAccess>]
module internal CodeFixHelpers =
    let reportCodeFixTelemetry
        (diagnostics: ImmutableArray<Diagnostic>)
        (doc: Document)
        (staticName: string)
        (additionalProps: (string * obj) array)
        =
        let ids =
            diagnostics |> Seq.map (fun d -> d.Id) |> Seq.distinct |> String.concat ","

        let defaultProps: (string * obj) array =
            [|
                "name", staticName
                "ids", ids
                "context.document.project.id", doc.Project.Id.Id.ToString()
                "context.document.id", doc.Id.Id.ToString()
                "context.diagnostics.count", diagnostics.Length
            |]

        let props: (string * obj) array = Array.concat [ additionalProps; defaultProps ]

        TelemetryReporter.ReportSingleEvent(TelemetryEvents.CodefixActivated, props)

    let createTextChangeCodeFix (codeFix, context: CodeFixContext) =
        CodeAction.Create(
            codeFix.Message,
            (fun cancellationToken ->
                cancellableTask {
                    let! sourceText = context.Document.GetTextAsync cancellationToken
                    let doc = context.Document.WithText(sourceText.WithChanges(codeFix.Changes))
                    reportCodeFixTelemetry context.Diagnostics context.Document codeFix.Name [||]
                    return doc
                }
                |> CancellableTask.start cancellationToken)
        )

[<AutoOpen>]
module internal CodeFixExtensions =
    type CodeFixContext with

        member ctx.RegisterFsharpFix(codeFix: IFSharpCodeFixProvider) =
            cancellableTask {
                match! codeFix.GetCodeFixIfAppliesAsync ctx with
                | ValueSome codeFix ->
                    let codeAction = CodeFixHelpers.createTextChangeCodeFix (codeFix, ctx)
                    ctx.RegisterCodeFix(codeAction, ctx.Diagnostics)
                | ValueNone -> ()
            }
            |> CancellableTask.startAsTask ctx.CancellationToken

        member ctx.RegisterFsharpFixes(codeFix: IFSharpMultiCodeFixProvider) =
            cancellableTask {
                let! codeFixes = codeFix.GetCodeFixesAsync ctx

                for codeFix in codeFixes do
                    let codeAction = CodeFixHelpers.createTextChangeCodeFix (codeFix, ctx)
                    ctx.RegisterCodeFix(codeAction, ctx.Diagnostics)
            }
            |> CancellableTask.startAsTask ctx.CancellationToken

        member ctx.GetSourceTextAsync() =
            cancellableTask {
                let! cancellationToken = CancellableTask.getCancellationToken ()
                return! ctx.Document.GetTextAsync cancellationToken
            }

        member ctx.GetSquigglyTextAsync() =
            cancellableTask {
                let! sourceText = ctx.GetSourceTextAsync()
                return sourceText.GetSubText(ctx.Span).ToString()
            }

        member ctx.GetErrorRangeAsync() =
            cancellableTask {
                let! sourceText = ctx.GetSourceTextAsync()
                return RoslynHelpers.TextSpanToFSharpRange(ctx.Document.FilePath, ctx.Span, sourceText)
            }

        member ctx.GetLineNumberAndText position =
            cancellableTask {
                let! sourceText = ctx.GetSourceTextAsync()
                let textLine = sourceText.Lines.GetLineFromPosition position
                let textLinePos = sourceText.Lines.GetLinePosition position
                let fcsTextLineNumber = Line.fromZ textLinePos.Line
                return fcsTextLineNumber, textLine.ToString()
            }

// This cannot be an extension on the code fix context
// because the underlying GetFixAllProvider method doesn't take the context in.
#nowarn "3511" // state machine not statically compilable

[<AutoOpen>]
module IFSharpCodeFixProviderExtensions =
    // Cache this no-op delegate.
    let private registerCodeFix =
        Action<CodeActions.CodeAction, ImmutableArray<Diagnostic>>(fun _ _ -> ())

    type IFSharpCodeFixProvider with

        member private provider.FixAllAsync (fixAllCtx: FixAllContext) (doc: Document) (allDiagnostics: ImmutableArray<Diagnostic>) =
            cancellableTask {
                let sw = Stopwatch.StartNew()

                let! token = CancellableTask.getCancellationToken ()
                let! sourceText = doc.GetTextAsync token

                let! codeFixOpts =
                    allDiagnostics
                    // The distinction is to avoid collisions of compiler and analyzer diags.
                    // See: https://github.com/dotnet/fsharp/issues/15620
                    // TODO: this crops the diags on a very high level,
                    // a proper fix is needed.
                    |> Seq.distinctBy (fun d -> d.Id, d.Location)
                    |> Seq.map (fun diag ->
                        let context = CodeFixContext(doc, diag, registerCodeFix, token)
                        provider.GetCodeFixIfAppliesAsync context)
                    |> CancellableTask.whenAll

                let codeFixes = codeFixOpts |> Seq.map ValueOption.toOption |> Seq.choose id
                let changes = codeFixes |> Seq.collect (fun codeFix -> codeFix.Changes)
                let updatedDoc = doc.WithText(sourceText.WithChanges changes)

                let name =
                    codeFixes
                    |> Seq.tryHead
                    |> Option.map (fun fix -> fix.Name)
                    // Now, I cannot see this happening.
                    // How could a bulk code fix get activated for zero changes?
                    // But since that's for telemetry purposes,
                    // let's be on the safe side.
                    |> Option.defaultValue "UnknownCodeFix"

                CodeFixHelpers.reportCodeFixTelemetry
                    allDiagnostics
                    updatedDoc
                    name
                    [| "scope", fixAllCtx.Scope.ToString(); "elapsedMs", sw.ElapsedMilliseconds |]

                return updatedDoc
            }

        member provider.RegisterFsharpFixAll() =
            FixAllProvider.Create(fun fixAllCtx doc allDiagnostics ->
                provider.FixAllAsync fixAllCtx doc allDiagnostics
                |> CancellableTask.start fixAllCtx.CancellationToken)

        member provider.RegisterFsharpFixAll filter =
            FixAllProvider.Create(fun fixAllCtx doc allDiagnostics ->
                let filteredDiagnostics = filter allDiagnostics

                provider.FixAllAsync fixAllCtx doc filteredDiagnostics
                |> CancellableTask.start fixAllCtx.CancellationToken)
