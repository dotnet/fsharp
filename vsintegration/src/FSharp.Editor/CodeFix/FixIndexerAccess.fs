// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.CodeActions

[<ExportCodeFixProvider(FSharpConstants.FSharpLanguageName, Name = "FixIndexerAccess"); Shared>]
type internal FSharpFixIndexerAccessCodeFixProvider() =
    inherit CodeFixProvider()
    let fixableDiagnosticIds = set ["FS3217"]
        
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
                            let span = ref context.Span
                        
                            // skip all braces and blanks until we find [
                            while 
                                (!span).End < sourceText.Length &&
                                 let t = TextSpan((!span).Start,(!span).Length + 1)
                                 let s = sourceText.GetSubText(t).ToString()
                                 s.[s.Length-1] <> '[' do
                                span := TextSpan((!span).Start,(!span).Length + 1)

                            !span,sourceText.GetSubText(!span).ToString()
                        with
                        | _ -> context.Span,sourceText.GetSubText(context.Span).ToString()

                    let codefix = 
                        createCodeFix(
                            FSComp.SR.addIndexerDot(), 
                            context,
                            TextChange(span, replacement.TrimEnd() + "."))
                    context.RegisterCodeFix(codefix, diagnostics))
        } |> RoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
