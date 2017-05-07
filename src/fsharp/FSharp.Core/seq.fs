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

        let getRaw (source:ISeq<_>) =
            match source with
            | :? Core.EnumerableBase<'T> as s -> s.GetRaw ()
            | _ -> upcast source

        let rawOrOriginal (raw:seq<_>) (original:ISeq<_>) =
            if obj.ReferenceEquals (raw, original) then original else toISeq raw

        let mkDelayedSeq (f: unit -> IEnumerable<'T>) = mkSeq (fun () -> f().GetEnumerator())

        [<CompiledName("Delay")>]
        let delay f = mkDelayedSeq f

        [<CompiledName("Unfold")>]
        let unfold f x =
            ISeq.unfold f x :> seq<_>

        [<CompiledName("Empty")>]
        let empty<'T> = (EmptyEnumerable :> seq<'T>)

        [<CompiledName("InitializeInfinite")>]
        let initInfinite f =
            ISeq.initInfinite f :> seq<_>

        [<CompiledName("Initialize")>]
        let init count f =
            ISeq.init count f :> seq<_>

        [<CompiledName("Iterate")>]
        let iter f (source : seq<'T>) =
            let original = toISeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.iter f arr
            | :? list<'T> as lst -> List.iter f lst
            | raw -> ISeq.iter f (rawOrOriginal raw original)

        [<CompiledName("Item")>]
        let item i (source : seq<'T>) =
            checkNonNull "source" source
            if i < 0 then invalidArgInputMustBeNonNegative "index" i
            use e = source.GetEnumerator()
            IEnumerator.nth i e

        [<CompiledName("TryItem")>]
        let tryItem i (source : seq<'T>) =
            let original = toISeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.tryItem i arr
            | :? list<'T> as lst -> List.tryItem i lst
            | raw -> ISeq.tryItem i (rawOrOriginal raw original)

        [<CompiledName("Get")>]
        let nth i (source : seq<'T>) =
            item i source

        [<CompiledName("IterateIndexed")>]
        let iteri f (source : seq<'T>) =
            let original = toISeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.iteri f arr
            | :? list<'T> as lst -> List.iteri f lst
            | raw -> 
                let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt f
                ISeq.iteri (fun idx a -> f.Invoke (idx,a)) (rawOrOriginal raw original)

        [<CompiledName("Exists")>]
        let exists f (source : seq<'T>) =
            let original = toISeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.exists f arr
            | :? list<'T> as lst -> List.exists f lst
            | raw -> ISeq.exists f (rawOrOriginal raw original)

        [<CompiledName("Contains")>]
        let inline contains element (source : seq<'T>) =
            ISeq.contains element (toISeq source)

        [<CompiledName("ForAll")>]
        let forall f (source : seq<'T>) =
            let original = toISeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.forall f arr
            | :? list<'T> as lst -> List.forall f lst
            | raw -> ISeq.forall f (rawOrOriginal raw original)

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
            ISeq.filter f (toISeq source) :> seq<_>

        [<CompiledName("Where")>]
        let where f source      = filter f source

        [<CompiledName("Map")>]
        let map    f source      =
            ISeq.map f (toISeq source) :> seq<_>

        [<CompiledName("MapIndexed")>]
        let mapi f source      =
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt f
            ISeq.mapi (fun idx a ->f.Invoke(idx,a)) (toISeq source) :> seq<_>

        [<CompiledName("MapIndexed2")>]
        let mapi2 f source1 source2 =
            let f = OptimizedClosures.FSharpFunc<int,'T,'U,'V>.Adapt f
            ISeq.mapi2 (fun idx a b -> f.Invoke (idx,a,b)) (source1 |> toISeq1) (source2 |> toISeq2) :> seq<_>

        [<CompiledName("Map2")>]
        let map2 f source1 source2 =
            ISeq.map2 f (source1 |> toISeq1) (source2 |> toISeq2) :> seq<_>

        [<CompiledName("Map3")>]
        let map3 f source1 source2 source3 =
            ISeq.map3 f (source1 |> toISeq1) (source2 |> toISeq2) (source3 |> toISeq3) :> seq<_>

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
        let tryPick f (source : seq<'T>)  =
            let original = toISeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.tryPick f arr
            | :? list<'T> as lst -> List.tryPick f lst
            | raw -> ISeq.tryPick f (rawOrOriginal raw original)

        [<CompiledName("Pick")>]
        let pick f source  =
            let original = toISeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.pick f arr
            | :? list<'T> as lst -> List.pick f lst
            | raw -> ISeq.pick f (rawOrOriginal raw original)

        [<CompiledName("TryFind")>]
        let tryFind f (source : seq<'T>)  =
            let original = toISeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.tryFind f arr
            | :? list<'T> as lst -> List.tryFind f lst
            | raw -> ISeq.tryFind f (rawOrOriginal raw original)

        [<CompiledName("Find")>]
        let find f source =
            let original = toISeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.find f arr
            | :? list<'T> as lst -> List.find f lst
            | raw -> ISeq.find f (rawOrOriginal raw original)

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
        let fold<'T,'State> f (x:'State) (source : seq<'T>)  =
            let original = toISeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.fold f x arr
            | :? list<'T> as lst -> List.fold f x lst
            | raw ->
                let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt f
                ISeq.fold (fun acc item -> f.Invoke (acc, item)) x (rawOrOriginal raw original)

        [<CompiledName("Fold2")>]
        let fold2<'T1,'T2,'State> f (state:'State) (source1: seq<'T1>) (source2: seq<'T2>) =
            let f = OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt f
            ISeq.fold2 (fun acc item1 item2 -> f.Invoke (acc, item1, item2)) state (source1 |> toISeq1) (source2 |> toISeq2)

        [<CompiledName("Reduce")>]
        let reduce f (source : seq<'T>)  =
            let original = toISeq source
            match getRaw original with
            | :? array<'T> as arr -> Array.reduce f arr
            | :? list<'T> as lst -> List.reduce f lst
            | raw ->
                let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt f
                ISeq.reduce (fun acc item -> f.Invoke (acc, item)) (rawOrOriginal raw original)

        [<CompiledName("Replicate")>]
        let replicate count x =
            ISeq.replicate count x :> seq<_>

        [<CompiledName("Append")>]
        let append (source1: seq<'T>) (source2: seq<'T>) =
            ISeq.append (source1 |> toISeq1) (source2 |> toISeq2) :> seq<_>

        [<CompiledName("Collect")>]
        let collect f sources = map f sources |> concat

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
            ISeq.singleton x :> seq<_>

        [<CompiledName("Truncate")>]
        let truncate n (source: seq<'T>) =
            ISeq.truncate n (toISeq source) :> seq<_>

        [<CompiledName("Pairwise")>]
        let pairwise (source: seq<'T>) =
            ISeq.pairwise (toISeq source) :> seq<_>

        [<CompiledName("Scan")>]
        let scan<'T,'State> f (z:'State) (source : seq<'T>) =
            ISeq.scan f z (toISeq source) :> seq<_>

        [<CompiledName("TryFindBack")>]
        let tryFindBack f (source : seq<'T>) =
            ISeq.tryFindBack f (toISeq source)

        [<CompiledName("FindBack")>]
        let findBack f source =
            ISeq.findBack f (toISeq source)

        [<CompiledName("ScanBack")>]
        let scanBack<'T,'State> f (source : seq<'T>) (acc:'State) =
            ISeq.scanBack f (toISeq source) acc :> seq<_>

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
                |> ISeq.map (fun (key,value) -> key, value :> seq<_>)
                :> seq<_>)

        [<CompiledName("Distinct")>]
        let distinct source =
            ISeq.distinct (toISeq source) :> seq<_>

        [<CompiledName("DistinctBy")>]
        let distinctBy keyf source =
            ISeq.distinctBy keyf (toISeq source) :> seq<_>

        [<CompiledName("SortBy")>]
        let sortBy keyf source =
            ISeq.sortBy keyf (toISeq source) :> seq<_>

        [<CompiledName("Sort")>]
        let sort source =
            ISeq.sort (toISeq source) :> seq<_>

        [<CompiledName("SortWith")>]
        let sortWith f source =
            ISeq.sortWith f (toISeq source) :> seq<_>

        [<CompiledName("SortByDescending")>]
        let inline sortByDescending keyf source =
            ISeq.sortByDescending keyf (toISeq source) :> seq<_>

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
            ISeq.takeWhile p (toISeq source) :> seq<_>

        [<CompiledName("Skip")>]
        let skip count (source: seq<_>) =
            ISeq.skip count (toISeq source) :> seq<_>

        [<CompiledName("SkipWhile")>]
        let skipWhile p (source: seq<_>) =
            ISeq.skipWhile p (toISeq source) :> seq<_>

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
        let permute f (source : seq<_>) =
            ISeq.permute f (toISeq source) :> seq<_>

        [<CompiledName("MapFold")>]
        let mapFold<'T,'State,'Result> (f: 'State -> 'T -> 'Result * 'State) acc source =
            ISeq.mapFold f acc (toISeq source) |> fun (iseq, state) -> iseq :> seq<_>, state

        [<CompiledName("MapFoldBack")>]
        let mapFoldBack<'T,'State,'Result> (f: 'T -> 'State -> 'Result * 'State) source acc =
            ISeq.mapFoldBack f (toISeq source) acc |> fun (iseq, state) -> iseq :> seq<_>, state

        [<CompiledName("Except")>]
        let except (itemsToExclude: seq<'T>) (source: seq<'T>) =
            ISeq.except itemsToExclude (toISeq source) :> seq<_>

        [<CompiledName("ChunkBySize")>]
        let chunkBySize chunkSize (source : seq<_>) =
            ISeq.chunkBySize chunkSize (toISeq source) :> seq<_>

        [<CompiledName("SplitInto")>]
        let splitInto count source =
            ISeq.splitInto count (toISeq source) :> seq<_>