// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.ClassifiedReferenceLineTests

open Xunit

open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Classification
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor

open FSharp.Editor.Tests.Helpers

let private source =
    """
module M

let add x y = x + y

let result =
    add 1 2
"""

[<Fact>]
let ``A reference line is classified from its first non-whitespace character with every character covered`` () =
    let document =
        RoslynTestHelpers.CreateSolution(source).Projects
        |> Seq.exactlyOne
        |> _.Documents
        |> Seq.exactlyOne

    let sourceText = document.GetTextAsync().Result

    let referenceSpan =
        TextSpan(source.LastIndexOf("add", System.StringComparison.Ordinal), 3)

    let classifier = FSharpClassificationService() :> IFSharpClassificationService

    let struct (classifiedSpans, highlightSpan) =
        (ClassifiedReferenceLine.classifyAsync classifier document sourceText referenceSpan) System.Threading.CancellationToken.None
        |> _.Result

    let line = sourceText.Lines.GetLineFromPosition referenceSpan.Start
    Assert.Equal("add 1 2", sourceText.ToString(TextSpan.FromBounds(classifiedSpans[0].TextSpan.Start, line.End)))
    Assert.Equal(line.End, classifiedSpans[classifiedSpans.Length - 1].TextSpan.End)

    for previous, next in Seq.pairwise classifiedSpans do
        Assert.Equal(previous.TextSpan.End, next.TextSpan.Start)

    Assert.Equal(TextSpan(0, 3), highlightSpan)

    let reference =
        classifiedSpans |> Seq.find (fun span -> span.TextSpan = referenceSpan)

    Assert.NotEqual<string>(ClassificationTypeNames.Text, reference.ClassificationType)

    Assert.Contains(classifiedSpans, (fun span -> span.ClassificationType = ClassificationTypeNames.NumericLiteral))
