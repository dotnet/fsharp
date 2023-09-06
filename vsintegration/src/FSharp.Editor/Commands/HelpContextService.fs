﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Generic
open System.Composition
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Classification
open Microsoft.VisualStudio.LanguageServices.Implementation.F1Help
open Microsoft.CodeAnalysis.Host.Mef
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open Microsoft.CodeAnalysis
open CancellableTasks

[<Shared>]
[<ExportLanguageService(typeof<IHelpContextService>, FSharpConstants.FSharpLanguageName)>]
type internal FSharpHelpContextService [<ImportingConstructor>] () =

    static member GetHelpTerm(document: Document, span: TextSpan, tokens: List<ClassifiedSpan>) : CancellableTask<string> =
        cancellableTask {
            let! cancellationToken = CancellableTask.getCancellationToken ()
            let! _, check = document.GetFSharpParseAndCheckResultsAsync(nameof (FSharpHelpContextService))

            let! sourceText = document.GetTextAsync(cancellationToken)
            let textLines = sourceText.Lines
            let lineInfo = textLines.GetLineFromPosition(span.Start)
            let line = lineInfo.LineNumber
            let lineText = lineInfo.ToString()

            let caretColumn = textLines.GetLinePosition(span.Start).Character

            let shouldTryToFindSurroundingIdent (token: ClassifiedSpan) =
                let content = sourceText.GetSubText(token.TextSpan)

                match token.ClassificationType with
                | ClassificationTypeNames.Text
                | ClassificationTypeNames.WhiteSpace -> true
                | (ClassificationTypeNames.Operator | ClassificationTypeNames.Punctuation) when content.Length > 0 && content.[0] = '.' ->
                    true
                | _ -> false

            let tokenInformation, col =
                let col =
                    if caretColumn = lineText.Length && caretColumn > 0 then
                        // if we are at the end of the line, we always step back one character
                        caretColumn - 1
                    else
                        caretColumn

                let getTokenAt line col =
                    if col < 0 || line < 0 then
                        ValueNone
                    else
                        let start = textLines.[line].Start + col
                        let span = TextSpan.FromBounds(start, start + 1)

                        tokens
                        |> Seq.tryFindIndexV (fun t -> t.TextSpan.Contains(span))
                        |> ValueOption.map (fun i -> tokens[i])

                match getTokenAt line col with
                | ValueSome t as original -> // when col > 0 && shouldTryToFindSurroundingIdent t ->
                    if shouldTryToFindSurroundingIdent t then
                        match getTokenAt line (col - 1) with
                        | ValueSome t as newInfo when not (shouldTryToFindSurroundingIdent t) -> newInfo, col - 1
                        | _ ->
                            match getTokenAt line (col + 1) with
                            | ValueSome t as newInfo when not (shouldTryToFindSurroundingIdent t) -> newInfo, col + 1
                            | _ -> original, col
                    else
                        original, col
                | otherwise -> otherwise, col

            match tokenInformation with
            | ValueNone -> return ""
            | ValueSome token ->
                match token.ClassificationType with
                | ClassificationTypeNames.Keyword
                | ClassificationTypeNames.Operator
                | ClassificationTypeNames.PreprocessorKeyword -> return sourceText.GetSubText(token.TextSpan).ToString() + "_FS"
                | ClassificationTypeNames.Comment -> return "comment_FS"
                | ClassificationTypeNames.Identifier ->
                    try
                        let island = QuickParse.GetCompleteIdentifierIsland false lineText col

                        match island with
                        | Some (s, colAtEndOfNames, _) when check.HasFullTypeCheckInfo ->
                            let qualId = PrettyNaming.GetLongNameFromString s

                            let f1Keyword =
                                check.GetF1Keyword(Line.fromZ line, colAtEndOfNames, lineText, qualId)

                            return Option.defaultValue "" f1Keyword

                        | _ -> return ""
                    with e ->
                        Assert.Exception e
                        return ""
                | _ -> return ""
        }

    interface IHelpContextService with
        member _.Language = FSharpConstants.FSharpLanguageLongName
        member _.Product = FSharpConstants.FSharpLanguageLongName

        member _.GetHelpTermAsync(document, textSpan, cancellationToken) =
            cancellableTask {
                let! cancellationToken = CancellableTask.getCancellationToken ()
                let! sourceText = document.GetTextAsync(cancellationToken)

                let defines, langVersion, strictIndentation = document.GetFsharpParsingOptions()

                let textLine = sourceText.Lines.GetLineFromPosition(textSpan.Start)

                let classifiedSpans = ResizeArray<_>()

                Tokenizer.classifySpans (
                    document.Id,
                    sourceText,
                    textLine.Span,
                    Some document.Name,
                    defines,
                    Some langVersion,
                    strictIndentation,
                    classifiedSpans,
                    cancellationToken
                )

                return! FSharpHelpContextService.GetHelpTerm(document, textSpan, classifiedSpans)
            }
            |> CancellableTask.start cancellationToken

        member _.FormatSymbol(_symbol) = Unchecked.defaultof<_>
