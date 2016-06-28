// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// Definitions internal for this library.
namespace Microsoft.FSharp.Primitives.Basics 

open Microsoft.FSharp.Core
open Microsoft.FSharp.Collections

module internal List =
    val allPairs : 'T1 list -> 'T2 list -> ('T1 * 'T2) list
    val distinctWithComparer : System.Collections.Generic.IEqualityComparer<'T> -> 'T list -> 'T list
    val distinctByWithComparer : System.Collections.Generic.IEqualityComparer<'Key> -> ('T -> 'Key) -> list:'T list -> 'T list when 'Key : equality
    val init : int -> (int -> 'T) -> 'T list
    val iter : ('T -> unit) -> 'T list -> unit
    val filter : predicate:('T -> bool) -> 'T list -> 'T list
    val collect : ('T -> 'U list) -> 'T list -> 'U list
    val partition : predicate:('T -> bool) -> 'T list -> 'T list * 'T list
    val map : mapping:('T -> 'U) -> 'T list -> 'U list
    val map2 : mapping:('T1 -> 'T2 -> 'U) -> 'T1 list -> 'T2 list -> 'U list
    val mapi : (int -> 'T -> 'U) -> 'T list -> 'U list
    val indexed : 'T list -> (int * 'T) list
    val mapFold : ('State -> 'T -> 'U * 'State) -> 'State -> 'T list -> 'U list * 'State
    val forall : predicate:('T -> bool) -> 'T list -> bool
    val exists : predicate:('T -> bool) -> 'T list -> bool
    val rev: 'T list -> 'T list
    val concat : seq<'T list> -> 'T list
    val iteri : action:(int -> 'T -> unit) -> 'T list -> unit
    val unfold : ('State -> ('T * 'State) option) -> 'State -> 'T list
    val unzip : ('T1 * 'T2) list -> 'T1 list * 'T2 list
    val unzip3 : ('T1 * 'T2 * 'T3) list -> 'T1 list * 'T2 list * 'T3 list
    val windowed : int -> 'T list -> 'T list list
    val chunkBySize : int -> 'T list -> 'T list list
    val splitInto : int -> 'T list -> 'T list list
    val zip : 'T1 list -> 'T2 list -> ('T1 * 'T2) list
    val zip3 : 'T1 list -> 'T2 list -> 'T3 list -> ('T1 * 'T2 * 'T3) list
    val ofArray : 'T[] -> 'T list
    val take : int -> 'T list -> 'T list
    val takeWhile : ('T -> bool) -> 'T list -> 'T list
    val toArray : 'T list -> 'T[]
    val inline ofSeq : seq<'T> -> 'T List
    val splitAt : int -> 'T list -> ('T list * 'T list)
    val truncate : int -> 'T list -> 'T list

module internal Array =
    // The input parameter should be checked by callers if necessary
    val inline zeroCreateUnchecked : int -> 'T[]

    val inline init : int -> (int -> 'T) -> 'T[]

    val splitInto : int -> 'T[] -> 'T[][]

    val findBack: predicate:('T -> bool) -> array:'T[] -> 'T

    val tryFindBack: predicate:('T -> bool) -> array:'T[] -> 'T option

    val findIndexBack: predicate:('T -> bool) -> array:'T[] -> int

    val tryFindIndexBack: predicate:('T -> bool) -> array:'T[] -> int option

    val mapFold : ('State -> 'T -> 'U * 'State) -> 'State -> 'T[] -> 'U[] * 'State

    val mapFoldBack : ('T -> 'State -> 'U * 'State) -> 'T[] -> 'State -> 'U[] * 'State

    val permute : indexMap:(int -> int) -> 'T[] -> 'T[]
    
    val scanSubRight: f:('T -> 'State -> 'State) -> array:'T[] -> start:int -> fin:int -> initState:'State -> 'State[]

    val inline subUnchecked : int -> int -> 'T[] -> 'T[]

    val unstableSortInPlaceBy: projection:('T -> 'Key) -> array:'T[] -> unit when 'Key : comparison 

    val unstableSortInPlace: array:'T[] -> unit when 'T : comparison 

    val stableSortInPlaceBy: projection:('T -> 'Key) -> array:'T[] -> unit when 'Key : comparison 

    val stableSortInPlaceWith: comparer:('T -> 'T -> int) -> array:'T[] -> unit

    val stableSortInPlace: array:'T[] -> unit when 'T : comparison 
