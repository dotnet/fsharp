// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Collections
    #nowarn "52" // The value has been copied to ensure the original is not mutated by this operation

    open System
    open System.Diagnostics
    open System.Collections
    open System.Collections.Generic
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
    open Microsoft.FSharp.Core.Operators
    open Microsoft.FSharp.Control
    open Microsoft.FSharp.Collections

    module internal IEnumerator =

      let noReset() = raise (new System.NotSupportedException(SR.GetString(SR.resetNotSupported)))
      let notStarted() = raise (new System.InvalidOperationException(SR.GetString(SR.enumerationNotStarted)))
      let alreadyFinished() = raise (new System.InvalidOperationException(SR.GetString(SR.enumerationAlreadyFinished)))
      let check started = if not started then notStarted()
      let dispose (r : System.IDisposable) = r.Dispose()

      let cast (e : IEnumerator) : IEnumerator<'T> =
          { new IEnumerator<'T> with
                member x.Current = unbox<'T> e.Current
            interface IEnumerator with
                member x.Current = unbox<'T> e.Current :> obj
                member x.MoveNext() = e.MoveNext()
                member x.Reset() = noReset()
            interface System.IDisposable with
                member x.Dispose() =
                    match e with
                    | :? System.IDisposable as e -> e.Dispose()
                    | _ -> ()   }

      /// A concrete implementation of an enumerator that returns no values
      [<Sealed>]
      type EmptyEnumerator<'T>() =
          let mutable started = false
          interface IEnumerator<'T> with
                member x.Current =
                  check started
                  (alreadyFinished() : 'T)

          interface System.Collections.IEnumerator with
              member x.Current =
                  check started
                  (alreadyFinished() : obj)
              member x.MoveNext() =
                  if not started then started <- true
                  false
              member x.Reset() = noReset()
          interface System.IDisposable with
                member x.Dispose() = ()
                
      let Empty<'T> () = (new EmptyEnumerator<'T>() :> IEnumerator<'T>)

      [<NoEquality; NoComparison>]
      type EmptyEnumerable<'T> =
            | EmptyEnumerable
            interface IEnumerable<'T> with
                member x.GetEnumerator() = Empty<'T>()
            interface IEnumerable with
                member x.GetEnumerator() = (Empty<'T>() :> IEnumerator)

      let readAndClear r =
          lock r (fun () -> match !r with None -> None | Some _ as res -> r := None; res)

      let generateWhileSome openf compute closef : IEnumerator<'U> =
          let started = ref false
          let curr = ref None
          let state = ref (Some(openf()))
          let getCurr() =
              check !started
              match !curr with None -> alreadyFinished() | Some x -> x
          let start() = if not !started then (started := true)

          let dispose() = readAndClear state |> Option.iter closef
          let finish() = (try dispose() finally curr := None)
          {  new IEnumerator<'U> with
                 member x.Current = getCurr()
             interface IEnumerator with
                 member x.Current = box (getCurr())
                 member x.MoveNext() =
                     start()
                     match !state with
                     | None -> false (* we started, then reached the end, then got another MoveNext *)
                     | Some s ->
                         match (try compute s with e -> finish(); reraise()) with
                         | None -> finish(); false
                         | Some _ as x -> curr := x; true

                 member x.Reset() = noReset()
             interface System.IDisposable with
                 member x.Dispose() = dispose() }

      [<Sealed>]
      type Singleton<'T>(v:'T) =
          let mutable started = false
          interface IEnumerator<'T> with
                member x.Current = v
          interface IEnumerator with
              member x.Current = box v
              member x.MoveNext() = if started then false else (started <- true; true)
              member x.Reset() = noReset()
          interface System.IDisposable with
              member x.Dispose() = ()

      let Singleton x = (new Singleton<'T>(x) :> IEnumerator<'T>)

      let EnumerateThenFinally f (e : IEnumerator<'T>) =
          { new IEnumerator<'T> with
                member x.Current = e.Current
            interface IEnumerator with
                member x.Current = (e :> IEnumerator).Current
                member x.MoveNext() = e.MoveNext()
                member x.Reset() = noReset()
            interface System.IDisposable with
                member x.Dispose() =
                    try
                        e.Dispose()
                    finally
                        f()
          }

      let inline checkNonNull argName arg =
            match box arg with
            | null -> nullArg argName
            | _ -> ()

      let mkSeq f =
            { new IEnumerable<'U> with
                member x.GetEnumerator() = f()
              interface IEnumerable with
                member x.GetEnumerator() = (f() :> IEnumerator) }

namespace Microsoft.FSharp.Collections.SeqComposition
  open System
  open System.Collections
  open System.Collections.Generic
  open Microsoft.FSharp.Core
  open Microsoft.FSharp.Collections
  open Microsoft.FSharp.Collections.IEnumerator
  open Microsoft.FSharp.Collections.SeqComposition
  open Microsoft.FSharp.Collections.SeqComposition.Factories
  open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
  open Microsoft.FSharp.Primitives.Basics
  open Microsoft.FSharp.Control

  module Core = 
    [<Struct; NoComparison; NoEquality>]
    type NoValue = struct end

    module internal Upcast =
        // The f# compiler outputs unnecessary unbox.any calls in upcasts. If this functionality
        // is fixed with the compiler then these functions can be removed.
        let inline seq<'T,'seq when 'seq :> ISeq<'T> and 'seq : not struct> (t:'seq) : ISeq<'T> = (# "" t : ISeq<'T> #)
        let inline enumerable<'T,'enumerable when 'enumerable :> IEnumerable<'T> and 'enumerable : not struct> (t:'enumerable) : IEnumerable<'T> = (# "" t : IEnumerable<'T> #)
        let inline enumerableNonGeneric<'enumerable when 'enumerable :> IEnumerable and 'enumerable : not struct> (t:'enumerable) : IEnumerable = (# "" t : IEnumerable #)
        let inline enumerator<'T,'enumerator when 'enumerator :> IEnumerator<'T> and 'enumerator : not struct> (t:'enumerator) : IEnumerator<'T> = (# "" t : IEnumerator<'T> #)
        let inline enumeratorNonGeneric<'enumerator when 'enumerator :> IEnumerator and 'enumerator : not struct> (t:'enumerator) : IEnumerator = (# "" t : IEnumerator #)
        let inline outOfBand<'outOfBand when 'outOfBand :> IOutOfBand and 'outOfBand : not struct> (t:'outOfBand) : IOutOfBand = (# "" t : IOutOfBand #)

    type ISkipable =
        // Seq.init(Infinite)? lazily uses Current. The only ISeq component that can do that is Skip
        // and it can only do it at the start of a sequence
        abstract CanSkip : unit -> bool

    let createFold (factory:TransformFactory<_,_>) (folder:Folder<_,_>) pipeIdx  =
        factory.Compose (Upcast.outOfBand folder) pipeIdx folder

    module Fold =
        // The consumers of IIterate are the execute and exeuteThin methods. IIterate is passed
        // as a generic argument. The types that implement IIterate are value types. This combination
        // means that the runtime will "inline" the methods. The alternatives to this were that the
        // code in execute/executeThin were duplicated for each of the Fold types, or we turned the
        // types back into normal functions and curried them then we would be creating garbage
        // each time one of these were called. This has been an optimization to minimize the impact
        // on very small collections.
        type IIterate<'T> =
            abstract Iterate<'U,'Result,'State> : outOfBand:Folder<'U,'Result> -> consumer:Activity<'T,'U> -> unit

        [<Struct;NoComparison;NoEquality>]
        type IterateEnumerable<'T> (enumerable:IEnumerable<'T>) =
            interface IIterate<'T> with
                member __.Iterate (outOfBand:Folder<'U,'Result>) (consumer:Activity<'T,'U>) =
                    use enumerator = enumerable.GetEnumerator ()
                    let rec iterate () =
                        if outOfBand.HaltedIdx = 0 && enumerator.MoveNext () then  
                            consumer.ProcessNext enumerator.Current |> ignore
                            iterate ()
                    iterate ()

        [<Struct;NoComparison;NoEquality>]
        type IterateArray<'T> (array:array<'T>) =
            interface IIterate<'T> with
                member __.Iterate (outOfBand:Folder<'U,'Result>) (consumer:Activity<'T,'U>) =
                    let array = array
                    let rec iterate idx =
                        if outOfBand.HaltedIdx = 0 && idx < array.Length then  
                            consumer.ProcessNext array.[idx] |> ignore
                            iterate (idx+1)
                    iterate 0

        [<Struct;NoComparison;NoEquality>]
        type IterateResizeArray<'T> (array:ResizeArray<'T>) =
            interface IIterate<'T> with
                member __.Iterate (outOfBand:Folder<'U,'Result>) (consumer:Activity<'T,'U>) =
                    let array = array
                    let rec iterate idx =
                        if outOfBand.HaltedIdx = 0 && idx < array.Count then  
                            consumer.ProcessNext array.[idx] |> ignore
                            iterate (idx+1)
                    iterate 0

        [<Struct;NoComparison;NoEquality>]
        type IterateList<'T> (alist:list<'T>) =
            interface IIterate<'T> with
                member __.Iterate (outOfBand:Folder<'U,'Result>) (consumer:Activity<'T,'U>) =
                    let rec iterate lst =
                        match lst with
                        | hd :: tl when outOfBand.HaltedIdx = 0 ->
                            consumer.ProcessNext hd |> ignore
                            iterate tl
                        | _ -> ()
                    iterate alist

        [<Struct;NoComparison;NoEquality>]
        type IterateSingleton<'T> (item:'T) =
            interface IIterate<'T> with
                member __.Iterate (outOfBand:Folder<'U,'Result>) (consumer:Activity<'T,'U>) =
                    if outOfBand.HaltedIdx = 0 then
                        consumer.ProcessNext item |> ignore

        [<Struct;NoComparison;NoEquality>]
        type IterateUnfold<'S,'T> (generator:'S->option<'T*'S>, state:'S) =
            interface IIterate<'T> with
                member __.Iterate (outOfBand:Folder<'U,'Result>) (consumer:Activity<'T,'U>) =
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
                member __.Iterate (outOfBand:Folder<'U,'Result>) (consumer:Activity<'T,'U>) =
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
        let execute (createFolder:PipeIdx->Folder<'U,'Result>) (transformFactory:TransformFactory<'T,'U>) pipeIdx (executeOn:#IIterate<'T>) =
            let result = createFolder (pipeIdx+1)
            let consumer = createFold transformFactory result pipeIdx
            try
                executeOn.Iterate result consumer
                consumer.ChainComplete result.HaltedIdx
                result.Result
            finally
                consumer.ChainDispose ()

        // executeThin is a specialization of execute, provided as a performance optimization, that can
        // be used when a sequence has been wrapped in an ISeq, but hasn't had an items added to its pipeline
        // i.e. a container that has ISeq.ofSeq applied. 
        let executeThin (createFolder:PipeIdx->Folder<'T,'Result>) (executeOn:#IIterate<'T>) =
            let result = createFolder 1
            try
                executeOn.Iterate result result
                result.ChainComplete result.HaltedIdx
                result.Result
            finally
                result.ChainDispose ()

        let executeConcat<'T,'U,'Result,'State,'Collection when 'Collection :> ISeq<'T>> (createFolder:PipeIdx->Folder<'U,'Result>) (transformFactory:TransformFactory<'T,'U>) pipeIdx (sources:ISeq<'Collection>) =
            let result = createFolder (pipeIdx+1)
            let consumer = createFold transformFactory result pipeIdx
            try
                let common  =
                    { new Folder<'T,NoValue>(Unchecked.defaultof<_>) with
                                override me.ProcessNext value = consumer.ProcessNext value }

                sources.Fold (fun _ ->
                    { new Folder<'Collection,NoValue>(Unchecked.defaultof<_>) with
                        override me.ProcessNext value =
                            value.Fold (fun _ -> common) |> ignore
                            me.HaltedIdx <- common.HaltedIdx
                            Unchecked.defaultof<_> (* return value unused in Fold context *) }) |> ignore

                consumer.ChainComplete result.HaltedIdx
                result.Result
            finally
                result.ChainDispose ()

        let executeConcatThin<'T,'Result,'State,'Collection when 'Collection :> ISeq<'T>> (createFolder:PipeIdx->Folder<'T,'Result>) (sources:ISeq<'Collection>) =
            let result = createFolder 1
            try
                let common =
                    { new Folder<'T,NoValue>(Unchecked.defaultof<_>) with
                        override me.ProcessNext value = result.ProcessNext value }

                sources.Fold (fun _ ->
                    { new Folder<'Collection,NoValue>(Unchecked.defaultof<_>) with
                        override me.ProcessNext value =
                            value.Fold (fun _ -> common) |> ignore
                            me.HaltedIdx <- common.HaltedIdx
                            Unchecked.defaultof<_> (* return value unused in Fold context *) }) |> ignore

                result.ChainComplete result.HaltedIdx
                result.Result
            finally
                result.ChainDispose ()

    type SeqProcessNextStates =
    | InProcess  = 0
    | NotStarted = 1
    | Finished   = 2

    type Result<'T>() =
        inherit Folder<'T,'T>(Unchecked.defaultof<'T>)

        member val SeqState = SeqProcessNextStates.NotStarted with get, set

        override this.ProcessNext (input:'T) : bool =
            this.Result <- input
            true

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
                activity.ChainDispose ()

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
                activity.ChainComplete result.HaltedIdx
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
                    activity.ChainDispose ()

    type ConcatEnumerator<'T,'U,'Collection when 'Collection :> IEnumerable<'T>> (sources:IEnumerable<'Collection>, activity:Activity<'T,'U>, result:Result<'U>) =
        inherit EnumeratorBase<'U>(result, activity)

        let main = sources.GetEnumerator ()

        let mutable active = EmptyEnumerators.Element

        let rec moveNext () =
            if result.HaltedIdx <> 0 then false
            else
                if active.MoveNext () then
                    if activity.ProcessNext active.Current then
                        true
                    else
                        moveNext ()
                elif main.MoveNext () then
                    active.Dispose ()
                    active <- main.Current.GetEnumerator ()
                    moveNext ()
                else
                    result.SeqState <- SeqProcessNextStates.Finished
                    activity.ChainComplete result.HaltedIdx
                    false

        interface IEnumerator with
            member __.MoveNext () =
                result.SeqState <- SeqProcessNextStates.InProcess
                moveNext ()

        interface IDisposable with
            member __.Dispose () =
                try
                    main.Dispose ()
                    active.Dispose ()
                finally
                    activity.ChainDispose ()

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
                activity.ChainComplete result.HaltedIdx
                false

        interface IEnumerator with
            member __.MoveNext () =
                result.SeqState <- SeqProcessNextStates.InProcess
                moveNext list

    let length (source:ISeq<_>) =
        source.Fold (fun _ ->
            { new Folder<'T,int>(0) with
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
            // fsharp doesn't allow abstract interface methods
            member this.GetEnumerator () : IEnumerator<'T> = derivedClassShouldImplement ()

        interface ISeq<'T> with
            // fsharp doesn't allow abstract interface methods
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

            member this.Fold<'Result> (f:PipeIdx->Folder<'U,'Result>) =
                Fold.execute f current pipeIdx (Fold.IterateEnumerable enumerable)

    and ConcatEnumerable<'T,'U,'Collection when 'Collection :> ISeq<'T>> (sources:ISeq<'Collection>, current:TransformFactory<'T,'U>, pipeIdx:PipeIdx) =
        inherit EnumerableBase<'U>()

        interface IEnumerable<'U> with
            member __.GetEnumerator () : IEnumerator<'U> =
                let result = Result<'U> ()
                Upcast.enumerator (new ConcatEnumerator<'T,'U,'Collection>(sources, createFold current result pipeIdx, result))

        interface ISeq<'U> with
            member this.PushTransform (next:TransformFactory<'U,'V>) : ISeq<'V> =
                Upcast.seq (new ConcatEnumerable<'T,'V,'Collection>(sources, ComposedFactory.Combine current next, pipeIdx+1))

            member this.Fold<'Result> (f:PipeIdx->Folder<'U,'Result>) =
                Fold.executeConcat f current pipeIdx sources

    and ThinConcatEnumerable<'T, 'Sources, 'Collection when 'Collection :> ISeq<'T>> (sources:'Sources, preEnumerate:'Sources->ISeq<'Collection>) =
        inherit EnumerableBase<'T>()

        interface IEnumerable<'T> with
            member this.GetEnumerator () : IEnumerator<'T> =
                let result = Result<'T> ()
                Upcast.enumerator (new ConcatEnumerator<'T,'T,'Collection> (preEnumerate sources, createFold IdentityFactory.Instance result 1, result))

        interface ISeq<'T> with
            member this.PushTransform (next:TransformFactory<'T,'U>) : ISeq<'U> =
                Upcast.seq (ConcatEnumerable<'T,'V,'Collection>(preEnumerate sources, next, 1))

            member this.Fold<'Result> (f:PipeIdx->Folder<'T,'Result>) =
                Fold.executeConcatThin f (preEnumerate sources)

    and AppendEnumerable<'T> (sources:list<ISeq<'T>>) =
        inherit ThinConcatEnumerable<'T, list<ISeq<'T>>, ISeq<'T>>(sources, fun sources -> Upcast.seq (ThinListEnumerable<ISeq<'T>>(List.rev sources)))

        override this.Append source =
            Upcast.seq (AppendEnumerable (source::sources))

    and ThinListEnumerable<'T>(alist:list<'T>) =
        inherit EnumerableBase<'T>()

        override __.Length () = alist.Length

        interface IEnumerable<'T> with
            member __.GetEnumerator () = (Upcast.enumerable alist).GetEnumerator ()

        interface ISeq<'T> with
            member __.PushTransform (next:TransformFactory<'T,'U>) : ISeq<'U> =
                Upcast.seq (new ListEnumerable<'T,'U>(alist, next, 1))

            member __.Fold<'Result> (f:PipeIdx->Folder<'T,'Result>) =
                Fold.executeThin f (Fold.IterateList alist)

    and ListEnumerable<'T,'U>(alist:list<'T>, transformFactory:TransformFactory<'T,'U>, pipeIdx:PipeIdx) =
        inherit EnumerableBase<'U>()

        interface IEnumerable<'U> with
            member this.GetEnumerator () : IEnumerator<'U> =
                let result = Result<'U> ()
                Upcast.enumerator (new ListEnumerator<'T,'U>(alist, createFold transformFactory result pipeIdx, result))

        interface ISeq<'U> with
            member __.PushTransform (next:TransformFactory<'U,'V>) : ISeq<'V> =
                Upcast.seq (new ListEnumerable<'T,'V>(alist, ComposedFactory.Combine transformFactory next, pipeIdx+1))

            member this.Fold<'Result> (f:PipeIdx->Folder<'U,'Result>) =
                Fold.execute f transformFactory pipeIdx (Fold.IterateList alist)

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
            member __.GetEnumerator () = enumerable.GetEnumerator ()

        interface ISeq<'T> with
            member __.PushTransform (next:TransformFactory<'T,'U>) : ISeq<'U> =
                Upcast.seq (new VanillaEnumerable<'T,'U>(enumerable, next, 1))

            member __.Fold<'Result> (f:PipeIdx->Folder<'T,'Result>) =
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

            member __.Fold<'Result> (f:PipeIdx->Folder<'T,'Result>) =
                (delayed()).Fold f

    type EmptyEnumerable<'T> private () =
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

            member this.Fold<'Result> (f:PipeIdx->Folder<'T,'Result>) =
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
                activity.ChainComplete result.HaltedIdx
                false

        interface IEnumerator with
            member __.MoveNext () =
                result.SeqState <- SeqProcessNextStates.InProcess
                moveNext ()

    type ArrayEnumerable<'T,'U>(array:array<'T>, transformFactory:TransformFactory<'T,'U>, pipeIdx:PipeIdx) =
        inherit EnumerableBase<'U>()

        interface IEnumerable<'U> with
            member __.GetEnumerator () : IEnumerator<'U> =
                let result = Result<'U> ()
                Upcast.enumerator (new ArrayEnumerator<'T,'U>(array, createFold transformFactory result pipeIdx, result))

        interface ISeq<'U> with
            member __.PushTransform (next:TransformFactory<'U,'V>) : ISeq<'V> =
                Upcast.seq (new ArrayEnumerable<'T,'V>(array, ComposedFactory.Combine transformFactory next, pipeIdx+1))

            member __.Fold<'Result> (f:PipeIdx->Folder<'U,'Result>) =
                Fold.execute f transformFactory pipeIdx (Fold.IterateArray array)

    type ThinArrayEnumerable<'T>(array:array<'T>) =
        inherit EnumerableBase<'T>()

        override __.Length () = array.Length

        interface IEnumerable<'T> with
            member __.GetEnumerator () = (Upcast.enumerable array).GetEnumerator ()

        interface ISeq<'T> with
            member __.PushTransform (next:TransformFactory<'T,'U>) : ISeq<'U> =
                Upcast.seq (new ArrayEnumerable<'T,'U>(array, next, 1))

            member __.Fold<'Result> (f:PipeIdx->Folder<'T,'Result>) =
                Fold.executeThin f (Fold.IterateArray array)

    type SingletonEnumerable<'T>(item:'T) =
        inherit EnumerableBase<'T>()

        override __.Length () = 1

        interface IEnumerable<'T> with
            member this.GetEnumerator () = Upcast.enumerator (new Singleton<'T>(item))

        interface ISeq<'T> with
            member __.PushTransform (next:TransformFactory<'T,'U>) : ISeq<'U> =
                Upcast.seq (new ArrayEnumerable<'T,'U>([|item|], next, 1))

            member this.Fold<'Result> (f:PipeIdx->Folder<'T,'Result>) =
                Fold.executeThin f (Fold.IterateSingleton item)

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
                activity.ChainComplete result.HaltedIdx
                false

        interface IEnumerator with
            member __.MoveNext () =
                result.SeqState <- SeqProcessNextStates.InProcess
                moveNext ()

    type ResizeArrayEnumerable<'T,'U>(resizeArray:ResizeArray<'T>, transformFactory:TransformFactory<'T,'U>, pipeIdx:PipeIdx) =
        inherit EnumerableBase<'U>()

        interface IEnumerable<'U> with
            member this.GetEnumerator () : IEnumerator<'U> =
                let result = Result<'U> ()
                Upcast.enumerator (new ResizeArrayEnumerator<'T,'U>(resizeArray, createFold transformFactory result pipeIdx, result))

        interface ISeq<'U> with
            member __.PushTransform (next:TransformFactory<'U,'V>) : ISeq<'V> =
                Upcast.seq (new ResizeArrayEnumerable<'T,'V>(resizeArray, ComposedFactory.Combine transformFactory next, pipeIdx+1))

            member __.Fold<'Result> (f:PipeIdx->Folder<'U,'Result>) =
                Fold.execute f transformFactory pipeIdx (Fold.IterateResizeArray resizeArray)

    type ThinResizeArrayEnumerable<'T>(resizeArray:ResizeArray<'T>) =
        inherit EnumerableBase<'T>()

        override __.Length () = resizeArray.Count

        interface IEnumerable<'T> with
            member __.GetEnumerator () = (Upcast.enumerable resizeArray).GetEnumerator ()

        interface ISeq<'T> with
            member __.PushTransform (next:TransformFactory<'T,'U>) : ISeq<'U> =
                Upcast.seq (new ResizeArrayEnumerable<'T,'U>(resizeArray, next, 1))

            member __.Fold<'Result> (f:PipeIdx->Folder<'T,'Result>) =
                Fold.executeThin f (Fold.IterateResizeArray resizeArray)

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

            member this.Fold<'Result> (f:PipeIdx->Folder<'U,'Result>) =
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
                activity.ChainComplete result.HaltedIdx
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

            member this.Fold<'Result> (createResult:PipeIdx->Folder<'U,'Result>) =
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

            member this.Fold<'Result> (f:PipeIdx->Folder<'T,'Result>) =
                Fold.executeThin f (Fold.IterateEnumerable (Upcast.enumerable this))

namespace Microsoft.FSharp.Core.CompilerServices

    open System
    open System.Diagnostics
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
    open Microsoft.FSharp.Core.Operators
    open Microsoft.FSharp.Control
    open Microsoft.FSharp.Collections
    open Microsoft.FSharp.Collections.IEnumerator
    open Microsoft.FSharp.Primitives.Basics
    open System.Collections
    open System.Collections.Generic
    open Microsoft.FSharp.Collections.SeqComposition
    open Microsoft.FSharp.Collections.SeqComposition.Factories
    open Microsoft.FSharp.Collections.SeqComposition.Core

    module RuntimeHelpers =

        [<Struct; NoComparison; NoEquality>]
        type internal StructBox<'T when 'T:equality>(value:'T) =
            member x.Value = value
            static member Comparer =
                let gcomparer = HashIdentity.Structural<'T>
                { new IEqualityComparer<StructBox<'T>> with
                       member __.GetHashCode(v) = gcomparer.GetHashCode(v.Value)
                       member __.Equals(v1,v2) = gcomparer.Equals(v1.Value,v2.Value) }

        let Generate openf compute closef =
            mkSeq (fun () -> IEnumerator.generateWhileSome openf compute closef)

        let GenerateUsing (openf : unit -> ('U :> System.IDisposable)) compute =
            Generate openf compute (fun (s:'U) -> s.Dispose())

        let EnumerateFromFunctions opener moveNext current =
            Generate
                opener
                (fun x -> if moveNext x then Some(current x) else None)
                (fun x -> match box(x) with :? System.IDisposable as id -> id.Dispose() | _ -> ())

        // A family of enumerators that can have additional 'finally' actions added to the enumerator through
        // the use of mutation. This is used to 'push' the disposal action for a 'use' into the next enumerator.
        // For example,
        //    seq { use x = ...
        //          while ... }
        // results in the 'while' loop giving an adjustable enumerator. This is then adjusted by adding the disposal action
        // from the 'use' into the enumerator. This means that we avoid constructing a two-deep enumerator chain in this
        // common case.
        type IFinallyEnumerator =
            abstract AppendFinallyAction : (unit -> unit) -> unit

        /// A concrete implementation of IEnumerable that adds the given compensation to the "Dispose" chain of any
        /// enumerators returned by the enumerable.
        [<Sealed>]
        type FinallyEnumerable<'T>(compensation: unit -> unit, restf: unit -> seq<'T>) =
            interface IEnumerable<'T> with
                member x.GetEnumerator() =
                    try
                        let ie = restf().GetEnumerator()
                        match ie with
                        | :? IFinallyEnumerator as a ->
                            a.AppendFinallyAction(compensation)
                            ie
                        | _ ->
                            IEnumerator.EnumerateThenFinally compensation ie
                    with e ->
                        compensation()
                        reraise()
            interface IEnumerable with
                member x.GetEnumerator() = ((x :> IEnumerable<'T>).GetEnumerator() :> IEnumerator)

        /// An optimized object for concatenating a sequence of enumerables
        [<Sealed>]
        type ConcatEnumerator<'T,'U when 'U :> seq<'T>>(sources: seq<'U>) =
            let mutable outerEnum = sources.GetEnumerator()
            let mutable currInnerEnum = IEnumerator.Empty()

            let mutable started = false
            let mutable finished = false
            let mutable compensations = []

            [<DefaultValue(false)>] // false = unchecked
            val mutable private currElement : 'T

            member x.Finish() =
                finished <- true
                try
                    match currInnerEnum with
                    | null -> ()
                    | _ ->
                        try
                            currInnerEnum.Dispose()
                        finally
                            currInnerEnum <- null
                finally
                    try
                        match outerEnum with
                        | null -> ()
                        | _ ->
                            try
                                outerEnum.Dispose()
                            finally
                                outerEnum <- null
                    finally
                        let rec iter comps =
                            match comps with
                            |   [] -> ()
                            |   h::t ->
                                    try h() finally iter t
                        try
                            compensations |> List.rev |> iter
                        finally
                            compensations <- []

            member x.GetCurrent() =
                IEnumerator.check started
                if finished then IEnumerator.alreadyFinished() else x.currElement

            interface IFinallyEnumerator with
                member x.AppendFinallyAction(f) =
                    compensations <- f :: compensations

            interface IEnumerator<'T> with
                member x.Current = x.GetCurrent()

            interface IEnumerator with
                member x.Current = box (x.GetCurrent())

                member x.MoveNext() =
                   if not started then (started <- true)
                   if finished then false
                   else
                      let rec takeInner () =
                        // check the inner list
                        if currInnerEnum.MoveNext() then
                            x.currElement <- currInnerEnum.Current
                            true
                        else
                            // check the outer list
                            let rec takeOuter() =
                                if outerEnum.MoveNext() then
                                    let ie = outerEnum.Current
                                    // Optimization to detect the statically-allocated empty IEnumerables
                                    match box ie with
                                    | :? EmptyEnumerable<'T> ->
                                         // This one is empty, just skip, don't call GetEnumerator, try again
                                         takeOuter()
                                    | _ ->
                                         // OK, this one may not be empty.
                                         // Don't forget to dispose of the enumerator for the inner list now we're done with it
                                         currInnerEnum.Dispose()
                                         currInnerEnum <- ie.GetEnumerator()
                                         takeInner ()
                                else
                                    // We're done
                                    x.Finish()
                                    false
                            takeOuter()
                      takeInner ()

                member x.Reset() = IEnumerator.noReset()

            interface System.IDisposable with
                member x.Dispose() =
                    if not finished then
                        x.Finish()

        let EnumerateUsing (resource : 'T :> System.IDisposable) (rest: 'T -> #seq<'U>) =
            (FinallyEnumerable((fun () -> match box resource with null -> () | _ -> resource.Dispose()),
                               (fun () -> rest resource :> seq<_>)) :> seq<_>)

        let mkConcatSeq (sources: seq<'U :> seq<'T>>) =
            mkSeq (fun () -> new ConcatEnumerator<_,_>(sources) :> IEnumerator<'T>)

        let EnumerateWhile (g : unit -> bool) (b: seq<'T>) : seq<'T> =
            let started = ref false
            let curr = ref None
            let getCurr() =
                IEnumerator.check !started
                match !curr with None -> IEnumerator.alreadyFinished() | Some x -> x
            let start() = if not !started then (started := true)

            let finish() = (curr := None)
            mkConcatSeq
               (mkSeq (fun () ->
                    { new IEnumerator<_> with
                          member x.Current = getCurr()
                       interface IEnumerator with
                          member x.Current = box (getCurr())
                          member x.MoveNext() =
                               start()
                               let keepGoing = (try g() with e -> finish (); reraise ()) in
                               if keepGoing then
                                   curr := Some(b); true
                               else
                                   finish(); false
                          member x.Reset() = IEnumerator.noReset()
                       interface System.IDisposable with
                          member x.Dispose() = () }))

        let EnumerateThenFinally (rest : seq<'T>) (compensation : unit -> unit)  =
            (FinallyEnumerable(compensation, (fun () -> rest)) :> seq<_>)

        let CreateEvent (add : 'Delegate -> unit) (remove : 'Delegate -> unit) (create : (obj -> 'Args -> unit) -> 'Delegate ) :IEvent<'Delegate,'Args> =
            // Note, we implement each interface explicitly: this works around a bug in the CLR
            // implementation on CompactFramework 3.7, used on Windows Phone 7
            { new obj() with
                  member x.ToString() = "<published event>"
              interface IEvent<'Delegate,'Args>
              interface IDelegateEvent<'Delegate> with
                 member x.AddHandler(h) = add h
                 member x.RemoveHandler(h) = remove h
              interface System.IObservable<'Args> with
                 member x.Subscribe(r:IObserver<'Args>) =
                     let h = create (fun _ args -> r.OnNext(args))
                     add h
                     { new System.IDisposable with
                          member x.Dispose() = remove h } }


    [<AbstractClass>]
    type GeneratedSequenceBase<'T>() =
        inherit EnumerableBase<'T>()

        let mutable redirectTo : GeneratedSequenceBase<'T> = Unchecked.defaultof<_>
        let mutable redirect : bool = false

        member internal x.GetCurrent () = if redirect then redirectTo.LastGenerated else x.LastGenerated

        abstract GetFreshEnumerator : unit -> IEnumerator<'T>
        abstract GenerateNext : next:byref<IEnumerable<'T>> -> int // 0 = Stop, 1 = Yield, 2 = Goto
        abstract Close: unit -> unit
        abstract CheckClose: bool
        abstract LastGenerated : 'T

        //[<System.Diagnostics.DebuggerNonUserCode; System.Diagnostics.DebuggerStepThroughAttribute>]
        member x.MoveNextImpl() =
             let active =
                 if redirect then redirectTo
                 else x
             let mutable target = null
             match active.GenerateNext(&target) with
             | 1 ->
                 true
             | 2 ->
                 match target.GetEnumerator() with
                 | :? GeneratedSequenceBase<'T> as g when not active.CheckClose ->
                     redirectTo <- g
                 | e ->
                     redirectTo <-
                           { new GeneratedSequenceBase<'T>() with
                                 member x.GetFreshEnumerator() = e
                                 member x.GenerateNext(_) = if e.MoveNext() then 1 else 0
                                 member x.Close() = try e.Dispose() finally active.Close()
                                 member x.CheckClose = true
                                 member x.LastGenerated = e.Current }
                 redirect <- true
                 x.MoveNextImpl()
             | _ (* 0 *)  ->
                 false

        interface IEnumerable<'T> with
            member x.GetEnumerator() = x.GetFreshEnumerator()
        interface IEnumerable with
            member x.GetEnumerator() = (x.GetFreshEnumerator() :> IEnumerator)
        interface IEnumerator<'T> with
            member x.Current = x.GetCurrent ()
            member x.Dispose() = if redirect then redirectTo.Close() else x.Close()
        interface IEnumerator with
            member x.Current = box (if redirect then redirectTo.LastGenerated else x.LastGenerated)

            //[<System.Diagnostics.DebuggerNonUserCode; System.Diagnostics.DebuggerStepThroughAttribute>]
            member x.MoveNext() = x.MoveNextImpl()

            member x.Reset() = raise <| new System.NotSupportedException()

        interface ISeq<'T> with
            member this.PushTransform<'U> (next:TransformFactory<'T,'U>) : ISeq<'U> =
                Upcast.seq (new GeneratedSequenceBaseEnumerable<'T,'U>(this, next, 1))

            member this.Fold<'Result> (createFolder:PipeIdx->Folder<'T,'Result>) =
                let result = createFolder 1
                try
                    use maybeGeneratedSequenceBase = this.GetFreshEnumerator ()
                    match maybeGeneratedSequenceBase with
                    | :? GeneratedSequenceBase<'T> as e -> // avoids two virtual function calls
                        while result.HaltedIdx = 0 && e.MoveNextImpl () do
                            result.ProcessNext (e.GetCurrent ()) |> ignore 
                    | e ->
                        while result.HaltedIdx = 0 && e.MoveNext () do
                            result.ProcessNext e.Current |> ignore

                    result.ChainComplete result.HaltedIdx
                    result.Result
                finally
                    result.ChainDispose ()

        override this.Length () =
            use maybeGeneratedSequenceBase = this.GetFreshEnumerator ()
            let mutable count = 0
            match maybeGeneratedSequenceBase with
            | :? GeneratedSequenceBase<'T> as e ->
                while e.MoveNextImpl () do
                    count <- count + 1
            | e ->
                while e.MoveNext () do
                    count <- count + 1
            count

    and GeneratedSequenceBaseEnumerable<'T,'U>(generatedSequence:GeneratedSequenceBase<'T>, current:TransformFactory<'T,'U>, pipeIdx:PipeIdx) =
        inherit EnumerableBase<'U>()

        interface IEnumerable<'U> with
            member this.GetEnumerator () : IEnumerator<'U> =
                let result = Result<'U> ()
                Upcast.enumerator (new VanillaEnumerator<'T,'U>(generatedSequence.GetFreshEnumerator(), current.Compose (Upcast.outOfBand result) pipeIdx result, result))

        interface ISeq<'U> with
            member __.PushTransform (next:TransformFactory<'U,'V>) : ISeq<'V> =
                Upcast.seq (new GeneratedSequenceBaseEnumerable<'T,'V>(generatedSequence, ComposedFactory.Combine current next, pipeIdx+1))

            member this.Fold<'Result> (createFolder:PipeIdx->Folder<'U,'Result>) =
                let result = createFolder (pipeIdx+1)
                let consumer = current.Compose (Upcast.outOfBand result) pipeIdx result
                try
                    use maybeGeneratedSequenceBase = generatedSequence.GetFreshEnumerator ()
                    match maybeGeneratedSequenceBase with
                    | :? GeneratedSequenceBase<'T> as e ->
                        while result.HaltedIdx = 0 && e.MoveNextImpl () do
                            consumer.ProcessNext (e.GetCurrent ()) |> ignore
                    | e ->
                        while result.HaltedIdx = 0 && e.MoveNext () do
                            consumer.ProcessNext e.Current |> ignore

                    consumer.ChainComplete result.HaltedIdx
                    result.Result
                finally
                    consumer.ChainDispose ()

