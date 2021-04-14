// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Internal.Utilities.Collections.Tagged

    #nowarn "51"
    #nowarn "69" // interface implementations in augmentations
    #nowarn "60" // override implementations in augmentations

    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
    open System.Collections.Generic

    [<NoEquality; NoComparison>]
    [<AllowNullLiteral>]
    type internal SetTree<'T>(k: 'T, h: int) =
        member _.Height = h
        member _.Key = k
        new(k: 'T) = SetTree(k,1)

    [<NoEquality; NoComparison>]
    [<Sealed>]
    [<AllowNullLiteral>]
    type internal SetTreeNode<'T>(v:'T, left:SetTree<'T>, right: SetTree<'T>, h: int) =
        inherit SetTree<'T>(v,h)
        member _.Left = left
        member _.Right = right

    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module SetTree = 
        let empty = null

        let inline isEmpty (t:SetTree<'T>) = isNull t

        let inline private asNode(value:SetTree<'T>) : SetTreeNode<'T> =
            value :?> SetTreeNode<'T>
            
        let rec countAux (t:SetTree<'T>) acc = 
            if isEmpty t then
                acc
            else
                if t.Height = 1 then
                    acc + 1
                else
                    let tn = asNode t
                    countAux tn.Left (countAux tn.Right (acc+1)) 

        let count s = countAux s 0

        let inline height (t:SetTree<'T>) = 
            if isEmpty t then 0
            else t.Height

        [<Literal>]
        let tolerance = 2

        let mk l k r : SetTree<'T> = 
            let hl = height l 
            let hr = height r 
            let m = if hl < hr then hr else hl
            if m = 0 then // m=0 ~ isEmpty l && isEmpty r
                SetTree k
            else
                SetTreeNode (k, l, r, m+1) :> SetTree<'T>

        let rebalance t1 v t2 =
            let t1h = height t1 
            let t2h = height t2 
            if  t2h > t1h + tolerance then // right is heavier than left 
                let t2' = asNode(t2)
                // one of the nodes must have height > height t1 + 1 
                if height t2'.Left > t1h + 1 then  // balance left: combination 
                    let t2l = asNode(t2'.Left)
                    mk (mk t1 v t2l.Left) t2l.Key (mk t2l.Right t2'.Key t2'.Right) 
                else // rotate left
                    mk (mk t1 v t2'.Left) t2.Key t2'.Right
            else
                if  t1h > t2h + tolerance then // left is heavier than right
                    let t1' = asNode(t1)
                    // one of the nodes must have height > height t2 + 1 
                    if height t1'.Right > t2h + 1 then 
                        // balance right: combination
                        let t1r = asNode(t1'.Right)
                        mk (mk t1'.Left t1.Key t1r.Left) t1r.Key (mk t1r.Right v t2)
                    else
                        mk t1'.Left t1'.Key (mk t1'.Right v t2)
                else mk t1 v t2

        let rec add (comparer: IComparer<'T>) k (t:SetTree<'T>) : SetTree<'T> = 
            if isEmpty t then SetTree k
            else
                let c = comparer.Compare(k, t.Key)
                if t.Height = 1 then
                    // nb. no check for rebalance needed for small trees, also be sure to reuse node already allocated 
                    if c < 0   then SetTreeNode (k, empty, t, 2) :> SetTree<'T>
                    elif c = 0 then t
                    else            SetTreeNode (k, t, empty, 2) :> SetTree<'T>
                else
                    let tn = asNode t
                    if   c < 0 then rebalance (add comparer k tn.Left) tn.Key tn.Right
                    elif c = 0 then t
                    else            rebalance tn.Left tn.Key (add comparer k tn.Right)

        let rec balance comparer (t1:SetTree<'T>) k (t2:SetTree<'T>) =
            // Given t1 < k < t2 where t1 and t2 are "balanced", 
            // return a balanced tree for <t1, k, t2>.
            // Recall: balance means subtrees heights differ by at most "tolerance"
            if isEmpty t1 then add comparer k t2 // drop t1 = empty
            elif isEmpty t2 then add comparer k t1 // drop t2 = empty
            else
                if t1.Height = 1 then add comparer k (add comparer t1.Key t2)
                else
                    let t1n = asNode t1
                    if t2.Height = 1 then add comparer k (add comparer t2.Key t1)
                    else
                        let t2n = asNode t2
                        // Have:  (t1l < k1 < t1r) < k < (t2l < k2 < t2r)
                        // Either (a) h1, h2 differ by at most 2 - no rebalance needed.
                        //        (b) h1 too small, i.e. h1+2 < h2
                        //        (c) h2 too small, i.e. h2+2 < h1 
                        if t1n.Height + tolerance < t2n.Height then
                            // case: b, h1 too small 
                            // push t1 into low side of t2, may increase height by 1 so rebalance 
                            rebalance (balance comparer t1 k t2n.Left) t2n.Key t2n.Right
                        elif t2n.Height + tolerance < t1n.Height then
                            // case: c, h2 too small 
                            // push t2 into high side of t1, may increase height by 1 so rebalance 
                            rebalance t1n.Left t1n.Key (balance comparer t1n.Right k t2)
                        else
                            // case: a, h1 and h2 meet balance requirement 
                            mk t1 k t2

        let rec split (comparer: IComparer<'T>) pivot (t:SetTree<'T>) =
            // Given a pivot and a set t
            // Return { x in t s.t. x < pivot }, pivot in t?, { x in t s.t. x > pivot } 
            if isEmpty t then empty, false, empty
            else
                if t.Height = 1 then
                    let c = comparer.Compare(t.Key, pivot)
                    if   c < 0 then t, false, empty // singleton under pivot 
                    elif c = 0 then empty, true, empty // singleton is    pivot 
                    else            empty, false, t        // singleton over  pivot
                else
                    let tn = asNode t
                    let c = comparer.Compare(pivot, tn.Key)
                    if   c < 0 then // pivot t1 
                        let t11Lo, havePivot, t11Hi = split comparer pivot tn.Left
                        t11Lo, havePivot, balance comparer t11Hi tn.Key tn.Right
                    elif c = 0 then // pivot is k1 
                        tn.Left, true, tn.Right
                    else            // pivot t2 
                        let t12Lo, havePivot, t12Hi = split comparer pivot tn.Right
                        balance comparer tn.Left tn.Key t12Lo, havePivot, t12Hi

        let rec spliceOutSuccessor (t:SetTree<'T>) = 
            if isEmpty t then failwith "internal error: Set.spliceOutSuccessor"
            else
                if t.Height = 1 then t.Key, empty
                else
                    let tn = asNode t
                    if isEmpty tn.Left then tn.Key, tn.Right
                    else let k3, l' = spliceOutSuccessor tn.Left in k3, mk l' tn.Key tn.Right

        let rec remove (comparer: IComparer<'T>) k (t:SetTree<'T>) = 
            if isEmpty t then t
            else
                let c = comparer.Compare(k, t.Key)
                if t.Height = 1 then
                    if c = 0 then empty else t
                else
                    let tn = asNode t
                    if   c < 0 then rebalance (remove comparer k tn.Left) tn.Key tn.Right
                    elif c = 0 then
                        if isEmpty tn.Left then tn.Right
                        elif isEmpty tn.Right then tn.Left
                        else
                            let sk, r' = spliceOutSuccessor tn.Right 
                            mk tn.Left sk r'
                    else rebalance tn.Left tn.Key (remove comparer k tn.Right)

        let rec mem (comparer: IComparer<'T>) k (t:SetTree<'T>) = 
            if isEmpty t then false
            else
                let c = comparer.Compare(k, t.Key) 
                if t.Height = 1 then (c = 0)
                else
                    let tn = asNode t
                    if   c < 0 then mem comparer k tn.Left
                    elif c = 0 then true
                    else mem comparer k tn.Right

        let rec iter f (t:SetTree<'T>) = 
            if isEmpty t then ()
            else
                if t.Height = 1 then f t.Key
                else
                    let tn = asNode t
                    iter f tn.Left; f tn.Key; iter f tn.Right

        let rec foldOpt (f:OptimizedClosures.FSharpFunc<_, _, _>) x (t:SetTree<'T>) = 
            if isEmpty t then x
            else
                if t.Height = 1 then f.Invoke(x, t.Key)
                else
                    let tn = asNode t 
                    let x = foldOpt f x tn.Left in 
                    let x = f.Invoke(x, tn.Key)
                    foldOpt f x tn.Right  

        // Fold, left-to-right. 
        //
        // NOTE: This differs from the behaviour of Map.fold which folds right-to-left.
        let rec fold f (t:SetTree<'T>) x =
            if isEmpty t then x
            else
              if t.Height = 1 then f t.Key x
              else
                let tn = asNode t
                fold f tn.Right (f tn.Key (fold f tn.Left x))

        let rec forall f (t:SetTree<'T>) = 
            if isEmpty t then true
            else
                if t.Height = 1 then f t.Key
                else
                    let tn = asNode t
                    f tn.Key && forall f tn.Left && forall f tn.Right

        let rec exists f (t:SetTree<'T>) = 
            if isEmpty t then false
            else
                if t.Height = 1 then f t.Key
                else
                    let tn = asNode t
                    f tn.Key || exists f tn.Left || exists f tn.Right

        let subset comparer a b  =
            forall (fun x -> mem comparer x b) a

        let properSubset comparer a b  =
            forall (fun x -> mem comparer x b) a && exists (fun x -> not (mem comparer x a)) b

        let rec filterAux comparer f (t:SetTree<'T>) acc = 
            if isEmpty t then acc
            else
                if t.Height = 1 then
                    if f t.Key then add comparer t.Key acc else acc
                else
                    let tn = asNode t
                    let acc = if f tn.Key then add comparer tn.Key acc else acc 
                    filterAux comparer f tn.Left (filterAux comparer f tn.Right acc)

        let filter comparer f s = filterAux comparer f s empty

        let rec diffAux comparer (t:SetTree<'T>) acc = 
            if isEmpty acc then acc
            else
                if isEmpty t then acc
                else
                    if t.Height = 1 then remove comparer t.Key acc
                    else
                        let tn = asNode t
                        diffAux comparer tn.Left (diffAux comparer tn.Right (remove comparer tn.Key acc)) 

        let diff comparer a b = diffAux comparer b a

        let rec union comparer (t1:SetTree<'T>) (t2:SetTree<'T>) =
            // Perf: tried bruteForce for low heights, but nothing significant 
            if isEmpty t1 then t2
            elif isEmpty t2 then t1
            else
                if t1.Height = 1 then add comparer t1.Key t2
                else
                    if t2.Height = 1 then add comparer t2.Key t1
                    else
                        let t1n = asNode t1
                        let t2n = asNode t2 // (t1l < k < t1r) AND (t2l < k2 < t2r) 
                        // Divide and Conquer:
                        //   Suppose t1 is largest.
                        //   Split t2 using pivot k1 into lo and hi.
                        //   Union disjoint subproblems and then combine. 
                        if t1n.Height > t2n.Height then
                            let lo, _, hi = split comparer t1n.Key t2 in
                            balance comparer (union comparer t1n.Left lo) t1n.Key (union comparer t1n.Right hi)
                        else
                            let lo, _, hi = split comparer t2n.Key t1 in
                            balance comparer (union comparer t2n.Left lo) t2n.Key (union comparer t2n.Right hi)

        let rec intersectionAux comparer b (t:SetTree<'T>) acc = 
            if isEmpty t then acc
            else
                if t.Height = 1 then
                    if mem comparer t.Key b then add comparer t.Key acc else acc
                else
                    let tn = asNode t 
                    let acc = intersectionAux comparer b tn.Right acc 
                    let acc = if mem comparer tn.Key b then add comparer tn.Key acc else acc 
                    intersectionAux comparer b tn.Left acc

        let intersection comparer a b = intersectionAux comparer b a empty

        let partition1 comparer f k (acc1, acc2) = if f k then (add comparer k acc1, acc2) else (acc1, add comparer k acc2) 

        let rec partitionAux comparer f (t:SetTree<'T>) acc = 
            if isEmpty t then acc
            else
                if t.Height = 1 then partition1 comparer f t.Key acc
                else
                    let tn = asNode t 
                    let acc = partitionAux comparer f tn.Right acc 
                    let acc = partition1 comparer f tn.Key acc
                    partitionAux comparer f tn.Left acc

        let partition comparer f s = partitionAux comparer f s (empty, empty)
        
        let rec minimumElementAux (t:SetTree<'T>) n = 
            if isEmpty t then n
            else
                if t.Height = 1 then t.Key
                else
                    let tn = asNode t
                    minimumElementAux tn.Left tn.Key

        and minimumElementOpt (t:SetTree<'T>) = 
            if isEmpty t then None
            else
                if t.Height = 1 then Some t.Key
                else
                    let tn = asNode t
                    Some(minimumElementAux tn.Left tn.Key) 

        and maximumElementAux (t:SetTree<'T>) n = 
            if isEmpty t then n
            else
                if t.Height = 1 then t.Key
                else
                    let tn = asNode t
                    maximumElementAux tn.Right tn.Key 

        and maximumElementOpt (t:SetTree<'T>) = 
            if isEmpty t then None
            else
                if t.Height = 1 then Some t.Key
                else
                    let tn = asNode t
                    Some(maximumElementAux tn.Right tn.Key)

        let minimumElement s = 
            match minimumElementOpt s with 
            | Some k -> k
            | None -> failwith "minimumElement"

        let maximumElement s = 
            match maximumElementOpt s with 
            | Some k -> k
            | None -> failwith "maximumElement"

        //--------------------------------------------------------------------------
        // Imperative left-to-right iterators.
        //--------------------------------------------------------------------------

        type SetIterator<'T>(s:SetTree<'T>) = 

            // collapseLHS:
            // a) Always returns either [] or a list starting with SetOne.
            // b) The "fringe" of the set stack is unchanged.
            let rec collapseLHS (stack: SetTree<'T> list)  =
                match stack with
                | [] -> []
                | x :: rest ->
                    if isEmpty x then collapseLHS rest
                    else
                        if x.Height = 1 then stack
                        else
                            let xn = asNode x
                            collapseLHS (xn.Left :: SetTree xn.Key :: xn.Right :: rest)

            // invariant: always collapseLHS result 
            let mutable stack = collapseLHS [s]
            // true when MoveNext has been called   
            let mutable started = false 

            let notStarted() = raise (new System.InvalidOperationException("Enumeration has not started. Call MoveNext."))
            let alreadyFinished() = raise (new System.InvalidOperationException("Enumeration already finished."))

            member _.Current =
                if started then
                    match stack with
                    | k :: _ -> k.Key
                    | []     -> alreadyFinished()
                else
                    notStarted()

            member _.MoveNext() = 
              if started then
                  match stack with
                  | [] -> false
                  | t :: rest ->
                      if t.Height = 1 then
                        stack <- collapseLHS rest
                        not stack.IsEmpty
                      else
                        failwith "Please report error: Set iterator, unexpected stack for moveNext"
              else
                  started <- true; // The first call to MoveNext "starts" the enumeration.
                  not stack.IsEmpty 

        let toSeq s = 
            let mutable i = SetIterator s
            { new IEnumerator<_> with 
                  member _.Current = i.Current
              interface System.Collections.IEnumerator with 
                  member _.Current = box i.Current
                  member _.MoveNext() = i.MoveNext()
                  member _.Reset() = i <- SetIterator s
              interface System.IDisposable with 
                  member _.Dispose() = () }

        //--------------------------------------------------------------------------
        // Set comparison.  This can be expensive.
        //--------------------------------------------------------------------------

        let rec compareStacks (comparer: IComparer<'T>) (l1:SetTree<'T> list) (l2:SetTree<'T> list) : int =
            let cont() =
                match l1, l2 with 
                | (x1 :: t1), _ when not (isEmpty x1) ->
                    if x1.Height = 1 then
                        compareStacks comparer (empty :: SetTree x1.Key :: t1) l2
                    else
                        let x1n = asNode x1
                        compareStacks comparer (x1n.Left :: (SetTreeNode (x1n.Key, empty, x1n.Right, 0) :> SetTree<'T>) :: t1) l2
                | _, (x2 :: t2) when not (isEmpty x2) ->
                    if x2.Height = 1 then
                        compareStacks comparer l1 (empty :: SetTree x2.Key :: t2)
                    else
                        let x2n = asNode x2
                        compareStacks comparer l1 (x2n.Left :: (SetTreeNode (x2n.Key, empty, x2n.Right, 0) :> SetTree<'T>  ) :: t2)
                | _ -> failwith "unexpected state in SetTree.compareStacks"
    
            match l1, l2 with 
            | [], [] ->  0
            | [], _  -> -1
            | _, [] ->  1
            | (x1 :: t1), (x2 :: t2) ->
                if isEmpty x1 then
                    if isEmpty x2 then compareStacks comparer t1 t2
                    else cont()
                elif isEmpty x2 then cont()
                else
                    if x1.Height = 1 then
                        if x2.Height = 1 then
                            let c = comparer.Compare(x1.Key, x2.Key) 
                            if c <> 0 then c else compareStacks comparer t1 t2
                        else
                            let x2n = asNode x2
                            if isEmpty x2n.Left then
                                let c = comparer.Compare(x1.Key, x2n.Key) 
                                if c <> 0 then c else compareStacks comparer (empty :: t1) (x2n.Right :: t2)
                            else cont()
                    else
                        let x1n = asNode x1
                        if isEmpty x1n.Left then
                            if x2.Height = 1 then
                                let c = comparer.Compare(x1n.Key, x2.Key) 
                                if c <> 0 then c else compareStacks comparer (x1n.Right :: t1) (empty :: t2)
                            else
                                let x2n = asNode x2
                                if isEmpty x2n.Left then
                                    let c = comparer.Compare(x1n.Key, x2n.Key) 
                                    if c <> 0 then c else compareStacks comparer (x1n.Right :: t1) (x2n.Right :: t2)
                                else cont()
                        else cont()
                            
        let compare comparer (t1:SetTree<'T>) (t2:SetTree<'T>) = 
            if isEmpty t1 then
                if isEmpty t2 then 0
                else -1
            else
                if isEmpty t2 then 1
                else compareStacks comparer [t1] [t2]

        let choose s = minimumElement s

        let toList (t:SetTree<'T>) = 
            let rec loop (t':SetTree<'T>) acc =
                if isEmpty t' then acc
                else
                    if t'.Height = 1 then t'.Key :: acc
                    else
                        let tn = asNode t'
                        loop tn.Left (tn.Key :: loop tn.Right acc)
            loop t []     

        let copyToArray s (arr: _[]) i =
            let mutable j = i 
            iter (fun x -> arr.[j] <- x; j <- j + 1) s

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
            mkFromEnumerator comparer empty ie 

        let ofArray comparer l = Array.fold (fun acc k -> add comparer k acc) empty l    


    [<System.Diagnostics.DebuggerDisplay ("Count = {Count}")>]
    [<Sealed>]
    type internal Set<'T,'ComparerTag> when 'ComparerTag :> IComparer<'T>(comparer: IComparer<'T>, tree: SetTree<'T>) =

        static let refresh (s:Set<_,_>) t =    Set<_,_>(comparer=s.Comparer, tree=t)

        member s.Tree = tree
        member s.Comparer : IComparer<'T> = comparer

        static member Empty(comparer: 'ComparerTag) : Set<'T,'ComparerTag> =  
            Set<_,_>(comparer=comparer, tree=SetTree.empty)


        member s.Add(x) : Set<'T,'ComparerTag> = refresh s (SetTree.add comparer x tree)
        member s.Remove(x) : Set<'T,'ComparerTag> = refresh s (SetTree.remove comparer x tree)
        member s.Count = SetTree.count tree
        member s.Contains(x) = SetTree.mem comparer  x tree
        member s.Iterate(x) = SetTree.iter  x tree
        member s.Fold f x  = SetTree.fold f tree x
        member s.IsEmpty  = SetTree.isEmpty tree

        member s.Partition predicate  : Set<'T,'ComparerTag> *  Set<'T,'ComparerTag> = 
            if SetTree.isEmpty s.Tree then s,s
            else
                let t1, t2 = SetTree.partition s.Comparer predicate s.Tree
                refresh s t1, refresh s t2

        member s.Filter predicate  : Set<'T,'ComparerTag> = 
          if SetTree.isEmpty s.Tree then s
          else
              SetTree.filter comparer predicate tree |> refresh s

        member s.Exists predicate = SetTree.exists predicate tree

        member s.ForAll predicate = SetTree.forall predicate tree

        static member (-) ((a: Set<'T,'ComparerTag>),(b: Set<'T,'ComparerTag>)) = Set<_,_>.Difference(a,b)

        static member (+)  ((a: Set<'T,'ComparerTag>),(b: Set<'T,'ComparerTag>)) = Set<_,_>.Union(a,b)

        static member Intersection((a: Set<'T,'ComparerTag>),(b: Set<'T,'ComparerTag>)) : Set<'T,'ComparerTag>  = 
            if SetTree.isEmpty b.Tree then b  (* A INTER 0 = 0 *)
            else
                if SetTree.isEmpty a.Tree then a (* 0 INTER B = 0 *)
                else SetTree.intersection a.Comparer  a.Tree b.Tree |> refresh a
           
        static member Union(a: Set<'T,'ComparerTag>,b: Set<'T,'ComparerTag>) : Set<'T,'ComparerTag>  = 
            if SetTree.isEmpty b.Tree then a  (* A U 0 = A *)
            else
                if SetTree.isEmpty a.Tree then b  (* 0 U B = B *)
                else SetTree.union a.Comparer  a.Tree b.Tree |> refresh a

        static member Difference(a: Set<'T,'ComparerTag>,b: Set<'T,'ComparerTag>) : Set<'T,'ComparerTag>  = 
            if SetTree.isEmpty a.Tree then a (* 0 - B = 0 *)
            else
                if SetTree.isEmpty b.Tree then a (* A - 0 = A *)
                else SetTree.diff a.Comparer  a.Tree b.Tree |> refresh a

        static member Equality(a: Set<'T,'ComparerTag>,b: Set<'T,'ComparerTag>) = 
            (SetTree.compare a.Comparer  a.Tree b.Tree = 0)

        static member Compare(a: Set<'T,'ComparerTag>,b: Set<'T,'ComparerTag>) = 
            SetTree.compare a.Comparer  a.Tree b.Tree

        member s.Choose = SetTree.choose tree

        member s.MinimumElement = SetTree.minimumElement tree

        member s.MaximumElement = SetTree.maximumElement tree

        member s.IsSubsetOf((y: Set<'T,'ComparerTag>)) = SetTree.subset comparer tree y.Tree 

        member s.IsSupersetOf((y: Set<'T,'ComparerTag>)) = SetTree.subset comparer y.Tree tree

        member s.ToList () = SetTree.toList tree

        member s.ToArray () = SetTree.toArray tree

        override this.Equals(that) = 
            match that with
            // Cast to the exact same type as this, otherwise not equal.
            | :? Set<'T,'ComparerTag> as that -> ((this :> System.IComparable).CompareTo(that) = 0)
            | _ -> false

        interface System.IComparable with
            // Cast s2 to the exact same type as s1, see 4884.
            // It is not OK to cast s2 to seq<'T>, since different compares could permute the elements.
            member s1.CompareTo(s2: obj) = SetTree.compare s1.Comparer s1.Tree ((s2 :?> Set<'T,'ComparerTag>).Tree)

        member this.ComputeHashCode() = 
                let combineHash x y = (x <<< 1) + y + 631 
                let mutable res = 0
                for x in this do
                    res <- combineHash res (Unchecked.hash x)
                abs res

        override this.GetHashCode() = this.ComputeHashCode()
          
        interface ICollection<'T> with 
            member s.Add(_) = raise (new System.NotSupportedException("ReadOnlyCollection"))
            member s.Clear() = raise (new System.NotSupportedException("ReadOnlyCollection"))
            member s.Remove(_) = raise (new System.NotSupportedException("ReadOnlyCollection"))
            member s.Contains(x) = SetTree.mem comparer x tree
            member s.CopyTo(arr,i) = SetTree.copyToArray tree arr i
            member s.IsReadOnly = true
            member s.Count = SetTree.count tree  

        interface IEnumerable<'T> with
            member s.GetEnumerator() = SetTree.toSeq tree

        interface System.Collections.IEnumerable with
            override s.GetEnumerator() = (SetTree.toSeq tree :> System.Collections.IEnumerator)

        static member Singleton(comparer,x) : Set<'T,'ComparerTag>  = 
            Set<_,_>.Empty(comparer).Add(x)

        static member Create(comparer : 'ComparerTag,l : seq<'T>) : Set<'T,'ComparerTag> = 
            Set<_,_>(comparer=comparer, tree=SetTree.ofSeq comparer l)


    [<NoEquality; NoComparison>]
    [<AllowNullLiteral>]
    type internal MapTree<'Key, 'Value>(k: 'Key, v: 'Value, h: int) =
        member _.Height = h
        member _.Key = k
        member _.Value = v
        new(k: 'Key, v: 'Value) = MapTree(k,v,1)
    
    [<NoEquality; NoComparison>]
    [<Sealed>]
    [<AllowNullLiteral>]
    type internal MapTreeNode<'Key, 'Value>(k:'Key, v:'Value, left:MapTree<'Key, 'Value>, right: MapTree<'Key, 'Value>, h: int) =
        inherit MapTree<'Key,'Value>(k, v, h)
        member _.Left = left
        member _.Right = right


    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module MapTree = 

        let empty = null 

        let inline isEmpty (m:MapTree<'Key, 'Value>) = isNull m
            
        let inline private asNode(value:MapTree<'Key,'Value>) : MapTreeNode<'Key,'Value> =
            value :?> MapTreeNode<'Key,'Value>
    
        let rec sizeAux acc (m:MapTree<'Key, 'Value>) = 
            if isEmpty m then
                acc
            else
                if m.Height = 1 then
                    acc + 1
                else
                    let mn = asNode m
                    sizeAux (sizeAux (acc+1) mn.Left) mn.Right 

        let size x = sizeAux 0 x

        let inline height (m: MapTree<'Key, 'Value>) = 
            if isEmpty m then 0
            else m.Height

        let mk l k v r : MapTree<'Key, 'Value> = 
            let hl = height l
            let hr = height r
            let m = max hl hr
            if m = 0 then // m=0 ~ isEmpty l && isEmpty r 
                MapTree(k,v)
            else
                MapTreeNode(k,v,l,r,m+1) :> MapTree<'Key, 'Value>
          
        let rebalance t1 (k: 'Key) (v: 'Value) t2 : MapTree<'Key, 'Value> =
            let t1h = height t1
            let t2h = height t2 
            if  t2h > t1h + 2 then (* right is heavier than left *)
                let t2' = asNode(t2)
                (* one of the nodes must have height > height t1 + 1 *)
                if height t2'.Left > t1h + 1 then  (* balance left: combination *)
                    let t2l = asNode(t2'.Left)
                    mk (mk t1 k v t2l.Left) t2l.Key t2l.Value (mk t2l.Right t2'.Key t2'.Value t2'.Right)
                else (* rotate left *)
                    mk (mk t1 k v t2'.Left) t2'.Key t2'.Value t2'.Right
            else
                if  t1h > t2h + 2 then (* left is heavier than right *)
                    let t1' = asNode(t1)
                    (* one of the nodes must have height > height t2 + 1 *)
                    if height t1'.Right > t2h + 1 then 
                    (* balance right: combination *)
                        let t1r = asNode(t1'.Right)
                        mk (mk t1'.Left t1'.Key t1'.Value t1r.Left) t1r.Key t1r.Value (mk t1r.Right k v t2)
                    else
                        mk t1'.Left t1'.Key t1'.Value (mk t1'.Right k v t2)
                else mk t1 k v t2

        let rec add (comparer: IComparer<'Key>) k (v: 'Value) (m: MapTree<'Key, 'Value>) : MapTree<'Key, 'Value> = 
            if isEmpty m then MapTree(k,v)
            else
                let c = comparer.Compare(k,m.Key)
                if m.Height = 1 then
                    if c < 0   then MapTreeNode (k,v,empty,m,2) :> MapTree<'Key, 'Value>
                    elif c = 0 then MapTree(k,v)
                    else            MapTreeNode (k,v,m,empty,2) :> MapTree<'Key, 'Value> 
                else
                    let mn = asNode m
                    if c < 0 then rebalance (add comparer k v mn.Left) mn.Key mn.Value mn.Right
                    elif c = 0 then MapTreeNode(k,v,mn.Left,mn.Right,mn.Height) :> MapTree<'Key, 'Value>
                    else rebalance mn.Left mn.Key mn.Value (add comparer k v mn.Right)

        let indexNotFound() = raise (new System.Collections.Generic.KeyNotFoundException("An index satisfying the predicate was not found in the collection"))

        let rec tryGetValue (comparer: IComparer<'Key>) k (v: byref<'Value>) (m: MapTree<'Key, 'Value>) =                     
            if isEmpty m then false
            else
                let c = comparer.Compare(k, m.Key)
                if c = 0 then v <- m.Value; true
                else
                    if m.Height = 1 then false
                    else
                        let mn = asNode m
                        tryGetValue comparer k &v (if c < 0 then mn.Left else mn.Right)

        let find (comparer: IComparer<'Key>) k (m: MapTree<'Key, 'Value>) =
            let mutable v = Unchecked.defaultof<'Value>
            if tryGetValue comparer k &v m then
                v
            else
                indexNotFound()

        let tryFind (comparer: IComparer<'Key>) k (m: MapTree<'Key, 'Value>) = 
            let mutable v = Unchecked.defaultof<'Value>
            if tryGetValue comparer k &v m then
                Some v
            else
                None

        let partition1 (comparer: IComparer<'Key>) (f: OptimizedClosures.FSharpFunc<_, _, _>) k v (acc1, acc2) = 
            if f.Invoke (k, v) then (add comparer k v acc1, acc2) else (acc1, add comparer k v acc2) 

        let rec partitionAux (comparer: IComparer<'Key>) (f: OptimizedClosures.FSharpFunc<_, _, _>) (m: MapTree<'Key, 'Value>) acc = 
            if isEmpty m then acc
            else
                if m.Height = 1 then        
                    partition1 comparer f m.Key m.Value acc
                else
                    let mn = asNode m
                    let acc = partitionAux comparer f mn.Right acc 
                    let acc = partition1 comparer f mn.Key mn.Value acc
                    partitionAux comparer f mn.Left acc

        let partition (comparer: IComparer<'Key>) f m =
            partitionAux comparer (OptimizedClosures.FSharpFunc<_, _, _>.Adapt f) m (empty, empty)

        let filter1 (comparer: IComparer<'Key>) (f: OptimizedClosures.FSharpFunc<_, _, _>) k v acc =
            if f.Invoke (k, v) then add comparer k v acc else acc 

        let rec filterAux (comparer: IComparer<'Key>) (f: OptimizedClosures.FSharpFunc<_, _, _>) (m: MapTree<'Key, 'Value>) acc = 
            if isEmpty m then acc
            else
                if m.Height = 1 then  
                    filter1 comparer f m.Key m.Value acc
                else
                    let mn = asNode m
                    let acc = filterAux comparer f mn.Left acc
                    let acc = filter1 comparer f mn.Key mn.Value acc
                    filterAux comparer f mn.Right acc

        let filter (comparer: IComparer<'Key>) f m =
              filterAux comparer (OptimizedClosures.FSharpFunc<_, _, _>.Adapt f) m empty

        let rec spliceOutSuccessor (m: MapTree<'Key, 'Value>) = 
            if isEmpty m then failwith "internal error: Map.spliceOutSuccessor"
            else
                if m.Height = 1 then
                    m.Key, m.Value, empty
                else
                    let mn = asNode m
                    if isEmpty mn.Left then mn.Key, mn.Value, mn.Right
                    else let k3, v3, l' = spliceOutSuccessor mn.Left in k3, v3, mk l' mn.Key mn.Value mn.Right

        let rec remove (comparer: IComparer<'Key>) k (m: MapTree<'Key, 'Value>) = 
            if isEmpty m then empty
            else
                let c = comparer.Compare(k, m.Key)
                if m.Height = 1 then 
                    if c = 0 then empty else m
                else
                    let mn = asNode m 
                    if c < 0 then rebalance (remove comparer k mn.Left) mn.Key mn.Value mn.Right
                    elif c = 0 then
                        if isEmpty mn.Left then mn.Right
                        elif isEmpty mn.Right then mn.Left
                        else
                            let sk, sv, r' = spliceOutSuccessor mn.Right 
                            mk mn.Left sk sv r'
                    else rebalance mn.Left mn.Key mn.Value (remove comparer k mn.Right)

        let rec mem (comparer: IComparer<'Key>) k (m: MapTree<'Key, 'Value>) = 
            if isEmpty m then false
            else
                let c = comparer.Compare(k, m.Key)
                if m.Height = 1 then 
                    c = 0
                else
                    let mn = asNode m
                    if c < 0 then mem comparer k mn.Left
                    else (c = 0 || mem comparer k mn.Right)

        let rec iterOpt (f: OptimizedClosures.FSharpFunc<_, _, _>) (m: MapTree<'Key, 'Value>) =
            if isEmpty m then ()
            else
                if m.Height = 1 then 
                    f.Invoke (m.Key, m.Value)
                else
                    let mn = asNode m
                    iterOpt f mn.Left; f.Invoke (mn.Key, mn.Value); iterOpt f mn.Right

        let iter f m =
            iterOpt (OptimizedClosures.FSharpFunc<_, _, _>.Adapt f) m

        let rec tryPickOpt (f: OptimizedClosures.FSharpFunc<_, _, _>) (m: MapTree<'Key, 'Value>) =
            if isEmpty m then None
            else
                if m.Height = 1 then 
                    f.Invoke (m.Key, m.Value)
                else
                    let mn = asNode m
                    match tryPickOpt f mn.Left with 
                    | Some _ as res -> res 
                    | None -> 
                    match f.Invoke (mn.Key, mn.Value) with 
                    | Some _ as res -> res 
                    | None -> 
                    tryPickOpt f mn.Right

        let tryPick f m =
            tryPickOpt (OptimizedClosures.FSharpFunc<_, _, _>.Adapt f) m

        let rec existsOpt (f: OptimizedClosures.FSharpFunc<_, _, _>) (m: MapTree<'Key, 'Value>) = 
            if isEmpty m then false
            else
                if m.Height = 1 then 
                    f.Invoke (m.Key, m.Value)
                else
                    let mn = asNode m
                    existsOpt f mn.Left || f.Invoke (mn.Key, mn.Value) || existsOpt f mn.Right
        

        let exists f m =
            existsOpt (OptimizedClosures.FSharpFunc<_, _, _>.Adapt f) m

        let rec forallOpt (f: OptimizedClosures.FSharpFunc<_, _, _>) (m: MapTree<'Key, 'Value>) = 
            if isEmpty m then true
            else
                if m.Height = 1 then 
                    f.Invoke (m.Key, m.Value)
                else
                    let mn = asNode m
                    forallOpt f mn.Left && f.Invoke (mn.Key, mn.Value) && forallOpt f mn.Right

        let forall f m =
            forallOpt (OptimizedClosures.FSharpFunc<_, _, _>.Adapt f) m

        let rec map (f:'Value -> 'Result) (m: MapTree<'Key, 'Value>) : MapTree<'Key, 'Result> = 
            if isEmpty m then empty
            else
                if m.Height = 1 then 
                    MapTree (m.Key, f m.Value)
                else
                    let mn = asNode m
                    let l2 = map f mn.Left 
                    let v2 = f mn.Value
                    let r2 = map f mn.Right
                    MapTreeNode (mn.Key, v2, l2, r2, mn.Height) :> MapTree<'Key, 'Result>

        let rec mapiOpt (f: OptimizedClosures.FSharpFunc<'Key, 'Value, 'Result>) (m: MapTree<'Key, 'Value>) = 
            if isEmpty m then empty
            else
                if m.Height = 1 then
                    MapTree (m.Key, f.Invoke (m.Key, m.Value))
                else
                    let mn = asNode m
                    let l2 = mapiOpt f mn.Left
                    let v2 = f.Invoke (mn.Key, mn.Value) 
                    let r2 = mapiOpt f mn.Right
                    MapTreeNode (mn.Key, v2, l2, r2, mn.Height) :> MapTree<'Key, 'Result>

        let mapi f m =
            mapiOpt (OptimizedClosures.FSharpFunc<_, _, _>.Adapt f) m

        // Fold, right-to-left. 
        //
        // NOTE: This differs from the behaviour of Set.fold which folds left-to-right.

        let rec foldBackOpt (f: OptimizedClosures.FSharpFunc<_, _, _, _>) (m: MapTree<'Key, 'Value>) x = 
            if isEmpty m then x
            else
                if m.Height = 1 then 
                    f.Invoke (m.Key, m.Value, x)
                else
                    let mn = asNode m
                    let x = foldBackOpt f mn.Right x
                    let x = f.Invoke (mn.Key, mn.Value, x)
                    foldBackOpt f mn.Left x

        let foldBack f m x =
            foldBackOpt (OptimizedClosures.FSharpFunc<_, _, _, _>.Adapt f) m x

        let foldSectionOpt (comparer: IComparer<'Key>) lo hi (f: OptimizedClosures.FSharpFunc<_, _, _, _>) (m: MapTree<'Key, 'Value>) x =
            let rec foldFromTo (f: OptimizedClosures.FSharpFunc<_, _, _, _>) (m: MapTree<'Key, 'Value>) x = 
                if isEmpty m then x
                else
                    if m.Height = 1 then 
                        let cLoKey = comparer.Compare(lo, m.Key)
                        let cKeyHi = comparer.Compare(m.Key, hi)
                        let x = if cLoKey <= 0 && cKeyHi <= 0 then f.Invoke (m.Key, m.Value, x) else x
                        x
                    else
                        let mn = asNode m
                        let cLoKey = comparer.Compare(lo, mn.Key)
                        let cKeyHi = comparer.Compare(mn.Key, hi)
                        let x = if cLoKey < 0 then foldFromTo f mn.Left x else x
                        let x = if cLoKey <= 0 && cKeyHi <= 0 then f.Invoke (mn.Key, mn.Value, x) else x
                        let x = if cKeyHi < 0 then foldFromTo f mn.Right x else x
                        x

            if comparer.Compare(lo, hi) = 1 then x else foldFromTo f m x

        let foldSection (comparer: IComparer<'Key>) lo hi f m x =
            foldSectionOpt comparer lo hi (OptimizedClosures.FSharpFunc<_, _, _, _>.Adapt f) m x

        let rec foldMapOpt (comparer: IComparer<'Key>) (f: OptimizedClosures.FSharpFunc<_, _, _, _>) (m: MapTree<'Key, 'Value>) z acc = 
            if isEmpty m then acc,z
            else
                if m.Height = 1 then
                    let mn = asNode m
                    let acc,z = foldMapOpt comparer f mn.Right z acc
                    let v',z = f.Invoke(mn.Key, mn.Value, z)
                    let acc = add comparer mn.Key v' acc
                    foldMapOpt comparer f mn.Left z acc
                else
                    let v',z = f.Invoke(m.Key, m.Value, z)
                    add comparer m.Key v' acc,z

        let foldMap (comparer: IComparer<'Key>) f (m: MapTree<'Key, 'Value>) z acc =
            foldMapOpt comparer (OptimizedClosures.FSharpFunc<_, _, _, _>.Adapt f) m z acc

        let toList m = foldBack (fun k v acc -> (k,v) :: acc) m []
        let toArray m = m |> toList |> Array.ofList
        let ofList comparer l = List.fold (fun acc (k,v) -> add comparer k v acc) empty l

        let rec mkFromEnumerator comparer acc (e : IEnumerator<_>) = 
            if e.MoveNext() then 
                let (x,y) = e.Current 
                mkFromEnumerator comparer (add comparer x y acc) e
            else acc

        let ofArray comparer (arr : array<'Key * 'Value>) =
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
          
        let copyToArray s (arr: _[]) i =
            let mutable j = i 
            s |> iter (fun x y -> arr.[j] <- KeyValuePair(x,y); j <- j + 1)


        /// Imperative left-to-right iterators.
        type MapIterator<'Key,'Value>(s:MapTree<'Key,'Value>) = 
            // collapseLHS:
            // a) Always returns either [] or a list starting with SetOne.
            // b) The "fringe" of the set stack is unchanged. 
            let rec collapseLHS (stack:MapTree<'Key, 'Value> list) =
                match stack with
                | [] -> []
                | m :: rest ->
                    if isEmpty m then collapseLHS rest
                    else
                        if m.Height = 1 then
                            stack
                        else
                            let mn = asNode m
                            collapseLHS (mn.Left :: MapTree (mn.Key, mn.Value) :: mn.Right :: rest)

              /// invariant: always collapseLHS result 
            let mutable stack = collapseLHS [s]
               /// true when MoveNext has been called   
            let mutable started = false

            let notStarted() = raise (new System.InvalidOperationException("Enumeration has not started. Call MoveNext."))
            let alreadyFinished() = raise (new System.InvalidOperationException("Enumeration already finished."))

            member _.Current =
                if started then
                    match stack with
                    | []            -> alreadyFinished()
                    | m :: _ ->
                        if m.Height = 1 then KeyValuePair<_, _>(m.Key, m.Value)
                        else
                            failwith "Please report error: Map iterator, unexpected stack for current"
                else
                    notStarted()

            member _.MoveNext() =
                if started then
                    match stack with
                    | [] -> false
                    | m :: rest ->
                        if m.Height = 1 then
                            stack <- collapseLHS rest
                            not stack.IsEmpty
                        else
                            failwith "Please report error: Map iterator, unexpected stack for moveNext"
                else
                    started <- true  (* The first call to MoveNext "starts" the enumeration. *)
                    not stack.IsEmpty

        let toSeq s = 
            let mutable i = MapIterator(s)
            { new IEnumerator<_> with 
                  member self.Current = i.Current
              interface System.Collections.IEnumerator with
                  member self.Current = box i.Current
                  member self.MoveNext() = i.MoveNext()
                  member self.Reset() = i <-  MapIterator(s)
              interface System.IDisposable with 
                  member self.Dispose() = ()}


    [<System.Diagnostics.DebuggerDisplay ("Count = {Count}")>]
    [<Sealed>]
    type internal Map<'Key,'T,'ComparerTag> when 'ComparerTag :> IComparer<'Key>( comparer: IComparer<'Key>, tree: MapTree<'Key,'T>) =

        static let refresh (m:Map<_,_,'ComparerTag>) t = 
            Map<_,_,'ComparerTag>(comparer=m.Comparer, tree=t)

        member s.Tree = tree
        member s.Comparer : IComparer<'Key> = comparer

        static member Empty(comparer : 'ComparerTag) = Map<'Key,'T,'ComparerTag>(comparer=comparer, tree=MapTree.empty)
        member m.Add(k,v) = refresh m (MapTree.add comparer k v tree)
        member m.IsEmpty = MapTree.isEmpty tree
        member m.Item with get(k : 'Key) = MapTree.find comparer k tree
        member m.First(f) = MapTree.tryPick f tree 
        member m.Exists(f) = MapTree.exists f tree 
        member m.Filter(f) = MapTree.filter comparer f tree |> refresh m 
        member m.ForAll(f) = MapTree.forall f tree 
        member m.Fold folder acc = MapTree.foldBack folder tree acc
        member m.FoldSection lo hi f acc = MapTree.foldSection comparer lo hi f tree acc 
        member m.FoldAndMap f z  = 
            let tree,z = MapTree.foldMap comparer f tree z MapTree.empty 
            refresh m tree, z
        member m.Iterate action = MapTree.iter action tree
        member m.MapRange mapping  = refresh m (MapTree.map mapping tree)
        member m.Map mapping  = refresh m (MapTree.mapi mapping tree)
        member m.Partition(f)  =
            let r1,r2 = MapTree.partition comparer f tree  
            refresh m r1, refresh m r2
        member m.Count = MapTree.size tree
        member m.ContainsKey(k) = MapTree.mem comparer k tree
        member m.Remove(k)  = refresh m (MapTree.remove comparer k tree)
        member m.TryFind(k) = MapTree.tryFind comparer k tree
        member m.ToList() = MapTree.toList tree
        member m.ToArray() = MapTree.toArray tree

        static member FromList(comparer : 'ComparerTag,l) : Map<'Key,'T,'ComparerTag> = 
            Map<_,_,_>(comparer=comparer, tree=MapTree.ofList comparer l)

        static member Create(comparer : 'ComparerTag, ie : seq<_>) : Map<'Key,'T,'ComparerTag> = 
            Map<_,_,_>(comparer=comparer, tree=MapTree.ofSeq comparer ie)
    
        interface IEnumerable<KeyValuePair<'Key, 'T>> with
            member s.GetEnumerator() = MapTree.toSeq tree

        interface System.Collections.IEnumerable with
            override s.GetEnumerator() = (MapTree.toSeq tree :> System.Collections.IEnumerator)

        override this.Equals(that) = 
            match that with
            // Cast to the exact same type as this, otherwise not equal.
            | :? Map<'Key,'T,'ComparerTag> as that -> ((this :> System.IComparable).CompareTo(that) = 0)
            | _ -> false

        interface System.IComparable with 
             member m1.CompareTo(m2: obj) = 
                 Seq.compareWith 
                   (fun (kvp1 : KeyValuePair<_,_>) (kvp2 : KeyValuePair<_,_>)-> 
                       let c = m1.Comparer.Compare(kvp1.Key,kvp2.Key) in 
                       if c <> 0 then c else Unchecked.compare kvp1.Value kvp2.Value)
                   // Cast m2 to the exact same type as m1, see 4884.
                   // It is not OK to cast m2 to seq<KeyValuePair<'Key,'T>>, since different compares could permute the KVPs.
                   m1 (m2 :?> Map<'Key,'T,'ComparerTag>)

        member this.ComputeHashCode() = 
            let combineHash x y = (x <<< 1) + y + 631 
            let mutable res = 0
            for KeyValue(x,y) in this do
                res <- combineHash res (Unchecked.hash x)
                res <- combineHash res (Unchecked.hash y)
            abs res

        override this.GetHashCode() = this.ComputeHashCode()


    type internal Map<'Key,'T> = Map<'Key, 'T, IComparer<'Key>>
    type internal Set<'T> = Set<'T, IComparer<'T>>
