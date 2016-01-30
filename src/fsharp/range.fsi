// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Microsoft.FSharp.Compiler.Range

open System.Text
open System.Collections.Generic
open Internal.Utilities
open Microsoft.FSharp.Compiler.AbstractIL 
open Microsoft.FSharp.Compiler.AbstractIL.Internal 
open Microsoft.FSharp.Compiler  

  
(* we keep a global tables of filenames that we can reference by integers *)
type FileIndex = int32 
val fileIndexOfFile : string -> FileIndex
val fileOfFileIndex : FileIndex -> string

[<Struct; CustomEquality; NoComparison>]
type pos =
    member Line : int
    member Column : int

    member Encoding : int32
    static member Decode : int32 -> pos
    /// The maximum number of bits needed to store an encoded position 
    static member EncodingSize : int32 
  
/// Create a position for the given line and column
val mkPos : line:int -> column:int -> pos

val posOrder : IComparer<pos>

[<Struct; CustomEquality; NoComparison>]
type range =
    member StartLine : int
    member StartColumn : int
    member EndLine : int
    member EndColumn : int
    member Start : pos
    member End : pos
    member StartRange: range
    member EndRange: range
    member FileIndex : int
    member FileName : string
    /// Synthetic marks ranges which are produced by intermediate compilation phases. This
    /// bit signifies that the range covers something that should not be visible to language
    /// service operations like dot-completion.
    member IsSynthetic : bool 
    member MakeSynthetic : unit -> range
    member ToShortString : unit -> string
    static member Zero : range
  
/// This view of range marks uses file indexes explicitly 
val mkFileIndexRange : FileIndex -> pos -> pos -> range

/// This view hides the use of file indexes and just uses filenames 
val mkRange : string -> pos -> pos -> range

val trimRangeToLine : range -> range

/// not a total order, but enough to sort on ranges 
val rangeOrder : IComparer<range>

val outputPos : System.IO.TextWriter -> pos -> unit
val outputRange : System.IO.TextWriter -> range -> unit
val boutputPos : StringBuilder -> pos -> unit
val boutputRange : StringBuilder -> range -> unit
    
val posLt : pos -> pos -> bool
val posGt : pos -> pos -> bool
val posEq : pos -> pos -> bool
val posGeq : pos -> pos -> bool

val unionRanges : range -> range -> range
val rangeContainsRange : range -> range -> bool
val rangeContainsPos : range -> pos -> bool
val rangeBeforePos : range -> pos -> bool

val rangeN : string -> int -> range
val pos0 : pos
val range0 : range
val rangeStartup : range
val rangeCmdArgs : range
 
(* For diagnostics *)  
val stringOfPos   : pos   -> string
val stringOfRange : range -> string

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

module Line =
    /// Convert a line number from zero-based line counting (used by Visual Studio) to one-based line counting (used internally in the F# compiler and in F# error messages) 
    val fromZ : Line0 -> int
    /// Convert a line number from one-based line counting (used internally in the F# compiler and in F# error messages) to zero-based line counting (used by Visual Studio)
    val toZ : int -> Line0 

module Pos =
    /// Convert a position from zero-based line counting (used by Visual Studio) to one-based line counting (used internally in the F# compiler and in F# error messages) 
    val fromZ : line:Line0 -> column:int -> pos
    /// Convert a position from one-based line counting (used internally in the F# compiler and in F# error messages) to zero-based line counting (used by Visual Studio)
    val toZ : pos -> Pos01

module Range =
    /// Convert a range from one-based line counting (used internally in the F# compiler and in F# error messages) to zero-based line counting (used by Visual Studio)
    val toZ : range -> Range01
    val toFileZ : range -> string * Range01
