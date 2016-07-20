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
open Microsoft.CodeAnalysis.Editor.Implementation.BraceMatching
open Microsoft.CodeAnalysis.Editor.Shared.Utilities
open Microsoft.CodeAnalysis.Formatting
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Tagging

open Microsoft.FSharp.Compiler.Parser
open Microsoft.FSharp.Compiler.SourceCodeServices

[<Shared>]
[<ExportLanguageService(typeof<IIndentationService>, FSharpCommonConstants.FSharpLanguageName)>]
type internal FSharpIndentationService() =

    static member GetDesiredIndentation(sourceText: SourceText, lineNumber: int, tabSize: int): Option<int> =
        if lineNumber = 0 then
            // No indentation on the first line of a document
            None
        else
            // Match indentation with previous line
            let previousLine = sourceText.Lines.[lineNumber - 1]
            let rec loop column spaces =
                if previousLine.Start + column >= previousLine.End then
                    spaces
                else match previousLine.Text.[previousLine.Start + column] with
                     | ' ' -> loop (column + 1) (spaces + 1)
                     | '\t' -> loop (column + 1) (((spaces / tabSize) + 1) * tabSize)
                     | _ -> spaces
            Some(loop 0 0)

    interface IIndentationService with
        member this.GetDesiredIndentationAsync(document: Document, lineNumber: int, cancellationToken: CancellationToken): Task<Nullable<IndentationResult>> =
            document.GetTextAsync(cancellationToken).ContinueWith(
                fun (sourceTextTask: Task<SourceText>) ->
                    let tabSize = document.Project.Solution.Workspace.Options.GetOption(FormattingOptions.TabSize, FSharpCommonConstants.FSharpLanguageName)
                    try match FSharpIndentationService.GetDesiredIndentation(sourceTextTask.Result, lineNumber, tabSize) with
                        | None -> Nullable<IndentationResult>()
                        | Some(indentation) -> Nullable<IndentationResult>(IndentationResult(sourceTextTask.Result.Lines.[lineNumber].Start, indentation))
                    with ex -> 
                        Assert.Exception(ex)
                        reraise()
                , TaskContinuationOptions.OnlyOnRanToCompletion)
