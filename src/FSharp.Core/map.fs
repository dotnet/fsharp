// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Collections

open System
open System.Collections.Generic
open System.Collections
open System.Diagnostics
open System.Runtime.CompilerServices
open System.Text
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators

[<NoEquality; NoComparison>]
[<AllowNullLiteral>]
type internal MapTree<'Key, 'Value>(k: 'Key, v: 'Value, h: int) =
    member _.Height = h
    member _.Key = k
    member _.Value = v
    new(k: 'Key, v: 'Value) = MapTree(k, v, 1)

[<NoEquality; NoComparison>]
[<Sealed>]
[<AllowNullLiteral>]
type internal MapTreeNode<'Key, 'Value>(k: 'Key, v: 'Value, left: MapTree<'Key, 'Value>, right: MapTree<'Key, 'Value>, h: int) =
    inherit MapTree<'Key, 'Value>(k, v, h)
    member _.Left = left
    member _.Right = right

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module MapTree =

    let empty = null

    let inline isEmpty (m: MapTree<'Key, 'Value>) =
        isNull m

    let inline private asNode (value: MapTree<'Key, 'Value>) : MapTreeNode<'Key, 'Value> =
        value :?> MapTreeNode<'Key, 'Value>

    let rec sizeAux acc (m: MapTree<'Key, 'Value>) =
        if isEmpty m then
            acc
        else if m.Height = 1 then
            acc + 1
        else
            let mn = asNode m
            sizeAux (sizeAux (acc + 1) mn.Left) mn.Right

    let size x =
        sizeAux 0 x

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

    let report () =
        traceCount <- traceCount + 1

        if traceCount % 1000000 = 0 then
            System.Console.WriteLine(
                "#MapOne = {0}, #MapNode = {1}, #Add = {2}, #Remove = {3}, #Unions = {4}, #Lookups = {5}, avMapTreeSizeOnNodeCreation = {6}, avMapSizeOnCreation = {7}, avMapSizeOnLookup = {8}",
                numOnes,
                numNodes,
                numAdds,
                numRemoves,
                numUnions,
                numLookups,
                (totalSizeOnNodeCreation / float (numNodes + numOnes)),
                (totalSizeOnMapAdd / float numAdds),
                (totalSizeOnMapLookup / float numLookups)
            )

            System.Console.WriteLine("#largestMapSize = {0}, largestMapStackTrace = {1}", largestMapSize, largestMapStackTrace)

    let MapTree (k, v) =
        report ()
        numOnes <- numOnes + 1
        totalSizeOnNodeCreation <- totalSizeOnNodeCreation + 1.0
        MapTree(k, v)

    let MapTreeNode (x, l, v, r, h) =
        report ()
        numNodes <- numNodes + 1
        let n = MapTreeNode(x, l, v, r, h)
        totalSizeOnNodeCreation <- totalSizeOnNodeCreation + float (size n)
        n
