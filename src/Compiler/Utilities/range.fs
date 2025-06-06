// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Anything to do with special names of identifiers and other lexical rules
namespace FSharp.Compiler.Text

open System
open System.IO
open System.Collections.Concurrent
open System.Collections.Generic
open Microsoft.FSharp.Core.Printf
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras.Bits
open FSharp.Compiler.IO
open Internal.Utilities.Library.Extras

type FileIndex = int32

[<AutoOpen>]
module PosImpl =
    [<Literal>]
    let columnBitCount = 22

    [<Literal>]
    let lineBitCount = 31

    let posBitCount = lineBitCount + columnBitCount

    let posColumnMask = mask64 0 columnBitCount

    let lineColumnMask = mask64 columnBitCount lineBitCount

[<Struct; CustomEquality; NoComparison>]
[<System.Diagnostics.DebuggerDisplay("{Line},{Column}")>]
type Position(code: int64) =

    new(l, c) =
        let l = max 0 l
        let c = max 0 c

        let p =
            (int64 c &&& posColumnMask)
            ||| ((int64 l <<< columnBitCount) &&& lineColumnMask)

        pos p

    member p.Line = int32 (uint64 code >>> columnBitCount)

    member p.Column = int32 (code &&& posColumnMask)

    member r.Encoding = code

    static member EncodingSize = posBitCount

    static member Decode(code: int64) : pos = Position(code)

    override p.Equals(obj) =
        match obj with
        | :? Position as p2 -> code = p2.Encoding
        | _ -> false

    override p.GetHashCode() = hash code

    override p.ToString() = sprintf "(%d,%d)" p.Line p.Column

    member p.IsAdjacentTo(otherPos: Position) =
        p.Line = otherPos.Line && p.Column + 1 = otherPos.Column

and pos = Position

[<RequireQualifiedAccess>]
type NotedSourceConstruct =
    | None
    | While
    | For
    | InOrTo
    | Try
    | Binding
    | Finally
    | With
    | Combine
    | DelayOrQuoteOrRun

[<AutoOpen>]
module RangeImpl =
    [<Literal>]
    let fileIndexBitCount = 20

    [<Literal>]
    let startColumnBitCount = columnBitCount // 22

    [<Literal>]
    let endColumnBitCount = columnBitCount // 22

    [<Literal>]
    let startLineBitCount = lineBitCount // 31

    [<Literal>]
    let heightBitCount = 27

    [<Literal>]
    let isSyntheticBitCount = 1

    [<Literal>]
    let debugPointKindBitCount = 4

    [<Literal>]
    let fileIndexShift = 0

    [<Literal>]
    let startColumnShift = 20

    [<Literal>]
    let endColumnShift = 42

    [<Literal>]
    let startLineShift = 0

    [<Literal>]
    let heightShift = 31

    [<Literal>]
    let isSyntheticShift = 58

    [<Literal>]
    let debugPointKindShift = 59

    [<Literal>]
    let fileIndexMask =
        0b0000000000000000000000000000000000000000000011111111111111111111L

    [<Literal>]
    let startColumnMask =
        0b0000000000000000000000111111111111111111111100000000000000000000L

    [<Literal>]
    let endColumnMask =
        0b1111111111111111111111000000000000000000000000000000000000000000L

    [<Literal>]
    let startLineMask =
        0b0000000000000000000000000000000001111111111111111111111111111111L

    [<Literal>]
    let heightMask = 0b0000001111111111111111111111111110000000000000000000000000000000L

    [<Literal>]
    let isSyntheticMask =
        0b0000010000000000000000000000000000000000000000000000000000000000L

    [<Literal>]
    let debugPointKindMask =
        0b0111100000000000000000000000000000000000000000000000000000000000L

