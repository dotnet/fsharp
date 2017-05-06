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

    module Upcast =
        // The f# compiler outputs unnecessary unbox.any calls in upcasts. If this functionality
        // is fixed with the compiler then these functions can be removed.
        let inline enumerable (t:#IEnumerable<'T>) : IEnumerable<'T> = (# "" t : IEnumerable<'T> #)

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

    [<RequireQualifiedAccess>]
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module Seq =
#if FX_NO_ICLONEABLE
        open Microsoft.FSharp.Core.ICloneableExtensions
#else
#endif

        open Microsoft.FSharp.Collections.Internal
        open Microsoft.FSharp.Collections.IEnumerator

        // these helpers are just to consolidate the null checking
        let inline toISeq  (source:seq<'T>)  : ISeq.Core.ISeq<'T> = checkNonNull "source" source;   ISeq.ofSeq source
        let inline toISeq1 (source1:seq<'T>) : ISeq.Core.ISeq<'T> = checkNonNull "source1" source1; ISeq.ofSeq source1
        let inline toISeq2 (source2:seq<'T>) : ISeq.Core.ISeq<'T> = checkNonNull "source2" source2; ISeq.ofSeq source2
        let inline toISeq3 (source3:seq<'T>) : ISeq.Core.ISeq<'T> = checkNonNull "source3" source3; ISeq.ofSeq source3
        let inline toISeqs (sources:seq<'T>) : ISeq.Core.ISeq<'T> = checkNonNull "sources" sources; ISeq.ofSeq sources

        let mkDelayedSeq (f: unit -> IEnumerable<'T>) = mkSeq (fun () -> f().GetEnumerator())
        let mkUnfoldSeq f x = mkSeq (fun () -> IEnumerator.unfold f x)
        let inline indexNotFound() = raise (new System.Collections.Generic.KeyNotFoundException(SR.GetString(SR.keyNotFoundAlt)))

        [<CompiledName("Delay")>]
        let delay f = mkDelayedSeq f

        [<CompiledName("Unfold")>]
        let unfold f x =
            ISeq.unfold f x |> Upcast.enumerable

        [<CompiledName("Empty")>]
        let empty<'T> = (EmptyEnumerable :> seq<'T>)

        [<CompiledName("InitializeInfinite")>]
        let initInfinite f =
            ISeq.initInfinite f |> Upcast.enumerable

        [<CompiledName("Initialize")>]
        let init count f =
            ISeq.init count f |> Upcast.enumerable

        [<CompiledName("Iterate")>]
        let iter f (source : seq<'T>) =
            ISeq.iter f (toISeq source)

        [<CompiledName("Item")>]
        let item i (source : seq<'T>) =
            checkNonNull "source" source
            if i < 0 then invalidArgInputMustBeNonNegative "index" i
            use e = source.GetEnumerator()
            IEnumerator.nth i e

        [<CompiledName("TryItem")>]
        let tryItem i (source : seq<'T>) =
            ISeq.tryItem i (toISeq source)

        [<CompiledName("Get")>]
        let nth i (source : seq<'T>) = item i source

        [<CompiledName("IterateIndexed")>]
        let iteri f (source : seq<'T>) =
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt f
            ISeq.iteri (fun idx a -> f.Invoke (idx,a)) (toISeq source)

        [<CompiledName("Exists")>]
        let exists f (source : seq<'T>) =
            ISeq.exists f (toISeq source)

        [<CompiledName("Contains")>]
        let inline contains element (source : seq<'T>) =
            ISeq.contains element (toISeq source)

        [<CompiledName("ForAll")>]
        let forall f (source : seq<'T>) =
            ISeq.forall f (toISeq source)

        [<CompiledName("Iterate2")>]
        let iter2 f (source1 : seq<_>) (source2 : seq<_>)    =
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt f
            ISeq.iter2 (fun a b -> f.Invoke(a,b)) (source1 |> toISeq1) (source2 |> toISeq2)

        [<CompiledName("IterateIndexed2")>]
        let iteri2 f (source1 : seq<_>) (source2 : seq<_>) =
            let f = OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt f
            ISeq.iteri2 (fun idx a b -> f.Invoke(idx,a,b)) (source1 |> toISeq1) (source2 |> toISeq2)

        [<CompiledName("Filter")>]
        let filter f source      =
            ISeq.filter f (toISeq source) |> Upcast.enumerable

        [<CompiledName("Where")>]
        let where f source      = filter f source

        [<CompiledName("Map")>]
        let map    f source      =
            ISeq.map f (toISeq source) |> Upcast.enumerable

        [<CompiledName("MapIndexed")>]
        let mapi f source      =
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt f
            ISeq.mapi (fun idx a ->f.Invoke(idx,a)) (toISeq source) |> Upcast.enumerable

        [<CompiledName("MapIndexed2")>]
        let mapi2 f source1 source2 =
            let f = OptimizedClosures.FSharpFunc<int,'T,'U,'V>.Adapt f
            ISeq.mapi2 (fun idx a b -> f.Invoke (idx,a,b)) (source1 |> toISeq1) (source2 |> toISeq2) |> Upcast.enumerable

        [<CompiledName("Map2")>]
        let map2 f source1 source2 =
            ISeq.map2 f (source1 |> toISeq1) (source2 |> toISeq2) |> Upcast.enumerable

        [<CompiledName("Map3")>]
        let map3 f source1 source2 source3 =
            ISeq.map3 f (source1 |> toISeq1) (source2 |> toISeq2) (source3 |> toISeq3) |> Upcast.enumerable

        [<CompiledName("Choose")>]
        let choose f source      =
            ISeq.choose f (toISeq source) |> Upcast.enumerable

        [<CompiledName("Indexed")>]
        let indexed source =
            ISeq.indexed (toISeq source) |> Upcast.enumerable

        [<CompiledName("Zip")>]
        let zip source1 source2  =
            ISeq.zip (source1 |> toISeq1) (source2 |> toISeq2) |> Upcast.enumerable

        [<CompiledName("Zip3")>]
        let zip3 source1 source2  source3 =
            ISeq.zip3 (source1 |> toISeq1) (source2 |> toISeq2) (source3 |> toISeq3) |> Upcast.enumerable

        [<CompiledName("Cast")>]
        let cast (source: IEnumerable) =
            source |> ISeq.cast |> Upcast.enumerable

        [<CompiledName("TryPick")>]
        let tryPick f (source : seq<'T>)  =
            ISeq.tryPick f (toISeq source)

        [<CompiledName("Pick")>]
        let pick f source  =
            ISeq.pick f (toISeq source)

        [<CompiledName("TryFind")>]
        let tryFind f (source : seq<'T>)  =
            ISeq.tryFind f (toISeq source)

        [<CompiledName("Find")>]
        let find f source =
            ISeq.find f (toISeq source)

        [<CompiledName("Take")>]
        let take count (source : seq<'T>)    =
            ISeq.take count (toISeq source) |> Upcast.enumerable

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
            sources |> toISeqs |> ISeq.map toISeq |> ISeq.concat |> Upcast.enumerable

        [<CompiledName("Length")>]
        let length (source : seq<'T>)    =
            ISeq.length (toISeq source)

        [<CompiledName("Fold")>]
        let fold<'T,'State> f (x:'State) (source : seq<'T>)  =
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt f
            ISeq.fold (fun acc item -> f.Invoke (acc, item)) x (toISeq source)

        [<CompiledName("Fold2")>]
        let fold2<'T1,'T2,'State> f (state:'State) (source1: seq<'T1>) (source2: seq<'T2>) =
            let f = OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt f
            ISeq.fold2 (fun acc item1 item2 -> f.Invoke (acc, item1, item2)) state (source1 |> toISeq1) (source2 |> toISeq2)

        [<CompiledName("Reduce")>]
        let reduce f (source : seq<'T>)  =
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt f
            ISeq.reduce (fun acc item -> f.Invoke (acc, item)) (toISeq source)

        let fromGenerator f = mkSeq(fun () -> Generator.EnumerateFromGenerator (f()))
        let toGenerator (ie : seq<_>) = Generator.GenerateFromEnumerator (ie.GetEnumerator())

        [<CompiledName("Replicate")>]
        let replicate count x =
            ISeq.replicate count x |> Upcast.enumerable

        [<CompiledName("Append")>]
        let append (source1: seq<'T>) (source2: seq<'T>) =
            ISeq.append (source1 |> toISeq1) (source2 |> toISeq2) |> Upcast.enumerable

        [<CompiledName("Collect")>]
        let collect f sources = map f sources |> concat

        [<CompiledName("CompareWith")>]
        let compareWith (f:'T -> 'T -> int) (source1 : seq<'T>) (source2: seq<'T>) =
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt f
            ISeq.compareWith (fun a b -> f.Invoke(a,b)) (source1 |> toISeq1) (source2 |> toISeq2)

        [<CompiledName("OfList")>]
        let ofList (source : 'T list) =
            ISeq.ofList source |> Upcast.enumerable

        [<CompiledName("ToList")>]
        let toList (source : seq<'T>) =
            ISeq.toList (toISeq source)

        // Create a new object to ensure underlying array may not be mutated by a backdoor cast
        [<CompiledName("OfArray")>]
        let ofArray (source : 'T array) =
            ISeq.ofArray source |> Upcast.enumerable

        [<CompiledName("ToArray")>]
        let toArray (source : seq<'T>)  =
            ISeq.toArray (toISeq source)

        [<CompiledName("FoldBack")>]
        let foldBack<'T,'State> f (source : seq<'T>) (x:'State) =
            ISeq.foldBack f (toISeq source) x

        [<CompiledName("FoldBack2")>]
        let foldBack2<'T1,'T2,'State> f (source1 : seq<'T1>) (source2 : seq<'T2>) (x:'State) =
            ISeq.foldBack2 f (toISeq1 source1) (toISeq2 source2) x

        [<CompiledName("ReduceBack")>]
        let reduceBack f (source : seq<'T>) =
            ISeq.reduceBack f (toISeq source)

        [<CompiledName("Singleton")>]
        let singleton x =
            ISeq.singleton x |> Upcast.enumerable

        [<CompiledName("Truncate")>]
        let truncate n (source: seq<'T>) =
            ISeq.truncate n (toISeq source) |> Upcast.enumerable

        [<CompiledName("Pairwise")>]
        let pairwise (source: seq<'T>) =
            ISeq.pairwise (toISeq source) |> Upcast.enumerable

        [<CompiledName("Scan")>]
        let scan<'T,'State> f (z:'State) (source : seq<'T>) =
            ISeq.scan f z (toISeq source) |> Upcast.enumerable

        [<CompiledName("TryFindBack")>]
        let tryFindBack f (source : seq<'T>) =
            ISeq.tryFindBack f (toISeq source)

        [<CompiledName("FindBack")>]
        let findBack f source =
            ISeq.findBack f (toISeq source)

        [<CompiledName("ScanBack")>]
        let scanBack<'T,'State> f (source : seq<'T>) (acc:'State) =
            ISeq.scanBack f (toISeq source) acc |> Upcast.enumerable

        [<CompiledName("FindIndex")>]
        let findIndex p (source:seq<_>) =
            ISeq.findIndex p (toISeq source)

        [<CompiledName("TryFindIndex")>]
        let tryFindIndex p (source:seq<_>) =
            ISeq.tryFindIndex p (toISeq source)

        [<CompiledName("TryFindIndexBack")>]
        let tryFindIndexBack f (source : seq<'T>) =
            ISeq.tryFindIndexBack f (toISeq source)

        [<CompiledName("FindIndexBack")>]
        let findIndexBack f source =
            ISeq.findIndexBack f (toISeq source)

        // windowed : int -> seq<'T> -> seq<'T[]>
        [<CompiledName("Windowed")>]
        let windowed windowSize (source: seq<_>) =
            ISeq.windowed windowSize (toISeq source) |> Upcast.enumerable

        [<CompiledName("Cache")>]
        let cache (source : seq<'T>) =
            ISeq.cache (toISeq source) |> Upcast.enumerable

        [<CompiledName("AllPairs")>]
        let allPairs source1 source2 =
            ISeq.allPairs (source1 |> toISeq1) (source2 |> toISeq2) |> Upcast.enumerable

        [<CodeAnalysis.SuppressMessage("Microsoft.Naming","CA1709:IdentifiersShouldBeCasedCorrectly"); CodeAnalysis.SuppressMessage("Microsoft.Naming","CA1707:IdentifiersShouldNotContainUnderscores"); CodeAnalysis.SuppressMessage("Microsoft.Naming","CA1704:IdentifiersShouldBeSpelledCorrectly")>]
        [<CompiledName("ReadOnly")>]
        let readonly (source:seq<_>) =
            checkNonNull "source" source
            mkSeq (fun () -> source.GetEnumerator())

        [<CompiledName("GroupBy")>]
        let groupBy (keyf:'T->'Key) (seq:seq<'T>) =
            let grouped = 
#if FX_RESHAPED_REFLECTION
                if (typeof<'Key>).GetTypeInfo().IsValueType
#else
                if typeof<'Key>.IsValueType
#endif
                    then seq |> toISeq |> ISeq.GroupBy.byVal keyf
                    else seq |> toISeq |> ISeq.GroupBy.byRef keyf

            grouped
            |> ISeq.map (fun (key,value) -> key, Upcast.enumerable value)
            |> Upcast.enumerable

        [<CompiledName("Distinct")>]
        let distinct source =
            ISeq.distinct (toISeq source) |> Upcast.enumerable

        [<CompiledName("DistinctBy")>]
        let distinctBy keyf source =
            ISeq.distinctBy keyf (toISeq source) |> Upcast.enumerable

        [<CompiledName("SortBy")>]
        let sortBy keyf source =
            ISeq.sortBy keyf (toISeq source) |> Upcast.enumerable

        [<CompiledName("Sort")>]
        let sort source =
            ISeq.sort (toISeq source) |> Upcast.enumerable

        [<CompiledName("SortWith")>]
        let sortWith f source =
            ISeq.sortWith f (toISeq source) |> Upcast.enumerable

        [<CompiledName("SortByDescending")>]
        let inline sortByDescending keyf source =
            ISeq.sortByDescending keyf (toISeq source) |> Upcast.enumerable

        [<CompiledName("SortDescending")>]
        let inline sortDescending source =
            ISeq.sortDescending (toISeq source) |> Upcast.enumerable

        [<CompiledName("CountBy")>]
        let countBy (keyf:'T->'Key) (source:seq<'T>) =
#if FX_RESHAPED_REFLECTION
            if (typeof<'Key>).GetTypeInfo().IsValueType
#else
            if typeof<'Key>.IsValueType
#endif
                then ISeq.CountBy.byVal keyf (toISeq source) |> Upcast.enumerable
                else ISeq.CountBy.byRef keyf (toISeq source) |> Upcast.enumerable

        [<CompiledName("Sum")>]
        let inline sum (source: seq< ^a>) : ^a =
            ISeq.sum (toISeq source)

        [<CompiledName("SumBy")>]
        let inline sumBy (f : 'T -> ^U) (source: seq<'T>) : ^U =
            ISeq.sumBy f (toISeq source)

        [<CompiledName("Average")>]
        let inline average (source: seq< ^a>) : ^a =
            ISeq.average (toISeq source)

        [<CompiledName("AverageBy")>]
        let inline averageBy (f : 'T -> ^U) (source: seq< 'T >) : ^U =
            ISeq.averageBy f (toISeq source)

        [<CompiledName("Min")>]
        let inline min (source: seq<_>) =
            ISeq.min (toISeq source)

        [<CompiledName("MinBy")>]
        let inline minBy (f : 'T -> 'U) (source: seq<'T>) : 'T =
            ISeq.minBy f (toISeq source)

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
            ISeq.max (toISeq source)

        [<CompiledName("MaxBy")>]
        let inline maxBy (f : 'T -> 'U) (source: seq<'T>) : 'T =
            ISeq.maxBy f (toISeq source)


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
            ISeq.takeWhile p (toISeq source) |> Upcast.enumerable

        [<CompiledName("Skip")>]
        let skip count (source: seq<_>) =
            ISeq.skip count (toISeq source) |> Upcast.enumerable

        [<CompiledName("SkipWhile")>]
        let skipWhile p (source: seq<_>) =
            ISeq.skipWhile p (toISeq source) |> Upcast.enumerable

        [<CompiledName("ForAll2")>]
        let forall2 p (source1: seq<_>) (source2: seq<_>) =
            let p = OptimizedClosures.FSharpFunc<_,_,_>.Adapt p
            ISeq.forall2 (fun a b -> p.Invoke(a,b)) (source1 |> toISeq1) (source2 |> toISeq2)

        [<CompiledName("Exists2")>]
        let exists2 p (source1: seq<_>) (source2: seq<_>) =
            let p = OptimizedClosures.FSharpFunc<_,_,_>.Adapt p
            ISeq.exists2 (fun a b -> p.Invoke(a,b)) (source1 |> toISeq1) (source2 |> toISeq2)

        [<CompiledName("Head")>]
        let head (source : seq<_>) =
            ISeq.head (toISeq source)

        [<CompiledName("TryHead")>]
        let tryHead (source : seq<_>) =
            ISeq.tryHead (toISeq source)

        [<CompiledName("Tail")>]
        let tail (source: seq<'T>) =
            ISeq.tail (toISeq source) |> Upcast.enumerable

        [<CompiledName("Last")>]
        let last (source : seq<_>) =
            ISeq.last (toISeq source)

        [<CompiledName("TryLast")>]
        let tryLast (source : seq<_>) =
            ISeq.tryLast (toISeq source)

        [<CompiledName("ExactlyOne")>]
        let exactlyOne (source : seq<_>) =
            ISeq.exactlyOne (toISeq source)

        [<CompiledName("Reverse")>]
        let rev source =
            ISeq.rev (toISeq source) |> Upcast.enumerable

        [<CompiledName("Permute")>]
        let permute f (source : seq<_>) =
            ISeq.permute f (toISeq source) |> Upcast.enumerable

        [<CompiledName("MapFold")>]
        let mapFold<'T,'State,'Result> (f: 'State -> 'T -> 'Result * 'State) acc source =
            ISeq.mapFold f acc (toISeq source) |> fun (iseq, state) -> Upcast.enumerable iseq, state

        [<CompiledName("MapFoldBack")>]
        let mapFoldBack<'T,'State,'Result> (f: 'T -> 'State -> 'Result * 'State) source acc =
            ISeq.mapFoldBack f (toISeq source) acc |> fun (iseq, state) -> Upcast.enumerable iseq, state

        [<CompiledName("Except")>]
        let except (itemsToExclude: seq<'T>) (source: seq<'T>) =
            ISeq.except itemsToExclude (toISeq source) |> Upcast.enumerable

        [<CompiledName("ChunkBySize")>]
        let chunkBySize chunkSize (source : seq<_>) =
            ISeq.chunkBySize chunkSize (toISeq source) |> Upcast.enumerable

        [<CompiledName("SplitInto")>]
        let splitInto count source =
            ISeq.splitInto count (toISeq source) |> Upcast.enumerable
