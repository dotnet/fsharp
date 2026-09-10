// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.UnicodeLexing

open System.IO
open FSharp.Compiler.Features
open FSharp.Compiler.Text
open Internal.Utilities.Text.Lexing

type Lexbuf = LexBuffer<char>

type LexBuffer<'char> with
    member GetLocalData<'T when 'T: not null> : key: string * initializer: (unit -> 'T) -> 'T
    member TryGetLocalData<'T when 'T: not null> : key: string -> 'T option

val StringAsLexbuf: reportLibraryOnlyFeatures: bool * langVersion: LanguageVersion * string -> Lexbuf

val FunctionAsLexbuf:
    reportLibraryOnlyFeatures: bool * langVersion: LanguageVersion * bufferFiller: (char[] * int * int -> int) -> Lexbuf

val SourceTextAsLexbuf:
    reportLibraryOnlyFeatures: bool * langVersion: LanguageVersion * sourceText: ISourceText -> Lexbuf

/// Will not dispose of the stream reader.
val StreamReaderAsLexbuf:
    reportLibraryOnlyFeatures: bool * langVersion: LanguageVersion * reader: StreamReader -> Lexbuf
