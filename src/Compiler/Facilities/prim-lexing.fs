// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

#nowarn "47" // recursive initialization of LexBuffer

namespace FSharp.Compiler.Text

open System
open System.IO
open System.Collections.Immutable
open Internal.Utilities.Library

open Internal.Utilities.Collections
open Internal.Utilities.Hashing

type ISourceText =

    abstract Item: index: int -> char with get

    abstract GetLineString: lineIndex: int -> string

    abstract GetLineCount: unit -> int

    abstract GetLastCharacterPosition: unit -> int * int

    abstract GetSubTextString: start: int * length: int -> string

    abstract SubTextEquals: target: string * startIndex: int -> bool

    abstract Length: int

    abstract ContentEquals: sourceText: ISourceText -> bool

    abstract CopyTo: sourceIndex: int * destination: char[] * destinationIndex: int * count: int -> unit

    abstract GetSubTextFromRange: range: range -> string

type ISourceTextNew =
    inherit ISourceText

    abstract GetChecksum: unit -> System.Collections.Immutable.ImmutableArray<byte>

[<Sealed>]
type StringText(str: string) =

    let getLines (str: string) =
        use reader = new StringReader(str)

        [|
            let mutable line = reader.ReadLine()

            while not (isNull line) do
                yield !!line
                line <- reader.ReadLine()

            if str.EndsWith("\n", StringComparison.Ordinal) then
                // last trailing space not returned
                // http://stackoverflow.com/questions/19365404/stringreader-omits-trailing-linebreak
                yield String.Empty
        |]

    let getLines =
        // This requires allocating and getting all the lines.
        // However, likely whoever is calling it is using a different implementation of ISourceText
        // So, it's ok that we do this for now.
        lazy getLines str

    member _.String = str

    override _.GetHashCode() = str.GetHashCode()

    override _.Equals(obj: obj) =
        match obj with
        | :? StringText as other -> other.String.Equals(str)
        | :? string as other -> other.Equals(str)
        | _ -> false

    override _.ToString() = str

    interface ISourceTextNew with

        member _.Item
            with get index = str[index]

        member _.GetLastCharacterPosition() =
            let lines = getLines.Value

            if lines.Length > 0 then
                (lines.Length, lines[lines.Length - 1].Length)
            else
                (0, 0)

        member _.GetLineString(lineIndex) = getLines.Value[lineIndex]

        member _.GetLineCount() = getLines.Value.Length

        member _.GetSubTextString(start, length) = str.Substring(start, length)

        member _.SubTextEquals(target, startIndex) =
            if startIndex < 0 || startIndex >= str.Length then
                invalidArg "startIndex" "Out of range."

            if String.IsNullOrEmpty(target) then
                invalidArg "target" "Is null or empty."

            let lastIndex = startIndex + target.Length

            if lastIndex <= startIndex || lastIndex >= str.Length then
                invalidArg "target" "Too big."

            str.IndexOfOrdinal(target, startIndex, target.Length) <> -1

        member _.Length = str.Length

        member this.ContentEquals(sourceText) =
            match sourceText with
            | :? StringText as sourceText when sourceText = this || sourceText.String = str -> true
            | _ -> false

        member _.CopyTo(sourceIndex, destination, destinationIndex, count) =
            str.CopyTo(sourceIndex, destination, destinationIndex, count)

        member this.GetSubTextFromRange(range) =
            let totalAmountOfLines = getLines.Value.Length

            if
                range.StartLine = 0
                && range.StartColumn = 0
                && range.EndLine = 0
                && range.EndColumn = 0
            then
                String.Empty
            elif
                range.StartLine < 1
                || (range.StartLine - 1) > totalAmountOfLines
                || range.EndLine < 1
                || (range.EndLine - 1) > totalAmountOfLines
            then
                invalidArg (nameof range) "The range is outside the file boundaries"
            else
                let sourceText = this :> ISourceText
                let startLine = range.StartLine - 1
                let line = sourceText.GetLineString startLine

                if range.StartLine = range.EndLine then
                    let length = range.EndColumn - range.StartColumn
                    line.Substring(range.StartColumn, length)
                else
                    let firstLineContent = line.Substring(range.StartColumn)
                    let sb = System.Text.StringBuilder().AppendLine(firstLineContent)

                    for lineNumber in range.StartLine .. range.EndLine - 2 do
                        sb.AppendLine(sourceText.GetLineString lineNumber) |> ignore

                    let lastLine = sourceText.GetLineString(range.EndLine - 1)
                    sb.Append(lastLine.Substring(0, range.EndColumn)).ToString()

        member _.GetChecksum() =
            str
            |> Md5Hasher.hashString
            |> fun byteArray -> ImmutableArray.Create<byte>(byteArray, 0, byteArray.Length)

