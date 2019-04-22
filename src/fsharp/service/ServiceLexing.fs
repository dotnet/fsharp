// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// Open up the compiler as an incremental service for lexing.
//--------------------------------------------------------------------------

namespace FSharp.Compiler.SourceCodeServices

open System
open System.Collections.Generic
open FSharp.Compiler.AbstractIL.Internal
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler
open FSharp.Compiler.Parser
open FSharp.Compiler.Range
open FSharp.Compiler.Ast
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.Lexhelp
open FSharp.Compiler.Lib
open Internal.Utilities

type Position = int * int
type Range = Position * Position

module FSharpTokenTag =
    let Identifier = tagOfToken (IDENT "a")
    let String = tagOfToken (STRING "a")

    let IDENT = tagOfToken (IDENT "a")
    let STRING = tagOfToken (STRING "a")
    let LPAREN = tagOfToken LPAREN
    let RPAREN = tagOfToken RPAREN
    let LBRACK = tagOfToken LBRACK
    let RBRACK = tagOfToken RBRACK
    let LBRACE = tagOfToken LBRACE
    let RBRACE = tagOfToken RBRACE
    let LBRACK_LESS = tagOfToken LBRACK_LESS
    let GREATER_RBRACK = tagOfToken GREATER_RBRACK
    let LESS = tagOfToken (LESS true)
    let GREATER = tagOfToken (GREATER true)
    let LBRACK_BAR = tagOfToken LBRACK_BAR
    let BAR_RBRACK = tagOfToken BAR_RBRACK
    let PLUS_MINUS_OP = tagOfToken (PLUS_MINUS_OP "a")
    let MINUS = tagOfToken MINUS
    let STAR = tagOfToken STAR
    let INFIX_STAR_DIV_MOD_OP = tagOfToken (INFIX_STAR_DIV_MOD_OP "a")
    let PERCENT_OP = tagOfToken (PERCENT_OP "a")
    let INFIX_AT_HAT_OP = tagOfToken (INFIX_AT_HAT_OP "a")
    let QMARK = tagOfToken QMARK
    let COLON = tagOfToken COLON
    let EQUALS = tagOfToken EQUALS
    let SEMICOLON = tagOfToken SEMICOLON
    let COMMA = tagOfToken COMMA
    let DOT = tagOfToken DOT
    let DOT_DOT = tagOfToken DOT_DOT
    let INT32_DOT_DOT = tagOfToken (INT32_DOT_DOT(0, true))
    let UNDERSCORE = tagOfToken UNDERSCORE
    let BAR = tagOfToken BAR
    let COLON_GREATER = tagOfToken COLON_GREATER
    let COLON_QMARK_GREATER = tagOfToken COLON_QMARK_GREATER
    let COLON_QMARK = tagOfToken COLON_QMARK
    let INFIX_BAR_OP = tagOfToken (INFIX_BAR_OP "a")
    let INFIX_COMPARE_OP = tagOfToken (INFIX_COMPARE_OP "a")
    let COLON_COLON = tagOfToken COLON_COLON
    let AMP_AMP = tagOfToken AMP_AMP
    let PREFIX_OP = tagOfToken (PREFIX_OP "a")
    let COLON_EQUALS = tagOfToken COLON_EQUALS
    let BAR_BAR = tagOfToken BAR_BAR
    let RARROW = tagOfToken RARROW
    let LARROW = tagOfToken LARROW
    let QUOTE = tagOfToken QUOTE
    let WHITESPACE = tagOfToken (WHITESPACE Unchecked.defaultof<_>)
    let COMMENT = tagOfToken (COMMENT Unchecked.defaultof<_>)
    let LINE_COMMENT = tagOfToken (LINE_COMMENT Unchecked.defaultof<_>)
    let BEGIN = tagOfToken BEGIN
    let DO = tagOfToken DO
    let FUNCTION = tagOfToken FUNCTION
    let THEN = tagOfToken THEN
    let ELSE = tagOfToken ELSE
    let STRUCT = tagOfToken STRUCT
    let CLASS = tagOfToken CLASS
    let TRY = tagOfToken TRY
    let NEW = tagOfToken NEW
    let WITH = tagOfToken WITH
    let OWITH = tagOfToken OWITH


/// This corresponds to a token categorization originally used in Visual Studio 2003.
///
/// NOTE: This corresponds to a token categorization originally used in Visual Studio 2003 and the original Babel source code.
/// It is not clear it is a primary logical classification that should be being used in the
/// more recent language service work.
type FSharpTokenColorKind =
    | Default = 0
    | Text = 0
    | Keyword = 1
    | Comment = 2
    | Identifier = 3
    | String = 4
    | UpperIdentifier = 5
    | InactiveCode = 7
    | PreprocessorKeyword = 8
    | Number = 9
    | Operator = 10
    | Punctuation = 11

/// Categorize an action the editor should take in response to a token, e.g. brace matching
///
/// NOTE: This corresponds to a token categorization originally used in Visual Studio 2003 and the original Babel source code.
/// It is not clear it is a primary logical classification that should be being used in the
/// more recent language service work.
type FSharpTokenTriggerClass =
    | None         = 0x00000000
    | MemberSelect = 0x00000001
    | MatchBraces  = 0x00000002
    | ChoiceSelect = 0x00000004
    | MethodTip    = 0x000000F0
    | ParamStart   = 0x00000010
    | ParamNext    = 0x00000020
    | ParamEnd     = 0x00000040


