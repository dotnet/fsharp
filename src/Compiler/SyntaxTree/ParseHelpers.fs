// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.ParseHelpers

open FSharp.Compiler.AbstractIL
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Features
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.UnicodeLexing
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Position
open FSharp.Compiler.Text.Range
open FSharp.Compiler.Xml
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
exception SyntaxError of obj (* ParseErrorContext<_> *)  * range: range

exception IndentationProblem of string * range

let warningStringOfCoords line column = sprintf "(%d:%d)" line (column + 1)

let warningStringOfPos (p: pos) = warningStringOfCoords p.Line p.Column

//------------------------------------------------------------------------
// Parsing: getting positions from the lexer
//------------------------------------------------------------------------

/// Get an F# compiler position from a lexer position
let posOfLexPosition (p: Position) = mkPos p.Line p.Column

/// Get an F# compiler range from a lexer range
let mkSynRange (p1: Position) (p2: Position) =
    mkFileIndexRange p1.FileIndex (posOfLexPosition p1) (posOfLexPosition p2)

type LexBuffer<'Char> with

    member lexbuf.LexemeRange = mkSynRange lexbuf.StartPos lexbuf.EndPos

/// Get the range corresponding to the result of a grammar rule while it is being reduced
let lhs (parseState: IParseState) =
    let p1 = parseState.ResultStartPosition
    let p2 = parseState.ResultEndPosition
    mkSynRange p1 p2

/// Get the range covering two of the r.h.s. symbols of a grammar rule while it is being reduced
let rhs2 (parseState: IParseState) i j =
    let p1 = parseState.InputStartPosition i
    let p2 = parseState.InputEndPosition j
    mkSynRange p1 p2

/// Get the range corresponding to one of the r.h.s. symbols of a grammar rule while it is being reduced
let rhs parseState i = rhs2 parseState i i

type IParseState with

    /// Get the generator used for compiler-generated argument names.
    member x.SynArgNameGenerator =
        let key = "SynArgNameGenerator"
        let bls = x.LexBuffer.BufferLocalStore

        let gen =
            match bls.TryGetValue key with
            | true, gen -> gen
            | _ ->
                let gen = box (SynArgNameGenerator())
                bls[key] <- gen
                gen

        gen :?> SynArgNameGenerator

    /// Reset the generator used for compiler-generated argument names.
    member x.ResetSynArgNameGenerator() = x.SynArgNameGenerator.Reset()

//------------------------------------------------------------------------
// Parsing: grabbing XmlDoc
//------------------------------------------------------------------------

/// XmlDoc F# lexer/parser state, held in the BufferLocalStore for the lexer.
module LexbufLocalXmlDocStore =
    // The key into the BufferLocalStore used to hold the current accumulated XmlDoc lines
    let private xmlDocKey = "XmlDoc"

    let private getCollector (lexbuf: Lexbuf) =
        match lexbuf.BufferLocalStore.TryGetValue xmlDocKey with
        | true, collector -> collector
        | _ ->
            let collector = box (XmlDocCollector())
            lexbuf.BufferLocalStore[ xmlDocKey ] <- collector
            collector

        |> unbox<XmlDocCollector>

    let ClearXmlDoc (lexbuf: Lexbuf) =
        lexbuf.BufferLocalStore[ xmlDocKey ] <- box (XmlDocCollector())

    /// Called from the lexer to save a single line of XML doc comment.
    let SaveXmlDocLine (lexbuf: Lexbuf, lineText, range: range) =
        let collector = getCollector lexbuf
        collector.AddXmlDocLine(lineText, range)

    let AddGrabPoint (lexbuf: Lexbuf) =
        let collector = getCollector lexbuf
        let startPos = lexbuf.StartPos
        collector.AddGrabPoint(mkPos startPos.Line startPos.Column)

    /// Allowed cases when there are comments after XmlDoc
    ///
    ///    /// X xmlDoc
    ///    // comment
    ///    //// comment
    ///    (* multiline comment *)
    ///    let x = ...        // X xmlDoc
    ///
    /// Remember the first position when a comment (//, (* *), ////) is encountered after the XmlDoc block
    /// then add a grab point if a new XmlDoc block follows the comments
    let AddGrabPointDelayed (lexbuf: Lexbuf) =
        let collector = getCollector lexbuf
        let startPos = lexbuf.StartPos
        collector.AddGrabPointDelayed(mkPos startPos.Line startPos.Column)

    /// Called from the parser each time we parse a construct that marks the end of an XML doc comment range,
    /// e.g. a 'type' declaration. The markerRange is the range of the keyword that delimits the construct.
    let GrabXmlDocBeforeMarker (lexbuf: Lexbuf, markerRange: range) =
        match lexbuf.BufferLocalStore.TryGetValue xmlDocKey with
        | true, collector ->
            let collector = unbox<XmlDocCollector> (collector)
            PreXmlDoc.CreateFromGrabPoint(collector, markerRange.Start)
        | _ -> PreXmlDoc.Empty

    let ReportInvalidXmlDocPositions (lexbuf: Lexbuf) =
        let collector = getCollector lexbuf
        collector.CheckInvalidXmlDocPositions()

