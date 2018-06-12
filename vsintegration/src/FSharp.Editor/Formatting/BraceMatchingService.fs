// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.ComponentModel.Composition
open Microsoft.CodeAnalysis.Editor
open Microsoft.FSharp.Compiler.SourceCodeServices
open System.Runtime.InteropServices

[<ExportBraceMatcher(FSharpConstants.FSharpLanguageName)>]
type internal FSharpBraceMatchingService 
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: FSharpProjectOptionsManager
    ) =
    
    static let defaultUserOpName = "BraceMatching"

    static member GetBraceMatchingResult(checker: FSharpChecker, sourceText, fileName, parsingOptions: FSharpParsingOptions, position: int, userOpName: string, [<Optional; DefaultParameterValue(false)>] forFormatting: bool) = 
        async {
            let! matchedBraces = checker.MatchBraces(fileName, sourceText.ToString(), parsingOptions, userOpName)
            let isPositionInRange range = 
                match RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, range) with
                | None -> false
                | Some span ->
                    if forFormatting then
                        let length = position - span.Start
                        length >= 0 && length <= span.Length
                    else
                        span.Contains position
            return matchedBraces |> Array.tryFind(fun (left, right) -> isPositionInRange left || isPositionInRange right)
        }
        
    interface IBraceMatcher with
        member this.FindBracesAsync(document, position, cancellationToken) = 
            asyncMaybe {
                let! parsingOptions, _options = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document)
                let! sourceText = document.GetTextAsync(cancellationToken)
                let! (left, right) = FSharpBraceMatchingService.GetBraceMatchingResult(checkerProvider.Checker, sourceText, document.Name, parsingOptions, position, defaultUserOpName)
                let! leftSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, left)
                let! rightSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, right)
                return BraceMatchingResult(leftSpan, rightSpan)
            } 
            |> Async.map Option.toNullable
            |> RoslynHelpers.StartAsyncAsTask cancellationToken
