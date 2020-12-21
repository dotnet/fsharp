// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Collections

open System
open System.Collections
open System.Collections.Generic
open System.Diagnostics
open System.Text
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.LanguagePrimitives
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
open Microsoft.FSharp.Core.Operators
open Microsoft.FSharp.Collections

module Sorting =


    let inline private mergeSeq (cmp : IComparer<'Key>) (li : int) (ri : int) (len : int) (src : ('Key * 'Value)[]) (dst : ('Key * 'Value)[]) (length : int) =
        let le = ri
        let re = min length (ri + len)
        let mutable oi = li
        let mutable li = li
        let mutable ri = ri

        while li < le && ri < re do
            let lv = src.[li]
            let rv = src.[ri]
            let c = cmp.Compare(fst lv, fst rv)
            if c <= 0 then
                dst.[oi] <- lv
                oi <- oi + 1
                li <- li + 1
            else
                dst.[oi] <- rv
                oi <- oi + 1
                ri <- ri + 1

        while li < le do
            dst.[oi] <- src.[li]
            oi <- oi + 1
            li <- li + 1
                
        while ri < re do
            dst.[oi] <- src.[ri]
            oi <- oi + 1
            ri <- ri + 1

    let inline private mergeSeqHandleDuplicates (cmp : IComparer<'Key>) (li : int) (ri : int) (len : int) (src : ('Key * 'Value)[]) (dst : ('Key * 'Value)[]) (length : int) =
        let le = ri
        let re = min length (ri + len)
        let start = li
        let mutable oi = li
        let mutable li = li
        let mutable ri = ri
        let mutable lastValue = Unchecked.defaultof<'Key * 'Value>

        let inline append (v : ('Key * 'Value)) =
            if oi > start && cmp.Compare(fst v, fst lastValue) = 0 then
                dst.[oi-1] <- v
                lastValue <- v
            else
                dst.[oi] <- v
                lastValue <- v
                oi <- oi + 1

        while li < le && ri < re do
            let lv = src.[li]
            let rv = src.[ri]
            let c = cmp.Compare(fst lv, fst rv)
            if c <= 0 then
                append lv
                li <- li + 1
            else
                append rv
                ri <- ri + 1

        while li < le do
            append src.[li]
            li <- li + 1
                
        while ri < re do
            append src.[ri]
            ri <- ri + 1

        oi
        
    // assumes length > 2
    let mergeSortHandleDuplicates (mutateArray : bool) (cmp : IComparer<'Key>) (arr : ('Key * 'Value)[]) (length : int) =
        let mutable src = Array.zeroCreate length
        let mutable dst = 
            // mutateArray => allowed to mutate arr
            if mutateArray then arr
            else Array.zeroCreate length

        // copy to sorted pairs
        let mutable i0 = 0
        let mutable i1 = 1
        while i1 < length do
            let va = arr.[i0]
            let vb = arr.[i1]
            let c = cmp.Compare(fst va, fst vb)
            if c <= 0 then
                src.[i0] <- va
                src.[i1] <- vb
            else
                src.[i0] <- vb
                src.[i1] <- va
                    
            i0 <- i0 + 2
            i1 <- i1 + 2

        if i0 < length then
            src.[i0] <- arr.[i0]
            i0 <- i0 + 1


        // merge sorted parts of length `sortedLength`
        let mutable sortedLength = 2
        let mutable sortedLengthDbl = 4
        while sortedLengthDbl < length do
            let mutable li = 0
            let mutable ri = sortedLength

            // merge case
            while ri < length do
                mergeSeq cmp li ri sortedLength src dst length
                li <- ri + sortedLength
                ri <- li + sortedLength

            // right got empty
            while li < length do
                dst.[li] <- src.[li]
                li <- li + 1
                    
            // sortedLength * 2
            sortedLength <- sortedLengthDbl
            sortedLengthDbl <- sortedLengthDbl <<< 1
            // swap src and dst
            let t = dst
            dst <- src
            src <- t

        // final merge-dedup run
        let cnt = mergeSeqHandleDuplicates cmp 0 sortedLength sortedLength src dst length
        struct(dst, cnt)

    let inline private mergeSeqV (cmp : IComparer<'Key>) (li : int) (ri : int) (len : int) (src : struct('Key * 'Value)[]) (dst : struct('Key * 'Value)[]) (length : int) =
        let le = ri
        let re = min length (ri + len)
        let mutable oi = li
        let mutable li = li
        let mutable ri = ri

        while li < le && ri < re do
            let struct(lk, lv) = src.[li]
            let struct(rk, rv) = src.[ri]
            let c = cmp.Compare(lk, rk)
            if c <= 0 then
                dst.[oi] <- struct(lk, lv)
                oi <- oi + 1
                li <- li + 1
            else
                dst.[oi] <- struct(rk, rv)
                oi <- oi + 1
                ri <- ri + 1

        while li < le do
            dst.[oi] <- src.[li]
            oi <- oi + 1
            li <- li + 1
                
        while ri < re do
            dst.[oi] <- src.[ri]
            oi <- oi + 1
            ri <- ri + 1

    let inline private mergeSeqHandleDuplicatesV (cmp : IComparer<'Key>) (li : int) (ri : int) (len : int) (src : struct('Key * 'Value)[]) (dst : struct('Key * 'Value)[]) (length : int) =
        let le = ri
        let re = min length (ri + len)
        let start = li
        let mutable oi = li
        let mutable li = li
        let mutable ri = ri
        let mutable lastKey = Unchecked.defaultof<'Key>

        let inline append k v =
            if oi > start && cmp.Compare(k, lastKey) = 0 then
                dst.[oi-1] <- struct(k,v)
                lastKey <- k
            else
                dst.[oi] <- struct(k,v)
                lastKey <- k
                oi <- oi + 1

        while li < le && ri < re do
            let struct(lk, lv) = src.[li]
            let struct(rk, rv) = src.[ri]
            let c = cmp.Compare(lk, rk)
            if c <= 0 then
                append lk lv
                li <- li + 1
            else
                append rk rv
                ri <- ri + 1

        while li < le do
            let struct(k,v) = src.[li]
            append k v
            li <- li + 1
                
        while ri < re do
            let struct(k,v) = src.[ri]
            append k v
            ri <- ri + 1

        oi
        
    // assumes length > 2
    let mergeSortHandleDuplicatesV (mutateArray : bool) (cmp : IComparer<'Key>) (arr : struct('Key * 'Value)[]) (length : int) =
        let mutable src = Array.zeroCreate length
        let mutable dst = 
            // mutateArray => allowed to mutate arr
            if mutateArray then arr
            else Array.zeroCreate length

        // copy to sorted pairs
        let mutable i0 = 0
        let mutable i1 = 1
        while i1 < length do
            let struct(ka,va) = arr.[i0] 
            let struct(kb,vb) = arr.[i1]

            let c = cmp.Compare(ka, kb)
            if c <= 0 then
                src.[i0] <- struct(ka, va)
                src.[i1] <- struct(kb, vb)
            else
                src.[i0] <- struct(kb, vb)
                src.[i1] <- struct(ka, va)
                    
            i0 <- i0 + 2
            i1 <- i1 + 2

        if i0 < length then
            src.[i0] <- arr.[i0]
            i0 <- i0 + 1


        // merge sorted parts of length `sortedLength`
        let mutable sortedLength = 2
        let mutable sortedLengthDbl = 4
        while sortedLengthDbl < length do
            let mutable li = 0
            let mutable ri = sortedLength

            // merge case
            while ri < length do
                mergeSeqV cmp li ri sortedLength src dst length
                li <- ri + sortedLength
                ri <- li + sortedLength

            // right got empty
            while li < length do
                dst.[li] <- src.[li]
                li <- li + 1
                    
            // sortedLength * 2
            sortedLength <- sortedLengthDbl
            sortedLengthDbl <- sortedLengthDbl <<< 1
            // swap src and dst
            let t = dst
            dst <- src
            src <- t


        // final merge-dedup run
        let cnt = mergeSeqHandleDuplicatesV cmp 0 sortedLength sortedLength src dst length
        struct(dst, cnt)


    let sortHandleDuplicates (mutateArray : bool) (cmp : IComparer<'T>) (arr : 'T[]) (length : int) =
        if length <= 0 then
            struct(arr, 0)
        else
            let arr =
                if mutateArray then arr
                else arr.[0 .. length-1]
            System.Array.Sort(arr, 0, length, cmp)

            let mutable i = 1
            let mutable oi = 1
            let mutable last = arr.[0]
            while i < length do
                let v = arr.[i]
                let c = cmp.Compare(last, v)
                last <- v
                i <- i + 1
                if c = 0 then
                    arr.[oi-1] <- v
                else
                    arr.[oi] <- v
                    oi <- oi + 1

            struct(arr, oi)
            

module MapImplementation = 

    

    [<AbstractClass>]
    type MapNode<'Key, 'Value>() =
        abstract member Count : int
        abstract member Height : int

        abstract member Add : comparer : IComparer<'Key> * key : 'Key * value : 'Value -> MapNode<'Key, 'Value>
        abstract member AddIfNotPresent : comparer : IComparer<'Key> * key : 'Key * value : 'Value -> MapNode<'Key, 'Value>
        abstract member Remove : comparer : IComparer<'Key> * key : 'Key -> MapNode<'Key, 'Value>
        abstract member AddInPlace : comparer : IComparer<'Key> * key : 'Key * value : 'Value -> MapNode<'Key, 'Value>
        abstract member TryRemove : comparer : IComparer<'Key> * key : 'Key -> option<MapNode<'Key,'Value> * 'Value>
        abstract member TryRemoveV : comparer : IComparer<'Key> * key : 'Key -> voption<struct(MapNode<'Key,'Value> * 'Value)>

        abstract member Map : mapping : OptimizedClosures.FSharpFunc<'Key, 'Value, 'T> -> MapNode<'Key, 'T>
        abstract member Filter : predicate : OptimizedClosures.FSharpFunc<'Key, 'Value, bool> -> MapNode<'Key, 'Value>
        abstract member Choose : mapping : OptimizedClosures.FSharpFunc<'Key, 'Value, option<'T>> -> MapNode<'Key, 'T>
        abstract member ChooseV : mapping : OptimizedClosures.FSharpFunc<'Key, 'Value, voption<'T>> -> MapNode<'Key, 'T>

        abstract member UnsafeRemoveHeadV : unit -> struct('Key * 'Value * MapNode<'Key, 'Value>)
        abstract member UnsafeRemoveTailV : unit -> struct(MapNode<'Key, 'Value> * 'Key * 'Value)
        
        abstract member GetViewBetween : comparer : IComparer<'Key> * min : 'Key * minInclusive : bool * max : 'Key * maxInclusive : bool -> MapNode<'Key, 'Value>
        abstract member WithMin : comparer : IComparer<'Key> * min : 'Key * minInclusive : bool -> MapNode<'Key, 'Value>
        abstract member WithMax : comparer : IComparer<'Key> * max : 'Key * maxInclusive : bool -> MapNode<'Key, 'Value>
        abstract member SplitV : comparer : IComparer<'Key> * key : 'Key -> struct(MapNode<'Key, 'Value> * MapNode<'Key, 'Value> * voption<'Value>)
        
        abstract member Change : comparer : IComparer<'Key> * key : 'Key * (option<'Value> -> option<'Value>) -> MapNode<'Key, 'Value>
        abstract member ChangeV : comparer : IComparer<'Key> * key : 'Key * (voption<'Value> -> voption<'Value>) -> MapNode<'Key, 'Value>
        
        //// find, findKey tryFindKey, pick, partition, tryPick
        //abstract member TryFindKey : pick : OptimizedClosures.FSharpFunc<'Key, 'Value, bool> -> option<'Key>
        //abstract member TryFindKeyV : pick : OptimizedClosures.FSharpFunc<'Key, 'Value, bool> -> voption<'Key>
        //abstract member TryPick : pick : OptimizedClosures.FSharpFunc<'Key, 'Value, option<'T>> -> option<'T>
        //abstract member TryPickV : pick : OptimizedClosures.FSharpFunc<'Key, 'Value, voption<'T>> -> voption<'T>


    and [<Sealed>]
        MapEmpty<'Key, 'Value> private() =
        inherit MapNode<'Key, 'Value>()

        static let instance = MapEmpty<'Key, 'Value>() :> MapNode<_,_>

        static member Instance : MapNode<'Key, 'Value> = instance

        override x.Count = 0
        override x.Height = 0
        override x.Add(_, key, value) =
            MapLeaf(key, value) :> MapNode<_,_>
            
        override x.AddIfNotPresent(_, key, value) =
            MapLeaf(key, value) :> MapNode<_,_>

        override x.AddInPlace(_, key, value) =
            MapLeaf(key, value) :> MapNode<_,_>

        override x.Remove(_,_) =
            x :> MapNode<_,_>
            
        override x.TryRemove(_,_) =
            None

        override x.TryRemoveV(_,_) =
            ValueNone

        override x.Map(_) = MapEmpty.Instance
        override x.Filter(_) = x :> MapNode<_,_>
        override x.Choose(_) = MapEmpty.Instance
        override x.ChooseV(_) = MapEmpty.Instance

        override x.UnsafeRemoveHeadV() = failwith "empty"
        override x.UnsafeRemoveTailV() = failwith "empty"

        override x.GetViewBetween(_comparer : IComparer<'Key>, _min : 'Key, _minInclusive : bool, _max : 'Key, _maxInclusive : bool) =
            x :> MapNode<_,_>
        override x.WithMin(_comparer : IComparer<'Key>, _min : 'Key, _minInclusive : bool) =
            x :> MapNode<_,_>
        override x.WithMax(_comparer : IComparer<'Key>, _max : 'Key, _maxInclusive : bool) =
            x :> MapNode<_,_>

        override x.SplitV(_,_) =
            (x :> MapNode<_,_>, x :> MapNode<_,_>, ValueNone)

        override x.Change(_comparer, key, update) =
            match update None with
            | None -> x :> MapNode<_,_>
            | Some v -> MapLeaf(key, v) :> MapNode<_,_>
            
        override x.ChangeV(_comparer, key, update) =
            match update ValueNone with
            | ValueNone -> x :> MapNode<_,_>
            | ValueSome v -> MapLeaf(key, v) :> MapNode<_,_>

    and [<Sealed>]
        MapLeaf<'Key, 'Value> =
        class 
            inherit MapNode<'Key, 'Value>
            val mutable public Key : 'Key
            val mutable public Value : 'Value

            override x.Height =
                1

            override x.Count =
                1

            override x.Add(comparer, key, value) =
                let c = comparer.Compare(key, x.Key)

                if c > 0 then
                    MapInner(x, key, value, MapEmpty.Instance) :> MapNode<'Key,'Value>
                elif c < 0 then
                    MapInner(MapEmpty.Instance, key, value, x) :> MapNode<'Key,'Value>
                else
                    MapLeaf(key, value) :> MapNode<'Key,'Value>
                    
            override x.AddIfNotPresent(comparer, key, value) =
                let c = comparer.Compare(key, x.Key)

                if c > 0 then
                    MapInner(x, key, value, MapEmpty.Instance) :> MapNode<'Key,'Value>
                elif c < 0 then
                    MapInner(MapEmpty.Instance, key, value, x) :> MapNode<'Key,'Value>
                else
                    x :> MapNode<'Key,'Value>
                    
            override x.AddInPlace(comparer, key, value) =
                let c = comparer.Compare(key, x.Key)

                if c > 0 then   
                    MapInner(x, key, value, MapEmpty.Instance) :> MapNode<'Key,'Value>
                elif c < 0 then
                    MapInner(MapEmpty.Instance, key, value, x) :> MapNode<'Key,'Value>
                else
                    x.Key <- key
                    x.Value <- value
                    x :> MapNode<'Key,'Value>

                
            override x.Remove(comparer, key) =
                if comparer.Compare(key, x.Key) = 0 then MapEmpty.Instance
                else x :> MapNode<_,_>
                
            override x.TryRemove(comparer, key) =
                if comparer.Compare(key, x.Key) = 0 then Some(MapEmpty.Instance, x.Value)
                else None
                
            override x.TryRemoveV(comparer, key) =
                if comparer.Compare(key, x.Key) = 0 then ValueSome(MapEmpty.Instance, x.Value)
                else ValueNone

            override x.Map(mapping : OptimizedClosures.FSharpFunc<'Key, 'Value, 'T>) =
                MapLeaf(x.Key, mapping.Invoke(x.Key, x.Value)) :> MapNode<_,_>
                
            override x.Filter(predicate : OptimizedClosures.FSharpFunc<'Key, 'Value, bool>) =
                if predicate.Invoke(x.Key, x.Value) then
                    x :> MapNode<_,_>
                else
                    MapEmpty.Instance

            override x.Choose(mapping : OptimizedClosures.FSharpFunc<'Key, 'Value, option<'T>>) =
                match mapping.Invoke(x.Key, x.Value) with
                | Some v -> 
                    MapLeaf(x.Key, v) :> MapNode<_,_>
                | None ->
                    MapEmpty.Instance

            override x.ChooseV(mapping : OptimizedClosures.FSharpFunc<'Key, 'Value, voption<'T>>) =
                match mapping.Invoke(x.Key, x.Value) with
                | ValueSome v -> 
                    MapLeaf(x.Key, v) :> MapNode<_,_>
                | ValueNone ->
                    MapEmpty.Instance

            override x.UnsafeRemoveHeadV() =
                struct(x.Key, x.Value, MapEmpty<'Key, 'Value>.Instance)

            override x.UnsafeRemoveTailV() =
                struct(MapEmpty<'Key, 'Value>.Instance, x.Key, x.Value)

            
            override x.GetViewBetween(comparer : IComparer<'Key>, min : 'Key, minInclusive : bool, max : 'Key, maxInclusive : bool) =
                let cMin = comparer.Compare(x.Key, min)
                if (if minInclusive then cMin >= 0 else cMin > 0) then
                    let cMax = comparer.Compare(x.Key, max)
                    if (if maxInclusive then cMax <= 0 else cMax < 0) then
                        x :> MapNode<_,_>
                    else
                        MapEmpty.Instance
                else
                    MapEmpty.Instance
                    
            override x.WithMin(comparer : IComparer<'Key>, min : 'Key, minInclusive : bool) =
                let cMin = comparer.Compare(x.Key, min)
                if (if minInclusive then cMin >= 0 else cMin > 0) then
                    x :> MapNode<_,_>
                else
                    MapEmpty.Instance
                    
            override x.WithMax(comparer : IComparer<'Key>, max : 'Key, maxInclusive : bool) =
                let cMax = comparer.Compare(x.Key, max)
                if (if maxInclusive then cMax <= 0 else cMax < 0) then
                    x :> MapNode<_,_>
                else
                    MapEmpty.Instance
                    
            override x.SplitV(comparer : IComparer<'Key>, key : 'Key) =
                let c = comparer.Compare(x.Key, key)
                if c > 0 then
                    struct(MapEmpty.Instance, x :> MapNode<_,_>, ValueNone)
                elif c < 0 then
                    struct(x :> MapNode<_,_>, MapEmpty.Instance, ValueNone)
                else
                    struct(MapEmpty.Instance, MapEmpty.Instance, ValueSome x.Value)
                 
            override x.Change(comparer, key, update) =
                let c = comparer.Compare(key, x.Key)
                if c > 0 then
                    match update None with
                    | None -> x :> MapNode<_,_>
                    | Some v -> MapInner(x, key, v, MapEmpty.Instance) :> MapNode<_,_>
                elif c < 0 then
                    match update None with
                    | None -> x :> MapNode<_,_>
                    | Some v -> MapInner(MapEmpty.Instance, key, v, x) :> MapNode<_,_>
                else    
                    match update (Some x.Value) with
                    | Some v ->
                        MapLeaf(key, v) :> MapNode<_,_>
                    | None ->
                        MapEmpty.Instance

            override x.ChangeV(comparer, key, update) =
                let c = comparer.Compare(key, x.Key)
                if c > 0 then
                    match update ValueNone with
                    | ValueNone -> x :> MapNode<_,_>
                    | ValueSome v -> MapInner(x, key, v, MapEmpty.Instance) :> MapNode<_,_>
                elif c < 0 then
                    match update ValueNone with
                    | ValueNone -> x :> MapNode<_,_>
                    | ValueSome v -> MapInner(MapEmpty.Instance, key, v, x) :> MapNode<_,_>
                else    
                    match update (ValueSome x.Value) with
                    | ValueSome v ->
                        MapLeaf(key, v) :> MapNode<_,_>
                    | ValueNone ->
                        MapEmpty.Instance

            new(k : 'Key, v : 'Value) = { Key = k; Value = v}
        end

    and [<Sealed>]
        MapInner<'Key, 'Value> =
        class 
            inherit MapNode<'Key, 'Value>

            val mutable public Left : MapNode<'Key, 'Value>
            val mutable public Right : MapNode<'Key, 'Value>
            val mutable public Key : 'Key
            val mutable public Value : 'Value
            val mutable public _Count : int
            val mutable public _Height : int

            static member Create(l : MapNode<'Key, 'Value>, k : 'Key, v : 'Value, r : MapNode<'Key, 'Value>) =
                let lh = l.Height
                let rh = r.Height
                let b = rh - lh

                if lh = 0 && rh = 0 then
                    MapLeaf(k, v) :> MapNode<_,_>
                elif b > 2 then
                    // right heavy
                    let r = r :?> MapInner<'Key, 'Value> // must work
                    
                    if r.Right.Height >= r.Left.Height then
                        // right right case
                        MapInner.Create(
                            MapInner.Create(l, k, v, r.Left),
                            r.Key, r.Value,
                            r.Right
                        ) 
                    else
                        // right left case
                        match r.Left with
                        | :? MapInner<'Key, 'Value> as rl ->
                            //let rl = r.Left :?> MapInner<'Key, 'Value>
                            let t1 = l
                            let t2 = rl.Left
                            let t3 = rl.Right
                            let t4 = r.Right

                            MapInner.Create(
                                MapInner.Create(t1, k, v, t2),
                                rl.Key, rl.Value,
                                MapInner.Create(t3, r.Key, r.Value, t4)
                            )
                        | _ ->
                            failwith "impossible"
                            

                elif b < -2 then   
                    let l = l :?> MapInner<'Key, 'Value> // must work
                    
                    if l.Left.Height >= l.Right.Height then
                        MapInner.Create(
                            l.Left,
                            l.Key, l.Value,
                            MapInner.Create(l.Right, k, v, r)
                        )

                    else
                        match l.Right with
                        | :? MapInner<'Key, 'Value> as lr -> 
                            let t1 = l.Left
                            let t2 = lr.Left
                            let t3 = lr.Right
                            let t4 = r
                            MapInner.Create(
                                MapInner.Create(t1, l.Key, l.Value, t2),
                                lr.Key, lr.Value,
                                MapInner.Create(t3, k, v, t4)
                            )
                        | _ ->
                            failwith "impossible"

                else
                    MapInner(l, k, v, r) :> MapNode<_,_>

            static member Join(l : MapNode<'Key, 'Value>, r : MapNode<'Key, 'Value>) =
                if l.Height = 0 then r
                elif r.Height = 0 then l
                elif l.Height > r.Height then
                    let struct(l1, k, v) = l.UnsafeRemoveTailV()
                    MapInner.Create(l1, k, v, r)
                else
                    let struct(k, v, r1) = r.UnsafeRemoveHeadV()
                    MapInner.Create(l, k, v, r1)

            override x.Count =
                x._Count

            override x.Height =
                x._Height
            
            override x.Add(comparer : IComparer<'Key>, key : 'Key, value : 'Value) =
                let c = comparer.Compare(key, x.Key)
                if c > 0 then
                    MapInner.Create(
                        x.Left, 
                        x.Key, x.Value,
                        x.Right.Add(comparer, key, value)
                    )
                elif c < 0 then
                    MapInner.Create(
                        x.Left.Add(comparer, key, value), 
                        x.Key, x.Value,
                        x.Right
                    )
                else
                    MapInner(
                        x.Left, 
                        key, value,
                        x.Right
                    ) :> MapNode<_,_>
               
            override x.AddIfNotPresent(comparer : IComparer<'Key>, key : 'Key, value : 'Value) =
                let c = comparer.Compare(key, x.Key)
                if c > 0 then
                    MapInner.Create(
                        x.Left, 
                        x.Key, x.Value,
                        x.Right.AddIfNotPresent(comparer, key, value)
                    )
                elif c < 0 then
                    MapInner.Create(
                        x.Left.AddIfNotPresent(comparer, key, value), 
                        x.Key, x.Value,
                        x.Right
                    )
                else
                    x :> MapNode<_,_>
                     
            override x.AddInPlace(comparer : IComparer<'Key>, key : 'Key, value : 'Value) =
                let c = comparer.Compare(key, x.Key)
                if c > 0 then
                    x.Right <- x.Right.AddInPlace(comparer, key, value)

                    let bal = abs (x.Right.Height - x.Left.Height)
                    if bal < 2 then 
                        x._Height <- 1 + max x.Left.Height x.Right.Height
                        x._Count <- 1 + x.Right.Count + x.Left.Count
                        x :> MapNode<_,_>
                    else 
                        MapInner.Create(
                            x.Left, 
                            x.Key, x.Value,
                            x.Right
                        )
                elif c < 0 then
                    x.Left <- x.Left.AddInPlace(comparer, key, value)
                    
                    let bal = abs (x.Right.Height - x.Left.Height)
                    if bal < 2 then 
                        x._Height <- 1 + max x.Left.Height x.Right.Height
                        x._Count <- 1 + x.Right.Count + x.Left.Count
                        x :> MapNode<_,_>
                    else
                        MapInner.Create(
                            x.Left, 
                            x.Key, x.Value,
                            x.Right
                        )
                else
                    x.Key <- key
                    x.Value <- value
                    x :> MapNode<_,_>

            override x.Remove(comparer : IComparer<'Key>, key : 'Key) =
                let c = comparer.Compare(key, x.Key)
                if c > 0 then
                    MapInner.Create(
                        x.Left, 
                        x.Key, x.Value,
                        x.Right.Remove(comparer, key)
                    )
                elif c < 0 then
                    MapInner.Create(
                        x.Left.Remove(comparer, key), 
                        x.Key, x.Value,
                        x.Right
                    )
                else
                    MapInner.Join(x.Left, x.Right)
                    
            override x.TryRemove(comparer : IComparer<'Key>, key : 'Key) =
                let c = comparer.Compare(key, x.Key)
                if c > 0 then
                    match x.Right.TryRemoveV(comparer, key) with
                    | ValueSome struct(newRight, value) ->
                        let newNode = 
                            MapInner.Create(
                                x.Left, 
                                x.Key, x.Value,
                                newRight
                            )
                        Some(newNode, value)
                    | ValueNone ->
                        None
                elif c < 0 then
                    match x.Left.TryRemoveV(comparer, key) with
                    | ValueSome struct(newLeft, value) ->
                        let newNode = 
                            MapInner.Create(
                                newLeft, 
                                x.Key, x.Value,
                                x.Right
                            )
                        Some(newNode, value)
                    | ValueNone ->
                        None
                else
                    Some(MapInner.Join(x.Left, x.Right), x.Value)
                           
            override x.TryRemoveV(comparer : IComparer<'Key>, key : 'Key) =
                let c = comparer.Compare(key, x.Key)
                if c > 0 then
                    match x.Right.TryRemoveV(comparer, key) with
                    | ValueSome struct(newRight, value) ->
                        let newNode = 
                            MapInner.Create(
                                x.Left, 
                                x.Key, x.Value,
                                newRight
                            )
                        ValueSome(newNode, value)
                    | ValueNone ->
                        ValueNone
                elif c < 0 then
                    match x.Left.TryRemoveV(comparer, key) with
                    | ValueSome struct(newLeft, value) ->
                        let newNode = 
                            MapInner.Create(
                                newLeft, 
                                x.Key, x.Value,
                                x.Right
                            )
                        ValueSome(newNode, value)
                    | ValueNone ->
                        ValueNone
                else
                    ValueSome(MapInner.Join(x.Left, x.Right), x.Value)

            override x.Map(mapping : OptimizedClosures.FSharpFunc<'Key, 'Value, 'T>) =
                MapInner(
                    x.Left.Map(mapping),
                    x.Key, mapping.Invoke(x.Key, x.Value),
                    x.Right.Map(mapping)
                ) :> MapNode<_,_>
                
            override x.Filter(predicate : OptimizedClosures.FSharpFunc<'Key, 'Value, bool>) =
                let l = x.Left.Filter(predicate)
                let self = predicate.Invoke(x.Key, x.Value)
                let r = x.Right.Filter(predicate)

                if self then
                    MapInner.Create(l, x.Key, x.Value, r)
                else
                    MapInner.Join(l, r)

            override x.Choose(mapping : OptimizedClosures.FSharpFunc<'Key, 'Value, option<'T>>) =
                let l = x.Left.Choose(mapping)
                let self = mapping.Invoke(x.Key, x.Value)
                let r = x.Right.Choose(mapping)
                match self with
                | Some value ->
                    MapInner.Create(l, x.Key, value, r)
                | None ->
                    MapInner.Join(l, r)
                    

            override x.ChooseV(mapping : OptimizedClosures.FSharpFunc<'Key, 'Value, voption<'T>>) =
                let l = x.Left.ChooseV(mapping)
                let self = mapping.Invoke(x.Key, x.Value)
                let r = x.Right.ChooseV(mapping)
                match self with
                | ValueSome value ->
                    MapInner.Create(l, x.Key, value, r)
                | ValueNone ->
                    MapInner.Join(l, r)

            override x.UnsafeRemoveHeadV() =
                if x.Left.Count = 0 then
                    struct(x.Key, x.Value, x.Right)
                else
                    let struct(k,v,l1) = x.Left.UnsafeRemoveHeadV()
                    struct(k, v, MapInner.Create(l1, x.Key, x.Value, x.Right))

            override x.UnsafeRemoveTailV() =   
                if x.Right.Count = 0 then
                    struct(x.Left, x.Key, x.Value)
                else
                    let struct(r1,k,v) = x.Right.UnsafeRemoveTailV()
                    struct(MapInner.Create(x.Left, x.Key, x.Value, r1), k, v)
                    

            override x.WithMin(comparer : IComparer<'Key>, min : 'Key, minInclusive : bool) =
                let c = comparer.Compare(x.Key, min)
                let greaterMin = if minInclusive then c >= 0 else c > 0
                if greaterMin then
                    MapInner.Create(
                        x.Left.WithMin(comparer, min, minInclusive),
                        x.Key, x.Value,
                        x.Right
                    )
                else
                    x.Right.WithMin(comparer, min, minInclusive)

                
            override x.WithMax(comparer : IComparer<'Key>, max : 'Key, maxInclusive : bool) =
                let c = comparer.Compare(x.Key, max)
                let smallerMax = if maxInclusive then c <= 0 else c < 0
                if smallerMax then
                    MapInner.Create(
                        x.Left,
                        x.Key, x.Value,
                        x.Right.WithMax(comparer, max, maxInclusive)
                    )
                else
                    x.Left.WithMax(comparer, max, maxInclusive)
                    
                    
            override x.SplitV(comparer : IComparer<'Key>, key : 'Key) =
                let c = comparer.Compare(key, x.Key)
                if c > 0 then
                    let struct(rl, rr, rv) = x.Right.SplitV(comparer, key)
                    struct(MapInner.Create(x.Left, x.Key, x.Value, rl), rr, rv)
                elif c < 0 then
                    let struct(ll, lr, lv) = x.Left.SplitV(comparer, key)
                    struct(ll, MapInner.Create(lr, x.Key, x.Value, x.Right), lv)
                else
                    struct(x.Left, x.Right, ValueSome x.Value)

            override x.GetViewBetween(comparer : IComparer<'Key>, min : 'Key, minInclusive : bool, max : 'Key, maxInclusive : bool) =
                let cMin = comparer.Compare(x.Key, min)
                let cMax = comparer.Compare(x.Key, max)

                let greaterMin = if minInclusive then cMin >= 0 else cMin > 0
                let smallerMax = if maxInclusive then cMax <= 0 else cMax < 0

                if not greaterMin then
                    x.Right.GetViewBetween(comparer, min, minInclusive, max, maxInclusive)

                elif not smallerMax then
                    x.Left.GetViewBetween(comparer, min, minInclusive, max, maxInclusive)

                elif greaterMin && smallerMax then
                    let l = x.Left.WithMin(comparer, min, minInclusive)
                    let r = x.Right.WithMax(comparer, max, maxInclusive)
                    MapInner.Create(l, x.Key, x.Value, r)

                elif greaterMin then
                    let l = x.Left.GetViewBetween(comparer, min, minInclusive, max, maxInclusive)
                    let r = x.Right.WithMax(comparer, max, maxInclusive)
                    MapInner.Create(l, x.Key, x.Value, r)

                elif smallerMax then
                    let l = x.Left.WithMin(comparer, min, minInclusive)
                    let r = x.Right.GetViewBetween(comparer, min, minInclusive, max, maxInclusive)
                    MapInner.Create(l, x.Key, x.Value, r)
                    
                else
                    failwith "invalid range"

            override x.Change(comparer, key, update) =
                let c = comparer.Compare(key, x.Key)
                if c > 0 then   
                    MapInner.Create(
                        x.Left,
                        x.Key, x.Value,
                        x.Right.Change(comparer, key, update)
                    )
                elif c < 0 then 
                    MapInner.Create(
                        x.Left.Change(comparer, key, update),
                        x.Key, x.Value,
                        x.Right
                    )
                else    
                    match update (Some x.Value) with
                    | Some v ->
                        MapInner(
                            x.Left,
                            key, v,
                            x.Right
                        ) :> MapNode<_,_>
                    | None ->
                        MapInner.Join(x.Left, x.Right)
                        
            override x.ChangeV(comparer, key, update) =
                let c = comparer.Compare(key, x.Key)
                if c > 0 then   
                    MapInner.Create(
                        x.Left,
                        x.Key, x.Value,
                        x.Right.ChangeV(comparer, key, update)
                    )
                elif c < 0 then 
                    MapInner.Create(
                        x.Left.ChangeV(comparer, key, update),
                        x.Key, x.Value,
                        x.Right
                    )
                else    
                    match update (ValueSome x.Value) with
                    | ValueSome v ->
                        MapInner(
                            x.Left,
                            key, v,
                            x.Right
                        ) :> MapNode<_,_>
                    | ValueNone ->
                        MapInner.Join(x.Left, x.Right)

            new(l : MapNode<'Key, 'Value>, k : 'Key, v : 'Value, r : MapNode<'Key, 'Value>) =
                assert(l.Count > 0 || r.Count > 0)      // not both empty
                assert(abs (r.Height - l.Height) <= 2)  // balanced
                {
                    Left = l
                    Right = r
                    Key = k
                    Value = v
                    _Count = 1 + l.Count + r.Count
                    _Height = 1 + max l.Height r.Height
                }
        end

    
    let inline combineHash (a: int) (b: int) =
        uint32 a ^^^ uint32 b + 0x9e3779b9u + ((uint32 a) <<< 6) + ((uint32 a) >>> 2) |> int


    let hash (n : MapNode<'K, 'V>) =
        let rec hash (acc : int) (n : MapNode<'K, 'V>) =    
            match n with
            | :? MapLeaf<'K, 'V> as n ->
                combineHash acc (combineHash (Unchecked.hash n.Key) (Unchecked.hash n.Value))

            | :? MapInner<'K, 'V> as n ->
                let acc = hash acc n.Left
                let acc = combineHash acc (combineHash (Unchecked.hash n.Key) (Unchecked.hash n.Value))
                hash acc n.Right
            | _ ->
                acc

        hash 0 n

    let rec equals (cmp : IComparer<'K>) (l : MapNode<'K,'V>) (r : MapNode<'K,'V>) =
        if l.Count <> r.Count then
            false
        else
            // counts identical
            match l with
            | :? MapLeaf<'K, 'V> as l ->
                let r = r :?> MapLeaf<'K, 'V> // has to hold (r.Count = 1)
                cmp.Compare(l.Key, r.Key) = 0 &&
                Unchecked.equals l.Value r.Value

            | :? MapInner<'K, 'V> as l ->
                match r with
                | :? MapInner<'K, 'V> as r ->
                    let struct(ll, lr, lv) = l.SplitV(cmp, r.Key)
                    match lv with
                    | ValueSome lv when Unchecked.equals lv r.Value ->
                        equals cmp ll r.Left &&
                        equals cmp lr r.Right
                    | _ ->
                        false
                | _ ->
                    false
            | _ ->
                true

open MapImplementation
open System.Diagnostics
open System.Runtime.InteropServices

[<DebuggerTypeProxy("Aardvark.Base.MapDebugView`2")>]
[<DebuggerDisplay("Count = {Count}")>]
[<CompiledName("FSharpMap`2")>]
[<Sealed>]
type Map< [<EqualityConditionalOn>] 'Key, [<EqualityConditionalOn;ComparisonConditionalOn>] 'Value when 'Key : comparison> private(comparer : IComparer<'Key>, root : MapNode<'Key, 'Value>) =
        
    static let defaultComparer = LanguagePrimitives.FastGenericComparer<'Key>
    static let empty = Map<'Key, 'Value>(defaultComparer, MapEmpty.Instance)

    [<NonSerialized>]
    // This type is logically immutable. This field is only mutated during deserialization.
    let mutable comparer = comparer
    
    [<NonSerialized>]
    // This type is logically immutable. This field is only mutated during deserialization.
    let mutable root = root

    // WARNING: The compiled name of this field may never be changed because it is part of the logical
    // WARNING: permanent serialization format for this type.
    let mutable serializedData = null

    static let toKeyValueArray(root : MapNode<_,_>) =
        let arr = Array.zeroCreate root.Count
        let rec copyTo (arr : array<_>) (index : int) (n : MapNode<_,_>) =
            match n with
            | :? MapInner<'Key, 'Value> as n ->
                let i = copyTo arr index n.Left
                arr.[i] <- KeyValuePair(n.Key, n.Value)
                
                copyTo arr (i+1) n.Right
            | :? MapLeaf<'Key, 'Value> as n ->
                arr.[index] <- KeyValuePair(n.Key, n.Value)
                index + 1
            | _ ->
                index

        copyTo arr 0 root |> ignore<int>
        arr

    static let fromArray (elements : struct('Key * 'Value)[]) =
        let cmp = defaultComparer
        match elements.Length with
        | 0 -> 
            MapEmpty.Instance
        | 1 ->
            let struct(k,v) = elements.[0]
            MapLeaf(k, v) :> MapNode<_,_>
        | 2 -> 
            let struct(k0,v0) = elements.[0]
            let struct(k1,v1) = elements.[1]
            let c = cmp.Compare(k0, k1)
            if c > 0 then MapInner(MapEmpty.Instance, k1, v1, MapLeaf(k0, v0)) :> MapNode<_,_>
            elif c < 0 then MapInner(MapLeaf(k0, v0), k1, v1, MapEmpty.Instance) :> MapNode<_,_>
            else MapLeaf(k1, v1):> MapNode<_,_>
        | 3 ->
            let struct(k0,v0) = elements.[0]
            let struct(k1,v1) = elements.[1]
            let struct(k2,v2) = elements.[2]
            MapLeaf(k0, v0).AddInPlace(cmp, k1, v1).AddInPlace(cmp, k2, v2)
        | 4 ->
            let struct(k0,v0) = elements.[0]
            let struct(k1,v1) = elements.[1]
            let struct(k2,v2) = elements.[2]
            let struct(k3,v3) = elements.[3]
            MapLeaf(k0, v0).AddInPlace(cmp, k1, v1).AddInPlace(cmp, k2, v2).AddInPlace(cmp, k3, v3)
        | 5 ->
            let struct(k0,v0) = elements.[0]
            let struct(k1,v1) = elements.[1]
            let struct(k2,v2) = elements.[2]
            let struct(k3,v3) = elements.[3]
            let struct(k4,v4) = elements.[4]
            MapLeaf(k0, v0).AddInPlace(cmp, k1, v1).AddInPlace(cmp, k2, v2).AddInPlace(cmp, k3, v3).AddInPlace(cmp, k4, v4)
        | _ ->
            let struct(arr, cnt) = Sorting.mergeSortHandleDuplicatesV false cmp elements elements.Length
            Map.CreateRoot(arr, cnt)

    [<System.Runtime.Serialization.OnSerializingAttribute>]
    member __.OnSerializing(context: System.Runtime.Serialization.StreamingContext) =
        ignore context
        serializedData <- toKeyValueArray root

    [<System.Runtime.Serialization.OnDeserializedAttribute>]
    member __.OnDeserialized(context: System.Runtime.Serialization.StreamingContext) =
        ignore context
        comparer <- defaultComparer
        serializedData <- null
        root <- serializedData |> Array.map (fun kvp -> struct(kvp.Key, kvp.Value)) |> fromArray 

    static member Empty = empty

    static member private CreateTree(cmp : IComparer<'Key>, arr : ('Key * 'Value)[], cnt : int)=
        let rec create (arr : ('Key * 'Value)[]) (l : int) (r : int) =
            if l > r then
                MapEmpty.Instance
            elif l = r then
                let (k,v) = arr.[l]
                MapLeaf(k, v) :> MapNode<_,_>
            else
                let m = (l+r)/2
                let (k,v) = arr.[m]
                MapInner(
                    create arr l (m-1),
                    k, v,
                    create arr (m+1) r
                ) :> MapNode<_,_>

        Map(cmp, create arr 0 (cnt-1))
        
    static member private CreateTree(cmp : IComparer<'Key>, arr : struct('Key * 'Value)[], cnt : int)=
        let rec create (arr : struct('Key * 'Value)[]) (l : int) (r : int) =
            if l = r then
                let struct(k,v) = arr.[l]
                MapLeaf(k, v) :> MapNode<_,_>
            elif l > r then
                MapEmpty.Instance
            else
                let m = (l+r)/2
                let struct(k,v) = arr.[m]
                MapInner(
                    create arr l (m-1),
                    k, v,
                    create arr (m+1) r
                ) :> MapNode<_,_>

        Map(cmp, create arr 0 (cnt-1))
  
    static member private CreateRoot(arr : struct('Key * 'Value)[], cnt : int)=
        let rec create (arr : struct('Key * 'Value)[]) (l : int) (r : int) =
            if l = r then
                let struct(k,v) = arr.[l]
                MapLeaf(k, v) :> MapNode<_,_>
            elif l > r then
                MapEmpty.Instance
            else
                let m = (l+r)/2
                let struct(k,v) = arr.[m]
                MapInner(
                    create arr l (m-1),
                    k, v,
                    create arr (m+1) r
                ) :> MapNode<_,_>

        create arr 0 (cnt-1)

    static member FromArray (elements : array<'Key * 'Value>) =
        let cmp = defaultComparer
        match elements.Length with
        | 0 -> 
            Map(cmp, MapEmpty.Instance)
        | 1 ->
            let (k,v) = elements.[0]
            Map(cmp, MapLeaf(k, v))
        | 2 -> 
            let (k0,v0) = elements.[0]
            let (k1,v1) = elements.[1]
            let c = cmp.Compare(k0, k1)
            if c > 0 then Map(cmp, MapInner(MapEmpty.Instance, k1, v1, MapLeaf(k0, v0)))
            elif c < 0 then Map(cmp, MapInner(MapLeaf(k0, v0), k1, v1, MapEmpty.Instance))
            else Map(cmp, MapLeaf(k1, v1))
        | 3 ->
            let (k0,v0) = elements.[0]
            let (k1,v1) = elements.[1]
            let (k2,v2) = elements.[2]
            Map(cmp, MapLeaf(k0, v0).AddInPlace(cmp, k1, v1).AddInPlace(cmp, k2, v2))
        | 4 ->
            let (k0,v0) = elements.[0]
            let (k1,v1) = elements.[1]
            let (k2,v2) = elements.[2]
            let (k3,v3) = elements.[3]
            Map(cmp, MapLeaf(k0, v0).AddInPlace(cmp, k1, v1).AddInPlace(cmp, k2, v2).AddInPlace(cmp, k3, v3))
        | 5 ->
            let (k0,v0) = elements.[0]
            let (k1,v1) = elements.[1]
            let (k2,v2) = elements.[2]
            let (k3,v3) = elements.[3]
            let (k4,v4) = elements.[4]
            Map(cmp, MapLeaf(k0, v0).AddInPlace(cmp, k1, v1).AddInPlace(cmp, k2, v2).AddInPlace(cmp, k3, v3).AddInPlace(cmp, k4, v4))
        | _ ->
            let struct(arr, cnt) = Sorting.mergeSortHandleDuplicates false cmp elements elements.Length
            Map.CreateTree(cmp, arr, cnt)
        
    static member FromArrayV (elements : array<struct('Key * 'Value)>) =
        let cmp = defaultComparer
        match elements.Length with
        | 0 -> 
            Map(cmp, MapEmpty.Instance)
        | 1 ->
            let struct(k,v) = elements.[0]
            Map(cmp, MapLeaf(k, v))
        | 2 -> 
            let struct(k0,v0) = elements.[0]
            let struct(k1,v1) = elements.[1]
            let c = cmp.Compare(k0, k1)
            if c > 0 then Map(cmp, MapInner(MapEmpty.Instance, k1, v1, MapLeaf(k0, v0)))
            elif c < 0 then Map(cmp, MapInner(MapLeaf(k0, v0), k1, v1, MapEmpty.Instance))
            else Map(cmp, MapLeaf(k1, v1))
        | 3 ->
            let struct(k0,v0) = elements.[0]
            let struct(k1,v1) = elements.[1]
            let struct(k2,v2) = elements.[2]
            Map(cmp, MapLeaf(k0, v0).AddInPlace(cmp, k1, v1).AddInPlace(cmp, k2, v2))
        | 4 ->
            let struct(k0,v0) = elements.[0]
            let struct(k1,v1) = elements.[1]
            let struct(k2,v2) = elements.[2]
            let struct(k3,v3) = elements.[3]
            Map(cmp, MapLeaf(k0, v0).AddInPlace(cmp, k1, v1).AddInPlace(cmp, k2, v2).AddInPlace(cmp, k3, v3))
        | 5 ->
            let struct(k0,v0) = elements.[0]
            let struct(k1,v1) = elements.[1]
            let struct(k2,v2) = elements.[2]
            let struct(k3,v3) = elements.[3]
            let struct(k4,v4) = elements.[4]
            Map(cmp, MapLeaf(k0, v0).AddInPlace(cmp, k1, v1).AddInPlace(cmp, k2, v2).AddInPlace(cmp, k3, v3).AddInPlace(cmp, k4, v4))
        | _ ->
            let struct(arr, cnt) = Sorting.mergeSortHandleDuplicatesV false cmp elements elements.Length
            Map.CreateTree(cmp, arr, cnt)
        
    static member FromList (elements : list<'Key * 'Value>) =
        let rec atMost (cnt : int) (l : list<_>) =
            match l with
            | [] -> true
            | _ :: t ->
                if cnt > 0 then atMost (cnt - 1) t
                else false

        let cmp = defaultComparer
        match elements with
        | [] -> 
            // cnt = 0
            Map(cmp, MapEmpty.Instance)

        | ((k0, v0) as t0) :: rest ->
            // cnt >= 1
            match rest with
            | [] -> 
                // cnt = 1
                Map(cmp, MapLeaf(k0, v0))
            | ((k1, v1) as t1) :: rest ->
                // cnt >= 2
                match rest with
                | [] ->
                    // cnt = 2
                    let c = cmp.Compare(k0, k1)
                    if c < 0 then Map(cmp, MapInner(MapLeaf(k0, v0), k1, v1, MapEmpty.Instance))
                    elif c > 0 then Map(cmp, MapInner(MapEmpty.Instance, k1, v1, MapLeaf(k0, v0)))
                    else Map(cmp, MapLeaf(k1, v1))
                | ((k2, v2) as t2) :: rest ->
                    // cnt >= 3
                    match rest with
                    | [] ->
                        // cnt = 3
                        Map(cmp, MapLeaf(k0,v0).AddInPlace(cmp, k1, v1).AddInPlace(cmp, k2, v2))
                    | ((k3, v3) as t3) :: rest ->
                        // cnt >= 4
                        match rest with
                        | [] ->
                            // cnt = 4
                            Map(cmp, MapLeaf(k0,v0).AddInPlace(cmp, k1, v1).AddInPlace(cmp, k2, v2).AddInPlace(cmp, k3, v3))
                        | ((k4, v4) as t4) :: rest ->
                            // cnt >= 5
                            match rest with
                            | [] ->
                                // cnt = 5
                                Map(cmp, MapLeaf(k0,v0).AddInPlace(cmp, k1, v1).AddInPlace(cmp, k2, v2).AddInPlace(cmp, k3, v3).AddInPlace(cmp, k4, v4))
                            | t5 :: rest ->
                                // cnt >= 6
                                let mutable arr = Array.zeroCreate 16
                                let mutable cnt = 6
                                arr.[0] <- t0
                                arr.[1] <- t1
                                arr.[2] <- t2
                                arr.[3] <- t3
                                arr.[4] <- t4
                                arr.[5] <- t5
                                for t in rest do
                                    if cnt >= arr.Length then System.Array.Resize(&arr, arr.Length <<< 1)
                                    arr.[cnt] <- t
                                    cnt <- cnt + 1
                                    
                                let struct(arr1, cnt1) = Sorting.mergeSortHandleDuplicates true cmp arr cnt
                                Map.CreateTree(cmp, arr1, cnt1)
                                
                                
                    
                

    static member FromListV (elements : list<struct('Key * 'Value)>) =
        let rec atMost (cnt : int) (l : list<_>) =
            match l with
            | [] -> true
            | _ :: t ->
                if cnt > 0 then atMost (cnt - 1) t
                else false

        let cmp = defaultComparer
        match elements with
        | [] -> 
            // cnt = 0
            Map(cmp, MapEmpty.Instance)

        | struct(k0, v0) :: rest ->
            // cnt >= 1
            match rest with
            | [] -> 
                // cnt = 1
                Map(cmp, MapLeaf(k0, v0))
            | struct(k1, v1) :: rest ->
                // cnt >= 2
                match rest with
                | [] ->
                    // cnt = 2
                    let c = cmp.Compare(k0, k1)
                    if c < 0 then Map(cmp, MapInner(MapLeaf(k0, v0), k1, v1, MapEmpty.Instance))
                    elif c > 0 then Map(cmp, MapInner(MapEmpty.Instance, k1, v1, MapLeaf(k0, v0)))
                    else Map(cmp, MapLeaf(k1, v1))
                | struct(k2, v2) :: rest ->
                    // cnt >= 3
                    match rest with
                    | [] ->
                        // cnt = 3
                        Map(cmp, MapLeaf(k0,v0).AddInPlace(cmp, k1, v1).AddInPlace(cmp, k2, v2))
                    | struct(k3, v3) :: rest ->
                        // cnt >= 4
                        match rest with
                        | [] ->
                            // cnt = 4
                            Map(cmp, MapLeaf(k0,v0).AddInPlace(cmp, k1, v1).AddInPlace(cmp, k2, v2).AddInPlace(cmp, k3, v3))
                        | struct(k4, v4) :: rest ->
                            // cnt >= 5
                            match rest with
                            | [] ->
                                // cnt = 5
                                Map(cmp, MapLeaf(k0,v0).AddInPlace(cmp, k1, v1).AddInPlace(cmp, k2, v2).AddInPlace(cmp, k3, v3).AddInPlace(cmp, k4, v4))
                            | t5 :: rest ->
                                // cnt >= 6
                                let mutable arr = Array.zeroCreate 16
                                let mutable cnt = 6
                                arr.[0] <- struct(k0, v0)
                                arr.[1] <- struct(k1, v1)
                                arr.[2] <- struct(k2, v2)
                                arr.[3] <- struct(k3, v3)
                                arr.[4] <- struct(k4, v4)
                                arr.[5] <- t5
                                for t in rest do
                                    if cnt >= arr.Length then System.Array.Resize(&arr, arr.Length <<< 1)
                                    arr.[cnt] <- t
                                    cnt <- cnt + 1
                                    
                                let struct(arr1, cnt1) = Sorting.mergeSortHandleDuplicatesV true cmp arr cnt
                                Map.CreateTree(cmp, arr1, cnt1)
 
    static member FromSeq (elements : seq<'Key * 'Value>) =
        match elements with
        | :? array<'Key * 'Value> as e -> Map.FromArray e
        | :? list<'Key * 'Value> as e -> Map.FromList e
        | _ ->
            let cmp = defaultComparer
            use e = elements.GetEnumerator()
            if e.MoveNext() then
                // cnt >= 1
                let t0 = e.Current
                let (k0,v0) = t0
                if e.MoveNext() then
                    // cnt >= 2
                    let t1 = e.Current
                    let (k1,v1) = t1
                    if e.MoveNext() then
                        // cnt >= 3 
                        let t2 = e.Current
                        let (k2,v2) = t2
                        if e.MoveNext() then
                            // cnt >= 4
                            let t3 = e.Current
                            let (k3, v3) = t3
                            if e.MoveNext() then
                                // cnt >= 5
                                let t4 = e.Current
                                let (k4, v4) = t4
                                if e.MoveNext() then
                                    // cnt >= 6
                                    let mutable arr = Array.zeroCreate 16
                                    let mutable cnt = 6
                                    arr.[0] <- t0
                                    arr.[1] <- t1
                                    arr.[2] <- t2
                                    arr.[3] <- t3
                                    arr.[4] <- t4
                                    arr.[5] <- e.Current

                                    while e.MoveNext() do
                                        if cnt >= arr.Length then System.Array.Resize(&arr, arr.Length <<< 1)
                                        arr.[cnt] <- e.Current
                                        cnt <- cnt + 1

                                    let struct(arr1, cnt1) = Sorting.mergeSortHandleDuplicates true cmp arr cnt
                                    Map.CreateTree(cmp, arr1, cnt1)

                                else
                                    // cnt = 5
                                    Map(cmp, MapLeaf(k0, v0).AddInPlace(cmp, k1, v1).AddInPlace(cmp, k2, v2).AddInPlace(cmp, k3, v3).AddInPlace(cmp, k4, v4))

                            else
                                // cnt = 4
                                Map(cmp, MapLeaf(k0, v0).AddInPlace(cmp, k1, v1).AddInPlace(cmp, k2, v2).AddInPlace(cmp, k3, v3))
                        else
                            Map(cmp, MapLeaf(k0, v0).AddInPlace(cmp, k1, v1).AddInPlace(cmp, k2, v2))
                    else
                        // cnt = 2
                        let c = cmp.Compare(k0, k1)
                        if c < 0 then Map(cmp, MapInner(MapLeaf(k0, v0), k1, v1, MapEmpty.Instance))
                        elif c > 0 then Map(cmp, MapInner(MapEmpty.Instance, k1, v1, MapLeaf(k0, v0)))
                        else Map(cmp, MapLeaf(k1, v1))
                else
                    // cnt = 1
                    Map(cmp, MapLeaf(k0, v0))

            else
                Map(cmp, MapEmpty.Instance)

    static member FromSeqV (elements : seq<struct('Key * 'Value)>) =
        match elements with
        | :? array<struct('Key * 'Value)> as e -> Map.FromArrayV e
        | :? list<struct('Key * 'Value)> as e -> Map.FromListV e
        | _ ->
            let cmp = defaultComparer
            use e = elements.GetEnumerator()
            if e.MoveNext() then
                // cnt >= 1
                let struct(k0,v0) = e.Current
                if e.MoveNext() then
                    // cnt >= 2
                    let struct(k1,v1) = e.Current
                    if e.MoveNext() then
                        // cnt >= 3 
                        let struct(k2,v2) = e.Current
                        if e.MoveNext() then
                            // cnt >= 4
                            let struct(k3, v3) = e.Current
                            if e.MoveNext() then
                                // cnt >= 5
                                let struct(k4, v4) = e.Current
                                if e.MoveNext() then
                                    // cnt >= 6
                                    let mutable arr = Array.zeroCreate 16
                                    let mutable cnt = 6
                                    arr.[0] <- struct(k0, v0)
                                    arr.[1] <- struct(k1, v1)
                                    arr.[2] <- struct(k2, v2)
                                    arr.[3] <- struct(k3, v3)
                                    arr.[4] <- struct(k4, v4)
                                    arr.[5] <- e.Current

                                    while e.MoveNext() do
                                        if cnt >= arr.Length then System.Array.Resize(&arr, arr.Length <<< 1)
                                        arr.[cnt] <- e.Current
                                        cnt <- cnt + 1

                                    let struct(arr1, cnt1) = Sorting.mergeSortHandleDuplicatesV true cmp arr cnt
                                    Map.CreateTree(cmp, arr1, cnt1)

                                else
                                    // cnt = 5
                                    Map(cmp, MapLeaf(k0, v0).AddInPlace(cmp, k1, v1).AddInPlace(cmp, k2, v2).AddInPlace(cmp, k3, v3).AddInPlace(cmp, k4, v4))

                            else
                                // cnt = 4
                                Map(cmp, MapLeaf(k0, v0).AddInPlace(cmp, k1, v1).AddInPlace(cmp, k2, v2).AddInPlace(cmp, k3, v3))
                        else
                            Map(cmp, MapLeaf(k0, v0).AddInPlace(cmp, k1, v1).AddInPlace(cmp, k2, v2))
                    else
                        // cnt = 2
                        let c = cmp.Compare(k0, k1)
                        if c < 0 then Map(cmp, MapInner(MapLeaf(k0, v0), k1, v1, MapEmpty.Instance))
                        elif c > 0 then Map(cmp, MapInner(MapEmpty.Instance, k1, v1, MapLeaf(k0, v0)))
                        else Map(cmp, MapLeaf(k1, v1))
                else
                    // cnt = 1
                    Map(cmp, MapLeaf(k0, v0))

            else
                Map(cmp, MapEmpty.Instance)

    static member Union(l : Map<'Key, 'Value>, r : Map<'Key, 'Value>) =
        let rec union (cmp : IComparer<'Key>) (l : MapNode<'Key, 'Value>) (r : MapNode<'Key, 'Value>) =
            match l with
            | :? MapEmpty<'Key, 'Value> ->  
                r
            | :? MapLeaf<'Key, 'Value> as l ->
                r.AddIfNotPresent(cmp, l.Key, l.Value)
            | :? MapInner<'Key, 'Value> as l ->
                match r with
                | :? MapEmpty<'Key, 'Value> ->
                    l :> MapNode<_,_>
                | :? MapLeaf<'Key, 'Value> as r ->
                    l.Add(cmp, r.Key, r.Value)
                | :? MapInner<'Key, 'Value> as r ->
                    if l.Count > r.Count then
                        let struct(rl, rr, rv) = r.SplitV(cmp, l.Key)
                        match rv with
                        | ValueSome rv ->
                            MapInner.Create(
                                union cmp l.Left rl, 
                                l.Key, rv, 
                                union cmp l.Right rr
                            )
                        | ValueNone ->
                            MapInner.Create(
                                union cmp l.Left rl, 
                                l.Key, l.Value, 
                                union cmp l.Right rr
                            )
                    else
                        let struct(ll, lr, _lv) = l.SplitV(cmp, r.Key)
                        MapInner.Create(
                            union cmp ll r.Left, 
                            r.Key, r.Value, 
                            union cmp lr r.Right
                        ) 
                | _ ->
                    failwith "unexpected node"
            | _ ->
                failwith "unexpected node"

        let cmp = defaultComparer
        Map(cmp, union cmp l.Root r.Root)

    static member UnionWith(l : Map<'Key, 'Value>, r : Map<'Key, 'Value>, resolve : 'Key -> 'Value -> 'Value -> 'Value) =
        let resolve = OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt resolve
        
        let rec union (cmp : IComparer<'Key>) (resolve : OptimizedClosures.FSharpFunc<_,_,_,_>) (l : MapNode<'Key, 'Value>) (r : MapNode<'Key, 'Value>) =
            match l with
            | :? MapEmpty<'Key, 'Value> ->  
                r
            | :? MapLeaf<'Key, 'Value> as l ->
                r.ChangeV(cmp, l.Key, function
                    | ValueSome rv -> resolve.Invoke(l.Key, l.Value, rv) |> ValueSome
                    | ValueNone -> l.Value |> ValueSome
                )
            | :? MapInner<'Key, 'Value> as l ->
                match r with
                | :? MapEmpty<'Key, 'Value> ->
                    l :> MapNode<_,_>
                | :? MapLeaf<'Key, 'Value> as r ->
                    l.ChangeV(cmp, r.Key, function
                        | ValueSome lv -> resolve.Invoke(r.Key, lv, r.Value) |> ValueSome
                        | ValueNone -> r.Value |> ValueSome
                    )
                | :? MapInner<'Key, 'Value> as r ->
                    if l.Count > r.Count then
                        let struct(rl, rr, rv) = r.SplitV(cmp, l.Key)
                        match rv with
                        | ValueSome rv ->
                            MapInner.Create(
                                union cmp resolve l.Left rl, 
                                l.Key, resolve.Invoke(l.Key, l.Value, rv), 
                                union cmp resolve l.Right rr
                            )
                        | ValueNone ->
                            MapInner.Create(
                                union cmp resolve l.Left rl, 
                                l.Key, l.Value, 
                                union cmp resolve l.Right rr
                            )
                    else
                        let struct(ll, lr, lv) = l.SplitV(cmp, r.Key)
                        match lv with
                        | ValueSome lv ->
                            MapInner.Create(
                                union cmp resolve ll r.Left, 
                                r.Key, resolve.Invoke(r.Key, lv, r.Value), 
                                union cmp resolve lr r.Right
                            )
                        | ValueNone ->
                            MapInner.Create(
                                union cmp resolve ll r.Left, 
                                r.Key, r.Value, 
                                union cmp resolve lr r.Right
                            )
                            
                | _ ->
                    failwith "unexpected node"
            | _ ->
                failwith "unexpected node"

        let cmp = defaultComparer
        Map(cmp, union cmp resolve l.Root r.Root)
        
    member x.Count = root.Count
    member x.IsEmpty = root.Count = 0
    member x.Root = root
    member x.Comparer = comparer

    static member ComputeDelta<'T>(l : Map<'Key, 'Value>, r : Map<'Key, 'Value>, add : Map<'Key, 'Value> -> Map<'Key, 'T>, remove : Map<'Key, 'Value> -> Map<'Key, 'T>, update : 'Key -> 'Value -> 'Value -> voption<'T>) : Map<'Key, 'T> =
        
        let inline add (cmp : IComparer<_>) (a : MapNode<'Key, 'Value>) = 
            if a.Count > 0 then add(Map<'Key, 'Value>(cmp, a)).Root
            else MapEmpty.Instance

        let inline remove (cmp : IComparer<_>) (a : MapNode<'Key, 'Value>) = 
            if a.Count > 0 then remove(Map<'Key, 'Value>(cmp, a)).Root
            else MapEmpty.Instance

        let rec computeDelta (cmp : IComparer<_>) (update : OptimizedClosures.FSharpFunc<_,_,_,_>) (l : MapNode<'Key, 'Value>) (r : MapNode<'Key, 'Value>) =
            match l with
            | :? MapLeaf<'Key, 'Value> as l ->
                match r with
                | :? MapLeaf<'Key, 'Value> as r ->
                    let c = cmp.Compare(l.Key, r.Key)
                    if c < 0 then
                        MapInner<'Key, 'T>.Join(remove cmp l, add cmp r)
                    elif c > 0 then
                        MapInner<'Key, 'T>.Join(add cmp r, remove cmp l)
                    else
                        match update.Invoke(l.Key, l.Value, r.Value) with
                        | ValueSome o -> MapLeaf(l.Key, o) :> MapNode<_,_>
                        | ValueNone -> MapEmpty.Instance
                | :? MapInner<'Key, 'Value> as r ->
                    let struct(rl, rr, rv) = r.SplitV(cmp, l.Key)

                    let a = computeDelta cmp update MapEmpty.Instance rl
                    let splitter = 
                        match rv with
                        | ValueSome rv -> update.Invoke(l.Key, l.Value, rv)
                        | ValueNone -> ValueNone
                    let b = computeDelta cmp update MapEmpty.Instance rr

                    match splitter with
                    | ValueSome v -> MapInner.Create(a, l.Key, v, b)
                    | ValueNone -> MapInner.Join(a, b)
                | _ ->
                    remove cmp l

            | :? MapInner<'Key, 'Value> as l ->
                match r with
                | :? MapLeaf<'Key, 'Value> as r ->
                    let struct(ll, lr, lv) = l.SplitV(cmp, r.Key)
                    let a = computeDelta cmp update ll MapEmpty.Instance
                    let splitter = 
                        match lv with
                        | ValueSome lv -> update.Invoke(l.Key, lv, r.Value)
                        | ValueNone -> ValueNone
                    let b = computeDelta cmp update lr MapEmpty.Instance

                    match splitter with
                    | ValueSome v -> MapInner.Create(a, l.Key, v, b)
                    | ValueNone -> MapInner.Join(a, b)
                | :? MapInner<'Key, 'Value> as r ->
                    if l.Count > r.Count then
                        let struct(rl, rr, rv) = r.SplitV(cmp, l.Key)
                        let a = computeDelta cmp update l.Left rl
                        let splitter = 
                            match rv with
                            | ValueSome rv -> update.Invoke(l.Key, l.Value, rv)
                            | ValueNone -> ValueNone
                        let b = computeDelta cmp update l.Right rr
                        match splitter with
                        | ValueSome v -> MapInner.Create(a, l.Key, v, b)
                        | ValueNone -> MapInner.Join(a, b)
                    else
                        let struct(ll, lr, lv) = l.SplitV(cmp, r.Key)
                        let a = computeDelta cmp update ll r.Left
                        let splitter = 
                            match lv with
                            | ValueSome lv -> update.Invoke(r.Key, lv, r.Value)
                            | ValueNone -> ValueNone
                        let b = computeDelta cmp update lr r.Right
                        match splitter with
                        | ValueSome v -> MapInner.Create(a, r.Key, v, b)
                        | ValueNone -> MapInner.Join(a, b)
                | _ ->
                    remove cmp l
            | _ ->
                add cmp r

        let cmp = defaultComparer
        let update = OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt update

        Map(cmp, computeDelta cmp update l.Root r.Root)


    member x.Add(key : 'Key, value : 'Value) =
        Map(comparer, root.Add(comparer, key, value))
       
    member x.AddMatch(key : 'Key, value : 'Value) =
        let rec add (cmp : IComparer<'Key>) (key : 'Key) (value : 'Value) (n : MapNode<'Key, 'Value>) =
            match n with
            | :? MapLeaf<'Key, 'Value> as n ->
                let c = cmp.Compare(key, n.Key)
                if c > 0 then
                    MapInner(n, key, value, MapEmpty.Instance) :> MapNode<_,_>
                elif c < 0 then
                    MapInner(MapEmpty.Instance, key, value, n) :> MapNode<_,_>
                else
                    MapLeaf(key, value) :> MapNode<_,_>
            | :? MapInner<'Key, 'Value> as n ->
                let c = cmp.Compare(key, n.Key)
                if c > 0 then
                    MapInner.Create(
                        n.Left,
                        n.Key, n.Value,
                        add cmp key value n.Right
                    )
                elif c < 0 then
                    MapInner.Create(
                        add cmp key value n.Left,
                        n.Key, n.Value,
                        n.Right
                    )
                else
                    MapInner(
                        n.Left,
                        key, value,
                        n.Right
                    ) :> MapNode<_,_>
            | _ ->
                MapLeaf(key, value) :> MapNode<_,_>
                
        Map(comparer, add comparer key value root)
            
    member x.Remove(key : 'Key) =
        Map(comparer, root.Remove(comparer, key))
        
    member x.RemoveMatch(key : 'Key) =
    
        let rec remove (cmp : IComparer<'Key>) (key : 'Key) (n : MapNode<'Key, 'Value>) =
            match n with
            | :? MapLeaf<'Key, 'Value> as n ->
                let c = cmp.Compare(key, n.Key)
                if c = 0 then MapEmpty.Instance
                else n :> MapNode<_,_>
            | :? MapInner<'Key, 'Value> as n ->
                let c = cmp.Compare(key, n.Key)
                if c > 0 then
                    MapInner.Create(
                        n.Left,
                        n.Key, n.Value,
                        remove cmp key n.Right
                    )
                elif c < 0 then
                    MapInner.Create(
                        remove cmp key n.Left,
                        n.Key, n.Value,
                        n.Right
                    )
                else
                    MapInner.Join(n.Left, n.Right)
            | _ ->
                MapEmpty.Instance
                
        Map(comparer, remove comparer key root)

    member x.Iter(action : 'Key -> 'Value -> unit) =
        let action = OptimizedClosures.FSharpFunc<_,_,_>.Adapt action
        let rec iter (action : OptimizedClosures.FSharpFunc<_,_,_>) (n : MapNode<_,_>) =
            match n with
            | :? MapInner<'Key, 'Value> as n ->
                iter action n.Left
                action.Invoke(n.Key, n.Value)
                iter action n.Right
            | :? MapLeaf<'Key, 'Value> as n ->
                action.Invoke(n.Key, n.Value)
            | _ ->
                ()
        iter action root

    member x.Map(mapping : 'Key -> 'Value -> 'T) =
        let mapping = OptimizedClosures.FSharpFunc<_,_,_>.Adapt mapping
        Map(comparer, root.Map(mapping))
        
    member x.Filter(predicate : 'Key -> 'Value -> bool) =
        let predicate = OptimizedClosures.FSharpFunc<_,_,_>.Adapt predicate
        Map(comparer, root.Filter(predicate))

    member x.Choose(mapping : 'Key -> 'Value -> option<'T>) =
        let mapping = OptimizedClosures.FSharpFunc<_,_,_>.Adapt mapping
        Map(comparer, root.Choose(mapping))

    member x.ChooseV(mapping : 'Key -> 'Value -> voption<'T>) =
        let mapping = OptimizedClosures.FSharpFunc<_,_,_>.Adapt mapping
        Map(comparer, root.ChooseV(mapping))

    member x.Exists(predicate : 'Key -> 'Value -> bool) =
        let predicate = OptimizedClosures.FSharpFunc<_,_,_>.Adapt predicate
        let rec exists (predicate : OptimizedClosures.FSharpFunc<_,_,_>) (n : MapNode<_,_>) =
            match n with
            | :? MapInner<'Key, 'Value> as n ->
                exists predicate n.Left ||
                predicate.Invoke(n.Key, n.Value) ||
                exists predicate n.Right
            | :? MapLeaf<'Key, 'Value> as n ->
                predicate.Invoke(n.Key, n.Value)
            | _ ->
                false
        exists predicate root
        
    member x.Forall(predicate : 'Key -> 'Value -> bool) =
        let rec forall (predicate : OptimizedClosures.FSharpFunc<_,_,_>) (n : MapNode<_,_>) =
            match n with
            | :? MapInner<'Key, 'Value> as n ->
                forall predicate n.Left &&
                predicate.Invoke(n.Key, n.Value) &&
                forall predicate n.Right
            | :? MapLeaf<'Key, 'Value> as n ->
                predicate.Invoke(n.Key, n.Value)
            | _ ->
                true
        let predicate = OptimizedClosures.FSharpFunc<_,_,_>.Adapt predicate
        forall predicate root

    member x.Fold(folder : 'State -> 'Key -> 'Value -> 'State, seed : 'State) =
        let folder = OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt folder

        let rec fold (folder : OptimizedClosures.FSharpFunc<_,_,_,_>) seed (n : MapNode<_,_>) =
            match n with
            | :? MapInner<'Key, 'Value> as n ->
                let s1 = fold folder seed n.Left
                let s2 = folder.Invoke(s1, n.Key, n.Value)
                fold folder s2 n.Right
            | :? MapLeaf<'Key, 'Value> as n ->
                folder.Invoke(seed, n.Key, n.Value)
            | _ ->
                seed

        fold folder seed root
        
    member x.FoldBack(folder : 'Key -> 'Value -> 'State -> 'State, seed : 'State) =
        let folder = OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt folder

        let rec foldBack (folder : OptimizedClosures.FSharpFunc<_,_,_,_>) seed (n : MapNode<_,_>) =
            match n with
            | :? MapInner<'Key, 'Value> as n ->
                let s1 = foldBack folder seed n.Right
                let s2 = folder.Invoke(n.Key, n.Value, s1)
                foldBack folder s2 n.Left
            | :? MapLeaf<'Key, 'Value> as n ->
                folder.Invoke(n.Key, n.Value, seed)
            | _ ->
                seed

        foldBack folder seed root
        
    member x.TryFind(key : 'Key) =
        let rec tryFind (cmp : IComparer<_>) key (n : MapNode<_,_>) =
            match n with
            | :? MapInner<'Key, 'Value> as n ->
                let c = cmp.Compare(key, n.Key)
                if c > 0 then tryFind cmp key n.Right
                elif c < 0 then tryFind cmp key n.Left
                else Some n.Value
            | :? MapLeaf<'Key, 'Value> as n ->
                let c = cmp.Compare(key, n.Key)
                if c = 0 then Some n.Value
                else None
            | _ ->
                None
        tryFind comparer key root
        
    member x.Find(key : 'Key) : 'Value =
        let rec run (cmp : IComparer<_>) key (n : MapNode<_,_>) =
            match n with
            | :? MapInner<'Key, 'Value> as n ->
                let c = cmp.Compare(key, n.Key)
                if c > 0 then run cmp key n.Right
                elif c < 0 then run cmp key n.Left
                else n.Value
            | :? MapLeaf<'Key, 'Value> as n ->
                let c = cmp.Compare(key, n.Key)
                if c = 0 then n.Value
                else raise <| KeyNotFoundException()
            | _ ->
                raise <| KeyNotFoundException()
        run comparer key root 
        

    member x.Item
        with get(key : 'Key) : 'Value = x.Find key

    member x.TryFindV(key : 'Key) =
        let rec tryFind (cmp : IComparer<_>) key (n : MapNode<_,_>) =
            match n with
            | :? MapInner<'Key, 'Value> as n ->
                let c = cmp.Compare(key, n.Key)
                if c > 0 then tryFind cmp key n.Right
                elif c < 0 then tryFind cmp key n.Left
                else ValueSome n.Value
            | :? MapLeaf<'Key, 'Value> as n ->
                let c = cmp.Compare(key, n.Key)
                if c = 0 then ValueSome n.Value
                else ValueNone
            | _ ->
                ValueNone
        tryFind comparer key root
     
    member x.TryFindKey(predicate : 'Key -> 'Value -> bool) =
        let rec run (predicate : OptimizedClosures.FSharpFunc<'Key, 'Value, bool>) (node : MapNode<'Key, 'Value>) =
            match node with
            | :? MapLeaf<'Key, 'Value> as l ->
                if predicate.Invoke(l.Key, l.Value) then Some l.Key
                else None
            | :? MapInner<'Key, 'Value> as n ->
                match run predicate n.Left with
                | None ->
                    if predicate.Invoke(n.Key, n.Value) then Some n.Key
                    else run predicate n.Right
                | res -> 
                    res
            | _ ->
                None
        run (OptimizedClosures.FSharpFunc<_,_,_>.Adapt predicate) root
        
    member x.TryFindKeyV(predicate : 'Key -> 'Value -> bool) =
        let rec run (predicate : OptimizedClosures.FSharpFunc<'Key, 'Value, bool>) (node : MapNode<'Key, 'Value>) =
            match node with
            | :? MapLeaf<'Key, 'Value> as l ->
                if predicate.Invoke(l.Key, l.Value) then ValueSome l.Key
                else ValueNone
            | :? MapInner<'Key, 'Value> as n ->
                match run predicate n.Left with
                | ValueNone ->
                    if predicate.Invoke(n.Key, n.Value) then ValueSome n.Key
                    else run predicate n.Right
                | res -> 
                    res
            | _ ->
                ValueNone
        run (OptimizedClosures.FSharpFunc<_,_,_>.Adapt predicate) root
        
    member x.FindKey(predicate : 'Key -> 'Value -> bool) =
        match x.TryFindKeyV predicate with
        | ValueSome k -> k
        | ValueNone -> raise <| KeyNotFoundException()
        
    member x.TryPick(mapping : 'Key -> 'Value -> option<'T>) =
        let rec run (mapping : OptimizedClosures.FSharpFunc<'Key, 'Value, option<'T>>) (node : MapNode<'Key, 'Value>) =
            match node with
            | :? MapLeaf<'Key, 'Value> as l ->
                mapping.Invoke(l.Key, l.Value)
                
            | :? MapInner<'Key, 'Value> as n ->
                match run mapping n.Left with
                | None ->
                    match mapping.Invoke(n.Key, n.Value) with
                    | Some _ as res -> res
                    | None -> run mapping n.Right
                | res -> 
                    res
            | _ ->
                None
        run (OptimizedClosures.FSharpFunc<_,_,_>.Adapt mapping) root
        
    //member x.Keys() =
    //    let rec run (n : MapNode<'Key, 'Value>) =
    //        match n with
    //        | :? MapInner<'Key, 'Value> as n ->
    //            SetNewImplementation.SetInner(
    //                run n.Left,
    //                n.Key,
    //                run n.Right
    //            ) :> SetNewImplementation.SetNode<_>
    //        | :? MapLeaf<'Key, 'Value> as n ->
    //            SetNewImplementation.SetLeaf(n.Key) :> SetNewImplementation.SetNode<_>
    //        | _ ->
    //            SetNewImplementation.SetEmpty.Instance
    //    SetNew(comparer, run root)

    member x.TryPickV(mapping : 'Key -> 'Value -> voption<'T>) =
        let rec run (mapping : OptimizedClosures.FSharpFunc<'Key, 'Value, voption<'T>>) (node : MapNode<'Key, 'Value>) =
            match node with
            | :? MapLeaf<'Key, 'Value> as l ->
                mapping.Invoke(l.Key, l.Value)
                
            | :? MapInner<'Key, 'Value> as n ->
                match run mapping n.Left with
                | ValueNone ->
                    match mapping.Invoke(n.Key, n.Value) with
                    | ValueSome _ as res -> res
                    | ValueNone -> run mapping n.Right
                | res -> 
                    res
            | _ ->
                ValueNone
        run (OptimizedClosures.FSharpFunc<_,_,_>.Adapt mapping) root
        
    member x.Pick(mapping : 'Key -> 'Value -> option<'T>) =
        match x.TryPick mapping with
        | Some k -> k
        | None -> raise <| KeyNotFoundException()
        
    member x.PickV(mapping : 'Key -> 'Value -> voption<'T>) =
        match x.TryPickV mapping with
        | ValueSome k -> k
        | ValueNone -> raise <| KeyNotFoundException()
        
    member x.Partition(predicate : 'Key -> 'Value -> bool) =
        let predicate = OptimizedClosures.FSharpFunc<_,_,_>.Adapt predicate

        let cnt = x.Count 
        let a0 = Array.zeroCreate cnt
        let a1 = Array.zeroCreate cnt
        x.CopyToV(a0, 0)

        let mutable i1 = 0
        let mutable i0 = 0
        for i in 0 .. cnt - 1 do
            let struct(k,v) = a0.[i]
            if predicate.Invoke(k, v) then 
                a0.[i0] <- struct(k,v)
                i0 <- i0 + 1
            else
                a1.[i1] <- struct(k,v)
                i1 <- i1 + 1

        Map.CreateTree(comparer, a0, i0), Map.CreateTree(comparer, a1, i1)

    member x.ContainsKey(key : 'Key) =
        let rec contains (cmp : IComparer<_>) key (n : MapNode<_,_>) =
            match n with
            | :? MapInner<'Key, 'Value> as n ->
                let c = cmp.Compare(key, n.Key)
                if c > 0 then contains cmp key n.Right
                elif c < 0 then contains cmp key n.Left
                else true
            | :? MapLeaf<'Key, 'Value> as n ->
                let c = cmp.Compare(key, n.Key)
                if c = 0 then true
                else false

            | _ ->
                false
        contains comparer key root

    member x.GetEnumerator() = new MapEnumerator<_,_>(root)

    member x.ToList() = 
        let rec toList acc (n : MapNode<_,_>) =
            match n with
            | :? MapInner<'Key, 'Value> as n ->
                toList ((n.Key, n.Value) :: toList acc n.Right) n.Left
            | :? MapLeaf<'Key, 'Value> as n ->
                (n.Key, n.Value) :: acc
            | _ ->
                acc
        toList [] root

    member x.ToListV() = 
        let rec toList acc (n : MapNode<_,_>) =
            match n with
            | :? MapInner<'Key, 'Value> as n ->
                toList (struct(n.Key, n.Value) :: toList acc n.Right) n.Left
            | :? MapLeaf<'Key, 'Value> as n ->
                struct(n.Key, n.Value) :: acc
            | _ ->
                acc
        toList [] root
       
    member x.ToArray() =
        let arr = Array.zeroCreate x.Count
        let rec copyTo (arr : array<_>) (index : int) (n : MapNode<_,_>) =
            match n with
            | :? MapInner<'Key, 'Value> as n ->
                let index = copyTo arr index n.Left
                arr.[index] <- (n.Key, n.Value)
                copyTo arr (index + 1) n.Right
            | :? MapLeaf<'Key, 'Value> as n ->
                arr.[index] <- (n.Key, n.Value)
                index + 1
            | _ ->
                index

        copyTo arr 0 root |> ignore<int>
        arr

    member x.ToArrayV() =
        let arr = Array.zeroCreate x.Count
        let rec copyTo (arr : array<_>) (index : int) (n : MapNode<_,_>) =
            match n with
            | :? MapInner<'Key, 'Value> as n ->
                let index = copyTo arr index n.Left
                arr.[index] <- struct(n.Key, n.Value)
                copyTo arr (index + 1) n.Right
            | :? MapLeaf<'Key, 'Value> as n ->
                arr.[index] <- struct(n.Key, n.Value)
                index + 1
            | _ ->
                index

        copyTo arr 0 root |> ignore<int>
        arr

    member x.CopyTo(array : ('Key * 'Value)[], startIndex : int) =
        if startIndex < 0 || startIndex + x.Count > array.Length then raise <| System.IndexOutOfRangeException("Map.CopyTo")
        let rec copyTo (arr : array<_>) (index : int) (n : MapNode<_,_>) =
            match n with
            | :? MapInner<'Key, 'Value> as n ->
                let index = copyTo arr index n.Left
                arr.[index] <- (n.Key, n.Value)
                copyTo arr (index + 1) n.Right
            | :? MapLeaf<'Key, 'Value> as n ->
                arr.[index] <- (n.Key, n.Value)
                index + 1
            | _ ->
                index
        copyTo array startIndex root |> ignore<int>

    member x.CopyToV(array : struct('Key * 'Value)[], startIndex : int) =
        if startIndex < 0 || startIndex + x.Count > array.Length then raise <| System.IndexOutOfRangeException("Map.CopyTo")
        let rec copyTo (arr : array<_>) (index : int) (n : MapNode<_,_>) =
            match n with
            | :? MapInner<'Key, 'Value> as n ->
                let index = copyTo arr index n.Left
                arr.[index] <- struct(n.Key, n.Value)
                copyTo arr (index + 1) n.Right
            | :? MapLeaf<'Key, 'Value> as n ->
                arr.[index] <- struct(n.Key, n.Value)
                index + 1
            | _ ->
                index
        copyTo array startIndex root |> ignore<int>

    member x.GetViewBetween(minInclusive : 'Key, maxInclusive : 'Key) = 
        Map(comparer, root.GetViewBetween(comparer, minInclusive, true, maxInclusive, true))
       
    member x.GetSlice(min : option<'Key>, max : option<'Key>) =
        match min with
        | Some min ->
            match max with
            | Some max ->
                x.GetViewBetween(min, max)
            | None ->
                x.WithMin min
        | None ->
            match max with
            | Some max ->
                x.WithMax max
            | None ->
                x

    member x.WithMin(minInclusive : 'Key) = 
        Map(comparer, root.WithMin(comparer, minInclusive, true))
        
    member x.WithMax(maxInclusive : 'Key) = 
        Map(comparer, root.WithMax(comparer, maxInclusive, true))

    member x.TryMinKeyValue() = 
        let rec run (node : MapNode<'Key, 'Value>) =
            match node with
            | :? MapLeaf<'Key, 'Value> as l -> 
                Some (l.Key, l.Value)
            | :? MapInner<'Key, 'Value> as n ->
                if n.Left.Count = 0 then Some (n.Key, n.Value)
                else run n.Left
            | _ ->
                None
        
        run root

    member x.TryMinKeyValueV() =
        let rec run (node : MapNode<'Key, 'Value>) =
            match node with
            | :? MapLeaf<'Key, 'Value> as l -> 
                ValueSome struct(l.Key, l.Value)
            | :? MapInner<'Key, 'Value> as n ->
                if n.Left.Count = 0 then ValueSome struct(n.Key, n.Value)
                else run n.Left
            | _ ->
                ValueNone
        run root

    member x.TryMaxKeyValue() =
        let rec run (node : MapNode<'Key, 'Value>) =
            match node with
            | :? MapLeaf<'Key, 'Value> as l -> 
                Some (l.Key, l.Value)
            | :? MapInner<'Key, 'Value> as n ->
                if n.Right.Count = 0 then Some (n.Key, n.Value)
                else run n.Right
            | _ ->
                None
        
        run root

    member x.TryMaxKeyValueV() = 
        let rec run (node : MapNode<'Key, 'Value>) =
            match node with
            | :? MapLeaf<'Key, 'Value> as l -> 
                ValueSome struct(l.Key, l.Value)
            | :? MapInner<'Key, 'Value> as n ->
                if n.Right.Count = 0 then ValueSome struct(n.Key, n.Value)
                else run n.Right
            | _ ->
                ValueNone
        run root

    member x.Change(key : 'Key, f : option<'Value> -> option<'Value>) =
        Map(comparer, root.Change(comparer, key, f))
        
    member x.ChangeV(key : 'Key, update : voption<'Value> -> voption<'Value>) =
        Map(comparer, root.ChangeV(comparer, key, update))

    member x.TryAt(index : int) =
        if index < 0 || index >= root.Count then None
        else 
            let rec search (index : int) (node : MapNode<'Key, 'Value>) =
                match node with
                | :? MapLeaf<'Key, 'Value> as l ->
                    if index = 0 then Some(l.Key, l.Value)
                    else None
                | :? MapInner<'Key, 'Value> as n ->
                    let lc = index - n.Left.Count
                    if lc < 0 then search index n.Left
                    elif lc > 0 then search (lc - 1) n.Right
                    else Some (n.Key, n.Value)
                | _ ->
                    None
            search index root
        
    member x.TryAtV(index : int) =
        if index < 0 || index >= root.Count then ValueNone
        else 
            let rec search (index : int) (node : MapNode<'Key, 'Value>) =
                match node with
                | :? MapLeaf<'Key, 'Value> as l ->
                    if index = 0 then ValueSome(struct(l.Key, l.Value))
                    else ValueNone
                | :? MapInner<'Key, 'Value> as n ->
                    let lc = index - n.Left.Count
                    if lc < 0 then search index n.Left
                    elif lc > 0 then search (lc - 1) n.Right
                    else ValueSome (struct(n.Key, n.Value))
                | _ ->
                    ValueNone
            search index root

    member x.CompareTo(other : Map<'Key, 'Value>) =
        let mutable le = x.GetEnumerator()
        let mutable re = other.GetEnumerator()

        let mutable result = 0 
        let mutable run = true
        while run do
            if le.MoveNext() then
                if re.MoveNext() then
                    let c = comparer.Compare(le.Current.Key, re.Current.Key)
                    if c <> 0 then 
                        result <- c
                        run <- false
                    else
                        let c = Unchecked.compare le.Current.Value re.Current.Value
                        if c <> 0 then 
                            result <- c
                            run <- false
                else
                    result <- 1
                    run <- false
            elif re.MoveNext() then
                result <- -1
                run <- false
            else
                run <- false
        result

    override x.GetHashCode() =
        hash root

    override x.Equals o =
        match o with
        | :? Map<'Key, 'Value> as o -> equals comparer root o.Root
        | _ -> false

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


    member x.TryGetValue(key : 'Key, [<Out>] value : byref<'Value>) =
        match x.TryFindV key with
        | ValueSome v ->
            value <- v
            true
        | ValueNone ->
            false

    interface System.IComparable with
        member x.CompareTo o = 
            match o with
            | :? Map<'Key, 'Value> as o -> x.CompareTo o
            | _ -> raise <| ArgumentException()

    //interface System.IComparable<Map<'Key, 'Value>> with
    //    member x.CompareTo o = x.CompareTo o

    interface System.Collections.IEnumerable with
        member x.GetEnumerator() = new MapEnumerator<_,_>(root) :> _

    interface System.Collections.Generic.IEnumerable<KeyValuePair<'Key, 'Value>> with
        member x.GetEnumerator() = new MapEnumerator<_,_>(root) :> _
        
    interface System.Collections.Generic.ICollection<KeyValuePair<'Key, 'Value>> with
        member x.Count = x.Count
        member x.IsReadOnly = true
        member x.Clear() = raise <| NotSupportedException()
        member x.Add(_) = raise <| NotSupportedException()
        member x.Remove(_) = raise <| NotSupportedException()
        member x.Contains(kvp : KeyValuePair<'Key, 'Value>) =
            match x.TryFindV kvp.Key with
            | ValueSome v -> Unchecked.equals v kvp.Value
            | ValueNone -> false
        member x.CopyTo(array : KeyValuePair<'Key, 'Value>[], startIndex : int) =
            if startIndex < 0 || startIndex + x.Count > array.Length then raise <| System.IndexOutOfRangeException("Map.CopyTo")
            let rec copyTo (arr : array<_>) (index : int) (n : MapNode<_,_>) =
                match n with
                | :? MapInner<'Key, 'Value> as n ->
                    let index = copyTo arr index n.Left
                    arr.[index] <- KeyValuePair(n.Key, n.Value)
                    copyTo arr (index + 1) n.Right
                | :? MapLeaf<'Key, 'Value> as n ->
                    arr.[index] <- KeyValuePair(n.Key, n.Value)
                    index + 1
                | _ ->
                    index
            copyTo array startIndex root |> ignore<int>
            
    interface System.Collections.Generic.IReadOnlyCollection<KeyValuePair<'Key, 'Value>> with
        member x.Count = x.Count

    interface System.Collections.Generic.IReadOnlyDictionary<'Key, 'Value> with
        member x.Item   
            with get(k : 'Key) = x.[k]

        member x.ContainsKey k = x.ContainsKey k
        member x.Keys = x |> Seq.map (fun (KeyValue(k,_v)) -> k)
        member x.Values = x |> Seq.map (fun (KeyValue(_k,v)) -> v)
        member x.TryGetValue(key : 'Key, [<Out>] value : byref<'Value>) = x.TryGetValue(key, &value)

    interface System.Collections.Generic.IDictionary<'Key, 'Value> with
        member x.TryGetValue(key : 'Key,  [<Out>] value : byref<'Value>) = x.TryGetValue(key, &value)

        member x.Add(_,_) = raise <| NotSupportedException()
        member x.Remove(_) = raise <| NotSupportedException()

        member x.Keys =
            let rec copyTo (arr : array<_>) (index : int) (n : MapNode<_,_>) =
                match n with
                | :? MapInner<'Key, 'Value> as n ->
                    let i = copyTo arr index n.Left
                    arr.[i] <- n.Key
                    copyTo arr (i+1) n.Right
                | :? MapLeaf<'Key, 'Value> as n ->
                    arr.[index] <- n.Key
                    index + 1
                | _ ->
                    index
            let arr = Array.zeroCreate x.Count
            copyTo arr 0 root |> ignore<int>
            arr :> _
            
        member x.Values =
            let rec copyTo (arr : array<_>) (index : int) (n : MapNode<_,_>) =
                match n with
                | :? MapInner<'Key, 'Value> as n ->
                    let i = copyTo arr index n.Left
                    arr.[i] <- n.Value
                    copyTo arr (i+1) n.Right
                | :? MapLeaf<'Key, 'Value> as n ->
                    arr.[index] <- n.Value
                    index + 1
                | _ ->
                    index
            let arr = Array.zeroCreate x.Count
            copyTo arr 0 root |> ignore<int>
            arr :> _

        member x.ContainsKey key =
            x.ContainsKey key

        member x.Item
            with get (key : 'Key) = x.Find key
            and set _ _ = raise <| NotSupportedException()

    new(comparer : IComparer<'Key>) = 
        Map<'Key, 'Value>(comparer, MapEmpty.Instance)

    new(elements : seq<'Key * 'Value>) =
        let m = Map.FromSeq elements
        Map<'Key, 'Value>(m.Comparer, m.Root)

and [<NoComparison; NoEquality>] 
    MapEnumerator<'Key, 'Value> =
    struct
        val mutable public Root : MapNode<'Key, 'Value>
        val mutable public Stack : list<struct(MapNode<'Key, 'Value> * bool)>
        val mutable public Value : KeyValuePair<'Key, 'Value>
        val mutable public Valid : bool

        member x.Current : KeyValuePair<'Key, 'Value> = 
            if x.Valid then x.Value
            else raise <| InvalidOperationException()

        member x.Reset() =
            if x.Root.Height > 0 then
                x.Stack <- [struct(x.Root, true)]
                x.Value <- Unchecked.defaultof<_>
            x.Valid <- false

        member x.Dispose() =
            x.Root <- MapEmpty.Instance
            x.Stack <- []
            x.Value <- Unchecked.defaultof<_>
            x.Valid <- false
                
        member inline private x.MoveNext(deep : bool, top : MapNode<'Key, 'Value>) =
            let mutable top = top
            let mutable run = true

            while run do
                match top with
                | :? MapLeaf<'Key, 'Value> as n ->
                    x.Value <- KeyValuePair(n.Key, n.Value)
                    run <- false

                | :? MapInner<'Key, 'Value> as n ->
                    if deep then
                        if n.Left.Height = 0 then
                            if n.Right.Height > 0 then x.Stack <- struct(n.Right, true) :: x.Stack
                            x.Value <- KeyValuePair(n.Key, n.Value)
                            run <- false
                        else
                            if n.Right.Height > 0 then x.Stack <- struct(n.Right, true) :: x.Stack
                            x.Stack <- struct(n :> MapNode<_,_>, false) :: x.Stack
                            top <- n.Left
                    else    
                        x.Value <- KeyValuePair(n.Key, n.Value)
                        run <- false

                | _ ->
                    failwith "empty node"
    
            
        member x.MoveNext() : bool =
            match x.Stack with
            | struct(n, deep) :: rest ->
                x.Stack <- rest
                x.MoveNext(deep, n)
                x.Valid <- true
                true
            | [] ->
                x.Valid <- false
                false
                            
            
        interface System.Collections.IEnumerator with
            member x.MoveNext() = x.MoveNext()
            member x.Reset() = x.Reset()
            member x.Current = x.Current :> obj

        interface System.Collections.Generic.IEnumerator<KeyValuePair<'Key, 'Value>> with
            member x.Dispose() = x.Dispose()
            member x.Current = x.Current



        new(r : MapNode<'Key, 'Value>) =
            if r.Height = 0 then
                {
                    Valid = false
                    Root = r
                    Stack = []
                    Value = Unchecked.defaultof<_>
                }
            else       
                { 
                    Valid = false
                    Root = r
                    Stack = [struct(r, true)]
                    Value = Unchecked.defaultof<_>
                }

    end

and internal MapDebugView<'Key, 'Value when 'Key : comparison> =

    [<DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>]
    val mutable public Entries : KeyValuePairDebugFriendly<'Key, 'Value>[]

    new(m : Map<'Key, 'Value>) =
        {
            Entries = Seq.toArray (Seq.map KeyValuePairDebugFriendly (Seq.truncate 10000 m))
        }
        
and 
    [<DebuggerDisplay("{keyValue.Value}", Name = "[{keyValue.Key}]", Type = "")>]
    internal  KeyValuePairDebugFriendly<'Key, 'Value>(keyValue : KeyValuePair<'Key, 'Value>) =

        [<DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>]
        member x.KeyValue = keyValue

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix); RequireQualifiedAccess>]
module Map =

    [<GeneralizableValue; CompiledName("Empty")>]
    let empty<'Key, 'Value when 'Key : comparison> = Map<'Key, 'Value>.Empty
    
    [<CompiledName("IsEmpty")>]
    let isEmpty (table : Map<'Key, 'Value>) = table.Count <= 0
    
    [<CompiledName("Count")>]
    let count (table : Map<'Key, 'Value>) = table.Count
    
    [<CompiledName("Add")>]
    let add (key : 'Key) (value : 'Value) (table : Map<'Key, 'Value>) = table.Add(key, value)
    
    [<CompiledName("Remove")>]
    let remove (key : 'Key) (table : Map<'Key, 'Value>) = table.Remove(key)

    [<CompiledName("Change")>]
    let change (key : 'Key) (f : option<'Value> -> option<'Value>) (table : Map<'Key, 'Value>) = table.Change(key, f)
    
    [<CompiledName("ChangeValue")>]
    let changeV (key : 'Key) (update : voption<'Value> -> voption<'Value>) (map : Map<'Key, 'Value>) = map.ChangeV(key, update)

    [<CompiledName("TryFind")>]
    let tryFind (key : 'Key) (table : Map<'Key, 'Value>) = table.TryFind(key)
    
    [<CompiledName("TryFindValue")>]
    let tryFindV (key : 'Key) (map : Map<'Key, 'Value>) = map.TryFindV(key)
    
    [<CompiledName("ContainsKey")>]
    let containsKey (key : 'Key) (table : Map<'Key, 'Value>) = table.ContainsKey(key)
    
    [<CompiledName("Iterate")>]
    let iter (action : 'Key -> 'Value -> unit) (table : Map<'Key, 'Value>) = table.Iter(action)
    
    [<CompiledName("Map")>]
    let map (mapping : 'Key -> 'Value -> 'T) (table : Map<'Key, 'Value>) = table.Map(mapping)
    
    [<CompiledName("Choose")>]
    let choose (mapping : 'Key -> 'Value -> option<'T>) (map : Map<'Key, 'Value>) = map.Choose(mapping)
    
    [<CompiledName("ChooseValue")>]
    let chooseV (mapping : 'Key -> 'Value -> voption<'T>) (map : Map<'Key, 'Value>) = map.ChooseV(mapping)

    [<CompiledName("Filter")>]
    let filter (predicate : 'Key -> 'Value -> bool) (table : Map<'Key, 'Value>) = table.Filter(predicate)

    [<CompiledName("Exists")>]
    let exists (predicate : 'Key -> 'Value -> bool) (table : Map<'Key, 'Value>) = table.Exists(predicate)
    
    [<CompiledName("ForAll")>]
    let forall (predicate : 'Key -> 'Value -> bool) (table : Map<'Key, 'Value>) = table.Forall(predicate)

    [<CompiledName("Fold")>]
    let fold<'Key,'Value,'State when 'Key : comparison> (folder : 'State -> 'Key -> 'Value -> 'State) (state : 'State) (table : Map<'Key, 'Value>) = 
        table.Fold(folder, state)
    
    [<CompiledName("FoldBack")>]
    let foldBack (folder : 'Key -> 'Value -> 'State -> 'State) (table : Map<'Key, 'Value>) (state : 'State) = 
        table.FoldBack(folder, state)

    [<CompiledName("OfSeq")>]
    let ofSeq (elements : seq<'Key * 'Value>) = Map.FromSeq elements
    
    [<CompiledName("OfList")>]
    let ofList (elements : list<'Key * 'Value>) = Map.FromList elements
    
    [<CompiledName("OfArray")>]
    let ofArray (elements : ('Key * 'Value)[]) = Map.FromArray elements
    
    [<CompiledName("OfSeqValue")>]
    let ofSeqV (values : seq<struct('Key * 'Value)>) = Map.FromSeqV values
    
    [<CompiledName("OfListValue")>]
    let ofListV (values : list<struct('Key * 'Value)>) = Map.FromListV values
    
    [<CompiledName("OfArrayValue")>]
    let ofArrayV (values : struct('Key * 'Value)[]) = Map.FromArrayV values

    [<CompiledName("ToSeq")>]
    let toSeq (table : Map<'Key, 'Value>) = table |> Seq.map (fun (KeyValue(k,v)) -> k, v)

    [<CompiledName("ToSeqValue")>]
    let toSeqV (map : Map<'Key, 'Value>) = map |> Seq.map (fun (KeyValue(k,v)) -> struct (k, v))

    [<CompiledName("ToList")>]
    let toList (table : Map<'Key, 'Value>) = table.ToList()
    
    [<CompiledName("ToListValue")>]
    let toListV (map : Map<'Key, 'Value>) = map.ToListV()
    
    [<CompiledName("ToArray")>]
    let toArray (table : Map<'Key, 'Value>) = table.ToArray()
    
    [<CompiledName("ToArrayValue")>]
    let toArrayV (map : Map<'Key, 'Value>) = map.ToArrayV()

    //[<CompiledName("Keys")>]
    //let keys (map : Map<'Key, 'Value>) = map.Keys()

    [<CompiledName("WithMin")>]
    let withMin (minInclusive : 'Key) (map : Map<'Key, 'Value>) = map.WithMin(minInclusive)
    
    [<CompiledName("WithMax")>]
    let withMax (maxInclusive : 'Key) (map : Map<'Key, 'Value>) = map.WithMax(maxInclusive)
    
    [<CompiledName("WithRange")>]
    let withRange (minInclusive : 'Key) (maxInclusive : 'Key) (map : Map<'Key, 'Value>) = map.GetViewBetween(minInclusive, maxInclusive)
    
    [<CompiledName("Union")>]
    let union (map1 : Map<'Key, 'Value>) (map2 : Map<'Key, 'Value>) = Map.Union(map1, map2)
    
    [<CompiledName("UnionMany")>]
    let unionMany (maps : #seq<Map<'Key, 'Value>>) =
        use e = (maps :> seq<_>).GetEnumerator()
        if e.MoveNext() then
            let mutable m = e.Current
            while e.MoveNext() do
                m <- union m e.Current
            m
        else
            empty
    
    [<CompiledName("UnionWith")>]
    let unionWith (resolve : 'Key -> 'Value -> 'Value -> 'Value) (map1 : Map<'Key, 'Value>) (map2 : Map<'Key, 'Value>) = Map.UnionWith(map1, map2, resolve)
    
    [<CompiledName("TryMax")>]
    let tryMax (map : Map<'Key, 'Value>) = map.TryMaxKeyValue()

    [<CompiledName("TryMin")>]
    let tryMin (map : Map<'Key, 'Value>) = map.TryMinKeyValue()

    [<CompiledName("TryMaxValue")>]
    let tryMaxV (map : Map<'Key, 'Value>) = map.TryMaxKeyValueV()

    [<CompiledName("TryMinValue")>]
    let tryMinV (map : Map<'Key, 'Value>) = map.TryMinKeyValueV()
    
    [<CompiledName("TryAt")>]
    let tryAt (index : int) (map : Map<'Key, 'Value>) = map.TryAt index
    
    [<CompiledName("TryAtValue")>]
    let tryAtV (index : int) (map : Map<'Key, 'Value>) = 
        map.TryAtV index
        
    [<CompiledName("Find")>]
    let find (key : 'Key) (table : Map<'Key, 'Value>) =
        table.Find key
        
    [<CompiledName("FindKey")>]
    let findKey (predicate : 'Key -> 'Value -> bool) (table : Map<'Key, 'Value>) =
        table.FindKey(predicate)
        
    [<CompiledName("TryFindKey")>]
    let tryFindKey (predicate : 'Key -> 'Value -> bool) (table : Map<'Key, 'Value>) =
        table.TryFindKey(predicate)
        
    [<CompiledName("TryFindKeyValue")>]
    let tryFindKeyV (predicate : 'Key -> 'Value -> bool) (map : Map<'Key, 'Value>) =
        map.TryFindKeyV(predicate)

    [<CompiledName("TryPick")>]
    let tryPick (chooser : 'Key -> 'Value -> option<'T>) (table : Map<'Key, 'Value>) =
        table.TryPick(chooser)
        
    [<CompiledName("TryPickValue")>]
    let tryPickV (mapping : 'Key -> 'Value -> voption<'T>) (map : Map<'Key, 'Value>) =
        map.TryPickV(mapping)
        
    [<CompiledName("Pick")>]
    let pick (chooser : 'Key -> 'Value -> option<'T>) (table : Map<'Key, 'Value>) =
        table.Pick(chooser)

    [<CompiledName("PickValue")>]
    let pickV (mapping : 'Key -> 'Value -> voption<'T>) (map : Map<'Key, 'Value>) =
        map.PickV(mapping)
        
    [<CompiledName("Partition")>]
    let partition (predicate : 'Key -> 'Value -> bool) (table : Map<'Key, 'Value>) =
        table.Partition(predicate)

