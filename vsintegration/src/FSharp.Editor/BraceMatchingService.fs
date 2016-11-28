// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Threading.Tasks
open Microsoft.CodeAnalysis.Editor
open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.FSharp.Compiler.SourceCodeServices

[<ExportBraceMatcher(FSharpCommonConstants.FSharpLanguageName)>]
type internal FSharpBraceMatchingService() =

    static member GetBraceMatchingResult(sourceText, fileName, options, position: int) = async {
        let! matchedBraces = FSharpLanguageService.Checker.MatchBracesAlternate(fileName, sourceText.ToString(), options)

        let isPositionInRange range = CommonRoslynHelpers.FSharpRangeToTextSpan(sourceText, range).Contains(position)
        return matchedBraces |> Array.tryFind(fun (left, right) -> isPositionInRange left || isPositionInRange right)
    }
        
    interface IBraceMatcher with
        member this.FindBracesAsync(document, position, cancellationToken) = 
            async {
                match FSharpLanguageService.TryGetOptionsForEditingDocumentOrProject(document)  with 
                | Some options ->
                    let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                    let! result = FSharpBraceMatchingService.GetBraceMatchingResult(sourceText, document.Name, options, position)
                    return match result with
                           | None -> Nullable()
                           | Some(left, right) ->
                               Nullable(BraceMatchingResult(
                                           CommonRoslynHelpers.FSharpRangeToTextSpan(sourceText, left),
                                           CommonRoslynHelpers.FSharpRangeToTextSpan(sourceText, right)))
                | None -> return Nullable()
            } |> CommonRoslynHelpers.StartAsyncAsTask cancellationToken
