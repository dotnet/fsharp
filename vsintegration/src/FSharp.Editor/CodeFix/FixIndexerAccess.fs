// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open SymbolHelpers

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = "FixIndexerAccess"); Shared>]
type internal FSharpFixIndexerAccessCodeFixProvider() =
    inherit CodeFixProvider()
    let fixableDiagnosticIds = set ["FS3217"]
        
    override __.FixableDiagnosticIds = Seq.toImmutableArray fixableDiagnosticIds

    override __.RegisterCodeFixesAsync context : Task =
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
                                let t = TextSpan(span.Start, span.Length + 1)
                                let s = sourceText.GetSubText(t).ToString()
                                s.[s.Length-1] <> '['

                            // skip all braces and blanks until we find [
                            while span.End < sourceText.Length && notStartOfBracket span do
                                span <- TextSpan(span.Start, span.Length + 1)

                            span,sourceText.GetSubText(span).ToString()
                        with
                        | _ -> context.Span,sourceText.GetSubText(context.Span).ToString()

                    let codefix = 
                        createTextChangeCodeFix(
                            FSComp.SR.addIndexerDot(), 
                            context,
                            (fun () -> asyncMaybe.Return [| TextChange(span, replacement.TrimEnd() + ".") |]))

                    context.RegisterCodeFix(codefix, diagnostics))
        } |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
