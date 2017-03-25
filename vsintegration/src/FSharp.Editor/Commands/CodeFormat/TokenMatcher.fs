// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Microsoft.VisualStudio.FSharp.Editor.TokenMatcher

open System
open System.Collections.Generic
open System.Diagnostics
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.PrettyNaming
open Microsoft.FSharp.Compiler.SourceCodeServices

type Token = 
   | EOL
   | Tok of FSharpTokenInfo * int
   override x.ToString() =
        match x with
        | EOL -> "<EOL>"
        | Tok(tokInfo, l) ->
            sprintf "Tok(%O, %O)" tokInfo.TokenName l

let tokenize defines (content : string) =
    seq { 
        let sourceTokenizer = FSharpSourceTokenizer(defines, Some "/tmp.fsx")
        let lines = String.normalizeThenSplitNewLine content
        let lexState = ref 0L
        for (i, line) in lines |> Seq.zip [1..lines.Length] do 
            let lineTokenizer = sourceTokenizer.CreateLineTokenizer line
            let finLine = ref false
            while not !finLine do
                let tok, newLexState = lineTokenizer.ScanToken(!lexState)
                lexState := newLexState
                match tok with 
                | None ->
                    if i <> lines.Length then
                        // New line except at the very last token 
                        yield (EOL, Environment.NewLine) 
                    finLine := true
                | Some t -> 
                    yield (Tok(t, i), line.[t.LeftColumn..t.RightColumn]) 
    }

/// Create the view as if there is no attached line number
let (|Token|_|) = function
    | EOL -> None
    | Tok(ti, _) -> Some ti

// This part of the module takes care of annotating the AST with additional information
// about comments

/// Whitespace token without EOL
let (|Space|_|) = function
    | (Token origTok, origTokText) when origTok.TokenName = "WHITESPACE" -> 
        Some origTokText
    | _ -> None

let (|NewLine|_|) = function
    | (EOL, tokText) -> Some tokText
    | _ -> None

let (|WhiteSpaces|_|) = function 
    | Space t1 :: moreOrigTokens -> 
        let rec loop ts acc = 
            match ts with 
            | NewLine t2 :: ts2
            | Space t2 :: ts2 -> loop ts2 (t2 :: acc)
            | _ -> List.rev acc, ts
        Some (loop moreOrigTokens [t1])
    | _ -> None

let (|RawDelimiter|_|) = function
    | (Token origTok, origTokText) when origTok.CharClass = FSharpTokenCharKind.Delimiter -> 
        Some origTokText
    | _ -> None

let (|RawAttribute|_|) = function
    | RawDelimiter "[<" :: moreOrigTokens -> 
        let rec loop ts acc = 
            match ts with 
            | RawDelimiter ">]" :: ts2 -> Some (List.rev(">]" :: acc), ts2)
            | (_, t2) :: ts2 -> loop ts2 (t2 :: acc)
            | [] -> None
        loop moreOrigTokens ["[<"]
    | _ -> None

let (|Comment|_|) = function
    | (Token ti, t) 
      when ti.CharClass = FSharpTokenCharKind.Comment || ti.CharClass = FSharpTokenCharKind.LineComment -> 
        Some t
    | _ -> None

let (|CommentChunk|_|) = function
    | Comment t1 :: moreOrigTokens -> 
        let rec loop ts acc = 
            match ts with
            | NewLine t2 :: ts2
            | Comment t2 :: ts2
            | Space t2 :: ts2 -> loop ts2 (t2 :: acc)
            | _ -> List.rev acc, ts
        Some (loop moreOrigTokens [t1])
    | _ -> None