//------------------------------------------------------------------------
// Parsing/lexing: status of #if/#endif processing in lexing, used for continutations
// for whitespace tokens in parser specification.
//------------------------------------------------------------------------

type LexerIfdefStackEntry =
    | IfDefIf
    | IfDefElse

/// Represents the active #if/#else blocks
type LexerIfdefStackEntries = (LexerIfdefStackEntry * range) list

type LexerIfdefStack = LexerIfdefStackEntries

/// Specifies how the 'endline' function in the lexer should continue after
/// it reaches end of line or eof. The options are to continue with 'token' function
/// or to continue with 'skip' function.
type LexerEndlineContinuation =
    | Token
    | Skip of int * range: range

type LexerIfdefExpression =
    | IfdefAnd of LexerIfdefExpression * LexerIfdefExpression
    | IfdefOr of LexerIfdefExpression * LexerIfdefExpression
    | IfdefNot of LexerIfdefExpression
    | IfdefId of string

let rec LexerIfdefEval (lookup: string -> bool) =
    function
    | IfdefAnd (l, r) -> (LexerIfdefEval lookup l) && (LexerIfdefEval lookup r)
    | IfdefOr (l, r) -> (LexerIfdefEval lookup l) || (LexerIfdefEval lookup r)
    | IfdefNot e -> not (LexerIfdefEval lookup e)
    | IfdefId id -> lookup id

