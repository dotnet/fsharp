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

      let rec nth index (e : IEnumerator<'T>) =
          if not (e.MoveNext()) then
            let shortBy = index + 1
            invalidArgFmt "index"
                "{0}\nseq was short by {1} {2}"
                [|SR.GetString SR.notEnoughElements; shortBy; (if shortBy = 1 then "element" else "elements")|]
          if index = 0 then e.Current
          else nth (index-1) e

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
    open Microsoft.FSharp.Collections.SeqComposition

    [<RequireQualifiedAccess>]
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module Seq =
        open Microsoft.FSharp.Collections.Internal
        open Microsoft.FSharp.Collections.IEnumerator

        // these helpers are just to consolidate the null checking
        let inline toISeq  (source:seq<'T>)  : ISeq<'T> = checkNonNull "source" source;   ISeq.ofSeq source
        let inline toISeq1 (source1:seq<'T>) : ISeq<'T> = checkNonNull "source1" source1; ISeq.ofSeq source1
        let inline toISeq2 (source2:seq<'T>) : ISeq<'T> = checkNonNull "source2" source2; ISeq.ofSeq source2
        let inline toISeq3 (source3:seq<'T>) : ISeq<'T> = checkNonNull "source3" source3; ISeq.ofSeq source3
        let inline toISeqs (sources:seq<'T>) : ISeq<'T> = checkNonNull "sources" sources; ISeq.ofSeq sources

        let mkDelayedSeq (f: unit -> IEnumerable<'T>) = mkSeq (fun () -> f().GetEnumerator())

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
            delay (fun () ->
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
                |> Upcast.enumerable)

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

        [<CompiledName("Max")>]
        let inline max (source: seq<_>) =
            ISeq.max (toISeq source)

        [<CompiledName("MaxBy")>]
        let inline maxBy (f : 'T -> 'U) (source: seq<'T>) : 'T =
            ISeq.maxBy f (toISeq source)

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