namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Generic
open System.Collections.Concurrent
open System.Diagnostics
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices
open System.Runtime.Caching

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Text

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Symbols
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.Tokenization

open Microsoft.VisualStudio.Core.Imaging
open Microsoft.VisualStudio.Imaging

open Microsoft.CodeAnalysis.ExternalAccess.FSharp

type private FSharpGlyph = FSharp.Compiler.EditorServices.FSharpGlyph
type private Glyph = Microsoft.CodeAnalysis.ExternalAccess.FSharp.FSharpGlyph

[<RequireQualifiedAccess>]
type internal LexerSymbolKind = 
    | Ident = 0
    | Operator = 1
    | Punctuation = 2
    | GenericTypeParameter = 3
    | StaticallyResolvedTypeParameter = 4
    | ActivePattern = 5
    | String = 6
    | Other = 7
    | Keyword = 8

type internal LexerSymbol =
    { Kind: LexerSymbolKind
      /// Last part of `LongIdent`
      Ident: Ident
      /// All parts of `LongIdent`
      FullIsland: string list }
    member x.Range: Range = x.Ident.idRange

[<RequireQualifiedAccess>]
type internal SymbolLookupKind =
    /// Position must lay inside symbol range.
    | Precise
    /// Position may lay one column outside of symbol range to the right.
    | Greedy

