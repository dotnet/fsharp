// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.ComponentModel.Composition
open Microsoft.CodeAnalysis.Editor
open Microsoft.FSharp.Compiler.SourceCodeServices

[<ExportBraceMatcher(FSharpConstants.FSharpLanguageName)>]
type internal FSharpBraceMatchingService 
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: FSharpProjectOptionsManager
    ) =
    
    static let defaultUserOpName = "BraceMatching"

    static member GetBraceMatchingResult(checker: FSharpChecker, sourceText, fileName, parsingOptions: FSharpParsingOptions, caretPosition: int, userOpName: string) = 
        async {
            let! matchedBraces = checker.MatchBraces(fileName, sourceText.ToString(), parsingOptions, userOpName)
            let isPositionInRange range =
                match RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, range) with
                | None -> false
                | Some range ->
                    // We need to see if the caret is contained within or on the outside of the span
                    // that we get back from the language service.
                    //
                    // Ex: let x = ((12))^
                    //
                    // The caret can be on the outside of the last paren, but this is actually 1 position
                    // further to the right than the end of the span that we get back.
                    range.Contains(caretPosition) || range.End = caretPosition + 1
            return matchedBraces |> Array.tryFind(fun (left, right) -> isPositionInRange left || isPositionInRange right)
        }
        
    interface IBraceMatcher with
        member __.FindBracesAsync(document, caretPosition, cancellationToken) = 
            asyncMaybe {
                let! parsingOptions, _options = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document)
                let! sourceText = document.GetTextAsync(cancellationToken)
                let! (left, right) = FSharpBraceMatchingService.GetBraceMatchingResult(checkerProvider.Checker, sourceText, document.Name, parsingOptions, caretPosition, defaultUserOpName)
                let! leftSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, left)
                let! rightSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, right)
                return BraceMatchingResult(leftSpan, rightSpan)
            } 
            |> Async.map Option.toNullable
            |> RoslynHelpers.StartAsyncAsTask cancellationToken
