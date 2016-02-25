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
    
    interface IBraceMatcher with
        member this.FindBracesAsync(document: Document, position: int, cancellationToken: CancellationToken): Task<Nullable<BraceMatchingResult>> =
            let computation = async {
                let! text = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                return this.FindBraces(text, position, cancellationToken)
            }
            Async.StartAsTask(computation, TaskCreationOptions.None, cancellationToken)
            
    member this.SupportedBraceTypes = [
        ('(', ')');
        ('<', '>');
        ('[', ']');
        ('{', '}');
    ]

    member this.FindBraces(text: SourceText, position: int, cancellationToken: CancellationToken) : Nullable<BraceMatchingResult> =
        let currentCharacter = text.[position]

        let proceedToLeft(i) = i - 1
        let proceedToRight(i) = i + 1

        let afterEndOfString(i) = i >= text.Length
        let beforeStartOfString(i) = i < 0

        let pickBraceType(leftBrace, rightBrace) =
            if currentCharacter = leftBrace then Some(proceedToRight, afterEndOfString, leftBrace, rightBrace)
            else if currentCharacter = rightBrace then Some(proceedToLeft, beforeStartOfString, rightBrace, leftBrace)
            else None

        match this.SupportedBraceTypes |> Seq.tryPick(pickBraceType) with
        | None -> Nullable()
        | Some(proceedFunc, stoppingCondition, matchedBrace, nonMatchedBrace) ->
            let mutable currentPosition = proceedFunc position
            let mutable result = Nullable()
            let mutable braceDepth = 0

            // TODO: ignore brace matching if part of a string literal or a comment

            while result.HasValue = false && stoppingCondition(currentPosition) = false do
                cancellationToken.ThrowIfCancellationRequested()
                if text.[currentPosition] = matchedBrace then
                    braceDepth <- braceDepth + 1
                else if text.[currentPosition] = nonMatchedBrace then
                    if braceDepth = 0 then
                        result <- Nullable(BraceMatchingResult(TextSpan(min position currentPosition, 1), TextSpan(max position currentPosition, 1)))
                    else
                        braceDepth <- braceDepth - 1
                currentPosition <- proceedFunc currentPosition
            result