#if DEBUG
    let _ = assert (posBitCount <= 64)
    let _ = assert (fileIndexBitCount + startColumnBitCount + endColumnBitCount <= 64)

    let _ =
        assert
            (startLineBitCount
             + heightBitCount
             + isSyntheticBitCount
             + debugPointKindBitCount
             <= 64)

    let _ = assert (startColumnShift = fileIndexShift + fileIndexBitCount)
    let _ = assert (endColumnShift = startColumnShift + startColumnBitCount)

    let _ = assert (heightShift = startLineShift + startLineBitCount)
    let _ = assert (isSyntheticShift = heightShift + heightBitCount)
    let _ = assert (debugPointKindShift = isSyntheticShift + isSyntheticBitCount)

    let _ = assert (fileIndexMask = mask64 fileIndexShift fileIndexBitCount)
    let _ = assert (startLineMask = mask64 startLineShift startLineBitCount)
    let _ = assert (startColumnMask = mask64 startColumnShift startColumnBitCount)
    let _ = assert (heightMask = mask64 heightShift heightBitCount)
    let _ = assert (endColumnMask = mask64 endColumnShift endColumnBitCount)
    let _ = assert (isSyntheticMask = mask64 isSyntheticShift isSyntheticBitCount)

    let _ =
        assert (debugPointKindMask = mask64 debugPointKindShift debugPointKindBitCount)
#endif

/// A unique-index table for file names.
type FileIndexTable() =
    let indexToFileTable = ResizeArray<_>(11)
    let fileToIndexTable = ConcurrentDictionary<string, int>()

    // Note: we should likely adjust this code to always normalize. However some testing (and possibly some
    // product behaviour) appears to be sensitive to error messages reporting un-normalized file names.
    // Currently all names going through 'mkRange' get normalized, while this going through just 'fileIndexOfFile'
    // do not.  Also any file names which are not put into ranges at all are non-normalized.
    //
    // TO move forward we should eventually introduce a new type NormalizedFileName that tracks this invariant.
    member t.FileToIndex normalize filePath =
        match fileToIndexTable.TryGetValue filePath with
        | true, idx -> idx
        | _ ->
            // Try again looking for a normalized entry.
            let normalizedFilePath =
                if normalize then
                    FileSystem.NormalizePathShim filePath
                else
                    filePath

            match fileToIndexTable.TryGetValue normalizedFilePath with
            | true, idx ->
                // Record the non-normalized entry if necessary
                if filePath <> normalizedFilePath then
                    fileToIndexTable[filePath] <- idx

                // Return the index
                idx

            | _ ->
                lock indexToFileTable (fun () ->
                    // See if it was added on another thread
                    match fileToIndexTable.TryGetValue normalizedFilePath with
                    | true, idx -> idx
                    | _ ->
                        // Okay it's really not there
                        let idx = indexToFileTable.Count

                        // Record the normalized entry
                        indexToFileTable.Add normalizedFilePath
                        fileToIndexTable[normalizedFilePath] <- idx

                        // Record the non-normalized entry if necessary
                        if filePath <> normalizedFilePath then
                            fileToIndexTable[filePath] <- idx

                        // Return the index
                        idx)

    member t.IndexToFile n =
        if n < 0 then
            failwithf "fileOfFileIndex: negative argument: n = %d\n" n

        if n >= indexToFileTable.Count then
            failwithf "fileOfFileIndex: invalid argument: n = %d\n" n

        indexToFileTable[n]

[<AutoOpen>]
module FileIndex =
    let maxFileIndex = pown32 fileIndexBitCount

    // ++GLOBAL MUTABLE STATE
    // WARNING: Global Mutable State, holding a mapping between integers and filenames
    let fileIndexTable = FileIndexTable()

    // If we exceed the maximum number of files we'll start to report incorrect file names
    let fileIndexOfFileAux normalize f =
        fileIndexTable.FileToIndex normalize f % maxFileIndex

    let fileIndexOfFile filePath = fileIndexOfFileAux false filePath

    let fileOfFileIndex idx = fileIndexTable.IndexToFile idx

    let unknownFileName = "unknown"
    let startupFileName = "startup"
    let commandLineArgsFileName = "commandLineArgs"

