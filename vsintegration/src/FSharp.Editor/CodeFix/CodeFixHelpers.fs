// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Threading

open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.CodeActions

[<RequireQualifiedAccess>]
module internal CodeFixHelpers =
    let createTextChangeCodeFix (title: string, context: CodeFixContext, computeTextChanges: unit -> Async<TextChange[] option>) =
        CodeAction.Create(
            title,
            (fun (cancellationToken: CancellationToken) ->
                async {
                    let! sourceText = context.Document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                    let! changesOpt = computeTextChanges()
                    match changesOpt with
                    | None -> return context.Document
                    | Some textChanges -> return context.Document.WithText(sourceText.WithChanges(textChanges))
                } |> RoslynHelpers.StartAsyncAsTask(cancellationToken)),
            title)

[<AutoOpen>]
module internal CodeFixExtensions =
    type CodeFixProvider with
        member this.GetPrunedDiagnostics(context: CodeFixContext) = 
            context.Diagnostics.RemoveAll(fun x -> this.FixableDiagnosticIds.Contains(x.Id) |> not)
            
        member this.RegisterFix(context: CodeFixContext, fixName, fixChange) =
            let replaceCodeFix =
                CodeFixHelpers.createTextChangeCodeFix(
                    fixName,
                    context,
                    (fun () -> asyncMaybe.Return [| fixChange |]))
            context.RegisterCodeFix(replaceCodeFix, this.GetPrunedDiagnostics(context))
                 