/// This corresponds to a token categorization originally used in Visual Studio 2003.
///
/// NOTE: This corresponds to a token categorization originally used in Visual Studio 2003 and the original Babel source code.
/// It is not clear it is a primary logical classification that should be being used in the
/// more recent language service work.
type FSharpTokenCharKind =
    | Default     = 0x00000000
    | Text        = 0x00000000
    | Keyword     = 0x00000001
    | Identifier  = 0x00000002
    | String      = 0x00000003
    | Literal     = 0x00000004
    | Operator    = 0x00000005
    | Delimiter   = 0x00000006
    | WhiteSpace  = 0x00000008
    | LineComment = 0x00000009
    | Comment     = 0x0000000A


/// Information about a particular token from the tokenizer
type FSharpTokenInfo = {
    LeftColumn: int
    RightColumn: int
    ColorClass: FSharpTokenColorKind
    CharClass: FSharpTokenCharKind
    FSharpTokenTriggerClass: FSharpTokenTriggerClass
    Tag: int
    TokenName: string
    FullMatchedLength: int }

//----------------------------------------------------------------------------
// Babel flags
//--------------------------------------------------------------------------

module internal TokenClassifications =

    //----------------------------------------------------------------------------
    //From tokens to flags
    //--------------------------------------------------------------------------

    let tokenInfo token =
        match token with
        | IDENT s ->
            if s.Length <= 0 then
                System.Diagnostics.Debug.Assert(false, "BUG: Received zero length IDENT token.")
                // This is related to 4783. Recover by treating as lower case identifier.
                (FSharpTokenColorKind.Identifier, FSharpTokenCharKind.Identifier, FSharpTokenTriggerClass.None)
            else
                if System.Char.ToUpperInvariant s.[0] = s.[0] then
                    (FSharpTokenColorKind.UpperIdentifier, FSharpTokenCharKind.Identifier, FSharpTokenTriggerClass.None)
                else
                    (FSharpTokenColorKind.Identifier, FSharpTokenCharKind.Identifier, FSharpTokenTriggerClass.None)

        // 'in' when used in a 'join' in a query expression
        | JOIN_IN ->
            (FSharpTokenColorKind.Identifier, FSharpTokenCharKind.Identifier, FSharpTokenTriggerClass.None)
        | DECIMAL _
        | BIGNUM _ | INT8 _  | UINT8 _ | INT16 _  | UINT16 _ | INT32 _ | UINT32 _ | INT64 _ | UINT64 _
        | UNATIVEINT _ | NATIVEINT _ | IEEE32 _ |  IEEE64 _ ->
            (FSharpTokenColorKind.Number, FSharpTokenCharKind.Literal, FSharpTokenTriggerClass.None)

        | INT32_DOT_DOT _ ->
          // This will color the whole "1.." expression in a 'number' color
          // (this isn't entirely correct, but it'll work for now - see bug 3727)
            (FSharpTokenColorKind.Number, FSharpTokenCharKind.Operator, FSharpTokenTriggerClass.None)

        | INFIX_STAR_DIV_MOD_OP ("mod"  | "land" |  "lor" | "lxor")
        | INFIX_STAR_STAR_OP ("lsl" | "lsr" | "asr") ->
            (FSharpTokenColorKind.Keyword, FSharpTokenCharKind.Keyword, FSharpTokenTriggerClass.None)

        | LPAREN_STAR_RPAREN
        | DOLLAR | COLON_GREATER  | COLON_COLON
        | PERCENT_OP _ | PLUS_MINUS_OP _ | PREFIX_OP _ | COLON_QMARK_GREATER
        | AMP   | AMP_AMP  | BAR_BAR  | QMARK | QMARK_QMARK | COLON_QMARK
        | HIGH_PRECEDENCE_TYAPP
        | COLON_EQUALS   | EQUALS | RQUOTE_DOT _
        | MINUS | ADJACENT_PREFIX_OP _ ->
            (FSharpTokenColorKind.Operator, FSharpTokenCharKind.Operator, FSharpTokenTriggerClass.None)

        | INFIX_COMPARE_OP _ // This is a whole family: .< .> .= .!= .$
        | FUNKY_OPERATOR_NAME _ // This is another whole family, including: .[] and .()
        //| INFIX_AT_HAT_OP _
        | INFIX_STAR_STAR_OP _
        | INFIX_AMP_OP _
        | INFIX_BAR_OP _
        | INFIX_STAR_DIV_MOD_OP _
        | INFIX_AMP_OP _ ->
                (FSharpTokenColorKind.Operator, FSharpTokenCharKind.Operator, FSharpTokenTriggerClass.None)

        | DOT_DOT ->
            (FSharpTokenColorKind.Operator, FSharpTokenCharKind.Operator, FSharpTokenTriggerClass.MemberSelect)

        | COMMA ->
            (FSharpTokenColorKind.Punctuation, FSharpTokenCharKind.Delimiter, FSharpTokenTriggerClass.ParamNext)

        | DOT ->
            (FSharpTokenColorKind.Punctuation, FSharpTokenCharKind.Delimiter, FSharpTokenTriggerClass.MemberSelect)

        | BAR ->
            (FSharpTokenColorKind.Punctuation, FSharpTokenCharKind.Delimiter, FSharpTokenTriggerClass.None (* FSharpTokenTriggerClass.ChoiceSelect *))

        | HASH | STAR | SEMICOLON  | SEMICOLON_SEMICOLON | COLON ->
            (FSharpTokenColorKind.Punctuation, FSharpTokenCharKind.Delimiter, FSharpTokenTriggerClass.None)

        | QUOTE | UNDERSCORE
        | INFIX_AT_HAT_OP _ ->
            (FSharpTokenColorKind.Identifier, FSharpTokenCharKind.Identifier, FSharpTokenTriggerClass.None)

        | LESS  _ ->
            (FSharpTokenColorKind.Punctuation, FSharpTokenCharKind.Operator, FSharpTokenTriggerClass.ParamStart)  // for type provider static arguments

        | GREATER _ ->
            (FSharpTokenColorKind.Punctuation, FSharpTokenCharKind.Operator, FSharpTokenTriggerClass.ParamEnd)    // for type provider static arguments

        | LPAREN ->
            // We need 'ParamStart' to trigger the 'GetDeclarations' method to show param info automatically
            // this is needed even if we don't use MPF for determining information about params
            (FSharpTokenColorKind.Punctuation, FSharpTokenCharKind.Delimiter, FSharpTokenTriggerClass.ParamStart ||| FSharpTokenTriggerClass.MatchBraces)

        | RPAREN | RPAREN_COMING_SOON | RPAREN_IS_HERE ->
            (FSharpTokenColorKind.Punctuation, FSharpTokenCharKind.Delimiter, FSharpTokenTriggerClass.ParamEnd ||| FSharpTokenTriggerClass.MatchBraces)

        | LBRACK_LESS ->
            (FSharpTokenColorKind.Punctuation, FSharpTokenCharKind.Delimiter, FSharpTokenTriggerClass.None )

        | LQUOTE _  | LBRACK  | LBRACE | LBRACK_BAR | LBRACE_BAR ->
            (FSharpTokenColorKind.Punctuation, FSharpTokenCharKind.Delimiter, FSharpTokenTriggerClass.MatchBraces )

        | GREATER_RBRACK  | GREATER_BAR_RBRACK ->
            (FSharpTokenColorKind.Punctuation, FSharpTokenCharKind.Delimiter, FSharpTokenTriggerClass.None )

        | RQUOTE _  | RBRACK  | RBRACE | RBRACE_COMING_SOON | RBRACE_IS_HERE | BAR_RBRACK | BAR_RBRACE ->
            (FSharpTokenColorKind.Punctuation, FSharpTokenCharKind.Delimiter, FSharpTokenTriggerClass.MatchBraces )

        | PUBLIC | PRIVATE | INTERNAL | BASE | GLOBAL
        | CONSTRAINT | INSTANCE | DELEGATE | INHERIT|CONSTRUCTOR|DEFAULT|OVERRIDE|ABSTRACT|CLASS
        | MEMBER | STATIC | NAMESPACE
        | OASSERT | OLAZY | ODECLEND | OBLOCKSEP | OEND | OBLOCKBEGIN | ORIGHT_BLOCK_END
        | OBLOCKEND | OBLOCKEND_COMING_SOON | OBLOCKEND_IS_HERE | OTHEN | OELSE | OLET(_)
        | OBINDER _ | BINDER _ | ODO | OWITH | OFUNCTION | OFUN | ORESET | ODUMMY _ | DO_BANG
        | ODO_BANG | YIELD _ | YIELD_BANG  _ | OINTERFACE_MEMBER
        | ELIF | RARROW | LARROW | SIG | STRUCT
        | UPCAST   | DOWNCAST   | NULL   | RESERVED    | MODULE    | AND    | AS   | ASSERT   | ASR
        | DOWNTO   | EXCEPTION   | FALSE   | FOR   | FUN   | FUNCTION
        | FINALLY   | LAZY   | MATCH  | MATCH_BANG  | MUTABLE   | NEW   | OF    | OPEN   | OR | VOID | EXTERN
        | INTERFACE | REC   | TO   | TRUE   | TRY   | TYPE   |  VAL   | INLINE   | WHEN  | WHILE   | WITH
        | IF | THEN  | ELSE | DO | DONE | LET(_) | IN (*| NAMESPACE*) | CONST
        | HIGH_PRECEDENCE_PAREN_APP | FIXED
        | HIGH_PRECEDENCE_BRACK_APP
        | TYPE_COMING_SOON | TYPE_IS_HERE | MODULE_COMING_SOON | MODULE_IS_HERE ->
            (FSharpTokenColorKind.Keyword, FSharpTokenCharKind.Keyword, FSharpTokenTriggerClass.None)

        | BEGIN ->
            (FSharpTokenColorKind.Keyword, FSharpTokenCharKind.Keyword, FSharpTokenTriggerClass.None)

        | END ->
            (FSharpTokenColorKind.Keyword, FSharpTokenCharKind.Keyword, FSharpTokenTriggerClass.None)

        | HASH_LIGHT _
        | HASH_LINE _
        | HASH_IF _
        | HASH_ELSE _
        | HASH_ENDIF _ ->
            (FSharpTokenColorKind.PreprocessorKeyword, FSharpTokenCharKind.WhiteSpace, FSharpTokenTriggerClass.None)

        | INACTIVECODE _ ->
            (FSharpTokenColorKind.InactiveCode, FSharpTokenCharKind.WhiteSpace, FSharpTokenTriggerClass.None)


        | LEX_FAILURE _
        | WHITESPACE _ ->
            (FSharpTokenColorKind.Default, FSharpTokenCharKind.WhiteSpace, FSharpTokenTriggerClass.None)

        | COMMENT _ ->
            (FSharpTokenColorKind.Comment, FSharpTokenCharKind.Comment, FSharpTokenTriggerClass.None)

        | LINE_COMMENT _ ->
            (FSharpTokenColorKind.Comment, FSharpTokenCharKind.LineComment, FSharpTokenTriggerClass.None)

        | STRING_TEXT _ ->
            (FSharpTokenColorKind.String, FSharpTokenCharKind.String, FSharpTokenTriggerClass.None)

        | KEYWORD_STRING _ ->
           (FSharpTokenColorKind.Keyword, FSharpTokenCharKind.Keyword, FSharpTokenTriggerClass.None)

        | BYTEARRAY _ | STRING  _
        | CHAR _ ->
            (FSharpTokenColorKind.String, FSharpTokenCharKind.String, FSharpTokenTriggerClass.None)

        | EOF _ -> failwith "tokenInfo"

