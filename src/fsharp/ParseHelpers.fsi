// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module public FSharp.Compiler.ParseHelpers

open FSharp.Compiler.AbstractIL
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.Range
open Internal.Utilities.Text.Lexing
open Internal.Utilities.Text.Parsing

/// The error raised by the parse_error_rich function, which is called by the parser engine
/// when a syntax error occurs. The first object is the ParseErrorContext which contains a dump of
/// information about the grammar at the point where the error occurred, e.g. what tokens
/// are valid to shift next at that point in the grammar. This information is processed in CompileOps.fs.
[<NoEquality; NoComparison>]
exception SyntaxError of obj * range: range

exception IndentationProblem of string * range

val warningStringOfCoords: line:int -> column:int -> string

val warningStringOfPos: p:pos -> string

val internal posOfLexPosition: p:Position -> pos

val internal mkSynRange: p1:Position -> p2:Position -> range

type internal LexBuffer<'Char> with
    member internal LexemeRange: range

val internal lhs: parseState:IParseState -> range

val internal rhs2: parseState:IParseState -> i:int -> j:int -> range

val internal rhs: parseState:IParseState -> i:int -> range

type internal IParseState with
    member internal SynArgNameGenerator: SyntaxTreeOps.SynArgNameGenerator
    member internal ResetSynArgNameGenerator: unit -> unit

module LexbufLocalXmlDocStore =
    val private xmlDocKey: string
    val internal ClearXmlDoc: lexbuf:UnicodeLexing.Lexbuf -> unit
    val internal SaveXmlDocLine: lexbuf:UnicodeLexing.Lexbuf * lineText:string * range:range -> unit
    val internal GrabXmlDocBeforeMarker: lexbuf:UnicodeLexing.Lexbuf * markerRange:range -> XmlDoc.PreXmlDoc
  
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

val LexerIfdefEval: lookup:(string -> bool) -> _arg1:LexerIfdefExpression -> bool

[<RequireQualifiedAccess>]
type LexerStringStyle =
    | Verbatim
    | TripleQuote
    | SingleQuote

[<RequireQualifiedAccess; StructAttribute>]
type LexerStringKind =
    { IsByteString: bool
      IsInterpolated: bool
      IsInterpolatedFirst: bool }
    static member ByteString: LexerStringKind
    static member InterpolatedStringFirst: LexerStringKind
    static member InterpolatedStringPart: LexerStringKind
    static member String: LexerStringKind
    
type LexerInterpolatedStringNesting =
    (int * LexerStringStyle * range) list

[<RequireQualifiedAccess; NoComparison;NoEquality>]
type LexerContinuation =
    | Token of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting
    | IfDefSkip of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting * int * range: range
    | String of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting * style: LexerStringStyle * kind: LexerStringKind * range: range
    | Comment of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting * int * range: range
    | SingleLineComment of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting * int * range: range
    | StringInComment of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting * style: LexerStringStyle * int * range: range
    | MLOnly of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting * range: range
    | EndLine of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting * LexerEndlineContinuation

    member LexerIfdefStack: LexerIfdefStackEntries

    member LexerInterpStringNesting: LexerInterpolatedStringNesting

    static member Default: LexerContinuation
    
and LexCont = LexerContinuation

val internal internalParseAssemblyCodeInstructions: s:string -> isFeatureSupported:(Features.LanguageFeature -> bool) -> m:range -> ILInstr[]

val ParseAssemblyCodeInstructions: s:string -> m:range -> ILInstr array val internal internalParseAssemblyCodeType: s:string -> isFeatureSupported:(Features.LanguageFeature -> bool) -> m:range -> ILType

val ParseAssemblyCodeType: s:string -> m:range -> ILType
