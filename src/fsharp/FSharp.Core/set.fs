// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Collections

    open System
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
    open Microsoft.FSharp.Core.Operators
    open Microsoft.FSharp.Collections
    open System.Collections
    open System.Collections.Generic
    open System.Diagnostics

    (* A classic functional language implementation of binary trees *)

    [<NoEquality; NoComparison>]
    type SetTree<'T> when 'T: comparison = 
        | SetNode of 'T * SetTree<'T> *  SetTree<'T> * Size:int

    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module internal SetTree = 
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
    #endif

        [<Sealed; AbstractClass>]
        type Constants<'Key when 'Key : comparison> private () = 
            static let empty = SetNode(Unchecked.defaultof<'Key>, Unchecked.defaultof<SetTree<'Key>>, Unchecked.defaultof<SetTree<'Key>>, 0)
            static member Empty = empty
    
        let size (SetNode (Size=s)) = s

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

        let inline (++) l r = Checked.(+) l r

        let inline mkLeaf k = SetNode (k, Constants.Empty, Constants.Empty, 1)

        let inline mk l k r = SetNode(k,l,r,(size l) ++ (size r) + 1)

        let private rebalanceRight l k (SetNode(rk,rl,rr,_)) =
            (* one of the nodes must have height > height t1 + 1 *)
            if size rl > size l then  (* balance left: combination *)
                match rl with 
                | SetNode(rlk,rll,rlr,_) -> mk (mk l k rll) rlk (mk rlr rk rr) 
            else (* rotate left *)
                mk (mk l k rl) rk rr

        let private rebalanceLeft (SetNode(lk,ll,lr,_)) k r =
            (* one of the nodes must have height > height t2 + 1 *)
            if size lr > size r then 
                (* balance right: combination *)
                match lr with 
                | SetNode(lrk,lrl,lrr,_) -> mk (mk ll lk lrl) lrk (mk lrr k r)
            else
                mk ll lk (mk lr k r)

        let inline rebalance l k r =
            let ls, rs = size l, size r 
            if   (rs >>> 1) > ls then rebalanceRight l k r 
            elif (ls >>> 1) > rs then rebalanceLeft  l k r
            else SetNode (k,l,r, ls ++ rs ++ 1)

        let rec add (comparer:IComparer<'Key>) k (SetNode(k2,l,r,s)) = 
            if s = 0 then mkLeaf k
            else
                let c = comparer.Compare(k,k2) 
                if c < 0 then
                    let l' = add comparer k l
                    let l's, rs = size l', size r 
                    if (l's >>> 1) > rs then
                        rebalanceLeft  l' k2 r
                    else
                        SetNode (k2,l',r, l's ++ rs ++ 1)
                elif c > 0 then
                    let r' = add comparer k r
                    let ls, r's = size l, size r' 
                    if (r's >>> 1) > ls then
                        rebalanceRight l k2 r' 
                    else
                        SetNode (k2,l,r', ls ++ r's ++ 1)
                else
                    SetNode(k,l,r,s)

        let rec balance comparer t1 k t2 =
            // Given t1 < k < t2 where t1 and t2 are "balanced",
            // return a balanced tree for <t1,k,t2>.
            // Recall: balance means subtrees heights differ by at most "tolerance"
            match t1,t2 with
            | SetNode (Size=0),t2  -> add comparer k t2 // drop t1 = empty 
            | t1,SetNode (Size=0)  -> add comparer k t1 // drop t2 = empty 
            | SetNode(k1,t11,t12,s1),SetNode(k2,t21,t22,s2) ->
                // Have:  (t11 < k1 < t12) < k < (t21 < k2 < t22)
                // Either (a) h1,h2 differ by at most 2 - no rebalance needed.
                //        (b) h1 too small, i.e. h1+2 < h2
                //        (c) h2 too small, i.e. h2+2 < h1 
                if   s1 < (s2 >>> 1) then
                    // case: b, h1 too small 
                    // push t1 into low side of t2, may increase height by 1 so rebalance 
                    rebalance (balance comparer t1 k t21) k2 t22
                elif s2 < (s1 >>> 1) then
                    // case: c, h2 too small 
                    // push t2 into high side of t1, may increase height by 1 so rebalance 
                    rebalance t11 k1 (balance comparer t12 k t2)
                else
                    // case: a, h1 and h2 meet balance requirement 
                    mk t1 k t2

        let rec split (comparer: IComparer<'T>) pivot t =
            // Given a pivot and a set t
            // Return { x in t s.t. x < pivot }, pivot in t? , { x in t s.t. x > pivot } 
            match t with
            | SetNode(Size=0) -> 
                Constants.Empty,false,Constants.Empty
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
        
        let rec spliceOutSuccessor t = 
            match t with 
            | SetNode(Size=0) -> failwith "internal error: Set.spliceOutSuccessor"
            | SetNode (k2,l,r,_) ->
                match l with 
                | SetNode(Size=0) -> k2,r
                | _ -> let k3,l' = spliceOutSuccessor l in k3,mk l' k2 r

        let rec remove (comparer: IComparer<'T>) k t = 
            match t with 
            | SetNode(Size=0) -> t
            | SetNode (k2,l,r,_) -> 
                let c = comparer.Compare(k,k2) 
                if   c < 0 then rebalance (remove comparer k l) k2 r
                elif c = 0 then 
                  match l,r with 
                  | SetNode(Size=0),_ -> r
                  | _,SetNode(Size=0) -> l
                  | _ -> 
                      let sk,r' = spliceOutSuccessor r 
                      mk l sk r'
                else rebalance l k2 (remove comparer k r) 

        let rec mem (comparer: IComparer<'T>) k t = 
            match t with 
            | SetNode(Size=0) -> false
            | SetNode(k2,l,r,_) -> 
                let c = comparer.Compare(k,k2) 
                if   c < 0 then mem comparer k l
                elif c = 0 then true
                else mem comparer k r

        let rec iter f t = 
            match t with 
            | SetNode(Size=0) -> ()            
            | SetNode(k2,l,r,_) -> iter f l; f k2; iter f r

        let rec foldBackOpt (f:OptimizedClosures.FSharpFunc<_,_,_>) m x = 
            match m with 
            | SetNode(Size=0) -> x
            | SetNode(k,l,r,_) -> foldBackOpt f l (f.Invoke(k, (foldBackOpt f r x)))

        let foldBack f m x = foldBackOpt (OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)) m x

        let rec foldOpt (f:OptimizedClosures.FSharpFunc<_,_,_>) x m = 
            match m with 
            | SetNode(Size=0) -> x
            | SetNode(k,l,r,_) -> 
                let x = foldOpt f x l in 
                let x = f.Invoke(x, k)
                foldOpt f x r

        let fold f x m = foldOpt (OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)) x m

        let rec forall f m = 
            match m with 
            | SetNode(Size=0) -> true          
            | SetNode(k2,l,r,_) -> f k2 && forall f l && forall f r

        let rec exists f m = 
            match m with 
            | SetNode(Size=0) -> false         
            | SetNode(k2,l,r,_) -> f k2 || exists f l || exists f r

        let isEmpty (SetNode(Size=s)) = s = 0

        let subset comparer a b  = forall (fun x -> mem comparer x b) a

        let psubset comparer a b  = forall (fun x -> mem comparer x b) a && exists (fun x -> not (mem comparer x a)) b

        let rec filterAux comparer f s acc = 
            match s with 
            | SetNode(Size=0) -> acc           
            | SetNode(k,l,r,_) -> 
                let acc = if f k then add comparer k acc else acc 
                filterAux comparer f l (filterAux comparer f r acc)

        let filter comparer f s = filterAux comparer f s Constants.Empty

        let rec diffAux comparer m acc = 
            match acc with
            | SetNode(Size=0) -> acc
            | _ ->
            match m with 
            | SetNode(Size=0) -> acc           
            | SetNode(k,l,r,_) -> diffAux comparer l (diffAux comparer r (remove comparer k acc))

        let diff comparer a b = diffAux comparer b a

        let rec union comparer t1 t2 =
            // Perf: tried bruteForce for low heights, but nothing significant 
            match t1,t2 with               
            | SetNode(Size=0),t -> t
            | t,SetNode(Size=0) -> t
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

        let rec intersectionAux comparer b m acc = 
            match m with 
            | SetNode(Size=0) -> acc
            | SetNode(k,l,r,_) -> 
                let acc = intersectionAux comparer b r acc 
                let acc = if mem comparer k b then add comparer k acc else acc 
                intersectionAux comparer b l acc

        let intersection comparer a b = intersectionAux comparer b a Constants.Empty

        let partition1 comparer f k (acc1,acc2) = if f k then (add comparer k acc1,acc2) else (acc1,add comparer k acc2) 
        
        let rec partitionAux comparer f s acc = 
            match s with 
            | SetNode(Size=0) -> acc           
            | SetNode(k,l,r,_) -> 
                let acc = partitionAux comparer f r acc 
                let acc = partition1 comparer f k acc
                partitionAux comparer f l acc

        let partition comparer f s = partitionAux comparer f s (Constants.Empty,Constants.Empty)

        //// It's easier to get many less-important algorithms right using this active pattern
        //let (|MatchSetNode|MatchSetEmpty|) s = 
        //    match s with 
        //    | SetNode(Size=0) -> MatchSetEmpty
        //    | SetNode(k2,l,r,_) -> MatchSetNode(k2,l,r)
        
        let rec minimumElementAux s n = 
            match s with 
            | SetNode(Size=0) -> n
            | SetNode(k,l,_,_) -> minimumElementAux l k

        and minimumElementOpt s = 
            match s with 
            | SetNode(Size=0) -> None
            | SetNode(k,l,_,_) -> Some(minimumElementAux l k)

        and maximumElementAux s n = 
            match s with 
            | SetNode(Size=0) -> n             
            | SetNode(k,_,r,_) -> maximumElementAux r k

        and maximumElementOpt s = 
            match s with 
            | SetNode(Size=0) -> None
            | SetNode(k,_,r,_) -> Some(maximumElementAux r k)

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
        type SetIterator<'T> when 'T: comparison  = 
            { mutable stack: SetTree<'T> list;  // invariant: always collapseLHS result 
              mutable started: bool           // true when MoveNext has been called   
            }

        // collapseLHS:
        // a) Always returns either [] or a list starting with SetOne.
        // b) The "fringe" of the set stack is unchanged.
        let rec collapseLHS stack =
            match stack with
            | []                       -> []
            | SetNode(Size=0)  :: rest -> collapseLHS rest
            | SetNode(_,_,_,1) :: _ -> stack
            | SetNode(k,l,r,_) :: rest -> collapseLHS (l :: (mkLeaf k) :: r :: rest)
          
        let mkIterator s = { stack = collapseLHS [s]; started = false }

        let notStarted() = raise (InvalidOperationException(SR.GetString(SR.enumerationNotStarted)))
        let alreadyFinished() = raise (InvalidOperationException(SR.GetString(SR.enumerationAlreadyFinished)))

        let current i =
            if i.started then
                match i.stack with
                  | SetNode(k,_,_,1) :: _ -> k
                  | []            -> alreadyFinished()
                  | _             -> failwith "Please report error: Set iterator, unexpected stack for current"
            else
                notStarted()

        let rec moveNext i =
            if i.started then
                match i.stack with
                  | SetNode(_,_,_,1) :: rest -> 
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
                  member __.Current = current !i
              interface IEnumerator with 
                  member __.Current = box (current !i)
                  member __.MoveNext() = moveNext !i
                  member __.Reset() = i :=  mkIterator s
              interface System.IDisposable with 
                  member __.Dispose() = () }

        //--------------------------------------------------------------------------
        // Set comparison.  This can be expensive.
        //--------------------------------------------------------------------------

        let (^^) hd tl =
            match hd with
            | SetNode(Size=0) -> tl
            | _ -> hd::tl

        let rec compareStacks (comparer:IComparer<'T>) l1 l2 =
            match l1,l2 with 
            | [],[] ->  0
            | [],_  -> -1
            | _ ,[] ->  1
            | (SetNode(n1k,SetNode(Size=0),n1r,_)::t1),(SetNode(n2k,SetNode(Size=0),n2r,_)::t2) -> 
                 match comparer.Compare (n1k,n2k) with
                 | 0 -> compareStacks comparer (n1r ^^ t1) (n2r ^^ t2)
                 | c -> c
            | (SetNode(n1k,(SetNode(Size=n1ls) as n1l),n1r,_)::t1),_ when n1ls > 0 -> 
                compareStacks comparer (n1l ^^ (mk Constants.Empty n1k n1r) ^^ t1) l2
            | _,(SetNode(n2k,n2l,n2r,_)::t2) -> 
                compareStacks comparer l1 (n2l ^^ (mk Constants.Empty n2k n2r) ^^ t2)
                
        let compare comparer s1 s2 =
            if obj.ReferenceEquals (s1,s2) then 0
            else compareStacks comparer (s1 ^^ []) (s2 ^^ [])

        let choose s = minimumElement s

        let toList s = 
            let rec loop m acc = 
                match m with 
                | SetNode(Size=0) -> acc
                | SetNode(k,l,r,_) -> loop l (k :: loop r acc)
            loop s []

        let copyToArray s (arr: _[]) i =
            let j = ref i 
            iter (fun x -> arr.[!j] <- x; j := !j + 1) s

        let toArray s = 
            let n = size s
            let res = Array.zeroCreate n 
            copyToArray s res 0;
            res



        let rec mkFromEnumerator comparer acc (e: IEnumerator<_>) = 
          if e.MoveNext() then 
            mkFromEnumerator comparer (add comparer e.Current acc) e
          else acc
          
        let ofSeq comparer (c: IEnumerable<_>) =
          use ie = c.GetEnumerator()
          mkFromEnumerator comparer Constants.Empty ie 

        let ofArray comparer l = Array.fold (fun acc k -> add comparer k acc) Constants.Empty l    


    [<Sealed>]
    [<CompiledName("FSharpSet`1")>]
    [<DebuggerTypeProxy(typedefof<SetDebugView<_>>)>]
    [<DebuggerDisplay("Count = {Count}")>]
    [<CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")>]
    type Set<[<EqualityConditionalOn>]'T when 'T: comparison >(comparer:IComparer<'T>, tree: SetTree<'T>) = 

#if !FX_NO_BINARY_SERIALIZATION
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

        static let empty: Set<'T> = 
            let comparer = LanguagePrimitives.FastGenericComparer<'T> 
            Set<'T>(comparer, SetTree.Constants.Empty)

#if !FX_NO_BINARY_SERIALIZATION
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

        [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
        member internal set.Comparer = comparer

        member internal set.Tree: SetTree<'T> = tree

        [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
        static member Empty: Set<'T> = empty

        member s.Add(value): Set<'T> = 
#if TRACE_SETS_AND_MAPS
            SetTree.report()
            SetTree.numAdds <- SetTree.numAdds + 1
            SetTree.totalSizeOnSetAdd <- SetTree.totalSizeOnSetAdd + float (SetTree.count s.Tree)
#endif
            Set<'T>(s.Comparer,SetTree.add s.Comparer value s.Tree )

        member s.Remove(value): Set<'T> = 
#if TRACE_SETS_AND_MAPS
            SetTree.report()
            SetTree.numRemoves <- SetTree.numRemoves + 1
#endif
            Set<'T>(s.Comparer,SetTree.remove s.Comparer value s.Tree)

        member s.Count = SetTree.size s.Tree

        member s.Contains(value) = 
#if TRACE_SETS_AND_MAPS
            SetTree.report()
            SetTree.numLookups <- SetTree.numLookups + 1
            SetTree.totalSizeOnSetLookup <- SetTree.totalSizeOnSetLookup + float (SetTree.count s.Tree)
#endif
            SetTree.mem s.Comparer  value s.Tree

        member s.Iterate(x) = SetTree.iter  x s.Tree

        member s.Fold f z  = 
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)
            SetTree.fold (fun x z -> f.Invoke(z, x)) z s.Tree 

        [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
        member s.IsEmpty  = SetTree.isEmpty s.Tree

        member s.Partition f : Set<'T> *  Set<'T> = 
            match s.Tree with 
            | SetNode(Size=0) -> s,s
            | _ -> let t1,t2 = SetTree.partition s.Comparer f s.Tree in Set(s.Comparer,t1), Set(s.Comparer,t2)

        member s.Filter f : Set<'T> = 
            match s.Tree with 
            | SetNode(Size=0) -> s
            | _ -> Set(s.Comparer,SetTree.filter s.Comparer f s.Tree)

        member s.Map f : Set<'U> = 
            let comparer = LanguagePrimitives.FastGenericComparer<'U>
            Set(comparer,SetTree.fold (fun acc k -> SetTree.add comparer (f k) acc) SetTree.Constants.Empty s.Tree)

        member s.Exists f = SetTree.exists f s.Tree

        member s.ForAll f = SetTree.forall f s.Tree

        [<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")>]
        static member (-) (set1: Set<'T>, set2: Set<'T>) = 
            match set1.Tree with 
            | SetNode(Size=0) -> set1 (* 0 - B = 0 *)
            | _ -> 
            match set2.Tree with 
            | SetNode(Size=0) -> set1 (* A - 0 = A *)
            | _ -> Set(set1.Comparer,SetTree.diff set1.Comparer  set1.Tree set2.Tree)

        [<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates")>]
        static member (+) (set1: Set<'T>, set2: Set<'T>) = 
#if TRACE_SETS_AND_MAPS
            SetTree.report()
            SetTree.numUnions <- SetTree.numUnions + 1
#endif
            match set2.Tree with 
            | SetNode(Size=0) -> set1  (* A U 0 = A *)
            | _ -> 
            match set1.Tree with 
            | SetNode(Size=0) -> set2  (* 0 U B = B *)
            | _ -> Set(set1.Comparer,SetTree.union set1.Comparer  set1.Tree set2.Tree)

        static member Intersection(a: Set<'T>, b: Set<'T>) : Set<'T>  = 
            match b.Tree with 
            | SetNode(Size=0) -> b  (* A INTER 0 = 0 *)
            | _ -> 
            match a.Tree with 
            | SetNode(Size=0) -> a (* 0 INTER B = 0 *)
            | _ -> Set(a.Comparer,SetTree.intersection a.Comparer a.Tree b.Tree)
           
        static member Union(sets:seq<Set<'T>>) : Set<'T>  = 
            Seq.fold (fun s1 s2 -> s1 + s2) Set<'T>.Empty sets

        static member Intersection(sets:seq<Set<'T>>) : Set<'T>  = 
            Seq.reduce (fun s1 s2 -> Set.Intersection(s1,s2)) sets

        static member Equality(a: Set<'T>, b: Set<'T>) = (SetTree.compare a.Comparer  a.Tree b.Tree = 0)

        static member Compare(a: Set<'T>, b: Set<'T>) = SetTree.compare a.Comparer  a.Tree b.Tree

        [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
        member x.Choose = SetTree.choose x.Tree

        [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
        member x.MinimumElement = SetTree.minimumElement x.Tree

        [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
        member x.MaximumElement = SetTree.maximumElement x.Tree

        member x.IsSubsetOf(otherSet: Set<'T>) = SetTree.subset x.Comparer x.Tree otherSet.Tree 

        member x.IsSupersetOf(otherSet: Set<'T>) = SetTree.subset x.Comparer otherSet.Tree x.Tree

        member x.IsProperSubsetOf(otherSet: Set<'T>) = SetTree.psubset x.Comparer x.Tree otherSet.Tree 

        member x.IsProperSupersetOf(otherSet: Set<'T>) = SetTree.psubset x.Comparer otherSet.Tree x.Tree

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
            member s.Count = s.Count

        interface IReadOnlyCollection<'T> with
            member s.Count = s.Count

        interface IEnumerable<'T> with
            member s.GetEnumerator() = SetTree.mkIEnumerator s.Tree

        interface IEnumerable with
            override s.GetEnumerator() = (SetTree.mkIEnumerator s.Tree :> IEnumerator)

        static member Singleton(x:'T) : Set<'T> = Set<'T>.Empty.Add(x)

        new (elements : seq<'T>) = 
            let comparer = LanguagePrimitives.FastGenericComparer<'T>
            Set(comparer,SetTree.ofSeq comparer elements)
          
        static member Create(elements : seq<'T>) =  Set<'T>(elements)
          
        static member FromArray(arr : 'T array) : Set<'T> = 
            let comparer = LanguagePrimitives.FastGenericComparer<'T>
            Set(comparer,SetTree.ofArray comparer arr)

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

             [<DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>]
             member x.Items = v |> Seq.truncate 1000 |> Seq.toArray 

namespace Microsoft.FSharp.Collections

    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.Operators
    open Microsoft.FSharp.Collections

    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    [<RequireQualifiedAccess>]
    module Set = 

        [<CompiledName("IsEmpty")>]
        let isEmpty (set: Set<'T>) = set.IsEmpty

        [<CompiledName("Contains")>]
        let contains element (set: Set<'T>) = set.Contains(element)

        [<CompiledName("Add")>]
        let add value (set: Set<'T>) = set.Add(value)

        [<CompiledName("Singleton")>]
        let singleton value = Set<'T>.Singleton(value)

        [<CompiledName("Remove")>]
        let remove value (set: Set<'T>) = set.Remove(value)

        [<CompiledName("Union")>]
        let union (set1: Set<'T>) (set2: Set<'T>)  = set1 + set2

        [<CompiledName("UnionMany")>]
        let unionMany sets = Set.Union(sets)

        [<CompiledName("Intersect")>]
        let intersect (set1: Set<'T>) (set2: Set<'T>)  = Set<'T>.Intersection(set1,set2)

        [<CompiledName("IntersectMany")>]
        let intersectMany sets  = Set.Intersection(sets)

        [<CompiledName("Iterate")>]
        let iter action (set: Set<'T>)  = set.Iterate(action)

        [<CompiledName("Empty")>]
        let empty<'T when 'T : comparison> : Set<'T> = Set<'T>.Empty

        [<CompiledName("ForAll")>]
        let forall predicate (set: Set<'T>) = set.ForAll predicate

        [<CompiledName("Exists")>]
        let exists predicate (set: Set<'T>) = set.Exists predicate

        [<CompiledName("Filter")>]
        let filter predicate (set: Set<'T>) = set.Filter predicate

        [<CompiledName("Partition")>]
        let partition predicate (set: Set<'T>) = set.Partition predicate 

        [<CompiledName("Fold")>]
        let fold<'T,'State  when 'T : comparison> folder (state:'State) (set: Set<'T>) = SetTree.fold folder state set.Tree

        [<CompiledName("FoldBack")>]
        let foldBack<'T,'State when 'T : comparison> folder (set: Set<'T>) (state:'State) = SetTree.foldBack folder set.Tree state

        [<CompiledName("Map")>]
        let map mapping (set: Set<'T>) = set.Map mapping

        [<CompiledName("Count")>]
        let count (set: Set<'T>) = set.Count

        [<CompiledName("OfList")>]
        let ofList elements = Set(List.toSeq elements)

        [<CompiledName("OfArray")>]
        let ofArray (array: 'T array) = Set<'T>.FromArray(array)

        [<CompiledName("ToList")>]
        let toList (set: Set<'T>) = set.ToList()
 
        [<CompiledName("ToArray")>]
        let toArray (set: Set<'T>) = set.ToArray()

        [<CompiledName("ToSeq")>]
        let toSeq (set: Set<'T>) = (set:> seq<'T>)

        [<CompiledName("OfSeq")>]
        let ofSeq (elements: seq<_>) = Set(elements)

        [<CompiledName("Difference")>]
        let difference (set1: Set<'T>) (set2: Set<'T>) = set1 - set2

        [<CompiledName("IsSubset")>]
        let isSubset (set1:Set<'T>) (set2: Set<'T>) = SetTree.subset set1.Comparer set1.Tree set2.Tree 

        [<CompiledName("IsSuperset")>]
        let isSuperset (set1:Set<'T>) (set2: Set<'T>) = SetTree.subset set1.Comparer set2.Tree set1.Tree

        [<CompiledName("IsProperSubset")>]
        let isProperSubset (set1:Set<'T>) (set2: Set<'T>) = SetTree.psubset set1.Comparer set1.Tree set2.Tree 

        [<CompiledName("IsProperSuperset")>]
        let isProperSuperset (set1:Set<'T>) (set2: Set<'T>) = SetTree.psubset set1.Comparer set2.Tree set1.Tree

        [<CompiledName("MinElement")>]
        let minElement (set: Set<'T>) = set.MinimumElement

        [<CompiledName("MaxElement")>]
        let maxElement (set: Set<'T>) = set.MaximumElement



