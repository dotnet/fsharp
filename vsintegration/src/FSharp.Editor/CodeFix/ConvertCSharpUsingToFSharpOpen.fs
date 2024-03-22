// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = "ConvertCSharpUsingToFSharpOpen"); Shared>]
type internal FSharpConvertCSharpUsingToFSharpOpen
    [<ImportingConstructor>]
    (
    ) =
    inherit CodeFixProvider()

    let fixableDiagnosticIds = set ["FS0039"; "FS0201"]
    let usingLength = "using".Length

    let isCSharpUsingShapeWithPos (context: CodeFixContext) (sourceText: SourceText) =
        // Walk back until whitespace
        let mutable pos = context.Span.Start - 1
        let mutable ch = sourceText.[pos]
        while pos > 0 && not(Char.IsWhiteSpace(ch)) do
            pos <- pos - 1
            ch <- sourceText.[pos]

        // Walk back whitespace
        ch <- sourceText.[pos]
        while pos > 0 && Char.IsWhiteSpace(ch) do
            pos <- pos - 1
            ch <- sourceText.[pos]

        // Take 'using' slice and don't forget that offset because computer math is annoying
        let start = pos - usingLength + 1
        let span = TextSpan(start, usingLength)
        let slice = sourceText.GetSubText(span).ToString()
        struct(slice = "using", start)

    let registerCodeFix (context: CodeFixContext) (diagnostics: ImmutableArray<Diagnostic>)  (str: string) (span: TextSpan) =
        let replacement =
            let str = str.Replace("using", "open").Replace(";", "")
            TextChange(span, str)
        let title = SR.ConvertCSharpUsingToFSharpOpen()
        let codeFix =
            CodeFixHelpers.createTextChangeCodeFix(
                title,
                context,
                (fun () -> asyncMaybe.Return [| replacement |]))
        context.RegisterCodeFix(codeFix, diagnostics)

    override _.FixableDiagnosticIds = Seq.toImmutableArray fixableDiagnosticIds

    override _.RegisterCodeFixesAsync context =
        asyncMaybe {
            let diagnostics =
                context.Diagnostics
                |> Seq.filter (fun x -> fixableDiagnosticIds |> Set.contains x.Id)
                |> Seq.toImmutableArray
            
            do! Option.guard(diagnostics.Length > 0)

            let! sourceText = context.Document.GetTextAsync(context.CancellationToken)

            // TODO: handle single-line case?
            let statementWithSemicolonSpan = TextSpan(context.Span.Start, context.Span.Length + 1)

            do! Option.guard (sourceText.Length >= statementWithSemicolonSpan.End)

            let statementWithSemicolon = sourceText.GetSubText(statementWithSemicolonSpan).ToString()

            // Top of the file case -- entire line gets a diagnostic
            if (statementWithSemicolon.StartsWith("using") && statementWithSemicolon.EndsWith(";")) then
                registerCodeFix context diagnostics statementWithSemicolon statementWithSemicolonSpan
            else
                // Only the identifier being opened has a diagnostic, so we try to find the rest of the statement
                let struct(isCSharpUsingShape, start) = isCSharpUsingShapeWithPos context sourceText
                if isCSharpUsingShape then
                    let len =  (context.Span.Start - start) + statementWithSemicolonSpan.Length
                    let fullSpan = TextSpan(start, len)
                    let str = sourceText.GetSubText(fullSpan).ToString()
                    registerCodeFix context diagnostics str fullSpan
        }
        |> Async.Ignore
        |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken) 
