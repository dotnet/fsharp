﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Threading

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.CodeActions
open Microsoft.VisualStudio.FSharp.Editor.Telemetry

[<RequireQualifiedAccess>]
module internal CodeFixHelpers =
    let createTextChangeCodeFix (title: string, context: CodeFixContext, computeTextChanges: unit -> Async<TextChange[] option>) =
        let props: (string * obj) list =
            [
                "title", title

                // The following can help building a unique but anonymized codefix target:
                // #projectid#documentid#span
                // Then we can check if the codefix was actually activated after its creation.
                "context.document.project.id", context.Document.Project.Id.Id.ToString()
                "context.document.id", context.Document.Id.Id.ToString()
                "context.span", context.Span.ToString()
            ]

        TelemetryReporter.reportEvent "codefixregistered" props

        CodeAction.Create(
            title,
            (fun (cancellationToken: CancellationToken) ->
                async {
                    let! sourceText = context.Document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                    let! changesOpt = computeTextChanges ()

                    match changesOpt with
                    | None -> return context.Document
                    | Some textChanges ->
                        // Note: "activated" doesn't mean "applied".
                        // It's one step prior to that:
                        // e.g. when one clicks (Ctrl + .) and looks at the potential change.
                        TelemetryReporter.reportEvent "codefixactivated" props
                        return context.Document.WithText(sourceText.WithChanges(textChanges))
                }
                |> RoslynHelpers.StartAsyncAsTask(cancellationToken)),
            title
        )

[<AutoOpen>]
module internal CodeFixExtensions =
    type CodeFixProvider with

        member this.GetPrunedDiagnostics(context: CodeFixContext) =
            context.Diagnostics.RemoveAll(fun x -> this.FixableDiagnosticIds.Contains(x.Id) |> not)

        member this.RegisterFix(context: CodeFixContext, fixName, fixChange) =
            let replaceCodeFix =
                CodeFixHelpers.createTextChangeCodeFix (fixName, context, (fun () -> asyncMaybe.Return [| fixChange |]))

            context.RegisterCodeFix(replaceCodeFix, this.GetPrunedDiagnostics(context))
