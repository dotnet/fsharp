// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.UnicodeLexing

//------------------------------------------------------------------
// Functions for Unicode char-based lexing (new code).
//

open FSharp.Compiler.AbstractIL.Internal.Library
open System.IO

open Internal.Utilities.Text.Lexing

type Lexbuf =  LexBuffer<char>

let StringAsLexbuf (supportsFeature: Features.LanguageFeature -> bool, s:string) : Lexbuf =
    LexBuffer<_>.FromChars (supportsFeature, s.ToCharArray())

let FunctionAsLexbuf (supportsFeature: Features.LanguageFeature -> bool, bufferFiller: char[] * int * int -> int) : Lexbuf =
    LexBuffer<_>.FromFunction(supportsFeature, bufferFiller)

let SourceTextAsLexbuf (supportsFeature: Features.LanguageFeature -> bool, sourceText) =
    LexBuffer<char>.FromSourceText(supportsFeature, sourceText)

let StreamReaderAsLexbuf (supportsFeature: Features.LanguageFeature -> bool, reader: StreamReader) =
    let mutable isFinished = false
    FunctionAsLexbuf (supportsFeature, fun (chars, start, length) ->
        if isFinished then 0
        else
            let nBytesRead = reader.Read(chars, start, length)
            if nBytesRead = 0 then
                isFinished <- true
                0
            else
                nBytesRead
    )
