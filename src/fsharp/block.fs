module Internal.Utilities.Library.Block

open System.Collections.Immutable

type block<'T> = ImmutableArray<'T>
type blockbuilder<'T> = ImmutableArray<'T>.Builder

[<RequireQualifiedAccess>]
module BlockBuilder =

    let create size : blockbuilder<'T> =
        if size = 0 then
            ImmutableArray.CreateBuilder()
        else
            ImmutableArray.CreateBuilder(size)

[<RequireQualifiedAccess>]
module Block =

    let empty<'T> = ImmutableArray<'T>.Empty

    let init n (f: int -> 'T) : block<_> =
        match n with
        | 0 -> ImmutableArray.Empty
        | 1 -> ImmutableArray.Create(f 0)
        | n ->
            if n < 0 then
                invalidArg "n" "Below zero."

            let builder = ImmutableArray.CreateBuilder(n)
            for i = 0 to n - 1 do
                builder.Add(f i)
            builder.ToImmutable()

    let iter f (arr: block<'T>) =
        for i = 0 to arr.Length - 1 do
            f arr.[i]

    let iteri f (arr: block<'T>) =
        for i = 0 to arr.Length - 1 do
            f i arr.[i]

    let iter2 f (arr1: block<'T1>) (arr2: block<'T2>) =
        if arr1.Length <> arr2.Length then
            invalidOp "Block lengths do not match."

        for i = 0 to arr1.Length - 1 do
            f arr1.[i] arr2.[i]

    let iteri2 f (arr1: block<'T1>) (arr2: block<'T2>) =
        if arr1.Length <> arr2.Length then
            invalidOp "Block lengths do not match."

        for i = 0 to arr1.Length - 1 do
            f i arr1.[i] arr2.[i]

    let map (mapper: 'T -> 'U) (arr: block<'T>) : block<_> =
        match arr.Length with
        | 0 -> block.Empty
        | 1 -> ImmutableArray.Create(mapper arr.[0])
        | _ ->
            let builder = ImmutableArray.CreateBuilder(arr.Length)
            for i = 0 to arr.Length - 1 do
                builder.Add(mapper arr.[i])
            builder.ToImmutable()

    let mapi (mapper: int -> 'T -> 'U) (arr: block<'T>) : block<_> =
        match arr.Length with
        | 0 -> block.Empty
        | 1 -> ImmutableArray.Create(mapper 0 arr.[0])
        | _ ->
            let builder = ImmutableArray.CreateBuilder(arr.Length)
            for i = 0 to arr.Length - 1 do
                builder.Add(mapper i arr.[i])
            builder.ToImmutable()

    let map2 (mapper: 'T1 -> 'T2 -> 'T) (arr1: block<'T1>) (arr2: block<'T2>) : block<_> =
        if arr1.Length <> arr2.Length then
            invalidOp "Block lengths do not match."
      
        match arr1.Length with
        | 0 -> ImmutableArray.Empty
        | 1 -> ImmutableArray.Create(mapper arr1.[0] arr2.[0])
        | n ->
            let builder = ImmutableArray.CreateBuilder(n)
            for i = 0 to n - 1 do
                builder.Add(mapper arr1.[i] arr2.[i])
            builder.ToImmutable()

    let mapi2 (mapper: int -> 'T1 -> 'T2 -> 'T) (arr1: block<'T1>) (arr2: block<'T2>) : block<_> =
        if arr1.Length <> arr2.Length then
            invalidOp "Block lengths do not match."
      
        match arr1.Length with
        | 0 -> ImmutableArray.Empty
        | 1 -> ImmutableArray.Create(mapper 0 arr1.[0] arr2.[0])
        | n ->
            let builder = ImmutableArray.CreateBuilder(n)
            for i = 0 to n - 1 do
                builder.Add(mapper i arr1.[i] arr2.[i])
            builder.ToImmutable()

    let concat (arr: block<block<'T>>) : block<'T> =
        if arr.IsEmpty then
            empty
        elif arr.Length = 1 then
            arr.[0]
        elif arr.Length = 2 then
            arr.[0].AddRange(arr.[1])
        else
            let mutable arr2 = arr.[0]
            for i = 1 to arr.Length - 1 do
                arr2 <- arr2.AddRange(arr.[i])
            arr2

    let forall predicate (arr: block<'T>) =
        match arr.Length with
        | 0 -> true
        | 1 -> predicate arr.[0]
        | n ->
            let mutable result = true
            let mutable i = 0
            while result && i < n do
                result <- predicate arr.[i]
                i <- i + 1
            result

    let forall2 predicate (arr1: block<'T1>) (arr2: block<'T2>) =
        if arr1.Length <> arr2.Length then
            invalidOp "Block lengths do not match."

        match arr1.Length with
        | 0 -> true
        | 1 -> predicate arr1.[0] arr2.[0]
        | n ->
            let mutable result = true
            let mutable i = 0
            while result && i < n do
                result <- predicate arr1.[i] arr2.[i]
                i <- i + 1
            result

    let tryFind predicate (arr: block<'T>) =
        match arr.Length with
        | 0 -> None
        | 1 -> if predicate arr.[0] then Some arr.[0] else None
        | n ->
            let mutable result = None
            let mutable i = 0
            while result.IsNone && i < n do
                if predicate arr.[i] then
                    result <- Some arr.[i]
                i <- i + 1
            result

    let tryFindIndex predicate (arr: block<'T>) =
        match arr.Length with
        | 0 -> None
        | 1 -> if predicate arr.[0] then Some 0 else None
        | n ->
            let mutable result = None
            let mutable i = 0
            while result.IsNone && i < n do
                if predicate arr.[i] then
                    result <- Some i
                i <- i + 1
            result

    let tryPick chooser (arr: block<'T>) =
        match arr.Length with
        | 0 -> None
        | 1 -> chooser arr.[0]
        | n ->
            let mutable result = None
            let mutable i = 0
            while result.IsNone && i < n do
                result <- chooser arr.[i]
                i <- i + 1
            result

    let ofSeq (xs: 'T seq) =
        ImmutableArray.CreateRange(xs)

    let append (arr1: block<'T1>) (arr2: block<'T1>) : block<_> =
        arr1.AddRange(arr2)

    let createOne (item: 'T) : block<_> =
        ImmutableArray.Create(item)

    let filter predicate (arr: block<'T>) : block<'T> =
        let builder = ImmutableArray.CreateBuilder(arr.Length)
        for i = 0 to arr.Length - 1 do
            if predicate arr.[i] then
                builder.Add(arr.[i])
        builder.ToImmutable()

    let exists predicate (arr: block<'T>) =
        let n = arr.Length
        let mutable result = false
        let mutable i = 0
        while not result && i < n do
            if predicate arr.[i] then
                result <- true
            i <- i + 1
        result

    let choose (chooser: 'T -> 'U option) (arr: block<'T>) : block<'U> =
        let builder = ImmutableArray.CreateBuilder(arr.Length)
        for i = 0 to arr.Length - 1 do
            let result = chooser arr.[i]
            if result.IsSome then
                builder.Add(result.Value)
        builder.ToImmutable()

    let isEmpty (arr: block<_>) = arr.Length = 0

    let fold folder state (arr: block<_>) =
        let f = OptimizedClosures.FSharpFunc<_, _, _>.Adapt(folder)
        let mutable state = state
        for i = 0 to arr.Length - 1 do 
            state <- f.Invoke(state, arr.[i])
        state
