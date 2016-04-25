// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Collections

    open System
    open System.Collections.Generic
    open System.Diagnostics
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
    open Microsoft.FSharp.Core.Operators
    open Microsoft.FSharp.Collections
    open Microsoft.FSharp.Primitives.Basics

    [<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]
    [<NoEquality; NoComparison>]
    type MapTree<'Key,'Value when 'Key : comparison > = 
        | MapEmpty 
        | MapOne of 'Key * 'Value
        | MapNode of 'Key * 'Value * MapTree<'Key,'Value> *  MapTree<'Key,'Value> * int
            // REVIEW: performance rumour has it that the data held in MapNode and MapOne should be
            // exactly one cache line. It is currently ~7 and 4 words respectively. 

    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module MapTree = 

        let rec sizeAux acc m = 
            match m with  
            | MapEmpty -> acc
            | MapOne _ -> acc + 1
            | MapNode(_,_,l,r,_) -> sizeAux (sizeAux (acc+1) l) r 

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

        let empty = MapEmpty 

        let height  = function
          | MapEmpty -> 0
          | MapOne _ -> 1
          | MapNode(_,_,_,_,h) -> h

        let isEmpty m = 
            match m with 
            | MapEmpty -> true
            | _ -> false

        let mk l k v r = 
            match l,r with 
            | MapEmpty,MapEmpty -> MapOne(k,v)
            | _ -> 
                let hl = height l 
                let hr = height r 
                let m = if hl < hr then hr else hl 
                MapNode(k,v,l,r,m+1)

        let rebalance t1 k v t2 =
            let t1h = height t1 
            let t2h = height t2 
            if  t2h > t1h + 2 then (* right is heavier than left *)
                match t2 with 
                | MapNode(t2k,t2v,t2l,t2r,_) -> 
                   (* one of the nodes must have height > height t1 + 1 *)
                   if height t2l > t1h + 1 then  (* balance left: combination *)
                     match t2l with 
                     | MapNode(t2lk,t2lv,t2ll,t2lr,_) ->
                        mk (mk t1 k v t2ll) t2lk t2lv (mk t2lr t2k t2v t2r) 
                     | _ -> failwith "rebalance"
                   else (* rotate left *)
                     mk (mk t1 k v t2l) t2k t2v t2r
                | _ -> failwith "rebalance"
            else
                if  t1h > t2h + 2 then (* left is heavier than right *)
                  match t1 with 
                  | MapNode(t1k,t1v,t1l,t1r,_) -> 
                    (* one of the nodes must have height > height t2 + 1 *)
                      if height t1r > t2h + 1 then 
                      (* balance right: combination *)
                        match t1r with 
                        | MapNode(t1rk,t1rv,t1rl,t1rr,_) ->
                            mk (mk t1l t1k t1v t1rl) t1rk t1rv (mk t1rr k v t2)
                        | _ -> failwith "rebalance"
                      else
                        mk t1l t1k t1v (mk t1r k v t2)
                  | _ -> failwith "rebalance"
                else mk t1 k v t2

        let rec add (comparer: IComparer<'Value>) k v m = 
            match m with 
            | MapEmpty -> MapOne(k,v)
            | MapOne(k2,_) -> 
                let c = comparer.Compare(k,k2) 
                if c < 0   then MapNode (k,v,MapEmpty,m,2)
                elif c = 0 then MapOne(k,v)
                else            MapNode (k,v,m,MapEmpty,2)
            | MapNode(k2,v2,l,r,h) -> 
                let c = comparer.Compare(k,k2) 
                if c < 0 then rebalance (add comparer k v l) k2 v2 r
                elif c = 0 then MapNode(k,v,l,r,h)
                else rebalance l k2 v2 (add comparer k v r) 

        let rec find (comparer: IComparer<'Value>) k m = 
            match m with 
            | MapEmpty -> raise (System.Collections.Generic.KeyNotFoundException())
            | MapOne(k2,v2) -> 
                let c = comparer.Compare(k,k2) 
                if c = 0 then v2
                else raise (System.Collections.Generic.KeyNotFoundException())
            | MapNode(k2,v2,l,r,_) -> 
                let c = comparer.Compare(k,k2) 
                if c < 0 then find comparer k l
                elif c = 0 then v2
                else find comparer k r

        let rec tryFind (comparer: IComparer<'Value>) k m = 
            match m with 
            | MapEmpty -> None
            | MapOne(k2,v2) -> 
                let c = comparer.Compare(k,k2) 
                if c = 0 then Some v2
                else None
            | MapNode(k2,v2,l,r,_) -> 
                let c = comparer.Compare(k,k2) 
                if c < 0 then tryFind comparer k l
                elif c = 0 then Some v2
                else tryFind comparer k r

        let partition1 (comparer: IComparer<'Value>) (f:OptimizedClosures.FSharpFunc<_,_,_>) k v (acc1,acc2) = 
            if f.Invoke(k, v) then (add comparer k v acc1,acc2) else (acc1,add comparer k v acc2) 
        
        let rec partitionAux (comparer: IComparer<'Value>) (f:OptimizedClosures.FSharpFunc<_,_,_>) s acc = 
            match s with 
            | MapEmpty -> acc
            | MapOne(k,v) -> partition1 comparer f k v acc
            | MapNode(k,v,l,r,_) -> 
                let acc = partitionAux comparer f r acc 
                let acc = partition1 comparer f k v acc
                partitionAux comparer f l acc

        let partition (comparer: IComparer<'Value>) f s = partitionAux comparer (OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)) s (empty,empty)

        let filter1 (comparer: IComparer<'Value>) (f:OptimizedClosures.FSharpFunc<_,_,_>) k v acc = if f.Invoke(k, v) then add comparer k v acc else acc 

        let rec filterAux (comparer: IComparer<'Value>) (f:OptimizedClosures.FSharpFunc<_,_,_>) s acc = 
            match s with 
            | MapEmpty -> acc
            | MapOne(k,v) -> filter1 comparer f k v acc
            | MapNode(k,v,l,r,_) ->
                let acc = filterAux comparer f l acc
                let acc = filter1 comparer f k v acc
                filterAux comparer f r acc

        let filter (comparer: IComparer<'Value>) f s = filterAux comparer (OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)) s empty

        let rec spliceOutSuccessor m = 
            match m with 
            | MapEmpty -> failwith "internal error: Map.spliceOutSuccessor"
            | MapOne(k2,v2) -> k2,v2,MapEmpty
            | MapNode(k2,v2,l,r,_) ->
                match l with 
                | MapEmpty -> k2,v2,r
                | _ -> let k3,v3,l' = spliceOutSuccessor l in k3,v3,mk l' k2 v2 r

        let rec remove (comparer: IComparer<'Value>) k m = 
            match m with 
            | MapEmpty -> empty
            | MapOne(k2,_) -> 
                let c = comparer.Compare(k,k2) 
                if c = 0 then MapEmpty else m
            | MapNode(k2,v2,l,r,_) -> 
                let c = comparer.Compare(k,k2) 
                if c < 0 then rebalance (remove comparer k l) k2 v2 r
                elif c = 0 then 
                  match l,r with 
                  | MapEmpty,_ -> r
                  | _,MapEmpty -> l
                  | _ -> 
                      let sk,sv,r' = spliceOutSuccessor r 
                      mk l sk sv r'
                else rebalance l k2 v2 (remove comparer k r) 

        let rec mem (comparer: IComparer<'Value>) k m = 
            match m with 
            | MapEmpty -> false
            | MapOne(k2,_) -> (comparer.Compare(k,k2) = 0)
            | MapNode(k2,_,l,r,_) -> 
                let c = comparer.Compare(k,k2) 
                if c < 0 then mem comparer k l
                else (c = 0 || mem comparer k r)

        let rec iterOpt (f:OptimizedClosures.FSharpFunc<_,_,_>) m =
            match m with 
            | MapEmpty -> ()
            | MapOne(k2,v2) -> f.Invoke(k2, v2)
            | MapNode(k2,v2,l,r,_) -> iterOpt f l; f.Invoke(k2, v2); iterOpt f r

        let iter f m = iterOpt (OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)) m

        let rec tryPickOpt (f:OptimizedClosures.FSharpFunc<_,_,_>) m =
            match m with 
            | MapEmpty -> None
            | MapOne(k2,v2) -> f.Invoke(k2, v2) 
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
            | MapEmpty -> false
            | MapOne(k2,v2) -> f.Invoke(k2, v2)
            | MapNode(k2,v2,l,r,_) -> existsOpt f l || f.Invoke(k2, v2) || existsOpt f r

        let exists f m = existsOpt (OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)) m

        let rec forallOpt (f:OptimizedClosures.FSharpFunc<_,_,_>) m = 
            match m with 
            | MapEmpty -> true
            | MapOne(k2,v2) -> f.Invoke(k2, v2)
            | MapNode(k2,v2,l,r,_) -> forallOpt f l && f.Invoke(k2, v2) && forallOpt f r

        let forall f m = forallOpt (OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)) m

        let rec map f m = 
            match m with 
            | MapEmpty -> empty
            | MapOne(k,v) -> MapOne(k,f v)
            | MapNode(k,v,l,r,h) -> 
                let l2 = map f l 
                let v2 = f v 
                let r2 = map f r 
                MapNode(k,v2,l2, r2,h)

        let rec mapiOpt (f:OptimizedClosures.FSharpFunc<_,_,_>) m = 
            match m with
            | MapEmpty -> empty
            | MapOne(k,v) -> MapOne(k, f.Invoke(k, v))
            | MapNode(k,v,l,r,h) -> 
                let l2 = mapiOpt f l 
                let v2 = f.Invoke(k, v) 
                let r2 = mapiOpt f r 
                MapNode(k,v2, l2, r2,h)

        let mapi f m = mapiOpt (OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)) m

        let rec foldBackOpt (f:OptimizedClosures.FSharpFunc<_,_,_,_>) m x = 
            match m with 
            | MapEmpty -> x
            | MapOne(k,v) -> f.Invoke(k,v,x)
            | MapNode(k,v,l,r,_) -> 
                let x = foldBackOpt f r x
                let x = f.Invoke(k,v,x)
                foldBackOpt f l x

        let foldBack f m x = foldBackOpt (OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt(f)) m x

        let rec foldOpt (f:OptimizedClosures.FSharpFunc<_,_,_,_>) x m  = 
            match m with 
            | MapEmpty -> x
            | MapOne(k,v) -> f.Invoke(x,k,v)
            | MapNode(k,v,l,r,_) -> 
                let x = foldOpt f x l
                let x = f.Invoke(x,k,v)
                foldOpt f x r

        let fold f x m = foldOpt (OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt(f)) x m

        let foldSectionOpt (comparer: IComparer<'Value>) lo hi (f:OptimizedClosures.FSharpFunc<_,_,_,_>) m x =
            let rec foldFromTo (f:OptimizedClosures.FSharpFunc<_,_,_,_>) m x = 
                match m with 
                | MapEmpty -> x
                | MapOne(k,v) ->
                    let cLoKey = comparer.Compare(lo,k)
                    let cKeyHi = comparer.Compare(k,hi)
                    let x = if cLoKey <= 0 && cKeyHi <= 0 then f.Invoke(k, v, x) else x
                    x
                | MapNode(k,v,l,r,_) ->
                    let cLoKey = comparer.Compare(lo,k)
                    let cKeyHi = comparer.Compare(k,hi)
                    let x = if cLoKey < 0                 then foldFromTo f l x else x
                    let x = if cLoKey <= 0 && cKeyHi <= 0 then f.Invoke(k, v, x) else x
                    let x = if cKeyHi < 0                 then foldFromTo f r x else x
                    x
           
            if comparer.Compare(lo,hi) = 1 then x else foldFromTo f m x

        let foldSection (comparer: IComparer<'Value>) lo hi f m x =
            foldSectionOpt comparer lo hi (OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt(f)) m x

        let toList m = 
            let rec loop m acc = 
                match m with 
                | MapEmpty -> acc
                | MapOne(k,v) -> (k,v)::acc
                | MapNode(k,v,l,r,_) -> loop l ((k,v)::loop r acc)
            loop m []
        let toArray m = m |> toList |> Array.ofList
        let ofList comparer l = List.fold (fun acc (k,v) -> add comparer k v acc) empty l

        let rec mkFromEnumerator comparer acc (e : IEnumerator<_>) = 
            if e.MoveNext() then 
                let (x,y) = e.Current 
                mkFromEnumerator comparer (add comparer x y acc) e
            else acc
          
        let ofArray comparer (arr : array<_>) =
            let mutable res = empty
            for (x,y) in arr do
                res <- add comparer x y res 
            res

        let ofSeq comparer (c : seq<'Key * 'T>) =
            match c with 
            | :? array<'Key * 'T> as xs -> ofArray comparer xs
            | :? list<'Key * 'T> as xs -> ofList comparer xs
            | _ -> 
                use ie = c.GetEnumerator()
                mkFromEnumerator comparer empty ie 

          
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
            | MapEmpty             :: rest -> collapseLHS rest
            | MapOne _         :: _ -> stack
            | (MapNode(k,v,l,r,_)) :: rest -> collapseLHS (l :: MapOne (k,v) :: r :: rest)
          
        let mkIterator s = { stack = collapseLHS [s]; started = false }

        let notStarted() = raise (new System.InvalidOperationException(SR.GetString(SR.enumerationNotStarted)))
        let alreadyFinished() = raise (new System.InvalidOperationException(SR.GetString(SR.enumerationAlreadyFinished)))

        let current i =
            if i.started then
                match i.stack with
                  | MapOne (k,v) :: _ -> new KeyValuePair<_,_>(k,v)
                  | []            -> alreadyFinished()
                  | _             -> failwith "Please report error: Map iterator, unexpected stack for current"
            else
                notStarted()

        let rec moveNext i =
          if i.started then
            match i.stack with
              | MapOne _ :: rest -> i.stack <- collapseLHS rest;
                                    not i.stack.IsEmpty
              | [] -> false
              | _ -> failwith "Please report error: Map iterator, unexpected stack for moveNext"
          else
              i.started <- true;  (* The first call to MoveNext "starts" the enumeration. *)
              not i.stack.IsEmpty

        let mkIEnumerator s = 
          let i = ref (mkIterator s) 
          { new IEnumerator<_> with 
                member self.Current = current !i
            interface System.Collections.IEnumerator with
                member self.Current = box (current !i)
                member self.MoveNext() = moveNext !i
                member self.Reset() = i :=  mkIterator s
            interface System.IDisposable with 
                member self.Dispose() = ()}



#if FX_NO_DEBUG_PROXIES
#else
    [<System.Diagnostics.DebuggerTypeProxy(typedefof<MapDebugView<_,_>>)>]
#endif
#if FX_NO_DEBUG_DISPLAYS
#else
    [<System.Diagnostics.DebuggerDisplay("Count = {Count}")>]
#endif
    [<Sealed>]
    [<CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")>]
    [<CompiledName("FSharpMap`2")>]
    type Map<[<EqualityConditionalOn>]'Key,[<EqualityConditionalOn;ComparisonConditionalOn>]'Value when 'Key : comparison >(comparer: IComparer<'Key>, tree: MapTree<'Key,'Value>) =

#if FX_NO_BINARY_SERIALIZATION
#else
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
            new Map<'Key,'Value>(comparer,MapTree<_,_>.MapEmpty)

#if FX_NO_BINARY_SERIALIZATION
#else
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

        new(ie : seq<_>) = 
           let comparer = LanguagePrimitives.FastGenericComparer<'Key> 
           new Map<_,_>(comparer,MapTree.ofSeq comparer ie)
    
#if FX_NO_DEBUG_DISPLAYS
#else
        [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
#endif
        member internal m.Comparer = comparer
        //[<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
        member internal m.Tree = tree
        member m.Add(k,v) : Map<'Key,'Value> = 
#if TRACE_SETS_AND_MAPS
            MapTree.report()
            MapTree.numAdds <- MapTree.numAdds + 1
            let size = MapTree.size m.Tree + 1
            MapTree.totalSizeOnMapAdd <- MapTree.totalSizeOnMapAdd + float size
            if size > MapTree.largestMapSize then 
               MapTree.largestMapSize <- size
               MapTree.largestMapStackTrace <- System.Diagnostics.StackTrace().ToString()
#endif
            new Map<'Key,'Value>(comparer,MapTree.add comparer k v tree)
#if FX_NO_DEBUG_DISPLAYS
#else
        [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
#endif
        member m.IsEmpty = MapTree.isEmpty tree
        member m.Item 
         with get(k : 'Key) = 
#if TRACE_SETS_AND_MAPS
            MapTree.report()
            MapTree.numLookups <- MapTree.numLookups + 1
            MapTree.totalSizeOnMapLookup <- MapTree.totalSizeOnMapLookup + float (MapTree.size tree)
#endif
            MapTree.find comparer k tree
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

        member m.ContainsKey(k) = 
#if TRACE_SETS_AND_MAPS
            MapTree.report()
            MapTree.numLookups <- MapTree.numLookups + 1
            MapTree.totalSizeOnMapLookup <- MapTree.totalSizeOnMapLookup + float (MapTree.size tree)
#endif
            MapTree.mem comparer k tree

        member m.Remove(k)  : Map<'Key,'Value> = 
            new Map<'Key,'Value>(comparer,MapTree.remove comparer k tree)

        member m.TryFind(k) = 
#if TRACE_SETS_AND_MAPS
            MapTree.report()
            MapTree.numLookups <- MapTree.numLookups + 1
            MapTree.totalSizeOnMapLookup <- MapTree.totalSizeOnMapLookup + float (MapTree.size tree)
#endif
            MapTree.tryFind comparer k tree

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
                    (m1 = m2) && (not m1 || ((e1.Current.Key = e2.Current.Key) && (Unchecked.equals e1.Current.Value e2.Current.Value) && loop()))
                loop()
            | _ -> false

        override this.GetHashCode() = this.ComputeHashCode()

        interface IEnumerable<KeyValuePair<'Key, 'Value>> with
            member m.GetEnumerator() = MapTree.mkIEnumerator tree

        interface System.Collections.IEnumerable with
            member m.GetEnumerator() = (MapTree.mkIEnumerator tree :> System.Collections.IEnumerator)

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
            member s.Add(x) = ignore(x); raise (NotSupportedException(SR.GetString(SR.mapCannotBeMutated)));
            member s.Clear() = raise (NotSupportedException(SR.GetString(SR.mapCannotBeMutated)));
            member s.Remove(x) = ignore(x); raise (NotSupportedException(SR.GetString(SR.mapCannotBeMutated)));
            member s.Contains(x) = s.ContainsKey(x.Key) && Unchecked.equals s.[x.Key] x.Value
            member s.CopyTo(arr,i) = MapTree.copyToArray tree arr i
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

        override x.ToString() = 
           match List.ofSeq (Seq.truncate 4 x) with 
           | [] -> "map []"
           | [KeyValue h1] -> System.Text.StringBuilder().Append("map [").Append(LanguagePrimitives.anyToStringShowingNull h1).Append("]").ToString()
           | [KeyValue h1;KeyValue h2] -> System.Text.StringBuilder().Append("map [").Append(LanguagePrimitives.anyToStringShowingNull h1).Append("; ").Append(LanguagePrimitives.anyToStringShowingNull h2).Append("]").ToString()
           | [KeyValue h1;KeyValue h2;KeyValue h3] -> System.Text.StringBuilder().Append("map [").Append(LanguagePrimitives.anyToStringShowingNull h1).Append("; ").Append(LanguagePrimitives.anyToStringShowingNull h2).Append("; ").Append(LanguagePrimitives.anyToStringShowingNull h3).Append("]").ToString()
           | KeyValue h1 :: KeyValue h2 :: KeyValue h3 :: _ -> System.Text.StringBuilder().Append("map [").Append(LanguagePrimitives.anyToStringShowingNull h1).Append("; ").Append(LanguagePrimitives.anyToStringShowingNull h2).Append("; ").Append(LanguagePrimitives.anyToStringShowingNull h3).Append("; ... ]").ToString() 


#if FX_NO_DEBUG_PROXIES
#else
    and 
        [<Sealed>]
        MapDebugView<'Key,'Value when 'Key : comparison>(v: Map<'Key,'Value>)  =  

         [<System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)>]
         member x.Items = v |> Seq.truncate 10000 |> Seq.toArray
#endif
        

namespace Microsoft.FSharp.Collections

    open System
    open System.Diagnostics
    open System.Collections.Generic
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
    open Microsoft.FSharp.Core.Operators
    open Microsoft.FSharp.Collections
    open Microsoft.FSharp.Primitives.Basics

    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    [<RequireQualifiedAccess>]
    module Map = 

        [<CompiledName("IsEmpty")>]
        let isEmpty (m:Map<_,_>) = m.IsEmpty

        [<CompiledName("Add")>]
        let add k v (m:Map<_,_>) = m.Add(k,v)

        [<CompiledName("Find")>]
        let find k (m:Map<_,_>) = m.[k]

        [<CompiledName("TryFind")>]
        let tryFind k (m:Map<_,_>) = m.TryFind(k)

        [<CompiledName("Remove")>]
        let remove k (m:Map<_,_>) = m.Remove(k)

        [<CompiledName("ContainsKey")>]
        let containsKey k (m:Map<_,_>) = m.ContainsKey(k)

        [<CompiledName("Iterate")>]
        let iter f (m:Map<_,_>) = m.Iterate(f)

        [<CompiledName("TryPick")>]
        let tryPick f (m:Map<_,_>) = m.TryPick(f)

        [<CompiledName("Pick")>]
        let pick f (m:Map<_,_>) = match tryPick f m with None -> raise (System.Collections.Generic.KeyNotFoundException()) | Some res -> res

        [<CompiledName("Exists")>]
        let exists f (m:Map<_,_>) = m.Exists(f)

        [<CompiledName("Filter")>]
        let filter f (m:Map<_,_>) = m.Filter(f)

        [<CompiledName("Partition")>]
        let partition f (m:Map<_,_>) = m.Partition(f)

        [<CompiledName("ForAll")>]
        let forall f (m:Map<_,_>) = m.ForAll(f)

        let mapRange f (m:Map<_,_>) = m.MapRange(f)

        [<CompiledName("Map")>]
        let map f (m:Map<_,_>) = m.Map(f)

        [<CompiledName("Fold")>]
        let fold<'Key,'T,'State when 'Key : comparison> f (z:'State) (m:Map<'Key,'T>) = MapTree.fold f z m.Tree

        [<CompiledName("FoldBack")>]
        let foldBack<'Key,'T,'State  when 'Key : comparison> f (m:Map<'Key,'T>) (z:'State) =  MapTree.foldBack  f m.Tree z
        
        [<CompiledName("ToSeq")>]
        let toSeq (m:Map<_,_>) = m |> Seq.map (fun kvp -> kvp.Key, kvp.Value)

        [<CompiledName("FindKey")>]
        let findKey f (m : Map<_,_>) = m |> toSeq |> Seq.pick (fun (k,v) -> if f k v then Some(k) else None)

        [<CompiledName("TryFindKey")>]
        let tryFindKey f (m : Map<_,_>) = m |> toSeq |> Seq.tryPick (fun (k,v) -> if f k v then Some(k) else None)

        [<CompiledName("OfList")>]
        let ofList (l: ('Key * 'Value) list) = Map<_,_>.ofList(l)

        [<CompiledName("OfSeq")>]
        let ofSeq l = Map<_,_>.Create(l)

        [<CompiledName("OfArray")>]
        let ofArray (array: ('Key * 'Value) array) = 
           let comparer = LanguagePrimitives.FastGenericComparer<'Key> 
           new Map<_,_>(comparer,MapTree.ofArray comparer array)

        [<CompiledName("ToList")>]
        let toList (m:Map<_,_>) = m.ToList()

        [<CompiledName("ToArray")>]
        let toArray (m:Map<_,_>) = m.ToArray()

        [<CompiledName("Empty")>]
        let empty<'Key,'Value  when 'Key : comparison> = Map<'Key,'Value>.Empty

        [<CompiledName("Count")>]
        let count (m:Map<_,_>) = m.Count