// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Microsoft.FSharp.Compiler.Lexhelp

open Internal.Utilities
open Internal.Utilities.Text
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.AbstractIL
open Microsoft.FSharp.Compiler.AbstractIL.Internal
open Microsoft.FSharp.Compiler



val internal stdinMockFilename : string

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

val internal usingLexbufForParsing : UnicodeLexing.Lexbuf * string -> (UnicodeLexing.Lexbuf -> 'a) -> 'a
val internal defaultStringFinisher : 'a -> 'b -> byte[] -> Parser.token
val internal callStringFinisher : ('a -> 'b -> byte[] -> 'c) -> AbstractIL.Internal.ByteBuffer -> 'a -> 'b -> 'c
val internal addUnicodeString : AbstractIL.Internal.ByteBuffer -> string -> unit
val internal addUnicodeChar : AbstractIL.Internal.ByteBuffer -> int -> unit
val internal addByteChar : AbstractIL.Internal.ByteBuffer -> char -> unit
val internal stringBufferAsString : byte[] -> string
val internal stringBufferAsBytes : AbstractIL.Internal.ByteBuffer -> byte[]
val internal stringBufferIsBytes : AbstractIL.Internal.ByteBuffer -> bool
val internal newline : Lexing.LexBuffer<'a> -> unit
val internal trigraph : char -> char -> char -> char
val internal digit : char -> int32
val internal hexdigit : char -> int32
val internal unicodeGraphShort : string -> uint16
val internal hexGraphShort : string -> uint16
val internal unicodeGraphLong : string -> LongUnicodeLexResult
val internal escape : char -> char

exception internal ReservedKeyword of string * Range.range
exception internal IndentationProblem of string * Range.range

module Keywords = 
    val internal KeywordOrIdentifierToken : lexargs -> UnicodeLexing.Lexbuf -> string -> Parser.token
    val internal IdentifierToken : lexargs -> UnicodeLexing.Lexbuf -> string -> Parser.token
    val internal QuoteIdentifierIfNeeded : string -> string
    val mutable internal permitFsharpKeywords : bool 
    val keywordNames : string list
