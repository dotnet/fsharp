// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Collections

  open System
  open System.Collections
  open System.Collections.Generic
  open Microsoft.FSharp.Core
  open Microsoft.FSharp.Collections

  [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
  module ISeq =
    module Core =
        [<Struct; NoComparison; NoEquality>]
        type NoValue = struct end

        /// Values is a mutable struct. It can be embedded within the folder type
        /// if two values are required for the calculation.
        [<Struct; NoComparison; NoEquality>]
        type Value<'a> =
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
        type Values<'a,'b,'c> =
            new : a:'a * b:'b * c:'c -> Values<'a,'b,'c>
            val mutable _1: 'a
            val mutable _2: 'b
            val mutable _3: 'c

        /// PipeIdx denotes the index of the element within the pipeline. 0 denotes the
        /// source of the chain.
        type PipeIdx = int

        /// Used within the pipline to provide out of band communications
        type IOutOfBand =
            /// Stop the processing of any further items down the pipeline
            abstract StopFurtherProcessing : PipeIdx -> unit

        /// Activity is the root class for chains of activities. It is in a non-generic
        /// form so that it can be used by subsequent activities
        [<AbstractClass>]
        type Activity =
            /// OnComplete is used to determine if the object has been processed correctly,
            /// and possibly throw exceptions to denote incorrect application (i.e. such as a Take
            /// operation which didn't have a source at least as large as was required). It is
            /// not called in the case of an exception being thrown whilst the stream is still
            /// being processed.
            abstract ChainComplete : PipeIdx -> unit
            /// OnDispose is used to cleanup the stream. It is always called at the last operation
            /// after the enumeration has completed.
            abstract ChainDispose : unit -> unit

        /// Activity is the base class of all elements within the pipeline
        [<AbstractClass>]
        type Activity<'T,'U> =
            inherit Activity
            new : unit -> Activity<'T,'U>
            abstract member ProcessNext : input:'T -> bool

        /// An activity that transforms the input from 'T to 'U, using 'State. It's intended usage
        /// is as a base class for an object expression that will be created 
        /// in the TransformFactory's Compose function.
        [<AbstractClass>]
        type Transform<'T,'U,'State> =
            inherit Activity<'T,'U>
            new : next:Activity * 'State -> Transform<'T,'U,'State>
            val mutable State : 'State
            val private Next : Activity

        /// An activity that transforms the input from 'T to 'U, using 'State
        /// and performs some post processing on the pipeline, either in the case of the stream
        /// ending sucessfully or when disposed. It's intended usage
        /// is as a base class for an object expression that will be created 
        /// in the TransformFactory's Compose function.
        [<AbstractClass>]
        type TransformWithPostProcessing<'T,'U,'State> =
            inherit Transform<'T,'U,'State>
            new : next:Activity * 'State -> TransformWithPostProcessing<'T,'U,'State>
            abstract OnComplete : PipeIdx -> unit
            abstract OnDispose  : unit -> unit

        /// Folder is a base class to assist with fold-like operations. It's intended usage
        /// is as a base class for an object expression that will be used from within
        /// the Fold function.
        [<AbstractClass>]
        type Folder<'T,'Result> =
            inherit Activity<'T,'T>
            new : 'Result -> Folder<'T,'Result>
            interface IOutOfBand
            val mutable Result : 'Result
            val mutable HaltedIdx : int
            member StopFurtherProcessing : PipeIdx -> unit

        /// Folder is a base class to assist with fold-like operations. It's intended usage
        /// is as a base class for an object expression that will be used from within
        /// the Fold function.
        [<AbstractClass>]
        type FolderWithState<'T,'Result,'State> =
            inherit Folder<'T,'Result>
            new : 'Result*'State -> FolderWithState<'T,'Result,'State>
            val mutable State : 'State

        /// Folder is a base class to assist with fold-like operations
        /// and performs some post processing on the pipeline, either in the case of the stream
        /// ending sucessfully or when disposed. It's intended usage
        /// is as a base class for an object expression that will be used from within
        /// the Fold function.
        [<AbstractClass>]
        type FolderWithPostProcessing<'T,'Result,'State> =
            inherit FolderWithState<'T,'Result,'State>
            new : 'Result*'State -> FolderWithPostProcessing<'T,'Result,'State>
            abstract OnDispose : unit -> unit
            abstract OnComplete : PipeIdx -> unit

        /// TransformFactory provides composition of Activities. Its intended to have a specialization
        /// for each type of ISeq Activity. ISeq's PushTransform method is used to build a stack
        /// of Actvities that will be composed.
        [<AbstractClass>]
        type TransformFactory<'T,'U> =
            new : unit -> TransformFactory<'T,'U>
            abstract member Compose : IOutOfBand -> PipeIdx -> Activity<'U,'V> -> Activity<'T,'V>

        /// ISeq<'T> is an extension to seq<'T> that provides the avilty to compose Activities
        /// as well as Fold the current Activity pipeline.
        type ISeq<'T> =
            inherit System.Collections.Generic.IEnumerable<'T>
            abstract member PushTransform : TransformFactory<'T,'U> -> ISeq<'U>
            abstract member Fold<'Result> : f:(PipeIdx->Folder<'T,'Result>) -> 'Result

    open Core

    /// ofResizeArrayUnchecked creates an ISeq over a ResizeArray that accesses the underlying
    /// structure via Index rather than via the GetEnumerator function. This provides faster access
    /// but doesn't check the version of the underlying object which means care has to be taken
    /// to ensure that it is not modified which the result ISeq exists.
    [<CompiledName "OfResizeArrayUnchecked">]
    val ofResizeArrayUnchecked : ResizeArray<'T> -> ISeq<'T>

    [<CompiledName "OfList">]
    val ofList : list<'T> -> ISeq<'T>

    [<CompiledName "OfArray">]
    val ofArray : array<'T> -> ISeq<'T>

    [<CompiledName "OfSeq">]
    val ofSeq : seq<'T> -> ISeq<'T>

    [<CompiledName "Average">]
    val inline average : source: ISeq< ^T> -> ^T
        when 'T:(static member Zero : ^T)
        and  'T:(static member (+) : ^T * ^T -> ^T)
        and  ^T:(static member DivideByInt : ^T * int -> ^T)

    [<CompiledName "AverageBy">]
    val inline averageBy : f:('T -> ^U) -> source:ISeq< 'T > -> ^U
        when ^U:(static member Zero : ^U)
        and  ^U:(static member (+) : ^U * ^U -> ^U)
        and  ^U:(static member DivideByInt : ^U * int -> ^U)

    [<CompiledName "Empty">]
    val empty<'T> : ISeq<'T>

    [<CompiledName "ExactlyOne">]
    val exactlyOne : ISeq<'T> -> 'T

    [<CompiledName "Fold">]
    val inline fold<'T,'State> : f:('State->'T->'State) -> seed:'State -> source:ISeq<'T> -> 'State

    [<CompiledName "Fold2">]
    val inline fold2<'T1,'T2,'State> : folder:('State->'T1->'T2->'State) -> state:'State -> source1: ISeq<'T1> -> source2: ISeq<'T2> -> 'State

    [<CompiledName "Unfold">]
    val unfold : generator:('State -> option<'T*'State>) -> state:'State -> ISeq<'T>

    [<CompiledName "InitializeInfinite">]
    val initInfinite : f:(int -> 'T) -> ISeq<'T>

    [<CompiledName "Initialize">]
    val init : count:int -> f:(int -> 'T) -> ISeq<'T>

    [<CompiledName "Iterate">]
    val inline iter : f:('T -> unit) -> source:ISeq<'T> -> unit

    [<CompiledName "Iterate2">]
    val inline iter2 : f:('T->'U->unit) -> source1 : ISeq<'T> -> source2 : ISeq<'U> -> unit

    [<CompiledName "IterateIndexed2">]
    val inline iteri2 : f:(int->'T->'U->unit) -> source1:ISeq<'T> -> source2:ISeq<'U> -> unit

    [<CompiledName "TryHead">]
    val tryHead : ISeq<'T> -> option<'T>

    [<CompiledName("Head")>]
    val head: source:ISeq<'T> -> 'T

    [<CompiledName "IterateIndexed">]
    val inline iteri : f:(int -> 'T -> unit) -> source:ISeq<'T> -> unit

    [<CompiledName "Except">]
    val inline except : itemsToExclude:seq<'T> -> source:ISeq<'T> -> ISeq<'T> when 'T:equality

    [<CompiledName "Exists">]
    val inline exists : f:('T -> bool) -> source:ISeq<'T> -> bool

    [<CompiledName "Exists2">]
    val inline exists2 : predicate:('T->'U->bool) -> source1:ISeq<'T> -> source2:ISeq<'U> -> bool

    [<CompiledName "Contains">]
    val inline contains : element:'T -> source:ISeq<'T> -> bool when 'T : equality

    [<CompiledName "ForAll">]
    val inline forall : f:('T -> bool) -> source:ISeq<'T> -> bool

    [<CompiledName "ForAll2">]
    val inline forall2 : predicate:('T->'U->bool) -> source1:ISeq<'T> -> source2:ISeq<'U> -> bool

    [<CompiledName "Filter">]
    val inline filter : f:('T -> bool) -> source:ISeq<'T> -> ISeq<'T>

    [<CompiledName "Map">]
    val inline map : f:('T -> 'U) -> source:ISeq<'T> -> ISeq<'U>

    [<CompiledName "MapIndexed">]
    val inline mapi : f:(int->'a->'b) -> source: ISeq<'a> -> ISeq<'b>

    [<CompiledName "Map2">]
    val inline map2<'T,'U,'V> : map:('T->'U->'V) -> source1:ISeq<'T> -> source2:ISeq<'U> -> ISeq<'V>

    [<CompiledName "MapIndexed2">]
    val inline mapi2<'T,'U,'V> : map:(int -> 'T->'U->'V) -> source1:ISeq<'T> -> source2:ISeq<'U> -> ISeq<'V>

    [<CompiledName "Map3">]
    val inline map3<'T,'U,'V,'W> : map:('T->'U->'V->'W) -> source1:ISeq<'T> -> source2:ISeq<'U> -> source3:ISeq<'V> -> ISeq<'W>

    [<CompiledName "CompareWith">]
    val inline compareWith : f:('T->'T->int) -> source1 :ISeq<'T> -> source2:ISeq<'T> -> int

    [<CompiledName "Choose">]
    val inline choose : f:('a->option<'b>) -> source: ISeq<'a> -> ISeq<'b>

    [<CompiledName "Distinct">]
    val inline distinct : source:ISeq<'T> -> ISeq<'T> when 'T:equality

    [<CompiledName "DistinctBy">]
    val inline distinctBy : keyf:('T->'Key) -> source:ISeq<'T> -> ISeq<'T> when 'Key:equality

    [<CompiledName "Max">]
    val inline max : source:ISeq<'T> -> 'T when 'T:comparison

    [<CompiledName "MaxBy">]
    val inline maxBy : f:('T->'U) -> source:ISeq<'T> -> 'T when 'U:comparison

    [<CompiledName "Min">]
    val inline min : source:ISeq<'T> -> 'T when 'T:comparison

    [<CompiledName "MinBy">]
    val inline minBy : f:('T->'U) -> source:ISeq<'T> -> 'T when 'U:comparison

    [<CompiledName "Pairwise">]
    val pairwise : source:ISeq<'T> -> ISeq<'T * 'T>

    [<CompiledName "Reduce">]
    val inline reduce : f:('T->'T->'T) -> source:ISeq<'T> -> 'T

    [<CompiledName "Scan">]
    val inline scan : folder:('State->'T->'State) -> initialState:'State -> source:ISeq<'T> -> ISeq<'State>

    [<CompiledName "Skip">]
    val skip : skipCount:int -> source:ISeq<'T> -> ISeq<'T>

    [<CompiledName "SkipWhile">]
    val inline skipWhile : predicate:('T->bool) -> source:ISeq<'T> -> ISeq<'T>

    [<CompiledName "Sum">]
    val inline sum : source:ISeq<'T> -> 'T
        when 'T:(static member Zero : ^T)
        and  'T:(static member (+) : ^T * ^T -> ^T)

    [<CompiledName "SumBy">]
    val inline sumBy : f :('T -> ^U) -> source:ISeq<'T> -> ^U
        when ^U:(static member Zero : ^U)
        and  ^U:(static member (+) : ^U * ^U -> ^U)

    [<CompiledName "Take">]
    val take : takeCount:int -> source:ISeq<'T> -> ISeq<'T>

    [<CompiledName "TakeWhile">]
    val inline takeWhile : predicate:('T->bool) -> source:ISeq<'T> -> ISeq<'T>

    [<CompiledName "Tail">]
    val tail : source:ISeq<'T> -> ISeq<'T>

    [<CompiledName "Truncate">]
    val truncate : truncateCount:int -> source:ISeq<'T> -> ISeq<'T>

    [<CompiledName "Indexed">]
    val indexed : source: ISeq<'a> -> ISeq<int * 'a>

    [<CompiledName "TryItem">]
    val tryItem : index:int -> source:ISeq<'T> -> option<'T>

    [<CompiledName "TryPick">]
    val inline tryPick : f:('T -> option<'U>) -> source:ISeq<'T> -> option<'U>

    [<CompiledName "TryFind">]
    val inline tryFind : f:('T -> bool) -> source:ISeq<'T> -> option<'T>

    [<CompiledName "TryFindIndex">]
    val inline tryFindIndex: predicate:('T->bool) -> source:ISeq<'T> -> option<int>

    [<CompiledName("Last")>]
    val last: source:ISeq<'T> -> 'T

    [<CompiledName "TryLast">]
    val tryLast : source:ISeq<'T> -> option<'T>

    [<CompiledName "Windowed">]
    val windowed : windowSize:int -> source:ISeq<'T> -> ISeq<array<'T>>

    [<CompiledName "Concat">]
    val concat : sources:ISeq<'Collection> -> ISeq<'T> when 'Collection :> ISeq<'T>

    [<CompiledName "Append">]
    val append: source1:ISeq<'T> -> source2:ISeq<'T> -> ISeq<'T>

    [<CompiledName "Delay">]
    val delay : (unit -> ISeq<'T>) -> ISeq<'T>

    [<CompiledName "GroupByVal">]
    val inline groupByVal : projection:('T -> 'Key) -> source:ISeq<'T> -> ISeq<'Key * ISeq<'T>> when 'Key : equality and 'Key : struct

    [<CompiledName "GroupByRef">]
    val inline groupByRef : projection:('T -> 'Key) -> source:ISeq<'T> -> ISeq<'Key * ISeq<'T>> when 'Key : equality and 'Key : not struct

    [<CompiledName("CountByVal")>]
    val inline countByVal : projection:('T -> 'Key) -> source:ISeq<'T> -> ISeq<'Key * int> when 'Key : equality and 'Key : struct

    [<CompiledName("CountByRef")>]
    val inline countByRef : projection:('T -> 'Key) -> source:ISeq<'T> -> ISeq<'Key * int> when 'Key : equality and 'Key : not struct

    [<CompiledName("Length")>]
    val length: source:ISeq<'T> -> int

    [<CompiledName("ToArray")>]
    val toArray: source:ISeq<'T> -> array<'T>

    [<CompiledName("SortBy")>]
    val sortBy : projection:('T->'Key) -> source:ISeq<'T> -> ISeq<'T> when 'Key : comparison

    [<CompiledName("Sort")>]
    val sort : source:ISeq<'T> -> ISeq<'T> when 'T : comparison

    [<CompiledName("SortWith")>]
    val sortWith : comparer:('T->'T->int) -> source:ISeq<'T> -> ISeq<'T>

    [<CompiledName("Reverse")>]
    val rev: source:ISeq<'T> -> ISeq<'T>

    [<CompiledName("Permute")>]
    val permute: indexMap:(int->int) -> source:ISeq<'T> -> ISeq<'T>

    [<CompiledName("ScanBack")>]
    val scanBack<'T,'State> : folder:('T->'State->'State) -> source:ISeq<'T> -> state:'State -> ISeq<'State>

    [<CompiledName("Zip")>]
    val zip: source1:ISeq<'T1> -> source2:ISeq<'T2> -> ISeq<'T1 * 'T2>

    [<CompiledName("ReduceBack")>]
    val inline reduceBack: reduction:('T->'T->'T) -> source:ISeq<'T> -> 'T

    [<CompiledName("FoldBack")>]
    val inline foldBack<'T,'State> : folder:('T->'State->'State) -> source:ISeq<'T> -> state:'State -> 'State

    [<CompiledName("FoldBack2")>]
    val inline foldBack2<'T1,'T2,'State> : folder:('T1->'T2->'State->'State) -> source1:ISeq<'T1> -> source2:ISeq<'T2> -> state:'State -> 'State

    module internal GroupBy =
        val inline byVal : projection:('T -> 'Key) -> source:ISeq<'T> -> ISeq<'Key * ISeq<'T>> when 'Key : equality
        val inline byRef : projection:('T -> 'Key) -> source:ISeq<'T> -> ISeq<'Key * ISeq<'T>> when 'Key : equality

    module internal CountBy =
        val inline byVal : projection:('T -> 'Key) -> source:ISeq<'T> -> ISeq<'Key * int> when 'Key : equality
        val inline byRef : projection:('T -> 'Key) -> source:ISeq<'T> -> ISeq<'Key * int> when 'Key : equality

    [<CompiledName("Cache")>]
    val cache: source:ISeq<'T> -> ISeq<'T>

    [<CompiledName("Collect")>]
    val collect: mapping:('T -> 'Collection) -> source:ISeq<'T> -> ISeq<'U>  when 'Collection :> ISeq<'U>

    [<CompiledName("AllPairs")>]
    val allPairs: source1:ISeq<'T1> -> source2:ISeq<'T2> -> ISeq<'T1 * 'T2>

    [<CompiledName("ToList")>]
    val toList: source:ISeq<'T> -> 'T list

    [<CompiledName("Replicate")>]
    val replicate: count:int -> initial:'T -> ISeq<'T>

    [<CompiledName("IsEmpty")>]
    val isEmpty: source:ISeq<'T> -> bool

    [<CompiledName("Cast")>]
    val cast: source:IEnumerable -> ISeq<'T>

    [<CompiledName("ChunkBySize")>]
    val chunkBySize: chunkSize:int -> source:ISeq<'T> -> ISeq<'T[]>

    [<CompiledName("SplitInto")>]
    val splitInto: count:int -> source:ISeq<'T> -> ISeq<'T[]>

    [<CompiledName("Find")>]
    val find: predicate:('T -> bool) -> source:ISeq<'T> -> 'T

    [<CompiledName("FindBack")>]
    val findBack: predicate:('T -> bool) -> source:ISeq<'T> -> 'T

    [<CompiledName("FindIndex")>]
    val findIndex: predicate:('T -> bool) -> source:ISeq<'T> -> int

    [<CompiledName("FindIndexBack")>]
    val findIndexBack: predicate:('T -> bool) -> source:ISeq<'T> -> int

    [<CompiledName("Pick")>]
    val pick: chooser:('T -> 'U option) -> source:ISeq<'T> -> 'U 

    [<CompiledName("MapFold")>]
    val mapFold<'T,'State,'Result> : mapping:('State -> 'T -> 'Result * 'State) -> state:'State -> source:ISeq<'T> -> ISeq<'Result> * 'State

    [<CompiledName("MapFoldBack")>]
    val mapFoldBack<'T,'State,'Result> : mapping:('T -> 'State -> 'Result * 'State) -> source:ISeq<'T> -> state:'State -> ISeq<'Result> * 'State

    [<CompiledName("Item")>]
    val item: index:int -> source:ISeq<'T> -> 'T

    [<CompiledName("Singleton")>]
    val singleton: value:'T -> ISeq<'T>

    [<CompiledName("SortDescending")>]
    val inline sortDescending : source:ISeq<'T> -> ISeq<'T> when 'T : comparison

    [<CompiledName("SortByDescending")>]
    val inline sortByDescending : projection:('T -> 'Key) -> source:ISeq<'T> -> ISeq<'T> when 'Key : comparison

    [<CompiledName("TryFindBack")>]
    val tryFindBack: predicate:('T -> bool) -> source:ISeq<'T> -> 'T option

    [<CompiledName("TryFindIndexBack")>]
    val tryFindIndexBack : predicate:('T -> bool) -> source:ISeq<'T> -> int option

    [<CompiledName("Zip3")>]
    val zip3: source1:ISeq<'T1> -> source2:ISeq<'T2> -> source3:ISeq<'T3> -> ISeq<'T1 * 'T2 * 'T3>
