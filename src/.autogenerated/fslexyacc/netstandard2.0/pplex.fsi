module internal FSharp.Compiler.PPLexer

open FSharp.Compiler.Lexhelp
open Internal.Utilities.Text.Lexing
open FSharp.Compiler.PPParser

/// Rule tokenstream
val tokenstream: args: LexArgs -> lexbuf: LexBuffer<char> -> token
/// Rule rest
val rest: lexbuf: LexBuffer<char> -> token
