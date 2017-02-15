// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Microsoft.FSharp.Compiler.Lexhelp

open Internal.Utilities
open Internal.Utilities.Text
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.AbstractIL
open Microsoft.FSharp.Compiler.AbstractIL.Internal
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library
open Microsoft.FSharp.Compiler



val stdinMockFilename : string

[<Sealed>]
type LightSyntaxStatus =
    new : initial:bool * warn : bool -> LightSyntaxStatus
    member ExplicitlySet : bool
    member Status : bool
    member Status : bool with set
    member WarnOnMultipleTokens : bool

[<Sealed>]
type LexResourceManager =
    new : unit -> LexResourceManager

type lexargs =
  { defines: string list;
    ifdefStack: LexerIfdefStack;
    resourceManager: LexResourceManager;
    lightSyntaxStatus: LightSyntaxStatus;
    errorLogger: ErrorLogger}

type LongUnicodeLexResult =
    | SurrogatePair of uint16 * uint16
    | SingleChar of uint16
    | Invalid

val resetLexbufPos : string -> UnicodeLexing.Lexbuf -> unit
val mkLexargs : 'a * string list * LightSyntaxStatus * LexResourceManager * LexerIfdefStack * ErrorLogger -> lexargs
val reusingLexbufForParsing : UnicodeLexing.Lexbuf -> (unit -> 'a) -> 'a 

val usingLexbufForParsing : UnicodeLexing.Lexbuf * string -> (UnicodeLexing.Lexbuf -> 'a) -> 'a
val defaultStringFinisher : 'a -> 'b -> byte[] -> Parser.token
val callStringFinisher : ('a -> 'b -> byte[] -> 'c) -> AbstractIL.Internal.ByteBuffer -> 'a -> 'b -> 'c
val addUnicodeString : AbstractIL.Internal.ByteBuffer -> string -> unit
val addUnicodeChar : AbstractIL.Internal.ByteBuffer -> int -> unit
val addByteChar : AbstractIL.Internal.ByteBuffer -> char -> unit
val stringBufferAsString : byte[] -> string
val stringBufferAsBytes : AbstractIL.Internal.ByteBuffer -> byte[]
val stringBufferIsBytes : AbstractIL.Internal.ByteBuffer -> bool
val newline : Lexing.LexBuffer<'a> -> unit
val trigraph : char -> char -> char -> char
val digit : char -> int32
val hexdigit : char -> int32
val unicodeGraphShort : string -> uint16
val hexGraphShort : string -> uint16
val unicodeGraphLong : string -> LongUnicodeLexResult
val escape : char -> char

exception ReservedKeyword of string * Range.range
exception IndentationProblem of string * Range.range

module Keywords = 
    val KeywordOrIdentifierToken : lexargs -> UnicodeLexing.Lexbuf -> string -> Parser.token
    val IdentifierToken : lexargs -> UnicodeLexing.Lexbuf -> string -> Parser.token
    val QuoteIdentifierIfNeeded : string -> string
    val NormalizeIdentifierBackticks : string -> string
    val keywordNames : string list
    val keywordTypes : Set<string>
    /// Keywords paired with their descriptions. Used in completion and quick info.
    val keywordsWithDescription : (string * string) list
