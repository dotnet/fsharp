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

    module IEnumerator =

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

      let rec tryItem index (e : IEnumerator<'T>) =
          if not (e.MoveNext()) then None
          elif index = 0 then Some(e.Current)
          else tryItem (index-1) e

      let rec nth index (e : IEnumerator<'T>) =
          if not (e.MoveNext()) then
            invalidArgFmt "index"
                "{0}\nseq was short by {1} {2}"
                [|SR.GetString SR.notEnoughElements; index; (if index=1 then "element" else "elements")|]
          if index = 0 then e.Current
          else nth (index-1) e

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


namespace Microsoft.FSharp.Core.CompilerServices

    open System
    open System.Diagnostics
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
    open Microsoft.FSharp.Core.Operators
    open Microsoft.FSharp.Control
    open Microsoft.FSharp.Collections
    open Microsoft.FSharp.Primitives.Basics
    open System.Collections
    open System.Collections.Generic

    module RuntimeHelpers =

        [<Struct; NoComparison; NoEquality>]
        type internal StructBox<'T when 'T : equality>(value:'T) =
            member x.Value = value
            static member Comparer =
                let gcomparer = HashIdentity.Structural<'T>
                { new IEqualityComparer<StructBox<'T>> with
                       member __.GetHashCode(v) = gcomparer.GetHashCode(v.Value)
                       member __.Equals(v1,v2) = gcomparer.Equals(v1.Value,v2.Value) }

        let inline checkNonNull argName arg =
            match box arg with
            | null -> nullArg argName
            | _ -> ()

        let mkSeq f =
            { new IEnumerable<'U> with
                member x.GetEnumerator() = f()
              interface IEnumerable with
                member x.GetEnumerator() = (f() :> IEnumerator) }
                
        [<NoEquality; NoComparison>]
        type EmptyEnumerable<'T> =
            | EmptyEnumerable
            interface IEnumerable<'T> with
                member x.GetEnumerator() = IEnumerator.Empty<'T>()
            interface IEnumerable with
                member x.GetEnumerator() = (IEnumerator.Empty<'T>() :> IEnumerator)

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
        let mutable redirectTo : GeneratedSequenceBase<'T> = Unchecked.defaultof<_>
        let mutable redirect : bool = false

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
            member x.Current = if redirect then redirectTo.LastGenerated else x.LastGenerated
            member x.Dispose() = if redirect then redirectTo.Close() else x.Close()
        interface IEnumerator with
            member x.Current = box (if redirect then redirectTo.LastGenerated else x.LastGenerated)

            //[<System.Diagnostics.DebuggerNonUserCode; System.Diagnostics.DebuggerStepThroughAttribute>]
            member x.MoveNext() = x.MoveNextImpl()

            member x.Reset() = raise <| new System.NotSupportedException()


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

    [<Sealed>]
    type CachedSeq<'T>(cleanup,res:seq<'T>) =
        interface System.IDisposable with
            member x.Dispose() = cleanup()
        interface System.Collections.Generic.IEnumerable<'T> with
            member x.GetEnumerator() = res.GetEnumerator()
        interface System.Collections.IEnumerable with
            member x.GetEnumerator() = (res :> System.Collections.IEnumerable).GetEnumerator()
        member obj.Clear() = cleanup()


    [<RequireQualifiedAccess>]
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module Seq =
        module SeqComposer =
            open IEnumerator

            type ISeqComponent =
                abstract OnComplete : int -> unit
                abstract OnDispose : unit -> unit

            type ISeqPipeline =
                abstract StopFurtherProcessing : int -> unit

            [<AbstractClass>]
            type SeqConsumer<'T,'U> () =
                abstract ProcessNext : input:'T -> bool

                interface ISeqComponent with
                    member __.OnComplete _ = ()
                    member __.OnDispose() = ()

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
                inherit SeqConsumer<'T,'T>

                val mutable Value : 'U

                new (init) = {
                    inherit SeqConsumer<'T,'T>()
                    Value = init
                }

            [<AbstractClass>]
            type SeqEnumerable<'T>() =
                abstract member ForEach<'a when 'a :> SeqConsumer<'T,'T>> : f:(ISeqPipeline->'a) -> 'a

            module Helpers =
                // used for performance reasons; these are not recursive calls, so should be safe
                // ** it should be noted that potential changes to the f# compiler may render this function
                // ineffictive **
                let inline avoidTailCall boolean = match boolean with true -> true | false -> false

                // The f# compiler outputs unnecessary unbox.any calls in upcasts. If this functionality
                // is fixed with the compiler then these functions can be removed.
                let inline upcastEnumerable (t:#IEnumerable<'T>) : IEnumerable<'T> = (# "" t : IEnumerable<'T> #)
                let inline upcastEnumerator (t:#IEnumerator<'T>) : IEnumerator<'T> = (# "" t : IEnumerator<'T> #)
                let inline upcastEnumeratorNonGeneric (t:#IEnumerator) : IEnumerator = (# "" t : IEnumerator #)
                let inline upcastISeqComponent (t:#ISeqComponent) : ISeqComponent = (# "" t : ISeqComponent #)

            let seqComponentTail =
                { new ISeqComponent with
                    member __.OnComplete _ = ()
                    member __.OnDispose()  = () }

            type [<AbstractClass>] SeqComponentFactory<'T,'U> () =
                abstract Create<'V> : ISeqPipeline -> SeqConsumer<'U,'V> -> haltingIdx:int -> SeqConsumer<'T,'V>

            and ComposedFactory<'T,'U,'V> private (first:SeqComponentFactory<'T,'U>, second:SeqComponentFactory<'U,'V>) =
                inherit SeqComponentFactory<'T,'V> ()
                override __.Create<'W> (result:ISeqPipeline) (next:SeqConsumer<'V,'W>) (haltingIdx:int) : SeqConsumer<'T,'W> =
                    first.Create result (second.Create result next (haltingIdx+1)) (haltingIdx+2)

                static member Combine (first:SeqComponentFactory<'T,'U>) (second:SeqComponentFactory<'U,'V>) : SeqComponentFactory<'T,'V> =
                    upcast ComposedFactory(first, second)

            and ChooseFactory<'T,'U> (filter:'T->option<'U>) =
                inherit SeqComponentFactory<'T,'U> ()
                override __.Create<'V> (_result:ISeqPipeline) (next:SeqConsumer<'U,'V>) (haltingIdx:int) : SeqConsumer<'T,'V> = upcast Choose (filter, next, haltingIdx) 
            
            and DistinctFactory<'T when 'T: equality> () =
                inherit SeqComponentFactory<'T,'T> ()
                override __.Create<'V> (_result:ISeqPipeline) (next:SeqConsumer<'T,'V>) (haltingIdx:int) : SeqConsumer<'T,'V> = upcast Distinct (next, haltingIdx) 

            and DistinctByFactory<'T,'Key when 'Key: equality> (keyFunction:'T-> 'Key) =
                inherit SeqComponentFactory<'T,'T> ()
                override __.Create<'V> (_result:ISeqPipeline) (next:SeqConsumer<'T,'V>) (haltingIdx:int) : SeqConsumer<'T,'V> = upcast DistinctBy (keyFunction, next, haltingIdx) 
            
            and ExceptFactory<'T when 'T: equality> (itemsToExclude: seq<'T>) =
                inherit SeqComponentFactory<'T,'T> ()
                override __.Create<'V> (_result:ISeqPipeline) (next:SeqConsumer<'T,'V>) (haltingIdx:int) : SeqConsumer<'T,'V> = upcast Except (itemsToExclude, next, haltingIdx) 

            and FilterFactory<'T> (filter:'T->bool) =
                inherit SeqComponentFactory<'T,'T> ()
                override __.Create<'V> (_result:ISeqPipeline) (next:SeqConsumer<'T,'V>) (haltingIdx:int) : SeqConsumer<'T,'V> =
                    match next with
                    | :? SeqComponent<'T,'V> as next -> upcast next.CreateFilter filter haltingIdx
                    | _ -> upcast Filter (filter, next, haltingIdx)

            and IdentityFactory<'T> () =
                inherit SeqComponentFactory<'T,'T> ()
                static let singleton = IdentityFactory<'T>()
                override __.Create<'V> (_result:ISeqPipeline) (next:SeqConsumer<'T,'V>) (_haltingIdx:int) : SeqConsumer<'T,'V> = next
                static member IdentityFactory = singleton

            and MapFactory<'T,'U> (map:'T->'U) =
                inherit SeqComponentFactory<'T,'U> ()
                override __.Create<'V> (_result:ISeqPipeline) (next:SeqConsumer<'U,'V>) (haltingIdx:int) : SeqConsumer<'T,'V> =
                    match next with
                    | :? SeqComponent<'U,'V> as next -> upcast next.CreateMap map haltingIdx
                    | _ -> upcast Map<_,_,_> (map, next, haltingIdx)

            and Map2FirstFactory<'First,'Second,'U> (map:'First->'Second->'U, input2:IEnumerable<'Second>) =
                inherit SeqComponentFactory<'First,'U> ()
                override __.Create<'V> (result:ISeqPipeline) (next:SeqConsumer<'U,'V>) (haltingIdx:int) : SeqConsumer<'First,'V> = upcast Map2First (map, input2, result, next, haltingIdx)

            and Map2SecondFactory<'First,'Second,'U> (map:'First->'Second->'U, input1:IEnumerable<'First>) =
                inherit SeqComponentFactory<'Second,'U> ()
                override __.Create<'V> (result:ISeqPipeline) (next:SeqConsumer<'U,'V>) (haltingIdx:int) : SeqConsumer<'Second,'V> = upcast Map2Second (map, input1, result, next, haltingIdx)

            and Map3Factory<'First,'Second,'Third,'U> (map:'First->'Second->'Third->'U, input2:IEnumerable<'Second>, input3:IEnumerable<'Third>) =
                inherit SeqComponentFactory<'First,'U> ()
                override __.Create<'V> (result:ISeqPipeline) (next:SeqConsumer<'U,'V>) (haltingIdx:int) : SeqConsumer<'First,'V> = upcast Map3 (map, input2, input3, result, next, haltingIdx)

            and MapiFactory<'T,'U> (mapi:int->'T->'U) =
                inherit SeqComponentFactory<'T,'U> ()
                override __.Create<'V> (_result:ISeqPipeline) (next:SeqConsumer<'U,'V>) (haltingIdx:int) : SeqConsumer<'T,'V> = upcast Mapi (mapi, next, haltingIdx) 

            and Mapi2Factory<'First,'Second,'U> (map:int->'First->'Second->'U, input2:IEnumerable<'Second>) =
                inherit SeqComponentFactory<'First,'U> ()
                override __.Create<'V> (result:ISeqPipeline) (next:SeqConsumer<'U,'V>) (haltingIdx:int) : SeqConsumer<'First,'V> = upcast Mapi2 (map, input2, result, next, haltingIdx)

            and PairwiseFactory<'T> () =
                inherit SeqComponentFactory<'T,'T*'T> ()
                override __.Create<'V> (_result:ISeqPipeline) (next:SeqConsumer<'T*'T,'V>) (haltingIdx:int) : SeqConsumer<'T,'V> = upcast Pairwise (next, haltingIdx)

            and ScanFactory<'T,'State> (folder:'State->'T->'State, initialState:'State) =
                inherit SeqComponentFactory<'T,'State> ()
                override __.Create<'V> (_result:ISeqPipeline) (next:SeqConsumer<'State,'V>) (haltingIdx:int) : SeqConsumer<'T,'V> = upcast Scan<_,_,_> (folder, initialState, next, haltingIdx)

            and SkipFactory<'T> (count:int) =
                inherit SeqComponentFactory<'T,'T> ()
                override __.Create<'V> (_result:ISeqPipeline) (next:SeqConsumer<'T,'V>) (haltingIdx:int) : SeqConsumer<'T,'V> = upcast Skip (count, next, haltingIdx) 

            and SkipWhileFactory<'T> (predicate:'T->bool) =
                inherit SeqComponentFactory<'T,'T> ()
                override __.Create<'V> (_result:ISeqPipeline) (next:SeqConsumer<'T,'V>) (haltingIdx:int) : SeqConsumer<'T,'V> = upcast SkipWhile (predicate, next, haltingIdx) 

            and TakeWhileFactory<'T> (predicate:'T->bool) =
                inherit SeqComponentFactory<'T,'T> ()
                override __.Create<'V> (result:ISeqPipeline) (next:SeqConsumer<'T,'V>) (haltingIdx:int) : SeqConsumer<'T,'V> = upcast TakeWhile (predicate, result, next, haltingIdx) 

            and TakeFactory<'T> (count:int) =
                inherit SeqComponentFactory<'T,'T> ()
                override __.Create<'V> (result:ISeqPipeline) (next:SeqConsumer<'T,'V>) (haltingIdx:int) : SeqConsumer<'T,'V> = upcast Take (count, result, next, haltingIdx) 
            
            and TailFactory<'T> () =
                inherit SeqComponentFactory<'T,'T> ()
                override __.Create<'V> (_result:ISeqPipeline) (next:SeqConsumer<'T,'V>) (haltingIdx:int) : SeqConsumer<'T,'V> = upcast Tail<'T,'V> (next, haltingIdx) 

            and TruncateFactory<'T> (count:int) =
                inherit SeqComponentFactory<'T,'T> ()
                override __.Create<'V> (result:ISeqPipeline) (next:SeqConsumer<'T,'V>) (haltingIdx:int) : SeqConsumer<'T,'V> = upcast Truncate (count, result, next, haltingIdx) 

            and [<AbstractClass>] SeqComponent<'T,'U> (next:ISeqComponent, haltingIdx:int) =
                inherit SeqConsumer<'T,'U>()

                // Seq.init(Infinite)? lazily uses Current. The only SeqComposer component that can do that is Skip
                // and it can only do it at the start of a sequence
                abstract Skipping : unit -> bool

                abstract CreateMap<'S> : map:('S->'T) -> haltingIdx:int -> SeqComponent<'S,'U>
                abstract CreateFilter  : filter:('T->bool) -> haltingIdx:int -> SeqComponent<'T,'U>

                interface ISeqComponent with
                    member __.OnComplete halted = next.OnComplete halted
                    member __.OnDispose ()  = next.OnDispose ()

                member __.HaltingIdx = haltingIdx

                default __.Skipping () = false

                default this.CreateMap<'S> (map:'S->'T)      (haltingIdx:int) = upcast Map<_,_,_> (map, this, haltingIdx) 
                default this.CreateFilter  (filter:'T->bool) (haltingIdx:int) = upcast Filter (filter, this, haltingIdx) 

            and Choose<'T,'U,'V> (choose:'T->option<'U>, next:SeqConsumer<'U,'V>, haltingIdx:int) =
                inherit SeqComponent<'T,'V>(next, haltingIdx)

                override __.ProcessNext (input:'T) : bool =
                    match choose input with
                    | Some value -> Helpers.avoidTailCall (next.ProcessNext value)
                    | None -> false

            and Distinct<'T,'V when 'T: equality> (next:SeqConsumer<'T,'V>, haltingIdx:int) =
                inherit SeqComponent<'T,'V>(next, haltingIdx)

                let hashSet = HashSet<'T>(HashIdentity.Structural<'T>)

                override __.ProcessNext (input:'T) : bool = 
                    if hashSet.Add input then
                        Helpers.avoidTailCall (next.ProcessNext input)
                    else
                        false

            and DistinctBy<'T,'Key,'V when 'Key: equality> (keyFunction: 'T -> 'Key, next:SeqConsumer<'T,'V>, haltingIdx:int) =
                inherit SeqComponent<'T,'V>(next, haltingIdx)

                let hashSet = HashSet<'Key>(HashIdentity.Structural<'Key>)

                override __.ProcessNext (input:'T) : bool = 
                    if hashSet.Add(keyFunction input) then
                        Helpers.avoidTailCall (next.ProcessNext input)
                    else
                        false

            and Except<'T,'V when 'T: equality> (itemsToExclude: seq<'T>, next:SeqConsumer<'T,'V>, haltingIdx:int) =
                inherit SeqComponent<'T,'V>(next, haltingIdx)

                let cached = lazy(HashSet(itemsToExclude, HashIdentity.Structural))

                override __.ProcessNext (input:'T) : bool = 
                    if cached.Value.Add input then
                        Helpers.avoidTailCall (next.ProcessNext input)
                    else
                        false

            and Filter<'T,'V> (filter:'T->bool, next:SeqConsumer<'T,'V>, haltingIdx:int) =
                inherit SeqComponent<'T,'V>(next, haltingIdx)

                override this.CreateMap<'S> (map:'S->'T) (haltingIdx:int) = upcast MapThenFilter<_,_,_> (map, filter, next, haltingIdx) 

                override __.ProcessNext (input:'T) : bool = 
                    if filter input then
                        Helpers.avoidTailCall (next.ProcessNext input)
                    else
                        false

            and FilterThenMap<'T,'U,'V> (filter:'T->bool, map:'T->'U, next:SeqConsumer<'U,'V>, haltingIdx:int) =
                inherit SeqComponent<'T,'V>(next, haltingIdx)

                override __.ProcessNext (input:'T) : bool = 
                    if filter input then
                        Helpers.avoidTailCall (next.ProcessNext (map input))
                    else
                        false

            and Map<'T,'U,'V> (map:'T->'U, next:SeqConsumer<'U,'V>, haltingIdx:int) =
                inherit SeqComponent<'T,'V>(next, haltingIdx)

                override this.CreateFilter (filter:'T->bool) (haltingIdx:int) = upcast FilterThenMap (filter, map, next, haltingIdx) 

                override __.ProcessNext (input:'T) : bool = 
                    Helpers.avoidTailCall (next.ProcessNext (map input))

            and Map2First<'First,'Second,'U,'V> (map:'First->'Second->'U, enumerable2:IEnumerable<'Second>, result:ISeqPipeline, next:SeqConsumer<'U,'V>, haltingIdx:int) =
                inherit SeqComponent<'First,'V>(next, haltingIdx)

                let input2 = enumerable2.GetEnumerator ()
                let map' = OptimizedClosures.FSharpFunc<_,_,_>.Adapt map

                override __.ProcessNext (input:'First) : bool =
                    if input2.MoveNext () then
                        Helpers.avoidTailCall (next.ProcessNext (map'.Invoke (input, input2.Current)))
                    else
                        result.StopFurtherProcessing haltingIdx
                        false

                interface ISeqComponent with
                    override __.OnDispose () =
                        try
                            input2.Dispose ()
                        finally
                            (Helpers.upcastISeqComponent next).OnDispose ()

            and Map2Second<'First,'Second,'U,'V> (map:'First->'Second->'U, enumerable1:IEnumerable<'First>, result:ISeqPipeline, next:SeqConsumer<'U,'V>, haltingIdx:int) =
                inherit SeqComponent<'Second,'V>(next, haltingIdx)

                let input1 = enumerable1.GetEnumerator ()
                let map' = OptimizedClosures.FSharpFunc<_,_,_>.Adapt map

                override __.ProcessNext (input:'Second) : bool =
                    if input1.MoveNext () then
                        Helpers.avoidTailCall (next.ProcessNext (map'.Invoke (input1.Current, input)))
                    else
                        result.StopFurtherProcessing haltingIdx
                        false

                interface ISeqComponent with
                    override __.OnDispose () =
                        try
                            input1.Dispose ()
                        finally
                            (Helpers.upcastISeqComponent next).OnDispose ()

            and Map3<'First,'Second,'Third,'U,'V> (map:'First->'Second->'Third->'U, enumerable2:IEnumerable<'Second>, enumerable3:IEnumerable<'Third>, result:ISeqPipeline, next:SeqConsumer<'U,'V>, haltingIdx:int) =
                inherit SeqComponent<'First,'V>(next, haltingIdx)

                let input2 = enumerable2.GetEnumerator ()
                let input3 = enumerable3.GetEnumerator ()
                let map' = OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt map

                override __.ProcessNext (input:'First) : bool =
                    if input2.MoveNext () && input3.MoveNext () then
                        Helpers.avoidTailCall (next.ProcessNext (map'.Invoke (input, input2.Current, input3.Current)))
                    else
                        result.StopFurtherProcessing haltingIdx
                        false

                interface ISeqComponent with
                    override __.OnDispose () =
                        try
                            input2.Dispose ()
                        finally
                            try
                                input3.Dispose ()
                            finally
                                (Helpers.upcastISeqComponent next).OnDispose ()

            and MapThenFilter<'T,'U,'V> (map:'T->'U, filter:'U->bool, next:SeqConsumer<'U,'V>, haltingIdx:int) =
                inherit SeqComponent<'T,'V>(next, haltingIdx)

                override __.ProcessNext (input:'T) : bool = 
                    let u = map input
                    if filter u then
                        Helpers.avoidTailCall (next.ProcessNext u)
                    else
                        false

            and Mapi<'T,'U,'V> (mapi:int->'T->'U, next:SeqConsumer<'U,'V>, haltingIdx:int) =
                inherit SeqComponent<'T,'V>(next, haltingIdx)

                let mutable idx = 0
                let mapi' = OptimizedClosures.FSharpFunc<_,_,_>.Adapt mapi

                override __.ProcessNext (input:'T) : bool = 
                    idx <- idx + 1
                    Helpers.avoidTailCall (next.ProcessNext (mapi'.Invoke (idx-1, input)))

            and Mapi2<'First,'Second,'U,'V> (map:int->'First->'Second->'U, enumerable2:IEnumerable<'Second>, result:ISeqPipeline, next:SeqConsumer<'U,'V>, haltingIdx:int) =
                inherit SeqComponent<'First,'V>(next, haltingIdx)

                let mutable idx = 0
                let input2 = enumerable2.GetEnumerator ()
                let mapi2' = OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt map

                override __.ProcessNext (input:'First) : bool =
                    if input2.MoveNext () then
                        idx <- idx + 1
                        Helpers.avoidTailCall (next.ProcessNext (mapi2'.Invoke (idx-1, input, input2.Current)))
                    else
                        result.StopFurtherProcessing haltingIdx
                        false

                interface ISeqComponent with
                    override __.OnDispose () =
                        try
                            input2.Dispose ()
                        finally
                            (Helpers.upcastISeqComponent next).OnDispose ()

            and Pairwise<'T,'V> (next:SeqConsumer<'T*'T,'V>, haltingIdx:int) =
                inherit SeqComponent<'T,'V>(next, haltingIdx)

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
                        Helpers.avoidTailCall (next.ProcessNext currentPair)

            and Scan<'T,'State,'V> (folder:'State->'T->'State, initialState: 'State, next:SeqConsumer<'State,'V>, haltingIdx:int) =
                inherit SeqComponent<'T,'V>(next, haltingIdx)

                let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt folder
                let mutable foldResult = initialState

                override __.ProcessNext (input:'T) : bool =
                    foldResult <- f.Invoke(foldResult, input)
                    Helpers.avoidTailCall (next.ProcessNext foldResult)

            and Skip<'T,'V> (skipCount:int, next:SeqConsumer<'T,'V>, haltingIdx:int) =
                inherit SeqComponent<'T,'V>(next, haltingIdx)

                let mutable count = 0

                override __.Skipping () =
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
                        Helpers.avoidTailCall (next.ProcessNext input)

                interface ISeqComponent with
                    override __.OnComplete halted =
                        if count < skipCount then
                            let x = skipCount - count
                            invalidOpFmt "tried to skip {0} {1} past the end of the seq"
                              [|SR.GetString SR.notEnoughElements; x; (if x=1 then "element" else "elements")|]
                        (Helpers.upcastISeqComponent next).OnComplete halted

            and SkipWhile<'T,'V> (predicate:'T->bool, next:SeqConsumer<'T,'V>, haltingIdx:int) =
                inherit SeqComponent<'T,'V>(next, haltingIdx)

                let mutable skip = true

                override __.ProcessNext (input:'T) : bool = 
                    if skip then
                        skip <- predicate input
                        if skip then
                            false
                        else
                            Helpers.avoidTailCall (next.ProcessNext input)
                    else
                        Helpers.avoidTailCall (next.ProcessNext input)

            and Take<'T,'V> (takeCount:int, result:ISeqPipeline, next:SeqConsumer<'T,'V>, haltingIdx:int) =
                inherit Truncate<'T, 'V>(takeCount, result, next, haltingIdx)

                interface ISeqComponent with
                    override this.OnComplete halted =
                        if halted  = 0 || halted > haltingIdx && this.Count < takeCount then
                            let x = takeCount - this.Count
                            invalidOpFmt "tried to take {0} {1} past the end of the seq"
                                [|SR.GetString SR.notEnoughElements; x; (if x=1 then "element" else "elements")|]
                        (Helpers.upcastISeqComponent next).OnComplete halted

            and TakeWhile<'T,'V> (predicate:'T->bool, result:ISeqPipeline, next:SeqConsumer<'T,'V>, haltingIdx:int) =
                inherit SeqComponent<'T,'V>(next, haltingIdx)

                override __.ProcessNext (input:'T) : bool = 
                    if predicate input then
                        Helpers.avoidTailCall (next.ProcessNext input)
                    else
                        result.StopFurtherProcessing haltingIdx
                        false

            and Tail<'T, 'V> (next:SeqConsumer<'T,'V>, haltingIdx:int) =
                inherit SeqComponent<'T,'V>(next, haltingIdx)

                let mutable first = true

                override __.ProcessNext (input:'T) : bool =
                    if first then
                        first <- false
                        false
                    else
                        Helpers.avoidTailCall (next.ProcessNext input)

                interface ISeqComponent with
                    override this.OnComplete halted =
                        if first then
                            invalidArg "source" (SR.GetString(SR.notEnoughElements))
                        (Helpers.upcastISeqComponent next).OnComplete halted

            and Truncate<'T,'V> (truncateCount:int, result:ISeqPipeline, next:SeqConsumer<'T,'V>, haltingIdx:int) =
                inherit SeqComponent<'T,'V>(next, haltingIdx)

                let mutable count = 0

                member __.Count = count

                override __.ProcessNext (input:'T) : bool = 
                    if count < truncateCount then
                        count <- count + 1
                        if count = truncateCount then
                            result.StopFurtherProcessing haltingIdx
                        next.ProcessNext input
                    else
                        result.StopFurtherProcessing haltingIdx
                        false

            type SeqProcessNextStates =
            | InProcess  = 0
            | NotStarted = 1
            | Finished   = 2

            type Result<'T>() =
                let mutable haltedIdx = 0
                
                member val Current = Unchecked.defaultof<'T> with get, set
                member val SeqState = SeqProcessNextStates.NotStarted with get, set
                member __.Halted = haltedIdx

                interface ISeqPipeline with
                    member __.StopFurtherProcessing haltingIdx = haltedIdx <- haltingIdx

            // SetResult<> is used at the end of the chain of SeqComponents to assign the final value
            type SetResult<'T> (result:Result<'T>) =
                inherit SeqConsumer<'T,'T>()

                override __.ProcessNext (input:'T) : bool =
                    result.Current <- input
                    true

            type Pipeline() =
                let mutable halted = 0
                interface ISeqPipeline with member x.StopFurtherProcessing haltingIdx = halted <- haltingIdx
                member __.Halted = halted

            module ForEach =
                let enumerable (enumerable:IEnumerable<'T>) (pipeline:Pipeline) (consumer:SeqConsumer<'T,'U>) =
                    use enumerator = enumerable.GetEnumerator ()
                    while (pipeline.Halted = 0) && (enumerator.MoveNext ()) do
                        consumer.ProcessNext enumerator.Current |> ignore

                let array (array:array<'T>) (pipeline:Pipeline) (consumer:SeqConsumer<'T,'U>) =
                    let mutable idx = 0
                    while (pipeline.Halted = 0) && (idx < array.Length) do
                        consumer.ProcessNext array.[idx] |> ignore
                        idx <- idx + 1

                let list (alist:list<'T>) (pipeline:Pipeline) (consumer:SeqConsumer<'T,'U>) =
                    let rec iterate lst =
                        match pipeline.Halted, lst with
                        | 0, hd :: tl ->
                            consumer.ProcessNext hd |> ignore
                            iterate tl
                        | _ -> ()
                    iterate alist

                let unfold (generator:'S->option<'T*'S>) state (pipeline:Pipeline) (consumer:SeqConsumer<'T,'U>) = 
                    let rec iterate current =
                        match pipeline.Halted, generator current with
                        | 0, Some (item, next) ->
                            consumer.ProcessNext item |> ignore
                            iterate next
                        | _ -> ()
    
                    iterate state

                let makeIsSkipping (consumer:SeqConsumer<'T,'U>) =
                    match consumer with
                    | :? SeqComponent<'T,'U> as c -> c.Skipping
                    | _ -> fun () -> false

                let init f (terminatingIdx:int) (pipeline:Pipeline) (consumer:SeqConsumer<'T,'U>) =
                    let mutable idx = -1
                    let isSkipping = makeIsSkipping consumer
                    let mutable maybeSkipping = true
                    while (pipeline.Halted = 0) && (idx < terminatingIdx) do
                        if maybeSkipping then
                            maybeSkipping <- isSkipping ()

                        if (not maybeSkipping) then
                            consumer.ProcessNext (f (idx+1)) |> ignore

                        idx <- idx + 1

                let execute (f:ISeqPipeline->#SeqConsumer<'U,'U>) (current:SeqComponentFactory<'T,'U>) executeOn =
                    let pipeline = Pipeline()
                    let result = f pipeline
                    let consumer = current.Create pipeline result 1
                    try
                        executeOn pipeline consumer
                        (Helpers.upcastISeqComponent consumer).OnComplete pipeline.Halted
                        result
                    finally
                        (Helpers.upcastISeqComponent consumer).OnDispose ()

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
                type EnumeratorBase<'T>(result:Result<'T>, seqComponent:ISeqComponent) =
                    interface IDisposable with
                        member __.Dispose() : unit =
                            seqComponent.OnDispose ()

                    interface IEnumerator with
                        member this.Current : obj = box ((Helpers.upcastEnumerator this)).Current
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
                    inherit SeqEnumerable<'T>()

                    abstract member Compose<'U>  : (SeqComponentFactory<'T,'U>) -> IEnumerable<'U>
                    abstract member Append<'T>   : (seq<'T>) -> IEnumerable<'T>

                    default this.Append source = Helpers.upcastEnumerable (AppendEnumerable [this; source])

                    interface IEnumerable with
                        member this.GetEnumerator () : IEnumerator =
                            let genericEnumerable = Helpers.upcastEnumerable this
                            let genericEnumerator = genericEnumerable.GetEnumerator ()
                            Helpers.upcastEnumeratorNonGeneric genericEnumerator

                    interface IEnumerable<'T> with
                        member this.GetEnumerator () : IEnumerator<'T> = failwith "library implementation error: derived class should implement (should be abstract)"


                and Enumerator<'T,'U>(source:IEnumerator<'T>, seqComponent:SeqConsumer<'T,'U>, result:Result<'U>) =
                    inherit EnumeratorBase<'U>(result, seqComponent)

                    let rec moveNext () =
                        if (result.Halted = 0) && source.MoveNext () then
                            if seqComponent.ProcessNext source.Current then
                                true
                            else
                                moveNext ()
                        else
                            result.SeqState <- SeqProcessNextStates.Finished
                            (Helpers.upcastISeqComponent seqComponent).OnComplete result.Halted
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
                                (Helpers.upcastISeqComponent seqComponent).OnDispose ()

                and Enumerable<'T,'U>(enumerable:IEnumerable<'T>, current:SeqComponentFactory<'T,'U>) =
                    inherit EnumerableBase<'U>()

                    interface IEnumerable<'U> with
                        member this.GetEnumerator () : IEnumerator<'U> =
                            let result = Result<'U> ()
                            Helpers.upcastEnumerator (new Enumerator<'T,'U>(enumerable.GetEnumerator(), current.Create result (SetResult<'U> result) 1, result))

                    override __.Compose (next:SeqComponentFactory<'U,'V>) : IEnumerable<'V> =
                        Helpers.upcastEnumerable (new Enumerable<'T,'V>(enumerable, ComposedFactory.Combine current next))

                    override this.ForEach (f:ISeqPipeline->#SeqConsumer<'U,'U>) =
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
                        member this.Current = box ((Helpers.upcastEnumerator this)).Current
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
                            Helpers.upcastEnumerator (new ConcatEnumerator<_,_> (sources |> List.rev))

                    override this.Compose (next:SeqComponentFactory<'T,'U>) : IEnumerable<'U> =
                        Helpers.upcastEnumerable (Enumerable<'T,'V>(this, next))

                    override this.Append source =
                        Helpers.upcastEnumerable (AppendEnumerable (source :: sources))

                    override this.ForEach (f:ISeqPipeline->#SeqConsumer<'T,'T>) =
                        ForEach.execute f IdentityFactory.IdentityFactory (ForEach.enumerable this)

                and ConcatEnumerable<'T, 'Collection when 'Collection :> seq<'T>> (sources:seq<'Collection>) =
                    inherit EnumerableBase<'T>()

                    interface IEnumerable<'T> with
                        member this.GetEnumerator () : IEnumerator<'T> =
                            Helpers.upcastEnumerator (new ConcatEnumerator<_,_> (sources))

                    override this.Compose (next:SeqComponentFactory<'T,'U>) : IEnumerable<'U> =
                        Helpers.upcastEnumerable (Enumerable<'T,'V>(this, next))

                    override this.ForEach (f:ISeqPipeline->#SeqConsumer<'T,'T>) =
                        ForEach.execute f IdentityFactory.IdentityFactory (ForEach.enumerable this)

                let create enumerable current =
                    Helpers.upcastEnumerable (Enumerable(enumerable, current))

            module Array =
                type Enumerator<'T,'U>(delayedArray:unit->array<'T>, seqComponent:SeqConsumer<'T,'U>, result:Result<'U>) =
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
                        if (result.Halted = 0) && idx < array.Length then
                            idx <- idx+1
                            if seqComponent.ProcessNext array.[idx-1] then
                                true
                            else
                                moveNext ()
                        else
                            result.SeqState <- SeqProcessNextStates.Finished
                            (Helpers.upcastISeqComponent seqComponent).OnComplete result.Halted
                            false

                    interface IEnumerator with
                        member __.MoveNext () =
                            initMoveNext ()
                            moveNext ()

                type Enumerable<'T,'U>(delayedArray:unit->array<'T>, current:SeqComponentFactory<'T,'U>) =
                    inherit Enumerable.EnumerableBase<'U>()

                    interface IEnumerable<'U> with
                        member this.GetEnumerator () : IEnumerator<'U> =
                            let result = Result<'U> ()
                            Helpers.upcastEnumerator (new Enumerator<'T,'U>(delayedArray, current.Create result (SetResult<'U> result) 1, result))

                    override __.Compose (next:SeqComponentFactory<'U,'V>) : IEnumerable<'V> =
                        Helpers.upcastEnumerable (new Enumerable<'T,'V>(delayedArray, ComposedFactory.Combine current next))

                    override this.ForEach (f:ISeqPipeline->#SeqConsumer<'U,'U>) =
                        ForEach.execute f current (ForEach.array (delayedArray ()))

                let createDelayed (delayedArray:unit->array<'T>) (current:SeqComponentFactory<'T,'U>) =
                    Helpers.upcastEnumerable (Enumerable(delayedArray, current))

                let create (array:array<'T>) (current:SeqComponentFactory<'T,'U>) =
                    createDelayed (fun () -> array) current

                let createDelayedId (delayedArray:unit -> array<'T>) =
                    createDelayed delayedArray IdentityFactory.IdentityFactory

                let createId (array:array<'T>) =
                    create array IdentityFactory.IdentityFactory

            module List =
                type Enumerator<'T,'U>(alist:list<'T>, seqComponent:SeqConsumer<'T,'U>, result:Result<'U>) =
                    inherit Enumerable.EnumeratorBase<'U>(result, seqComponent)

                    let mutable list = alist

                    let rec moveNext current =
                        match result.Halted, current with
                        | 0, head::tail -> 
                            if seqComponent.ProcessNext head then
                                list <- tail
                                true
                            else
                                moveNext tail
                        | _ ->
                            result.SeqState <- SeqProcessNextStates.Finished
                            (Helpers.upcastISeqComponent seqComponent).OnComplete result.Halted
                            false

                    interface IEnumerator with
                        member __.MoveNext () =
                            result.SeqState <- SeqProcessNextStates.InProcess
                            moveNext list

                type Enumerable<'T,'U>(alist:list<'T>, current:SeqComponentFactory<'T,'U>) =
                    inherit Enumerable.EnumerableBase<'U>()

                    interface IEnumerable<'U> with
                        member this.GetEnumerator () : IEnumerator<'U> =
                            let result = Result<'U> ()
                            Helpers.upcastEnumerator (new Enumerator<'T,'U>(alist, current.Create result (SetResult<'U> result) 1, result))

                    override __.Compose (next:SeqComponentFactory<'U,'V>) : IEnumerable<'V> =
                        Helpers.upcastEnumerable (new Enumerable<'T,'V>(alist, ComposedFactory.Combine current next))

                    override this.ForEach (f:ISeqPipeline->#SeqConsumer<'U,'U>) =
                        ForEach.execute f current (ForEach.list alist)

                let create alist current =
                    Helpers.upcastEnumerable (Enumerable(alist, current))

            module Unfold =
                type Enumerator<'T,'U,'State>(generator:'State->option<'T*'State>, state:'State, seqComponent:SeqConsumer<'T,'U>, signal:Result<'U>) =
                    inherit Enumerable.EnumeratorBase<'U>(signal, seqComponent)

                    let mutable current = state

                    let rec moveNext () =
                        match signal.Halted, generator current with
                        | 0, Some (item, nextState) ->
                            current <- nextState
                            if seqComponent.ProcessNext item then
                                true
                            else
                                moveNext ()
                        | _ -> false

                    interface IEnumerator with
                        member __.MoveNext () =
                            signal.SeqState <- SeqProcessNextStates.InProcess
                            moveNext ()

                type Enumerable<'T,'U,'GeneratorState>(generator:'GeneratorState->option<'T*'GeneratorState>, state:'GeneratorState, current:SeqComponentFactory<'T,'U>) =
                    inherit Enumerable.EnumerableBase<'U>()

                    interface IEnumerable<'U> with
                        member this.GetEnumerator () : IEnumerator<'U> =
                            let result = Result<'U> ()
                            Helpers.upcastEnumerator (new Enumerator<'T,'U,'GeneratorState>(generator, state, current.Create result (SetResult<'U> result) 1, result))

                    override this.Compose (next:SeqComponentFactory<'U,'V>) : IEnumerable<'V> =
                        Helpers.upcastEnumerable (new Enumerable<'T,'V,'GeneratorState>(generator, state, ComposedFactory.Combine current next))

                    override this.ForEach (f:ISeqPipeline->#SeqConsumer<'U,'U>) =
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

                type Enumerator<'T,'U>(count:Nullable<int>, f:int->'T, seqComponent:SeqConsumer<'T,'U>, signal:Result<'U>) =
                    inherit Enumerable.EnumeratorBase<'U>(signal, seqComponent)

                    let isSkipping =
                        ForEach.makeIsSkipping seqComponent

                    let terminatingIdx =
                        getTerminatingIdx count

                    let mutable maybeSkipping = true
                    let mutable idx = -1

                    let rec moveNext () =
                        if (signal.Halted = 0) && idx < terminatingIdx then
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
                        elif (signal.Halted = 0) && idx = System.Int32.MaxValue then
                            raise <| System.InvalidOperationException (SR.GetString(SR.enumerationPastIntMaxValue))
                        else
                            signal.SeqState <- SeqProcessNextStates.Finished
                            (Helpers.upcastISeqComponent seqComponent).OnComplete signal.Halted
                            false

                    interface IEnumerator with
                        member __.MoveNext () =
                            signal.SeqState <- SeqProcessNextStates.InProcess
                            moveNext ()

                type Enumerable<'T,'U>(count:Nullable<int>, f:int->'T, current:SeqComponentFactory<'T,'U>) =
                    inherit Enumerable.EnumerableBase<'U>()

                    interface IEnumerable<'U> with
                        member this.GetEnumerator () : IEnumerator<'U> =
                            let result = Result<'U> ()
                            Helpers.upcastEnumerator (new Enumerator<'T,'U>(count, f, current.Create result (SetResult<'U> result) 1, result))

                    override this.Compose (next:SeqComponentFactory<'U,'V>) : IEnumerable<'V> =
                        Helpers.upcastEnumerable (new Enumerable<'T,'V>(count, f, ComposedFactory.Combine current next))

                    override this.ForEach (createResult:ISeqPipeline->#SeqConsumer<'U,'U>) =
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

                    override this.Compose (next:SeqComponentFactory<'T,'U>) : IEnumerable<'U> =
                        Helpers.upcastEnumerable (Enumerable<'T,'V>(count, f, next))

                    override this.ForEach (f:ISeqPipeline->#SeqConsumer<'T,'T>) =
                        ForEach.execute f IdentityFactory.IdentityFactory (ForEach.enumerable (Helpers.upcastEnumerable this))

#if FX_NO_ICLONEABLE
        open Microsoft.FSharp.Core.ICloneableExtensions
#else
#endif

        open Microsoft.FSharp.Core.CompilerServices.RuntimeHelpers

        let mkDelayedSeq (f: unit -> IEnumerable<'T>) = mkSeq (fun () -> f().GetEnumerator())
        let inline indexNotFound() = raise (new System.Collections.Generic.KeyNotFoundException(SR.GetString(SR.keyNotFoundAlt)))
        
        [<CompiledName("ToComposer")>]
        let toComposer (source:seq<'T>): SeqComposer.SeqEnumerable<'T> = 
            checkNonNull "source" source
            match source with
            | :? SeqComposer.Enumerable.EnumerableBase<'T> as s -> upcast s
            | :? array<'T> as a -> upcast SeqComposer.Array.Enumerable((fun () -> a), SeqComposer.IdentityFactory.IdentityFactory)
            | :? list<'T> as a -> upcast SeqComposer.List.Enumerable(a, SeqComposer.IdentityFactory.IdentityFactory)
            | _ -> upcast SeqComposer.Enumerable.Enumerable<'T,'T>(source, SeqComposer.IdentityFactory.IdentityFactory)
        
        let inline foreach f (source:seq<_>) =
            source
            |> toComposer
            |> fun composer -> composer.ForEach f

        [<CompiledName("Delay")>]
        let delay f = mkDelayedSeq f

        [<CompiledName("Unfold")>]
        let unfold (generator:'State->option<'T * 'State>) (state:'State) : seq<'T> =
            SeqComposer.Helpers.upcastEnumerable (new SeqComposer.Unfold.Enumerable<'T,'T,'State>(generator, state, SeqComposer.IdentityFactory.IdentityFactory))

        [<CompiledName("Empty")>]
        let empty<'T> = (EmptyEnumerable :> seq<'T>)

        [<CompiledName("InitializeInfinite")>]
        let initInfinite<'T> (f:int->'T) : IEnumerable<'T> =
            SeqComposer.Helpers.upcastEnumerable (new SeqComposer.Init.EnumerableDecider<'T>(Nullable (), f))

        [<CompiledName("Initialize")>]
        let init<'T> (count:int) (f:int->'T) : IEnumerable<'T> =
            if count < 0 then invalidArgInputMustBeNonNegative "count" count
            elif count = 0 then empty else
            SeqComposer.Helpers.upcastEnumerable (new SeqComposer.Init.EnumerableDecider<'T>(Nullable count, f))

        [<CompiledName("Iterate")>]
        let iter f (source : seq<'T>) =
            source
            |> foreach (fun _ ->
                    { new SeqComposer.SeqConsumer<'T,'T> () with
                        override this.ProcessNext value =
                            f value
                            Unchecked.defaultof<bool> })
            |> ignore

        [<CompiledName("Item")>]
        let item i (source : seq<'T>) =
            checkNonNull "source" source
            if i < 0 then invalidArgInputMustBeNonNegative "index" i
            use e = source.GetEnumerator()
            IEnumerator.nth i e

        [<CompiledName("TryItem")>]
        let tryItem i (source : seq<'T>) =
            if i < 0 then None else
            source 
            |> foreach (fun pipeline ->
                { new SeqComposer.Folder<'T, SeqComposer.Values<int, Option<'T>>> (SeqComposer.Values<_, _> (0, None)) with
                    override this.ProcessNext value =
                        if this.Value._1 = i then
                            this.Value._2 <- Some value
                            pipeline.StopFurtherProcessing 1
                        else
                            this.Value._1 <- this.Value._1 + 1
                        Unchecked.defaultof<bool> })
            |> fun item -> item.Value._2

        [<CompiledName("Get")>]
        let nth i (source : seq<'T>) = item i source

        [<CompiledName("IterateIndexed")>]
        let iteri f (source : seq<'T>) =
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)
            source
            |> foreach (fun _ ->
                { new SeqComposer.Folder<'T, int> (0) with
                    override this.ProcessNext value =
                        f.Invoke(this.Value, value)
                        this.Value <- this.Value + 1
                        Unchecked.defaultof<bool> })
            |> ignore

        [<CompiledName("Exists")>]
        let exists f (source : seq<'T>) =
            source
            |> foreach (fun pipeline ->
                { new SeqComposer.Folder<'T, bool> (false) with
                    override this.ProcessNext value =
                        if f value then
                            this.Value <- true
                            pipeline.StopFurtherProcessing 1
                        Unchecked.defaultof<bool> 
                    })
            |> fun exists -> exists.Value

        [<CompiledName("Contains")>]
        let inline contains element (source : seq<'T>) =
            source
            |> foreach (fun pipeline ->
                { new SeqComposer.Folder<'T, bool> (false) with
                    override this.ProcessNext value =
                        if element = value then
                            this.Value <- true
                            pipeline.StopFurtherProcessing 1
                        Unchecked.defaultof<bool>
                    })
            |> fun contains -> contains.Value

        [<CompiledName("ForAll")>]
        let forall f (source : seq<'T>) =
            source
            |> foreach (fun pipeline ->
                { new SeqComposer.Folder<'T, bool> (true) with
                    override this.ProcessNext value =
                        if not (f value) then
                            this.Value <- false
                            pipeline.StopFurtherProcessing 1
                        Unchecked.defaultof<bool>
                    })
            |> fun forall -> forall.Value

        [<CompiledName("Iterate2")>]
        let iter2 f (source1 : seq<_>) (source2 : seq<_>)    =
            checkNonNull "source1" source1
            checkNonNull "source2" source2
            use e1 = source1.GetEnumerator()
            use e2 = source2.GetEnumerator()
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)
            while (e1.MoveNext() && e2.MoveNext()) do
                f.Invoke(e1.Current, e2.Current)

        [<CompiledName("IterateIndexed2")>]
        let iteri2 f (source1 : seq<_>) (source2 : seq<_>) =
            checkNonNull "source1" source1
            checkNonNull "source2" source2
            use e1 = source1.GetEnumerator()
            use e2 = source2.GetEnumerator()
            let f = OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt(f)
            let mutable i = 0
            while (e1.MoveNext() && e2.MoveNext()) do
                f.Invoke(i, e1.Current, e2.Current)
                i <- i + 1

        // Build an IEnumerble by wrapping/transforming iterators as they get generated.
        let revamp f (ie : seq<_>) = mkSeq (fun () -> f (ie.GetEnumerator()))
        let revamp2 f (ie1 : seq<_>) (source2 : seq<_>) =
            mkSeq (fun () -> f (ie1.GetEnumerator()) (source2.GetEnumerator()))
        let revamp3 f (ie1 : seq<_>) (source2 : seq<_>) (source3 : seq<_>) =
            mkSeq (fun () -> f (ie1.GetEnumerator()) (source2.GetEnumerator()) (source3.GetEnumerator()))

        let private seqFactory createSeqComponent (source:seq<'T>) =
            checkNonNull "source" source
            match source with
            | :? SeqComposer.Enumerable.EnumerableBase<'T> as s -> s.Compose createSeqComponent
            | :? array<'T> as a -> SeqComposer.Array.create a createSeqComponent
            | :? list<'T> as a -> SeqComposer.List.create a createSeqComponent
            | _ -> SeqComposer.Enumerable.create source createSeqComponent

        [<CompiledName("Filter")>]
        let filter<'T> (f:'T->bool) (source:seq<'T>) : seq<'T> =
            source |> seqFactory (SeqComposer.FilterFactory f)

        [<CompiledName("Where")>]
        let where f source = filter f source

        [<CompiledName("Map")>]
        let map<'T,'U> (f:'T->'U) (source:seq<'T>) : seq<'U> =
            source |> seqFactory (SeqComposer.MapFactory f)

        [<CompiledName("MapIndexed")>]
        let mapi f source      =
            source |> seqFactory (SeqComposer.MapiFactory f)

        [<CompiledName("MapIndexed2")>]
        let mapi2 f source1 source2 =
            checkNonNull "source2" source2
            source1 |> seqFactory (SeqComposer.Mapi2Factory (f, source2))

        [<CompiledName("Map2")>]
        let map2<'T,'U,'V> (f:'T->'U->'V) (source1:seq<'T>) (source2:seq<'U>) : seq<'V> =
            checkNonNull "source1" source1
            match source1 with
            | :? SeqComposer.Enumerable.EnumerableBase<'T> as s -> s.Compose (SeqComposer.Map2FirstFactory (f, source2))
            | _ -> source2 |> seqFactory (SeqComposer.Map2SecondFactory (f, source1))

        [<CompiledName("Map3")>]
        let map3 f source1 source2 source3 =
            checkNonNull "source2" source2
            checkNonNull "source3" source3
            source1 |> seqFactory (SeqComposer.Map3Factory (f, source2, source3))

        [<CompiledName("Choose")>]
        let choose f source      =
            source |> seqFactory (SeqComposer.ChooseFactory f)

        [<CompiledName("Indexed")>]
        let indexed source =
            source |> seqFactory (SeqComposer.MapiFactory (fun i x -> i,x) )

        [<CompiledName("Zip")>]
        let zip source1 source2  =
            map2 (fun x y -> x,y) source1 source2

        [<CompiledName("Zip3")>]
        let zip3 source1 source2  source3 =
            map2 (fun x (y,z) -> x,y,z) source1 (zip source2 source3)

        [<CompiledName("Cast")>]
        let cast (source: IEnumerable) =
            checkNonNull "source" source
            mkSeq (fun () -> IEnumerator.cast (source.GetEnumerator()))

        [<CompiledName("TryPick")>]
        let tryPick f (source : seq<'T>)  =
            source
            |> foreach (fun pipeline ->
                { new SeqComposer.Folder<'T, Option<'U>> (None) with
                    override this.ProcessNext value =
                        match f value with
                        | (Some _) as some ->
                            this.Value <- some
                            pipeline.StopFurtherProcessing 1
                        | None -> ()
                        Unchecked.defaultof<bool> })
            |> fun pick -> pick.Value

        [<CompiledName("Pick")>]
        let pick f source  =
            match tryPick f source with
            | None -> indexNotFound()
            | Some x -> x

        [<CompiledName("TryFind")>]
        let tryFind f (source : seq<'T>)  =
            source 
            |> foreach (fun pipeline ->
                { new SeqComposer.Folder<'T, Option<'T>> (None) with
                    override this.ProcessNext value =
                        if f value then
                            this.Value <- Some value
                            pipeline.StopFurtherProcessing 1
                        Unchecked.defaultof<bool> })
            |> fun find -> find.Value

        [<CompiledName("Find")>]
        let find f source =
            match tryFind f source with
            | None -> indexNotFound()
            | Some x -> x

        [<CompiledName("Take")>]
        let take count (source : seq<'T>)    =
            if count < 0 then invalidArgInputMustBeNonNegative "count" count
            (* Note: don't create or dispose any IEnumerable if n = 0 *)
            if count = 0 then empty else
            source |> seqFactory (SeqComposer.TakeFactory count)

        [<CompiledName("IsEmpty")>]
        let isEmpty (source : seq<'T>)  =
            checkNonNull "source" source
            match source with
            | :? ('T[]) as a -> a.Length = 0
            | :? list<'T> as a -> a.IsEmpty
            | :? ICollection<'T> as a -> a.Count = 0
            | _ ->
                use ie = source.GetEnumerator()
                not (ie.MoveNext())

        [<CompiledName("Concat")>]
        let concat (sources:seq<#seq<'T>>) : seq<'T> =
            checkNonNull "sources" sources
            upcast SeqComposer.Enumerable.ConcatEnumerable sources

        [<CompiledName("Length")>]
        let length (source : seq<'T>)    =
            checkNonNull "source" source
            match source with
            | :? ('T[]) as a -> a.Length
            | :? ('T list) as a -> a.Length
            | :? ICollection<'T> as a -> a.Count
            | _ ->
                use e = source.GetEnumerator()
                let mutable state = 0
                while e.MoveNext() do
                    state <-  state + 1
                state

        [<CompiledName("Fold")>]
        let fold<'T,'State> f (x:'State) (source:seq<'T>) =
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)

            source
            |> foreach (fun _ ->
                { new SeqComposer.Folder<'T,'State> (x) with
                    override this.ProcessNext value =
                        this.Value <- f.Invoke (this.Value, value)
                        Unchecked.defaultof<bool> })
            |> fun folded -> folded.Value

        [<CompiledName("Fold2")>]
        let fold2<'T1,'T2,'State> f (state:'State) (source1: seq<'T1>) (source2: seq<'T2>) =
            checkNonNull "source1" source1
            checkNonNull "source2" source2

            use e1 = source1.GetEnumerator()
            use e2 = source2.GetEnumerator()

            let f = OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt(f)

            let mutable state = state
            while e1.MoveNext() && e2.MoveNext() do
                state <- f.Invoke(state, e1.Current, e2.Current)

            state

        [<CompiledName("Reduce")>]
        let reduce f (source : seq<'T>)  =
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)

            source
            |> foreach (fun _ ->
                { new SeqComposer.Folder<'T, SeqComposer.Values<bool,'T>> (SeqComposer.Values<_,_>(true, Unchecked.defaultof<'T>)) with
                    override this.ProcessNext value =
                        if this.Value._1 then
                            this.Value._1 <- false
                            this.Value._2 <- value
                        else
                            this.Value._2 <- f.Invoke (this.Value._2, value)
                        Unchecked.defaultof<bool>

                  interface SeqComposer.ISeqComponent with
                    member this.OnComplete _ = 
                        if (this:?>SeqComposer.Folder<'T, SeqComposer.Values<bool,'T>>).Value._1 then
                            invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
                })
            |> fun reduced -> reduced.Value._2

        [<CompiledName("Replicate")>]
        let replicate count x =
            #if FX_ATLEAST_40
            System.Linq.Enumerable.Repeat(x,count)
            #else
            if count < 0 then invalidArg "count" (SR.GetString(SR.inputMustBeNonNegative))
            seq { for _ in 1 .. count -> x }
            #endif


        [<CompiledName("Append")>]
        let append (source1: seq<'T>) (source2: seq<'T>) =
            checkNonNull "source1" source1
            checkNonNull "source2" source2
            match source1 with
            | :? SeqComposer.Enumerable.EnumerableBase<'T> as s -> s.Append source2
            | _ -> SeqComposer.Helpers.upcastEnumerable (new SeqComposer.Enumerable.AppendEnumerable<_>([source2; source1]))


        [<CompiledName("Collect")>]
        let collect f sources = map f sources |> concat

        [<CompiledName("CompareWith")>]
        let compareWith (f:'T -> 'T -> int) (source1 : seq<'T>) (source2: seq<'T>) =
            checkNonNull "source1" source1
            checkNonNull "source2" source2
            use e1 = source1.GetEnumerator()
            use e2 = source2.GetEnumerator()
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)
            let rec go () =
                let e1ok = e1.MoveNext()
                let e2ok = e2.MoveNext()
                let c = if e1ok = e2ok then 0 else if e1ok then 1 else -1
                if c <> 0 then c else
                if not e1ok || not e2ok then 0
                else
                    let c = f.Invoke(e1.Current, e2.Current)
                    if c <> 0 then c else
                    go ()
            go()

        [<CompiledName("OfList")>]
        let ofList (source : 'T list) =
            (source :> seq<'T>)

        [<CompiledName("ToList")>]
        let toList (source : seq<'T>) =
            checkNonNull "source" source
            Microsoft.FSharp.Primitives.Basics.List.ofSeq source

        // Create a new object to ensure underlying array may not be mutated by a backdoor cast
        [<CompiledName("OfArray")>]
        let ofArray (source : 'T array) =
            checkNonNull "source" source
            SeqComposer.Array.createId source

        [<CompiledName("ToArray")>]
        let toArray (source : seq<'T>)  =
            checkNonNull "source" source
            match source with
            | :? ('T[]) as res -> (res.Clone() :?> 'T[])
            | :? ('T list) as res -> List.toArray res
            | :? ICollection<'T> as res ->
                // Directly create an array and copy ourselves.
                // This avoids an extra copy if using ResizeArray in fallback below.
                let arr = Array.zeroCreateUnchecked res.Count
                res.CopyTo(arr, 0)
                arr
            | _ ->
                let res = ResizeArray<_>(source)
                res.ToArray()

        let foldArraySubRight (f:OptimizedClosures.FSharpFunc<'T,_,_>) (arr: 'T[]) start fin acc =
            let mutable state = acc
            for i = fin downto start do
                state <- f.Invoke(arr.[i], state)
            state

        [<CompiledName("FoldBack")>]
        let foldBack<'T,'State> f (source : seq<'T>) (x:'State) =
            checkNonNull "source" source
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)
            let arr = toArray source
            let len = arr.Length
            foldArraySubRight f arr 0 (len - 1) x

        [<CompiledName("FoldBack2")>]
        let foldBack2<'T1,'T2,'State> f (source1 : seq<'T1>) (source2 : seq<'T2>) (x:'State) =
            let zipped = zip source1 source2
            foldBack ((<||) f) zipped x

        [<CompiledName("ReduceBack")>]
        let reduceBack f (source : seq<'T>) =
            checkNonNull "source" source
            let arr = toArray source
            match arr.Length with
            | 0 -> invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
            | len ->
                let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)
                foldArraySubRight f arr 0 (len - 2) arr.[len - 1]

        [<CompiledName("Singleton")>]
        let singleton x = mkSeq (fun () -> IEnumerator.Singleton x)


        [<CompiledName("Truncate")>]
        let truncate n (source: seq<'T>) =
            if n <= 0 then empty else
            source |> seqFactory (SeqComposer.TruncateFactory n)

        [<CompiledName("Pairwise")>]
        let pairwise<'T> (source:seq<'T>) : seq<'T*'T> =
            source |> seqFactory (SeqComposer.PairwiseFactory ())

        [<CompiledName("Scan")>]
        let scan<'T,'State> f (z:'State) (source : seq<'T>): seq<'State> =
            let first = [|z|] :> IEnumerable<'State>
            let rest = source |> seqFactory (SeqComposer.ScanFactory (f, z))
            upcast SeqComposer.Enumerable.ConcatEnumerable [|first; rest;|]

        [<CompiledName("TryFindBack")>]
        let tryFindBack f (source : seq<'T>) =
            checkNonNull "source" source
            source |> toArray |> Array.tryFindBack f

        [<CompiledName("FindBack")>]
        let findBack f source =
            checkNonNull "source" source
            source |> toArray |> Array.findBack f

        [<CompiledName("ScanBack")>]
        let scanBack<'T,'State> f (source : seq<'T>) (acc:'State) =
            checkNonNull "source" source
            mkDelayedSeq(fun () ->
                let arr = source |> toArray
                let res = Array.scanSubRight f arr 0 (arr.Length - 1) acc
                res :> seq<_>)

        [<CompiledName("TryFindIndex")>]
        let tryFindIndex p (source:seq<_>) =
            source
            |> foreach (fun pipeline ->
                { new SeqComposer.Folder<'T, SeqComposer.Values<Option<int>, int>> (SeqComposer.Values<_,_>(None, 0)) with
                    override this.ProcessNext value =
                        if p value then
                            this.Value._1 <- Some(this.Value._2)
                            pipeline.StopFurtherProcessing 1
                        else
                            this.Value._2 <- this.Value._2 + 1
                        Unchecked.defaultof<bool>
                })
            |> fun tried -> tried.Value._1

        [<CompiledName("FindIndex")>]
        let findIndex p (source:seq<_>) =
            match tryFindIndex p source with
            | None -> indexNotFound()
            | Some x -> x

        [<CompiledName("TryFindIndexBack")>]
        let tryFindIndexBack f (source : seq<'T>) =
            checkNonNull "source" source
            source |> toArray |> Array.tryFindIndexBack f

        [<CompiledName("FindIndexBack")>]
        let findIndexBack f source =
            checkNonNull "source" source
            source |> toArray |> Array.findIndexBack f

        // windowed : int -> seq<'T> -> seq<'T[]>
        [<CompiledName("Windowed")>]
        let windowed windowSize (source: seq<_>) =
            checkNonNull "source" source
            if windowSize <= 0 then invalidArgFmt "windowSize" "{0}\nwindowSize = {1}"
                                        [|SR.GetString SR.inputMustBePositive; windowSize|]
            seq {
                let arr = Array.zeroCreateUnchecked windowSize
                let r = ref (windowSize - 1)
                let i = ref 0
                use e = source.GetEnumerator()
                while e.MoveNext() do
                    arr.[!i] <- e.Current
                    i := (!i + 1) % windowSize
                    if !r = 0 then
                        if windowSize < 32 then
                            yield Array.init windowSize (fun j -> arr.[(!i+j) % windowSize])
                        else
                            let result = Array.zeroCreateUnchecked windowSize
                            Array.Copy(arr, !i, result, 0, windowSize - !i)
                            Array.Copy(arr, 0, result, windowSize - !i, !i)
                            yield result
                    else r := (!r - 1)
            }

        [<CompiledName("Cache")>]
        let cache (source : seq<'T>) =
            checkNonNull "source" source
            // Wrap a seq to ensure that it is enumerated just once and only as far as is necessary.
            //
            // This code is required to be thread safe.
            // The necessary calls should be called at most once (include .MoveNext() = false).
            // The enumerator should be disposed (and dropped) when no longer required.
            //------
            // The state is (prefix,enumerator) with invariants:
            //   * the prefix followed by elts from the enumerator are the initial sequence.
            //   * the prefix contains only as many elements as the longest enumeration so far.
            let prefix      = ResizeArray<_>()
            let enumeratorR = ref None : IEnumerator<'T> option option ref // nested options rather than new type...
                               // None          = Unstarted.
                               // Some(Some e)  = Started.
                               // Some None     = Finished.
            let oneStepTo i =
              // If possible, step the enumeration to prefix length i (at most one step).
              // Be speculative, since this could have already happened via another thread.
              if not (i < prefix.Count) then // is a step still required?
                  // If not yet started, start it (create enumerator).
                  match !enumeratorR with
                  | None -> enumeratorR := Some (Some (source.GetEnumerator()))
                  | Some _ -> ()
                  match (!enumeratorR).Value with
                  | Some enumerator -> if enumerator.MoveNext() then
                                          prefix.Add(enumerator.Current)
                                       else
                                          enumerator.Dispose()     // Move failed, dispose enumerator,
                                          enumeratorR := Some None // drop it and record finished.
                  | None -> ()
            let result =
                unfold (fun i ->
                              // i being the next position to be returned
                              // A lock is needed over the reads to prefix.Count since the list may be being resized
                              // NOTE: we could change to a reader/writer lock here
                              lock enumeratorR (fun () ->
                                  if i < prefix.Count then
                                    Some (prefix.[i],i+1)
                                  else
                                    oneStepTo i
                                    if i < prefix.Count then
                                      Some (prefix.[i],i+1)
                                    else
                                      None)) 0
            let cleanup() =
               lock enumeratorR (fun () ->
                   prefix.Clear()
                   begin match !enumeratorR with
                   | Some (Some e) -> IEnumerator.dispose e
                   | _ -> ()
                   end
                   enumeratorR := None)
            (new CachedSeq<_>(cleanup, result) :> seq<_>)

        [<CompiledName("AllPairs")>]
        let allPairs source1 source2 =
            checkNonNull "source1" source1
            checkNonNull "source2" source2
            let cached = cache source2
            source1 |> collect (fun x -> cached |> map (fun y -> x,y))

        [<CodeAnalysis.SuppressMessage("Microsoft.Naming","CA1709:IdentifiersShouldBeCasedCorrectly"); CodeAnalysis.SuppressMessage("Microsoft.Naming","CA1707:IdentifiersShouldNotContainUnderscores"); CodeAnalysis.SuppressMessage("Microsoft.Naming","CA1704:IdentifiersShouldBeSpelledCorrectly")>]
        [<CompiledName("ReadOnly")>]
        let readonly (source:seq<_>) =
            checkNonNull "source" source
            mkSeq (fun () -> source.GetEnumerator())

        let inline groupByImpl (comparer:IEqualityComparer<'SafeKey>) (keyf:'T->'SafeKey) (getKey:'SafeKey->'Key) (seq:seq<'T>) =
            checkNonNull "seq" seq

            let dict = Dictionary<_,ResizeArray<_>> comparer

            // Previously this was 1, but I think this is rather stingy, considering that we are already paying
            // for at least a key, the ResizeArray reference, which includes an array reference, an Entry in the
            // Dictionary, plus any empty space in the Dictionary of unfilled hash buckets.
            let minimumBucketSize = 4

            // Build the groupings
            seq |> iter (fun v ->
                let safeKey = keyf v
                let mutable prev = Unchecked.defaultof<_>
                match dict.TryGetValue (safeKey, &prev) with
                | true -> prev.Add v
                | false ->
                    let prev = ResizeArray ()
                    dict.[safeKey] <- prev
                    prev.Add v)

            // Trim the size of each result group, don't trim very small buckets, as excessive work, and garbage for
            // minimal gain
            dict |> iter (fun group -> if group.Value.Count > minimumBucketSize then group.Value.TrimExcess())

            // Return the sequence-of-sequences. Don't reveal the
            // internal collections: just reveal them as sequences
            dict |> map (fun group -> (getKey group.Key, readonly group.Value))

        // We avoid wrapping a StructBox, because under 64 JIT we get some "hard" tailcalls which affect performance
        let groupByValueType (keyf:'T->'Key) (seq:seq<'T>) = seq |> groupByImpl HashIdentity.Structural<'Key> keyf id

        // Wrap a StructBox around all keys in case the key type is itself a type using null as a representation
        let groupByRefType   (keyf:'T->'Key) (seq:seq<'T>) = seq |> groupByImpl StructBox<'Key>.Comparer (fun t -> StructBox (keyf t)) (fun sb -> sb.Value)

        [<CompiledName("GroupBy")>]
        let groupBy (keyf:'T->'Key) (seq:seq<'T>) =
#if FX_RESHAPED_REFLECTION
            if (typeof<'Key>).GetTypeInfo().IsValueType
#else
            if typeof<'Key>.IsValueType
#endif
                then mkDelayedSeq (fun () -> groupByValueType keyf seq)
                else mkDelayedSeq (fun () -> groupByRefType   keyf seq)

        [<CompiledName("Distinct")>]
        let distinct source =
            source |> seqFactory (SeqComposer.DistinctFactory ())

        [<CompiledName("DistinctBy")>]
        let distinctBy keyf source =
            source |> seqFactory (SeqComposer.DistinctByFactory keyf)

        [<CompiledName("SortBy")>]
        let sortBy keyf source =
            checkNonNull "source" source
            let delayedSort () =
                let array = source |> toArray
                Array.stableSortInPlaceBy keyf array
                array
            SeqComposer.Array.createDelayedId delayedSort

        [<CompiledName("Sort")>]
        let sort source =
            checkNonNull "source" source
            let delayedSort () =
                let array = source |> toArray
                Array.stableSortInPlace array
                array
            SeqComposer.Array.createDelayedId delayedSort

        [<CompiledName("SortWith")>]
        let sortWith f source =
            checkNonNull "source" source
            let delayedSort () =
                let array = source |> toArray
                Array.stableSortInPlaceWith f array
                array
            SeqComposer.Array.createDelayedId delayedSort

        [<CompiledName("SortByDescending")>]
        let inline sortByDescending keyf source =
            checkNonNull "source" source
            let inline compareDescending a b = compare (keyf b) (keyf a)
            sortWith compareDescending source

        [<CompiledName("SortDescending")>]
        let inline sortDescending source =
            checkNonNull "source" source
            let inline compareDescending a b = compare b a
            sortWith compareDescending source

        let inline countByImpl (comparer:IEqualityComparer<'SafeKey>) (keyf:'T->'SafeKey) (getKey:'SafeKey->'Key) (source:seq<'T>) =
            checkNonNull "source" source

            let dict = Dictionary comparer

            // Build the groupings
            source |> iter (fun v ->
                let safeKey = keyf v
                let mutable prev = Unchecked.defaultof<_>
                if dict.TryGetValue(safeKey, &prev)
                    then dict.[safeKey] <- prev + 1
                    else dict.[safeKey] <- 1)

            dict |> map (fun group -> (getKey group.Key, group.Value))

        // We avoid wrapping a StructBox, because under 64 JIT we get some "hard" tailcalls which affect performance
        let countByValueType (keyf:'T->'Key) (seq:seq<'T>) = seq |> countByImpl HashIdentity.Structural<'Key> keyf id

        // Wrap a StructBox around all keys in case the key type is itself a type using null as a representation
        let countByRefType   (keyf:'T->'Key) (seq:seq<'T>) = seq |> countByImpl StructBox<'Key>.Comparer (fun t -> StructBox (keyf t)) (fun sb -> sb.Value)

        [<CompiledName("CountBy")>]
        let countBy (keyf:'T->'Key) (source:seq<'T>) =
            checkNonNull "source" source

#if FX_RESHAPED_REFLECTION
            if (typeof<'Key>).GetTypeInfo().IsValueType
#else
            if typeof<'Key>.IsValueType
#endif
                then mkDelayedSeq (fun () -> countByValueType keyf source)
                else mkDelayedSeq (fun () -> countByRefType   keyf source)
        
        [<CompiledName("Sum")>]
        let inline sum (source:seq<'a>) : 'a =
            source
            |> foreach (fun _ ->
                { new SeqComposer.Folder<'a,'a> (LanguagePrimitives.GenericZero) with
                    override this.ProcessNext value =
                        this.Value <- Checked.(+) this.Value value
                        Unchecked.defaultof<bool> })
            |> fun sum -> sum.Value

        [<CompiledName("SumBy")>]
        let inline sumBy (f : 'T -> ^U) (source: seq<'T>) : ^U =
            source
            |> foreach (fun _ ->
                { new SeqComposer.Folder<'T,'U> (LanguagePrimitives.GenericZero< ^U>) with
                    override this.ProcessNext value =
                        this.Value <- Checked.(+) this.Value (f value)
                        Unchecked.defaultof<bool> })
            |> fun sum -> sum.Value

        [<CompiledName("Average")>]
        let inline average (source: seq< ^a>) : ^a =
            source
            |> foreach (fun _ ->
                { new SeqComposer.Folder<'a, SeqComposer.Values<'a, int>> (SeqComposer.Values<_,_>(LanguagePrimitives.GenericZero, 0)) with
                    override this.ProcessNext value =
                        this.Value._1 <- Checked.(+) this.Value._1 value
                        this.Value._2 <- this.Value._2 + 1
                        Unchecked.defaultof<bool>

                  interface SeqComposer.ISeqComponent with
                    member this.OnComplete _ = 
                        if (this:?>SeqComposer.Folder<'a, SeqComposer.Values<'a, int>>).Value._2 = 0 then
                            invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString })
            |> fun total -> LanguagePrimitives.DivideByInt< ^a> total.Value._1 total.Value._2

        [<CompiledName("AverageBy")>]
        let inline averageBy (f : 'T -> ^U) (source: seq< 'T >) : ^U =
            source
            |> foreach (fun _ ->
                { new SeqComposer.Folder<'T,SeqComposer.Values<'U, int>> (SeqComposer.Values<_,_>(LanguagePrimitives.GenericZero, 0)) with
                    override this.ProcessNext value =
                        this.Value._1 <- Checked.(+) this.Value._1 (f value)
                        this.Value._2 <- this.Value._2 + 1
                        Unchecked.defaultof<bool>
                  interface SeqComposer.ISeqComponent with
                    member this.OnComplete _ = 
                        if (this:?>SeqComposer.Folder<'T,SeqComposer.Values<'U, int>>).Value._2 = 0 then
                            invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString })
            |> fun total -> LanguagePrimitives.DivideByInt< ^U> total.Value._1 total.Value._2

        [<CompiledName("Min")>]
        let inline min (source: seq<_>) =
            source
            |> foreach (fun _ ->
                { new SeqComposer.Folder<'T,SeqComposer.Values<bool,'T>> (SeqComposer.Values<_,_>(true, Unchecked.defaultof<'T>)) with
                    override this.ProcessNext value =
                        if this.Value._1 then
                            this.Value._1 <- false
                            this.Value._2 <- value
                        elif value < this.Value._2 then
                            this.Value._2 <- value
                        Unchecked.defaultof<bool>

                  interface SeqComposer.ISeqComponent with
                    member this.OnComplete _ = 
                        if (this:?>SeqComposer.Folder<'T,SeqComposer.Values<bool,'T>>).Value._1 then
                            invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
                })
            |> fun min -> min.Value._2

        [<CompiledName("MinBy")>]
        let inline minBy (f : 'T -> 'U) (source: seq<'T>) : 'T =
            source
            |> foreach (fun _ ->
                { new SeqComposer.Folder<'T,SeqComposer.Values<bool,'U,'T>> (SeqComposer.Values<_,_,_>(true,Unchecked.defaultof<'U>,Unchecked.defaultof<'T>)) with
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
                        Unchecked.defaultof<bool>

                  interface SeqComposer.ISeqComponent with
                    member this.OnComplete _ = 
                        if (this:?>SeqComposer.Folder<'T,SeqComposer.Values<bool,'U,'T>>).Value._1 then
                            invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
                })
            |> fun min -> min.Value._3
(*
        [<CompiledName("MinValueBy")>]
        let inline minValBy (f : 'T -> 'U) (source: seq<'T>) : 'U =
            checkNonNull "source" source
            use e = source.GetEnumerator()
            if not (e.MoveNext()) then
                invalidArg "source" InputSequenceEmptyString
            let first = e.Current
            let mutable acc = f first
            while e.MoveNext() do
                let currv = e.Current
                let curr = f currv
                if curr < acc then
                    acc <- curr
            acc

*)
        [<CompiledName("Max")>]
        let inline max (source: seq<_>) =
            source
            |> foreach (fun _ ->
                { new SeqComposer.Folder<'T,SeqComposer.Values<bool,'T>> (SeqComposer.Values<_,_>(true, Unchecked.defaultof<'T>)) with
                    override this.ProcessNext value =
                        if this.Value._1 then
                            this.Value._1 <- false
                            this.Value._2 <- value
                        elif value > this.Value._2 then
                            this.Value._2 <- value
                        Unchecked.defaultof<bool>

                  interface SeqComposer.ISeqComponent with
                    member this.OnComplete _ = 
                        if (this:?>SeqComposer.Folder<'T,SeqComposer.Values<bool,'T>>).Value._1 then
                            invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
                })
            |> fun max -> max.Value._2

        [<CompiledName("MaxBy")>]
        let inline maxBy (f : 'T -> 'U) (source: seq<'T>) : 'T =
            source
            |> foreach (fun _ ->
                { new SeqComposer.Folder<'T,SeqComposer.Values<bool,'U,'T>> (SeqComposer.Values<_,_,_>(true,Unchecked.defaultof<'U>,Unchecked.defaultof<'T>)) with
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
                        Unchecked.defaultof<bool>

                  interface SeqComposer.ISeqComponent with
                    member this.OnComplete _ = 
                        if (this:?>SeqComposer.Folder<'T,SeqComposer.Values<bool,'U,'T>>).Value._1 then
                            invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
                })
            |> fun min -> min.Value._3

(*
        [<CompiledName("MaxValueBy")>]
        let inline maxValBy (f : 'T -> 'U) (source: seq<'T>) : 'U =
            checkNonNull "source" source
            use e = source.GetEnumerator()
            if not (e.MoveNext()) then
                invalidArg "source" InputSequenceEmptyString
            let first = e.Current
            let mutable acc = f first
            while e.MoveNext() do
                let currv = e.Current
                let curr = f currv
                if curr > acc then
                    acc <- curr
            acc

*)
        [<CompiledName("TakeWhile")>]
        let takeWhile p (source: seq<_>) =
            source |> seqFactory (SeqComposer.TakeWhileFactory p)

        [<CompiledName("Skip")>]
        let skip count (source: seq<_>) =
            source |> seqFactory (SeqComposer.SkipFactory count)

        [<CompiledName("SkipWhile")>]
        let skipWhile p (source: seq<_>) =
            source |> seqFactory (SeqComposer.SkipWhileFactory p)

        [<CompiledName("ForAll2")>]
        let forall2 p (source1: seq<_>) (source2: seq<_>) =
            checkNonNull "source1" source1
            checkNonNull "source2" source2
            use e1 = source1.GetEnumerator()
            use e2 = source2.GetEnumerator()
            let p = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(p)
            let mutable ok = true
            while (ok && e1.MoveNext() && e2.MoveNext()) do
                ok <- p.Invoke(e1.Current, e2.Current)
            ok

        [<CompiledName("Exists2")>]
        let exists2 p (source1: seq<_>) (source2: seq<_>) =
            checkNonNull "source1" source1
            checkNonNull "source2" source2
            use e1 = source1.GetEnumerator()
            use e2 = source2.GetEnumerator()
            let p = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(p)
            let mutable ok = false
            while (not ok && e1.MoveNext() && e2.MoveNext()) do
                ok <- p.Invoke(e1.Current, e2.Current)
            ok
        
        [<CompiledName("TryHead")>]
        let tryHead (source : seq<_>) =
            source
            |> foreach (fun pipeline ->
                { new SeqComposer.Folder<'T, Option<'T>> (None) with
                    override this.ProcessNext value =
                        this.Value <- Some value
                        pipeline.StopFurtherProcessing 1
                        Unchecked.defaultof<bool> })
            |> fun head -> head.Value

        [<CompiledName("Head")>]
        let head (source : seq<_>) =
            match tryHead source with
            | None -> invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
            | Some x -> x

        [<CompiledName("Tail")>]
        let tail (source: seq<'T>) =
            source |> seqFactory (SeqComposer.TailFactory ())
        
        [<CompiledName("TryLast")>]
        let tryLast (source : seq<_>) =
            source
            |> foreach (fun _ ->
                { new SeqComposer.Folder<'T, SeqComposer.Values<bool,'T>> (SeqComposer.Values<bool,'T>(true, Unchecked.defaultof<'T>)) with
                    override this.ProcessNext value =
                        if this.Value._1 then
                            this.Value._1 <- false
                        this.Value._2 <- value
                        Unchecked.defaultof<bool> })
            |> fun tried -> 
                if tried.Value._1 then
                    None
                else
                    Some tried.Value._2

        [<CompiledName("Last")>]
        let last (source : seq<_>) =
            match tryLast source with
            | None -> invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
            | Some x -> x

        [<CompiledName("ExactlyOne")>]
        let exactlyOne (source : seq<_>) =
            source
            |> foreach (fun pipeline ->
                { new SeqComposer.Folder<'T, SeqComposer.Values<bool,'T, bool>> (SeqComposer.Values<bool,'T, bool>(true, Unchecked.defaultof<'T>, false)) with
                    override this.ProcessNext value =
                        if this.Value._1 then
                            this.Value._1 <- false
                            this.Value._2 <- value
                        else
                            this.Value._3 <- true
                            pipeline.StopFurtherProcessing 1
                        Unchecked.defaultof<bool> 
                   interface SeqComposer.ISeqComponent with
                      member this.OnComplete _ = 
                        let value =  (this:?>SeqComposer.Folder<'T,SeqComposer.Values<bool,'T, bool>>)
                        if value.Value._1 then
                            invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
                        elif value.Value._3 then
                            invalidArg "source" (SR.GetString(SR.inputSequenceTooLong))
                            })
            |> fun one -> one.Value._2

        [<CompiledName("Reverse")>]
        let rev source =
            checkNonNull "source" source
            let delayedReverse () = 
                let array = source |> toArray 
                Array.Reverse array
                array
            SeqComposer.Array.createDelayedId delayedReverse

        [<CompiledName("Permute")>]
        let permute f (source:seq<_>) =
            checkNonNull "source" source
            let delayedPermute () =
                source
                |> toArray
                |> Array.permute f
            SeqComposer.Array.createDelayedId delayedPermute

        [<CompiledName("MapFold")>]
        let mapFold<'T,'State,'Result> (f: 'State -> 'T -> 'Result * 'State) acc source =
            checkNonNull "source" source
            let arr,state = source |> toArray |> Array.mapFold f acc
            readonly arr, state

        [<CompiledName("MapFoldBack")>]
        let mapFoldBack<'T,'State,'Result> (f: 'T -> 'State -> 'Result * 'State) source acc =
            checkNonNull "source" source
            let array = source |> toArray
            let arr,state = Array.mapFoldBack f array acc
            readonly arr, state

        [<CompiledName("Except")>]
        let except (itemsToExclude: seq<'T>) (source: seq<'T>) =
            checkNonNull "itemsToExclude" itemsToExclude
            source |> seqFactory (SeqComposer.ExceptFactory itemsToExclude)

        [<CompiledName("ChunkBySize")>]
        let chunkBySize chunkSize (source : seq<_>) =
            checkNonNull "source" source
            if chunkSize <= 0 then invalidArgFmt "chunkSize" "{0}\nchunkSize = {1}"
                                    [|SR.GetString SR.inputMustBePositive; chunkSize|]
            seq { use e = source.GetEnumerator()
                  let nextChunk() =
                      let res = Array.zeroCreateUnchecked chunkSize
                      res.[0] <- e.Current
                      let i = ref 1
                      while !i < chunkSize && e.MoveNext() do
                          res.[!i] <- e.Current
                          i := !i + 1
                      if !i = chunkSize then
                          res
                      else
                          res |> Array.subUnchecked 0 !i
                  while e.MoveNext() do
                      yield nextChunk() }

        [<CompiledName("SplitInto")>]
        let splitInto count source =
            checkNonNull "source" source
            if count <= 0 then invalidArgFmt "count" "{0}\ncount = {1}"
                                [|SR.GetString SR.inputMustBePositive; count|]
            mkDelayedSeq (fun () ->
                source |> toArray |> Array.splitInto count :> seq<_>)
