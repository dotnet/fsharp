// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.ComponentModel.Composition
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.SourceCodeServices.ItemDescriptionIcons

[<RequireQualifiedAccess>]
type internal LexerSymbolKind =
    | Ident
    | Operator
    | GenericTypeParameter
    | StaticallyResolvedTypeParameter
    | Other

type internal LexerSymbol =
    { Kind: LexerSymbolKind
      Line: int
      LeftColumn: int
      RightColumn: int
      Text: string 
      FileName: string }
    member x.Range: Range.range = 
        Range.mkRange x.FileName (Range.mkPos (x.Line + 1) x.LeftColumn) (Range.mkPos (x.Line + 1) x.RightColumn)

[<RequireQualifiedAccess>]
type internal SymbolLookupKind =
    | Fuzzy
    | ByRightColumn
    | ByLongIdent

type private DraftToken =
    { Kind: LexerSymbolKind
      Token: FSharpTokenInfo 
      RightColumn: int }
    static member inline Create kind token = 
        { Kind = kind; Token = token; RightColumn = token.LeftColumn + token.FullMatchedLength - 1 }

type internal SourceLineData(lineStart: int, lexStateAtStartOfLine: FSharpTokenizerLexState, lexStateAtEndOfLine: FSharpTokenizerLexState, 
                             hashCode: int, classifiedSpans: IReadOnlyList<ClassifiedSpan>, tokens: FSharpTokenInfo list) =
    member val LineStart = lineStart
    member val LexStateAtStartOfLine = lexStateAtStartOfLine
    member val LexStateAtEndOfLine = lexStateAtEndOfLine
    member val HashCode = hashCode
    member val ClassifiedSpans = classifiedSpans
    member val Tokens = tokens

    member data.IsValid(textLine: TextLine) =
        data.LineStart = textLine.Start && 
        let lineContents = textLine.Text.ToString(textLine.Span)
        data.HashCode = lineContents.GetHashCode() 

type internal SourceTextData(approxLines: int) =
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

    /// Go backwards to find the last cached scanned line that is valid.
    member x.GetLastValidCachedLine (startLine: int,  sourceLines: TextLineCollection) : int =
        let mutable i = startLine
        while i > 0 && (match x.[i] with Some data -> not (data.IsValid(sourceLines.[i])) | None -> true)  do
            i <- i - 1
        i

