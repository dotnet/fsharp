// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Threading
open System.Threading.Tasks
open System.Collections.Immutable
open System.Diagnostics

open Microsoft
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.CodeActions
open Microsoft.VisualStudio.FSharp.Editor.Telemetry

[<RequireQualifiedAccess>]
module internal CodeFixHelpers =
    let private reportCodeFixTelemetry (diagnostics: ImmutableArray<Diagnostic>) (doc: Document) (staticName: string) (additionalProps: (string * obj) array) =
        let ids =
            diagnostics |> Seq.map (fun d -> d.Id) |> Seq.distinct |> String.concat ","

        let defaultProps: (string * obj) array =
            [|
                "name", staticName;
                "ids", ids;
                "context.document.project.id", doc.Project.Id.Id.ToString();
                "context.document.id", doc.Id.Id.ToString();
                "context.diagnostics.count", diagnostics.Length
            |]

        let props: (string * obj) array =
            Array.concat [additionalProps; defaultProps]

        TelemetryReporter.ReportSingleEvent ("codefixactivated", props)

    let createFixAllProvider name getChanges =
        FixAllProvider.Create(fun fixAllCtx doc allDiagnostics ->
            backgroundTask {
                let sw = Stopwatch.StartNew()
                let! (changes: seq<TextChange>) = getChanges (doc, allDiagnostics, fixAllCtx.CancellationToken)
                let! text = doc.GetTextAsync(fixAllCtx.CancellationToken)
                let doc = doc.WithText(text.WithChanges(changes))

                do
                    reportCodeFixTelemetry
                        allDiagnostics
                        doc
                        name
                        [| "scope", fixAllCtx.Scope.ToString(); "elapsedMs", sw.ElapsedMilliseconds |]

                return doc
            })

    let createTextChangeCodeFix (name: string, title: string, context: CodeFixContext, changes: TextChange seq) =
        CodeAction.Create(
            title,
            (fun (cancellationToken: CancellationToken) ->
                backgroundTask {
                    let! sourceText = context.Document.GetTextAsync(cancellationToken)
                    let doc = context.Document.WithText(sourceText.WithChanges(changes))
                    reportCodeFixTelemetry context.Diagnostics context.Document name [||]
                    return doc
                }),
            name
        )

[<AutoOpen>]
module internal CodeFixExtensions =
    type CodeFixContext with

        member ctx.RegisterFsharpFix(staticName, title, changes, ?diagnostics) =
            let codeAction =
                CodeFixHelpers.createTextChangeCodeFix (staticName, title, ctx, changes)

            let diag = diagnostics |> Option.defaultValue ctx.Diagnostics
            ctx.RegisterCodeFix(codeAction, diag)