module internal TestExpose =
  let TokenInfo tok = TokenClassifications.tokenInfo tok

    //----------------------------------------------------------------------------
    // Lexer states encoded to/from integers
    //--------------------------------------------------------------------------
[<Struct; CustomEquality; NoComparison>]
type FSharpTokenizerLexState =
    { PosBits: int64
      OtherBits: int64 }
    static member Initial = { PosBits = 0L; OtherBits = 0L }
    member this.Equals (other: FSharpTokenizerLexState) = (this.PosBits = other.PosBits) && (this.OtherBits = other.OtherBits)
    override this.Equals (obj: obj) = match obj with :? FSharpTokenizerLexState as other -> this.Equals other | _ -> false
    override this.GetHashCode () = hash this.PosBits + hash this.OtherBits

type FSharpTokenizerColorState =
    | Token = 1
    | IfDefSkip = 3
    | String = 4
    | Comment = 5
    | StringInComment = 6
    | VerbatimStringInComment = 7
    | CamlOnly = 8
    | VerbatimString = 9
    | SingleLineComment = 10
    | EndLineThenSkip = 11
    | EndLineThenToken = 12
    | TripleQuoteString = 13
    | TripleQuoteStringInComment = 14
    | InitialState = 0


module internal LexerStateEncoding =

    let computeNextLexState token (prevLexcont: LexerWhitespaceContinuation) =
      match token with
      | HASH_LINE s
      | HASH_LIGHT s
      | HASH_IF(_, _, s)
      | HASH_ELSE(_, _, s)
      | HASH_ENDIF(_, _, s)
      | INACTIVECODE s
      | WHITESPACE s
      | COMMENT s
      | LINE_COMMENT s
      | STRING_TEXT s
      | EOF s -> s
      | BYTEARRAY _ | STRING _ -> LexCont.Token(prevLexcont.LexerIfdefStack)
      | _ -> prevLexcont

    // Note that this will discard all lexcont state, including the ifdefStack.
    let revertToDefaultLexCont = LexCont.Token []

    let lexstateNumBits = 4
    let ncommentsNumBits = 4
    let hardwhiteNumBits = 1
    let ifdefstackCountNumBits = 8
    let ifdefstackNumBits = 24           // 0 means if, 1 means else
    let _ = assert (lexstateNumBits
                    + ncommentsNumBits
                    + hardwhiteNumBits
                    + ifdefstackCountNumBits
                    + ifdefstackNumBits <= 64)

    let lexstateStart         = 0
    let ncommentsStart        = lexstateNumBits
    let hardwhitePosStart     = lexstateNumBits+ncommentsNumBits
    let ifdefstackCountStart  = lexstateNumBits+ncommentsNumBits+hardwhiteNumBits
    let ifdefstackStart       = lexstateNumBits+ncommentsNumBits+hardwhiteNumBits+ifdefstackCountNumBits

    let lexstateMask          = Bits.mask64 lexstateStart lexstateNumBits
    let ncommentsMask         = Bits.mask64 ncommentsStart ncommentsNumBits
    let hardwhitePosMask      = Bits.mask64 hardwhitePosStart hardwhiteNumBits
    let ifdefstackCountMask   = Bits.mask64 ifdefstackCountStart ifdefstackCountNumBits
    let ifdefstackMask        = Bits.mask64 ifdefstackStart ifdefstackNumBits

    let bitOfBool b = if b then 1 else 0
    let boolOfBit n = (n = 1L)

    let inline colorStateOfLexState (state: FSharpTokenizerLexState) =
        enum<FSharpTokenizerColorState> (int32 ((state.OtherBits &&& lexstateMask) >>> lexstateStart))

    let inline lexStateOfColorState (state: FSharpTokenizerColorState) =
        (int64 state <<< lexstateStart) &&& lexstateMask

    let encodeLexCont (colorState: FSharpTokenizerColorState) ncomments (b: pos) ifdefStack light =
        let mutable ifdefStackCount = 0
        let mutable ifdefStackBits = 0
        for ifOrElse in ifdefStack do
            match ifOrElse with
                | (IfDefIf, _) -> ()
                | (IfDefElse, _) ->
                    ifdefStackBits <- (ifdefStackBits ||| (1 <<< ifdefStackCount))
            ifdefStackCount <- ifdefStackCount + 1

        let bits =
            lexStateOfColorState colorState
            ||| ((ncomments <<< ncommentsStart) &&& ncommentsMask)
            ||| ((int64 (bitOfBool light) <<< hardwhitePosStart) &&& hardwhitePosMask)
            ||| ((int64 ifdefStackCount <<< ifdefstackCountStart) &&& ifdefstackCountMask)
            ||| ((int64 ifdefStackBits <<< ifdefstackStart) &&& ifdefstackMask)
        { PosBits = b.Encoding
          OtherBits = bits }


    let decodeLexCont (state: FSharpTokenizerLexState) =
        let mutable ifDefs = []
        let bits = state.OtherBits
        let ifdefStackCount = int32 ((bits &&& ifdefstackCountMask) >>> ifdefstackCountStart)
        if ifdefStackCount>0 then
            let ifdefStack = int32 ((bits &&& ifdefstackMask) >>> ifdefstackStart)
            for i in 1..ifdefStackCount do
                let bit = ifdefStackCount-i
                let mask = 1 <<< bit
                let ifDef = (if ifdefStack &&& mask = 0 then IfDefIf else IfDefElse)
                ifDefs <- (ifDef, range0) :: ifDefs
        colorStateOfLexState state,
        int32 ((bits &&& ncommentsMask) >>> ncommentsStart),
        pos.Decode state.PosBits,
        ifDefs,
        boolOfBit ((bits &&& hardwhitePosMask) >>> hardwhitePosStart)

    let encodeLexInt lightSyntaxStatus (lexcont: LexerWhitespaceContinuation) =
        let tag, n1, p1, ifd =
            match lexcont with
            | LexCont.Token ifd -> FSharpTokenizerColorState.Token, 0L, pos0, ifd
            | LexCont.IfDefSkip (ifd, n, m) -> FSharpTokenizerColorState.IfDefSkip, int64 n, m.Start, ifd
            | LexCont.EndLine(LexerEndlineContinuation.Skip(ifd, n, m)) -> FSharpTokenizerColorState.EndLineThenSkip, int64 n, m.Start, ifd
            | LexCont.EndLine(LexerEndlineContinuation.Token ifd) -> FSharpTokenizerColorState.EndLineThenToken, 0L, pos0, ifd
            | LexCont.String (ifd, m) -> FSharpTokenizerColorState.String, 0L, m.Start, ifd
            | LexCont.Comment (ifd, n, m) -> FSharpTokenizerColorState.Comment, int64 n, m.Start, ifd
            | LexCont.SingleLineComment (ifd, n, m) -> FSharpTokenizerColorState.SingleLineComment, int64 n, m.Start, ifd
            | LexCont.StringInComment (ifd, n, m) -> FSharpTokenizerColorState.StringInComment, int64 n, m.Start, ifd
            | LexCont.VerbatimStringInComment (ifd, n, m) -> FSharpTokenizerColorState.VerbatimStringInComment, int64 n, m.Start, ifd
            | LexCont.TripleQuoteStringInComment (ifd, n, m) -> FSharpTokenizerColorState.TripleQuoteStringInComment, int64 n, m.Start, ifd
            | LexCont.MLOnly (ifd, m) -> FSharpTokenizerColorState.CamlOnly, 0L, m.Start, ifd
            | LexCont.VerbatimString (ifd, m) -> FSharpTokenizerColorState.VerbatimString, 0L, m.Start, ifd
            | LexCont.TripleQuoteString (ifd, m) -> FSharpTokenizerColorState.TripleQuoteString, 0L, m.Start, ifd
        encodeLexCont tag n1 p1 ifd lightSyntaxStatus


    let decodeLexInt (state: FSharpTokenizerLexState) =
        let tag, n1, p1, ifd, lightSyntaxStatusInital = decodeLexCont state
        let lexcont =
            match tag with
            |  FSharpTokenizerColorState.Token -> LexCont.Token ifd
            |  FSharpTokenizerColorState.IfDefSkip -> LexCont.IfDefSkip (ifd, n1, mkRange "file" p1 p1)
            |  FSharpTokenizerColorState.String -> LexCont.String (ifd, mkRange "file" p1 p1)
            |  FSharpTokenizerColorState.Comment -> LexCont.Comment (ifd, n1, mkRange "file" p1 p1)
            |  FSharpTokenizerColorState.SingleLineComment -> LexCont.SingleLineComment (ifd, n1, mkRange "file" p1 p1)
            |  FSharpTokenizerColorState.StringInComment -> LexCont.StringInComment (ifd, n1, mkRange "file" p1 p1)
            |  FSharpTokenizerColorState.VerbatimStringInComment -> LexCont.VerbatimStringInComment (ifd, n1, mkRange "file" p1 p1)
            |  FSharpTokenizerColorState.TripleQuoteStringInComment -> LexCont.TripleQuoteStringInComment (ifd, n1, mkRange "file" p1 p1)
            |  FSharpTokenizerColorState.CamlOnly -> LexCont.MLOnly (ifd, mkRange "file" p1 p1)
            |  FSharpTokenizerColorState.VerbatimString -> LexCont.VerbatimString (ifd, mkRange "file" p1 p1)
            |  FSharpTokenizerColorState.TripleQuoteString -> LexCont.TripleQuoteString (ifd, mkRange "file" p1 p1)
            |  FSharpTokenizerColorState.EndLineThenSkip -> LexCont.EndLine(LexerEndlineContinuation.Skip(ifd, n1, mkRange "file" p1 p1))
            |  FSharpTokenizerColorState.EndLineThenToken -> LexCont.EndLine(LexerEndlineContinuation.Token ifd)
            | _ -> LexCont.Token []
        lightSyntaxStatusInital, lexcont

    let callLexCont lexcont args skip lexbuf =
        let argsWithIfDefs ifd =
            if !args.ifdefStack = ifd then
                args
            else
                {args with ifdefStack = ref ifd}
        match lexcont with
        | LexCont.EndLine cont -> Lexer.endline cont args skip lexbuf
        | LexCont.Token ifd -> Lexer.token (argsWithIfDefs ifd) skip lexbuf
        | LexCont.IfDefSkip (ifd, n, m) -> Lexer.ifdefSkip n m (argsWithIfDefs ifd) skip lexbuf
        // Q: What's this magic 100 number for? Q: it's just an initial buffer size.
        | LexCont.String (ifd, m) -> Lexer.string (ByteBuffer.Create 100, defaultStringFinisher, m, (argsWithIfDefs ifd)) skip lexbuf
        | LexCont.Comment (ifd, n, m) -> Lexer.comment (n, m, (argsWithIfDefs ifd)) skip lexbuf
        // The first argument is 'None' because we don't need XML comments when called from VS
        | LexCont.SingleLineComment (ifd, n, m) -> Lexer.singleLineComment (None, n, m, (argsWithIfDefs ifd)) skip lexbuf
        | LexCont.StringInComment (ifd, n, m) -> Lexer.stringInComment n m (argsWithIfDefs ifd) skip lexbuf
        | LexCont.VerbatimStringInComment (ifd, n, m) -> Lexer.verbatimStringInComment n m (argsWithIfDefs ifd) skip lexbuf
        | LexCont.TripleQuoteStringInComment (ifd, n, m) -> Lexer.tripleQuoteStringInComment n m (argsWithIfDefs ifd) skip lexbuf
        | LexCont.MLOnly (ifd, m) -> Lexer.mlOnly m (argsWithIfDefs ifd) skip lexbuf
        | LexCont.VerbatimString (ifd, m) -> Lexer.verbatimString (ByteBuffer.Create 100, defaultStringFinisher, m, (argsWithIfDefs ifd)) skip lexbuf
        | LexCont.TripleQuoteString (ifd, m) -> Lexer.tripleQuoteString (ByteBuffer.Create 100, defaultStringFinisher, m, (argsWithIfDefs ifd)) skip lexbuf

