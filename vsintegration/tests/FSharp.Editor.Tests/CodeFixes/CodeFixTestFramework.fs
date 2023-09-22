// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.CodeFixTestFramework

open System
open System.Collections.Immutable
open System.Text.RegularExpressions

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks

open FSharp.Compiler.Diagnostics
open FSharp.Editor.Tests.Helpers

type TestCodeFix = { Message: string; FixedCode: string }

type Mode =
    | Auto
    | WithOption of CustomProjectOption: string
    | WithSignature of FsiCode: string
    | Manual of Squiggly: string * Diagnostic: string
    | WithSettings of CodeFixesOptions

let inline toOption o =
    match o with
    | ValueSome v -> Some v
    | _ -> None

let mockAction =
    Action<CodeActions.CodeAction, ImmutableArray<Diagnostic>>(fun _ _ -> ())

let parseDiagnostic diagnostic =
    let regex = Regex "([A-Z]+)(\d+)"
    let matchGroups = regex.Match(diagnostic).Groups
    let prefix = matchGroups[1].Value
    let number = int matchGroups[2].Value
    number, prefix

let getDocument code mode =
    match mode with
    | Auto -> RoslynTestHelpers.GetFsDocument code
    | WithOption option -> RoslynTestHelpers.GetFsDocument(code, option)
    | WithSignature fsiCode -> RoslynTestHelpers.GetFsiAndFsDocuments fsiCode code |> Seq.last
    | Manual _ -> RoslynTestHelpers.GetFsDocument code
    | WithSettings settings -> RoslynTestHelpers.GetFsDocument(code, customEditorOptions = settings)

let getRelevantDiagnostics (document: Document) =
    cancellableTask {
        let! _, checkFileResults = document.GetFSharpParseAndCheckResultsAsync "test"

        return checkFileResults.Diagnostics
    }
    |> CancellableTask.startWithoutCancellation
    |> fun task -> task.Result

let createTestCodeFixContext (code: string) (mode: Mode) (fixProvider: 'T :> CodeFixProvider) =
    cancellableTask {
        let! cancellationToken = CancellableTask.getCancellationToken ()

        let sourceText = SourceText.From code
        let document = getDocument code mode
        let diagnosticIds = fixProvider.FixableDiagnosticIds

        let diagnostics =
            match mode with
            | Auto ->
                getRelevantDiagnostics document
                |> Array.filter (fun d -> diagnosticIds |> Seq.contains d.ErrorNumberText)
            | WithOption _ -> getRelevantDiagnostics document
            | WithSignature _ -> getRelevantDiagnostics document
            | WithSettings _ -> getRelevantDiagnostics document
            | Manual (squiggly, diagnostic) ->
                let spanStart = code.IndexOf squiggly
                let span = TextSpan(spanStart, squiggly.Length)
                let range = RoslynHelpers.TextSpanToFSharpRange(document.FilePath, span, sourceText)
                let number, prefix = parseDiagnostic diagnostic

                [|
                    FSharpDiagnostic.Create(FSharpDiagnosticSeverity.Warning, "test", number, range, prefix)
                |]

        let range = diagnostics[0].Range
        let location = RoslynHelpers.RangeToLocation(range, sourceText, document.FilePath)
        let textSpan = RoslynHelpers.FSharpRangeToTextSpan(sourceText, range)

        let roslynDiagnostics =
            diagnostics
            |> Array.map (fun diagnostic -> RoslynHelpers.ConvertError(diagnostic, location))
            |> ImmutableArray.ToImmutableArray

        return CodeFixContext(document, textSpan, roslynDiagnostics, mockAction, cancellationToken)
    }

let tryFix (code: string) mode (fixProvider: 'T :> IFSharpCodeFixProvider) =
    cancellableTask {
        let sourceText = SourceText.From code

        let! context = createTestCodeFixContext code mode fixProvider

        let! result = fixProvider.GetCodeFixIfAppliesAsync context

        return
            (result
             |> toOption
             |> Option.map (fun codeFix ->
                 {
                     Message = codeFix.Message
                     FixedCode = (sourceText.WithChanges codeFix.Changes).ToString()
                 }))
    }
    |> CancellableTask.startWithoutCancellation
    |> fun task -> task.Result

let multiFix (code: string) mode (fixProvider: 'T :> IFSharpMultiCodeFixProvider) =
    cancellableTask {
        let sourceText = SourceText.From code

        let! context = createTestCodeFixContext code mode fixProvider

        let! result = fixProvider.GetCodeFixesAsync context

        return
            result
            |> Seq.map (fun codeFix ->
                {
                    Message = codeFix.Message
                    FixedCode = (sourceText.WithChanges codeFix.Changes).ToString()
                })
    }
    |> CancellableTask.startWithoutCancellation
    |> fun task -> task.Result