module SourceText =

    let ofString str = StringText(str) :> ISourceText

module SourceTextNew =

    let ofString str = StringText(str) :> ISourceTextNew

    let ofISourceText (sourceText: ISourceText) =
        { new ISourceTextNew with
            member _.Item
                with get index = sourceText[index]

            member _.GetLineString(x) = sourceText.GetLineString(x)

            member _.GetLineCount() = sourceText.GetLineCount()

            member _.GetLastCharacterPosition() = sourceText.GetLastCharacterPosition()

            member _.GetSubTextString(x, y) = sourceText.GetSubTextString(x, y)

            member _.SubTextEquals(x, y) = sourceText.SubTextEquals(x, y)

            member _.Length = sourceText.Length

            member _.ContentEquals(x) = sourceText.ContentEquals(x)

            member _.CopyTo(a, b, c, d) = sourceText.CopyTo(a, b, c, d)

            member _.GetSubTextFromRange(x) = sourceText.GetSubTextFromRange(x)

            member _.GetChecksum() =
                // TODO: something better...
                !!sourceText.ToString()
                |> Md5Hasher.hashString
                |> fun byteArray -> ImmutableArray.Create<byte>(byteArray, 0, byteArray.Length)
        }

// NOTE: the code in this file is a drop-in replacement runtime for Lexing.fs from the FsLexYacc repository

namespace Internal.Utilities.Text.Lexing

open FSharp.Compiler.Text
open Microsoft.FSharp.Core
open Microsoft.FSharp.Collections
open FSharp.Compiler.Features
open System.Collections.Generic

[<Struct>]
type internal Position =
    val FileIndex: int
    val Line: int
    val OriginalLine: int
    val AbsoluteOffset: int
    val StartOfLineAbsoluteOffset: int
    member x.Column = x.AbsoluteOffset - x.StartOfLineAbsoluteOffset

    new(fileIndex: int, line: int, originalLine: int, startOfLineAbsoluteOffset: int, absoluteOffset: int) =
        {
            FileIndex = fileIndex
            Line = line
            OriginalLine = originalLine
            AbsoluteOffset = absoluteOffset
            StartOfLineAbsoluteOffset = startOfLineAbsoluteOffset
        }

    member x.NextLine =
        Position(x.FileIndex, x.Line + 1, x.OriginalLine + 1, x.AbsoluteOffset, x.AbsoluteOffset)

    member x.EndOfToken n =
        Position(x.FileIndex, x.Line, x.OriginalLine, x.StartOfLineAbsoluteOffset, x.AbsoluteOffset + n)

    member x.ShiftColumnBy by =
        Position(x.FileIndex, x.Line, x.OriginalLine, x.StartOfLineAbsoluteOffset, x.AbsoluteOffset + by)

    member x.ColumnMinusOne =
        Position(x.FileIndex, x.Line, x.OriginalLine, x.StartOfLineAbsoluteOffset, x.StartOfLineAbsoluteOffset - 1)

    member x.ApplyLineDirective(fileIdx, line) =
        Position(fileIdx, line, x.OriginalLine + 1, x.AbsoluteOffset, x.AbsoluteOffset)

    override p.ToString() = $"({p.Line},{p.Column})"

    static member Empty = Position()

    static member FirstLine fileIdx = Position(fileIdx, 1, 1, 0, 0)

