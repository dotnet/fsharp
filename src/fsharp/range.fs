// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Anything to do with special names of identifiers and other lexical rules 
module Microsoft.FSharp.Compiler.Range

open System.IO
open System.Collections.Generic
open Microsoft.FSharp.Core.Printf
open Internal.Utilities
open Microsoft.FSharp.Compiler.AbstractIL 
open Microsoft.FSharp.Compiler.AbstractIL.Internal 
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library
open Microsoft.FSharp.Compiler  
open Microsoft.FSharp.Compiler.Lib
open Microsoft.FSharp.Compiler.Lib.Bits

type FileIndex = int32 

[<Literal>]
let columnBitCount = 9
[<Literal>]
let lineBitCount = 16

let posBitCount = lineBitCount + columnBitCount
let _ = assert (posBitCount <= 32)
let posColumnMask  = mask32 0 columnBitCount
let lineColumnMask = mask32 columnBitCount lineBitCount
let inline (lsr)  (x:int) (y:int)  = int32 (uint32 x >>> y)

[<Struct; CustomEquality; NoComparison>]
[<System.Diagnostics.DebuggerDisplay("{Line},{Column}")>]
type pos =
    val private line: int16
    val private column: int16
    new(line: int, col: int) = { line = int16 line; column = int16 col }
    member p.Line = int p.line
    member p.Column = int p.column
    override p.Equals(obj) = match obj with :? pos as p2 -> p.Line = p2.Line && p.Column = p2.Column | _ -> false
    override p.GetHashCode() = hash p.Line + hash p.Column
    override p.ToString() = sprintf "(%d,%d)" p.Line p.Column

[<Literal>]
let fileIndexBitCount = 15
[<Literal>]
let isSyntheticBitCount = 1
#if DEBUG
let _ = assert (fileIndexBitCount + isSyntheticBitCount = 16)
#endif
 
[<Literal>]
let fileIndexShift   = 0 
[<Literal>]
let isSyntheticShift = 15

[<Literal>]
let fileIndexMask =   0b011111111111111s
[<Literal>]
let isSyntheticMask = 0b100000000000000s

// This is just a standard unique-index table
type FileIndexTable() = 
    let indexToFileTable = new ResizeArray<_>(11)
    let fileToIndexTable = new Dictionary<string, int>(11)
    member t.FileToIndex f = 
        let mutable res = 0 
        let ok = fileToIndexTable.TryGetValue(f, &res) 
        if ok then res 
        else
            lock fileToIndexTable (fun () -> 
                let mutable res = 0 in
                let ok = fileToIndexTable.TryGetValue(f, &res) in
                if ok then res 
                else
                    let n = indexToFileTable.Count in
                    indexToFileTable.Add(f)
                    fileToIndexTable.[f] <- n
                    n)

    member t.IndexToFile n = 
        (if n < 0 then failwithf "fileOfFileIndex: negative argument: n = %d\n" n)
        (if n >= indexToFileTable.Count then failwithf "fileOfFileIndex: invalid argument: n = %d\n" n)
        indexToFileTable.[n]

let maxFileIndex = pown32 fileIndexBitCount

// ++GLOBAL MUTABLE STATE
// WARNING: Global Mutable State, holding a mapping between integers and filenames
let fileIndexTable = new FileIndexTable()

// If we exceed the maximum number of files we'll start to report incorrect file names
let fileIndexOfFile f = fileIndexTable.FileToIndex(f) % maxFileIndex 
let fileOfFileIndex n = fileIndexTable.IndexToFile(n)

let mkPos l c = pos (l, c)

[<Struct; CustomEquality; NoComparison>]
#if DEBUG
[<System.Diagnostics.DebuggerDisplay("({StartLine},{StartColumn}-{EndLine},{EndColumn}) {FileName} IsSynthetic={IsSynthetic} -> {DebugCode}")>]
#else
[<System.Diagnostics.DebuggerDisplay("({StartLine},{StartColumn}-{EndLine},{EndColumn}) {FileName} IsSynthetic={IsSynthetic}")>]
#endif
type range =
    val Start: pos
    val End: pos
    val Code: int16
    new(fidx: int, s, e) = { Start = s; End = e; Code = int16 fidx }
    static member Zero = range(0, pos(0, 0), pos(0, 0))
    
    member r.StartLine   = int r.Start.Line
    member r.StartColumn = int r.Start.Column
    member r.EndLine     = int r.End.Line
    member r.EndColumn   = int r.End.Column
    member r.IsSynthetic = int32((r.Code &&& isSyntheticMask) >>> isSyntheticShift) <> 0 
    member r.FileIndex = int32(r.Code &&& fileIndexMask)
    member m.StartRange = range (m.FileIndex, m.Start, m.Start)
    member m.EndRange = range (m.FileIndex, m.End, m.End)
    member r.FileName = fileOfFileIndex r.FileIndex
#if DEBUG
    member r.DebugCode =
        try
            let endCol = r.EndColumn - 1
            let startCol = r.StartColumn - 1
            File.ReadAllLines(r.FileName)
            |> Seq.skip (r.StartLine - 1)
            |> Seq.take (r.EndLine - r.StartLine + 1)
            |> String.concat "\n"
            |> fun s -> s.Substring(startCol + 1, s.LastIndexOf("\n") + 1 - startCol + endCol)
        with e ->
            e.ToString()        
