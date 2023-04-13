// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Threading
open System.Threading.Tasks
open System.Collections.Immutable

open Microsoft
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.CodeActions
open Microsoft.VisualStudio.FSharp.Editor.Telemetry

[<RequireQualifiedAccess>]
module internal CodeFixHelpers =
    let reportCodeFixRecommendation (diagnostics: ImmutableArray<Diagnostic>) (doc: Document) (staticName: string) =
        let ids =
            diagnostics |> Seq.map (fun d -> d.Id) |> Seq.distinct |> String.concat ","

        let props: (string * obj) list =
            [
                "name", staticName
                "ids", ids
                "context.document.project.id", doc.Project.Id.Id.ToString()
                "context.document.id", doc.Id.Id.ToString()
                "context.diagnostics.count", diagnostics.Length
            ]

        TelemetryReporter.reportEvent "codefixrecommendation" props

    let createTextChangeCodeFix
        (
            name: string,
            title: string,
            context: CodeFixContext,
            computeTextChanges: unit -> Async<TextChange[] option>
        ) =

        // I don't understand how we can get anything but a single diagnostic here - as we get a single "title" here.
        // But since we don't have proper testing yet, keeping it like this to verify this theory in telemetry.
        let ids = context.Diagnostics |> Seq.map (fun d -> d.Id) |> String.concat ","

        let props: (string * obj) list =
            [
                "name", name
                "ids", ids
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

        member this.RegisterFix(name, title, context: CodeFixContext, fixChange) =
            let replaceCodeFix =
                CodeFixHelpers.createTextChangeCodeFix (name, title, context, (fun () -> asyncMaybe.Return [| fixChange |]))

            context.RegisterCodeFix(replaceCodeFix, this.GetPrunedDiagnostics(context))