#endif

    let inline height (m: MapTree<'Key, 'Value>) =
        if isEmpty m then 0 else m.Height

    [<Literal>]
    let tolerance = 2

    let mk l k v r : MapTree<'Key, 'Value> =
        let hl = height l
        let hr = height r
        let m = if hl < hr then hr else hl

        if m = 0 then // m=0 ~ isEmpty l && isEmpty r
            MapTree(k, v)
        else
            MapTreeNode(k, v, l, r, m + 1) :> MapTree<'Key, 'Value> // new map is higher by 1 than the highest

    let rebalance t1 (k: 'Key) (v: 'Value) t2 : MapTree<'Key, 'Value> =
        let t1h = height t1
        let t2h = height t2

        if t2h > t1h + tolerance then (* right is heavier than left *)
            let t2' = asNode (t2)
            (* one of the nodes must have height > height t1 + 1 *)
            if height t2'.Left > t1h + 1 then (* balance left: combination *)
                let t2l = asNode (t2'.Left)
                mk (mk t1 k v t2l.Left) t2l.Key t2l.Value (mk t2l.Right t2'.Key t2'.Value t2'.Right)
            else (* rotate left *)
                mk (mk t1 k v t2'.Left) t2'.Key t2'.Value t2'.Right
        else if t1h > t2h + tolerance then (* left is heavier than right *)
            let t1' = asNode (t1)
            (* one of the nodes must have height > height t2 + 1 *)
            if height t1'.Right > t2h + 1 then
                (* balance right: combination *)
                let t1r = asNode (t1'.Right)
                mk (mk t1'.Left t1'.Key t1'.Value t1r.Left) t1r.Key t1r.Value (mk t1r.Right k v t2)
            else
                mk t1'.Left t1'.Key t1'.Value (mk t1'.Right k v t2)
        else
            mk t1 k v t2

    let rec add (comparer: IComparer<'Key>) k (v: 'Value) (m: MapTree<'Key, 'Value>) : MapTree<'Key, 'Value> =
        if isEmpty m then
            MapTree(k, v)
        else
            let c = comparer.Compare(k, m.Key)

            if m.Height = 1 then
                if c < 0 then
                    MapTreeNode(k, v, empty, m, 2) :> MapTree<'Key, 'Value>
                elif c = 0 then
                    MapTree(k, v)
                else
                    MapTreeNode(k, v, m, empty, 2) :> MapTree<'Key, 'Value>
            else
                let mn = asNode m

                if c < 0 then
                    rebalance (add comparer k v mn.Left) mn.Key mn.Value mn.Right
                elif c = 0 then
                    MapTreeNode(k, v, mn.Left, mn.Right, mn.Height) :> MapTree<'Key, 'Value>
                else
                    rebalance mn.Left mn.Key mn.Value (add comparer k v mn.Right)

    let rec tryGetValue (comparer: IComparer<'Key>) k (v: byref<'Value>) (m: MapTree<'Key, 'Value>) =
        if isEmpty m then
            false
        else
            let c = comparer.Compare(k, m.Key)

            if c = 0 then
                v <- m.Value
                true
            else if m.Height = 1 then
                false
            else
                let mn = asNode m
                tryGetValue comparer k &v (if c < 0 then mn.Left else mn.Right)

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    let throwKeyNotFound () =
        raise (KeyNotFoundException())

    [<MethodImpl(MethodImplOptions.NoInlining)>]
    let find (comparer: IComparer<'Key>) k (m: MapTree<'Key, 'Value>) =
        let mutable v = Unchecked.defaultof<'Value>

        if tryGetValue comparer k &v m then
            v
        else
            throwKeyNotFound ()

    let tryFind (comparer: IComparer<'Key>) k (m: MapTree<'Key, 'Value>) =
        let mutable v = Unchecked.defaultof<'Value>

        if tryGetValue comparer k &v m then Some v else None

    let partition1 (comparer: IComparer<'Key>) (f: OptimizedClosures.FSharpFunc<_, _, _>) k v (acc1, acc2) =
        if f.Invoke(k, v) then
            (add comparer k v acc1, acc2)
        else
            (acc1, add comparer k v acc2)

    let rec partitionAux (comparer: IComparer<'Key>) (f: OptimizedClosures.FSharpFunc<_, _, _>) (m: MapTree<'Key, 'Value>) acc =
        if isEmpty m then
            acc
        else if m.Height = 1 then
            partition1 comparer f m.Key m.Value acc
        else
            let mn = asNode m
            let acc = partitionAux comparer f mn.Right acc
            let acc = partition1 comparer f mn.Key mn.Value acc
            partitionAux comparer f mn.Left acc

    let partition (comparer: IComparer<'Key>) f m =
        partitionAux comparer (OptimizedClosures.FSharpFunc<_, _, _>.Adapt f) m (empty, empty)

    let filter1 (comparer: IComparer<'Key>) (f: OptimizedClosures.FSharpFunc<_, _, _>) k v acc =
        if f.Invoke(k, v) then add comparer k v acc else acc

    let rec filterAux (comparer: IComparer<'Key>) (f: OptimizedClosures.FSharpFunc<_, _, _>) (m: MapTree<'Key, 'Value>) acc =
        if isEmpty m then
            acc
        else if m.Height = 1 then
            filter1 comparer f m.Key m.Value acc
        else
            let mn = asNode m
            let acc = filterAux comparer f mn.Left acc
            let acc = filter1 comparer f mn.Key mn.Value acc
            filterAux comparer f mn.Right acc

    let filter (comparer: IComparer<'Key>) f m =
        filterAux comparer (OptimizedClosures.FSharpFunc<_, _, _>.Adapt f) m empty

    let rec spliceOutSuccessor (m: MapTree<'Key, 'Value>) =
        if isEmpty m then
            failwith "internal error: Map.spliceOutSuccessor"
        else if m.Height = 1 then
            m.Key, m.Value, empty
        else
            let mn = asNode m

            if isEmpty mn.Left then
                mn.Key, mn.Value, mn.Right
            else
                let k3, v3, l' = spliceOutSuccessor mn.Left in k3, v3, mk l' mn.Key mn.Value mn.Right

    let rec remove (comparer: IComparer<'Key>) k (m: MapTree<'Key, 'Value>) =
        if isEmpty m then
            empty
        else
            let c = comparer.Compare(k, m.Key)

            if m.Height = 1 then
                if c = 0 then empty else m
            else
                let mn = asNode m

                if c < 0 then
                    rebalance (remove comparer k mn.Left) mn.Key mn.Value mn.Right
                elif c = 0 then
                    if isEmpty mn.Left then
                        mn.Right
                    elif isEmpty mn.Right then
                        mn.Left
                    else
                        let sk, sv, r' = spliceOutSuccessor mn.Right
                        mk mn.Left sk sv r'
                else
                    rebalance mn.Left mn.Key mn.Value (remove comparer k mn.Right)

    let rec change (comparer: IComparer<'Key>) k (u: 'Value option -> 'Value option) (m: MapTree<'Key, 'Value>) : MapTree<'Key, 'Value> =
        if isEmpty m then
            match u None with
            | None -> m
            | Some v -> MapTree(k, v)
        else if m.Height = 1 then
            let c = comparer.Compare(k, m.Key)

            if c < 0 then
                match u None with
                | None -> m
                | Some v -> MapTreeNode(k, v, empty, m, 2) :> MapTree<'Key, 'Value>
            elif c = 0 then
                match u (Some m.Value) with
                | None -> empty
                | Some v -> MapTree(k, v)
            else
                match u None with
                | None -> m
                | Some v -> MapTreeNode(k, v, m, empty, 2) :> MapTree<'Key, 'Value>
        else
            let mn = asNode m
            let c = comparer.Compare(k, mn.Key)

            if c < 0 then
                rebalance (change comparer k u mn.Left) mn.Key mn.Value mn.Right
            elif c = 0 then
                match u (Some mn.Value) with
                | None ->
                    if isEmpty mn.Left then
                        mn.Right
                    elif isEmpty mn.Right then
                        mn.Left
                    else
                        let sk, sv, r' = spliceOutSuccessor mn.Right
                        mk mn.Left sk sv r'
                | Some v -> MapTreeNode(k, v, mn.Left, mn.Right, mn.Height) :> MapTree<'Key, 'Value>
            else
                rebalance mn.Left mn.Key mn.Value (change comparer k u mn.Right)

    let rec mem (comparer: IComparer<'Key>) k (m: MapTree<'Key, 'Value>) =
        if isEmpty m then
            false
        else
            let c = comparer.Compare(k, m.Key)

            if m.Height = 1 then
                c = 0
            else
                let mn = asNode m

                if c < 0 then
                    mem comparer k mn.Left
                else
                    (c = 0 || mem comparer k mn.Right)

    let rec iterOpt (f: OptimizedClosures.FSharpFunc<_, _, _>) (m: MapTree<'Key, 'Value>) =
        if isEmpty m then
            ()
        else if m.Height = 1 then
            f.Invoke(m.Key, m.Value)
        else
            let mn = asNode m
            iterOpt f mn.Left
            f.Invoke(mn.Key, mn.Value)
            iterOpt f mn.Right

    let iter f m =
        iterOpt (OptimizedClosures.FSharpFunc<_, _, _>.Adapt f) m

    let rec tryPickOpt (f: OptimizedClosures.FSharpFunc<_, _, _>) (m: MapTree<'Key, 'Value>) =
        if isEmpty m then
            None
        else if m.Height = 1 then
            f.Invoke(m.Key, m.Value)
        else
            let mn = asNode m

            match tryPickOpt f mn.Left with
            | Some _ as res -> res
            | None ->
                match f.Invoke(mn.Key, mn.Value) with
                | Some _ as res -> res
                | None -> tryPickOpt f mn.Right

    let tryPick f m =
        tryPickOpt (OptimizedClosures.FSharpFunc<_, _, _>.Adapt f) m

    let rec existsOpt (f: OptimizedClosures.FSharpFunc<_, _, _>) (m: MapTree<'Key, 'Value>) =
        if isEmpty m then
            false
        else if m.Height = 1 then
            f.Invoke(m.Key, m.Value)
        else
            let mn = asNode m
            existsOpt f mn.Left || f.Invoke(mn.Key, mn.Value) || existsOpt f mn.Right

    let exists f m =
        existsOpt (OptimizedClosures.FSharpFunc<_, _, _>.Adapt f) m

    let rec forallOpt (f: OptimizedClosures.FSharpFunc<_, _, _>) (m: MapTree<'Key, 'Value>) =
        if isEmpty m then
            true
        else if m.Height = 1 then
            f.Invoke(m.Key, m.Value)
        else
            let mn = asNode m
            forallOpt f mn.Left && f.Invoke(mn.Key, mn.Value) && forallOpt f mn.Right

    let forall f m =
        forallOpt (OptimizedClosures.FSharpFunc<_, _, _>.Adapt f) m

    let rec map (f: 'Value -> 'Result) (m: MapTree<'Key, 'Value>) : MapTree<'Key, 'Result> =
        if isEmpty m then
            empty
        else if m.Height = 1 then
            MapTree(m.Key, f m.Value)
        else
            let mn = asNode m
            let l2 = map f mn.Left
            let v2 = f mn.Value
            let r2 = map f mn.Right
            MapTreeNode(mn.Key, v2, l2, r2, mn.Height) :> MapTree<'Key, 'Result>

    let rec mapiOpt (f: OptimizedClosures.FSharpFunc<'Key, 'Value, 'Result>) (m: MapTree<'Key, 'Value>) =
        if isEmpty m then
            empty
        else if m.Height = 1 then
            MapTree(m.Key, f.Invoke(m.Key, m.Value))
        else
            let mn = asNode m
            let l2 = mapiOpt f mn.Left
            let v2 = f.Invoke(mn.Key, mn.Value)
            let r2 = mapiOpt f mn.Right
            MapTreeNode(mn.Key, v2, l2, r2, mn.Height) :> MapTree<'Key, 'Result>

    let mapi f m =
        mapiOpt (OptimizedClosures.FSharpFunc<_, _, _>.Adapt f) m

    let rec foldBackOpt (f: OptimizedClosures.FSharpFunc<_, _, _, _>) (m: MapTree<'Key, 'Value>) x =
        if isEmpty m then
            x
        else if m.Height = 1 then
            f.Invoke(m.Key, m.Value, x)
        else
            let mn = asNode m
            let x = foldBackOpt f mn.Right x
            let x = f.Invoke(mn.Key, mn.Value, x)
            foldBackOpt f mn.Left x

    let foldBack f m x =
        foldBackOpt (OptimizedClosures.FSharpFunc<_, _, _, _>.Adapt f) m x

    let rec foldOpt (f: OptimizedClosures.FSharpFunc<_, _, _, _>) x (m: MapTree<'Key, 'Value>) =
        if isEmpty m then
            x
        else if m.Height = 1 then
            f.Invoke(x, m.Key, m.Value)
        else
            let mn = asNode m
            let x = foldOpt f x mn.Left
            let x = f.Invoke(x, mn.Key, mn.Value)
            foldOpt f x mn.Right

    let fold f x m =
        foldOpt (OptimizedClosures.FSharpFunc<_, _, _, _>.Adapt f) x m

    let foldSectionOpt (comparer: IComparer<'Key>) lo hi (f: OptimizedClosures.FSharpFunc<_, _, _, _>) (m: MapTree<'Key, 'Value>) x =
        let rec foldFromTo (f: OptimizedClosures.FSharpFunc<_, _, _, _>) (m: MapTree<'Key, 'Value>) x =
            if isEmpty m then
                x
            else if m.Height = 1 then
                let cLoKey = comparer.Compare(lo, m.Key)
                let cKeyHi = comparer.Compare(m.Key, hi)

                let x =
                    if cLoKey <= 0 && cKeyHi <= 0 then
                        f.Invoke(m.Key, m.Value, x)
                    else
                        x

                x
            else
                let mn = asNode m
                let cLoKey = comparer.Compare(lo, mn.Key)
                let cKeyHi = comparer.Compare(mn.Key, hi)

                let x = if cLoKey < 0 then foldFromTo f mn.Left x else x

                let x =
                    if cLoKey <= 0 && cKeyHi <= 0 then
                        f.Invoke(mn.Key, mn.Value, x)
                    else
                        x

                let x = if cKeyHi < 0 then foldFromTo f mn.Right x else x

                x

        if comparer.Compare(lo, hi) = 1 then x else foldFromTo f m x

    let foldSection (comparer: IComparer<'Key>) lo hi f m x =
        foldSectionOpt comparer lo hi (OptimizedClosures.FSharpFunc<_, _, _, _>.Adapt f) m x

    let toList (m: MapTree<'Key, 'Value>) =
        let rec loop (m: MapTree<'Key, 'Value>) acc =
            if isEmpty m then
                acc
            else if m.Height = 1 then
                (m.Key, m.Value) :: acc
            else
                let mn = asNode m
                loop mn.Left ((mn.Key, mn.Value) :: loop mn.Right acc)

        loop m []

    let toArray m =
        m |> toList |> Array.ofList

    let ofList comparer l =
        List.fold (fun acc (k, v) -> add comparer k v acc) empty l

    let rec mkFromEnumerator comparer acc (e: IEnumerator<_>) =
        if e.MoveNext() then
            let (x, y) = e.Current
            mkFromEnumerator comparer (add comparer x y acc) e
        else
            acc

    let ofArray comparer (arr: array<'Key * 'Value>) =
        let mutable res = empty

        for (x, y) in arr do
            res <- add comparer x y res

        res

    let ofSeq comparer (c: seq<'Key * 'T>) =
        match c with
        | :? (('Key * 'T)[]) as xs -> ofArray comparer xs
        | :? (('Key * 'T) list) as xs -> ofList comparer xs
        | _ ->
            use ie = c.GetEnumerator()
            mkFromEnumerator comparer empty ie

    let copyToArray m (arr: _[]) i =
        let mutable j = i

        m
        |> iter (fun x y ->
            arr.[j] <- KeyValuePair(x, y)
            j <- j + 1)

    /// Imperative left-to-right iterators.
    [<NoEquality; NoComparison>]
    type MapIterator<'Key, 'Value when 'Key: comparison> =
        {
            /// invariant: always collapseLHS result
            mutable stack: MapTree<'Key, 'Value> list

            /// true when MoveNext has been called
            mutable started: bool
        }

    // collapseLHS:
    // a) Always returns either [] or a list starting with MapOne.
    // b) The "fringe" of the set stack is unchanged.
    let rec collapseLHS (stack: MapTree<'Key, 'Value> list) =
        match stack with
        | [] -> []
        | m :: rest ->
            if isEmpty m then
                collapseLHS rest
            else if m.Height = 1 then
                stack
            else
                let mn = asNode m
                collapseLHS (mn.Left :: MapTree(mn.Key, mn.Value) :: mn.Right :: rest)

    let mkIterator m =
        {
            stack = collapseLHS [ m ]
            started = false
        }

    let notStarted () =
        raise (InvalidOperationException(SR.GetString(SR.enumerationNotStarted)))

    let alreadyFinished () =
        raise (InvalidOperationException(SR.GetString(SR.enumerationAlreadyFinished)))

    let unexpectedStackForCurrent () =
        failwith "Please report error: Map iterator, unexpected stack for current"

    let unexpectedStackForMoveNext () =
        failwith "Please report error: Map iterator, unexpected stack for moveNext"

    let current i =
        if i.started then
            match i.stack with
            | [] -> alreadyFinished ()
            | m :: _ ->
                if m.Height = 1 then
                    KeyValuePair<_, _>(m.Key, m.Value)
                else
                    unexpectedStackForCurrent ()
        else
            notStarted ()

    let rec moveNext i =
        if i.started then
            match i.stack with
            | [] -> false
            | m :: rest ->
                if m.Height = 1 then
                    i.stack <- collapseLHS rest
                    not i.stack.IsEmpty
                else
                    unexpectedStackForMoveNext ()
        else
            i.started <- true (* The first call to MoveNext "starts" the enumeration. *)
            not i.stack.IsEmpty

    let mkIEnumerator m =
        let mutable i = mkIterator m

        { new IEnumerator<_> with
            member _.Current = current i
          interface System.Collections.IEnumerator with
              member _.Current = box (current i)

              member _.MoveNext() =
                  moveNext i

              member _.Reset() =
                  i <- mkIterator m
          interface System.IDisposable with
              member _.Dispose() =
                  ()
        }

    let rec leftmost m =
        if isEmpty m then
            throwKeyNotFound ()
        else if m.Height = 1 then
            (m.Key, m.Value)
        else
            let nd = asNode m

            if isNull nd.Left then
                (m.Key, m.Value)
            else
                leftmost nd.Left

    let rec rightmost m =
        if isEmpty m then
            throwKeyNotFound ()
        else if m.Height = 1 then
            (m.Key, m.Value)
        else
            let nd = asNode m

            if isNull nd.Right then
                (m.Key, m.Value)
            else
                rightmost nd.Right

[<System.Diagnostics.DebuggerTypeProxy(typedefof<MapDebugView<_, _>>)>]
[<System.Diagnostics.DebuggerDisplay("Count = {Count}")>]
[<Sealed>]
[<CompiledName("FSharpMap`2")>]
type Map<[<EqualityConditionalOn>] 'Key, [<EqualityConditionalOn; ComparisonConditionalOn>] 'Value when 'Key: comparison>
    (
        comparer: IComparer<'Key>,
        tree: MapTree<'Key, 'Value>
    ) =

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

    // We use .NET generics per-instantiation static fields to avoid allocating a new object for each empty
    // set (it is just a lookup into a .NET table of type-instantiation-indexed static fields).
    static let empty =
        let comparer = LanguagePrimitives.FastGenericComparer<'Key>
        new Map<'Key, 'Value>(comparer, MapTree.empty)

    [<System.Runtime.Serialization.OnSerializingAttribute>]
    member _.OnSerializing(context: System.Runtime.Serialization.StreamingContext) =
        ignore context
        serializedData <- MapTree.toArray tree |> Array.map (fun (k, v) -> KeyValuePair(k, v))

    // Do not set this to null, since concurrent threads may also be serializing the data
    //[<System.Runtime.Serialization.OnSerializedAttribute>]
    //member _.OnSerialized(context: System.Runtime.Serialization.StreamingContext) =
    //    serializedData <- null

    [<System.Runtime.Serialization.OnDeserializedAttribute>]
    member _.OnDeserialized(context: System.Runtime.Serialization.StreamingContext) =
        ignore context
        comparer <- LanguagePrimitives.FastGenericComparer<'Key>

        tree <-
            serializedData
            |> Array.map (fun kvp -> kvp.Key, kvp.Value)
            |> MapTree.ofArray comparer

        serializedData <- null

    static member Empty: Map<'Key, 'Value> = empty

    static member Create(ie: IEnumerable<_>) : Map<'Key, 'Value> =
        let comparer = LanguagePrimitives.FastGenericComparer<'Key>
        new Map<_, _>(comparer, MapTree.ofSeq comparer ie)

    new(elements: seq<_>) =
        let comparer = LanguagePrimitives.FastGenericComparer<'Key>
        new Map<_, _>(comparer, MapTree.ofSeq comparer elements)

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member internal m.Comparer = comparer

    //[<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member internal m.Tree = tree

    member m.Add(key, value) : Map<'Key, 'Value> =
#if TRACE_SETS_AND_MAPS
        MapTree.report ()
        MapTree.numAdds <- MapTree.numAdds + 1
        let size = MapTree.size m.Tree + 1
        MapTree.totalSizeOnMapAdd <- MapTree.totalSizeOnMapAdd + float size

        if size > MapTree.largestMapSize then
            MapTree.largestMapSize <- size
            MapTree.largestMapStackTrace <- System.Diagnostics.StackTrace().ToString()
#endif
        new Map<'Key, 'Value>(comparer, MapTree.add comparer key value tree)

    member m.Change(key, f) : Map<'Key, 'Value> =
        new Map<'Key, 'Value>(comparer, MapTree.change comparer key f tree)

    [<DebuggerBrowsable(DebuggerBrowsableState.Never)>]
    member m.IsEmpty = MapTree.isEmpty tree

    member m.Item
        with get (key: 'Key) =
#if TRACE_SETS_AND_MAPS
            MapTree.report ()
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

    member m.FoldSection (lo: 'Key) (hi: 'Key) f (acc: 'z) =
        MapTree.foldSection comparer lo hi f tree acc

    member m.Iterate f =
        MapTree.iter f tree

    member m.MapRange(f: 'Value -> 'Result) =
        new Map<'Key, 'Result>(comparer, MapTree.map f tree)

    member m.Map f =
        new Map<'Key, 'b>(comparer, MapTree.mapi f tree)

    member m.Partition predicate : Map<'Key, 'Value> * Map<'Key, 'Value> =
        let r1, r2 = MapTree.partition comparer predicate tree
        new Map<'Key, 'Value>(comparer, r1), new Map<'Key, 'Value>(comparer, r2)

    member m.Count = MapTree.size tree

    member m.ContainsKey key =
#if TRACE_SETS_AND_MAPS
        MapTree.report ()
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
        MapTree.report ()
        MapTree.numLookups <- MapTree.numLookups + 1
        MapTree.totalSizeOnMapLookup <- MapTree.totalSizeOnMapLookup + float (MapTree.size tree)
#endif
        MapTree.tryFind comparer key tree

    member m.ToList() =
        MapTree.toList tree

    member m.ToArray() =
        MapTree.toArray tree

    member m.Keys = KeyCollection(m) :> ICollection<'Key>

    member m.Values = ValueCollection(m) :> ICollection<'Value>

    member m.MinKeyValue = MapTree.leftmost tree
    member m.MaxKeyValue = MapTree.rightmost tree

    static member ofList l : Map<'Key, 'Value> =
        let comparer = LanguagePrimitives.FastGenericComparer<'Key>
        new Map<_, _>(comparer, MapTree.ofList comparer l)

    member this.ComputeHashCode() =
        let combineHash x y =
            (x <<< 1) + y + 631

        let mutable res = 0

        for (KeyValue (x, y)) in this do
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

                (m1 = m2)
                && (not m1
                    || (let e1c = e1.Current
                        let e2c = e2.Current
                        ((e1c.Key = e2c.Key) && (Unchecked.equals e1c.Value e2c.Value) && loop ())))

            loop ()
        | _ -> false

    override this.GetHashCode() =
        this.ComputeHashCode()

    interface IEnumerable<KeyValuePair<'Key, 'Value>> with
        member _.GetEnumerator() =
            MapTree.mkIEnumerator tree

    interface IEnumerable with
        member _.GetEnumerator() =
            (MapTree.mkIEnumerator tree :> IEnumerator)

    interface IDictionary<'Key, 'Value> with
        member m.Item
            with get x = m.[x]
            and set _ _ = raise (NotSupportedException(SR.GetString(SR.mapCannotBeMutated)))

        member m.Keys = m.Keys

        member m.Values = m.Values

        member m.Add(_, _) =
            raise (NotSupportedException(SR.GetString(SR.mapCannotBeMutated)))

        member m.ContainsKey k =
            m.ContainsKey k

        member m.TryGetValue(k, r) =
            m.TryGetValue(k, &r)

        member m.Remove(_) =
            raise (NotSupportedException(SR.GetString(SR.mapCannotBeMutated)))

    interface ICollection<KeyValuePair<'Key, 'Value>> with
        member _.Add(_) =
            raise (NotSupportedException(SR.GetString(SR.mapCannotBeMutated)))

        member _.Clear() =
            raise (NotSupportedException(SR.GetString(SR.mapCannotBeMutated)))

        member _.Remove(_) =
            raise (NotSupportedException(SR.GetString(SR.mapCannotBeMutated)))

        member m.Contains x =
            m.ContainsKey x.Key && Unchecked.equals m.[x.Key] x.Value

        member _.CopyTo(arr, i) =
            MapTree.copyToArray tree arr i

        member _.IsReadOnly = true

        member m.Count = m.Count

    interface System.IComparable with
        member m.CompareTo(obj: obj) =
            match obj with
            | :? Map<'Key, 'Value> as m2 ->
                Seq.compareWith
                    (fun (kvp1: KeyValuePair<_, _>) (kvp2: KeyValuePair<_, _>) ->
                        let c = comparer.Compare(kvp1.Key, kvp2.Key) in

                        if c <> 0 then
                            c
                        else
                            Unchecked.compare kvp1.Value kvp2.Value)
                    m
                    m2
            | _ -> invalidArg "obj" (SR.GetString(SR.notComparable))

    interface IReadOnlyCollection<KeyValuePair<'Key, 'Value>> with
        member m.Count = m.Count

    interface IReadOnlyDictionary<'Key, 'Value> with

        member m.Item
            with get key = m.[key]

        member m.Keys = m.Keys :> IEnumerable<'Key>

        member m.TryGetValue(key, value: byref<'Value>) =
            m.TryGetValue(key, &value)

        member m.Values = m.Values :> IEnumerable<'Value>

        member m.ContainsKey key =
            m.ContainsKey key

    override x.ToString() =
        match List.ofSeq (Seq.truncate 4 x) with
        | [] -> "map []"
        | [ KeyValue h1 ] ->
            let txt1 = LanguagePrimitives.anyToStringShowingNull h1
            StringBuilder().Append("map [").Append(txt1).Append("]").ToString()
        | [ KeyValue h1; KeyValue h2 ] ->
            let txt1 = LanguagePrimitives.anyToStringShowingNull h1
            let txt2 = LanguagePrimitives.anyToStringShowingNull h2

            StringBuilder()
                .Append("map [")
                .Append(txt1)
                .Append("; ")
                .Append(txt2)
                .Append("]")
                .ToString()
        | [ KeyValue h1; KeyValue h2; KeyValue h3 ] ->
            let txt1 = LanguagePrimitives.anyToStringShowingNull h1
            let txt2 = LanguagePrimitives.anyToStringShowingNull h2
            let txt3 = LanguagePrimitives.anyToStringShowingNull h3

            StringBuilder()
                .Append("map [")
                .Append(txt1)
                .Append("; ")
                .Append(txt2)
                .Append("; ")
                .Append(txt3)
                .Append("]")
                .ToString()
        | KeyValue h1 :: KeyValue h2 :: KeyValue h3 :: _ ->
            let txt1 = LanguagePrimitives.anyToStringShowingNull h1
            let txt2 = LanguagePrimitives.anyToStringShowingNull h2
            let txt3 = LanguagePrimitives.anyToStringShowingNull h3

            StringBuilder()
                .Append("map [")
                .Append(txt1)
                .Append("; ")
                .Append(txt2)
                .Append("; ")
                .Append(txt3)
                .Append("; ... ]")
                .ToString()

and [<Sealed>] MapDebugView<'Key, 'Value when 'Key: comparison>(v: Map<'Key, 'Value>) =

    [<DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>]
    member x.Items =
        v |> Seq.truncate 10000 |> Seq.map KeyValuePairDebugFriendly |> Seq.toArray

and [<DebuggerDisplay("{keyValue.Value}", Name = "[{keyValue.Key}]", Type = "")>] KeyValuePairDebugFriendly<'Key, 'Value>
    (
        keyValue: KeyValuePair<'Key, 'Value>
    ) =

    [<DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>]
    member x.KeyValue = keyValue

and KeyCollection<'Key, 'Value when 'Key: comparison>(parent: Map<'Key, 'Value>) =
    interface ICollection<'Key> with
        member _.Add(_) =
            raise (NotSupportedException(SR.GetString(SR.mapCannotBeMutated)))

        member _.Clear() =
            raise (NotSupportedException(SR.GetString(SR.mapCannotBeMutated)))

        member _.Remove(_) =
            raise (NotSupportedException(SR.GetString(SR.mapCannotBeMutated)))

        member _.Contains x =
            parent.ContainsKey x

        member _.CopyTo(arr, index) =
            if isNull arr then nullArg "arr"

            if index < 0 then
                invalidArg "index" "index must be positive"

            if index + parent.Count > arr.Length then
                invalidArg "index" "array is smaller than index plus the number of items to copy"

            let mutable i = index

            for item in parent do
                arr.[i] <- item.Key
                i <- i + 1

        member _.IsReadOnly = true

        member _.Count = parent.Count

    interface IEnumerable<'Key> with
        member _.GetEnumerator() =
            (seq {
                for item in parent do
                    item.Key
            })
                .GetEnumerator()

    interface IEnumerable with
        member _.GetEnumerator() =
            (seq {
                for item in parent do
                    item.Key
            })
                .GetEnumerator()
            :> IEnumerator

and ValueCollection<'Key, 'Value when 'Key: comparison>(parent: Map<'Key, 'Value>) =
    interface ICollection<'Value> with
        member _.Add(_) =
            raise (NotSupportedException(SR.GetString(SR.mapCannotBeMutated)))

        member _.Clear() =
            raise (NotSupportedException(SR.GetString(SR.mapCannotBeMutated)))

        member _.Remove(_) =
            raise (NotSupportedException(SR.GetString(SR.mapCannotBeMutated)))

        member _.Contains x =
            parent.Exists(fun _ value -> Unchecked.equals value x)

        member _.CopyTo(arr, index) =
            if isNull arr then nullArg "arr"

            if index < 0 then
                invalidArg "index" "index must be positive"

            if index + parent.Count > arr.Length then
                invalidArg "index" "array is smaller than index plus the number of items to copy"

            let mutable i = index

            for item in parent do
                arr.[i] <- item.Value
                i <- i + 1

        member _.IsReadOnly = true

        member _.Count = parent.Count

    interface IEnumerable<'Value> with
        member _.GetEnumerator() =
            (seq {
                for item in parent do
                    item.Value
            })
                .GetEnumerator()

    interface IEnumerable with
        member _.GetEnumerator() =
            (seq {
                for item in parent do
                    item.Value
            })
                .GetEnumerator()
            :> IEnumerator

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Map =

    [<CompiledName("IsEmpty")>]
    let isEmpty (table: Map<_, _>) =
        table.IsEmpty

    [<CompiledName("Add")>]
    let add key value (table: Map<_, _>) =
        table.Add(key, value)

    [<CompiledName("Change")>]
    let change key f (table: Map<_, _>) =
        table.Change(key, f)

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
    let fold<'Key, 'T, 'State when 'Key: comparison> folder (state: 'State) (table: Map<'Key, 'T>) =
        MapTree.fold folder state table.Tree

    [<CompiledName("FoldBack")>]
    let foldBack<'Key, 'T, 'State when 'Key: comparison> folder (table: Map<'Key, 'T>) (state: 'State) =
        MapTree.foldBack folder table.Tree state

    [<CompiledName("ToSeq")>]
    let toSeq (table: Map<_, _>) =
        table |> Seq.map (fun kvp -> kvp.Key, kvp.Value)

    [<CompiledName("FindKey")>]
    let findKey predicate (table: Map<_, _>) =
        table
        |> Seq.pick (fun kvp ->
            let k = kvp.Key in

            if predicate k kvp.Value then Some k else None)

    [<CompiledName("TryFindKey")>]
    let tryFindKey predicate (table: Map<_, _>) =
        table
        |> Seq.tryPick (fun kvp ->
            let k = kvp.Key in

            if predicate k kvp.Value then Some k else None)

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
    let empty<'Key, 'Value when 'Key: comparison> = Map<'Key, 'Value>.Empty

    [<CompiledName("Count")>]
    let count (table: Map<_, _>) =
        table.Count

    [<CompiledName("Keys")>]
    let keys (table: Map<_, _>) =
        table.Keys

    [<CompiledName("Values")>]
    let values (table: Map<_, _>) =
        table.Values

    [<CompiledName("MinKeyValue")>]
    let minKeyValue (table: Map<_, _>) =
        table.MinKeyValue

    [<CompiledName("MaxKeyValue")>]
    let maxKeyValue (table: Map<_, _>) =
        table.MaxKeyValue
