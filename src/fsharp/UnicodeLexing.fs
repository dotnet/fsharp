// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Functions for Unicode char-based lexing
module internal FSharp.Compiler.UnicodeLexing

open System.IO
open Internal.Utilities.Text.Lexing

type Lexbuf = LexBuffer<char>

let StringAsLexbuf (reportLibraryOnlyFeatures, langVersion, s: string) =
    LexBuffer<char>.FromChars (reportLibraryOnlyFeatures, langVersion, s.ToCharArray())

let FunctionAsLexbuf (reportLibraryOnlyFeatures, langVersion, bufferFiller) =
    LexBuffer<char>.FromFunction (reportLibraryOnlyFeatures, langVersion, bufferFiller)

let SourceTextAsLexbuf (reportLibraryOnlyFeatures, langVersion, sourceText) =
    LexBuffer<char>.FromSourceText (reportLibraryOnlyFeatures, langVersion, sourceText)

let StreamReaderAsLexbuf (reportLibraryOnlyFeatures, langVersion, reader: StreamReader) =
    let mutable isFinished = false

    FunctionAsLexbuf(
        reportLibraryOnlyFeatures,
        langVersion,
        fun (chars, start, length) ->
            if isFinished then
                0
            else
                let nBytesRead = reader.Read(chars, start, length)

                if nBytesRead = 0 then
                    isFinished <- true
                    0
                else
                    nBytesRead
    )
