// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.CodeActions

[<ExportCodeFixProvider(FSharpCommonConstants.FSharpLanguageName, Name = "PrefixUnusedValueWithUnderscore"); Shared>]
type internal FSharpPrefixUnusedValueWithUnderscoreCodeFixProvider() =
    inherit CodeFixProvider()
    let fixableDiagnosticIds = ["FS1182"]
        
    override __.FixableDiagnosticIds = fixableDiagnosticIds.ToImmutableArray()

    override __.RegisterCodeFixesAsync context : Task =
       async {
            let title = "Prefix value name with underscore"
            context.RegisterCodeFix(
                CodeAction.Create(
                    title,
                    (fun (cancellationToken: CancellationToken) ->
                        async {
                            let! sourceText = context.Document.GetTextAsync() |> Async.AwaitTask
                            return context.Document.WithText(sourceText.WithChanges(TextChange(TextSpan(context.Span.Start, 0), "_")))
                        } |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)),
                    title), (context.Diagnostics |> Seq.filter (fun x -> fixableDiagnosticIds |> List.contains x.Id)).ToImmutableArray())
        } |> CommonRoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)