// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.Hints

open System.Collections.Immutable
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.InlineHints
open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Compiler.Text
open HintService

module NativeToRoslynHintConverter =

    let rangeToSpan range sourceText = 
        let symbolSpan = RoslynHelpers.FSharpRangeToTextSpan(sourceText, range)
        let overshadowLength = 0 // anything >0 means overlaying the code
        TextSpan(symbolSpan.End, overshadowLength)

    let nativeToRoslynText (taggedText: TaggedText) = 
        let tag = RoslynHelpers.roslynTag taggedText.Tag
        let text = taggedText.Text
        RoslynTaggedText(tag, text)

    let convert sourceText hint =
        let span = rangeToSpan hint.Range sourceText
        let displayParts = hint.Parts |> Seq.map nativeToRoslynText
        FSharpInlineHint(span, displayParts.ToImmutableArray())