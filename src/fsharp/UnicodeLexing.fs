// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.UnicodeLexing

//------------------------------------------------------------------
// Functions for Unicode char-based lexing (new code).
//

open System.IO
open Internal.Utilities.Text.Lexing

type Lexbuf =  LexBuffer<char>

let StringAsLexbuf (supportsFeature, s: string) =
    LexBuffer<char>.FromChars (supportsFeature, s.ToCharArray())

let FunctionAsLexbuf (supportsFeature, bufferFiller) =
    LexBuffer<char>.FromFunction(supportsFeature, bufferFiller)

let SourceTextAsLexbuf (supportsFeature, sourceText) =
    LexBuffer<char>.FromSourceText(supportsFeature, sourceText)

let StreamReaderAsLexbuf (supportsFeature, reader: StreamReader) =
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