/// Ifdef F# lexer/parser state, held in the BufferLocalStore for the lexer.
/// Used to capture #if, #else and #endif as syntax trivia.
module LexbufIfdefStore =
    // The key into the BufferLocalStore used to hold the compiler directives
    let private ifDefKey = "Ifdef"

    let private getStore (lexbuf: Lexbuf) : ResizeArray<ConditionalDirectiveTrivia> =
        match lexbuf.BufferLocalStore.TryGetValue ifDefKey with
        | true, store -> store
        | _ ->
            let store = box (ResizeArray<ConditionalDirectiveTrivia>())
            lexbuf.BufferLocalStore[ ifDefKey ] <- store
            store
        |> unbox<ResizeArray<ConditionalDirectiveTrivia>>

    let private mkRangeWithoutLeadingWhitespace (lexed: string) (m: range) : range =
        let startColumn = lexed.Length - lexed.TrimStart().Length
        mkFileIndexRange m.FileIndex (mkPos m.StartLine startColumn) m.End

    let SaveIfHash (lexbuf: Lexbuf, lexed: string, expr: LexerIfdefExpression, range: range) =
        let store = getStore lexbuf

        let expr =
            let rec visit (expr: LexerIfdefExpression) : IfDirectiveExpression =
                match expr with
                | LexerIfdefExpression.IfdefAnd (l, r) -> IfDirectiveExpression.And(visit l, visit r)
                | LexerIfdefExpression.IfdefOr (l, r) -> IfDirectiveExpression.Or(visit l, visit r)
                | LexerIfdefExpression.IfdefNot e -> IfDirectiveExpression.Not(visit e)
                | LexerIfdefExpression.IfdefId id -> IfDirectiveExpression.Ident id

            visit expr

        let m = mkRangeWithoutLeadingWhitespace lexed range

        store.Add(ConditionalDirectiveTrivia.If(expr, m))

    let SaveElseHash (lexbuf: Lexbuf, lexed: string, range: range) =
        let store = getStore lexbuf
        let m = mkRangeWithoutLeadingWhitespace lexed range
        store.Add(ConditionalDirectiveTrivia.Else(m))

    let SaveEndIfHash (lexbuf: Lexbuf, lexed: string, range: range) =
        let store = getStore lexbuf
        let m = mkRangeWithoutLeadingWhitespace lexed range
        store.Add(ConditionalDirectiveTrivia.EndIf(m))

    let GetTrivia (lexbuf: Lexbuf) : ConditionalDirectiveTrivia list =
        let store = getStore lexbuf
        Seq.toList store

/// Used to capture the ranges of code comments as syntax trivia
module LexbufCommentStore =
    // The key into the BufferLocalStore used to hold the compiler directives
    let private commentKey = "Comments"

    let private getStore (lexbuf: Lexbuf) : ResizeArray<CommentTrivia> =
        match lexbuf.BufferLocalStore.TryGetValue commentKey with
        | true, store -> store
        | _ ->
            let store = box (ResizeArray<CommentTrivia>())
            lexbuf.BufferLocalStore[ commentKey ] <- store
            store
        |> unbox<ResizeArray<CommentTrivia>>

    let SaveSingleLineComment (lexbuf: Lexbuf, startRange: range, endRange: range) =
        let store = getStore lexbuf
        let m = unionRanges startRange endRange
        store.Add(CommentTrivia.LineComment(m))

    let SaveBlockComment (lexbuf: Lexbuf, startRange: range, endRange: range) =
        let store = getStore lexbuf
        let m = unionRanges startRange endRange
        store.Add(CommentTrivia.BlockComment(m))

    let GetComments (lexbuf: Lexbuf) : CommentTrivia list =
        let store = getStore lexbuf
        Seq.toList store

    let ClearComments (lexbuf: Lexbuf) : unit =
        lexbuf.BufferLocalStore.Remove(commentKey) |> ignore

//------------------------------------------------------------------------
// Parsing: continuations for whitespace tokens
//------------------------------------------------------------------------

[<RequireQualifiedAccess>]
type LexerStringStyle =
    | Verbatim
    | TripleQuote
    | SingleQuote

[<RequireQualifiedAccess; Struct>]
type LexerStringKind =
    {
        IsByteString: bool
        IsInterpolated: bool
        IsInterpolatedFirst: bool
    }

    static member String =
        {
            IsByteString = false
            IsInterpolated = false
            IsInterpolatedFirst = false
        }

    static member ByteString =
        {
            IsByteString = true
            IsInterpolated = false
            IsInterpolatedFirst = false
        }

    static member InterpolatedStringFirst =
        {
            IsByteString = false
            IsInterpolated = true
            IsInterpolatedFirst = true
        }

    static member InterpolatedStringPart =
        {
            IsByteString = false
            IsInterpolated = true
            IsInterpolatedFirst = false
        }

/// Represents the degree of nesting of '{..}' and the style of the string to continue afterwards, in an interpolation fill.
/// Nesting counters and styles of outer interpolating strings are pushed on this stack.
type LexerInterpolatedStringNesting = (int * LexerStringStyle * range) list

