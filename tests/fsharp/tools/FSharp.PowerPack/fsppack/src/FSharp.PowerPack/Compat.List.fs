#nowarn "62"

namespace Microsoft.FSharp.Compatibility


open Microsoft.FSharp.Core.OptimizedClosures
open System.Collections.Generic
open System.Diagnostics


module List = 

    let invalidArg arg msg = raise (new System.ArgumentException((msg:string),(arg:string)))        

    let nonempty x = match x with [] -> false | _ -> true
    let rec contains x l = match l with [] -> false | h::t -> x = h || contains x t
    let mem x l = contains x l
    let rec memq x l = 
        match l with 
        | [] -> false 
        | h::t -> LanguagePrimitives.PhysicalEquality x h || memq x t

    let rec rev_map2_acc (f:FSharpFunc<_,_,_>) l1 l2 acc =
        match l1,l2 with 
        | [],[] -> acc
        | h1::t1, h2::t2 -> rev_map2_acc f t1 t2 (f.Invoke(h1,h2) :: acc)
        | _ -> invalidArg "l2" "the lists have different lengths"

    let rev_map2 f l1 l2 = 
        let f = FSharpFunc<_,_,_>.Adapt(f)
        rev_map2_acc f l1 l2 []

    let rec rev_append l1 l2 = 
        match l1 with 
        | [] -> l2
        | h::t -> rev_append t (h::l2)


    let rec rev_map_acc f l acc =
        match l with 
        | [] -> acc
        | h::t -> rev_map_acc f t (f h :: acc)

    let rev_map f l = rev_map_acc f l []

    let indexNotFound() = raise (new System.Collections.Generic.KeyNotFoundException("An index satisfying the predicate was not found in the collection"))

    let rec assoc x l = 
        match l with 
        | [] -> indexNotFound()
        | ((h,r)::t) -> if x = h then r else assoc x t

    let rec try_assoc x l = 
        match l with 
        | [] -> None
        | ((h,r)::t) -> if x = h then Some(r) else try_assoc x t

    let rec mem_assoc x l = 
        match l with 
        | [] -> false
        | ((h,_)::t) -> x = h || mem_assoc x t

    let rec remove_assoc x l = 
        match l with 
        | [] -> []
        | (((h,_) as p) ::t) -> if x = h then t else p:: remove_assoc x t

    let rec assq x l = 
        match l with 
        | [] -> indexNotFound()
        | ((h,r)::t) -> if LanguagePrimitives.PhysicalEquality x h then r else assq x t

    let rec try_assq x l = 
        match l with 
        | [] -> None
        | ((h,r)::t) -> if LanguagePrimitives.PhysicalEquality x h then Some r else try_assq x t

    let rec mem_assq x l = 
        match l with 
        | [] -> false
        | ((h,_)::t) -> LanguagePrimitives.PhysicalEquality x h || mem_assq x t

    let rec remove_assq x l = 
        match l with 
        | [] -> []
        | (((h,_) as p) ::t) -> if LanguagePrimitives.PhysicalEquality x h then t else p:: remove_assq x t

    let scanReduce f l = 
        match l with 
        | [] -> invalidArg "l" "the input list is empty"
        | (h::t) -> List.scan f h t

    let scanArraySubRight<'T,'State> (f:FSharpFunc<'T,'State,'State>) (arr:_[]) start fin initState = 
        let mutable state = initState  
        let mutable res = [state]  
        for i = fin downto start do
            state <- f.Invoke(arr.[i], state);
            res <- state :: res
        res

    let scanReduceBack f l = 
        match l with 
        | [] -> invalidArg "l" "the input list is empty"
        | _ -> 
            let f = FSharpFunc<_,_,_>.Adapt(f)
            let arr = Array.ofList l 
            let arrn = Array.length arr 
            scanArraySubRight f arr 0 (arrn - 2) arr.[arrn - 1]

    let fold_left f z xs = List.fold f z xs

    let fold_left2 f z xs1 xs2 = List.fold2 f z xs1 xs2

    let fold_right f xs z = List.foldBack f xs z

    let fold_right2 f xs1 xs2 z = List.foldBack2 f xs1 xs2 z

    let for_all f xs = List.forall f xs

    let for_all2 f xs1 xs2 = List.forall2 f xs1 xs2

    let stable_sort f xs = List.sortWith f xs

    let split x =  List.unzip x

    let combine x1 x2 =  List.zip x1 x2

    let find_all f x = List.filter f x

    let flatten (list:seq<list<_>>) = List.concat list

    let of_array (array:'T array) = List.ofArray array

    let to_array (list:'T list) = List.toArray list

    let hd list = List.head list

    let tl list = List.tail list

