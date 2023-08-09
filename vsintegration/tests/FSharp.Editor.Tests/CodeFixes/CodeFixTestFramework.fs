// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.CodeFixTestFramework

open System
open System.Collections.Immutable
open System.Threading

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
    | Manual of Squiggly: string * Number: int

let inline toOption o =
    match o with
    | ValueSome v -> Some v
    | _ -> None

let mockAction =
    Action<CodeActions.CodeAction, ImmutableArray<Diagnostic>>(fun _ _ -> ())

let getDocument code mode =
    match mode with
    | Auto -> RoslynTestHelpers.GetFsDocument code
    | WithOption option -> RoslynTestHelpers.GetFsDocument(code, option)
    | Manual _ -> RoslynTestHelpers.GetFsDocument code

let getRelevantDiagnostic (document: Document) =
    cancellableTask {
        let! _, checkFileResults = document.GetFSharpParseAndCheckResultsAsync "test"

        return checkFileResults.Diagnostics |> Seq.head
    }

let createTestCodeFixContext (code: string) document (mode: Mode) =
    cancellableTask {
        let! cancellationToken = CancellableTask.getCancellationToken ()

        let sourceText = SourceText.From code

        let! diagnostic =
            match mode with
            | Auto -> getRelevantDiagnostic document
            | WithOption _ -> getRelevantDiagnostic document
            | Manual (squiggly, number) ->
                let spanStart = code.IndexOf squiggly
                let span = TextSpan(spanStart, squiggly.Length)
                let range = RoslynHelpers.TextSpanToFSharpRange(document.FilePath, span, sourceText)
                CancellableTask.singleton (FSharpDiagnostic.Create(FSharpDiagnosticSeverity.Warning, "test", number, range))

        let location =
            RoslynHelpers.RangeToLocation(diagnostic.Range, sourceText, document.FilePath)

        let roslynDiagnostic = RoslynHelpers.ConvertError(diagnostic, location)

        return CodeFixContext(document, roslynDiagnostic, mockAction, cancellationToken)
    }

let tryFix (code: string) mode (fixProvider: IFSharpCodeFixProvider) =
    cancellableTask {
        let sourceText = SourceText.From code
        let document = getDocument code mode

        let! context = createTestCodeFixContext code document mode

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
    |> CancellableTask.start CancellationToken.None
    |> fun task -> task.Result
