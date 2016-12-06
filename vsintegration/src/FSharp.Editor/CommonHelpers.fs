// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.FSharp.Compiler.SourceCodeServices

module CommonHelpers =
    type private SourceLineData(lineStart: int, lexStateAtStartOfLine: FSharpTokenizerLexState, lexStateAtEndOfLine: FSharpTokenizerLexState, hashCode: int, classifiedSpans: IReadOnlyList<ClassifiedSpan>) =
        member val LineStart = lineStart
        member val LexStateAtStartOfLine = lexStateAtStartOfLine
        member val LexStateAtEndOfLine = lexStateAtEndOfLine
        member val HashCode = hashCode
        member val ClassifiedSpans = classifiedSpans
    
        member data.IsValid(textLine: TextLine) =
            data.LineStart = textLine.Start && 
            let lineContents = textLine.Text.ToString(textLine.Span)
            data.HashCode = lineContents.GetHashCode() 
    
    type private SourceTextData(approxLines: int) =
        let data = ResizeArray<SourceLineData option>(approxLines)
        let extendTo i =
            if i >= data.Count then 
                data.Capacity <- i + 1
                for j in data.Count .. i do
                    data.Add(None)
        member x.Item 
          with get (i:int) = extendTo  i; data.[i]
          and set (i:int) v = extendTo  i; data.[i] <- v
    
        member x.ClearFrom(n) =
            let mutable i = n
            while i < data.Count && data.[i].IsSome do
                data.[i] <- None
                i <- i + 1

    let private dataCache = ConditionalWeakTable<DocumentId, SourceTextData>()

    let internal compilerTokenToRoslynToken(colorKind: FSharpTokenColorKind) : string = 
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
        | FSharpTokenColorKind.Default 
        | _ -> ClassificationTypeNames.Text

    let private scanSourceLine(sourceTokenizer: FSharpSourceTokenizer, textLine: TextLine, lineContents: string, lexState: FSharpTokenizerLexState) : SourceLineData =
        let colorMap = Array.create textLine.Span.Length ClassificationTypeNames.Text
        let lineTokenizer = sourceTokenizer.CreateLineTokenizer(lineContents)
            
        let scanAndColorNextToken(lineTokenizer: FSharpLineTokenizer, lexState: Ref<FSharpTokenizerLexState>) : Option<FSharpTokenInfo> =
            let tokenInfoOption, nextLexState = lineTokenizer.ScanToken(lexState.Value)
            lexState.Value <- nextLexState
            if tokenInfoOption.IsSome then
                let classificationType = compilerTokenToRoslynToken(tokenInfoOption.Value.ColorClass)
                for i = tokenInfoOption.Value.LeftColumn to tokenInfoOption.Value.RightColumn do
                    Array.set colorMap i classificationType
            tokenInfoOption

        let previousLexState = ref lexState
        let mutable tokenInfoOption = scanAndColorNextToken(lineTokenizer, previousLexState)
        while tokenInfoOption.IsSome do
            tokenInfoOption <- scanAndColorNextToken(lineTokenizer, previousLexState)

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

        SourceLineData(textLine.Start, lexState, previousLexState.Value, lineContents.GetHashCode(), classifiedSpans)

    let getColorizationData(documentKey: DocumentId, sourceText: SourceText, textSpan: TextSpan, fileName: string option, defines: string list, cancellationToken: CancellationToken) : List<ClassifiedSpan> =
        try
            let sourceTokenizer = FSharpSourceTokenizer(defines, fileName)
            let lines = sourceText.Lines
            // We keep incremental data per-document.  When text changes we correlate text line-by-line (by hash codes of lines)
            let sourceTextData = dataCache.GetValue(documentKey, fun key -> SourceTextData(lines.Count))

            let startLine = lines.GetLineFromPosition(textSpan.Start).LineNumber
            let endLine = lines.GetLineFromPosition(textSpan.End).LineNumber
            
            // Go backwards to find the last cached scanned line that is valid
            let scanStartLine = 
                let mutable i = startLine
                while i > 0 && (match sourceTextData.[i-1] with Some data -> not (data.IsValid(lines.[i])) | None -> true)  do
                    i <- i - 1
                i
                
            // Rescan the lines if necessary and report the information
            let result = new List<ClassifiedSpan>()
            let mutable lexState = if scanStartLine = 0 then 0L else sourceTextData.[scanStartLine - 1].Value.LexStateAtEndOfLine

            for i = scanStartLine to endLine do
                cancellationToken.ThrowIfCancellationRequested()
                let textLine = lines.[i]
                let lineContents = textLine.Text.ToString(textLine.Span)

                let lineData = 
                    // We can reuse the old data when 
                    //   1. the line starts at the same overall position
                    //   2. the hash codes match
                    //   3. the start-of-line lex states are the same
                    match sourceTextData.[i] with 
                    | Some data when data.IsValid(textLine) && data.LexStateAtStartOfLine = lexState -> 
                        data
                    | _ -> 
                        // Otherwise, we recompute
                        let newData = scanSourceLine(sourceTokenizer, textLine, lineContents, lexState)
                        sourceTextData.[i] <- Some newData
                        newData
                    
                lexState <- lineData.LexStateAtEndOfLine

                if startLine <= i then
                    result.AddRange(lineData.ClassifiedSpans |> Seq.filter(fun token ->
                        textSpan.Contains(token.TextSpan.Start) ||
                        textSpan.Contains(token.TextSpan.End - 1) ||
                        (token.TextSpan.Start <= textSpan.Start && textSpan.End <= token.TextSpan.End)))

            // If necessary, invalidate all subsequent lines after endLine
            if endLine < lines.Count - 1 then 
                match sourceTextData.[endLine+1] with 
                | Some data  -> 
                    if data.LexStateAtStartOfLine <> lexState then
                        sourceTextData.ClearFrom (endLine+1)
                | None -> ()

            result
        with 
        | :? System.OperationCanceledException -> reraise()
        |  ex -> 
            Assert.Exception(ex)
            List<ClassifiedSpan>()

    let tryClassifyAtPosition (documentKey, sourceText: SourceText, filePath, defines, position: int, cancellationToken) =
        let textLine = sourceText.Lines.GetLineFromPosition(position)
        let textLinePos = sourceText.Lines.GetLinePosition(position)
        let textLineColumn = textLinePos.Character

        let classifiedSpanOption =
            getColorizationData(documentKey, sourceText, textLine.Span, Some(filePath), defines, cancellationToken)
            |> Seq.tryFind(fun classifiedSpan -> classifiedSpan.TextSpan.Contains(position))

        match classifiedSpanOption with
        | Some(classifiedSpan) ->
            match classifiedSpan.ClassificationType with
            | ClassificationTypeNames.ClassName
            | ClassificationTypeNames.DelegateName
            | ClassificationTypeNames.EnumName
            | ClassificationTypeNames.InterfaceName
            | ClassificationTypeNames.ModuleName
            | ClassificationTypeNames.StructName
            | ClassificationTypeNames.TypeParameterName
            | ClassificationTypeNames.Identifier -> 
                match QuickParse.GetCompleteIdentifierIsland true (textLine.ToString()) textLineColumn with
                | Some (islandIdentifier, islandColumn, isQuoted) -> 
                    let qualifiers = if isQuoted then [islandIdentifier] else islandIdentifier.Split '.' |> Array.toList
                    Some (islandColumn, qualifiers, classifiedSpan.TextSpan)
                | None -> None
            | ClassificationTypeNames.Operator ->
                let islandColumn = sourceText.Lines.GetLinePositionSpan(classifiedSpan.TextSpan).End.Character
                Some (islandColumn, [""], classifiedSpan.TextSpan) 
            | _ -> None
        | _ -> None