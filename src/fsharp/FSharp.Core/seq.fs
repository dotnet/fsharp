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
        let inline toISeq  (source:seq<'T>)  : ISeq<'T> = checkNonNull "source" source;   ISeq.ofSeq source
        let inline toISeq1 (source1:seq<'T>) : ISeq<'T> = checkNonNull "source1" source1; ISeq.ofSeq source1
        let inline toISeq2 (source2:seq<'T>) : ISeq<'T> = checkNonNull "source2" source2; ISeq.ofSeq source2
        let inline toISeq3 (source3:seq<'T>) : ISeq<'T> = checkNonNull "source3" source3; ISeq.ofSeq source3
        let inline toISeqs (sources:seq<'T>) : ISeq<'T> = checkNonNull "sources" sources; ISeq.ofSeq sources

        let getRaw (source:ISeq<_>) =
            match source with
            | :? Core.EnumerableBase<'T> as s -> s.GetRaw ()
            | _ -> upcast source

        let rawOrOriginal (raw:seq<_>) (original:ISeq<_>) =
            if obj.ReferenceEquals (raw, original) then original else toISeq raw

        let mkDelayedSeq (f: unit -> IEnumerable<'T>) = mkSeq (fun () -> f().GetEnumerator())

        [<CompiledName("Delay")>]
        let delay generator = mkDelayedSeq generator

        [<CompiledName("Unfold")>]
        let unfold generator state =
            ISeq.unfold generator state :> seq<_>

        [<CompiledName("Empty")>]
        let empty<'T> = (EmptyEnumerable :> seq<'T>)

        [<CompiledName("InitializeInfinite")>]
        let initInfinite initializer =
            ISeq.initInfinite initializer :> seq<_>

        [<CompiledName("Initialize")>]
        let init count initializer =
            ISeq.init count initializer :> seq<_>

        [<CompiledName("Iterate")>]
        let iter action (source:seq<'T>) =
            let original = toISeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.iter action arr
            | :? list<'T> as lst -> List.iter action lst
            | raw -> ISeq.iter action (rawOrOriginal raw original)

        [<CompiledName("Item")>]
        let item index (source : seq<'T>) =
            checkNonNull "source" source
            if index < 0 then invalidArgInputMustBeNonNegative "index" index
            use e = source.GetEnumerator()
            IEnumerator.nth index e

        [<CompiledName("TryItem")>]
        let tryItem index (source : seq<'T>) =
            let original = toISeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.tryItem index arr
            | :? list<'T> as lst -> List.tryItem index lst
            | raw -> ISeq.tryItem index (rawOrOriginal raw original)

        [<CompiledName("Get")>]
        let nth index (source : seq<'T>) = item index source

        [<CompiledName("IterateIndexed")>]
        let iteri action (source:seq<'T>) =
            let original = toISeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.iteri action arr
            | :? list<'T> as lst -> List.iteri action lst
            | raw -> 
                let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt action
                ISeq.iteri (fun idx a -> f.Invoke (idx,a)) (rawOrOriginal raw original)

        [<CompiledName("Exists")>]
        let exists predicate (source:seq<'T>) =
            let original = toISeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.exists predicate arr
            | :? list<'T> as lst -> List.exists predicate lst
            | raw -> ISeq.exists predicate (rawOrOriginal raw original)

        [<CompiledName("Contains")>]
        let inline contains element (source : seq<'T>) =
            ISeq.contains element (toISeq source)

        [<CompiledName("ForAll")>]
        let forall predicate (source:seq<'T>) =
            let original = toISeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.forall predicate arr
            | :? list<'T> as lst -> List.forall predicate lst
            | raw -> ISeq.forall predicate (rawOrOriginal raw original)

        [<CompiledName("Iterate2")>]
        let iter2 action (source1:seq<_>) (source2:seq<_>)    =
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt action
            ISeq.iter2 (fun a b -> f.Invoke(a,b)) (source1 |> toISeq1) (source2 |> toISeq2)

        [<CompiledName("IterateIndexed2")>]
        let iteri2 action (source1 : seq<_>) (source2 : seq<_>) =
            let f = OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt action
            ISeq.iteri2 (fun idx a b -> f.Invoke(idx,a,b)) (source1 |> toISeq1) (source2 |> toISeq2)

        [<CompiledName("Filter")>]
        let filter predicate source =
            ISeq.filter predicate (toISeq source) :> seq<_>

        [<CompiledName("Where")>]
        let where predicate source = filter predicate source

        [<CompiledName("Map")>]
        let map mapping source =
            ISeq.map mapping (toISeq source) :> seq<_>

        [<CompiledName("MapIndexed")>]
        let mapi mapping source =
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt mapping
            ISeq.mapi (fun idx a ->f.Invoke(idx,a)) (toISeq source) :> seq<_>

        [<CompiledName("MapIndexed2")>]
        let mapi2 mapping source1 source2 =
            let f = OptimizedClosures.FSharpFunc<int,'T,'U,'V>.Adapt mapping
            ISeq.mapi2 (fun idx a b -> f.Invoke (idx,a,b)) (source1 |> toISeq1) (source2 |> toISeq2) :> seq<_>

        [<CompiledName("Map2")>]
        let map2 mapping source1 source2 =
            ISeq.map2 mapping (source1 |> toISeq1) (source2 |> toISeq2) :> seq<_>

        [<CompiledName("Map3")>]
        let map3 mapping source1 source2 source3 =
            ISeq.map3 mapping (source1 |> toISeq1) (source2 |> toISeq2) (source3 |> toISeq3) :> seq<_>

        [<CompiledName("Choose")>]
        let choose f source      =
            ISeq.choose f (toISeq source) :> seq<_>

        [<CompiledName("Indexed")>]
        let indexed source =
            ISeq.indexed (toISeq source) :> seq<_>

        [<CompiledName("Zip")>]
        let zip source1 source2  =
            ISeq.zip (source1 |> toISeq1) (source2 |> toISeq2) :> seq<_>

        [<CompiledName("Zip3")>]
        let zip3 source1 source2  source3 =
            ISeq.zip3 (source1 |> toISeq1) (source2 |> toISeq2) (source3 |> toISeq3) :> seq<_>

        [<CompiledName("Cast")>]
        let cast (source: IEnumerable) =
            source |> ISeq.cast :> seq<_>

        [<CompiledName("TryPick")>]
        let tryPick chooser (source : seq<'T>)  =
            let original = toISeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.tryPick chooser arr
            | :? list<'T> as lst -> List.tryPick chooser lst
            | raw -> ISeq.tryPick chooser (rawOrOriginal raw original)

        [<CompiledName("Pick")>]
        let pick chooser source  =
            let original = toISeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.pick chooser arr
            | :? list<'T> as lst -> List.pick chooser lst
            | raw -> ISeq.pick chooser (rawOrOriginal raw original)

        [<CompiledName("TryFind")>]
        let tryFind predicate (source : seq<'T>)  =
            let original = toISeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.tryFind predicate arr
            | :? list<'T> as lst -> List.tryFind predicate lst
            | raw -> ISeq.tryFind predicate (rawOrOriginal raw original)

        [<CompiledName("Find")>]
        let find predicate source =
            let original = toISeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.find predicate arr
            | :? list<'T> as lst -> List.find predicate lst
            | raw -> ISeq.find predicate (rawOrOriginal raw original)

        [<CompiledName("Take")>]
        let take count (source : seq<'T>)    =
            ISeq.take count (toISeq source) :> seq<_>

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
            sources |> toISeqs |> ISeq.map toISeq |> ISeq.concat :> seq<_>

        [<CompiledName("Length")>]
        let length (source : seq<'T>)    =
            ISeq.length (toISeq source)

        [<CompiledName("Fold")>]
        let fold<'T,'State> folder (state:'State) (source:seq<'T>)  =
            let original = toISeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.fold folder state arr
            | :? list<'T> as lst -> List.fold folder state lst
            | raw ->
                let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt folder
                ISeq.fold (fun acc item -> f.Invoke (acc, item)) state (rawOrOriginal raw original)

        [<CompiledName("Fold2")>]
        let fold2<'T1,'T2,'State> folder (state:'State) (source1:seq<'T1>) (source2:seq<'T2>) =
            let f = OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt folder
            ISeq.fold2 (fun acc item1 item2 -> f.Invoke (acc, item1, item2)) state (source1 |> toISeq1) (source2 |> toISeq2)

        [<CompiledName("Reduce")>]
        let reduce reduction (source : seq<'T>)  =
            let original = toISeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.reduce reduction arr
            | :? list<'T> as lst -> List.reduce reduction lst
            | raw ->
                let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt reduction
                ISeq.reduce (fun acc item -> f.Invoke (acc, item)) (rawOrOriginal raw original)

        [<CompiledName("Replicate")>]
        let replicate count initial =
            ISeq.replicate count initial :> seq<_>

        [<CompiledName("Append")>]
        let append (source1: seq<'T>) (source2: seq<'T>) =
            ISeq.append (source1 |> toISeq1) (source2 |> toISeq2) :> seq<_>

        [<CompiledName("Collect")>]
        let collect mapping source = map mapping source |> concat

        [<CompiledName("CompareWith")>]
        let compareWith (f:'T -> 'T -> int) (source1 : seq<'T>) (source2: seq<'T>) =
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt f
            ISeq.compareWith (fun a b -> f.Invoke(a,b)) (source1 |> toISeq1) (source2 |> toISeq2)

        [<CompiledName("OfList")>]
        let ofList (source : 'T list) =
            ISeq.ofList source :> seq<_>

        [<CompiledName("ToList")>]
        let toList (source : seq<'T>) =
            ISeq.toList (toISeq source)

        [<CompiledName("OfArray")>]
        let ofArray (source : 'T array) =
            ISeq.ofArray source :> seq<_>

        [<CompiledName("ToArray")>]
        let toArray (source : seq<'T>)  =
            ISeq.toArray (toISeq source)

        [<CompiledName("FoldBack")>]
        let foldBack<'T,'State> folder (source:seq<'T>) (state:'State) =
            ISeq.foldBack folder (toISeq source) state

        [<CompiledName("FoldBack2")>]
        let foldBack2<'T1,'T2,'State> folder (source1:seq<'T1>) (source2:seq<'T2>) (state:'State) =
            ISeq.foldBack2 folder (toISeq1 source1) (toISeq2 source2) state

        [<CompiledName("ReduceBack")>]
        let reduceBack reduction (source:seq<'T>) =
            ISeq.reduceBack reduction (toISeq source)

        [<CompiledName("Singleton")>]
        let singleton value =
            ISeq.singleton value :> seq<_>

        [<CompiledName("Truncate")>]
        let truncate count (source: seq<'T>) =
            ISeq.truncate count (toISeq source) :> seq<_>

        [<CompiledName("Pairwise")>]
        let pairwise (source: seq<'T>) =
            ISeq.pairwise (toISeq source) :> seq<_>

        [<CompiledName("Scan")>]
        let scan<'T,'State> folder (state:'State) (source : seq<'T>) =
            ISeq.scan folder state (toISeq source) :> seq<_>

        [<CompiledName("TryFindBack")>]
        let tryFindBack predicate (source : seq<'T>) =
            ISeq.tryFindBack predicate (toISeq source)

        [<CompiledName("FindBack")>]
        let findBack predicate source =
            ISeq.findBack predicate (toISeq source)

        [<CompiledName("ScanBack")>]
        let scanBack<'T,'State> folder (source:seq<'T>) (state:'State) =
            ISeq.scanBack folder (toISeq source) state :> seq<_>

        [<CompiledName("FindIndex")>]
        let findIndex predicate (source:seq<_>) =
            ISeq.findIndex predicate (toISeq source)

        [<CompiledName("TryFindIndex")>]
        let tryFindIndex predicate (source:seq<_>) =
            ISeq.tryFindIndex predicate (toISeq source)

        [<CompiledName("TryFindIndexBack")>]
        let tryFindIndexBack predicate (source : seq<'T>) =
            ISeq.tryFindIndexBack predicate (toISeq source)

        [<CompiledName("FindIndexBack")>]
        let findIndexBack predicate source =
            ISeq.findIndexBack predicate (toISeq source)

        // windowed : int -> seq<'T> -> seq<'T[]>
        [<CompiledName("Windowed")>]
        let windowed windowSize (source: seq<_>) =
            ISeq.windowed windowSize (toISeq source) :> seq<_>

        [<CompiledName("Cache")>]
        let cache (source : seq<'T>) =
            ISeq.cache (toISeq source) :> seq<_>

        [<CompiledName("AllPairs")>]
        let allPairs source1 source2 =
            ISeq.allPairs (source1 |> toISeq1) (source2 |> toISeq2) :> seq<_>

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
                        then source |> toISeq |> ISeq.GroupBy.byVal projection
                        else source |> toISeq |> ISeq.GroupBy.byRef projection

                grouped
                |> ISeq.map (fun (key,value) -> key, value :> seq<_>)
                :> seq<_>)

        [<CompiledName("Distinct")>]
        let distinct source =
            ISeq.distinct (toISeq source) :> seq<_>

        [<CompiledName("DistinctBy")>]
        let distinctBy keyf source =
            ISeq.distinctBy keyf (toISeq source) :> seq<_>

        [<CompiledName("SortBy")>]
        let sortBy projection source =
            ISeq.sortBy projection (toISeq source) :> seq<_>

        [<CompiledName("Sort")>]
        let sort source =
            ISeq.sort (toISeq source) :> seq<_>

        [<CompiledName("SortWith")>]
        let sortWith comparer source =
            ISeq.sortWith comparer (toISeq source) :> seq<_>

        [<CompiledName("SortByDescending")>]
        let inline sortByDescending projection source =
            ISeq.sortByDescending projection (toISeq source) :> seq<_>

        [<CompiledName("SortDescending")>]
        let inline sortDescending source =
            ISeq.sortDescending (toISeq source) :> seq<_>

        [<CompiledName("CountBy")>]
        let countBy (keyf:'T->'Key) (source:seq<'T>) =
#if FX_RESHAPED_REFLECTION
            if (typeof<'Key>).GetTypeInfo().IsValueType
#else
            if typeof<'Key>.IsValueType
#endif
                then ISeq.CountBy.byVal keyf (toISeq source) :> seq<_>
                else ISeq.CountBy.byRef keyf (toISeq source) :> seq<_>

        [<CompiledName("Sum")>]
        let inline sum (source: seq< ^a>) : ^a =
            ISeq.sum (toISeq source)

        [<CompiledName("SumBy")>]
        let inline sumBy (projection : 'T -> ^U) (source: seq<'T>) : ^U =
            ISeq.sumBy projection (toISeq source)

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
        let inline minBy (projection:'T->'U) (source:seq<'T>) : 'T =
            ISeq.minBy projection (toISeq source)

        [<CompiledName("Max")>]
        let inline max (source: seq<_>) =
            ISeq.max (toISeq source)

        [<CompiledName("MaxBy")>]
        let inline maxBy (projection:'T->'U) (source:seq<'T>) : 'T =
            ISeq.maxBy projection (toISeq source)

        [<CompiledName("TakeWhile")>]
        let takeWhile predicate (source: seq<_>) =
            ISeq.takeWhile predicate (toISeq source) :> seq<_>

        [<CompiledName("Skip")>]
        let skip count (source: seq<_>) =
            ISeq.skip count (toISeq source) :> seq<_>

        [<CompiledName("SkipWhile")>]
        let skipWhile predicate (source: seq<_>) =
            ISeq.skipWhile predicate (toISeq source) :> seq<_>

        [<CompiledName("ForAll2")>]
        let forall2 predicate (source1:seq<_>) (source2:seq<_>) =
            let p = OptimizedClosures.FSharpFunc<_,_,_>.Adapt predicate
            ISeq.forall2 (fun a b -> p.Invoke(a,b)) (source1 |> toISeq1) (source2 |> toISeq2)

        [<CompiledName("Exists2")>]
        let exists2 predicate (source1:seq<_>) (source2:seq<_>) =
            let p = OptimizedClosures.FSharpFunc<_,_,_>.Adapt predicate
            ISeq.exists2 (fun a b -> p.Invoke(a,b)) (source1 |> toISeq1) (source2 |> toISeq2)

        [<CompiledName("Head")>]
        let head (source : seq<_>) =
            let original = toISeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.head arr
            | :? list<'T> as lst -> List.head lst
            | raw -> ISeq.head (rawOrOriginal raw original)

        [<CompiledName("TryHead")>]
        let tryHead (source : seq<_>) =
            let original = toISeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.tryHead arr
            | :? list<'T> as lst -> List.tryHead lst
            | raw -> ISeq.tryHead (rawOrOriginal raw original)

        [<CompiledName("Tail")>]
        let tail (source: seq<'T>) =
            ISeq.tail (toISeq source) :> seq<_>

        [<CompiledName("Last")>]
        let last (source : seq<_>) =
            let original = toISeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.last arr
            | :? list<'T> as lst -> List.last lst
            | raw -> ISeq.last (rawOrOriginal raw original)

        [<CompiledName("TryLast")>]
        let tryLast (source : seq<_>) =
            let original = toISeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.tryLast arr
            | :? list<'T> as lst -> List.tryLast lst
            | raw -> ISeq.tryLast (rawOrOriginal raw original)

        [<CompiledName("ExactlyOne")>]
        let exactlyOne (source : seq<_>) =
            let original = toISeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.exactlyOne arr
            | :? list<'T> as lst -> List.exactlyOne lst
            | raw -> ISeq.exactlyOne (rawOrOriginal raw original)

        [<CompiledName("Reverse")>]
        let rev source =
            ISeq.rev (toISeq source) :> seq<_>

        [<CompiledName("Permute")>]
        let permute indexMap (source : seq<_>) =
            ISeq.permute indexMap (toISeq source) :> seq<_>

        [<CompiledName("MapFold")>]
        let mapFold<'T,'State,'Result> (mapping:'State->'T->'Result*'State) state source =
            ISeq.mapFold mapping state (toISeq source) |> fun (iseq, state) -> iseq :> seq<_>, state

        [<CompiledName("MapFoldBack")>]
        let mapFoldBack<'T,'State,'Result> (mapping:'T->'State->'Result*'State) source state =
            ISeq.mapFoldBack mapping (toISeq source) state |> fun (iseq, state) -> iseq :> seq<_>, state

        [<CompiledName("Except")>]
        let except (itemsToExclude: seq<'T>) (source: seq<'T>) =
            ISeq.except itemsToExclude (toISeq source) :> seq<_>

        [<CompiledName("ChunkBySize")>]
        let chunkBySize chunkSize (source : seq<_>) =
            ISeq.chunkBySize chunkSize (toISeq source) :> seq<_>

        [<CompiledName("SplitInto")>]
        let splitInto count source =
            ISeq.splitInto count (toISeq source) :> seq<_>