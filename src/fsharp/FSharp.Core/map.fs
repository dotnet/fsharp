// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Collections

open System
open System.Collections.Generic
open System.Diagnostics
open System.Text
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators

[<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]
[<NoEquality; NoComparison>]
type MapTree<'Key, 'Value when 'Key : comparison > = 
    | MapEmpty 
    | MapOne of 'Key * 'Value
    | MapNode of 'Key * 'Value * MapTree<'Key, 'Value> *  MapTree<'Key, 'Value> * int

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module MapTree = 

    let rec sizeAux acc m = 
        match m with 
        | MapEmpty -> acc
        | MapOne _ -> acc + 1
        | MapNode (_, _, l, r, _) -> sizeAux (sizeAux (acc+1) l) r 

    let size x = sizeAux 0 x

#if TRACE_SETS_AND_MAPS
    let mutable traceCount = 0
    let mutable numOnes = 0
    let mutable numNodes = 0
    let mutable numAdds = 0
    let mutable numRemoves = 0
    let mutable numLookups = 0
    let mutable numUnions = 0
    let mutable totalSizeOnNodeCreation = 0.0
    let mutable totalSizeOnMapAdd = 0.0
    let mutable totalSizeOnMapLookup = 0.0
    let mutable largestMapSize = 0
    let mutable largestMapStackTrace = Unchecked.defaultof<_>

    let report() = 
       traceCount <- traceCount + 1 
       if traceCount % 1000000 = 0 then 
           System.Console.WriteLine(
               "#MapOne = {0}, #MapNode = {1}, #Add = {2}, #Remove = {3}, #Unions = {4}, #Lookups = {5}, avMapTreeSizeOnNodeCreation = {6}, avMapSizeOnCreation = {7}, avMapSizeOnLookup = {8}", 
               numOnes, numNodes, numAdds, numRemoves, numUnions, numLookups, 
               (totalSizeOnNodeCreation / float (numNodes + numOnes)), (totalSizeOnMapAdd / float numAdds), 
               (totalSizeOnMapLookup / float numLookups))
           System.Console.WriteLine("#largestMapSize = {0}, largestMapStackTrace = {1}", largestMapSize, largestMapStackTrace)

    let MapOne n = 
        report()
        numOnes <- numOnes + 1
        totalSizeOnNodeCreation <- totalSizeOnNodeCreation + 1.0
        MapTree.MapOne n

    let MapNode (x, l, v, r, h) = 
        report()
        numNodes <- numNodes + 1
        let n = MapTree.MapNode (x, l, v, r, h)
        totalSizeOnNodeCreation <- totalSizeOnNodeCreation + float (size n)
        n
