// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Collections
    open System
    open System.Collections
    open System.Collections.Generic
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Collections

    module internal IEnumerator =

        val noReset : unit -> 'T

        val notStarted : unit -> 'T

        val alreadyFinished : unit -> 'T

        val check : started:bool -> unit

        val dispose : r:System.IDisposable -> unit

        val cast : e:System.Collections.IEnumerator -> System.Collections.Generic.IEnumerator<'T>

        val Empty : unit -> System.Collections.Generic.IEnumerator<'T>

        val Singleton : x:'T -> System.Collections.Generic.IEnumerator<'T>

        val EnumerateThenFinally : f:(unit -> unit) -> e:System.Collections.Generic.IEnumerator<'T> -> System.Collections.Generic.IEnumerator<'T>

        val inline checkNonNull : argName:string -> arg:'T -> unit

        val mkSeq : f:(unit -> System.Collections.Generic.IEnumerator<'U>) -> System.Collections.Generic.IEnumerable<'U>


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
    type internal EnumerableBase<'T> =
        new : unit -> EnumerableBase<'T>
        abstract member Append : IConsumableSeq<'T> -> IConsumableSeq<'T>
        abstract member Length : unit -> int
        abstract member GetRaw : unit -> seq<'T>
        default Append : IConsumableSeq<'T> -> IConsumableSeq<'T>
        default Length : unit -> int
        default GetRaw : unit -> seq<'T>
        interface IConsumableSeq<'T>

    [<AbstractClass>]
    type internal EnumerableWithTransform<'T,'U> =
        inherit EnumerableBase<'U>
        new : ISeqTransform<'T,'U> * PipeIdx -> EnumerableWithTransform<'T,'U>

    [<Class>]
    type internal IdentityTransform<'T> =
        interface ISeqTransform<'T,'T> 
        static member Instance : ISeqTransform<'T,'T> 

    type internal ISkipable =
        // Seq.init(Infinite)? lazily uses Current. The only IConsumableSeq component that can do that is Skip
        // and it can only do it at the start of a sequence
        abstract CanSkip : unit -> bool

    type internal ThinConcatEnumerable<'T, 'Sources, 'Collection when 'Collection :> IConsumableSeq<'T>> =
        inherit EnumerableBase<'T>
        new : 'Sources * ('Sources->IConsumableSeq<'Collection>) -> ThinConcatEnumerable<'T, 'Sources, 'Collection>
        interface IConsumableSeq<'T>

    type internal AppendEnumerable<'T> =
        inherit ThinConcatEnumerable<'T, list<IConsumableSeq<'T>>, IConsumableSeq<'T>>
        new : list<IConsumableSeq<'T>> -> AppendEnumerable<'T>
        override Append : IConsumableSeq<'T> -> IConsumableSeq<'T>

    type internal ResizeArrayEnumerable<'T,'U> = 
        inherit EnumerableWithTransform<'T,'U>
        new : ResizeArray<'T> * ISeqTransform<'T,'U> * PipeIdx -> ResizeArrayEnumerable<'T,'U>
        interface IConsumableSeq<'U>

    type internal ThinResizeArrayEnumerable<'T> =
        inherit ResizeArrayEnumerable<'T,'T>
        new : ResizeArray<'T> -> ThinResizeArrayEnumerable<'T>

    type internal ArrayEnumerable<'T,'U> =
        inherit EnumerableWithTransform<'T,'U>
        new : array<'T> * ISeqTransform<'T,'U> * PipeIdx -> ArrayEnumerable<'T,'U>
        interface IConsumableSeq<'U>

    type internal ThinArrayEnumerable<'T> =
        inherit ArrayEnumerable<'T, 'T>
        new : array<'T> -> ThinArrayEnumerable<'T>
        interface IEnumerable<'T>

    type internal VanillaEnumerable<'T,'U> =
        inherit EnumerableWithTransform<'T,'U>
        new : IEnumerable<'T> * ISeqTransform<'T,'U> * PipeIdx -> VanillaEnumerable<'T,'U>
        interface IConsumableSeq<'U>

    type internal ThinEnumerable<'T> =
        inherit VanillaEnumerable<'T,'T>
        new : IEnumerable<'T> -> ThinEnumerable<'T>
        interface IEnumerable<'T>

    type internal UnfoldEnumerable<'T,'U,'GeneratorState> =
        inherit EnumerableWithTransform<'T,'U>
        new : ('GeneratorState->option<'T*'GeneratorState>)*'GeneratorState*ISeqTransform<'T,'U>*PipeIdx -> UnfoldEnumerable<'T,'U,'GeneratorState>
        interface IConsumableSeq<'U>

    type internal InitEnumerableDecider<'T> =
        inherit EnumerableBase<'T>
        new : Nullable<int>* (int->'T) * PipeIdx -> InitEnumerableDecider<'T>
        interface IConsumableSeq<'T>
        
    type internal SingletonEnumerable<'T> =
        inherit EnumerableBase<'T>
        new : 'T -> SingletonEnumerable<'T>
        interface IConsumableSeq<'T>

    type internal InitEnumerable<'T,'U> =
        inherit EnumerableWithTransform<'T,'U>
        new : Nullable<int> * (int->'T) * ISeqTransform<'T,'U> * PipeIdx -> InitEnumerable<'T,'U>
        interface IConsumableSeq<'U>

    type internal DelayedEnumerable<'T> =
        inherit EnumerableBase<'T>
        new : (unit->IConsumableSeq<'T>) * PipeIdx -> DelayedEnumerable<'T>
        interface IConsumableSeq<'T>

    type internal EmptyEnumerable<'T> =
        inherit EnumerableBase<'T>
        private new : unit -> EmptyEnumerable<'T>
        static member Instance : IConsumableSeq<'T>
        interface IConsumableSeq<'T>

namespace Microsoft.FSharp.Core.CompilerServices

    open System
    open System.Collections
    open System.Collections.Generic
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Collections
        
    [<RequireQualifiedAccess>]
    /// <summary>A group of functions used as part of the compiled representation of F# sequence expressions.</summary>
    module RuntimeHelpers = 

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
        interface IDisposable 
        interface SeqComposition.IConsumableSeq<'T>
