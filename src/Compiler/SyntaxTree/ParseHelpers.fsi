// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.ParseHelpers

open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.Syntax
open FSharp.Compiler.Features
open FSharp.Compiler.Text
open FSharp.Compiler.Xml
open Internal.Utilities.Text.Lexing
open Internal.Utilities.Text.Parsing

/// The error raised by the parse_error_rich function, which is called by the parser engine
/// when a syntax error occurs. The first object is the ParseErrorContext which contains a dump of
/// information about the grammar at the point where the error occurred, e.g. what tokens
/// are valid to shift next at that point in the grammar. This information is processed in CompileOps.fs.
[<NoEquality; NoComparison>]
exception SyntaxError of obj * range: range

exception IndentationProblem of string * range

val warningStringOfCoords: line: int -> column: int -> string

val warningStringOfPos: p: pos -> string

val posOfLexPosition: p: Position -> pos

val mkSynRange: p1: Position -> p2: Position -> range

type LexBuffer<'Char> with

    member LexemeRange: range

val lhs: parseState: IParseState -> range

val rhs2: parseState: IParseState -> i: int -> j: int -> range

val rhs: parseState: IParseState -> i: int -> range

type IParseState with

    member SynArgNameGenerator: SyntaxTreeOps.SynArgNameGenerator
    member ResetSynArgNameGenerator: unit -> unit

module LexbufLocalXmlDocStore =

    val ClearXmlDoc: lexbuf: UnicodeLexing.Lexbuf -> unit

    val SaveXmlDocLine: lexbuf: UnicodeLexing.Lexbuf * lineText: string * range: range -> unit

    val GrabXmlDocBeforeMarker: lexbuf: UnicodeLexing.Lexbuf * markerRange: range -> PreXmlDoc

    val AddGrabPoint: lexbuf: UnicodeLexing.Lexbuf -> unit

    val AddGrabPointDelayed: lexbuf: UnicodeLexing.Lexbuf -> unit

    val ReportInvalidXmlDocPositions: lexbuf: UnicodeLexing.Lexbuf -> range list

type LexerIfdefStackEntry =
    | IfDefIf
    | IfDefElse

type LexerIfdefStackEntries = (LexerIfdefStackEntry * range) list

type LexerIfdefStack = LexerIfdefStackEntries

type LexerEndlineContinuation =
    | Token
    | Skip of int * range: range

type LexerIfdefExpression =
    | IfdefAnd of LexerIfdefExpression * LexerIfdefExpression
    | IfdefOr of LexerIfdefExpression * LexerIfdefExpression
    | IfdefNot of LexerIfdefExpression
    | IfdefId of string

val LexerIfdefEval: lookup: (string -> bool) -> _arg1: LexerIfdefExpression -> bool

module LexbufIfdefStore =

    val SaveIfHash: lexbuf: UnicodeLexing.Lexbuf * lexed: string * expr: LexerIfdefExpression * range: range -> unit

    val SaveElseHash: lexbuf: UnicodeLexing.Lexbuf * lexed: string * range: range -> unit

    val SaveEndIfHash: lexbuf: UnicodeLexing.Lexbuf * lexed: string * range: range -> unit

    val GetTrivia: lexbuf: UnicodeLexing.Lexbuf -> SyntaxTrivia.ConditionalDirectiveTrivia list

module LexbufCommentStore =

    val SaveSingleLineComment: lexbuf: UnicodeLexing.Lexbuf * startRange: range * endRange: range -> unit

    val SaveBlockComment: lexbuf: UnicodeLexing.Lexbuf * startRange: range * endRange: range -> unit

    val GetComments: lexbuf: UnicodeLexing.Lexbuf -> SyntaxTrivia.CommentTrivia list

    val ClearComments: lexbuf: UnicodeLexing.Lexbuf -> unit

[<RequireQualifiedAccess>]
type LexerStringStyle =
    | Verbatim
    | TripleQuote
    | SingleQuote

[<RequireQualifiedAccess; Struct>]
type LexerStringKind =
    { IsByteString: bool
      IsInterpolated: bool
      IsInterpolatedFirst: bool }

    static member ByteString: LexerStringKind

    static member InterpolatedStringFirst: LexerStringKind

    static member InterpolatedStringPart: LexerStringKind

    static member String: LexerStringKind

type LexerInterpolatedStringNesting = (int * LexerStringStyle * range) list

[<RequireQualifiedAccess; NoComparison; NoEquality>]
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

    member LexerIfdefStack: LexerIfdefStackEntries

    member LexerInterpStringNesting: LexerInterpolatedStringNesting

    static member Default: LexerContinuation

and LexCont = LexerContinuation

val ParseAssemblyCodeInstructions:
    s: string -> reportLibraryOnlyFeatures: bool -> langVersion: LanguageVersion -> m: range -> ILInstr[]

val grabXmlDocAtRangeStart: parseState: IParseState * optAttributes: SynAttributeList list * range: range -> PreXmlDoc

val grabXmlDoc: parseState: IParseState * optAttributes: SynAttributeList list * elemIdx: int -> PreXmlDoc

val ParseAssemblyCodeType:
    s: string -> reportLibraryOnlyFeatures: bool -> langVersion: LanguageVersion -> m: range -> ILType

val reportParseErrorAt: range -> (int * string) -> unit

val raiseParseErrorAt: range -> (int * string) -> 'a

val mkSynMemberDefnGetSet:
    parseState: IParseState ->
    opt_inline: bool ->
    mWith: range ->
    classDefnMemberGetSetElements: (bool * SynAttributeList list * (SynPat * range) * SynReturnInfo option * range option * SynExpr * range) list ->
    mAnd: range option ->
    mWhole: range ->
    propertyNameBindingPat: SynPat ->
    optPropertyType: SynReturnInfo option ->
    visNoLongerUsed: SynAccess option ->
    memFlagsBuilder: (SynMemberKind -> SynMemberFlags) ->
    attrs: SynAttributeList list ->
    rangeStart: range ->
        SynMemberDefn list

val mkTupleOrDivide: isStruct: bool -> elementTypes: TupleTypeSegment list -> SynType
