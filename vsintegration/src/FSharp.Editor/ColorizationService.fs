// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Concurrent
open System.Collections.Generic
open System.Linq
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices

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

type private SourceLineData(lexStateAtEndOfLine: FSharpTokenizerLexState, hashCode: int, classifiedSpans: IReadOnlyList<ClassifiedSpan>) =
    member val LexStateAtEndOfLine = lexStateAtEndOfLine
    member val HashCode = hashCode
    member val ClassifiedSpans = classifiedSpans

type private SourceTextData(lines: int) =
    member val Lines = Array.create<Option<SourceLineData>> lines None

[<ExportLanguageService(typeof<IEditorClassificationService>, FSharpCommonConstants.FSharpLanguageName)>]
type internal FSharpColorizationService() =

    static let DataCache = ConditionalWeakTable<SourceText, SourceTextData>()
    
    static let scanSourceLine(sourceTokenizer: FSharpSourceTokenizer, textLine: TextLine, lineContents: string, lexState: FSharpTokenizerLexState) : SourceLineData =
    
        let colorMap = Array.create textLine.Span.Length ClassificationTypeNames.Text
        let lineTokenizer = sourceTokenizer.CreateLineTokenizer(lineContents)

        let compilerTokenToRoslynToken(colorKind: FSharpTokenColorKind) : string = 
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
            
        let scanAndColorNextToken(lineTokenizer: FSharpLineTokenizer, lexState: Ref<FSharpTokenizerLexState>) : Option<FSharpTokenInfo> =
            let tokenInfoOption, nextLexState = lineTokenizer.ScanToken(lexState.Value)
            lexState.Value <- nextLexState
            if tokenInfoOption.IsSome then
                let classificationType = compilerTokenToRoslynToken(tokenInfoOption.Value.ColorClass)
                for i = tokenInfoOption.Value.LeftColumn to tokenInfoOption.Value.RightColumn do
                    Array.set colorMap i classificationType
            tokenInfoOption

        let previousLextState = ref(lexState)
        let mutable tokenInfoOption = scanAndColorNextToken(lineTokenizer, previousLextState)
        while tokenInfoOption.IsSome do
            tokenInfoOption <- scanAndColorNextToken(lineTokenizer, previousLextState)

        let mutable startPosition = 0
        let mutable endPosition = startPosition
        let classifiedSpans = new List<ClassifiedSpan>()

        while startPosition < colorMap.Length do
            let classificationType = colorMap.[startPosition]
            endPosition <- startPosition
            while endPosition < colorMap.Length && classificationType = colorMap.[endPosition] do
                endPosition <- endPosition + 1
            let textSpan = new TextSpan(textLine.Start + startPosition, endPosition - startPosition)
            classifiedSpans.Add(new ClassifiedSpan(classificationType, textSpan))
            startPosition <- endPosition

        SourceLineData(previousLextState.Value, lineContents.GetHashCode(), classifiedSpans)

    static member GetColorizationData(sourceText: SourceText, textSpan: TextSpan, fileName: Option<string>, defines: string list, cancellationToken: CancellationToken) : List<ClassifiedSpan> =
        try
            let sourceTokenizer = FSharpSourceTokenizer(defines, fileName)
            let sourceTextData = DataCache.GetValue(sourceText, fun key -> SourceTextData(key.Lines.Count))

            let startLine = sourceText.Lines.GetLineFromPosition(textSpan.Start).LineNumber
            let endLine = sourceText.Lines.GetLineFromPosition(textSpan.End).LineNumber
            
            // Get the last cached scanned line
            let mutable scanStartLine = startLine
            while scanStartLine > 0 && sourceTextData.Lines.[scanStartLine - 1].IsNone do
                scanStartLine <- scanStartLine - 1
                
            let result = new List<ClassifiedSpan>()
            let mutable lexState = if scanStartLine = 0 then 0L else sourceTextData.Lines.[scanStartLine - 1].Value.LexStateAtEndOfLine

            for i = scanStartLine to sourceText.Lines.Count - 1 do
                cancellationToken.ThrowIfCancellationRequested()

                let textLine = sourceText.Lines.[i]
                let lineContents = textLine.Text.ToString(textLine.Span)
                let lineHashCode = lineContents.GetHashCode()

                let mutable lineData = sourceTextData.Lines.[i]
                if lineData.IsNone || lineData.Value.HashCode <> lineHashCode then
                    lineData <- Some(scanSourceLine(sourceTokenizer, textLine, lineContents, lexState))
                    
                lexState <- lineData.Value.LexStateAtEndOfLine
                sourceTextData.Lines.[i] <- lineData

                if startLine <= i && i <= endLine then
                    result.AddRange(lineData.Value.ClassifiedSpans |> Seq.filter(fun token ->
                        textSpan.Contains(token.TextSpan.Start) ||
                        textSpan.Contains(token.TextSpan.End - 1) ||
                        (token.TextSpan.Start <= textSpan.Start && textSpan.End <= token.TextSpan.End)))

            result
        with ex -> 
            Assert.Exception(ex)
            reraise()  

    interface IEditorClassificationService with
        
        // Do not perform classification if we don't have project options (#defines matter)
        member this.AddLexicalClassifications(_: SourceText, _: TextSpan, _: List<ClassifiedSpan>, _: CancellationToken) = ()
        
        member this.AddSyntacticClassificationsAsync(document: Document, textSpan: TextSpan, result: List<ClassifiedSpan>, cancellationToken: CancellationToken) =
            document.GetTextAsync(cancellationToken).ContinueWith(
                fun (sourceTextTask: Task<SourceText>) ->
                    match FSharpLanguageService.GetOptions(document.Project.Id) with
                    | Some(options) ->
                        let defines = CompilerEnvironment.GetCompilationDefinesForEditing(document.Name, options.OtherOptions |> Seq.toList)
                        if sourceTextTask.Status = TaskStatus.RanToCompletion then
                            result.AddRange(FSharpColorizationService.GetColorizationData(sourceTextTask.Result, textSpan, None, defines, cancellationToken))
                    | None -> ()
                , cancellationToken)

        // FSROSLYNTODO: Due to issue 12732 on Roslyn side, semantic classification is tied to C#/VB only.
        // Once that is exposed to F#, enable the below code path, and add tests accourdingly.
        member this.AddSemanticClassificationsAsync(_, _, _, _) =
            (*
            let computation = async {
                try
                    let options = CommonRoslynHelpers.GetFSharpProjectOptionsForRoslynProject(document.Project)
                    let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                    let! parseResults = FSharpChecker.Instance.ParseFileInProject(document.Name, sourceText.ToString(), options)
                    let! checkResultsAnswer = FSharpChecker.Instance.CheckFileInProject(parseResults, document.Name, 0, textSpan.ToString(), options)

                    let extraColorizationData = match checkResultsAnswer with
                                                | FSharpCheckFileAnswer.Aborted -> failwith "Compilation isn't complete yet"
                                                | FSharpCheckFileAnswer.Succeeded(results) -> results.GetExtraColorizationsAlternate()
                                                |> Seq.map(fun (range, tokenColorKind) -> ClassifiedSpan(CommonRoslynHelpers.FSharpRangeToTextSpan(sourceText, range), compilerTokenToRoslynToken(tokenColorKind)))
                                                |> Seq.toList

                    result.AddRange(extraColorizationData)
                with ex ->
                    Assert.Exception(ex)
                    raise(ex)
            }
            Task.Run(fun () -> Async.RunSynchronously(computation, cancellationToken = cancellationToken))
            *)
            Task.CompletedTask
            
        // Do not perform classification if we don't have project options (#defines matter)
        member this.AdjustStaleClassification(_: SourceText, classifiedSpan: ClassifiedSpan) : ClassifiedSpan = classifiedSpan
