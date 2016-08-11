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
[<ExportLanguageService(typeof<ILanguageDebugInfoService>, FSharpCommonConstants.FSharpLanguageName)>]
type internal FSharpLanguageDebugInfoService() =

    static member GetDataTipInformation(sourceText: SourceText, position: int, tokens: List<ClassifiedSpan>): DebugDataTipInfo =
        let tokenIndex = tokens |> Seq.tryFindIndex(fun t -> t.TextSpan.Contains(position))

        if tokenIndex.IsNone then
            Unchecked.defaultof<DebugDataTipInfo>
        else
            let token = tokens.[tokenIndex.Value]
        
            let constructLiteralDataTip() =
                new DebugDataTipInfo(token.TextSpan, sourceText.GetSubText(token.TextSpan).ToString())

            let constructIdentifierDataTip() =
                let textLine = sourceText.Lines.GetLineFromPosition(position)
                match QuickParse.GetCompleteIdentifierIsland false (textLine.ToString()) (position - textLine.Start) with
                | None -> Unchecked.defaultof<DebugDataTipInfo>
                | Some(island, islandPosition, _) -> new DebugDataTipInfo(TextSpan.FromBounds(islandPosition, islandPosition + island.Length), island)
                
            // FSROSLYNTODO: enable numeric literal and identifier data tips in VS
            match token.ClassificationType with
            | ClassificationTypeNames.NumericLiteral -> constructLiteralDataTip()
            | ClassificationTypeNames.StringLiteral -> constructLiteralDataTip()
            | ClassificationTypeNames.Identifier -> constructIdentifierDataTip()
            | _ -> Unchecked.defaultof<DebugDataTipInfo>


    interface ILanguageDebugInfoService with
        
        // FSROSLYNTODO: This is used to get function names in breakpoint window. It should return fully qualified function name and line offset from the start of the function.
        member this.GetLocationInfoAsync(_, _, _): Task<DebugLocationInfo> =
            Task.FromResult(Unchecked.defaultof<DebugLocationInfo>)

        member this.GetDataTipInfoAsync(document: Document, position: int, cancellationToken: CancellationToken): Task<DebugDataTipInfo> =
            let computation = async {
                let options = CommonRoslynHelpers.GetFSharpProjectOptionsForRoslynProject(document.Project)
                let defines = CompilerEnvironment.GetCompilationDefinesForEditing(document.Name, options.OtherOptions |> Seq.toList)

                let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                let textSpan = TextSpan.FromBounds(0, sourceText.Length)
                let tokens = FSharpColorizationService.GetColorizationData(sourceText, textSpan, Some(document.Name), defines, cancellationToken)

                return FSharpLanguageDebugInfoService.GetDataTipInformation(sourceText, position, tokens)
            }

            Async.StartAsTask(computation, TaskCreationOptions.None, cancellationToken).ContinueWith(fun(task: Task<DebugDataTipInfo>) ->
                if task.Status = TaskStatus.RanToCompletion then
                    task.Result
                else
                    Assert.Exception(task.Exception.GetBaseException())
                    raise(task.Exception.GetBaseException())
            , cancellationToken)
            