// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.CodeActions

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = "AddParentheses"); Shared>]
type internal FSharpWrapExpressionInParenthesesFixProvider() =
    inherit CodeFixProvider()

    let fixableDiagnosticIds = set ["FS0597"]
        
    override _.FixableDiagnosticIds = Seq.toImmutableArray fixableDiagnosticIds
    
    override this.RegisterCodeFixesAsync context : Task =
        async {
            let title = SR.WrapExpressionInParentheses()

            let getChangedText (sourceText: SourceText) =
                sourceText.WithChanges(TextChange(TextSpan(context.Span.Start, 0), "("))
                          .WithChanges(TextChange(TextSpan(context.Span.End + 1, 0), ")"))

            context.RegisterCodeFix(
                CodeAction.Create(
                    title,
                    (fun (cancellationToken: CancellationToken) ->
                        async {
                            let! sourceText = context.Document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                            return context.Document.WithText(getChangedText sourceText)
                        } |> RoslynHelpers.StartAsyncAsTask(cancellationToken)),
                    title), context.Diagnostics |> Seq.filter (fun x -> this.FixableDiagnosticIds.Contains x.Id) |> Seq.toImmutableArray)
        } |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
