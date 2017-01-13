// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Diagnostics
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.CodeActions

[<ExportCodeFixProvider(FSharpCommonConstants.FSharpLanguageName, Name = PredefinedCodeFixProviderNames.SimplifyNames); Shared>]
type internal FSharpRemoveQualificationCodeFixProvider() =
    inherit CodeFixProvider()
    let fixableDiagnosticIds = [IDEDiagnosticIds.RemoveQualificationDiagnosticId]
        
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
           let! sourceText = context.Document.GetTextAsync()
           let longIdent = sourceText.ToString(context.Span) 
           context.RegisterCodeFix(createCodeFix(sprintf "%s '%s'" SR.SimplifyName.Value longIdent, context, TextChange(context.Span, "")), diagnostics)
        } |> CommonRoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)