[<Export(typeof<Lexer>); System.Composition.Shared>]
type internal Lexer() =
    let dataCache = ConditionalWeakTable<DocumentId, SourceTextData>()

    let scanSourceLine(sourceTokenizer: FSharpSourceTokenizer, textLine: TextLine, lineContents: string, lexState: FSharpTokenizerLexState) : SourceLineData =
        let colorMap = Array.create textLine.Span.Length ClassificationTypeNames.Text
        let lineTokenizer = sourceTokenizer.CreateLineTokenizer(lineContents)
        let tokens = ResizeArray()
            
        let scanAndColorNextToken(lineTokenizer: FSharpLineTokenizer, lexState: Ref<FSharpTokenizerLexState>) : Option<FSharpTokenInfo> =
            let tokenInfoOption, nextLexState = lineTokenizer.ScanToken(lexState.Value)
            lexState.Value <- nextLexState
            if tokenInfoOption.IsSome then
                let classificationType = CommonHelpers.compilerTokenToRoslynToken(tokenInfoOption.Value.ColorClass)
                for i = tokenInfoOption.Value.LeftColumn to tokenInfoOption.Value.RightColumn do
                    Array.set colorMap i classificationType
                tokens.Add tokenInfoOption.Value
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

        SourceLineData(textLine.Start, lexState, previousLexState.Value, lineContents.GetHashCode(), classifiedSpans, List.ofSeq tokens)

    /// Returns symbol at a given position.
    let getSymbolFromTokens (fileName: string, tokens: FSharpTokenInfo list, linePos: LinePosition, lineStr: string, lookupKind: SymbolLookupKind) : LexerSymbol option =
        let isIdentifier t = t.CharClass = FSharpTokenCharKind.Identifier
        let isOperator t = t.ColorClass = FSharpTokenColorKind.Operator
    
        let inline (|GenericTypeParameterPrefix|StaticallyResolvedTypeParameterPrefix|Other|) (token: FSharpTokenInfo) =
            if token.Tag = FSharpTokenTag.QUOTE then GenericTypeParameterPrefix
            elif token.Tag = FSharpTokenTag.INFIX_AT_HAT_OP then
                 // The lexer return INFIX_AT_HAT_OP token for both "^" and "@" symbols.
                 // We have to check the char itself to distinguish one from another.
                 if token.FullMatchedLength = 1 && lineStr.[token.LeftColumn] = '^' then 
                    StaticallyResolvedTypeParameterPrefix
                 else Other
            else Other
       
        // Operators: Filter out overlapped operators (>>= operator is tokenized as three distinct tokens: GREATER, GREATER, EQUALS. 
        // Each of them has FullMatchedLength = 3. So, we take the first GREATER and skip the other two).
        //
        // Generic type parameters: we convert QUOTE + IDENT tokens into single IDENT token, altering its LeftColumn 
        // and FullMathedLength (for "'type" which is tokenized as (QUOTE, left=2) + (IDENT, left=3, length=4) 
        // we'll get (IDENT, left=2, length=5).
        //
        // Statically resolved type parameters: we convert INFIX_AT_HAT_OP + IDENT tokens into single IDENT token, altering its LeftColumn 
        // and FullMathedLength (for "^type" which is tokenized as (INFIX_AT_HAT_OP, left=2) + (IDENT, left=3, length=4) 
        // we'll get (IDENT, left=2, length=5).
        let tokens = 
            tokens
            |> List.fold (fun (acc, lastToken) (token: FSharpTokenInfo) ->
                match lastToken with
                | Some t when token.LeftColumn <= t.RightColumn -> acc, lastToken
                | _ ->
                    match token with
                    | GenericTypeParameterPrefix -> acc, Some (DraftToken.Create LexerSymbolKind.GenericTypeParameter token)
                    | StaticallyResolvedTypeParameterPrefix -> acc, Some (DraftToken.Create LexerSymbolKind.StaticallyResolvedTypeParameter token)
                    | Other ->
                        let draftToken =
                            match lastToken with
                            | Some { Kind = LexerSymbolKind.GenericTypeParameter | LexerSymbolKind.StaticallyResolvedTypeParameter as kind } when isIdentifier token ->
                                DraftToken.Create kind { token with LeftColumn = token.LeftColumn - 1
                                                                    FullMatchedLength = token.FullMatchedLength + 1 }
                            // ^ operator                                                
                            | Some { Kind = LexerSymbolKind.StaticallyResolvedTypeParameter } ->
                                DraftToken.Create LexerSymbolKind.Operator { token with LeftColumn = token.LeftColumn - 1
                                                                                        FullMatchedLength = 1 }
                            | _ -> 
                                let kind = 
                                    if isOperator token then LexerSymbolKind.Operator 
                                    elif isIdentifier token then LexerSymbolKind.Ident 
                                    else LexerSymbolKind.Other

                                DraftToken.Create kind token
                        draftToken :: acc, Some draftToken
                ) ([], None)
            |> fst
           
        // One or two tokens that in touch with the cursor (for "let x|(g) = ()" the tokens will be "x" and "(")
        let tokensUnderCursor = 
            match lookupKind with
            | SymbolLookupKind.Fuzzy ->
                tokens |> List.filter (fun x -> x.Token.LeftColumn <= linePos.Character && x.RightColumn + 1 >= linePos.Character)
            | SymbolLookupKind.ByRightColumn ->
                tokens |> List.filter (fun x -> x.RightColumn = linePos.Character)
            | SymbolLookupKind.ByLongIdent ->
                tokens |> List.filter (fun x -> x.Token.LeftColumn <= linePos.Character)
                
        //printfn "Filtered tokens: %+A" tokensUnderCursor
        match lookupKind with
        | SymbolLookupKind.ByLongIdent ->
            // Try to find start column of the long identifiers
            // Assume that tokens are ordered in an decreasing order of start columns
            let rec tryFindStartColumn tokens =
               match tokens with
               | { DraftToken.Kind = LexerSymbolKind.Ident; Token = t1 } :: {Kind = LexerSymbolKind.Operator; Token = t2 } :: remainingTokens ->
                    if t2.Tag = FSharpTokenTag.DOT then
                        tryFindStartColumn remainingTokens
                    else
                        Some t1.LeftColumn
               | { Kind = LexerSymbolKind.Ident; Token = t } :: _ ->
                   Some t.LeftColumn
               | _ :: _ | [] ->
                   None
            let decreasingTokens =
                match tokensUnderCursor |> List.sortBy (fun token -> - token.Token.LeftColumn) with
                // Skip the first dot if it is the start of the identifier
                | {Kind = LexerSymbolKind.Operator; Token = t} :: remainingTokens when t.Tag = FSharpTokenTag.DOT ->
                    remainingTokens
                | newTokens -> newTokens
            
            match decreasingTokens with
            | [] -> None
            | first :: _ ->
                tryFindStartColumn decreasingTokens
                |> Option.map (fun leftCol ->
                    { Kind = LexerSymbolKind.Ident
                      Line = linePos.Line
                      LeftColumn = leftCol
                      RightColumn = first.RightColumn + 1
                      Text = lineStr.[leftCol..first.RightColumn]
                      FileName = fileName })
        | SymbolLookupKind.Fuzzy 
        | SymbolLookupKind.ByRightColumn ->
            // Select IDENT token. If failed, select OPERATOR token.
            tokensUnderCursor
            |> List.tryFind (fun { DraftToken.Kind = k } -> 
                match k with 
                | LexerSymbolKind.Ident 
                | LexerSymbolKind.GenericTypeParameter 
                | LexerSymbolKind.StaticallyResolvedTypeParameter -> true 
                | _ -> false) 
            |> Option.orElseWith (fun _ -> tokensUnderCursor |> List.tryFind (fun { DraftToken.Kind = k } -> k = LexerSymbolKind.Operator))
            |> Option.map (fun token ->
                { Kind = token.Kind
                  Line = linePos.Line
                  LeftColumn = token.Token.LeftColumn
                  RightColumn = token.RightColumn + 1
                  Text = lineStr.Substring(token.Token.LeftColumn, token.Token.FullMatchedLength)
                  FileName = fileName })

    member __.GetSourceLineDatas(documentKey: DocumentId, sourceText: SourceText, startLine: int, endLine: int, fileName: string option, defines: string list, 
                                 cancellationToken: CancellationToken) : ResizeArray<SourceLineData> =
        let sourceTokenizer = FSharpSourceTokenizer(defines, fileName)
        let lines = sourceText.Lines
        // We keep incremental data per-document.  When text changes we correlate text line-by-line (by hash codes of lines)
        let sourceTextData = dataCache.GetValue(documentKey, fun key -> SourceTextData(lines.Count))
        let scanStartLine = sourceTextData.GetLastValidCachedLine(startLine, lines)
            
        // Rescan the lines if necessary and report the information
        let result = ResizeArray()
        let mutable lexState = if scanStartLine = 0 then 0L else sourceTextData.[scanStartLine].Value.LexStateAtEndOfLine

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
                result.Add(lineData)

            // If necessary, invalidate all subsequent lines after endLine
            if endLine < lines.Count - 1 then 
                match sourceTextData.[endLine+1] with 
                | Some data  -> 
                    if data.LexStateAtStartOfLine <> lexState then
                        sourceTextData.ClearFrom (endLine+1)
                | None -> ()

        result

    member this.GetColorizationData(documentKey: DocumentId, sourceText: SourceText, textSpan: TextSpan, fileName: string option, defines: string list, 
                                    cancellationToken: CancellationToken) : List<ClassifiedSpan> =
        try
            let lines = sourceText.Lines
            let startLine = lines.GetLineFromPosition(textSpan.Start).LineNumber
            let endLine = lines.GetLineFromPosition(textSpan.End).LineNumber
            
            // Rescan the lines if necessary and report the information
            let result = new List<ClassifiedSpan>()
            for lineData in this.GetSourceLineDatas(documentKey, sourceText, startLine, endLine, fileName, defines, cancellationToken) do
                result.AddRange(lineData.ClassifiedSpans |> Seq.filter(fun token ->
                    textSpan.Contains(token.TextSpan.Start) ||
                    textSpan.Contains(token.TextSpan.End - 1) ||
                    (token.TextSpan.Start <= textSpan.Start && textSpan.End <= token.TextSpan.End)))
            result
        with 
        | :? System.OperationCanceledException -> reraise()
        |  ex -> 
            Assert.Exception(ex)
            List<ClassifiedSpan>()

    member this.GetSymbolAtPosition(documentKey: DocumentId, sourceText: SourceText, position: int, fileName: string, defines: string list, lookupKind: SymbolLookupKind) : LexerSymbol option =
        try
            let textLine = sourceText.Lines.GetLineFromPosition(position)
            let textLinePos = sourceText.Lines.GetLinePosition(position)
            let sourceLineDatas = this.GetSourceLineDatas(documentKey, sourceText, textLinePos.Line, textLinePos.Line, Some fileName, defines, CancellationToken.None)
            assert(sourceLineDatas.Count = 1)
            let lineContents = textLine.Text.ToString(textLine.Span)
            getSymbolFromTokens(fileName, sourceLineDatas.[0].Tokens, textLinePos, lineContents, lookupKind)
        with 
        | :? System.OperationCanceledException -> reraise()
        |  ex -> 
            Assert.Exception(ex)
            None