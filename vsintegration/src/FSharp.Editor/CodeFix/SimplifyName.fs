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

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = PredefinedCodeFixProviderNames.SimplifyNames); Shared>]
type internal FSharpSimplifyNameCodeFixProvider() =
    inherit CodeFixProvider()
    let fixableDiagnosticId = IDEDiagnosticIds.SimplifyNamesDiagnosticId
        
    let createCodeFix (title: string, context: CodeFixContext, textChange: TextChange) =
        CodeAction.Create(
            title,
            (fun (cancellationToken: CancellationToken) ->
                async {
                    let! cancellationToken = Async.CancellationToken
                    let! sourceText = context.Document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                    return context.Document.WithText(sourceText.WithChanges(textChange))
                } |> RoslynHelpers.StartAsyncAsTask(cancellationToken)),
            title)

    override __.FixableDiagnosticIds = ImmutableArray.Create(fixableDiagnosticId)

    override __.RegisterCodeFixesAsync(context: CodeFixContext) : Task =
       async {
           for diagnostic in context.Diagnostics |> Seq.filter (fun x -> x.Id = fixableDiagnosticId) do
               let title =
                   match diagnostic.Properties.TryGetValue(SimplifyNameDiagnosticAnalyzer.LongIdentPropertyKey) with
                   | true, longIdent -> sprintf "%s '%s'" SR.SimplifyName.Value longIdent
                   | _ -> SR.SimplifyName.Value

               context.RegisterCodeFix(
                   createCodeFix(title, context, TextChange(context.Span, "")), 
                   ImmutableArray.Create(diagnostic))
       } 
       |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)