[<Struct; CustomEquality; NoComparison>]
[<System.Diagnostics.DebuggerDisplay("({StartLine},{StartColumn}-{EndLine},{EndColumn}) {ShortFileName} -> {DebugCode}")>]
type Range(code1: int64, code2: int64) =
    [<Obsolete("Use Range.range0 instead")>]
    static member Zero = range (0L, 0L)

    new(fIdx, bl, bc, el, ec) =
        let code1 =
            ((int64 fIdx) &&& fileIndexMask)
            ||| ((int64 bc <<< startColumnShift) &&& startColumnMask)
            ||| ((int64 ec <<< endColumnShift) &&& endColumnMask)

        let code2 =
            ((int64 bl <<< startLineShift) &&& startLineMask)
            ||| ((int64 (el - bl) <<< heightShift) &&& heightMask)

        range (code1, code2)

    new(fIdx, b: pos, e: pos) = range (fIdx, b.Line, b.Column, e.Line, e.Column)

    member _.StartLine = int32 (uint64 (code2 &&& startLineMask) >>> startLineShift)

    member _.StartColumn = int32 (uint64 (code1 &&& startColumnMask) >>> startColumnShift)

    member m.EndLine = int32 (uint64 (code2 &&& heightMask) >>> heightShift) + m.StartLine

    member _.EndColumn = int32 (uint64 (code1 &&& endColumnMask) >>> endColumnShift)

    member _.IsSynthetic =
        int32 (uint64 (code2 &&& isSyntheticMask) >>> isSyntheticShift) <> 0

    member _.NotedSourceConstruct =
        match int32 (uint64 (code2 &&& debugPointKindMask) >>> debugPointKindShift) with
        | 1 -> NotedSourceConstruct.While
        | 2 -> NotedSourceConstruct.For
        | 3 -> NotedSourceConstruct.Try
        | 4 -> NotedSourceConstruct.Finally
        | 5 -> NotedSourceConstruct.Binding
        | 6 -> NotedSourceConstruct.InOrTo
        | 7 -> NotedSourceConstruct.With
        | 8 -> NotedSourceConstruct.Combine
        | 9 -> NotedSourceConstruct.DelayOrQuoteOrRun
        | _ -> NotedSourceConstruct.None

    member m.Start = pos (m.StartLine, m.StartColumn)

    member m.End = pos (m.EndLine, m.EndColumn)

    member _.FileIndex = int32 (code1 &&& fileIndexMask)

    member m.StartRange = range (m.FileIndex, m.Start, m.Start)

    member m.EndRange = range (m.FileIndex, m.End, m.End)

    member m.FileName = fileOfFileIndex m.FileIndex

    member m.ShortFileName = Path.GetFileName(fileOfFileIndex m.FileIndex)

    member _.MakeSynthetic() =
        range (code1, code2 ||| isSyntheticMask)

    member m.IsAdjacentTo(otherRange: Range) =
        m.FileIndex = otherRange.FileIndex && m.End.Encoding = otherRange.Start.Encoding

    member _.NoteSourceConstruct(kind) =
        let code =
            match kind with
            | NotedSourceConstruct.None -> 0
            | NotedSourceConstruct.While -> 1
            | NotedSourceConstruct.For -> 2
            | NotedSourceConstruct.Try -> 3
            | NotedSourceConstruct.Finally -> 4
            | NotedSourceConstruct.Binding -> 5
            | NotedSourceConstruct.InOrTo -> 6
            | NotedSourceConstruct.With -> 7
            | NotedSourceConstruct.Combine -> 8
            | NotedSourceConstruct.DelayOrQuoteOrRun -> 9

        range (code1, (code2 &&& ~~~debugPointKindMask) ||| (int64 code <<< debugPointKindShift))

    member _.Code1 = code1

    member _.Code2 = code2

    member m.DebugCode =
        let name = m.FileName

        if
            name = unknownFileName
            || name = startupFileName
            || name = commandLineArgsFileName
        then
            name
        else

            try
                let endCol = m.EndColumn - 1
                let startCol = m.StartColumn - 1

                if FileSystem.IsInvalidPathShim m.FileName then
                    "path invalid: " + m.FileName
                elif not (FileSystem.FileExistsShim m.FileName) then
                    "nonexistent file: " + m.FileName
                else
                    FileSystem.OpenFileForReadShim(m.FileName).ReadLines()
                    |> Seq.skip (m.StartLine - 1)
                    |> Seq.take (m.EndLine - m.StartLine + 1)
                    |> String.concat "\n"
                    |> fun s -> s.Substring(startCol + 1, s.LastIndexOf("\n", StringComparison.Ordinal) + 1 - startCol + endCol)
            with e ->
                e.ToString()

    member _.Equals(m2: range) =
        let code2 = code2 &&& ~~~(debugPointKindMask ||| isSyntheticMask)
        let rcode2 = m2.Code2 &&& ~~~(debugPointKindMask ||| isSyntheticMask)
        code1 = m2.Code1 && code2 = rcode2

    override m.Equals(obj) =
        match obj with
        | :? range as m2 -> m.Equals(m2)
        | _ -> false

    override _.GetHashCode() =
        let code2 = code2 &&& ~~~(debugPointKindMask ||| isSyntheticMask)
        hash code1 + hash code2

    override r.ToString() =
        sprintf "(%d,%d--%d,%d)" r.StartLine r.StartColumn r.EndLine r.EndColumn

