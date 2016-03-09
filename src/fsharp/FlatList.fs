// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Internal.Utilities 

open System.Collections
open System.Collections.Generic

//-------------------------------------------------------------------------
// Library: flat list  (immutable arrays)
//------------------------------------------------------------------------
#if FLAT_LIST_AS_ARRAY_STRUCT
//#else
[<Struct>]
type internal FlatList<'T> =
    val internal array : 'T[]
    internal new (arr: 'T[]) = { array = (match arr with null -> null | arr -> if arr.Length = 0 then null else arr) }
    member x.Item with get(n:int) = x.array.[n]
    member x.Length = match x.array with null -> 0 | arr -> arr.Length
    member x.IsEmpty = match x.array with null -> true | _ -> false
    static member Empty : FlatList<'T> = FlatList(null)
    interface IEnumerable<'T> with 
        member x.GetEnumerator() : IEnumerator<'T> = 
            match x.array with 
            | null -> Seq.empty.GetEnumerator()
            | arr -> (arr :> IEnumerable<'T>).GetEnumerator()
    interface IEnumerable with 
        member x.GetEnumerator() : IEnumerator = 
            match x.array with 
            | null -> (Seq.empty :> IEnumerable).GetEnumerator()
            | arr -> (arr :> IEnumerable).GetEnumerator()


[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module internal FlatList =

    let empty<'T> = FlatList<'T>.Empty

    let collect (f: 'T -> FlatList<'T>) (x:FlatList<_>) = 
        match x.array with 
        | null -> FlatList.Empty 
        | arr -> 
           if arr.Length = 1 then f arr.[0]
           else FlatList(Array.map (fun x -> match (f x).array with null -> [| |] | arr -> arr) arr |> Array.concat)

    let exists f (x:FlatList<_>) = 
        match x.array with 
        | null -> false 
        | arr -> Array.exists f arr

    let filter f (x:FlatList<_>) = 
        match x.array with 
        | null -> FlatList.Empty 
        | arr -> FlatList(Array.filter f arr)

    let fold f acc (x:FlatList<_>) = 
        match x.array with 
        | null -> acc 
        | arr -> Array.fold f acc arr

    let fold2 f acc (x:FlatList<_>) (y:FlatList<_>) = 
        match x.array,y.array with 
        | null,null -> acc 
        | null,_ | _,null -> invalidArg "x" "mismatched list lengths"
        | arr1,arr2 -> Array.fold2 f acc arr1 arr2

    let foldBack f (x:FlatList<_>) acc  = 
        match x.array with 
        | null -> acc 
        | arr -> Array.foldBack f arr acc

    let foldBack2 f (x:FlatList<_>) (y:FlatList<_>) acc = 
        match x.array,y.array with 
        | null,null -> acc 
        | null,_ | _,null -> invalidArg "x" "mismatched list lengths"
        | arr1,arr2 -> Array.foldBack2 f arr1 arr2 acc

    let map2 f (x:FlatList<_>) (y:FlatList<_>) = 
        match x.array,y.array with 
        | null,null -> FlatList.Empty 
        | null,_ | _,null -> invalidArg "x" "mismatched list lengths"
        | arr1,arr2 -> FlatList(Array.map2 f arr1 arr2)

    let forall f (x:FlatList<_>) = 
        match x.array with 
        | null -> true 
        | arr -> Array.forall f arr

    let forall2 f (x1:FlatList<_>) (x2:FlatList<_>) = 
        match x1.array, x2.array with 
        | null,null -> true
        | null,_ | _,null -> invalidArg "x1" "mismatched list lengths"
        | arr1,arr2 -> Array.forall2 f arr1 arr2

    let iter2 f (x1:FlatList<_>) (x2:FlatList<_>) = 
        match x1.array, x2.array with 
        | null,null -> ()
        | null,_ | _,null -> invalidArg "x1" "mismatched list lengths"
        | arr1,arr2 -> Array.iter2 f arr1 arr2

    let partition f (x:FlatList<_>) = 
        match x.array with 
        | null -> FlatList.Empty,FlatList.Empty 
        | arr -> 
            let arr1,arr2 = Array.partition f arr 
            FlatList(arr1),FlatList(arr2)

    let (* inline *) sum (x:FlatList<int>) = 
        match x.array with 
        | null -> 0 
        | arr -> Array.sum arr

    let (* inline *) sumBy (f: 'T -> int) (x:FlatList<'T>) = 
        match x.array with 
        | null -> 0 
        | arr -> Array.sumBy f arr

    let unzip (x:FlatList<_>) = 
        match x.array with 
        | null -> FlatList.Empty,FlatList.Empty 
        | arr -> let arr1,arr2 = Array.unzip arr in FlatList(arr1),FlatList(arr2)

    let physicalEquality (x:FlatList<_>) (y:FlatList<_>) = 
        LanguagePrimitives.PhysicalEquality x.array y.array 

    let tryFind f (x:FlatList<_>) = 
        match x.array with 
        | null -> None 
        | arr -> Array.tryFind f arr

    let concat (x:FlatList<_>) = 
        match x.array with 
        | null -> FlatList.Empty 
        | arr -> FlatList(Array.concat arr)

    let isEmpty (x:FlatList<_>) = x.IsEmpty
    let one(x) = FlatList([| x |])

    let toMap (x:FlatList<_>) = match x.array with null -> Map.empty | arr -> Map.ofArray arr
    let length (x:FlatList<_>) = x.Length

    let map f (x:FlatList<_>) = 
        match x.array with 
        | null -> FlatList.Empty 
        | arr -> FlatList(Array.map f arr)

    let mapi f (x:FlatList<_>) = 
        match x.array with 
        | null -> FlatList.Empty 
        | arr -> FlatList(Array.mapi f arr)

    let iter f (x:FlatList<_>) = 
        match x.array with 
        | null -> ()
        | arr -> Array.iter f arr

    let iteri f (x:FlatList<_>) = 
        match x.array with 
        | null -> ()
        | arr -> Array.iteri f arr

    let toList (x:FlatList<_>) = 
        match x.array with 
        | null -> [] 
        | arr -> Array.toList arr

    let append(l1 : FlatList<'T>) (l2 : FlatList<'T>) = 
        match l1.array, l2.array with 
        | null,_ -> l2
        | _,null -> l1
        | arr1, arr2 -> FlatList(Array.append arr1 arr2)
        
    let ofSeq l = 
        FlatList(Array.ofSeq l)

    let ofList l = 
        match l with 
        | [] -> FlatList.Empty 
        | l -> FlatList(Array.ofList l)

    let init n f = 
        if n = 0 then 
            FlatList.Empty 
        else 
            FlatList(Array.init n f)

    let zip (x:FlatList<_>) (y:FlatList<_>) = 
        match x.array,y.array with 
        | null,null -> FlatList.Empty
        | null,_ | _,null -> invalidArg "x" "mismatched list lengths"
        | arr1,arr2 -> FlatList(Array.zip arr1 arr2)

#endif
#if FLAT_LIST_AS_LIST

#else
type internal FlatList<'T> ='T list

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module internal FlatList =
    let empty<'T> : 'T list = []
    let collect (f: 'T -> FlatList<'T>) (x:FlatList<_>) =  List.collect f x
    let exists f (x:FlatList<_>) = List.exists f x
    let filter f (x:FlatList<_>) = List.filter f x
    let fold f acc (x:FlatList<_>) = List.fold f acc x
    let fold2 f acc (x:FlatList<_>) (y:FlatList<_>) = List.fold2 f acc x y
    let foldBack f (x:FlatList<_>) acc  = List.foldBack f x acc
    let foldBack2 f (x:FlatList<_>) (y:FlatList<_>) acc = List.foldBack2 f x y acc
    let map2 f (x:FlatList<_>) (y:FlatList<_>) = List.map2 f x y
    let forall f (x:FlatList<_>) = List.forall f x
    let forall2 f (x1:FlatList<_>) (x2:FlatList<_>) = List.forall2 f x1 x2
    let iter2 f (x1:FlatList<_>) (x2:FlatList<_>) = List.iter2 f x1 x2 
    let partition f (x:FlatList<_>) = List.partition f x
    let (* inline *) sum (x:FlatList<int>) = List.sum x
    let (* inline *) sumBy (f: 'T -> int) (x:FlatList<'T>) = List.sumBy f x
    let unzip (x:FlatList<_>) = List.unzip x
    let physicalEquality (x:FlatList<_>) (y:FlatList<_>) = (LanguagePrimitives.PhysicalEquality x y)
    let tryFind f (x:FlatList<_>) = List.tryFind f x
    let concat (x:FlatList<_>) = List.concat x
    let isEmpty (x:FlatList<_>) = List.isEmpty x
    let one(x) = [x]
    let toMap (x:FlatList<_>) = Map.ofList x
    let length (x:FlatList<_>) = List.length x
    let map f (x:FlatList<_>) = List.map f x
    let mapi f (x:FlatList<_>) = List.mapi f x
    let iter f (x:FlatList<_>) = List.iter f x
    let iteri f (x:FlatList<_>) = List.iteri f x
    let toList (x:FlatList<_>) = x
    let ofSeq (x:seq<_>) = List.ofSeq x
    let append(l1 : FlatList<'T>) (l2 : FlatList<'T>) =  List.append l1 l2
    let ofList(l) = l
    let init n f = List.init n f
    let zip (x:FlatList<_>) (y:FlatList<_>) = List.zip x y
#endif

#if FLAT_LIST_AS_ARRAY
//#else
type internal FlatList<'T> ='T array

type internal FlatListEmpty<'T>() =
    // cache the empty array in a generic static field
    static let empty : 'T array = [| |]
    static member Empty : 'T array = empty

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module internal FlatList =
    let empty<'T> : 'T array = FlatListEmpty<'T>.Empty
    //let empty<'T> : 'T array = [| |]
    let collect (f: 'T -> FlatList<'T>) (x:FlatList<_>) =  x |> Array.map f |> Array.concat 
    let exists f x = Array.exists f x
    let filter f x = Array.filter f x
    let fold f acc x = Array.fold f acc x
    let fold2 f acc x y = Array.fold2 f acc x y
    let foldBack f x acc  = Array.foldBack f x acc
    let foldBack2 f x y acc = Array.foldBack2 f x y acc
    let map2 f x y = Array.map2 f x y
    let forall f x = Array.forall f x
    let forall2 f x1 x2 = Array.forall2 f x1 x2
    let iter2 f x1 x2 = Array.iter2 f x1 x2 
    let partition f x = Array.partition f x
    let (* inline *) sum (x:FlatList<int>) = Array.sum x
    let (* inline *) sumBy (f: 'T -> int) (x:FlatList<'T>) = Array.sumBy f x
    let unzip x = Array.unzip x
    let physicalEquality (x:FlatList<_>) (y:FlatList<_>) = LanguagePrimitives.PhysicalEquality x y
    let tryFind f x = Array.tryFind f x
    let concat x = Array.concat x
    let isEmpty x = Array.isEmpty x
    let one x = [| x |]
    let toMap x = Map.ofArray x
    let length x = Array.length x
    let map f x = Array.map f x
    let mapi f x = Array.mapi f x
    let iter f x = Array.iter f x
    let iteri f x = Array.iteri f x
    let toList x = Array.toList x
    let append l1 l2  =  Array.append l1 l2
    let ofSeq l = Array.ofSeq l
    let ofList l = Array.ofList l
    let init n f = Array.init n f
    let zip  x y  = Array.zip x y
#endif

