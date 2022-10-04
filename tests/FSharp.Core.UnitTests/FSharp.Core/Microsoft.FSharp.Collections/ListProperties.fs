// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.Core.UnitTests.Collections

open System
open System.Collections.Generic
open Xunit
open FsCheck
open Utils

type ListProperties () =
    inherit TestClassWithSimpleNameAppDomainResolver ()

    member _.chunkBySize_and_collect<'a when 'a : equality> (xs : 'a list) size =
        size > 0 ==> (lazy
            let a = List.chunkBySize size xs
            let b = List.collect id a
            b = xs)

    [<Fact>]
    member this.``chunkBySize is reversable with collect`` () =
        Check.QuickThrowOnFailure this.chunkBySize_and_collect<int>
        Check.QuickThrowOnFailure this.chunkBySize_and_collect<string>
        Check.QuickThrowOnFailure this.chunkBySize_and_collect<NormalFloat>

    member _.windowed_and_length<'a when 'a : equality> (xs : 'a list) size =
        size > 0 ==> (lazy
            List.windowed size xs
            |> List.forall (fun x -> x.Length = size))


    [<Fact>]
    member this.``windowed returns list with correct length`` () =
        Check.QuickThrowOnFailure this.windowed_and_length<int>
        Check.QuickThrowOnFailure this.windowed_and_length<string>
        Check.QuickThrowOnFailure this.windowed_and_length<NormalFloat>

    member _.windowed_and_order<'a when 'a : equality> (listsize:PositiveInt) size =
        size > 1 ==> (lazy
            let xs = [1..(int listsize)]

            List.windowed size xs
            |> List.forall (fun w -> w = List.sort w))

    [<Fact>]
    member this.``windowed returns succeeding elements`` () =
        Check.QuickThrowOnFailure this.windowed_and_order<int>
        Check.QuickThrowOnFailure this.windowed_and_order<string>
        Check.QuickThrowOnFailure this.windowed_and_order<NormalFloat>

    member _.partition_and_sort<'a when 'a : comparison> (xs : 'a list) =
        let rec qsort xs = 
            match xs with
            | [] -> []
            | (x:'a)::xs -> 
                let smaller,larger = List.partition (fun y -> y <= x) xs
                qsort smaller @ [x] @ qsort larger

        qsort xs = (List.sort xs)

    [<Fact>]
    member this.``partition can be used to sort`` () =
        Check.QuickThrowOnFailure this.partition_and_sort<int>
        Check.QuickThrowOnFailure this.partition_and_sort<string>
        Check.QuickThrowOnFailure this.partition_and_sort<NormalFloat>

    member _.windowed_and_pairwise<'a when 'a : equality> (xs : 'a list) =
        let a = List.windowed 2 xs
        let b = List.pairwise xs |> List.map (fun (x,y) -> [x;y])

        a = b

    [<Fact>]
    member this.``windowed 2 is like pairwise`` () =
        Check.QuickThrowOnFailure this.windowed_and_pairwise<int>
        Check.QuickThrowOnFailure this.windowed_and_pairwise<string>
        Check.QuickThrowOnFailure this.windowed_and_pairwise<NormalFloat>

    [<Fact>]
    member this.``chunkBySize produces chunks exactly of size `chunkSize`, except the last one, which can be smaller, but not empty``() =
        let prop (a: _ list) (PositiveInt chunkSize) =   
            match a |> List.chunkBySize chunkSize |> Seq.toList with
            | [] -> a = []
            | h :: [] -> h.Length <= chunkSize
            | chunks ->
                let lastChunk = chunks |> List.last
                let headChunks = chunks |> Seq.take (chunks.Length - 1) |> Seq.toList

                headChunks |> List.forall (List.length >> (=) chunkSize)
                &&
                lastChunk <> []
                &&
                lastChunk.Length <= chunkSize

        Check.QuickThrowOnFailure prop

    member _.splitInto_and_collect<'a when 'a : equality> (xs : 'a list) count =
        count > 0 ==> (lazy
            let a = List.splitInto count xs
            let b = List.collect id a
            b = xs)

    [<Fact>]
    member  this.``splitInto is reversable with collect`` () =
        Check.QuickThrowOnFailure this.splitInto_and_collect<int>
        Check.QuickThrowOnFailure this.splitInto_and_collect<string>
        Check.QuickThrowOnFailure this.splitInto_and_collect<NormalFloat>

    member _.splitAt_and_append<'a when 'a : equality> (xs : 'a list) index =
        (index >= 0 && index <= xs.Length) ==> (lazy
            let a1,a2 = List.splitAt index xs
            let b = List.append a1 a2
            b = xs && a1.Length = index)

    [<Fact>]
    member this.``splitAt is reversable with append`` () =
        Check.QuickThrowOnFailure this.splitAt_and_append<int>
        Check.QuickThrowOnFailure this.splitAt_and_append<string>
        Check.QuickThrowOnFailure this.splitAt_and_append<NormalFloat>

    member _.indexed_and_zip<'a when 'a : equality> (xs : 'a list) =
        let a = [0..xs.Length-1]
        let b = List.indexed xs
        b = List.zip a xs

    [<Fact>]
    member this.``indexed is adding correct indexes`` () =
        Check.QuickThrowOnFailure this.indexed_and_zip<int>
        Check.QuickThrowOnFailure this.indexed_and_zip<string>
        Check.QuickThrowOnFailure this.indexed_and_zip<NormalFloat>

    member _.zip_and_zip3<'a when 'a : equality> (xs' : ('a*'a*'a) list) =
        let xs = List.map (fun (x,y,z) -> x) xs'
        let xs2 = List.map (fun (x,y,z) -> y) xs'
        let xs3 = List.map (fun (x,y,z) -> z) xs'

        let a = List.zip3 xs xs2 xs3
        let b = List.zip (List.zip xs xs2) xs3 |> List.map (fun ((a,b),c) -> a,b,c)
        a = b

    [<Fact>]
    member this.``two zips can be used for zip3`` () =
        Check.QuickThrowOnFailure this.zip_and_zip3<int>
        Check.QuickThrowOnFailure this.zip_and_zip3<string>
        Check.QuickThrowOnFailure this.zip_and_zip3<NormalFloat>

    member _.zip_and_unzip<'a when 'a : equality> (xs' : ('a*'a) list) =
        let xs,xs2 = List.unzip xs'
        List.zip xs xs2 = xs'

    [<Fact>]
    member this.``zip and unzip are dual`` () =
        Check.QuickThrowOnFailure this.zip_and_unzip<int>
        Check.QuickThrowOnFailure this.zip_and_unzip<string>
        Check.QuickThrowOnFailure this.zip_and_unzip<NormalFloat>

    member _.zip3_and_unzip3<'a when 'a : equality> (xs' : ('a*'a*'a) list) =
        let xs,xs2,xs3 = List.unzip3 xs'
        List.zip3 xs xs2 xs3 = xs'

    [<Fact>]
    member this.``zip3 and unzip3 are dual`` () =
        Check.QuickThrowOnFailure this.zip3_and_unzip3<int>
        Check.QuickThrowOnFailure this.zip3_and_unzip3<string>
        Check.QuickThrowOnFailure this.zip3_and_unzip3<NormalFloat>

    [<Fact>]
    member _.``splitInto produces chunks exactly `count` chunks with equal size (+/- 1)``() =
        let prop (a: _ list) (PositiveInt count') =
            let count = min a.Length count'
            match a |> List.splitInto count' |> Seq.toList with
            | [] -> a = []
            | h :: [] -> (a.Length = 1 || count = 1) && h = a
            | chunks ->
                let lastChunk = chunks |> List.last
                let lastLength = lastChunk |> List.length

                chunks.Length = count
                &&
                chunks |> List.forall (fun c -> List.length c = lastLength || List.length c = lastLength + 1)

        Check.QuickThrowOnFailure prop

    member _.sort_and_sortby (xs : list<float>) (xs2 : list<float>) =
        let a = List.sortBy id xs |> Seq.toArray 
        let b = List.sort xs |> Seq.toArray
        let mutable result = true
        for i in 0 .. a.Length - 1 do
            if a.[i] <> b.[i] then
                if System.Double.IsNaN a.[i] <> System.Double.IsNaN b.[i] then
                    result <- false
        result 

    [<Fact>]
    member this.``sort behaves like sortby id`` () =   
        Check.QuickThrowOnFailure this.sort_and_sortby

    member _. filter_and_except<'a when 'a : comparison>  (xs : list<'a>) (itemsToExclude : Set<'a>) =
        let a = List.filter (fun x -> Set.contains x itemsToExclude |> not) xs |> List.distinct
        let b = List.except itemsToExclude xs
        a = b

    [<Fact>]
    member this.``filter and except work similar`` () =   
        Check.QuickThrowOnFailure this.filter_and_except<int>
        Check.QuickThrowOnFailure this.filter_and_except<string>
        Check.QuickThrowOnFailure this.filter_and_except<NormalFloat>    

    member _.filter_and_where<'a when 'a : comparison>  (xs : list<'a>) predicate =
        let a = List.filter predicate xs
        let b = List.where predicate xs
        a = b

    [<Fact>]
    member this.``filter and where work similar`` () =   
        Check.QuickThrowOnFailure this.filter_and_where<int>
        Check.QuickThrowOnFailure this.filter_and_where<string>
        Check.QuickThrowOnFailure this.filter_and_where<NormalFloat>

    member _.find_and_pick<'a when 'a : comparison>  (xs : list<'a>) predicate =
        let a = runAndCheckIfAnyError (fun () -> List.find predicate xs)
        let b = runAndCheckIfAnyError (fun () -> List.pick (fun x -> if predicate x then Some x else None) xs)
        a = b

    [<Fact>]
    member this.``pick works like find`` () =   
        Check.QuickThrowOnFailure this.find_and_pick<int>
        Check.QuickThrowOnFailure this.find_and_pick<string>
        Check.QuickThrowOnFailure this.find_and_pick<NormalFloat>

    member _.choose_and_pick<'a when 'a : comparison>  (xs : list<'a>) predicate =
        let a = runAndCheckIfAnyError (fun () -> List.choose predicate xs |> List.head)
        let b = runAndCheckIfAnyError (fun () -> List.pick predicate xs)
        a = b

    [<Fact>]
    member this.``pick works like choose + head`` () =   
        Check.QuickThrowOnFailure this.choose_and_pick<int>
        Check.QuickThrowOnFailure this.choose_and_pick<string>
        Check.QuickThrowOnFailure this.choose_and_pick<NormalFloat>

    member _.head_and_tail<'a when 'a : comparison>  (xs : list<'a>) =
        xs <> [] ==> (lazy
            let h = List.head xs
            let t = List.tail xs
            xs = h :: t)

    [<Fact>]
    member this.``head and tail gives the list`` () =   
        Check.QuickThrowOnFailure this.head_and_tail<int>
        Check.QuickThrowOnFailure this.head_and_tail<string>
        Check.QuickThrowOnFailure this.head_and_tail<NormalFloat>

    member _.tryHead_and_tail<'a when 'a : comparison>  (xs : list<'a>) =
        match xs with
        | [] -> List.tryHead xs = None
        | _ ->
            let h = (List.tryHead xs).Value
            let t = List.tail xs
            xs = h :: t

    [<Fact>]
    member this.``tryHead and tail gives the list`` () =   
        Check.QuickThrowOnFailure this.tryHead_and_tail<int>
        Check.QuickThrowOnFailure this.tryHead_and_tail<string>
        Check.QuickThrowOnFailure this.tryHead_and_tail<NormalFloat>

    member _.skip_and_take<'a when 'a : comparison>  (xs : list<'a>) (count:NonNegativeInt) =
        let count = int count
        if xs <> [] && count <= xs.Length then
            let s = List.skip count xs
            let t = List.take count xs
            xs = t @ s
        else true

    [<Fact>]
    member this.``skip and take gives the list`` () =   
        Check.QuickThrowOnFailure this.skip_and_take<int>
        Check.QuickThrowOnFailure this.skip_and_take<string>
        Check.QuickThrowOnFailure this.skip_and_take<NormalFloat>

    member _.truncate_and_take<'a when 'a : comparison>  (xs : list<'a>) (count:NonNegativeInt) =
        let count = int count
        if xs <> [] && count <= xs.Length then
            let a = List.take (min count xs.Length) xs
            let b = List.truncate count xs
            a = b
        else true

    [<Fact>]
    member this.``truncate and take work similar`` () =   
        Check.QuickThrowOnFailure this.truncate_and_take<int>
        Check.QuickThrowOnFailure this.truncate_and_take<string>
        Check.QuickThrowOnFailure this.truncate_and_take<NormalFloat>

    member this.skipWhile_and_takeWhile<'a when 'a : comparison>  (xs : list<'a>) f =
        if xs <> [] then
            let s = List.skipWhile f xs
            let t = List.takeWhile f xs
            xs = t @ s
        else true

    [<Fact>]
    member this.``skipWhile and takeWhile gives the list`` () =   
        Check.QuickThrowOnFailure this.skipWhile_and_takeWhile<int>
        Check.QuickThrowOnFailure this.skipWhile_and_takeWhile<string>
        Check.QuickThrowOnFailure this.skipWhile_and_takeWhile<NormalFloat>

    member this.find_and_exists<'a when 'a : comparison>  (xs : list<'a>) f =
        let a = 
            try
                List.find f xs |> ignore
                true
            with
            | _ -> false
        let b = List.exists f xs
        a = b

    [<Fact>]
    member this.``find and exists work similar`` () =   
        Check.QuickThrowOnFailure this.find_and_exists<int>
        Check.QuickThrowOnFailure this.find_and_exists<string>
        Check.QuickThrowOnFailure this.find_and_exists<NormalFloat>

    member _.exists_and_forall<'a when 'a : comparison>  (xs : list<'a>) (F (_, predicate)) =
        let a = List.forall (predicate >> not) xs
        let b = List.exists predicate xs
        a = not b

    [<Fact>]
    member this.``exists and forall are dual`` () =   
        Check.QuickThrowOnFailure this.exists_and_forall<int>
        Check.QuickThrowOnFailure this.exists_and_forall<string>
        Check.QuickThrowOnFailure this.exists_and_forall<NormalFloat>

    member _.head_and_isEmpty<'a when 'a : comparison>  (xs : list<'a>) =
        let a = 
            try
                List.head xs |> ignore
                true
            with
            | _ -> false
        let b = List.isEmpty xs

        a = not b

    [<Fact>]
    member this.``head fails when list isEmpty`` () =   
        Check.QuickThrowOnFailure this.head_and_isEmpty<int>
        Check.QuickThrowOnFailure this.head_and_isEmpty<string>
        Check.QuickThrowOnFailure this.head_and_isEmpty<NormalFloat>

    member _.head_and_last<'a when 'a : comparison>  (xs : list<'a>) =
        let a = run (fun () -> xs |> List.rev |> List.last)
        let b = run (fun () -> List.head xs)
        a = b

    [<Fact>]
    member this.``head is the same as last of a reversed list`` () =   
        Check.QuickThrowOnFailure this.head_and_last<int>
        Check.QuickThrowOnFailure this.head_and_last<string>
        Check.QuickThrowOnFailure this.head_and_last<NormalFloat>

    member _.head_and_item<'a when 'a : comparison>  (xs : list<'a>) =
        let a = runAndCheckErrorType (fun () -> xs |> List.item 0)
        let b = runAndCheckErrorType (fun () -> List.head xs)

        a = b

    [<Fact>]
    member this.``head is the same as item 0`` () =   
        Check.QuickThrowOnFailure this.head_and_item<int>
        Check.QuickThrowOnFailure this.head_and_item<string>
        Check.QuickThrowOnFailure this.head_and_item<NormalFloat>

    member _.item_and_tryItem<'a when 'a : comparison>  (xs : list<'a>) pos =
        let a = runAndCheckErrorType (fun () -> xs |> List.item pos)
        let b = List.tryItem pos xs

        match a with
        | Success a -> b.Value = a
        | _ -> b = None

    [<Fact>]
    member this.``tryItem is safe item`` () =   
        Check.QuickThrowOnFailure this.item_and_tryItem<int>
        Check.QuickThrowOnFailure this.item_and_tryItem<string>
        Check.QuickThrowOnFailure this.item_and_tryItem<NormalFloat>

    member _.pick_and_tryPick<'a when 'a : comparison>  (xs : list<'a>) f =
        let a = runAndCheckErrorType (fun () -> xs |> List.pick f)
        let b = List.tryPick f xs

        match a with
        | Success a -> b.Value = a
        | _ -> b = None

    [<Fact>]
    member this.``tryPick is safe pick`` () =   
        Check.QuickThrowOnFailure this.pick_and_tryPick<int>
        Check.QuickThrowOnFailure this.pick_and_tryPick<string>
        Check.QuickThrowOnFailure this.pick_and_tryPick<NormalFloat>

    member _.last_and_tryLast<'a when 'a : comparison>  (xs : list<'a>) =
        let a = runAndCheckErrorType (fun () -> xs |> List.last)
        let b = List.tryLast xs

        match a with
        | Success a -> b.Value = a
        | _ -> b = None

    [<Fact>]
    member this.``tryLast is safe last`` () =   
        Check.QuickThrowOnFailure this.last_and_tryLast<int>
        Check.QuickThrowOnFailure this.last_and_tryLast<string>
        Check.QuickThrowOnFailure this.last_and_tryLast<NormalFloat>

    member _.length_and_isEmpty<'a when 'a : comparison>  (xs : list<'a>) =
        let a = List.length xs = 0
        let b = List.isEmpty xs

        a = b

    [<Fact>]
    member this.``list isEmpty if and only if length is 0`` () =   
        Check.QuickThrowOnFailure this.length_and_isEmpty<int>
        Check.QuickThrowOnFailure this.length_and_isEmpty<string>
        Check.QuickThrowOnFailure this.length_and_isEmpty<NormalFloat>

    member _.min_and_max (xs : list<int>) =
        let a = run (fun () -> List.min xs)
        let b = run (fun () -> xs |> List.map ((*) -1) |> List.max |> fun x -> -x)

        a = b

    [<Fact>]
    member this.``min is opposite of max`` () =   
        Check.QuickThrowOnFailure this.min_and_max

    member _.minBy_and_maxBy (xs : list<int>) f =
        let a = run (fun () -> List.minBy f xs)
        let b = run (fun () -> xs |> List.map ((*) -1) |> List.maxBy f |> fun x -> -x)

        a = b

    [<Fact>]
    member this.``minBy is opposite of maxBy`` () =   
        Check.QuickThrowOnFailure this.minBy_and_maxBy

    member _.minBy_and_min (xs : list<int>) =
        let a = run (fun () -> List.minBy id xs)
        let b = run (fun () -> xs |> List.min)

        a = b

    [<Fact>]
    member this.``minBy id is same as min`` () =   
        Check.QuickThrowOnFailure this.minBy_and_min

    member _.min_and_sort<'a when 'a : comparison>  (xs : list<'a>) =
        let a = runAndCheckErrorType (fun () -> List.min xs)
        let b = runAndCheckErrorType (fun () -> xs |> List.sort |> List.head)

        a = b

    [<Fact>]
    member this.``head element after sort is min element`` () =   
        Check.QuickThrowOnFailure this.min_and_sort<int>
        Check.QuickThrowOnFailure this.min_and_sort<string>
        Check.QuickThrowOnFailure this.min_and_sort<NormalFloat>

    member _.pairwise<'a when 'a : comparison>  (xs : list<'a>) =
        let xs' = List.pairwise xs 
        let f = xs' |> List.map fst
        let s = xs' |> List.map snd
        let a = List.length xs'
        let b = List.length xs

        if xs = [] then 
            xs' = []
        else 
            a = b - 1 &&
              f = (xs |> List.rev |> List.tail |> List.rev) && // all elements but last one
              s = (xs |> List.tail) // all elements but first one

    [<Fact>]
    member this.``pairwise works as expected`` () =   
        Check.QuickThrowOnFailure this.pairwise<int>
        Check.QuickThrowOnFailure this.pairwise<string>
        Check.QuickThrowOnFailure this.pairwise<NormalFloat>

    member _.permute<'a when 'a : comparison>  (xs' : list<int*'a>) =
        let xs = List.map snd xs'
 
        let permutations = 
            List.map fst xs'
            |> List.indexed
            |> List.sortBy snd
            |> List.map fst
            |> List.indexed
            |> dict

        let permutation x = permutations.[x]


        match run (fun () -> xs |> List.indexed |> List.permute permutation) with
        | Success s ->         
            let originals = s |> List.map fst
            let rs = s |> List.map snd
            for o in originals do
                let x' = xs |> List.item o
                let x = rs |> List.item (permutation o)
                Assert.AreEqual(x',x)
            true
        | _ -> true

    [<Fact>]
    member this.``permute works as expected`` () =   
        Check.QuickThrowOnFailure this.permute<int>
        Check.QuickThrowOnFailure this.permute<string>
        Check.QuickThrowOnFailure this.permute<NormalFloat>

    member _.mapi_and_map<'a when 'a : comparison>  (xs : list<'a>) f =
        let indices = System.Collections.Generic.List<int>()
        let f' i x =
            indices.Add i
            f x
        let a = List.map f xs
        let b = List.mapi f' xs

        a = b && (Seq.toList indices = [0..xs.Length-1])

    [<Fact>]
    member this.``mapi behaves like map with correct order`` () =   
        Check.QuickThrowOnFailure this.mapi_and_map<int>
        Check.QuickThrowOnFailure this.mapi_and_map<string>
        Check.QuickThrowOnFailure this.mapi_and_map<NormalFloat>

    member _.reduce_and_fold<'a when 'a : comparison> (xs : list<'a>) seed (F (_, f)) =
        match xs with
        | [] -> List.fold f seed xs = seed
        | _ ->
            let ar = xs |> List.fold f seed
            let br = seed :: xs  |> List.reduce f
            ar = br

    [<Fact>]
    member this.``reduce works like fold with given seed`` () =
        Check.QuickThrowOnFailure this.reduce_and_fold<int>
        Check.QuickThrowOnFailure this.reduce_and_fold<string>
        Check.QuickThrowOnFailure this.reduce_and_fold<NormalFloat>

    member _.scan_and_fold<'a when 'a : comparison> (xs : list<'a>) seed (F (_, f)) =
        let ar : 'a list = List.scan f seed xs
        let f' (l,c) x =
            let c' = f c x
            c'::l,c'

        let br,_ = List.fold f' ([seed],seed) xs
        ar = List.rev br

    [<Fact>]
    member this.``scan works like fold but returns intermediate values`` () =
        Check.QuickThrowOnFailure this.scan_and_fold<int>
        Check.QuickThrowOnFailure this.scan_and_fold<string>
        Check.QuickThrowOnFailure this.scan_and_fold<NormalFloat>

    member _.scanBack_and_foldBack<'a when 'a : comparison> (xs : list<'a>) seed (F (_, f)) =
        let ar : 'a list = List.scanBack f xs seed
        let f' x (l,c) =
            let c' = f x c
            c'::l,c'

        let br,_ = List.foldBack f' xs ([seed],seed)
        ar = br

    [<Fact>]
    member this.``scanBack works like foldBack but returns intermediate values`` () =
        Check.QuickThrowOnFailure this.scanBack_and_foldBack<int>
        Check.QuickThrowOnFailure this.scanBack_and_foldBack<string>
        Check.QuickThrowOnFailure this.scanBack_and_foldBack<NormalFloat>

    member _.reduceBack_and_foldBack<'a when 'a : comparison> (xs : list<'a>) seed (F (_, f)) =
        match xs with
        | [] -> List.foldBack f xs seed = seed
        | _ ->
            let ar = List.foldBack f xs seed
            let br = List.reduceBack f (xs @ [seed])
            ar = br

    [<Fact>]
    member this.``reduceBack works like foldBack with given seed`` () =
        Check.QuickThrowOnFailure this.reduceBack_and_foldBack<int>
        Check.QuickThrowOnFailure this.reduceBack_and_foldBack<string>
        Check.QuickThrowOnFailure this.reduceBack_and_foldBack<NormalFloat>

    member _.replicate<'a when 'a : comparison> (x:'a) (count:NonNegativeInt) =
        let count = int count
        let xs = List.replicate count x
        xs.Length = count && List.forall ((=) x) xs

    [<Fact>]
    member this.``replicate creates n instances of the given element`` () =
        Check.QuickThrowOnFailure this.replicate<int>
        Check.QuickThrowOnFailure this.replicate<string>
        Check.QuickThrowOnFailure this.replicate<NormalFloat>

    member _.singleton_and_replicate<'a when 'a : comparison> (x:'a) (count:NonNegativeInt) =
        let count = int count
        let xs = List.replicate count x
        let ys = [for i in 1..count -> List.singleton x] |> List.concat
        xs = ys

    [<Fact>]
    member this.``singleton can be used to replicate`` () =
        Check.QuickThrowOnFailure this.singleton_and_replicate<int>
        Check.QuickThrowOnFailure this.singleton_and_replicate<string>
        Check.QuickThrowOnFailure this.singleton_and_replicate<NormalFloat>

    member _.mapFold_and_map_and_fold<'a when 'a : comparison> (xs : list<'a>) mapF foldF start =
        let f s x = 
            let x' = mapF x
            let s' = foldF s x'
            x',s'

        let a,ar = xs |> List.mapFold f start
        let b = xs |> List.map mapF 
        let br = b |> List.fold foldF start
        a = b && ar = br

    [<Fact>]
    member this.``mapFold works like map + fold`` () =   
        Check.QuickThrowOnFailure this.mapFold_and_map_and_fold<int>
        Check.QuickThrowOnFailure this.mapFold_and_map_and_fold<string>
        Check.QuickThrowOnFailure this.mapFold_and_map_and_fold<NormalFloat>

    member _.mapFoldBack_and_map_and_foldBack<'a when 'a : comparison> (xs : list<'a>) mapF foldF start =
        let f x s = 
            let x' = mapF x
            let s' = foldF x' s
            x',s'

        let a,ar = List.mapFoldBack f xs start
        let b = xs |> List.map mapF 
        let br = List.foldBack foldF b start
        a = b && ar = br

    [<Fact>]
    member this.``mapFoldBack works like map + foldBack`` () =   
        Check.QuickThrowOnFailure this.mapFoldBack_and_map_and_foldBack<int>
        Check.QuickThrowOnFailure this.mapFoldBack_and_map_and_foldBack<string>
        Check.QuickThrowOnFailure this.mapFoldBack_and_map_and_foldBack<NormalFloat>

    member _.findBack_and_exists<'a when 'a : comparison>  (xs : list<'a>) f =
        let a = 
            try
                List.findBack f xs |> ignore
                true
            with
            | _ -> false
        let b = List.exists f xs
        a = b

    [<Fact>]
    member this.``findBack and exists work similar`` () =   
        Check.QuickThrowOnFailure this.findBack_and_exists<int>
        Check.QuickThrowOnFailure this.findBack_and_exists<string>
        Check.QuickThrowOnFailure this.findBack_and_exists<NormalFloat>

    member _.findBack_and_find<'a when 'a : comparison>  (xs : list<'a>) predicate =
        let a = run (fun () -> xs |> List.findBack predicate)
        let b = run (fun () -> xs |> List.rev |> List.find predicate)
        a = b

    [<Fact>]
    member this.``findBack and find work in reverse`` () =   
        Check.QuickThrowOnFailure this.findBack_and_find<int>
        Check.QuickThrowOnFailure this.findBack_and_find<string>
        Check.QuickThrowOnFailure this.findBack_and_find<NormalFloat>

    member _.tryFindBack_and_tryFind<'a when 'a : comparison>  (xs : list<'a>) predicate =
        let a = xs |> List.tryFindBack predicate
        let b = xs |> List.rev |> List.tryFind predicate
        a = b

    [<Fact>]
    member this.``tryFindBack and tryFind work in reverse`` () =   
        Check.QuickThrowOnFailure this.tryFindBack_and_tryFind<int>
        Check.QuickThrowOnFailure this.tryFindBack_and_tryFind<string>
        Check.QuickThrowOnFailure this.tryFindBack_and_tryFind<NormalFloat>

    member _.tryFindIndexBack_and_tryFindIndex<'a when 'a : comparison>  (xs : list<'a>) predicate =
        let a = xs |> List.tryFindIndexBack predicate
        let b = xs |> List.rev |> List.tryFindIndex predicate
        match a,b with
        | Some a, Some b -> a = (xs.Length - b - 1)
        | _ -> a = b

    [<Fact>]
    member this.``tryFindIndexBack and tryIndexFind work in reverse`` () =   
        Check.QuickThrowOnFailure this.tryFindIndexBack_and_tryFindIndex<int>
        Check.QuickThrowOnFailure this.tryFindIndexBack_and_tryFindIndex<string>
        Check.QuickThrowOnFailure this.tryFindIndexBack_and_tryFindIndex<NormalFloat>

    member _.rev<'a when 'a : comparison>  (xs : list<'a>) =
        let list = System.Collections.Generic.List<_>()
        for x in xs do
            list.Insert(0,x)

        xs |> List.rev |> List.rev = xs && Seq.toList list = List.rev xs

    [<Fact>]
    member this.``rev reverses a list`` () =   
        Check.QuickThrowOnFailure this.rev<int>
        Check.QuickThrowOnFailure this.rev<string>
        Check.QuickThrowOnFailure this.rev<NormalFloat>

    member _.findIndexBack_and_findIndex<'a when 'a : comparison>  (xs : list<'a>) (F (_, predicate)) =
        let a = run (fun () -> xs |> List.findIndex predicate)
        let b = run (fun () -> xs |> List.rev |> List.findIndexBack predicate)
        match a,b with
        | Success a, Success b -> a = (xs.Length - b - 1)
        | _ -> a = b

    [<Fact>]
    member this.``findIndexBack and findIndex work in reverse`` () =
        Check.QuickThrowOnFailure this.findIndexBack_and_findIndex<int>
        Check.QuickThrowOnFailure this.findIndexBack_and_findIndex<string>
        Check.QuickThrowOnFailure this.findIndexBack_and_findIndex<NormalFloat>

    member _.skip_and_skipWhile<'a when 'a : comparison>  (xs : list<'a>) (count:NonNegativeInt) =
        let count = int count
        count <= xs.Length ==> (lazy 
            let ys = List.indexed xs
            let a = runAndCheckErrorType (fun () -> List.skip count ys)
            let b = runAndCheckErrorType (fun () -> List.skipWhile (fun (p,_) -> p < count) ys)

            a = b)

    [<Fact>]
    member this.``skip and skipWhile are consistent`` () =   
        Check.QuickThrowOnFailure this.skip_and_skipWhile<int>
        Check.QuickThrowOnFailure this.skip_and_skipWhile<string>
        Check.QuickThrowOnFailure this.skip_and_skipWhile<NormalFloat>

    member _.distinct_works_like_set<'a when 'a : comparison> (xs : 'a list) =
        let a = List.distinct xs
        let b = Set.ofList xs

        let mutable result = (a.Length = b.Count)
        for x in a do
            if Set.contains x b |> not then
                result <- false

        for x in b do
            if List.exists ((=) x) a |> not then
                result <- false
        result

    [<Fact>]
    member this.``distinct creates same elements like a set`` () =
        Check.QuickThrowOnFailure this.distinct_works_like_set<int>
        Check.QuickThrowOnFailure this.distinct_works_like_set<string>
        Check.QuickThrowOnFailure this.distinct_works_like_set<NormalFloat>

    member _.sort_and_sortDescending<'a when 'a : comparison>  (xs : list<'a>) =
        let a = run (fun () -> xs |> List.sort)
        let b = run (fun () -> xs |> List.sortDescending |> List.rev)
        a = b

    [<Fact>]
    member this.``sort and sortDescending work in reverse`` () =   
        Check.QuickThrowOnFailure this.sort_and_sortDescending<int>
        Check.QuickThrowOnFailure this.sort_and_sortDescending<string>
        Check.QuickThrowOnFailure this.sort_and_sortDescending<NormalFloat>

    member _.sortByStable<'a when 'a : comparison> (xs : 'a []) =
        let indexed = xs |> Seq.indexed |> Seq.toList
        let sorted = indexed |> List.sortBy snd
        isStable sorted
    
    [<Fact>]
    member this.``List.sortBy is stable`` () =
        Check.QuickThrowOnFailure this.sortByStable<int>
        Check.QuickThrowOnFailure this.sortByStable<string>

    member _.sortWithStable<'a when 'a : comparison> (xs : 'a []) =
        let indexed = xs |> Seq.indexed |> Seq.toList
        let sorted = indexed |> List.sortWith (fun x y -> compare (snd x) (snd y))
        isStable sorted
    
    [<Fact>]
    member this.``List.sortWithStable is stable`` () =
        Check.QuickThrowOnFailure this.sortWithStable<int>
        Check.QuickThrowOnFailure this.sortWithStable<string>

    member _.distinctByStable<'a when 'a : comparison> (xs : 'a []) =
        let indexed = xs |> Seq.indexed |> Seq.toList
        let sorted = indexed |> List.distinctBy snd
        isStable sorted
    
    [<Fact>]
    member this.``List.distinctBy is stable`` () =
        Check.QuickThrowOnFailure this.distinctByStable<int>
        Check.QuickThrowOnFailure this.distinctByStable<string>
    
    [<Fact>]
    member _.``List.sum calculates the sum`` () =
        let sum (xs : int list) =
            let s = List.sum xs
            let mutable r = 0
            for x in xs do r <- r + x    
            s = r
        Check.QuickThrowOnFailure sum

    member this.sumBy<'a> (xs : 'a list) (f:'a -> int) =
        let s = xs |> List.map f |> List.sum
        let r = List.sumBy f xs
        r = s

    [<Fact>]
    member this.``List.sumBy calculates the sum of the mapped list`` () =
        Check.QuickThrowOnFailure this.sumBy<int>
        Check.QuickThrowOnFailure this.sumBy<string> 
        Check.QuickThrowOnFailure this.sumBy<float>

    member _.allPairsCount<'a, 'b> (xs : 'a list) (ys : 'b list) =
        let pairs = List.allPairs xs ys
        pairs.Length = xs.Length * ys.Length

    [<Fact>]
    member this.``List.allPairs produces the correct number of pairs`` () =
        Check.QuickThrowOnFailure this.allPairsCount<int, int>
        Check.QuickThrowOnFailure this.allPairsCount<string, string>
        Check.QuickThrowOnFailure this.allPairsCount<float, float>

    member _.allPairsFst<'a, 'b when 'a : equality> (xs : 'a list) (ys : 'b list) =
        let pairsFst = List.allPairs xs ys |> List.map fst
        let check = xs |> List.collect (List.replicate ys.Length)
        pairsFst = check

    [<Fact(Skip="Test is flaky and bugged on .NET7, will be re-enabled when https://github.com/dotnet/fsharp/issues/13563 is fixed")>]
    member this.``List.allPairs first elements are correct`` () =
        Check.QuickThrowOnFailure this.allPairsFst<int, int>
        Check.QuickThrowOnFailure this.allPairsFst<string, string>
        Check.QuickThrowOnFailure this.allPairsFst<NormalFloat, NormalFloat>

    member _.allPairsSnd<'a, 'b when 'b : equality> (xs : 'a list) (ys : 'b list) =
        let pairsSnd = List.allPairs xs ys |> List.map snd
        let check = [ for i in 1 .. xs.Length do yield! ys ]
        pairsSnd = check

    [<Fact>]
    member this.``List.allPairs second elements are correct`` () =
        Check.QuickThrowOnFailure this.allPairsFst<int, int>
        Check.QuickThrowOnFailure this.allPairsFst<string, string>
        Check.QuickThrowOnFailure this.allPairsFst<NormalFloat, NormalFloat>
