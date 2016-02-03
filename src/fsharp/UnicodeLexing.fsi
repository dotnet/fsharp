// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Microsoft.FSharp.Compiler.UnicodeLexing

open Microsoft.FSharp.Text
open Internal.Utilities.Text.Lexing

type Lexbuf =  LexBuffer<char>
val internal StringAsLexbuf : string -> Lexbuf
val public FunctionAsLexbuf : (char [] * int * int -> int) -> Lexbuf
val public UnicodeFileAsLexbuf :string * int option * (*retryLocked*) bool -> Lexbuf
