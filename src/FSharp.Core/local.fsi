// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Core

open Microsoft.FSharp.Core

[<AutoOpen>]
module internal DetailedExceptions =
    val inline invalidArgFmt: arg: string -> format: string -> paramArray: objnull array -> 'T
    val inline invalidOpFmt: format: string -> paramArray: objnull array -> 'T
    val invalidArgDifferentListLength: arg1: string -> arg2: string -> diff: int -> 'T

    val invalidArg3ListsDifferent:
        arg1: string -> arg2: string -> arg3: string -> len1: int -> len2: int -> len3: int -> 'T

    val invalidOpListNotEnoughElements: index: int -> 'T
    val invalidOpExceededSeqLength: fnName: string -> diff: int -> len: int -> 'T
    val inline invalidArgInputMustBeNonNegative: arg: string -> count: int -> 'T
    val inline invalidArgInputMustBePositive: arg: string -> count: int -> 'T
    val invalidArgOutOfRange: arg: string -> index: int -> text: string -> bound: int -> 'T
    val invalidArgDifferentArrayLength: arg1: string -> len1: int -> arg2: string -> len2: int -> 'T

    val invalidArg3ArraysDifferent:
        arg1: string -> arg2: string -> arg3: string -> len1: int -> len2: int -> len3: int -> 'T

// Definitions internal for this library.
namespace Microsoft.FSharp.Primitives.Basics

open System
open Microsoft.FSharp.Core
open Microsoft.FSharp.Collections

module internal List =
    val allPairs: 'T1 list -> 'T2 list -> ('T1 * 'T2) list
    val choose: ('T -> 'U option) -> 'T list -> 'U list
    val countBy: System.Collections.Generic.Dictionary<'T1, int> -> ('T1 -> 'T2) -> ('T2 * int) list
    val pairwise: 'T list -> ('T * 'T) list

    val groupBy:
        System.Collections.Generic.IEqualityComparer<'SafeKey> ->
        ('T -> 'SafeKey) ->
        ('SafeKey -> 'Key) ->
        'T list ->
            ('Key * 'T list) list

    val distinctWithComparer: System.Collections.Generic.IEqualityComparer<'T> -> 'T list -> 'T list

    val distinctByWithComparer:
        System.Collections.Generic.IEqualityComparer<'Key> -> ('T -> 'Key) -> list: 'T list -> 'T list
            when 'Key: equality

    val init: int -> (int -> 'T) -> 'T list
    val filter: predicate: ('T -> bool) -> 'T list -> 'T list
    val collect: ('T -> 'U list) -> 'T list -> 'U list
    val partition: predicate: ('T -> bool) -> 'T list -> 'T list * 'T list
    val map: mapping: ('T -> 'U) -> 'T list -> 'U list
    val map2: mapping: ('T1 -> 'T2 -> 'U) -> 'T1 list -> 'T2 list -> 'U list
    val map3: mapping: ('T1 -> 'T2 -> 'T3 -> 'U) -> 'T1 list -> 'T2 list -> 'T3 list -> 'U list
    val scan: ('State -> 'T -> 'State) -> 'State -> 'T list -> 'State list
    val mapi: (int -> 'T -> 'U) -> 'T list -> 'U list
    val mapi2: (int -> 'T1 -> 'T2 -> 'U) -> 'T1 list -> 'T2 list -> 'U list
    val indexed: 'T list -> (int * 'T) list
    val mapFold: ('State -> 'T -> 'U * 'State) -> 'State -> 'T list -> 'U list * 'State
    val forall: predicate: ('T -> bool) -> 'T list -> bool
    val exists: predicate: ('T -> bool) -> 'T list -> bool
    val rev: 'T list -> 'T list
    val concat: seq<'T list> -> 'T list
    val unfold: ('State -> ('T * 'State) option) -> 'State -> 'T list
    val unzip: ('T1 * 'T2) list -> 'T1 list * 'T2 list
    val unzip3: ('T1 * 'T2 * 'T3) list -> 'T1 list * 'T2 list * 'T3 list
    val windowed: int -> 'T list -> 'T list list
    val chunkBySize: int -> 'T list -> 'T list list
    val splitInto: int -> 'T list -> 'T list list
    val zip: 'T1 list -> 'T2 list -> ('T1 * 'T2) list
    val zip3: 'T1 list -> 'T2 list -> 'T3 list -> ('T1 * 'T2 * 'T3) list
    val ofArray: 'T array -> 'T list
    val take: int -> 'T list -> 'T list
    val takeWhile: ('T -> bool) -> 'T list -> 'T list
    val toArray: 'T list -> 'T array
    val inline ofSeq: seq<'T> -> 'T List
    val splitAt: int -> 'T list -> ('T list * 'T list)
    val transpose: 'T list list -> 'T list list
    val truncate: int -> 'T list -> 'T list
    val tryLastV: 'T list -> 'T ValueOption

module internal Array =
    // The input parameter should be checked by callers if necessary
    val inline zeroCreateUnchecked: int -> 'T array

    val inline init: int -> (int -> 'T) -> 'T array

    val splitInto: int -> 'T array -> 'T array array

    val findBack: predicate: ('T -> bool) -> array: 'T array -> 'T

    val tryFindBack: predicate: ('T -> bool) -> array: 'T array -> 'T option

    val findIndexBack: predicate: ('T -> bool) -> array: 'T array -> int

    val tryFindIndexBack: predicate: ('T -> bool) -> array: 'T array -> int option

    val mapFold: ('State -> 'T -> 'U * 'State) -> 'State -> 'T array -> 'U array * 'State

    val mapFoldBack: ('T -> 'State -> 'U * 'State) -> 'T array -> 'State -> 'U array * 'State

    val permute: indexMap: (int -> int) -> 'T array -> 'T array

    val scanSubRight:
        f: ('T -> 'State -> 'State) -> array: 'T array -> start: int -> fin: int -> initState: 'State -> 'State array

    val inline subUnchecked: int -> int -> 'T array -> 'T array

    val unstableSortInPlaceBy: projection: ('T -> 'Key) -> array: 'T array -> unit when 'Key: comparison

    val unstableSortInPlace: array: 'T array -> unit when 'T: comparison

    val stableSortInPlaceBy: projection: ('T -> 'Key) -> array: 'T array -> unit when 'Key: comparison

    val stableSortInPlaceWith: comparer: ('T -> 'T -> int) -> array: 'T array -> unit

    val stableSortInPlace: array: 'T array -> unit when 'T: comparison

module internal Random =

    val next: randomizer: (unit -> float) -> minValue: int -> maxValue: int -> int
    val getMaxSetSizeForSampling: count: int -> int

    val shuffleArrayInPlaceWith: random: Random -> array: 'T[] -> unit
    val shuffleArrayInPlaceBy: randomizer: (unit -> float) -> array: 'T[] -> unit

module internal Seq =
    val tryLastV: 'T seq -> 'T ValueOption
