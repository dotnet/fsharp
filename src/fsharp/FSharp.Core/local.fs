// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Primitives.Basics 

open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
open Microsoft.FSharp.Core.LanguagePrimitives.ErrorStrings
open Microsoft.FSharp.Collections
open Microsoft.FSharp.Core.Operators
open System.Diagnostics.CodeAnalysis                                    
open System.Collections.Generic
open System.Runtime.InteropServices
#if FX_NO_ICLONEABLE
open Microsoft.FSharp.Core.ICloneableExtensions            
#else
#endif  


module internal List = 

    let arrayZeroCreate (n:int) = (# "newarr !0" type ('T) n : 'T array #)

    [<SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")>]      
    let nonempty x = match x with [] -> false | _ -> true

    let rec iter f x = match x with [] -> () | (h::t) -> f h; iter f t

    // optimized mutation-based implementation. This code is only valid in fslib, where mutation of private
    // tail cons cells is permitted in carefully written library code.
    let inline setFreshConsTail cons t = cons.(::).1 <- t
    let inline freshConsNoTail h = h :: (# "ldnull" : 'T list #)

    let rec distinctToFreshConsTail cons (hashSet:HashSet<_>) list = 
        match list with
        | [] -> setFreshConsTail cons []
        | (x::rest) ->
            if hashSet.Add(x) then
                let cons2 = freshConsNoTail x
                setFreshConsTail cons cons2
                distinctToFreshConsTail cons2 hashSet rest
            else
                distinctToFreshConsTail cons hashSet rest

    let distinct (comparer: System.Collections.Generic.IEqualityComparer<'T>) (list:'T list) =       
        match list with
        | [] -> []
        | [h] -> [h]
        | (x::rest) ->
            let hashSet =  System.Collections.Generic.HashSet<'T>(comparer)
            hashSet.Add(x) |> ignore
            let cons = freshConsNoTail x
            distinctToFreshConsTail cons hashSet rest
            cons

    let rec distinctByToFreshConsTail cons (hashSet:HashSet<_>) keyf list = 
        match list with
        | [] -> setFreshConsTail cons []
        | (x::rest) ->
            if hashSet.Add(keyf x) then
                let cons2 = freshConsNoTail x
                setFreshConsTail cons cons2
                distinctByToFreshConsTail cons2 hashSet keyf rest
            else
                distinctByToFreshConsTail cons hashSet keyf rest

    let distinctBy (comparer: System.Collections.Generic.IEqualityComparer<'Key>) (keyf:'T -> 'Key) (list:'T list) =       
        match list with
        | [] -> []
        | [h] -> [h]
        | (x::rest) ->
            let hashSet = System.Collections.Generic.HashSet<'Key>(comparer)
            hashSet.Add(keyf x) |> ignore
            let cons = freshConsNoTail x
            distinctByToFreshConsTail cons hashSet keyf rest
            cons

    let rec mapToFreshConsTail cons f x = 
        match x with
        | [] -> 
            setFreshConsTail cons [];
        | (h::t) -> 
            let cons2 = freshConsNoTail (f h)
            setFreshConsTail cons cons2;
            mapToFreshConsTail cons2 f t

    let map f x = 
        match x with
        | [] -> []
        | [h] -> [f h]
        | (h::t) -> 
            let cons = freshConsNoTail (f h)
            mapToFreshConsTail cons f t
            cons

    let rec mapiToFreshConsTail cons (f:OptimizedClosures.FSharpFunc<_,_,_>) x i = 
        match x with
        | [] -> 
            setFreshConsTail cons [];
        | (h::t) -> 
            let cons2 = freshConsNoTail (f.Invoke(i,h))
            setFreshConsTail cons cons2;
            mapiToFreshConsTail cons2 f t (i+1)

    let mapi f x = 
        match x with
        | [] -> []
        | [h] -> [f 0 h]
        | (h::t) -> 
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)
            let cons = freshConsNoTail (f.Invoke(0,h))
            mapiToFreshConsTail cons f t 1
            cons

    let rec map2ToFreshConsTail cons (f:OptimizedClosures.FSharpFunc<_,_,_>) xs1 xs2 = 
        match xs1,xs2 with
        | [],[] -> 
            setFreshConsTail cons [];
        | (h1::t1),(h2::t2) -> 
            let cons2 = freshConsNoTail (f.Invoke(h1,h2))
            setFreshConsTail cons cons2;
            map2ToFreshConsTail cons2 f t1 t2
        | _ -> invalidArg "xs2" (SR.GetString(SR.listsHadDifferentLengths))

    let map2 f xs1 xs2 = 
        match xs1,xs2 with
        | [],[] -> []
        | (h1::t1),(h2::t2) -> 
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)
            let cons = freshConsNoTail (f.Invoke(h1,h2))
            map2ToFreshConsTail cons f t1 t2
            cons
        | _ -> invalidArg "xs2" (SR.GetString(SR.listsHadDifferentLengths))

    let rec forall f xs1 = 
        match xs1 with 
        | [] -> true
        | (h1::t1) -> f h1 && forall f t1

    let rec exists f xs1 = 
        match xs1 with 
        | [] -> false
        | (h1::t1) -> f h1 || exists f t1

    // optimized mutation-based implementation. This code is only valid in fslib, where mutation of private
    // tail cons cells is permitted in carefully written library code.
    let rec revAcc xs acc = 
        match xs with 
        | [] -> acc
        | h::t -> revAcc t (h::acc)

    let rev xs = 
        match xs with 
        | [] -> xs
        | [_] -> xs
        | h1::h2::t -> revAcc t [h2;h1]

    // return the last cons it the chain
    let rec appendToFreshConsTail cons xs = 
        match xs with 
        | [] -> 
            setFreshConsTail cons xs // note, xs = []
            cons
        | h::t -> 
            let cons2 = freshConsNoTail h
            setFreshConsTail cons cons2
            appendToFreshConsTail cons2 t

    // optimized mutation-based implementation. This code is only valid in fslib, where mutation of private
    // tail cons cells is permitted in carefully written library code.
    let rec collectToFreshConsTail (f:'T -> 'U list) (list:'T list) cons = 
        match list with 
        | [] -> 
            setFreshConsTail cons []
        | h::t -> 
            collectToFreshConsTail f t (appendToFreshConsTail cons (f h))

    let rec collect (f:'T -> 'U list) (list:'T list) = 
        match list with
        | [] -> []
        | [h] -> f h
        | _ ->
            let cons = freshConsNoTail (Unchecked.defaultof<'U>)
            collectToFreshConsTail f list cons
            cons.Tail 

    // optimized mutation-based implementation. This code is only valid in fslib, where mutation of private
    // tail cons cells is permitted in carefully written library code.
    let rec filterToFreshConsTail cons f l = 
        match l with 
        | [] -> 
            setFreshConsTail cons l; // note, l = nil
        | h::t -> 
            if f h then 
                let cons2 = freshConsNoTail h 
                setFreshConsTail cons cons2;
                filterToFreshConsTail cons2 f t
            else 
                filterToFreshConsTail cons f t
      
    let rec filter f l = 
        match l with 
        | [] -> l
        | h :: ([] as nil) -> if f h then l else nil
        | h::t -> 
            if f h then   
                let cons = freshConsNoTail h 
                filterToFreshConsTail cons f t; 
                cons
            else 
                filter f t

    let iteri f x = 
        let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)
        let rec loop n x = match x with [] -> () | (h::t) -> f.Invoke(n,h); loop (n+1) t
        loop 0 x

    // optimized mutation-based implementation. This code is only valid in fslib, where mutation of private
    // tail cons cells is permitted in carefully written library code.
    let rec concatToFreshConsTail cons h1 l = 
        match l with 
        | [] -> setFreshConsTail cons h1
        | h2::t -> concatToFreshConsTail (appendToFreshConsTail cons h1) h2 t
      
    // optimized mutation-based implementation. This code is only valid in fslib, where mutation of private
    // tail cons cells is permitted in carefully written library code.
    let rec concatToEmpty l = 
        match l with 
        | [] -> []
        | []::t -> concatToEmpty t 
        | (h::t1)::tt2 -> 
            let res = freshConsNoTail h
            concatToFreshConsTail res t1 tt2;
            res

    let seqToList (e : IEnumerable<'T>) = 
        match e with 
        | :? list<'T> as l -> l
        | _ -> 
            use ie = e.GetEnumerator()
            let mutable res = [] 
            while ie.MoveNext() do
                res <- ie.Current :: res
            rev res

    let concat (l : seq<_>) = 
        match seqToList l with 
        | [] -> []
        | [h] -> h
        | [h1;h2] -> h1 @ h2
        | l -> concatToEmpty l

    let rec initToFreshConsTail cons i n f = 
        if i < n then 
            let cons2 = freshConsNoTail (f i)
            setFreshConsTail cons cons2;
            initToFreshConsTail cons2 (i+1) n f 
        else 
            setFreshConsTail cons []
           
      
    let init count f = 
        if count < 0 then  invalidArg "count" InputMustBeNonNegativeString
        if count = 0 then [] 
        else 
            let res = freshConsNoTail (f 0)
            initToFreshConsTail res 1 count f
            res

    let rec takeFreshConsTail cons n l =
        if n = 0 then setFreshConsTail cons [] else
        match l with
        | [] -> raise <| System.InvalidOperationException (SR.GetString(SR.notEnoughElements))
        | x::xs ->
            let cons2 = freshConsNoTail x
            setFreshConsTail cons cons2
            takeFreshConsTail cons2 (n - 1) xs
 
    let take n l =
        if n < 0 then invalidArg "count" InputMustBeNonNegativeString
        if n = 0 then [] else 
        match l with
        | [] -> raise <| System.InvalidOperationException (SR.GetString(SR.notEnoughElements))
        | x::xs ->
            let cons = freshConsNoTail x
            takeFreshConsTail cons (n - 1) xs
            cons
      
    // optimized mutation-based implementation. This code is only valid in fslib, where mutation of private
    // tail cons cells is permitted in carefully written library code.
    let rec partitionToFreshConsTails consL consR p l = 
        match l with 
        | [] -> 
            setFreshConsTail consL l; // note, l = nil
            setFreshConsTail consR l; // note, l = nil
            
        | h::t -> 
            let cons' = freshConsNoTail h
            if p h then 
                setFreshConsTail consL cons';
                partitionToFreshConsTails cons' consR p t
            else 
                setFreshConsTail consR cons';
                partitionToFreshConsTails consL cons' p t
      
    let rec partitionToFreshConsTailLeft consL p l = 
        match l with 
        | [] -> 
            setFreshConsTail consL l; // note, l = nil
            l // note, l = nil
        | h::t -> 
            let cons' = freshConsNoTail h 
            if p h then 
                setFreshConsTail consL cons';
                partitionToFreshConsTailLeft cons'  p t
            else 
                partitionToFreshConsTails consL cons' p t; 
                cons'

    let rec partitionToFreshConsTailRight consR p l = 
        match l with 
        | [] -> 
            setFreshConsTail consR l; // note, l = nil
            l // note, l = nil
        | h::t -> 
            let cons' = freshConsNoTail h 
            if p h then 
                partitionToFreshConsTails cons' consR p t; 
                cons'
            else 
                setFreshConsTail consR cons';
                partitionToFreshConsTailRight cons' p t

    let partition p l = 
        match l with 
        | [] -> l,l
        | h :: ([] as nil) -> if p h then l,nil else nil,l
        | h::t -> 
            let cons = freshConsNoTail h 
            if p h 
            then cons, (partitionToFreshConsTailLeft cons p t)
            else (partitionToFreshConsTailRight cons p t), cons
           
    // optimized mutation-based implementation. This code is only valid in fslib, where mutation of private
    // tail cons cells is permitted in carefully written library code.
    let rec unzipToFreshConsTail cons1a cons1b x = 
        match x with 
        | [] -> 
            setFreshConsTail cons1a []
            setFreshConsTail cons1b []
        | ((h1,h2)::t) -> 
            let cons2a = freshConsNoTail h1
            let cons2b = freshConsNoTail h2
            setFreshConsTail cons1a cons2a;
            setFreshConsTail cons1b cons2b;
            unzipToFreshConsTail cons2a cons2b t

    // optimized mutation-based implementation. This code is only valid in fslib, where mutation of private
    // tail cons cells is permitted in carefully written library code.
    let unzip x = 
        match x with 
        | [] -> 
            [],[]
        | ((h1,h2)::t) -> 
            let res1a = freshConsNoTail h1
            let res1b = freshConsNoTail h2
            unzipToFreshConsTail res1a res1b t; 
            res1a,res1b

    // optimized mutation-based implementation. This code is only valid in fslib, where mutation of private
    // tail cons cells is permitted in carefully written library code.
    let rec unzip3ToFreshConsTail cons1a cons1b cons1c x = 
        match x with 
        | [] -> 
            setFreshConsTail cons1a [];
            setFreshConsTail cons1b [];
            setFreshConsTail cons1c [];
        | ((h1,h2,h3)::t) -> 
            let cons2a = freshConsNoTail h1
            let cons2b = freshConsNoTail h2
            let cons2c = freshConsNoTail h3
            setFreshConsTail cons1a cons2a;
            setFreshConsTail cons1b cons2b;
            setFreshConsTail cons1c cons2c;
            unzip3ToFreshConsTail cons2a cons2b cons2c t

    // optimized mutation-based implementation. This code is only valid in fslib, where mutation of private
    // tail cons cells is permitted in carefully written library code.
    let unzip3 x = 
        match x with 
        | [] -> 
            [],[],[]
        | ((h1,h2,h3)::t) -> 
            let res1a = freshConsNoTail h1
            let res1b = freshConsNoTail h2
            let res1c = freshConsNoTail h3 
            unzip3ToFreshConsTail res1a res1b res1c t; 
            res1a,res1b,res1c

    // optimized mutation-based implementation. This code is only valid in fslib, where mutation of private
    // tail cons cells is permitted in carefully written library code.
    let rec zipToFreshConsTail cons xs1 xs2 = 
        match xs1,xs2 with 
        | [],[] -> 
            setFreshConsTail cons []
        | (h1::t1),(h2::t2) -> 
            let cons2 = freshConsNoTail (h1,h2)
            setFreshConsTail cons cons2;
            zipToFreshConsTail cons2 t1 t2
        | _ -> 
            invalidArg "xs2" (SR.GetString(SR.listsHadDifferentLengths))

    // optimized mutation-based implementation. This code is only valid in fslib, where mutation of private
    // tail cons cells is permitted in carefully written library code.
    let zip  xs1 xs2 = 
        match xs1,xs2 with 
        | [],[] -> []
        | (h1::t1),(h2::t2) -> 
            let res = freshConsNoTail (h1,h2)
            zipToFreshConsTail res t1 t2; 
            res
        | _ -> 
            invalidArg "xs2" (SR.GetString(SR.listsHadDifferentLengths))

    // optimized mutation-based implementation. This code is only valid in fslib, where mutation of private
    // tail cons cells is permitted in carefully written library code.
    let rec zip3ToFreshConsTail cons xs1 xs2 xs3 = 
        match xs1,xs2,xs3 with 
        | [],[],[] -> 
            setFreshConsTail cons [];
        | (h1::t1),(h2::t2),(h3::t3) -> 
            let cons2 = freshConsNoTail (h1,h2,h3)
            setFreshConsTail cons cons2;
            zip3ToFreshConsTail cons2 t1 t2 t3
        | _ -> 
            invalidArg "xs1" (SR.GetString(SR.listsHadDifferentLengths))

    // optimized mutation-based implementation. This code is only valid in fslib, where mutation of private
    // tail cons cells is permitted in carefully written library code.
    let zip3 xs1 xs2 xs3 = 
        match xs1,xs2,xs3 with 
        | [],[],[] -> 
            []
        | (h1::t1),(h2::t2),(h3::t3) -> 
            let res = freshConsNoTail (h1,h2,h3) 
            zip3ToFreshConsTail res t1 t2 t3; 
            res
        | _ -> 
            invalidArg "xs1" (SR.GetString(SR.listsHadDifferentLengths))

    let toArray (l:'T list) =
        let len = l.Length 
        let res = arrayZeroCreate len 
        let rec loop i l = 
            match l with 
            | [] -> ()
            | h::t -> 
                res.[i] <- h
                loop (i+1) t
        loop 0 l
        res

    let ofArray (arr:'T[]) =
        let len = arr.Length
        let mutable res = ([]: 'T list) 
        for i = len - 1 downto 0 do 
            res <- arr.[i] :: res
        res

    // NOTE: This implementation is now only used for List.sortWith. We should change that to use the stable sort via arrays
    // below, and remove this implementation.
    module StableSortImplementation =
        // Internal copy of stable sort
        let rec revAppend xs1 xs2 = 
            match xs1 with 
            | [] -> xs2
            | h::t -> revAppend t (h::xs2)
        let half x = x >>> 1 

        let rec merge cmp a b acc = 
            match a,b with 
            | [], a | a,[] -> revAppend acc a
            | x::a', y::b' -> if cmp x y > 0 then merge cmp a  b' (y::acc) else merge cmp a' b  (x::acc)

        let sort2 cmp x y = 
            if cmp x y > 0 then [y;x] else [x;y]

        let sort3 cmp x y z = 
            let cxy = cmp x y
            let cyz = cmp y z
            if cxy > 0 && cyz < 0 then 
                if cmp x z > 0 then [y;z;x] else [y;x;z]
            elif cxy < 0 && cyz > 0 then 
                if cmp x z > 0 then [z;x;y] else [x;z;y]
            elif cxy > 0 then 
                if cyz > 0 then  [z;y;x]
                else [y;z;x]
            else 
                if cyz > 0 then [z;x;y]
                else [x;y;z] 

        let trivial a = match a with [] | [_] -> true | _ -> false
            
        (* tail recursive using a ref *)

        let rec stableSortInner cmp la ar =
          if la < 4 then (* sort two || three new entries *)
            match !ar with 
             | x::y::b -> 
                  if la = 2 then ( ar := b; sort2 cmp x y )
                  else begin
                    match b with 
                    | z::c -> ( ar := c; sort3 cmp x y z )
                    | _ -> failwith "never" 
                  end
             | _ -> failwith "never"
          else (* divide *)
            let lb = half la
            let sb = stableSortInner cmp lb ar
            let sc = stableSortInner cmp (la - lb) ar
            merge cmp sb sc []

        let stableSort cmp (a: 'T list) = 
            if trivial a then a else
            let ar = ref a
            stableSortInner cmp a.Length ar
        
    let sortWith cmp a = StableSortImplementation.stableSort cmp a
    
module internal Array = 

    open System
    open System.Collections.Generic

#if FX_NO_ARRAY_KEY_SORT
    // Mimic behavior of BCL QSort routine, used under the hood by various array sorting APIs
    let qsort<'Key,'Value>(keys : 'Key[], values : 'Value[], start : int, last : int, comparer : IComparer<'Key>) =  
            let valuesExist = 
                match values with
                | null -> false
                | _ -> true
                
            let swap (p1, p2) =
                let tk = keys.[p1]
                keys.[p1] <- keys.[p2]
                keys.[p2] <- tk
                if valuesExist then
                    let tv = values.[p1]
                    values.[p1] <- values.[p2]
                    values.[p2] <- tv
                    
            let partition (left, right, pivot) =
                let value = keys.[pivot]
                swap (pivot, right)
                let mutable store = left
                
                for i in left..(right - 1) do
                    if comparer.Compare(keys.[i],value) < 0 then
                        swap(i, store)
                        store <- store + 1

                swap (store, right)
                store
            
            let rec qs (left, right) =
                if left < right then
                    let pivot = left + (right-left)/2
                    let newpivot = partition(left,right,pivot)
                    qs(left,newpivot - 1)
                    qs(newpivot+1,right)
            
            qs(start, last)
            
    type System.Array with
        static member Sort<'Key,'Value when 'Key : comparison>(keys : 'Key[], values : 'Value[], comparer : IComparer<'Key>) =
            let valuesExist = 
                match values with
                | null -> false
                | _ -> true
            match keys,values with
            | null,_ -> raise (ArgumentNullException())
            | _,_ when valuesExist && (keys.Length <> values.Length) -> raise (ArgumentException())
            | _,_ ->
            let comparer = match comparer with null -> LanguagePrimitives.FastGenericComparer<'Key> | _ -> comparer
            qsort(keys, values, 0, keys.Length-1, comparer)
        static member Sort<'Key,'Value  when 'Key : comparison>(keys : 'Key[], values : 'Value[]) =
            let valuesExist = 
                match values with
                | null -> false
                | _ -> true
            match keys,values with
            | null,_ -> raise (ArgumentNullException())
            | _,_ when valuesExist && (keys.Length <> values.Length) -> raise (ArgumentException())
            | _,_ ->   
            qsort(keys,values,0,keys.Length-1,LanguagePrimitives.FastGenericComparer<'Key>)
(*
        static member Sort<'Key,'Value when 'Key : comparison>(keys : 'Key[], values : 'Value[], start : int, last : int) =
            match keys with
            | null -> raise (ArgumentNullException())
            | _ ->        
            qsort(keys,values,start,last,LanguagePrimitives.FastGenericComparer<'Key>)
*)
        static member Sort<'Key,'Value when 'Key : comparison>(keys : 'Key[], values : 'Value[], start : int, length : int, comparer : IComparer<'Key>) =
            match keys with
            | null -> raise (ArgumentNullException())
            | _ ->        
            let comparer = match comparer with null -> LanguagePrimitives.FastGenericComparer<'Key> | _ -> comparer
            qsort(keys,values,start,start+length-1,comparer)
#else
#endif

    // The input parameter should be checked by callers if necessary
    let inline zeroCreateUnchecked (count:int) = 
        (# "newarr !0" type ('T) count : 'T array #)

    let inline init (count:int) (f: int -> 'T) = 
        if count < 0 then invalidArg "count" InputMustBeNonNegativeString
        let arr = (zeroCreateUnchecked count : 'T array)  
        for i = 0 to count - 1 do 
            arr.[i] <- f i
        arr

    let permute indexMap (arr : _[]) = 
        let res  = zeroCreateUnchecked arr.Length
        let inv = zeroCreateUnchecked arr.Length
        for i = 0 to arr.Length - 1 do 
            let j = indexMap i 
            if j < 0 || j >= arr.Length then invalidArg "indexMap" (SR.GetString(SR.notAPermutation))
            res.[j] <- arr.[i]
            inv.[j] <- 1uy
        for i = 0 to arr.Length - 1 do 
            if inv.[i] <> 1uy then invalidArg "indexMap" (SR.GetString(SR.notAPermutation))
        res


    let unstableSortInPlaceBy (f: 'T -> 'U) (array : array<'T>) =
        let len = array.Length 
        if len < 2 then () 
        else
            let keys = zeroCreateUnchecked array.Length
            for i = 0 to array.Length - 1 do 
                keys.[i] <- f array.[i]
            System.Array.Sort<_,_>(keys, array, LanguagePrimitives.FastGenericComparerCanBeNull<_>)


    let unstableSortInPlace (array : array<'T>) = 
        let len = array.Length 
        if len < 2 then () 
        else System.Array.Sort<_>(array, LanguagePrimitives.FastGenericComparerCanBeNull<_>)

    let stableSortWithKeys (array:array<'T>) (keys:array<'Key>)  =
        // 'places' is an array or integers storing the permutation performed by the sort
        let places = zeroCreateUnchecked array.Length 
        for i = 0 to array.Length - 1 do 
            places.[i] <- i 

        let cFast = LanguagePrimitives.FastGenericComparerCanBeNull<'Key>
        System.Array.Sort<_,_>(keys, places, cFast) 
        // 'array2' is a copy of the original values
        let array2 = (array.Clone() :?> array<'T>)

        // 'c' is a comparer for the keys
        let c = LanguagePrimitives.FastGenericComparer<'Key>

        // Walk through any chunks where the keys are equal
        let mutable i = 0
        let len = array.Length
        while i <  len do 
            let mutable j = i
            let ki = keys.[i]
            while j < len && (j = i || c.Compare(ki, keys.[j]) = 0) do 
               j <- j + 1
            // Copy the values into the result array and re-sort the chunk if needed by the original place indexes
            for n = i to j - 1 do
               array.[n] <- array2.[places.[n]]
            if j - i >= 2 then 
                System.Array.Sort<_,_>(places, array, i, j-i, null) 
            i <- j

    let stableSortInPlaceBy (f: 'T -> 'U) (array : array<'T>) =
        let len = array.Length 
        if len < 2 then () 
        else
            // 'keys' is an array storing the projected keys
            let keys = zeroCreateUnchecked array.Length
            for i = 0 to array.Length - 1 do 
                keys.[i] <- f array.[i]
            stableSortWithKeys array keys

    let stableSortInPlace (array : array<'T>) =
        let len = array.Length 
        if len < 2 then () 
        else
            let cFast = LanguagePrimitives.FastGenericComparerCanBeNull<'T>
            match cFast with 
            | null -> 
                // An optimization for the cases where the keys and values coincide and do not have identity, e.g. are integers
                // In this case an unstable sort is just as good as a stable sort (and faster)
                System.Array.Sort<_,_>(array, null) 
            | _ -> 
                // 'keys' is an array storing the projected keys
                let keys = (array.Clone() :?> array<'T>)
                stableSortWithKeys array keys
