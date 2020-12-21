// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler

open System.Collections.Generic
  
/// An index into a global tables of filenames
type FileIndex = int32 

/// Represents a position in a file
[<Struct; CustomEquality; NoComparison>]
type Pos =

    /// The line number for the position
    member Line: int

    /// The column number for the position
    member Column: int

    /// The encoding of the position as a 64-bit integer
    member Encoding: int64

    /// Decode a position fro a 64-bit integer
    static member Decode: int64 -> Pos

    /// The maximum number of bits needed to store an encoded position 
    static member EncodingSize: int
  
/// Represents a range within a known file
[<Struct; CustomEquality; NoComparison>]
type Range =

    /// The start line of the range
    member StartLine: int

    /// The start column of the range
    member StartColumn: int

    /// The line number for the end position of the range
    member EndLine: int

    /// The column number for the end position of the range
    member EndColumn: int

    /// The start position of the range
    member Start: Pos

    /// The end position of the range
    member End: Pos

    /// The empty range that is located at the start position of the range
    member StartRange: Range

    /// The empty range that is located at the end position of the range
    member EndRange: Range

    /// The file index for the range
    member FileIndex: int

    /// The file name for the file of the range
    member FileName: string

    /// Synthetic marks ranges which are produced by intermediate compilation phases. This
    /// bit signifies that the range covers something that should not be visible to language
    /// service operations like dot-completion.
    member IsSynthetic: bool 

    /// Convert a range to be synthetic
    member MakeSynthetic: unit -> Range

    /// Convert a range to string
    member ToShortString: unit -> string

    /// The range where all values are zero
    static member Zero: Range
  
/// Represents a line number when using zero-based line counting (used by Visual Studio)
#if CHECK_LINE0_TYPES
// Visual Studio uses line counts starting at 0, F# uses them starting at 1 
[<Measure>] type ZeroBasedLineAnnotation

type Line0 = int<ZeroBasedLineAnnotation>
#else
type Line0 = int
#endif

/// Represents a position using zero-based line counting (used by Visual Studio)
type Pos01 = Line0 * int

/// Represents a range using zero-based line counting (used by Visual Studio)
type Range01 = Pos01 * Pos01

module Pos =
    /// Create a position for the given line and column
    val mkPos: line:int -> column:int -> Pos

    
    /// Compare positions for less-than
    val posLt: Pos -> Pos -> bool

    /// Compare positions for greater-than
    val posGt: Pos -> Pos -> bool

    /// Compare positions for equality
    val posEq: Pos -> Pos -> bool

    /// Compare positions for greater-than-or-equal-to
    val posGeq: Pos -> Pos -> bool

    /// Convert a position from zero-based line counting (used by Visual Studio) to one-based line counting (used internally in the F# compiler and in F# error messages) 
    val fromZ: line:Line0 -> column:int -> Pos

    /// Convert a position from one-based line counting (used internally in the F# compiler and in F# error messages) to zero-based line counting (used by Visual Studio)
    val toZ: Pos -> Pos01

module FileIndex =

    /// Convert a file path to an index
    val fileIndexOfFile: filePath: string -> FileIndex

    /// Convert an index into a file path
    val fileOfFileIndex: FileIndex -> string

    /// The zero position
    val pos0: Pos

module Range =

    /// Ordering on positions
    val posOrder: IComparer<Pos>

    val unknownFileName: string
    val startupFileName: string
    val commandLineArgsFileName: string

    /// This view of range marks uses file indexes explicitly 
    val mkFileIndexRange: FileIndex -> Pos -> Pos -> Range

    /// This view hides the use of file indexes and just uses filenames 
    val mkRange: string -> Pos -> Pos -> Range

    /// Make a range for the first non-whitespace line of the file if any. Otherwise use line 1 chars 0-80.
    /// This involves reading the file.
    val mkFirstLineOfFile: string -> Range

    val equals: Range -> Range -> bool

    /// Reduce a range so it only covers a line
    val trimRangeToLine: Range -> Range

    /// not a total order, but enough to sort on ranges 
    val rangeOrder: IComparer<Range>

    /// Output a position
    val outputPos: System.IO.TextWriter -> Pos -> unit

    /// Output a range
    val outputRange: System.IO.TextWriter -> Range -> unit

    /// Union two ranges, taking their first occurring start position and last occurring end position
    val unionRanges: Range -> Range -> Range

    /// Test to see if one range contains another range
    val rangeContainsRange: Range -> Range -> bool

    /// Test to see if a range contains a position
    val rangeContainsPos: Range -> Pos -> bool

    /// Test to see if a range occurs fully before a position
    val rangeBeforePos: Range -> Pos -> bool

    /// Make a dummy range for a file
    val rangeN: string -> int -> Range

    /// The zero range
    val range0: Range

    /// A range associated with a dummy file called "startup"
    val rangeStartup: Range

    /// A range associated with a dummy file for the command line arguments
    val rangeCmdArgs: Range
 
    /// Convert a position to a string
    val stringOfPos  : Pos   -> string

    /// Convert a range to a string
    val stringOfRange: Range -> string

    /// Convert a range from one-based line counting (used internally in the F# compiler and in F# error messages) to zero-based line counting (used by Visual Studio)
    val toZ: Range -> Range01

    /// Convert a range from one-based line counting (used internally in the F# compiler and in F# error messages) to zero-based line counting (used by Visual Studio)
    val toFileZ: Range -> string * Range01

    /// Equality comparer for range.
    val comparer: IEqualityComparer<Range>

module Line =

    /// Convert a line number from zero-based line counting (used by Visual Studio) to one-based line counting (used internally in the F# compiler and in F# error messages) 
    val fromZ: Line0 -> int

    /// Convert a line number from one-based line counting (used internally in the F# compiler and in F# error messages) to zero-based line counting (used by Visual Studio)
    val toZ: int -> Line0 


