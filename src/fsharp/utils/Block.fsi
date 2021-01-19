// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Internal.Utilities

type internal 'T block = System.Collections.Immutable.ImmutableArray<'T>

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module internal Block =

    /// Creates a block that contains the elements of one block followed by the elements of another block.
    val append : 'T block -> 'T block -> 'T block 

    /// O(1). Returns block of no elements.
    [<GeneralizableValue>]
    val empty<'T> : 'T block 

(*
    ///Applies the supplied function to each element of a block, concatenates the results, and returns the combined block.
    val collect : ('T -> 'T block) -> 'T block -> 'T block

    /// Creates a block that contains the elements of each of the supplied sequence of blocks.
    val concat : block<'T []> -> 'T block

    /// O(n) worst case. Tests whether any element of a block satisfies the supplied predicate.
    val exists : ('T -> bool) -> 'T block -> bool

    /// Returns a block that contains only the elements of the supplied block for which the supplied condition returns true.
    val filter : ('T -> bool) -> 'T block -> 'T block

    /// Applies a function to each element of a block from left to right (first to last), threading an accumulator argument through the computation. 
    val fold : ('State -> 'T -> 'State) -> 'State -> 'T block -> 'State

    /// Applies a function to pairs of elements from two supplied blocks, left-to-right, threading an accumulator argument through the computation. The two input blocks must have the same lengths; otherwise, ArgumentException is raised.
    val fold2 : ('State -> 'T1 -> 'T2 -> 'State) -> 'State -> 'T1 block -> 'T2 block -> 'State 
      
    /// Applies a function to each element of a block from right to left (last to first), threading an accumulator argument through the computation.
    val foldBack : ('T -> 'State -> 'State) -> 'T block -> 'State -> 'State

    /// Applies a function to pairs of elements from two supplied blocks, right-to-left, threading an accumulator argument through the computation. The two input blocks must have the same lengths; otherwise, ArgumentException is raised.
    val foldBack2 : ('T1 -> 'T2 -> 'State -> 'State) -> 'T1 block -> 'T2 block -> 'State -> 'State

    /// Tests whether all elements of a block satisfy the supplied condition.
    val forall : ('T -> bool) -> 'T block -> bool

    /// Tests whether all corresponding elements of two supplied blocks satisfy a supplied condition.
    val forall2 : ('T1 -> 'T2 -> bool) -> 'T1 block -> 'T2 block -> bool

    /// Uses a supplied function to create a block of the supplied dimension.
    val init : int -> f:(int -> 'T) -> 'T block
    
    /// O(1). O(1). Returns true if the block has no elements.
    val isEmpty : 'T block -> bool
    
    /// Applies the supplied function to each element of a block.
    val iter : ('T -> unit) -> 'T block -> unit

    /// Applies the supplied function to a pair of elements from matching indexes in two blocks, also passing the index of the elements. The two blocks must have the same lengths; otherwise, an ArgumentException is raised.
    val iter2 : ('T1 -> 'T2 -> unit) -> 'T1 block -> 'T2 block -> unit

    /// Applies the supplied function to each element of a block. The integer passed to the function indicates the index of the element.
    val iteri : (int -> 'T -> unit) -> 'T block -> unit

    /// O(1). Returns the number of items in the block.
    val length : 'T block -> int

    /// Creates a block whose elements are the results of applying the supplied function to each of the elements of a supplied block.
    val map : ('T1 -> 'T2) -> 'T1 block -> 'T2 block

    /// Creates a block whose elements are the results of applying the supplied function to the corresponding elements of two supplied blocks. The two input blocks must have the same lengths; otherwise, ArgumentException is raised.
    val map2 : ('T1 -> 'T2 -> 'T3) -> 'T1 block -> 'T2 block -> 'T3 block

    /// Creates a block whose elements are the results of applying the supplied function to each of the elements of a supplied block. An integer index passed to the function indicates the index of the element being transformed.
    val mapi : (int -> 'T1 -> 'T2) -> 'T1 block -> 'T2 block
*)
    /// TBD
    val toArray: 'T block -> 'T[]

    /// TBD
    val toList: 'T block -> 'T list

    /// TBD
    val toSeq: 'T block -> 'T seq

    /// Creates a block from the supplied array
    val ofArray: 'T[] -> 'T block

    /// Creates a block from the supplied list.
    val ofList : 'T list -> 'T block

    /// Creates a block from the supplied enumerable object.
    val ofSeq : seq<'T> -> 'T block
(*
    /// Splits a block into two blocks, one containing the elements for which the supplied condition returns true, and the other containing those for which it returns false.
    val partition : ('T -> bool) -> 'T block -> 'T block * 'T block

    /// Reverses the order of the elements in a supplied array.
    val rev : 'T block -> 'T block

    /// O(1). Returns a block of one element.
    val singleton : 'T -> 'T block

    /// Returns the sum of the elements in the block.
    val sum : int block -> int

    /// Returns the sum of the results generated by applying a function to each element of a block.
    val sumBy : ('T -> int) -> 'T block -> int

    /// Converts the supplied block to a list.
    val toList : 'T block -> 'T list

    /// Converts the supplied block of tuple pairs to a map.
    val toMap : block<'Key * 'T> -> Map<'Key,'T> when 'Key : comparison

    /// Returns the first element in the supplied block for which the supplied function returns true. Returns None if no such element exists.
    val tryFind : ('T -> bool) -> 'T block -> 'T option

    /// Splits a block of tuple pairs into a tuple of two blocks.
    val unzip : ('T1 * 'T2) block -> 'T1 block * 'T2 block

    /// Combines two blocks into a block of tuples that have two elements. The two blocks must have equal lengths; otherwise, ArgumentException is raised.
    val zip : 'T1 block -> 'T2 block -> ('T1 * 'T2) block
*)

[<AutoOpen>]
module internal BlockAutoOpens =

    //val block: 'T[] -> 'T block
    val (|Block|): 'T block -> 'T[]

    type System.Collections.Immutable.ImmutableArray<'T> with

        /// O(1). Returns true if the block has no elements.
        member inline IsEmpty : bool

        /// Slicing syntax
        member inline GetSlice: start: int option * finish: int option -> 'T block