#endif

    let empty = MapEmpty 

    let height (m: MapTree<'Key, 'Value>) = 
        match m with
        | MapEmpty -> 0
        | MapOne _ -> 1
        | MapNode (_, _, _, _, h) -> h

    let isEmpty (m: MapTree<'Key, 'Value>) = 
        match m with 
        | MapEmpty -> true
        | _ -> false

    let mk l k v r : MapTree<'Key, 'Value> = 
        match l, r with 
        | MapEmpty, MapEmpty -> MapOne (k, v)
        | _ -> 
            let hl = height l 
            let hr = height r 
            let m = if hl < hr then hr else hl 
            MapNode (k, v, l, r, m+1)

    let rebalance t1 (k: 'Key) (v: 'Value) t2 =
        let t1h = height t1 
        let t2h = height t2 
        if  t2h > t1h + 2 then (* right is heavier than left *)
            match t2 with 
            | MapNode (t2k, t2v, t2l, t2r, _) -> 
               // one of the nodes must have height > height t1 + 1 
               if height t2l > t1h + 1 then 
                   // balance left: combination 
                   match t2l with 
                   | MapNode (t2lk, t2lv, t2ll, t2lr, _) ->
                      mk (mk t1 k v t2ll) t2lk t2lv (mk t2lr t2k t2v t2r) 
                   | _ -> failwith "rebalance"
               else 
                   // rotate left 
                   mk (mk t1 k v t2l) t2k t2v t2r
            | _ -> failwith "rebalance"
        else
            if  t1h > t2h + 2 then (* left is heavier than right *)
                match t1 with 
                | MapNode (t1k, t1v, t1l, t1r, _) -> 
                    // one of the nodes must have height > height t2 + 1 
                    if height t1r > t2h + 1 then 
                        // balance right: combination 
                        match t1r with 
                        | MapNode (t1rk, t1rv, t1rl, t1rr, _) ->
                            mk (mk t1l t1k t1v t1rl) t1rk t1rv (mk t1rr k v t2)
                        | _ -> failwith "rebalance"
                    else
                        mk t1l t1k t1v (mk t1r k v t2)
                | _ -> failwith "rebalance"
            else mk t1 k v t2

    let rec add (comparer: IComparer<'Key>) k (v: 'Value) (m: MapTree<'Key, 'Value>) = 
        match m with 
        | MapEmpty -> MapOne (k, v)
        | MapOne (k2, _) -> 
            let c = comparer.Compare(k, k2) 
            if c < 0   then MapNode (k, v, MapEmpty, m, 2)
            elif c = 0 then MapOne (k, v)
            else            MapNode (k, v, m, MapEmpty, 2)
        | MapNode (k2, v2, l, r, h) -> 
            let c = comparer.Compare(k, k2) 
            if c < 0 then rebalance (add comparer k v l) k2 v2 r
            elif c = 0 then MapNode (k, v, l, r, h)
            else rebalance l k2 v2 (add comparer k v r) 

    let rec tryGetValue (comparer: IComparer<'Key>) k (v: byref<'Value>) (m: MapTree<'Key, 'Value>) = 
        match m with 
        | MapEmpty -> false
        | MapOne (k2, v2) -> 
            let c = comparer.Compare(k, k2) 
            if c = 0 then v <- v2; true
            else false
        | MapNode (k2, v2, l, r, _) -> 
            let c = comparer.Compare(k, k2) 
            if c < 0 then tryGetValue comparer k &v l
            elif c = 0 then v <- v2; true
            else tryGetValue comparer k &v r

    let find (comparer: IComparer<'Key>) k (m: MapTree<'Key, 'Value>) =
        let mutable v = Unchecked.defaultof<'Value>
        if tryGetValue comparer k &v m then
            v
        else
            raise (KeyNotFoundException())

    let tryFind (comparer: IComparer<'Key>) k (m: MapTree<'Key, 'Value>) = 
        let mutable v = Unchecked.defaultof<'Value>
        if tryGetValue comparer k &v m then
            Some v
        else
            None

    let partition1 (comparer: IComparer<'Key>) (f: OptimizedClosures.FSharpFunc<_, _, _>) k v (acc1, acc2) = 
        if f.Invoke (k, v) then (add comparer k v acc1, acc2) else (acc1, add comparer k v acc2) 

    let rec partitionAux (comparer: IComparer<'Key>) (f: OptimizedClosures.FSharpFunc<_, _, _>) m acc = 
        match m with 
        | MapEmpty -> acc
        | MapOne (k, v) -> partition1 comparer f k v acc
        | MapNode (k, v, l, r, _) -> 
            let acc = partitionAux comparer f r acc 
            let acc = partition1 comparer f k v acc
            partitionAux comparer f l acc

    let partition (comparer: IComparer<'Key>) f m =
        partitionAux comparer (OptimizedClosures.FSharpFunc<_, _, _>.Adapt f) m (empty, empty)

    let filter1 (comparer: IComparer<'Key>) (f: OptimizedClosures.FSharpFunc<_, _, _>) k v acc =
        if f.Invoke (k, v) then add comparer k v acc else acc 

    let rec filterAux (comparer: IComparer<'Key>) (f: OptimizedClosures.FSharpFunc<_, _, _>) m acc = 
        match m with 
        | MapEmpty -> acc
        | MapOne (k, v) -> filter1 comparer f k v acc
        | MapNode (k, v, l, r, _) ->
            let acc = filterAux comparer f l acc
            let acc = filter1 comparer f k v acc
            filterAux comparer f r acc

    let filter (comparer: IComparer<'Key>) f m =
        filterAux comparer (OptimizedClosures.FSharpFunc<_, _, _>.Adapt f) m empty

    let rec spliceOutSuccessor (m: MapTree<'Key, 'Value>) = 
        match m with 
        | MapEmpty -> failwith "internal error: Map.spliceOutSuccessor"
        | MapOne (k2, v2) -> k2, v2, MapEmpty
        | MapNode (k2, v2, l, r, _) ->
            match l with 
            | MapEmpty -> k2, v2, r
            | _ -> let k3, v3, l' = spliceOutSuccessor l in k3, v3, mk l' k2 v2 r

    let rec remove (comparer: IComparer<'Key>) k (m: MapTree<'Key, 'Value>) = 
        match m with 
        | MapEmpty -> empty
        | MapOne (k2, _) -> 
            let c = comparer.Compare(k, k2) 
            if c = 0 then MapEmpty else m
        | MapNode (k2, v2, l, r, _) -> 
            let c = comparer.Compare(k, k2) 
            if c < 0 then rebalance (remove comparer k l) k2 v2 r
            elif c = 0 then 
                match l, r with 
                | MapEmpty, _ -> r
                | _, MapEmpty -> l
                | _ -> 
                    let sk, sv, r' = spliceOutSuccessor r 
                    mk l sk sv r'
            else rebalance l k2 v2 (remove comparer k r) 

    let rec mem (comparer: IComparer<'Key>) k (m: MapTree<'Key, 'Value>) = 
        match m with 
        | MapEmpty -> false
        | MapOne (k2, _) -> (comparer.Compare(k, k2) = 0)
        | MapNode (k2, _, l, r, _) -> 
            let c = comparer.Compare(k, k2) 
            if c < 0 then mem comparer k l
            else (c = 0 || mem comparer k r)

    let rec iterOpt (f: OptimizedClosures.FSharpFunc<_, _, _>) (m: MapTree<'Key, 'Value>) =
        match m with 
        | MapEmpty -> ()
        | MapOne (k2, v2) -> f.Invoke (k2, v2)
        | MapNode (k2, v2, l, r, _) -> iterOpt f l; f.Invoke (k2, v2); iterOpt f r

    let iter f m =
        iterOpt (OptimizedClosures.FSharpFunc<_, _, _>.Adapt f) m

    let rec tryPickOpt (f: OptimizedClosures.FSharpFunc<_, _, _>) m =
        match m with 
        | MapEmpty -> None
        | MapOne (k2, v2) -> f.Invoke (k2, v2) 
        | MapNode (k2, v2, l, r, _) -> 
            match tryPickOpt f l with 
            | Some _ as res -> res 
            | None -> 
            match f.Invoke (k2, v2) with 
            | Some _ as res -> res 
            | None -> 
            tryPickOpt f r

    let tryPick f m =
        tryPickOpt (OptimizedClosures.FSharpFunc<_, _, _>.Adapt f) m

    let rec existsOpt (f: OptimizedClosures.FSharpFunc<_, _, _>) m = 
        match m with 
        | MapEmpty -> false
        | MapOne (k2, v2) -> f.Invoke (k2, v2)
        | MapNode (k2, v2, l, r, _) -> existsOpt f l || f.Invoke (k2, v2) || existsOpt f r

    let exists f m =
        existsOpt (OptimizedClosures.FSharpFunc<_, _, _>.Adapt f) m

    let rec forallOpt (f: OptimizedClosures.FSharpFunc<_, _, _>) m = 
        match m with 
        | MapEmpty -> true
        | MapOne (k2, v2) -> f.Invoke (k2, v2)
        | MapNode (k2, v2, l, r, _) -> forallOpt f l && f.Invoke (k2, v2) && forallOpt f r

    let forall f m =
        forallOpt (OptimizedClosures.FSharpFunc<_, _, _>.Adapt f) m

    let rec map f m = 
        match m with 
        | MapEmpty -> empty
        | MapOne (k, v) -> MapOne (k, f v)
        | MapNode (k, v, l, r, h) -> 
            let l2 = map f l 
            let v2 = f v 
            let r2 = map f r 
            MapNode (k, v2, l2, r2, h)

    let rec mapiOpt (f: OptimizedClosures.FSharpFunc<_, _, _>) m = 
        match m with
        | MapEmpty -> empty
        | MapOne (k, v) -> MapOne (k, f.Invoke (k, v))
        | MapNode (k, v, l, r, h) -> 
            let l2 = mapiOpt f l 
            let v2 = f.Invoke (k, v) 
            let r2 = mapiOpt f r 
            MapNode (k, v2, l2, r2, h)

    let mapi f m =
        mapiOpt (OptimizedClosures.FSharpFunc<_, _, _>.Adapt f) m

    let rec foldBackOpt (f: OptimizedClosures.FSharpFunc<_, _, _, _>) m x = 
        match m with 
        | MapEmpty -> x
        | MapOne (k, v) -> f.Invoke (k, v, x)
        | MapNode (k, v, l, r, _) -> 
            let x = foldBackOpt f r x
            let x = f.Invoke (k, v, x)
            foldBackOpt f l x

    let foldBack f m x =
        foldBackOpt (OptimizedClosures.FSharpFunc<_, _, _, _>.Adapt f) m x

    let rec foldOpt (f: OptimizedClosures.FSharpFunc<_, _, _, _>) x m  = 
        match m with 
        | MapEmpty -> x
        | MapOne (k, v) -> f.Invoke (x, k, v)
        | MapNode (k, v, l, r, _) -> 
            let x = foldOpt f x l
            let x = f.Invoke (x, k, v)
            foldOpt f x r

    let fold f x m =
        foldOpt (OptimizedClosures.FSharpFunc<_, _, _, _>.Adapt f) x m

    let foldSectionOpt (comparer: IComparer<'Key>) lo hi (f: OptimizedClosures.FSharpFunc<_, _, _, _>) m x =
        let rec foldFromTo (f: OptimizedClosures.FSharpFunc<_, _, _, _>) m x = 
            match m with 
            | MapEmpty -> x
            | MapOne (k, v) ->
                let cLoKey = comparer.Compare(lo, k)
                let cKeyHi = comparer.Compare(k, hi)
                let x = if cLoKey <= 0 && cKeyHi <= 0 then f.Invoke (k, v, x) else x
                x
            | MapNode (k, v, l, r, _) ->
                let cLoKey = comparer.Compare(lo, k)
                let cKeyHi = comparer.Compare(k, hi)
                let x = if cLoKey < 0 then foldFromTo f l x else x
                let x = if cLoKey <= 0 && cKeyHi <= 0 then f.Invoke (k, v, x) else x
                let x = if cKeyHi < 0 then foldFromTo f r x else x
                x

        if comparer.Compare(lo, hi) = 1 then x else foldFromTo f m x

    let foldSection (comparer: IComparer<'Key>) lo hi f m x =
        foldSectionOpt comparer lo hi (OptimizedClosures.FSharpFunc<_, _, _, _>.Adapt f) m x

    let toList m = 
        let rec loop m acc = 
            match m with 
            | MapEmpty -> acc
            | MapOne (k, v) -> (k, v) :: acc
            | MapNode (k, v, l, r, _) -> loop l ((k, v) :: loop r acc)
        loop m []

    let toArray m =
        m |> toList |> Array.ofList

    let ofList comparer l =
        List.fold (fun acc (k, v) -> add comparer k v acc) empty l

    let rec mkFromEnumerator comparer acc (e : IEnumerator<_>) = 
        if e.MoveNext() then 
            let (x, y) = e.Current 
            mkFromEnumerator comparer (add comparer x y acc) e
        else acc

    let ofArray comparer (arr : array<_>) =
        let mutable res = empty
        for (x, y) in arr do
            res <- add comparer x y res 
        res

    let ofSeq comparer (c : seq<'Key * 'T>) =
        match c with 
        | :? array<'Key * 'T> as xs -> ofArray comparer xs
        | :? list<'Key * 'T> as xs -> ofList comparer xs
        | _ -> 
            use ie = c.GetEnumerator()
            mkFromEnumerator comparer empty ie 

    let copyToArray m (arr: _[]) i =
        let j = ref i 
        m |> iter (fun x y -> arr.[!j] <- KeyValuePair(x, y); j := !j + 1)

    /// Imperative left-to-right iterators.
    [<NoEquality; NoComparison>]
    type MapIterator<'Key, 'Value when 'Key : comparison > = 
         { /// invariant: always collapseLHS result 
           mutable stack: MapTree<'Key, 'Value> list

           /// true when MoveNext has been called 
           mutable started : bool }

    // collapseLHS:
    // a) Always returns either [] or a list starting with MapOne.
    // b) The "fringe" of the set stack is unchanged. 
    let rec collapseLHS stack =
        match stack with
        | [] -> []
        | MapEmpty :: rest -> collapseLHS rest
        | MapOne _ :: _ -> stack
        | (MapNode (k, v, l, r, _)) :: rest -> collapseLHS (l :: MapOne (k, v) :: r :: rest)

    let mkIterator m =
        { stack = collapseLHS [m]; started = false }

    let notStarted() =
        raise (InvalidOperationException(SR.GetString(SR.enumerationNotStarted)))

    let alreadyFinished() =
        raise (InvalidOperationException(SR.GetString(SR.enumerationAlreadyFinished)))

    let current i =
        if i.started then
            match i.stack with
              | MapOne (k, v) :: _ -> new KeyValuePair<_, _>(k, v)
              | []            -> alreadyFinished()
              | _             -> failwith "Please report error: Map iterator, unexpected stack for current"
        else
            notStarted()

    let rec moveNext i =
        if i.started then
            match i.stack with
            | MapOne _ :: rest ->
                i.stack <- collapseLHS rest
                not i.stack.IsEmpty
            | [] -> false
            | _ -> failwith "Please report error: Map iterator, unexpected stack for moveNext"
        else
            i.started <- true  (* The first call to MoveNext "starts" the enumeration. *)
            not i.stack.IsEmpty

    let mkIEnumerator m = 
        let mutable i = mkIterator m 
        { new IEnumerator<_> with 
              member __.Current = current i

          interface System.Collections.IEnumerator with
              member __.Current = box (current i)
              member __.MoveNext() = moveNext i
              member __.Reset() = i <- mkIterator m

          interface System.IDisposable with 
              member __.Dispose() = ()}

[<System.Diagnostics.DebuggerTypeProxy(typedefof<MapDebugView<_, _>>)>]
[<System.Diagnostics.DebuggerDisplay("Count = {Count}")>]
[<Sealed>]
[<CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710: IdentifiersShouldHaveCorrectSuffix")>]
[<CompiledName("FSharpMap`2")>]
type Map<[<EqualityConditionalOn>]'Key, [<EqualityConditionalOn; ComparisonConditionalOn>]'Value when 'Key : comparison >(comparer: IComparer<'Key>, tree: MapTree<'Key, 'Value>) =

#if !FX_NO_BINARY_SERIALIZATION
    [<System.NonSerialized>]
    // This type is logically immutable. This field is only mutated during deserialization. 
    let mutable comparer = comparer 
 
    [<System.NonSerialized>]
    // This type is logically immutable. This field is only mutated during deserialization. 
    let mutable tree = tree 

    // This type is logically immutable. This field is only mutated during serialization and deserialization. 
    //
    // WARNING: The compiled name of this field may never be changed because it is part of the logical 
    // WARNING: permanent serialization format for this type.
    let mutable serializedData = null 
#endif

    // We use .NET generics per-instantiation static fields to avoid allocating a new object for each empty
    // set (it is just a lookup into a .NET table of type-instantiation-indexed static fields).
    static let empty = 
        let comparer = LanguagePrimitives.FastGenericComparer<'Key> 
        new Map<'Key, 'Value>(comparer, MapTree<_, _>.MapEmpty)

#if !FX_NO_BINARY_SERIALIZATION
    [<System.Runtime.Serialization.OnSerializingAttribute>]
    member __.OnSerializing(context: System.Runtime.Serialization.StreamingContext) =
        ignore context
        serializedData <- MapTree.toArray tree |> Array.map (fun (k, v) -> KeyValuePair(k, v))

    // Do not set this to null, since concurrent threads may also be serializing the data
    //[<System.Runtime.Serialization.OnSerializedAttribute>]
    //member __.OnSerialized(context: System.Runtime.Serialization.StreamingContext) =
    //    serializedData <- null

    [<System.Runtime.Serialization.OnDeserializedAttribute>]
    member __.OnDeserialized(context: System.Runtime.Serialization.StreamingContext) =
        ignore context
        comparer <- LanguagePrimitives.FastGenericComparer<'Key>
        tree <- serializedData |> Array.map (fun (KeyValue(k, v)) -> (k, v)) |> MapTree.ofArray comparer 
        serializedData <- null
#endif

    static member Empty : Map<'Key, 'Value> =
        empty

    static member Create(ie : IEnumerable<_>) : Map<'Key, 'Value> = 
        let comparer = LanguagePrimitives.FastGenericComparer<'Key> 
        new Map<_, _>(comparer, MapTree.ofSeq comparer ie)

    new (elements : seq<_>) = 
        let comparer = LanguagePrimitives.FastGenericComparer<'Key> 
        new Map<_, _>(comparer, MapTree.ofSeq comparer elements)

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member internal m.Comparer = comparer

    //[<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member internal m.Tree = tree

    member m.Add(key, value) : Map<'Key, 'Value> = 
#if TRACE_SETS_AND_MAPS
        MapTree.report()
        MapTree.numAdds <- MapTree.numAdds + 1
        let size = MapTree.size m.Tree + 1
        MapTree.totalSizeOnMapAdd <- MapTree.totalSizeOnMapAdd + float size
        if size > MapTree.largestMapSize then 
            MapTree.largestMapSize <- size
            MapTree.largestMapStackTrace <- System.Diagnostics.StackTrace().ToString()
#endif
        new Map<'Key, 'Value>(comparer, MapTree.add comparer key value tree)

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member m.IsEmpty = MapTree.isEmpty tree

    member m.Item 
     with get(key : 'Key) = 
#if TRACE_SETS_AND_MAPS
        MapTree.report()
        MapTree.numLookups <- MapTree.numLookups + 1
        MapTree.totalSizeOnMapLookup <- MapTree.totalSizeOnMapLookup + float (MapTree.size tree)
#endif
        MapTree.find comparer key tree

    member m.TryPick f =
        MapTree.tryPick f tree 

    member m.Exists predicate =
        MapTree.exists predicate tree 

    member m.Filter predicate =
        new Map<'Key, 'Value>(comparer, MapTree.filter comparer predicate tree)

    member m.ForAll predicate =
        MapTree.forall predicate tree 

    member m.Fold f acc =
        MapTree.foldBack f tree acc

    member m.FoldSection (lo:'Key) (hi:'Key) f (acc:'z) =
        MapTree.foldSection comparer lo hi f tree acc 

    member m.Iterate f =
        MapTree.iter f tree

    member m.MapRange f =
        new Map<'Key, 'b>(comparer, MapTree.map f tree)

    member m.Map f =
        new Map<'Key, 'b>(comparer, MapTree.mapi f tree)

    member m.Partition predicate : Map<'Key, 'Value> * Map<'Key, 'Value> = 
        let r1, r2 = MapTree.partition comparer predicate tree
        new Map<'Key, 'Value>(comparer, r1), new Map<'Key, 'Value>(comparer, r2)

    member m.Count =
        MapTree.size tree

    member m.ContainsKey key = 
#if TRACE_SETS_AND_MAPS
        MapTree.report()
        MapTree.numLookups <- MapTree.numLookups + 1
        MapTree.totalSizeOnMapLookup <- MapTree.totalSizeOnMapLookup + float (MapTree.size tree)
#endif
        MapTree.mem comparer key tree

    member m.Remove key = 
        new Map<'Key, 'Value>(comparer, MapTree.remove comparer key tree)

    member m.TryGetValue(key, [<System.Runtime.InteropServices.Out>] value: byref<'Value>) = 
        MapTree.tryGetValue comparer key &value tree

    member m.TryFind key = 
#if TRACE_SETS_AND_MAPS
        MapTree.report()
        MapTree.numLookups <- MapTree.numLookups + 1
        MapTree.totalSizeOnMapLookup <- MapTree.totalSizeOnMapLookup + float (MapTree.size tree)
#endif
        MapTree.tryFind comparer key tree

    member m.ToList() =
        MapTree.toList tree

    member m.ToArray() =
        MapTree.toArray tree

    static member ofList l : Map<'Key, 'Value> = 
       let comparer = LanguagePrimitives.FastGenericComparer<'Key> 
       new Map<_, _>(comparer, MapTree.ofList comparer l)

    member this.ComputeHashCode() = 
        let combineHash x y = (x <<< 1) + y + 631 
        let mutable res = 0
        for (KeyValue(x, y)) in this do
            res <- combineHash res (hash x)
            res <- combineHash res (Unchecked.hash y)
        res

    override this.Equals that = 
        match that with 
        | :? Map<'Key, 'Value> as that -> 
            use e1 = (this :> seq<_>).GetEnumerator() 
            use e2 = (that :> seq<_>).GetEnumerator() 
            let rec loop () = 
                let m1 = e1.MoveNext() 
                let m2 = e2.MoveNext()
                (m1 = m2) && (not m1 || 
                                 (let e1c, e2c = e1.Current, e2.Current
                                  ((e1c.Key = e2c.Key) && (Unchecked.equals e1c.Value e2c.Value) && loop())))
            loop()
        | _ -> false

    override this.GetHashCode() = this.ComputeHashCode()

    interface IEnumerable<KeyValuePair<'Key, 'Value>> with
        member __.GetEnumerator() = MapTree.mkIEnumerator tree

    interface System.Collections.IEnumerable with
        member __.GetEnumerator() = (MapTree.mkIEnumerator tree :> System.Collections.IEnumerator)

    interface IDictionary<'Key, 'Value> with 
        member m.Item 
            with get x = m.[x] 
            and  set x v = ignore(x, v); raise (NotSupportedException(SR.GetString(SR.mapCannotBeMutated)))

        // REVIEW: this implementation could avoid copying the Values to an array 
        member m.Keys = ([| for kvp in m -> kvp.Key |] :> ICollection<'Key>)

        // REVIEW: this implementation could avoid copying the Values to an array 
        member m.Values = ([| for kvp in m -> kvp.Value |] :> ICollection<'Value>)

        member m.Add(k, v) = ignore(k, v); raise (NotSupportedException(SR.GetString(SR.mapCannotBeMutated)))

        member m.ContainsKey k = m.ContainsKey k

        member m.TryGetValue(k, r) = m.TryGetValue(k, &r) 

        member m.Remove(k : 'Key) = ignore k; (raise (NotSupportedException(SR.GetString(SR.mapCannotBeMutated))) : bool)

    interface ICollection<KeyValuePair<'Key, 'Value>> with 
        member __.Add x = ignore x; raise (NotSupportedException(SR.GetString(SR.mapCannotBeMutated)))

        member __.Clear() = raise (NotSupportedException(SR.GetString(SR.mapCannotBeMutated)))

        member __.Remove x = ignore x; raise (NotSupportedException(SR.GetString(SR.mapCannotBeMutated)))

        member m.Contains x = m.ContainsKey x.Key && Unchecked.equals m.[x.Key] x.Value

        member __.CopyTo(arr, i) = MapTree.copyToArray tree arr i

        member __.IsReadOnly = true

        member m.Count = m.Count

    interface System.IComparable with 
        member m.CompareTo(obj: obj) = 
            match obj with 
            | :? Map<'Key, 'Value>  as m2->
                Seq.compareWith 
                   (fun (kvp1 : KeyValuePair<_, _>) (kvp2 : KeyValuePair<_, _>)-> 
                       let c = comparer.Compare(kvp1.Key, kvp2.Key) in 
                       if c <> 0 then c else Unchecked.compare kvp1.Value kvp2.Value)
                   m m2 
            | _ -> 
                invalidArg "obj" (SR.GetString(SR.notComparable))

    interface IReadOnlyCollection<KeyValuePair<'Key, 'Value>> with
        member m.Count = m.Count

    interface IReadOnlyDictionary<'Key, 'Value> with

        member m.Item with get key = m.[key]

        member m.Keys = seq { for kvp in m -> kvp.Key }

        member m.TryGetValue(key, value: byref<'Value>) = m.TryGetValue(key, &value) 

        member m.Values = seq { for kvp in m -> kvp.Value }

        member m.ContainsKey key = m.ContainsKey key

    override x.ToString() = 
        match List.ofSeq (Seq.truncate 4 x) with 
        | [] -> "map []"
        | [KeyValue h1] ->
            let txt1 = LanguagePrimitives.anyToStringShowingNull h1
            StringBuilder().Append("map [").Append(txt1).Append("]").ToString()
        | [KeyValue h1; KeyValue h2] ->
            let txt1 = LanguagePrimitives.anyToStringShowingNull h1
            let txt2 = LanguagePrimitives.anyToStringShowingNull h2
            StringBuilder().Append("map [").Append(txt1).Append("; ").Append(txt2).Append("]").ToString()
        | [KeyValue h1; KeyValue h2; KeyValue h3] ->
            let txt1 = LanguagePrimitives.anyToStringShowingNull h1
            let txt2 = LanguagePrimitives.anyToStringShowingNull h2
            let txt3 = LanguagePrimitives.anyToStringShowingNull h3
            StringBuilder().Append("map [").Append(txt1).Append("; ").Append(txt2).Append("; ").Append(txt3).Append("]").ToString()
        | KeyValue h1 :: KeyValue h2 :: KeyValue h3 :: _ ->
            let txt1 = LanguagePrimitives.anyToStringShowingNull h1
            let txt2 = LanguagePrimitives.anyToStringShowingNull h2
            let txt3 = LanguagePrimitives.anyToStringShowingNull h3
            StringBuilder().Append("map [").Append(txt1).Append("; ").Append(txt2).Append("; ").Append(txt3).Append("; ... ]").ToString() 

and
    [<Sealed>]
    MapDebugView<'Key, 'Value when 'Key : comparison>(v: Map<'Key, 'Value>)  = 

        [<DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>]
        member x.Items =
            v |> Seq.truncate 10000 |> Seq.map KeyValuePairDebugFriendly |> Seq.toArray

and
    [<DebuggerDisplay("{keyValue.Value}", Name = "[{keyValue.Key}]", Type = "")>]
    KeyValuePairDebugFriendly<'Key, 'Value>(keyValue : KeyValuePair<'Key, 'Value>) =

        [<DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>]
        member x.KeyValue = keyValue

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Map = 

    [<CompiledName("IsEmpty")>]
    let isEmpty (table: Map<_, _>) =
        table.IsEmpty

    [<CompiledName("Add")>]
    let add key value (table: Map<_, _>) =
        table.Add (key, value)

    [<CompiledName("Find")>]
    let find key (table: Map<_, _>) =
        table.[key]

    [<CompiledName("TryFind")>]
    let tryFind key (table: Map<_, _>) =
        table.TryFind key

    [<CompiledName("Remove")>]
    let remove key (table: Map<_, _>) =
        table.Remove key

    [<CompiledName("ContainsKey")>]
    let containsKey key (table: Map<_, _>) =
        table.ContainsKey key

    [<CompiledName("Iterate")>]
    let iter action (table: Map<_, _>) =
        table.Iterate action

    [<CompiledName("TryPick")>]
    let tryPick chooser (table: Map<_, _>) =
        table.TryPick chooser

    [<CompiledName("Pick")>]
    let pick chooser (table: Map<_, _>) =
        match tryPick chooser table with
        | None -> raise (KeyNotFoundException())
        | Some res -> res

    [<CompiledName("Exists")>]
    let exists predicate (table: Map<_, _>) =
        table.Exists predicate

    [<CompiledName("Filter")>]
    let filter predicate (table: Map<_, _>) =
        table.Filter predicate

    [<CompiledName("Partition")>]
    let partition predicate (table: Map<_, _>) =
        table.Partition predicate

    [<CompiledName("ForAll")>]
    let forall predicate (table: Map<_, _>) =
        table.ForAll predicate

    [<CompiledName("Map")>]
    let map mapping (table: Map<_, _>) =
        table.Map mapping

    [<CompiledName("Fold")>]
    let fold<'Key, 'T, 'State when 'Key : comparison> folder (state:'State) (table: Map<'Key, 'T>) =
        MapTree.fold folder state table.Tree

    [<CompiledName("FoldBack")>]
    let foldBack<'Key, 'T, 'State  when 'Key : comparison> folder (table: Map<'Key, 'T>) (state:'State) =
        MapTree.foldBack  folder table.Tree state

    [<CompiledName("ToSeq")>]
    let toSeq (table: Map<_, _>) =
        table |> Seq.map (fun kvp -> kvp.Key, kvp.Value)

    [<CompiledName("FindKey")>]
    let findKey predicate (table : Map<_, _>) =
        table |> toSeq |> Seq.pick (fun (k, v) -> if predicate k v then Some k else None)

    [<CompiledName("TryFindKey")>]
    let tryFindKey predicate (table : Map<_, _>) =
        table |> toSeq |> Seq.tryPick (fun (k, v) -> if predicate k v then Some k else None)

    [<CompiledName("OfList")>]
    let ofList (elements: ('Key * 'Value) list) =
        Map<_, _>.ofList elements

    [<CompiledName("OfSeq")>]
    let ofSeq elements =
        Map<_, _>.Create elements

    [<CompiledName("OfArray")>]
    let ofArray (elements: ('Key * 'Value) array) = 
       let comparer = LanguagePrimitives.FastGenericComparer<'Key> 
       new Map<_, _>(comparer, MapTree.ofArray comparer elements)

    [<CompiledName("ToList")>]
    let toList (table: Map<_, _>) =
        table.ToList()

    [<CompiledName("ToArray")>]
    let toArray (table: Map<_, _>) =
        table.ToArray()

    [<CompiledName("Empty")>]
    let empty<'Key, 'Value  when 'Key : comparison> =
        Map<'Key, 'Value>.Empty

    [<CompiledName("Count")>]
    let count (table: Map<_, _>) =
        table.Count
