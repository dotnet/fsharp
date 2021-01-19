// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Internal.Utilities

open System.Collections
open System.Collections.Generic
open System.Collections.Immutable
open System.Linq

type 'T block = System.Collections.Immutable.ImmutableArray<'T>

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Block =

    let append(l1 : 'T block) (l2 : 'T block) = l1.AddRange(l2)

    let empty<'T> = block<'T>.Empty

(*
    let collect (f: 'T -> 'T block) (x:block<_>) = 
        match x.array with 
        | null -> block.Empty 
        | arr -> 
           if arr.Length = 1 then f arr.[0]
           else block(Array.map (fun x -> match (f x).array with null -> [| |] | arr -> arr) arr |> Array.concat)

    let concat (x:block<_>) = 

    let exists f (x:block<_>) = 
        match x.array with 
        | null -> false 
        | arr -> Array.exists f arr

    let filter f (x:block<_>) = 
        match x.array with 
        | null -> block.Empty 
        | arr -> block(Array.filter f arr)

    let fold f acc (x:block<_>) = 
        match x.array with 
        | null -> acc 
        | arr -> Array.fold f acc arr

    let fold2 f acc (x:block<_>) (y:block<_>) = 
        match x.array,y.array with 
        | null,null -> acc 
        | null,_ | _,null -> invalidArg "x" "mismatched list lengths"
        | arr1,arr2 -> Array.fold2 f acc arr1 arr2

    let foldBack f (x:block<_>) acc  = 
        match x.array with 
        | null -> acc 
        | arr -> Array.foldBack f arr acc

    let foldBack2 f (x:block<_>) (y:block<_>) acc = 
        match x.array,y.array with 
        | null,null -> acc 
        | null,_ | _,null -> invalidArg "x" "mismatched list lengths"
        | arr1,arr2 -> Array.foldBack2 f arr1 arr2 acc

    let forall f (x:block<_>) = 
        match x.array with 
        | null -> true 
        | arr -> Array.forall f arr

    let forall2 f (x1:block<_>) (x2:block<_>) = 
        match x1.array, x2.array with 
        | null,null -> true
        | null,_ | _,null -> invalidArg "x1" "mismatched list lengths"
        | arr1,arr2 -> Array.forall2 f arr1 arr2

    let init n f = 
        if n = 0 then 
            block.Empty 
        else 
            block(Array.init n f)

    let isEmpty (x:block<_>) = x.IsEmpty

    let iter f (x:block<_>) = 
        match x.array with 
        | null -> ()
        | arr -> Array.iter f arr

    let iter2 f (x1:block<_>) (x2:block<_>) = 
        match x1.array, x2.array with 
        | null,null -> ()
        | null,_ | _,null -> invalidArg "x1" "mismatched list lengths"
        | arr1,arr2 -> Array.iter2 f arr1 arr2

    let iteri f (x:block<_>) = 
        match x.array with 
        | null -> ()
        | arr -> Array.iteri f arr

    let length (x:block<_>) = x.Length

    let map f (x:block<_>) = 
        match x.array with 
        | null -> block.Empty 
        | arr -> block(Array.map f arr)

    let map2 f (x:block<_>) (y:block<_>) = 
        match x.array,y.array with 
        | null,null -> block.Empty 
        | null,_ | _,null -> invalidArg "x" "mismatched list lengths"
        | arr1,arr2 -> block(Array.map2 f arr1 arr2)

    let mapi f (x:block<_>) = 
        match x.array with 
        | null -> block.Empty 
        | arr -> block(Array.mapi f arr)
*)
    let ofArray (l: 'T[]) = ImmutableArray.CreateRange(l)

    let ofList (l: 'T list) = ImmutableArray.CreateRange(l)

    let ofSeq (l: 'T seq) = ImmutableArray.CreateRange(l)

    let toArray (b: 'T block) = b.ToArray()
    let toList (b: 'T block) = List.ofSeq b
    let toSeq (b: 'T block) = (b :> 'T seq)

(*
    let partition f (x:block<_>) = 
        match x.array with 
        | null -> block.Empty,block.Empty 
        | arr -> 
            let arr1,arr2 = Array.partition f arr 
            block(arr1),block(arr2)

    let physicalEquality (x:block<_>) (y:block<_>) = 
        LanguagePrimitives.PhysicalEquality x.array y.array 

    let rev (x:block<_>) = 
        match x.array with 
        | null -> block.Empty 
        | arr -> block(Array.rev arr)

    let singleton(x) = block([| x |])

    let sum (x:block<int>) = 
        match x.array with 
        | null -> 0 
        | arr -> Array.sum arr

    let sumBy (f: 'T -> int) (x:'T block) = 
        match x.array with 
        | null -> 0 
        | arr -> Array.sumBy f arr

    let toList (x:block<_>) = 
        match x.array with 
        | null -> [] 
        | arr -> Array.toList arr

    let toMap (x:block<_>) = match x.array with null -> Map.empty | arr -> Map.ofArray arr

    let tryFind f (x:block<_>) = 
        match x.array with 
        | null -> None 
        | arr -> Array.tryFind f arr

    let unzip (x:block<_>) = 
        match x.array with 
        | null -> block.Empty,block.Empty 
        | arr -> let arr1,arr2 = Array.unzip arr in block(arr1),block(arr2)

    let zip (x:block<_>) (y:block<_>) = 
        match x.array,y.array with 
        | null,null -> block.Empty
        | null,_ | _,null -> invalidArg "x" "mismatched list lengths"
        | arr1,arr2 -> block(Array.zip arr1 arr2)
*)

[<AutoOpen>]
module BlockAutoOpens =

    //let block (x: 'T[]) = Block.ofArray x
    let (|Block|) (x: 'T block) = Block.toArray x

    type ImmutableArray<'T> with
        member inline x.IsEmpty = x.Length = 0

        member inline x.GetSlice(start: int option, finish: int option) =
            let start = defaultArg start 0
            let finish = match finish with None -> x.Length-1 | Some v -> v
            let len = finish-start+1
            ImmutableArray.Create(x, start, len)
