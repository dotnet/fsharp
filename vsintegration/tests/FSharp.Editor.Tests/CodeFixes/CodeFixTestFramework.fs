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

let mockAction =
    Action<CodeActions.CodeAction, ImmutableArray<Diagnostic>>(fun _ _ -> ())

let getRelevantDiagnostic (document: Document) errorNumber =
    cancellableTask {
        let! _, checkFileResults = document.GetFSharpParseAndCheckResultsAsync "test"

        return
            checkFileResults.Diagnostics
            |> Seq.where (fun d -> d.ErrorNumber = errorNumber)
            |> Seq.head
    }

let createTestCodeFixContext (code: string) (document: Document) (diagnostic: FSharpDiagnostic) =
    cancellableTask {
        let! cancellationToken = CancellableTask.getCurrentCancellationToken ()

        let sourceText = SourceText.From code

        let location =
            RoslynHelpers.RangeToLocation(diagnostic.Range, sourceText, document.FilePath)

        let roslynDiagnostic = RoslynHelpers.ConvertError(diagnostic, location)

        return CodeFixContext(document, roslynDiagnostic, mockAction, cancellationToken)
    }

let tryFix (code: string) diagnostic (fixProvider: IFSharpCodeFixProvider) =
    cancellableTask {
        let sourceText = SourceText.From code
        let document = RoslynTestHelpers.GetFsDocument code

        let! diagnostic = getRelevantDiagnostic document diagnostic
        let! context = createTestCodeFixContext code document diagnostic

        let! result = fixProvider.GetCodeFixIfAppliesAsync context

        return
            (result
             |> Option.map (fun codeFix ->
                 {
                     Message = codeFix.Message
                     FixedCode = (sourceText.WithChanges codeFix.Changes).ToString()
                 }))
    }
    |> CancellableTask.start CancellationToken.None
    |> fun task -> task.Result