/// Get all comment chunks before a token 
let (|CommentChunks|_|) = function
    | CommentChunk(ts1, moreOrigTokens) -> 
        let rec loop ts acc = 
            match ts with 
            | WhiteSpaces(_, CommentChunk(ts2, ts')) ->
                // Just keep a newline between two comment chunks
                loop ts' (ts2 :: [Environment.NewLine] :: acc)
            | CommentChunk(ts2, ts') -> 
                loop ts' (ts2 :: acc)
            | _ -> (List.rev acc |> List.map (String.concat "")), ts
        Some (loop moreOrigTokens [ts1])
    | _ -> None      

/// Given a list of tokens, attach comments to appropriate positions
let collectComments tokens =
    let rec loop origTokens (dic : Dictionary<_, _>) =
        match origTokens with
        | (Token origTok, _) :: moreOrigTokens
            when origTok.CharClass <> FSharpTokenCharKind.Comment && origTok.CharClass <> FSharpTokenCharKind.LineComment ->
            loop moreOrigTokens dic
        | NewLine _ :: moreOrigTokens -> loop moreOrigTokens dic
        | CommentChunks(ts, WhiteSpaces(_, (Tok(origTok, lineNo), _) :: moreOrigTokens))
        | CommentChunks(ts, (Tok(origTok, lineNo), _) :: moreOrigTokens) ->
            dic.Add(mkPos lineNo origTok.LeftColumn, ts)
            loop moreOrigTokens dic
        | _ -> dic
    loop tokens (Dictionary())

let (|RawIdent|_|) = function
   | (Token ti, t) when ti.TokenName = "IDENT" -> 
        Some t
   | _ -> None

let (|SkipUntilIdent|_|) origTokens =
    let rec loop = function
        | RawIdent t :: moreOrigTokens -> Some(t, moreOrigTokens)
        | NewLine _ :: _ -> None
        | (Token ti, _) :: _ when ti.ColorClass = FSharpTokenColorKind.PreprocessorKeyword -> None
        | _ :: moreOrigTokens -> loop moreOrigTokens
        | [] -> None
    loop origTokens

let (|SkipUntilEOL|_|) origTokens =
    let rec loop = function
        | NewLine t :: moreOrigTokens -> Some(t, moreOrigTokens)
        | (Token ti, _) :: _ when ti.ColorClass = FSharpTokenColorKind.PreprocessorKeyword -> None
        | _ :: moreOrigTokens -> loop moreOrigTokens
        | [] -> None
    loop origTokens

/// Skip all whitespaces or comments in an active block
let (|SkipWhiteSpaceOrComment|_|) origTokens =
    let rec loop = function
        | Space _ :: moreOrigTokens
        | NewLine _ :: moreOrigTokens -> loop moreOrigTokens
        | (Token ti, _) :: moreOrigTokens 
            when ti.CharClass = FSharpTokenCharKind.Comment || ti.CharClass = FSharpTokenCharKind.LineComment ->
            loop moreOrigTokens
        | (Token ti, _) :: _ when ti.ColorClass = FSharpTokenColorKind.PreprocessorKeyword -> None
        | t :: moreOrigTokens -> Some(t, moreOrigTokens)
        | [] -> None
    loop origTokens

/// Filter all directives
let collectDirectives tokens =
    let rec loop origTokens (dic : Dictionary<_, _>)  = 
        match origTokens with 
        | (Token _, "#if") :: 
          SkipUntilIdent(t, SkipUntilEOL(_, SkipWhiteSpaceOrComment((Tok(origTok, lineNo), _), moreOrigTokens))) -> 
            dic.Add(mkPos lineNo origTok.LeftColumn, t) |> ignore
            loop moreOrigTokens dic
        | _ :: moreOrigTokens -> loop moreOrigTokens dic
        | [] -> dic
    loop tokens (Dictionary()) 

/// Filter all constants to be used in lexing
let filterConstants content =
    let rec loop origTokens (hs : HashSet<_>)  = 
        match origTokens with 
        | (Token _, "#if") :: 
          SkipUntilIdent(t, SkipUntilEOL(_, moreOrigTokens)) -> 
            hs.Add(t) |> ignore
            loop moreOrigTokens hs
        | _ :: moreOrigTokens -> loop moreOrigTokens hs
        | [] -> hs
    let hs = loop (tokenize [] content |> Seq.toList) (HashSet())
    Seq.toList hs

/// Filter all defined constants to be used in parsing
let filterDefines content =
    filterConstants content
    |> Seq.map (sprintf "--define:%s")
    |> Seq.toArray

/// Filter all comments and directives; assuming all constants are defined
let filterCommentsAndDirectives content =
    let constants = filterConstants content
    let tokens = tokenize constants content |> Seq.toList
    (collectComments tokens, collectDirectives tokens)

let rec (|RawLongIdent|_|) = function
   | RawIdent t1 :: RawDelimiter "." :: RawLongIdent(toks, moreOrigTokens) -> 
        Some (t1 :: "." :: toks, moreOrigTokens)
   | RawIdent t1 :: moreOrigTokens -> 
        Some ([t1], moreOrigTokens)
   | _ -> None

let (|RawOpenChunk|_|) = function
   | (Token _, "open") ::
     Space t ::
     RawLongIdent(toks, moreOrigTokens) -> 
        Some ("open" :: t :: toks, moreOrigTokens)
   | _ -> None

let (|NewTokenAfterWhitespaceOrNewLine|_|) toks = 
    let rec loop toks acc = 
        match toks with
        | (EOL, tt) :: more -> loop more (tt::acc)
        | (Token tok, tt) :: more 
           when tok.CharClass = FSharpTokenCharKind.WhiteSpace && tok.ColorClass <> FSharpTokenColorKind.InactiveCode 
                && tok.ColorClass <> FSharpTokenColorKind.PreprocessorKeyword -> 
            loop more (tt::acc)
        | newTok :: more -> 
            Some(List.rev acc, newTok, more)
        | [] -> None
    loop toks []

// This part processes the token stream post- pretty printing

type LineCommentStickiness = 
    | StickyLeft 
    | StickyRight 
    | NotApplicable
    override x.ToString() =
        match x with
        | StickyLeft -> "left"
        | StickyRight -> "right"
        | NotApplicable -> "unknown"

type MarkedToken = 
    | Marked of Token * string * LineCommentStickiness
    member x.Text = 
        let (Marked(_,t,_)) = x
        t
    override x.ToString() =
        let (Marked(tok, s, stickiness)) = x
        sprintf "Marked(%O, %A, %O)" tok s stickiness

/// Decompose a marked token to a raw token
let (|Wrapped|) (Marked(origTok, origTokText, _)) =
    (origTok, origTokText)

let (|SpaceToken|_|) = function
    | Wrapped(Space tokText) -> Some tokText
    | _ -> None

let (|NewLineToken|_|) = function
    | Wrapped(NewLine tokText) -> Some tokText
    | _ -> None

let (|WhiteSpaceTokens|_|) = function
   | SpaceToken t1 :: moreOrigTokens -> 
       let rec loop ts acc = 
           match ts with 
           | NewLineToken t2 :: ts2
           | SpaceToken t2 :: ts2 -> loop ts2 (t2 :: acc)
           | _ -> List.rev acc, ts
       Some (loop moreOrigTokens [t1])
   | _ -> None

let (|Delimiter|_|) = function
    | Wrapped(RawDelimiter tokText) -> Some tokText
    | _ -> None

let (|Attribute|_|) = function 
   | Delimiter "[<" :: moreOrigTokens -> 
       let rec loop ts acc = 
           match ts with 
           | Delimiter ">]" :: ts2 -> Some (List.rev(">]" :: acc), ts2)
           | Marked(_, t2, _) :: ts2 -> loop ts2 (t2 :: acc)
           | [] -> None
       loop moreOrigTokens ["[<"]
   | _ -> None

let (|PreprocessorKeywordToken|_|) requiredText = function
    | Marked(Token origTok, origTokText, _) 
        when origTok.ColorClass = FSharpTokenColorKind.PreprocessorKeyword && origTokText = requiredText -> 
        Some origTokText
    | _ -> None

let (|InactiveCodeToken|_|) = function
    | Marked(Token origTok, origTokText, _) 
        when origTok.ColorClass = FSharpTokenColorKind.InactiveCode -> Some origTokText
    | _ -> None

let (|LineCommentToken|_|) wantStickyLeft = function
    | Marked(Token origTok, origTokText, lcs) 
        when (not wantStickyLeft || (lcs = StickyLeft)) && 
             origTok.CharClass = FSharpTokenCharKind.LineComment -> Some origTokText
    | _ -> None

let (|BlockCommentToken|_|) = function
    | Marked(Token origTok, origTokText, _) when origTok.CharClass = FSharpTokenCharKind.Comment -> 
        Some origTokText
    | _ -> None

let (|BlockCommentOrNewLineToken|_|) = function
    | BlockCommentToken tokText -> Some tokText
    | NewLineToken tokText -> Some tokText
    | _ -> None

let (|LineCommentChunk|_|) wantStickyLeft = function
   | LineCommentToken wantStickyLeft t1 :: moreOrigTokens -> 
       let rec loop ts acc = 
           match ts with 
           | LineCommentToken false t2 :: ts2 -> loop ts2 (t2 :: acc)
           | _ -> List.rev acc, ts
       Some (loop moreOrigTokens [t1])
   | _ -> None

// TODO: does not cope with directives that have comments, e.g. 
//      #if (* hello *) FOOBAR
// or
//      #endif // FOOBAR
// or ones with extra whitespace at the end of line

let (|Ident|_|) = function
    | Wrapped(RawIdent tokText) -> Some tokText
    | _ -> None

let (|PreprocessorDirectiveChunk|_|) = function
   | PreprocessorKeywordToken "#if" t1 :: 
     SpaceToken t2 ::
     Ident t3 ::
     moreOrigTokens -> 
        Some ([t1; t2; t3], moreOrigTokens)

   | PreprocessorKeywordToken "#else" t1 :: moreOrigTokens ->
        Some ([t1], moreOrigTokens)

   | PreprocessorKeywordToken "#endif" t1 :: moreOrigTokens -> 
        Some ([t1], moreOrigTokens)

   | _ -> None

let (|InactiveCodeChunk|_|) = function
   | InactiveCodeToken t1 :: moreOrigTokens -> 
       let rec loop ts acc = 
           match ts with 
           | InactiveCodeToken t2 :: ts2 -> loop ts2 (t2 :: acc) 
           | NewLineToken t2 :: ts2 -> loop ts2 (t2 :: acc) 
           | _ -> List.rev acc, ts
       Some (loop moreOrigTokens [t1])
   | _ -> None

let (|BlockCommentChunk|_|) = function
   | BlockCommentToken t1 :: moreOrigTokens -> 
       let rec loop ts acc = 
           match ts with 
           | BlockCommentOrNewLineToken t2 :: ts2 -> loop ts2 (t2 :: acc)
           | _ -> List.rev acc, ts
       Some (loop moreOrigTokens [t1])
   | _ -> None  

/// Add a flag into the token stream indicating if the first token in 
/// the tokens of a line comment is sticky-to-the-left
///       text // comment
/// or sticky-to-the-right
///       // comment
///
let markStickiness (tokens: seq<Token * string>) = 
    seq { let inWhiteSpaceAtStartOfLine = ref true
          let inLineComment = ref false
          for (tio, tt) in tokens do 
             match tio with 
             | Token ti when ti.CharClass = FSharpTokenCharKind.LineComment ->
                  if !inLineComment then 
                      // Subsequent tokens in a line comment
                      yield Marked(tio, tt, NotApplicable)
                  else
                      // First token in a line comment. 
                      inLineComment := true
                      yield Marked(tio, tt, if !inWhiteSpaceAtStartOfLine then StickyRight else StickyLeft)
             
             // Comments can't be attached to Delimiters
             | Token ti 
                  when !inWhiteSpaceAtStartOfLine 
                       && (ti.CharClass = FSharpTokenCharKind.WhiteSpace || ti.CharClass = FSharpTokenCharKind.Delimiter) ->
                  // Whitespace at start of line
                  yield Marked(tio, tt, NotApplicable)
             | Tok _ ->
                  // Some other token on a line
                  inWhiteSpaceAtStartOfLine := false
                  yield Marked(tio, tt, NotApplicable)
             | EOL -> 
                  // End of line marker
                 inLineComment := false
                 inWhiteSpaceAtStartOfLine := true
                 yield Marked(tio, tt, NotApplicable) }

let rec (|LongIdent|_|) = function
   | Ident t1 :: Delimiter "." :: LongIdent(toks, moreOrigTokens) -> 
        Some (t1 :: "." :: toks, moreOrigTokens)
   | Ident t1 :: moreOrigTokens -> 
        Some ([t1], moreOrigTokens)
   | _ -> None

let (|OpenChunk|_|) = function
   | Marked(Token _, "open", _) ::
     SpaceToken t ::
     LongIdent(toks, moreOrigTokens) -> 
        Some ("open" :: t :: toks, moreOrigTokens)
   | _ -> None
 
/// Assume that originalText and newText are derived from the same AST. 
/// Pick all comments and directives from originalText to insert into newText               
let integrateComments (originalText : string) (newText : string) =
    let origTokens = tokenize (filterConstants originalText) originalText |> markStickiness |> Seq.toList
    //Seq.iter (fun (Marked(_, s, t)) -> Console.WriteLine("sticky information: {0} -- {1}", s, t)) origTokens
    let newTokens = tokenize [] newText |> Seq.toList

    let buffer = System.Text.StringBuilder()
    let column = ref 0
    let indent = ref 0

    let addText (text : string) = 
        //Debug.WriteLine("ADDING '{0}'", text)
        buffer.Append text |> ignore
        if text = Environment.NewLine then column := 0
        else column := !column + text.Length

    let maintainIndent f =  
        let c = !column
        f()
        Debug.WriteLine("maintain indent at {0}", c)
        addText Environment.NewLine
        addText (String.replicate c " ")

    let saveIndent c =
        indent := c

    let restoreIndent f =
        let c = !indent
        Debug.WriteLine("set indent back to {0}", c)
        addText Environment.NewLine
        addText (String.replicate c " ")
        f()

    // Assume that starting whitespaces after EOL give indentation of a chunk
    let rec getIndent = function
        | (Token _, _) :: moreNewTokens -> getIndent moreNewTokens
        | NewLine _ :: moreNewTokens ->
            match moreNewTokens with
            | Space origTokText :: _ -> String.length origTokText
            | _ -> 0
        | _ -> 0
        
    let countStartingSpaces (lines: string []) = 
        if lines.Length = 0 then 0
        else
            Seq.min [ for line in lines -> line.Length - line.TrimStart(' ').Length ]

    let tokensMatch t1 t2 = 
        match t1, t2 with 
        | Marked(Token origTok, origTokText, _), (Token newTok, newTokText) -> 
            origTok.CharClass = newTok.CharClass && origTokText = newTokText
        // Use this pattern to avoid discrepancy between two versions of the same identifier
        | Ident origTokText, RawIdent newTokText ->
            DecompileOpName(origTokText.Trim('`')) = DecompileOpName(newTokText.Trim('`'))
        | _ -> false

    let rec loop origTokens newTokens = 
        //Debug.WriteLine("*** Matching between {0} and {1}", sprintf "%A" <| tryHead origTokens, sprintf "%A" <| tryHead newTokens)
        match origTokens, newTokens with 
        | (Marked(Token origTok, _, _) :: moreOrigTokens),  _ 
            when origTok.CharClass = FSharpTokenCharKind.WhiteSpace && origTok.ColorClass <> FSharpTokenColorKind.InactiveCode 
                && origTok.ColorClass <> FSharpTokenColorKind.PreprocessorKeyword ->
            Debug.WriteLine "dropping whitespace from orig tokens" 
            loop moreOrigTokens newTokens 

        | (NewLineToken _ :: moreOrigTokens), _ ->
            Debug.WriteLine "dropping newline from orig tokens" 
            loop moreOrigTokens newTokens
        
        // Not a comment, drop the original token text until something matches
        | (Delimiter tokText :: moreOrigTokens), _ when tokText = ";" || tokText = ";;" ->
            Debug.WriteLine("dropping '{0}' from original text", box tokText)
            loop moreOrigTokens newTokens 

        // Inject #if... #else or #endif directive
        // These directives could occur inside an inactive code chunk
        // Assume that only #endif directive follows by an EOL 
        | (PreprocessorDirectiveChunk (tokensText, moreOrigTokens)), newTokens ->            
            let text = String.concat "" tokensText
            Debug.WriteLine("injecting preprocessor directive '{0}'", box text)
            addText Environment.NewLine
            for x in tokensText do addText x
            let moreNewTokens =
                if String.startsWithOrdinal "#endif" text then
                    match newTokens with
                    | WhiteSpaces(ws, moreNewTokens) ->
                        // There are some whitespaces, use them up
                        for s in ws do addText s
                        moreNewTokens
                    | _ :: _ ->
                        // This fixes the case where newTokens advance too fast
                        // and emit whitespaces even before #endif 
                        restoreIndent id
                        newTokens
                    | [] -> []
                elif String.startsWithOrdinal "#if" text then
                    // Save current indentation for #else branch
                    let indent = getIndent newTokens 
                    saveIndent indent
                    newTokens
                else newTokens
            match moreNewTokens with
            | (Token t, _) :: _ when t.ColorClass = FSharpTokenColorKind.PreprocessorKeyword -> addText Environment.NewLine
            | _ -> ()
            loop moreOrigTokens moreNewTokens

        // Inject inactive code
        // These chunks come out from any #else branch in our scenarios
        | (InactiveCodeChunk (tokensText, moreOrigTokens)),  _ ->
            Debug.WriteLine("injecting inactive code '{0}'", String.concat "" tokensText |> box)
            let text = String.concat "" tokensText
            let lines = (String.normalizeNewLine text).Split([|'\n'|], StringSplitOptions.RemoveEmptyEntries)
            // What is current indentation of this chunk
            let numSpaces = countStartingSpaces lines
            Debug.WriteLine("the number of starting spaces is {0}", numSpaces)
            // Write the chunk in the same indentation with #if branch
            for line in lines do
                if String.startsWithOrdinal "#" line.[numSpaces..] then
                    // Naive recognition of inactive preprocessors
                    addText Environment.NewLine
                    addText line.[numSpaces..]
                else
                    restoreIndent (fun () -> addText line.[numSpaces..])
            loop moreOrigTokens newTokens

        | (LineCommentChunk true (commentTokensText, moreOrigTokens)), [] ->
            Debug.WriteLine("injecting the last stick-to-the-left line comment '{0}'", String.concat "" commentTokensText |> box)
            addText " "
            for x in commentTokensText do addText x
            loop moreOrigTokens newTokens 

        // Inject line commment that is sticky-to-the-left, e.g. 
        //   let f x = 
        //       x + x  // HERE
        // Because it is sticky-to-the-left, we do it _before_ emitting end-of-line from the newText        
        | (LineCommentChunk true (commentTokensText, moreOrigTokens)),  _ ->
            let tokText = String.concat "" commentTokensText
            Debug.WriteLine("injecting sticky-to-the-left line comment '{0}'", box tokText)
              
            match newTokens with 
            // If there is a new line coming, use it up
            | Space _ :: (EOL, newTokText) :: moreNewTokens | (EOL, newTokText) :: moreNewTokens ->
                addText " "
                for x in commentTokensText do addText x
                Debug.WriteLine "emitting newline for end of sticky-to-left comment" 
                addText newTokText 
                loop moreOrigTokens moreNewTokens 
            // Otherwise, skip a whitespace token and maintain the indentation
            | Space _ :: moreNewTokens | moreNewTokens -> 
                addText " "
                maintainIndent (fun () -> 
                    for x in commentTokensText do addText x)
                loop moreOrigTokens moreNewTokens 

        // Emit end-of-line from new tokens
        | _,  (NewLine newTokText :: moreNewTokens) ->
            Debug.WriteLine("emitting newline in new tokens '{0}'", newTokText)
            addText newTokText 
            loop origTokens moreNewTokens 

        | _,  ((Token newTok, newTokText) :: moreNewTokens) 
            when newTok.CharClass = FSharpTokenCharKind.WhiteSpace && newTok.ColorClass <> FSharpTokenColorKind.InactiveCode ->
            Debug.WriteLine("emitting whitespace '{0}' in new tokens", newTokText |> box)
            addText newTokText 
            loop origTokens moreNewTokens 

        | (Delimiter tokText :: newTokens), (RawDelimiter newTokText :: moreNewTokens) 
            when tokText = newTokText && newTokText <> "[<" && newTokText <> ">]" && newTokText <> "|" ->
            Debug.WriteLine("emitting matching delimiter '{0}' in new tokens", newTokText |> box)
            addText newTokText 
            loop newTokens moreNewTokens 

        // Emit all unmatched RawDelimiter tokens
        | _, (RawDelimiter newTokText :: moreNewTokens) 
            when newTokText <> "[<" && newTokText <> ">]" && newTokText <> "|" ->
            Debug.WriteLine("emitting non-matching '{0}' in new tokens", newTokText |> box)
            addText newTokText 
            loop origTokens moreNewTokens 

        // Process the last line or block comments
        | (LineCommentChunk false (commentTokensText, moreOrigTokens)), []
        | (BlockCommentChunk (commentTokensText, moreOrigTokens)), [] ->
            Debug.WriteLine("injecting the last line or block comment '{0}'", String.concat "" commentTokensText |> box)
            // Until block comments can't have new line in the beginning, add two consecutive new lines
            addText Environment.NewLine
            for x in commentTokensText do addText x
            loop moreOrigTokens newTokens 

        // Inject line commment, after all whitespace and newlines emitted, so
        // the line comment will appear just before the subsequent text, e.g. 
        //   let f x = 
        //       // HERE
        //       x + x
        | (LineCommentChunk false (commentTokensText, moreOrigTokens)),  _ ->
            Debug.WriteLine("injecting line comment '{0}'", String.concat "" commentTokensText |> box)
            maintainIndent (fun () -> for x in commentTokensText do addText x)
            loop moreOrigTokens newTokens 

        // Inject block commment 
        | (BlockCommentChunk (commentTokensText, moreOrigTokens)),  _ ->
            Debug.WriteLine("injecting block comment '{0}'", String.concat "" commentTokensText |> box)
            let comments = String.concat "" commentTokensText
            if comments.IndexOf('\n') = -1 then
                // This is an inline block comment
                addText comments
                addText " "
            else
                let len = List.length commentTokensText
                maintainIndent (fun () -> 
                    commentTokensText |> List.iteri (fun i x ->
                        // Drop the last newline 
                        if i = len - 1 && x = Environment.NewLine then ()
                        else addText x))
            loop moreOrigTokens newTokens 

        // Consume attributes in the new text
        | _, RawAttribute(newTokensText, moreNewTokens) ->
            Debug.WriteLine("no matching of attribute tokens")
            for x in newTokensText do addText x
            loop origTokens moreNewTokens
              
        // Skip attributes in the old text
        | (Attribute (tokensText, moreOrigTokens)), _ ->
            Debug.WriteLine("skip matching of attribute tokens '{0}'", box tokensText)
            loop moreOrigTokens newTokens
         
        // Open declarations may be reordered, so we match them even if two identifiers are different
        | OpenChunk(tokensText, moreOrigTokens), RawOpenChunk(newTokensText, moreNewTokens) ->
            Debug.WriteLine("matching two open chunks '{0}'", String.concat "" tokensText |> box)
            for x in newTokensText do addText x
            loop moreOrigTokens moreNewTokens    

        // Matching tokens
        | (origTok :: moreOrigTokens), (newTok :: moreNewTokens) when tokensMatch origTok newTok ->
            Debug.WriteLine("matching token '{0}'", box origTok.Text)
            addText (snd newTok)
            loop moreOrigTokens moreNewTokens 

        // Matching tokens, after one new token, compensating for insertions of "|", ";" and others
        | (origTok :: moreOrigTokens), (newTok1 :: NewTokenAfterWhitespaceOrNewLine(whiteTokens, newTok2, moreNewTokens)) 
            when tokensMatch origTok newTok2 ->
            Debug.WriteLine("fresh non-matching new token '{0}'", snd newTok1 |> box)
            addText (snd newTok1)
            Debug.WriteLine("matching token '{0}' (after one fresh new token)", snd newTok2 |> box)
            for x in whiteTokens do addText x
            addText (snd newTok2)
            loop moreOrigTokens moreNewTokens 

        // Not a comment, drop the original token text until something matches
        | (origTok :: moreOrigTokens), _ ->
            Debug.WriteLine("dropping '{0}' from original text", box origTok.Text)
            loop moreOrigTokens newTokens 

        // Dangling text at the end 
        | [], ((_, newTokText) :: moreNewTokens) ->
            Debug.WriteLine("dangling new token '{0}'", box newTokText)
            addText newTokText 
            loop [] moreNewTokens 

        // Dangling input text - extra comments or whitespace
        | (Marked(origTok, origTokText, _) :: moreOrigTokens), [] ->
            Debug.WriteLine("dropping dangling old token '{0}'", box origTokText)
            loop moreOrigTokens [] 

        | [], [] -> 
            ()

    loop origTokens newTokens 
    buffer.ToString()