/// The parser defines a number of tokens for whitespace and
/// comments eliminated by the lexer.  These carry a specification of
/// a continuation for the lexer for continued processing after we've dealt with
/// the whitespace.
[<RequireQualifiedAccess>]
[<NoComparison; NoEquality>]
type LexerContinuation =
    | Token of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting
    | IfDefSkip of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting * int * range: range
    | String of
        ifdef: LexerIfdefStackEntries *
        nesting: LexerInterpolatedStringNesting *
        style: LexerStringStyle *
        kind: LexerStringKind *
        range: range
    | Comment of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting * int * range: range
    | SingleLineComment of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting * int * range: range
    | StringInComment of
        ifdef: LexerIfdefStackEntries *
        nesting: LexerInterpolatedStringNesting *
        style: LexerStringStyle *
        int *
        range: range
    | MLOnly of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting * range: range
    | EndLine of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting * LexerEndlineContinuation

    static member Default = LexCont.Token([], [])

    member x.LexerIfdefStack =
        match x with
        | LexCont.Token (ifdef = ifd)
        | LexCont.IfDefSkip (ifdef = ifd)
        | LexCont.String (ifdef = ifd)
        | LexCont.Comment (ifdef = ifd)
        | LexCont.SingleLineComment (ifdef = ifd)
        | LexCont.StringInComment (ifdef = ifd)
        | LexCont.EndLine (ifdef = ifd)
        | LexCont.MLOnly (ifdef = ifd) -> ifd

    member x.LexerInterpStringNesting =
        match x with
        | LexCont.Token (nesting = nesting)
        | LexCont.IfDefSkip (nesting = nesting)
        | LexCont.String (nesting = nesting)
        | LexCont.Comment (nesting = nesting)
        | LexCont.SingleLineComment (nesting = nesting)
        | LexCont.StringInComment (nesting = nesting)
        | LexCont.EndLine (nesting = nesting)
        | LexCont.MLOnly (nesting = nesting) -> nesting

and LexCont = LexerContinuation

//------------------------------------------------------------------------
// Parse IL assembly code
//------------------------------------------------------------------------

let ParseAssemblyCodeInstructions s reportLibraryOnlyFeatures langVersion m : IL.ILInstr[] =
#if NO_INLINE_IL_PARSER
    ignore s
    ignore isFeatureSupported

    errorR (Error((193, "Inline IL not valid in a hosted environment"), m))
    [||]
#else
    try
        AsciiParser.ilInstrs AsciiLexer.token (StringAsLexbuf(reportLibraryOnlyFeatures, langVersion, s))
    with _ ->
        errorR (Error(FSComp.SR.astParseEmbeddedILError (), m))
        [||]
#endif

let ParseAssemblyCodeType s reportLibraryOnlyFeatures langVersion m =
    ignore s

#if NO_INLINE_IL_PARSER
    errorR (Error((193, "Inline IL not valid in a hosted environment"), m))
    IL.PrimaryAssemblyILGlobals.typ_Object
#else
    try
        AsciiParser.ilType AsciiLexer.token (StringAsLexbuf(reportLibraryOnlyFeatures, langVersion, s))
    with RecoverableParseError ->
        errorR (Error(FSComp.SR.astParseEmbeddedILTypeError (), m))
        IL.PrimaryAssemblyILGlobals.typ_Object
#endif

let grabXmlDocAtRangeStart (parseState: IParseState, optAttributes: SynAttributeList list, range: range) =
    let grabPoint =
        match optAttributes with
        | [] -> range
        | h :: _ -> h.Range

    LexbufLocalXmlDocStore.GrabXmlDocBeforeMarker(parseState.LexBuffer, grabPoint)

let grabXmlDoc (parseState: IParseState, optAttributes: SynAttributeList list, elemIdx) =
    grabXmlDocAtRangeStart (parseState, optAttributes, rhs parseState elemIdx)