//----------------------------------------------------------------------------
// Colorization
//----------------------------------------------------------------------------

// Information beyond just tokens that can be derived by looking at just a single line.
// For example metacommands like #load.
type SingleLineTokenState =
    | BeforeHash = 0
    | NoFurtherMatchPossible = 1


/// Split a line into tokens and attach information about the tokens. This information is used by Visual Studio.
[<Sealed>]
type FSharpLineTokenizer(lexbuf: UnicodeLexing.Lexbuf,
                         maxLength: int option,
                         filename: Option<string>,
                         lexArgsLightOn: lexargs,
                         lexArgsLightOff: lexargs) =

    let skip = false   // don't skip whitespace in the lexer

    let mutable singleLineTokenState = SingleLineTokenState.BeforeHash
    let fsx = match filename with
              | None -> false
              | Some value -> CompileOps.IsScript value

    // ----------------------------------------------------------------------------------
    // This implements post-processing of #directive tokens - not very elegant, but it works...
    // We get the whole "   #if IDENT // .. .. " thing as a single token from the lexer,
    // so we need to split it into tokens that are used by VS for colorization

    // Stack for tokens that are split during postprocessing
    let mutable tokenStack = new Stack<_>()
    let delayToken tok = tokenStack.Push tok

    // Process: anywhite* #<directive>
    let processDirective (str: string) directiveLength delay cont =
        let hashIdx = str.IndexOf("#", StringComparison.Ordinal)
        if (hashIdx <> 0) then delay(WHITESPACE cont, 0, hashIdx - 1)
        delay(HASH_IF(range0, "", cont), hashIdx, hashIdx + directiveLength)
        hashIdx + directiveLength + 1

    // Process: anywhite* ("//" [^'\n''\r']*)?
    let processWhiteAndComment (str: string) offset delay cont =
        let rest = str.Substring(offset, str.Length - offset)
        let comment = rest.IndexOf('/')
        let spaceLength = if comment = -1 then rest.Length else comment
        if (spaceLength > 0) then delay(WHITESPACE cont, offset, offset + spaceLength - 1)
        if (comment <> -1) then delay(COMMENT cont, offset + comment, offset + rest.Length - 1)

    // Split a directive line from lexer into tokens usable in VS
    let processDirectiveLine ofs f =
        let delayed = new ResizeArray<_>()
        f (fun (tok, s, e) -> delayed.Add (tok, s + ofs, e + ofs) )
        // delay all the tokens and return the remaining one
        for i = delayed.Count - 1 downto 1 do delayToken delayed.[i]
        delayed.[0]

    // Split the following line:
    //  anywhite* ("#else"|"#endif") anywhite* ("//" [^'\n''\r']*)?
    let processHashEndElse ofs (str: string) length cont =
        processDirectiveLine ofs (fun delay ->
            // Process: anywhite* "#else"   /   anywhite* "#endif"
            let offset = processDirective str length delay cont
            // Process: anywhite* ("//" [^'\n''\r']*)?
            processWhiteAndComment str offset delay cont )

    // Split the following line:
    //  anywhite* "#if" anywhite+ ident anywhite* ("//" [^'\n''\r']*)?
    let processHashIfLine ofs (str: string) cont =
        let With n m = if (n < 0) then m else n
        processDirectiveLine ofs (fun delay ->
            // Process: anywhite* "#if"
            let offset = processDirective str 2 delay cont
            // Process: anywhite+ ident
            let rest, spaces =
                let w = str.Substring offset
                let r = w.TrimStart [| ' '; '\t' |]
                r, w.Length - r.Length
            let beforeIdent = offset + spaces
            let identLength = With (rest.IndexOfAny([| '/'; '\t'; ' ' |])) rest.Length
            delay(WHITESPACE cont, offset, beforeIdent - 1)
            delay(IDENT(rest.Substring(0, identLength)), beforeIdent, beforeIdent + identLength - 1)
            // Process: anywhite* ("//" [^'\n''\r']*)?
            let offset = beforeIdent + identLength
            processWhiteAndComment str offset delay cont )

    // ----------------------------------------------------------------------------------



    do match filename with
        | None -> lexbuf.EndPos <- Internal.Utilities.Text.Lexing.Position.Empty
        | Some value -> resetLexbufPos value lexbuf

    member x.ScanToken lexintInitial: FSharpTokenInfo option * FSharpTokenizerLexState =

        use unwindBP = PushThreadBuildPhaseUntilUnwind BuildPhase.Parse
        use unwindEL = PushErrorLoggerPhaseUntilUnwind (fun _ -> DiscardErrorsLogger)

        let lightSyntaxStatusInital, lexcontInitial = LexerStateEncoding.decodeLexInt lexintInitial
        let lightSyntaxStatus = LightSyntaxStatus(lightSyntaxStatusInital, false)

        // Build the arguments to the lexer function
        let lexargs = if lightSyntaxStatusInital then lexArgsLightOn else lexArgsLightOff

        let GetTokenWithPosition lexcontInitial =
            // Column of token
            let ColumnsOfCurrentToken() =
                let leftp = lexbuf.StartPos
                let rightp = lexbuf.EndPos
                let leftc = leftp.Column
                let rightc =
                    match maxLength with
                    | Some mx when rightp.Line > leftp.Line -> mx
                    | _ -> rightp.Column
                let rightc = rightc - 1
                leftc, rightc

            // Get the token & position - either from a stack or from the lexer
            try
                if (tokenStack.Count > 0) then true, tokenStack.Pop()
                else
                  // Choose which lexer entrypoint to call and call it
                  let token = LexerStateEncoding.callLexCont lexcontInitial lexargs skip lexbuf
                  let leftc, rightc = ColumnsOfCurrentToken()

                  // Splits tokens like ">." into multiple tokens - this duplicates behavior from the 'lexfilter'
                  // which cannot be (easily) used from the language service. The rules here are not always valid,
                  // because sometimes token shouldn't be split. However it is just for colorization &
                  // for VS (which needs to recognize when user types ".").
                  match token with
                  | HASH_IF (m, lineStr, cont) when lineStr <> "" ->
                      false, processHashIfLine m.StartColumn lineStr cont
                  | HASH_ELSE (m, lineStr, cont) when lineStr <> "" ->
                      false, processHashEndElse m.StartColumn lineStr 4 cont
                  | HASH_ENDIF (m, lineStr, cont) when lineStr <> "" ->
                      false, processHashEndElse m.StartColumn lineStr 5 cont
                  | RQUOTE_DOT (s, raw) ->
                      delayToken(DOT, rightc, rightc)
                      false, (RQUOTE (s, raw), leftc, rightc - 1)
                  | INFIX_COMPARE_OP (LexFilter.TyparsCloseOp(greaters, afterOp) as opstr) ->
                      match afterOp with
                      | None -> ()
                      | Some tok -> delayToken(tok, leftc + greaters.Length, rightc)
                      for i = greaters.Length - 1 downto 1 do
                          delayToken(greaters.[i] false, leftc + i, rightc - opstr.Length + i + 1)
                      false, (greaters.[0] false, leftc, rightc - opstr.Length + 1)
                  // break up any operators that start with '.' so that we can get auto-popup-completion for e.g. "x.+1"  when typing the dot
                  | INFIX_STAR_STAR_OP opstr when opstr.StartsWithOrdinal(".") ->
                      delayToken(INFIX_STAR_STAR_OP(opstr.Substring 1), leftc+1, rightc)
                      false, (DOT, leftc, leftc)
                  | PLUS_MINUS_OP opstr when opstr.StartsWithOrdinal(".") ->
                      delayToken(PLUS_MINUS_OP(opstr.Substring 1), leftc+1, rightc)
                      false, (DOT, leftc, leftc)
                  | INFIX_COMPARE_OP opstr when opstr.StartsWithOrdinal(".") ->
                      delayToken(INFIX_COMPARE_OP(opstr.Substring 1), leftc+1, rightc)
                      false, (DOT, leftc, leftc)
                  | INFIX_AT_HAT_OP opstr when opstr.StartsWithOrdinal(".") ->
                      delayToken(INFIX_AT_HAT_OP(opstr.Substring 1), leftc+1, rightc)
                      false, (DOT, leftc, leftc)
                  | INFIX_BAR_OP opstr when opstr.StartsWithOrdinal(".") ->
                      delayToken(INFIX_BAR_OP(opstr.Substring 1), leftc+1, rightc)
                      false, (DOT, leftc, leftc)
                  | PREFIX_OP opstr when opstr.StartsWithOrdinal(".") ->
                      delayToken(PREFIX_OP(opstr.Substring 1), leftc+1, rightc)
                      false, (DOT, leftc, leftc)
                  | INFIX_STAR_DIV_MOD_OP opstr when opstr.StartsWithOrdinal(".") ->
                      delayToken(INFIX_STAR_DIV_MOD_OP(opstr.Substring 1), leftc+1, rightc)
                      false, (DOT, leftc, leftc)
                  | INFIX_AMP_OP opstr when opstr.StartsWithOrdinal(".") ->
                      delayToken(INFIX_AMP_OP(opstr.Substring 1), leftc+1, rightc)
                      false, (DOT, leftc, leftc)
                  | ADJACENT_PREFIX_OP opstr when opstr.StartsWithOrdinal(".") ->
                      delayToken(ADJACENT_PREFIX_OP(opstr.Substring 1), leftc+1, rightc)
                      false, (DOT, leftc, leftc)
                  | FUNKY_OPERATOR_NAME opstr when opstr.StartsWithOrdinal(".") ->
                      delayToken(FUNKY_OPERATOR_NAME(opstr.Substring 1), leftc+1, rightc)
                      false, (DOT, leftc, leftc)
                  | _ -> false, (token, leftc, rightc)
            with
            | e -> false, (EOF LexerStateEncoding.revertToDefaultLexCont, 0, 0) // REVIEW: report lex failure here

        // Grab a token
        let isCached, (token, leftc, rightc) = GetTokenWithPosition lexcontInitial

        // Check for end-of-string and failure
        let tokenDataOption, lexcontFinal, tokenTag =
            match token with
            | EOF lexcont ->
                // End of text! No more tokens.
                None, lexcont, 0
            | LEX_FAILURE _ ->
                None, LexerStateEncoding.revertToDefaultLexCont, 0
            | _ ->
                // Get the information about the token
                let (colorClass, charClass, triggerClass) = TokenClassifications.tokenInfo token
                let lexcontFinal =
                    // If we're using token from cache, we don't move forward with lexing
                    if isCached then lexcontInitial else LexerStateEncoding.computeNextLexState token lexcontInitial
                let tokenTag = tagOfToken token
                let fullMatchedLength = lexbuf.EndPos.AbsoluteOffset - lexbuf.StartPos.AbsoluteOffset
                let tokenData =
                    { TokenName = token_to_string token
                      LeftColumn=leftc
                      RightColumn=rightc
                      ColorClass=colorClass
                      CharClass=charClass
                      FSharpTokenTriggerClass=triggerClass
                      Tag=tokenTag
                      FullMatchedLength=fullMatchedLength}
                Some tokenData, lexcontFinal, tokenTag

        // Get the final lex int and color state
        let FinalState lexcontFinal =
            LexerStateEncoding.encodeLexInt lightSyntaxStatus.Status lexcontFinal

        // Check for patterns like #-IDENT and see if they look like meta commands for .fsx files. If they do then merge them into a single token.
        let tokenDataOption, lexintFinal =
            let lexintFinal = FinalState lexcontFinal
            match tokenDataOption, singleLineTokenState, tokenTagToTokenId tokenTag with
            | Some tokenData, SingleLineTokenState.BeforeHash, TOKEN_HASH ->
                // Don't allow further matches.
                singleLineTokenState <- SingleLineTokenState.NoFurtherMatchPossible
                // Peek at the next token
                let isCached, (nextToken, _, rightc) = GetTokenWithPosition lexcontInitial
                match nextToken with
                | IDENT possibleMetacommand ->
                    match fsx, possibleMetacommand with
                    // These are for script (.fsx and .fsscript) files.
                    | true, "r"
                    | true, "reference"
                    | true, "I"
                    | true, "load"
                    | true, "time"
                    | true, "dbgbreak"
                    | true, "cd"
