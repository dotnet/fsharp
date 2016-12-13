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

        /// <summary>ICompletionChain is used to correctly handle cleaning up of the pipeline. A
        /// base implementation is provided in Consumer, and should not be overwritten. Consumer
        /// provides it's own OnComplete and OnDispose function which should be used to handle
        /// a particular consumers cleanup.</summary>
        type ICompletionChain =
            /// <summary>OnComplete is used to determine if the object has been processed correctly,
            /// and possibly throw exceptions to denote incorrect application (i.e. such as a Take
            /// operation which didn't have a source at least as large as was required). It is
            /// not called in the case of an exception being thrown whilst the stream is still
            /// being processed.</summary>
            abstract ChainComplete : stopTailCall:byref<unit>*PipeIdx -> unit
            /// <summary>OnDispose is used to cleanup the stream. It is always called at the last operation
            /// after the enumeration has completed.</summary>
            abstract ChainDispose : stopTailCall:byref<unit> -> unit

        /// <summary>Consumer is the base class of all elements within the pipeline</summary>
        [<AbstractClass>]
        type Consumer<'T,'U> =
            interface ICompletionChain
            new : unit -> Consumer<'T,'U>
            abstract member ProcessNext : input:'T -> bool

        [<AbstractClass>]
        type ConsumerWithState<'T,'U,'Value> =
            inherit Consumer<'T,'U>
            val mutable Value : 'Value
            new : init:'Value -> ConsumerWithState<'T,'U,'Value>

        [<AbstractClass>]
        type ConsumerChainedWithState<'T,'U,'Value> =
            inherit ConsumerWithState<'T,'U,'Value>
            interface ICompletionChain
            val private Next : ICompletionChain
            new : next:ICompletionChain * init:'Value -> ConsumerChainedWithState<'T,'U,'Value>

        [<AbstractClass>]
        type ConsumerChained<'T,'U> =
            inherit ConsumerChainedWithState<'T,'U,NoValue>
            new : next:ICompletionChain -> ConsumerChained<'T,'U>

        [<AbstractClass>]
        type ConsumerChainedWithStateAndCleanup<'T,'U,'Value> =
            inherit ConsumerChainedWithState<'T,'U,'Value>
            interface ICompletionChain

            abstract OnComplete : PipeIdx -> unit
            abstract OnDispose  : unit -> unit

            new : next:ICompletionChain * init:'Value -> ConsumerChainedWithStateAndCleanup<'T,'U,'Value>

        [<AbstractClass>]
        type ConsumerChainedWithCleanup<'T,'U> =
            inherit ConsumerChainedWithStateAndCleanup<'T,'U,NoValue>
            new : next:ICompletionChain -> ConsumerChainedWithCleanup<'T,'U>

        /// <summary>Folder is a base class to assist with fold-like operations. It's intended usage
        /// is as a base class for an object expression that will be used from within
        /// the ForEach function.</summary>
        [<AbstractClass>]
        type Folder<'T,'Value> =
            inherit ConsumerWithState<'T,'T,'Value>
            new : init:'Value -> Folder<'T,'Value>

        [<AbstractClass>]
        type FolderWithOnComplete<'T, 'Value> =
            inherit Folder<'T,'Value>
            interface ICompletionChain

            abstract OnComplete : PipeIdx -> unit

            new : init:'Value -> FolderWithOnComplete<'T,'Value>

        [<AbstractClass>]
        type SeqFactory<'T,'U> =
            new : unit -> SeqFactory<'T,'U>
            abstract PipeIdx : PipeIdx
            default  PipeIdx : PipeIdx
            abstract member Create : IOutOfBand -> PipeIdx -> Consumer<'U,'V> -> Consumer<'T,'V>

        type ISeq<'T> =
            inherit System.Collections.Generic.IEnumerable<'T>
            abstract member Compose : SeqFactory<'T,'U> -> ISeq<'U>
            abstract member ForEach : f:((unit -> unit) -> 'a) -> 'a when 'a :> Consumer<'T,'T>

    open Core

    module internal Seq =
        type ComposedFactory<'T,'U,'V> =
          class
            inherit  SeqFactory<'T,'V>
            private new : first: SeqFactory<'T,'U> *
                          second: SeqFactory<'U,'V> *
                          secondPipeIdx: PipeIdx ->
                             ComposedFactory<'T,'U,'V>
            static member
              Combine : first: SeqFactory<'T,'U> ->
                          second: SeqFactory<'U,'V> ->
                             SeqFactory<'T,'V>
          end
        and IdentityFactory<'T> =
          class
            inherit  SeqFactory<'T,'T>
            new : unit ->  IdentityFactory<'T>
            static member Instance :  SeqFactory<'T,'T>
          end
        and Map2FirstFactory<'First,'Second,'U> =
          class
            inherit  SeqFactory<'First,'U>
            new : map:('First -> 'Second -> 'U) *
                  input2:IEnumerable<'Second> ->
                     Map2FirstFactory<'First,'Second,'U>
          end
        and Map2SecondFactory<'First,'Second,'U> =
          class
            inherit  SeqFactory<'Second,'U>
            new : map:('First -> 'Second -> 'U) *
                  input1:IEnumerable<'First> ->
                     Map2SecondFactory<'First,'Second,'U>
          end
        and Map3Factory<'First,'Second,'Third,'U> =
          class
            inherit  SeqFactory<'First,'U>
            new : map:('First -> 'Second -> 'Third -> 'U) *
                  input2:IEnumerable<'Second> *
                  input3:IEnumerable<'Third> ->
                     Map3Factory<'First,'Second,'Third,'U>
          end
        and Mapi2Factory<'First,'Second,'U> =
          class
            inherit  SeqFactory<'First,'U>
            new : map:(int -> 'First -> 'Second -> 'U) *
                  input2:IEnumerable<'Second> ->
                     Mapi2Factory<'First,'Second,'U>
          end
        and ISkipping =
          interface
            abstract member Skipping : unit -> bool
          end

        and Map2First<'First,'Second,'U,'V> =
          class
            inherit  ConsumerChainedWithCleanup<'First,'V>
            new : map:('First -> 'Second -> 'U) *
                  enumerable2:IEnumerable<'Second> *
                  outOfBand: IOutOfBand *
                  next: Consumer<'U,'V> * pipeIdx:int ->
                     Map2First<'First,'Second,'U,'V>
            override OnDispose : unit -> unit
            override ProcessNext : input:'First -> bool
          end
        and Map2Second<'First,'Second,'U,'V> =
          class
            inherit  ConsumerChainedWithCleanup<'Second,'V>
            new : map:('First -> 'Second -> 'U) *
                  enumerable1:IEnumerable<'First> *
                  outOfBand: IOutOfBand *
                  next: Consumer<'U,'V> * pipeIdx:int ->
                     Map2Second<'First,'Second,'U,'V>
            override OnDispose : unit -> unit
            override ProcessNext : input:'Second -> bool
          end
        and Map3<'First,'Second,'Third,'U,'V> =
          class
            inherit  ConsumerChainedWithCleanup<'First,'V>
            new : map:('First -> 'Second -> 'Third -> 'U) *
                  enumerable2:IEnumerable<'Second> *
                  enumerable3:IEnumerable<'Third> *
                  outOfBand: IOutOfBand *
                  next: Consumer<'U,'V> * pipeIdx:int ->
                     Map3<'First,'Second,'Third,'U,'V>
            override OnDispose : unit -> unit
            override ProcessNext : input:'First -> bool
          end
        and Mapi2<'First,'Second,'U,'V> =
          class
            inherit  ConsumerChainedWithCleanup<'First,'V>
            new : map:(int -> 'First -> 'Second -> 'U) *
                  enumerable2:IEnumerable<'Second> *
                  outOfBand: IOutOfBand *
                  next: Consumer<'U,'V> * pipeIdx:int ->
                     Mapi2<'First,'Second,'U,'V>
            override OnDispose : unit -> unit
            override ProcessNext : input:'First -> bool
          end
        type SeqProcessNextStates =
          |  InProcess  =  0
          |  NotStarted  =  1
          |  Finished  =  2
        type Result<'T> =
          class
            interface  IOutOfBand
            new : unit ->  Result<'T>
            member Current : 'T
            member HaltedIdx : int
            member SeqState :  SeqProcessNextStates
            member Current : 'T with set
            member SeqState :  SeqProcessNextStates with set
          end
        type SetResult<'T> =
          class
            inherit  Consumer<'T,'T>
            new : result: Result<'T> ->  SetResult<'T>
            override ProcessNext : input:'T -> bool
          end
        type OutOfBand =
          class
            interface  IOutOfBand
            new : unit ->  OutOfBand
            member HaltedIdx : int
          end
        module ForEach = begin
          val enumerable :
            enumerable:IEnumerable<'T> ->
              outOfBand: OutOfBand ->
                consumer: Consumer<'T,'U> -> unit
          val array :
            array:'T array ->
              outOfBand: OutOfBand ->
                consumer: Consumer<'T,'U> -> unit
          val list :
            alist:'T list ->
              outOfBand: OutOfBand ->
                consumer: Consumer<'T,'U> -> unit
          val unfold :
            generator:('S -> ('T * 'S) option) ->
              state:'S ->
                outOfBand: OutOfBand ->
                  consumer: Consumer<'T,'U> -> unit
          val makeIsSkipping :
            consumer: Consumer<'T,'U> -> (unit -> bool)
          val init :
            f:(int -> 'T) ->
              terminatingIdx:int ->
                outOfBand: OutOfBand ->
                  consumer: Consumer<'T,'U> -> unit
          val execute :
            f:((unit -> unit) -> 'a) ->
              current: SeqFactory<'T,'U> ->
                executeOn:( OutOfBand ->  Consumer<'T,'U> ->
                             unit) -> 'a when 'a :>  Consumer<'U,'U>
        end
        module Enumerable = begin
          type Empty<'T> =
            class
              interface IDisposable
              interface IEnumerator
              interface IEnumerator<'T>
              new : unit ->  Empty<'T>
            end
          type EmptyEnumerators<'T> =
            class
              new : unit ->  EmptyEnumerators<'T>
              static member Element : IEnumerator<'T>
            end
          [<AbstractClass>]
          type EnumeratorBase<'T> =
            class
              interface IEnumerator<'T>
              interface IEnumerator
              interface IDisposable
              new : result: Result<'T> *
                    seqComponent: ICompletionChain ->
                       EnumeratorBase<'T>
            end
          and [<AbstractClass>] EnumerableBase<'T> =
            class
              interface  ISeq<'T>
              interface IEnumerable<'T>
              interface IEnumerable
              new : unit ->  EnumerableBase<'T>
              abstract member
                Append : seq<'T> -> IEnumerable<'T>
              override
                Append : source:seq<'T> -> IEnumerable<'T>
            end
          and Enumerator<'T,'U> =
            class
              inherit  EnumeratorBase<'U>
              interface IDisposable
              interface IEnumerator
              new : source:IEnumerator<'T> *
                    seqComponent: Consumer<'T,'U> *
                    result: Result<'U> ->
                       Enumerator<'T,'U>
            end
          and Enumerable<'T,'U> =
            class
              inherit  EnumerableBase<'U>
              interface  ISeq<'U>
              interface IEnumerable<'U>
              new : enumerable:IEnumerable<'T> *
                    current: SeqFactory<'T,'U> ->
                       Enumerable<'T,'U>
            end
          and ConcatEnumerator<'T,'Collection when 'Collection :> seq<'T>> =
            class
              interface IDisposable
              interface IEnumerator
              interface IEnumerator<'T>
              new : sources:seq<'Collection> ->
                       ConcatEnumerator<'T,'Collection>
            end
          and AppendEnumerable<'T> =
            class
              inherit  EnumerableBase<'T>
              interface  ISeq<'T>
              interface IEnumerable<'T>
              new : sources:seq<'T> list ->  AppendEnumerable<'T>
              override
                Append : source:seq<'T> ->
                           IEnumerable<'T>
            end
          and ConcatEnumerable<'T,'Collection when 'Collection :> seq<'T>> =
            class
              inherit  EnumerableBase<'T>
              interface  ISeq<'T>
              interface IEnumerable<'T>
              new : sources:seq<'Collection> ->
                       ConcatEnumerable<'T,'Collection>
            end
          val create :
            enumerable:IEnumerable<'a> ->
              current: SeqFactory<'a,'b> ->  ISeq<'b>
        end
        module EmptyEnumerable = begin
          type Enumerable<'T> =
            class
              inherit  Enumerable.EnumerableBase<'T>
              interface  ISeq<'T>
              interface IEnumerable<'T>
              new : unit ->  Enumerable<'T>
              override
                Append : source:seq<'T> -> IEnumerable<'T>
              static member Instance :  ISeq<'T>
            end
        end
        module Array = begin
          type Enumerator<'T,'U> =
            class
              inherit  Enumerable.EnumeratorBase<'U>
              interface IEnumerator
              new : delayedArray:(unit -> 'T array) *
                    seqComponent: Consumer<'T,'U> *
                    result: Result<'U> ->  Enumerator<'T,'U>
            end
          type Enumerable<'T,'U> =
            class
              inherit  Enumerable.EnumerableBase<'U>
              interface  ISeq<'U>
              interface IEnumerable<'U>
              new : delayedArray:(unit -> 'T array) *
                    current: SeqFactory<'T,'U> ->
                       Enumerable<'T,'U>
            end
          val createDelayed :
            delayedArray:(unit -> 'T array) ->
              current: SeqFactory<'T,'U> ->  ISeq<'U>
          val create :
            array:'T array ->
              current: SeqFactory<'T,'U> ->  ISeq<'U>
          val createDelayedId :
            delayedArray:(unit -> 'T array) ->  ISeq<'T>
          val createId : array:'T array ->  ISeq<'T>
        end
        module List = begin
          type Enumerator<'T,'U> =
            class
              inherit  Enumerable.EnumeratorBase<'U>
              interface IEnumerator
              new : alist:'T list * seqComponent: Consumer<'T,'U> *
                    result: Result<'U> ->  Enumerator<'T,'U>
            end
          type Enumerable<'T,'U> =
            class
              inherit  Enumerable.EnumerableBase<'U>
              interface  ISeq<'U>
              interface IEnumerable<'U>
              new : alist:'T list * current: SeqFactory<'T,'U> ->
                       Enumerable<'T,'U>
            end
          val create :
            alist:'a list ->
              current: SeqFactory<'a,'b> ->  ISeq<'b>
        end
        module Unfold = begin
          type Enumerator<'T,'U,'State> =
            class
              inherit  Enumerable.EnumeratorBase<'U>
              interface IEnumerator
              new : generator:('State -> ('T * 'State) option) * state:'State *
                    seqComponent: Consumer<'T,'U> *
                    result: Result<'U> ->
                       Enumerator<'T,'U,'State>
            end
          type Enumerable<'T,'U,'GeneratorState> =
            class
              inherit  Enumerable.EnumerableBase<'U>
              interface  ISeq<'U>
              interface IEnumerable<'U>
              new : generator:('GeneratorState -> ('T * 'GeneratorState) option) *
                    state:'GeneratorState * current: SeqFactory<'T,'U> ->
                       Enumerable<'T,'U,'GeneratorState>
            end
        end
        module Init = begin
          val getTerminatingIdx : count:Nullable<int> -> int
          type Enumerator<'T,'U> =
            class
              inherit  Enumerable.EnumeratorBase<'U>
              interface IEnumerator
              new : count:Nullable<int> * f:(int -> 'T) *
                    seqComponent: Consumer<'T,'U> *
                    result: Result<'U> ->  Enumerator<'T,'U>
            end
          type Enumerable<'T,'U> =
            class
              inherit  Enumerable.EnumerableBase<'U>
              interface  ISeq<'U>
              interface IEnumerable<'U>
              new : count:Nullable<int> * f:(int -> 'T) *
                    current: SeqFactory<'T,'U> ->
                       Enumerable<'T,'U>
            end
          val upto :
            lastOption:int option ->
              f:(int -> 'U) -> IEnumerator<'U>
          type EnumerableDecider<'T> =
            class
              inherit  Enumerable.EnumerableBase<'T>
              interface  ISeq<'T>
              interface IEnumerable<'T>
              new : count:Nullable<int> * f:(int -> 'T) ->
                       EnumerableDecider<'T>
            end
        end

        [<CompiledName "ToComposer">]
        val toComposer : source:seq<'T> ->  ISeq<'T>

        val inline foreach : f:((unit -> unit) -> 'a) -> source: ISeq<'b> -> 'a when 'a :>  Consumer<'b,'b>

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
        val empty<'T> :  ISeq<'T>

        [<CompiledName "Fold">]
        val inline fold<'T,'State> : f:('State->'T->'State) -> seed:'State -> source:ISeq<'T> -> 'State

        [<CompiledName "Unfold">]
        val unfold : generator:('State -> ('T * 'State) option) -> state:'State ->  ISeq<'T>

        [<CompiledName "InitializeInfinite">]
        val initInfinite : f:(int -> 'T) ->  ISeq<'T>

        [<CompiledName "Initialize">]
        val init : count:int -> f:(int -> 'T) ->  ISeq<'T>

        [<CompiledName "Iterate">]
        val iter : f:('T -> unit) -> source: ISeq<'T> -> unit

        [<CompiledName "TryHead">]
        val tryHead : source: ISeq<'T> -> 'T option

        [<CompiledName "IterateIndexed">]
        val iteri : f:(int -> 'T -> unit) -> source: ISeq<'T> -> unit

        [<CompiledName "Except">]
        val inline except : itemsToExclude:seq<'T> -> source:ISeq<'T> -> ISeq<'T> when 'T:equality

        [<CompiledName "Exists">]
        val exists : f:('T -> bool) -> source: ISeq<'T> -> bool

        [<CompiledName "Contains">]
        val inline contains : element:'T -> source: ISeq<'T> -> bool when 'T : equality

        [<CompiledName "ForAll">]
        val forall : f:('T -> bool) -> source: ISeq<'T> -> bool

        [<CompiledName "Filter">]
        val inline filter : f:('T -> bool) -> source: ISeq<'T> ->  ISeq<'T>

        [<CompiledName "Map">]
        val inline map : f:('T -> 'U) -> source: ISeq<'T> ->  ISeq<'U>

        [<CompiledName "MapIndexed">]
        val inline mapi : f:(int->'a->'b) -> source: ISeq<'a> -> ISeq<'b>

        val mapi_adapt : f:OptimizedClosures.FSharpFunc<int,'a,'b> -> source: ISeq<'a> -> ISeq<'b>

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
        val inline pairwise : source:ISeq<'T> -> ISeq<'T * 'T>

        [<CompiledName "Scan">]
        val inline scan : folder:('State->'T->'State) -> initialState:'State -> source:ISeq<'T> -> ISeq<'State>

        [<CompiledName "Skip">]
        val inline skip : skipCount:int -> source:ISeq<'T> -> ISeq<'T>

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
        val inline take : takeCount:int -> source:ISeq<'T> -> ISeq<'T>

        [<CompiledName "TakeWhile">]
        val inline takeWhile : predicate:('T->bool) -> source:ISeq<'T> -> ISeq<'T>

        [<CompiledName "Tail">]
        val inline tail : source:ISeq<'T> -> ISeq<'T>

        [<CompiledName "Truncate">]
        val inline truncate : truncateCount:int -> source:ISeq<'T> -> ISeq<'T>

        [<CompiledName "Indexed">]
        val inline indexed : source: ISeq<'a> -> ISeq<int * 'a>

        [<CompiledName "TryItem">]
        val tryItem : index:int -> source: ISeq<'T> -> 'T option

        [<CompiledName "TryPick">]
        val tryPick : f:('T -> 'U option) -> source: ISeq<'T> -> Option<'U>

        [<CompiledName "TryFind">]
        val tryFind : f:('T -> bool) -> source: ISeq<'T> -> Option<'T>

        [<CompiledName "Windowed">]
        val inline windowed : windowSize:int -> source:ISeq<'T> -> ISeq<'T[]>
