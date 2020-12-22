// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.ParseHelpers

open FSharp.Compiler.AbstractIL
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.Pos
open FSharp.Compiler.Range
open Internal.Utilities.Text.Lexing
open Internal.Utilities.Text.Parsing

/// The error raised by the parse_error_rich function, which is called by the parser engine
/// when a syntax error occurs. The first object is the ParseErrorContext which contains a dump of
/// information about the grammar at the point where the error occurred, e.g. what tokens
/// are valid to shift next at that point in the grammar. This information is processed in CompileOps.fs.
[<NoEquality; NoComparison>]
exception SyntaxError of obj * range: Range

exception IndentationProblem of string * Range

val warningStringOfCoords: line:int -> column:int -> string

val warningStringOfPos: p:Pos -> string

val posOfLexPosition: p:Position -> Pos

val mkSynRange: p1:Position -> p2:Position -> Range

type LexBuffer<'Char> with
    member LexemeRange: Range

val lhs: parseState:IParseState -> Range

val rhs2: parseState:IParseState -> i:int -> j:int -> Range

val rhs: parseState:IParseState -> i:int -> Range

type IParseState with
    member SynArgNameGenerator: SyntaxTreeOps.SynArgNameGenerator
    member ResetSynArgNameGenerator: unit -> unit

module LexbufLocalXmlDocStore =
    val ClearXmlDoc: lexbuf:UnicodeLexing.Lexbuf -> unit
    val SaveXmlDocLine: lexbuf:UnicodeLexing.Lexbuf * lineText:string * range:Range -> unit
    val GrabXmlDocBeforeMarker: lexbuf:UnicodeLexing.Lexbuf * markerRange:Range -> XmlDoc.PreXmlDoc
  
type LexerIfdefStackEntry =
    | IfDefIf
    | IfDefElse

type LexerIfdefStackEntries = (LexerIfdefStackEntry * Range) list

type LexerIfdefStack = LexerIfdefStackEntries

type LexerEndlineContinuation =
    | Token
    | Skip of int * range: Range

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
    (int * LexerStringStyle * Range) list

[<RequireQualifiedAccess; NoComparison;NoEquality>]
type LexerContinuation =
    | Token of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting
    | IfDefSkip of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting * int * range: Range
    | String of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting * style: LexerStringStyle * kind: LexerStringKind * range: Range
    | Comment of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting * int * range: Range
    | SingleLineComment of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting * int * range: Range
    | StringInComment of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting * style: LexerStringStyle * int * range: Range
    | MLOnly of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting * range: Range
    | EndLine of ifdef: LexerIfdefStackEntries * nesting: LexerInterpolatedStringNesting * LexerEndlineContinuation

    member LexerIfdefStack: LexerIfdefStackEntries

    member LexerInterpStringNesting: LexerInterpolatedStringNesting

    static member Default: LexerContinuation
    
and LexCont = LexerContinuation

val ParseAssemblyCodeInstructions: s:string -> isFeatureSupported:(Features.LanguageFeature -> bool) -> m:Range -> ILInstr[]

val ParseAssemblyCodeType: s:string -> isFeatureSupported:(Features.LanguageFeature -> bool) -> m:Range -> ILType
