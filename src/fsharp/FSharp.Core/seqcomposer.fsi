// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Collections

  open System
  open System.Collections
  open System.Collections.Generic
  open Microsoft.FSharp.Core
  open Microsoft.FSharp.Collections

  [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
  module Composer =
    module Core =
        [<Struct; NoComparison; NoEquality>]
        type NoValue = struct end

        /// <summary>Values is a mutable struct. It can be embedded within the folder type
        /// if two values are required for the calculation.</summary>
        [<Struct; NoComparison; NoEquality>]
        type Values<'a,'b> =
            new : a:'a * b:'b -> Values<'a,'b>
            val mutable _1: 'a
            val mutable _2: 'b

        /// <summary>Values is a mutable struct. It can be embedded within the folder type
        /// if three values are required for the calculation.</summary>
        [<Struct; NoComparison; NoEquality>]
        type Values<'a,'b,'c> =
            new : a:'a * b:'b * c:'c -> Values<'a,'b,'c>
            val mutable _1: 'a
            val mutable _2: 'b
            val mutable _3: 'c

        /// <summary>PipeIdx denotes the index of the element within the pipeline. 0 denotes the
        /// source of the chain.</summary>
        type PipeIdx = int

        type IOutOfBand =
            abstract StopFurtherProcessing : PipeIdx -> unit

        /// <summary>Activity is the root class for chains of activities. It is in a non-generic
        /// form so that it can be used by subsequent activities</summary>
        [<AbstractClass>]
        type Activity =
            /// <summary>OnComplete is used to determine if the object has been processed correctly,
            /// and possibly throw exceptions to denote incorrect application (i.e. such as a Take
            /// operation which didn't have a source at least as large as was required). It is
            /// not called in the case of an exception being thrown whilst the stream is still
            /// being processed.</summary>
            abstract ChainComplete : stopTailCall:byref<unit>*PipeIdx -> unit
            /// <summary>OnDispose is used to cleanup the stream. It is always called at the last operation
            /// after the enumeration has completed.</summary>
            abstract ChainDispose : stopTailCall:byref<unit> -> unit

        /// <summary>Activity is the base class of all elements within the pipeline</summary>
        [<AbstractClass>]
        type Activity<'T,'U> =
            inherit Activity
            new : unit -> Activity<'T,'U>
            abstract member ProcessNext : input:'T -> bool

        [<AbstractClass>]
        type Transform<'T,'U,'State> =
            inherit Activity<'T,'U>
            new : next:Activity * 'State -> Transform<'T,'U,'State>
            val mutable State : 'State
            val private Next : Activity

        [<AbstractClass>]
        type TransformWithPostProcessing<'T,'U,'State> =
            inherit Activity<'T,'U>
            new : next:Activity * 'State -> TransformWithPostProcessing<'T,'U,'State>
            val mutable State : 'State
            val private Next : Activity
            abstract OnComplete : PipeIdx -> unit
            abstract OnDispose  : unit -> unit

        /// <summary>Folder is a base class to assist with fold-like operations. It's intended usage
        /// is as a base class for an object expression that will be used from within
        /// the Fold function.</summary>
        [<AbstractClass>]
        type Folder<'T,'Result,'State> =
            inherit Activity<'T,'T>
            new : 'Result*'State -> Folder<'T,'Result,'State>
            interface IOutOfBand
            val mutable State : 'State
            val mutable Result : 'Result
            val mutable HaltedIdx : int
            member StopFurtherProcessing : PipeIdx -> unit

        [<AbstractClass>]
        type FolderWithPostProcessing<'T,'Result,'State> =
            inherit Folder<'T,'Result,'State>
            new : 'Result*'State -> FolderWithPostProcessing<'T,'Result,'State>

            abstract OnDispose : unit -> unit
            abstract OnComplete : PipeIdx -> unit

        [<AbstractClass>]
        type SeqFactory<'T,'U> =
            new : unit -> SeqFactory<'T,'U>
            abstract member Create : IOutOfBand -> PipeIdx -> Activity<'U,'V> -> Activity<'T,'V>

        type ISeq<'T> =
            inherit System.Collections.Generic.IEnumerable<'T>
            abstract member Compose : SeqFactory<'T,'U> -> ISeq<'U>
            abstract member Fold<'Result,'State> : f:(PipeIdx->Folder<'T,'Result,'State>) -> 'Result

    open Core

    [<CompiledName "OfSeq">]
    val ofSeq : source:seq<'T> ->  ISeq<'T>

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
    val unfold : generator:('State -> ('T * 'State) option) -> state:'State ->  ISeq<'T>

    [<CompiledName "InitializeInfinite">]
    val initInfinite : f:(int -> 'T) ->  ISeq<'T>

    [<CompiledName "Initialize">]
    val init : count:int -> f:(int -> 'T) ->  ISeq<'T>

    [<CompiledName "Iterate">]
    val inline iter : f:('T -> unit) -> source: ISeq<'T> -> unit

    [<CompiledName "Iterate2">]
    val inline iter2 : f:('T->'U->unit) -> source1 : ISeq<'T> -> source2 : ISeq<'U> -> unit

    [<CompiledName "IterateIndexed2">]
    val inline iteri2 : f:(int->'T->'U->unit) -> source1:ISeq<'T> -> source2:ISeq<'U> -> unit

    [<CompiledName "TryHead">]
    val tryHead : ISeq<'T> -> 'T option

    [<CompiledName "IterateIndexed">]
    val inline iteri : f:(int -> 'T -> unit) -> source: ISeq<'T> -> unit

    [<CompiledName "Except">]
    val inline except : itemsToExclude:seq<'T> -> source:ISeq<'T> -> ISeq<'T> when 'T:equality

    [<CompiledName "Exists">]
    val inline exists : f:('T -> bool) -> source: ISeq<'T> -> bool

    [<CompiledName "Exists2">]
    val inline exists2 : predicate:('T->'U->bool) -> source1:ISeq<'T> -> source2:ISeq<'U> -> bool

    [<CompiledName "Contains">]
    val inline contains : element:'T -> source: ISeq<'T> -> bool when 'T : equality

    [<CompiledName "ForAll">]
    val inline forall : f:('T -> bool) -> source: ISeq<'T> -> bool

    [<CompiledName "ForAll2">]
    val inline forall2 : predicate:('T->'U->bool) -> source1:ISeq<'T> -> source2:ISeq<'U> -> bool

    [<CompiledName "Filter">]
    val inline filter : f:('T -> bool) -> source: ISeq<'T> ->  ISeq<'T>

    [<CompiledName "Map">]
    val inline map : f:('T -> 'U) -> source: ISeq<'T> ->  ISeq<'U>

    [<CompiledName "MapIndexed">]
    val inline mapi : f:(int->'a->'b) -> source: ISeq<'a> -> ISeq<'b>

    [<CompiledName "Map2">]
    val inline map2<'First,'Second,'U> : map:('First->'Second->'U) -> source1:ISeq<'First> -> source2:ISeq<'Second> -> ISeq<'U>

    [<CompiledName "MapIndexed2">]
    val inline mapi2<'First,'Second,'U> : map:(int -> 'First->'Second->'U) -> source1:ISeq<'First> -> source2:ISeq<'Second> -> ISeq<'U>

    [<CompiledName "Map3">]
    val inline map3<'First,'Second,'Third,'U> : map:('First->'Second->'Third->'U) -> source1:ISeq<'First> -> source2:ISeq<'Second> -> source3:ISeq<'Third> -> ISeq<'U>

    [<CompiledName "CompareWith">]
    val inline compareWith : f:('T -> 'T -> int) -> source1 :ISeq<'T> -> source2:ISeq<'T> -> int

    [<CompiledName "Choose">]
    val inline choose : f:('a->option<'b>) -> source: ISeq<'a> -> ISeq<'b>

    [<CompiledName "Distinct">]
    val inline distinct : source: ISeq<'T> -> ISeq<'T> when 'T:equality

    [<CompiledName "DistinctBy">]
    val inline distinctBy : keyf:('T->'Key) -> source: ISeq<'T> -> ISeq<'T> when 'Key:equality

    [<CompiledName "Max">]
    val inline max : source: ISeq<'T> -> 'T when 'T:comparison

    [<CompiledName "MaxBy">]
    val inline maxBy : f:('T -> 'U) -> source: ISeq<'T> -> 'T when 'U:comparison

    [<CompiledName "Min">]
    val inline min : source: ISeq<'T> -> 'T when 'T:comparison

    [<CompiledName "MinBy">]
    val inline minBy : f:('T -> 'U) -> source: ISeq<'T> -> 'T when 'U:comparison

    [<CompiledName "Pairwise">]
    val pairwise : source:ISeq<'T> -> ISeq<'T * 'T>

    [<CompiledName "Reduce">]
    val inline reduce : f:('T->'T->'T) -> source: ISeq<'T> -> 'T

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
    val inline sumBy : f :('T -> ^U) -> source: ISeq<'T> -> ^U
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
    val tryItem : index:int -> source: ISeq<'T> -> 'T option

    [<CompiledName "TryPick">]
    val inline tryPick : f:('T -> 'U option) -> source: ISeq<'T> -> Option<'U>

    [<CompiledName "TryFind">]
    val inline tryFind : f:('T -> bool) -> source: ISeq<'T> -> Option<'T>

    [<CompiledName "TryFindIndex">]
    val inline tryFindIndex: predicate:('T->bool) -> source:ISeq<'T> -> int option

    [<CompiledName "TryLast">]
    val inline tryLast : source:ISeq<'T> -> 'T option

    [<CompiledName "Windowed">]
    val windowed : windowSize:int -> source:ISeq<'T> -> ISeq<'T[]>

    [<CompiledName("Concat")>]
    val concat : sources:ISeq<'Collection> -> ISeq<'T> when 'Collection :> ISeq<'T>

    [<CompiledName("Append")>]
    val append: source1:ISeq<'T> -> source2:ISeq<'T> -> ISeq<'T>

    module internal Array = begin
        val createDelayed   : (unit -> 'T array) -> SeqFactory<'T,'U> ->  ISeq<'U>
        val create          : 'T array -> SeqFactory<'T,'U> ->  ISeq<'U>
        val createDelayedId : (unit -> 'T array) ->  ISeq<'T>
        val createId        : 'T array ->  ISeq<'T>
    end
