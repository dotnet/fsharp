// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.CodeActions

[<ExportCodeFixProvider(FSharpCommonConstants.FSharpLanguageName, Name = "ProposeUpperCaseLabel"); Shared>]
type internal FSharpProposeUpperCaseLabelCodeFixProvider() =
    inherit CodeFixProvider()
    let fixableDiagnosticIds = ["FS0053"]
        
    let createCodeFix (title: string, context: CodeFixContext, textChange: TextChange) =
        CodeAction.Create(
            title,
            (fun (cancellationToken: CancellationToken) ->
                async {
                    let! sourceText = context.Document.GetTextAsync()
                    return context.Document.WithText(sourceText.WithChanges(textChange))
                } |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)),
            title)

    override __.FixableDiagnosticIds = fixableDiagnosticIds.ToImmutableArray()

    override __.RegisterCodeFixesAsync context : Task =
        async {
            let diagnostics = (context.Diagnostics |> Seq.filter (fun x -> fixableDiagnosticIds |> List.contains x.Id)).ToImmutableArray()
            if context.Span.Length > 0 then
                let! sourceText = context.Document.GetTextAsync(context.CancellationToken)
                let originalText = sourceText.ToString(context.Span)
                if originalText.Length > 0 then
                    let newText = originalText.[0].ToString().ToUpper() + originalText.Substring(1)
                    context.RegisterCodeFix(createCodeFix(FSComp.SR.replaceWithSuggestion newText, context, TextChange(context.Span, newText)), diagnostics)
        } |> CommonRoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)