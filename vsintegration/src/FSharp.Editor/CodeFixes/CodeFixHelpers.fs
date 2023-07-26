﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

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

open CancellableTasks

[<RequireQualifiedAccess>]
module internal CodeFixHelpers =
    let reportCodeFixTelemetry
        (diagnostics: ImmutableArray<Diagnostic>)
        (doc: Document)
        (staticName: string)
        (additionalProps: (string * obj) array)
        =
        let ids =
            diagnostics |> Seq.map (fun d -> d.Id) |> Seq.distinct |> String.concat ","

        let defaultProps: (string * obj) array =
            [|
                "name", staticName
                "ids", ids
                "context.document.project.id", doc.Project.Id.Id.ToString()
                "context.document.id", doc.Id.Id.ToString()
                "context.diagnostics.count", diagnostics.Length
            |]

        let props: (string * obj) array = Array.concat [ additionalProps; defaultProps ]

        TelemetryReporter.ReportSingleEvent(TelemetryEvents.CodefixActivated, props)

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

        member ctx.RegisterFsharpFix(codeFix: IFSharpCodeFixProvider) =
            cancellableTask {
                match! codeFix.GetCodeFixIfAppliesAsync ctx with
                | Some codeFix -> ctx.RegisterFsharpFix(codeFix.Name, codeFix.Message, codeFix.Changes)
                | None -> ()
            }
            |> CancellableTask.startAsTask ctx.CancellationToken

        member ctx.GetSourceTextAsync() =
            cancellableTask {
                let! cancellationToken = CancellableTask.getCurrentCancellationToken ()
                return! ctx.Document.GetTextAsync cancellationToken
            }

        member ctx.GetSquigglyTextAsync() =
            cancellableTask {
                let! sourceText = ctx.GetSourceTextAsync()
                return sourceText.GetSubText(ctx.Span).ToString()
            }

        member ctx.GetErrorRangeAsync() =
            cancellableTask {
                let! sourceText = ctx.GetSourceTextAsync()
                return RoslynHelpers.TextSpanToFSharpRange(ctx.Document.FilePath, ctx.Span, sourceText)
            }

// This cannot be an extension on the code fix context
// because the underlying GetFixAllProvider method doesn't take the context in.
#nowarn "3511" // state machine not statically compilable

[<AutoOpen>]
module IFSharpCodeFixProviderExtensions =
    type IFSharpCodeFixProvider with

        // this is not used anywhere, it's just needed to create the context
        static member private Action =
            Action<CodeActions.CodeAction, ImmutableArray<Diagnostic>>(fun _ _ -> ())

        member private this.FixAllAsync (fixAllCtx: FixAllContext) (doc: Document) (allDiagnostics: ImmutableArray<Diagnostic>) = 
            cancellableTask {
                let sw = Stopwatch.StartNew()

                let! token = CancellableTask.getCurrentCancellationToken ()
                let! sourceText = doc.GetTextAsync token

                let! codeFixOpts =
                    allDiagnostics
                    // The distiction is to avoid collisions of compiler and analyzer diags
                    // See: https://github.com/dotnet/fsharp/issues/15620
                    |> Seq.distinctBy (fun d -> d.Id, d.Location)
                    |> Seq.map (fun diag -> CodeFixContext(doc, diag, IFSharpCodeFixProvider.Action, token))
                    |> Seq.map (fun context -> this.GetCodeFixIfAppliesAsync context)
                    |> Seq.map (fun task -> task token)
                    |> Task.WhenAll

                let codeFixes = codeFixOpts |> Seq.choose id
                let changes = codeFixes |> Seq.collect (fun codeFix -> codeFix.Changes)
                let updatedDoc = doc.WithText(sourceText.WithChanges changes)

                let name =
                    codeFixes
                    |> Seq.tryHead
                    |> Option.map (fun fix -> fix.Name)
                    // Now, I cannot see this happening.
                    // How could a bulk code fix get activated for zero changes?
                    // But since that's for telemetry purposes,
                    // let's be on the safe side.
                    |> Option.defaultValue "UnknownCodeFix"

                CodeFixHelpers.reportCodeFixTelemetry
                    allDiagnostics
                    updatedDoc
                    name
                    [| "scope", fixAllCtx.Scope.ToString(); "elapsedMs", sw.ElapsedMilliseconds |]

                return updatedDoc
            }

        member provider.RegisterFsharpFixAll() =
            FixAllProvider.Create(fun fixAllCtx doc allDiagnostics -> 
                provider.FixAllAsync fixAllCtx doc allDiagnostics
                |> CancellableTask.start fixAllCtx.CancellationToken)
