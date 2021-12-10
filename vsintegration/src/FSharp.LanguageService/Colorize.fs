// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

//------- DEPRECATED CODE ONLY ACTIVE IN UNIT TESTING VIA "UNROSLYNIZED" UNIT TESTS ---------------

//------- DEPRECATED CODE ONLY ACTIVE IN UNIT TESTING VIA "UNROSLYNIZED" UNIT TESTS ---------------

namespace Microsoft.VisualStudio.FSharp.LanguageService

open System.Collections.Generic
open System.Collections
open System.Diagnostics
open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.TextManager.Interop
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Text
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text
open FSharp.Compiler.Tokenization

#nowarn "45" // This method will be made public in the underlying IL because it may implement an interface or override a method


/// Maintain a two-way lookup of lexstate to colorstate
/// In practice this table will be quite small. All of F# only uses 38 distinct LexStates.
//
// Note: DEPRECATED CODE ONLY ACTIVE IN UNIT TESTING VIA "UNROSLYNIZED" UNIT TESTS 
//
// Note: Tests using this code should either be adjusted to test the corresponding feature in
// FSharp.Editor, or deleted.  However, the tests may be exercising underlying F# Compiler 
// functionality and thus have considerable value, they should ony be deleted if we are sure this 
// is not the case.
//
module internal ColorStateLookup_DEPRECATED =
    type ColorStateTable() =
        let mutable nextInt = 0
        let toInt = Dictionary<FSharpTokenizerLexState,int>()
        let toLexState = Dictionary<int,FSharpTokenizerLexState>()

        let Add(lexState) =
            let result = nextInt
            nextInt <- nextInt+1
            System.Diagnostics.Debug.Assert(nextInt<1000, "ColorStateTable exceeded 1000.")
            toInt.Add(lexState,result)
            toLexState.Add(result,lexState)
            result

        do Add(FSharpTokenizerLexState.Initial)|>ignore // Add the 'unknown' state.

        static member private TryGet<'tKey,'tVal>(dict:Dictionary<'tKey,'tVal>,key:'tKey) : 'tVal option =
            let mutable result = Unchecked.defaultof<'tVal>
            let ok = dict.TryGetValue(key,&result)
            if ok then Some(result)
            else None

        member cst.ColorStateOfLexState(lexState) =
            match ColorStateTable.TryGet(toInt,lexState) with
            | Some(i) -> i
            | None -> Add(lexState)

        member cst.LexStateOfColorState(i) =
            match ColorStateTable.TryGet(toLexState,i) with
            | Some(lexState) -> lexState
            | None -> failwith "Unknown colorstate"

    let private cst = ColorStateTable()

    let ColorStateOfLexState lexState = cst.ColorStateOfLexState(lexState)
    let LexStateOfColorState colorState = cst.LexStateOfColorState(colorState)

/// A single scanner object which can be used to scan different lines of text.
/// Each time a scan of new line of text is started the makeLineTokenizer function is called.
///
/// An instance of this is stored in the IVsUserData for the IVsTextLines buffer
/// and retrieved using languageServiceState.GetColorizer(IVsTextLines).
//
//    Notes:
//      - SetLineText() is called one line at a time.
//      - An instance of FSharpScanner_DEPRECATED is associated with exactly one buffer (IVsTextLines).
//
// Note: DEPRECATED CODE ONLY ACTIVE IN UNIT TESTING VIA "UNROSLYNIZED" UNIT TESTS. 
//
// Note: Tests using this code should either be adjusted to test the corresponding feature in
// FSharp.Editor, or deleted.  However, the tests may be exercising underlying F# Compiler 
// functionality and thus have considerable value, they should ony be deleted if we are sure this 
// is not the case.
//
type internal FSharpScanner_DEPRECATED(makeLineTokenizer : string -> FSharpLineTokenizer) =
    let mutable lineTokenizer = makeLineTokenizer ""

    let mutable extraColorizations : IDictionary<Line0, SemanticClassificationItem[] > option = None

    /// Decode compiler FSharpTokenColorKind into VS TokenColor.
    let lookupTokenColor colorKind =
        match colorKind with
        | FSharpTokenColorKind.Comment -> TokenColor.Comment
        | FSharpTokenColorKind.Identifier -> TokenColor.Identifier
        | FSharpTokenColorKind.Keyword -> TokenColor.Keyword
        | FSharpTokenColorKind.String -> TokenColor.String
        | FSharpTokenColorKind.Text -> TokenColor.Text
        | FSharpTokenColorKind.UpperIdentifier -> TokenColor.Identifier
        | FSharpTokenColorKind.Number -> TokenColor.Number
        | FSharpTokenColorKind.InactiveCode -> enum 6          // Custom index into colorable item array, 1-based index, see array of FSharpColorableItem_DEPRECATED in servicem.fs
        | FSharpTokenColorKind.PreprocessorKeyword -> enum 7   // Custom index into colorable item array, 1-based index, see array of FSharpColorableItem_DEPRECATED in servicem.fs
        | FSharpTokenColorKind.Operator -> enum 8              // Custom index into colorable item array, 1-based index, see array of FSharpColorableItem_DEPRECATED in servicem.fs
        | FSharpTokenColorKind.Default | _ -> TokenColor.Text

    /// Decode compiler FSharpTokenColorKind into VS TokenType.
    let lookupTokenType colorKind =
        match colorKind with
        | FSharpTokenColorKind.Comment -> TokenType.Comment
        | FSharpTokenColorKind.Identifier -> TokenType.Identifier
        | FSharpTokenColorKind.Keyword -> TokenType.Keyword
        | FSharpTokenColorKind.String -> TokenType.String
        | FSharpTokenColorKind.Text -> TokenType.Text
        | FSharpTokenColorKind.UpperIdentifier -> TokenType.Identifier
        | FSharpTokenColorKind.Number -> TokenType.Literal
        | FSharpTokenColorKind.InactiveCode -> TokenType.Unknown
        | FSharpTokenColorKind.PreprocessorKeyword -> TokenType.Unknown
        | FSharpTokenColorKind.Operator -> TokenType.Operator
        | FSharpTokenColorKind.Default
        | _ -> TokenType.Text

    /// Scan a token from a line. This should only be used in cases where color information is irrelevant.
    /// Used by GetFullLineInfo (and only thus in a small workaroud in GetDeclarations) and GetTokenInformationAt (thus GetF1KeywordString).
    member ws.ScanTokenWithDetails (lexState: _ ref) =
        let colorInfoOption, newLexState = lineTokenizer.ScanToken(lexState.Value)
        lexState.Value <- newLexState
        colorInfoOption

    /// Scan a token from a line and write information about it into the tokeninfo object.
    member ws.ScanTokenAndProvideInfoAboutIt(_line, tokenInfo:TokenInfo, lexState: _ ref) =
        let colorInfoOption, newLexState = lineTokenizer.ScanToken(!lexState)
        lexState.Value <- newLexState
        match colorInfoOption with
        | None -> false
        | Some colorInfo ->
            let color = colorInfo.ColorClass
            tokenInfo.Trigger <- enum (int32 colorInfo.FSharpTokenTriggerClass) // cast one enum to another
            tokenInfo.StartIndex <- colorInfo.LeftColumn
            tokenInfo.EndIndex <- colorInfo.RightColumn
            tokenInfo.Color <- lookupTokenColor color
            tokenInfo.Token <- colorInfo.Tag
            tokenInfo.Type <- lookupTokenType color
            true

    /// Start tokenizing a line
    member ws.SetLineText lineText =
        lineTokenizer <- makeLineTokenizer lineText

    /// Adjust the set of extra colorizations and return a sorted list of affected lines.
    member _.SetExtraColorizations (tokens: SemanticClassificationItem[]) =
        if tokens.Length = 0 && extraColorizations.IsNone then
            [| |]
        else
            let newExtraColorizationsKeyed = dict (tokens |> Array.groupBy (fun item -> Line.toZ item.Range.StartLine))
            let oldExtraColorizationsKeyedOpt = extraColorizations
            extraColorizations <- Some newExtraColorizationsKeyed
            let changedLines =
                match oldExtraColorizationsKeyedOpt with
                | None -> newExtraColorizationsKeyed.Keys |> Seq.toArray
                | Some oldExtraColorizationsKeyed ->
                   // When only colorizing query keywords, in most situations we expect both oldExtraColorizationsKeyed and newExtraColorizationsKeyed to be relatively small
                   let inOneButNotTheOther = HashSet(oldExtraColorizationsKeyed.Keys)
                   inOneButNotTheOther.SymmetricExceptWith(newExtraColorizationsKeyed.Keys)
                   let inBoth = HashSet(oldExtraColorizationsKeyed.Keys)
                   inBoth.IntersectWith(newExtraColorizationsKeyed.Keys)
                   inBoth.RemoveWhere(fun i -> newExtraColorizationsKeyed.[i] = oldExtraColorizationsKeyed.[i]) |> ignore
                   Array.append (Seq.toArray inOneButNotTheOther) (Seq.toArray inBoth)
            Array.sortInPlace changedLines
            changedLines


/// Implement the MPF Colorizer functionality.
///   onClose is a method to call when shutting down the colorizer.
//
// Note: DEPRECATED CODE ONLY ACTIVE IN UNIT TESTING VIA "UNROSLYNIZED" UNIT TESTS. 
//
// Note: Tests using this code should either be adjusted to test the corresponding feature in
// FSharp.Editor, or deleted.  However, the tests may be exercising underlying F# Compiler 
// functionality and thus have considerable value, they should ony be deleted if we are sure this 
// is not the case.
//
type internal FSharpColorizer_DEPRECATED
                             (onClose:FSharpColorizer_DEPRECATED->unit,
                              buffer:IVsTextLines,
                              scanner:FSharpScanner_DEPRECATED) =


    inherit Colorizer()
    let currentTokenInfo = new TokenInfo()
    let mutable cachedLineInfo = [||]
    let mutable cachedLine = 0
    let mutable cachedLineState = 0
    let mutable cachedLineText = ""

    /// Close the colorizer. Is supposed to result in TextBuffers being released.
    override c.CloseColorizer() = onClose c

    /// Start state at the beginning of parsing a file.
    override c.GetStartState(state) =
        state <- ColorStateLookup_DEPRECATED.ColorStateOfLexState(FSharpTokenizerLexState.Initial)
        VSConstants.S_OK

    /// Colorize a line of text. Resulting per-character attributes are stored into attrs
    /// Return value is tokenization state at the start of the next line.
    ///
    /// This is the core entry-point to all our colorization: VS calls this when it wants color information.
    override c.ColorizeLine(line, _length, _ptrLineText, lastColorState, attrs) =

        let refState = ref (ColorStateLookup_DEPRECATED.LexStateOfColorState lastColorState)

        let lineText = VsTextLines.LineText buffer line

        let length = lineText.Length
        let mutable linepos = 0
        //let newSnapshot = lazy (FSharpSourceBase_DEPRECATED.GetWpfTextViewFromVsTextView(scanner.TextView).TextSnapshot)
        try
            scanner.SetLineText lineText
            currentTokenInfo.EndIndex <- -1
            while scanner.ScanTokenAndProvideInfoAboutIt(line, currentTokenInfo, refState) do
                if attrs <> null then
                    for i in linepos..(currentTokenInfo.StartIndex-1) do
                        attrs.[i] <- uint32 TokenColor.Text

                let color = uint32 currentTokenInfo.Color

                // Though I've never seen VS do spell checking, the HUMAN_TEXT_ATTR indicates
                // that the token is eligible to be spellchecked (for example).
                let color = if currentTokenInfo.Type = TokenType.Comment ||
                                currentTokenInfo.Type = TokenType.LineComment ||
                                currentTokenInfo.Type = TokenType.String ||
                                currentTokenInfo.Type = TokenType.Text then
                                color ||| uint32 COLORIZER_ATTRIBUTE.HUMAN_TEXT_ATTR
                            else
                                color

                if attrs <> null then
                    for i in currentTokenInfo.StartIndex..currentTokenInfo.EndIndex do
                        attrs.[i] <- color

                linepos<-currentTokenInfo.EndIndex+1
          with e ->
                System.Diagnostics.Debug.Assert(false, sprintf "Exception while colorizing: %A" e)
                reraise()

        if attrs <> null then
            for i in linepos..(length-1) do
                attrs.[i] <- uint32 TokenColor.Text

        ColorStateLookup_DEPRECATED.ColorStateOfLexState !refState


    ///Get the state at the end of the given line.
    override c.GetStateAtEndOfLine(line,length,ptr,state) = (c :> IVsColorizer).ColorizeLine(line, length, ptr, state, null)

    member c.GetFullLineInfo(lineText,lastColorState) =
        let refState = ref (ColorStateLookup_DEPRECATED.LexStateOfColorState lastColorState)
        scanner.SetLineText lineText
        let rec tokens() =
            seq { match scanner.ScanTokenWithDetails(refState) with
                  | Some tok ->
                      yield tok
                      yield! tokens()
                  | None -> () }
        tokens() |> Array.ofSeq

    [<CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId="length-1")>] // exceeds EndIndex
    member private c.GetColorInfo(line,lineText,length,lastColorState) =
        let refState = ref (ColorStateLookup_DEPRECATED.LexStateOfColorState lastColorState)
        scanner.SetLineText lineText

        let cache = new ResizeArray<TokenInfo>()
        let mutable tokenInfo = new TokenInfo(EndIndex = -1)
        let mutable firstTime = true
        //let newSnapshot = lazy (FSharpSourceBase_DEPRECATED.GetWpfTextViewFromVsTextView(textView).TextSnapshot)
        while scanner.ScanTokenAndProvideInfoAboutIt(line, tokenInfo, refState) do
            if firstTime && tokenInfo.StartIndex > 1 then
                cache.Add(new TokenInfo(0, tokenInfo.StartIndex - 1, TokenType.WhiteSpace))
            firstTime<-false
            cache.Add tokenInfo
            tokenInfo<-new TokenInfo()

        if cache.Count > 0 then
            tokenInfo <- cache.[cache.Count - 1]
        if tokenInfo.EndIndex < (length - 1) then
            cache.Add (new TokenInfo(tokenInfo.EndIndex + 1, length - 1, TokenType.WhiteSpace))

        cachedLineInfo <- cache.ToArray()
        ColorStateLookup_DEPRECATED.ColorStateOfLexState !refState

    /// Ultimately called by GetWordExtent in Source.cs in the C# code.
    override c.GetLineInfo(buffer, line, colorState:IVsTextColorState) =
        let length = VsTextLines.LengthOfLine buffer line
        if length = 0 then null
        else
            let lineText = VsTextLines.LineText buffer line
            let vsState = Com.ThrowOnFailure1 (colorState.GetColorStateAtStartOfLine(line))

            if cachedLine = line && cachedLineText = lineText && cachedLineState = vsState && cachedLineInfo <> null then
                cachedLineInfo
            else
                cachedLineInfo <- null
                cachedLine <- line
                cachedLineText <- lineText
                cachedLineState <- vsState
                let _ = c.GetColorInfo(line,lineText, length, vsState)
                cachedLineInfo

    /// Provide token information for the token at the given line and column
    member c.GetTokenInfoAt(colorState,line,col) =
        let state = VsTextColorState.GetColorStateAtStartOfLine colorState line
        let lexState = ref (ColorStateLookup_DEPRECATED.LexStateOfColorState state)

        let lineText = VsTextLines.LineText buffer line
        let tokenInfo = new TokenInfo()
        scanner.SetLineText lineText
        tokenInfo.EndIndex <- -1
        while scanner.ScanTokenAndProvideInfoAboutIt(line, tokenInfo, lexState) && not (col>=tokenInfo.StartIndex && col<=tokenInfo.EndIndex) do
            ()
        tokenInfo

    /// Provide token information for the token at the given line and column (2nd variation - allows caller to get token info if an additional string were to be inserted)
    member c.GetTokenInfoAt(colorState,line,col,trialString,trialStringInsertionCol) =
        let state = VsTextColorState.GetColorStateAtStartOfLine colorState line
        let lexState = ref (ColorStateLookup_DEPRECATED.LexStateOfColorState state)

        let lineText = (VsTextLines.LineText buffer line).Insert(trialStringInsertionCol, trialString)
        let tokenInfo = new TokenInfo()
        scanner.SetLineText lineText
        tokenInfo.EndIndex <- -1
        while scanner.ScanTokenAndProvideInfoAboutIt(line, tokenInfo, lexState) && not (col>=tokenInfo.StartIndex && col<=tokenInfo.EndIndex) do
            ()
        tokenInfo

    /// Provide token information for the token at the given line and column (3rd variation)
    member c.GetTokenInformationAt(colorState,line,col) =
        let state = VsTextColorState.GetColorStateAtStartOfLine colorState line
        let lexState = ref (ColorStateLookup_DEPRECATED.LexStateOfColorState state)

        let lineText = VsTextLines.LineText buffer line
        scanner.SetLineText lineText

        let rec searchForToken () =
            match scanner.ScanTokenWithDetails lexState with
            |   None -> None
            |   Some ti as result ->
                if col >= ti.LeftColumn && col <= ti.RightColumn then
                    result
                else
                    searchForToken()
        searchForToken ()

    member c.Buffer = buffer

    member _.SetExtraColorizations tokens = scanner.SetExtraColorizations tokens


