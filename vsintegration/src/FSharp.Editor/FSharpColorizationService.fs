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
open Microsoft.CodeAnalysis.Editor.Implementation.Classification
open Microsoft.CodeAnalysis.Editor.Shared.Utilities
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Tagging

open Microsoft.FSharp.Compiler.SourceCodeServices

[<ExportLanguageService(typeof<IEditorClassificationService>, FSharpCommonConstants.FSharpLanguageName)>]
type internal FSharpColorizationService() =

    // Create expensive resources upfront
    let sourceTokenizer = FSharpSourceTokenizer([], String.Empty)
    let lineEndingStates = new ConcurrentDictionary<int, FSharpTokenizerLexState>()

    interface IEditorClassificationService with
        
        member this.AddLexicalClassifications(text: SourceText, textSpan: TextSpan, result: List<ClassifiedSpan>, cancellationToken: CancellationToken) =
            this.ClassifySourceTextAsync(text, textSpan, result, cancellationToken).Wait(cancellationToken)
        
        member this.AddSyntacticClassificationsAsync(document: Document, textSpan: TextSpan, result: List<ClassifiedSpan>, cancellationToken: CancellationToken) =
            this.ClassifyDocumentAsync(document, textSpan, result, cancellationToken)

        member this.AddSemanticClassificationsAsync(document: Document, textSpan: TextSpan, result: List<ClassifiedSpan>, cancellationToken: CancellationToken) =
            this.ClassifyDocumentAsync(document, textSpan, result, cancellationToken)

        member this.AdjustStaleClassification(text: SourceText, classifiedSpan: ClassifiedSpan) : ClassifiedSpan =
            let result = new List<ClassifiedSpan>()
            this.ClassifySourceTextAsync(text, classifiedSpan.TextSpan, result, CancellationToken.None).Wait()
            if result.Any() then result.First() else new ClassifiedSpan(ClassificationTypeNames.WhiteSpace, classifiedSpan.TextSpan)

    member this.ClassifyDocumentAsync(document: Document, textSpan: TextSpan, result: List<ClassifiedSpan>, cancellationToken: CancellationToken) =
            let sourceText = document.GetTextAsync(cancellationToken).Result
            // TODO: add types colorization if available from intellisense
            this.ClassifySourceTextAsync(sourceText, textSpan, result, cancellationToken)
        
    member this.ClassifySourceTextAsync(text: SourceText, textSpan: TextSpan, result: List<ClassifiedSpan>, cancellationToken: CancellationToken) =
        Task.Run(fun () ->
            let startLineNumber = text.Lines.GetLineFromPosition(textSpan.Start).LineNumber
            let endLineNumber = text.Lines.GetLineFromPosition(textSpan.End).LineNumber

            for i = startLineNumber to endLineNumber - 1 do
                cancellationToken.ThrowIfCancellationRequested()

                let currentLine = text.Lines.Item(i)
                let previousLexState = ref(if i = 0 then 0L else lineEndingStates.GetOrAdd(i - 1, 0L))
                let tokens = this.ClassifySourceLineAsync(currentLine, previousLexState)

                result.AddRange(tokens)
                lineEndingStates.[i] <- previousLexState.Value
        )

    member this.ClassifySourceLineAsync(textLine: TextLine, previousLexState: Ref<FSharpTokenizerLexState>): List<ClassifiedSpan> =
        let lineTokenizer = sourceTokenizer.CreateLineTokenizer(textLine.Text.ToString(textLine.Span))
        let colorMap = Array.create textLine.Span.Length ClassificationTypeNames.WhiteSpace
        
        let scanNextToken() =
            let tokenInfoOption, currentLexState = lineTokenizer.ScanToken(previousLexState.Value)
            previousLexState := currentLexState
            match tokenInfoOption with
            | None -> false
            | Some(tokenInfo) ->
                let classificationType = this.CompilerTokenToRoslynToken(tokenInfo.ColorClass)
                for i = tokenInfo.LeftColumn to tokenInfo.RightColumn do
                    Array.set colorMap i classificationType
                true

        while scanNextToken() do ()

        let mutable startPosition = 0
        let mutable endPosition = startPosition
        let result = new List<ClassifiedSpan>()

        while startPosition < colorMap.Length do
            let classificationType = colorMap.[startPosition]
            endPosition <- startPosition
            while endPosition < colorMap.Length && classificationType = colorMap.[endPosition] do
                endPosition <- endPosition + 1
            let textSpan = new TextSpan(textLine.Start + startPosition, endPosition - startPosition + 1)
            result.Add(new ClassifiedSpan(classificationType, textSpan))
            startPosition <- endPosition

        result

    member this.CompilerTokenToRoslynToken(colorKind: FSharpTokenColorKind) : string = 
        match colorKind with
        | FSharpTokenColorKind.Comment -> ClassificationTypeNames.Comment
        | FSharpTokenColorKind.Identifier -> ClassificationTypeNames.Identifier
        | FSharpTokenColorKind.Keyword -> ClassificationTypeNames.Keyword
        | FSharpTokenColorKind.String -> ClassificationTypeNames.StringLiteral
        | FSharpTokenColorKind.Text -> ClassificationTypeNames.Text
        | FSharpTokenColorKind.UpperIdentifier -> ClassificationTypeNames.Identifier
        | FSharpTokenColorKind.Number -> ClassificationTypeNames.NumericLiteral
        | FSharpTokenColorKind.InactiveCode -> ClassificationTypeNames.ExcludedCode 
        | FSharpTokenColorKind.PreprocessorKeyword -> ClassificationTypeNames.PreprocessorKeyword 
        | FSharpTokenColorKind.Operator -> ClassificationTypeNames.Operator
        | FSharpTokenColorKind.TypeName  -> ClassificationTypeNames.ClassName
        | FSharpTokenColorKind.Default | _ -> ClassificationTypeNames.Text
