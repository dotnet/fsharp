// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Collections

    open System
    open System.Diagnostics
    open System.Collections
    open System.Collections.Generic
    open System.Reflection
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
    open Microsoft.FSharp.Core.Operators
    open Microsoft.FSharp.Core.CompilerServices
    open Microsoft.FSharp.Control
    open Microsoft.FSharp.Collections
    open Microsoft.FSharp.Primitives.Basics

    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module ISeq =
        open IEnumerator

        module Core =
            [<Struct; NoComparison; NoEquality>]
            type NoValue = struct end

            [<Struct; NoComparison; NoEquality>]
            type Value<'a> =
                val mutable _1 : 'a
                new (a:'a) = { _1 = a }

            [<Struct; NoComparison; NoEquality>]
            type Values<'a,'b> =
                val mutable _1 : 'a
                val mutable _2 : 'b
                new (a:'a, b: 'b) = { _1 = a;  _2 = b }

            [<Struct; NoComparison; NoEquality>]
            type Values<'a,'b,'c> =
                val mutable _1 : 'a
                val mutable _2 : 'b
                val mutable _3 : 'c
                new (a:'a, b:'b, c:'c) = { _1 = a; _2 = b; _3 = c }

            type PipeIdx = int

            type IOutOfBand =
                abstract StopFurtherProcessing : PipeIdx -> unit

            [<AbstractClass>]
            type Activity() =
                abstract ChainComplete : stopTailCall:byref<unit> * PipeIdx -> unit
                abstract ChainDispose  : stopTailCall:byref<unit> -> unit

            [<AbstractClass>]
            type Activity<'T,'U> () =
                inherit Activity()
                abstract ProcessNext : input:'T -> bool

            [<AbstractClass>]
            type Transform<'T,'U,'State> =
                inherit Activity<'T,'U>
                
                new (next:Activity, initState:'State) = {
                    inherit Activity<'T,'U> ()
                    State = initState
                    Next = next
                }

                val mutable State : 'State
                val Next : Activity
                
                override this.ChainComplete (stopTailCall, terminatingIdx) =
                    this.Next.ChainComplete (&stopTailCall, terminatingIdx)
                override this.ChainDispose stopTailCall =
                    this.Next.ChainDispose (&stopTailCall)

            [<AbstractClass>]
            type TransformWithPostProcessing<'T,'U,'State>(next:Activity, initState:'State) =
                inherit Transform<'T,'U,'State>(next, initState)

                abstract OnComplete : PipeIdx -> unit
                abstract OnDispose  : unit -> unit

                override this.ChainComplete (stopTailCall, terminatingIdx) =
                    this.OnComplete terminatingIdx
                    this.Next.ChainComplete (&stopTailCall, terminatingIdx)
                override this.ChainDispose stopTailCall  =
                    try     this.OnDispose ()
                    finally this.Next.ChainDispose (&stopTailCall)

            [<AbstractClass>]
            type Folder<'T,'Result,'State> =
                inherit Activity<'T,'T>

                val mutable Result : 'Result
                val mutable State : 'State

                val mutable HaltedIdx : int
                member this.StopFurtherProcessing pipeIdx = this.HaltedIdx <- pipeIdx
                interface IOutOfBand with
                    member this.StopFurtherProcessing pipeIdx = this.StopFurtherProcessing pipeIdx

                new (initalResult,initState) = {
                    inherit Activity<'T,'T>()
                    State = initState
                    HaltedIdx = 0
                    Result = initalResult
                }

                override this.ChainComplete (_,_) = ()
                override this.ChainDispose _ = ()

            [<AbstractClass>]
            type FolderWithPostProcessing<'T,'Result,'State>(initResult,initState) =
                inherit Folder<'T,'Result,'State>(initResult,initState)

                abstract OnComplete : PipeIdx -> unit
                abstract OnDispose : unit -> unit

                override this.ChainComplete (stopTailCall, terminatingIdx) =
                    this.OnComplete terminatingIdx
                override this.ChainDispose _ =
                    this.OnDispose ()

            [<AbstractClass>]
            type TransformFactory<'T,'U> () =
                abstract Compose<'V> : IOutOfBand -> PipeIdx -> Activity<'U,'V> -> Activity<'T,'V>

            type ISeq<'T> =
                inherit IEnumerable<'T>
                abstract member PushTransform<'U> : TransformFactory<'T,'U> -> ISeq<'U>
                abstract member Fold<'Result,'State> : f:(PipeIdx->Folder<'T,'Result,'State>) -> 'Result

        open Core

        module internal TailCall =
            // used for performance reasons; these are not recursive calls, so should be safe
            // ** it should be noted that potential changes to the f# compiler may render this function
            // ineffictive **
            let inline avoid boolean = match boolean with true -> true | false -> false

        module internal Upcast =
            // The f# compiler outputs unnecessary unbox.any calls in upcasts. If this functionality
            // is fixed with the compiler then these functions can be removed.
            let inline seq<'T,'seq when 'seq :> ISeq<'T> and 'seq : not struct> (t:'seq) : ISeq<'T> = (# "" t : ISeq<'T> #)
            let inline enumerable<'T,'enumerable when 'enumerable :> IEnumerable<'T> and 'enumerable : not struct> (t:'enumerable) : IEnumerable<'T> = (# "" t : IEnumerable<'T> #)
            let inline enumerableNonGeneric<'enumerable when 'enumerable :> IEnumerable and 'enumerable : not struct> (t:'enumerable) : IEnumerable = (# "" t : IEnumerable #)
            let inline enumerator<'T,'enumerator when 'enumerator :> IEnumerator<'T> and 'enumerator : not struct> (t:'enumerator) : IEnumerator<'T> = (# "" t : IEnumerator<'T> #)
            let inline enumeratorNonGeneric<'enumerator when 'enumerator :> IEnumerator and 'enumerator : not struct> (t:'enumerator) : IEnumerator = (# "" t : IEnumerator #)
            let inline outOfBand<'outOfBand when 'outOfBand :> IOutOfBand and 'outOfBand : not struct> (t:'outOfBand) : IOutOfBand = (# "" t : IOutOfBand #)

        let createFold (factory:TransformFactory<_,_>) (folder:Folder<_,_,_>) pipeIdx  =
            factory.Compose (Upcast.outOfBand folder) pipeIdx folder

        let inline valueComparer<'T when 'T : equality> ()=
            let c = HashIdentity.Structural<'T>
            { new IEqualityComparer<Value<'T>> with
                    member __.GetHashCode o    = c.GetHashCode o._1
                    member __.Equals (lhs,rhs) = c.Equals (lhs._1, rhs._1) }

        type ComposedFactory<'T,'U,'V> private (first:TransformFactory<'T,'U>, second:TransformFactory<'U,'V>) =
            inherit TransformFactory<'T,'V>()

            override this.Compose<'W> (outOfBand:IOutOfBand) (pipeIdx:PipeIdx) (next:Activity<'V,'W>) : Activity<'T,'W> =
                first.Compose outOfBand (pipeIdx-1) (second.Compose outOfBand pipeIdx next)

            static member Combine (first:TransformFactory<'T,'U>) (second:TransformFactory<'U,'V>) : TransformFactory<'T,'V> =
                upcast ComposedFactory(first, second)

        and IdentityFactory<'T> private () =
            inherit TransformFactory<'T,'T> ()
            static let singleton : TransformFactory<'T,'T> = upcast (IdentityFactory<'T>())
            override __.Compose<'V> (_outOfBand:IOutOfBand) (_pipeIdx:PipeIdx) (next:Activity<'T,'V>) : Activity<'T,'V> = next
            static member Instance = singleton

        and ISkipable =
            // Seq.init(Infinite)? lazily uses Current. The only ISeq component that can do that is Skip
            // and it can only do it at the start of a sequence
            abstract CanSkip : unit -> bool

        type SeqProcessNextStates =
        | InProcess  = 0
        | NotStarted = 1
        | Finished   = 2

        type Result<'T>() =
            inherit Folder<'T,'T,NoValue>(Unchecked.defaultof<'T>,Unchecked.defaultof<NoValue>)

            member val SeqState = SeqProcessNextStates.NotStarted with get, set

            override this.ProcessNext (input:'T) : bool =
                this.Result <- input
                true

        module Fold =
            // The consumers of IIterate are the execute and exeuteThin methods. IIterate is passed
            // as a generic argument. The types that implement IIterate are value types. This combination
            // means that the runtime will "inline" the methods. The alternatives to this were that the
            // code in execute/executeThin were duplicated for each of the Fold types, or we turned the
            // types back into normal functions and curried them then we would be creating garbage
            // each time one of these were called. This has been an optimization to minimize the impact
            // on very small collections.
            type IIterate<'T> =
                abstract Iterate<'U,'Result,'State> : outOfBand:Folder<'U,'Result,'State> -> consumer:Activity<'T,'U> -> unit

            [<Struct;NoComparison;NoEquality>]
            type IterateEnumerable<'T> (enumerable:IEnumerable<'T>) =
                interface IIterate<'T> with
                    member __.Iterate (outOfBand:Folder<'U,'Result,'State>) (consumer:Activity<'T,'U>) =
                        use enumerator = enumerable.GetEnumerator ()
                        let rec iterate () =
                            if outOfBand.HaltedIdx = 0 && enumerator.MoveNext () then  
                                consumer.ProcessNext enumerator.Current |> ignore
                                iterate ()
                        iterate ()

            [<Struct;NoComparison;NoEquality>]
            type IterateArray<'T> (array:array<'T>) =
                interface IIterate<'T> with
                    member __.Iterate (outOfBand:Folder<'U,'Result,'State>) (consumer:Activity<'T,'U>) =
                        let array = array
                        let rec iterate idx =
                            if outOfBand.HaltedIdx = 0 && idx < array.Length then  
                                consumer.ProcessNext array.[idx] |> ignore
                                iterate (idx+1)
                        iterate 0

            [<Struct;NoComparison;NoEquality>]
            type IterateResizeArray<'T> (array:ResizeArray<'T>) =
                interface IIterate<'T> with
                    member __.Iterate (outOfBand:Folder<'U,'Result,'State>) (consumer:Activity<'T,'U>) =
                        let array = array
                        let rec iterate idx =
                            if outOfBand.HaltedIdx = 0 && idx < array.Count then  
                                consumer.ProcessNext array.[idx] |> ignore
                                iterate (idx+1)
                        iterate 0

            [<Struct;NoComparison;NoEquality>]
            type IterateList<'T> (alist:list<'T>) =
                interface IIterate<'T> with
                    member __.Iterate (outOfBand:Folder<'U,'Result,'State>) (consumer:Activity<'T,'U>) =
                        let rec iterate lst =
                            match lst with
                            | hd :: tl when outOfBand.HaltedIdx = 0 ->
                                consumer.ProcessNext hd |> ignore
                                iterate tl
                            | _ -> ()
                        iterate alist

            [<Struct;NoComparison;NoEquality>]
            type IterateUnfold<'S,'T> (generator:'S->option<'T*'S>, state:'S) =
                interface IIterate<'T> with
                    member __.Iterate (outOfBand:Folder<'U,'Result,'State>) (consumer:Activity<'T,'U>) =
                        let generator = generator
                        let rec iterate current =
                            if outOfBand.HaltedIdx <> 0 then ()
                            else
                                match generator current with
                                | Some (item, next) ->
                                    consumer.ProcessNext item |> ignore
                                    iterate next
                                | _ -> ()
                        iterate state

            [<Struct;NoComparison;NoEquality>]
            type IterateInit<'T> (f:int->'T, terminatingIdx:int) =
                interface IIterate<'T> with
                    member __.Iterate (outOfBand:Folder<'U,'Result,'State>) (consumer:Activity<'T,'U>) =
                        let terminatingIdx = terminatingIdx
                        let f = f

                        let firstIdx = 
                            match box consumer with
                            | :? ISkipable as skipping ->
                                let rec skip idx =
                                    if idx = terminatingIdx || outOfBand.HaltedIdx <> 0 then
                                        terminatingIdx
                                    elif skipping.CanSkip () then
                                        skip (idx+1)
                                    else
                                        idx
                                skip -1
                            | _ -> -1

                        let rec iterate idx =
                            if idx < terminatingIdx then
                                consumer.ProcessNext (f (idx+1)) |> ignore
                                if outOfBand.HaltedIdx = 0 then
                                    iterate (idx+1)
                                else
                                    idx
                            else
                                idx

                        let finalIdx = iterate firstIdx
                        if outOfBand.HaltedIdx = 0 && finalIdx = System.Int32.MaxValue then
                            raise <| System.InvalidOperationException (SR.GetString(SR.enumerationPastIntMaxValue))

            // execute, and it's companion, executeThin, are hosting functions that ensure the correct sequence
            // of creation, iteration and disposal for the pipeline
            let execute (createFolder:PipeIdx->Folder<'U,'Result,'State>) (transformFactory:TransformFactory<'T,'U>) pipeIdx (executeOn:#IIterate<'T>) =
                let mutable stopTailCall = ()
                let result = createFolder (pipeIdx+1)
                let consumer = createFold transformFactory result pipeIdx
                try
                    executeOn.Iterate result consumer
                    consumer.ChainComplete (&stopTailCall, result.HaltedIdx)
                    result.Result
                finally
                    consumer.ChainDispose (&stopTailCall)

            // executeThin is a specialization of execute, provided as a performance optimization, that can
            // be used when a sequence has been wrapped in an ISeq, but hasn't had an items added to its pipeline
            // i.e. a container that has ISeq.ofSeq applied. 
            let executeThin (createFolder:PipeIdx->Folder<'T,'Result,'State>) (executeOn:#IIterate<'T>) =
                let mutable stopTailCall = ()
                let result = createFolder 1
                try
                    executeOn.Iterate result result
                    result.ChainComplete (&stopTailCall, result.HaltedIdx)
                    result.Result
                finally
                    result.ChainDispose (&stopTailCall)

        module Wrap =
            type EmptyEnumerator<'T>() =
                let current () = failwith "library implementation error: Current should never be called"
                interface IEnumerator<'T> with
                    member __.Current = current ()
                interface IEnumerator with
                    member __.Current = current ()
                    member __.MoveNext () = false
                    member __.Reset (): unit = noReset ()
                interface IDisposable with
                    member __.Dispose () = ()

            type EmptyEnumerators<'T>() =
                static let element : IEnumerator<'T> = upcast (new EmptyEnumerator<'T> ())
                static member Element = element

            [<AbstractClass>]
            type EnumeratorBase<'T>(result:Result<'T>, activity:Activity) =
                interface IDisposable with
                    member __.Dispose () : unit =
                        let mutable stopTailCall = ()
                        activity.ChainDispose (&stopTailCall)

                interface IEnumerator with
                    member this.Current : obj = box ((Upcast.enumerator this)).Current
                    member __.MoveNext () = failwith "library implementation error: derived class should implement (should be abstract)"
                    member __.Reset () : unit = noReset ()

                interface IEnumerator<'T> with
                    member __.Current =
                        if result.SeqState = SeqProcessNextStates.InProcess then result.Result
                        else
                            match result.SeqState with
                            | SeqProcessNextStates.NotStarted -> notStarted()
                            | SeqProcessNextStates.Finished -> alreadyFinished()
                            | _ -> failwith "library implementation error: all states should have been handled"

            type VanillaEnumerator<'T,'U>(source:IEnumerator<'T>, activity:Activity<'T,'U>, result:Result<'U>) =
                inherit EnumeratorBase<'U>(result, activity)

                let rec moveNext () =
                    if (result.HaltedIdx = 0) && source.MoveNext () then
                        if activity.ProcessNext source.Current then
                            true
                        else
                            moveNext ()
                    else
                        result.SeqState <- SeqProcessNextStates.Finished
                        let mutable stopTailCall = ()
                        activity.ChainComplete (&stopTailCall, result.HaltedIdx)
                        false

                interface IEnumerator with
                    member __.MoveNext () =
                        result.SeqState <- SeqProcessNextStates.InProcess
                        moveNext ()

                interface IDisposable with
                    member __.Dispose () =
                        try
                            source.Dispose ()
                        finally
                            let mutable stopTailCall = ()
                            activity.ChainDispose (&stopTailCall)

            type ConcatEnumerator<'T, 'Collection when 'Collection :> seq<'T>> (sources:seq<'Collection>) =
                let mutable state = SeqProcessNextStates.NotStarted
                let main = sources.GetEnumerator ()

                let mutable active = EmptyEnumerators.Element

                let rec moveNext () =
                    if active.MoveNext () then
                        true
                    elif main.MoveNext () then
                        active.Dispose ()
                        active <- main.Current.GetEnumerator ()
                        moveNext ()
                    else
                        state <- SeqProcessNextStates.Finished
                        false

                interface IEnumerator<'T> with
                    member __.Current =
                        if state = SeqProcessNextStates.InProcess then active.Current
                        else
                            match state with
                            | SeqProcessNextStates.NotStarted -> notStarted()
                            | SeqProcessNextStates.Finished -> alreadyFinished()
                            | _ -> failwith "library implementation error: all states should have been handled"

                interface IEnumerator with
                    member this.Current = box ((Upcast.enumerator this)).Current
                    member __.MoveNext () =
                        state <- SeqProcessNextStates.InProcess
                        moveNext ()
                    member __.Reset () = noReset ()

                interface IDisposable with
                    member __.Dispose () =
                        main.Dispose ()
                        active.Dispose ()

            let length (source:ISeq<_>) =
                source.Fold (fun _ ->
                    upcast { new Folder<'T,int,NoValue>(0,Unchecked.defaultof<_>) with
                        override this.ProcessNext v =
                            this.Result <- this.Result + 1
                            Unchecked.defaultof<_> (* return value unused in Fold context *) })

            [<AbstractClass>]
            type EnumerableBase<'T> () =
                let derivedClassShouldImplement () =
                    failwith "library implementation error: derived class should implement (should be abstract)"

                abstract member Append : ISeq<'T> -> ISeq<'T>
                abstract member Length : unit -> int

                default this.Append source = Upcast.seq (AppendEnumerable [source; this])
                default this.Length () = length this

                interface IEnumerable with
                    member this.GetEnumerator () : IEnumerator =
                        let genericEnumerable = Upcast.enumerable this
                        let genericEnumerator = genericEnumerable.GetEnumerator ()
                        Upcast.enumeratorNonGeneric genericEnumerator

                interface IEnumerable<'T> with
                    member this.GetEnumerator () : IEnumerator<'T> = derivedClassShouldImplement ()

                interface ISeq<'T> with
                    member __.PushTransform _ = derivedClassShouldImplement ()
                    member __.Fold _ = derivedClassShouldImplement ()

            and VanillaEnumerable<'T,'U>(enumerable:IEnumerable<'T>, current:TransformFactory<'T,'U>, pipeIdx:PipeIdx) =
                inherit EnumerableBase<'U>()

                interface IEnumerable<'U> with
                    member this.GetEnumerator () : IEnumerator<'U> =
                        let result = Result<'U> ()
                        Upcast.enumerator (new VanillaEnumerator<'T,'U>(enumerable.GetEnumerator(), createFold current result pipeIdx, result))

                interface ISeq<'U> with
                    member __.PushTransform (next:TransformFactory<'U,'V>) : ISeq<'V> =
                        Upcast.seq (new VanillaEnumerable<'T,'V>(enumerable, ComposedFactory.Combine current next, pipeIdx+1))

                    member this.Fold<'Result,'State> (f:PipeIdx->Folder<'U,'Result,'State>) =
                        Fold.execute f current pipeIdx (Fold.IterateEnumerable enumerable)

            and ConcatEnumerable<'T, 'Collection, 'Collections when 'Collection :> seq<'T> and 'Collections :> seq<'Collection>> (sources:'Collections, preEnumerate:'Collections->'Collections) =
                inherit EnumerableBase<'T>()

                interface IEnumerable<'T> with
                    member this.GetEnumerator () : IEnumerator<'T> =
                        Upcast.enumerator (new ConcatEnumerator<_,_> (preEnumerate sources))

                interface ISeq<'T> with
                    member this.PushTransform (next:TransformFactory<'T,'U>) : ISeq<'U> =
                        Upcast.seq (VanillaEnumerable<'T,'V>(this, next, 1))

                    member this.Fold<'Result,'State> (f:PipeIdx->Folder<'T,'Result,'State>) =
                        Fold.executeThin f (Fold.IterateEnumerable this)

            and AppendEnumerable<'T> (sources:list<ISeq<'T>>) =
                inherit ConcatEnumerable<'T, ISeq<'T>, list<ISeq<'T>>>(sources, List.rev)

                override this.Append source =
                    Upcast.seq (AppendEnumerable (source::sources))

            /// ThinEnumerable is used when the IEnumerable provided to ofSeq is neither an array or a list
            type ThinEnumerable<'T>(enumerable:IEnumerable<'T>) =
                inherit EnumerableBase<'T>()

                override __.Length () =
                    match enumerable with
                    | :? ICollection<'T> as a -> a.Count
#if !FSCORE_PORTABLE_OLD
                    | :? IReadOnlyCollection<'T> as a -> a.Count
#endif
                    | _ ->
                        use e = enumerable.GetEnumerator ()
                        let mutable count = 0
                        while e.MoveNext () do
                            count <- count + 1
                        count

                interface IEnumerable<'T> with
                    member this.GetEnumerator () = enumerable.GetEnumerator ()

                interface ISeq<'T> with
                    member __.PushTransform (next:TransformFactory<'T,'U>) : ISeq<'U> =
                        Upcast.seq (new VanillaEnumerable<'T,'U>(enumerable, next, 1))

                    member this.Fold<'Result,'State> (f:PipeIdx->Folder<'T,'Result,'State>) =
                        Fold.executeThin f (Fold.IterateEnumerable enumerable)

            type DelayedEnumerable<'T>(delayed:unit->ISeq<'T>, pipeIdx:PipeIdx) =
                inherit EnumerableBase<'T>()

                override __.Length () =
                    match delayed() with
                    | :? EnumerableBase<'T> as s -> s.Length ()
                    | s -> length s

                interface IEnumerable<'T> with
                    member this.GetEnumerator () : IEnumerator<'T> = (delayed()).GetEnumerator ()

                interface ISeq<'T> with
                    member __.PushTransform (next:TransformFactory<'T,'U>) : ISeq<'U> =
                        Upcast.seq (new DelayedEnumerable<'U>((fun () -> (delayed()).PushTransform next), pipeIdx+1))

                    member this.Fold<'Result,'State> (f:PipeIdx->Folder<'T,'Result,'State>) =
                        (delayed()).Fold f

            type EmptyEnumerable<'T> () =
                inherit EnumerableBase<'T>()

                static let singleton = EmptyEnumerable<'T>() :> ISeq<'T>
                static member Instance = singleton

                override __.Length () = 0

                interface IEnumerable<'T> with
                    member this.GetEnumerator () : IEnumerator<'T> = IEnumerator.Empty<'T>()

                override this.Append source = source

                interface ISeq<'T> with
                    member this.PushTransform (next:TransformFactory<'T,'U>) : ISeq<'U> =
                        Upcast.seq (VanillaEnumerable<'T,'V>(this, next, 1))

                    member this.Fold<'Result,'State> (f:PipeIdx->Folder<'T,'Result,'State>) =
                        Fold.executeThin f (Fold.IterateEnumerable this)

            type ArrayEnumerator<'T,'U>(array:array<'T>, activity:Activity<'T,'U>, result:Result<'U>) =
                inherit EnumeratorBase<'U>(result, activity)

                let mutable idx = 0

                let rec moveNext () =
                    if (result.HaltedIdx = 0) && idx < array.Length then
                        idx <- idx+1
                        if activity.ProcessNext array.[idx-1] then
                            true
                        else
                            moveNext ()
                    else
                        result.SeqState <- SeqProcessNextStates.Finished
                        let mutable stopTailCall = ()
                        activity.ChainComplete (&stopTailCall, result.HaltedIdx)
                        false

                interface IEnumerator with
                    member __.MoveNext () =
                        result.SeqState <- SeqProcessNextStates.InProcess
                        moveNext ()

            type ArrayEnumerable<'T,'U>(array:array<'T>, transformFactory:TransformFactory<'T,'U>, pipeIdx:PipeIdx) =
                inherit EnumerableBase<'U>()

                override this.Length () =
                    if obj.ReferenceEquals (transformFactory, IdentityFactory<'U>.Instance) then
                        array.Length
                    else
                        length this

                interface IEnumerable<'U> with
                    member this.GetEnumerator () : IEnumerator<'U> =
                        let result = Result<'U> ()
                        Upcast.enumerator (new ArrayEnumerator<'T,'U>(array, createFold transformFactory result pipeIdx, result))

                interface ISeq<'U> with
                    member __.PushTransform (next:TransformFactory<'U,'V>) : ISeq<'V> =
                        Upcast.seq (new ArrayEnumerable<'T,'V>(array, ComposedFactory.Combine transformFactory next, 1))

                    member this.Fold<'Result,'State> (f:PipeIdx->Folder<'U,'Result,'State>) =
                        Fold.execute f transformFactory pipeIdx (Fold.IterateArray array)

            type ResizeArrayEnumerator<'T,'U>(array:ResizeArray<'T>, activity:Activity<'T,'U>, result:Result<'U>) =
                inherit EnumeratorBase<'U>(result, activity)

                let mutable idx = 0

                let rec moveNext () =
                    if (result.HaltedIdx = 0) && idx < array.Count then
                        idx <- idx+1
                        if activity.ProcessNext array.[idx-1] then
                            true
                        else
                            moveNext ()
                    else
                        result.SeqState <- SeqProcessNextStates.Finished
                        let mutable stopTailCall = ()
                        activity.ChainComplete (&stopTailCall, result.HaltedIdx)
                        false

                interface IEnumerator with
                    member __.MoveNext () =
                        result.SeqState <- SeqProcessNextStates.InProcess
                        moveNext ()

            type ResizeArrayEnumerable<'T,'U>(resizeArray:ResizeArray<'T>, transformFactory:TransformFactory<'T,'U>, pipeIdx:PipeIdx) =
                inherit EnumerableBase<'U>()

                override this.Length () =
                    if obj.ReferenceEquals (transformFactory, IdentityFactory<'U>.Instance) then
                        resizeArray.Count
                    else
                        length this

                interface IEnumerable<'U> with
                    member this.GetEnumerator () : IEnumerator<'U> =
                        let result = Result<'U> ()
                        Upcast.enumerator (new ResizeArrayEnumerator<'T,'U>(resizeArray, createFold transformFactory result pipeIdx, result))

                interface ISeq<'U> with
                    member __.PushTransform (next:TransformFactory<'U,'V>) : ISeq<'V> =
                        Upcast.seq (new ResizeArrayEnumerable<'T,'V>(resizeArray, ComposedFactory.Combine transformFactory next, 1))

                    member this.Fold<'Result,'State> (f:PipeIdx->Folder<'U,'Result,'State>) =
                        Fold.execute f transformFactory pipeIdx (Fold.IterateResizeArray resizeArray)

            type ListEnumerator<'T,'U>(alist:list<'T>, activity:Activity<'T,'U>, result:Result<'U>) =
                inherit EnumeratorBase<'U>(result, activity)

                let mutable list = alist

                let rec moveNext current =
                    match result.HaltedIdx, current with
                    | 0, head::tail ->
                        if activity.ProcessNext head then
                            list <- tail
                            true
                        else
                            moveNext tail
                    | _ ->
                        result.SeqState <- SeqProcessNextStates.Finished
                        let mutable stopTailCall = ()
                        activity.ChainComplete (&stopTailCall, result.HaltedIdx)
                        false

                interface IEnumerator with
                    member __.MoveNext () =
                        result.SeqState <- SeqProcessNextStates.InProcess
                        moveNext list

            type ListEnumerable<'T,'U>(alist:list<'T>, transformFactory:TransformFactory<'T,'U>, pipeIdx:PipeIdx) =
                inherit EnumerableBase<'U>()

                override this.Length () =
                    if obj.ReferenceEquals (transformFactory, IdentityFactory<'U>.Instance) then
                        alist.Length
                    else
                        length this

                interface IEnumerable<'U> with
                    member this.GetEnumerator () : IEnumerator<'U> =
                        let result = Result<'U> ()
                        Upcast.enumerator (new ListEnumerator<'T,'U>(alist, createFold transformFactory result pipeIdx, result))

                interface ISeq<'U> with
                    member __.PushTransform (next:TransformFactory<'U,'V>) : ISeq<'V> =
                        Upcast.seq (new ListEnumerable<'T,'V>(alist, ComposedFactory.Combine transformFactory next, pipeIdx+1))

                    member this.Fold<'Result,'State> (f:PipeIdx->Folder<'U,'Result,'State>) =
                        Fold.execute f transformFactory pipeIdx (Fold.IterateList alist)

            type UnfoldEnumerator<'T,'U,'State>(generator:'State->option<'T*'State>, state:'State, activity:Activity<'T,'U>, result:Result<'U>) =
                inherit EnumeratorBase<'U>(result, activity)

                let mutable current = state

                let rec moveNext () =
                    if result.HaltedIdx <> 0 then
                        false
                    else
                        match generator current with
                        | Some (item, nextState) ->
                            current <- nextState
                            if activity.ProcessNext item then
                                true
                            else
                                moveNext ()
                        | _ -> false

                interface IEnumerator with
                    member __.MoveNext () =
                        result.SeqState <- SeqProcessNextStates.InProcess
                        moveNext ()

            type UnfoldEnumerable<'T,'U,'GeneratorState>(generator:'GeneratorState->option<'T*'GeneratorState>, state:'GeneratorState, transformFactory:TransformFactory<'T,'U>, pipeIdx:PipeIdx) =
                inherit EnumerableBase<'U>()

                interface IEnumerable<'U> with
                    member this.GetEnumerator () : IEnumerator<'U> =
                        let result = Result<'U> ()
                        Upcast.enumerator (new UnfoldEnumerator<'T,'U,'GeneratorState>(generator, state, createFold transformFactory result pipeIdx, result))

                interface ISeq<'U> with
                    member this.PushTransform (next:TransformFactory<'U,'V>) : ISeq<'V> =
                        Upcast.seq (new UnfoldEnumerable<'T,'V,'GeneratorState>(generator, state, ComposedFactory.Combine transformFactory next, pipeIdx+1))

                    member this.Fold<'Result,'State> (f:PipeIdx->Folder<'U,'Result,'State>) =
                        Fold.execute f transformFactory pipeIdx (Fold.IterateUnfold (generator, state))

            let getInitTerminatingIdx (count:Nullable<int>) =
                // we are offset by 1 to allow for values going up to System.Int32.MaxValue
                // System.Int32.MaxValue is an illegal value for the "infinite" sequence
                if count.HasValue then
                    count.Value - 1
                else
                    System.Int32.MaxValue

            type InitEnumerator<'T,'U>(count:Nullable<int>, f:int->'T, activity:Activity<'T,'U>, result:Result<'U>) =
                inherit EnumeratorBase<'U>(result, activity)

                let isSkipping =
                    match box activity with
                    | :? ISkipable as skip -> skip.CanSkip
                    | _ -> fun () -> false

                let terminatingIdx =
                    getInitTerminatingIdx count

                let mutable maybeSkipping = true
                let mutable idx = -1

                let rec moveNext () =
                    if result.HaltedIdx = 0 && idx < terminatingIdx then
                        idx <- idx + 1

                        if maybeSkipping then
                            // Skip can only is only checked at the start of the sequence, so once
                            // triggered, we stay triggered.
                            maybeSkipping <- isSkipping ()

                        if maybeSkipping then
                            moveNext ()
                        elif activity.ProcessNext (f idx) then
                            true
                        else
                            moveNext ()
                    elif result.HaltedIdx = 0 && idx = System.Int32.MaxValue then
                        raise <| System.InvalidOperationException (SR.GetString(SR.enumerationPastIntMaxValue))
                    else
                        result.SeqState <- SeqProcessNextStates.Finished
                        let mutable stopTailCall = ()
                        activity.ChainComplete (&stopTailCall, result.HaltedIdx)
                        false

                interface IEnumerator with
                    member __.MoveNext () =
                        result.SeqState <- SeqProcessNextStates.InProcess
                        moveNext ()

            type InitEnumerable<'T,'U>(count:Nullable<int>, f:int->'T, transformFactory:TransformFactory<'T,'U>, pipeIdx:PipeIdx) =
                inherit EnumerableBase<'U>()

                interface IEnumerable<'U> with
                    member this.GetEnumerator () : IEnumerator<'U> =
                        let result = Result<'U> ()
                        Upcast.enumerator (new InitEnumerator<'T,'U>(count, f, createFold transformFactory result pipeIdx, result))

                interface ISeq<'U> with
                    member this.PushTransform (next:TransformFactory<'U,'V>) : ISeq<'V> =
                        Upcast.seq (new InitEnumerable<'T,'V>(count, f, ComposedFactory.Combine transformFactory next, pipeIdx+1))

                    member this.Fold<'Result,'State> (createResult:PipeIdx->Folder<'U,'Result,'State>) =
                        let terminatingIdx = getInitTerminatingIdx count
                        Fold.execute createResult transformFactory pipeIdx (Fold.IterateInit (f, terminatingIdx))

            // The original implementation of "init" delayed the calculation of Current, and so it was possible
            // to do MoveNext without it's value being calculated.
            // I can imagine only two scenerios where that is possibly sane, although a simple solution is readily
            // at hand in both cases. The first is that of an expensive generator function, where you skip the
            // first n elements. The simple solution would have just been to have a map ((+) n) as the first operation
            // instead. The second case would be counting elements, but that is only of use if you're not filtering
            // or mapping or doing anything else (as that would cause Current to be evaluated!) and
            // so you already know what the count is!! Anyway, someone thought it was a good idea, so
            // I have had to add an extra function that is used in Skip to determine if we are touching
            // Current or not.
            // InitEnumerableDecider returns the original implementation of init when GetEnumerator is called
            // If any Activites are added to the pipeline then the original implementation is ignored, as special
            // handling has been added to Current isn't calculated whilst skipping enumerated items.
            type InitEnumerableDecider<'T>(count:Nullable<int>, f:int->'T, pipeIdx:PipeIdx) =
                inherit EnumerableBase<'T>()

                let upto lastOption f =
                    match lastOption with
                    | Some b when b<0 -> failwith "library implementation error: upto can never be called with a negative value"
                    | _ ->
                        let unstarted   = -1  // index value means unstarted (and no valid index)
                        let completed   = -2  // index value means completed (and no valid index)
                        let unreachable = -3  // index is unreachable from 0,1,2,3,...
                        let finalIndex  = match lastOption with
                                            | Some b -> b             // here b>=0, a valid end value.
                                            | None   -> unreachable   // run "forever", well as far as Int32.MaxValue since indexing with a bounded type.
                        // The Current value for a valid index is "f i".
                        // Lazy<_> values are used as caches, to store either the result or an exception if thrown.
                        // These "Lazy<_>" caches are created only on the first call to current and forced immediately.
                        // The lazy creation of the cache nodes means enumerations that skip many Current values are not delayed by GC.
                        // For example, the full enumeration of Seq.initInfinite in the tests.
                        // state
                        let index   = ref unstarted
                        // a Lazy node to cache the result/exception
                        let current = ref (Unchecked.defaultof<_>)
                        let setIndex i = index := i; current := (Unchecked.defaultof<_>) // cache node unprimed, initialised on demand.
                        let getCurrent() =
                            if !index = unstarted then notStarted()
                            if !index = completed then alreadyFinished()
                            match box !current with
                            | null -> current := Lazy<_>.Create(fun () -> f !index)
                            | _ ->  ()
                            // forced or re-forced immediately.
                            (!current).Force()
                        { new IEnumerator<'U> with
                                member x.Current = getCurrent()
                            interface IEnumerator with
                                member x.Current = box (getCurrent())
                                member x.MoveNext() =
                                    if !index = completed then
                                        false
                                    elif !index = unstarted then
                                        setIndex 0
                                        true
                                    else (
                                        if !index = System.Int32.MaxValue then raise <| System.InvalidOperationException (SR.GetString(SR.enumerationPastIntMaxValue))
                                        if !index = finalIndex then
                                            false
                                        else
                                            setIndex (!index + 1)
                                            true
                                    )
                                member this.Reset() = noReset()
                            interface System.IDisposable with
                                member x.Dispose () = () }

                override this.Length () =
                    if count.HasValue then
                        count.Value
                    else
                        raise (System.InvalidOperationException (SR.GetString(SR.enumerationPastIntMaxValue)))

                interface IEnumerable<'T> with
                    member this.GetEnumerator () : IEnumerator<'T> =
                        // we defer back to the original implementation as, as it's quite idiomatic in it's decision
                        // to calculate Current in a lazy fashion. I doubt anyone is really using this functionality
                        // in the way presented, but it's possible.
                        upto (if count.HasValue then Some (count.Value-1) else None) f

                interface ISeq<'T> with
                    member this.PushTransform (next:TransformFactory<'T,'U>) : ISeq<'U> =
                        Upcast.seq (InitEnumerable<'T,'V>(count, f, next, pipeIdx+1))

                    member this.Fold<'Result,'State> (f:PipeIdx->Folder<'T,'Result,'State>) =
                        Fold.executeThin f (Fold.IterateEnumerable (Upcast.enumerable this))

        /// wraps a ResizeArray in the ISeq framework. Care must be taken that the underlying ResizeArray
        /// is not modified whilst it can be accessed as the ISeq, so check on version is performed.
        /// i.e. usually iteration on calls the enumerator provied by GetEnumerator ensure that the
        /// list hasn't been modified (throwing an exception if it has), but such a check is not
        /// performed in this case. If you want this funcitonality, then use the ofSeq function instead.
        [<CompiledName "OfResizeArrayUnchecked">]
        let ofResizeArrayUnchecked (source:ResizeArray<'T>) : ISeq<'T> =
            Upcast.seq (Wrap.ResizeArrayEnumerable (source, IdentityFactory.Instance, 1))

        [<CompiledName "OfArray">]
        let ofArray (source:array<'T>) : ISeq<'T> =
            checkNonNull "source" source
            Upcast.seq (Wrap.ArrayEnumerable (source, IdentityFactory.Instance, 1))

        [<CompiledName "OfList">]
        let ofList (source:list<'T>) : ISeq<'T> =
            Upcast.seq (Wrap.ListEnumerable (source, IdentityFactory.Instance, 1))

        [<CompiledName "OfSeq">]
        let ofSeq (source:seq<'T>) : ISeq<'T> =
            match source with
            | :? ISeq<'T>  as seq   -> seq
            | :? array<'T> as array -> ofArray array
            | :? list<'T>  as list  -> ofList list
            | null                  -> nullArg "source"
            | _                     -> Upcast.seq (Wrap.ThinEnumerable<'T> source)

        [<CompiledName "Average">]
        let inline average (source:ISeq<'T>) =
            source.Fold (fun _ ->
                upcast { new FolderWithPostProcessing<'T,'T,int> (LanguagePrimitives.GenericZero, 0) with
                    override this.ProcessNext value =
                        this.Result <- Checked.(+) this.Result value
                        this.State <- this.State + 1
                        Unchecked.defaultof<_> (* return value unused in Fold context *)

                    override this.OnComplete _ =
                        if this.State = 0 then
                            invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
                        this.Result <- LanguagePrimitives.DivideByInt<'T> this.Result this.State 
                    override this.OnDispose () = () })

        [<CompiledName "AverageBy">]
        let inline averageBy (f:'T->'U) (source:ISeq<'T>) =
            source.Fold (fun _ ->
                upcast { new FolderWithPostProcessing<'T,'U,int>(LanguagePrimitives.GenericZero,0) with
                    override this.ProcessNext value =
                        this.Result <- Checked.(+) this.Result (f value)
                        this.State <- this.State + 1
                        Unchecked.defaultof<_> (* return value unused in Fold context *)

                    override this.OnComplete _ =
                        if this.State = 0 then
                            invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
                        this.Result <- LanguagePrimitives.DivideByInt<'U> this.Result this.State
                    override this.OnDispose () = () })

        [<CompiledName "Empty">]
        let empty<'T> = Wrap.EmptyEnumerable<'T>.Instance

        [<CompiledName "ExactlyOne">]
        let exactlyOne (source:ISeq<'T>) : 'T =
            source.Fold (fun pipeIdx ->
                upcast { new FolderWithPostProcessing<'T,'T,Values<bool, bool>>(Unchecked.defaultof<'T>, Values<bool,bool>(true, false)) with
                    override this.ProcessNext value =
                        if this.State._1 then
                            this.State._1 <- false
                            this.Result <- value
                        else
                            this.State._2 <- true
                            this.StopFurtherProcessing pipeIdx
                        Unchecked.defaultof<_> (* return value unused in Fold context *)

                    override this.OnComplete _ =
                        if this.State._1 then
                            invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
                        elif this.State._2 then
                            invalidArg "source" (SR.GetString SR.inputSequenceTooLong)
                    override this.OnDispose () = () })

        [<CompiledName "Fold">]
        let inline fold<'T,'State> (f:'State->'T->'State) (seed:'State) (source:ISeq<'T>) : 'State =
            source.Fold (fun _ ->
                upcast { new Folder<'T,'State,NoValue>(seed,Unchecked.defaultof<NoValue>) with
                    override this.ProcessNext value =
                        this.Result <- f this.Result value
                        Unchecked.defaultof<_> (* return value unused in Fold context *) })

        [<CompiledName "Fold2">]
        let inline fold2<'T1,'T2,'State> (folder:'State->'T1->'T2->'State) (state:'State) (source1:ISeq<'T1>) (source2: ISeq<'T2>) =
            source1.Fold (fun pipeIdx ->
                upcast { new FolderWithPostProcessing<_,'State,IEnumerator<'T2>>(state,source2.GetEnumerator()) with
                    override this.ProcessNext value =
                        if this.State.MoveNext() then
                            this.Result <- folder this.Result value this.State.Current
                        else
                            this.StopFurtherProcessing pipeIdx
                        Unchecked.defaultof<_> (* return value unused in Fold context *)

                    override this.OnComplete _ = ()
                    override this.OnDispose () = this.State.Dispose () })

        [<CompiledName "Unfold">]
        let unfold (generator:'State->option<'T * 'State>) (state:'State) : ISeq<'T> =
            Upcast.seq (new Wrap.UnfoldEnumerable<'T,'T,'State>(generator, state, IdentityFactory.Instance, 1))

        [<CompiledName "InitializeInfinite">]
        let initInfinite<'T> (f:int->'T) : ISeq<'T> =
            Upcast.seq (new Wrap.InitEnumerableDecider<'T>(Nullable (), f, 1))

        [<CompiledName "Initialize">]
        let init<'T> (count:int) (f:int->'T) : ISeq<'T> =
            if count < 0 then invalidArgInputMustBeNonNegative "count" count
            elif count = 0 then empty else
            Upcast.seq (new Wrap.InitEnumerableDecider<'T>(Nullable count, f, 1))

        [<CompiledName "Iterate">]
        let inline iter f (source:ISeq<'T>) =
            source.Fold (fun _ ->
                upcast { new Folder<'T,unit,NoValue> ((),Unchecked.defaultof<NoValue>) with
                    override this.ProcessNext value =
                        f value
                        Unchecked.defaultof<_> (* return value unused in Fold context *) })

        [<CompiledName "Iterate2">]
        let inline iter2 (f:'T->'U->unit) (source1:ISeq<'T>) (source2:ISeq<'U>) : unit =
            source1.Fold (fun pipeIdx ->
                upcast { new FolderWithPostProcessing<'T,unit,IEnumerator<'U>> ((),source2.GetEnumerator()) with
                    override this.ProcessNext value =
                        if this.State.MoveNext() then
                            f value this.State.Current
                        else
                            this.StopFurtherProcessing pipeIdx
                        Unchecked.defaultof<_> (* return value unused in Fold context *)

                    override this.OnComplete _ = ()
                    override this.OnDispose () = this.State.Dispose () })

        [<CompiledName "IterateIndexed2">]
        let inline iteri2 (f:int->'T->'U->unit) (source1:ISeq<'T>) (source2:ISeq<'U>) : unit =
            source1.Fold (fun pipeIdx ->
                upcast { new FolderWithPostProcessing<'T,unit,Values<int,IEnumerator<'U>>>((),Values<_,_>(0,source2.GetEnumerator())) with
                    override this.ProcessNext value =
                        if this.State._2.MoveNext() then
                            f this.State._1 value this.State._2.Current
                            this.State._1 <- this.State._1 + 1
                            Unchecked.defaultof<_>
                        else
                            this.StopFurtherProcessing pipeIdx
                            Unchecked.defaultof<_>
                    override this.OnComplete _ = () 
                    override this.OnDispose () = this.State._2.Dispose () })

        [<CompiledName "TryHead">]
        let tryHead (source:ISeq<'T>) =
            source.Fold (fun pipeIdx ->
                upcast { new Folder<'T, Option<'T>,NoValue> (None,Unchecked.defaultof<NoValue>) with
                    override this.ProcessNext value =
                        this.Result <- Some value
                        this.StopFurtherProcessing pipeIdx
                        Unchecked.defaultof<_> (* return value unused in Fold context *) })

        [<CompiledName "Head">]
        let head (source:ISeq<_>) =
            match tryHead source with
            | None -> invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
            | Some x -> x

        [<CompiledName "IterateIndexed">]
        let inline iteri f (source:ISeq<'T>) =
            source.Fold (fun _ ->
                { new Folder<'T,unit,int> ((),0) with
                    override this.ProcessNext value =
                        f this.State value
                        this.State <- this.State + 1
                        Unchecked.defaultof<_> (* return value unused in Fold context *) })

        [<CompiledName "Except">]
        let inline except (itemsToExclude: seq<'T>) (source:ISeq<'T>) : ISeq<'T> when 'T:equality =
            source.PushTransform { new TransformFactory<'T,'T>() with
                override __.Compose _ _ next =
                    upcast { new Transform<'T,'V,Lazy<HashSet<'T>>>(next,lazy(HashSet<'T>(itemsToExclude,HashIdentity.Structural<'T>))) with
                        override this.ProcessNext (input:'T) : bool =
                            this.State.Value.Add input && TailCall.avoid (next.ProcessNext input) }}

        [<CompiledName "Exists">]
        let inline exists f (source:ISeq<'T>) =
            source.Fold (fun pipeIdx ->
                upcast { new Folder<'T, bool,NoValue> (false,Unchecked.defaultof<NoValue>) with
                    override this.ProcessNext value =
                        if f value then
                            this.Result <- true
                            this.StopFurtherProcessing pipeIdx
                        Unchecked.defaultof<_> (* return value unused in Fold context *) })

        [<CompiledName "Exists2">]
        let inline exists2 (predicate:'T->'U->bool) (source1:ISeq<'T>) (source2: ISeq<'U>) : bool =
            source1.Fold (fun pipeIdx ->
                upcast { new FolderWithPostProcessing<'T,bool,IEnumerator<'U>>(false,source2.GetEnumerator()) with
                    override this.ProcessNext value =
                        if this.State.MoveNext() then
                            if predicate value this.State.Current then
                                this.Result <- true
                                this.StopFurtherProcessing pipeIdx
                        else
                            this.StopFurtherProcessing pipeIdx
                        Unchecked.defaultof<_> (* return value unused in Fold context *)

                    override this.OnComplete _ = ()
                    override this.OnDispose () = this.State.Dispose () })

        [<CompiledName "Contains">]
        let inline contains element (source:ISeq<'T>) =
            source.Fold (fun pipeIdx ->
                upcast { new Folder<'T, bool,NoValue> (false,Unchecked.defaultof<NoValue>) with
                    override this.ProcessNext value =
                        if element = value then
                            this.Result <- true
                            this.StopFurtherProcessing pipeIdx
                        Unchecked.defaultof<_> (* return value unused in Fold context *) })

        [<CompiledName "ForAll">]
        let inline forall predicate (source:ISeq<'T>) =
            source.Fold (fun pipeIdx ->
                upcast { new Folder<'T, bool,NoValue> (true,Unchecked.defaultof<NoValue>) with
                    override this.ProcessNext value =
                        if not (predicate value) then
                            this.Result <- false
                            this.StopFurtherProcessing pipeIdx
                        Unchecked.defaultof<_> (* return value unused in Fold context *) })

        [<CompiledName "ForAll2">]
        let inline forall2 predicate (source1:ISeq<'T>) (source2:ISeq<'U>) : bool =
            source1.Fold (fun pipeIdx ->
                upcast { new FolderWithPostProcessing<'T,bool,IEnumerator<'U>>(true,source2.GetEnumerator()) with
                    override this.ProcessNext value =
                        if this.State.MoveNext() then
                            if not (predicate value this.State.Current) then
                                this.Result <- false
                                this.StopFurtherProcessing pipeIdx
                        else
                            this.StopFurtherProcessing pipeIdx
                        Unchecked.defaultof<_> (* return value unused in Fold context *)

                    override this.OnComplete _ = ()
                    override this.OnDispose () = this.State.Dispose () })

        [<CompiledName "Filter">]
        let inline filter<'T> (f:'T->bool) (source:ISeq<'T>) : ISeq<'T> =
            source.PushTransform { new TransformFactory<'T,'T>() with
                override __.Compose _ _ next =
                    upcast { new Transform<'T,'V,NoValue>(next,Unchecked.defaultof<NoValue>) with
                        override __.ProcessNext input =
                            if f input then TailCall.avoid (next.ProcessNext input)
                            else false } }

        [<CompiledName "Map">]
        let inline map<'T,'U> (f:'T->'U) (source:ISeq<'T>) : ISeq<'U> =
            source.PushTransform { new TransformFactory<'T,'U>() with
                override __.Compose _ _ next =
                    upcast { new Transform<'T,'V,NoValue>(next,Unchecked.defaultof<NoValue>) with
                        override __.ProcessNext input =
                            TailCall.avoid (next.ProcessNext (f input)) } }

        [<CompiledName "MapIndexed">]
        let inline mapi f (source:ISeq<_>) =
            source.PushTransform { new TransformFactory<'T,'U>() with
                override __.Compose _ _ next =
                    upcast { new Transform<'T,'V,int>(next, -1) with
                        override this.ProcessNext (input:'T) : bool =
                            this.State <- this.State  + 1
                            TailCall.avoid (next.ProcessNext (f this.State input)) } }

        [<CompiledName "Map2">]
        let inline map2<'T,'U,'V> (map:'T->'U->'V) (source1:ISeq<'T>) (source2:ISeq<'U>) : ISeq<'V> =
            source1.PushTransform { new TransformFactory<'T,'V>() with
                override __.Compose outOfBand pipeIdx (next:Activity<'V,'W>) =
                    upcast { new TransformWithPostProcessing<'T,'W, IEnumerator<'U>>(next, (source2.GetEnumerator ())) with
                        override this.ProcessNext input =
                            if this.State.MoveNext () then
                                TailCall.avoid (next.ProcessNext (map input this.State.Current))
                            else
                                outOfBand.StopFurtherProcessing pipeIdx
                                false
                        override this.OnComplete _ = () 
                        override this.OnDispose () = this.State.Dispose () }}

        [<CompiledName "MapIndexed2">]
        let inline mapi2<'T,'U,'V> (map:int->'T->'U->'V) (source1:ISeq<'T>) (source2:ISeq<'U>) : ISeq<'V> =
            source1.PushTransform { new TransformFactory<'T,'V>() with
                override __.Compose<'W> outOfBand pipeIdx next =
                    upcast { new TransformWithPostProcessing<'T,'W, Values<int,IEnumerator<'U>>>(next, Values<_,_>(-1,source2.GetEnumerator ())) with
                        override this.ProcessNext t =
                            let idx : byref<_> = &this.State._1
                            let u = this.State._2
                            if u.MoveNext () then
                                idx <- idx + 1
                                TailCall.avoid (next.ProcessNext (map idx t u.Current))
                            else
                                outOfBand.StopFurtherProcessing pipeIdx
                                false
                        override this.OnDispose () = this.State._2.Dispose ()
                        override this.OnComplete _ = () }}

        [<CompiledName "Map3">]
        let inline map3<'T,'U,'V,'W>(map:'T->'U->'V->'W) (source1:ISeq<'T>) (source2:ISeq<'U>) (source3:ISeq<'V>) : ISeq<'W> =
            source1.PushTransform { new TransformFactory<'T,'W>() with
                override __.Compose<'X> outOfBand pipeIdx next =
                    upcast { new TransformWithPostProcessing<'T,'X,Values<IEnumerator<'U>,IEnumerator<'V>>>(next,Values<_,_>(source2.GetEnumerator(),source3.GetEnumerator())) with
                        override this.ProcessNext t =
                            let u = this.State._1
                            let v = this.State._2
                            if u.MoveNext() && v.MoveNext ()  then
                                TailCall.avoid (next.ProcessNext (map t u.Current v.Current))
                            else
                                outOfBand.StopFurtherProcessing pipeIdx
                                false
                        override this.OnComplete _ = () 
                        override this.OnDispose () = 
                            this.State._1.Dispose ()
                            this.State._2.Dispose () }}

        [<CompiledName "CompareWith">]
        let inline compareWith (f:'T->'T->int) (source1:ISeq<'T>) (source2:ISeq<'T>) : int =
            source1.Fold (fun pipeIdx ->
                upcast { new FolderWithPostProcessing<'T,int,IEnumerator<'T>>(0,source2.GetEnumerator()) with
                    override this.ProcessNext value =
                        if not (this.State.MoveNext()) then
                            this.Result <- 1
                            this.StopFurtherProcessing pipeIdx
                        else
                            let c = f value this.State.Current
                            if c <> 0 then
                                this.Result <- c
                                this.StopFurtherProcessing pipeIdx
                        Unchecked.defaultof<_> (* return value unused in Fold context *)
                    override this.OnComplete _ =
                        if this.Result = 0 && this.State.MoveNext() then
                            this.Result <- -1
                    override this.OnDispose () = this.State.Dispose () })

        [<CompiledName "Choose">]
        let inline choose (f:'T->option<'U>) (source:ISeq<'T>) : ISeq<'U> =
            source.PushTransform { new TransformFactory<'T,'U>() with
                override __.Compose _ _ next =
                    upcast { new Transform<'T,'V,NoValue>(next,Unchecked.defaultof<NoValue>) with
                        override __.ProcessNext input =
                            match f input with
                            | Some value -> TailCall.avoid (next.ProcessNext value)
                            | None       -> false } }

        [<CompiledName "Distinct">]
        let inline distinct (source:ISeq<'T>) : ISeq<'T> when 'T:equality =
            source.PushTransform { new TransformFactory<'T,'T>() with
                override __.Compose _ _ next =
                    upcast { new Transform<'T,'V,HashSet<'T>>(next,HashSet HashIdentity.Structural) with
                        override this.ProcessNext (input:'T) : bool =
                            this.State.Add input && TailCall.avoid (next.ProcessNext input) }}

        [<CompiledName "DistinctBy">]
        let inline distinctBy (keyf:'T->'Key) (source:ISeq<'T>) :ISeq<'T>  when 'Key:equality =
            source.PushTransform { new TransformFactory<'T,'T>() with
                override __.Compose _ _ next =
                    upcast { new Transform<'T,'V,HashSet<'Key>> (next,HashSet HashIdentity.Structural) with
                        override this.ProcessNext (input:'T) : bool =
                            this.State.Add (keyf input) && TailCall.avoid (next.ProcessNext input) }}

        [<CompiledName "Max">]
        let inline max (source:ISeq<'T>) : 'T when 'T:comparison =
            source.Fold (fun _ ->
                upcast { new FolderWithPostProcessing<'T,'T,bool>(Unchecked.defaultof<'T>,true) with
                    override this.ProcessNext value =
                        if this.State then
                            this.State <- false
                            this.Result <- value
                        elif value > this.Result then
                            this.Result <- value
                        Unchecked.defaultof<_> (* return value unused in Fold context *)

                    override this.OnComplete _ =
                        if this.State then
                            invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
                    override this.OnDispose () = () })

        [<CompiledName "MaxBy">]
        let inline maxBy (f:'T->'U) (source:ISeq<'T>) : 'T when 'U:comparison =
            source.Fold (fun _ ->
                upcast { new FolderWithPostProcessing<'T,'T,Values<bool,'U>>(Unchecked.defaultof<'T>,Values<_,_>(true,Unchecked.defaultof<'U>)) with
                    override this.ProcessNext value =
                        match this.State._1, f value with
                        | true, valueU ->
                            this.State._1 <- false
                            this.State._2 <- valueU
                            this.Result <- value
                        | false, valueU when valueU > this.State._2 ->
                            this.State._2 <- valueU
                            this.Result <- value
                        | _ -> ()
                        Unchecked.defaultof<_> (* return value unused in Fold context *)

                    override this.OnComplete _ =
                        if this.State._1 then
                            invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
                    override this.OnDispose () = () })

        [<CompiledName "Min">]
        let inline min (source:ISeq<'T>) : 'T when 'T:comparison =
            source.Fold (fun _ ->
                upcast { new FolderWithPostProcessing<'T,'T,bool>(Unchecked.defaultof<'T>,true) with
                    override this.ProcessNext value =
                        if this.State then
                            this.State <- false
                            this.Result <- value
                        elif value < this.Result then
                            this.Result <- value
                        Unchecked.defaultof<_> (* return value unused in Fold context *)

                    override this.OnComplete _ =
                        if this.State then
                            invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
                    override this.OnDispose () = () })

        [<CompiledName "MinBy">]
        let inline minBy (f:'T->'U) (source:ISeq<'T>) : 'T =
            source.Fold (fun _ ->
                upcast { new FolderWithPostProcessing<'T,'T,Values<bool,'U>>(Unchecked.defaultof<'T>,Values<_,_>(true,Unchecked.defaultof< 'U>)) with
                    override this.ProcessNext value =
                        match this.State._1, f value with
                        | true, valueU ->
                            this.State._1 <- false
                            this.State._2 <- valueU
                            this.Result <- value
                        | false, valueU when valueU < this.State._2 ->
                            this.State._2 <- valueU
                            this.Result <- value
                        | _ -> ()
                        Unchecked.defaultof<_> (* return value unused in Fold context *)

                    override this.OnComplete _ =
                        if this.State._1 then
                            invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
                    override this.OnDispose () = () })

        [<CompiledName "Pairwise">]
        let pairwise (source:ISeq<'T>) : ISeq<'T*'T> =
            source.PushTransform { new TransformFactory<'T,'T * 'T>() with
                override __.Compose _ _ next =
                    upcast { new Transform<'T,'U,Values<bool,'T>>(next, Values<bool,'T>(true, Unchecked.defaultof<'T>)) with
                            // member this.isFirst   = this.State._1
                            // member this.lastValue = this.State._2
                            override this.ProcessNext (input:'T) : bool =
                                if this.State._1  then
                                    this.State._2 <- input
                                    this.State._1 <- false
                                    false
                                else
                                    let currentPair = this.State._2, input
                                    this.State._2 <- input
                                    TailCall.avoid (next.ProcessNext currentPair) }}

        [<CompiledName "Reduce">]
        let inline reduce (f:'T->'T->'T) (source: ISeq<'T>) : 'T =
            source.Fold (fun _ ->
                upcast { new FolderWithPostProcessing<'T,'T,bool>(Unchecked.defaultof<'T>,true) with
                    override this.ProcessNext value =
                        if this.State then
                            this.State <- false
                            this.Result <- value
                        else
                            this.Result <- f this.Result value
                        Unchecked.defaultof<_> (* return value unused in Fold context *)

                    override this.OnComplete _ =
                        if this.State then
                            invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
                    override this.OnDispose () = () })

        [<CompiledName("Concat")>]
        let concat (sources:ISeq<#ISeq<'T>>) : ISeq<'T> =
            Upcast.seq (Wrap.ConcatEnumerable (sources, id))

        [<CompiledName "Scan">]
        let inline scan (folder:'State->'T->'State) (initialState:'State) (source:ISeq<'T>) :ISeq<'State> =
            let head = ofSeq [| initialState |]
            let tail = 
                source.PushTransform { new TransformFactory<'T,'State>() with
                    override __.Compose _ _ next =
                        upcast { new Transform<'T,'V,'State>(next, initialState) with
                            override this.ProcessNext (input:'T) : bool =
                                this.State <- folder this.State input
                                TailCall.avoid (next.ProcessNext this.State) } }
            concat (ofSeq [| head ; tail |])

        [<CompiledName "Skip">]
        let skip (skipCount:int) (source:ISeq<'T>) : ISeq<'T> =
            source.PushTransform { new TransformFactory<'T,'T>() with
                override __.Compose _ _ next =
                    let mutable this = Unchecked.defaultof<TransformWithPostProcessing<'T,'U,int>>
                    let skipper = 
                        { new TransformWithPostProcessing<'T,'U,int>(next,(*count*)0) with
                            // member this.count = this.State
                            override this.ProcessNext (input:'T) : bool =
                                if this.State < skipCount then
                                    this.State <- this.State + 1
                                    false
                                else
                                    TailCall.avoid (next.ProcessNext input)

                            override this.OnComplete _ =
                                if this.State < skipCount then
                                    let x = skipCount - this.State
                                    invalidOpFmt "{0}\ntried to skip {1} {2} past the end of the seq"
                                        [|SR.GetString SR.notEnoughElements; x; (if x=1 then "element" else "elements")|]
                            override this.OnDispose () = ()

                        interface ISkipable with
                            member __.CanSkip () =
                                if this.State < skipCount then
                                    this.State <- this.State + 1
                                    true
                                else
                                    false }
                    this <- skipper
                    upcast this }

        [<CompiledName "SkipWhile">]
        let inline skipWhile (predicate:'T->bool) (source:ISeq<'T>) : ISeq<'T> =
            source.PushTransform { new TransformFactory<'T,'T>() with
                override __.Compose _ _ next =
                    upcast { new Transform<'T,'V,bool>(next,true) with
                        // member this.skip = this.State
                        override this.ProcessNext (input:'T) : bool =
                            if this.State then
                                this.State <- predicate input
                                if this.State then
                                    false
                                else
                                    TailCall.avoid (next.ProcessNext input)
                            else
                                TailCall.avoid (next.ProcessNext input) }}

        [<CompiledName "Sum">]
        let inline sum (source:ISeq<'T>) =
            source.Fold (fun _ ->
                upcast { new Folder<'T,'T,NoValue> (LanguagePrimitives.GenericZero,Unchecked.defaultof<NoValue>) with
                    override this.ProcessNext value =
                        this.Result <- Checked.(+) this.Result value
                        Unchecked.defaultof<_> (* return value unused in Fold context *) })

        [<CompiledName "SumBy">]
        let inline sumBy (f:'T->'U) (source:ISeq<'T>) =
            source.Fold (fun _ ->
                upcast { new Folder<'T,'U,NoValue> (LanguagePrimitives.GenericZero<'U>,Unchecked.defaultof<NoValue>) with
                    override this.ProcessNext value =
                        this.Result <- Checked.(+) this.Result (f value)
                        Unchecked.defaultof<_> (* return value unused in Fold context *) })

        [<CompiledName "Take">]
        let take (takeCount:int) (source:ISeq<'T>) : ISeq<'T> =
            source.PushTransform { new TransformFactory<'T,'T>() with
                member __.Compose outOfBand pipelineIdx next =
                    if takeCount = 0 then
                        outOfBand.StopFurtherProcessing pipelineIdx

                    upcast { new TransformWithPostProcessing<'T,'U,int>(next,(*count*)0) with
                        // member this.count = this.State
                        override this.ProcessNext (input:'T) : bool =
                            if this.State < takeCount then
                                this.State <- this.State + 1
                                if this.State = takeCount then
                                    outOfBand.StopFurtherProcessing pipelineIdx
                                TailCall.avoid (next.ProcessNext input)
                            else
                                outOfBand.StopFurtherProcessing pipelineIdx
                                false

                        override this.OnComplete terminatingIdx =
                            if terminatingIdx < pipelineIdx && this.State < takeCount then
                                let x = takeCount - this.State
                                invalidOpFmt "tried to take {0} {1} past the end of the seq"
                                    [|SR.GetString SR.notEnoughElements; x; (if x=1 then "element" else "elements")|]
                        override this.OnDispose () = () }}

        [<CompiledName "TakeWhile">]
        let inline takeWhile (predicate:'T->bool) (source:ISeq<'T>) : ISeq<'T> =
            source.PushTransform { new TransformFactory<'T,'T>() with
                member __.Compose outOfBand pipeIdx next =
                    upcast { new Transform<'T,'V,NoValue>(next,Unchecked.defaultof<NoValue>) with
                        override __.ProcessNext (input:'T) : bool =
                            if predicate input then
                                TailCall.avoid (next.ProcessNext input)
                            else
                                outOfBand.StopFurtherProcessing pipeIdx
                                false }}

        [<CompiledName "Tail">]
        let tail (source:ISeq<'T>) : ISeq<'T> =
            source.PushTransform { new TransformFactory<'T,'T>() with
                member __.Compose _ _ next =
                    upcast { new TransformWithPostProcessing<'T,'V,bool>(next,true) with
                        // member this.isFirst = this.State
                        override this.ProcessNext (input:'T) : bool =
                            if this.State then
                                this.State <- false
                                false
                            else
                                TailCall.avoid (next.ProcessNext input)
                        override this.OnComplete _ =
                            if this.State then
                                invalidArg "source" (SR.GetString SR.notEnoughElements) 
                        override this.OnDispose () = () }}

        [<CompiledName "Truncate">]
        let truncate (truncateCount:int) (source:ISeq<'T>) : ISeq<'T> =
            source.PushTransform { new TransformFactory<'T,'T>() with
                member __.Compose outOfBand pipeIdx next =
                    upcast { new Transform<'T,'U,int>(next,(*count*)0) with
                        // member this.count = this.State
                        override this.ProcessNext (input:'T) : bool =
                            if this.State < truncateCount then
                                this.State <- this.State + 1
                                if this.State = truncateCount then
                                    outOfBand.StopFurtherProcessing pipeIdx
                                TailCall.avoid (next.ProcessNext input)
                            else
                                outOfBand.StopFurtherProcessing pipeIdx
                                false }}

        [<CompiledName "Indexed">]
        let indexed source =
            mapi (fun i x -> i,x) source

        [<CompiledName "TryItem">]
        let tryItem index (source:ISeq<'T>) =
            if index < 0 then None else
                source.PushTransform { new TransformFactory<'T,'T>() with
                    override __.Compose _ _ next =
                        let mutable this = Unchecked.defaultof<Transform<'T,'U,int>>
                        let skipper = 
                            { new Transform<'T,'U,int>(next,(*count*)0) with
                                // member this.count = this.State
                                override this.ProcessNext (input:'T) : bool =
                                    if this.State < index then
                                        this.State <- this.State + 1
                                        false
                                    else
                                        TailCall.avoid (next.ProcessNext input)

                            interface ISkipable with
                                member __.CanSkip () =
                                    if this.State < index then
                                        this.State <- this.State + 1
                                        true
                                    else
                                        false }
                        this <- skipper
                        upcast this }
                |> tryHead

        [<CompiledName "TryPick">]
        let inline tryPick f (source:ISeq<'T>)  =
            source.Fold (fun pipeIdx ->
                upcast { new Folder<'T, Option<'U>,NoValue> (None,Unchecked.defaultof<NoValue>) with
                    override this.ProcessNext value =
                        match f value with
                        | (Some _) as some ->
                            this.Result <- some
                            this.StopFurtherProcessing pipeIdx
                        | None -> ()
                        Unchecked.defaultof<_> (* return value unused in Fold context *) })

        [<CompiledName "TryFind">]
        let inline tryFind f (source:ISeq<'T>)  =
            source.Fold (fun pipeIdx ->
                upcast { new Folder<'T, Option<'T>,NoValue> (None,Unchecked.defaultof<NoValue>) with
                    override this.ProcessNext value =
                        if f value then
                            this.Result <- Some value
                            this.StopFurtherProcessing pipeIdx
                        Unchecked.defaultof<_> (* return value unused in Fold context *) })

        [<CompiledName "TryFindIndex">]
        let inline tryFindIndex (predicate:'T->bool) (source:ISeq<'T>) : int option =
            source.Fold (fun pipeIdx ->
                { new Folder<'T, Option<int>, int>(None, 0) with
                    // member this.index = this.State
                    override this.ProcessNext value =
                        if predicate value then
                            this.Result <- Some this.State
                            this.StopFurtherProcessing pipeIdx
                        else
                            this.State <- this.State + 1
                        Unchecked.defaultof<_> (* return value unused in Fold context *) })

        [<CompiledName "TryLast">]
        let tryLast (source:ISeq<'T>) : 'T option =
            source.Fold (fun _ ->
                upcast { new FolderWithPostProcessing<'T,option<'T>,Values<bool,'T>>(None,Values<bool,'T>(true, Unchecked.defaultof<'T>)) with
                    // member this.noItems = this.State._1
                    // memebr this.last    = this.State._2
                    override this.ProcessNext value =
                        if this.State._1 then
                            this.State._1 <- false
                        this.State._2 <- value
                        Unchecked.defaultof<_> (* return value unused in Fold context *)
                    override this.OnComplete _ =
                        if not this.State._1 then
                            this.Result <- Some this.State._2
                    override this.OnDispose () = () })

        [<CompiledName("Last")>]
        let last (source:ISeq<_>) =
            match tryLast source with
            | None -> invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
            | Some x -> x

        [<CompiledName "Windowed">]
        let windowed (windowSize:int) (source:ISeq<'T>) : ISeq<'T[]> =
            if windowSize <= 0 then
                invalidArgFmt "windowSize" "{0}\nwindowSize = {1}" [|SR.GetString SR.inputMustBePositive; windowSize|]

            source.PushTransform { new TransformFactory<'T,'T[]>() with
                member __.Compose outOfBand pipeIdx next =
                    upcast {
                        new Transform<'T,'U,Values<'T[],int,int>>(next,Values<'T[],int,int>(Array.zeroCreateUnchecked windowSize, 0, windowSize-1)) with
                            override this.ProcessNext (input:'T) : bool =
                                let circularBuffer     =  this.State._1
                                let idx     : byref<_> = &this.State._2
                                let priming : byref<_> = &this.State._3

                                circularBuffer.[idx] <- input

                                idx <- idx + 1
                                if idx = windowSize then
                                    idx <- 0

                                if priming > 0 then
                                    priming <- priming - 1
                                    false
                                elif windowSize < 32 then
                                    let idx = idx
                                    let window :'T [] = Array.init windowSize (fun i -> circularBuffer.[(idx+i) % windowSize]: 'T)
                                    TailCall.avoid (next.ProcessNext window)
                                else
                                    let window = Array.zeroCreateUnchecked windowSize
                                    Array.Copy (circularBuffer, idx, window, 0, windowSize - idx)
                                    Array.Copy (circularBuffer, 0, window, windowSize - idx, idx)
                                    TailCall.avoid (next.ProcessNext window) }}

        [<CompiledName("Append")>]
        let append (source1:ISeq<'T>) (source2: ISeq<'T>) : ISeq<'T> =
            match source1 with
            | :? Wrap.EnumerableBase<'T> as s -> s.Append source2
            | _ -> Upcast.seq (new Wrap.AppendEnumerable<_>([source2; source1]))

        [<CompiledName "Delay">]
        let delay (delayed:unit->ISeq<'T>) =
            Upcast.seq (Wrap.DelayedEnumerable (delayed, 1))

        module internal GroupBy =
            let inline private impl (comparer:IEqualityComparer<'SafeKey>) (keyf:'T->'SafeKey) (getKey:'SafeKey->'Key) (source:ISeq<'T>) =
                source.Fold (fun _ ->
                    upcast { new FolderWithPostProcessing<'T,ISeq<'Key*ISeq<'T>>,_>(Unchecked.defaultof<_>,Dictionary comparer) with
                        override this.ProcessNext v =
                            let safeKey = keyf v
                            match this.State.TryGetValue safeKey with
                            | false, _ ->
                                let prev = ResizeArray ()
                                this.State.[safeKey] <- prev
                                prev.Add v
                            | true, prev -> prev.Add v
                            Unchecked.defaultof<_> (* return value unused in Fold context *)

                        override this.OnComplete _ =
                            let maxWastage = 4
                            for value in this.State.Values do
                                if value.Capacity - value.Count > maxWastage then value.TrimExcess ()

                            this.Result <-
                                this.State
                                |> ofSeq
                                |> map (fun kv -> getKey kv.Key, ofResizeArrayUnchecked kv.Value)

                        override this.OnDispose () = () })

            let inline byVal (keyf:'T->'Key) (source:ISeq<'T>) =
                delay (fun () -> impl HashIdentity.Structural<'Key> keyf id source) 

            let inline byRef (keyf:'T->'Key) (source:ISeq<'T>) =
                delay (fun () -> impl (valueComparer<'Key> ()) (keyf >> Value) (fun v -> v._1) source)
        
        [<CompiledName("GroupByVal")>]
        let inline groupByVal<'T,'Key when 'Key : equality and 'Key : struct> (keyf:'T->'Key) (source:ISeq<'T>) =
            GroupBy.byVal keyf source

        [<CompiledName("GroupByRef")>]
        let inline groupByRef<'T,'Key when 'Key : equality and 'Key : not struct> (keyf:'T->'Key) (source:ISeq<'T>) =
            GroupBy.byRef keyf source

        module CountBy =
            let inline private impl (comparer:IEqualityComparer<'SafeKey>) (keyf:'T->'SafeKey) (getKey:'SafeKey->'Key) (source:ISeq<'T>) =
                source.Fold (fun _ ->
                    upcast { new FolderWithPostProcessing<'T,ISeq<'Key*int>,_>(Unchecked.defaultof<_>,Dictionary comparer) with
                        override this.ProcessNext v =
                            let safeKey = keyf v
                            this.State.[safeKey] <- 
                                match this.State.TryGetValue safeKey with
                                | true, prev -> prev + 1
                                | false, _   -> 1
                            Unchecked.defaultof<_> (* return value unused in Fold context *)

                        override this.OnComplete _ =
                            this.Result <-
                                this.State
                                |> ofSeq
                                |> map (fun group -> getKey group.Key, group.Value)

                        override this.OnDispose () = () })

            let inline byVal (keyf:'T->'Key) (source:ISeq<'T>) =
                delay (fun () -> impl HashIdentity.Structural<'Key> keyf id source) 

            let inline byRef (keyf:'T->'Key) (source:ISeq<'T>) =
                delay (fun () -> impl (valueComparer<'Key> ()) (keyf >> Value) (fun v -> v._1) source)
        
        [<CompiledName("CountByVal")>]
        let inline countByVal<'T,'Key when 'Key : equality and 'Key : struct>  (projection:'T -> 'Key) (source:ISeq<'T>) =
            CountBy.byVal projection source 

        [<CompiledName("CountByRef")>]
        let inline countByRef<'T,'Key when 'Key : equality and 'Key : not struct> (projection:'T -> 'Key) (source:ISeq<'T>) =
            CountBy.byRef projection source 

        [<CompiledName("Length")>]
        let length (source:ISeq<'T>)  =
            match source with
            | :? Wrap.EnumerableBase<'T> as s -> s.Length ()
            | _ -> Wrap.length source

        [<CompiledName("ToArray")>]
        let toArray (source:ISeq<'T>)  =
            source.Fold (fun _ ->
                upcast { new FolderWithPostProcessing<'T,array<'T>,_>(Unchecked.defaultof<_>,ResizeArray ()) with
                    override this.ProcessNext v =
                        this.State.Add v
                        Unchecked.defaultof<_> (* return value unused in Fold context *)
                    override this.OnComplete _ =
                        this.Result <- this.State.ToArray ()
                    override this.OnDispose () = () })

        [<CompiledName("SortBy")>]
        let sortBy keyf source =
            delay (fun () ->
                let array = source |> toArray
                Array.stableSortInPlaceBy keyf array
                ofArray array)

        [<CompiledName("Sort")>]
        let sort source =
            delay (fun () -> 
                let array = source |> toArray
                Array.stableSortInPlace array
                ofArray array)

        [<CompiledName("SortWith")>]
        let sortWith f source =
            delay (fun () ->
                let array = source |> toArray
                Array.stableSortInPlaceWith f array
                ofArray array)

        [<CompiledName("Reverse")>]
        let rev source =
            delay (fun () ->
                let array = source |> toArray
                Array.Reverse array
                ofArray array)

        [<CompiledName("Permute")>]
        let permute f (source:ISeq<_>) =
            delay (fun () ->
                source
                |> toArray
                |> Array.permute f
                |> ofArray)

        [<CompiledName("ScanBack")>]
        let scanBack<'T,'State> f (source:ISeq<'T>) (acc:'State) : ISeq<'State> =
            delay (fun () ->
                let array = source |> toArray
                Array.scanSubRight f array 0 (array.Length - 1) acc
                |> ofArray)

        let inline foldArraySubRight f (arr: 'T[]) start fin acc =
            let mutable state = acc
            for i = fin downto start do
                state <- f arr.[i] state
            state

        [<CompiledName("FoldBack")>]
        let inline foldBack<'T,'State> f (source: ISeq<'T>) (x:'State) =
            let arr = toArray source
            let len = arr.Length
            foldArraySubRight f arr 0 (len - 1) x

        [<CompiledName("Zip")>]
        let zip source1 source2 =
            map2 (fun x y -> x,y) source1 source2

        [<CompiledName("FoldBack2")>]
        let inline foldBack2<'T1,'T2,'State> f (source1:ISeq<'T1>) (source2:ISeq<'T2>) (x:'State) =
            let zipped = zip source1 source2
            foldBack ((<||) f) zipped x

        [<CompiledName("ReduceBack")>]
        let inline reduceBack f (source:ISeq<'T>) =
            let arr = toArray source
            match arr.Length with
            | 0 -> invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
            | len -> foldArraySubRight f arr 0 (len - 2) arr.[len - 1]

        [<Sealed>]
        type CachedSeq<'T>(source:ISeq<'T>) =
            let sync = obj ()

            // Wrap a seq to ensure that it is enumerated just once and only as far as is necessary.
            //
            // This code is required to be thread safe.
            // The necessary calls should be called at most once (include .MoveNext() = false).
            // The enumerator should be disposed (and dropped) when no longer required.
            //------
            // The state is (prefix,enumerator) with invariants:
            //   * the prefix followed by elts from the enumerator are the initial sequence.
            //   * the prefix contains only as many elements as the longest enumeration so far.

            let prefix = ResizeArray ()

            let mutable started = false
            let mutable enumeratorR = None : option<IEnumerator<'T>>
                
            // function should only be called from within the lock
            let oneStepTo i =
                // If possible, step the enumeration to prefix length i (at most one step).
                // Be speculative, since this could have already happened via another thread.
                if not (i < prefix.Count) then // is a step still required?
                    // If not yet started, start it (create enumerator).
                    if not started then
                        started <- true
                        enumeratorR <- Some (source.GetEnumerator())

                    match enumeratorR with
                    | Some enumerator when enumerator.MoveNext() ->
                        prefix.Add enumerator.Current
                    | Some enumerator ->
                        enumerator.Dispose () // Move failed, dispose enumerator,
                        enumeratorR <- None   // drop it and record finished.
                    | _ -> ()

            let unfolding i =
                // i being the next position to be returned
                // A lock is needed over the reads to prefix.Count since the list may be being resized
                // NOTE: we could change to a reader/writer lock here
                lock sync (fun () ->
                    if i < prefix.Count then
                        Some (prefix.[i], i+1)
                    else
                        oneStepTo i
                        if i < prefix.Count then
                            Some (prefix.[i], i+1)
                        else
                            None)

            let cached = Upcast.seq (new Wrap.UnfoldEnumerable<'T,'T,int>(unfolding, 0, IdentityFactory.Instance, 1))

            interface System.IDisposable with
                member __.Dispose() =
                    lock sync (fun () ->
                       prefix.Clear()

                       enumeratorR
                       |> Option.iter IEnumerator.dispose

                       started <- false
                       enumeratorR <- None)

            interface System.Collections.Generic.IEnumerable<'T> with
                member __.GetEnumerator() = cached.GetEnumerator()

            interface System.Collections.IEnumerable with
                member __.GetEnumerator() = (Upcast.enumerableNonGeneric cached).GetEnumerator()

            interface ISeq<'T> with
                member __.PushTransform next = cached.PushTransform next
                member __.Fold f = cached.Fold f

            member this.Clear() = (this :> IDisposable).Dispose ()

        [<CompiledName("Cache")>]
        let cache (source:ISeq<'T>) : ISeq<'T> =
            Upcast.seq (new CachedSeq<_> (source))

        [<CompiledName("Collect")>]
        let collect f sources = map f sources |> concat

        [<CompiledName("AllPairs")>]
        let allPairs (source1:ISeq<'T1>) (source2:ISeq<'T2>) : ISeq<'T1 * 'T2> =
            checkNonNull "source1" source1
            checkNonNull "source2" source2
            let cached = cache source2
            source1 |> collect (fun x -> cached |> map (fun y -> x,y))

        [<CompiledName("ToList")>]
        let toList (source : ISeq<'T>) =
            checkNonNull "source" source
            Microsoft.FSharp.Primitives.Basics.List.ofSeq source

        [<CompiledName("Replicate")>]
        let replicate count x =
            System.Linq.Enumerable.Repeat(x,count) |> ofSeq

        [<CompiledName("IsEmpty")>]
        let isEmpty (source : ISeq<'T>)  =
            use ie = source.GetEnumerator()
            not (ie.MoveNext())

        [<CompiledName("Cast")>]
        let cast (source: IEnumerable) : ISeq<'T> =
            match source with
            | :? ISeq<'T> as s -> s
            | :? ISeq<obj> as s -> s |> map unbox // covariant on ref types
            | _ -> 
                mkSeq (fun () -> IEnumerator.cast (source.GetEnumerator())) |> ofSeq

        [<CompiledName("ChunkBySize")>]
        let chunkBySize chunkSize (source : ISeq<'T>) : ISeq<'T[]> =
            if chunkSize <= 0 then invalidArgFmt "chunkSize" "{0}\nchunkSize = {1}"
                                    [|SR.GetString SR.inputMustBePositive; chunkSize|]

            source.PushTransform { new TransformFactory<'T,'T[]>() with
                member __.Compose outOfBand pipeIdx next =
                    upcast {
                        new TransformWithPostProcessing<'T,'U,Values<'T[],int>>(next,Values<'T[],int>(Array.zeroCreateUnchecked chunkSize, 0)) with
                            override this.ProcessNext (input:'T) : bool =
                                this.State._1.[this.State._2] <- input
                                this.State._2 <- this.State._2 + 1
                                if this.State._2 <> chunkSize then false
                                else
                                    this.State._2 <- 0
                                    let tmp = this.State._1
                                    this.State._1 <- Array.zeroCreateUnchecked chunkSize
                                    TailCall.avoid (next.ProcessNext tmp)
                            override this.OnComplete _ =
                                if this.State._2 > 0 then
                                    System.Array.Resize (&this.State._1, this.State._2)
                                    next.ProcessNext this.State._1 |> ignore
                            override this.OnDispose () = () }}

        let mkDelayedSeq (f: unit -> IEnumerable<'T>) = mkSeq (fun () -> f().GetEnumerator()) |> ofSeq

        [<CompiledName("SplitInto")>]
        let splitInto count (source:ISeq<'T>) : ISeq<'T[]> =
            if count <= 0 then invalidArgFmt "count" "{0}\ncount = {1}"
                                [|SR.GetString SR.inputMustBePositive; count|]
            mkDelayedSeq (fun () ->
                source |> toArray |> Array.splitInto count :> seq<_>)

        let inline indexNotFound() = raise (new System.Collections.Generic.KeyNotFoundException(SR.GetString(SR.keyNotFoundAlt)))

        [<CompiledName("Find")>]
        let find f source =
            match tryFind f source with
            | None -> indexNotFound()
            | Some x -> x

        [<CompiledName("FindIndex")>]
        let findIndex p (source:ISeq<_>) =
            use ie = source.GetEnumerator()
            let rec loop i =
                if ie.MoveNext() then
                    if p ie.Current then
                        i
                    else loop (i+1)
                else
                    indexNotFound()
            loop 0

        [<CompiledName("FindBack")>]
        let findBack f source =
            source |> toArray |> Array.findBack f

        [<CompiledName("FindIndexBack")>]
        let findIndexBack f source =
            source |> toArray |> Array.findIndexBack f

        [<CompiledName("Pick")>]
        let pick f source  =
            match tryPick f source with
            | None -> indexNotFound()
            | Some x -> x

        [<CompiledName("MapFold")>]
        let mapFold<'T,'State,'Result> (f: 'State -> 'T -> 'Result * 'State) acc source =
            let arr,state = source |> toArray |> Array.mapFold f acc
            ofArray arr, state

        [<CompiledName("MapFoldBack")>]
        let mapFoldBack<'T,'State,'Result> (f: 'T -> 'State -> 'Result * 'State) source acc =
            let array = source |> toArray
            let arr,state = Array.mapFoldBack f array acc
            ofArray arr, state

        let rec nth index (e : IEnumerator<'T>) =
            if not (e.MoveNext()) then
              let shortBy = index + 1
              invalidArgFmt "index"
                "{0}\nseq was short by {1} {2}"
                [|SR.GetString SR.notEnoughElements; shortBy; (if shortBy = 1 then "element" else "elements")|]
            if index = 0 then e.Current
            else nth (index-1) e

        [<CompiledName("Item")>]
        let item i (source : ISeq<'T>) =
            if i < 0 then invalidArgInputMustBeNonNegative "index" i
            use e = source.GetEnumerator()
            nth i e

        [<CompiledName("Singleton")>]
        let singleton x = mkSeq (fun () -> IEnumerator.Singleton x) |> ofSeq

        [<CompiledName("SortDescending")>]
        let inline sortDescending source =
            let inline compareDescending a b = compare b a
            sortWith compareDescending source

        [<CompiledName("SortByDescending")>]
        let inline sortByDescending keyf source =
            let inline compareDescending a b = compare (keyf b) (keyf a)
            sortWith compareDescending source

        [<CompiledName("TryFindBack")>]
        let tryFindBack f (source : ISeq<'T>) =
            source |> toArray |> Array.tryFindBack f

        [<CompiledName("TryFindIndexBack")>]
        let tryFindIndexBack f (source : ISeq<'T>) =
            source |> toArray |> Array.tryFindIndexBack f

        [<CompiledName("Zip3")>]
        let zip3 source1 source2  source3 =
            map2 (fun x (y,z) -> x,y,z) source1 (zip source2 source3)
