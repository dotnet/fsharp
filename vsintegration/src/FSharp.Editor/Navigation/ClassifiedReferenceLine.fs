// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// The line a Find All References entry shows, classified while the search runs, as the C# and VB searches classify it
/// with `ClassifiedSpansAndHighlightSpanFactory`, instead of by the window one entry at a time after the search.
module internal Microsoft.VisualStudio.FSharp.Editor.ClassifiedReferenceLine

open System
open System.Collections.Generic
open System.Collections.Immutable

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Classification
open Microsoft.CodeAnalysis.Text

open CancellableTasks

/// From the first non-whitespace character of the reference's line to the end of the line.
let lineSpanOf (sourceText: SourceText) (referenceSpan: TextSpan) =
    let line = sourceText.Lines.GetLineFromPosition referenceSpan.Start
    let mutable start = line.Start

    while start < line.End && Char.IsWhiteSpace sourceText[start] do
        start <- start + 1

    let firstNonWhitespace = if start = line.End then line.Start else start
    TextSpan.FromBounds(min firstNonWhitespace referenceSpan.Start, line.End)

let private byStart =
    Comparison<ClassifiedSpan>(fun left right -> left.TextSpan.Start - right.TextSpan.Start)

/// Sorted and clipped to the line. A span that starts before the one kept ahead of it ends is emptied, as
/// `ClassifierHelper.AdjustSpans` empties it.
let private adjusted (lineSpan: TextSpan) (spans: List<ClassifiedSpan>) =
    spans.Sort byStart
    let result = List<ClassifiedSpan>(spans.Count)

    for span in spans do
        let intersection = span.TextSpan.Intersection lineSpan

        let kept =
            intersection.HasValue
            && (result.Count = 0
                || result[result.Count - 1].TextSpan.End <= intersection.Value.Start)

        result.Add(ClassifiedSpan(span.ClassificationType, (if kept then intersection.Value else TextSpan())))

    result

/// Every semantic span, and what no semantic span covers of each syntactic one.
let private merged (syntactic: List<ClassifiedSpan>) (semantic: List<ClassifiedSpan>) =
    let semanticParts = semantic.FindAll(fun span -> not span.TextSpan.IsEmpty)
    let result = List<ClassifiedSpan>(semanticParts)

    for part in syntactic do
        let mutable start = part.TextSpan.Start

        for covering in semanticParts do
            if covering.TextSpan.Start < part.TextSpan.End && covering.TextSpan.End > start then
                if covering.TextSpan.Start > start then
                    result.Add(ClassifiedSpan(part.ClassificationType, TextSpan.FromBounds(start, covering.TextSpan.Start)))

                start <- max start covering.TextSpan.End

        if start < part.TextSpan.End then
            result.Add(ClassifiedSpan(part.ClassificationType, TextSpan.FromBounds(start, part.TextSpan.End)))

    result.Sort byStart
    result

let private withGapsFilled (lineStart: int) (spans: List<ClassifiedSpan>) =
    let result = ImmutableArray.CreateBuilder<ClassifiedSpan>(spans.Count)
    let mutable position = lineStart

    for span in spans do
        if not span.TextSpan.IsEmpty then
            if position < span.TextSpan.Start then
                result.Add(ClassifiedSpan(ClassificationTypeNames.Text, TextSpan.FromBounds(position, span.TextSpan.Start)))

            result.Add span
            position <- span.TextSpan.End

    result.ToImmutable()

/// The classified line of the reference, and the reference's place in it: what `ClassifiedSpansAndHighlightSpan` holds.
let classifyAsync (classifier: IFSharpClassificationService) (document: Document) (sourceText: SourceText) (referenceSpan: TextSpan) =
    cancellableTask {
        let! ct = CancellableTask.getCancellationToken ()
        let lineSpan = lineSpanOf sourceText referenceSpan
        let syntactic = List<ClassifiedSpan>()
        let semantic = List<ClassifiedSpan>()
        do! classifier.AddSyntacticClassificationsAsync(document, lineSpan, syntactic, ct)
        do! classifier.AddSemanticClassificationsAsync(document, lineSpan, semantic, ct)

        let classifiedSpans =
            merged (adjusted lineSpan syntactic) (adjusted lineSpan semantic)
            |> withGapsFilled lineSpan.Start

        return struct (classifiedSpans, TextSpan(referenceSpan.Start - lineSpan.Start, referenceSpan.Length))
    }
