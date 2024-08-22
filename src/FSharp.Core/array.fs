// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Collections

//#nowarn "1118" // 'Make' marked 'inline', perhaps because a recursive value was marked 'inline'

open System
open System.Diagnostics
open System.Collections.Generic
open Microsoft.FSharp.Core
open Microsoft.FSharp.Collections
open Microsoft.FSharp.Core.Operators
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators

/// Basic operations on arrays
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Array =

    let inline checkNonNull argName arg =
        if isNull arg then
            nullArg argName

    let inline indexNotFound () =
        raise (KeyNotFoundException(SR.GetString(SR.keyNotFoundAlt)))

    [<CompiledName("Length")>]
    let length (array: _ array) =
        checkNonNull "array" array
        array.Length

    [<CompiledName("Last")>]
    let inline last (array: 'T array) =
        checkNonNull "array" array

        if array.Length = 0 then
            invalidArg "array" LanguagePrimitives.ErrorStrings.InputArrayEmptyString

        array.[array.Length - 1]

    [<CompiledName("TryLast")>]
    let tryLast (array: 'T array) =
        checkNonNull "array" array

        if array.Length = 0 then
            None
        else
            Some array.[array.Length - 1]

    [<CompiledName("Initialize")>]
    let inline init count initializer =
        Microsoft.FSharp.Primitives.Basics.Array.init count initializer

    [<CompiledName("ZeroCreate")>]
    let zeroCreate count =
        if count < 0 then
            invalidArgInputMustBeNonNegative "count" count

        Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked count

    [<CompiledName("Create")>]
    let create (count: int) (value: 'T) =
        if count < 0 then
            invalidArgInputMustBeNonNegative "count" count

        let array: 'T array =
            Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked count

        for i = 0 to Operators.Checked.(-) array.Length 1 do // use checked arithmetic here to satisfy FxCop
            array.[i] <- value

        array

    [<CompiledName("TryHead")>]
    let tryHead (array: 'T array) =
        checkNonNull "array" array

        if array.Length = 0 then
            None
        else
            Some array.[0]

    [<CompiledName("IsEmpty")>]
    let isEmpty (array: 'T array) =
        checkNonNull "array" array
        array.Length = 0

    [<CompiledName("Tail")>]
    let tail (array: 'T array) =
        checkNonNull "array" array

        if array.Length = 0 then
            invalidArg "array" (SR.GetString(SR.notEnoughElements))

        Microsoft.FSharp.Primitives.Basics.Array.subUnchecked 1 (array.Length - 1) array

    [<CompiledName("Empty")>]
    let empty<'T> : 'T array = [||]

    [<CompiledName("CopyTo")>]
    let inline blit (source: 'T array) (sourceIndex: int) (target: 'T array) (targetIndex: int) (count: int) =
        Array.Copy(source, sourceIndex, target, targetIndex, count)

    let concatArrays (arrs: 'T array array) : 'T array =
        let mutable acc = 0

        for h in arrs do
            acc <- acc + h.Length

        let res = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked acc

        let mutable j = 0

        for i = 0 to arrs.Length - 1 do
            let h = arrs.[i]
            let len = h.Length
            Array.Copy(h, 0, res, j, len)
            j <- j + len

        res

    [<CompiledName("Concat")>]
    let concat (arrays: seq<'T array>) =
        checkNonNull "arrays" arrays

        match arrays with
        | :? ('T array array) as ts -> ts |> concatArrays // avoid a clone, since we only read the array
        | _ -> arrays |> Seq.toArray |> concatArrays

    [<CompiledName("Replicate")>]
    let replicate count initial =
        if count < 0 then
            invalidArgInputMustBeNonNegative "count" count

        let arr: 'T array =
            Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked count

        for i = 0 to arr.Length - 1 do
            arr.[i] <- initial

        arr

    [<CompiledName("Collect")>]
    let collect (mapping: 'T -> 'U array) (array: 'T array) : 'U array =
        checkNonNull "array" array
        let len = array.Length

        let result =
            Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked<'U array> len

        for i = 0 to result.Length - 1 do
            result.[i] <- mapping array.[i]

        concatArrays result

    [<CompiledName("SplitAt")>]
    let splitAt index (array: 'T array) =
        checkNonNull "array" array

        if index < 0 then
            invalidArgInputMustBeNonNegative "index" index

        if array.Length < index then
            raise <| InvalidOperationException(SR.GetString(SR.notEnoughElements))

        if index = 0 then
            let right =
                Microsoft.FSharp.Primitives.Basics.Array.subUnchecked 0 array.Length array

            [||], right
        elif index = array.Length then
            let left =
                Microsoft.FSharp.Primitives.Basics.Array.subUnchecked 0 array.Length array

            left, [||]
        else
            let res1 = Microsoft.FSharp.Primitives.Basics.Array.subUnchecked 0 index array

            let res2 =
                Microsoft.FSharp.Primitives.Basics.Array.subUnchecked index (array.Length - index) array

            res1, res2

    [<CompiledName("Take")>]
    let take count (array: 'T array) =
        checkNonNull "array" array

        if count < 0 then
            invalidArgInputMustBeNonNegative "count" count

        if count = 0 then
            empty
        else
            if count > array.Length then
                raise <| InvalidOperationException(SR.GetString(SR.notEnoughElements))

            Microsoft.FSharp.Primitives.Basics.Array.subUnchecked 0 count array

    [<CompiledName("TakeWhile")>]
    let takeWhile predicate (array: 'T array) =
        checkNonNull "array" array

        if array.Length = 0 then
            empty
        else
            let mutable count = 0

            while count < array.Length && predicate array.[count] do
                count <- count + 1

            Microsoft.FSharp.Primitives.Basics.Array.subUnchecked 0 count array

    let inline countByImpl
        (comparer: IEqualityComparer<'SafeKey>)
        ([<InlineIfLambda>] projection: 'T -> 'SafeKey)
        ([<InlineIfLambda>] getKey: 'SafeKey -> 'Key)
        (array: 'T array)
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
                    dict.[safeKey] <- prev + 1
                else
                    dict.[safeKey] <- 1

            let res = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked dict.Count
            let mutable i = 0

            for group in dict do
                res.[i] <- getKey group.Key, group.Value
                i <- i + 1

            res

    // We avoid wrapping a StructBox, because under 64 JIT we get some "hard" tailcalls which affect performance
    let countByValueType (projection: 'T -> 'Key) (array: 'T array) =
        countByImpl HashIdentity.Structural<'Key> projection id array

    // Wrap a StructBox around all keys in case the key type is itself a type using null as a representation
    let countByRefType (projection: 'T -> 'Key) (array: 'T array) =
        countByImpl
            RuntimeHelpers.StructBox<'Key>.Comparer
            (projection >> RuntimeHelpers.StructBox)
            (fun sb -> sb.Value)
            array

    [<CompiledName("CountBy")>]
    let countBy (projection: 'T -> 'Key) (array: 'T array) =
        checkNonNull "array" array

        if typeof<'Key>.IsValueType then
            countByValueType projection array
        else
            countByRefType projection array

    [<CompiledName("Append")>]
    let append (array1: 'T array) (array2: 'T array) =
        checkNonNull "array1" array1
        checkNonNull "array2" array2
        let n1 = array1.Length
        let n2 = array2.Length

        let res: 'T array =
            Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked (n1 + n2)

        Array.Copy(array1, 0, res, 0, n1)
        Array.Copy(array2, 0, res, n1, n2)
        res

    [<CompiledName("Head")>]
    let head (array: 'T array) =
        checkNonNull "array" array

        if array.Length = 0 then
            invalidArg "array" LanguagePrimitives.ErrorStrings.InputArrayEmptyString
        else
            array.[0]

    [<CompiledName("Copy")>]
    let copy (array: 'T array) =
        checkNonNull "array" array
        (array.Clone() :?> 'T array) // this is marginally faster
    //let len = array.Length
    //let res = zeroCreate len
    //for i = 0 to len - 1 do
    //    res.[i] <- array.[i]
    //res

    [<CompiledName("ToList")>]
    let toList array =
        checkNonNull "array" array
        List.ofArray array

    [<CompiledName("OfList")>]
    let ofList list =
        List.toArray list

    [<CompiledName("Indexed")>]
    let indexed (array: 'T array) =
        checkNonNull "array" array
        let res = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked array.Length

        for i = 0 to res.Length - 1 do
            res.[i] <- (i, array.[i])

        res

    [<CompiledName("Iterate")>]
    let inline iter ([<InlineIfLambda>] action) (array: 'T array) =
        checkNonNull "array" array

        for i = 0 to array.Length - 1 do
            action array.[i]

    [<CompiledName("Distinct")>]
    let distinct (array: 'T array) =
        checkNonNull "array" array
        let temp = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked array.Length
        let mutable i = 0

        let hashSet = HashSet<'T>(HashIdentity.Structural<'T>)

        for v in array do
            if hashSet.Add(v) then
                temp.[i] <- v
                i <- i + 1

        Microsoft.FSharp.Primitives.Basics.Array.subUnchecked 0 i temp

    [<CompiledName("Map")>]
    let inline map ([<InlineIfLambda>] mapping: 'T -> 'U) (array: 'T array) =
        checkNonNull "array" array

        let res: 'U array =
            Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked array.Length

        for i = 0 to res.Length - 1 do
            res.[i] <- mapping array.[i]

        res

    [<CompiledName("Iterate2")>]
    let iter2 action (array1: 'T array) (array2: 'U array) =
        checkNonNull "array1" array1
        checkNonNull "array2" array2
        let f = OptimizedClosures.FSharpFunc<_, _, _>.Adapt(action)

        if array1.Length <> array2.Length then
            invalidArgDifferentArrayLength "array1" array1.Length "array2" array2.Length

        for i = 0 to array1.Length - 1 do
            f.Invoke(array1.[i], array2.[i])

    [<CompiledName("DistinctBy")>]
    let distinctBy projection (array: 'T array) =
        checkNonNull "array" array
        let length = array.Length

        if length = 0 then
            Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked 0
        else

            let temp = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked array.Length
            let mutable i = 0
            let hashSet = HashSet<_>(HashIdentity.Structural<_>)

            for v in array do
                if hashSet.Add(projection v) then
                    temp.[i] <- v
                    i <- i + 1

            Microsoft.FSharp.Primitives.Basics.Array.subUnchecked 0 i temp

    [<CompiledName("Map2")>]
    let map2 mapping (array1: 'T array) (array2: 'U array) =
        checkNonNull "array1" array1
        checkNonNull "array2" array2
        let f = OptimizedClosures.FSharpFunc<_, _, _>.Adapt(mapping)

        if array1.Length <> array2.Length then
            invalidArgDifferentArrayLength "array1" array1.Length "array2" array2.Length

        let res = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked array1.Length

        for i = 0 to res.Length - 1 do
            res.[i] <- f.Invoke(array1.[i], array2.[i])

        res

    [<CompiledName("Map3")>]
    let map3 mapping (array1: 'T1 array) (array2: 'T2 array) (array3: 'T3 array) =
        checkNonNull "array1" array1
        checkNonNull "array2" array2
        checkNonNull "array3" array3
        let f = OptimizedClosures.FSharpFunc<_, _, _, _>.Adapt(mapping)
        let len1 = array1.Length

        if len1 <> array2.Length || len1 <> array3.Length then
            invalidArg3ArraysDifferent "array1" "array2" "array3" len1 array2.Length array3.Length

        let res = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked len1

        for i = 0 to res.Length - 1 do
            res.[i] <- f.Invoke(array1.[i], array2.[i], array3.[i])

        res

    [<CompiledName("MapIndexed2")>]
    let mapi2 mapping (array1: 'T array) (array2: 'U array) =
        checkNonNull "array1" array1
        checkNonNull "array2" array2
        let f = OptimizedClosures.FSharpFunc<_, _, _, _>.Adapt(mapping)

        if array1.Length <> array2.Length then
            invalidArgDifferentArrayLength "array1" array1.Length "array2" array2.Length

        let res = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked array1.Length

        for i = 0 to res.Length - 1 do
            res.[i] <- f.Invoke(i, array1.[i], array2.[i])

        res

    [<CompiledName("IterateIndexed")>]
    let iteri action (array: 'T array) =
        checkNonNull "array" array
        let f = OptimizedClosures.FSharpFunc<_, _, _>.Adapt(action)

        for i = 0 to array.Length - 1 do
            f.Invoke(i, array.[i])

    [<CompiledName("IterateIndexed2")>]
    let iteri2 action (array1: 'T array) (array2: 'U array) =
        checkNonNull "array1" array1
        checkNonNull "array2" array2
        let f = OptimizedClosures.FSharpFunc<_, _, _, _>.Adapt(action)

        if array1.Length <> array2.Length then
            invalidArgDifferentArrayLength "array1" array1.Length "array2" array2.Length

        for i = 0 to array1.Length - 1 do
            f.Invoke(i, array1.[i], array2.[i])

    [<CompiledName("MapIndexed")>]
    let mapi (mapping: int -> 'T -> 'U) (array: 'T array) =
        checkNonNull "array" array
        let f = OptimizedClosures.FSharpFunc<_, _, _>.Adapt(mapping)
        let res = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked array.Length

        for i = 0 to array.Length - 1 do
            res.[i] <- f.Invoke(i, array.[i])

        res

    [<CompiledName("MapFold")>]
    let mapFold<'T, 'State, 'Result> (mapping: 'State -> 'T -> 'Result * 'State) state array =
        checkNonNull "array" array
        Microsoft.FSharp.Primitives.Basics.Array.mapFold mapping state array

    [<CompiledName("MapFoldBack")>]
    let mapFoldBack<'T, 'State, 'Result> (mapping: 'T -> 'State -> 'Result * 'State) array state =
        checkNonNull "array" array
        Microsoft.FSharp.Primitives.Basics.Array.mapFoldBack mapping array state

    [<CompiledName("Exists")>]
    let inline exists ([<InlineIfLambda>] predicate: 'T -> bool) (array: 'T array) =
        checkNonNull "array" array
        let mutable state = false
        let mutable i = 0

        while not state && i < array.Length do
            state <- predicate array.[i]
            i <- i + 1

        state

    [<CompiledName("Contains")>]
    let inline contains value (array: 'T array) =
        checkNonNull "array" array
        let mutable state = false
        let mutable i = 0

        while not state && i < array.Length do
            state <- value = array.[i]
            i <- i + 1

        state

    [<CompiledName("Exists2")>]
    let exists2 predicate (array1: _ array) (array2: _ array) =
        checkNonNull "array1" array1
        checkNonNull "array2" array2
        let f = OptimizedClosures.FSharpFunc<_, _, _>.Adapt(predicate)
        let len1 = array1.Length

        if len1 <> array2.Length then
            invalidArgDifferentArrayLength "array1" array1.Length "array2" array2.Length

        let rec loop i =
            i < len1 && (f.Invoke(array1.[i], array2.[i]) || loop (i + 1))

        loop 0

    [<CompiledName("ForAll")>]
    let forall (predicate: 'T -> bool) (array: 'T array) =
        checkNonNull "array" array
        let len = array.Length

        let rec loop i =
            i >= len || (predicate array.[i] && loop (i + 1))

        loop 0

    [<CompiledName("ForAll2")>]
    let forall2 predicate (array1: _ array) (array2: _ array) =
        checkNonNull "array1" array1
        checkNonNull "array2" array2
        let f = OptimizedClosures.FSharpFunc<_, _, _>.Adapt(predicate)
        let len1 = array1.Length

        if len1 <> array2.Length then
            invalidArgDifferentArrayLength "array1" array1.Length "array2" array2.Length

        let rec loop i =
            i >= len1 || (f.Invoke(array1.[i], array2.[i]) && loop (i + 1))

        loop 0

    let inline groupByImpl
        (comparer: IEqualityComparer<'SafeKey>)
        ([<InlineIfLambda>] keyf: 'T -> 'SafeKey)
        ([<InlineIfLambda>] getKey: 'SafeKey -> 'Key)
        (array: 'T array)
        =
        let length = array.Length

        if length = 0 then
            Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked 0
        else
            let dict = Dictionary<_, ResizeArray<_>> comparer

            // Build the groupings
            for i = 0 to length - 1 do
                let v = array.[i]
                let safeKey = keyf v
                let mutable prev = Unchecked.defaultof<_>

                if dict.TryGetValue(safeKey, &prev) then
                    prev.Add v
                else
                    let prev = ResizeArray()
                    dict.[safeKey] <- prev
                    prev.Add v

            // Return the array-of-arrays.
            let result = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked dict.Count
            let mutable i = 0

            for group in dict do
                result.[i] <- getKey group.Key, group.Value.ToArray()
                i <- i + 1

            result

    // We avoid wrapping a StructBox, because under 64 JIT we get some "hard" tailcalls which affect performance
    let groupByValueType (keyf: 'T -> 'Key) (array: 'T array) =
        groupByImpl HashIdentity.Structural<'Key> keyf id array

    // Wrap a StructBox around all keys in case the key type is itself a type using null as a representation
    let groupByRefType (keyf: 'T -> 'Key) (array: 'T array) =
        groupByImpl
            RuntimeHelpers.StructBox<'Key>.Comparer
            (keyf >> RuntimeHelpers.StructBox)
            (fun sb -> sb.Value)
            array

    [<CompiledName("GroupBy")>]
    let groupBy (projection: 'T -> 'Key) (array: 'T array) =
        checkNonNull "array" array

        if typeof<'Key>.IsValueType then
            groupByValueType projection array
        else
            groupByRefType projection array

    [<CompiledName("Pick")>]
    let pick chooser (array: _ array) =
        checkNonNull "array" array

        let rec loop i =
            if i >= array.Length then
                indexNotFound ()
            else
                match chooser array.[i] with
                | None -> loop (i + 1)
                | Some res -> res

        loop 0

    [<CompiledName("TryPick")>]
    let tryPick chooser (array: _ array) =
        checkNonNull "array" array

        let rec loop i =
            if i >= array.Length then
                None
            else
                match chooser array.[i] with
                | None -> loop (i + 1)
                | res -> res

        loop 0

    [<CompiledName("Choose")>]
    let choose (chooser: 'T -> 'U Option) (array: 'T array) =
        checkNonNull "array" array

        let mutable i = 0
        let mutable first = Unchecked.defaultof<'U>
        let mutable found = false

        while i < array.Length && not found do
            let element = array.[i]

            match chooser element with
            | None -> i <- i + 1
            | Some b ->
                first <- b
                found <- true

        if i <> array.Length then

            let chunk1: 'U array =
                Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked ((array.Length >>> 2) + 1)

            chunk1.[0] <- first
            let mutable count = 1
            i <- i + 1

            while count < chunk1.Length && i < array.Length do
                let element = array.[i]

                match chooser element with
                | None -> ()
                | Some b ->
                    chunk1.[count] <- b
                    count <- count + 1

                i <- i + 1

            if i < array.Length then
                let chunk2: 'U array =
                    Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked (array.Length - i)

                count <- 0

                while i < array.Length do
                    let element = array.[i]

                    match chooser element with
                    | None -> ()
                    | Some b ->
                        chunk2.[count] <- b
                        count <- count + 1

                    i <- i + 1

                let res: 'U array =
                    Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked (chunk1.Length + count)

                Array.Copy(chunk1, res, chunk1.Length)
                Array.Copy(chunk2, 0, res, chunk1.Length, count)
                res
            else
                Microsoft.FSharp.Primitives.Basics.Array.subUnchecked 0 count chunk1
        else
            empty

    // The filter module is a space and performance for Array.filter based optimization that uses
    // a bitarray to store the results of the filtering of every element of the array. This means
    // that the only additional temporary garbage that needs to be allocated is {array.Length/8} bytes.
    //
    // Other optimizations include:
    // - arrays < 32 elements don't allocate any garbage at all
    // - when the predicate yields consecutive runs of true data that is >= 32 elements (and fall
    //   into maskArray buckets) are copied in chunks using System.Array.Copy
    module Filter =
        let private populateMask<'a> (f: 'a -> bool) (src: 'a array) (maskArray: uint32 array) =
            let mutable count = 0

            for maskIdx = 0 to maskArray.Length - 1 do
                let srcIdx = maskIdx * 32
                let mutable mask = 0u

                if f src.[srcIdx + 0x00] then
                    mask <- mask ||| (1u <<< 0x00)
                    count <- count + 1

                if f src.[srcIdx + 0x01] then
                    mask <- mask ||| (1u <<< 0x01)
                    count <- count + 1

                if f src.[srcIdx + 0x02] then
                    mask <- mask ||| (1u <<< 0x02)
                    count <- count + 1

                if f src.[srcIdx + 0x03] then
                    mask <- mask ||| (1u <<< 0x03)
                    count <- count + 1

                if f src.[srcIdx + 0x04] then
                    mask <- mask ||| (1u <<< 0x04)
                    count <- count + 1

                if f src.[srcIdx + 0x05] then
                    mask <- mask ||| (1u <<< 0x05)
                    count <- count + 1

                if f src.[srcIdx + 0x06] then
                    mask <- mask ||| (1u <<< 0x06)
                    count <- count + 1

                if f src.[srcIdx + 0x07] then
                    mask <- mask ||| (1u <<< 0x07)
                    count <- count + 1

                if f src.[srcIdx + 0x08] then
                    mask <- mask ||| (1u <<< 0x08)
                    count <- count + 1

                if f src.[srcIdx + 0x09] then
                    mask <- mask ||| (1u <<< 0x09)
                    count <- count + 1

                if f src.[srcIdx + 0x0A] then
                    mask <- mask ||| (1u <<< 0x0A)
                    count <- count + 1

                if f src.[srcIdx + 0x0B] then
                    mask <- mask ||| (1u <<< 0x0B)
                    count <- count + 1

                if f src.[srcIdx + 0x0C] then
                    mask <- mask ||| (1u <<< 0x0C)
                    count <- count + 1

                if f src.[srcIdx + 0x0D] then
                    mask <- mask ||| (1u <<< 0x0D)
                    count <- count + 1

                if f src.[srcIdx + 0x0E] then
                    mask <- mask ||| (1u <<< 0x0E)
                    count <- count + 1

                if f src.[srcIdx + 0x0F] then
                    mask <- mask ||| (1u <<< 0x0F)
                    count <- count + 1

                if f src.[srcIdx + 0x10] then
                    mask <- mask ||| (1u <<< 0x10)
                    count <- count + 1

                if f src.[srcIdx + 0x11] then
                    mask <- mask ||| (1u <<< 0x11)
                    count <- count + 1

                if f src.[srcIdx + 0x12] then
                    mask <- mask ||| (1u <<< 0x12)
                    count <- count + 1

                if f src.[srcIdx + 0x13] then
                    mask <- mask ||| (1u <<< 0x13)
                    count <- count + 1

                if f src.[srcIdx + 0x14] then
                    mask <- mask ||| (1u <<< 0x14)
                    count <- count + 1

                if f src.[srcIdx + 0x15] then
                    mask <- mask ||| (1u <<< 0x15)
                    count <- count + 1

                if f src.[srcIdx + 0x16] then
                    mask <- mask ||| (1u <<< 0x16)
                    count <- count + 1

                if f src.[srcIdx + 0x17] then
                    mask <- mask ||| (1u <<< 0x17)
                    count <- count + 1

                if f src.[srcIdx + 0x18] then
                    mask <- mask ||| (1u <<< 0x18)
                    count <- count + 1

                if f src.[srcIdx + 0x19] then
                    mask <- mask ||| (1u <<< 0x19)
                    count <- count + 1

                if f src.[srcIdx + 0x1A] then
                    mask <- mask ||| (1u <<< 0x1A)
                    count <- count + 1

                if f src.[srcIdx + 0x1B] then
                    mask <- mask ||| (1u <<< 0x1B)
                    count <- count + 1

                if f src.[srcIdx + 0x1C] then
                    mask <- mask ||| (1u <<< 0x1C)
                    count <- count + 1

                if f src.[srcIdx + 0x1D] then
                    mask <- mask ||| (1u <<< 0x1D)
                    count <- count + 1

                if f src.[srcIdx + 0x1E] then
                    mask <- mask ||| (1u <<< 0x1E)
                    count <- count + 1

                if f src.[srcIdx + 0x1F] then
                    mask <- mask ||| (1u <<< 0x1F)
                    count <- count + 1

                maskArray.[maskIdx] <- mask

            count

#if BUILDING_WITH_LKG || NO_NULLCHECKING_LIB_SUPPORT
        let private createMask<'a>
            (f: 'a -> bool)
            (src: 'a array)
            (maskArrayOut: byref<uint32 array>)
            (leftoverMaskOut: byref<uint32>)
            =
#else
        let private createMask<'a>
            (f: 'a -> bool)
            (src: array<'a>)
            (maskArrayOut: byref<array<uint32> | null>)
            (leftoverMaskOut: byref<uint32>)
            =
#endif
            let maskArrayLength = src.Length / 0x20

            // null when there are less than 32 items in src array.
            let maskArray =
                if maskArrayLength = 0 then
                    null
                else
                    Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked<uint32> maskArrayLength

            let mutable count =
                match maskArray with
                | null -> 0
                | maskArray -> populateMask f src maskArray

            let leftoverMask =
                match src.Length % 0x20 with
                | 0 -> 0u
                | _ ->
                    let mutable mask = 0u
                    let mutable elementMask = 1u

                    for arrayIdx = maskArrayLength * 0x20 to src.Length - 1 do
                        if f src.[arrayIdx] then
                            mask <- mask ||| elementMask
                            count <- count + 1

                        elementMask <- elementMask <<< 1

                    mask

            maskArrayOut <- maskArray
            leftoverMaskOut <- leftoverMask
            count

        let private populateDstViaMask<'a> (src: 'a array) (maskArray: uint32 array) (dst: 'a array) =
            let mutable dstIdx = 0
            let mutable batchCount = 0

            for maskIdx = 0 to maskArray.Length - 1 do
                let mask = maskArray.[maskIdx]

                if mask = 0xFFFFFFFFu then
                    batchCount <- batchCount + 1
                else
                    let srcIdx = maskIdx * 0x20

                    if batchCount <> 0 then
                        let batchSize = batchCount * 0x20
                        System.Array.Copy(src, srcIdx - batchSize, dst, dstIdx, batchSize)
                        dstIdx <- dstIdx + batchSize
                        batchCount <- 0

                    if mask <> 0u then
                        if mask &&& (1u <<< 0x00) <> 0u then
                            dst.[dstIdx] <- src.[srcIdx + 0x00]
                            dstIdx <- dstIdx + 1

                        if mask &&& (1u <<< 0x01) <> 0u then
                            dst.[dstIdx] <- src.[srcIdx + 0x01]
                            dstIdx <- dstIdx + 1

                        if mask &&& (1u <<< 0x02) <> 0u then
                            dst.[dstIdx] <- src.[srcIdx + 0x02]
                            dstIdx <- dstIdx + 1

                        if mask &&& (1u <<< 0x03) <> 0u then
                            dst.[dstIdx] <- src.[srcIdx + 0x03]
                            dstIdx <- dstIdx + 1

                        if mask &&& (1u <<< 0x04) <> 0u then
                            dst.[dstIdx] <- src.[srcIdx + 0x04]
                            dstIdx <- dstIdx + 1

                        if mask &&& (1u <<< 0x05) <> 0u then
                            dst.[dstIdx] <- src.[srcIdx + 0x05]
                            dstIdx <- dstIdx + 1

                        if mask &&& (1u <<< 0x06) <> 0u then
                            dst.[dstIdx] <- src.[srcIdx + 0x06]
                            dstIdx <- dstIdx + 1

                        if mask &&& (1u <<< 0x07) <> 0u then
                            dst.[dstIdx] <- src.[srcIdx + 0x07]
                            dstIdx <- dstIdx + 1

                        if mask &&& (1u <<< 0x08) <> 0u then
                            dst.[dstIdx] <- src.[srcIdx + 0x08]
                            dstIdx <- dstIdx + 1

                        if mask &&& (1u <<< 0x09) <> 0u then
                            dst.[dstIdx] <- src.[srcIdx + 0x09]
                            dstIdx <- dstIdx + 1

                        if mask &&& (1u <<< 0x0A) <> 0u then
                            dst.[dstIdx] <- src.[srcIdx + 0x0A]
                            dstIdx <- dstIdx + 1

                        if mask &&& (1u <<< 0x0B) <> 0u then
                            dst.[dstIdx] <- src.[srcIdx + 0x0B]
                            dstIdx <- dstIdx + 1

                        if mask &&& (1u <<< 0x0C) <> 0u then
                            dst.[dstIdx] <- src.[srcIdx + 0x0C]
                            dstIdx <- dstIdx + 1

                        if mask &&& (1u <<< 0x0D) <> 0u then
                            dst.[dstIdx] <- src.[srcIdx + 0x0D]
                            dstIdx <- dstIdx + 1

                        if mask &&& (1u <<< 0x0E) <> 0u then
                            dst.[dstIdx] <- src.[srcIdx + 0x0E]
                            dstIdx <- dstIdx + 1

                        if mask &&& (1u <<< 0x0F) <> 0u then
                            dst.[dstIdx] <- src.[srcIdx + 0x0F]
                            dstIdx <- dstIdx + 1

                        if mask &&& (1u <<< 0x10) <> 0u then
                            dst.[dstIdx] <- src.[srcIdx + 0x10]
                            dstIdx <- dstIdx + 1

                        if mask &&& (1u <<< 0x11) <> 0u then
                            dst.[dstIdx] <- src.[srcIdx + 0x11]
                            dstIdx <- dstIdx + 1

                        if mask &&& (1u <<< 0x12) <> 0u then
                            dst.[dstIdx] <- src.[srcIdx + 0x12]
                            dstIdx <- dstIdx + 1

                        if mask &&& (1u <<< 0x13) <> 0u then
                            dst.[dstIdx] <- src.[srcIdx + 0x13]
                            dstIdx <- dstIdx + 1

                        if mask &&& (1u <<< 0x14) <> 0u then
                            dst.[dstIdx] <- src.[srcIdx + 0x14]
                            dstIdx <- dstIdx + 1

                        if mask &&& (1u <<< 0x15) <> 0u then
                            dst.[dstIdx] <- src.[srcIdx + 0x15]
                            dstIdx <- dstIdx + 1

                        if mask &&& (1u <<< 0x16) <> 0u then
                            dst.[dstIdx] <- src.[srcIdx + 0x16]
                            dstIdx <- dstIdx + 1

                        if mask &&& (1u <<< 0x17) <> 0u then
                            dst.[dstIdx] <- src.[srcIdx + 0x17]
                            dstIdx <- dstIdx + 1

                        if mask &&& (1u <<< 0x18) <> 0u then
                            dst.[dstIdx] <- src.[srcIdx + 0x18]
                            dstIdx <- dstIdx + 1

                        if mask &&& (1u <<< 0x19) <> 0u then
                            dst.[dstIdx] <- src.[srcIdx + 0x19]
                            dstIdx <- dstIdx + 1

                        if mask &&& (1u <<< 0x1A) <> 0u then
                            dst.[dstIdx] <- src.[srcIdx + 0x1A]
                            dstIdx <- dstIdx + 1

                        if mask &&& (1u <<< 0x1B) <> 0u then
                            dst.[dstIdx] <- src.[srcIdx + 0x1B]
                            dstIdx <- dstIdx + 1

                        if mask &&& (1u <<< 0x1C) <> 0u then
                            dst.[dstIdx] <- src.[srcIdx + 0x1C]
                            dstIdx <- dstIdx + 1

                        if mask &&& (1u <<< 0x1D) <> 0u then
                            dst.[dstIdx] <- src.[srcIdx + 0x1D]
                            dstIdx <- dstIdx + 1

                        if mask &&& (1u <<< 0x1E) <> 0u then
                            dst.[dstIdx] <- src.[srcIdx + 0x1E]
                            dstIdx <- dstIdx + 1

                        if mask &&& (1u <<< 0x1F) <> 0u then
                            dst.[dstIdx] <- src.[srcIdx + 0x1F]
                            dstIdx <- dstIdx + 1

            if batchCount <> 0 then
                let srcIdx = maskArray.Length * 0x20
                let batchSize = batchCount * 0x20
                System.Array.Copy(src, srcIdx - batchSize, dst, dstIdx, batchSize)
                dstIdx <- dstIdx + batchSize

            dstIdx

#if BUILDING_WITH_LKG || NO_NULLCHECKING_LIB_SUPPORT
        let private filterViaMask (maskArray: uint32 array) (leftoverMask: uint32) (count: int) (src: _ array) =
#else
        let private filterViaMask (maskArray: uint32 array | null) (leftoverMask: uint32) (count: int) (src: _ array) =
#endif
            let dst = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked count

            let mutable dstIdx = 0

            let srcIdx =
                match maskArray with
                | null -> 0
                | _ ->
                    dstIdx <- populateDstViaMask src maskArray dst
                    maskArray.Length * 0x20

            let mutable elementMask = 1u

            for srcIdx = srcIdx to src.Length - 1 do
                if leftoverMask &&& elementMask <> 0u then
                    dst.[dstIdx] <- src.[srcIdx]
                    dstIdx <- dstIdx + 1

                elementMask <- elementMask <<< 1

            dst

        let filter f (src: _ array) =
            let mutable maskArray = Unchecked.defaultof<_>
            let mutable leftOverMask = Unchecked.defaultof<_>

            match createMask f src &maskArray &leftOverMask with
            | 0 -> empty
            | count -> filterViaMask maskArray leftOverMask count src

    [<CompiledName("Filter")>]
    let filter predicate (array: _ array) =
        checkNonNull "array" array
        Filter.filter predicate array

    [<CompiledName("Where")>]
    let where predicate (array: _ array) =
        filter predicate array

    [<CompiledName("Except")>]
    let except (itemsToExclude: seq<_>) (array: _ array) =
        checkNonNull "itemsToExclude" itemsToExclude
        checkNonNull "array" array

        if array.Length = 0 then
            array
        else
            let cached = HashSet(itemsToExclude, HashIdentity.Structural)
            array |> filter cached.Add

    [<CompiledName("Partition")>]
    let partition predicate (array: _ array) =
        checkNonNull "array" array
        let res = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked array.Length
        let mutable upCount = 0
        let mutable downCount = array.Length - 1

        for x in array do
            if predicate x then
                res.[upCount] <- x
                upCount <- upCount + 1
            else
                res.[downCount] <- x
                downCount <- downCount - 1

        let res1 = Microsoft.FSharp.Primitives.Basics.Array.subUnchecked 0 upCount res

        let res2 =
            Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked (array.Length - upCount)

        downCount <- array.Length - 1

        for i = 0 to res2.Length - 1 do
            res2.[i] <- res.[downCount]
            downCount <- downCount - 1

        res1, res2

    [<CompiledName("Find")>]
    let find predicate (array: _ array) =
        checkNonNull "array" array

        let rec loop i =
            if i >= array.Length then
                indexNotFound ()
            else if predicate array.[i] then
                array.[i]
            else
                loop (i + 1)

        loop 0

    [<CompiledName("TryFind")>]
    let tryFind predicate (array: _ array) =
        checkNonNull "array" array

        let rec loop i =
            if i >= array.Length then
                None
            else if predicate array.[i] then
                Some array.[i]
            else
                loop (i + 1)

        loop 0

    [<CompiledName("Skip")>]
    let skip count (array: 'T array) =
        checkNonNull "array" array

        if count > array.Length then
            invalidArgOutOfRange "count" count "array.Length" array.Length

        if count = array.Length then
            empty
        else
            let count = max count 0
            Microsoft.FSharp.Primitives.Basics.Array.subUnchecked count (array.Length - count) array

    [<CompiledName("SkipWhile")>]
    let skipWhile predicate (array: 'T array) =
        checkNonNull "array" array
        let mutable i = 0

        while i < array.Length && predicate array.[i] do
            i <- i + 1

        match array.Length - i with
        | 0 -> empty
        | resLen -> Microsoft.FSharp.Primitives.Basics.Array.subUnchecked i resLen array

    [<CompiledName("FindBack")>]
    let findBack predicate (array: _ array) =
        checkNonNull "array" array
        Microsoft.FSharp.Primitives.Basics.Array.findBack predicate array

    [<CompiledName("TryFindBack")>]
    let tryFindBack predicate (array: _ array) =
        checkNonNull "array" array
        Microsoft.FSharp.Primitives.Basics.Array.tryFindBack predicate array

    [<CompiledName("FindIndexBack")>]
    let findIndexBack predicate (array: _ array) =
        checkNonNull "array" array
        Microsoft.FSharp.Primitives.Basics.Array.findIndexBack predicate array

    [<CompiledName("TryFindIndexBack")>]
    let tryFindIndexBack predicate (array: _ array) =
        checkNonNull "array" array
        Microsoft.FSharp.Primitives.Basics.Array.tryFindIndexBack predicate array

    [<CompiledName("Windowed")>]
    let windowed windowSize (array: 'T array) =
        checkNonNull "array" array

        if windowSize <= 0 then
            invalidArgInputMustBePositive "windowSize" windowSize

        let len = array.Length

        if windowSize > len then
            empty
        else
            let res: 'T array array =
                Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked (len - windowSize + 1)

            for i = 0 to len - windowSize do
                res.[i] <- Microsoft.FSharp.Primitives.Basics.Array.subUnchecked i windowSize array

            res

    [<CompiledName("ChunkBySize")>]
    let chunkBySize chunkSize (array: 'T array) =
        checkNonNull "array" array

        if chunkSize <= 0 then
            invalidArgInputMustBePositive "chunkSize" chunkSize

        let len = array.Length

        if len = 0 then
            empty
        else if chunkSize > len then
            [| copy array |]
        else
            let chunkCount = (len - 1) / chunkSize + 1

            let res: 'T array array =
                Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked chunkCount

            for i = 0 to len / chunkSize - 1 do
                res.[i] <- Microsoft.FSharp.Primitives.Basics.Array.subUnchecked (i * chunkSize) chunkSize array

            if len % chunkSize <> 0 then
                res.[chunkCount - 1] <-
                    Microsoft.FSharp.Primitives.Basics.Array.subUnchecked
                        ((chunkCount - 1) * chunkSize)
                        (len % chunkSize)
                        array

            res

    [<CompiledName("SplitInto")>]
    let splitInto count (array: _ array) =
        checkNonNull "array" array

        if count <= 0 then
            invalidArgInputMustBePositive "count" count

        Microsoft.FSharp.Primitives.Basics.Array.splitInto count array

    [<CompiledName("Zip")>]
    let zip (array1: _ array) (array2: _ array) =
        checkNonNull "array1" array1
        checkNonNull "array2" array2
        let len1 = array1.Length

        if len1 <> array2.Length then
            invalidArgDifferentArrayLength "array1" array1.Length "array2" array2.Length

        let res = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked len1

        for i = 0 to res.Length - 1 do
            res.[i] <- (array1.[i], array2.[i])

        res

    [<CompiledName("Zip3")>]
    let zip3 (array1: _ array) (array2: _ array) (array3: _ array) =
        checkNonNull "array1" array1
        checkNonNull "array2" array2
        checkNonNull "array3" array3
        let len1 = array1.Length

        if len1 <> array2.Length || len1 <> array3.Length then
            invalidArg3ArraysDifferent "array1" "array2" "array3" len1 array2.Length array3.Length

        let res = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked len1

        for i = 0 to res.Length - 1 do
            res.[i] <- (array1.[i], array2.[i], array3.[i])

        res

    [<CompiledName("AllPairs")>]
    let allPairs (array1: _ array) (array2: _ array) =
        checkNonNull "array1" array1
        checkNonNull "array2" array2
        let len1 = array1.Length
        let len2 = array2.Length
        let res = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked (len1 * len2)

        for i = 0 to array1.Length - 1 do
            for j = 0 to array2.Length - 1 do
                res.[i * len2 + j] <- (array1.[i], array2.[j])

        res

    [<CompiledName("Unfold")>]
    let unfold<'T, 'State> (generator: 'State -> ('T * 'State) option) (state: 'State) =
        let res = ResizeArray<_>()

        let rec loop state =
            match generator state with
            | None -> ()
            | Some(x, s') ->
                res.Add(x)
                loop s'

        loop state
        res.ToArray()

    [<CompiledName("Unzip")>]
    let unzip (array: _ array) =
        checkNonNull "array" array
        let len = array.Length
        let res1 = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked len
        let res2 = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked len

        for i = 0 to array.Length - 1 do
            let x, y = array.[i]
            res1.[i] <- x
            res2.[i] <- y

        res1, res2

    [<CompiledName("Unzip3")>]
    let unzip3 (array: _ array) =
        checkNonNull "array" array
        let len = array.Length
        let res1 = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked len
        let res2 = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked len
        let res3 = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked len

        for i = 0 to array.Length - 1 do
            let x, y, z = array.[i]
            res1.[i] <- x
            res2.[i] <- y
            res3.[i] <- z

        res1, res2, res3

    [<CompiledName("Reverse")>]
    let rev (array: _ array) =
        checkNonNull "array" array
        let res = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked array.Length
        let mutable j = array.Length - 1

        for i = 0 to array.Length - 1 do
            res.[j] <- array.[i]
            j <- j - 1

        res

    [<CompiledName("Fold")>]
    let fold<'T, 'State> (folder: 'State -> 'T -> 'State) (state: 'State) (array: 'T array) =
        checkNonNull "array" array
        let f = OptimizedClosures.FSharpFunc<_, _, _>.Adapt(folder)
        let mutable state = state

        for i = 0 to array.Length - 1 do
            state <- f.Invoke(state, array.[i])

        state

    [<CompiledName("FoldBack")>]
    let foldBack<'T, 'State> (folder: 'T -> 'State -> 'State) (array: 'T array) (state: 'State) =
        checkNonNull "array" array
        let f = OptimizedClosures.FSharpFunc<_, _, _>.Adapt(folder)
        let mutable res = state

        for i = array.Length - 1 downto 0 do
            res <- f.Invoke(array.[i], res)

        res

    [<CompiledName("FoldBack2")>]
    let foldBack2<'T1, 'T2, 'State> folder (array1: 'T1 array) (array2: 'T2 array) (state: 'State) =
        checkNonNull "array1" array1
        checkNonNull "array2" array2
        let f = OptimizedClosures.FSharpFunc<_, _, _, _>.Adapt(folder)
        let mutable res = state
        let len = array1.Length

        if len <> array2.Length then
            invalidArgDifferentArrayLength "array1" len "array2" array2.Length

        for i = len - 1 downto 0 do
            res <- f.Invoke(array1.[i], array2.[i], res)

        res

    [<CompiledName("Fold2")>]
    let fold2<'T1, 'T2, 'State> folder (state: 'State) (array1: 'T1 array) (array2: 'T2 array) =
        checkNonNull "array1" array1
        checkNonNull "array2" array2
        let f = OptimizedClosures.FSharpFunc<_, _, _, _>.Adapt(folder)
        let mutable state = state

        if array1.Length <> array2.Length then
            invalidArgDifferentArrayLength "array1" array1.Length "array2" array2.Length

        for i = 0 to array1.Length - 1 do
            state <- f.Invoke(state, array1.[i], array2.[i])

        state

    let foldSubRight f (array: _ array) start fin acc =
        checkNonNull "array" array
        let f = OptimizedClosures.FSharpFunc<_, _, _>.Adapt(f)
        let mutable res = acc

        for i = fin downto start do
            res <- f.Invoke(array.[i], res)

        res

    let scanSubLeft f initState (array: _ array) start fin =
        checkNonNull "array" array
        let f = OptimizedClosures.FSharpFunc<_, _, _>.Adapt(f)
        let mutable state = initState
        let res = create (2 + fin - start) initState

        for i = start to fin do
            state <- f.Invoke(state, array.[i])
            res.[i - start + 1] <- state

        res

    [<CompiledName("Scan")>]
    let scan<'T, 'State> folder (state: 'State) (array: 'T array) =
        checkNonNull "array" array
        let len = array.Length
        scanSubLeft folder state array 0 (len - 1)

    [<CompiledName("ScanBack")>]
    let scanBack<'T, 'State> folder (array: 'T array) (state: 'State) =
        checkNonNull "array" array
        Microsoft.FSharp.Primitives.Basics.Array.scanSubRight folder array 0 (array.Length - 1) state

    [<CompiledName("Singleton")>]
    let inline singleton value =
        [| value |]

    [<CompiledName("Pairwise")>]
    let pairwise (array: 'T array) =
        checkNonNull "array" array

        if array.Length < 2 then
            empty
        else
            init (array.Length - 1) (fun i -> array.[i], array.[i + 1])

    [<CompiledName("Reduce")>]
    let reduce reduction (array: _ array) =
        checkNonNull "array" array
        let len = array.Length

        if len = 0 then
            invalidArg "array" LanguagePrimitives.ErrorStrings.InputArrayEmptyString
        else
            let f = OptimizedClosures.FSharpFunc<_, _, _>.Adapt(reduction)
            let mutable res = array.[0]

            for i = 1 to array.Length - 1 do
                res <- f.Invoke(res, array.[i])

            res

    [<CompiledName("ReduceBack")>]
    let reduceBack reduction (array: _ array) =
        checkNonNull "array" array
        let len = array.Length

        if len = 0 then
            invalidArg "array" LanguagePrimitives.ErrorStrings.InputArrayEmptyString
        else
            foldSubRight reduction array 0 (len - 2) array.[len - 1]

    [<CompiledName("SortInPlaceWith")>]
    let sortInPlaceWith comparer (array: 'T array) =
        checkNonNull "array" array
        let len = array.Length

        if len < 2 then
            ()
        elif len = 2 then
            let c = comparer array.[0] array.[1]

            if c > 0 then
                let tmp = array.[0]
                array.[0] <- array.[1]
                array.[1] <- tmp
        else
            Array.Sort(array, ComparisonIdentity.FromFunction(comparer))

    [<CompiledName("SortInPlaceBy")>]
    let sortInPlaceBy (projection: 'T -> 'U) (array: 'T array) =
        checkNonNull "array" array
        Microsoft.FSharp.Primitives.Basics.Array.unstableSortInPlaceBy projection array

    [<CompiledName("SortInPlace")>]
    let sortInPlace (array: 'T array) =
        checkNonNull "array" array
        Microsoft.FSharp.Primitives.Basics.Array.unstableSortInPlace array

    [<CompiledName("SortWith")>]
    let sortWith (comparer: 'T -> 'T -> int) (array: 'T array) =
        checkNonNull "array" array
        let result = copy array
        sortInPlaceWith comparer result
        result

    [<CompiledName("SortBy")>]
    let sortBy projection array =
        checkNonNull "array" array
        let result = copy array
        sortInPlaceBy projection result
        result

    [<CompiledName("Sort")>]
    let sort array =
        checkNonNull "array" array
        let result = copy array
        sortInPlace result
        result

    [<CompiledName("SortByDescending")>]
    let inline sortByDescending projection array =
        checkNonNull "array" array

        let inline compareDescending a b =
            compare (projection b) (projection a)

        sortWith compareDescending array

    [<CompiledName("SortDescending")>]
    let inline sortDescending array =
        checkNonNull "array" array

        let inline compareDescending a b =
            compare b a

        sortWith compareDescending array

    [<CompiledName("ToSeq")>]
    let toSeq array =
        checkNonNull "array" array
        Seq.ofArray array

    [<CompiledName("OfSeq")>]
    let ofSeq source =
        checkNonNull "source" source
        Seq.toArray source

    [<CompiledName("FindIndex")>]
    let findIndex predicate (array: _ array) =
        checkNonNull "array" array
        let len = array.Length

        let rec go n =
            if n >= len then indexNotFound ()
            elif predicate array.[n] then n
            else go (n + 1)

        go 0

    [<CompiledName("TryFindIndex")>]
    let tryFindIndex predicate (array: _ array) =
        checkNonNull "array" array
        let len = array.Length

        let rec go n =
            if n >= len then None
            elif predicate array.[n] then Some n
            else go (n + 1)

        go 0

    [<CompiledName("Permute")>]
    let permute indexMap (array: _ array) =
        checkNonNull "array" array
        Microsoft.FSharp.Primitives.Basics.Array.permute indexMap array

    [<CompiledName("Sum")>]
    let inline sum (array: ^T array) : ^T =
        checkNonNull "array" array
        let mutable acc = LanguagePrimitives.GenericZero< ^T>

        for i = 0 to array.Length - 1 do
            acc <- Checked.(+) acc array.[i]

        acc

    [<CompiledName("SumBy")>]
    let inline sumBy ([<InlineIfLambda>] projection: 'T -> ^U) (array: 'T array) : ^U =
        checkNonNull "array" array
        let mutable acc = LanguagePrimitives.GenericZero< ^U>

        for i = 0 to array.Length - 1 do
            acc <- Checked.(+) acc (projection array.[i])

        acc

    [<CompiledName("Min")>]
    let inline min (array: _ array) =
        checkNonNull "array" array

        if array.Length = 0 then
            invalidArg "array" LanguagePrimitives.ErrorStrings.InputArrayEmptyString

        let mutable acc = array.[0]

        for i = 1 to array.Length - 1 do
            let curr = array.[i]

            if curr < acc then
                acc <- curr

        acc

    [<CompiledName("MinBy")>]
    let inline minBy ([<InlineIfLambda>] projection) (array: _ array) =
        checkNonNull "array" array

        if array.Length = 0 then
            invalidArg "array" LanguagePrimitives.ErrorStrings.InputArrayEmptyString

        let mutable accv = array.[0]
        let mutable acc = projection accv

        for i = 1 to array.Length - 1 do
            let currv = array.[i]
            let curr = projection currv

            if curr < acc then
                acc <- curr
                accv <- currv

        accv

    [<CompiledName("Max")>]
    let inline max (array: _ array) =
        checkNonNull "array" array

        if array.Length = 0 then
            invalidArg "array" LanguagePrimitives.ErrorStrings.InputArrayEmptyString

        let mutable acc = array.[0]

        for i = 1 to array.Length - 1 do
            let curr = array.[i]

            if curr > acc then
                acc <- curr

        acc

    [<CompiledName("MaxBy")>]
    let inline maxBy projection (array: _ array) =
        checkNonNull "array" array

        if array.Length = 0 then
            invalidArg "array" LanguagePrimitives.ErrorStrings.InputArrayEmptyString

        let mutable accv = array.[0]
        let mutable acc = projection accv

        for i = 1 to array.Length - 1 do
            let currv = array.[i]
            let curr = projection currv

            if curr > acc then
                acc <- curr
                accv <- currv

        accv

    [<CompiledName("Average")>]
    let inline average (array: 'T array) =
        checkNonNull "array" array

        if array.Length = 0 then
            invalidArg "array" LanguagePrimitives.ErrorStrings.InputArrayEmptyString

        let mutable acc = LanguagePrimitives.GenericZero< ^T>

        for i = 0 to array.Length - 1 do
            acc <- Checked.(+) acc array.[i]

        LanguagePrimitives.DivideByInt< ^T> acc array.Length

    [<CompiledName("AverageBy")>]
    let inline averageBy ([<InlineIfLambda>] projection: 'T -> ^U) (array: 'T array) : ^U =
        checkNonNull "array" array

        if array.Length = 0 then
            invalidArg "array" LanguagePrimitives.ErrorStrings.InputArrayEmptyString

        let mutable acc = LanguagePrimitives.GenericZero< ^U>

        for i = 0 to array.Length - 1 do
            acc <- Checked.(+) acc (projection array.[i])

        LanguagePrimitives.DivideByInt< ^U> acc array.Length

    [<CompiledName("CompareWith")>]
    let inline compareWith ([<InlineIfLambda>] comparer: 'T -> 'T -> int) (array1: 'T array) (array2: 'T array) =
        checkNonNull "array1" array1
        checkNonNull "array2" array2

        let length1 = array1.Length
        let length2 = array2.Length

        let mutable i = 0
        let mutable result = 0

        if length1 < length2 then
            while i < array1.Length && result = 0 do
                result <- comparer array1.[i] array2.[i]
                i <- i + 1
        else
            while i < array2.Length && result = 0 do
                result <- comparer array1.[i] array2.[i]
                i <- i + 1

        if result <> 0 then result
        elif length1 = length2 then 0
        elif length1 < length2 then -1
        else 1

    [<CompiledName("GetSubArray")>]
    let sub (array: 'T array) (startIndex: int) (count: int) =
        checkNonNull "array" array

        if startIndex < 0 then
            invalidArgInputMustBeNonNegative "startIndex" startIndex

        if count < 0 then
            invalidArgInputMustBeNonNegative "count" count

        if startIndex + count > array.Length then
            invalidArgOutOfRange "count" count "array.Length" array.Length

        Microsoft.FSharp.Primitives.Basics.Array.subUnchecked startIndex count array

    [<CompiledName("Item")>]
    let item index (array: _ array) =
        array.[index]

    [<CompiledName("TryItem")>]
    let tryItem index (array: 'T array) =
        checkNonNull "array" array

        if index < 0 || index >= array.Length then
            None
        else
            Some(array.[index])

    [<CompiledName("Get")>]
    let get (array: _ array) index =
        array.[index]

    [<CompiledName("Set")>]
    let set (array: _ array) index value =
        array.[index] <- value

    [<CompiledName("Fill")>]
    let fill (target: 'T array) (targetIndex: int) (count: int) (value: 'T) =
        checkNonNull "target" target

        if targetIndex < 0 then
            invalidArgInputMustBeNonNegative "targetIndex" targetIndex

        if count < 0 then
            invalidArgInputMustBeNonNegative "count" count

        for i = targetIndex to targetIndex + count - 1 do
            target.[i] <- value

    [<CompiledName("ExactlyOne")>]
    let exactlyOne (array: 'T array) =
        checkNonNull "array" array

        if array.Length = 1 then
            array.[0]
        elif array.Length = 0 then
            invalidArg "array" LanguagePrimitives.ErrorStrings.InputSequenceEmptyString
        else
            invalidArg "array" (SR.GetString(SR.inputSequenceTooLong))

    [<CompiledName("TryExactlyOne")>]
    let tryExactlyOne (array: 'T array) =
        checkNonNull "array" array

        if array.Length = 1 then
            Some array.[0]
        else
            None

    let transposeArrays (array: 'T array array) =
        let len = array.Length

        if len = 0 then
            Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked 0
        else
            let lenInner = array.[0].Length

            for j in 1 .. len - 1 do
                if lenInner <> array.[j].Length then
                    invalidArgDifferentArrayLength
                        "array.[0]"
                        lenInner
                        (String.Format("array.[{0}]", j))
                        array.[j].Length

            let result: 'T array array =
                Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked lenInner

            for i in 0 .. lenInner - 1 do
                result.[i] <- Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked len

                for j in 0 .. len - 1 do
                    result.[i].[j] <- array.[j].[i]

            result

    [<CompiledName("Transpose")>]
    let transpose (arrays: seq<'T array>) =
        checkNonNull "arrays" arrays

        match arrays with
        | :? ('T array array) as ts -> ts |> transposeArrays // avoid a clone, since we only read the array
        | _ -> arrays |> Seq.toArray |> transposeArrays

    [<CompiledName("Truncate")>]
    let truncate count (array: 'T array) =
        checkNonNull "array" array

        if count <= 0 then
            empty
        else
            let len = array.Length
            let count' = Operators.min count len
            Microsoft.FSharp.Primitives.Basics.Array.subUnchecked 0 count' array

    [<CompiledName("RemoveAt")>]
    let removeAt (index: int) (source: 'T array) : 'T array =
        checkNonNull "source" source

        if index < 0 || index >= source.Length then
            invalidArg "index" "index must be within bounds of the array"

        let length = source.Length - 1
        let result = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked length

        if index > 0 then
            Array.Copy(source, result, index)

        if length - index > 0 then
            Array.Copy(source, index + 1, result, index, length - index)

        result

    [<CompiledName("RemoveManyAt")>]
    let removeManyAt (index: int) (count: int) (source: 'T array) : 'T array =
        checkNonNull "source" source

        if index < 0 || index > source.Length - count then
            invalidArg "index" "index must be within bounds of the array"

        let length = source.Length - count
        let result = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked length

        if index > 0 then
            Array.Copy(source, result, index)

        if length - index > 0 then
            Array.Copy(source, index + count, result, index, length - index)

        result

    [<CompiledName("UpdateAt")>]
    let updateAt (index: int) (value: 'T) (source: 'T array) : 'T array =
        checkNonNull "source" source

        if index < 0 || index >= source.Length then
            invalidArg "index" "index must be within bounds of the array"

        let length = source.Length
        let result = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked length

        if length > 0 then
            Array.Copy(source, result, length)

        result.[index] <- value

        result

    [<CompiledName("InsertAt")>]
    let insertAt (index: int) (value: 'T) (source: 'T array) : 'T array =
        checkNonNull "source" source

        if index < 0 || index > source.Length then
            invalidArg "index" "index must be within bounds of the array"

        let length = source.Length + 1
        let result = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked length

        if index > 0 then
            Array.Copy(source, result, index)

        result.[index] <- value

        if source.Length - index > 0 then
            Array.Copy(source, index, result, index + 1, source.Length - index)

        result

    [<CompiledName("InsertManyAt")>]
    let insertManyAt (index: int) (values: seq<'T>) (source: 'T array) : 'T array =
        checkNonNull "source" source

        if index < 0 || index > source.Length then
            invalidArg "index" "index must be within bounds of the array"

        let valuesArray = Seq.toArray values

        if valuesArray.Length = 0 then
            source
        else
            let length = source.Length + valuesArray.Length
            let result = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked length

            if index > 0 then
                Array.Copy(source, result, index)

            Array.Copy(valuesArray, 0, result, index, valuesArray.Length)

            if source.Length - index > 0 then
                Array.Copy(source, index, result, index + valuesArray.Length, source.Length - index)

            result

    [<CompiledName("RandomShuffleWith")>]
    let randomShuffleWith (random: Random) (source: 'T array) : 'T array =
        checkNonNull "random" random
        checkNonNull "source" source

        let result = copy source

        Microsoft.FSharp.Primitives.Basics.Random.shuffleArrayInPlaceWith random result

        result

    [<CompiledName("RandomShuffleBy")>]
    let randomShuffleBy (randomizer: unit -> float) (source: 'T array) : 'T array =
        checkNonNull "source" source

        let result = copy source

        Microsoft.FSharp.Primitives.Basics.Random.shuffleArrayInPlaceBy randomizer result

        result

    [<CompiledName("RandomShuffle")>]
    let randomShuffle (source: 'T array) : 'T array =
        randomShuffleWith ThreadSafeRandom.Shared source

    [<CompiledName("RandomShuffleInPlaceWith")>]
    let randomShuffleInPlaceWith (random: Random) (source: 'T array) =
        checkNonNull "random" random
        checkNonNull "source" source

        Microsoft.FSharp.Primitives.Basics.Random.shuffleArrayInPlaceWith random source

    [<CompiledName("RandomShuffleInPlaceBy")>]
    let randomShuffleInPlaceBy (randomizer: unit -> float) (source: 'T array) =
        checkNonNull "source" source

        Microsoft.FSharp.Primitives.Basics.Random.shuffleArrayInPlaceBy randomizer source

    [<CompiledName("RandomShuffleInPlace")>]
    let randomShuffleInPlace (source: 'T array) =
        randomShuffleInPlaceWith ThreadSafeRandom.Shared source

    [<CompiledName("RandomChoiceWith")>]
    let randomChoiceWith (random: Random) (source: 'T array) : 'T =
        checkNonNull "random" random
        checkNonNull "source" source

        let inputLength = source.Length

        if inputLength = 0 then
            invalidArg "source" LanguagePrimitives.ErrorStrings.InputArrayEmptyString

        let i = random.Next(0, inputLength)
        source[i]

    [<CompiledName("RandomChoiceBy")>]
    let randomChoiceBy (randomizer: unit -> float) (source: 'T array) : 'T =
        checkNonNull "source" source

        let inputLength = source.Length

        if inputLength = 0 then
            invalidArg "source" LanguagePrimitives.ErrorStrings.InputArrayEmptyString

        let i = Microsoft.FSharp.Primitives.Basics.Random.next randomizer 0 inputLength
        source[i]

    [<CompiledName("RandomChoice")>]
    let randomChoice (source: 'T array) : 'T =
        randomChoiceWith ThreadSafeRandom.Shared source

    [<CompiledName("RandomChoicesWith")>]
    let randomChoicesWith (random: Random) (count: int) (source: 'T array) : 'T array =
        checkNonNull "random" random
        checkNonNull "source" source

        if count < 0 then
            invalidArgInputMustBeNonNegative "count" count

        let inputLength = source.Length

        if inputLength = 0 then
            invalidArg "source" LanguagePrimitives.ErrorStrings.InputArrayEmptyString

        let result = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked count

        for i = 0 to count - 1 do
            let j = random.Next(0, inputLength)
            result[i] <- source[j]

        result

    [<CompiledName("RandomChoicesBy")>]
    let randomChoicesBy (randomizer: unit -> float) (count: int) (source: 'T array) : 'T array =
        checkNonNull "source" source

        if count < 0 then
            invalidArgInputMustBeNonNegative "count" count

        let inputLength = source.Length

        if inputLength = 0 then
            invalidArg "source" LanguagePrimitives.ErrorStrings.InputArrayEmptyString

        let result = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked count

        for i = 0 to count - 1 do
            let j = Microsoft.FSharp.Primitives.Basics.Random.next randomizer 0 inputLength
            result[i] <- source[j]

        result

    [<CompiledName("RandomChoices")>]
    let randomChoices (count: int) (source: 'T array) : 'T array =
        randomChoicesWith ThreadSafeRandom.Shared count source

    [<CompiledName("RandomSampleWith")>]
    let randomSampleWith (random: Random) (count: int) (source: 'T array) : 'T array =
        checkNonNull "random" random
        checkNonNull "source" source

        if count < 0 then
            invalidArgInputMustBeNonNegative "count" count

        let inputLength = source.Length

        if inputLength = 0 then
            invalidArg "source" LanguagePrimitives.ErrorStrings.InputArrayEmptyString

        if count > inputLength then
            invalidArg "count" (SR.GetString(SR.notEnoughElements))

        let result = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked count

        let setSize =
            Microsoft.FSharp.Primitives.Basics.Random.getMaxSetSizeForSampling count

        if inputLength <= setSize then
            let pool = copy source

            for i = 0 to count - 1 do
                let j = random.Next(0, inputLength - i)
                result[i] <- pool[j]
                pool[j] <- pool[inputLength - i - 1]
        else
            let selected = HashSet()

            for i = 0 to count - 1 do
                let mutable j = random.Next(0, inputLength)

                while not (selected.Add j) do
                    j <- random.Next(0, inputLength)

                result[i] <- source[j]

        result

    [<CompiledName("RandomSampleBy")>]
    let randomSampleBy (randomizer: unit -> float) (count: int) (source: 'T array) : 'T array =
        checkNonNull "source" source

        if count < 0 then
            invalidArgInputMustBeNonNegative "count" count

        let inputLength = source.Length

        if inputLength = 0 then
            invalidArg "source" LanguagePrimitives.ErrorStrings.InputArrayEmptyString

        if count > inputLength then
            invalidArg "count" (SR.GetString(SR.notEnoughElements))

        let result = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked count

        // algorithm taken from https://github.com/python/cpython/blob/69b3e8ea569faabccd74036e3d0e5ec7c0c62a20/Lib/random.py#L363-L456
        let setSize =
            Microsoft.FSharp.Primitives.Basics.Random.getMaxSetSizeForSampling count

        if inputLength <= setSize then
            let pool = copy source

            for i = 0 to count - 1 do
                let j =
                    Microsoft.FSharp.Primitives.Basics.Random.next randomizer 0 (inputLength - i)

                result[i] <- pool[j]
                pool[j] <- pool[inputLength - i - 1]
        else
            let selected = HashSet()

            for i = 0 to count - 1 do
                let mutable j =
                    Microsoft.FSharp.Primitives.Basics.Random.next randomizer 0 inputLength

                while not (selected.Add j) do
                    j <- Microsoft.FSharp.Primitives.Basics.Random.next randomizer 0 inputLength

                result[i] <- source[j]

        result

    [<CompiledName("RandomSample")>]
    let randomSample (count: int) (source: 'T array) : 'T array =
        randomSampleWith ThreadSafeRandom.Shared count source

    module Parallel =
        open System.Threading
        open System.Threading.Tasks
        open System.Collections.Concurrent

        [<CompiledName("Exists")>]
        let exists (predicate: 'T -> bool) (array: 'T array) =
            checkNonNull "array" array

            Parallel
                .For(
                    0,
                    array.Length,
                    (fun i pState ->
                        if predicate array[i] then
                            pState.Stop())
                )
                .IsCompleted
            |> not

        [<CompiledName("ForAll")>]
        let forall (predicate: 'T -> bool) (array: 'T array) =
            // Not exists $condition <==> (opposite of $condition is true forall)
            exists (predicate >> not) array |> not

        let inline tryFindIndexAux predicate (array: _ array) =
            checkNonNull (nameof array) array

            let pResult =
                Parallel.For(
                    0,
                    array.Length,
                    (fun i pState ->
                        if predicate array[i] then
                            pState.Break())
                )

            pResult.LowestBreakIteration

        [<CompiledName("TryFindIndex")>]
        let tryFindIndex predicate (array: _ array) =
            let i = tryFindIndexAux predicate array
            if i.HasValue then Some (int (i.GetValueOrDefault()))
            else None

        [<CompiledName("TryFind")>]
        let tryFind predicate (array: _ array) =
            let i = tryFindIndexAux predicate array
            if i.HasValue then Some array[int (i.GetValueOrDefault())]
            else None

        [<CompiledName("TryPick")>]
        let tryPick chooser (array: _ array) =
            checkNonNull "array" array
            let allChosen = System.Collections.Concurrent.ConcurrentDictionary()

            let pResult =
                Parallel.For(
                    0,
                    array.Length,
                    (fun i pState ->
                        match chooser array[i] with
                        | None -> ()
                        | chosenElement ->
                            allChosen[i] <- chosenElement
                            pState.Break())
                )

            if pResult.LowestBreakIteration.HasValue then allChosen[int (pResult.LowestBreakIteration.GetValueOrDefault())]
            else None

        [<CompiledName("Choose")>]
        let choose chooser (array: 'T array) =
            checkNonNull "array" array
            let inputLength = array.Length

            let isChosen: bool array =
                Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked inputLength

            let results: 'U array =
                Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked inputLength

            let mutable outputLength = 0

            Parallel.For(
                0,
                inputLength,
                (fun () -> 0),
                (fun i _ count ->
                    match chooser array.[i] with
                    | None -> count
                    | Some v ->
                        isChosen.[i] <- true
                        results.[i] <- v
                        count + 1),
                Action<int>(fun x -> System.Threading.Interlocked.Add(&outputLength, x) |> ignore)
            )
            |> ignore

            let output =
                Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked outputLength

            let mutable curr = 0

            for i = 0 to isChosen.Length - 1 do
                if isChosen.[i] then
                    output.[curr] <- results.[i]
                    curr <- curr + 1

            output

        [<CompiledName("Collect")>]
        let collect (mapping: 'T -> 'U array) (array: 'T array) : 'U array =
            checkNonNull "array" array
            let inputLength = array.Length

            let result =
                Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked inputLength

            Parallel.For(0, inputLength, (fun i -> result.[i] <- mapping array.[i]))
            |> ignore

            concatArrays result

        [<CompiledName("Map")>]
        let map (mapping: 'T -> 'U) (array: 'T array) : 'U array =
            checkNonNull "array" array
            let inputLength = array.Length

            let result =
                Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked inputLength

            Parallel.For(0, inputLength, (fun i -> result.[i] <- mapping array.[i]))
            |> ignore

            result

        [<CompiledName("MapIndexed")>]
        let mapi mapping (array: 'T array) =
            checkNonNull "array" array
            let f = OptimizedClosures.FSharpFunc<_, _, _>.Adapt(mapping)
            let inputLength = array.Length

            let result =
                Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked inputLength

            Parallel.For(0, inputLength, (fun i -> result.[i] <- f.Invoke(i, array.[i])))
            |> ignore

            result

        // The following two parameters were benchmarked and found to be optimal.
        // Benchmark was run using: 11th Gen Intel Core i9-11950H 2.60GHz, 1 CPU, 16 logical and 8 physical cores
        let maxPartitions = Environment.ProcessorCount // The maximum number of partitions to use
        let minChunkSize = 256 // The minimum size of a chunk to be sorted in parallel

        let createPartitionsUpToWithMinChunkSize maxIdxExclusive minChunkSize (array: 'T array) =
            [|
                let chunkSize =
                    match maxIdxExclusive with
                    | smallSize when smallSize < minChunkSize -> smallSize
                    | biggerSize when biggerSize % maxPartitions = 0 -> biggerSize / maxPartitions
                    | biggerSize -> (biggerSize / maxPartitions) + 1

                let mutable offset = 0

                while (offset + chunkSize) < maxIdxExclusive do
                    yield new ArraySegment<'T>(array, offset, chunkSize)
                    offset <- offset + chunkSize

                yield new ArraySegment<'T>(array, offset, maxIdxExclusive - offset)
            |]

        let createPartitionsUpTo maxIdxExclusive (array: 'T array) =
            createPartitionsUpToWithMinChunkSize maxIdxExclusive minChunkSize array

        (* This function is there also as a support vehicle for other aggregations. 
           It is public in order to be called from inlined functions, the benefit of inlining call into it is significant *)
        [<CompiledName("ReduceBy")>]
        let reduceBy (projection: 'T -> 'U) (reduction: 'U -> 'U -> 'U) (array: 'T array) =
            checkNonNull "array" array

            if array.Length = 0 then
                invalidArg "array" LanguagePrimitives.ErrorStrings.InputArrayEmptyString

            let chunks = createPartitionsUpToWithMinChunkSize array.Length 2 array // We need at least 2 elements/chunk for 'reduction'

            let chunkResults =
                Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked chunks.Length

            Parallel.For(
                0,
                chunks.Length,
                fun chunkIdx ->
                    let chunk = chunks[chunkIdx]
                    let mutable res = projection array[chunk.Offset]
                    let lastIdx = chunk.Offset + chunk.Count - 1

                    for i = chunk.Offset + 1 to lastIdx do
                        let projected = projection array[i]
                        res <- reduction res projected

                    chunkResults[chunkIdx] <- res
            )
            |> ignore

            let mutable finalResult = chunkResults[0]

            for i = 1 to chunkResults.Length - 1 do
                finalResult <- reduction finalResult chunkResults[i]

            finalResult

        [<CompiledName("Reduce")>]
        let inline reduce ([<InlineIfLambda>] reduction) (array: _ array) =
            array |> reduceBy id reduction

        let inline vFst struct (a, _) =
            a

        let inline vSnd struct (_, b) =
            b

        [<CompiledName("MinBy")>]
        let inline minBy ([<InlineIfLambda>] projection) (array: _ array) =

            array
            |> reduceBy (fun x -> struct (projection x, x)) (fun a b -> if vFst a < vFst b then a else b)
            |> vSnd

        [<CompiledName("Min")>]
        let inline min (array: _ array) =
            array |> reduce (fun a b -> if a < b then a else b)

        [<CompiledName("SumBy")>]
        let inline sumBy ([<InlineIfLambda>] projection: 'T -> ^U) (array: 'T array) : ^U =
            if array.Length = 0 then
                LanguagePrimitives.GenericZero
            else
                array |> reduceBy projection Operators.Checked.(+)

        [<CompiledName("Sum")>]
        let inline sum (array: ^T array) : ^T =
            array |> sumBy id

        [<CompiledName("MaxBy")>]
        let inline maxBy projection (array: _ array) =

            array
            |> reduceBy (fun x -> struct (projection x, x)) (fun a b -> if vFst a > vFst b then a else b)
            |> vSnd

        [<CompiledName("Max")>]
        let inline max (array: _ array) =
            array |> reduce (fun a b -> if a > b then a else b)

        [<CompiledName("AverageBy")>]
        let inline averageBy ([<InlineIfLambda>] projection: 'T -> ^U) (array: 'T array) : ^U =
            let sum = array |> reduceBy projection Operators.Checked.(+)
            LanguagePrimitives.DivideByInt sum (array.Length)

        [<CompiledName("Average")>]
        let inline average (array: 'T array) =
            array |> averageBy id

        [<CompiledName("Zip")>]
        let zip (array1: _ array) (array2: _ array) =
            checkNonNull "array1" array1
            checkNonNull "array2" array2
            let len1 = array1.Length

            if len1 <> array2.Length then
                invalidArgDifferentArrayLength "array1" len1 "array2" array2.Length

            let res = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked len1
            let inputChunks = createPartitionsUpTo array1.Length array1

            Parallel.For(
                0,
                inputChunks.Length,
                fun chunkIdx ->
                    let chunk = inputChunks[chunkIdx]

                    for elemIdx = chunk.Offset to (chunk.Offset + chunk.Count - 1) do
                        res[elemIdx] <- (array1[elemIdx], array2[elemIdx])
            )
            |> ignore

            res

        let inline groupByImplParallel
            (comparer: IEqualityComparer<'SafeKey>)
            ([<InlineIfLambda>] keyf: 'T -> 'SafeKey)
            ([<InlineIfLambda>] getKey: 'SafeKey -> 'Key)
            (array: 'T array)
            =
            let counts =
                new ConcurrentDictionary<_, _>(
                    concurrencyLevel = maxPartitions,
                    capacity = Operators.min (array.Length) 1_000,
                    comparer = comparer
                )

            let valueFactory = new Func<_, _>(fun _ -> ref 0)

            let projectedValues =
                Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked array.Length

            let inputChunks = createPartitionsUpTo array.Length array

            Parallel.For(
                0,
                inputChunks.Length,
                fun chunkIdx ->
                    let chunk = inputChunks[chunkIdx]

                    for elemIdx = chunk.Offset to (chunk.Offset + chunk.Count - 1) do
                        let projected = keyf array[elemIdx]
                        projectedValues[elemIdx] <- projected
                        let counter = counts.GetOrAdd(projected, valueFactory = valueFactory)
                        Interlocked.Increment(counter) |> ignore
            )
            |> ignore

            let finalResults =
                Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked counts.Count

            let mutable finalIdx = 0

            let finalResultsLookup =
                new Dictionary<'SafeKey, int ref * 'T array>(capacity = counts.Count, comparer = comparer)

            for kvp in counts do
                let arrayForThisGroup =
                    Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked kvp.Value.Value

                finalResults.[finalIdx] <- getKey kvp.Key, arrayForThisGroup
                finalResultsLookup[kvp.Key] <- kvp.Value, arrayForThisGroup
                finalIdx <- finalIdx + 1

            Parallel.For(
                0,
                inputChunks.Length,
                fun chunkIdx ->
                    let chunk = inputChunks[chunkIdx]

                    for elemIdx = chunk.Offset to (chunk.Offset + chunk.Count - 1) do
                        let key = projectedValues[elemIdx]
                        let (counter, arrayForThisGroup) = finalResultsLookup[key]
                        let idxToWrite = Interlocked.Decrement(counter)
                        arrayForThisGroup[idxToWrite] <- array[elemIdx]
            )
            |> ignore

            finalResults

        let groupByValueTypeParallel (keyf: 'T -> 'Key) (array: 'T array) =
            // Is it a bad idea to put floating points as keys for grouping? Yes
            // But would the implementation fail with KeyNotFound "nan" if we just leave it? Also yes
            // Here we  enforce nan=nan equality to prevent throwing
            if typeof<'Key> = typeof<float> || typeof<'Key> = typeof<float32> then
                let genericCmp =
                    HashIdentity.FromFunctions<'Key>
                        (LanguagePrimitives.GenericHash)
                        (LanguagePrimitives.GenericEqualityER)

                groupByImplParallel genericCmp keyf id array
            else
                groupByImplParallel HashIdentity.Structural<'Key> keyf id array

        // Just like in regular Array.groupBy: Wrap a StructBox around all keys in order to avoid nulls
        // (dotnet doesn't allow null keys in dictionaries)
        let groupByRefTypeParallel (keyf: 'T -> 'Key) (array: 'T array) =
            groupByImplParallel
                RuntimeHelpers.StructBox<'Key>.Comparer
                (keyf >> RuntimeHelpers.StructBox)
                (fun sb -> sb.Value)
                array

        [<CompiledName("GroupBy")>]
        let groupBy (projection: 'T -> 'Key) (array: 'T array) =
            checkNonNull "array" array

            if typeof<'Key>.IsValueType then
                groupByValueTypeParallel projection array
            else
                groupByRefTypeParallel projection array

        [<CompiledName("Iterate")>]
        let iter action (array: 'T array) =
            checkNonNull "array" array
            Parallel.For(0, array.Length, (fun i -> action array.[i])) |> ignore

        [<CompiledName("IterateIndexed")>]
        let iteri action (array: 'T array) =
            checkNonNull "array" array
            let f = OptimizedClosures.FSharpFunc<_, _, _>.Adapt(action)
            Parallel.For(0, array.Length, (fun i -> f.Invoke(i, array.[i]))) |> ignore

        [<CompiledName("Initialize")>]
        let init count initializer =
            let result = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked count
            Parallel.For(0, count, (fun i -> result.[i] <- initializer i)) |> ignore
            result

        let countAndCollectTrueItems predicate (array: 'T array) =
            checkNonNull "array" array
            let inputLength = array.Length

            let isTrue =
                Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked inputLength

            let mutable trueLength = 0

            Parallel.For(
                0,
                inputLength,
                (fun () -> 0),
                (fun i _ trueCount ->
                    if predicate array.[i] then
                        isTrue.[i] <- true
                        trueCount + 1
                    else
                        trueCount),
                Action<int>(fun x -> System.Threading.Interlocked.Add(&trueLength, x) |> ignore)
            )
            |> ignore

            trueLength, isTrue

        [<CompiledName("Filter")>]
        let filter predicate (array: 'T array) =
            let trueLength, isTrue = countAndCollectTrueItems predicate array
            let res = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked trueLength
            let mutable resIdx = 0

            for i = 0 to isTrue.Length - 1 do
                if isTrue.[i] then
                    res.[resIdx] <- array.[i]
                    resIdx <- resIdx + 1

            res

        [<CompiledName("Partition")>]
        let partition predicate (array: 'T array) =
            let trueLength, isTrue = countAndCollectTrueItems predicate array
            let res1 = Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked trueLength

            let res2 =
                Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked (array.Length - trueLength)

            let mutable iTrue = 0
            let mutable iFalse = 0

            for i = 0 to isTrue.Length - 1 do
                if isTrue.[i] then
                    res1.[iTrue] <- array.[i]
                    iTrue <- iTrue + 1
                else
                    res2.[iFalse] <- array.[i]
                    iFalse <- iFalse + 1

            res1, res2

        let private createPartitions (array: 'T array) =
            createPartitionsUpTo array.Length array

        let inline pickPivot
            ([<InlineIfLambda>] cmpAtIndex: int -> int -> int)
            ([<InlineIfLambda>] swapAtIndex: int -> int -> unit)
            (orig: ArraySegment<'T>)
            =
            let inline swapIfGreater (i: int) (j: int) =
                if cmpAtIndex i j > 0 then
                    swapAtIndex i j

            // Set pivot to be a median of {first,mid,last}

            let firstIdx = orig.Offset
            let lastIDx = orig.Offset + orig.Count - 1
            let midIdx = orig.Offset + orig.Count / 2

            swapIfGreater firstIdx midIdx
            swapIfGreater firstIdx lastIDx
            swapIfGreater midIdx lastIDx
            midIdx

        let inline partitionIntoTwo
            ([<InlineIfLambda>] cmpWithPivot: int -> int)
            ([<InlineIfLambda>] swapAtIndex: int -> int -> unit)
            (orig: ArraySegment<'T>)
            =
            let mutable leftIdx = orig.Offset + 1 // Leftmost is already < pivot
            let mutable rightIdx = orig.Offset + orig.Count - 2 // Rightmost is already > pivot

            while leftIdx < rightIdx do
                while cmpWithPivot leftIdx < 0 do
                    leftIdx <- leftIdx + 1

                while cmpWithPivot rightIdx > 0 do
                    rightIdx <- rightIdx - 1

                if leftIdx < rightIdx then
                    swapAtIndex leftIdx rightIdx
                    leftIdx <- leftIdx + 1
                    rightIdx <- rightIdx - 1

            let lastIdx = orig.Offset + orig.Count - 1
            // There might be more elements being (=)pivot. Exclude them from further work
            while cmpWithPivot leftIdx >= 0 && leftIdx > orig.Offset do
                leftIdx <- leftIdx - 1

            while cmpWithPivot rightIdx <= 0 && rightIdx < lastIdx do
                rightIdx <- rightIdx + 1

            new ArraySegment<_>(orig.Array, offset = orig.Offset, count = leftIdx - orig.Offset + 1),
            new ArraySegment<_>(orig.Array, offset = rightIdx, count = lastIdx - rightIdx + 1)

        let partitionIntoTwoUsingComparer
            (cmp: 'T -> 'T -> int)
            (orig: ArraySegment<'T>)
            : ArraySegment<'T> * ArraySegment<'T> =
            let array = orig.Array

            let inline swap i j =
                let tmp = array[i]
                array[i] <- array[j]
                array[j] <- tmp

            let pivotIdx = pickPivot (fun i j -> cmp array[i] array[j]) swap orig

            let pivotItem = array[pivotIdx]
            partitionIntoTwo (fun idx -> cmp array[idx] pivotItem) swap orig

        let partitionIntoTwoUsingKeys (keys: 'A array) (orig: ArraySegment<'T>) : ArraySegment<'T> * ArraySegment<'T> =
            let array = orig.Array

            let inline swap i j =
                let tmpKey = keys[i]
                keys[i] <- keys[j]
                keys[j] <- tmpKey

                let tmp = array.[i]
                array.[i] <- array.[j]
                array.[j] <- tmp

            let pivotIdx = pickPivot (fun i j -> compare keys[i] keys[j]) swap orig

            let pivotKey = keys[pivotIdx]
            partitionIntoTwo (fun idx -> compare keys[idx] pivotKey) swap orig

        let inline sortInPlaceHelper
            (array: 'T array)
            ([<InlineIfLambda>] partitioningFunc: ArraySegment<'T> -> ArraySegment<'T> * ArraySegment<'T>)
            ([<InlineIfLambda>] sortingFunc: ArraySegment<'T> -> unit)
            =
            let rec sortChunk (segment: ArraySegment<_>) freeWorkers =
                match freeWorkers with
                // Really small arrays are not worth creating a Task for, sort them immediately as well
                | 0
                | 1 -> sortingFunc segment
                | _ when segment.Count <= minChunkSize -> sortingFunc segment
                | _ ->
                    let left, right = partitioningFunc segment
                    // If either of the two is too small, sort small segments straight away.
                    // If the other happens to be big, leave it with all works in its recursive step
                    if left.Count <= minChunkSize || right.Count <= minChunkSize then
                        sortChunk left freeWorkers
                        sortChunk right freeWorkers
                    else
                        // Pivot-based partitions might be unbalanced. Split  free workers for left/right proportional to their size
                        let itemsPerWorker = Operators.max ((left.Count + right.Count) / freeWorkers) 1

                        let workersForLeftTask =
                            (left.Count / itemsPerWorker)
                            |> Operators.max 1
                            |> Operators.min (freeWorkers - 1)

                        let leftTask = Task.Run(fun () -> sortChunk left workersForLeftTask)
                        sortChunk right (freeWorkers - workersForLeftTask)
                        leftTask.Wait()

            let bigSegment = new ArraySegment<_>(array, 0, array.Length)
            sortChunk bigSegment maxPartitions

        let sortInPlaceWithHelper
            (partitioningComparer: 'T -> 'T -> int)
            (sortingComparer: IComparer<'T>)
            (inputArray: 'T array)
            =
            let partitioningFunc = partitionIntoTwoUsingComparer partitioningComparer

            let sortingFunc =
                fun (s: ArraySegment<'T>) -> Array.Sort<'T>(inputArray, s.Offset, s.Count, sortingComparer)

            sortInPlaceHelper inputArray partitioningFunc sortingFunc

        let sortKeysAndValuesInPlace (inputKeys: 'TKey array) (values: 'TValue array) =
            let partitioningFunc = partitionIntoTwoUsingKeys inputKeys
            let sortingComparer = LanguagePrimitives.FastGenericComparerCanBeNull<'TKey>

            let sortingFunc =
                fun (s: ArraySegment<'T>) ->
                    Array.Sort<'TKey, 'TValue>(inputKeys, values, s.Offset, s.Count, sortingComparer)

            sortInPlaceHelper values partitioningFunc sortingFunc

        [<CompiledName("SortInPlaceWith")>]
        let sortInPlaceWith comparer (array: 'T array) =
            checkNonNull "array" array
            let sortingComparer = ComparisonIdentity.FromFunction(comparer)
            sortInPlaceWithHelper comparer sortingComparer array

        [<CompiledName("SortInPlaceBy")>]
        let sortInPlaceBy (projection: 'T -> 'U) (array: 'T array) =
            checkNonNull "array" array

            let inputKeys: 'U array =
                Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked array.Length

            let partitions = createPartitions array

            Parallel.For(
                0,
                partitions.Length,
                fun i ->
                    let segment = partitions.[i]

                    for idx = segment.Offset to (segment.Offset + segment.Count - 1) do
                        inputKeys[idx] <- projection array[idx]
            )
            |> ignore

            sortKeysAndValuesInPlace inputKeys array

        [<CompiledName("SortInPlace")>]
        let sortInPlace (array: 'T array) =
            checkNonNull "array" array

            let sortingComparer: IComparer<'T> =
                LanguagePrimitives.FastGenericComparerCanBeNull<'T>

            let partitioningFunc = compare
            sortInPlaceWithHelper partitioningFunc sortingComparer array

        [<CompiledName("SortWith")>]
        let sortWith (comparer: 'T -> 'T -> int) (array: 'T array) =
            let result = copy array
            sortInPlaceWith comparer result
            result

        [<CompiledName("SortBy")>]
        let sortBy projection (array: 'T array) =
            checkNonNull "array" array

            let inputKeys =
                Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked array.Length

            let clone =
                Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked array.Length

            let partitions = createPartitions clone

            Parallel.For(
                0,
                partitions.Length,
                fun i ->
                    let segment = partitions.[i]

                    for idx = segment.Offset to (segment.Offset + segment.Count - 1) do
                        clone[idx] <- array[idx]
                        inputKeys.[idx] <- projection array[idx]
            )
            |> ignore

            sortKeysAndValuesInPlace inputKeys clone
            clone

        [<CompiledName("Sort")>]
        let sort array =
            let result = copy array
            sortInPlace result
            result

        let reverseInPlace (array: 'T array) =
            let segments = createPartitionsUpTo (array.Length / 2) array
            let lastIdx = array.Length - 1

            Parallel.For(
                0,
                segments.Length,
                fun idx ->
                    let s = segments[idx]

                    for i = s.Offset to (s.Offset + s.Count - 1) do
                        let tmp = array[i]
                        array[i] <- array[lastIdx - i]
                        array[lastIdx - i] <- tmp
            )
            |> ignore

            array

        [<CompiledName("SortByDescending")>]
        let sortByDescending projection array =
            array |> sortBy projection |> reverseInPlace

        [<CompiledName("SortDescending")>]
        let sortDescending array =
            array |> sort |> reverseInPlace