[<RequireQualifiedAccess>]
module internal Tokenizer =


    let (|Public|Internal|Protected|Private|) (a: FSharpAccessibility) =
        if a.IsPublic then Public
        elif a.IsInternal then Internal
        elif a.IsPrivate then Private
        else Protected

    let FSharpGlyphToRoslynGlyph (glyph: FSharpGlyph, accessibility: FSharpAccessibility) =
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
        | FSharpGlyph.EnumMember -> Glyph.EnumMemberPublic
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
        | FSharpGlyph.TypeParameter -> Glyph.TypeParameter

    let GetImageIdForSymbol(symbolOpt:FSharpSymbol option, kind:LexerSymbolKind) =
        let imageId =
            match kind with
            | LexerSymbolKind.Operator -> KnownImageIds.Operator
            | _ ->
                match symbolOpt with
                | None -> KnownImageIds.Package
                | Some symbol ->
                match symbol with
                | :? FSharpUnionCase as x ->
                    match x.Accessibility with
                    | Public -> KnownImageIds.EnumerationPublic
                    | Internal -> KnownImageIds.EnumerationInternal
                    | Protected -> KnownImageIds.EnumerationProtected
                    | Private -> KnownImageIds.EnumerationPrivate
                | :? FSharpActivePatternCase -> KnownImageIds.EnumerationPublic
                | :? FSharpField as x ->
                if x.IsLiteral then
                    match x.Accessibility with
                    | Public -> KnownImageIds.ConstantPublic
                    | Internal -> KnownImageIds.ConstantInternal
                    | Protected -> KnownImageIds.ConstantProtected
                    | Private -> KnownImageIds.ConstantPrivate
                else
                    match x.Accessibility with
                    | Public -> KnownImageIds.FieldPublic
                    | Internal -> KnownImageIds.FieldInternal
                    | Protected -> KnownImageIds.FieldProtected
                    | Private -> KnownImageIds.FieldPrivate
                | :? FSharpParameter -> KnownImageIds.Parameter
                | :? FSharpMemberOrFunctionOrValue as x ->
                    if x.LiteralValue.IsSome then
                        match x.Accessibility with
                        | Public -> KnownImageIds.ConstantPublic
                        | Internal -> KnownImageIds.ConstantInternal
                        | Protected -> KnownImageIds.ConstantProtected
                        | Private -> KnownImageIds.ConstantPrivate
                    elif x.IsExtensionMember then KnownImageIds.ExtensionMethod
                    elif x.IsProperty || x.IsPropertyGetterMethod || x.IsPropertySetterMethod then
                        match x.Accessibility with
                        | Public -> KnownImageIds.PropertyPublic
                        | Internal -> KnownImageIds.PropertyInternal
                        | Protected -> KnownImageIds.PropertyProtected
                        | Private -> KnownImageIds.PropertyPrivate
                    elif x.IsEvent then
                        match x.Accessibility with
                        | Public -> KnownImageIds.EventPublic
                        | Internal -> KnownImageIds.EventInternal
                        | Protected -> KnownImageIds.EventProtected
                        | Private -> KnownImageIds.EventPrivate
                    else
                        match x.Accessibility with
                        | Public -> KnownImageIds.MethodPublic
                        | Internal -> KnownImageIds.MethodInternal
                        | Protected -> KnownImageIds.MethodProtected
                        | Private -> KnownImageIds.MethodPrivate
                | :? FSharpEntity as x ->
                    if x.IsValueType then
                        match x.Accessibility with
                        | Public -> KnownImageIds.StructurePublic
                        | Internal -> KnownImageIds.StructureInternal
                        | Protected -> KnownImageIds.StructureProtected
                        | Private -> KnownImageIds.StructurePrivate
                    elif x.IsFSharpModule then
                        match x.Accessibility with
                        | Public -> KnownImageIds.ModulePublic
                        | Internal -> KnownImageIds.ModuleInternal
                        | Protected -> KnownImageIds.ModuleProtected
                        | Private -> KnownImageIds.ModulePrivate
                    elif x.IsEnum || x.IsFSharpUnion then
                        match x.Accessibility with
                        | Public -> KnownImageIds.EnumerationPublic
                        | Internal -> KnownImageIds.EnumerationInternal
                        | Protected -> KnownImageIds.EnumerationProtected
                        | Private -> KnownImageIds.EnumerationPrivate
                    elif x.IsInterface then
                        match x.Accessibility with
                        | Public -> KnownImageIds.InterfacePublic
                        | Internal -> KnownImageIds.InterfaceInternal
                        | Protected -> KnownImageIds.InterfaceProtected
                        | Private -> KnownImageIds.InterfacePrivate
                    elif x.IsDelegate then
                        match x.Accessibility with
                        | Public -> KnownImageIds.DelegatePublic
                        | Internal -> KnownImageIds.DelegateInternal
                        | Protected -> KnownImageIds.DelegateProtected
                        | Private -> KnownImageIds.DelegatePrivate
                    elif x.IsNamespace then
                        KnownImageIds.Namespace
                    else
                        match x.Accessibility with
                        | Public -> KnownImageIds.ClassPublic
                        | Internal -> KnownImageIds.ClassInternal
                        | Protected -> KnownImageIds.ClassProtected
                        | Private -> KnownImageIds.ClassPrivate
                | :? FSharpGenericParameter -> KnownImageIds.Type
                | _ -> KnownImageIds.None
        if imageId = KnownImageIds.None then
            None
        else
            Some(ImageId(KnownImageIds.ImageCatalogGuid, imageId))

    let GetGlyphForSymbol (symbol: FSharpSymbol, kind: LexerSymbolKind) =
        match kind with
        | LexerSymbolKind.Operator -> Glyph.Operator
        | _ ->
            match symbol with
            | :? FSharpUnionCase as x ->
                match x.Accessibility with
                | Public -> Glyph.EnumPublic
                | Internal -> Glyph.EnumInternal
                | Protected -> Glyph.EnumProtected
                | Private -> Glyph.EnumPrivate
            | :? FSharpActivePatternCase -> Glyph.EnumPublic
            | :? FSharpField as x ->
            if x.IsLiteral then
                match x.Accessibility with
                | Public -> Glyph.ConstantPublic
                | Internal -> Glyph.ConstantInternal
                | Protected -> Glyph.ConstantProtected
                | Private -> Glyph.ConstantPrivate
            else
                match x.Accessibility with
                | Public -> Glyph.FieldPublic
                | Internal -> Glyph.FieldInternal
                | Protected -> Glyph.FieldProtected
                | Private -> Glyph.FieldPrivate
            | :? FSharpParameter -> Glyph.Parameter
            | :? FSharpMemberOrFunctionOrValue as x ->
                if x.LiteralValue.IsSome then
                    match x.Accessibility with
                    | Public -> Glyph.ConstantPublic
                    | Internal -> Glyph.ConstantInternal
                    | Protected -> Glyph.ConstantProtected
                    | Private -> Glyph.ConstantPrivate
                elif x.IsExtensionMember then
                    match x.Accessibility with
                    | Public -> Glyph.ExtensionMethodPublic
                    | Internal -> Glyph.ExtensionMethodInternal
                    | Protected -> Glyph.ExtensionMethodProtected
                    | Private -> Glyph.ExtensionMethodPrivate
                elif x.IsProperty || x.IsPropertyGetterMethod || x.IsPropertySetterMethod then
                    match x.Accessibility with
                    | Public -> Glyph.PropertyPublic
                    | Internal -> Glyph.PropertyInternal
                    | Protected -> Glyph.PropertyProtected
                    | Private -> Glyph.PropertyPrivate
                elif x.IsEvent then
                    match x.Accessibility with
                    | Public -> Glyph.EventPublic
                    | Internal -> Glyph.EventInternal
                    | Protected -> Glyph.EventProtected
                    | Private -> Glyph.EventPrivate
                else
                    match x.Accessibility with
                    | Public -> Glyph.MethodPublic
                    | Internal -> Glyph.MethodInternal
                    | Protected -> Glyph.MethodProtected
                    | Private -> Glyph.MethodPrivate
            | :? FSharpEntity as x ->
                if x.IsValueType then
                    match x.Accessibility with
                    | Public -> Glyph.StructurePublic
                    | Internal -> Glyph.StructureInternal
                    | Protected -> Glyph.StructureProtected
                    | Private -> Glyph.StructurePrivate
                elif x.IsFSharpModule then
                    match x.Accessibility with
                    | Public -> Glyph.ModulePublic
                    | Internal -> Glyph.ModuleInternal
                    | Protected -> Glyph.ModuleProtected
                    | Private -> Glyph.ModulePrivate
                elif x.IsEnum || x.IsFSharpUnion then
                    match x.Accessibility with
                    | Public -> Glyph.EnumPublic
                    | Internal -> Glyph.EnumInternal
                    | Protected -> Glyph.EnumProtected
                    | Private -> Glyph.EnumPrivate
                elif x.IsInterface then
                    match x.Accessibility with
                    | Public -> Glyph.InterfacePublic
                    | Internal -> Glyph.InterfaceInternal
                    | Protected -> Glyph.InterfaceProtected
                    | Private -> Glyph.InterfacePrivate
                elif x.IsDelegate then
                    match x.Accessibility with
                    | Public -> Glyph.DelegatePublic
                    | Internal -> Glyph.DelegateInternal
                    | Protected -> Glyph.DelegateProtected
                    | Private -> Glyph.DelegatePrivate
                elif x.IsNamespace then
                    Glyph.Namespace
                else
                    match x.Accessibility with
                    | Public -> Glyph.ClassPublic
                    | Internal -> Glyph.ClassInternal
                    | Protected -> Glyph.ClassProtected
                    | Private -> Glyph.ClassPrivate
            | :? FSharpGenericParameter -> Glyph.TypeParameter
            | _ -> Glyph.None


    type FSharpTokenInfo with
        member token.IsIdentifier = (token.CharClass = FSharpTokenCharKind.Identifier)
        member token.IsOperator = (token.ColorClass = FSharpTokenColorKind.Operator)
        member token.IsPunctuation = (token.ColorClass = FSharpTokenColorKind.Punctuation)
        member token.IsString = (token.ColorClass = FSharpTokenColorKind.String)
    
    /// This is the information we save for each token in a line for each active document.
    /// It is a memory-critical data structure - do not make larger. This used to be ~100 bytes class, is now 8-byte struct
    let [<Literal>] TagMask           = 0xFFFF000000000000UL // note, there are some spare bits here
    let [<Literal>] KindMask          = 0x0000FF0000000000UL
    let [<Literal>] LeftColumnMask    = 0x000000FFFFF00000UL
    let [<Literal>] MatchedLengthMask = 0x00000000000FFFFFUL

    [<Struct>]
    type SavedTokenInfo = 
        { Bits: uint64 }
        //TagSaved: uint16 
        //KindSaved: byte
        //LeftColumnSaved: uint20  (up to 1048576)
        //MatchedLengthSaved: uint20 (up to 1048576)

        member token.Tag =                        int ((token.Bits &&& TagMask) >>> 48)
        member token.Kind = enum<LexerSymbolKind>(int ((token.Bits &&& KindMask) >>> 40))
        member token.LeftColumn =                 int ((token.Bits &&& LeftColumnMask) >>> 20)
        member token.MatchedLength =              int ((token.Bits &&& MatchedLengthMask))

        member token.IsIdentifier = (token.Kind = LexerSymbolKind.Ident)
        member token.IsOperator = (token.Kind = LexerSymbolKind.Operator)
        member token.IsPunctuation = (token.Kind = LexerSymbolKind.Punctuation)

        static member inline Create (token: FSharpTokenInfo) = 
            let kind = 
                if token.IsOperator then LexerSymbolKind.Operator 
                elif token.IsIdentifier then LexerSymbolKind.Ident 
                elif token.IsPunctuation then LexerSymbolKind.Punctuation
                elif token.IsString then LexerSymbolKind.String
                elif token.ColorClass = FSharpTokenColorKind.Keyword then LexerSymbolKind.Keyword
                else LexerSymbolKind.Other
            Debug.Assert(uint32 token.Tag < 0xFFFFu)
            Debug.Assert(uint32 kind < 0xFFu)
            Debug.Assert(uint32 token.LeftColumn < 0xFFFFFu)
            Debug.Assert(uint32 token.FullMatchedLength < 0xFFFFFu)
            { Bits = 
                 ((uint64 token.Tag        <<< 48) &&& TagMask) |||
                 ((uint64 kind             <<< 40) &&& KindMask) |||
                 ((uint64 token.LeftColumn <<< 20) &&& LeftColumnMask) |||
                  (uint64 token.FullMatchedLength  &&& MatchedLengthMask) }

        member token.RightColumn = token.LeftColumn + token.MatchedLength - 1 
    
    /// An intermediate extraction of information from the token
    type private DraftTokenInfo = 
        { Kind: LexerSymbolKind
          LeftColumn: int 
          MatchedLength: int } 

        static member Create kind (token: SavedTokenInfo) = { Kind = kind; LeftColumn = int token.LeftColumn; MatchedLength = int token.MatchedLength }
        
        member token.RightColumn = token.LeftColumn + token.MatchedLength - 1 

    /// This is the data saved about each line.  It is held strongly while a file is open and 
    /// is important for memory performance
    type private SourceLineData(lineStart: int, lexStateAtStartOfLine: FSharpTokenizerLexState, lexStateAtEndOfLine: FSharpTokenizerLexState, 
                                hashCode: int, classifiedSpans: ClassifiedSpan[], savedTokens: SavedTokenInfo[]) =
        member val LineStart = lineStart
        member val LexStateAtStartOfLine = lexStateAtStartOfLine
        member val LexStateAtEndOfLine = lexStateAtEndOfLine
        member val HashCode = hashCode
        member val ClassifiedSpans = classifiedSpans
        member val SavedTokens = savedTokens
    
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

    /// This saves the tokenization data for a file for as long as the DocumentId object is alive.
    /// This seems risky - if one single thing leaks a DocumentId (e.g. stores it in some global table of documents 
    /// that have been closed), then we leak **all** this associated data, forever.

    type private PerDocumentSavedData = ConcurrentDictionary<string list, SourceTextData>
    let private dataCache = new MemoryCache("FSharp.Editor.Tokenization")

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
        let tokens = ResizeArray<SavedTokenInfo>()
        let mutable tokenInfoOption = None
        let mutable previousLexState = lexState
            
        let processToken() =
            let classificationType = compilerTokenToRoslynToken(tokenInfoOption.Value.ColorClass)
            for i = tokenInfoOption.Value.LeftColumn to tokenInfoOption.Value.RightColumn do
                Array.set colorMap i classificationType

            let token = tokenInfoOption.Value
            let savedToken = SavedTokenInfo.Create token

            tokens.Add savedToken

        let scanAndColorNextToken() =
            let info, nextLexState = lineTokenizer.ScanToken(previousLexState)
            tokenInfoOption <- info
            previousLexState <- nextLexState

            // Apply some hacks to clean up the token stream (we apply more later)
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

        SourceLineData(textLine.Start, lexState, previousLexState, lineContents.GetHashCode(), classifiedSpans.ToArray(), tokens.ToArray())


    // We keep incremental data per-document.  When text changes we correlate text line-by-line (by hash codes of lines)
    // We index the data by the active defines in the document.
    let private getSourceTextData(documentKey: DocumentId, defines: string list, linesCount) =
        let key = documentKey.ToString()
        let dict = 
            match dataCache.Get(key) with
            | :? PerDocumentSavedData as dict -> dict
            | _ -> 
                let dict = new PerDocumentSavedData(1,1,HashIdentity.Structural)
                let cacheItem = CacheItem(key, dict)
                // evict per-document data after a sliding window
                let policy = CacheItemPolicy(SlidingExpiration=DefaultTuning.PerDocumentSavedDataSlidingWindow)
                dataCache.Set(cacheItem, policy)
                dict 
        if dict.ContainsKey(defines) then 
            dict.[defines] 
        else 
            let data = SourceTextData(linesCount) 
            dict.TryAdd(defines, data) |> ignore
            data

    /// Generates a list of Classified Spans for tokens which undergo syntactic classification (i.e., are not typechecked).
    let getClassifiedSpans(documentKey: DocumentId, sourceText: SourceText, textSpan: TextSpan, fileName: string option, defines: string list, 
                             cancellationToken: CancellationToken) : List<ClassifiedSpan> =
            try
                let sourceTokenizer = FSharpSourceTokenizer(defines, fileName)
                let lines = sourceText.Lines
                let sourceTextData = getSourceTextData(documentKey, defines, lines.Count)
 
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
                let mutable lexState = if scanStartLine = 0 then FSharpTokenizerLexState.Initial else sourceTextData.[scanStartLine - 1].Value.LexStateAtEndOfLine
 
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
                        | Some data when data.IsValid(textLine) && data.LexStateAtStartOfLine.Equals(lexState) -> 
                            data
                        | _ -> 
                            // Otherwise, we recompute
                            let newData = scanSourceLine(sourceTokenizer, textLine, lineContents, lexState)
                            sourceTextData.[i] <- Some newData
                            newData
                     
                    lexState <- lineData.LexStateAtEndOfLine
 
                    if startLine <= i then
                        result.AddRange(lineData.ClassifiedSpans |> Array.filter(fun token ->
                            textSpan.Contains(token.TextSpan.Start) ||
                            textSpan.Contains(token.TextSpan.End - 1) ||
                            (token.TextSpan.Start <= textSpan.Start && textSpan.End <= token.TextSpan.End)))

                // If necessary, invalidate all subsequent lines after endLine
                if endLine < lines.Count - 1 then 
                    match sourceTextData.[endLine+1] with 
                    | Some data  -> 
                        if not (data.LexStateAtStartOfLine.Equals(lexState)) then
                            sourceTextData.ClearFrom (endLine+1)
                    | None -> ()
                result
            with 
            | :? System.OperationCanceledException -> reraise()
            |  ex -> 
                Assert.Exception(ex)
                List<ClassifiedSpan>()

    /// Returns symbol at a given position.
    let private getSymbolFromSavedTokens 
        (
            fileName: string, 
            savedTokens: SavedTokenInfo[], 
            linePos: LinePosition, 
            lineStr: string, 
            lookupKind: SymbolLookupKind,
            wholeActivePatterns: bool,
            allowStringToken: bool
        ) 
        : LexerSymbol option =
        
        let (|GenericTypeParameterPrefix|StaticallyResolvedTypeParameterPrefix|ActivePattern|Other|) (token: SavedTokenInfo) =
            if token.Tag = FSharpTokenTag.QUOTE then GenericTypeParameterPrefix
            elif token.Tag = FSharpTokenTag.INFIX_AT_HAT_OP then
                    // The lexer return INFIX_AT_HAT_OP token for both "^" and "@" symbols.
                    // We have to check the char itself to distinguish one from another.
                    if token.MatchedLength = 1 && token.LeftColumn < lineStr.Length && lineStr.[token.LeftColumn] = '^' then 
                        StaticallyResolvedTypeParameterPrefix
                    else Other
            elif token.Tag = FSharpTokenTag.LPAREN then
                if token.MatchedLength = 1 && token.LeftColumn+1 < lineStr.Length && lineStr.[token.LeftColumn+1] = '|' then
                    ActivePattern
                else Other
            else Other
       
        // Operators: Filter out overlapped operators (>>= operator is tokenized as three distinct tokens: GREATER, GREATER, EQUALS. 
        // Each of them has MatchedLength = 3. So, we take the first GREATER and skip the other two).
        //
        // Generic type parameters: we convert QUOTE + IDENT tokens into single IDENT token, altering its LeftColumn 
        // and FullMathedLength (for "'type" which is tokenized as (QUOTE, left=2) + (IDENT, left=3, length=4) 
        // we'll get (IDENT, left=2, length=5).
        //
        // Statically resolved type parameters: we convert INFIX_AT_HAT_OP + IDENT tokens into single IDENT token, altering its LeftColumn 
        // and FullMathedLength (for "^type" which is tokenized as (INFIX_AT_HAT_OP, left=2) + (IDENT, left=3, length=4) 
        // we'll get (IDENT, left=2, length=5).
        let draftTokens = 
            let tokensCount = savedTokens.Length
            (([], None), savedTokens) ||> Array.foldi (fun (acc, lastToken: DraftTokenInfo option) index token ->
                match lastToken with
                | Some t when token.LeftColumn <= t.RightColumn -> acc, lastToken
                | Some ({ Kind = LexerSymbolKind.ActivePattern } as lastToken) when
                    wholeActivePatterns &&
                    (token.Tag = FSharpTokenTag.BAR || token.Tag = FSharpTokenTag.IDENT || token.Tag = FSharpTokenTag.UNDERSCORE) ->
                    
                    let mergedToken =
                        { lastToken with 
                            Kind = LexerSymbolKind.Ident
                            MatchedLength = lastToken.MatchedLength + token.MatchedLength }

                    acc, Some mergedToken
                | _ ->
                    let isLastToken = index = tokensCount - 1
                    match token with
                    | GenericTypeParameterPrefix when not isLastToken -> acc, Some (DraftTokenInfo.Create LexerSymbolKind.GenericTypeParameter token)
                    | StaticallyResolvedTypeParameterPrefix when not isLastToken -> acc, Some (DraftTokenInfo.Create LexerSymbolKind.StaticallyResolvedTypeParameter token)
                    | ActivePattern when wholeActivePatterns -> acc, Some (DraftTokenInfo.Create LexerSymbolKind.ActivePattern token)
                    | _ ->
                        let draftToken =
                            match lastToken with
                            | Some { Kind = LexerSymbolKind.GenericTypeParameter | LexerSymbolKind.StaticallyResolvedTypeParameter as kind } when token.IsIdentifier ->
                                { Kind = kind
                                  LeftColumn = token.LeftColumn - 1
                                  MatchedLength = token.MatchedLength + 1 }
                            // ^ operator                                                
                            | Some { Kind = LexerSymbolKind.StaticallyResolvedTypeParameter } ->
                                { Kind = LexerSymbolKind.Operator 
                                  LeftColumn = token.LeftColumn - 1
                                  MatchedLength = 1 }
                            | Some ( { Kind = LexerSymbolKind.ActivePattern } as ap) when wholeActivePatterns && token.Tag = FSharpTokenTag.RPAREN ->
                                { Kind = LexerSymbolKind.Ident
                                  LeftColumn = ap.LeftColumn 
                                  MatchedLength = ap.MatchedLength }
                            | _ -> 
                                { Kind = token.Kind
                                  LeftColumn = token.LeftColumn 
                                  MatchedLength = token.MatchedLength }
                        draftToken :: acc, Some draftToken
                ) 
            |> fst
           
        // One or two tokens that in touch with the cursor (for "let x|(g) = ()" the tokens will be "x" and "(")
        let tokensUnderCursor = 
            let rightColumnCorrection = 
                match lookupKind with 
                | SymbolLookupKind.Precise -> 0
                | SymbolLookupKind.Greedy -> 1
            
            draftTokens |> List.filter (fun x -> x.LeftColumn <= linePos.Character && (x.RightColumn + rightColumnCorrection) >= linePos.Character)
                
        // Select IDENT token. If failed, select OPERATOR token.
        tokensUnderCursor
        |> List.tryFind (fun token ->             
            match token.Kind with 
            | LexerSymbolKind.Ident
            | LexerSymbolKind.Keyword 
            | LexerSymbolKind.ActivePattern
            | LexerSymbolKind.GenericTypeParameter         
            | LexerSymbolKind.StaticallyResolvedTypeParameter -> true 
            | _ -> false) 
        |> Option.orElseWith (fun _ -> tokensUnderCursor |> List.tryFind (fun token -> token.Kind = LexerSymbolKind.Operator))
        |> Option.orElseWith (fun _ -> if allowStringToken then tokensUnderCursor |> List.tryFind (fun token -> token.Kind = LexerSymbolKind.String) else None)
        |> Option.map (fun token ->
            let partialName = QuickParse.GetPartialLongNameEx(lineStr, token.RightColumn)
            let identStr = lineStr.Substring(token.LeftColumn, token.MatchedLength)
            {   Kind = token.Kind
                Ident = 
                    Ident(identStr, 
                        Range.mkRange 
                            fileName 
                            (Position.mkPos (linePos.Line + 1) token.LeftColumn)
                            (Position.mkPos (linePos.Line + 1) (token.RightColumn + 1))) 
                FullIsland = partialName.QualifyingIdents @ [identStr] })

    let private getCachedSourceLineData(documentKey: DocumentId, sourceText: SourceText, position: int, fileName: string, defines: string list) = 
        let textLine = sourceText.Lines.GetLineFromPosition(position)
        let textLinePos = sourceText.Lines.GetLinePosition(position)
        let lineNumber = textLinePos.Line + 1 // FCS line number
        let sourceTokenizer = FSharpSourceTokenizer(defines, Some fileName)
        let lines = sourceText.Lines
        // We keep incremental data per-document. When text changes we correlate text line-by-line (by hash codes of lines)
        let sourceTextData = getSourceTextData(documentKey, defines, lines.Count)
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
        let lexState = if scanStartLine = 0 then FSharpTokenizerLexState.Initial else sourceTextData.[scanStartLine - 1].Value.LexStateAtEndOfLine
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
            lineData.SavedTokens   
        with 
        |  ex -> 
            Assert.Exception(ex)
            [| |]

    let getSymbolAtPosition
        (
            documentKey: DocumentId, 
            sourceText: SourceText, 
            position: int, 
            fileName: string, 
            defines: string list, 
            lookupKind: SymbolLookupKind,
            wholeActivePatterns: bool,
            allowStringToken: bool
        ) 
        : LexerSymbol option =
        
        try
            let lineData, textLinePos, lineContents = getCachedSourceLineData(documentKey, sourceText, position, fileName, defines)
            getSymbolFromSavedTokens(fileName, lineData.SavedTokens, textLinePos, lineContents, lookupKind, wholeActivePatterns, allowStringToken)
        with 
        | :? System.OperationCanceledException -> reraise()
        |  ex -> 
            Assert.Exception(ex)
            None

    /// Fix invalid span if it appears to have redundant suffix and prefix.
    let fixupSpan (sourceText: SourceText, span: TextSpan) : TextSpan =
        let text = sourceText.GetSubText(span).ToString()
        // backticked ident
        if text.EndsWith "``" then
            match text.LastIndexOf("``", text.Length - 3, text.Length - 2) with
            | -1 | 0 -> span
            | index -> TextSpan(span.Start + index, text.Length - index)
        else 
            match text.LastIndexOf '.' with
            | -1 | 0 -> span
            | index -> TextSpan(span.Start + index + 1, text.Length - index - 1)

    let private doubleBackTickDelimiter = "``"

    let isDoubleBacktickIdent (s: string) =
        let doubledDelimiter = 2 * doubleBackTickDelimiter.Length
        if s.Length > doubledDelimiter && s.StartsWith(doubleBackTickDelimiter, StringComparison.Ordinal) && s.EndsWith(doubleBackTickDelimiter, StringComparison.Ordinal) then
            let inner = s.AsSpan(doubleBackTickDelimiter.Length, s.Length - doubledDelimiter)
            not (inner.Contains(doubleBackTickDelimiter.AsSpan(), StringComparison.Ordinal))
        else false

    let isValidNameForSymbol (lexerSymbolKind: LexerSymbolKind, symbol: FSharpSymbol, name: string) : bool =
        
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
            not (String.IsNullOrEmpty s) && FSharpKeywords.NormalizeIdentifierBackticks s |> isIdentifier
        
        let forbiddenChars = [| '.'; '+'; '$'; '&'; '['; ']'; '/'; '\\'; '*'; '\"' |]
        
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
        | LexerSymbolKind.Operator, _ -> PrettyNaming.IsOperatorDisplayName name
        | LexerSymbolKind.Punctuation, _ -> PrettyNaming.IsPunctuation name
        | LexerSymbolKind.GenericTypeParameter, _ -> isGenericTypeParameter name
        | LexerSymbolKind.StaticallyResolvedTypeParameter, _ -> isStaticallyResolvedTypeParameter name
        | _ ->
            match symbol with
            | :? FSharpEntity as e when e.IsClass || e.IsFSharpRecord || e.IsFSharpUnion || e.IsValueType || e.IsFSharpModule || e.IsInterface -> isTypeNameIdent name
            | _ -> isFixableIdentifier name
