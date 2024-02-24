module internal FSharp.Compiler.AbstractIL.AsciiLexer

open Internal.Utilities.Text.Lexing
open FSharp.Compiler.AbstractIL.AsciiParser

/// Rule token
val token: lexbuf: LexBuffer<char> -> token
