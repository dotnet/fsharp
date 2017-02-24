// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Diagnostics
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.CodeActions

[<ExportCodeFixProvider(FSharpCommonConstants.FSharpLanguageName, Name = "RemoveUnusedOpens"); Shared>]
type internal FSharpRemoveUnusedOpensCodeFixProvider() =
    inherit CodeFixProvider()
    let fixableDiagnosticIds = [IDEDiagnosticIds.RemoveUnnecessaryImportsDiagnosticId]
        
    let createCodeFix (title: string, context: CodeFixContext, textChange: TextChange) =
        CodeAction.Create(
            title,
            (fun (cancellationToken: CancellationToken) ->
                async {
                    let! sourceText = context.Document.GetTextAsync()
                    return context.Document.WithText(sourceText.WithChanges(textChange))
                } |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)),
            title)

    override __.FixableDiagnosticIds = Seq.toImmutableArray fixableDiagnosticIds

    override __.RegisterCodeFixesAsync context : Task =
        async {
            let! sourceText = context.Document.GetTextAsync()
            let line = sourceText.Lines.GetLineFromPosition context.Span.Start
            let diagnostics = context.Diagnostics |> Seq.filter (fun x -> fixableDiagnosticIds |> List.contains x.Id) |> Seq.toImmutableArray
            context.RegisterCodeFix(createCodeFix(SR.RemoveUnusedOpens.Value, context, TextChange(line.SpanIncludingLineBreak, "")), diagnostics)
        } |> CommonRoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)

    override __.GetFixAllProvider() = WellKnownFixAllProviders.BatchFixer
 