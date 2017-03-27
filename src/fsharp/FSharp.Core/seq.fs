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

    module Internal =
     module IEnumerator =
      open Microsoft.FSharp.Collections.IEnumerator

      let rec tryItem index (e : IEnumerator<'T>) =
          if not (e.MoveNext()) then None
          elif index = 0 then Some(e.Current)
          else tryItem (index-1) e

      let rec nth index (e : IEnumerator<'T>) =
          if not (e.MoveNext()) then
            let shortBy = index + 1
            invalidArgFmt "index"
                "{0}\nseq was short by {1} {2}"
                [|SR.GetString SR.notEnoughElements; shortBy; (if shortBy = 1 then "element" else "elements")|]
          if index = 0 then e.Current
          else nth (index-1) e

      [<NoEquality; NoComparison>]
      type MapEnumeratorState =
          | NotStarted
          | InProcess
          | Finished

      [<AbstractClass>]
      type MapEnumerator<'T> () =
          let mutable state = NotStarted
          [<DefaultValue(false)>]
          val mutable private curr : 'T

          member this.GetCurrent () =
              match state with
              |   NotStarted -> notStarted()
              |   Finished -> alreadyFinished()
              |   InProcess -> ()
              this.curr

          abstract DoMoveNext : byref<'T> -> bool
          abstract Dispose : unit -> unit

          interface IEnumerator<'T> with
              member this.Current = this.GetCurrent()

          interface IEnumerator with
              member this.Current = box(this.GetCurrent())
              member this.MoveNext () =
                  state <- InProcess
                  if this.DoMoveNext(&this.curr) then
                      true
                  else
                      state <- Finished
                      false
              member this.Reset() = noReset()
          interface System.IDisposable with
              member this.Dispose() = this.Dispose()

      let map f (e : IEnumerator<_>) : IEnumerator<_>=
          upcast
              { new MapEnumerator<_>() with
                    member this.DoMoveNext (curr : byref<_>) =
                        if e.MoveNext() then
                            curr <- (f e.Current)
                            true
                        else
                            false
                    member this.Dispose() = e.Dispose()
              }

      let mapi f (e : IEnumerator<_>) : IEnumerator<_> =
          let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)
          let i = ref (-1)
          upcast
              {  new MapEnumerator<_>() with
                     member this.DoMoveNext curr =
                        i := !i + 1
                        if e.MoveNext() then
                           curr <- f.Invoke(!i, e.Current)
                           true
                        else
                           false
                     member this.Dispose() = e.Dispose()
              }

      let map2 f (e1 : IEnumerator<_>) (e2 : IEnumerator<_>) : IEnumerator<_>=
          let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)
          upcast
              {  new MapEnumerator<_>() with
                     member this.DoMoveNext curr =
                        let n1 = e1.MoveNext()
                        let n2 = e2.MoveNext()
                        if n1 && n2 then
                           curr <- f.Invoke(e1.Current, e2.Current)
                           true
                        else
                           false
                     member this.Dispose() =
                        try
                            e1.Dispose()
                        finally
                            e2.Dispose()
              }

      let mapi2 f (e1 : IEnumerator<_>) (e2 : IEnumerator<_>) : IEnumerator<_> =
          let f = OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt(f)
          let i = ref (-1)
          upcast
              {  new MapEnumerator<_>() with
                     member this.DoMoveNext curr =
                        i := !i + 1
                        if (e1.MoveNext() && e2.MoveNext()) then
                           curr <- f.Invoke(!i, e1.Current, e2.Current)
                           true
                        else
                           false
                     member this.Dispose() =
                        try
                            e1.Dispose()
                        finally
                            e2.Dispose()
              }

      let map3 f (e1 : IEnumerator<_>) (e2 : IEnumerator<_>) (e3 : IEnumerator<_>) : IEnumerator<_> =
        let f = OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt(f)
        upcast
            {  new MapEnumerator<_>() with
                   member this.DoMoveNext curr =
                      let n1 = e1.MoveNext()
                      let n2 = e2.MoveNext()
                      let n3 = e3.MoveNext()

                      if n1 && n2 && n3 then
                         curr <- f.Invoke(e1.Current, e2.Current, e3.Current)
                         true
                      else
                         false
                   member this.Dispose() =
                      try
                          e1.Dispose()
                      finally
                          try
                              e2.Dispose()
                          finally
                              e3.Dispose()
            }

      let choose f (e : IEnumerator<'T>) =
          let started = ref false
          let curr = ref None
          let get() =  check !started; (match !curr with None -> alreadyFinished() | Some x -> x)
          { new IEnumerator<'U> with
                member x.Current = get()
            interface IEnumerator with
                member x.Current = box (get())
                member x.MoveNext() =
                    if not !started then started := true
                    curr := None
                    while ((!curr).IsNone && e.MoveNext()) do
                        curr := f e.Current
                    Option.isSome !curr
                member x.Reset() = noReset()
            interface System.IDisposable with
                member x.Dispose() = e.Dispose()  }

      let filter f (e : IEnumerator<'T>) =
          let started = ref false
          let this =
              { new IEnumerator<'T> with
                    member x.Current = check !started; e.Current
                interface IEnumerator with
                    member x.Current = check !started; box e.Current
                    member x.MoveNext() =
                        let rec next() =
                            if not !started then started := true
                            e.MoveNext() && (f  e.Current || next())
                        next()
                    member x.Reset() = noReset()
                interface System.IDisposable with
                    member x.Dispose() = e.Dispose()  }
          this

      let unfold f x : IEnumerator<_> =
          let state = ref x
          upcast
              {  new MapEnumerator<_>() with
                    member this.DoMoveNext curr =
                        match f !state with
                        |   None -> false
                        |   Some(r,s) ->
                                curr <- r
                                state := s
                                true
                    member this.Dispose() = ()
              }

      let upto lastOption f =
          match lastOption with
          | Some b when b<0 -> Empty()    // a request for -ve length returns empty sequence
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

      [<Sealed>]
      type ArrayEnumerator<'T>(arr: 'T array) =
          let mutable curr = -1
          let mutable len = arr.Length
          member x.Get() =
               if curr >= 0 then
                 if curr >= len then alreadyFinished()
                 else arr.[curr]
               else
                 notStarted()
          interface IEnumerator<'T> with
                member x.Current = x.Get()
          interface System.Collections.IEnumerator with
                member x.MoveNext() =
                       if curr >= len then false
                       else
                         curr <- curr + 1
                         (curr < len)
                member x.Current = box(x.Get())
                member x.Reset() = noReset()
          interface System.IDisposable with
                member x.Dispose() = ()

      let ofArray arr = (new ArrayEnumerator<'T>(arr) :> IEnumerator<'T>)

    // Use generators for some implementations of IEnumerables.
    //
    module Generator =

        open System.Collections
        open System.Collections.Generic

        [<NoEquality; NoComparison>]
        type Step<'T> =
            | Stop
            | Yield of 'T
            | Goto of Generator<'T>

        and Generator<'T> =
            abstract Apply: (unit -> Step<'T>)
            abstract Disposer: (unit -> unit) option

        let disposeG (g:Generator<'T>) =
            match g.Disposer with
            | None -> ()
            | Some f -> f()

        let appG (g:Generator<_>) =
            //System.Console.WriteLine("{0}.appG", box g)
            let res = g.Apply()
            match res with
            | Goto(next) ->
                Goto(next)
            | Yield _ ->
                res
            | Stop ->
                //System.Console.WriteLine("appG: Stop")
                disposeG g
                res

        // Binding.
        //
        // We use a type definition to apply a local dynamic optimization.
        // We automatically right-associate binding, i.e. push the continuations to the right.
        // That is, bindG (bindG G1 cont1) cont2 --> bindG G1 (cont1 o cont2)
        // This makes constructs such as the following linear rather than quadratic:
        //
        //  let rec rwalk n = { if n > 0 then
        //                         yield! rwalk (n-1)
        //                         yield n }

        type GenerateThen<'T>(g:Generator<'T>, cont : unit -> Generator<'T>) =
            member self.Generator = g
            member self.Cont = cont
            interface Generator<'T> with
                 member x.Apply = (fun () ->
                      match appG g with
                      | Stop ->
                          // OK, move onto the generator given by the continuation
                          Goto(cont())

                      | Yield _ as res ->
                          res

                      | Goto next ->
                          Goto(GenerateThen<_>.Bind(next,cont)))
                 member x.Disposer =
                      g.Disposer


            static member Bind (g:Generator<'T>, cont) =
                match g with
                | :? GenerateThen<'T> as g -> GenerateThen<_>.Bind(g.Generator,(fun () -> GenerateThen<_>.Bind (g.Cont(), cont)))
                | g -> (new GenerateThen<'T>(g, cont) :> Generator<'T>)


        let bindG g cont = GenerateThen<_>.Bind(g,cont)


        // Internal type. Drive an underlying generator. Crucially when the generator returns
        // a new generator we simply update our current generator and continue. Thus the enumerator
        // effectively acts as a reference cell holding the current generator. This means that
        // infinite or large generation chains (e.g. caused by long sequences of append's, including
        // possible delay loops) can be referenced via a single enumerator.
        //
        // A classic case where this arises in this sort of sequence expression:
        //    let rec data s = { yield s;
        //                       yield! data (s + random()) }
        //
        // This translates to
        //    let rec data s = Seq.delay (fun () -> Seq.append (Seq.singleton s) (Seq.delay (fun () -> data (s+random()))))
        //
        // When you unwind through all the Seq, IEnumerator and Generator objects created,
        // you get (data s).GetEnumerator being an "GenerateFromEnumerator(EnumeratorWrappingLazyGenerator(...))" for the append.
        // After one element is yielded, we move on to the generator for the inner delay, which in turn
        // comes back to be a "GenerateFromEnumerator(EnumeratorWrappingLazyGenerator(...))".
        //
        // Defined as a type so we can optimize Enumerator/Generator chains in enumerateFromLazyGenerator
        // and GenerateFromEnumerator.

        [<Sealed>]
        type EnumeratorWrappingLazyGenerator<'T>(g:Generator<'T>) =
            let mutable g = g
            let mutable curr = None
            let mutable finished = false
            member e.Generator = g
            interface IEnumerator<'T> with
                member x.Current= match curr with Some(v) -> v | None -> raise <| System.InvalidOperationException (SR.GetString(SR.moveNextNotCalledOrFinished))
            interface System.Collections.IEnumerator with
                member x.Current = box (x :> IEnumerator<_>).Current
                member x.MoveNext() =
                    not finished &&
                    (match appG g with
                     | Stop ->
                        curr <- None
                        finished <- true
                        false
                     | Yield(v) ->
                        curr <- Some(v)
                        true
                     | Goto(next) ->
                        (g <- next)
                        (x :> IEnumerator).MoveNext())
                member x.Reset() = IEnumerator.noReset()
            interface System.IDisposable with
                member x.Dispose() =
                    if not finished then disposeG g

        // Internal type, used to optimize Enumerator/Generator chains
        type LazyGeneratorWrappingEnumerator<'T>(e:System.Collections.Generic.IEnumerator<'T>) =
            member g.Enumerator = e
            interface Generator<'T> with
                member g.Apply = (fun () ->
                    if e.MoveNext() then
                        Yield(e.Current)
                    else
                        Stop)
                member g.Disposer= Some(e.Dispose)

        let EnumerateFromGenerator(g:Generator<'T>) =
            match g with
            | :? LazyGeneratorWrappingEnumerator<'T> as g -> g.Enumerator
            | _ -> (new EnumeratorWrappingLazyGenerator<_>(g) :> System.Collections.Generic.IEnumerator<_>)

        let GenerateFromEnumerator (e:System.Collections.Generic.IEnumerator<'T>) =
            match e with
            | :? EnumeratorWrappingLazyGenerator<'T> as e ->  e.Generator
            | _ -> (new LazyGeneratorWrappingEnumerator<'T>(e) :> Generator<'T>)


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

#if FX_NO_ICLONEABLE
        open Microsoft.FSharp.Core.ICloneableExtensions
#else
#endif

        open Microsoft.FSharp.Collections.Internal
        open Microsoft.FSharp.Collections.IEnumerator

        let mkDelayedSeq (f: unit -> IEnumerable<'T>) = mkSeq (fun () -> f().GetEnumerator())
        let mkUnfoldSeq f x = mkSeq (fun () -> IEnumerator.unfold f x)
        let inline indexNotFound() = raise (new System.Collections.Generic.KeyNotFoundException(SR.GetString(SR.keyNotFoundAlt)))

        [<CompiledName("Delay")>]
        let delay f = mkDelayedSeq f

        [<CompiledName("Unfold")>]
        let unfold f x = mkUnfoldSeq f x

        [<CompiledName("Empty")>]
        let empty<'T> = (EmptyEnumerable :> seq<'T>)

        [<CompiledName("InitializeInfinite")>]
        let initInfinite f = mkSeq (fun () -> IEnumerator.upto None f)

        [<CompiledName("Initialize")>]
        let init count f =
            if count < 0 then invalidArgInputMustBeNonNegative "count" count
            mkSeq (fun () -> IEnumerator.upto (Some (count-1)) f)

        [<CompiledName("Iterate")>]
        let iter f (source : seq<'T>) =
            checkNonNull "source" source
            use e = source.GetEnumerator()
            while e.MoveNext() do
                f e.Current

        [<CompiledName("Item")>]
        let item i (source : seq<'T>) =
            checkNonNull "source" source
            if i < 0 then invalidArgInputMustBeNonNegative "index" i
            use e = source.GetEnumerator()
            IEnumerator.nth i e

        [<CompiledName("TryItem")>]
        let tryItem i (source : seq<'T>) =
            checkNonNull "source" source
            if i < 0 then None else
            use e = source.GetEnumerator()
            IEnumerator.tryItem i e

        [<CompiledName("Get")>]
        let nth i (source : seq<'T>) = item i source

        [<CompiledName("IterateIndexed")>]
        let iteri f (source : seq<'T>) =
            checkNonNull "source" source
            use e = source.GetEnumerator()
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)
            let mutable i = 0
            while e.MoveNext() do
                f.Invoke(i, e.Current)
                i <- i + 1

        [<CompiledName("Exists")>]
        let exists f (source : seq<'T>) =
            checkNonNull "source" source
            use e = source.GetEnumerator()
            let mutable state = false
            while (not state && e.MoveNext()) do
                state <- f e.Current
            state

        [<CompiledName("Contains")>]
        let inline contains element (source : seq<'T>) =
            checkNonNull "source" source
            use e = source.GetEnumerator()
            let mutable state = false
            while (not state && e.MoveNext()) do
                state <- element = e.Current
            state

        [<CompiledName("ForAll")>]
        let forall f (source : seq<'T>) =
            checkNonNull "source" source
            use e = source.GetEnumerator()
            let mutable state = true
            while (state && e.MoveNext()) do
                state <- f e.Current
            state


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

        [<CompiledName("Filter")>]
        let filter f source      =
            checkNonNull "source" source
            revamp  (IEnumerator.filter f) source

        [<CompiledName("Where")>]
        let where f source      = filter f source

        [<CompiledName("Map")>]
        let map    f source      =
            checkNonNull "source" source
            revamp  (IEnumerator.map    f) source

        [<CompiledName("MapIndexed")>]
        let mapi f source      =
            checkNonNull "source" source
            revamp  (IEnumerator.mapi   f) source

        [<CompiledName("MapIndexed2")>]
        let mapi2 f source1 source2 =
            checkNonNull "source1" source1
            checkNonNull "source2" source2
            revamp2 (IEnumerator.mapi2    f) source1 source2

        [<CompiledName("Map2")>]
        let map2 f source1 source2 =
            checkNonNull "source1" source1
            checkNonNull "source2" source2
            revamp2 (IEnumerator.map2    f) source1 source2

        [<CompiledName("Map3")>]
        let map3 f source1 source2 source3 =
            checkNonNull "source1" source1
            checkNonNull "source2" source2
            checkNonNull "source3" source3
            revamp3 (IEnumerator.map3    f) source1 source2 source3

        [<CompiledName("Choose")>]
        let choose f source      =
            checkNonNull "source" source
            revamp  (IEnumerator.choose f) source

        [<CompiledName("Indexed")>]
        let indexed source =
            checkNonNull "source" source
            mapi (fun i x -> i,x) source

        [<CompiledName("Zip")>]
        let zip source1 source2  =
            checkNonNull "source1" source1
            checkNonNull "source2" source2
            map2 (fun x y -> x,y) source1 source2

        [<CompiledName("Zip3")>]
        let zip3 source1 source2  source3 =
            checkNonNull "source1" source1
            checkNonNull "source2" source2
            checkNonNull "source3" source3
            map2 (fun x (y,z) -> x,y,z) source1 (zip source2 source3)

        [<CompiledName("Cast")>]
        let cast (source: IEnumerable) =
            checkNonNull "source" source
            mkSeq (fun () -> IEnumerator.cast (source.GetEnumerator()))

        [<CompiledName("TryPick")>]
        let tryPick f (source : seq<'T>)  =
            checkNonNull "source" source
            use e = source.GetEnumerator()
            let mutable res = None
            while (Option.isNone res && e.MoveNext()) do
                res <-  f e.Current
            res

        [<CompiledName("Pick")>]
        let pick f source  =
            checkNonNull "source" source
            match tryPick f source with
            | None -> indexNotFound()
            | Some x -> x

        [<CompiledName("TryFind")>]
        let tryFind f (source : seq<'T>)  =
            checkNonNull "source" source
            use e = source.GetEnumerator()
            let mutable res = None
            while (Option.isNone res && e.MoveNext()) do
                let c = e.Current
                if f c then res <- Some(c)
            res

        [<CompiledName("Find")>]
        let find f source =
            checkNonNull "source" source
            match tryFind f source with
            | None -> indexNotFound()
            | Some x -> x

        [<CompiledName("Take")>]
        let take count (source : seq<'T>)    =
            checkNonNull "source" source
            if count < 0 then invalidArgInputMustBeNonNegative "count" count
            (* Note: don't create or dispose any IEnumerable if n = 0 *)
            if count = 0 then empty else
            seq { use e = source.GetEnumerator()
                  for x in 0 .. count - 1 do
                      if not (e.MoveNext()) then
                          invalidOpFmt "tried to take {0} {1} past the end of the seq"
                            [|SR.GetString SR.notEnoughElements; x; (if x=1 then "element" else "elements")|]
                      yield e.Current }

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
        let concat sources =
            checkNonNull "sources" sources
            RuntimeHelpers.mkConcatSeq sources

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
        let fold<'T,'State> f (x:'State) (source : seq<'T>)  =
            checkNonNull "source" source
            use e = source.GetEnumerator()
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)
            let mutable state = x
            while e.MoveNext() do
                state <- f.Invoke(state, e.Current)
            state

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
            checkNonNull "source" source
            use e = source.GetEnumerator()
            if not (e.MoveNext()) then invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)
            let mutable state = e.Current
            while e.MoveNext() do
                state <- f.Invoke(state, e.Current)
            state

        let fromGenerator f = mkSeq(fun () -> Generator.EnumerateFromGenerator (f()))
        let toGenerator (ie : seq<_>) = Generator.GenerateFromEnumerator (ie.GetEnumerator())

        [<CompiledName("Replicate")>]
        let replicate count x =
            System.Linq.Enumerable.Repeat(x,count)

        [<CompiledName("Append")>]
        let append (source1: seq<'T>) (source2: seq<'T>) =
            checkNonNull "source1" source1
            checkNonNull "source2" source2
            fromGenerator(fun () -> Generator.bindG (toGenerator source1) (fun () -> toGenerator source2))


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
            mkSeq (fun () -> IEnumerator.ofArray source)

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
            checkNonNull "source" source
            seq { let i = ref 0
                  use ie = source.GetEnumerator()
                  while !i < n && ie.MoveNext() do
                     i := !i + 1
                     yield ie.Current }

        [<CompiledName("Pairwise")>]
        let pairwise (source: seq<'T>) =
            checkNonNull "source" source
            seq { use ie = source.GetEnumerator()
                  if ie.MoveNext() then
                      let iref = ref ie.Current
                      while ie.MoveNext() do
                          let j = ie.Current
                          yield (!iref, j)
                          iref := j }

        [<CompiledName("Scan")>]
        let scan<'T,'State> f (z:'State) (source : seq<'T>) =
            checkNonNull "source" source
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)
            seq { let zref = ref z
                  yield !zref
                  use ie = source.GetEnumerator()
                  while ie.MoveNext() do
                      zref := f.Invoke(!zref, ie.Current)
                      yield !zref }

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

        [<CompiledName("FindIndex")>]
        let findIndex p (source:seq<_>) =
            checkNonNull "source" source
            use ie = source.GetEnumerator()
            let rec loop i =
                if ie.MoveNext() then
                    if p ie.Current then
                        i
                    else loop (i+1)
                else
                    indexNotFound()
            loop 0

        [<CompiledName("TryFindIndex")>]
        let tryFindIndex p (source:seq<_>) =
            checkNonNull "source" source
            use ie = source.GetEnumerator()
            let rec loop i =
                if ie.MoveNext() then
                    if p ie.Current then
                        Some i
                    else loop (i+1)
                else
                    None
            loop 0

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
        let groupByRefType   (keyf:'T->'Key) (seq:seq<'T>) = seq |> groupByImpl RuntimeHelpers.StructBox<'Key>.Comparer (fun t -> RuntimeHelpers.StructBox (keyf t)) (fun sb -> sb.Value)

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
            checkNonNull "source" source
            seq { let hashSet = HashSet<'T>(HashIdentity.Structural<'T>)
                  for v in source do
                      if hashSet.Add(v) then
                          yield v }

        [<CompiledName("DistinctBy")>]
        let distinctBy keyf source =
            checkNonNull "source" source
            seq { let hashSet = HashSet<_>(HashIdentity.Structural<_>)
                  for v in source do
                    if hashSet.Add(keyf v) then
                        yield v }

        [<CompiledName("SortBy")>]
        let sortBy keyf source =
            checkNonNull "source" source
            mkDelayedSeq (fun () ->
                let array = source |> toArray
                Array.stableSortInPlaceBy keyf array
                array :> seq<_>)

        [<CompiledName("Sort")>]
        let sort source =
            checkNonNull "source" source
            mkDelayedSeq (fun () ->
                let array = source |> toArray
                Array.stableSortInPlace array
                array :> seq<_>)

        [<CompiledName("SortWith")>]
        let sortWith f source =
            checkNonNull "source" source
            mkDelayedSeq (fun () ->
                let array = source |> toArray
                Array.stableSortInPlaceWith f array
                array :> seq<_>)

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
        let countByRefType   (keyf:'T->'Key) (seq:seq<'T>) = seq |> countByImpl RuntimeHelpers.StructBox<'Key>.Comparer (fun t -> RuntimeHelpers.StructBox (keyf t)) (fun sb -> sb.Value)

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
        let inline sum (source: seq< ^a>) : ^a =
            use e = source.GetEnumerator()
            let mutable acc = LanguagePrimitives.GenericZero< ^a>
            while e.MoveNext() do
                acc <- Checked.(+) acc e.Current
            acc

        [<CompiledName("SumBy")>]
        let inline sumBy (f : 'T -> ^U) (source: seq<'T>) : ^U =
            use e = source.GetEnumerator()
            let mutable acc = LanguagePrimitives.GenericZero< ^U>
            while e.MoveNext() do
                acc <- Checked.(+) acc (f e.Current)
            acc

        [<CompiledName("Average")>]
        let inline average (source: seq< ^a>) : ^a =
            checkNonNull "source" source
            use e = source.GetEnumerator()
            let mutable acc = LanguagePrimitives.GenericZero< ^a>
            let mutable count = 0
            while e.MoveNext() do
                acc <- Checked.(+) acc e.Current
                count <- count + 1
            if count = 0 then
                invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
            LanguagePrimitives.DivideByInt< ^a> acc count

        [<CompiledName("AverageBy")>]
        let inline averageBy (f : 'T -> ^U) (source: seq< 'T >) : ^U =
            checkNonNull "source" source
            use e = source.GetEnumerator()
            let mutable acc = LanguagePrimitives.GenericZero< ^U>
            let mutable count = 0
            while e.MoveNext() do
                acc <- Checked.(+) acc (f e.Current)
                count <- count + 1
            if count = 0 then
                invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
            LanguagePrimitives.DivideByInt< ^U> acc count

        [<CompiledName("Min")>]
        let inline min (source: seq<_>) =
            checkNonNull "source" source
            use e = source.GetEnumerator()
            if not (e.MoveNext()) then
                invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
            let mutable acc = e.Current
            while e.MoveNext() do
                let curr = e.Current
                if curr < acc then
                    acc <- curr
            acc

        [<CompiledName("MinBy")>]
        let inline minBy (f : 'T -> 'U) (source: seq<'T>) : 'T =
            checkNonNull "source" source
            use e = source.GetEnumerator()
            if not (e.MoveNext()) then
                invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
            let first = e.Current
            let mutable acc = f first
            let mutable accv = first
            while e.MoveNext() do
                let currv = e.Current
                let curr = f currv
                if curr < acc then
                    acc <- curr
                    accv <- currv
            accv

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
            checkNonNull "source" source
            use e = source.GetEnumerator()
            if not (e.MoveNext()) then
                invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
            let mutable acc = e.Current
            while e.MoveNext() do
                let curr = e.Current
                if curr > acc then
                    acc <- curr
            acc

        [<CompiledName("MaxBy")>]
        let inline maxBy (f : 'T -> 'U) (source: seq<'T>) : 'T =
            checkNonNull "source" source
            use e = source.GetEnumerator()
            if not (e.MoveNext()) then
                invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
            let first = e.Current
            let mutable acc = f first
            let mutable accv = first
            while e.MoveNext() do
                let currv = e.Current
                let curr = f currv
                if curr > acc then
                    acc <- curr
                    accv <- currv
            accv


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
            checkNonNull "source" source
            seq { use e = source.GetEnumerator()
                  let latest = ref Unchecked.defaultof<_>
                  while e.MoveNext() && (latest := e.Current; p !latest) do
                      yield !latest }

        [<CompiledName("Skip")>]
        let skip count (source: seq<_>) =
            checkNonNull "source" source
            seq { use e = source.GetEnumerator()
                  for x in 1 .. count do
                      if not (e.MoveNext()) then
                        invalidOpFmt "tried to skip {0} {1} past the end of the seq"
                          [|SR.GetString SR.notEnoughElements; x; (if x=1 then "element" else "elements")|]
                  while e.MoveNext() do
                      yield e.Current }

        [<CompiledName("SkipWhile")>]
        let skipWhile p (source: seq<_>) =
            checkNonNull "source" source
            seq { use e = source.GetEnumerator()
                  let latest = ref (Unchecked.defaultof<_>)
                  let ok = ref false
                  while e.MoveNext() do
                      if (latest := e.Current; (!ok || not (p !latest))) then
                          ok := true
                          yield !latest }


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

        [<CompiledName("Head")>]
        let head (source : seq<_>) =
            checkNonNull "source" source
            use e = source.GetEnumerator()
            if (e.MoveNext()) then e.Current
            else invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString

        [<CompiledName("TryHead")>]
        let tryHead (source : seq<_>) =
            checkNonNull "source" source
            use e = source.GetEnumerator()
            if (e.MoveNext()) then Some e.Current
            else None

        [<CompiledName("Tail")>]
        let tail (source: seq<'T>) =
            checkNonNull "source" source
            seq { use e = source.GetEnumerator()
                  if not (e.MoveNext()) then
                      invalidArg "source" (SR.GetString(SR.notEnoughElements))
                  while e.MoveNext() do
                      yield e.Current }

        [<CompiledName("Last")>]
        let last (source : seq<_>) =
            checkNonNull "source" source
            use e = source.GetEnumerator()
            if e.MoveNext() then
                let mutable res = e.Current
                while (e.MoveNext()) do res <- e.Current
                res
            else
                invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString

        [<CompiledName("TryLast")>]
        let tryLast (source : seq<_>) =
            checkNonNull "source" source
            use e = source.GetEnumerator()
            if e.MoveNext() then
                let mutable res = e.Current
                while (e.MoveNext()) do res <- e.Current
                Some res
            else
                None

        [<CompiledName("ExactlyOne")>]
        let exactlyOne (source : seq<_>) =
            checkNonNull "source" source
            use e = source.GetEnumerator()
            if e.MoveNext() then
                let v = e.Current
                if e.MoveNext() then
                    invalidArg "source" (SR.GetString(SR.inputSequenceTooLong))
                else
                    v
            else
                invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString

        [<CompiledName("Reverse")>]
        let rev source =
            checkNonNull "source" source
            mkDelayedSeq (fun () ->
                let array = source |> toArray
                Array.Reverse array
                array :> seq<_>)

        [<CompiledName("Permute")>]
        let permute f (source : seq<_>) =
            checkNonNull "source" source
            mkDelayedSeq (fun () ->
                source |> toArray |> Array.permute f :> seq<_>)

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
            checkNonNull "source" source

            seq {
                use e = source.GetEnumerator()
                if e.MoveNext() then
                    let cached = HashSet(itemsToExclude, HashIdentity.Structural)
                    let next = e.Current
                    if (cached.Add next) then yield next
                    while e.MoveNext() do
                        let next = e.Current
                        if (cached.Add next) then yield next }

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
