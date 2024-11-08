// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.LexerStore

open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.SyntaxTrivia
open FSharp.Compiler.UnicodeLexing
open FSharp.Compiler.Text
open FSharp.Compiler.Xml

val getSynArgNameGenerator: Lexbuf -> SynArgNameGenerator

[<RequireQualifiedAccess>]
module XmlDocStore =

    val SaveXmlDocLine: lexbuf: Lexbuf * lineText: string * range: range -> unit

    val GrabXmlDocBeforeMarker: lexbuf: Lexbuf * markerRange: range -> PreXmlDoc

    val AddGrabPoint: lexbuf: Lexbuf -> unit

    val AddGrabPointDelayed: lexbuf: Lexbuf -> unit

    val ReportInvalidXmlDocPositions: lexbuf: Lexbuf -> range list

type LexerIfdefExpression =
    | IfdefAnd of LexerIfdefExpression * LexerIfdefExpression
    | IfdefOr of LexerIfdefExpression * LexerIfdefExpression
    | IfdefNot of LexerIfdefExpression
    | IfdefId of string

val LexerIfdefEval: lookup: (string -> bool) -> _arg1: LexerIfdefExpression -> bool

[<RequireQualifiedAccess>]
module IfdefStore =

    val SaveIfHash: lexbuf: Lexbuf * lexed: string * expr: LexerIfdefExpression * range: range -> unit

    val SaveElseHash: lexbuf: Lexbuf * lexed: string * range: range -> unit

    val SaveEndIfHash: lexbuf: Lexbuf * lexed: string * range: range -> unit

    val GetTrivia: lexbuf: Lexbuf -> ConditionalDirectiveTrivia list

[<RequireQualifiedAccess>]
module CommentStore =

    val SaveSingleLineComment: lexbuf: Lexbuf * startRange: range * endRange: range -> unit

    val SaveBlockComment: lexbuf: Lexbuf * startRange: range * endRange: range -> unit

    val GetComments: lexbuf: Lexbuf -> CommentTrivia list
