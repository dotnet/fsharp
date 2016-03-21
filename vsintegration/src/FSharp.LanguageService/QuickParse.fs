// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.LanguageService

open System
open System.Globalization
open Microsoft.FSharp.Compiler.SourceCodeServices

/// Methods for cheaply and innacurately parsing F#.
///
/// These methods are very old and are mostly to do with extracting "long identifier islands" 
///     A.B.C
/// from F# source code, an approach taken from pre-F# VS samples for implementing intelliense.
///
/// This code should really no longer be needed since the language service has access to 
/// parsed F# source code ASTs.  However, the long identifiers are still passed back to GetDeclarations and friends in the 
/// F# Compiler Service and it's annoyingly hard to remove their use completely.
///
/// In general it is unlikely much progress will be made by fixing this code - it will be better to 
/// extract more information from the F# ASTs.
///
/// It's also surprising how hard even the job of getting long identifier islands can be. For example the code 
/// below is inaccurate for long identifier chains involving ``...`` identifiers.  And there are special cases
/// for active pattern names and so on.
module internal QuickParse =
    open Microsoft.FSharp.Compiler.SourceCodeServices.PrettyNaming
  
    let magicalAdjustmentConstant = 1 // 0 puts us immediately *before* the last character; 1 puts us after the last character

    // Adjusts the token tag for the given identifier
    // - if we're inside active pattern name (at the bar), correct the token TAG to be an identifier
    let CorrectIdentifierToken (s:string) tokenTag = 
        if (s.EndsWith("|")) then (Microsoft.FSharp.Compiler.Parser.tagOfToken (Microsoft.FSharp.Compiler.Parser.token.IDENT s)) 
        else tokenTag

    let rec isValidStrippedName (name:string) idx = 
        if (idx = name.Length) then false
        elif (IsIdentifierPartCharacter name.[idx]) then true
        else isValidStrippedName name (idx + 1)

    // Utility function that recognizes whether a name is valid active pattern name
    // Extracts the 'core' part without surrounding bars and checks whether it contains some identifier
    // (Note, this doesn't have to be precise, because this is checked by backround compiler,
    // but it has to be good enough to distinguish operators and active pattern names)
    let private isValidActivePatternName (name:string) = 

      // Strip the surrounding bars (e.g. from "|xyz|_|") to get "xyz"
      match name.StartsWith("|", System.StringComparison.Ordinal), 
            name.EndsWith("|_|", System.StringComparison.Ordinal), 
            name.EndsWith("|", System.StringComparison.Ordinal) with
      | true, true, _ when name.Length > 4 -> isValidStrippedName (name.Substring(1, name.Length - 4)) 0
      | true, _, true when name.Length > 2 -> isValidStrippedName (name.Substring(1, name.Length - 2)) 0
      | _ -> false
    
    /// Given a string and a position in that string, find an identifier as
    /// expected by `GotoDefinition`. This will work when the cursor is
    /// immediately before the identifier, within the identifier, or immediately
    /// after the identifier.
    ///
    /// 'tolerateJustAfter' indicates that we tolerate being one character after the identifier, used
    /// for goto-definition
    
    /// In general, only identifiers composed from upper/lower letters and '.' are supported, but there
    /// are a couple of explicitly handled exceptions to allow some common scenarios:
    /// - When the name contains only letters and '|' symbol, it may be an active pattern, so we 
    ///   treat it as a valid identifier - e.g. let ( |Identitiy| ) a = a
    ///   (but other identifiers that include '|' are not allowed - e.g. '||' operator)
    /// - It searches for double tick (``) to see if the identifier could be something like ``a b``

    /// REVIEW: Also support, e.g., operators, performing the necessary mangling.
    /// (i.e., I would like that the name returned here can be passed as-is
    /// (post `.`-chopping) to `GetDeclarationLocation.)
        
    /// In addition, return the position where a `.` would go if we were making
    /// a call to `DeclItemsForNamesAtPosition` for intellisense. This will
    /// allow us to use find the correct qualified items rather than resorting
    /// to the more expensive and less accurate environment lookup.
    let GetCompleteIdentifierIslandImpl (s : string) (p : int) : (string*int*bool) option =
        if p<0 || s = null || p>=s.Length then None 
        else
            let fixup =
                match () with
                // at a valid position, on a valid character
                | _ when (p < s.Length) && (s.[p] = '|' || IsIdentifierPartCharacter s.[p]) -> Some p
                | _ -> None // not on a word or '.'
            
            
            let (|Char|_|) p = if p >=0 && p < s.Length then Some(s.[p]) else None   
            let (|IsLongIdentifierPartChar|_|) c = if IsLongIdentifierPartCharacter c then Some () else None           
            let (|IsIdentifierPartChar|_|) c = if IsIdentifierPartCharacter c then Some () else None

            let rec searchLeft p =
                match (p - 1), (p - 2) with
                | Char '|', Char '[' -> p // boundary of array declaration - stop
                | Char '|', _ 
                | Char IsLongIdentifierPartChar, _ -> searchLeft (p - 1) // allow normal chars and '.'s
                | _ -> p

            let rec searchRight p =
                match (p + 1), (p + 2) with
                | Char '|', Char ']' -> p // boundary of array declaration - stop
                | Char '|', _
                | Char IsIdentifierPartChar, _ -> searchRight (p + 1) // allow only normal chars (stop at '.')
                | _ -> p                 
            
            let tickColsOpt = 
                let rec walkOutsideBackticks i =
                    if i >= s.Length then None 
                    else
                    match i, i + 1 with
                    | Char '`', Char '`' ->
                        // dive into backticked part
                        // if pos = i then it will be included in backticked range ($``identifier``)
                        walkInsideBackticks (i + 2) i
                    | _, _ -> 
                        if i >= p then None
                        else
                            // we still not reached position p - continue walking 
                            walkOutsideBackticks (i + 1)
                and walkInsideBackticks i start = 
                    if i >= s.Length then None // non-closed backticks
                    else
                    match i, i + 1 with
                    | Char '`', Char '`' ->
                        // found closing pair of backticks
                        // if target position is between start and current pos + 1 (entire range of escaped identifier including backticks) - return success
                        // else climb outside and continue walking
                        if p >= start && p < (i + 2) then Some (start, i)
                        else walkOutsideBackticks (i + 2)
                    | _, _ -> walkInsideBackticks (i + 1) start                    

                walkOutsideBackticks 0
            
            match tickColsOpt with
            | Some (prevTickTick, idxTickTick) ->
                // inside ``identifier`` (which can contain any characters!) so we try returning its location
                let pos = idxTickTick + 1 + magicalAdjustmentConstant
                let ident = s.Substring(prevTickTick, idxTickTick - prevTickTick + 2)
                Some(ident, pos, true)
            | _ ->
                // find location of an ordinary identifier
                fixup |> Option.bind (fun p ->
                    let l = searchLeft p
                    let r = searchRight p
                    let ident = s.Substring (l, r - l + 1)
                    if (ident.IndexOf('|') <> -1 && not(isValidActivePatternName(ident))) then None else
                        let pos   = r + magicalAdjustmentConstant 
                        Some(ident, pos, false)
                    )

    let GetCompleteIdentifierIsland (tolerateJustAfter:bool) (s : string) (p : int) : (string*int*bool) option =
        if String.IsNullOrEmpty(s) then None
        else     
            let directResult = GetCompleteIdentifierIslandImpl s p
            if tolerateJustAfter && directResult = None then 
                GetCompleteIdentifierIslandImpl s (p-1)
            else 
                directResult

    /// Get the partial long name of the identifier to the left of index.
    let GetPartialLongName(line:string,index) =
        if line = null then ([],"")
        elif index<0 then ([],"")
        elif index>=line.Length then ([],"")
        else
            let IsIdentifierPartCharacter(pos) = IsIdentifierPartCharacter(line.[pos])
            let IsLongIdentifierPartCharacter(pos) = IsLongIdentifierPartCharacter(line.[pos])
            let IsDot(pos) = line.[pos]='.'

            let rec InLeadingIdentifier(pos,right,(prior,residue)) = 
                let PushName() = 
                    ((line.Substring(pos+1,right-pos-1))::prior),residue
                if pos < 0 then PushName()
                elif IsIdentifierPartCharacter(pos) then InLeadingIdentifier(pos-1,right,(prior,residue))
                elif IsDot(pos) then InLeadingIdentifier(pos-1,pos,PushName())
                else PushName()

            let rec InName(pos,startResidue,right) =
                let NameAndResidue() = 
                    [line.Substring(pos+1,startResidue-pos-1)],(line.Substring(startResidue+1,right-startResidue))
                if pos < 0 then [line.Substring(pos+1,startResidue-pos-1)],(line.Substring(startResidue+1,right-startResidue))
                elif IsIdentifierPartCharacter(pos) then InName(pos-1,startResidue,right) 
                elif IsDot(pos) then InLeadingIdentifier(pos-1,pos,NameAndResidue())
                else NameAndResidue()

            let rec InResidue(pos,right) =
                if pos < 0 then [],(line.Substring(pos+1,right-pos))
                elif IsDot(pos) then InName(pos-1,pos,right)
                elif IsLongIdentifierPartCharacter(pos) then InResidue(pos-1, right)
                else [],(line.Substring(pos+1,right-pos))
                
            let result = InResidue(index,index)
            result
    
    type private EatCommentCallContext =
        | SkipWhiteSpaces of string * string list * bool
        | StartIdentifier of string list * bool

    /// Get the partial long name of the identifier to the left of index.
    let GetPartialLongNameEx(line:string,index) : (string list * string) =
        if line = null then ([],"")
        elif index<0 then ([],"")
        elif index>=line.Length then ([],"")
        else
            let IsIdentifierPartCharacter(pos) = IsIdentifierPartCharacter(line.[pos])
            let IsIdentifierStartCharacter(pos) = IsIdentifierPartCharacter(pos)
            let IsDot(pos) = line.[pos]='.'
            let IsTick(pos) = line.[pos]='`'
            let IsEndOfComment(pos) = pos < index - 1 && line.[pos] = '*' && line.[pos + 1] = ')'
            let IsStartOfComment(pos) = pos < index - 1 && line.[pos] = '(' && line.[pos + 1] = '*'
            let IsWhitespace(pos) = Char.IsWhiteSpace(line.[pos])

            let rec SkipWhitespaceBeforeDotIdentifier(pos, ident, current,throwAwayNext) =
                if pos > index then [],""  // we're in whitespace after an identifier, if this is where the cursor is, there is no PLID here
                elif IsWhitespace(pos) then SkipWhitespaceBeforeDotIdentifier(pos+1,ident,current,throwAwayNext)
                elif IsDot(pos) then AtStartOfIdentifier(pos+1,ident::current,throwAwayNext)
                elif IsStartOfComment pos then EatComment(1, pos + 1, EatCommentCallContext.SkipWhiteSpaces(ident, current, throwAwayNext))
                else AtStartOfIdentifier(pos,[],false) // Throw away what we have and start over.

            and EatComment (nesting, pos, callContext) = 
                if pos > index then [], ""
                else
                if IsStartOfComment(pos) then
                    // track balance of closing '*)'
                    EatComment(nesting + 1, pos + 2, callContext)
                else
                if IsEndOfComment(pos) then
                    if nesting = 1 then 
                        // all right, we are at the end of comment, jump outside
                        match callContext with
                        | EatCommentCallContext.SkipWhiteSpaces(ident, current, throwAway) ->
                            SkipWhitespaceBeforeDotIdentifier(pos + 2, ident, current, throwAway)
                        | EatCommentCallContext.StartIdentifier(current, throwAway) ->
                            AtStartOfIdentifier(pos + 2, current, throwAway)
                    else 
                        // reduce level of nesting and continue
                        EatComment(nesting - 1, pos + 2, callContext)
                else
                    // eat next char
                    EatComment(nesting, pos + 1, callContext)

            and InUnquotedIdentifier(left:int,pos:int,current,throwAwayNext) =
                if pos > index then 
                    if throwAwayNext then [],"" else current,(line.Substring(left,pos-left))
                else
                    if IsIdentifierPartCharacter(pos) then InUnquotedIdentifier(left,pos+1,current,throwAwayNext)
                    elif IsDot(pos) then 
                        let ident = line.Substring(left,pos-left)
                        AtStartOfIdentifier(pos+1,ident::current,throwAwayNext)
                    elif IsWhitespace(pos) || IsStartOfComment(pos) then 
                        let ident = line.Substring(left,pos-left)
                        SkipWhitespaceBeforeDotIdentifier(pos, ident, current,throwAwayNext)
                    else AtStartOfIdentifier(pos,[],false) // Throw away what we have and start over.

            and InQuotedIdentifier(left:int,pos:int, current,throwAwayNext) =
                if pos > index then 
                    if throwAwayNext then [],"" else current,(line.Substring(left,pos-left))
                else
                    let remainingLength = line.Length-pos
                    if IsTick(pos) && remainingLength>1 && IsTick(pos+1) then 
                        let ident = line.Substring(left, pos-left)
                        SkipWhitespaceBeforeDotIdentifier(pos+2,ident,current,throwAwayNext) 
                    else InQuotedIdentifier(left,pos+1,current,throwAwayNext)                    

            and AtStartOfIdentifier(pos:int, current, throwAwayNext) =
                if pos > index then 
                    if throwAwayNext then [],"" else current,""
                else
                    if IsWhitespace(pos) then AtStartOfIdentifier(pos+1,current,throwAwayNext)
                    else
                        let remainingLength = line.Length-pos
                        if IsTick(pos) && remainingLength>1 && IsTick(pos+1) then InQuotedIdentifier(pos+2,pos+2,current,throwAwayNext)
                        elif IsStartOfComment(pos) then EatComment(1, pos + 1, EatCommentCallContext.StartIdentifier(current, throwAwayNext))
                        elif IsIdentifierStartCharacter(pos) then InUnquotedIdentifier(pos,pos+1,current,throwAwayNext)
                        elif IsDot(pos) then 
                            if pos=0 then
                                // dot on first char of line, currently treat it like empty identifier to the left
                                AtStartOfIdentifier(pos+1,""::current,throwAwayNext)                            
                            elif not(pos>0 && (IsIdentifierPartCharacter(pos-1) || IsWhitespace(pos-1))) then
                                // it's not dots as part.of.a.long.ident, it's e.g. the range operator (..), or some other multi-char operator ending in dot
                                if line.[pos-1] = ')' then
                                    // one very problematic case is someCall(args).Name
                                    // without special logic, we will decide that ). is an operator and parse Name as the plid
                                    // but in fact this is an expression tail, and we don't want a plid, rather we need to use expression typings at that location
                                    // so be sure not to treat the name here as a plid
                                    AtStartOfIdentifier(pos+1,[],true) // Throw away what we have, and the next apparent plid, and start over.
                                else
                                    AtStartOfIdentifier(pos+1,[],false) // Throw away what we have and start over.
                            else
                                AtStartOfIdentifier(pos+1,""::current,throwAwayNext)                            
                        else AtStartOfIdentifier(pos+1,[],throwAwayNext)
            let plid, residue = AtStartOfIdentifier(0,[],false)
            let plid = (List.rev plid)
            match plid with
            | s::_rest when s.Length > 0 && Char.IsDigit(s.[0]) -> ([],"")  // "2.0" is not a longId (this might not be right for ``2.0`` but good enough for common case)
            | _ -> plid, residue


    
    let TokenNameEquals (tokenInfo : FSharpTokenInfo) token2 = 
        String.Compare(tokenInfo .TokenName, token2, StringComparison.OrdinalIgnoreCase)=0  
    
    // The prefix of the sequence of token names to look for in TestMemberOrOverrideDeclaration, in reverse order
    let private expected = [ [|"dot"|]; [|"ident"|]; [|"member"; "override"|] ]

    /// Tests whether the user is typing something like "member x." or "override (*comment*) x."
    let internal TestMemberOrOverrideDeclaration (tokens:FSharpTokenInfo[]) =
        let filteredReversed = 
            tokens 
            |> Array.filter (fun tok ->
                // cut out whitespaces\comments\access modifiers
                not (TokenNameEquals tok "comment") && 
                not (TokenNameEquals tok "whitespace") &&
                not (TokenNameEquals tok "private") &&
                not (TokenNameEquals tok "internal") &&
                not (TokenNameEquals tok "public")
                ) 
            |> Array.rev
        
        if filteredReversed.Length < expected.Length then false
        else 
            // check whether sequences match
            (filteredReversed, expected) ||> Seq.forall2 (fun tok expect ->
                expect |> Array.exists (TokenNameEquals tok) )
                        
