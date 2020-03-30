// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module public FSharp.Compiler.ParseHelpers

open FSharp.Compiler.AbstractIL
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.Features
open FSharp.Compiler.Range
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.UnicodeLexing
open FSharp.Compiler.XmlDoc
open Internal.Utilities.Text.Lexing
open Internal.Utilities.Text.Parsing

//------------------------------------------------------------------------
// Parsing: Error recovery exception for fsyacc
//------------------------------------------------------------------------

/// The error raised by the parse_error_rich function, which is called by the parser engine
/// when a syntax error occurs. The first object is the ParseErrorContext which contains a dump of
/// information about the grammar at the point where the error occurred, e.g. what tokens
/// are valid to shift next at that point in the grammar. This information is processed in CompileOps.fs.
[<NoEquality; NoComparison>]
exception SyntaxError of obj (* ParseErrorContext<_> *) * range: range

//------------------------------------------------------------------------
// Parsing: getting positions from the lexer
//------------------------------------------------------------------------

/// Get an F# compiler position from a lexer position
let internal posOfLexPosition (p: Position) =
    mkPos p.Line p.Column

/// Get an F# compiler range from a lexer range
let internal mkSynRange (p1: Position) (p2: Position) =
    mkFileIndexRange p1.FileIndex (posOfLexPosition p1) (posOfLexPosition p2)

type LexBuffer<'Char> with
    member internal lexbuf.LexemeRange  = mkSynRange lexbuf.StartPos lexbuf.EndPos

/// Get the range corresponding to the result of a grammar rule while it is being reduced
let internal lhs (parseState: IParseState) =
    let p1 = parseState.ResultStartPosition
    let p2 = parseState.ResultEndPosition
    mkSynRange p1 p2

/// Get the range covering two of the r.h.s. symbols of a grammar rule while it is being reduced
let internal rhs2 (parseState: IParseState) i j =
    let p1 = parseState.InputStartPosition i
    let p2 = parseState.InputEndPosition j
    mkSynRange p1 p2

/// Get the range corresponding to one of the r.h.s. symbols of a grammar rule while it is being reduced
let internal rhs parseState i = rhs2 parseState i i

type IParseState with

    /// Get the generator used for compiler-generated argument names.
    member internal x.SynArgNameGenerator =
        let key = "SynArgNameGenerator"
        let bls = x.LexBuffer.BufferLocalStore
        let gen =
            match bls.TryGetValue key with
            | true, gen -> gen
            | _ ->
                let gen = box (SynArgNameGenerator())
                bls.[key] <- gen
                gen
        gen :?> SynArgNameGenerator

    /// Reset the generator used for compiler-generated argument names.
    member internal x.ResetSynArgNameGenerator() = x.SynArgNameGenerator.Reset()

//------------------------------------------------------------------------
// Parsing: grabbing XmlDoc
//------------------------------------------------------------------------

/// XmlDoc F# lexer/parser state, held in the BufferLocalStore for the lexer.
/// This is the only use of the lexer BufferLocalStore in the codebase.
module LexbufLocalXmlDocStore =
    // The key into the BufferLocalStore used to hold the current accumulated XmlDoc lines
    let private xmlDocKey = "XmlDoc"

    let internal ClearXmlDoc (lexbuf: Lexbuf) =
        lexbuf.BufferLocalStore.[xmlDocKey] <- box (XmlDocCollector())

    /// Called from the lexer to save a single line of XML doc comment.
    let internal SaveXmlDocLine (lexbuf: Lexbuf, lineText, pos) =
        let collector =
            match lexbuf.BufferLocalStore.TryGetValue xmlDocKey with
            | true, collector -> collector
            | _ ->
                let collector = box (XmlDocCollector())
                lexbuf.BufferLocalStore.[xmlDocKey] <- collector
                collector
        let collector = unbox<XmlDocCollector>(collector)
        collector.AddXmlDocLine(lineText, pos)

    /// Called from the parser each time we parse a construct that marks the end of an XML doc comment range,
    /// e.g. a 'type' declaration. The markerRange is the range of the keyword that delimits the construct.
    let internal GrabXmlDocBeforeMarker (lexbuf: Lexbuf, markerRange: range)  =
        match lexbuf.BufferLocalStore.TryGetValue xmlDocKey with
        | true, collector ->
            let collector = unbox<XmlDocCollector>(collector)
            PreXmlDoc.CreateFromGrabPoint(collector, markerRange.End)
        | _ ->
            PreXmlDoc.Empty


//------------------------------------------------------------------------
// Parsing/lexing: status of #if/#endif processing in lexing, used for continutations
// for whitespace tokens in parser specification.
//------------------------------------------------------------------------

type LexerIfdefStackEntry =
    | IfDefIf
    | IfDefElse

type LexerIfdefStackEntries = (LexerIfdefStackEntry * range) list

type LexerIfdefStack = LexerIfdefStackEntries

/// Specifies how the 'endline' function in the lexer should continue after
/// it reaches end of line or eof. The options are to continue with 'token' function
/// or to continue with 'skip' function.
type LexerEndlineContinuation =
    | Token of LexerIfdefStackEntries
    | Skip of LexerIfdefStackEntries * int * range: range
    member x.LexerIfdefStack =
      match x with
      | LexerEndlineContinuation.Token ifd
      | LexerEndlineContinuation.Skip(ifd, _, _) -> ifd

