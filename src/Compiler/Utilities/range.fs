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
                    lock fileToIndexTable (fun () -> fileToIndexTable[filePath] <- idx)

                // Return the index
                idx

            | _ ->
                lock fileToIndexTable (fun () ->
                    // Get the new index
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
type _RangeBackground(code1: int64, code2: int64) =
    static member Zero = _rangeBackground (0L, 0L)

    new(fIdx, bl, bc, el, ec) =
        let code1 =
            ((int64 fIdx) &&& fileIndexMask)
            ||| ((int64 bc <<< startColumnShift) &&& startColumnMask)
            ||| ((int64 ec <<< endColumnShift) &&& endColumnMask)

        let code2 =
            ((int64 bl <<< startLineShift) &&& startLineMask)
            ||| ((int64 (el - bl) <<< heightShift) &&& heightMask)

        _rangeBackground (code1, code2)

    new(fIdx, b: pos, e: pos) = _rangeBackground (fIdx, b.Line, b.Column, e.Line, e.Column)

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

    member m.StartRange = _rangeBackground (m.FileIndex, m.Start, m.Start)

    member m.EndRange = _rangeBackground (m.FileIndex, m.End, m.End)

    member m.FileName = fileOfFileIndex m.FileIndex

    member m.ShortFileName = Path.GetFileName(fileOfFileIndex m.FileIndex)

    member _.MakeSynthetic() =
        _rangeBackground (code1, code2 ||| isSyntheticMask)

    member m.IsAdjacentTo(otherRange: _RangeBackground) =
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

        _rangeBackground (code1, (code2 &&& ~~~debugPointKindMask) ||| (int64 code <<< debugPointKindShift))

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
                    "non existing file: " + m.FileName
                else
                    FileSystem.OpenFileForReadShim(m.FileName).ReadLines()
                    |> Seq.skip (m.StartLine - 1)
                    |> Seq.take (m.EndLine - m.StartLine + 1)
                    |> String.concat "\n"
                    |> fun s -> s.Substring(startCol + 1, s.LastIndexOf("\n", StringComparison.Ordinal) + 1 - startCol + endCol)
            with e ->
                e.ToString()

    member _.Equals(m2: _rangeBackground) =
        let code2 = code2 &&& ~~~(debugPointKindMask ||| isSyntheticMask)
        let rcode2 = m2.Code2 &&& ~~~(debugPointKindMask ||| isSyntheticMask)
        code1 = m2.Code1 && code2 = rcode2

    override m.Equals(obj) =
        match obj with
        | :? _rangeBackground as m2 -> m.Equals(m2)
        | _ -> false

    override _.GetHashCode() =
        let code2 = code2 &&& ~~~(debugPointKindMask ||| isSyntheticMask)
        hash code1 + hash code2

    override r.ToString() =
        sprintf "(%d,%d--%d,%d)" r.StartLine r.StartColumn r.EndLine r.EndColumn

    member m.IsZero = m.Equals _rangeBackground.Zero

and _rangeBackground = _RangeBackground

[<Struct; CustomEquality; NoComparison>]
[<System.Diagnostics.DebuggerDisplay("({OriginalStartLine},{OriginalStartColumn}-{OriginalEndLine},{OriginalEndColumn}) {OriginalShortFileName} -> ({StartLine},{StartColumn}-{EndLine},{EndColumn}) {ShortFileName} -> {DebugCode}")>]
type Range(range1: _rangeBackground, range2: _rangeBackground) =
    static member Zero = range (_rangeBackground.Zero, _rangeBackground.Zero)

    new(fIdx, bl, bc, el, ec) = range (_rangeBackground (fIdx, bl, bc, el, ec), _RangeBackground.Zero)

    new(fIdx, bl, bc, el, ec, fIdx2, bl2, bc2, el2, ec2) =
        range (_rangeBackground (fIdx, bl, bc, el, ec), _rangeBackground (fIdx2, bl2, bc2, el2, ec2))

    new(fIdx, b: pos, e: pos) = range (_rangeBackground (fIdx, b.Line, b.Column, e.Line, e.Column), _rangeBackground.Zero)

    new(fIdx, b: pos, e: pos, fIdx2, b2: pos, e2: pos) =
        range (
            _rangeBackground (fIdx, b.Line, b.Column, e.Line, e.Column),
            _rangeBackground (fIdx2, b2.Line, b2.Column, e2.Line, e2.Column)
        )

    member _.StartLine = range1.StartLine

    member _.StartColumn = range1.StartColumn

    member _.EndLine = range1.EndLine

    member _.EndColumn = range1.EndColumn

    member _.IsSynthetic = range1.IsSynthetic

    member _.NotedSourceConstruct = range1.NotedSourceConstruct

    member _.Start = range1.Start

    member _.End = range1.End

    member _.FileIndex = range1.FileIndex

    member _.StartRange = Range(range1.StartRange, range2.StartRange)

    member _.EndRange = Range(range1.EndRange, range2.EndRange)

    member _.FileName = range1.FileName

    member _.ShortFileName = range1.ShortFileName

    member _.MakeSynthetic() =
        range (range1.MakeSynthetic(), range2.MakeSynthetic())

    member _.IsAdjacentTo(otherRange: Range) = range1.IsAdjacentTo otherRange.Range1

    member _.NoteSourceConstruct(kind) =
        range (range1.NoteSourceConstruct kind, range2.NoteSourceConstruct kind)

    member _.Code1 = range1.Code1

    member _.Code2 = range1.Code2

    member _.Range1 = range1

    member _.Range2 = range2

    member _.DebugCode = range1.DebugCode

    member _.Equals(m2: range) =
        range1.Equals m2.Range1 && range2.Equals m2.Range2

    override m.Equals(obj) =
        match obj with
        | :? range as m2 -> m.Equals(m2)
        | _ -> false

    override _.GetHashCode() =
        range1.GetHashCode() + range2.GetHashCode()

    override _.ToString() =
        range1.ToString()
        + if range2.IsZero then
              String.Empty
          else
              $"(from: {range2.ToString()})"

    member _.HasOriginalRange = not range2.IsZero

    member _.OriginalStartLine = range2.StartLine

    member _.OriginalStartColumn = range2.StartColumn

    member _.OriginalEndLine = range2.EndLine

    member _.OriginalEndColumn = range2.EndColumn

    member _.OriginalIsSynthetic = range2.IsSynthetic

    member _.OriginalNotedSourceConstruct = range2.NotedSourceConstruct

    member _.OriginalStart = range2.Start

    member _.OriginalEnd = range2.End

    member _.OriginalFileIndex = range2.FileIndex

    member _.OriginalStartRange = Range(range2.StartRange, range2.StartRange)

    member _.OriginalEndRange = Range(range2.EndRange, range2.EndRange)

    member _.OriginalFileName = range2.FileName

    member _.OriginalShortFileName = range2.ShortFileName

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

    let mkFileIndexRangeWithOriginRange fileIndex startPos endPos fileIndex2 startPos2 endPos2 =
        range (fileIndex, startPos, endPos, fileIndex2, startPos2, endPos2)

    let posOrder =
        Order.orderOn (fun (p: pos) -> p.Line, p.Column) (Pair.order (Int32.order, Int32.order))

    let rangeOrder =
        Order.orderOn (fun (r: range) -> r.FileName, (r.Start, r.End)) (Pair.order (String.order, Pair.order (posOrder, posOrder)))

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
                if m1.OriginalFileIndex = m2.OriginalFileIndex then
                    range (
                        m1.FileIndex,
                        start.StartLine,
                        start.StartColumn,
                        finish.EndLine,
                        finish.EndColumn,
                        m1.OriginalFileIndex,
                        start.OriginalStartLine,
                        start.OriginalStartColumn,
                        finish.OriginalEndLine,
                        finish.OriginalEndColumn
                    )
                else
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

module internal FileContent =
    let private fileContentDict = ConcurrentDictionary<string, string array>()
    let private newlineMarkDict = ConcurrentDictionary<string, string>()

    let readFileContents (fileNames: string list) =
        for fileName in fileNames do
            if FileSystem.FileExistsShim fileName then
                use fileStream = FileSystem.OpenFileForReadShim(fileName)
                let text = fileStream.ReadAllText()

                let newlineMark =
                    if text.IndexOf('\n') > -1 && text.IndexOf('\r') = -1 then
                        "\n"
                    else
                        "\r\n"

                newlineMarkDict[fileName] <- newlineMark
                fileContentDict[fileName] <- text.Split([| newlineMark |], StringSplitOptions.None)

    type IFileContentGetLine =
        abstract GetLine: fileName: string -> line: int -> string
        abstract GetLineNewLineMark: fileName: string -> string

    let mutable getLineDynamic =
        { new IFileContentGetLine with
            member this.GetLine (fileName: string) (line: int) : string =
                match fileContentDict.TryGetValue fileName with
                | true, lines when lines.Length > line -> lines[line - 1]
                | _ -> String.Empty

            member this.GetLineNewLineMark(fileName: string) : string =
                match newlineMarkDict.TryGetValue fileName with
                | true, res -> res
                | _ -> String.Empty
        }

    let getCodeText (m: range) =
        let endCol = m.EndColumn - 1
        let startCol = m.StartColumn - 1

        let s =
            let filename, startLine, endLine =
                if m.HasOriginalRange then
                    m.OriginalFileName, m.OriginalStartLine, m.OriginalEndLine
                else
                    m.FileName, m.StartLine, m.EndLine

            [| for i in startLine..endLine -> getLineDynamic.GetLine filename i |]
            |> String.concat (getLineDynamic.GetLineNewLineMark filename)

        if String.IsNullOrEmpty s then
            s
        else
            s.Substring(startCol + 1, s.LastIndexOf("\n", StringComparison.Ordinal) + 1 - startCol + endCol)
