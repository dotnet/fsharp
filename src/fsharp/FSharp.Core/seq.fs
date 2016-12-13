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
    open Microsoft.FSharp.Collections.Composer
    open Microsoft.FSharp.Collections.Composer.Core
    open Microsoft.FSharp.Collections.Composer.Seq
    open Microsoft.FSharp.Primitives.Basics
    open Microsoft.FSharp.Collections.IEnumerator

    module Upcast =
        // The f# compiler outputs unnecessary unbox.any calls in upcasts. If this functionality
        // is fixed with the compiler then these functions can be removed.
        let inline enumerable (t:#IEnumerable<'T>) : IEnumerable<'T> = (# "" t : IEnumerable<'T> #)

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
        let mkDelayedSeq (f: unit -> IEnumerable<'T>) = mkSeq (fun () -> f().GetEnumerator())
        let inline indexNotFound() = raise (new System.Collections.Generic.KeyNotFoundException(SR.GetString(SR.keyNotFoundAlt)))

        [<CompiledName("ToComposer")>]
        let toComposer (source:seq<'T>): Composer.Core.ISeq<'T> =
            Composer.Seq.toComposer source

        let inline foreach f (source:seq<_>) =
            Composer.Seq.foreach f (toComposer source)

        let private seqFactory (createSeqComponent:#SeqFactory<_,_>) (source:seq<'T>) =
            match source with
            | :? Composer.Core.ISeq<'T> as s -> Upcast.enumerable (s.Compose createSeqComponent)
            | :? array<'T> as a -> Upcast.enumerable (Composer.Seq.Array.create a createSeqComponent)
            | :? list<'T> as a -> Upcast.enumerable (Composer.Seq.List.create a createSeqComponent)
            | null -> nullArg "source"
            | _ -> Upcast.enumerable (Composer.Seq.Enumerable.create source createSeqComponent)

        [<CompiledName("Delay")>]
        let delay f = mkDelayedSeq f

        [<CompiledName("Unfold")>]
        let unfold (generator:'State->option<'T * 'State>) (state:'State) : seq<'T> =
            Composer.Seq.unfold generator state
            |> Upcast.enumerable

        [<CompiledName("Empty")>]
        let empty<'T> = (EmptyEnumerable :> seq<'T>)

        [<CompiledName("InitializeInfinite")>]
        let initInfinite<'T> (f:int->'T) : IEnumerable<'T> =
            Composer.Seq.initInfinite f
            |> Upcast.enumerable

        [<CompiledName("Initialize")>]
        let init<'T> (count:int) (f:int->'T) : IEnumerable<'T> =
            Composer.Seq.init count f
            |> Upcast.enumerable

        [<CompiledName("Iterate")>]
        let iter f (source : seq<'T>) =
            source |> toComposer |> Composer.Seq.iter f

        [<CompiledName("TryHead")>]
        let tryHead (source : seq<_>) =
            source |> toComposer |> Composer.Seq.tryHead

        [<CompiledName("Skip")>]
        let skip count (source: seq<_>) =
            source |> toComposer |> Composer.Seq.skip count |> Upcast.enumerable

        let invalidArgumnetIndex = invalidArgFmt "index"

        [<CompiledName("Item")>]
        let item i (source : seq<'T>) =
            if i < 0 then invalidArgInputMustBeNonNegative "index" i else
                source
                |> toComposer |> Composer.Seq.skip i |> Upcast.enumerable
                |> tryHead
                |> function
                    | None -> invalidArgFmt "index" "{0}\nseq was short by 1 element"  [|SR.GetString SR.notEnoughElements|]
                    | Some value -> value

        [<CompiledName("TryItem")>]
        let tryItem i (source:seq<'T>) =
            source |> toComposer |> Composer.Seq.tryItem i

        [<CompiledName("Get")>]
        let nth i (source : seq<'T>) = item i source

        [<CompiledName("IterateIndexed")>]
        let iteri f (source:seq<'T>) =
            source |> toComposer |> Composer.Seq.iteri f

        [<CompiledName("Exists")>]
        let exists f (source:seq<'T>) =
            source |> toComposer |> Composer.Seq.exists f

        [<CompiledName("Contains")>]
        let inline contains element (source:seq<'T>) =
            source |> toComposer |> Composer.Seq.contains element

        [<CompiledName("ForAll")>]
        let forall f (source:seq<'T>) =
            source |> toComposer |> Composer.Seq.forall f

        [<CompiledName("Iterate2")>]
        let iter2 f (source1 : seq<_>) (source2 : seq<_>)    =
            checkNonNull "source2" source2

            use e2 = source2.GetEnumerator()
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)

            source1
            |> foreach (fun halt ->
                { new Composer.Core.Folder<_,_> () with
                    override this.ProcessNext value =
                        if (e2.MoveNext()) then
                            f.Invoke(value, e2.Current)
                        else
                            halt()
                        Unchecked.defaultof<_> (* return value unsed in ForEach context *) })
            |> ignore

        [<CompiledName("IterateIndexed2")>]
        let iteri2 f (source1 : seq<_>) (source2 : seq<_>) =
            checkNonNull "source2" source2

            use e2 = source2.GetEnumerator()
            let f = OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt(f)

            source1
            |> foreach (fun halt ->
                { new Composer.Core.Folder<_,int> (0) with
                    override this.ProcessNext value =
                        if (e2.MoveNext()) then
                            f.Invoke(this.Value, value, e2.Current)
                            this.Value <- this.Value + 1
                        else
                            halt()
                        Unchecked.defaultof<_> (* return value unsed in ForEach context *) })
            |> ignore

        // Build an IEnumerble by wrapping/transforming iterators as they get generated.
        let revamp f (ie : seq<_>) = mkSeq (fun () -> f (ie.GetEnumerator()))
        let revamp2 f (ie1 : seq<_>) (source2 : seq<_>) =
            mkSeq (fun () -> f (ie1.GetEnumerator()) (source2.GetEnumerator()))
        let revamp3 f (ie1 : seq<_>) (source2 : seq<_>) (source3 : seq<_>) =
            mkSeq (fun () -> f (ie1.GetEnumerator()) (source2.GetEnumerator()) (source3.GetEnumerator()))

        [<CompiledName("Filter")>]
        let filter<'T> (f:'T->bool) (source:seq<'T>) : seq<'T> =
            source |> toComposer |> Composer.Seq.filter f |> Upcast.enumerable

        [<CompiledName("Where")>]
        let where f source = filter f source

        [<CompiledName("Map")>]
        let map<'T,'U> (f:'T->'U) (source:seq<'T>) : seq<'U> =
            source |> toComposer |> Composer.Seq.map f |> Upcast.enumerable

        [<CompiledName("MapIndexed")>]
        let mapi f source =
            let f' = OptimizedClosures.FSharpFunc<_,_,_>.Adapt f
            source |> toComposer |> Composer.Seq.mapi_adapt f' |> Upcast.enumerable

        [<CompiledName("MapIndexed2")>]
        let mapi2 f source1 source2 =
            checkNonNull "source2" source2
            source1 |> seqFactory (Composer.Seq.Mapi2Factory (f, source2))

        [<CompiledName("Map2")>]
        let map2<'T,'U,'V> (f:'T->'U->'V) (source1:seq<'T>) (source2:seq<'U>) : seq<'V> =
            checkNonNull "source1" source1
            match source1 with
            | :? Composer.Core.ISeq<'T> as s -> Upcast.enumerable (s.Compose (Composer.Seq.Map2FirstFactory (f, source2)))
            | _ -> source2 |> seqFactory (Composer.Seq.Map2SecondFactory (f, source1))

        [<CompiledName("Map3")>]
        let map3 f source1 source2 source3 =
            checkNonNull "source2" source2
            checkNonNull "source3" source3
            source1 |> seqFactory (Composer.Seq.Map3Factory (f, source2, source3))

        [<CompiledName("Choose")>]
        let choose f source =
            source |> toComposer |> Composer.Seq.choose f |> Upcast.enumerable

        [<CompiledName("Indexed")>]
        let indexed source =
            source |> toComposer |> Composer.Seq.indexed |> Upcast.enumerable

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
            source |> toComposer |> Composer.Seq.tryPick f

        [<CompiledName("Pick")>]
        let pick f source  =
            match tryPick f source with
            | None -> indexNotFound()
            | Some x -> x

        [<CompiledName("TryFind")>]
        let tryFind f (source : seq<'T>)  =
            source |> toComposer |> Composer.Seq.tryFind f

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
            source |> toComposer |> Composer.Seq.take count |> Upcast.enumerable

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
            upcast Composer.Seq.Enumerable.ConcatEnumerable sources

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

        [<CompiledName "Fold">]
        let fold<'T,'State> (f:'State->'T->'State)  (x:'State) (source:seq<'T>) =
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)
            source |> toComposer
            |> Composer.Seq.fold<'T,'State>(fun (a:'State) (b:'T) -> f.Invoke(a,b)) x


        [<CompiledName("Fold2")>]
        let fold2<'T1,'T2,'State> f (state:'State) (source1: seq<'T1>) (source2: seq<'T2>) =
            checkNonNull "source2" source2

            use e2 = source2.GetEnumerator()
            let f = OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt(f)

            source1
            |> foreach (fun halt ->
                { new Composer.Core.Folder<_,'State> (state) with
                    override this.ProcessNext value =
                        if (e2.MoveNext()) then
                            this.Value <- f.Invoke(this.Value, value, e2.Current)
                        else
                            halt()
                        Unchecked.defaultof<_> (* return value unsed in ForEach context *) })
            |> fun fold -> fold.Value

        [<CompiledName("Reduce")>]
        let reduce f (source : seq<'T>)  =
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)

            source
            |> foreach (fun _ ->
                { new Composer.Core.FolderWithOnComplete<'T, Composer.Core.Values<bool,'T>> (Composer.Core.Values<_,_>(true, Unchecked.defaultof<'T>)) with
                    override this.ProcessNext value =
                        if this.Value._1 then
                            this.Value._1 <- false
                            this.Value._2 <- value
                        else
                            this.Value._2 <- f.Invoke (this.Value._2, value)
                        Unchecked.defaultof<_> (* return value unsed in ForEach context *)

                    member this.OnComplete _ =
                        if this.Value._1 then
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
            | :? Composer.Seq.Enumerable.EnumerableBase<'T> as s -> s.Append source2
            | _ -> Upcast.enumerable (new Composer.Seq.Enumerable.AppendEnumerable<_>([source2; source1]))


        [<CompiledName("Collect")>]
        let collect f sources = map f sources |> concat

        [<CompiledName("CompareWith")>]
        let compareWith (f:'T -> 'T -> int) (source1 : seq<'T>) (source2: seq<'T>) =
            checkNonNull "source2" source2

            use e2 = source2.GetEnumerator()
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)

            source1
            |> foreach (fun halt ->
                { new Composer.Core.FolderWithOnComplete<'T,int> (0) with
                    override this.ProcessNext value =
                        if not (e2.MoveNext()) then
                            this.Value <- 1
                            halt ()
                        else
                            let c = f.Invoke (value, e2.Current)
                            if c <> 0 then
                                this.Value <- c
                                halt ()
                        Unchecked.defaultof<_> (* return value unsed in ForEach context *)
                    member this.OnComplete _ =
                        if this.Value = 0 && e2.MoveNext() then
                            this.Value <- -1 })
            |> fun compare -> compare.Value

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
            Upcast.enumerable (Composer.Seq.Array.createId source)

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


        [<CompiledName "Truncate">]
        let truncate n (source: seq<'T>) =
            if n <= 0 then empty else
            source |> toComposer |> Composer.Seq.truncate n |> Upcast.enumerable

        [<CompiledName "Pairwise">]
        let pairwise<'T> (source:seq<'T>) : seq<'T*'T> =
            source |> toComposer |> Composer.Seq.pairwise  |> Upcast.enumerable

        [<CompiledName "Scan">]
        let scan<'T,'State> (folder:'State->'T->'State) (state:'State) (source:seq<'T>) : seq<'State> =
            source |> toComposer |> Composer.Seq.scan folder state |> Upcast.enumerable

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
            |> foreach (fun halt ->
                { new Composer.Core.Folder<'T, Composer.Core.Values<Option<int>, int>> (Composer.Core.Values<_,_>(None, 0)) with
                    override this.ProcessNext value =
                        if p value then
                            this.Value._1 <- Some(this.Value._2)
                            halt ()
                        else
                            this.Value._2 <- this.Value._2 + 1
                        Unchecked.defaultof<_> (* return value unsed in ForEach context *) })
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
            if windowSize <= 0 then invalidArgFmt "windowSize" "{0}\nwindowSize = {1}"
                                        [|SR.GetString SR.inputMustBePositive; windowSize|]
            source |> toComposer |> Composer.Seq.windowed windowSize |> Upcast.enumerable

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
            source |> toComposer |> Composer.Seq.distinct |> Upcast.enumerable

        [<CompiledName("DistinctBy")>]
        let distinctBy keyf source =
            source |> toComposer |> Composer.Seq.distinctBy keyf |> Upcast.enumerable

        [<CompiledName("SortBy")>]
        let sortBy keyf source =
            checkNonNull "source" source
            let delayedSort () =
                let array = source |> toArray
                Array.stableSortInPlaceBy keyf array
                array
            Upcast.enumerable (Composer.Seq.Array.createDelayedId delayedSort)

        [<CompiledName("Sort")>]
        let sort source =
            checkNonNull "source" source
            let delayedSort () =
                let array = source |> toArray
                Array.stableSortInPlace array
                array
            Upcast.enumerable (Composer.Seq.Array.createDelayedId delayedSort)

        [<CompiledName("SortWith")>]
        let sortWith f source =
            checkNonNull "source" source
            let delayedSort () =
                let array = source |> toArray
                Array.stableSortInPlaceWith f array
                array
            Upcast.enumerable (Composer.Seq.Array.createDelayedId delayedSort)

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

        [<CompiledName "Sum">]
        let inline sum (source:seq<'a>) : 'a =
            source |> toComposer |> Composer.Seq.sum

        [<CompiledName "SumBy">]
        let inline sumBy (f : 'T -> ^U) (source: seq<'T>) : ^U =
            source |> toComposer |> Composer.Seq.sumBy f

        [<CompiledName "Average">]
        let inline average (source: seq< ^a>) : ^a =
            source |> toComposer |> Composer.Seq.average

        [<CompiledName "AverageBy">]
        let inline averageBy (f : 'T -> ^U) (source: seq< 'T >) : ^U =
            source |> toComposer |> Composer.Seq.averageBy f

        [<CompiledName "Min">]
        let inline min (source: seq<'T>): 'T when 'T : comparison =
            source |> toComposer |> Composer.Seq.min

        [<CompiledName "MinBy">]
        let inline minBy (projection: 'T -> 'U when 'U:comparison) (source: seq<'T>) : 'T =
            source |> toComposer |> Composer.Seq.minBy projection
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
                { new Composer.Core.FolderWithOnComplete<'T,Composer.Core.Values<bool,'T>> (Composer.Core.Values<_,_>(true, Unchecked.defaultof<'T>)) with
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

        [<CompiledName("MaxBy")>]
        let inline maxBy (f : 'T -> 'U) (source: seq<'T>) : 'T =
            source
            |> foreach (fun _ ->
                { new Composer.Core.FolderWithOnComplete<'T,Composer.Core.Values<bool,'U,'T>> (Composer.Core.Values<_,_,_>(true,Unchecked.defaultof<'U>,Unchecked.defaultof<'T>)) with
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
        [<CompiledName "TakeWhile">]
        let takeWhile predicate (source: seq<_>) =
            source |> toComposer |> Composer.Seq.takeWhile predicate |> Upcast.enumerable

        [<CompiledName "SkipWhile">]
        let skipWhile predicate (source: seq<_>) =
            source |> toComposer |> Composer.Seq.skipWhile predicate |> Upcast.enumerable

        [<CompiledName("ForAll2")>]
        let forall2 p (source1: seq<_>) (source2: seq<_>) =
            checkNonNull "source2" source2

            use e2 = source2.GetEnumerator()
            let p = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(p)

            source1
            |> foreach (fun halt ->
                { new Composer.Core.Folder<_,bool> (true) with
                    override this.ProcessNext value =
                        if (e2.MoveNext()) then
                            if not (p.Invoke(value, e2.Current)) then
                                this.Value <- false
                                halt()
                        else
                            halt()
                        Unchecked.defaultof<_> (* return value unsed in ForEach context *) })
            |> fun all -> all.Value

        [<CompiledName("Exists2")>]
        let exists2 p (source1: seq<_>) (source2: seq<_>) =
            checkNonNull "source2" source2

            use e2 = source2.GetEnumerator()
            let p = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(p)

            source1
            |> foreach (fun halt ->
                { new Composer.Core.Folder<_,bool> (false) with
                    override this.ProcessNext value =
                        if (e2.MoveNext()) then
                            if p.Invoke(value, e2.Current) then
                                this.Value <- true
                                halt()
                        else
                            halt()
                        Unchecked.defaultof<_> (* return value unsed in ForEach context *) })
            |> fun exists -> exists.Value

        [<CompiledName("Head")>]
        let head (source : seq<_>) =
            match tryHead source with
            | None -> invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
            | Some x -> x

        [<CompiledName "Tail">]
        let tail (source: seq<'T>) =
            source |> toComposer |> Composer.Seq.tail |> Upcast.enumerable

        [<CompiledName("TryLast")>]
        let tryLast (source : seq<_>) =
            source
            |> foreach (fun _ ->
                { new Composer.Core.Folder<'T, Composer.Core.Values<bool,'T>> (Composer.Core.Values<bool,'T>(true, Unchecked.defaultof<'T>)) with
                    override this.ProcessNext value =
                        if this.Value._1 then
                            this.Value._1 <- false
                        this.Value._2 <- value
                        Unchecked.defaultof<_> (* return value unsed in ForEach context *) })
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
            |> foreach (fun halt ->
                { new Composer.Core.FolderWithOnComplete<'T, Composer.Core.Values<bool,'T, bool>> (Composer.Core.Values<bool,'T, bool>(true, Unchecked.defaultof<'T>, false)) with
                    override this.ProcessNext value =
                        if this.Value._1 then
                            this.Value._1 <- false
                            this.Value._2 <- value
                        else
                            this.Value._3 <- true
                            halt ()
                        Unchecked.defaultof<_> (* return value unsed in ForEach context *)

                      member this.OnComplete _ =
                        if this.Value._1 then
                            invalidArg "source" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
                        elif this.Value._3 then
                            invalidArg "source" (SR.GetString(SR.inputSequenceTooLong)) })
            |> fun one -> one.Value._2

        [<CompiledName("Reverse")>]
        let rev source =
            checkNonNull "source" source
            let delayedReverse () =
                let array = source |> toArray
                Array.Reverse array
                array
            Upcast.enumerable (Composer.Seq.Array.createDelayedId delayedReverse)

        [<CompiledName("Permute")>]
        let permute f (source:seq<_>) =
            checkNonNull "source" source
            let delayedPermute () =
                source
                |> toArray
                |> Array.permute f
            Upcast.enumerable (Composer.Seq.Array.createDelayedId delayedPermute)

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

        [<CompiledName "Except">]
        let except (itemsToExclude: seq<'T>) (source: seq<'T>) =
            checkNonNull "itemsToExclude" itemsToExclude
            if isEmpty itemsToExclude then source else
            source |> toComposer |> Composer.Seq.except itemsToExclude |> Upcast.enumerable

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
