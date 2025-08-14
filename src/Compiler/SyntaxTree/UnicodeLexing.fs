// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Functions for Unicode char-based lexing
module internal FSharp.Compiler.UnicodeLexing

open System.IO
open Internal.Utilities.Text.Lexing

type Lexbuf = LexBuffer<char>

type LexBuffer<'char> with

    member lexbuf.GetLocalData<'T when 'T: not null>(key: string, initializer) =
        match lexbuf.BufferLocalStore.TryGetValue key with
        | true, data -> data :?> 'T
        | _ ->
            let data = initializer ()
            lexbuf.BufferLocalStore[key] <- data
            data

    member lexbuf.TryGetLocalData<'T when 'T: not null>(key: string) =
        match lexbuf.BufferLocalStore.TryGetValue key with
        | true, data -> Some(data :?> 'T)
        | _ -> None

let StringAsLexbuf (reportLibraryOnlyFeatures, langVersion, strictIndentation, s: string) =
    LexBuffer<char>.FromChars(reportLibraryOnlyFeatures, langVersion, strictIndentation, s.ToCharArray())

let FunctionAsLexbuf (reportLibraryOnlyFeatures, langVersion, strictIndentation, bufferFiller) =
    LexBuffer<char>.FromFunction(reportLibraryOnlyFeatures, langVersion, strictIndentation, bufferFiller)

let SourceTextAsLexbuf (reportLibraryOnlyFeatures, langVersion, strictIndentation, sourceText) =
    LexBuffer<char>.FromSourceText(reportLibraryOnlyFeatures, langVersion, strictIndentation, sourceText)

let StreamReaderAsLexbuf (reportLibraryOnlyFeatures, langVersion, strictIndentation, reader: StreamReader) =
    let mutable isFinished = false

    FunctionAsLexbuf(
        reportLibraryOnlyFeatures,
        langVersion,
        strictIndentation,
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