/// Implements IVsColorableItem and IVsMergeableUIItem, for colored text items
//
// Note: DEPRECATED CODE ONLY ACTIVE IN UNIT TESTING VIA "UNROSLYNIZED" UNIT TESTS. 
//
// Note: Tests using this code should either be adjusted to test the corresponding feature in
// FSharp.Editor, or deleted.  However, the tests may be exercising underlying F# Compiler 
// functionality and thus have considerable value, they should ony be deleted if we are sure this 
// is not the case.
//
type internal FSharpColorableItem_DEPRECATED(canonicalName: string, displayName : Lazy<string>, foreground, background) =

    interface IVsColorableItem with

        member x.GetDefaultColors(piForeground, piBackground) =
            piForeground.[0] <- foreground
            piBackground.[0] <- background
            VSConstants.S_OK

        member x.GetDefaultFontFlags(pdwFontFlags) =
            pdwFontFlags <- 0u
            VSConstants.S_OK

        member x.GetDisplayName(pbstrName) =
            pbstrName <- displayName.Force()
            VSConstants.S_OK

    interface IVsMergeableUIItem with

        member this.GetCanonicalName(s) =
            s <- canonicalName
            VSConstants.S_OK

        member this.GetDescription(s) =
            s <- ""
            VSConstants.S_OK

        member x.GetDisplayName(s) =
            s <- displayName.Force()
            VSConstants.S_OK

        member x.GetMergingPriority(i) =
            i <- 0x1000  // as per docs, MS products should use a value between 0x1000 and 0x2000
            VSConstants.S_OK
