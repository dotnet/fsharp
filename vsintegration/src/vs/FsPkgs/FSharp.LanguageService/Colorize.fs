// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.LanguageService

open System.Collections.Generic
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.TextManager.Interop 
open System.Diagnostics
open Microsoft.VisualStudio
open System.Collections
open Microsoft.VisualStudio.Text

#nowarn "45" // This method will be made public in the underlying IL because it may implement an interface or override a method


/// Maintain a two-way lookup of lexstate to colorstate
/// In practice this table will be quite small. All of F# only uses 38 distinct LexStates.            
module internal ColorStateLookup =
    type ColorStateTable() = 
        let mutable nextInt = 0
        let toInt = Dictionary<LexState,int>()
        let toLexState = Dictionary<int,LexState>()
            
        let Add(lexState) =
            let result = nextInt
            nextInt <- nextInt+1
            System.Diagnostics.Debug.Assert(nextInt<1000, "ColorStateTable exceeded 1000.")
            toInt.Add(lexState,result)
            toLexState.Add(result,lexState)
            result
            
        do Add(0L)|>ignore // Add the 'unknown' state.                
        
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
    
//    Notes:
//      - SetLineText() is called one line at a time.
//      - An instance of FSharpScanner is associated with exactly one buffer (IVsTextLines).
type internal FSharpScanner(makeLineTokenizer : string -> LineTokenizer) = 
    let mutable lineTokenizer = makeLineTokenizer ""

    let mutable extraColorizations = None
    let tryFindExtraInfo ((* newSnapshot:Lazy<ITextSnapshot>, *) line, c1, c2) = 
        match extraColorizations with 
        | None -> None
        | Some ((* hasTextChanged, *) table:IDictionary<_,_>) -> 
             match table.TryGetValue line with 
             | false,_ -> None
             | true,entries -> 
                 entries |> Array.tryPick (fun ((((_,sc),(_,ec)) as range),t) ->
                     ignore range 
#if COLORIZE_TYPES
                     // If we are colorizing type names, then a lot more late-colorization is going on, and we have to be more precise and
                     // check snapshots. However it is not clear where to get the new snapshot from, or if it is expensive to get it.
                     // This is one of the reasons why COLORIZE_TYPES is not enabled.
                     if sc <= c1 &&  c2+1 <= ec (* && not (hasTextChanged (newSnapshot.Force(),range)) *) then 
#else
                     // If we are only colorizing query keywords, and not types, then we can check the exact token range, and that tends to be enough 
                     // to get pretty good incremental accuracy (while waiting for a re-typecheck to refresh results completely)
                     if sc = c1 &&  c2+1 = ec then 
#endif
                         Some t 
                     else 
                         None)
                 
    /// Decode compiler TokenColorKind into VS TokenColor.
    let lookupTokenColor colorKind = 
        match colorKind with
        | TokenColorKind.Comment -> TokenColor.Comment
        | TokenColorKind.Identifier -> TokenColor.Identifier
        | TokenColorKind.Keyword -> TokenColor.Keyword
        | TokenColorKind.String -> TokenColor.String
        | TokenColorKind.Text -> TokenColor.Text
        | TokenColorKind.UpperIdentifier -> TokenColor.Identifier
        | TokenColorKind.Number -> TokenColor.Number
        | TokenColorKind.InactiveCode -> enum 6          // Custom index into colorable item array, 1-based index, see array of FSharpColorableItem in servicem.fs
        | TokenColorKind.PreprocessorKeyword -> enum 7   // Custom index into colorable item array, 1-based index, see array of FSharpColorableItem in servicem.fs
        | TokenColorKind.Operator -> enum 8              // Custom index into colorable item array, 1-based index, see array of FSharpColorableItem in servicem.fs
#if COLORIZE_TYPES
        | TokenColorKind.TypeName -> enum 9              // Custom index into colorable item array, 1-based index, see array of FSharpColorableItem in servicem.fs
#endif
        | TokenColorKind.Default | _ -> TokenColor.Text

    let lookupTokenType colorKind = 
        match colorKind with
        | TokenColorKind.Comment -> TokenType.Comment
        | TokenColorKind.Identifier -> TokenType.Identifier
        | TokenColorKind.Keyword -> TokenType.Keyword
        | TokenColorKind.String -> TokenType.String
        | TokenColorKind.Text -> TokenType.Text
        | TokenColorKind.UpperIdentifier -> TokenType.Identifier
        | TokenColorKind.Number -> TokenType.Literal
        | TokenColorKind.InactiveCode -> TokenType.Unknown
        | TokenColorKind.PreprocessorKeyword -> TokenType.Unknown
        | TokenColorKind.Operator -> TokenType.Operator
#if COLORIZE_TYPES
        | TokenColorKind.TypeName -> TokenType.Identifier
