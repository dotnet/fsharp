// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Collections

    open System
    open System.Collections.Generic
    open System.Diagnostics
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators

    [<NoEquality; NoComparison>]
    type MapTree<'Key,'Value when 'Key : comparison > = 
        | MapNode of 'Key * 'Value * MapTree<'Key,'Value> *  MapTree<'Key,'Value> * int

    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module MapTree = 
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
               System.Console.WriteLine("#MapOne = {0}, #MapNode = {1}, #Add = {2}, #Remove = {3}, #Unions = {4}, #Lookups = {5}, avMapTreeSizeOnNodeCreation = {6}, avMapSizeOnCreation = {7}, avMapSizeOnLookup = {8}",numOnes,numNodes,numAdds,numRemoves,numUnions,numLookups,(totalSizeOnNodeCreation / float (numNodes + numOnes)),(totalSizeOnMapAdd / float numAdds),(totalSizeOnMapLookup / float numLookups))
               System.Console.WriteLine("#largestMapSize = {0}, largestMapStackTrace = {1}",largestMapSize, largestMapStackTrace)

        let MapOne n = 
            report(); 
            numOnes <- numOnes + 1; 
            totalSizeOnNodeCreation <- totalSizeOnNodeCreation + 1.0; 
            MapTree.MapOne n

        let MapNode (x,l,v,r,h) = 
            report(); 
            numNodes <- numNodes + 1; 
            let n = MapTree.MapNode(x,l,v,r,h)
            totalSizeOnNodeCreation <- totalSizeOnNodeCreation + float (size n); 
            n
    #endif

        [<Sealed; AbstractClass>]
        type Constants<'Key, 'Value when 'Key : comparison> private () = 
            static let empty = MapNode(Unchecked.defaultof<'Key>, Unchecked.defaultof<'Value>, Unchecked.defaultof<MapTree<'Key,'Value>>, Unchecked.defaultof<MapTree<'Key,'Value>>, 0)
            static member Empty = empty

        let inline size (MapNode(_,_,_,_,s)) = s

        let inline isEmpty (MapNode(_,_,_,_,s)) = s = 0

        let inline mk l k v r =
            MapNode (k,v,l,r, size l + size r + 1)

        let inline mkLeaf k v =
            MapNode (k, v, Constants.Empty, Constants.Empty, 1)

        let private rebalanceRight l k v (MapNode(rk,rv,rl,rr,_)) =
            (* one of the nodes must have height > height t1 + 1 *)
            if size rl > size l then  (* balance left: combination *)
                match rl with 
                | MapNode(rlk,rlv,rll,rlr,_) -> mk (mk l k v rll) rlk rlv (mk rlr rk rv rr) 
            else (* rotate left *)
                mk (mk l k v rl) rk rv rr

        let private rebalanceLeft (MapNode(lk,lv,ll,lr,_)) k v r =
            (* one of the nodes must have height > height t2 + 1 *)
            if size lr > size r then 
                (* balance right: combination *)
                match lr with 
                | MapNode(lrk,lrv,lrl,lrr,_) -> mk (mk ll lk lv lrl) lrk lrv (mk lrr k v r)
            else
                mk ll lk lv (mk lr k v r)

        let inline rebalance l k v r =
            let ls, rs = size l, size r 
            if   (rs >>> 1) > ls then rebalanceRight l k v r 
            elif (ls >>> 1) > rs then rebalanceLeft  l k v r
            else MapNode (k,v,l,r, ls+rs+1)

        let rec add (comparer:IComparer<'Key>) k v (MapNode(k2,v2,l,r,s)) = 
            if s = 0 then  mkLeaf k v
            else
                let c = comparer.Compare(k,k2) 
                if c < 0 then
                    let l' = add comparer k v l
                    let l's, rs = size l', size r 
                    if (l's >>> 1) > rs then
                        rebalanceLeft  l' k2 v2 r
                    else
                        MapNode (k2,v2,l',r, l's+rs+1)
                elif c > 0 then
                    let r' = add comparer k v r
                    let ls, r's = size l, size r' 
                    if (r's >>> 1) > ls then
                        rebalanceRight l k2 v2 r' 
                    else
                        MapNode (k2,v2,l,r', ls+r's+1)
                else
                    MapNode(k,v,l,r,s)

        let inline private findImpl (comparer:IComparer<'Key>) k m found notfound =
            let rec loop (MapNode(k2,_,_,_,s) as m) =
                if s = 0 then notfound ()
                else
                    let c = comparer.Compare(k,k2) 
                    if   c < 0 then match m with MapNode(_,_,l,_,_) -> loop l
                    elif c > 0 then match m with MapNode(_,_,_,r,_) -> loop r 
                    else match m with MapNode(_,v2,_,_,_) -> found v2
            loop m

        let find    comparer k m = findImpl comparer k m id   (fun () -> raise (KeyNotFoundException ()))
        let tryFind comparer k m = findImpl comparer k m Some (fun () -> None)

        let partition1 (comparer:IComparer<'Key>) (f:OptimizedClosures.FSharpFunc<_,_,_>) k v (acc1,acc2) = 
            if f.Invoke(k, v) then (add comparer k v acc1,acc2) else (acc1,add comparer k v acc2) 
        
        let rec partitionAux (comparer:IComparer<'Key>) (f:OptimizedClosures.FSharpFunc<_,_,_>) s acc = 
            match s with 
            | MapNode(_,_,_,_,0) -> acc
            | MapNode(k,v,l,r,_) -> 
                let acc = partitionAux comparer f r acc 
                let acc = partition1 comparer f k v acc
                partitionAux comparer f l acc

        let partition (comparer:IComparer<'Key>) f s = partitionAux comparer (OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)) s (Constants.Empty,Constants.Empty)

        let filter1 (comparer:IComparer<'Key>) (f:OptimizedClosures.FSharpFunc<_,_,_>) k v acc = if f.Invoke(k, v) then add comparer k v acc else acc 

        let rec filterAux (comparer:IComparer<'Key>) (f:OptimizedClosures.FSharpFunc<_,_,_>) s acc = 
            match s with 
            | MapNode(_,_,_,_,0) -> acc
            | MapNode(k,v,l,r,_) ->
                let acc = filterAux comparer f l acc
                let acc = filter1 comparer f k v acc
                filterAux comparer f r acc

        let filter (comparer:IComparer<'Key>) f s = filterAux comparer (OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)) s Constants.Empty

        let rec spliceOutSuccessor m = 
            match m with 
            | MapNode(_,_,_,_,0) -> failwith "internal error: Map.spliceOutSuccessor"
            | MapNode(k2,v2,l,r,_) ->
                match l with 
                | MapNode(_,_,_,_,0) -> k2,v2,r
                | _ -> let k3,v3,l' = spliceOutSuccessor l in k3,v3,mk l' k2 v2 r

        let rec remove (comparer:IComparer<'Key>) k m = 
            match m with 
            | MapNode(_,_,_,_,0) -> Constants.Empty
            | MapNode(k2,v2,l,r,_) -> 
                let c = comparer.Compare(k,k2) 
                if c < 0 then rebalance (remove comparer k l) k2 v2 r
                elif c = 0 then 
                  match l,r with 
                  | MapNode(_,_,_,_,0),_ -> r
                  | _,MapNode(_,_,_,_,0) -> l
                  | _ -> 
                      let sk,sv,r' = spliceOutSuccessor r 
                      mk l sk sv r'
                else rebalance l k2 v2 (remove comparer k r) 

        let rec mem (comparer:IComparer<'Key>) k m = 
            match m with 
            | MapNode(_,_,_,_,0) -> false
            | MapNode(k2,_,l,r,_) -> 
                let c = comparer.Compare(k,k2) 
                if c < 0 then mem comparer k l
                else (c = 0 || mem comparer k r)

        let rec iterOpt (f:OptimizedClosures.FSharpFunc<_,_,_>) m =
            match m with 
            | MapNode(_,_,_,_,0) -> ()
            | MapNode(k2,v2,l,r,_) -> iterOpt f l; f.Invoke(k2, v2); iterOpt f r

        let iter f m = iterOpt (OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)) m

        let rec tryPickOpt (f:OptimizedClosures.FSharpFunc<_,_,_>) m =
            match m with 
            | MapNode(_,_,_,_,0) -> None
            | MapNode(k2,v2,l,r,_) -> 
                match tryPickOpt f l with 
                | Some _ as res -> res 
                | None -> 
                match f.Invoke(k2, v2) with 
                | Some _ as res -> res 
                | None -> 
                tryPickOpt f r

        let tryPick f m = tryPickOpt (OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)) m

        let rec existsOpt (f:OptimizedClosures.FSharpFunc<_,_,_>) m = 
            match m with 
            | MapNode(_,_,_,_,0) -> false
            | MapNode(k2,v2,l,r,_) -> existsOpt f l || f.Invoke(k2, v2) || existsOpt f r

        let exists f m = existsOpt (OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)) m

        let rec forallOpt (f:OptimizedClosures.FSharpFunc<_,_,_>) m = 
            match m with 
            | MapNode(_,_,_,_,0) -> true
            | MapNode(k2,v2,l,r,_) -> forallOpt f l && f.Invoke(k2, v2) && forallOpt f r

        let forall f m = forallOpt (OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)) m

        let rec map f m = 
            match m with 
            | MapNode(_,_,_,_,0) -> Constants.Empty
            | MapNode(k,v,l,r,h) -> 
                let l2 = map f l 
                let v2 = f v 
                let r2 = map f r 
                MapNode(k,v2,l2, r2,h)

        let rec mapiOpt (f:OptimizedClosures.FSharpFunc<_,_,_>) m = 
            match m with
            | MapNode(_,_,_,_,0) -> Constants.Empty
            | MapNode(k,v,l,r,h) -> 
                let l2 = mapiOpt f l 
                let v2 = f.Invoke(k, v) 
                let r2 = mapiOpt f r 
                MapNode(k,v2, l2, r2,h)

        let mapi f m = mapiOpt (OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)) m

        let rec foldBackOpt (f:OptimizedClosures.FSharpFunc<_,_,_,_>) m x = 
            match m with 
            | MapNode(_,_,_,_,0) -> x
            | MapNode(k,v,l,r,_) -> 
                let x = foldBackOpt f r x
                let x = f.Invoke(k,v,x)
                foldBackOpt f l x

        let foldBack f m x = foldBackOpt (OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt(f)) m x

        let rec foldOpt (f:OptimizedClosures.FSharpFunc<_,_,_,_>) x m  = 
            match m with 
            | MapNode(_,_,_,_,0) -> x
            | MapNode(k,v,l,r,_) -> 
                let x = foldOpt f x l
                let x = f.Invoke(x,k,v)
                foldOpt f x r

        let fold f x m = foldOpt (OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt(f)) x m

        let foldSectionOpt (comparer:IComparer<'Key>) lo hi (f:OptimizedClosures.FSharpFunc<_,_,_,_>) m x =
            let rec foldFromTo (f:OptimizedClosures.FSharpFunc<_,_,_,_>) m x = 
                match m with 
                | MapNode(_,_,_,_,0) -> x
                | MapNode(k,v,l,r,_) ->
                    let cLoKey = comparer.Compare(lo,k)
                    let cKeyHi = comparer.Compare(k,hi)
                    let x = if cLoKey < 0                 then foldFromTo f l x else x
                    let x = if cLoKey <= 0 && cKeyHi <= 0 then f.Invoke(k, v, x) else x
                    let x = if cKeyHi < 0                 then foldFromTo f r x else x
                    x
           
            if comparer.Compare(lo,hi) = 1 then x else foldFromTo f m x

        let foldSection (comparer:IComparer<'Key>) lo hi f m x =
            foldSectionOpt comparer lo hi (OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt(f)) m x

        // create a mapping function which indexes the array, but with duplicate values removed
        let private getLatestAccessor (comparer:IComparer<'Key>) (keys:IReadOnlyList<'Key>) (array:IReadOnlyList<'T>) =
            let rec getFirstDuplicateKey i =
                if i >= array.Count-1 then None
                elif comparer.Compare (keys.[i], keys.[i+1]) = 0 then Some i
                else getFirstDuplicateKey (i+1)

            match getFirstDuplicateKey 0 with
            | None -> (fun i -> array.[i]), array.Count
            | Some idx ->
                let indexes = ResizeArray (array.Count-idx)
                indexes.Add (idx+1)
                for i = idx+2 to array.Count-1 do
                    if comparer.Compare (keys.[i-1], keys.[i]) = 0 then
                        indexes.[indexes.Count-1] <- i
                    else
                        indexes.Add i
                (fun i -> if i < idx then array.[i] else array.[indexes.[i-idx]]), idx+indexes.Count

        let constructViaArray comparer (data:seq<'Key*'Value>) =
            let array = data |> Seq.toArray
            if array.Length = 0 then Constants.Empty
            else
                let keys = array |> Array.map fst

                Microsoft.FSharp.Primitives.Basics.Array.stableSortWithKeysAndComparer comparer array keys

                let getKV, count =
                    getLatestAccessor comparer keys array

                let rec loop lower upper =
                    assert (lower <= upper)
                    let mid = lower + (upper-lower)/2
                    let k,v = getKV mid
                    if mid = upper then
                        mkLeaf k v
                    else
                        let right = loop (mid+1) upper
                        if mid = lower then
                            mk Constants.Empty k v right
                        else
                            let left = loop lower (mid-1)
                            mk left k v right

                loop 0 (count-1)

        let toList m = 
            let rec loop m acc = 
                match m with 
                | MapNode(_,_,_,_,0) -> acc
                | MapNode(k,v,l,r,_) -> loop l ((k,v)::loop r acc)
            loop m []

        let toArray m = m |> toList |> Array.ofList

        [<Literal>]
        let largeObjectHeapBytes = 85000

        let maxInitializationObjectCount<'Key, 'Value> () =
            largeObjectHeapBytes * 10 / 9 / sizeof<'Key*'Value> / 2

        let ofList comparer (l:list<'Key*'Value>) =
            if l |> List.isEmpty then Constants.Empty
            else
                let chunk = ResizeArray ()
                let maxCount = maxInitializationObjectCount<'Key, 'Value> ()
                let rec populate x =
                    match x with
                    | [] -> x
                    | hd::tl ->
                        chunk.Add hd
                        if chunk.Count = maxCount then tl
                        else populate tl
                let remainder = populate l
                let chunkTree = constructViaArray comparer chunk
                remainder |> List.fold (fun acc (k,v) -> add comparer k v acc) chunkTree

        let ofSeqlImpl comparer (e:IEnumerator<'Key*'Value>) = 
            if not (e.MoveNext()) then Constants.Empty
            else
                let chunk = ResizeArray ()
                let maxCount = maxInitializationObjectCount<'Key, 'Value> ()
                let rec populate () =
                    chunk.Add e.Current
                    if chunk.Count = maxCount then true
                    elif e.MoveNext () then populate ()
                    else false

                let more = populate ()
                let chunkTree = constructViaArray comparer chunk

                if not more then
                    chunkTree
                else
                    let rec addRemainder acc =
                        if e.MoveNext () then
                            let x, y = e.Current
                            addRemainder (add comparer x y acc)
                        else
                            acc
                    addRemainder chunkTree
          
        let ofArray comparer (arr : array<_>) =
            constructViaArray comparer arr

        let ofSeq comparer (c : seq<'Key * 'T>) =
            match c with 
            | :? array<'Key * 'T> as xs -> ofArray comparer xs
            | :? list<'Key * 'T> as xs -> ofList comparer xs
            | _ -> 
                use ie = c.GetEnumerator()
                ofSeqlImpl comparer ie 

          
        let copyToArray s (arr: _[]) i =
            let j = ref i 
            s |> iter (fun x y -> arr.[!j] <- KeyValuePair(x,y); j := !j + 1)


        /// Imperative left-to-right iterators.
        [<NoEquality; NoComparison>]
        type MapIterator<'Key,'Value when 'Key : comparison > = 
             { /// invariant: always collapseLHS result 
               mutable stack: MapTree<'Key,'Value> list;  
               /// true when MoveNext has been called   
               mutable started : bool }

        // collapseLHS:
        // a) Always returns either [] or a list starting with MapOne.
        // b) The "fringe" of the set stack is unchanged. 
        let rec collapseLHS stack =
            match stack with
            | []                           -> []
            | MapNode(_,_,_,_,0) :: rest -> collapseLHS rest
            | MapNode(_,_,MapNode(_,_,_,_,0),MapNode(_,_,_,_,0),_) :: _ -> stack
            | MapNode(k,v,l,r,_) :: rest -> collapseLHS (l :: (mkLeaf k v) :: r :: rest)
          
        let mkIterator s = { stack = collapseLHS [s]; started = false }

        let notStarted() = raise (InvalidOperationException(SR.GetString(SR.enumerationNotStarted)))
        let alreadyFinished() = raise (InvalidOperationException(SR.GetString(SR.enumerationAlreadyFinished)))

        let current i =
            if i.started then
                match i.stack with
                  | MapNode(k,v,MapNode(_,_,_,_,0),MapNode(_,_,_,_,0),_) :: _ -> new KeyValuePair<_,_>(k,v)
                  | []            -> alreadyFinished()
                  | _             -> failwith "Please report error: Map iterator, unexpected stack for current"
            else
                notStarted()

        let rec moveNext i =
          if i.started then
            match i.stack with
              | MapNode(_,_,MapNode(_,_,_,_,0),MapNode(_,_,_,_,0),_) :: rest ->
                i.stack <- collapseLHS rest
                not i.stack.IsEmpty
              | [] -> false
              | _ -> failwith "Please report error: Map iterator, unexpected stack for moveNext"
          else
              i.started <- true  (* The first call to MoveNext "starts" the enumeration. *)
              not i.stack.IsEmpty

        let mkIEnumerator s = 
          let i = ref (mkIterator s) 
          { new IEnumerator<_> with 
                member __.Current = current !i
            interface System.Collections.IEnumerator with
                member __.Current = box (current !i)
                member __.MoveNext() = moveNext !i
                member __.Reset() = i :=  mkIterator s
            interface System.IDisposable with 
                member __.Dispose() = ()}



    [<System.Diagnostics.DebuggerTypeProxy(typedefof<MapDebugView<_,_>>)>]
    [<System.Diagnostics.DebuggerDisplay("Count = {Count}")>]
    [<Sealed>]
    [<CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")>]
    [<CompiledName("FSharpMap`2")>]
    type Map<[<EqualityConditionalOn>]'Key,[<EqualityConditionalOn;ComparisonConditionalOn>]'Value when 'Key : comparison >(comparer:IComparer<'Key>, tree: MapTree<'Key,'Value>) =

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
            new Map<'Key,'Value>(comparer,MapTree.Constants.Empty)

#if !FX_NO_BINARY_SERIALIZATION
        [<System.Runtime.Serialization.OnSerializingAttribute>]
        member __.OnSerializing(context: System.Runtime.Serialization.StreamingContext) =
            ignore(context)
            serializedData <- MapTree.toArray tree |> Array.map (fun (k,v) -> KeyValuePair(k,v))

        // Do not set this to null, since concurrent threads may also be serializing the data
        //[<System.Runtime.Serialization.OnSerializedAttribute>]
        //member __.OnSerialized(context: System.Runtime.Serialization.StreamingContext) =
        //    serializedData <- null

        [<System.Runtime.Serialization.OnDeserializedAttribute>]
        member __.OnDeserialized(context: System.Runtime.Serialization.StreamingContext) =
            ignore(context)
            comparer <- LanguagePrimitives.FastGenericComparer<'Key>
            tree <- serializedData |> Array.map (fun (KeyValue(k,v)) -> (k,v)) |> MapTree.ofArray comparer 
            serializedData <- null
#endif

        static member Empty : Map<'Key,'Value> = empty

        static member Create(ie : IEnumerable<_>) : Map<'Key,'Value> = 
           let comparer = LanguagePrimitives.FastGenericComparer<'Key> 
           new Map<_,_>(comparer,MapTree.ofSeq comparer ie)
    
        static member Create() : Map<'Key,'Value> = empty

        new(elements : seq<_>) = 
           let comparer = LanguagePrimitives.FastGenericComparer<'Key> 
           new Map<_,_>(comparer,MapTree.ofSeq comparer elements)
    
        [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
        member internal m.Comparer = comparer
        //[<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
        member internal m.Tree = tree
        member m.Add(key,value) : Map<'Key,'Value> = 
#if TRACE_SETS_AND_MAPS
            MapTree.report()
            MapTree.numAdds <- MapTree.numAdds + 1
            let size = MapTree.size m.Tree + 1
            MapTree.totalSizeOnMapAdd <- MapTree.totalSizeOnMapAdd + float size
            if size > MapTree.largestMapSize then 
               MapTree.largestMapSize <- size
               MapTree.largestMapStackTrace <- System.Diagnostics.StackTrace().ToString()
#endif
            new Map<'Key,'Value>(comparer,MapTree.add comparer key value tree)

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
        member m.TryPick(f) = MapTree.tryPick f tree 
        member m.Exists(f) = MapTree.exists f tree 
        member m.Filter(f)  : Map<'Key,'Value> = new Map<'Key,'Value>(comparer ,MapTree.filter comparer f tree)
        member m.ForAll(f) = MapTree.forall f tree 
        member m.Fold f acc = MapTree.foldBack f tree acc

        member m.FoldSection (lo:'Key) (hi:'Key) f (acc:'z) = MapTree.foldSection comparer lo hi f tree acc 

        member m.Iterate f = MapTree.iter f tree

        member m.MapRange f  = new Map<'Key,'b>(comparer,MapTree.map f tree)

        member m.Map f  = new Map<'Key,'b>(comparer,MapTree.mapi f tree)

        member m.Partition(f)  : Map<'Key,'Value> * Map<'Key,'Value> = 
            let r1,r2 = MapTree.partition comparer f tree  in 
            new Map<'Key,'Value>(comparer,r1), new Map<'Key,'Value>(comparer,r2)

        member m.Count = MapTree.size tree

        member m.ContainsKey(key) = 
#if TRACE_SETS_AND_MAPS
            MapTree.report()
            MapTree.numLookups <- MapTree.numLookups + 1
            MapTree.totalSizeOnMapLookup <- MapTree.totalSizeOnMapLookup + float (MapTree.size tree)
#endif
            MapTree.mem comparer key tree

        member m.Remove(key)  : Map<'Key,'Value> = 
            new Map<'Key,'Value>(comparer,MapTree.remove comparer key tree)

        member m.TryFind(key) = 
#if TRACE_SETS_AND_MAPS
            MapTree.report()
            MapTree.numLookups <- MapTree.numLookups + 1
            MapTree.totalSizeOnMapLookup <- MapTree.totalSizeOnMapLookup + float (MapTree.size tree)
#endif
            MapTree.tryFind comparer key tree

        member m.ToList() = MapTree.toList tree

        member m.ToArray() = MapTree.toArray tree

        static member ofList(l) : Map<'Key,'Value> = 
           let comparer = LanguagePrimitives.FastGenericComparer<'Key> 
           new Map<_,_>(comparer,MapTree.ofList comparer l)
           
        member this.ComputeHashCode() = 
            let combineHash x y = (x <<< 1) + y + 631 
            let mutable res = 0
            for (KeyValue(x,y)) in this do
                res <- combineHash res (hash x)
                res <- combineHash res (Unchecked.hash y)
            abs res

        override this.Equals(that) = 
            match that with 
            | :? Map<'Key,'Value> as that -> 
                use e1 = (this :> seq<_>).GetEnumerator() 
                use e2 = (that :> seq<_>).GetEnumerator() 
                let rec loop () = 
                    let m1 = e1.MoveNext() 
                    let m2 = e2.MoveNext()
                    (m1 = m2) && (not m1 || let e1c, e2c = e1.Current, e2.Current in ((e1c.Key = e2c.Key) && (Unchecked.equals e1c.Value e2c.Value) && loop()))
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
                and  set x v = ignore(x,v); raise (NotSupportedException(SR.GetString(SR.mapCannotBeMutated)))

            // REVIEW: this implementation could avoid copying the Values to an array    
            member s.Keys = ([| for kvp in s -> kvp.Key |] :> ICollection<'Key>)

            // REVIEW: this implementation could avoid copying the Values to an array    
            member s.Values = ([| for kvp in s -> kvp.Value |] :> ICollection<'Value>)

            member s.Add(k,v) = ignore(k,v); raise (NotSupportedException(SR.GetString(SR.mapCannotBeMutated)))
            member s.ContainsKey(k) = s.ContainsKey(k)
            member s.TryGetValue(k,r) = if s.ContainsKey(k) then (r <- s.[k]; true) else false
            member s.Remove(k : 'Key) = ignore(k); (raise (NotSupportedException(SR.GetString(SR.mapCannotBeMutated))) : bool)

        interface ICollection<KeyValuePair<'Key, 'Value>> with 
            member __.Add(x) = ignore(x); raise (NotSupportedException(SR.GetString(SR.mapCannotBeMutated)));
            member __.Clear() = raise (NotSupportedException(SR.GetString(SR.mapCannotBeMutated)));
            member __.Remove(x) = ignore(x); raise (NotSupportedException(SR.GetString(SR.mapCannotBeMutated)));
            member s.Contains(x) = s.ContainsKey(x.Key) && Unchecked.equals s.[x.Key] x.Value
            member __.CopyTo(arr,i) = MapTree.copyToArray tree arr i
            member s.IsReadOnly = true
            member s.Count = s.Count

        interface System.IComparable with 
            member m.CompareTo(obj: obj) = 
                match obj with 
                | :? Map<'Key,'Value>  as m2->
                    Seq.compareWith 
                       (fun (kvp1 : KeyValuePair<_,_>) (kvp2 : KeyValuePair<_,_>)-> 
                           let c = comparer.Compare(kvp1.Key,kvp2.Key) in 
                           if c <> 0 then c else Unchecked.compare kvp1.Value kvp2.Value)
                       m m2 
                | _ -> 
                    invalidArg "obj" (SR.GetString(SR.notComparable))

        interface IReadOnlyCollection<KeyValuePair<'Key, 'Value>> with
            member s.Count = s.Count

        interface IReadOnlyDictionary<'Key, 'Value> with
            member s.Item with get(key) = s.[key]
            member s.Keys = seq { for kvp in s -> kvp.Key }
            member s.TryGetValue(key, value) = if s.ContainsKey(key) then (value <- s.[key]; true) else false
            member s.Values = seq { for kvp in s -> kvp.Value }
            member s.ContainsKey key = s.ContainsKey key

        override x.ToString() = 
           match List.ofSeq (Seq.truncate 4 x) with 
           | [] -> "map []"
           | [KeyValue h1] -> System.Text.StringBuilder().Append("map [").Append(LanguagePrimitives.anyToStringShowingNull h1).Append("]").ToString()
           | [KeyValue h1;KeyValue h2] -> System.Text.StringBuilder().Append("map [").Append(LanguagePrimitives.anyToStringShowingNull h1).Append("; ").Append(LanguagePrimitives.anyToStringShowingNull h2).Append("]").ToString()
           | [KeyValue h1;KeyValue h2;KeyValue h3] -> System.Text.StringBuilder().Append("map [").Append(LanguagePrimitives.anyToStringShowingNull h1).Append("; ").Append(LanguagePrimitives.anyToStringShowingNull h2).Append("; ").Append(LanguagePrimitives.anyToStringShowingNull h3).Append("]").ToString()
           | KeyValue h1 :: KeyValue h2 :: KeyValue h3 :: _ -> System.Text.StringBuilder().Append("map [").Append(LanguagePrimitives.anyToStringShowingNull h1).Append("; ").Append(LanguagePrimitives.anyToStringShowingNull h2).Append("; ").Append(LanguagePrimitives.anyToStringShowingNull h3).Append("; ... ]").ToString() 

    and
        [<Sealed>]
        MapDebugView<'Key,'Value when 'Key : comparison>(v: Map<'Key,'Value>)  =  

            [<DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>]
            member x.Items =
                v |> Seq.truncate 10000 |> Seq.map KeyValuePairDebugFriendly |> Seq.toArray
    
    and
        [<DebuggerDisplay("{keyValue.Value}", Name = "[{keyValue.Key}]", Type = "")>]
        KeyValuePairDebugFriendly<'Key,'Value>(keyValue : KeyValuePair<'Key, 'Value>) =
        
            [<DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>]
            member x.KeyValue = keyValue
        

namespace Microsoft.FSharp.Collections

    open System
    open System.Diagnostics
    open System.Collections.Generic
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.Operators
    open Microsoft.FSharp.Collections

    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    [<RequireQualifiedAccess>]
    module Map = 

        [<CompiledName("IsEmpty")>]
        let isEmpty (table:Map<_,_>) = table.IsEmpty

        [<CompiledName("Add")>]
        let add key value (table:Map<_,_>) = table.Add(key,value)

        [<CompiledName("Find")>]
        let find key (table:Map<_,_>) = table.[key]

        [<CompiledName("TryFind")>]
        let tryFind key (table:Map<_,_>) = table.TryFind(key)

        [<CompiledName("Remove")>]
        let remove key (table:Map<_,_>) = table.Remove(key)

        [<CompiledName("ContainsKey")>]
        let containsKey key (table:Map<_,_>) = table.ContainsKey(key)

        [<CompiledName("Iterate")>]
        let iter action (table:Map<_,_>) = table.Iterate(action)

        [<CompiledName("TryPick")>]
        let tryPick chooser (table:Map<_,_>) = table.TryPick(chooser)

        [<CompiledName("Pick")>]
        let pick chooser (table:Map<_,_>) = match tryPick chooser table with None -> raise (KeyNotFoundException()) | Some res -> res

        [<CompiledName("Exists")>]
        let exists predicate (table:Map<_,_>) = table.Exists(predicate)

        [<CompiledName("Filter")>]
        let filter predicate (table:Map<_,_>) = table.Filter(predicate)

        [<CompiledName("Partition")>]
        let partition predicate (table:Map<_,_>) = table.Partition(predicate)

        [<CompiledName("ForAll")>]
        let forall predicate (table:Map<_,_>) = table.ForAll(predicate)

        let mapRange f (m:Map<_,_>) = m.MapRange(f)

        [<CompiledName("Map")>]
        let map mapping (table:Map<_,_>) = table.Map(mapping)

        [<CompiledName("Fold")>]
        let fold<'Key,'T,'State when 'Key : comparison> folder (state:'State) (table:Map<'Key,'T>) = MapTree.fold folder state table.Tree

        [<CompiledName("FoldBack")>]
        let foldBack<'Key,'T,'State  when 'Key : comparison> folder (table:Map<'Key,'T>) (state:'State) =  MapTree.foldBack  folder table.Tree state
        
        [<CompiledName("ToSeq")>]
        let toSeq (table:Map<_,_>) = table |> Seq.map (fun kvp -> kvp.Key, kvp.Value)

        [<CompiledName("FindKey")>]
        let findKey predicate (table : Map<_,_>) = table |> toSeq |> Seq.pick (fun (k,v) -> if predicate k v then Some(k) else None)

        [<CompiledName("TryFindKey")>]
        let tryFindKey predicate (table : Map<_,_>) = table |> toSeq |> Seq.tryPick (fun (k,v) -> if predicate k v then Some(k) else None)

        [<CompiledName("OfList")>]
        let ofList (elements: ('Key * 'Value) list) = Map<_,_>.ofList(elements)

        [<CompiledName("OfSeq")>]
        let ofSeq elements = Map<_,_>.Create(elements)

        [<CompiledName("OfArray")>]
        let ofArray (elements: ('Key * 'Value) array) = 
           let comparer = LanguagePrimitives.FastGenericComparer<'Key> 
           new Map<_,_>(comparer,MapTree.ofArray comparer elements)

        [<CompiledName("ToList")>]
        let toList (table:Map<_,_>) = table.ToList()

        [<CompiledName("ToArray")>]
        let toArray (table:Map<_,_>) = table.ToArray()

        [<CompiledName("Empty")>]
        let empty<'Key,'Value  when 'Key : comparison> = Map<'Key,'Value>.Empty

        [<CompiledName("Count")>]
        let count (table:Map<_,_>) = table.Count