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
    module Composer =
        open IEnumerator

        module Core =
            [<Struct; NoComparison; NoEquality>]
            type NoValue = struct end

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

            type ICompletionChain =
                abstract ChainComplete : stopTailCall:byref<unit> * PipeIdx -> unit
                abstract ChainDispose  : stopTailCall:byref<unit> -> unit

            [<AbstractClass>]
            type Consumer<'T,'U> () =
                abstract ProcessNext : input:'T -> bool

                interface ICompletionChain with
                    member this.ChainComplete (_,_) = ()
                    member this.ChainDispose _ = ()

            [<AbstractClass>]
            type ConsumerWithState<'T,'U,'Value> =
                inherit Consumer<'T,'U>

                val mutable Value : 'Value

                new (init) = {
                    Value = init
                }

            [<AbstractClass>]
            type ConsumerChainedWithState<'T,'U,'Value> =
                inherit ConsumerWithState<'T,'U,'Value>

                val private Next : ICompletionChain

                new (next:ICompletionChain, init) = {
                    inherit ConsumerWithState<'T,'U,'Value> (init)
                    Next = next
                }

                interface ICompletionChain with
                    member this.ChainComplete (stopTailCall, terminatingIdx) =
                        this.Next.ChainComplete (&stopTailCall, terminatingIdx)
                    member this.ChainDispose stopTailCall =
                        this.Next.ChainDispose (&stopTailCall)

            [<AbstractClass>]
            type ConsumerChained<'T,'U>(next:ICompletionChain) =
                inherit ConsumerChainedWithState<'T,'U,NoValue>(next, Unchecked.defaultof<NoValue>)

            [<AbstractClass>]
            type ConsumerChainedWithStateAndCleanup<'T,'U,'Value> (next, init) =
                inherit ConsumerChainedWithState<'T,'U,'Value>(next, init)

                abstract OnComplete : PipeIdx -> unit
                abstract OnDispose  : unit -> unit

                interface ICompletionChain with
                    member this.ChainComplete (stopTailCall, terminatingIdx) =
                        this.OnComplete terminatingIdx
                        next.ChainComplete (&stopTailCall, terminatingIdx)
                    member this.ChainDispose stopTailCall  =
                        try     this.OnDispose ()
                        finally next.ChainDispose (&stopTailCall)

            [<AbstractClass>]
            type ConsumerChainedWithCleanup<'T,'U>(next:ICompletionChain) =
                inherit ConsumerChainedWithStateAndCleanup<'T,'U,NoValue>(next, Unchecked.defaultof<NoValue>)

            [<AbstractClass>]
            type Folder<'T,'U>(init) =
                inherit ConsumerWithState<'T,'T,'U>(init)

            [<AbstractClass>]
            type FolderWithOnComplete<'T, 'U>(init) =
                inherit Folder<'T,'U>(init)

                abstract OnComplete : PipeIdx -> unit

                interface ICompletionChain with
                    member this.ChainComplete (stopTailCall, terminatingIdx) =
                        this.OnComplete terminatingIdx
                    member this.ChainDispose _ = ()

            [<AbstractClass>]
            type SeqFactory<'T,'U> () =
                abstract PipeIdx : PipeIdx
                abstract Create<'V> : IOutOfBand -> PipeIdx -> Consumer<'U,'V> -> Consumer<'T,'V>

                default __.PipeIdx = 1

                member this.Build outOfBand next = this.Create outOfBand 1 next

            type ISeq<'T> =
                inherit IEnumerable<'T>
                abstract member Compose<'U> : (SeqFactory<'T,'U>) -> ISeq<'U>
                abstract member ForEach<'consumer when 'consumer :> Consumer<'T,'T>> : f:((unit->unit)->'consumer) -> 'consumer

        open Core

        module internal TailCall =
            // used for performance reasons; these are not recursive calls, so should be safe
            // ** it should be noted that potential changes to the f# compiler may render this function
            // ineffictive **
            let inline avoid boolean = match boolean with true -> true | false -> false

        module internal Upcast =
            // The f# compiler outputs unnecessary unbox.any calls in upcasts. If this functionality
            // is fixed with the compiler then these functions can be removed.
            let inline seq (t:#ISeq<'T>) : ISeq<'T> = (# "" t : ISeq<'T> #)
            let inline enumerable (t:#IEnumerable<'T>) : IEnumerable<'T> = (# "" t : IEnumerable<'T> #)
            let inline enumerator (t:#IEnumerator<'T>) : IEnumerator<'T> = (# "" t : IEnumerator<'T> #)
            let inline enumeratorNonGeneric (t:#IEnumerator) : IEnumerator = (# "" t : IEnumerator #)
            let inline iCompletionChain (t:#ICompletionChain) : ICompletionChain = (# "" t : ICompletionChain #)

        module internal Seq =
            type ComposedFactory<'T,'U,'V> private (first:SeqFactory<'T,'U>, second:SeqFactory<'U,'V>, secondPipeIdx:PipeIdx) =
                inherit SeqFactory<'T,'V>()

                override __.PipeIdx =
                    secondPipeIdx

                override this.Create<'W> (outOfBand:IOutOfBand) (pipeIdx:PipeIdx) (next:Consumer<'V,'W>) : Consumer<'T,'W> =
                    first.Create outOfBand pipeIdx (second.Create outOfBand secondPipeIdx next)

                static member Combine (first:SeqFactory<'T,'U>) (second:SeqFactory<'U,'V>) : SeqFactory<'T,'V> =
                    upcast ComposedFactory(first, second, first.PipeIdx+1)

            and IdentityFactory<'T> () =
                inherit SeqFactory<'T,'T> ()
                static let singleton : SeqFactory<'T,'T> = upcast (IdentityFactory<'T>())
                override __.Create<'V> (_outOfBand:IOutOfBand) (_pipeIdx:PipeIdx) (next:Consumer<'T,'V>) : Consumer<'T,'V> = next
                static member Instance = singleton

            and Map2FirstFactory<'First,'Second,'U> (map:'First->'Second->'U, input2:IEnumerable<'Second>) =
                inherit SeqFactory<'First,'U> ()
                override this.Create<'V> (outOfBand:IOutOfBand) (pipeIdx:PipeIdx) (next:Consumer<'U,'V>) : Consumer<'First,'V> = upcast Map2First (map, input2, outOfBand, next, pipeIdx)

            and Map2SecondFactory<'First,'Second,'U> (map:'First->'Second->'U, input1:IEnumerable<'First>) =
                inherit SeqFactory<'Second,'U> ()
                override this.Create<'V> (outOfBand:IOutOfBand) (pipeIdx:PipeIdx) (next:Consumer<'U,'V>) : Consumer<'Second,'V> = upcast Map2Second (map, input1, outOfBand, next, pipeIdx)

            and Map3Factory<'First,'Second,'Third,'U> (map:'First->'Second->'Third->'U, input2:IEnumerable<'Second>, input3:IEnumerable<'Third>) =
                inherit SeqFactory<'First,'U> ()
                override this.Create<'V> (outOfBand:IOutOfBand) (pipeIdx:PipeIdx) (next:Consumer<'U,'V>) : Consumer<'First,'V> = upcast Map3 (map, input2, input3, outOfBand, next, pipeIdx)

            and Mapi2Factory<'First,'Second,'U> (map:int->'First->'Second->'U, input2:IEnumerable<'Second>) =
                inherit SeqFactory<'First,'U> ()
                override this.Create<'V> (outOfBand:IOutOfBand) (pipeIdx:PipeIdx) (next:Consumer<'U,'V>) : Consumer<'First,'V> = upcast Mapi2 (map, input2, outOfBand, next, pipeIdx)

            and ISkipping =
                // Seq.init(Infinite)? lazily uses Current. The only Composer component that can do that is Skip
                // and it can only do it at the start of a sequence
                abstract Skipping : unit -> bool

            and Map2First<'First,'Second,'U,'V> (map:'First->'Second->'U, enumerable2:IEnumerable<'Second>, outOfBand:IOutOfBand, next:Consumer<'U,'V>, pipeIdx:int) =
                inherit ConsumerChainedWithCleanup<'First,'V>(Upcast.iCompletionChain next)

                let input2 = enumerable2.GetEnumerator ()
                let map' = OptimizedClosures.FSharpFunc<_,_,_>.Adapt map

                override __.ProcessNext (input:'First) : bool =
                    if input2.MoveNext () then
                        TailCall.avoid (next.ProcessNext (map'.Invoke (input, input2.Current)))
                    else
                        outOfBand.StopFurtherProcessing pipeIdx
                        false

                override __.OnComplete _ = ()
                override __.OnDispose () =
                    input2.Dispose ()

            and Map2Second<'First,'Second,'U,'V> (map:'First->'Second->'U, enumerable1:IEnumerable<'First>, outOfBand:IOutOfBand, next:Consumer<'U,'V>, pipeIdx:int) =
                inherit ConsumerChainedWithCleanup<'Second,'V>(Upcast.iCompletionChain next)

                let input1 = enumerable1.GetEnumerator ()
                let map' = OptimizedClosures.FSharpFunc<_,_,_>.Adapt map

                override __.ProcessNext (input:'Second) : bool =
                    if input1.MoveNext () then
                        TailCall.avoid (next.ProcessNext (map'.Invoke (input1.Current, input)))
                    else
                        outOfBand.StopFurtherProcessing pipeIdx
                        false

                override __.OnComplete _ = ()
                override __.OnDispose () =
                    input1.Dispose ()

            and Map3<'First,'Second,'Third,'U,'V> (map:'First->'Second->'Third->'U, enumerable2:IEnumerable<'Second>, enumerable3:IEnumerable<'Third>, outOfBand:IOutOfBand, next:Consumer<'U,'V>, pipeIdx:int) =
                inherit ConsumerChainedWithCleanup<'First,'V>(Upcast.iCompletionChain next)

                let input2 = enumerable2.GetEnumerator ()
                let input3 = enumerable3.GetEnumerator ()
                let map' = OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt map

                override __.ProcessNext (input:'First) : bool =
                    if input2.MoveNext () && input3.MoveNext () then
                        TailCall.avoid (next.ProcessNext (map'.Invoke (input, input2.Current, input3.Current)))
                    else
                        outOfBand.StopFurtherProcessing pipeIdx
                        false

                override __.OnComplete _ = ()
                override __.OnDispose () =
                    try     input2.Dispose ()
                    finally input3.Dispose ()

            and Mapi2<'First,'Second,'U,'V> (map:int->'First->'Second->'U, enumerable2:IEnumerable<'Second>, outOfBand:IOutOfBand, next:Consumer<'U,'V>, pipeIdx:int) =
                inherit ConsumerChainedWithCleanup<'First,'V>(Upcast.iCompletionChain next)

                let mutable idx = 0
                let input2 = enumerable2.GetEnumerator ()
                let mapi2' = OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt map

                override __.ProcessNext (input:'First) : bool =
                    if input2.MoveNext () then
                        idx <- idx + 1
                        TailCall.avoid (next.ProcessNext (mapi2'.Invoke (idx-1, input, input2.Current)))
                    else
                        outOfBand.StopFurtherProcessing pipeIdx
                        false

                override __.OnComplete _ = ()
                override __.OnDispose () =
                    input2.Dispose ()

            type SeqProcessNextStates =
            | InProcess  = 0
            | NotStarted = 1
            | Finished   = 2

            type Result<'T>() =
                let mutable haltedIdx = 0

                member val Current = Unchecked.defaultof<'T> with get, set
                member val SeqState = SeqProcessNextStates.NotStarted with get, set
                member __.HaltedIdx = haltedIdx

                interface IOutOfBand with
                    member __.StopFurtherProcessing pipeIdx = haltedIdx <- pipeIdx

            // SetResult<> is used at the end of the chain of SeqComponents to assign the final value
            type SetResult<'T> (result:Result<'T>) =
                inherit Consumer<'T,'T>()

                override __.ProcessNext (input:'T) : bool =
                    result.Current <- input
                    true

            type OutOfBand() =
                let mutable haltedIdx = 0
                interface IOutOfBand with member __.StopFurtherProcessing pipeIdx = haltedIdx <- pipeIdx
                member __.HaltedIdx = haltedIdx

            module ForEach =
                let enumerable (enumerable:IEnumerable<'T>) (outOfBand:OutOfBand) (consumer:Consumer<'T,'U>) =
                    use enumerator = enumerable.GetEnumerator ()
                    while (outOfBand.HaltedIdx = 0) && (enumerator.MoveNext ()) do
                        consumer.ProcessNext enumerator.Current |> ignore

                let array (array:array<'T>) (outOfBand:OutOfBand) (consumer:Consumer<'T,'U>) =
                    let mutable idx = 0
                    while (outOfBand.HaltedIdx = 0) && (idx < array.Length) do
                        consumer.ProcessNext array.[idx] |> ignore
                        idx <- idx + 1

                let list (alist:list<'T>) (outOfBand:OutOfBand) (consumer:Consumer<'T,'U>) =
                    let rec iterate lst =
                        match outOfBand.HaltedIdx, lst with
                        | 0, hd :: tl ->
                            consumer.ProcessNext hd |> ignore
                            iterate tl
                        | _ -> ()
                    iterate alist

                let unfold (generator:'S->option<'T*'S>) state (outOfBand:OutOfBand) (consumer:Consumer<'T,'U>) =
                    let rec iterate current =
                        match outOfBand.HaltedIdx, generator current with
                        | 0, Some (item, next) ->
                            consumer.ProcessNext item |> ignore
                            iterate next
                        | _ -> ()

                    iterate state

                let makeIsSkipping (consumer:Consumer<'T,'U>) =
                    match box consumer with
                    | :? ISkipping as skip -> skip.Skipping
                    | _ -> fun () -> false

                let init f (terminatingIdx:int) (outOfBand:OutOfBand) (consumer:Consumer<'T,'U>) =
                    let mutable idx = -1
                    let isSkipping = makeIsSkipping consumer
                    let mutable maybeSkipping = true
                    while (outOfBand.HaltedIdx = 0) && (idx < terminatingIdx) do
                        if maybeSkipping then
                            maybeSkipping <- isSkipping ()

                        if not maybeSkipping then
                            consumer.ProcessNext (f (idx+1)) |> ignore

                        idx <- idx + 1

                let execute (f:(unit->unit)->#Consumer<'U,'U>) (current:SeqFactory<'T,'U>) executeOn =
                    let pipeline = OutOfBand()
                    let result = f (fun () -> (pipeline:>IOutOfBand).StopFurtherProcessing (current.PipeIdx+1))
                    let consumer = current.Build pipeline result
                    try
                        executeOn pipeline consumer
                        let mutable stopTailCall = ()
                        (Upcast.iCompletionChain consumer).ChainComplete (&stopTailCall, pipeline.HaltedIdx)
                        result
                    finally
                        let mutable stopTailCall = ()
                        (Upcast.iCompletionChain consumer).ChainDispose (&stopTailCall)

            module Enumerable =
                type Empty<'T>() =
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
                    static let element : IEnumerator<'T> = upcast (new Empty<'T> ())
                    static member Element = element

                [<AbstractClass>]
                type EnumeratorBase<'T>(result:Result<'T>, seqComponent:ICompletionChain) =
                    interface IDisposable with
                        member __.Dispose() : unit =
                            let mutable stopTailCall = ()
                            seqComponent.ChainDispose (&stopTailCall)

                    interface IEnumerator with
                        member this.Current : obj = box ((Upcast.enumerator this)).Current
                        member __.MoveNext () = failwith "library implementation error: derived class should implement (should be abstract)"
                        member __.Reset () : unit = noReset ()

                    interface IEnumerator<'T> with
                        member __.Current =
                            if result.SeqState = SeqProcessNextStates.InProcess then result.Current
                            else
                                match result.SeqState with
                                | SeqProcessNextStates.NotStarted -> notStarted()
                                | SeqProcessNextStates.Finished -> alreadyFinished()
                                | _ -> failwith "library implementation error: all states should have been handled"

                and [<AbstractClass>] EnumerableBase<'T> () =
                    let derivedClassShouldImplement () =
                        failwith "library implementation error: derived class should implement (should be abstract)"

                    abstract member Append   : (seq<'T>) -> IEnumerable<'T>

                    default this.Append source = Upcast.enumerable (AppendEnumerable [this; source])

                    interface IEnumerable with
                        member this.GetEnumerator () : IEnumerator =
                            let genericEnumerable = Upcast.enumerable this
                            let genericEnumerator = genericEnumerable.GetEnumerator ()
                            Upcast.enumeratorNonGeneric genericEnumerator

                    interface IEnumerable<'T> with
                        member this.GetEnumerator () : IEnumerator<'T> = derivedClassShouldImplement ()

                    interface ISeq<'T> with
                        member __.Compose _ = derivedClassShouldImplement ()
                        member __.ForEach _ = derivedClassShouldImplement ()

                and Enumerator<'T,'U>(source:IEnumerator<'T>, seqComponent:Consumer<'T,'U>, result:Result<'U>) =
                    inherit EnumeratorBase<'U>(result, seqComponent)

                    let rec moveNext () =
                        if (result.HaltedIdx = 0) && source.MoveNext () then
                            if seqComponent.ProcessNext source.Current then
                                true
                            else
                                moveNext ()
                        else
                            result.SeqState <- SeqProcessNextStates.Finished
                            let mutable stopTailCall = ()
                            (Upcast.iCompletionChain seqComponent).ChainComplete (&stopTailCall, result.HaltedIdx)
                            false

                    interface IEnumerator with
                        member __.MoveNext () =
                            result.SeqState <- SeqProcessNextStates.InProcess
                            moveNext ()

                    interface IDisposable with
                        member __.Dispose() =
                            try
                                source.Dispose ()
                            finally
                                let mutable stopTailCall = ()
                                (Upcast.iCompletionChain seqComponent).ChainDispose (&stopTailCall)

                and Enumerable<'T,'U>(enumerable:IEnumerable<'T>, current:SeqFactory<'T,'U>) =
                    inherit EnumerableBase<'U>()

                    interface IEnumerable<'U> with
                        member this.GetEnumerator () : IEnumerator<'U> =
                            let result = Result<'U> ()
                            Upcast.enumerator (new Enumerator<'T,'U>(enumerable.GetEnumerator(), current.Build result (SetResult<'U> result), result))

                    interface ISeq<'U> with
                        member __.Compose (next:SeqFactory<'U,'V>) : ISeq<'V> =
                            Upcast.seq (new Enumerable<'T,'V>(enumerable, ComposedFactory.Combine current next))

                        member this.ForEach (f:(unit->unit)->#Consumer<'U,'U>) =
                            ForEach.execute f current (ForEach.enumerable enumerable)

                and ConcatEnumerator<'T, 'Collection when 'Collection :> seq<'T>> (sources:seq<'Collection>) =
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
                        member __.Dispose() =
                            main.Dispose ()
                            active.Dispose ()

                and AppendEnumerable<'T> (sources:list<seq<'T>>) =
                    inherit EnumerableBase<'T>()

                    interface IEnumerable<'T> with
                        member this.GetEnumerator () : IEnumerator<'T> =
                            Upcast.enumerator (new ConcatEnumerator<_,_> (sources |> List.rev))

                    override this.Append source =
                        Upcast.enumerable (AppendEnumerable (source :: sources))

                    interface ISeq<'T> with
                        member this.Compose (next:SeqFactory<'T,'U>) : ISeq<'U> =
                            Upcast.seq (Enumerable<'T,'V>(this, next))

                        member this.ForEach (f:(unit->unit)->#Consumer<'T,'T>) =
                            ForEach.execute f IdentityFactory.Instance (ForEach.enumerable this)

                and ConcatEnumerable<'T, 'Collection when 'Collection :> seq<'T>> (sources:seq<'Collection>) =
                    inherit EnumerableBase<'T>()

                    interface IEnumerable<'T> with
                        member this.GetEnumerator () : IEnumerator<'T> =
                            Upcast.enumerator (new ConcatEnumerator<_,_> (sources))

                    interface ISeq<'T> with
                        member this.Compose (next:SeqFactory<'T,'U>) : ISeq<'U> =
                            Upcast.seq (Enumerable<'T,'V>(this, next))

                        member this.ForEach (f:(unit->unit)->#Consumer<'T,'T>) =
                            ForEach.execute f IdentityFactory.Instance (ForEach.enumerable this)

                let create enumerable current =
                    Upcast.seq (Enumerable(enumerable, current))

            module EmptyEnumerable =
                type Enumerable<'T> () =
                    inherit Enumerable.EnumerableBase<'T>()

                    static let singleton = Enumerable<'T>() :> ISeq<'T>
                    static member Instance = singleton

                    interface IEnumerable<'T> with
                        member this.GetEnumerator () : IEnumerator<'T> = IEnumerator.Empty<'T>()

                    override this.Append source =
                        Upcast.enumerable (Enumerable.Enumerable<'T,'T> (source, IdentityFactory.Instance))

                    interface ISeq<'T> with
                        member this.Compose (next:SeqFactory<'T,'U>) : ISeq<'U> =
                            Upcast.seq (Enumerable.Enumerable<'T,'V>(this, next))

                        member this.ForEach (f:(unit->unit)->#Consumer<'T,'T>) =
                            ForEach.execute f IdentityFactory.Instance (ForEach.enumerable this)



            module Array =
                type Enumerator<'T,'U>(delayedArray:unit->array<'T>, seqComponent:Consumer<'T,'U>, result:Result<'U>) =
                    inherit Enumerable.EnumeratorBase<'U>(result, seqComponent)

                    let mutable idx = 0
                    let mutable array = Unchecked.defaultof<_>

                    let mutable initMoveNext = Unchecked.defaultof<_>
                    do
                        initMoveNext <-
                            fun () ->
                                result.SeqState <- SeqProcessNextStates.InProcess
                                array <- delayedArray ()
                                initMoveNext <- ignore

                    let rec moveNext () =
                        if (result.HaltedIdx = 0) && idx < array.Length then
                            idx <- idx+1
                            if seqComponent.ProcessNext array.[idx-1] then
                                true
                            else
                                moveNext ()
                        else
                            result.SeqState <- SeqProcessNextStates.Finished
                            let mutable stopTailCall = ()
                            (Upcast.iCompletionChain seqComponent).ChainComplete (&stopTailCall, result.HaltedIdx)
                            false

                    interface IEnumerator with
                        member __.MoveNext () =
                            initMoveNext ()
                            moveNext ()

                type Enumerable<'T,'U>(delayedArray:unit->array<'T>, current:SeqFactory<'T,'U>) =
                    inherit Enumerable.EnumerableBase<'U>()

                    interface IEnumerable<'U> with
                        member this.GetEnumerator () : IEnumerator<'U> =
                            let result = Result<'U> ()
                            Upcast.enumerator (new Enumerator<'T,'U>(delayedArray, current.Build result (SetResult<'U> result), result))

                    interface ISeq<'U> with
                        member __.Compose (next:SeqFactory<'U,'V>) : ISeq<'V> =
                            Upcast.seq (new Enumerable<'T,'V>(delayedArray, ComposedFactory.Combine current next))

                        member this.ForEach (f:(unit->unit)->#Consumer<'U,'U>) =
                            ForEach.execute f current (ForEach.array (delayedArray ()))

                let createDelayed (delayedArray:unit->array<'T>) (current:SeqFactory<'T,'U>) =
                    Upcast.seq (Enumerable(delayedArray, current))

                let create (array:array<'T>) (current:SeqFactory<'T,'U>) =
                    createDelayed (fun () -> array) current

                let createDelayedId (delayedArray:unit -> array<'T>) =
                    createDelayed delayedArray IdentityFactory.Instance

                let createId (array:array<'T>) =
                    create array IdentityFactory.Instance

            module List =
                type Enumerator<'T,'U>(alist:list<'T>, seqComponent:Consumer<'T,'U>, result:Result<'U>) =
                    inherit Enumerable.EnumeratorBase<'U>(result, seqComponent)

                    let mutable list = alist

                    let rec moveNext current =
                        match result.HaltedIdx, current with
                        | 0, head::tail ->
                            if seqComponent.ProcessNext head then
                                list <- tail
                                true
                            else
                                moveNext tail
                        | _ ->
                            result.SeqState <- SeqProcessNextStates.Finished
                            let mutable stopTailCall = ()
                            (Upcast.iCompletionChain seqComponent).ChainComplete (&stopTailCall, result.HaltedIdx)
                            false

                    interface IEnumerator with
                        member __.MoveNext () =
                            result.SeqState <- SeqProcessNextStates.InProcess
                            moveNext list

                type Enumerable<'T,'U>(alist:list<'T>, current:SeqFactory<'T,'U>) =
                    inherit Enumerable.EnumerableBase<'U>()

                    interface IEnumerable<'U> with
                        member this.GetEnumerator () : IEnumerator<'U> =
                            let result = Result<'U> ()
                            Upcast.enumerator (new Enumerator<'T,'U>(alist, current.Build result (SetResult<'U> result), result))

                    interface ISeq<'U> with
                        member __.Compose (next:SeqFactory<'U,'V>) : ISeq<'V> =
                            Upcast.seq (new Enumerable<'T,'V>(alist, ComposedFactory.Combine current next))

                        member this.ForEach (f:(unit->unit)->#Consumer<'U,'U>) =
                            ForEach.execute f current (ForEach.list alist)

                let create alist current =
                    Upcast.seq (Enumerable(alist, current))

            module Unfold =
                type Enumerator<'T,'U,'State>(generator:'State->option<'T*'State>, state:'State, seqComponent:Consumer<'T,'U>, result:Result<'U>) =
                    inherit Enumerable.EnumeratorBase<'U>(result, seqComponent)

                    let mutable current = state

                    let rec moveNext () =
                        match result.HaltedIdx, generator current with
                        | 0, Some (item, nextState) ->
                            current <- nextState
                            if seqComponent.ProcessNext item then
                                true
                            else
                                moveNext ()
                        | _ -> false

                    interface IEnumerator with
                        member __.MoveNext () =
                            result.SeqState <- SeqProcessNextStates.InProcess
                            moveNext ()

                type Enumerable<'T,'U,'GeneratorState>(generator:'GeneratorState->option<'T*'GeneratorState>, state:'GeneratorState, current:SeqFactory<'T,'U>) =
                    inherit Enumerable.EnumerableBase<'U>()

                    interface IEnumerable<'U> with
                        member this.GetEnumerator () : IEnumerator<'U> =
                            let result = Result<'U> ()
                            Upcast.enumerator (new Enumerator<'T,'U,'GeneratorState>(generator, state, current.Build result (SetResult<'U> result), result))

                    interface ISeq<'U> with
                        member this.Compose (next:SeqFactory<'U,'V>) : ISeq<'V> =
                            Upcast.seq (new Enumerable<'T,'V,'GeneratorState>(generator, state, ComposedFactory.Combine current next))

                        member this.ForEach (f:(unit->unit)->#Consumer<'U,'U>) =
                            ForEach.execute f current (ForEach.unfold generator state)

            module Init =
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

                let getTerminatingIdx (count:Nullable<int>) =
                    // we are offset by 1 to allow for values going up to System.Int32.MaxValue
                    // System.Int32.MaxValue is an illegal value for the "infinite" sequence
                    if count.HasValue then
                        count.Value - 1
                    else
                        System.Int32.MaxValue

                type Enumerator<'T,'U>(count:Nullable<int>, f:int->'T, seqComponent:Consumer<'T,'U>, result:Result<'U>) =
                    inherit Enumerable.EnumeratorBase<'U>(result, seqComponent)

                    let isSkipping =
                        ForEach.makeIsSkipping seqComponent

                    let terminatingIdx =
                        getTerminatingIdx count

                    let mutable maybeSkipping = true
                    let mutable idx = -1

                    let rec moveNext () =
                        if (result.HaltedIdx = 0) && idx < terminatingIdx then
                            idx <- idx + 1

                            if maybeSkipping then
                                // Skip can only is only checked at the start of the sequence, so once
                                // triggered, we stay triggered.
                                maybeSkipping <- isSkipping ()

                            if maybeSkipping then
                                moveNext ()
                            elif seqComponent.ProcessNext (f idx) then
                                true
                            else
                                moveNext ()
                        elif (result.HaltedIdx = 0) && idx = System.Int32.MaxValue then
                            raise <| System.InvalidOperationException (SR.GetString(SR.enumerationPastIntMaxValue))
                        else
                            result.SeqState <- SeqProcessNextStates.Finished
                            let mutable stopTailCall = ()
                            (Upcast.iCompletionChain seqComponent).ChainComplete (&stopTailCall, result.HaltedIdx)
                            false

                    interface IEnumerator with
                        member __.MoveNext () =
                            result.SeqState <- SeqProcessNextStates.InProcess
                            moveNext ()

                type Enumerable<'T,'U>(count:Nullable<int>, f:int->'T, current:SeqFactory<'T,'U>) =
                    inherit Enumerable.EnumerableBase<'U>()

                    interface IEnumerable<'U> with
                        member this.GetEnumerator () : IEnumerator<'U> =
                            let result = Result<'U> ()
                            Upcast.enumerator (new Enumerator<'T,'U>(count, f, current.Build result (SetResult<'U> result), result))

                    interface ISeq<'U> with
                        member this.Compose (next:SeqFactory<'U,'V>) : ISeq<'V> =
                            Upcast.seq (new Enumerable<'T,'V>(count, f, ComposedFactory.Combine current next))

                        member this.ForEach (createResult:(unit->unit)->#Consumer<'U,'U>) =
                            let terminatingIdx = getTerminatingIdx count
                            ForEach.execute createResult current (ForEach.init f terminatingIdx)

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
                                member self.Reset() = noReset()
                            interface System.IDisposable with
                                member x.Dispose() = () }

                type EnumerableDecider<'T>(count:Nullable<int>, f:int->'T) =
                    inherit Enumerable.EnumerableBase<'T>()

                    interface IEnumerable<'T> with
                        member this.GetEnumerator () : IEnumerator<'T> =
                            // we defer back to the original implementation as, as it's quite idiomatic in it's decision
                            // to calculate Current in a lazy fashion. I doubt anyone is really using this functionality
                            // in the way presented, but it's possible.
                            upto (if count.HasValue then Some (count.Value-1) else None) f

                    interface ISeq<'T> with
                        member this.Compose (next:SeqFactory<'T,'U>) : ISeq<'U> =
                            Upcast.seq (Enumerable<'T,'V>(count, f, next))

                        member this.ForEach (f:(unit->unit)->#Consumer<'T,'T>) =
                            ForEach.execute f IdentityFactory.Instance (ForEach.enumerable (Upcast.enumerable this))

            [<CompiledName "ToComposer">]
            let toComposer (source:seq<'T>) : ISeq<'T> =
                match source with
                | :? ISeq<'T> as s -> s
                | :? array<'T> as a -> Upcast.seq (Array.Enumerable((fun () -> a), IdentityFactory.Instance))
                | :? list<'T> as a -> Upcast.seq (List.Enumerable(a, IdentityFactory.Instance))
                | null -> nullArg "source"
                | _ -> Upcast.seq (Enumerable.Enumerable<'T,'T>(source, IdentityFactory.Instance))

            let inline foreach f (source:ISeq<_>) =
                source.ForEach f

            let inline compose (factory:#SeqFactory<_,_>) (source:ISeq<'T>) =
                source.Compose factory

            [<CompiledName "Average">]
            let inline average (source: ISeq< ^T>) : ^T
                when ^T:(static member Zero : ^T)
                and  ^T:(static member (+) : ^T * ^T -> ^T)
                and  ^T:(static member DivideByInt : ^T * int -> ^T) =
                source
                |> foreach (fun _ ->
                    { new FolderWithOnComplete< ^T, Values< ^T, int>> (Values<_,_>(LanguagePrimitives.GenericZero, 0)) with
                        override this.ProcessNext value =
                            this.Value._1 <- Checked.(+) this.Value._1 value
                            this.Value._2 <- this.Value._2 + 1
                            Unchecked.defaultof<_> (* return value unsed in ForEach context *)

                        member this.OnComplete _ =
                            if this.Value._2 = 0 then
                                invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString })
                |> fun total -> LanguagePrimitives.DivideByInt< ^T> total.Value._1 total.Value._2

            [<CompiledName "AverageBy">]
            let inline averageBy (f : 'T -> ^U) (source: ISeq< 'T >) : ^U
                when ^U:(static member Zero : ^U)
                and  ^U:(static member (+) : ^U * ^U -> ^U)
                and  ^U:(static member DivideByInt : ^U * int -> ^U) =
                source
                |> foreach (fun _ ->
                    { new FolderWithOnComplete<'T,Values<'U, int>>(Values<_,_>(LanguagePrimitives.GenericZero, 0)) with
                        override this.ProcessNext value =
                            this.Value._1 <- Checked.(+) this.Value._1 (f value)
                            this.Value._2 <- this.Value._2 + 1
                            Unchecked.defaultof<_> (* return value unsed in ForEach context *)

                        member this.OnComplete _ =
                            if this.Value._2 = 0 then
                                invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString })
                |> fun total -> LanguagePrimitives.DivideByInt< ^U> total.Value._1 total.Value._2

            [<CompiledName "Empty">]
            let empty<'T> = EmptyEnumerable.Enumerable<'T>.Instance

            [<CompiledName "Fold">]
            let inline fold<'T,'State> (f:'State->'T->'State) (seed:'State) (source:ISeq<'T>) : 'State =
                source
                |> foreach (fun _ ->
                    { new Folder<'T,'State>(seed) with
                        override this.ProcessNext value =
                            this.Value <- f this.Value value
                            Unchecked.defaultof<_> (* return value unsed in ForEach context *)
                    })
                |> fun folded -> folded.Value

            [<CompiledName "Unfold">]
            let unfold (generator:'State->option<'T * 'State>) (state:'State) : ISeq<'T> =
                Upcast.seq (new Unfold.Enumerable<'T,'T,'State>(generator, state, IdentityFactory.Instance))

            [<CompiledName "InitializeInfinite">]
            let initInfinite<'T> (f:int->'T) : ISeq<'T> =
                Upcast.seq (new Init.EnumerableDecider<'T>(Nullable (), f))

            [<CompiledName "Initialize">]
            let init<'T> (count:int) (f:int->'T) : ISeq<'T> =
                if count < 0 then invalidArgInputMustBeNonNegative "count" count
                elif count = 0 then empty else
                Upcast.seq (new Init.EnumerableDecider<'T>(Nullable count, f))

            [<CompiledName "Iterate">]
            let iter f (source:ISeq<'T>) =
                source
                |> foreach (fun _ ->
                    { new Consumer<'T,'T> () with
                        override this.ProcessNext value =
                            f value
                            Unchecked.defaultof<_> (* return value unsed in ForEach context *) })
                |> ignore

            [<CompiledName "TryHead">]
            let tryHead (source:ISeq<'T>) =
                source
                |> foreach (fun halt ->
                    { new Folder<'T, Option<'T>> (None) with
                        override this.ProcessNext value =
                            this.Value <- Some value
                            halt ()
                            Unchecked.defaultof<_> (* return value unsed in ForEach context *) })
                |> fun head -> head.Value



            [<CompiledName "IterateIndexed">]
            let iteri f (source:ISeq<'T>) =
                let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt f

                source
                |> foreach (fun _ ->
                    { new Folder<'T, int> (0) with
                        override this.ProcessNext value =
                            f.Invoke(this.Value, value)
                            this.Value <- this.Value + 1
                            Unchecked.defaultof<_> (* return value unsed in ForEach context *) })
                |> ignore

            [<CompiledName "Except">]
            let inline except (itemsToExclude: seq<'T>) (source:ISeq<'T>) : ISeq<'T> when 'T:equality =
                source |> compose { new SeqFactory<'T,'T>() with
                    member __.Create _ _ next =
                        upcast { new ConsumerChainedWithState<'T,'V,Lazy<HashSet<'T>>>
                                        (Upcast.iCompletionChain next,lazy(HashSet<'T>(itemsToExclude,HashIdentity.Structural<'T>))) with
                            override this.ProcessNext (input:'T) : bool =
                                if this.Value.Value.Add input then TailCall.avoid (next.ProcessNext input)
                                else false
                        }}

            [<CompiledName "Exists">]
            let exists f (source:ISeq<'T>) =
                source
                |> foreach (fun halt ->
                    { new Folder<'T, bool> (false) with
                        override this.ProcessNext value =
                            if f value then
                                this.Value <- true
                                halt ()
                            Unchecked.defaultof<_> (* return value unsed in ForEach context *) })
                |> fun exists -> exists.Value

            [<CompiledName "Contains">]
            let inline contains element (source:ISeq<'T>) =
                source
                |> foreach (fun halt ->
                    { new Folder<'T, bool> (false) with
                        override this.ProcessNext value =
                            if element = value then
                                this.Value <- true
                                halt ()
                            Unchecked.defaultof<_> (* return value unsed in ForEach context *) })
                |> fun contains -> contains.Value

            [<CompiledName "ForAll">]
            let forall f (source:ISeq<'T>) =
                source
                |> foreach (fun halt ->
                    { new Folder<'T, bool> (true) with
                        override this.ProcessNext value =
                            if not (f value) then
                                this.Value <- false
                                halt ()
                            Unchecked.defaultof<_> (* return value unsed in ForEach context *) })
                |> fun forall -> forall.Value

            [<CompiledName "Filter">]
            let inline filter<'T> (f:'T->bool) (source:ISeq<'T>) : ISeq<'T> =
                source |> compose { new SeqFactory<'T,'T>() with
                    member __.Create _ _ next =
                        upcast { new ConsumerChained<'T,'V>(Upcast.iCompletionChain next) with
                            member __.ProcessNext input =
                                if f input then TailCall.avoid (next.ProcessNext input)
                                else false } }

            [<CompiledName "Map">]
            let inline map<'T,'U> (f:'T->'U) (source:ISeq<'T>) : ISeq<'U> =
                source |> compose { new SeqFactory<'T,'U>() with
                    member __.Create _ _ next =
                        upcast { new ConsumerChained<'T,'V>(Upcast.iCompletionChain next) with
                            member __.ProcessNext input =
                                TailCall.avoid (next.ProcessNext (f input)) } }

            [<CompiledName "MapIndexed">]
            let inline mapi f source =
                source |> compose { new SeqFactory<'T,'U>() with
                    member __.Create _ _ next =
                        upcast { new ConsumerChainedWithState<'T,'V,int>(Upcast.iCompletionChain next, -1) with
                            override this.ProcessNext (input:'T) : bool =
                                this.Value <- this.Value  + 1
                                TailCall.avoid (next.ProcessNext (f this.Value input)) } }

            let mapi_adapt (f:OptimizedClosures.FSharpFunc<_,_,_>) source =
                source |> compose { new SeqFactory<'T,'U>() with
                    member __.Create _ _ next =
                        upcast { new ConsumerChainedWithState<'T,'V,int>(Upcast.iCompletionChain next, -1) with
                            override this.ProcessNext (input:'T) : bool =
                                this.Value <- this.Value  + 1
                                TailCall.avoid (next.ProcessNext (f.Invoke (this.Value, input))) } }

            [<CompiledName "Choose">]
            let inline choose (f:'T->option<'U>) (source:ISeq<'T>) : ISeq<'U> =
                source |> compose { new SeqFactory<'T,'U>() with
                    member __.Create _ _ next =
                        upcast { new ConsumerChained<'T,'V>(Upcast.iCompletionChain next) with
                            member __.ProcessNext input =
                                match f input with
                                | Some value -> TailCall.avoid (next.ProcessNext value)
                                | None       -> false } }

            [<CompiledName "Distinct">]
            let inline distinct (source:ISeq<'T>) : ISeq<'T> when 'T:equality =
                source |> compose { new SeqFactory<'T,'T>() with
                    member __.Create _ _ next =
                        upcast { new ConsumerChainedWithState<'T,'V,HashSet<'T>>
                                        (Upcast.iCompletionChain next,(HashSet<'T>(HashIdentity.Structural<'T>))) with
                            override this.ProcessNext (input:'T) : bool =
                                if this.Value.Add input then TailCall.avoid (next.ProcessNext input)
                                else false } }

            [<CompiledName "DistinctBy">]
            let inline distinctBy (keyf:'T->'Key) (source:ISeq<'T>) :ISeq<'T>  when 'Key:equality =
                source |> compose { new SeqFactory<'T,'T>() with
                    member __.Create _ _ next =
                        upcast { new ConsumerChainedWithState<'T,'V,HashSet<'Key>>
                                        (Upcast.iCompletionChain next,(HashSet<'Key>(HashIdentity.Structural<'Key>))) with
                            override this.ProcessNext (input:'T) : bool =
                                if this.Value.Add (keyf input) then TailCall.avoid (next.ProcessNext input)
                                else false } }

            [<CompiledName "Max">]
            let inline max (source: ISeq<'T>) : 'T when 'T:comparison =
                source
                |> foreach (fun _ ->
                    { new FolderWithOnComplete<'T,Values<bool,'T>>(Values<_,_>(true, Unchecked.defaultof<'T>)) with
                        override this.ProcessNext value =
                            if this.Value._1 then
                                this.Value._1 <- false
                                this.Value._2 <- value
                            elif value > this.Value._2 then
                                this.Value._2 <- value
                            Unchecked.defaultof<_> (* return value unsed in ForEach context *)

                        member this.OnComplete _ =
                            if this.Value._1 then
                                invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
                    })
                |> fun max -> max.Value._2

            [<CompiledName "MaxBy">]
            let inline maxBy (f :'T -> 'U) (source: ISeq<'T>) : 'T when 'U:comparison =
                source
                |> foreach (fun _ ->
                    { new FolderWithOnComplete<'T,Values<bool,'U,'T>>(Values<_,_,_>(true,Unchecked.defaultof<'U>,Unchecked.defaultof<'T>)) with
                        override this.ProcessNext value =
                            match this.Value._1, f value with
                            | true, valueU ->
                                this.Value._1 <- false
                                this.Value._2 <- valueU
                                this.Value._3 <- value
                            | false, valueU when valueU > this.Value._2 ->
                                this.Value._2 <- valueU
                                this.Value._3 <- value
                            | _ -> ()
                            Unchecked.defaultof<_> (* return value unsed in ForEach context *)

                        member this.OnComplete _ =
                            if this.Value._1 then
                                invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
                    })
                |> fun min -> min.Value._3

            [<CompiledName "Min">]
            let inline min (source: ISeq< 'T>) : 'T when 'T:comparison =
                source
                |> foreach (fun _ ->
                    { new FolderWithOnComplete<'T,Values<bool,'T>>(Values<_,_>(true, Unchecked.defaultof<'T>)) with
                        override this.ProcessNext value =
                            if this.Value._1 then
                                this.Value._1 <- false
                                this.Value._2 <- value
                            elif value < this.Value._2 then
                                this.Value._2 <- value
                            Unchecked.defaultof<_> (* return value unsed in ForEach context *)

                        member this.OnComplete _ =
                            if this.Value._1 then
                                invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
                    })
                |> fun min -> min.Value._2

            [<CompiledName "MinBy">]
            let inline minBy (f : 'T -> 'U) (source: ISeq<'T>) : 'T =
                source
                |> foreach (fun _ ->
                    { new FolderWithOnComplete< 'T,Values<bool,'U,'T>>(Values<_,_,_>(true,Unchecked.defaultof< 'U>,Unchecked.defaultof< 'T>)) with
                        override this.ProcessNext value =
                            match this.Value._1, f value with
                            | true, valueU ->
                                this.Value._1 <- false
                                this.Value._2 <- valueU
                                this.Value._3 <- value
                            | false, valueU when valueU < this.Value._2 ->
                                this.Value._2 <- valueU
                                this.Value._3 <- value
                            | _ -> ()
                            Unchecked.defaultof<_> (* return value unsed in ForEach context *)

                        member this.OnComplete _ =
                            if this.Value._1 then
                                invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
                    })
                |> fun min -> min.Value._3

            [<CompiledName "Pairwise">]
            let inline pairwise (source:ISeq<'T>) : ISeq<'T * 'T> =
                source |> compose { new SeqFactory<'T,'T * 'T>() with
                    member __.Create _ _ next =
                        upcast { new ConsumerChainedWithState<'T,'U,Values<bool,'T>>
                                    (   Upcast.iCompletionChain next
                                    ,   Values<bool,'T>
                                        ((* isFirst   = _1*) true
                                        ,(* lastValue = _2*) Unchecked.defaultof<'T>
                                        )
                                    ) with
                                override self.ProcessNext (input:'T) : bool =
                                    if (*isFirst*) self.Value._1  then
                                        self.Value._2 (*lastValue*)<- input
                                        self.Value._1 (*isFirst*)<- false
                                        false
                                    else
                                        let currentPair = self.Value._2, input
                                        self.Value._2 (*lastValue*)<- input
                                        TailCall.avoid (next.ProcessNext currentPair)
                        }}

            [<CompiledName "Scan">]
            let inline scan (folder:'State->'T->'State) (initialState: 'State) (source:ISeq<'T>) :ISeq<'State> =
                source |> compose { new SeqFactory<'T,'State>() with
                    member __.Create _ _ next =
                        upcast { new ConsumerChainedWithState<'T,'V,'State>(Upcast.iCompletionChain next, initialState) with
                            override this.ProcessNext (input:'T) : bool =
                                this.Value <- folder this.Value input
                                TailCall.avoid (next.ProcessNext this.Value) } }


            let scan_adapt (folder:OptimizedClosures.FSharpFunc<'State,'T,'State>) (initialState: 'State) (source:ISeq<'T>) :ISeq<'State> =
                source |> compose { new SeqFactory<'T,'State>() with
                    member __.Create _ _ next =
                        upcast { new ConsumerChainedWithState<'T,'V,'State>(Upcast.iCompletionChain next, initialState) with
                            override this.ProcessNext (input:'T) : bool =
                                this.Value <- folder.Invoke(this.Value,input)
                                TailCall.avoid (next.ProcessNext this.Value) } }

            [<CompiledName "Skip">]
            let inline skip (skipCount:int) (source:ISeq<'T>) : ISeq<'T> =
                source |> compose { new SeqFactory<'T,'T>() with
                    member __.Create _ _ next =
                        upcast {
                            new ConsumerChainedWithStateAndCleanup<'T,'U,int>(Upcast.iCompletionChain next,(*count*)0) with

                                override self.ProcessNext (input:'T) : bool =
                                    if (*count*) self.Value < skipCount then
                                        self.Value <- self.Value + 1
                                        false
                                    else
                                        TailCall.avoid (next.ProcessNext input)

                                override self.OnDispose () = ()
                                override self.OnComplete _ =
                                    if (*count*) self.Value < skipCount then
                                        let x = skipCount - self.Value
                                        invalidOpFmt "{0}\ntried to skip {1} {2} past the end of the seq"
                                            [|SR.GetString SR.notEnoughElements; x; (if x=1 then "element" else "elements")|]

                            interface ISkipping with
                                member self.Skipping () =
                                    let self = self :?> ConsumerChainedWithState<'T,'U,int>
                                    if (*count*) self.Value < skipCount then
                                        self.Value <- self.Value + 1
                                        true
                                    else
                                        false
                        }}

            [<CompiledName "SkipWhile">]
            let inline skipWhile (predicate:'T->bool) (source:ISeq<'T>) : ISeq<'T> =
                source |> compose { new SeqFactory<'T,'T>() with
                    member __.Create _ _ next =
                        upcast { new ConsumerChainedWithState<'T,'V,bool>(Upcast.iCompletionChain next,true) with
                            override self.ProcessNext (input:'T) : bool =
                                if self.Value (*skip*) then
                                    self.Value <- predicate input
                                    if self.Value (*skip*) then
                                        false
                                    else
                                        TailCall.avoid (next.ProcessNext input)
                                else
                                    TailCall.avoid (next.ProcessNext input) }}

            [<CompiledName "Sum">]
            let inline sum (source:ISeq< ^T>) : ^T
                when ^T:(static member Zero : ^T)
                and  ^T:(static member (+) :  ^T *  ^T ->  ^T) =
                source
                |> foreach (fun _ ->
                    { new Folder< ^T,^T> (LanguagePrimitives.GenericZero) with
                        override this.ProcessNext value =
                            this.Value <- Checked.(+) this.Value value
                            Unchecked.defaultof<_> (* return value unsed in ForEach context *) })
                |> fun sum -> sum.Value

            [<CompiledName "SumBy">]
            let inline sumBy (f : 'T -> ^U) (source: ISeq<'T>) : ^U
                when ^U:(static member Zero : ^U)
                and  ^U:(static member (+) :  ^U *  ^U ->  ^U) =
                source
                |> foreach (fun _ ->
                    { new Folder<'T,'U> (LanguagePrimitives.GenericZero< ^U>) with
                        override this.ProcessNext value =
                            this.Value <- Checked.(+) this.Value (f value)
                            Unchecked.defaultof<_> (* return value unsed in ForEach context *) })
                |> fun sum -> sum.Value

            [<CompiledName "Take">]
            let inline take (takeCount:int) (source:ISeq<'T>) : ISeq<'T> =
                source |> compose { new SeqFactory<'T,'T>() with
                    member __.Create outOfBand pipelineIdx next =
                        upcast {
                            new ConsumerChainedWithStateAndCleanup<'T,'U,int>(Upcast.iCompletionChain next,(*count*)0) with
                                override self.ProcessNext (input:'T) : bool =
                                    if (*count*) self.Value < takeCount then
                                        self.Value <- self.Value + 1
                                        if self.Value = takeCount then
                                            outOfBand.StopFurtherProcessing pipelineIdx
                                        TailCall.avoid (next.ProcessNext input)
                                    else
                                        outOfBand.StopFurtherProcessing pipelineIdx
                                        false

                                override this.OnDispose () = ()
                                override this.OnComplete terminatingIdx =
                                    if terminatingIdx < pipelineIdx && this.Value < takeCount then
                                        let x = takeCount - this.Value
                                        invalidOpFmt "tried to take {0} {1} past the end of the seq"
                                            [|SR.GetString SR.notEnoughElements; x; (if x=1 then "element" else "elements")|]
                        }}

            [<CompiledName "TakeWhile">]
            let inline takeWhile (predicate:'T->bool) (source:ISeq<'T>) : ISeq<'T> =
                source |> compose { new SeqFactory<'T,'T>() with
                    member __.Create outOfBand pipeIdx next =
                        upcast { new ConsumerChained<'T,'V>(Upcast.iCompletionChain next) with
                            override __.ProcessNext (input:'T) : bool =
                                if predicate input then
                                    TailCall.avoid (next.ProcessNext input)
                                else
                                    outOfBand.StopFurtherProcessing pipeIdx
                                    false
                        }}

            [<CompiledName "Tail">]
            let inline tail (source:ISeq<'T>) :ISeq<'T> =
                source |> compose { new SeqFactory<'T,'T>() with
                    member __.Create _ _ next =
                        upcast { new ConsumerChainedWithStateAndCleanup<'T,'V,bool>(Upcast.iCompletionChain next,(*first*) true) with
                            override self.ProcessNext (input:'T) : bool =
                                if (*first*) self.Value then
                                    self.Value <- false
                                    false
                                else
                                    TailCall.avoid (next.ProcessNext input)

                            override self.OnDispose () = ()
                            override self.OnComplete _ =
                                if (*first*) self.Value then
                                    invalidArg "source" (SR.GetString(SR.notEnoughElements))
                        }}

            [<CompiledName "Truncate">]
            let inline truncate (truncateCount:int) (source:ISeq<'T>) : ISeq<'T> =
                source |> compose { new SeqFactory<'T,'T>() with
                    member __.Create outOfBand pipeIdx next =
                        upcast {
                            new ConsumerChainedWithState<'T,'U,int>(Upcast.iCompletionChain next,(*count*)0) with
                                override self.ProcessNext (input:'T) : bool =
                                    if (*count*) self.Value < truncateCount then
                                        self.Value <- self.Value + 1
                                        if self.Value = truncateCount then
                                            outOfBand.StopFurtherProcessing pipeIdx
                                        TailCall.avoid (next.ProcessNext input)
                                    else
                                        outOfBand.StopFurtherProcessing pipeIdx
                                        false
                        }}

            [<CompiledName "Indexed">]
            let inline indexed source =
                mapi (fun i x -> i,x) source

            [<CompiledName "TryItem">]
            let tryItem index (source:ISeq<'T>) =
                if index < 0 then None else
                source |> skip index |> tryHead

            [<CompiledName "TryPick">]
            let tryPick f (source:ISeq<'T>)  =
                source
                |> foreach (fun halt ->
                    { new Folder<'T, Option<'U>> (None) with
                        override this.ProcessNext value =
                            match f value with
                            | (Some _) as some ->
                                this.Value <- some
                                halt ()
                            | None -> ()
                            Unchecked.defaultof<_> (* return value unsed in ForEach context *) })
                |> fun pick -> pick.Value

            [<CompiledName "TryFind">]
            let tryFind f (source:ISeq<'T>)  =
                source
                |> foreach (fun halt ->
                    { new Folder<'T, Option<'T>> (None) with
                        override this.ProcessNext value =
                            if f value then
                                this.Value <- Some value
                                halt ()
                            Unchecked.defaultof<_> (* return value unsed in ForEach context *) })
                |> fun find -> find.Value

            [<CompiledName "Windowed">]
            let inline windowed (windowSize:int) (source:ISeq<'T>) : ISeq<'T[]> =
                source |> compose { new SeqFactory<'T,'T[]>() with
                    member __.Create outOfBand pipeIdx next =
                        upcast {
                            new ConsumerChainedWithState<'T,'U,Values<'T[],int,int>>
                                        (   Upcast.iCompletionChain next
                                        ,   Values<'T[],int,int>
                                            ((*circularBuffer = _1 *) Array.zeroCreateUnchecked windowSize
                                            ,(* idx = _2 *)          0
                                            ,(* priming = _3 *)      windowSize-1
                                            )
                                        ) with
                                override self.ProcessNext (input:'T) : bool =
                                    self.Value._1.[(* idx *)self.Value._2] <- input

                                    self.Value._2 <- (* idx *)self.Value._2 + 1
                                    if (* idx *) self.Value._2 = windowSize then
                                        self.Value._2 <- 0

                                    if (* priming  *) self.Value._3 > 0 then
                                        self.Value._3 <- self.Value._3 - 1
                                        false
                                    else
                                        if windowSize < 32 then
                                            let window :'T [] = Array.init windowSize (fun i -> self.Value._1.[((* idx *)self.Value._2+i) % windowSize]: 'T)
                                            TailCall.avoid (next.ProcessNext window)
                                        else
                                            let window = Array.zeroCreateUnchecked windowSize
                                            Array.Copy((*circularBuffer*)self.Value._1, (* idx *)self.Value._2, window, 0, windowSize - (* idx *)self.Value._2)
                                            Array.Copy((*circularBuffer*)self.Value._1, 0, window, windowSize - (* idx *)self.Value._2, (* idx *)self.Value._2)
                                            TailCall.avoid (next.ProcessNext window)

                        }}
