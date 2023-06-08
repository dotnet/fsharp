// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.CodeFixes.CodeFixTestFramework

open System.Threading

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks

open FSharp.Editor.Tests.Helpers

type TestCodeFix = { Title: string; FixedCode: string }

let getRelevantDiagnostic (document: Document) errorNumber =
    cancellableTask {
        let! _, checkFileResults = document.GetFSharpParseAndCheckResultsAsync "test"

        return
            checkFileResults.Diagnostics
            |> Seq.where (fun d -> d.ErrorNumber = errorNumber)
            |> Seq.exactlyOne
    }

let fix (code: string) diagnostic (fixProvider: IFSharpCodeFix) =
    cancellableTask {
        let sourceText = SourceText.From code
        let document = RoslynTestHelpers.GetFsDocument code

        let! diagnostic = getRelevantDiagnostic document diagnostic

        let diagnosticSpan =
            RoslynHelpers.FSharpRangeToTextSpan(sourceText, diagnostic.Range)

        let! title, changes = fixProvider.GetChangesAsync document diagnosticSpan
        let fixedSourceText = sourceText.WithChanges changes

        return
            {
                Title = title
                FixedCode = fixedSourceText.ToString()
            }
    }
    |> CancellableTask.start CancellationToken.None
    |> fun task -> task.Result
