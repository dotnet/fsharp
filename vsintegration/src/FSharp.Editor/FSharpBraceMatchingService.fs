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

[<ExportBraceMatcher(FSharpCommonConstants.FSharpLanguageName)>]
type internal FSharpBraceMatchingService() =
              
    static member private SupportedBraceTypes = [
        ('(', ')');
        ('<', '>');
        ('[', ']');
        ('{', '}');
    ]
    
    static member private IgnoredClassificationTypes = [
        ClassificationTypeNames.Comment;
        ClassificationTypeNames.StringLiteral;
        ClassificationTypeNames.ExcludedCode;
    ]

    interface IBraceMatcher with
        member this.FindBracesAsync(document: Document, position: int, cancellationToken: CancellationToken): Task<Nullable<BraceMatchingResult>> =
            let computation = async {
                let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                return
                    try
                        FSharpBraceMatchingService.GetBraceMatchingResult(sourceText, document.Name, position, cancellationToken)
                    with ex -> 
                        Assert.Exception(ex)
                        reraise()
            }
            Async.StartAsTask(computation, TaskCreationOptions.None, cancellationToken)
  
    // Helper function to proxy Roslyn types to tests
    static member FindMatchingBrace(sourceText: SourceText, fileName: string, position: int, cancellationToken: CancellationToken) : Option<int> =
        let braceMatchingResult = FSharpBraceMatchingService.GetBraceMatchingResult(sourceText, fileName, position, cancellationToken)
        if braceMatchingResult.HasValue then
            if braceMatchingResult.Value.LeftSpan.Start = position then
                Some(braceMatchingResult.Value.RightSpan.Start)
            else if braceMatchingResult.Value.RightSpan.Start = position then
                Some(braceMatchingResult.Value.LeftSpan.Start)
            else
                None
        else
            None

    static member private GetBraceMatchingResult(sourceText: SourceText, fileName: string, position: int, cancellationToken: CancellationToken) : Nullable<BraceMatchingResult> =
        if position < 0 || position >= sourceText.Length then
            Nullable()
        else
            let classificationData = FSharpColorizationService.GetColorizationData(sourceText, fileName, [], cancellationToken)

            let shouldBeIgnored(characterPosition) =
                match classificationData.GetClassifiedSpan(characterPosition) with
                | None -> false
                | Some(classifiedSpan) -> FSharpBraceMatchingService.IgnoredClassificationTypes |> Seq.contains classifiedSpan.ClassificationType
                
            if shouldBeIgnored(position) then
                Nullable()
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

                match FSharpBraceMatchingService.SupportedBraceTypes |> Seq.tryPick(pickBraceType) with
                | None -> Nullable()
                | Some(proceedFunc, stoppingCondition, matchedBrace, nonMatchedBrace) ->
                    let mutable currentPosition = proceedFunc position
                    let mutable result = Nullable()
                    let mutable braceDepth = 0

                    while result.HasValue = false && stoppingCondition(currentPosition) = false do
                        cancellationToken.ThrowIfCancellationRequested()
                        if shouldBeIgnored(currentPosition) = false then
                            if sourceText.[currentPosition] = matchedBrace then
                                braceDepth <- braceDepth + 1
                            else if sourceText.[currentPosition] = nonMatchedBrace then
                                if braceDepth = 0 then
                                    result <- Nullable(BraceMatchingResult(TextSpan(min position currentPosition, 1), TextSpan(max position currentPosition, 1)))
                                else
                                    braceDepth <- braceDepth - 1
                        currentPosition <- proceedFunc currentPosition
                    result
