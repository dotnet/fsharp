// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal Microsoft.FSharp.Compiler.UnicodeLexing

open Microsoft.FSharp.Text
open Internal.Utilities.Text.Lexing

type Lexbuf =  LexBuffer<uint16>
val internal StringAsLexbuf : string -> Lexbuf
val public FunctionAsLexbuf : (uint16[] * int * int -> int) -> Lexbuf
val public UnicodeFileAsLexbuf :string * int option * (*retryLocked*) bool -> Lexbuf
