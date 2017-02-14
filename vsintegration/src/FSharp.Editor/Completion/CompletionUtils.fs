// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Threading

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Completion
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Text

module CompletionUtils =
    let private completionTriggers = [| '.' |]
    
    let shouldTriggerCompletion(sourceText: SourceText, caretPosition: int, trigger: CompletionTriggerKind, getInfo: (unit -> DocumentId * string * string list)) =
        // Skip if we are at the start of a document
        if caretPosition = 0 then
            false
        
        // Skip if it was triggered by an operation other than insertion
        elif not (trigger = CompletionTriggerKind.Insertion) then
            false
        
        // Skip if we are not on a completion trigger
        else
            let triggerPosition = caretPosition - 1
            let c = sourceText.[triggerPosition]
            
            if not (completionTriggers |> Array.contains c) then
                false
            
            // do not trigger completion if it's not single dot, i.e. range expression
            elif triggerPosition > 0 && sourceText.[triggerPosition - 1] = '.' then
                false
            
            // Trigger completion if we are on a valid classification type
            else
                let documentId, filePath, defines = getInfo()
                let textLines = sourceText.Lines
                let triggerLine = textLines.GetLineFromPosition(triggerPosition)

                let classifiedSpanOption =
                    CommonHelpers.getColorizationData(documentId, sourceText, triggerLine.Span, Some(filePath), defines, CancellationToken.None)
                    |> Seq.tryFind(fun classifiedSpan -> classifiedSpan.TextSpan.Contains(triggerPosition))
                
                match classifiedSpanOption with
                | None -> false
                | Some(classifiedSpan) ->
                    match classifiedSpan.ClassificationType with
                    | ClassificationTypeNames.Comment
                    | ClassificationTypeNames.StringLiteral
                    | ClassificationTypeNames.ExcludedCode
                    | ClassificationTypeNames.NumericLiteral -> false
                    | _ -> true // anything else is a valid classification type