// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.Lexhelp

open Internal.Utilities
open Internal.Utilities.Text

open FSharp.Compiler
open FSharp.Compiler.AbstractIL.Internal
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.Parser
open FSharp.Compiler.ParseHelpers
open FSharp.Compiler.Range

val stdinMockFilename: string

/// Lexer args: status of #light processing.  Mutated when a #light
/// directive is processed. This alters the behaviour of the lexfilter.
[<Sealed>]
type LightSyntaxStatus =
    new: initial:bool * warn: bool -> LightSyntaxStatus
    member ExplicitlySet: bool
    member Status: bool
    member Status: bool with set
    member WarnOnMultipleTokens: bool

[<Sealed>]
type LexResourceManager =
    new: ?capacity: int -> LexResourceManager

/// The context applicable to all lexing functions (tokens, strings etc.)
type LexArgs =
    {
      defines: string list
      resourceManager: LexResourceManager
      errorLogger: ErrorLogger
      applyLineDirectives: bool
      pathMap: PathMap
      mutable ifdefStack: LexerIfdefStack
      mutable lightStatus : LightSyntaxStatus
      mutable stringNest: LexerInterpolatedStringNesting
    }

type LongUnicodeLexResult =
    | SurrogatePair of uint16 * uint16
    | SingleChar of uint16
    | Invalid

val resetLexbufPos: string -> UnicodeLexing.Lexbuf -> unit

val mkLexargs: string list * LightSyntaxStatus * LexResourceManager * LexerIfdefStack * ErrorLogger * PathMap -> LexArgs

val reusingLexbufForParsing: UnicodeLexing.Lexbuf -> (unit -> 'a) -> 'a 

val usingLexbufForParsing: UnicodeLexing.Lexbuf * string -> (UnicodeLexing.Lexbuf -> 'a) -> 'a

type LexerStringFinisher =
    | LexerStringFinisher of (ByteBuffer -> LexerStringKind -> bool -> LexerContinuation -> token)
    
    member Finish: buf: ByteBuffer -> kind: LexerStringKind -> isInterpolatedStringPart: bool -> cont: LexerContinuation -> token

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

exception ReservedKeyword of string * Range.range

exception IndentationProblem of string * Range.range

module Keywords = 

    val KeywordOrIdentifierToken: LexArgs -> UnicodeLexing.Lexbuf -> string -> token

    val IdentifierToken: LexArgs -> UnicodeLexing.Lexbuf -> string -> token

    val DoesIdentifierNeedQuotation: string -> bool

    val QuoteIdentifierIfNeeded: string -> string

    val NormalizeIdentifierBackticks: string -> string

    val keywordNames: string list

    /// Keywords paired with their descriptions. Used in completion and quick info.
    val keywordsWithDescription: (string * string) list