type LexerIfdefExpression =
    | IfdefAnd of LexerIfdefExpression*LexerIfdefExpression
    | IfdefOr of LexerIfdefExpression*LexerIfdefExpression
    | IfdefNot of LexerIfdefExpression
    | IfdefId of string

let rec LexerIfdefEval (lookup: string -> bool) = function
    | IfdefAnd (l, r)    -> (LexerIfdefEval lookup l) && (LexerIfdefEval lookup r)
    | IfdefOr (l, r)     -> (LexerIfdefEval lookup l) || (LexerIfdefEval lookup r)
    | IfdefNot e        -> not (LexerIfdefEval lookup e)
    | IfdefId id        -> lookup id

//------------------------------------------------------------------------
// Parsing: continuations for whitespace tokens
//------------------------------------------------------------------------

/// The parser defines a number of tokens for whitespace and
/// comments eliminated by the lexer.  These carry a specification of
/// a continuation for the lexer for continued processing after we've dealt with
/// the whitespace.
[<RequireQualifiedAccess>]
[<NoComparison; NoEquality>]
type LexerWhitespaceContinuation =
    | Token of ifdef: LexerIfdefStackEntries
    | IfDefSkip of ifdef: LexerIfdefStackEntries * int * range: range
    | String of ifdef: LexerIfdefStackEntries * range: range
    | VerbatimString of ifdef: LexerIfdefStackEntries * range: range
    | TripleQuoteString of ifdef: LexerIfdefStackEntries * range: range
    | Comment of ifdef: LexerIfdefStackEntries * int * range: range
    | SingleLineComment of ifdef: LexerIfdefStackEntries * int * range: range
    | StringInComment of ifdef: LexerIfdefStackEntries * int * range: range
    | VerbatimStringInComment of ifdef: LexerIfdefStackEntries * int * range: range
    | TripleQuoteStringInComment of ifdef: LexerIfdefStackEntries * int * range: range
    | MLOnly of ifdef: LexerIfdefStackEntries * range: range
    | EndLine of LexerEndlineContinuation

    member x.LexerIfdefStack =
        match x with
        | LexCont.Token (ifdef=ifd)
        | LexCont.IfDefSkip (ifdef=ifd)
        | LexCont.String (ifdef=ifd)
        | LexCont.VerbatimString (ifdef=ifd)
        | LexCont.Comment (ifdef=ifd)
        | LexCont.SingleLineComment (ifdef=ifd)
        | LexCont.TripleQuoteString (ifdef=ifd)
        | LexCont.StringInComment (ifdef=ifd)
        | LexCont.VerbatimStringInComment (ifdef=ifd)
        | LexCont.TripleQuoteStringInComment (ifdef=ifd)
        | LexCont.MLOnly (ifdef=ifd) -> ifd
        | LexCont.EndLine endl -> endl.LexerIfdefStack

and LexCont = LexerWhitespaceContinuation

//------------------------------------------------------------------------
// Parse IL assembly code
//------------------------------------------------------------------------

let internal internalParseAssemblyCodeInstructions s isFeatureSupported m =
#if NO_INLINE_IL_PARSER
    ignore s
    ignore isFeatureSupported

    errorR(Error((193, "Inline IL not valid in a hosted environment"), m))
    [| |]
#else
    try
        FSharp.Compiler.AbstractIL.Internal.AsciiParser.ilInstrs
           FSharp.Compiler.AbstractIL.Internal.AsciiLexer.token
           (UnicodeLexing.StringAsLexbuf(isFeatureSupported, s))
    with _ ->
      errorR(Error(FSComp.SR.astParseEmbeddedILError(), m)); [||]
#endif

let ParseAssemblyCodeInstructions s m =
    // Public API can not answer the isFeatureSupported questions, so here we support everything
    let isFeatureSupported (_featureId:LanguageFeature) = true
    internalParseAssemblyCodeInstructions s isFeatureSupported m

let internal internalParseAssemblyCodeType s isFeatureSupported m =
    ignore s
    ignore isFeatureSupported

#if NO_INLINE_IL_PARSER
    errorR(Error((193, "Inline IL not valid in a hosted environment"), m))
    IL.EcmaMscorlibILGlobals.typ_Object
#else
    let isFeatureSupported (_featureId:LanguageFeature) = true
    try
        FSharp.Compiler.AbstractIL.Internal.AsciiParser.ilType
           FSharp.Compiler.AbstractIL.Internal.AsciiLexer.token
           (UnicodeLexing.StringAsLexbuf(isFeatureSupported, s))
    with RecoverableParseError ->
      errorR(Error(FSComp.SR.astParseEmbeddedILTypeError(), m));
      IL.EcmaMscorlibILGlobals.typ_Object
#endif

/// Helper for parsing the inline IL fragments.
let ParseAssemblyCodeType s m =
    // Public API can not answer the isFeatureSupported questions, so here we support everything
    let isFeatureSupported (_featureId:LanguageFeature) = true
    internalParseAssemblyCodeType s isFeatureSupported m
