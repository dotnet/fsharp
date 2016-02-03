// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Collections

    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
    open Microsoft.FSharp.Core.Operators
    open Microsoft.FSharp.Collections
    open System.Collections
    open System.Collections.Generic
    open System.Diagnostics

    (* A classic functional language implementation of binary trees *)

    [<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]
    [<NoEquality; NoComparison>]
    type SetTree<'T> when 'T : comparison = 
        | SetEmpty                                          // height = 0   
        | SetNode of 'T * SetTree<'T> *  SetTree<'T> * int    // height = int 
        | SetOne  of 'T                                     // height = 1   
            // OPTIMIZATION: store SetNode(k,SetEmpty,SetEmpty,1) --->  SetOne(k) 
            // REVIEW: performance rumour has it that the data held in SetNode and SetOne should be
            // exactly one cache line on typical architectures. They are currently 
            // ~6 and 3 words respectively. 


    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module internal SetTree = 
        let rec countAux s acc = 
            match s with 
            | SetNode(_,l,r,_) -> countAux l (countAux r (acc+1))
            | SetOne(_) -> acc+1
            | SetEmpty -> acc           

        let count s = countAux s 0

    #if TRACE_SETS_AND_MAPS
        let mutable traceCount = 0
        let mutable numOnes = 0
        let mutable numNodes = 0
        let mutable numAdds = 0
        let mutable numRemoves = 0
        let mutable numLookups = 0
        let mutable numUnions = 0
        let mutable totalSizeOnNodeCreation = 0.0
        let mutable totalSizeOnSetAdd = 0.0
        let mutable totalSizeOnSetLookup = 0.0
        let report() = 
           traceCount <- traceCount + 1 
           if traceCount % 10000 = 0 then 
               System.Console.WriteLine("#SetOne = {0}, #SetNode = {1}, #Add = {2}, #Remove = {3}, #Unions = {4}, #Lookups = {5}, avSetSizeOnNodeCreation = {6}, avSetSizeOnSetCreation = {7}, avSetSizeOnSetLookup = {8}",numOnes,numNodes,numAdds,numRemoves,numUnions,numLookups,(totalSizeOnNodeCreation / float (numNodes + numOnes)),(totalSizeOnSetAdd / float numAdds),(totalSizeOnSetLookup / float numLookups))

        let SetOne n = 
            report(); 
            numOnes <- numOnes + 1; 
            totalSizeOnNodeCreation <- totalSizeOnNodeCreation + 1.0; 
            SetTree.SetOne n

        let SetNode (x,l,r,h) = 
            report(); 
            numNodes <- numNodes + 1; 
            let n = SetTree.SetNode(x,l,r,h)
            totalSizeOnNodeCreation <- totalSizeOnNodeCreation + float (count n); 
            n
    #else
        let SetOne n = SetTree.SetOne n
        let SetNode (x,l,r,h) = SetTree.SetNode(x,l,r,h)
        
    #endif
    

        let height t = 
            match t with 
            | SetEmpty -> 0
            | SetOne _ -> 1
            | SetNode (_,_,_,h) -> h

    #if CHECKED
        let rec checkInvariant t =
            // A good sanity check, loss of balance can hit perf 
            match t with 
            | SetEmpty -> true
            | SetOne _ -> true
            | SetNode (k,t1,t2,h) ->
                let h1 = height t1 
                let h2 = height t2 
                (-2 <= (h1 - h2) && (h1 - h2) <= 2) && checkInvariant t1 && checkInvariant t2
    #endif

        let tolerance = 2

        let mk l k r = 
            match l,r with 
            | SetEmpty,SetEmpty -> SetOne (k)
            | _ -> 
              let hl = height l 
              let hr = height r 
              let m = if hl < hr then hr else hl 
              SetNode(k,l,r,m+1)

        let rebalance t1 k t2 =
            let t1h = height t1 
            let t2h = height t2 
            if  t2h > t1h + tolerance then // right is heavier than left 
                match t2 with 
                | SetNode(t2k,t2l,t2r,_) -> 
                    // one of the nodes must have height > height t1 + 1 
                    if height t2l > t1h + 1 then  // balance left: combination 
                        match t2l with 
                        | SetNode(t2lk,t2ll,t2lr,_) ->
                            mk (mk t1 k t2ll) t2lk (mk t2lr t2k t2r) 
                        | _ -> failwith "rebalance"
                    else // rotate left 
                        mk (mk t1 k t2l) t2k t2r
                | _ -> failwith "rebalance"
            else
                if  t1h > t2h + tolerance then // left is heavier than right 
                    match t1 with 
                    | SetNode(t1k,t1l,t1r,_) -> 
                        // one of the nodes must have height > height t2 + 1 
                        if height t1r > t2h + 1 then 
                            // balance right: combination 
                            match t1r with 
                            | SetNode(t1rk,t1rl,t1rr,_) ->
                                mk (mk t1l t1k t1rl) t1rk (mk t1rr k t2)
                            | _ -> failwith "rebalance"
                        else
                            mk t1l t1k (mk t1r k t2)
                    | _ -> failwith "rebalance"
                else mk t1 k t2

        let rec add (comparer: IComparer<'T>) k t = 
            match t with 
            | SetNode (k2,l,r,_) -> 
                let c = comparer.Compare(k,k2) 
                if   c < 0 then rebalance (add comparer k l) k2 r
                elif c = 0 then t
                else            rebalance l k2 (add comparer k r)
            | SetOne(k2) -> 
                // nb. no check for rebalance needed for small trees, also be sure to reuse node already allocated 
                let c = comparer.Compare(k,k2) 
                if c < 0   then SetNode (k,SetEmpty,t,2)
                elif c = 0 then t
                else            SetNode (k,t,SetEmpty,2)                  
            | SetEmpty -> SetOne(k)

        let rec balance comparer t1 k t2 =
            // Given t1 < k < t2 where t1 and t2 are "balanced",
            // return a balanced tree for <t1,k,t2>.
            // Recall: balance means subtrees heights differ by at most "tolerance"
            match t1,t2 with
            | SetEmpty,t2  -> add comparer k t2 // drop t1 = empty 
            | t1,SetEmpty  -> add comparer k t1 // drop t2 = empty 
            | SetOne k1,t2 -> add comparer k (add comparer k1 t2)
            | t1,SetOne k2 -> add comparer k (add comparer k2 t1)
            | SetNode(k1,t11,t12,h1),SetNode(k2,t21,t22,h2) ->
                // Have:  (t11 < k1 < t12) < k < (t21 < k2 < t22)
                // Either (a) h1,h2 differ by at most 2 - no rebalance needed.
                //        (b) h1 too small, i.e. h1+2 < h2
                //        (c) h2 too small, i.e. h2+2 < h1 
                if   h1+tolerance < h2 then
                    // case: b, h1 too small 
                    // push t1 into low side of t2, may increase height by 1 so rebalance 
                    rebalance (balance comparer t1 k t21) k2 t22
                elif h2+tolerance < h1 then
                    // case: c, h2 too small 
                    // push t2 into high side of t1, may increase height by 1 so rebalance 
                    rebalance t11 k1 (balance comparer t12 k t2)
                else
                    // case: a, h1 and h2 meet balance requirement 
                    mk t1 k t2

        let rec split (comparer : IComparer<'T>) pivot t =
            // Given a pivot and a set t
            // Return { x in t s.t. x < pivot }, pivot in t? , { x in t s.t. x > pivot } 
            match t with
            | SetNode(k1,t11,t12,_) ->
                let c = comparer.Compare(pivot,k1)
                if   c < 0 then // pivot t1 
                    let t11Lo,havePivot,t11Hi = split comparer pivot t11
                    t11Lo,havePivot,balance comparer t11Hi k1 t12
                elif c = 0 then // pivot is k1 
                    t11,true,t12
                else            // pivot t2 
                    let t12Lo,havePivot,t12Hi = split comparer pivot t12
                    balance comparer t11 k1 t12Lo,havePivot,t12Hi
            | SetOne k1 ->
                let c = comparer.Compare(k1,pivot)
                if   c < 0 then t       ,false,SetEmpty // singleton under pivot 
                elif c = 0 then SetEmpty,true ,SetEmpty // singleton is    pivot 
                else            SetEmpty,false,t        // singleton over  pivot 
            | SetEmpty  -> 
                SetEmpty,false,SetEmpty
        
        let rec spliceOutSuccessor t = 
            match t with 
            | SetEmpty -> failwith "internal error: Set.spliceOutSuccessor"
            | SetOne (k2) -> k2,SetEmpty
            | SetNode (k2,l,r,_) ->
                match l with 
                | SetEmpty -> k2,r
                | _ -> let k3,l' = spliceOutSuccessor l in k3,mk l' k2 r

        let rec remove (comparer: IComparer<'T>) k t = 
            match t with 
            | SetEmpty -> t
            | SetOne (k2) -> 
                let c = comparer.Compare(k,k2) 
                if   c = 0 then SetEmpty
                else            t
            | SetNode (k2,l,r,_) -> 
                let c = comparer.Compare(k,k2) 
                if   c < 0 then rebalance (remove comparer k l) k2 r
                elif c = 0 then 
                  match l,r with 
                  | SetEmpty,_ -> r
                  | _,SetEmpty -> l
                  | _ -> 
                      let sk,r' = spliceOutSuccessor r 
                      mk l sk r'
                else rebalance l k2 (remove comparer k r) 

        let rec mem (comparer: IComparer<'T>) k t = 
            match t with 
            | SetNode(k2,l,r,_) -> 
                let c = comparer.Compare(k,k2) 
                if   c < 0 then mem comparer k l
                elif c = 0 then true
                else mem comparer k r
            | SetOne(k2) -> (comparer.Compare(k,k2) = 0)
            | SetEmpty -> false

        let rec iter f t = 
            match t with 
            | SetNode(k2,l,r,_) -> iter f l; f k2; iter f r
            | SetOne(k2) -> f k2
            | SetEmpty -> ()            

        let rec foldBackOpt (f:OptimizedClosures.FSharpFunc<_,_,_>) m x = 
            match m with 
            | SetNode(k,l,r,_) -> foldBackOpt f l (f.Invoke(k, (foldBackOpt f r x)))
            | SetOne(k) -> f.Invoke(k, x)
            | SetEmpty -> x

        let foldBack f m x = foldBackOpt (OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)) m x

        let rec foldOpt (f:OptimizedClosures.FSharpFunc<_,_,_>) x m = 
            match m with 
            | SetNode(k,l,r,_) -> 
                let x = foldOpt f x l in 
                let x = f.Invoke(x, k)
                foldOpt f x r
            | SetOne(k) -> f.Invoke(x, k)
            | SetEmpty -> x

        let fold f x m = foldOpt (OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)) x m

        let rec forall f m = 
            match m with 
            | SetNode(k2,l,r,_) -> f k2 && forall f l && forall f r
            | SetOne(k2) -> f k2
            | SetEmpty -> true          

        let rec exists f m = 
            match m with 
            | SetNode(k2,l,r,_) -> f k2 || exists f l || exists f r
            | SetOne(k2) -> f k2
            | SetEmpty -> false         

        let isEmpty m = match m with  | SetEmpty -> true | _ -> false

        let subset comparer a b  = forall (fun x -> mem comparer x b) a

        let psubset comparer a b  = forall (fun x -> mem comparer x b) a && exists (fun x -> not (mem comparer x a)) b

        let rec filterAux comparer f s acc = 
            match s with 
            | SetNode(k,l,r,_) -> 
                let acc = if f k then add comparer k acc else acc 
                filterAux comparer f l (filterAux comparer f r acc)
            | SetOne(k) -> if f k then add comparer k acc else acc
            | SetEmpty -> acc           

        let filter comparer f s = filterAux comparer f s SetEmpty

        let rec diffAux comparer m acc = 
            match m with 
            | SetNode(k,l,r,_) -> diffAux comparer l (diffAux comparer r (remove comparer k acc))
            | SetOne(k) -> remove comparer k acc
            | SetEmpty -> acc           

        let diff comparer a b = diffAux comparer b a

        let rec union comparer t1 t2 =
            // Perf: tried bruteForce for low heights, but nothing significant 
            match t1,t2 with               
            | SetNode(k1,t11,t12,h1),SetNode(k2,t21,t22,h2) -> // (t11 < k < t12) AND (t21 < k2 < t22) 
                // Divide and Conquer:
                //   Suppose t1 is largest.
                //   Split t2 using pivot k1 into lo and hi.
                //   Union disjoint subproblems and then combine. 
                if h1 > h2 then
                  let lo,_,hi = split comparer k1 t2 in
                  balance comparer (union comparer t11 lo) k1 (union comparer t12 hi)
                else
                  let lo,_,hi = split comparer k2 t1 in
                  balance comparer (union comparer t21 lo) k2 (union comparer t22 hi)
            | SetEmpty,t -> t
            | t,SetEmpty -> t
            | SetOne k1,t2 -> add comparer k1 t2
            | t1,SetOne k2 -> add comparer k2 t1

        let rec intersectionAux comparer b m acc = 
            match m with 
            | SetNode(k,l,r,_) -> 
                let acc = intersectionAux comparer b r acc 
                let acc = if mem comparer k b then add comparer k acc else acc 
                intersectionAux comparer b l acc
            | SetOne(k) -> 
                if mem comparer k b then add comparer k acc else acc
            | SetEmpty -> acc

        let intersection comparer a b = intersectionAux comparer b a SetEmpty

        let partition1 comparer f k (acc1,acc2) = if f k then (add comparer k acc1,acc2) else (acc1,add comparer k acc2) 
        
        let rec partitionAux comparer f s acc = 
            match s with 
            | SetNode(k,l,r,_) -> 
                let acc = partitionAux comparer f r acc 
                let acc = partition1 comparer f k acc
                partitionAux comparer f l acc
            | SetOne(k) -> partition1 comparer f k acc
            | SetEmpty -> acc           

        let partition comparer f s = partitionAux comparer f s (SetEmpty,SetEmpty)

        // It's easier to get many less-important algorithms right using this active pattern
        let (|MatchSetNode|MatchSetEmpty|) s = 
            match s with 
            | SetNode(k2,l,r,_) -> MatchSetNode(k2,l,r)
            | SetOne(k2) -> MatchSetNode(k2,SetEmpty,SetEmpty)
            | SetEmpty -> MatchSetEmpty
        
        let rec minimumElementAux s n = 
            match s with 
            | SetNode(k,l,_,_) -> minimumElementAux l k
            | SetOne(k) -> k
            | SetEmpty -> n

        and minimumElementOpt s = 
            match s with 
            | SetNode(k,l,_,_) -> Some(minimumElementAux l k)
            | SetOne(k) -> Some k
            | SetEmpty -> None

        and maximumElementAux s n = 
            match s with 
            | SetNode(k,_,r,_) -> maximumElementAux r k
            | SetOne(k) -> k
            | SetEmpty -> n             

        and maximumElementOpt s = 
            match s with 
            | SetNode(k,_,r,_) -> Some(maximumElementAux r k)
            | SetOne(k) -> Some(k)
            | SetEmpty -> None

        let minimumElement s = 
            match minimumElementOpt s with 
            | Some(k) -> k
            | None -> invalidArg "s" (SR.GetString(SR.setContainsNoElements)) 

        let maximumElement s = 
            match maximumElementOpt s with 
            | Some(k) -> k
            | None -> invalidArg "s" (SR.GetString(SR.setContainsNoElements)) 


        //--------------------------------------------------------------------------
        // Imperative left-to-right iterators.
        //--------------------------------------------------------------------------

        [<NoEquality; NoComparison>]
        type SetIterator<'T> when 'T : comparison  = 
            { mutable stack: SetTree<'T> list;  // invariant: always collapseLHS result 
              mutable started : bool           // true when MoveNext has been called   
            }

        // collapseLHS:
        // a) Always returns either [] or a list starting with SetOne.
        // b) The "fringe" of the set stack is unchanged.
        let rec collapseLHS stack =
            match stack with
            | []                       -> []
            | SetEmpty         :: rest -> collapseLHS rest
            | SetOne _         :: _ -> stack
            | SetNode(k,l,r,_) :: rest -> collapseLHS (l :: SetOne k :: r :: rest)
          
        let mkIterator s = { stack = collapseLHS [s]; started = false }

        let notStarted() = raise (new System.InvalidOperationException(SR.GetString(SR.enumerationNotStarted)))
        let alreadyFinished() = raise (new System.InvalidOperationException(SR.GetString(SR.enumerationAlreadyFinished)))

        let current i =
            if i.started then
                match i.stack with
                  | SetOne k :: _ -> k
                  | []            -> alreadyFinished()
                  | _             -> failwith "Please report error: Set iterator, unexpected stack for current"
            else
                notStarted()

        let rec moveNext i =
            if i.started then
                match i.stack with
                  | SetOne _ :: rest -> 
                      i.stack <- collapseLHS rest;
                      not i.stack.IsEmpty 
                  | [] -> false
                  | _ -> failwith "Please report error: Set iterator, unexpected stack for moveNext"
            else
                i.started <- true;  // The first call to MoveNext "starts" the enumeration.
                not i.stack.IsEmpty 

        let mkIEnumerator s = 
            let i = ref (mkIterator s) 
            { new IEnumerator<_> with 
                  member x.Current = current !i
              interface IEnumerator with 
                  member x.Current = box (current !i)
                  member x.MoveNext() = moveNext !i
                  member x.Reset() = i :=  mkIterator s
              interface System.IDisposable with 
                  member x.Dispose() = () }

        //--------------------------------------------------------------------------
        // Set comparison.  This can be expensive.
        //--------------------------------------------------------------------------

        let rec compareStacks (comparer: IComparer<'T>) l1 l2 =
            match l1,l2 with 
            | [],[] ->  0
            | [],_  -> -1
            | _ ,[] ->  1
            | (SetEmpty  _ :: t1),(SetEmpty    :: t2) -> compareStacks comparer t1 t2
            | (SetOne(n1k) :: t1),(SetOne(n2k) :: t2) -> 
                 let c = comparer.Compare(n1k,n2k) 
                 if c <> 0 then c else compareStacks comparer t1 t2
            | (SetOne(n1k) :: t1),(SetNode(n2k,SetEmpty,n2r,_) :: t2) -> 
                 let c = comparer.Compare(n1k,n2k) 
                 if c <> 0 then c else compareStacks comparer (SetEmpty :: t1) (n2r :: t2)
            | (SetNode(n1k,(SetEmpty as emp),n1r,_) :: t1),(SetOne(n2k) :: t2) -> 
                 let c = comparer.Compare(n1k,n2k) 
                 if c <> 0 then c else compareStacks comparer (n1r :: t1) (emp :: t2)
            | (SetNode(n1k,SetEmpty,n1r,_) :: t1),(SetNode(n2k,SetEmpty,n2r,_) :: t2) -> 
                 let c = comparer.Compare(n1k,n2k) 
                 if c <> 0 then c else compareStacks comparer (n1r :: t1) (n2r :: t2)
            | (SetOne(n1k) :: t1),_ -> 
                compareStacks comparer (SetEmpty :: SetOne(n1k) :: t1) l2
            | (SetNode(n1k,n1l,n1r,_) :: t1),_ -> 
                compareStacks comparer (n1l :: SetNode(n1k,SetEmpty,n1r,0) :: t1) l2
            | _,(SetOne(n2k) :: t2) -> 
                compareStacks comparer l1 (SetEmpty :: SetOne(n2k) :: t2)
            | _,(SetNode(n2k,n2l,n2r,_) :: t2) -> 
                compareStacks comparer l1 (n2l :: SetNode(n2k,SetEmpty,n2r,0) :: t2)
                
        let compare comparer s1 s2 = 
            match s1,s2 with 
            | SetEmpty,SetEmpty -> 0
            | SetEmpty,_ -> -1
            | _,SetEmpty -> 1
            | _ -> compareStacks comparer [s1] [s2]

        let choose s = minimumElement s

        let toList s = 
            let rec loop m acc = 
                match m with 
                | SetNode(k,l,r,_) -> loop l (k :: loop r acc)
                | SetOne(k) ->  k ::acc
                | SetEmpty -> acc
            loop s []

        let copyToArray s (arr: _[]) i =
            let j = ref i 
            iter (fun x -> arr.[!j] <- x; j := !j + 1) s

        let toArray s = 
            let n = (count s) 
            let res = Array.zeroCreate n 
            copyToArray s res 0;
            res



        let rec mkFromEnumerator comparer acc (e : IEnumerator<_>) = 
          if e.MoveNext() then 
            mkFromEnumerator comparer (add comparer e.Current acc) e
          else acc
          
        let ofSeq comparer (c : IEnumerable<_>) =
          use ie = c.GetEnumerator()
          mkFromEnumerator comparer SetEmpty ie 

        let ofArray comparer l = Array.fold (fun acc k -> add comparer k acc) SetEmpty l    


    [<Sealed>]
    [<CompiledName("FSharpSet`1")>]
#if FX_NO_DEBUG_PROXIES
#else
    [<System.Diagnostics.DebuggerTypeProxy(typedefof<SetDebugView<_>>)>]
#endif
#if FX_NO_DEBUG_DISPLAYS
#else
    [<System.Diagnostics.DebuggerDisplay("Count = {Count}")>]
#endif
    [<CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")>]
    type Set<[<EqualityConditionalOn>]'T when 'T : comparison >(comparer:IComparer<'T>, tree: SetTree<'T>) = 

#if FX_NO_BINARY_SERIALIZATION
#else
        [<System.NonSerialized>]
        // NOTE: This type is logically immutable. This field is only mutated during deserialization. 
        let mutable comparer = comparer 
        
        [<System.NonSerialized>]
        // NOTE: This type is logically immutable. This field is only mutated during deserialization. 
        let mutable tree = tree  
        
        // NOTE: This type is logically immutable. This field is only mutated during serialization and deserialization. 
        //
        // WARNING: The compiled name of this field may never be changed because it is part of the logical 
        // WARNING: permanent serialization format for this type.
        let mutable serializedData = null 
#endif

        // We use .NET generics per-instantiation static fields to avoid allocating a new object for each empty
        // set (it is just a lookup into a .NET table of type-instantiation-indexed static fields).

        static let empty : Set<'T> = 
            let comparer = LanguagePrimitives.FastGenericComparer<'T> 
            new Set<'T>(comparer, SetEmpty)

#if FX_NO_BINARY_SERIALIZATION
#else
        [<System.Runtime.Serialization.OnSerializingAttribute>]
        member __.OnSerializing(context: System.Runtime.Serialization.StreamingContext) =
            ignore(context)
            serializedData <- SetTree.toArray tree

        // Do not set this to null, since concurrent threads may also be serializing the data
        //[<System.Runtime.Serialization.OnSerializedAttribute>]
        //member __.OnSerialized(context: System.Runtime.Serialization.StreamingContext) =
        //    serializedData <- null

        [<System.Runtime.Serialization.OnDeserializedAttribute>]
        member __.OnDeserialized(context: System.Runtime.Serialization.StreamingContext) =
            ignore(context)
            comparer <- LanguagePrimitives.FastGenericComparer<'T>
            tree <- SetTree.ofArray comparer serializedData
            serializedData <- null
#endif

#if FX_NO_DEBUG_DISPLAYS
#else
        [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
#endif
        member internal set.Comparer = comparer
        //[<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
        member internal set.Tree : SetTree<'T> = tree

#if FX_NO_DEBUG_DISPLAYS
#else
        [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
#endif
        static member Empty : Set<'T> = empty

        member s.Add(x) : Set<'T> = 
#if TRACE_SETS_AND_MAPS
            SetTree.report()
            SetTree.numAdds <- SetTree.numAdds + 1
            SetTree.totalSizeOnSetAdd <- SetTree.totalSizeOnSetAdd + float (SetTree.count s.Tree)
#endif
            new Set<'T>(s.Comparer,SetTree.add s.Comparer x s.Tree )

        member s.Remove(x) : Set<'T> = 
#if TRACE_SETS_AND_MAPS
            SetTree.report()
            SetTree.numRemoves <- SetTree.numRemoves + 1
#endif
            new Set<'T>(s.Comparer,SetTree.remove s.Comparer x s.Tree)

        member s.Count = SetTree.count s.Tree

        member s.Contains(x) = 
#if TRACE_SETS_AND_MAPS
            SetTree.report()
            SetTree.numLookups <- SetTree.numLookups + 1
            SetTree.totalSizeOnSetLookup <- SetTree.totalSizeOnSetLookup + float (SetTree.count s.Tree)
#endif
            SetTree.mem s.Comparer  x s.Tree
        member s.Iterate(x) = SetTree.iter  x s.Tree
        member s.Fold f z  = 
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)
            SetTree.fold (fun x z -> f.Invoke(z, x)) z s.Tree 

#if FX_NO_DEBUG_DISPLAYS
#else
        [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
#endif
        member s.IsEmpty  = SetTree.isEmpty s.Tree

        member s.Partition f  : Set<'T> *  Set<'T> = 
            match s.Tree with 
            | SetEmpty -> s,s
            | _ -> let t1,t2 = SetTree.partition s.Comparer f s.Tree in new Set<_>(s.Comparer,t1), new Set<_>(s.Comparer,t2)

        member s.Filter f  : Set<'T> = 
            match s.Tree with 
            | SetEmpty -> s
            | _ -> new Set<_>(s.Comparer,SetTree.filter s.Comparer f s.Tree)

        member s.Map f  : Set<'U> = 
            let comparer = LanguagePrimitives.FastGenericComparer<'U>
            new Set<_>(comparer,SetTree.fold (fun acc k -> SetTree.add comparer (f k) acc) (SetTree<_>.SetEmpty) s.Tree)

        member s.Exists f = SetTree.exists f s.Tree

        member s.ForAll f = SetTree.forall f s.Tree

        [<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")>]
        static member (-) (a: Set<'T>, b: Set<'T>) = 
            match a.Tree with 
            | SetEmpty -> a (* 0 - B = 0 *)
            | _ -> 
            match b.Tree with 
            | SetEmpty -> a (* A - 0 = A *)
            | _ -> new Set<_>(a.Comparer,SetTree.diff a.Comparer  a.Tree b.Tree)

        [<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")>]
        static member (+) (a: Set<'T>, b: Set<'T>) = 
#if TRACE_SETS_AND_MAPS
            SetTree.report()
            SetTree.numUnions <- SetTree.numUnions + 1
#endif
            match b.Tree with 
            | SetEmpty -> a  (* A U 0 = A *)
            | _ -> 
            match a.Tree with 
            | SetEmpty -> b  (* 0 U B = B *)
            | _ -> new Set<_>(a.Comparer,SetTree.union a.Comparer  a.Tree b.Tree)

        static member Intersection(a: Set<'T>, b: Set<'T>) : Set<'T>  = 
            match b.Tree with 
            | SetEmpty -> b  (* A INTER 0 = 0 *)
            | _ -> 
            match a.Tree with 
            | SetEmpty -> a (* 0 INTER B = 0 *)
            | _ -> new Set<_>(a.Comparer,SetTree.intersection a.Comparer a.Tree b.Tree)
           
        static member Union(sets:seq<Set<'T>>) : Set<'T>  = 
            Seq.fold (fun s1 s2 -> s1 + s2) Set<'T>.Empty sets

        static member Intersection(sets:seq<Set<'T>>) : Set<'T>  = 
            Seq.reduce (fun s1 s2 -> Set<_>.Intersection(s1,s2)) sets

        static member Equality(a: Set<'T>, b: Set<'T>) = (SetTree.compare a.Comparer  a.Tree b.Tree = 0)

        static member Compare(a: Set<'T>, b: Set<'T>) = SetTree.compare a.Comparer  a.Tree b.Tree

#if FX_NO_DEBUG_DISPLAYS
#else
        [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
#endif
        member x.Choose = SetTree.choose x.Tree

#if FX_NO_DEBUG_DISPLAYS
#else
        [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
#endif
        member x.MinimumElement = SetTree.minimumElement x.Tree

#if FX_NO_DEBUG_DISPLAYS
#else
        [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
#endif
        member x.MaximumElement = SetTree.maximumElement x.Tree

        member x.IsSubsetOf(y: Set<'T>) = SetTree.subset x.Comparer x.Tree y.Tree 
        member x.IsSupersetOf(y: Set<'T>) = SetTree.subset x.Comparer y.Tree x.Tree
        member x.IsProperSubsetOf(y: Set<'T>) = SetTree.psubset x.Comparer x.Tree y.Tree 
        member x.IsProperSupersetOf(y: Set<'T>) = SetTree.psubset x.Comparer y.Tree x.Tree
        member x.ToList () = SetTree.toList x.Tree
        member x.ToArray () = SetTree.toArray x.Tree

        member this.ComputeHashCode() = 
            let combineHash x y = (x <<< 1) + y + 631 
            let mutable res = 0
            for x in this do
                res <- combineHash res (hash x)
            abs res

        override this.GetHashCode() = this.ComputeHashCode()

        override this.Equals(that) = 
            match that with 
            | :? Set<'T> as that -> 
                use e1 = (this :> seq<_>).GetEnumerator() 
                use e2 = (that :> seq<_>).GetEnumerator() 
                let rec loop () = 
                    let m1 = e1.MoveNext() 
                    let m2 = e2.MoveNext()
                    (m1 = m2) && (not m1 || ((e1.Current = e2.Current) && loop()))
                loop()
            | _ -> false

        interface System.IComparable with 
            member this.CompareTo(that: obj) = SetTree.compare this.Comparer this.Tree ((that :?> Set<'T>).Tree)
          
        interface ICollection<'T> with 
            member s.Add(x)      = ignore(x); raise (new System.NotSupportedException("ReadOnlyCollection"))
            member s.Clear()     = raise (new System.NotSupportedException("ReadOnlyCollection"))
            member s.Remove(x)   = ignore(x); raise (new System.NotSupportedException("ReadOnlyCollection"))
            member s.Contains(x) = SetTree.mem s.Comparer x s.Tree
            member s.CopyTo(arr,i) = SetTree.copyToArray s.Tree arr i
            member s.IsReadOnly = true
            member s.Count = SetTree.count s.Tree  

        interface IEnumerable<'T> with
            member s.GetEnumerator() = SetTree.mkIEnumerator s.Tree

        interface IEnumerable with
            override s.GetEnumerator() = (SetTree.mkIEnumerator s.Tree :> IEnumerator)

        static member Singleton(x:'T) : Set<'T> = Set<'T>.Empty.Add(x)

        new (elements : seq<'T>) = 
            let comparer = LanguagePrimitives.FastGenericComparer<'T>
            new Set<_>(comparer,SetTree.ofSeq comparer elements)
          
        static member Create(elements : seq<'T>) =  new Set<'T>(elements)
          
        static member FromArray(arr : 'T array) : Set<'T> = 
            let comparer = LanguagePrimitives.FastGenericComparer<'T>
            new Set<_>(comparer,SetTree.ofArray comparer arr)

        override x.ToString() = 
           match List.ofSeq (Seq.truncate 4 x) with 
           | [] -> "set []"
           | [h1] -> System.Text.StringBuilder().Append("set [").Append(LanguagePrimitives.anyToStringShowingNull h1).Append("]").ToString()
           | [h1;h2] -> System.Text.StringBuilder().Append("set [").Append(LanguagePrimitives.anyToStringShowingNull h1).Append("; ").Append(LanguagePrimitives.anyToStringShowingNull h2).Append("]").ToString()
           | [h1;h2;h3] -> System.Text.StringBuilder().Append("set [").Append(LanguagePrimitives.anyToStringShowingNull h1).Append("; ").Append(LanguagePrimitives.anyToStringShowingNull h2).Append("; ").Append(LanguagePrimitives.anyToStringShowingNull h3).Append("]").ToString()
           | h1 :: h2 :: h3 :: _ -> System.Text.StringBuilder().Append("set [").Append(LanguagePrimitives.anyToStringShowingNull h1).Append("; ").Append(LanguagePrimitives.anyToStringShowingNull h2).Append("; ").Append(LanguagePrimitives.anyToStringShowingNull h3).Append("; ... ]").ToString() 

    and 
        [<Sealed>]
        SetDebugView<'T when 'T : comparison>(v: Set<'T>)  =  

#if FX_NO_DEBUG_DISPLAYS
#else
             [<System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.RootHidden)>]
#endif
             member x.Items = v |> Seq.truncate 1000 |> Seq.toArray 

namespace Microsoft.FSharp.Collections

    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
    open Microsoft.FSharp.Core.Operators
    open Microsoft.FSharp.Collections
    open System.Collections
    open System.Collections.Generic
    open System.Diagnostics

    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    [<RequireQualifiedAccess>]
    module Set = 

        [<CompiledName("IsEmpty")>]
        let isEmpty (s : Set<'T>) = s.IsEmpty

        [<CompiledName("Contains")>]
        let contains x (s : Set<'T>) = s.Contains(x)

        [<CompiledName("Add")>]
        let add x (s : Set<'T>) = s.Add(x)

        [<CompiledName("Singleton")>]
        let singleton x = Set<'T>.Singleton(x)

        [<CompiledName("Remove")>]
        let remove x (s : Set<'T>) = s.Remove(x)

        [<CompiledName("Union")>]
        let union (s1 : Set<'T>)  (s2 : Set<'T>)  = s1 + s2

        [<CompiledName("UnionMany")>]
        let unionMany sets  = Set<_>.Union(sets)

        [<CompiledName("Intersect")>]
        let intersect (s1 : Set<'T>)  (s2 : Set<'T>)  = Set<'T>.Intersection(s1,s2)

        [<CompiledName("IntersectMany")>]
        let intersectMany sets  = Set<_>.Intersection(sets)


        [<CompiledName("Iterate")>]
        let iter f (s : Set<'T>)  = s.Iterate(f)

        [<CompiledName("Empty")>]
        let empty<'T when 'T : comparison> : Set<'T> = Set<'T>.Empty

        [<CompiledName("ForAll")>]
        let forall f (s : Set<'T>) = s.ForAll f

        [<CompiledName("Exists")>]
        let exists f (s : Set<'T>) = s.Exists f

        [<CompiledName("Filter")>]
        let filter f (s : Set<'T>) = s.Filter f

        [<CompiledName("Partition")>]
        let partition f (s : Set<'T>) = s.Partition f 

        [<CompiledName("Fold")>]
        let fold<'T,'State  when 'T : comparison> f (z:'State) (s : Set<'T>) = SetTree.fold f z s.Tree

        [<CompiledName("FoldBack")>]
        let foldBack<'T,'State when 'T : comparison> f (s : Set<'T>) (z:'State) = SetTree.foldBack f s.Tree z

        [<CompiledName("Map")>]
        let map f (s : Set<'T>) = s.Map f

        [<CompiledName("Count")>]
        let count (s : Set<'T>) = s.Count

        [<CompiledName("MinumumElement")>]
        let minimumElement (s : Set<'T>) = s.MinimumElement

        [<CompiledName("MaximumElement")>]
        let maximumElement (s : Set<'T>) = s.MaximumElement

        [<CompiledName("OfList")>]
        let ofList l = new Set<_>(List.toSeq l)

        [<CompiledName("OfArray")>]
        let ofArray (l : 'T array) = Set<'T>.FromArray(l)

        [<CompiledName("ToList")>]
        let toList (s : Set<'T>) = s.ToList()
 
        [<CompiledName("ToArray")>]
        let toArray (s : Set<'T>) = s.ToArray()

        [<CompiledName("ToSeq")>]
        let toSeq (s : Set<'T>) = (s :> seq<'T>)

        [<CompiledName("OfSeq")>]
        let ofSeq (c : seq<_>) = new Set<_>(c)


        [<CompiledName("Difference")>]
        let difference (s1: Set<'T>) (s2: Set<'T>) = s1 - s2

        [<CompiledName("IsSubset")>]
        let isSubset (x:Set<'T>) (y: Set<'T>) = SetTree.subset x.Comparer x.Tree y.Tree 

        [<CompiledName("IsSuperset")>]
        let isSuperset (x:Set<'T>) (y: Set<'T>) = SetTree.subset x.Comparer y.Tree x.Tree

        [<CompiledName("IsProperSubset")>]
        let isProperSubset (x:Set<'T>) (y: Set<'T>) = SetTree.psubset x.Comparer x.Tree y.Tree 

        [<CompiledName("IsProperSuperset")>]
        let isProperSuperset (x:Set<'T>) (y: Set<'T>) = SetTree.psubset x.Comparer y.Tree x.Tree

        [<CompiledName("MinElement")>]
        let minElement (s : Set<'T>) = s.MinimumElement

        [<CompiledName("MaxElement")>]
        let maxElement (s : Set<'T>) = s.MaximumElement



