// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

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

module Pos =
    // Visual Studio uses line counts starting at 0, F# uses them starting at 1 
    val fromVS : line:int -> column:int -> pos
    val toVS : pos -> (int * int)


module Range =
    val toVS : range -> (int * int) * (int * int)
