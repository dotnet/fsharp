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
            type PipeIdx = int

            type ICompletionChaining =
                abstract OnComplete : stopTailCall:byref<unit> * PipeIdx -> unit
                abstract OnDispose  : stopTailCall:byref<unit> -> unit

            type IOutOfBand =
                abstract StopFurtherProcessing : PipeIdx -> unit

            [<AbstractClass>]
            type Consumer<'T,'U> () =
                abstract ProcessNext : input:'T -> bool

                abstract OnComplete : PipeIdx -> unit
                abstract OnDispose  : unit -> unit

                default __.OnComplete _ = ()
                default __.OnDispose () = ()

                interface ICompletionChaining with
                    member this.OnComplete (_, terminatingIdx) =
                        this.OnComplete terminatingIdx

                    member this.OnDispose _ =
                        try this.OnDispose ()
                        finally ()

            [<Struct; NoComparison; NoEquality>]
            type Values<'a,'b> =
                val mutable _1 : 'a
                val mutable _2 : 'b

                new (a:'a, b: 'b) = {
                    _1 = a
                    _2 = b
                }

            [<Struct; NoComparison; NoEquality>]
            type Values<'a,'b,'c> =
                val mutable _1 : 'a
                val mutable _2 : 'b
                val mutable _3 : 'c

                new (a:'a, b:'b, c:'c) = {
                    _1 = a
                    _2 = b
                    _3 = c
                }

            [<AbstractClass>]
            type Folder<'T, 'U> =
                inherit Consumer<'T,'T>

                val mutable Value : 'U

                new (init) = {
                    inherit Consumer<'T,'T>()
                    Value = init
                }

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
            let inline iCompletionChaining (t:#ICompletionChaining) : ICompletionChaining = (# "" t : ICompletionChaining #)

        module internal Seq =
            type ComposedFactory<'T,'U,'V> private (first:SeqFactory<'T,'U>, second:SeqFactory<'U,'V>, secondPipeIdx:PipeIdx) =
                inherit SeqFactory<'T,'V>()

                override __.PipeIdx =
                    secondPipeIdx

                override this.Create<'W> (outOfBand:IOutOfBand) (pipeIdx:PipeIdx) (next:Consumer<'V,'W>) : Consumer<'T,'W> =
                    first.Create outOfBand pipeIdx (second.Create outOfBand secondPipeIdx next)

                static member Combine (first:SeqFactory<'T,'U>) (second:SeqFactory<'U,'V>) : SeqFactory<'T,'V> =
                    upcast ComposedFactory(first, second, first.PipeIdx+1)

            and DistinctFactory<'T when 'T: equality> () =
                inherit SeqFactory<'T,'T> ()
                override this.Create<'V> (_outOfBand:IOutOfBand) (_pipeIdx:PipeIdx) (next:Consumer<'T,'V>) : Consumer<'T,'V> = upcast Distinct (next)

            and DistinctByFactory<'T,'Key when 'Key: equality> (keyFunction:'T-> 'Key) =
                inherit SeqFactory<'T,'T> ()
                override this.Create<'V> (_outOfBand:IOutOfBand) (_pipeIdx:PipeIdx) (next:Consumer<'T,'V>) : Consumer<'T,'V> = upcast DistinctBy (keyFunction, next)

            and ExceptFactory<'T when 'T: equality> (itemsToExclude: seq<'T>) =
                inherit SeqFactory<'T,'T> ()
                override this.Create<'V> (_outOfBand:IOutOfBand) (_pipeIdx:PipeIdx) (next:Consumer<'T,'V>) : Consumer<'T,'V> = upcast Except (itemsToExclude, next)

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

            and PairwiseFactory<'T> () =
                inherit SeqFactory<'T,'T*'T> ()
                override this.Create<'V> (_outOfBand:IOutOfBand) (_pipeIdx:PipeIdx) (next:Consumer<'T*'T,'V>) : Consumer<'T,'V> = upcast Pairwise (next)

            and ScanFactory<'T,'State> (folder:'State->'T->'State, initialState:'State) =
                inherit SeqFactory<'T,'State> ()
                override this.Create<'V> (_outOfBand:IOutOfBand) (_pipeIdx:PipeIdx) (next:Consumer<'State,'V>) : Consumer<'T,'V> = upcast Scan<_,_,_> (folder, initialState, next)

            and SkipFactory<'T> (count:int, onNotEnoughElements) =
                inherit SeqFactory<'T,'T> ()
                override this.Create<'V> (_outOfBand:IOutOfBand) (_pipeIdx:PipeIdx) (next:Consumer<'T,'V>) : Consumer<'T,'V> = upcast Skip (count, onNotEnoughElements, next)

            and SkipWhileFactory<'T> (predicate:'T->bool) =
                inherit SeqFactory<'T,'T> ()
                override this.Create<'V> (_outOfBand:IOutOfBand) (_pipeIdx:PipeIdx) (next:Consumer<'T,'V>) : Consumer<'T,'V> = upcast SkipWhile (predicate, next)

            and TakeWhileFactory<'T> (predicate:'T->bool) =
                inherit SeqFactory<'T,'T> ()
                override this.Create<'V> (outOfBand:IOutOfBand) (pipeIdx:PipeIdx) (next:Consumer<'T,'V>) : Consumer<'T,'V> = upcast TakeWhile (predicate, outOfBand, next, pipeIdx)

            and TakeFactory<'T> (count:int) =
                inherit SeqFactory<'T,'T> ()
                override this.Create<'V> (outOfBand:IOutOfBand) (pipeIdx:PipeIdx) (next:Consumer<'T,'V>) : Consumer<'T,'V> = upcast Take (count, outOfBand, next, pipeIdx)

            and TailFactory<'T> () =
                inherit SeqFactory<'T,'T> ()
                override this.Create<'V> (_outOfBand:IOutOfBand) (_pipeIdx:PipeIdx) (next:Consumer<'T,'V>) : Consumer<'T,'V> = upcast Tail<'T,'V> (next)

            and TruncateFactory<'T> (count:int) =
                inherit SeqFactory<'T,'T> ()
                override this.Create<'V> (outOfBand:IOutOfBand) (pipeIdx:PipeIdx) (next:Consumer<'T,'V>) : Consumer<'T,'V> = upcast Truncate (count, outOfBand, next, pipeIdx)

            and WindowedFactory<'T> (windowSize:int) =
                inherit SeqFactory<'T, 'T[]> ()
                override this.Create<'V> (_outOfBand:IOutOfBand) (_pipeIdx:PipeIdx) (next:Consumer<'T[],'V>) : Consumer<'T,'V> = upcast Windowed (windowSize, next)

            and ISkipping =
                // Seq.init(Infinite)? lazily uses Current. The only Composer component that can do that is Skip
                // and it can only do it at the start of a sequence
                abstract Skipping : unit -> bool

            and [<AbstractClass>] SeqComponentSimple<'T,'U> (next:ICompletionChaining) =
                inherit Consumer<'T,'U>()

                interface ICompletionChaining with
                    member this.OnComplete (stopTailCall, terminatingIdx) =
                        next.OnComplete (&stopTailCall, terminatingIdx)
                    member this.OnDispose stopTailCall =
                        next.OnDispose (&stopTailCall)

            and [<AbstractClass>] SeqComponentSimpleValue<'T,'U,'Value>  =
                inherit SeqComponentSimple<'T,'U>

                val mutable Value : 'Value

                new (next, init) = {
                    inherit SeqComponentSimple<'T,'U>(next)
                    Value = init
                }

            and [<AbstractClass>] SeqComponent<'T,'U> (next:ICompletionChaining) =
                inherit Consumer<'T,'U>()

                interface ICompletionChaining with
                    member this.OnComplete (stopTailCall, terminatingIdx) =
                        this.OnComplete terminatingIdx
                        next.OnComplete (&stopTailCall, terminatingIdx)
                    member this.OnDispose stopTailCall  =
                        try     this.OnDispose ()
                        finally next.OnDispose (&stopTailCall)


            and Distinct<'T,'V when 'T: equality> (next:Consumer<'T,'V>) =
                inherit SeqComponent<'T,'V>(Upcast.iCompletionChaining next)

                let hashSet = HashSet<'T>(HashIdentity.Structural<'T>)

                override __.ProcessNext (input:'T) : bool =
                    if hashSet.Add input then
                        TailCall.avoid (next.ProcessNext input)
                    else
                        false

            and DistinctBy<'T,'Key,'V when 'Key: equality> (keyFunction: 'T -> 'Key, next:Consumer<'T,'V>) =
                inherit SeqComponent<'T,'V>(Upcast.iCompletionChaining next)

                let hashSet = HashSet<'Key>(HashIdentity.Structural<'Key>)

                override __.ProcessNext (input:'T) : bool =
                    if hashSet.Add(keyFunction input) then
                        TailCall.avoid (next.ProcessNext input)
                    else
                        false

            and Except<'T,'V when 'T: equality> (itemsToExclude: seq<'T>, next:Consumer<'T,'V>) =
                inherit SeqComponent<'T,'V>(Upcast.iCompletionChaining next)

                let cached = lazy(HashSet(itemsToExclude, HashIdentity.Structural))

                override __.ProcessNext (input:'T) : bool =
                    if cached.Value.Add input then
                        TailCall.avoid (next.ProcessNext input)
                    else
                        false

            and Map2First<'First,'Second,'U,'V> (map:'First->'Second->'U, enumerable2:IEnumerable<'Second>, outOfBand:IOutOfBand, next:Consumer<'U,'V>, pipeIdx:int) =
                inherit SeqComponent<'First,'V>(Upcast.iCompletionChaining next)

                let input2 = enumerable2.GetEnumerator ()
                let map' = OptimizedClosures.FSharpFunc<_,_,_>.Adapt map

                override __.ProcessNext (input:'First) : bool =
                    if input2.MoveNext () then
                        TailCall.avoid (next.ProcessNext (map'.Invoke (input, input2.Current)))
                    else
                        outOfBand.StopFurtherProcessing pipeIdx
                        false

                override __.OnDispose () =
                    input2.Dispose ()

            and Map2Second<'First,'Second,'U,'V> (map:'First->'Second->'U, enumerable1:IEnumerable<'First>, outOfBand:IOutOfBand, next:Consumer<'U,'V>, pipeIdx:int) =
                inherit SeqComponent<'Second,'V>(Upcast.iCompletionChaining next)

                let input1 = enumerable1.GetEnumerator ()
                let map' = OptimizedClosures.FSharpFunc<_,_,_>.Adapt map

                override __.ProcessNext (input:'Second) : bool =
                    if input1.MoveNext () then
                        TailCall.avoid (next.ProcessNext (map'.Invoke (input1.Current, input)))
                    else
                        outOfBand.StopFurtherProcessing pipeIdx
                        false

                override __.OnDispose () =
                    input1.Dispose ()

            and Map3<'First,'Second,'Third,'U,'V> (map:'First->'Second->'Third->'U, enumerable2:IEnumerable<'Second>, enumerable3:IEnumerable<'Third>, outOfBand:IOutOfBand, next:Consumer<'U,'V>, pipeIdx:int) =
                inherit SeqComponent<'First,'V>(Upcast.iCompletionChaining next)

                let input2 = enumerable2.GetEnumerator ()
                let input3 = enumerable3.GetEnumerator ()
                let map' = OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt map

                override __.ProcessNext (input:'First) : bool =
                    if input2.MoveNext () && input3.MoveNext () then
                        TailCall.avoid (next.ProcessNext (map'.Invoke (input, input2.Current, input3.Current)))
                    else
                        outOfBand.StopFurtherProcessing pipeIdx
                        false

                override __.OnDispose () =
                    try     input2.Dispose ()
                    finally input3.Dispose ()

            and Mapi2<'First,'Second,'U,'V> (map:int->'First->'Second->'U, enumerable2:IEnumerable<'Second>, outOfBand:IOutOfBand, next:Consumer<'U,'V>, pipeIdx:int) =
                inherit SeqComponent<'First,'V>(Upcast.iCompletionChaining next)

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

                override __.OnDispose () =
                    input2.Dispose ()

            and Pairwise<'T,'V> (next:Consumer<'T*'T,'V>) =
                inherit SeqComponent<'T,'V>(Upcast.iCompletionChaining next)

                let mutable isFirst = true
                let mutable lastValue = Unchecked.defaultof<'T>

                override __.ProcessNext (input:'T) : bool =
                    if isFirst then
                        lastValue <- input
                        isFirst <- false
                        false
                    else
                        let currentPair = lastValue, input
                        lastValue <- input
                        TailCall.avoid (next.ProcessNext currentPair)

            and Scan<'T,'State,'V> (folder:'State->'T->'State, initialState: 'State, next:Consumer<'State,'V>) =
                inherit SeqComponent<'T,'V>(Upcast.iCompletionChaining next)

                let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt folder
                let mutable foldResult = initialState

                override __.ProcessNext (input:'T) : bool =
                    foldResult <- f.Invoke(foldResult, input)
                    TailCall.avoid (next.ProcessNext foldResult)

            and Skip<'T,'V> (skipCount:int, notEnoughElements:string->array<obj>->unit, next:Consumer<'T,'V>) =
                inherit SeqComponent<'T,'V>(Upcast.iCompletionChaining next)

                let mutable count = 0

                interface ISkipping with
                    member __.Skipping () =
                        if count < skipCount then
                            count <- count + 1
                            true
                        else
                            false

                override __.ProcessNext (input:'T) : bool =
                    if count < skipCount then
                        count <- count + 1
                        false
                    else
                        TailCall.avoid (next.ProcessNext input)

                override __.OnComplete _ =
                    if count < skipCount then
                        let x = skipCount - count
                        notEnoughElements "{0}\ntried to skip {1} {2} past the end of the seq"
                            [|SR.GetString SR.notEnoughElements; x; (if x=1 then "element" else "elements")|]

            and SkipWhile<'T,'V> (predicate:'T->bool, next:Consumer<'T,'V>) =
                inherit SeqComponent<'T,'V>(Upcast.iCompletionChaining next)

                let mutable skip = true

                override __.ProcessNext (input:'T) : bool =
                    if skip then
                        skip <- predicate input
                        if skip then
                            false
                        else
                            TailCall.avoid (next.ProcessNext input)
                    else
                        TailCall.avoid (next.ProcessNext input)

            and Take<'T,'V> (takeCount:int, outOfBand:IOutOfBand, next:Consumer<'T,'V>, pipelineIdx:int) =
                inherit Truncate<'T, 'V>(takeCount, outOfBand, next, pipelineIdx)

                override this.OnComplete terminatingIdx =
                    if terminatingIdx < pipelineIdx && this.Count < takeCount then
                        let x = takeCount - this.Count
                        invalidOpFmt "tried to take {0} {1} past the end of the seq"
                            [|SR.GetString SR.notEnoughElements; x; (if x=1 then "element" else "elements")|]

            and TakeWhile<'T,'V> (predicate:'T->bool, outOfBand:IOutOfBand, next:Consumer<'T,'V>, pipeIdx:int) =
                inherit SeqComponent<'T,'V>(Upcast.iCompletionChaining next)

                override __.ProcessNext (input:'T) : bool =
                    if predicate input then
                        TailCall.avoid (next.ProcessNext input)
                    else
                        outOfBand.StopFurtherProcessing pipeIdx
                        false

            and Tail<'T, 'V> (next:Consumer<'T,'V>) =
                inherit SeqComponent<'T,'V>(Upcast.iCompletionChaining next)

                let mutable first = true

                override __.ProcessNext (input:'T) : bool =
                    if first then
                        first <- false
                        false
                    else
                        TailCall.avoid (next.ProcessNext input)

                override this.OnComplete _ =
                    if first then
                        invalidArg "source" (SR.GetString(SR.notEnoughElements))

            and Truncate<'T,'V> (truncateCount:int, outOfBand:IOutOfBand, next:Consumer<'T,'V>, pipeIdx:int) =
                inherit SeqComponent<'T,'V>(Upcast.iCompletionChaining next)

                let mutable count = 0

                member __.Count = count

                override __.ProcessNext (input:'T) : bool =
                    if count < truncateCount then
                        count <- count + 1
                        if count = truncateCount then
                            outOfBand.StopFurtherProcessing pipeIdx
                        TailCall.avoid (next.ProcessNext input)
                    else
                        outOfBand.StopFurtherProcessing pipeIdx
                        false

            and Windowed<'T,'V> (windowSize: int, next:Consumer<'T[],'V>) =
                inherit SeqComponent<'T,'V>(Upcast.iCompletionChaining next)

                let circularBuffer = Array.zeroCreateUnchecked windowSize
                let mutable idx = 0

                let mutable priming = windowSize - 1

                override __.ProcessNext (input:'T) : bool =
                    circularBuffer.[idx] <- input

                    idx <- idx + 1
                    if idx = windowSize then
                        idx <- 0

                    if priming > 0 then
                        priming <- priming - 1
                        false
                    else
                        if windowSize < 32 then
                            let window = Array.init windowSize (fun i -> circularBuffer.[(idx+i) % windowSize])
                            TailCall.avoid (next.ProcessNext window)
                        else
                            let window = Array.zeroCreateUnchecked windowSize
                            Array.Copy(circularBuffer, idx, window, 0, windowSize - idx)
                            Array.Copy(circularBuffer, 0, window, windowSize - idx, idx)
                            TailCall.avoid (next.ProcessNext window)

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
                        (Upcast.iCompletionChaining consumer).OnComplete (&stopTailCall, pipeline.HaltedIdx)
                        result
                    finally
                        let mutable stopTailCall = ()
                        (Upcast.iCompletionChaining consumer).OnDispose (&stopTailCall)

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
                type EnumeratorBase<'T>(result:Result<'T>, seqComponent:ICompletionChaining) =
                    interface IDisposable with
                        member __.Dispose() : unit =
                            let mutable stopTailCall = ()
                            seqComponent.OnDispose (&stopTailCall)

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
                            (Upcast.iCompletionChaining seqComponent).OnComplete (&stopTailCall, result.HaltedIdx)
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
                                (Upcast.iCompletionChaining seqComponent).OnDispose (&stopTailCall)

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
                            (Upcast.iCompletionChaining seqComponent).OnComplete (&stopTailCall, result.HaltedIdx)
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
                            (Upcast.iCompletionChaining seqComponent).OnComplete (&stopTailCall, result.HaltedIdx)
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
                            (Upcast.iCompletionChaining seqComponent).OnComplete (&stopTailCall, result.HaltedIdx)
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

            [<CompiledName("ToComposer")>]
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

            [<CompiledName("Empty")>]
            let empty<'T> = EmptyEnumerable.Enumerable<'T>.Instance

            [<CompiledName("Unfold")>]
            let unfold (generator:'State->option<'T * 'State>) (state:'State) : ISeq<'T> =
                Upcast.seq (new Unfold.Enumerable<'T,'T,'State>(generator, state, IdentityFactory.Instance))

            [<CompiledName("InitializeInfinite")>]
            let initInfinite<'T> (f:int->'T) : ISeq<'T> =
                Upcast.seq (new Init.EnumerableDecider<'T>(Nullable (), f))

            [<CompiledName("Initialize")>]
            let init<'T> (count:int) (f:int->'T) : ISeq<'T> =
                if count < 0 then invalidArgInputMustBeNonNegative "count" count
                elif count = 0 then empty else
                Upcast.seq (new Init.EnumerableDecider<'T>(Nullable count, f))

            [<CompiledName("Iterate")>]
            let iter f (source:ISeq<'T>) =
                source
                |> foreach (fun _ ->
                    { new Consumer<'T,'T> () with
                        override this.ProcessNext value =
                            f value
                            Unchecked.defaultof<_> (* return value unsed in ForEach context *) })
                |> ignore

            [<CompiledName("TryHead")>]
            let tryHead (source:ISeq<'T>) =
                source
                |> foreach (fun halt ->
                    { new Folder<'T, Option<'T>> (None) with
                        override this.ProcessNext value =
                            this.Value <- Some value
                            halt ()
                            Unchecked.defaultof<_> (* return value unsed in ForEach context *) })
                |> fun head -> head.Value

            [<CompiledName("TryItem")>]
            let tryItem i (source:ISeq<'T>) =
                if i < 0 then None else
                source.Compose (SkipFactory(i, fun _ _ -> ()))
                |> tryHead

            [<CompiledName("IterateIndexed")>]
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

            [<CompiledName("Exists")>]
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

            [<CompiledName("Contains")>]
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

            [<CompiledName("ForAll")>]
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

            [<CompiledName("Filter")>]
            let inline filter<'T> (f:'T->bool) (source:ISeq<'T>) : ISeq<'T> =
                source |> compose { new SeqFactory<'T,'T>() with
                    member __.Create _ _ next =
                        upcast { new SeqComponentSimple<'T,'V>(Upcast.iCompletionChaining next) with
                            member __.ProcessNext input =
                                if f input then TailCall.avoid (next.ProcessNext input)
                                else false } }

            [<CompiledName("Map")>]
            let inline map<'T,'U> (f:'T->'U) (source:ISeq<'T>) : ISeq<'U> =
                source |> compose { new SeqFactory<'T,'U>() with
                    member __.Create _ _ next =
                        upcast { new SeqComponentSimple<'T,'V>(Upcast.iCompletionChaining next) with
                            member __.ProcessNext input =
                                TailCall.avoid (next.ProcessNext (f input)) } }

            [<CompiledName("MapIndexed")>]
            let inline mapi f source =
                source |> compose { new SeqFactory<'T,'U>() with
                    member __.Create _ _ next =
                        upcast { new SeqComponentSimpleValue<'T,'V,int>(Upcast.iCompletionChaining next, -1) with
                            override this.ProcessNext (input:'T) : bool =
                                this.Value <- this.Value  + 1
                                TailCall.avoid (next.ProcessNext (f this.Value input)) } }

            let mapi_adapt (f:OptimizedClosures.FSharpFunc<_,_,_>) source =
                source |> compose { new SeqFactory<'T,'U>() with
                    member __.Create _ _ next =
                        upcast { new SeqComponentSimpleValue<'T,'V,int>(Upcast.iCompletionChaining next, -1) with
                            override this.ProcessNext (input:'T) : bool =
                                this.Value <- this.Value  + 1
                                TailCall.avoid (next.ProcessNext (f.Invoke (this.Value, input))) } }

            [<CompiledName("Choose")>]
            let inline choose (f:'T->option<'U>) (source:ISeq<'T>) : ISeq<'U> =
                source |> compose { new SeqFactory<'T,'U>() with
                    member __.Create _ _ next =
                        upcast { new SeqComponentSimple<'T,'V>(Upcast.iCompletionChaining next) with
                            member __.ProcessNext input =
                                match f input with
                                | Some value -> TailCall.avoid (next.ProcessNext value)
                                | None       -> false } }


            [<CompiledName("Indexed")>]
            let inline indexed source =
                mapi (fun i x -> i,x) source

            [<CompiledName("TryPick")>]
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

            [<CompiledName("TryFind")>]
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