// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Collections
    open System
    open System.Collections
    open System.Collections.Generic
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Collections
    module internal IEnumerator =
        val noReset : unit -> 'a
        val notStarted : unit -> 'a
        val alreadyFinished : unit -> 'a
        val check : started:bool -> unit
        val dispose : r:System.IDisposable -> unit
        val cast :
            e:System.Collections.IEnumerator ->
            System.Collections.Generic.IEnumerator<'T>
        [<SealedAttribute ()>]
        type EmptyEnumerator<'T> =
            class
            interface System.IDisposable
            interface System.Collections.IEnumerator
            interface System.Collections.Generic.IEnumerator<'T>
            new : unit -> EmptyEnumerator<'T>
            end
        val Empty : unit -> System.Collections.Generic.IEnumerator<'T>
        [<NoEqualityAttribute (); NoComparisonAttribute ()>]
        type EmptyEnumerable<'T> =
            | EmptyEnumerable
            with
            interface System.Collections.IEnumerable
            interface System.Collections.Generic.IEnumerable<'T>
            end

        val readAndClear : r:'a option ref -> 'a option
        val generateWhileSome :
            openf:(unit -> 'a) ->
            compute:('a -> 'U option) ->
                closef:('a -> unit) -> System.Collections.Generic.IEnumerator<'U>
        [<SealedAttribute ()>]
        type Singleton<'T> =
            class
            interface System.IDisposable
            interface System.Collections.IEnumerator
            interface System.Collections.Generic.IEnumerator<'T>
            new : v:'T -> Singleton<'T>
            end
        val Singleton : x:'T -> System.Collections.Generic.IEnumerator<'T>
        val EnumerateThenFinally :
            f:(unit -> unit) ->
            e:System.Collections.Generic.IEnumerator<'T> ->
                System.Collections.Generic.IEnumerator<'T>
        val inline checkNonNull : argName:string -> arg:'a -> unit
        val mkSeq :
            f:(unit -> System.Collections.Generic.IEnumerator<'U>) ->
            System.Collections.Generic.IEnumerable<'U>

namespace Microsoft.FSharp.Collections.SeqComposition
    open System
    open System.Collections
    open System.Collections.Generic
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Collections

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
        override ChainComplete : PipeIdx -> unit
        override ChainDispose : unit -> unit

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

namespace Microsoft.FSharp.Core.CompilerServices

    open System
    open System.Collections
    open System.Collections.Generic
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Collections
        
    [<RequireQualifiedAccess>]
    /// <summary>A group of functions used as part of the compiled representation of F# sequence expressions.</summary>
    module RuntimeHelpers = 

        [<Struct; NoComparison; NoEquality>]
        type internal StructBox<'T when 'T : equality> = 
            new : value:'T -> StructBox<'T>
            member Value : 'T
            static member Comparer : IEqualityComparer<StructBox<'T>>

        val internal mkConcatSeq : sources:(seq<#seq<'T>>) -> seq<'T>

        /// <summary>The F# compiler emits calls to this function to 
        /// implement the <c>while</c> operator for F# sequence expressions.</summary>
        ///
        /// <param name="guard">A function that indicates whether iteration should continue.</param>
        /// <param name="source">The input sequence.</param>
        ///
        /// <returns>The result sequence.</returns>
        val EnumerateWhile   : guard:(unit -> bool) -> source:seq<'T> -> seq<'T>

        /// <summary>The F# compiler emits calls to this function to 
        /// implement the <c>try/finally</c> operator for F# sequence expressions.</summary>
        ///
        /// <param name="source">The input sequence.</param>
        /// <param name="compensation">A computation to be included in an enumerator's Dispose method.</param>
        ///
        /// <returns>The result sequence.</returns>
        val EnumerateThenFinally :  source:seq<'T> -> compensation:(unit -> unit) -> seq<'T>
        
        /// <summary>The F# compiler emits calls to this function to implement the compiler-intrinsic
        /// conversions from untyped System.Collections.IEnumerable sequences to typed sequences.</summary>
        ///
        /// <param name="create">An initializer function.</param>
        /// <param name="moveNext">A function to iterate and test if end of sequence is reached.</param>
        /// <param name="current">A function to retrieve the current element.</param>
        ///
        /// <returns>The resulting typed sequence.</returns>
        val EnumerateFromFunctions: create:(unit -> 'T) -> moveNext:('T -> bool) -> current:('T -> 'U) -> seq<'U>

        /// <summary>The F# compiler emits calls to this function to implement the <c>use</c> operator for F# sequence
        /// expressions.</summary>
        ///
        /// <param name="resource">The resource to be used and disposed.</param>
        /// <param name="source">The input sequence.</param>
        ///
        /// <returns>The result sequence.</returns>
        val EnumerateUsing : resource:'T -> source:('T -> 'Collection) -> seq<'U> when 'T :> IDisposable and 'Collection :> seq<'U>

        /// <summary>Creates an anonymous event with the given handlers.</summary>
        ///
        /// <param name="addHandler">A function to handle adding a delegate for the event to trigger.</param>
        /// <param name="removeHandler">A function to handle removing a delegate that the event triggers.</param>
        /// <param name="createHandler">A function to produce the delegate type the event can trigger.</param>
        ///
        /// <returns>The initialized event.</returns>
        val CreateEvent : addHandler : ('Delegate -> unit) -> removeHandler : ('Delegate -> unit) -> createHandler : ((obj -> 'Args -> unit) -> 'Delegate) -> Microsoft.FSharp.Control.IEvent<'Delegate,'Args>

    [<AbstractClass>]
    /// <summary>The F# compiler emits implementations of this type for compiled sequence expressions.</summary>
    type GeneratedSequenceBase<'T> =
        /// <summary>The F# compiler emits implementations of this type for compiled sequence expressions.</summary>
        ///
        /// <returns>A new sequence generator for the expression.</returns>
        new : unit -> GeneratedSequenceBase<'T>
        /// <summary>The F# compiler emits implementations of this type for compiled sequence expressions.</summary>
        ///
        /// <returns>A new enumerator for the sequence.</returns>
        abstract GetFreshEnumerator : unit -> IEnumerator<'T>
        /// <summary>The F# compiler emits implementations of this type for compiled sequence expressions.</summary>
        ///
        /// <param name="result">A reference to the sequence.</param>
        ///
        /// <returns>A 0, 1, and 2 respectively indicate Stop, Yield, and Goto conditions for the sequence generator.</returns>
        abstract GenerateNext : result:byref<IEnumerable<'T>> -> int
        /// <summary>The F# compiler emits implementations of this type for compiled sequence expressions.</summary>
        abstract Close: unit -> unit
        /// <summary>The F# compiler emits implementations of this type for compiled sequence expressions.</summary>
        abstract CheckClose: bool
        /// <summary>The F# compiler emits implementations of this type for compiled sequence expressions.</summary>
        abstract LastGenerated : 'T
        interface IEnumerable<'T> 
        interface IEnumerable
        interface IEnumerator<'T> 
        interface IEnumerator 