#if DEBUG
                    | true, "terms"
                    | true, "types"
                    | true, "savedll"
                    | true, "nosavedll"
#endif
                    | true, "silentCd"
                    | true, "q"
                    | true, "quit"
                    | true, "help"
                    // These are for script and non-script
                    | _, "nowarn" ->
                        // Merge both tokens into one.
                        let lexcontFinal = if isCached then lexcontInitial else LexerStateEncoding.computeNextLexState token lexcontInitial
                        let tokenData = {tokenData with RightColumn=rightc;ColorClass=FSharpTokenColorKind.PreprocessorKeyword;CharClass=FSharpTokenCharKind.Keyword;FSharpTokenTriggerClass=FSharpTokenTriggerClass.None}
                        let lexintFinal = FinalState lexcontFinal
                        Some tokenData, lexintFinal
                    | _ -> tokenDataOption, lexintFinal
                | _ -> tokenDataOption, lexintFinal
            | _, SingleLineTokenState.BeforeHash, TOKEN_WHITESPACE ->
                // Allow leading whitespace.
                tokenDataOption, lexintFinal
            | _ ->
                singleLineTokenState <- SingleLineTokenState.NoFurtherMatchPossible
                tokenDataOption, lexintFinal

        tokenDataOption, lexintFinal

    static member ColorStateOfLexState(lexState: FSharpTokenizerLexState) =
        LexerStateEncoding.colorStateOfLexState lexState

    static member LexStateOfColorState(colorState: FSharpTokenizerColorState) =
        { PosBits = 0L; OtherBits = LexerStateEncoding.lexStateOfColorState colorState }

