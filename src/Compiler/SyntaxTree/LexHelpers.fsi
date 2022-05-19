// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.Lexhelp

open FSharp.Compiler.IO
open Internal.Utilities
open Internal.Utilities.Text

open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.ParseHelpers
open FSharp.Compiler.UnicodeLexing
open FSharp.Compiler.Parser
open FSharp.Compiler.Text

val stdinMockFileName: string

/// Lexer args: status of #light processing.  Mutated when a #light
/// directive is processed. This alters the behaviour of the lexfilter.
[<Sealed>]
type IndentationAwareSyntaxStatus =
    new: initial: bool * warn: bool -> IndentationAwareSyntaxStatus
    member ExplicitlySet: bool
    member Status: bool
    member Status: bool with set
    member WarnOnMultipleTokens: bool

[<Sealed>]
type LexResourceManager =
    new: ?capacity: int -> LexResourceManager

/// The context applicable to all lexing functions (tokens, strings etc.)
type LexArgs =
    { conditionalDefines: string list
      resourceManager: LexResourceManager
      diagnosticsLogger: DiagnosticsLogger
      applyLineDirectives: bool
      pathMap: PathMap
      mutable ifdefStack: LexerIfdefStack
      mutable indentationSyntaxStatus: IndentationAwareSyntaxStatus
      mutable stringNest: LexerInterpolatedStringNesting }

type LongUnicodeLexResult =
    | SurrogatePair of uint16 * uint16
    | SingleChar of uint16
    | Invalid

val resetLexbufPos: string -> Lexbuf -> unit

val mkLexargs:
    conditionalDefines: string list *
    indentationSyntaxStatus: IndentationAwareSyntaxStatus *
    resourceManager: LexResourceManager *
    ifdefStack: LexerIfdefStack *
    diagnosticsLogger: DiagnosticsLogger *
    pathMap: PathMap ->
        LexArgs

val reusingLexbufForParsing: Lexbuf -> (unit -> 'a) -> 'a

val usingLexbufForParsing: Lexbuf * string -> (Lexbuf -> 'a) -> 'a

type LexerStringFinisherContext =
    | InterpolatedPart = 1
    | Verbatim = 2
    | TripleQuote = 4

type LexerStringFinisher =
    | LexerStringFinisher of (ByteBuffer -> LexerStringKind -> LexerStringFinisherContext -> LexerContinuation -> token)

    member Finish:
        buf: ByteBuffer ->
        kind: LexerStringKind ->
        context: LexerStringFinisherContext ->
        cont: LexerContinuation ->
            token

    static member Default: LexerStringFinisher

val addUnicodeString: ByteBuffer -> string -> unit

val addUnicodeChar: ByteBuffer -> int -> unit

val addByteChar: ByteBuffer -> char -> unit

val stringBufferAsString: ByteBuffer -> string

val stringBufferAsBytes: ByteBuffer -> byte[]

val stringBufferIsBytes: ByteBuffer -> bool

val newline: Lexing.LexBuffer<'a> -> unit

val advanceColumnBy: Lexing.LexBuffer<'a> -> n: int -> unit

val trigraph: char -> char -> char -> char

val digit: char -> int32

val hexdigit: char -> int32

val unicodeGraphShort: string -> uint16

val hexGraphShort: string -> uint16

val unicodeGraphLong: string -> LongUnicodeLexResult

val escape: char -> char

exception ReservedKeyword of string * range

module Keywords =

    val KeywordOrIdentifierToken: LexArgs -> Lexbuf -> string -> token

    val IdentifierToken: LexArgs -> Lexbuf -> string -> token

    val keywordNames: string list
