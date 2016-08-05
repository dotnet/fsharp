// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Concurrent
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open System.Linq

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Editor.Implementation.Debugging
open Microsoft.CodeAnalysis.Editor.Shared.Utilities
open Microsoft.CodeAnalysis.Formatting
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Tagging

open Microsoft.FSharp.Compiler.Parser
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.Range

[<Shared>]
[<ExportLanguageService(typeof<IBreakpointResolutionService>, FSharpCommonConstants.FSharpLanguageName)>]
type internal FSharpBreakpointResolutionService() =

    static member GetBreakpointLocation(sourceText: SourceText, fileName: string, textSpan: TextSpan, options: FSharpProjectOptions) = async {
        let! parseResults = FSharpChecker.Instance.ParseFileInProject(fileName, sourceText.ToString(), options)
        let textLine = sourceText.Lines.GetLineFromPosition(textSpan.Start)

        let textLineNumber = textLine.LineNumber + 1 // Roslyn line numbers are zero-based
        let textColumnNumber = textSpan.Start - textLine.Start

        return parseResults.ValidateBreakpointLocation(mkPos textLineNumber textColumnNumber)
    }

    interface IBreakpointResolutionService with
        member this.ResolveBreakpointAsync(document: Document, textSpan: TextSpan, cancellationToken: CancellationToken): Task<BreakpointResolutionResult> =
            let computation = async {
                let options = CommonRoslynHelpers.GetFSharpProjectOptionsForRoslynProject(document.Project)
                let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                let! location = FSharpBreakpointResolutionService.GetBreakpointLocation(sourceText, document.Name, textSpan, options)
                
                return match location with
                       | None -> null
                       | Some(range) -> BreakpointResolutionResult.CreateSpanResult(document, CommonRoslynHelpers.FSharpRangeToTextSpan(sourceText, range))
            }

            Async.StartAsTask(computation, TaskCreationOptions.None, cancellationToken).ContinueWith(fun(task: Task<BreakpointResolutionResult>) ->
                if task.Status = TaskStatus.RanToCompletion then
                    task.Result
                else
                    Assert.Exception(task.Exception.GetBaseException())
                    raise(task.Exception.GetBaseException())
            , cancellationToken)
            
        // FSROSLYNTODO: enable placing breakpoints by when user suplies fully-qualified function names
        member this.ResolveBreakpointsAsync(_, _, _): Task<IEnumerable<BreakpointResolutionResult>> =
            Task.FromResult(Enumerable.Empty<BreakpointResolutionResult>())
