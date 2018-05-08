// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

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
    open Microsoft.FSharp.Collections.SeqComposition

    [<RequireQualifiedAccess>]
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module Seq =

        open Microsoft.FSharp.Collections.Internal
        open Microsoft.FSharp.Collections.IEnumerator

        // these helpers are just to consolidate the null checking
        let inline toIConsumableSeq  (source:seq<'T>)  : IConsumableSeq<'T> = checkNonNull "source" source;   IConsumableSeq.ofSeq source
        let inline toIConsumableSeq1 (source1:seq<'T>) : IConsumableSeq<'T> = checkNonNull "source1" source1; IConsumableSeq.ofSeq source1
        let inline toIConsumableSeq2 (source2:seq<'T>) : IConsumableSeq<'T> = checkNonNull "source2" source2; IConsumableSeq.ofSeq source2
        let inline toIConsumableSeq3 (source3:seq<'T>) : IConsumableSeq<'T> = checkNonNull "source3" source3; IConsumableSeq.ofSeq source3
        let inline toIConsumableSeqs (sources:seq<'T>) : IConsumableSeq<'T> = checkNonNull "sources" sources; IConsumableSeq.ofSeq sources

        let getRaw (source:IConsumableSeq<_>) =
            match source with
            | :? Core.EnumerableBase<'T> as s -> s.GetRaw ()
            | _ -> upcast source

        let rawOrOriginal (raw:seq<_>) (original:IConsumableSeq<_>) =
            if obj.ReferenceEquals (raw, original) then original else toIConsumableSeq raw

        let mkDelayedSeq (f: unit -> IEnumerable<'T>) = mkSeq (fun () -> f().GetEnumerator())

        [<CompiledName("Delay")>]
        let delay generator = mkDelayedSeq generator

        [<CompiledName("Unfold")>]
        let unfold generator state =
            IConsumableSeq.unfold generator state :> seq<_>

        [<CompiledName("Empty")>]
        let empty<'T> = (EmptyEnumerable :> seq<'T>)

        [<CompiledName("InitializeInfinite")>]
        let initInfinite initializer =
            IConsumableSeq.initInfinite initializer :> seq<_>

        [<CompiledName("Initialize")>]
        let init count initializer =
            IConsumableSeq.init count initializer :> seq<_>

        [<CompiledName("Iterate")>]
        let iter action (source:seq<'T>) =
            let original = toIConsumableSeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.iter action arr
            | :? list<'T> as lst -> List.iter action lst
            | raw -> IConsumableSeq.iter action (rawOrOriginal raw original)

        [<CompiledName("Item")>]
        let item index (source : seq<'T>) =
            checkNonNull "source" source
            if index < 0 then invalidArgInputMustBeNonNegative "index" index
            use e = source.GetEnumerator()
            IEnumerator.nth index e

        [<CompiledName("TryItem")>]
        let tryItem index (source : seq<'T>) =
            let original = toIConsumableSeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.tryItem index arr
            | :? list<'T> as lst -> List.tryItem index lst
            | raw -> IConsumableSeq.tryItem index (rawOrOriginal raw original)

        [<CompiledName("Get")>]
        let nth index (source : seq<'T>) = item index source

        [<CompiledName("IterateIndexed")>]
        let iteri action (source:seq<'T>) =
            let original = toIConsumableSeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.iteri action arr
            | :? list<'T> as lst -> List.iteri action lst
            | raw -> 
                let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt action
                IConsumableSeq.iteri (fun idx a -> f.Invoke (idx,a)) (rawOrOriginal raw original)

        [<CompiledName("Exists")>]
        let exists predicate (source:seq<'T>) =
            let original = toIConsumableSeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.exists predicate arr
            | :? list<'T> as lst -> List.exists predicate lst
            | raw -> IConsumableSeq.exists predicate (rawOrOriginal raw original)

        [<CompiledName("Contains")>]
        let inline contains value (source:seq<'T>) =
            IConsumableSeq.contains value (toIConsumableSeq source)

        [<CompiledName("ForAll")>]
        let forall predicate (source:seq<'T>) =
            let original = toIConsumableSeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.forall predicate arr
            | :? list<'T> as lst -> List.forall predicate lst
            | raw -> IConsumableSeq.forall predicate (rawOrOriginal raw original)

        [<CompiledName("Iterate2")>]
        let iter2 action (source1:seq<_>) (source2:seq<_>)    =
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt action
            IConsumableSeq.iter2 (fun a b -> f.Invoke(a,b)) (source1 |> toIConsumableSeq1) (source2 |> toIConsumableSeq2)

        [<CompiledName("IterateIndexed2")>]
        let iteri2 action (source1 : seq<_>) (source2 : seq<_>) =
            let f = OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt action
            IConsumableSeq.iteri2 (fun idx a b -> f.Invoke(idx,a,b)) (source1 |> toIConsumableSeq1) (source2 |> toIConsumableSeq2)

        [<CompiledName("Filter")>]
        let filter predicate source =
            IConsumableSeq.filter predicate (toIConsumableSeq source) :> seq<_>

        [<CompiledName("Where")>]
        let where predicate source = filter predicate source

        [<CompiledName("Map")>]
        let map mapping source =
            IConsumableSeq.map mapping (toIConsumableSeq source) :> seq<_>

        [<CompiledName("MapIndexed")>]
        let mapi mapping source =
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt mapping
            IConsumableSeq.mapi (fun idx a ->f.Invoke(idx,a)) (toIConsumableSeq source) :> seq<_>

        [<CompiledName("MapIndexed2")>]
        let mapi2 mapping source1 source2 =
            let f = OptimizedClosures.FSharpFunc<int,'T,'U,'V>.Adapt mapping
            IConsumableSeq.mapi2 (fun idx a b -> f.Invoke (idx,a,b)) (source1 |> toIConsumableSeq1) (source2 |> toIConsumableSeq2) :> seq<_>

        [<CompiledName("Map2")>]
        let map2 mapping source1 source2 =
            IConsumableSeq.map2 mapping (source1 |> toIConsumableSeq1) (source2 |> toIConsumableSeq2) :> seq<_>

        [<CompiledName("Map3")>]
        let map3 mapping source1 source2 source3 =
            IConsumableSeq.map3 mapping (source1 |> toIConsumableSeq1) (source2 |> toIConsumableSeq2) (source3 |> toIConsumableSeq3) :> seq<_>

        [<CompiledName("Choose")>]
        let choose chooser source =
            IConsumableSeq.choose chooser (toIConsumableSeq source) :> seq<_>

        [<CompiledName("Indexed")>]
        let indexed source =
            IConsumableSeq.indexed (toIConsumableSeq source) :> seq<_>

        [<CompiledName("Zip")>]
        let zip source1 source2  =
            IConsumableSeq.zip (source1 |> toIConsumableSeq1) (source2 |> toIConsumableSeq2) :> seq<_>

        [<CompiledName("Zip3")>]
        let zip3 source1 source2  source3 =
            IConsumableSeq.zip3 (source1 |> toIConsumableSeq1) (source2 |> toIConsumableSeq2) (source3 |> toIConsumableSeq3) :> seq<_>

        [<CompiledName("Cast")>]
        let cast (source: IEnumerable) =
            source |> IConsumableSeq.cast :> seq<_>

        [<CompiledName("TryPick")>]
        let tryPick chooser (source : seq<'T>)  =
            let original = toIConsumableSeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.tryPick chooser arr
            | :? list<'T> as lst -> List.tryPick chooser lst
            | raw -> IConsumableSeq.tryPick chooser (rawOrOriginal raw original)

        [<CompiledName("Pick")>]
        let pick chooser source  =
            let original = toIConsumableSeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.pick chooser arr
            | :? list<'T> as lst -> List.pick chooser lst
            | raw -> IConsumableSeq.pick chooser (rawOrOriginal raw original)

        [<CompiledName("TryFind")>]
        let tryFind predicate (source : seq<'T>)  =
            let original = toIConsumableSeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.tryFind predicate arr
            | :? list<'T> as lst -> List.tryFind predicate lst
            | raw -> IConsumableSeq.tryFind predicate (rawOrOriginal raw original)

        [<CompiledName("Find")>]
        let find predicate source =
            let original = toIConsumableSeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.find predicate arr
            | :? list<'T> as lst -> List.find predicate lst
            | raw -> IConsumableSeq.find predicate (rawOrOriginal raw original)

        [<CompiledName("Take")>]
        let take count (source : seq<'T>)    =
            IConsumableSeq.take count (toIConsumableSeq source) :> seq<_>

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
            sources |> toIConsumableSeqs |> IConsumableSeq.map toIConsumableSeq |> IConsumableSeq.concat :> seq<_>

        [<CompiledName("Length")>]
        let length (source : seq<'T>)    =
            IConsumableSeq.length (toIConsumableSeq source)

        [<CompiledName("Fold")>]
        let fold<'T,'State> folder (state:'State) (source:seq<'T>)  =
            let original = toIConsumableSeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.fold folder state arr
            | :? list<'T> as lst -> List.fold folder state lst
            | raw ->
                let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt folder
                IConsumableSeq.fold (fun acc item -> f.Invoke (acc, item)) state (rawOrOriginal raw original)

        [<CompiledName("Fold2")>]
        let fold2<'T1,'T2,'State> folder (state:'State) (source1:seq<'T1>) (source2:seq<'T2>) =
            let f = OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt folder
            IConsumableSeq.fold2 (fun acc item1 item2 -> f.Invoke (acc, item1, item2)) state (source1 |> toIConsumableSeq1) (source2 |> toIConsumableSeq2)

        [<CompiledName("Reduce")>]
        let reduce reduction (source : seq<'T>)  =
            let original = toIConsumableSeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.reduce reduction arr
            | :? list<'T> as lst -> List.reduce reduction lst
            | raw ->
                let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt reduction
                IConsumableSeq.reduce (fun acc item -> f.Invoke (acc, item)) (rawOrOriginal raw original)

        [<CompiledName("Replicate")>]
        let replicate count initial =
            IConsumableSeq.replicate count initial :> seq<_>

        [<CompiledName("Append")>]
        let append (source1: seq<'T>) (source2: seq<'T>) =
            IConsumableSeq.append (source1 |> toIConsumableSeq1) (source2 |> toIConsumableSeq2) :> seq<_>

        [<CompiledName("Collect")>]
        let collect mapping source =
            map mapping source |> concat

        [<CompiledName("CompareWith")>]
        let compareWith (comparer:'T->'T->int) (source1:seq<'T>) (source2:seq<'T>) =
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt comparer
            IConsumableSeq.compareWith (fun a b -> f.Invoke(a,b)) (source1 |> toIConsumableSeq1) (source2 |> toIConsumableSeq2)

        [<CompiledName("OfList")>]
        let ofList (source : 'T list) =
            IConsumableSeq.ofList source :> seq<_>

        [<CompiledName("ToList")>]
        let toList (source : seq<'T>) =
            let original = toIConsumableSeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.toList arr
            | :? list<'T> as lst -> lst
            | raw -> IConsumableSeq.toList (rawOrOriginal raw original)

        [<CompiledName("OfArray")>]
        let ofArray (source : 'T array) =
            IConsumableSeq.ofArray source :> seq<_>

        [<CompiledName("ToArray")>]
        let toArray (source : seq<'T>)  =
            let original = toIConsumableSeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.copy arr
            | :? list<'T> as lst -> List.toArray lst
            | :? ICollection<'T> as res ->
                // Directly create an array and copy ourselves.
                // This avoids an extra copy if using ResizeArray in fallback below.
                let arr = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked res.Count
                res.CopyTo(arr, 0)
                arr
            | raw -> IConsumableSeq.toArray (rawOrOriginal raw original)

        [<CompiledName("FoldBack")>]
        let foldBack<'T,'State> folder (source:seq<'T>) (state:'State) =
            IConsumableSeq.foldBack folder (toIConsumableSeq source) state

        [<CompiledName("FoldBack2")>]
        let foldBack2<'T1,'T2,'State> folder (source1:seq<'T1>) (source2:seq<'T2>) (state:'State) =
            IConsumableSeq.foldBack2 folder (toIConsumableSeq1 source1) (toIConsumableSeq2 source2) state

        [<CompiledName("ReduceBack")>]
        let reduceBack reduction (source:seq<'T>) =
            IConsumableSeq.reduceBack reduction (toIConsumableSeq source)

        [<CompiledName("Singleton")>]
        let singleton value =
            IConsumableSeq.singleton value :> seq<_>

        [<CompiledName("Truncate")>]
        let truncate count (source: seq<'T>) =
            IConsumableSeq.truncate count (toIConsumableSeq source) :> seq<_>

        [<CompiledName("Pairwise")>]
        let pairwise (source: seq<'T>) =
            IConsumableSeq.pairwise (toIConsumableSeq source) :> seq<_>

        [<CompiledName("Scan")>]
        let scan<'T,'State> folder (state:'State) (source : seq<'T>) =
            IConsumableSeq.scan folder state (toIConsumableSeq source) :> seq<_>

        [<CompiledName("TryFindBack")>]
        let tryFindBack predicate (source : seq<'T>) =
            IConsumableSeq.tryFindBack predicate (toIConsumableSeq source)

        [<CompiledName("FindBack")>]
        let findBack predicate source =
            IConsumableSeq.findBack predicate (toIConsumableSeq source)

        [<CompiledName("ScanBack")>]
        let scanBack<'T,'State> folder (source:seq<'T>) (state:'State) =
            IConsumableSeq.scanBack folder (toIConsumableSeq source) state :> seq<_>

        [<CompiledName("FindIndex")>]
        let findIndex predicate (source:seq<_>) =
            IConsumableSeq.findIndex predicate (toIConsumableSeq source)

        [<CompiledName("TryFindIndex")>]
        let tryFindIndex predicate (source:seq<_>) =
            IConsumableSeq.tryFindIndex predicate (toIConsumableSeq source)

        [<CompiledName("TryFindIndexBack")>]
        let tryFindIndexBack predicate (source : seq<'T>) =
            IConsumableSeq.tryFindIndexBack predicate (toIConsumableSeq source)

        [<CompiledName("FindIndexBack")>]
        let findIndexBack predicate source =
            IConsumableSeq.findIndexBack predicate (toIConsumableSeq source)

        // windowed : int -> seq<'T> -> seq<'T[]>
        [<CompiledName("Windowed")>]
        let windowed windowSize (source: seq<_>) =
            IConsumableSeq.windowed windowSize (toIConsumableSeq source) :> seq<_>

        [<CompiledName("Cache")>]
        let cache (source : seq<'T>) =
            IConsumableSeq.cache (toIConsumableSeq source) :> seq<_>

        [<CompiledName("AllPairs")>]
        let allPairs source1 source2 =
            IConsumableSeq.allPairs (source1 |> toIConsumableSeq1) (source2 |> toIConsumableSeq2) :> seq<_>

        [<CodeAnalysis.SuppressMessage("Microsoft.Naming","CA1709:IdentifiersShouldBeCasedCorrectly"); CodeAnalysis.SuppressMessage("Microsoft.Naming","CA1707:IdentifiersShouldNotContainUnderscores"); CodeAnalysis.SuppressMessage("Microsoft.Naming","CA1704:IdentifiersShouldBeSpelledCorrectly")>]
        [<CompiledName("ReadOnly")>]
        let readonly (source:seq<_>) =
            checkNonNull "source" source
            mkSeq (fun () -> source.GetEnumerator())

        [<CompiledName("GroupBy")>]
        let groupBy (projection:'T->'Key) (source:seq<'T>) =
            delay (fun () ->
                let grouped = 
#if FX_RESHAPED_REFLECTION
                    if (typeof<'Key>).GetTypeInfo().IsValueType
#else
                    if typeof<'Key>.IsValueType
#endif
                        then source |> toIConsumableSeq |> IConsumableSeq.GroupBy.byVal projection
                        else source |> toIConsumableSeq |> IConsumableSeq.GroupBy.byRef projection

                grouped
                |> IConsumableSeq.map (fun (key,value) -> key, value :> seq<_>)
                :> seq<_>)

        [<CompiledName("Transpose")>]
        let transpose (source: seq<#seq<'T>>) =
            checkNonNull "source" source
            source
            |> collect indexed
            |> groupBy fst
            |> map (snd >> (map snd))

        [<CompiledName("Distinct")>]
        let distinct source =
            IConsumableSeq.distinct (toIConsumableSeq source) :> seq<_>

        [<CompiledName("DistinctBy")>]
        let distinctBy projection source =
            IConsumableSeq.distinctBy projection (toIConsumableSeq source) :> seq<_>

        [<CompiledName("SortBy")>]
        let sortBy projection source =
            IConsumableSeq.sortBy projection (toIConsumableSeq source) :> seq<_>

        [<CompiledName("Sort")>]
        let sort source =
            IConsumableSeq.sort (toIConsumableSeq source) :> seq<_>

        [<CompiledName("SortWith")>]
        let sortWith comparer source =
            IConsumableSeq.sortWith comparer (toIConsumableSeq source) :> seq<_>

        [<CompiledName("SortByDescending")>]
        let inline sortByDescending projection source =
            IConsumableSeq.sortByDescending projection (toIConsumableSeq source) :> seq<_>

        [<CompiledName("SortDescending")>]
        let inline sortDescending source =
            IConsumableSeq.sortDescending (toIConsumableSeq source) :> seq<_>

        [<CompiledName("CountBy")>]
        let countBy (projection:'T->'Key) (source:seq<'T>) =
#if FX_RESHAPED_REFLECTION
            if (typeof<'Key>).GetTypeInfo().IsValueType
#else
            if typeof<'Key>.IsValueType
#endif
                then IConsumableSeq.CountBy.byVal projection (toIConsumableSeq source) :> seq<_>
                else IConsumableSeq.CountBy.byRef projection (toIConsumableSeq source) :> seq<_>

        [<CompiledName("Sum")>]
        let inline sum (source: seq< ^a>) : ^a =
            IConsumableSeq.sum (toIConsumableSeq source)

        [<CompiledName("SumBy")>]
        let inline sumBy (projection:'T-> ^U) (source:seq<'T>) : ^U =
            IConsumableSeq.sumBy projection (toIConsumableSeq source)

        [<CompiledName("Average")>]
        let inline average (source:seq< ^a>) : ^a =
            IConsumableSeq.average (toIConsumableSeq source)

        [<CompiledName("AverageBy")>]
        let inline averageBy (projection:'T-> ^U) (source:seq<'T>) : ^U =
            IConsumableSeq.averageBy projection (toIConsumableSeq source)

        [<CompiledName("Min")>]
        let inline min (source:seq<_>) =
            IConsumableSeq.min (toIConsumableSeq source)

        [<CompiledName("MinBy")>]
        let inline minBy (projection:'T->'U) (source:seq<'T>) : 'T =
            IConsumableSeq.minBy projection (toIConsumableSeq source)

        [<CompiledName("Max")>]
        let inline max (source: seq<_>) =
            IConsumableSeq.max (toIConsumableSeq source)

        [<CompiledName("MaxBy")>]
        let inline maxBy (projection:'T->'U) (source:seq<'T>) : 'T =
            IConsumableSeq.maxBy projection (toIConsumableSeq source)

        [<CompiledName("TakeWhile")>]
        let takeWhile predicate (source: seq<_>) =
            IConsumableSeq.takeWhile predicate (toIConsumableSeq source) :> seq<_>

        [<CompiledName("Skip")>]
        let skip count (source: seq<_>) =
            IConsumableSeq.skip count (toIConsumableSeq source) :> seq<_>

        [<CompiledName("SkipWhile")>]
        let skipWhile predicate (source: seq<_>) =
            IConsumableSeq.skipWhile predicate (toIConsumableSeq source) :> seq<_>

        [<CompiledName("ForAll2")>]
        let forall2 predicate (source1:seq<_>) (source2:seq<_>) =
            let p = OptimizedClosures.FSharpFunc<_,_,_>.Adapt predicate
            IConsumableSeq.forall2 (fun a b -> p.Invoke(a,b)) (source1 |> toIConsumableSeq1) (source2 |> toIConsumableSeq2)

        [<CompiledName("Exists2")>]
        let exists2 predicate (source1:seq<_>) (source2:seq<_>) =
            let p = OptimizedClosures.FSharpFunc<_,_,_>.Adapt predicate
            IConsumableSeq.exists2 (fun a b -> p.Invoke(a,b)) (source1 |> toIConsumableSeq1) (source2 |> toIConsumableSeq2)

        [<CompiledName("Head")>]
        let head (source : seq<_>) =
            let original = toIConsumableSeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.head arr
            | :? list<'T> as lst -> List.head lst
            | raw -> IConsumableSeq.head (rawOrOriginal raw original)

        [<CompiledName("TryHead")>]
        let tryHead (source : seq<_>) =
            let original = toIConsumableSeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.tryHead arr
            | :? list<'T> as lst -> List.tryHead lst
            | raw -> IConsumableSeq.tryHead (rawOrOriginal raw original)

        [<CompiledName("Tail")>]
        let tail (source: seq<'T>) =
            IConsumableSeq.tail (toIConsumableSeq source) :> seq<_>

        [<CompiledName("Last")>]
        let last (source : seq<_>) =
            let original = toIConsumableSeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.last arr
            | :? list<'T> as lst -> List.last lst
            | raw -> IConsumableSeq.last (rawOrOriginal raw original)

        [<CompiledName("TryLast")>]
        let tryLast (source : seq<_>) =
            let original = toIConsumableSeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.tryLast arr
            | :? list<'T> as lst -> List.tryLast lst
            | raw -> IConsumableSeq.tryLast (rawOrOriginal raw original)

        [<CompiledName("ExactlyOne")>]
        let exactlyOne (source : seq<_>) =
            let original = toIConsumableSeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.exactlyOne arr
            | :? list<'T> as lst -> List.exactlyOne lst
            | raw -> IConsumableSeq.exactlyOne (rawOrOriginal raw original)

        [<CompiledName("Reverse")>]
        let rev source =
            IConsumableSeq.delay (fun () ->
                let original = toIConsumableSeq source
                match getRaw original with
                | :? array<'T> as arr -> Array.rev arr |> IConsumableSeq.ofArray
                | :? list<'T> as lst -> List.rev lst :> _
                | raw -> IConsumableSeq.rev (rawOrOriginal raw original)) :> seq<_>

        [<CompiledName("Permute")>]
        let permute indexMap (source : seq<_>) =
            IConsumableSeq.permute indexMap (toIConsumableSeq source) :> seq<_>

        [<CompiledName("MapFold")>]
        let mapFold<'T,'State,'Result> (mapping:'State->'T->'Result*'State) state source =
            IConsumableSeq.mapFold mapping state (toIConsumableSeq source) |> fun (iseq, state) -> iseq :> seq<_>, state

        [<CompiledName("MapFoldBack")>]
        let mapFoldBack<'T,'State,'Result> (mapping:'T->'State->'Result*'State) source state =
            IConsumableSeq.mapFoldBack mapping (toIConsumableSeq source) state |> fun (iseq, state) -> iseq :> seq<_>, state

        [<CompiledName("Except")>]
        let except (itemsToExclude: seq<'T>) (source: seq<'T>) =
            IConsumableSeq.except itemsToExclude (toIConsumableSeq source) :> seq<_>

        [<CompiledName("ChunkBySize")>]
        let chunkBySize chunkSize (source : seq<_>) =
            IConsumableSeq.chunkBySize chunkSize (toIConsumableSeq source) :> seq<_>

        [<CompiledName("SplitInto")>]
        let splitInto count source =
            IConsumableSeq.splitInto count (toIConsumableSeq source) :> seq<_>