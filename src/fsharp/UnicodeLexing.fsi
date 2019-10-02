// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.UnicodeLexing

open FSharp.Compiler.Features
open FSharp.Compiler.Text
open Microsoft.FSharp.Text
open Internal.Utilities.Text.Lexing

type Lexbuf =  LexBuffer<char>
val internal StringAsLexbuf: (Features.LanguageFeature -> bool) * string -> Lexbuf
val public FunctionAsLexbuf: (Features.LanguageFeature -> bool) * (char [] * int * int -> int) -> Lexbuf
val public UnicodeFileAsLexbuf: (Features.LanguageFeature -> bool) * string * int option * (*retryLocked*) bool -> Lexbuf
val public SourceTextAsLexbuf: (Features.LanguageFeature -> bool) * ISourceText -> Lexbuf
