// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.CodeActions

[<ExportCodeFixProvider(FSharpCommonConstants.FSharpLanguageName, Name = "AddNewKeyword"); Shared>]
type internal FSharpAddNewKeywordCodeFixProvider() =
    inherit CodeFixProvider()

    override __.FixableDiagnosticIds = ImmutableArray.Create "FS0760"

    override this.RegisterCodeFixesAsync context : Task =
        async {
            let title = SR.AddNewKeyword.Value
            context.RegisterCodeFix(
                CodeAction.Create(
                    title,
                    (fun (cancellationToken: CancellationToken) ->
                        async {
                            let! sourceText = context.Document.GetTextAsync()
                            return context.Document.WithText(sourceText.WithChanges(TextChange(TextSpan(context.Span.Start, 0), "new ")))
                        } |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)),
                    title), context.Diagnostics |> Seq.filter (fun x -> this.FixableDiagnosticIds.Contains x.Id) |> Seq.toImmutableArray)
        } |> CommonRoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
 