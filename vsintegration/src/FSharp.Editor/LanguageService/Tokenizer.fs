namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.SourceCodeServices


[<RequireQualifiedAccess>]
type internal LexerSymbolKind =
    | Ident
    | Operator
    | Punctuation
    | GenericTypeParameter
    | StaticallyResolvedTypeParameter
    | ActivePattern
    | Other

type internal LexerSymbol =
    { Kind: LexerSymbolKind
      /// Last part of `LongIdent`
      Ident: Ident
      /// All parts of `LongIdent`
      FullIsland: string list }
    member x.Range: Range.range = x.Ident.idRange

[<RequireQualifiedAccess>]
type internal SymbolLookupKind =
    /// Position must lay inside symbol range.
    | Precise
    /// Position may lay one column outside of symbol range to the right.
    | Greedy

[<RequireQualifiedAccess>]
module internal Tokenizer =

    let (|Public|Internal|Protected|Private|) (a: FSharpAccessibility option) =
        match a with
        | None -> Public
        | Some a ->
            if a.IsPublic then Public
            elif a.IsInternal then Internal
            elif a.IsPrivate then Private
            else Protected

    let FSharpGlyphToRoslynGlyph (glyph: FSharpGlyph, accessibility: FSharpAccessibility option) =
        match glyph with
        | FSharpGlyph.Class
        | FSharpGlyph.Exception
        | FSharpGlyph.Typedef
        | FSharpGlyph.Type ->
            match accessibility with
            | Public -> Glyph.ClassPublic
            | Internal -> Glyph.ClassInternal
            | Protected -> Glyph.ClassProtected
            | Private -> Glyph.ClassPrivate
        | FSharpGlyph.Constant -> 
            match accessibility with
            | Public -> Glyph.ConstantPublic
            | Internal -> Glyph.ConstantInternal
            | Protected -> Glyph.ConstantProtected
            | Private -> Glyph.ConstantPrivate
        | FSharpGlyph.Delegate ->
            match accessibility with
            | Public -> Glyph.DelegatePublic
            | Internal -> Glyph.DelegateInternal
            | Protected -> Glyph.DelegateProtected
            | Private -> Glyph.DelegatePrivate
        | FSharpGlyph.Enum 
        | FSharpGlyph.Union ->
            match accessibility with
            | Public -> Glyph.EnumPublic
            | Internal -> Glyph.EnumInternal
            | Protected -> Glyph.EnumProtected
            | Private -> Glyph.EnumPrivate
        | FSharpGlyph.EnumMember -> Glyph.EnumMember
        | FSharpGlyph.Event ->
            match accessibility with
            | Public -> Glyph.EventPublic
            | Internal -> Glyph.EventInternal
            | Protected -> Glyph.EventProtected
            | Private -> Glyph.EventPrivate
        | FSharpGlyph.Field ->
            match accessibility with
            | Public -> Glyph.FieldPublic
            | Internal -> Glyph.FieldInternal
            | Protected -> Glyph.FieldProtected
            | Private -> Glyph.FieldPrivate
        | FSharpGlyph.Interface ->
            match accessibility with
            | Public -> Glyph.InterfacePublic
            | Internal -> Glyph.InterfaceInternal
            | Protected -> Glyph.InterfaceProtected
            | Private -> Glyph.InterfacePrivate
        | FSharpGlyph.Method
        | FSharpGlyph.OverridenMethod ->
            match accessibility with
            | Public -> Glyph.MethodPublic
            | Internal -> Glyph.MethodInternal
            | Protected -> Glyph.MethodProtected
            | Private -> Glyph.MethodPrivate
        | FSharpGlyph.ExtensionMethod ->
            match accessibility with
            | Public -> Glyph.ExtensionMethodPublic
            | Internal -> Glyph.ExtensionMethodInternal
            | Protected -> Glyph.ExtensionMethodProtected
            | Private -> Glyph.ExtensionMethodPrivate
        | FSharpGlyph.Module ->
            match accessibility with
            | Public -> Glyph.ModulePublic
            | Internal -> Glyph.ModuleInternal
            | Protected -> Glyph.ModuleProtected
            | Private -> Glyph.ModulePrivate
        | FSharpGlyph.NameSpace -> Glyph.Namespace
        | FSharpGlyph.Property -> 
            match accessibility with
            | Public -> Glyph.PropertyPublic
            | Internal -> Glyph.PropertyInternal
            | Protected -> Glyph.PropertyProtected
            | Private -> Glyph.PropertyPrivate
        | FSharpGlyph.Struct ->
            match accessibility with
            | Public -> Glyph.StructurePublic
            | Internal -> Glyph.StructureInternal
            | Protected -> Glyph.StructureProtected
            | Private -> Glyph.StructurePrivate
        | FSharpGlyph.Variable -> Glyph.Local
        | FSharpGlyph.Error -> Glyph.Error

    let GetGlyphForSymbol (symbol: FSharpSymbol, kind: LexerSymbolKind) =
        match kind with
        | LexerSymbolKind.Operator -> Glyph.Operator
        | _ ->
            match symbol with
            | :? FSharpUnionCase as x ->
                match Some x.Accessibility with
                | Public -> Glyph.EnumPublic
                | Internal -> Glyph.EnumInternal
                | Protected -> Glyph.EnumProtected
                | Private -> Glyph.EnumPrivate
            | :? FSharpActivePatternCase -> Glyph.EnumPublic
            | :? FSharpField as x ->
            if x.IsLiteral then
                match Some x.Accessibility with
                | Public -> Glyph.ConstantPublic
                | Internal -> Glyph.ConstantInternal
                | Protected -> Glyph.ConstantProtected
                | Private -> Glyph.ConstantPrivate
            else
                match Some x.Accessibility with
                | Public -> Glyph.FieldPublic
                | Internal -> Glyph.FieldInternal
                | Protected -> Glyph.FieldProtected
                | Private -> Glyph.FieldPrivate
            | :? FSharpParameter -> Glyph.Parameter
            | :? FSharpMemberOrFunctionOrValue as x ->
                if x.LiteralValue.IsSome then
                    match Some x.Accessibility with
                    | Public -> Glyph.ConstantPublic
                    | Internal -> Glyph.ConstantInternal
                    | Protected -> Glyph.ConstantProtected
                    | Private -> Glyph.ConstantPrivate
                elif x.IsExtensionMember then
                    match Some x.Accessibility with
                    | Public -> Glyph.ExtensionMethodPublic
                    | Internal -> Glyph.ExtensionMethodInternal
                    | Protected -> Glyph.ExtensionMethodProtected
                    | Private -> Glyph.ExtensionMethodPrivate
                elif x.IsProperty || x.IsPropertyGetterMethod || x.IsPropertySetterMethod then
                    match Some x.Accessibility with
                    | Public -> Glyph.PropertyPublic
                    | Internal -> Glyph.PropertyInternal
                    | Protected -> Glyph.PropertyProtected
                    | Private -> Glyph.PropertyPrivate
                elif x.IsEvent then
                    match Some x.Accessibility with
                    | Public -> Glyph.EventPublic
                    | Internal -> Glyph.EventInternal
                    | Protected -> Glyph.EventProtected
                    | Private -> Glyph.EventPrivate
                else
                    match Some x.Accessibility with
                    | Public -> Glyph.MethodPublic
                    | Internal -> Glyph.MethodInternal
                    | Protected -> Glyph.MethodProtected
                    | Private -> Glyph.MethodPrivate
            | :? FSharpEntity as x ->
                if x.IsValueType then
                    match Some x.Accessibility with
                    | Public -> Glyph.StructurePublic
                    | Internal -> Glyph.StructureInternal
                    | Protected -> Glyph.StructureProtected
                    | Private -> Glyph.StructurePrivate
                elif x.IsFSharpModule then
                    match Some x.Accessibility with
                    | Public -> Glyph.ModulePublic
                    | Internal -> Glyph.ModuleInternal
                    | Protected -> Glyph.ModuleProtected
                    | Private -> Glyph.ModulePrivate
                elif x.IsEnum || x.IsFSharpUnion then
                    match Some x.Accessibility with
                    | Public -> Glyph.EnumPublic
                    | Internal -> Glyph.EnumInternal
                    | Protected -> Glyph.EnumProtected
                    | Private -> Glyph.EnumPrivate
                elif x.IsInterface then
                    match Some x.Accessibility with
                    | Public -> Glyph.InterfacePublic
                    | Internal -> Glyph.InterfaceInternal
                    | Protected -> Glyph.InterfaceProtected
                    | Private -> Glyph.InterfacePrivate
                elif x.IsDelegate then
                    match Some x.Accessibility with
                    | Public -> Glyph.DelegatePublic
                    | Internal -> Glyph.DelegateInternal
                    | Protected -> Glyph.DelegateProtected
                    | Private -> Glyph.DelegatePrivate
                elif x.IsNamespace then
                    Glyph.Namespace
                else
                    match Some x.Accessibility with
                    | Public -> Glyph.ClassPublic
                    | Internal -> Glyph.ClassInternal
                    | Protected -> Glyph.ClassProtected
                    | Private -> Glyph.ClassPrivate
            | _ -> Glyph.None


    type private SourceLineData(lineStart: int, lexStateAtStartOfLine: FSharpTokenizerLexState, lexStateAtEndOfLine: FSharpTokenizerLexState, 
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
        | FSharpTokenColorKind.Punctuation -> ClassificationTypeNames.Punctuation
        | FSharpTokenColorKind.Default | _ -> ClassificationTypeNames.Text

    let private scanSourceLine(sourceTokenizer: FSharpSourceTokenizer, textLine: TextLine, lineContents: string, lexState: FSharpTokenizerLexState) : SourceLineData =
        let colorMap = Array.create textLine.Span.Length ClassificationTypeNames.Text
        let lineTokenizer = sourceTokenizer.CreateLineTokenizer(lineContents)
        let tokens = ResizeArray()
        let mutable tokenInfoOption = None
        let previousLexState = ref lexState
            
        let processToken() =
            let classificationType = compilerTokenToRoslynToken(tokenInfoOption.Value.ColorClass)
            for i = tokenInfoOption.Value.LeftColumn to tokenInfoOption.Value.RightColumn do
                Array.set colorMap i classificationType
            tokens.Add tokenInfoOption.Value

        let scanAndColorNextToken() =
            let info, nextLexState = lineTokenizer.ScanToken(!previousLexState)
            tokenInfoOption <- info
            previousLexState := nextLexState
            match info with
            | Some info when info.Tag = FSharpTokenTag.INT32_DOT_DOT ->
                    tokenInfoOption <- 
                        Some { LeftColumn = info.LeftColumn
                               RightColumn = info.RightColumn - 2
                               ColorClass = FSharpTokenColorKind.Number
                               CharClass = FSharpTokenCharKind.Literal
                               FSharpTokenTriggerClass = info.FSharpTokenTriggerClass
                               Tag = info.Tag
                               TokenName = "INT32"
                               FullMatchedLength = info.FullMatchedLength - 2 }
                    processToken()

                    tokenInfoOption <- 
                        Some { LeftColumn = info.RightColumn - 1
                               RightColumn = info.RightColumn
                               ColorClass = FSharpTokenColorKind.Operator
                               CharClass = FSharpTokenCharKind.Operator
                               FSharpTokenTriggerClass = info.FSharpTokenTriggerClass
                               Tag = FSharpTokenTag.DOT_DOT
                               TokenName = "DOT_DOT"
                               FullMatchedLength = 2 }
                    processToken()
                    
            | Some _ -> processToken()
            | _ -> ()

        scanAndColorNextToken()
        while tokenInfoOption.IsSome do scanAndColorNextToken()

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

    let getColorizationData(documentKey: DocumentId, sourceText: SourceText, textSpan: TextSpan, fileName: string option, defines: string list, 
                            cancellationToken: CancellationToken) : List<ClassifiedSpan> =
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
                    while i > 0 && (match sourceTextData.[i] with Some data -> not (data.IsValid(lines.[i])) | None -> true)  do
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

    type private DraftToken = {   
        Kind: LexerSymbolKind
        Token: FSharpTokenInfo 
        RightColumn: int 
    } with
        static member inline Create kind token = 
            { Kind = kind; Token = token; RightColumn = token.LeftColumn + token.FullMatchedLength - 1 }
    
    /// Returns symbol at a given position.
    let private getSymbolFromTokens 
        (
            fileName: string, 
            tokens: FSharpTokenInfo list, 
            linePos: LinePosition, 
            lineStr: string, 
            lookupKind: SymbolLookupKind,
            wholeActivePatterns: bool
        ) 
        : LexerSymbol option =
        
        let isIdentifier t = t.CharClass = FSharpTokenCharKind.Identifier
        let isOperator t = t.ColorClass = FSharpTokenColorKind.Operator
        let isPunctuation t = t.ColorClass = FSharpTokenColorKind.Punctuation
    
        let (|GenericTypeParameterPrefix|StaticallyResolvedTypeParameterPrefix|ActivePattern|Other|) (token: FSharpTokenInfo) =
            if token.Tag = FSharpTokenTag.QUOTE then GenericTypeParameterPrefix
            elif token.Tag = FSharpTokenTag.INFIX_AT_HAT_OP then
                    // The lexer return INFIX_AT_HAT_OP token for both "^" and "@" symbols.
                    // We have to check the char itself to distinguish one from another.
                    if token.FullMatchedLength = 1 && token.LeftColumn < lineStr.Length && lineStr.[token.LeftColumn] = '^' then 
                        StaticallyResolvedTypeParameterPrefix
                    else Other
            elif token.Tag = FSharpTokenTag.LPAREN then
                if token.FullMatchedLength = 1 && token.LeftColumn+1 < lineStr.Length && lineStr.[token.LeftColumn+1] = '|' then
                    ActivePattern
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
            let tokensCount = tokens.Length
            tokens
            |> List.foldi (fun (acc, lastToken) index (token: FSharpTokenInfo) ->
                match lastToken with
                | Some t when token.LeftColumn <= t.RightColumn -> acc, lastToken
                | Some ({ Kind = LexerSymbolKind.ActivePattern } as lastToken) when
                    wholeActivePatterns &&
                    (token.Tag = FSharpTokenTag.BAR || token.Tag = FSharpTokenTag.IDENT || token.Tag = FSharpTokenTag.UNDERSCORE) ->
                    
                    let mergedToken =
                        {lastToken.Token with Tag = FSharpTokenTag.IDENT
                                                    RightColumn = token.RightColumn
                                                    FullMatchedLength = lastToken.Token.FullMatchedLength + token.FullMatchedLength }

                    acc, Some { lastToken with Token = mergedToken; RightColumn = lastToken.RightColumn + token.FullMatchedLength }
                | _ ->
                    let isLastToken = index = tokensCount - 1
                    match token with
                    | GenericTypeParameterPrefix when not isLastToken -> acc, Some (DraftToken.Create LexerSymbolKind.GenericTypeParameter token)
                    | StaticallyResolvedTypeParameterPrefix when not isLastToken -> acc, Some (DraftToken.Create LexerSymbolKind.StaticallyResolvedTypeParameter token)
                    | ActivePattern when wholeActivePatterns -> acc, Some (DraftToken.Create LexerSymbolKind.ActivePattern token)
                    | _ ->
                        let draftToken =
                            match lastToken with
                            | Some { Kind = LexerSymbolKind.GenericTypeParameter | LexerSymbolKind.StaticallyResolvedTypeParameter as kind } when isIdentifier token ->
                                DraftToken.Create kind { token with LeftColumn = token.LeftColumn - 1
                                                                    FullMatchedLength = token.FullMatchedLength + 1 }
                            // ^ operator                                                
                            | Some { Kind = LexerSymbolKind.StaticallyResolvedTypeParameter } ->
                                DraftToken.Create LexerSymbolKind.Operator { token with LeftColumn = token.LeftColumn - 1
                                                                                        FullMatchedLength = 1 }
                            | Some ( { Kind = LexerSymbolKind.ActivePattern } as ap) when wholeActivePatterns && token.Tag = FSharpTokenTag.RPAREN ->
                                DraftToken.Create LexerSymbolKind.Ident ap.Token
                            | _ -> 
                                let kind = 
                                    if isOperator token then LexerSymbolKind.Operator 
                                    elif isIdentifier token then LexerSymbolKind.Ident 
                                    elif isPunctuation token then LexerSymbolKind.Punctuation
                                    else LexerSymbolKind.Other

                                DraftToken.Create kind token
                        draftToken :: acc, Some draftToken
                ) ([], None)
            |> fst
           
        // One or two tokens that in touch with the cursor (for "let x|(g) = ()" the tokens will be "x" and "(")
        let tokensUnderCursor = 
            let rightColumnCorrection = 
                match lookupKind with 
                | SymbolLookupKind.Precise -> 0
                | SymbolLookupKind.Greedy -> 1
            
            tokens |> List.filter (fun x -> x.Token.LeftColumn <= linePos.Character && (x.RightColumn + rightColumnCorrection) >= linePos.Character)
                
        // Select IDENT token. If failed, select OPERATOR token.
        tokensUnderCursor
        |> List.tryFind (fun { DraftToken.Kind = k } -> 
            match k with 
            | LexerSymbolKind.Ident
            | LexerSymbolKind.ActivePattern
            | LexerSymbolKind.GenericTypeParameter 
            | LexerSymbolKind.StaticallyResolvedTypeParameter -> true 
            | _ -> false) 
        |> Option.orElseWith (fun _ -> tokensUnderCursor |> List.tryFind (fun { DraftToken.Kind = k } -> k = LexerSymbolKind.Operator))
        |> Option.map (fun token ->
            let plid, _ = QuickParse.GetPartialLongNameEx(lineStr, token.RightColumn)
            let identStr = lineStr.Substring(token.Token.LeftColumn, token.Token.FullMatchedLength)
            {   Kind = token.Kind
                Ident = 
                    Ident(identStr, 
                        Range.mkRange 
                            fileName 
                            (Range.mkPos (linePos.Line + 1) token.Token.LeftColumn)
                            (Range.mkPos (linePos.Line + 1) (token.RightColumn + 1))) 
                FullIsland = plid @ [identStr] })

    let private getCachedSourceLineData(documentKey: DocumentId, sourceText: SourceText, position: int, fileName: string, defines: string list) = 
        let textLine = sourceText.Lines.GetLineFromPosition(position)
        let textLinePos = sourceText.Lines.GetLinePosition(position)
        let lineNumber = textLinePos.Line + 1 // FCS line number
        let sourceTokenizer = FSharpSourceTokenizer(defines, Some fileName)
        let lines = sourceText.Lines
        // We keep incremental data per-document. When text changes we correlate text line-by-line (by hash codes of lines)
        let sourceTextData = dataCache.GetValue(documentKey, fun key -> SourceTextData(lines.Count))
        // Go backwards to find the last cached scanned line that is valid
        let scanStartLine = 
            let mutable i = min (lines.Count - 1) lineNumber
            while i > 0 &&
                (match sourceTextData.[i] with 
                | Some data -> not (data.IsValid(lines.[i])) 
                | None -> true
                ) do  
                i <- i - 1
            i
        let lexState = if scanStartLine = 0 then 0L else sourceTextData.[scanStartLine - 1].Value.LexStateAtEndOfLine
        let lineContents = textLine.Text.ToString(textLine.Span)
        
        // We can reuse the old data when 
        //   1. the line starts at the same overall position
        //   2. the hash codes match
        //   3. the start-of-line lex states are the same
        match sourceTextData.[lineNumber] with 
        | Some data when data.IsValid(textLine) && data.LexStateAtStartOfLine = lexState -> 
            data, textLinePos, lineContents
        | _ -> 
            // Otherwise, we recompute
            let newData = scanSourceLine(sourceTokenizer, textLine, lineContents, lexState)
            sourceTextData.[lineNumber] <- Some newData
            newData, textLinePos, lineContents
           
    let tokenizeLine (documentKey, sourceText, position, fileName, defines) =
        try
            let lineData, _, _ = getCachedSourceLineData(documentKey, sourceText, position, fileName, defines)
            lineData.Tokens   
        with 
        |  ex -> 
            Assert.Exception(ex)
            []

    let getSymbolAtPosition
        (
            documentKey: DocumentId, 
            sourceText: SourceText, 
            position: int, 
            fileName: string, 
            defines: string list, 
            lookupKind: SymbolLookupKind,
            wholeActivePatterns: bool
        ) 
        : LexerSymbol option =
        
        try
            let lineData, textLinePos, lineContents = getCachedSourceLineData(documentKey, sourceText, position, fileName, defines)
            getSymbolFromTokens(fileName, lineData.Tokens, textLinePos, lineContents, lookupKind, wholeActivePatterns)
        with 
        | :? System.OperationCanceledException -> reraise()
        |  ex -> 
            Assert.Exception(ex)
            None

    /// Fix invalid span if it appears to have redundant suffix and prefix.
    let fixupSpan (sourceText: SourceText, span: TextSpan) : TextSpan =
        let text = sourceText.GetSubText(span).ToString()
        match text.LastIndexOf '.' with
        | -1 | 0 -> span
        | index -> TextSpan(span.Start + index + 1, text.Length - index - 1)

    let isValidNameForSymbol (lexerSymbolKind: LexerSymbolKind, symbol: FSharpSymbol, name: string) : bool =
        let doubleBackTickDelimiter = "``"
        
        let isDoubleBacktickIdent (s: string) =
            let doubledDelimiter = 2 * doubleBackTickDelimiter.Length
            if s.StartsWith(doubleBackTickDelimiter) && s.EndsWith(doubleBackTickDelimiter) && s.Length > doubledDelimiter then
                let inner = s.Substring(doubleBackTickDelimiter.Length, s.Length - doubledDelimiter)
                not (inner.Contains(doubleBackTickDelimiter))
            else false
        
        let isIdentifier (ident: string) =
            if isDoubleBacktickIdent ident then
                true
            else
                ident 
                |> Seq.mapi (fun i c -> i, c)
                |> Seq.forall (fun (i, c) -> 
                        if i = 0 then PrettyNaming.IsIdentifierFirstCharacter c 
                        else PrettyNaming.IsIdentifierPartCharacter c) 
        
        let isFixableIdentifier (s: string) = 
            not (String.IsNullOrEmpty s) && Lexhelp.Keywords.NormalizeIdentifierBackticks s |> isIdentifier
        
        let forbiddenChars = [| '.'; '+'; '$'; '&'; '['; ']'; '/'; '\\'; '*'; '\'' |]
        
        let isTypeNameIdent (s: string) =
            not (String.IsNullOrEmpty s) && s.IndexOfAny forbiddenChars = -1 && isFixableIdentifier s 
        
        let isUnionCaseIdent (s: string) =
            isTypeNameIdent s && Char.IsUpper(s.Replace(doubleBackTickDelimiter, "").[0])
        
        let isTypeParameter (prefix: char) (s: string) =
            s.Length >= 2 && s.[0] = prefix && isIdentifier s.[1..]
        
        let isGenericTypeParameter = isTypeParameter '''
        let isStaticallyResolvedTypeParameter = isTypeParameter '^'
        
        match lexerSymbolKind, symbol with
        | _, :? FSharpUnionCase -> isUnionCaseIdent name
        | _, :? FSharpActivePatternCase -> 
            // Different from union cases, active patterns don't accept double-backtick identifiers
            isFixableIdentifier name && not (String.IsNullOrEmpty name) && Char.IsUpper(name.[0]) 
        | LexerSymbolKind.Operator, _ -> PrettyNaming.IsOperatorName name
        | LexerSymbolKind.Punctuation, _ -> PrettyNaming.IsPunctuation name
        | LexerSymbolKind.GenericTypeParameter, _ -> isGenericTypeParameter name
        | LexerSymbolKind.StaticallyResolvedTypeParameter, _ -> isStaticallyResolvedTypeParameter name
        | (LexerSymbolKind.Ident | LexerSymbolKind.ActivePattern | LexerSymbolKind.Other), _ ->
            match symbol with
            | :? FSharpEntity as e when e.IsClass || e.IsFSharpRecord || e.IsFSharpUnion || e.IsValueType || e.IsFSharpModule || e.IsInterface -> isTypeNameIdent name
            | _ -> isFixableIdentifier name
