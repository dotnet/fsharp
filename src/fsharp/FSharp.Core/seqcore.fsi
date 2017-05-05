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
  open Microsoft.FSharp.Collections.SeqComposition

  module Core = 
    [<Struct; NoComparison; NoEquality>]
    type NoValue = struct end

    [<AbstractClass>]
    type EnumerableBase<'T> =
        new : unit -> EnumerableBase<'T>
        abstract member Append : ISeq<'T> -> ISeq<'T>
        abstract member Length : unit -> int
        interface ISeq<'T>

    [<AbstractClass>]
    type SeqFactoryBase<'T,'U> =
        inherit EnumerableBase<'U>
        new : ITransformFactory<'T,'U> * PipeIdx -> SeqFactoryBase<'T,'U>

    [<Class>]
    type internal IdentityFactory<'T> =
        interface ITransformFactory<'T,'T> 
        static member Instance : ITransformFactory<'T,'T> 

    type internal ISkipable =
        // Seq.init(Infinite)? lazily uses Current. The only ISeq component that can do that is Skip
        // and it can only do it at the start of a sequence
        abstract CanSkip : unit -> bool

    val internal length : ISeq<'T> -> int

    type internal ThinConcatEnumerable<'T, 'Sources, 'Collection when 'Collection :> ISeq<'T>> =
        inherit EnumerableBase<'T>
        new : 'Sources * ('Sources->ISeq<'Collection>) -> ThinConcatEnumerable<'T, 'Sources, 'Collection>
        interface ISeq<'T>

    type internal AppendEnumerable<'T> =
        inherit ThinConcatEnumerable<'T, list<ISeq<'T>>, ISeq<'T>>
        new : list<ISeq<'T>> -> AppendEnumerable<'T>
        override Append : ISeq<'T> -> ISeq<'T>

    type internal ThinResizeArrayEnumerable<'T> =
        inherit EnumerableBase<'T>
        new : ResizeArray<'T> -> ThinResizeArrayEnumerable<'T>
        interface ISeq<'T>

    type internal ThinArrayEnumerable<'T> =
        inherit EnumerableBase<'T>
        new : array<'T> -> ThinArrayEnumerable<'T>
        interface ISeq<'T>

    type internal ThinEnumerable<'T> =
        inherit EnumerableBase<'T>
        new : IEnumerable<'T> -> ThinEnumerable<'T>
        interface ISeq<'T>

    type internal UnfoldEnumerable<'T,'U,'GeneratorState> =
        inherit SeqFactoryBase<'T,'U>
        new : ('GeneratorState->option<'T*'GeneratorState>)*'GeneratorState*ITransformFactory<'T,'U>*PipeIdx -> UnfoldEnumerable<'T,'U,'GeneratorState>
        interface ISeq<'U>

    type internal InitEnumerableDecider<'T> =
        inherit EnumerableBase<'T>
        new : Nullable<int>* (int->'T) * PipeIdx -> InitEnumerableDecider<'T>
        interface ISeq<'T>
        
    type internal SingletonEnumerable<'T> =
        inherit EnumerableBase<'T>
        new : 'T -> SingletonEnumerable<'T>
        interface ISeq<'T>

    type internal InitEnumerable<'T,'U> =
        inherit SeqFactoryBase<'T,'U>
        new : Nullable<int> * (int->'T) * ITransformFactory<'T,'U> * PipeIdx -> InitEnumerable<'T,'U>
        interface ISeq<'U>

    type internal DelayedEnumerable<'T> =
        inherit EnumerableBase<'T>
        new : (unit->ISeq<'T>) * PipeIdx -> DelayedEnumerable<'T>
        interface ISeq<'T>

    type internal EmptyEnumerable<'T> =
        inherit EnumerableBase<'T>
        private new : unit -> EmptyEnumerable<'T>
        static member Instance : ISeq<'T>
        interface ISeq<'T>

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
        inherit SeqComposition.Core.EnumerableBase<'T>
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
        interface SeqComposition.ISeq<'T>