[<Sealed>]
type FSharpSourceTokenizer(defineConstants: string list, filename: string option) =
    let lexResourceManager = new Lexhelp.LexResourceManager()

    let lexArgsLightOn = mkLexargs(filename, defineConstants, LightSyntaxStatus(true, false), lexResourceManager, ref [], DiscardErrorsLogger, PathMap.empty)
    let lexArgsLightOff = mkLexargs(filename, defineConstants, LightSyntaxStatus(false, false), lexResourceManager, ref [], DiscardErrorsLogger, PathMap.empty)

    member this.CreateLineTokenizer(lineText: string) =
        let lexbuf = UnicodeLexing.StringAsLexbuf lineText
        FSharpLineTokenizer(lexbuf, Some lineText.Length, filename, lexArgsLightOn, lexArgsLightOff)


    member this.CreateBufferTokenizer bufferFiller =
        let lexbuf = UnicodeLexing.FunctionAsLexbuf bufferFiller
        FSharpLineTokenizer(lexbuf, None, filename, lexArgsLightOn, lexArgsLightOff)

module Keywords =
    open FSharp.Compiler.Lexhelp.Keywords

    let QuoteIdentifierIfNeeded s = QuoteIdentifierIfNeeded s
    let NormalizeIdentifierBackticks s = NormalizeIdentifierBackticks s
    let KeywordsWithDescription = keywordsWithDescription