#endif
    member r.MakeSynthetic() = range(int (r.Code ||| isSyntheticMask), r.Start, r.End)
    override r.ToString() = sprintf "%s (%d,%d--%d,%d) IsSynthetic=%b" r.FileName r.StartLine r.StartColumn r.EndLine r.EndColumn r.IsSynthetic
    member r.ToShortString() = sprintf "(%d,%d--%d,%d)" r.StartLine r.StartColumn r.EndLine r.EndColumn
    override r.Equals(obj) = match obj with :? range as r2 -> r.Start.Equals r2.Code && r.End.Equals r2.End && r.Code = r2.Code | _ -> false
    override r.GetHashCode() = hash r.Start + hash r.End + hash r.Code

let mkRange f b e =
    // remove relative parts from full path
    let normalizedFilePath = if Path.IsPathRooted f then try Path.GetFullPath f with _ -> f else f
    range (fileIndexOfFile normalizedFilePath, b, e)

let mkFileIndexRange fi b e = range (fi, b, e)

(* end representation, start derived ops *)
                 
let posOrder   = Order.orderOn (fun (p:pos) -> p.Line, p.Column) (Pair.order (Int32.order, Int32.order))
(* rangeOrder: not a total order, but enough to sort on ranges *)      
let rangeOrder = Order.orderOn (fun (r:range) -> r.FileName, r.Start) (Pair.order (String.order, posOrder))

let outputPos   (os:TextWriter) (m:pos)   = fprintf os "(%d,%d)" m.Line m.Column
let outputRange (os:TextWriter) (m:range) = fprintf os "%s%a-%a" m.FileName outputPos m.Start outputPos m.End
let boutputPos   os (m:pos)   = bprintf os "(%d,%d)" m.Line m.Column
let boutputRange os (m:range) = bprintf os "%s%a-%a" m.FileName boutputPos m.Start boutputPos m.End
    
let posGt (p1:pos) (p2:pos) = (p1.Line > p2.Line || (p1.Line = p2.Line && p1.Column > p2.Column))
let posEq (p1:pos) (p2:pos) = (p1.Line = p2.Line &&  p1.Column = p2.Column)
let posGeq p1 p2 = posEq p1 p2 || posGt p1 p2
let posLt p1 p2 = posGt p2 p1

// This is deliberately written in an allocation-free way, i.e. m1.Start, m1.End etc. are not called
let unionRanges (m1:range) (m2:range) = 
    if m1.FileIndex <> m2.FileIndex then m2 else
    let b = 
      if (m1.StartLine > m2.StartLine || (m1.StartLine = m2.StartLine && m1.StartColumn > m2.StartColumn)) then m2
      else m1
    let e = 
      if (m1.EndLine > m2.EndLine || (m1.EndLine = m2.EndLine && m1.EndColumn > m2.EndColumn)) then m1
      else m2
    range (m1.FileIndex, b.Start, e.End)

let rangeContainsRange (m1:range) (m2:range) =
    m1.FileIndex = m2.FileIndex &&
    posGeq m2.Start m1.Start &&
    posGeq m1.End m2.End

let rangeContainsPos (m1:range) p =
    posGeq p m1.Start &&
    posGeq m1.End p

let rangeBeforePos (m1:range) p =
    posGeq p m1.End

let rangeN filename line = mkRange filename (mkPos line 0) (mkPos line 0)
let pos0 = mkPos 1 0
let range0 =  rangeN "unknown" 1
let rangeStartup = rangeN "startup" 1
let rangeCmdArgs = rangeN "commandLineArgs" 0

let trimRangeToLine (r:range) =
    let startL = r.StartLine
    let endL , _endC   = r.EndLine, r.EndColumn
    if endL <= startL then
      r
    else
      let endL, endC = startL+1, 0   (* Trim to the start of the next line (we do not know the end of the current line) *)
      range (r.FileIndex, r.Start, pos(endL, endC))

(* For Diagnostics *)
let stringOfPos   (pos:pos) = sprintf "(%d,%d)" pos.Line pos.Column
let stringOfRange (r:range) = sprintf "%s%s-%s" r.FileName (stringOfPos r.Start) (stringOfPos r.End)

#if CHECK_LINE0_TYPES // turn on to check that we correctly transform zero-based line counts to one-based line counts
// Visual Studio uses line counts starting at 0, F# uses them starting at 1 
[<Measure>] type ZeroBasedLineAnnotation

type Line0 = int<ZeroBasedLineAnnotation>
#else
type Line0 = int
#endif
type Pos01 = Line0 * int
type Range01 = Pos01 * Pos01

module Line =
    // Visual Studio uses line counts starting at 0, F# uses them starting at 1 
    let fromZ (line:Line0) = int line+1
    let toZ (line:int) : Line0 = LanguagePrimitives.Int32WithMeasure(line - 1)

module Pos =
    let fromZ (line:Line0) idx = mkPos (Line.fromZ line) idx 
    let toZ (p:pos) = (Line.toZ p.Line, p.Column)


module Range =
    let toZ (m:range) = Pos.toZ m.Start, Pos.toZ m.End
    let toFileZ (m:range) = m.FileName, toZ m