#endif
        | TokenColorKind.Default 
        | _ -> TokenType.Text
        
    member ws.ScanTokenWithDetails lexState =
        let colorInfoOption, newLexState = lineTokenizer.ScanToken(!lexState)
        lexState := newLexState
        colorInfoOption
            
    member ws.ScanTokenAndProvideInfoAboutIt(line, tokenInfo:TokenInfo, lexState) =
        let colorInfoOption, newLexState = lineTokenizer.ScanToken(!lexState)
        lexState := newLexState
        match colorInfoOption with 
        | None -> false 
        | Some colorInfo -> 
            let color = 
                // Upgrade identifiers to keywords based on extra info
                match colorInfo.ColorClass with 
                | TokenColorKind.Identifier 
                | TokenColorKind.UpperIdentifier -> 
                    match tryFindExtraInfo (line, colorInfo.LeftColumn, colorInfo.RightColumn) with 
                    | None -> TokenColorKind.Identifier 
                    | Some info -> info // extra info found
                | c -> c

            tokenInfo.Trigger <- enum (int32 colorInfo.TriggerClass) // cast one enum to another
            tokenInfo.StartIndex <- colorInfo.LeftColumn
            tokenInfo.EndIndex <- colorInfo.RightColumn
            tokenInfo.Color <- lookupTokenColor color
            tokenInfo.Token <- colorInfo.Tag
            tokenInfo.Type <- lookupTokenType color 
            true

    // This is called one line at a time.
    member ws.SetLineText lineText = 
        lineTokenizer <- makeLineTokenizer lineText

    /// Adjust the set of extra colorizations and return a sorted list of changed lines.
    member __.SetExtraColorizations (tokens: (Microsoft.FSharp.Compiler.SourceCodeServices.Range * Microsoft.FSharp.Compiler.SourceCodeServices.TokenColorKind)[]) = 
        if tokens.Length = 0 && extraColorizations.IsNone then 
            [| |] 
        else
            let newExtraColorizationsKeyed = dict (tokens |> Seq.groupBy (fun (((sl,_),(_,_)), _) -> sl) |> Seq.map (fun (k,v) -> (k, Seq.toArray v))) 
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
            (*
            if changedLines.Length = 0 then [| |] else
            // Skip common parts in the sequence
            [| let prev = ref (changedLines.[0] - 1)
               let prevIdx = ref 0
               for i in 0 .. changedLines.Length - 1 do 
                   let curr = changedLines.[i]
                   if curr <> prev.Value + 1 then 
                       yield (prevIdx,curr)
                       prevIdx := i
                       prev := curr |]
                       *)
/// Implement the MPF Colorizer functionality.
///   onClose is a method to call when shutting down the colorizer.
type internal FSharpColorizer(onClose:FSharpColorizer->unit,        
                              buffer:IVsTextLines,
                              scanner:FSharpScanner) =
    

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
        state <- ColorStateLookup.ColorStateOfLexState(0L)
        VSConstants.S_OK

    /// Colorize a line of text. Resulting per-character attributes are stored into attrs
    /// Return value is tokenization state at the start of the next line.
    ///
    /// This is the core entry-point to all our colorization: VS calls this when it wants color information.
    override c.ColorizeLine(line, _length, _ptrLineText, lastColorState, attrs) = 

        let refState = ref (ColorStateLookup.LexStateOfColorState lastColorState)

        let lineText = VsTextLines.LineText buffer line
        
        let length = lineText.Length
        let mutable linepos = 0
        //let newSnapshot = lazy (SourceImpl.GetWpfTextViewFromVsTextView(scanner.TextView).TextSnapshot)
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

        ColorStateLookup.ColorStateOfLexState !refState

        
    ///Get the state at the end of the given line.
    override c.GetStateAtEndOfLine(line,length,ptr,state) = (c :> IVsColorizer).ColorizeLine(line, length, ptr, state, null)

    member c.GetFullLineInfo(lineText,lastColorState) = 
        let refState = ref (ColorStateLookup.LexStateOfColorState lastColorState)
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
        let refState = ref (ColorStateLookup.LexStateOfColorState lastColorState)
        scanner.SetLineText lineText

        let cache = new ResizeArray<TokenInfo>()
        let mutable tokenInfo = new TokenInfo(EndIndex = -1)
        let mutable firstTime = true 
        //let newSnapshot = lazy (SourceImpl.GetWpfTextViewFromVsTextView(textView).TextSnapshot)
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
        ColorStateLookup.ColorStateOfLexState !refState
        
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
       
    member c.GetTokenInfoAt(colorState,line,col) =
        let state = VsTextColorState.GetColorStateAtStartOfLine colorState line
        let lexState = ref (ColorStateLookup.LexStateOfColorState state)
            
        let lineText = VsTextLines.LineText buffer line
        let tokenInfo = new TokenInfo()
        scanner.SetLineText lineText
        tokenInfo.EndIndex <- -1
        while scanner.ScanTokenAndProvideInfoAboutIt(line, tokenInfo, lexState) && not (col>=tokenInfo.StartIndex && col<=tokenInfo.EndIndex) do 
            ()
        tokenInfo

    member c.GetTokenInfoAt(colorState,line,col,trialString,trialStringInsertionCol) =
        let state = VsTextColorState.GetColorStateAtStartOfLine colorState line
        let lexState = ref (ColorStateLookup.LexStateOfColorState state)
            
        let lineText = (VsTextLines.LineText buffer line).Insert(trialStringInsertionCol, trialString)
        let tokenInfo = new TokenInfo()
        scanner.SetLineText lineText
        tokenInfo.EndIndex <- -1
        while scanner.ScanTokenAndProvideInfoAboutIt(line, tokenInfo, lexState) && not (col>=tokenInfo.StartIndex && col<=tokenInfo.EndIndex) do 
            ()
        tokenInfo

    member c.GetTokenInformationAt(colorState,line,col) =
        let state = VsTextColorState.GetColorStateAtStartOfLine colorState line
        let lexState = ref (ColorStateLookup.LexStateOfColorState state)
            
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

    member __.SetExtraColorizations tokens = scanner.SetExtraColorizations tokens
