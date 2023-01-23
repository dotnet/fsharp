// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

// Taken from Utilities/illib.fs


open System
open System.Collections.Generic
open System.Collections.Concurrent
open System.Diagnostics
open System.IO
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices


[<AutoOpen>]
module internal PervasiveAutoOpens =
    /// Logical shift right treating int32 as unsigned integer.
    /// Code that uses this should probably be adjusted to use unsigned integer types.
    let (>>>&) (x: int32) (n: int32) = int32 (uint32 x >>> n)

    let notlazy v = Lazy<_>.CreateFromValue v

    let inline isNil l = List.isEmpty l

    /// Returns true if the list has less than 2 elements. Otherwise false.
    let inline isNilOrSingleton l =
        match l with
        | []
        | [ _ ] -> true
        | _ -> false

    /// Returns true if the list contains exactly 1 element. Otherwise false.
    let inline isSingleton l =
        match l with
        | [ _ ] -> true
        | _ -> false

    type 'T MaybeNull when 'T: null and 'T: not struct = 'T

    let inline isNotNull (x: 'T) = not (isNull x)

    let inline (|NonNullQuick|) (x: 'T MaybeNull) =
        match x with
        | null -> raise (NullReferenceException())
        | v -> v

    let inline nonNull (x: 'T MaybeNull) =
        match x with
        | null -> raise (NullReferenceException())
        | v -> v

    let inline (|Null|NonNull|) (x: 'T MaybeNull) : Choice<unit, 'T> =
        match x with
        | null -> Null
        | v -> NonNull v

    let inline nullArgCheck paramName (x: 'T MaybeNull) =
        match x with
        | null -> raise (ArgumentNullException(paramName))
        | v -> v

    let inline (===) x y = LanguagePrimitives.PhysicalEquality x y

    /// Per the docs the threshold for the Large Object Heap is 85000 bytes: https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/large-object-heap#how-an-object-ends-up-on-the-large-object-heap-and-how-gc-handles-them
    /// We set the limit to be 80k to account for larger pointer sizes for when F# is running 64-bit.
    let LOH_SIZE_THRESHOLD_BYTES = 80_000

    type String with

        member inline x.StartsWithOrdinal value =
            x.StartsWith(value, StringComparison.Ordinal)

        member inline x.EndsWithOrdinal value =
            x.EndsWith(value, StringComparison.Ordinal)

        member inline x.EndsWithOrdinalIgnoreCase value =
            x.EndsWith(value, StringComparison.OrdinalIgnoreCase)

    /// Get an initialization hole
    let getHole (r: _ ref) =
        match r.Value with
        | None -> failwith "getHole"
        | Some x -> x

    let reportTime =
        let mutable tFirst = None
        let mutable tPrev = None

        fun showTimes descr ->
            if showTimes then
                let t = Process.GetCurrentProcess().UserProcessorTime.TotalSeconds

                let prev =
                    match tPrev with
                    | None -> 0.0
                    | Some t -> t

                let first =
                    match tFirst with
                    | None ->
                        (tFirst <- Some t
                         t)
                    | Some t -> t

                printf "  ilwrite: Cpu %4.1f (total)   %4.1f (delta) - %s\n" (t - first) (t - prev) descr
                tPrev <- Some t

    let foldOn p f z x = f z (p x)

    let notFound () = raise (KeyNotFoundException())

    type Async with

        static member RunImmediate(computation: Async<'T>, ?cancellationToken) =
            let cancellationToken = defaultArg cancellationToken Async.DefaultCancellationToken
            let ts = TaskCompletionSource<'T>()
            let task = ts.Task

            Async.StartWithContinuations(
                computation,
                (fun k -> ts.SetResult k),
                (fun exn -> ts.SetException exn),
                (fun _ -> ts.SetCanceled()),
                cancellationToken
            )

            task.Result

/// An efficient lazy for inline storage in a class type. Results in fewer thunks.
[<Struct>]
type InlineDelayInit<'T when 'T: not struct> =
    new(f: unit -> 'T) =
        {
            store = Unchecked.defaultof<'T>
            func = Func<_>(f)
        }

    val mutable store: 'T
    val mutable func: Func<'T>

    member x.Value =
        match x.func with
        | null -> x.store
        | _ ->
            let res = LazyInitializer.EnsureInitialized(&x.store, x.func)
            x.func <- Unchecked.defaultof<_>
            res

//-------------------------------------------------------------------------
// Library: projections
//------------------------------------------------------------------------

module Order =
    let orderBy (p: 'T -> 'U) =
        { new IComparer<'T> with
            member _.Compare(x, xx) = compare (p x) (p xx)
        }

    let orderOn p (pxOrder: IComparer<'U>) =
        { new IComparer<'T> with
            member _.Compare(x, xx) = pxOrder.Compare(p x, p xx)
        }

    let toFunction (pxOrder: IComparer<'U>) x y = pxOrder.Compare(x, y)

//-------------------------------------------------------------------------
// Library: arrays, lists, options, resizearrays
//-------------------------------------------------------------------------

module Array =

    let mapq f inp =
        match inp with
        | [||] -> inp
        | _ ->
            let res = Array.map f inp
            let len = inp.Length
            let mutable eq = true
            let mutable i = 0

            while eq && i < len do
                if not (inp[i] === res[i]) then
                    eq <- false

                i <- i + 1

            if eq then inp else res

    let lengthsEqAndForall2 p l1 l2 =
        Array.length l1 = Array.length l2 && Array.forall2 p l1 l2

    let order (eltOrder: IComparer<'T>) =
        { new IComparer<array<'T>> with
            member _.Compare(xs, ys) =
                let c = compare xs.Length ys.Length

                if c <> 0 then
                    c
                else
                    let rec loop i =
                        if i >= xs.Length then
                            0
                        else
                            let c = eltOrder.Compare(xs[i], ys[i])
                            if c <> 0 then c else loop (i + 1)

                    loop 0
        }

    let existsOne p l =
        let rec forallFrom p l n =
            (n >= Array.length l) || (p l[n] && forallFrom p l (n + 1))

        let rec loop p l n =
            (n < Array.length l)
            && (if p l[n] then
                    forallFrom (fun x -> not (p x)) l (n + 1)
                else
                    loop p l (n + 1))

        loop p l 0

    let existsTrue (arr: bool[]) =
        let rec loop n =
            (n < arr.Length) && (arr[n] || loop (n + 1))

        loop 0

    let findFirstIndexWhereTrue (arr: _[]) p =
        let rec look lo hi =
            assert ((lo >= 0) && (hi >= 0))
            assert ((lo <= arr.Length) && (hi <= arr.Length))

            if lo = hi then
                lo
            else
                let i = (lo + hi) / 2

                if p arr[i] then
                    if i = 0 then i
                    else if p arr[i - 1] then look lo i
                    else i
                else
                    // not true here, look after
                    look (i + 1) hi

        look 0 arr.Length

    /// pass an array byref to reverse it in place
    let revInPlace (array: 'T[]) =
        if Array.isEmpty array then
            ()
        else
            let arrLen, revLen = array.Length - 1, array.Length / 2 - 1

            for idx in 0..revLen do
                let t1 = array[idx]
                let t2 = array[arrLen - idx]
                array[idx] <- t2
                array[arrLen - idx] <- t1

    /// Async implementation of Array.map.
    let mapAsync (mapping: 'T -> Async<'U>) (array: 'T[]) : Async<'U[]> =
        let len = Array.length array
        let result = Array.zeroCreate len

        async { // Apply the mapping function to each array element.
            for i in 0 .. len - 1 do
                let! mappedValue = mapping array[i]
                result[i] <- mappedValue

            // Return the completed results.
            return result
        }

    /// Returns a new array with an element replaced with a given value.
    let replace index value (array: _[]) =
        if index >= array.Length then
            raise (IndexOutOfRangeException "index")

        let res = Array.copy array
        res[index] <- value
        res

    /// Optimized arrays equality. ~100x faster than `array1 = array2` on strings.
    /// ~2x faster for floats
    /// ~0.8x slower for ints
    let inline areEqual (xs: 'T[]) (ys: 'T[]) =
        match xs, ys with
        | null, null -> true
        | [||], [||] -> true
        | null, _
        | _, null -> false
        | _ when xs.Length <> ys.Length -> false
        | _ ->
            let mutable break' = false
            let mutable i = 0
            let mutable result = true

            while i < xs.Length && not break' do
                if xs[i] <> ys[i] then
                    break' <- true
                    result <- false

                i <- i + 1

            result

    /// Returns all heads of a given array.
    /// For [|1;2;3|] it returns [|[|1; 2; 3|]; [|1; 2|]; [|1|]|]
    let heads (array: 'T[]) =
        let res = Array.zeroCreate<'T[]> array.Length

        for i = array.Length - 1 downto 0 do
            res[i] <- array[0..i]

        res

    /// check if subArray is found in the wholeArray starting
    /// at the provided index
    let inline isSubArray (subArray: 'T[]) (wholeArray: 'T[]) index =
        if subArray.Length = 0 then
            true
        elif subArray.Length > wholeArray.Length then
            false
        elif subArray.Length = wholeArray.Length then
            areEqual subArray wholeArray
        else
            let rec loop subidx idx =
                if subidx = subArray.Length then
                    true
                elif subArray[subidx] = wholeArray[idx] then
                    loop (subidx + 1) (idx + 1)
                else
                    false

            loop 0 index

    /// Returns true if one array has another as its subset from index 0.
    let startsWith (prefix: _[]) (whole: _[]) = isSubArray prefix whole 0

    /// Returns true if one array has trailing elements equal to another's.
    let endsWith (suffix: _[]) (whole: _[]) =
        isSubArray suffix whole (whole.Length - suffix.Length)

module Option =

    let mapFold f s opt =
        match opt with
        | None -> None, s
        | Some x ->
            let x2, s2 = f s x
            Some x2, s2

    let attempt (f: unit -> 'T) =
        try
            Some(f ())
        with _ ->
            None

module List =

    let sortWithOrder (c: IComparer<'T>) elements =
        List.sortWith (Order.toFunction c) elements

    let splitAfter n l =
        let rec split_after_acc n l1 l2 =
            if n <= 0 then
                List.rev l1, l2
            else
                split_after_acc (n - 1) ((List.head l2) :: l1) (List.tail l2)

        split_after_acc n [] l

    let existsi f xs =
        let rec loop i xs =
            match xs with
            | [] -> false
            | h :: t -> f i h || loop (i + 1) t

        loop 0 xs

    let lengthsEqAndForall2 p l1 l2 =
        List.length l1 = List.length l2 && List.forall2 p l1 l2

    let rec findi n f l =
        match l with
        | [] -> None
        | h :: t -> if f h then Some(h, n) else findi (n + 1) f t

    let splitChoose select l =
        let rec ch acc1 acc2 l =
            match l with
            | [] -> List.rev acc1, List.rev acc2
            | x :: xs ->
                match select x with
                | Choice1Of2 sx -> ch (sx :: acc1) acc2 xs
                | Choice2Of2 sx -> ch acc1 (sx :: acc2) xs

        ch [] [] l

    let rec checkq l1 l2 =
        match l1, l2 with
        | h1 :: t1, h2 :: t2 -> h1 === h2 && checkq t1 t2
        | _ -> true

    let mapq (f: 'T -> 'T) inp =
        assert not typeof<'T>.IsValueType

        match inp with
        | [] -> inp
        | [ h1a ] ->
            let h2a = f h1a
            if h1a === h2a then inp else [ h2a ]
        | [ h1a; h1b ] ->
            let h2a = f h1a
            let h2b = f h1b

            if h1a === h2a && h1b === h2b then inp else [ h2a; h2b ]
        | [ h1a; h1b; h1c ] ->
            let h2a = f h1a
            let h2b = f h1b
            let h2c = f h1c

            if h1a === h2a && h1b === h2b && h1c === h2c then
                inp
            else
                [ h2a; h2b; h2c ]
        | _ ->
            let res = List.map f inp
            if checkq inp res then inp else res

    let frontAndBack l =
        let rec loop acc l =
            match l with
            | [] ->
                Debug.Assert(false, "empty list")
                invalidArg "l" "empty list"
            | [ h ] -> List.rev acc, h
            | h :: t -> loop (h :: acc) t

        loop [] l

    let tryFrontAndBack l =
        match l with
        | [] -> None
        | _ -> Some(frontAndBack l)

    let tryRemove f inp =
        let rec loop acc l =
            match l with
            | [] -> None
            | h :: t -> if f h then Some(h, List.rev acc @ t) else loop (h :: acc) t

        loop [] inp

    let zip4 l1 l2 l3 l4 =
        List.zip l1 (List.zip3 l2 l3 l4)
        |> List.map (fun (x1, (x2, x3, x4)) -> (x1, x2, x3, x4))

    let unzip4 l =
        let a, b, cd = List.unzip3 (List.map (fun (x, y, z, w) -> (x, y, (z, w))) l)
        let c, d = List.unzip cd
        a, b, c, d

    let rec iter3 f l1 l2 l3 =
        match l1, l2, l3 with
        | h1 :: t1, h2 :: t2, h3 :: t3 ->
            f h1 h2 h3
            iter3 f t1 t2 t3
        | [], [], [] -> ()
        | _ -> failwith "iter3"

    let takeUntil p l =
        let rec loop acc l =
            match l with
            | [] -> List.rev acc, []
            | x :: xs -> if p x then List.rev acc, l else loop (x :: acc) xs

        loop [] l

    let order (eltOrder: IComparer<'T>) =
        { new IComparer<'T list> with
            member _.Compare(xs, ys) =
                let rec loop xs ys =
                    match xs, ys with
                    | [], [] -> 0
                    | [], _ -> -1
                    | _, [] -> 1
                    | x :: xs, y :: ys ->
                        let cxy = eltOrder.Compare(x, y)
                        if cxy = 0 then loop xs ys else cxy

                loop xs ys
        }

    let indexNotFound () =
        raise (KeyNotFoundException("An index satisfying the predicate was not found in the collection"))

    let rec assoc x l =
        match l with
        | [] -> indexNotFound ()
        | (h, r) :: t -> if x = h then r else assoc x t

    let rec memAssoc x l =
        match l with
        | [] -> false
        | (h, _) :: t -> x = h || memAssoc x t

    let rec memq x l =
        match l with
        | [] -> false
        | h :: t -> LanguagePrimitives.PhysicalEquality x h || memq x t

    let mapNth n f xs =
        let rec mn i =
            function
            | [] -> []
            | x :: xs -> if i = n then f x :: xs else x :: mn (i + 1) xs

        mn 0 xs

    let count pred xs =
        List.fold (fun n x -> if pred x then n + 1 else n) 0 xs

    let headAndTail l =
        match l with
        | [] -> failwith "headAndTail"
        | h :: t -> (h, t)

    // WARNING: not tail-recursive
    let mapHeadTail fhead ftail =
        function
        | [] -> []
        | [ x ] -> [ fhead x ]
        | x :: xs -> fhead x :: List.map ftail xs

    let collectFold f s l =
        let l, s = List.mapFold f s l
        List.concat l, s

    let collect2 f xs ys = List.concat (List.map2 f xs ys)

    let toArraySquared xss =
        xss |> List.map List.toArray |> List.toArray

    let iterSquared f xss = xss |> List.iter (List.iter f)

    let collectSquared f xss = xss |> List.collect (List.collect f)

    let mapSquared f xss = xss |> List.map (List.map f)

    let mapFoldSquared f z xss = List.mapFold (List.mapFold f) z xss

    let forallSquared f xss = xss |> List.forall (List.forall f)

    let mapiSquared f xss =
        xss |> List.mapi (fun i xs -> xs |> List.mapi (fun j x -> f i j x))

    let existsSquared f xss =
        xss |> List.exists (fun xs -> xs |> List.exists (fun x -> f x))

    let mapiFoldSquared f z xss =
        mapFoldSquared f z (xss |> mapiSquared (fun i j x -> (i, j, x)))

    let duplicates (xs: 'T list) =
        xs
        |> List.groupBy id
        |> List.filter (fun (_, elems) -> Seq.length elems > 1)
        |> List.map fst

    let internal allEqual (xs: 'T list) =
        match xs with
        | [] -> true
        | h :: t -> t |> List.forall (fun h2 -> h = h2)

    let isSingleton xs =
        match xs with
        | [ _ ] -> true
        | _ -> false
