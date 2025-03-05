// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Collections

open System
open System.Collections.Generic
open System.Collections.Immutable

open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module ImmutableArray =

    [<CompiledName("Length")>]
    let inline length (array: ImmutableArray<'T>) : int =
        array.Length

    [<CompiledName("Singleton")>]
    let inline singleton (item: 'T) : ImmutableArray<_> =
        ImmutableArray.Create(item)

    [<CompiledName("Initialize")>]
    let inline init n ([<InlineIfLambda>] f: int -> 'T) : ImmutableArray<_> =
        match n with
        | 0 -> ImmutableArray.Empty
        | 1 -> ImmutableArray.Create(f 0)
        | n ->
            if n < 0 then
                invalidArg "n" "Below zero."

            let builder = ImmutableArray.CreateBuilder(n)
            for i = 0 to n - 1 do
                builder.Add(f i)
            builder.MoveToImmutable()

    [<CompiledName("ZeroCreate")>]
    let inline zeroCreate (count: int) : ImmutableArray<'T> =
        if count < 0 then
            invalidArgInputMustBeNonNegative (nameof(count)) count

        init count (fun _ -> LanguagePrimitives.GenericZero)

    [<CompiledName("Create")>]
    let inline create (count: int) (value: 'T) : ImmutableArray<'T> =
        if count < 0 then
            invalidArgInputMustBeNonNegative (nameof(count)) count

        init count (fun _ -> value)

    [<CompiledName("IsEmpty")>]
    let inline isEmpty (arr: ImmutableArray<_>) = arr.IsEmpty

    [<CompiledName("Empty")>]
    [<GeneralizableValue>]
    let empty<'T> = ImmutableArray<'T>.Empty

    let inline concatImmutableArrays (arrs: ImmutableArray<ImmutableArray<'T>>) : ImmutableArray<'T> =
        match arrs.Length with
        | 0 -> ImmutableArray.Empty
        | 1 -> arrs[0]
        | 2 -> arrs[0].AddRange(arrs[1])
        | n ->
            let mutable acc = 0

            for h in arrs do
                acc <- acc + h.Length

            let builder = ImmutableArray.CreateBuilder(acc)

            for i = 0 to n - 1 do
                builder.AddRange(arrs[i])

            builder.MoveToImmutable()

    [<CompiledName("Concat")>]
    let inline concat (arrs: IEnumerable<ImmutableArray<'T>>) : ImmutableArray<'T> =
        match arrs with
        | :? ImmutableArray<ImmutableArray<'T>> as arrs -> concatImmutableArrays arrs
        | arrs -> concatImmutableArrays (ImmutableArray.CreateRange(arrs))

    [<CompiledName("Replicate")>]
    let inline replicate (count: int) (value: 'T) : ImmutableArray<'T> =
        create count value

    [<CompiledName("Map")>]
    let inline map ([<InlineIfLambda>] mapper: 'T -> 'U) (arr: ImmutableArray<'T>) : ImmutableArray<_> =
        match arr.Length with
        | 0 -> ImmutableArray.Empty
        | 1 -> ImmutableArray.Create(mapper arr[0])
        | _ ->
            let builder = ImmutableArray.CreateBuilder(arr.Length)
            for i = 0 to arr.Length - 1 do
                builder.Add(mapper arr[i])
            builder.MoveToImmutable()

    [<CompiledName("MapIndexed")>]
    let inline mapi ([<InlineIfLambda>] mapper: int -> 'T -> 'U) (arr: ImmutableArray<'T>) : ImmutableArray<_> =
        match arr.Length with
        | 0 -> ImmutableArray.Empty
        | 1 -> ImmutableArray.Create(mapper 0 arr[0])
        | n ->
            let builder = ImmutableArray.CreateBuilder(n)
            for i = 0 to arr.Length - 1 do
                builder.Add(mapper i arr[i])
            builder.MoveToImmutable()

    [<CompiledName("Map2")>]
    let map2 (mapper: 'T1 -> 'T2 -> 'T) (array1: ImmutableArray<'T1>) (array2: ImmutableArray<'T2>) : ImmutableArray<_> =
        if array1.Length <> array2.Length then
            invalidArgDifferentArrayLength (nameof(array1)) array1.Length (nameof(array2)) array2.Length

        match array1.Length with
        | 0 -> ImmutableArray.Empty
        | 1 -> ImmutableArray.Create(mapper array1[0] array2[0])
        | n ->
            let builder = ImmutableArray.CreateBuilder(n)
            for i = 0 to n - 1 do
                builder.Add(mapper array1[i] array2[i])
            builder.MoveToImmutable()

    [<CompiledName("MapIndexed2")>]
    let mapi2 (mapper: int -> 'T1 -> 'T2 -> 'T) (array1: ImmutableArray<'T1>) (array2: ImmutableArray<'T2>) : ImmutableArray<_> =
        if array1.Length <> array2.Length then
            invalidArgDifferentArrayLength (nameof(array1)) array1.Length (nameof(array2)) array2.Length

        match array1.Length with
        | 0 -> ImmutableArray.Empty
        | 1 -> ImmutableArray.Create(mapper 0 array1[0] array2[0])
        | n ->
            let builder = ImmutableArray.CreateBuilder(n)
            for i = 0 to n - 1 do
                builder.Add(mapper i array1[i] array2[i])
            builder.MoveToImmutable()

    [<CompiledName("Collect")>]
    let collect (mapper: 'T -> ImmutableArray<'U>) (array: ImmutableArray<'T>) : ImmutableArray<'U> =
        let result = map mapper array
        concatImmutableArrays result

    [<CompiledName("Take")>]
    let take count (array: ImmutableArray<'T>) : ImmutableArray<'T> =
        if count < 0 then
            invalidArgInputMustBeNonNegative (nameof(count)) count
        elif count = 0 || array.Length = 0 then
            empty
        elif count > array.Length then
            raise (InvalidOperationException(SR.GetString(SR.notEnoughElements)))
        else
            let builder = ImmutableArray.CreateBuilder(count)
            builder.AddRange(array, count)
            builder.MoveToImmutable();

    [<CompiledName("TakeWhile")>]
    let inline takeWhile ([<InlineIfLambda>] predicate) (array: ImmutableArray<'T>) : ImmutableArray<'T> =
        let len = array.Length
        if len = 0 then
            empty
        else
            let mutable count = 0

            let builder = ImmutableArray.CreateBuilder(len)

            while count < len && predicate array[count] do
                count <- count + 1
                builder.Add(array[count])

            builder.Capacity <- count
            builder.Count <- count
            builder.MoveToImmutable()

    let inline countByImpl
        (comparer: IEqualityComparer<'SafeKey>)
        ([<InlineIfLambda>] projection: 'T -> 'SafeKey)
        ([<InlineIfLambda>] getKey: 'SafeKey -> 'Key)
        (array: ImmutableArray<'T>)
        =

        let length = array.Length

        if length = 0 then
            Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked 0
        else

            let dict = Dictionary comparer

            // Build the groupings
            for v in array do
                let safeKey = projection v
                let mutable prev = Unchecked.defaultof<_>

                if dict.TryGetValue(safeKey, &prev) then
                    dict[safeKey] <- prev + 1
                else
                    dict[safeKey] <- 1

            let res = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked dict.Count
            let mutable i = 0

            for group in dict do
                res[i] <- getKey group.Key, group.Value
                i <- i + 1

            res

    // We avoid wrapping a StructBox, because under 64 JIT we get some "hard" tailcalls which affect performance
    let inline countByValueType ([<InlineIfLambda>] projection: 'T -> 'Key) (array: ImmutableArray<'T>) =
        countByImpl HashIdentity.Structural<'Key> projection id array

    // Wrap a StructBox around all keys in case the key type is itself a type using null as a representation
    let countByRefType (projection: 'T -> 'Key) (array: ImmutableArray<'T>) =
        countByImpl
            RuntimeHelpers.StructBox<'Key>.Comparer
            (fun t -> RuntimeHelpers.StructBox(projection t))
            (fun sb -> sb.Value)
            array

    [<CompiledName("CountBy")>]
    let countBy (projection: 'T -> 'Key) (array: ImmutableArray<'T>) =
        // We return regular array here, since doesn't make much sense to return an immutable array of tuples.
        if typeof<'Key>.IsValueType then
            countByValueType projection array
        else
            countByRefType projection array

    [<CompiledName("Append")>]
    let inline append (array1: ImmutableArray<'T1>) (array2: ImmutableArray<'T1>) : ImmutableArray<_> =
        array1.AddRange(array2)

    [<CompiledName("Iterate")>]
    let inline iter ([<InlineIfLambda>] f) (arr: ImmutableArray<'T>) =
        for i = 0 to arr.Length - 1 do
            f arr[i]

    [<CompiledName("IterateIndexed")>]
    let inline iteri ([<InlineIfLambda>] f) (arr: ImmutableArray<'T>) =
        for i = 0 to arr.Length - 1 do
            f i arr[i]

    [<CompiledName("Iterate2")>]
    let iter2 f (array1: ImmutableArray<'T1>) (array2: ImmutableArray<'T2>) =
        if array1.Length <> array2.Length then
            invalidArgDifferentArrayLength (nameof(array1)) array1.Length (nameof(array2)) array2.Length

        for i = 0 to array1.Length - 1 do
            f array1[i] array2[i]

    [<CompiledName("IterateIndexed2")>]
    let iteri2  f (array1: ImmutableArray<'T1>) (array2: ImmutableArray<'T2>) =
        if array1.Length <> array2.Length then
            invalidArgDifferentArrayLength (nameof(array1)) array1.Length (nameof(array2)) array2.Length

        for i = 0 to array1.Length - 1 do
            f i array1[i] array2[i]


    [<CompiledName("ForAll")>]
    let inline forall ([<InlineIfLambda>] predicate) (arr: ImmutableArray<'T>) : bool =
        let len = arr.Length
        let rec loop i =
            i >= len || (predicate arr[i] && loop (i+1))
        loop 0

    [<CompiledName("ForAll2")>]
    let forall2 predicate (array1: ImmutableArray<'T1>) (array2: ImmutableArray<'T2>) : bool =
        if array1.Length <> array2.Length then
            invalidArgDifferentArrayLength (nameof(array1)) array1.Length (nameof(array2)) array2.Length

        let f = OptimizedClosures.FSharpFunc<_, _, _>.Adapt(predicate)

        let len1 = array1.Length
        let rec loop i =
            i >= len1 || (f.Invoke(array1[i], array2[i]) && loop (i+1))
        loop 0

    [<CompiledName("TryFind")>]
    let inline tryFind ([<InlineIfLambda>] predicate) (arr: ImmutableArray<'T>) =
        let len = arr.Length

        let rec loop i =
            if i >= len then
                None
            elif predicate arr[i] then
                Some arr[i]
            else
                loop (i+1)

        if len = 0 then
            None
        else
            loop 0

    [<CompiledName("TryFindIndex")>]
    let inline tryFindIndex ([<InlineIfLambda>] predicate) (arr: ImmutableArray<'T>) =
        let len = arr.Length

        let rec go n =
            if n >= len then
                None
            elif predicate arr[n] then
                Some n
            else go (n+1)

        if len = 0 then
            None
        else
            go 0

    [<CompiledName("TryPick")>]
    let inline tryPick ([<InlineIfLambda>] chooser) (arr: ImmutableArray<'T>) =
        let len = arr.Length
        let rec loop i =
            if i >= len then
                None
            else
                match chooser arr[i] with
                | None -> loop(i+1)
                | res -> res

        if len = 0 then
            None
        else
            loop 0

    [<CompiledName("Head")>]
    let inline head (array: ImmutableArray<'T>) : 'T =
        if array.Length = 0 then
            invalidArg (nameof(array)) LanguagePrimitives.ErrorStrings.InputArrayEmptyString
        else
            array[0]

    [<CompiledName("TryHead")>]
    let inline tryHead (array: ImmutableArray<'T>) : 'T option =
        if array.Length = 0 then
            None
        else
            Some array[0]

    [<CompiledName("Tail")>]
    let tail (array: ImmutableArray<'T>) : ImmutableArray<'T> =
        // Since we target ns2.0 & ns2.1, we cannot use Slice method here.
        let len = array.Length

        if len = 0 then
            invalidArg (nameof(array)) (SR.GetString(SR.notEnoughElements))
        elif len = 1 then
            singleton array[0]
        else
            let builder = ImmutableArray.CreateBuilder(len - 1)

            for i = 1 to len - 1 do
                builder.Add(array[i])

            builder.MoveToImmutable()

    [<CompiledName("Last")>]
    let inline last (array: ImmutableArray<'T>) : 'T =
        if array.Length = 0 then
            invalidArg (nameof(array)) LanguagePrimitives.ErrorStrings.InputArrayEmptyString

        array[array.Length - 1]

    [<CompiledName("TryLast")>]
    let inline tryLast (array: ImmutableArray<'T>) : 'T option =
        if array.Length = 0 then
            None
        else
            Some array[array.Length - 1]

    [<CompiledName("SplitAt")>]
    let splitAt index (array: ImmutableArray<'T>) =

        if index < 0 then
            invalidArgInputMustBeNonNegative "index" index

        let len = array.Length

        if len < index then
            raise <| InvalidOperationException(SR.GetString(SR.notEnoughElements))

        if index = 0 then
            empty, array
        elif index = len then
            array, empty
        else
            let builderLeft  = ImmutableArray.CreateBuilder(index)
            let builderRight = ImmutableArray.CreateBuilder(len - index)

            for i = 0 to len do
                if i < index then
                    builderLeft.Add(array[i])
                else
                    builderRight.Add(array[i])

            builderLeft.MoveToImmutable(), builderRight.MoveToImmutable()

    [<CompiledName("ToArray")>]
    let inline toArray (array: ImmutableArray<'T>) : 'T array =
        let len = array.Length
        let res = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked len
        for i = 0 to len - 1 do
            res[i] <- array[i]
        res

    [<CompiledName("OfArray")>]
    let inline ofArray (list: 'T array) : ImmutableArray<'T> =
        ImmutableArray.CreateRange list

    [<CompiledName("OfSeq")>]
    let inline ofSeq (xs: IEnumerable<'T>) =
        ImmutableArray.CreateRange(xs)

    [<CompiledName("ToList")>]
    let inline toList (array: ImmutableArray<'T>) : 'T list =
        List.ofSeq array

    [<CompiledName("OfList")>]
    let inline ofList (list: 'T list) : ImmutableArray<'T> =
        ImmutableArray.CreateRange list

    [<CompiledName("Filter")>]
    let inline filter ([<InlineIfLambda>] predicate) (arr: ImmutableArray<'T>) : ImmutableArray<'T> =
        let len = arr.Length
        let builder = ImmutableArray.CreateBuilder(len)
        for i = 0 to len - 1 do
            if predicate arr[i] then
                builder.Add(arr[i])
        builder.Capacity <- builder.Count
        builder.MoveToImmutable()

    [<CompiledName("Exists")>]
    let inline exists ([<InlineIfLambda>] predicate) (arr: ImmutableArray<'T>) : bool =
        let len = arr.Length
        let rec loop i =
            i < len && (predicate arr[i] || loop (i+1))
        len > 0 && loop 0

    [<CompiledName("Choose")>]
    let inline choose ([<InlineIfLambda>] chooser: 'T -> 'U option) (arr: ImmutableArray<'T>) : ImmutableArray<'U> =
        let len = arr.Length
        let builder = ImmutableArray.CreateBuilder(len)
        for i = 0 to len - 1 do
            let result = chooser arr[i]
            if result.IsSome then
                builder.Add(result.Value)
        builder.Capacity <- builder.Count
        builder.MoveToImmutable()

    [<CompiledName("Distinct")>]
    let inline distinct (array: ImmutableArray<'T>) : ImmutableArray<'T> =
        let temp = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked array.Length
        let mutable i = 0

        let hashSet = HashSet<'T>(HashIdentity.Structural<'T>)

        for v in array do
            if hashSet.Add(v) then
                temp[i] <- v
                i <- i + 1

        ImmutableArray.CreateRange(temp)

    [<CompiledName("DistinctBy")>]
    let inline distinctBy projection (array: ImmutableArray<'T>) : ImmutableArray<'T> =
        let length = array.Length

        if length = 0 then
            empty
        else

            let temp = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked array.Length
            let mutable i = 0
            let hashSet = HashSet<_>(HashIdentity.Structural<_>)

            for v in array do
                if hashSet.Add(projection v) then
                    temp[i] <- v
                    i <- i + 1

            ImmutableArray.CreateRange(temp)

    [<CompiledName("Fold")>]
    let inline fold ([<InlineIfLambda>] folder) state (arr: ImmutableArray<_>) =
        let f = OptimizedClosures.FSharpFunc<_, _, _>.Adapt(folder)
        let mutable state = state
        for i = 0 to arr.Length - 1 do
            state <- f.Invoke(state, arr[i])
        state
