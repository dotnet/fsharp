// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Threading

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Text

module internal CompletionUtils =
    let shouldProvideCompletion (documentId: DocumentId, filePath: string, defines: string list, text: SourceText, position: int) : bool =
        let textLines = text.Lines
        let triggerLine = textLines.GetLineFromPosition position
        let colorizationData = CommonHelpers.getColorizationData(documentId, text, triggerLine.Span, Some filePath, defines, CancellationToken.None)
        colorizationData.Count = 0 || // we should provide completion at the start of empty line, where there are no tokens at all
        colorizationData.Exists (fun classifiedSpan -> 
            classifiedSpan.TextSpan.IntersectsWith position &&
            (
                match classifiedSpan.ClassificationType with
                | ClassificationTypeNames.Comment
                | ClassificationTypeNames.StringLiteral
                | ClassificationTypeNames.ExcludedCode
                | ClassificationTypeNames.NumericLiteral -> false
                | _ -> true // anything else is a valid classification type
            ))