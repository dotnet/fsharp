// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.ComponentModel.Composition
open Microsoft.CodeAnalysis.Editor
open Microsoft.FSharp.Compiler.SourceCodeServices

[<ExportBraceMatcher(FSharpConstants.FSharpLanguageName)>]
type internal FSharpBraceMatchingService 
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: ProjectInfoManager
    ) =

    static let userOpName = "BraceMatching"
    static member GetBraceMatchingResult(checker: FSharpChecker, sourceText, fileName, options, position: int) = 
        async {
            let! matchedBraces = checker.MatchBraces(fileName, sourceText.ToString(), options, userOpName = userOpName)
            let isPositionInRange range = 
                match RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, range) with
                | None -> false
                | Some range -> range.Contains(position)
            return matchedBraces |> Array.tryFind(fun (left, right) -> isPositionInRange left || isPositionInRange right)
        }
        
    interface IBraceMatcher with
        member this.FindBracesAsync(document, position, cancellationToken) = 
            asyncMaybe {
                let! options = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document)
                let! sourceText = document.GetTextAsync(cancellationToken)
                let! (left, right) = FSharpBraceMatchingService.GetBraceMatchingResult(checkerProvider.Checker, sourceText, document.Name, options, position)
                let! leftSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, left)
                let! rightSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, right)
                return BraceMatchingResult(leftSpan, rightSpan)
            } 
            |> Async.map Option.toNullable
            |> RoslynHelpers.StartAsyncAsTask cancellationToken
