// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable
open System.Threading.Tasks

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open FSharp.Compiler.Diagnostics

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = "FixIndexerAccess"); Shared>]
type internal FSharpFixIndexerAccessCodeFixProvider() =
    inherit CodeFixProvider()
    let fixableDiagnosticIds = set ["FS3217"]
        
    override _.FixableDiagnosticIds = Seq.toImmutableArray fixableDiagnosticIds

    override _.RegisterCodeFixesAsync context : Task =
        async {
            let diagnostics = 
                context.Diagnostics 
                |> Seq.filter (fun x -> fixableDiagnosticIds |> Set.contains x.Id)
                |> Seq.toList
            if not (List.isEmpty diagnostics) then
                let! sourceText = context.Document.GetTextAsync() |> Async.AwaitTask

                diagnostics
                |> Seq.iter (fun diagnostic ->
                    let diagnostics = ImmutableArray.Create diagnostic
                    let span,replacement =
                        try
                            let mutable span = context.Span

                            let notStartOfBracket (span: TextSpan) =
                                let t = sourceText.GetSubText(TextSpan(span.Start, span.Length + 1))
                                t.[t.Length-1] <> '['

                            // skip all braces and blanks until we find [
                            while span.End < sourceText.Length && notStartOfBracket span do
                                span <- TextSpan(span.Start, span.Length + 1)

                            span,sourceText.GetSubText(span).ToString()
                        with
                        | _ -> context.Span,sourceText.GetSubText(context.Span).ToString()

                    let codefix = 
                        CodeFixHelpers.createTextChangeCodeFix(
                            CompilerDiagnostics.GetErrorMessage FSharpDiagnosticKind.AddIndexerDot, 
                            context,
                            (fun () -> asyncMaybe.Return [| TextChange(span, replacement.TrimEnd() + ".") |]))

                    context.RegisterCodeFix(codefix, diagnostics))
        } |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