and range = Range

#if CHECK_LINE0_TYPES // turn on to check that we correctly transform zero-based line counts to one-based line counts
// Visual Studio uses line counts starting at 0, F# uses them starting at 1
[<Measure>]
type ZeroBasedLineAnnotation

type Line0 = int<ZeroBasedLineAnnotation>
#else
type Line0 = int
#endif

type Position01 = Line0 * int

type Range01 = Position01 * Position01

module Line =

    let fromZ (line: Line0) = int line + 1

    let toZ (line: int) : Line0 =
        LanguagePrimitives.Int32WithMeasure(line - 1)

[<AutoOpen>]
module Position =

    let mkPos line column = Position(line, column)

    let outputPos (os: TextWriter) (m: pos) = fprintf os "(%d,%d)" m.Line m.Column

    let posGt (p1: pos) (p2: pos) =
        let p1Line = p1.Line
        let p2Line = p2.Line
        p1Line > p2Line || p1Line = p2Line && p1.Column > p2.Column

    let posEq (p1: pos) (p2: pos) = p1.Encoding = p2.Encoding

    let posGeq p1 p2 = posEq p1 p2 || posGt p1 p2

    let posLt p1 p2 = posGt p2 p1

    let fromZ (line: Line0) column = mkPos (Line.fromZ line) column

    let toZ (p: pos) = (Line.toZ p.Line, p.Column)

    (* For Diagnostics *)
    let stringOfPos (pos: pos) = sprintf "(%d,%d)" pos.Line pos.Column

    let pos0 = mkPos 1 0

