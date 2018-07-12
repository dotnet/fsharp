// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Collections

  open System
  open System.Collections
  open System.Collections.Generic
  open Microsoft.FSharp.Core
  open Microsoft.FSharp.Collections
  open Microsoft.FSharp.Collections.SeqComposition

  [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
  module IConsumableSeq =
    module Core =
        /// Values is a mutable struct. It can be embedded within the folder type
        /// if two values are required for the calculation.
        [<Struct; NoComparison; NoEquality>]
        type internal Value<'a> =
            new : a:'a -> Value<'a>
            val mutable _1: 'a

        /// Values is a mutable struct. It can be embedded within the folder type
        /// if two values are required for the calculation.
        [<Struct; NoComparison; NoEquality>]
        type Values<'a,'b> =
            new : a:'a * b:'b -> Values<'a,'b>
            val mutable _1: 'a
            val mutable _2: 'b

        /// Values is a mutable struct. It can be embedded within the folder type
        /// if three values are required for the calculation.
        [<Struct; NoComparison; NoEquality>]
        type internal Values<'a,'b,'c> =
            new : a:'a * b:'b * c:'c -> Values<'a,'b,'c>
            val mutable _1: 'a
            val mutable _2: 'b
            val mutable _3: 'c


        /// An activity that transforms the input from 'T to 'U, using 'State. It's intended usage
        /// is as a base class for an object expression that will be created 
        /// in the ISeqTransform's Compose function.
        [<AbstractClass>]
        type internal SeqTransformActivity<'T,'U,'State> =
            inherit SeqConsumerActivity<'T,'U>
            new : next:SeqConsumerActivity * 'State -> SeqTransformActivity<'T,'U,'State>
            val mutable State : 'State
            val private Next : SeqConsumerActivity

        /// An activity that transforms the input from 'T to 'U, using 'State
        /// and performs some post processing on the pipeline, either in the case of the stream
        /// ending sucessfully or when disposed. It's intended usage
        /// is as a base class for an object expression that will be created 
        /// in the ISeqTransform's Compose function.
        [<AbstractClass>]
        type internal SeqTransformActivityWithPostProcessing<'T,'U,'State> =
            inherit SeqTransformActivity<'T,'U,'State>
            new : next:SeqConsumerActivity * 'State -> SeqTransformActivityWithPostProcessing<'T,'U,'State>

            /// OnComplete is used to determine if the object has been processed correctly,
            /// and possibly throw exceptions to denote incorrect application (i.e. such as a Take
            /// operation which didn't have a source at least as large as was required). It is
            /// not called in the case of an exception being thrown whilst the stream is still
            /// being processed.
            abstract OnComplete : PipeIdx -> unit

            /// OnDispose is used to cleanup the stream. It is always called at the last operation
            /// after the enumeration has completed.
            abstract OnDispose  : unit -> unit

        /// SeqConsumer is a base class to assist with fold-like operations. It's intended usage
        /// is as a base class for an object expression that will be used from within
        /// the Fold function.
        [<AbstractClass>]
        type SeqConsumerWithState<'T,'Result,'State> =
            inherit SeqConsumer<'T,'Result>
            new : 'Result*'State -> SeqConsumerWithState<'T,'Result,'State>
            val mutable State : 'State

        /// SeqConsumer is a base class to assist with fold-like operations
        /// and performs some post processing on the pipeline, either in the case of the stream
        /// ending sucessfully or when disposed. It's intended usage
        /// is as a base class for an object expression that will be used from within
        /// the Fold function.
        [<AbstractClass>]
        type SeqConsumerWithPostProcessing<'T,'Result,'State> =
            inherit SeqConsumerWithState<'T,'Result,'State>
            new : 'Result*'State -> SeqConsumerWithPostProcessing<'T,'Result,'State>
            abstract OnDispose : unit -> unit
            abstract OnComplete : PipeIdx -> unit

    open Core

    /// ofResizeArrayUnchecked creates an IConsumableSeq over a ResizeArray that accesses the underlying
    /// structure via Index rather than via the GetEnumerator function. This provides faster access
    /// but doesn't check the version of the underlying object which means care has to be taken
    /// to ensure that it is not modified which the result IConsumableSeq exists.
    [<CompiledName "OfResizeArrayUnchecked">]
    val internal ofResizeArrayUnchecked : ResizeArray<'T> -> IConsumableSeq<'T>

    [<CompiledName "OfList">]
    val internal ofList : list<'T> -> IConsumableSeq<'T>

    [<CompiledName "OfArray">]
    val internal ofArray : array<'T> -> IConsumableSeq<'T>

    [<CompiledName "OfSeq">]
    val ofSeq : seq<'T> -> IConsumableSeq<'T>

    [<CompiledName "Average">]
    val inline average : source: IConsumableSeq< ^T> -> ^T
        when 'T:(static member Zero : ^T)
        and  'T:(static member (+) : ^T * ^T -> ^T)
        and  ^T:(static member DivideByInt : ^T * int -> ^T)

    [<CompiledName "AverageBy">]
    val inline averageBy : f:('T -> ^U) -> source:IConsumableSeq< 'T > -> ^U
        when ^U:(static member Zero : ^U)
        and  ^U:(static member (+) : ^U * ^U -> ^U)
        and  ^U:(static member DivideByInt : ^U * int -> ^U)

    [<CompiledName "Empty">]
    val internal empty<'T> : IConsumableSeq<'T>

    [<CompiledName "ExactlyOne">]
    val internal exactlyOne : IConsumableSeq<'T> -> 'T

    [<CompiledName "Fold">]
    val inline internal fold<'T,'State> : f:('State->'T->'State) -> seed:'State -> source:IConsumableSeq<'T> -> 'State

    [<CompiledName "Fold2">]
    val inline internal fold2<'T1,'T2,'State> : folder:('State->'T1->'T2->'State) -> state:'State -> source1: IConsumableSeq<'T1> -> source2: IConsumableSeq<'T2> -> 'State

    [<CompiledName "Unfold">]
    val internal unfold : generator:('State -> option<'T*'State>) -> state:'State -> IConsumableSeq<'T>

    [<CompiledName "InitializeInfinite">]
    val internal initInfinite : f:(int -> 'T) -> IConsumableSeq<'T>

    [<CompiledName "Initialize">]
    val internal init : count:int -> f:(int -> 'T) -> IConsumableSeq<'T>

    [<CompiledName "Iterate">]
    val inline internal iter : action:('T -> unit) -> source:IConsumableSeq<'T> -> unit

    [<CompiledName "Iterate2">]
    val inline internal iter2 : action:('T->'U->unit) -> source1 : IConsumableSeq<'T> -> source2 : IConsumableSeq<'U> -> unit

    [<CompiledName "IterateIndexed2">]
    val inline internal iteri2 : action:(int->'T->'U->unit) -> source1:IConsumableSeq<'T> -> source2:IConsumableSeq<'U> -> unit

    [<CompiledName "TryHead">]
    val internal tryHead : IConsumableSeq<'T> -> option<'T>

    [<CompiledName("Head")>]
    val internal head: source:IConsumableSeq<'T> -> 'T

    [<CompiledName "IterateIndexed">]
    val inline internal iteri : action:(int -> 'T -> unit) -> source:IConsumableSeq<'T> -> unit

    [<CompiledName "Except">]
    val inline internal except : itemsToExclude:seq<'T> -> source:IConsumableSeq<'T> -> IConsumableSeq<'T> when 'T:equality

    [<CompiledName "Exists">]
    val inline internal exists : predicate:('T -> bool) -> source:IConsumableSeq<'T> -> bool

    [<CompiledName "Exists2">]
    val inline internal exists2 : predicate:('T->'U->bool) -> source1:IConsumableSeq<'T> -> source2:IConsumableSeq<'U> -> bool

    [<CompiledName "Contains">]
    val inline contains : element:'T -> source:IConsumableSeq<'T> -> bool when 'T : equality

    [<CompiledName "ForAll">]
    val inline internal forall : predicate:('T -> bool) -> source:IConsumableSeq<'T> -> bool

    [<CompiledName "ForAll2">]
    val inline internal forall2 : predicate:('T->'U->bool) -> source1:IConsumableSeq<'T> -> source2:IConsumableSeq<'U> -> bool

    [<CompiledName "Filter">]
    val inline internal filter : predicate:('T -> bool) -> source:IConsumableSeq<'T> -> IConsumableSeq<'T>

    [<CompiledName "Map">]
    val inline internal map : mapping:('T -> 'U) -> source:IConsumableSeq<'T> -> IConsumableSeq<'U>

    [<CompiledName "MapIndexed">]
    val inline internal mapi : mapping:(int->'a->'b) -> source: IConsumableSeq<'a> -> IConsumableSeq<'b>

    [<CompiledName "Map2">]
    val inline internal map2<'T,'U,'V> : mapping:('T->'U->'V) -> source1:IConsumableSeq<'T> -> source2:IConsumableSeq<'U> -> IConsumableSeq<'V>

    [<CompiledName "MapIndexed2">]
    val inline internal mapi2<'T,'U,'V> : mapping:(int -> 'T->'U->'V) -> source1:IConsumableSeq<'T> -> source2:IConsumableSeq<'U> -> IConsumableSeq<'V>

    [<CompiledName "Map3">]
    val inline internal map3<'T,'U,'V,'W> : mapping:('T->'U->'V->'W) -> source1:IConsumableSeq<'T> -> source2:IConsumableSeq<'U> -> source3:IConsumableSeq<'V> -> IConsumableSeq<'W>

    [<CompiledName "CompareWith">]
    val inline internal compareWith : comparer:('T->'T->int) -> source1 :IConsumableSeq<'T> -> source2:IConsumableSeq<'T> -> int

    [<CompiledName "Choose">]
    val inline internal choose : f:('a->option<'b>) -> source: IConsumableSeq<'a> -> IConsumableSeq<'b>

    [<CompiledName "Distinct">]
    val inline internal distinct : source:IConsumableSeq<'T> -> IConsumableSeq<'T> when 'T:equality

    [<CompiledName "DistinctBy">]
    val inline internal distinctBy : keyf:('T->'Key) -> source:IConsumableSeq<'T> -> IConsumableSeq<'T> when 'Key:equality

    [<CompiledName "Max">]
    val inline max : source:IConsumableSeq<'T> -> 'T when 'T:comparison

    [<CompiledName "MaxBy">]
    val inline maxBy : f:('T->'U) -> source:IConsumableSeq<'T> -> 'T when 'U:comparison

    [<CompiledName "Min">]
    val inline min : source:IConsumableSeq<'T> -> 'T when 'T:comparison

    [<CompiledName "MinBy">]
    val inline minBy : f:('T->'U) -> source:IConsumableSeq<'T> -> 'T when 'U:comparison

    [<CompiledName "Pairwise">]
    val internal pairwise : source:IConsumableSeq<'T> -> IConsumableSeq<'T * 'T>

    [<CompiledName "Reduce">]
    val inline internal reduce : f:('T->'T->'T) -> source:IConsumableSeq<'T> -> 'T

    [<CompiledName "Scan">]
    val internal scan : folder:('State->'T->'State) -> initialState:'State -> source:IConsumableSeq<'T> -> IConsumableSeq<'State>

    [<CompiledName "Skip">]
    val internal skip : skipCount:int -> source:IConsumableSeq<'T> -> IConsumableSeq<'T>

    [<CompiledName "SkipWhile">]
    val inline internal skipWhile : predicate:('T->bool) -> source:IConsumableSeq<'T> -> IConsumableSeq<'T>

    [<CompiledName "Sum">]
    val inline sum : source:IConsumableSeq<'T> -> 'T
        when 'T:(static member Zero : ^T)
        and  'T:(static member (+) : ^T * ^T -> ^T)

    [<CompiledName "SumBy">]
    val inline sumBy : f :('T -> ^U) -> source:IConsumableSeq<'T> -> ^U
        when ^U:(static member Zero : ^U)
        and  ^U:(static member (+) : ^U * ^U -> ^U)

    [<CompiledName "Take">]
    val internal take : takeCount:int -> source:IConsumableSeq<'T> -> IConsumableSeq<'T>

    [<CompiledName "TakeWhile">]
    val inline internal takeWhile : predicate:('T->bool) -> source:IConsumableSeq<'T> -> IConsumableSeq<'T>

    [<CompiledName "Tail">]
    val internal tail : source:IConsumableSeq<'T> -> IConsumableSeq<'T>

    [<CompiledName "Truncate">]
    val internal truncate : truncateCount:int -> source:IConsumableSeq<'T> -> IConsumableSeq<'T>

    [<CompiledName "Indexed">]
    val internal indexed : source: IConsumableSeq<'a> -> IConsumableSeq<int * 'a>

    [<CompiledName "TryItem">]
    val internal tryItem : index:int -> source:IConsumableSeq<'T> -> option<'T>

    [<CompiledName "TryPick">]
    val inline internal tryPick : f:('T -> option<'U>) -> source:IConsumableSeq<'T> -> option<'U>

    [<CompiledName "TryFind">]
    val inline internal tryFind : f:('T -> bool) -> source:IConsumableSeq<'T> -> option<'T>

    [<CompiledName "TryFindIndex">]
    val inline internal tryFindIndex: predicate:('T->bool) -> source:IConsumableSeq<'T> -> option<int>

    [<CompiledName("Last")>]
    val internal last: source:IConsumableSeq<'T> -> 'T

    [<CompiledName "TryLast">]
    val internal tryLast : source:IConsumableSeq<'T> -> option<'T>

    [<CompiledName "Windowed">]
    val internal windowed : windowSize:int -> source:IConsumableSeq<'T> -> IConsumableSeq<array<'T>>

    [<CompiledName "Concat">]
    val internal concat : sources:IConsumableSeq<'Collection> -> IConsumableSeq<'T> when 'Collection :> IConsumableSeq<'T>

    [<CompiledName "Append">]
    val internal append: source1:IConsumableSeq<'T> -> source2:IConsumableSeq<'T> -> IConsumableSeq<'T>

    [<CompiledName "Delay">]
    val internal delay : (unit -> IConsumableSeq<'T>) -> IConsumableSeq<'T>

    [<CompiledName "GroupByVal">]
    val inline internal groupByVal : projection:('T -> 'Key) -> source:IConsumableSeq<'T> -> IConsumableSeq<'Key * IConsumableSeq<'T>> when 'Key : equality and 'Key : struct

    [<CompiledName "GroupByRef">]
    val inline internal groupByRef : projection:('T -> 'Key) -> source:IConsumableSeq<'T> -> IConsumableSeq<'Key * IConsumableSeq<'T>> when 'Key : equality and 'Key : not struct

    [<CompiledName("CountByVal")>]
    val inline internal countByVal : projection:('T -> 'Key) -> source:IConsumableSeq<'T> -> IConsumableSeq<'Key * int> when 'Key : equality and 'Key : struct

    [<CompiledName("CountByRef")>]
    val inline internal countByRef : projection:('T -> 'Key) -> source:IConsumableSeq<'T> -> IConsumableSeq<'Key * int> when 'Key : equality and 'Key : not struct

    [<CompiledName("Length")>]
    val internal length: source:IConsumableSeq<'T> -> int

    [<CompiledName("ToArray")>]
    val internal toArray: source:IConsumableSeq<'T> -> array<'T>

    [<CompiledName("ToResizeArray")>]
    val internal toResizeArray: source:IConsumableSeq<'T> -> ResizeArray<'T>

    [<CompiledName("SortBy")>]
    val internal sortBy : projection:('T->'Key) -> source:IConsumableSeq<'T> -> IConsumableSeq<'T> when 'Key : comparison

    [<CompiledName("Sort")>]
    val internal sort : source:IConsumableSeq<'T> -> IConsumableSeq<'T> when 'T : comparison

    [<CompiledName("SortWith")>]
    val sortWith : comparer:('T->'T->int) -> source:IConsumableSeq<'T> -> IConsumableSeq<'T>

    [<CompiledName("Reverse")>]
    val internal rev: source:IConsumableSeq<'T> -> IConsumableSeq<'T>

    [<CompiledName("Permute")>]
    val internal permute: indexMap:(int->int) -> source:IConsumableSeq<'T> -> IConsumableSeq<'T>

    [<CompiledName("ScanBack")>]
    val internal scanBack<'T,'State> : folder:('T->'State->'State) -> source:IConsumableSeq<'T> -> state:'State -> IConsumableSeq<'State>

    [<CompiledName("Zip")>]
    val internal zip: source1:IConsumableSeq<'T1> -> source2:IConsumableSeq<'T2> -> IConsumableSeq<'T1 * 'T2>

    [<CompiledName("ReduceBack")>]
    val inline internal reduceBack: reduction:('T->'T->'T) -> source:IConsumableSeq<'T> -> 'T

    [<CompiledName("FoldBack")>]
    val inline internal foldBack<'T,'State> : folder:('T->'State->'State) -> source:IConsumableSeq<'T> -> state:'State -> 'State

    [<CompiledName("FoldBack2")>]
    val inline internal foldBack2<'T1,'T2,'State> : folder:('T1->'T2->'State->'State) -> source1:IConsumableSeq<'T1> -> source2:IConsumableSeq<'T2> -> state:'State -> 'State

    module internal GroupBy =
        val inline byVal : projection:('T -> 'Key) -> source:IConsumableSeq<'T> -> IConsumableSeq<'Key * IConsumableSeq<'T>> when 'Key : equality
        val inline byRef : projection:('T -> 'Key) -> source:IConsumableSeq<'T> -> IConsumableSeq<'Key * IConsumableSeq<'T>> when 'Key : equality

    module internal CountBy =
        val inline byVal : projection:('T -> 'Key) -> source:IConsumableSeq<'T> -> IConsumableSeq<'Key * int> when 'Key : equality
        val inline byRef : projection:('T -> 'Key) -> source:IConsumableSeq<'T> -> IConsumableSeq<'Key * int> when 'Key : equality

    [<CompiledName("Cache")>]
    val internal cache: source:IConsumableSeq<'T> -> IConsumableSeq<'T>

    [<CompiledName("Collect")>]
    val internal collect: mapping:('T -> 'Collection) -> source:IConsumableSeq<'T> -> IConsumableSeq<'U>  when 'Collection :> IConsumableSeq<'U>

    [<CompiledName("AllPairs")>]
    val internal allPairs: source1:IConsumableSeq<'T1> -> source2:IConsumableSeq<'T2> -> IConsumableSeq<'T1 * 'T2>

    [<CompiledName("ToList")>]
    val internal toList: source:IConsumableSeq<'T> -> 'T list

    [<CompiledName("Replicate")>]
    val internal replicate: count:int -> initial:'T -> IConsumableSeq<'T>

    [<CompiledName("IsEmpty")>]
    val internal isEmpty: source:IConsumableSeq<'T> -> bool

    [<CompiledName("Cast")>]
    val internal cast: source:IEnumerable -> IConsumableSeq<'T>

    [<CompiledName("ChunkBySize")>]
    val internal chunkBySize: chunkSize:int -> source:IConsumableSeq<'T> -> IConsumableSeq<'T[]>

    [<CompiledName("SplitInto")>]
    val internal splitInto: count:int -> source:IConsumableSeq<'T> -> IConsumableSeq<'T[]>

    [<CompiledName("Find")>]
    val internal find: predicate:('T -> bool) -> source:IConsumableSeq<'T> -> 'T

    [<CompiledName("FindBack")>]
    val internal findBack: predicate:('T -> bool) -> source:IConsumableSeq<'T> -> 'T

    [<CompiledName("FindIndex")>]
    val internal findIndex: predicate:('T -> bool) -> source:IConsumableSeq<'T> -> int

    [<CompiledName("FindIndexBack")>]
    val internal findIndexBack: predicate:('T -> bool) -> source:IConsumableSeq<'T> -> int

    [<CompiledName("Pick")>]
    val internal pick: chooser:('T -> 'U option) -> source:IConsumableSeq<'T> -> 'U 

    [<CompiledName("MapFold")>]
    val internal mapFold<'T,'State,'Result> : mapping:('State -> 'T -> 'Result * 'State) -> state:'State -> source:IConsumableSeq<'T> -> IConsumableSeq<'Result> * 'State

    [<CompiledName("MapFoldBack")>]
    val internal mapFoldBack<'T,'State,'Result> : mapping:('T -> 'State -> 'Result * 'State) -> source:IConsumableSeq<'T> -> state:'State -> IConsumableSeq<'Result> * 'State

    [<CompiledName("Item")>]
    val internal item: index:int -> source:IConsumableSeq<'T> -> 'T

    [<CompiledName("Singleton")>]
    val internal singleton: value:'T -> IConsumableSeq<'T>

    [<CompiledName("SortDescending")>]
    val inline sortDescending : source:IConsumableSeq<'T> -> IConsumableSeq<'T> when 'T : comparison

    [<CompiledName("SortByDescending")>]
    val inline sortByDescending : projection:('T -> 'Key) -> source:IConsumableSeq<'T> -> IConsumableSeq<'T> when 'Key : comparison

    [<CompiledName("TryFindBack")>]
    val internal tryFindBack: predicate:('T -> bool) -> source:IConsumableSeq<'T> -> 'T option

    [<CompiledName("TryFindIndexBack")>]
    val internal tryFindIndexBack : predicate:('T -> bool) -> source:IConsumableSeq<'T> -> int option

    [<CompiledName("Zip3")>]
    val internal zip3: source1:IConsumableSeq<'T1> -> source2:IConsumableSeq<'T2> -> source3:IConsumableSeq<'T3> -> IConsumableSeq<'T1 * 'T2 * 'T3>
