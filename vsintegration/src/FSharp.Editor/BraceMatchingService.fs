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
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Tagging

open Microsoft.FSharp.Compiler.Parser
open Microsoft.FSharp.Compiler.SourceCodeServices

// TODO: add defines flags if available from project sites and files

[<ExportBraceMatcher(FSharpCommonConstants.FSharpLanguageName)>]
type internal FSharpBraceMatchingService() =
              
    static let supportedBraceTypes = [
        ('(', ')');
        ('<', '>');
        ('[', ']');
        ('{', '}');
    ]
    
    static let ignoredClassificationTypes = [
        ClassificationTypeNames.Comment;
        ClassificationTypeNames.StringLiteral;
        ClassificationTypeNames.ExcludedCode;
    ]
    
    static let getBraceMatchingResult(sourceText: SourceText, fileName: Option<string>, defines: string list, position: int, cancellationToken: CancellationToken) : Option<BraceMatchingResult> =
        if position < 0 || position >= sourceText.Length then
            None
        else
            let shouldBeIgnored(characterPosition) =
                let textSpan = TextSpan(characterPosition, 1)
                let classifiedSpans = FSharpColorizationService.GetColorizationData(sourceText, textSpan, fileName, defines, cancellationToken)
                if classifiedSpans.Any() then
                    ignoredClassificationTypes |> Seq.contains (classifiedSpans.First().ClassificationType)
                else
                    false
                
            if shouldBeIgnored(position) then
                None
            else
                let currentCharacter = sourceText.[position]

                let proceedToStartOfString(i) = i - 1
                let proceedToEndOfString(i) = i + 1

                let afterEndOfString(i) = i >= sourceText.Length
                let beforeStartOfString(i) = i < 0

                let pickBraceType(leftBrace, rightBrace) =
                    if currentCharacter = leftBrace then Some(proceedToEndOfString, afterEndOfString, leftBrace, rightBrace)
                    else if currentCharacter = rightBrace then Some(proceedToStartOfString, beforeStartOfString, rightBrace, leftBrace)
                    else None

                match supportedBraceTypes |> List.tryPick(pickBraceType) with
                | None -> None
                | Some(proceedFunc, stoppingCondition, matchedBrace, nonMatchedBrace) ->
                    let mutable currentPosition = proceedFunc position
                    let mutable result = None
                    let mutable braceDepth = 0

                    while result.IsSome = false && stoppingCondition(currentPosition) = false do
                        cancellationToken.ThrowIfCancellationRequested()
                        if shouldBeIgnored(currentPosition) = false then
                            if sourceText.[currentPosition] = matchedBrace then
                                braceDepth <- braceDepth + 1
                            else if sourceText.[currentPosition] = nonMatchedBrace then
                                if braceDepth = 0 then
                                    result <- Some(BraceMatchingResult(TextSpan(min position currentPosition, 1), TextSpan(max position currentPosition, 1)))
                                else
                                    braceDepth <- braceDepth - 1
                        currentPosition <- proceedFunc currentPosition
                    result

    interface IBraceMatcher with
        member this.FindBracesAsync(document: Document, position: int, cancellationToken: CancellationToken): Task<Nullable<BraceMatchingResult>> =
            document.GetTextAsync(cancellationToken).ContinueWith(
                fun (sourceTextTask: Task<SourceText>) ->
                    try match getBraceMatchingResult(sourceTextTask.Result, Some(document.Name), [], position, cancellationToken) with
                        | None -> Nullable()
                        | Some(braceMatchingResult) -> Nullable(braceMatchingResult)
                    with ex -> 
                        Assert.Exception(ex)
                        reraise()
                , TaskContinuationOptions.OnlyOnRanToCompletion)

    // Helper function to proxy Roslyn types to tests
    static member FindMatchingBrace(sourceText: SourceText, fileName: Option<string>, defines: string list, position: int, cancellationToken: CancellationToken) : Option<int> =
        match getBraceMatchingResult(sourceText, fileName, defines, position, cancellationToken) with
        | None -> None
        | Some(braceMatchingResult) ->
            if braceMatchingResult.LeftSpan.Start = position then
                Some(braceMatchingResult.RightSpan.Start)
            else if braceMatchingResult.RightSpan.Start = position then
                Some(braceMatchingResult.LeftSpan.Start)
            else
                None