module Range =
    let mkRange filePath startPos endPos =
        range (fileIndexOfFileAux true filePath, startPos, endPos)

    let equals (r1: range) (r2: range) = r1.Equals(r2)

    let mkFileIndexRange fileIndex startPos endPos = range (fileIndex, startPos, endPos)

    let posOrder =
        let pairOrder = Pair.order (Int32.order, Int32.order)
        let lineAndColumn = fun (p: pos) -> struct (p.Line, p.Column)

        { new IComparer<pos> with
            member _.Compare(x, xx) =
                pairOrder.Compare(lineAndColumn x, lineAndColumn xx)
        }

    let rangeOrder =
        let tripleOrder = Pair.order (String.order, Pair.order (posOrder, posOrder))
        let fileLineColumn = fun (r: range) -> struct (r.FileName, struct (r.Start, r.End))

        { new IComparer<range> with
            member _.Compare(x, xx) =
                tripleOrder.Compare(fileLineColumn x, fileLineColumn xx)
        }

    let outputRange (os: TextWriter) (m: range) =
        fprintf os "%s%a-%a" m.FileName outputPos m.Start outputPos m.End

    /// This is deliberately written in an allocation-free way, i.e. m1.Start, m1.End etc. are not called
    let unionRanges (m1: range) (m2: range) =
        if m1.FileIndex <> m2.FileIndex then
            m2
        else if

            // If all identical then return m1. This preserves NotedSourceConstruct when no merging takes place
            m1.Code1 = m2.Code1 && m1.Code2 = m2.Code2
        then
            m1
        else

            let start =
                if
                    (m1.StartLine > m2.StartLine
                     || (m1.StartLine = m2.StartLine && m1.StartColumn > m2.StartColumn))
                then
                    m2
                else
                    m1

            let finish =
                if
                    (m1.EndLine > m2.EndLine
                     || (m1.EndLine = m2.EndLine && m1.EndColumn > m2.EndColumn))
                then
                    m1
                else
                    m2

            let m =
                range (m1.FileIndex, start.StartLine, start.StartColumn, finish.EndLine, finish.EndColumn)

            if m1.IsSynthetic || m2.IsSynthetic then
                m.MakeSynthetic()
            else
                m

    let withStartEnd (startPos: Position) (endPos: Position) (r: range) = range (r.FileIndex, startPos, endPos)

    let withStart (startPos: Position) (r: range) = range (r.FileIndex, startPos, r.End)

    let withEnd (endPos: Position) (r: range) = range (r.FileIndex, r.Start, endPos)

    let shiftStart (lineDelta: int) (columnDelta: int) (r: range) =
        let shiftedStart = mkPos (r.Start.Line + lineDelta) (r.StartColumn + columnDelta)
        range (r.FileIndex, shiftedStart, r.End)

    let shiftEnd (lineDelta: int) (columnDelta: int) (r: range) =
        let shiftedEnd = mkPos (r.End.Line + lineDelta) (r.EndColumn + columnDelta)
        range (r.FileIndex, r.Start, shiftedEnd)

    let rangeContainsRange (m1: range) (m2: range) =
        m1.FileIndex = m2.FileIndex && posGeq m2.Start m1.Start && posGeq m1.End m2.End

    let rangeContainsPos (m1: range) p = posGeq p m1.Start && posGeq m1.End p

    let rangeBeforePos (m1: range) p = posGeq p m1.End

    let rangeN fileName line =
        mkRange fileName (mkPos line 0) (mkPos line 0)

    let range0 = rangeN unknownFileName 1

    let rangeStartup = rangeN startupFileName 1

    let rangeCmdArgs = rangeN commandLineArgsFileName 0

    let trimRangeToLine (r: range) =
        let startL, startC = r.StartLine, r.StartColumn
        let endL, _endC = r.EndLine, r.EndColumn

        if endL <= startL then
            r
        else
            // Trim to the start of the next line (we do not know the end of the current line)
            let endL, endC = startL + 1, 0
            range (r.FileIndex, startL, startC, endL, endC)

    let stringOfRange (r: range) =
        sprintf "%s%s-%s" r.FileName (stringOfPos r.Start) (stringOfPos r.End)

    let toZ (m: range) = toZ m.Start, toZ m.End

    let toFileZ (m: range) = m.FileName, toZ m

    let comparer =
        { new IEqualityComparer<range> with
            member _.Equals(x1, x2) = equals x1 x2
            member _.GetHashCode o = o.GetHashCode()
        }

    let mkFirstLineOfFile (file: string) =
        try
            if not (FileSystem.FileExistsShim file) then
                mkRange file (mkPos 1 0) (mkPos 1 80)
            else
                let lines = FileSystem.OpenFileForReadShim(file).ReadLines() |> Seq.indexed

                let nonWhiteLine =
                    lines |> Seq.tryFind (fun (_, s) -> not (String.IsNullOrWhiteSpace s))

                match nonWhiteLine with
                | Some(i, s) -> mkRange file (mkPos (i + 1) 0) (mkPos (i + 1) s.Length)
                | None ->

                    let nonEmptyLine = lines |> Seq.tryFind (fun (_, s) -> not (String.IsNullOrEmpty s))

                    match nonEmptyLine with
                    | Some(i, s) -> mkRange file (mkPos (i + 1) 0) (mkPos (i + 1) s.Length)
                    | None -> mkRange file (mkPos 1 0) (mkPos 1 80)
        with _ ->
            mkRange file (mkPos 1 0) (mkPos 1 80)