type internal LexBufferFiller<'Char> = LexBuffer<'Char> -> unit

and [<Sealed>] internal LexBuffer<'Char>
    (filler: LexBufferFiller<'Char>, reportLibraryOnlyFeatures: bool, langVersion: LanguageVersion, strictIndentation: bool option) =
    let context = Dictionary<string, obj>(1)
    let mutable buffer = [||]
    /// number of valid characters beyond bufferScanStart.
    let mutable bufferMaxScanLength = 0
    /// count into the buffer when scanning.
    let mutable bufferScanStart = 0
    /// number of characters scanned so far.
    let mutable bufferScanLength = 0
    /// length of the scan at the last accepting state.
    let mutable lexemeLength = 0
    /// action related to the last accepting state.
    let mutable bufferAcceptAction = 0
    let mutable eof = false
    let mutable startPos = Position.Empty
    let mutable endPos = Position.Empty

    // Throw away all the input besides the lexeme, which is placed at start of buffer
    let discardInput () =
        Array.blit buffer bufferScanStart buffer 0 bufferScanLength
        bufferScanStart <- 0
        bufferMaxScanLength <- bufferScanLength

    member lexbuf.EndOfScan() : int =
        //Printf.eprintf "endOfScan, lexBuffer.lexemeLength = %d\n" lexBuffer.lexemeLength;
        if bufferAcceptAction < 0 then
            failwith "unrecognized input"

        //printf "endOfScan %d state %d on unconsumed input '%c' (%d)\n" a s (Char.chr inp) inp;
        //Printf.eprintf "accept, lexeme = %s\n" (lexeme lexBuffer);
        lexbuf.StartPos <- endPos
        lexbuf.EndPos <- endPos.EndOfToken(lexbuf.LexemeLength)
        bufferAcceptAction

    member lexbuf.StartPos
        with get () = startPos
        and set b = startPos <- b

    member lexbuf.EndPos
        with get () = endPos
        and set b = endPos <- b

    member lexbuf.LexemeView =
        System.ReadOnlySpan<'Char>(buffer, bufferScanStart, lexemeLength)

    member lexbuf.LexemeChar n = buffer[n + bufferScanStart]

    member lexbuf.LexemeContains(c: 'Char) =
        array.IndexOf(buffer, c, bufferScanStart, lexemeLength) >= 0

    member lexbuf.BufferLocalStore = (context :> IDictionary<_, _>)

    member lexbuf.LexemeLength
        with get (): int = lexemeLength
        and set v = lexemeLength <- v

    member lexbuf.Buffer
        with get (): 'Char[] = buffer
        and set v = buffer <- v

    member lexbuf.BufferMaxScanLength
        with get () = bufferMaxScanLength
        and set v = bufferMaxScanLength <- v

    member lexbuf.BufferScanLength
        with get () = bufferScanLength
        and set v = bufferScanLength <- v

    member lexbuf.BufferScanStart
        with get (): int = bufferScanStart
        and set v = bufferScanStart <- v

    member lexbuf.BufferAcceptAction
        with get () = bufferAcceptAction
        and set v = bufferAcceptAction <- v

    member lexbuf.RefillBuffer() = filler lexbuf

    static member LexemeString(lexbuf: LexBuffer<char>) =
        System.String(lexbuf.Buffer, lexbuf.BufferScanStart, lexbuf.LexemeLength)

    member lexbuf.IsPastEndOfStream
        with get () = eof
        and set b = eof <- b

    member lexbuf.DiscardInput() = discardInput ()

    member x.BufferScanPos = bufferScanStart + bufferScanLength

    member lexbuf.EnsureBufferSize n =
        if lexbuf.BufferScanPos + n >= buffer.Length then
            let repl = Array.zeroCreate (lexbuf.BufferScanPos + n)
            Array.blit buffer bufferScanStart repl bufferScanStart bufferScanLength
            buffer <- repl

    member _.ReportLibraryOnlyFeatures = reportLibraryOnlyFeatures

    member _.LanguageVersion = langVersion

    member _.SupportsFeature featureId = langVersion.SupportsFeature featureId

    member _.StrictIndentation = strictIndentation

    member _.CheckLanguageFeatureAndRecover featureId range =
        FSharp.Compiler.DiagnosticsLogger.checkLanguageFeatureAndRecover langVersion featureId range

    static member FromFunction
        (reportLibraryOnlyFeatures, langVersion, strictIndentation, f: 'Char[] * int * int -> int)
        : LexBuffer<'Char> =
        let extension = Array.zeroCreate 4096

        let filler (lexBuffer: LexBuffer<'Char>) =
            let n = f (extension, 0, extension.Length)
            lexBuffer.EnsureBufferSize n
            Array.blit extension 0 lexBuffer.Buffer lexBuffer.BufferScanPos n
            lexBuffer.BufferMaxScanLength <- lexBuffer.BufferScanLength + n

        new LexBuffer<'Char>(filler, reportLibraryOnlyFeatures, langVersion, strictIndentation)

    // Important: This method takes ownership of the array
    static member FromArrayNoCopy(reportLibraryOnlyFeatures, langVersion, strictIndentation, buffer: 'Char[]) : LexBuffer<'Char> =
        let lexBuffer =
            new LexBuffer<'Char>((fun _ -> ()), reportLibraryOnlyFeatures, langVersion, strictIndentation)

        lexBuffer.Buffer <- buffer
        lexBuffer.BufferMaxScanLength <- buffer.Length
        lexBuffer

    // Important: this method does copy the array
    static member FromArray(reportLibraryOnlyFeatures, langVersion, strictIndentation, s: 'Char[]) : LexBuffer<'Char> =
        let buffer = Array.copy s

        LexBuffer<'Char>.FromArrayNoCopy(reportLibraryOnlyFeatures, langVersion, strictIndentation, buffer)

    // Important: This method takes ownership of the array
    static member FromChars(reportLibraryOnlyFeatures, langVersion, strictIndentation, arr: char[]) =
        LexBuffer.FromArrayNoCopy(reportLibraryOnlyFeatures, langVersion, strictIndentation, arr)

    static member FromSourceText(reportLibraryOnlyFeatures, langVersion, strictIndentation, sourceText: ISourceText) =
        let mutable currentSourceIndex = 0

        LexBuffer<char>
            .FromFunction(
                reportLibraryOnlyFeatures,
                langVersion,
                strictIndentation,
                fun (chars, start, length) ->
                    let lengthToCopy =
                        if currentSourceIndex + length <= sourceText.Length then
                            length
                        else
                            sourceText.Length - currentSourceIndex

                    if lengthToCopy <= 0 then
                        0
                    else
                        sourceText.CopyTo(currentSourceIndex, chars, start, lengthToCopy)
                        currentSourceIndex <- currentSourceIndex + lengthToCopy
                        lengthToCopy
            )

module GenericImplFragments =
    let startInterpret (lexBuffer: LexBuffer<char>) =
        lexBuffer.BufferScanStart <- lexBuffer.BufferScanStart + lexBuffer.LexemeLength
        lexBuffer.BufferMaxScanLength <- lexBuffer.BufferMaxScanLength - lexBuffer.LexemeLength
        lexBuffer.BufferScanLength <- 0
        lexBuffer.LexemeLength <- 0
        lexBuffer.BufferAcceptAction <- -1

    let afterRefill (trans: uint16[][], sentinel, lexBuffer: LexBuffer<char>, scanUntilSentinel, endOfScan, state, eofPos) =
        // end of file occurs if we couldn't extend the buffer
        if lexBuffer.BufferScanLength = lexBuffer.BufferMaxScanLength then
            let snew = int trans[state].[eofPos] // == EOF

            if snew = sentinel then
                endOfScan ()
            else
                if lexBuffer.IsPastEndOfStream then
                    failwith "End of file on lexing stream"

                lexBuffer.IsPastEndOfStream <- true
                //printf "state %d --> %d on eof\n" state snew;
                scanUntilSentinel lexBuffer snew
        else
            scanUntilSentinel lexBuffer state

    let onAccept (lexBuffer: LexBuffer<char>, a) =
        lexBuffer.LexemeLength <- lexBuffer.BufferScanLength
        lexBuffer.BufferAcceptAction <- a

open GenericImplFragments

[<Sealed>]
type internal UnicodeTables(trans: uint16[] array, accept: uint16[]) =
    let sentinel = 255 * 256 + 255
    let numUnicodeCategories = 30
    let numLowUnicodeChars = 128

    let numSpecificUnicodeChars =
        (trans[0].Length - 1 - numLowUnicodeChars - numUnicodeCategories) / 2

    let lookupUnicodeCharacters state inp =
        let inpAsInt = int inp
        // Is it a fast ASCII character?
        if inpAsInt < numLowUnicodeChars then
            int trans[state].[inpAsInt]
        else
            // Search for a specific unicode character
            let baseForSpecificUnicodeChars = numLowUnicodeChars

            let rec loop i =
                if i >= numSpecificUnicodeChars then
                    // OK, if we failed then read the 'others' entry in the alphabet,
                    // which covers all Unicode characters not covered in other
                    // ways
                    let baseForUnicodeCategories = numLowUnicodeChars + numSpecificUnicodeChars * 2
                    let unicodeCategory = System.Char.GetUnicodeCategory(inp)
                    //System.Console.WriteLine("inp = {0}, unicodeCategory = {1}", [| box inp; box unicodeCategory |]);
                    int trans[state].[baseForUnicodeCategories + int32 unicodeCategory]
                else
                    // This is the specific unicode character
                    let c = char (int trans[state].[baseForSpecificUnicodeChars + i * 2])
                    //System.Console.WriteLine("c = {0}, inp = {1}, i = {2}", [| box c; box inp; box i |]);
                    // OK, have we found the entry for a specific unicode character?
                    if c = inp then
                        int trans[state].[baseForSpecificUnicodeChars + i * 2 + 1]
                    else
                        loop (i + 1)

            loop 0

    let eofPos = numLowUnicodeChars + 2 * numSpecificUnicodeChars + numUnicodeCategories

    let rec scanUntilSentinel lexBuffer state =
        // Return an endOfScan after consuming the input
        let a = int accept[state]

        if a <> sentinel then
            onAccept (lexBuffer, a)

        if lexBuffer.BufferScanLength = lexBuffer.BufferMaxScanLength then
            lexBuffer.DiscardInput()
            lexBuffer.RefillBuffer()
            // end of file occurs if we couldn't extend the buffer
            afterRefill (trans, sentinel, lexBuffer, scanUntilSentinel, lexBuffer.EndOfScan, state, eofPos)
        else
            // read a character - end the scan if there are no further transitions
            let inp = lexBuffer.Buffer[lexBuffer.BufferScanPos]

            // Find the new state
            let snew = lookupUnicodeCharacters state inp

            if snew = sentinel then
                lexBuffer.EndOfScan()
            else
                lexBuffer.BufferScanLength <- lexBuffer.BufferScanLength + 1
                //printf "state %d --> %d on '%c' (%d)\n" s snew (char inp) inp;
                scanUntilSentinel lexBuffer snew

    // Each row for the Unicode table has format
    //      128 entries for ASCII characters
    //      A variable number of 2*UInt16 entries for SpecificUnicodeChars
    //      30 entries, one for each UnicodeCategory
    //      1 entry for EOF

    member tables.Interpret(initialState, lexBuffer: LexBuffer<char>) =
        startInterpret (lexBuffer)
        scanUntilSentinel lexBuffer initialState

    static member Create(trans, accept) = UnicodeTables(trans, accept)
