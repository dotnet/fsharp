// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Collections

open System
open System.Collections.Generic
open System.ComponentModel
open System.IO
open System.Diagnostics
open Microsoft.FSharp.Core
open Microsoft.FSharp.Collections
open Microsoft.FSharp.Core.CompilerServices


[<AutoOpen>]
module CollectionExtensions =

    [<CompiledName("CreateSet")>]
    let inline set elements =
        Set.ofSeq elements

    let dummyArray = [||]

    let inline dont_tail_call f =
        let result = f ()
        dummyArray.Length |> ignore // pretty stupid way to avoid tail call, would be better if attribute existed, but this should be inlineable by the JIT
        result

    let inline ICollection_Contains<'collection, 'item when 'collection :> ICollection<'item>>
        (collection: 'collection)
        (item: 'item)
        =
        collection.Contains item

    [<DebuggerDisplay("Count = {Count}")>]
    [<DebuggerTypeProxy(typedefof<DictDebugView<_, _, _>>)>]
    type DictImpl<'SafeKey, 'Key, 'T>
        (t: Dictionary<'SafeKey, 'T>, makeSafeKey: 'Key -> 'SafeKey, getKey: 'SafeKey -> 'Key) =

        member _.Count = t.Count

        // Give a read-only view of the dictionary
        interface IDictionary<'Key, 'T> with
            member _.Item
                with get x = dont_tail_call (fun () -> t.[makeSafeKey x])
                and set _ _ = raise (NotSupportedException(SR.GetString(SR.thisValueCannotBeMutated)))

            member _.Keys =
                let keys = t.Keys

                { new ICollection<'Key> with
                    member _.Add(x) =
                        raise (NotSupportedException(SR.GetString(SR.thisValueCannotBeMutated)))

                    member _.Clear() =
                        raise (NotSupportedException(SR.GetString(SR.thisValueCannotBeMutated)))

                    member _.Remove(x) =
                        raise (NotSupportedException(SR.GetString(SR.thisValueCannotBeMutated)))

                    member _.Contains(x) =
                        t.ContainsKey(makeSafeKey x)

                    member _.CopyTo(arr, i) =
                        let mutable n = 0

                        for k in keys do
                            arr.[i + n] <- getKey k
                            n <- n + 1

                    member _.IsReadOnly = true

                    member _.Count = keys.Count
                  interface IEnumerable<'Key> with
                      member _.GetEnumerator() =
                          (keys |> Seq.map getKey).GetEnumerator()
                  interface System.Collections.IEnumerable with
                      member _.GetEnumerator() =
                          ((keys |> Seq.map getKey) :> System.Collections.IEnumerable).GetEnumerator()
                }

            member _.Values = upcast t.Values

            member _.Add(_, _) =
                raise (NotSupportedException(SR.GetString(SR.thisValueCannotBeMutated)))

            member _.ContainsKey(k) =
                dont_tail_call (fun () -> t.ContainsKey(makeSafeKey k))

            member _.TryGetValue(k, r) =
                let safeKey = makeSafeKey k

                match t.TryGetValue safeKey with
                | true, tsafe ->
                    (r <- tsafe
                     true)
                | false, _ -> false

            member _.Remove(_: 'Key) =
                (raise (NotSupportedException(SR.GetString(SR.thisValueCannotBeMutated))): bool)

        interface IReadOnlyDictionary<'Key, 'T> with

            member _.Item
                with get key = t.[makeSafeKey key]

            member _.Keys = t.Keys |> Seq.map getKey

            member _.TryGetValue(key, r) =
                match t.TryGetValue(makeSafeKey key) with
                | false, _ -> false
                | true, value ->
                    r <- value
                    true

            member _.Values = (t :> IReadOnlyDictionary<_, _>).Values

            member _.ContainsKey k =
                t.ContainsKey(makeSafeKey k)

        interface ICollection<KeyValuePair<'Key, 'T>> with

            member _.Add(_) =
                raise (NotSupportedException(SR.GetString(SR.thisValueCannotBeMutated)))

            member _.Clear() =
                raise (NotSupportedException(SR.GetString(SR.thisValueCannotBeMutated)))

            member _.Remove(_) =
                raise (NotSupportedException(SR.GetString(SR.thisValueCannotBeMutated)))

            member _.Contains(KeyValue(k, v)) =
                ICollection_Contains t (KeyValuePair<_, _>(makeSafeKey k, v))

            member _.CopyTo(arr, i) =
                let mutable n = 0

                for (KeyValue(k, v)) in t do
                    arr.[i + n] <- KeyValuePair<_, _>(getKey k, v)
                    n <- n + 1

            member _.IsReadOnly = true

            member _.Count = t.Count

        interface IReadOnlyCollection<KeyValuePair<'Key, 'T>> with
            member _.Count = t.Count

        interface IEnumerable<KeyValuePair<'Key, 'T>> with

            member _.GetEnumerator() =
                // We use an array comprehension here instead of seq {} as otherwise we get incorrect
                // IEnumerator.Reset() and IEnumerator.Current semantics.
                let kvps = [| for (KeyValue(k, v)) in t -> KeyValuePair(getKey k, v) |] :> seq<_>
                kvps.GetEnumerator()

        interface System.Collections.IEnumerable with
            member _.GetEnumerator() =
                // We use an array comprehension here instead of seq {} as otherwise we get incorrect
                // IEnumerator.Reset() and IEnumerator.Current semantics.
                let kvps =
                    [| for (KeyValue(k, v)) in t -> KeyValuePair(getKey k, v) |] :> System.Collections.IEnumerable

                kvps.GetEnumerator()

    and DictDebugView<'SafeKey, 'Key, 'T>(d: DictImpl<'SafeKey, 'Key, 'T>) =
        [<DebuggerBrowsable(DebuggerBrowsableState.RootHidden)>]
        member _.Items = Array.ofSeq d

    let inline dictImpl
        (comparer: IEqualityComparer<'SafeKey>)
        (makeSafeKey: 'Key -> 'SafeKey)
        (getKey: 'SafeKey -> 'Key)
        (l: seq<'Key * 'T>)
        =
        let t = Dictionary comparer

        for (k, v) in l do
            t.[makeSafeKey k] <- v

        DictImpl(t, makeSafeKey, getKey)

    // We avoid wrapping a StructBox, because under 64 JIT we get some "hard" tailcalls which affect performance
    let dictValueType (l: seq<'Key * 'T>) =
        dictImpl HashIdentity.Structural<'Key> id id l

    // Wrap a StructBox around all keys in case the key type is itself a type using null as a representation
    let dictRefType (l: seq<'Key * 'T>) =
        dictImpl RuntimeHelpers.StructBox<'Key>.Comparer (RuntimeHelpers.StructBox) (fun sb -> sb.Value) l

    [<CompiledName("CreateDictionary")>]
    let dict (keyValuePairs: seq<'Key * 'T>) : IDictionary<'Key, 'T> =
        if typeof<'Key>.IsValueType then
            dictValueType keyValuePairs
        else
            dictRefType keyValuePairs

    [<CompiledName("CreateReadOnlyDictionary")>]
    let readOnlyDict (keyValuePairs: seq<'Key * 'T>) : IReadOnlyDictionary<'Key, 'T> =
        if typeof<'Key>.IsValueType then
            dictValueType keyValuePairs
        else
            dictRefType keyValuePairs

    let inline checkNonNullNullArg argName arg =
        match box arg with
        | null -> nullArg argName
        | _ -> ()

    let inline checkNonNullInvalidArg argName message arg =
        match box arg with
        | null -> invalidArg argName message
        | _ -> ()

    let getArray (vals: seq<'T>) =
        match vals with
        | :? ('T array) as arr -> arr
        | _ -> Seq.toArray vals

    [<CompiledName("CreateArray2D")>]
    let array2D (rows: seq<#seq<'T>>) =
        checkNonNullNullArg "rows" rows
        let rowsArr = getArray rows
        let m = rowsArr.Length

        if m = 0 then
            Array2D.zeroCreate 0 0
        else
            checkNonNullInvalidArg "rows" (SR.GetString(SR.nullsNotAllowedInArray)) rowsArr.[0]
            let firstRowArr = getArray rowsArr.[0]
            let n = firstRowArr.Length
            let res = Array2D.zeroCreate m n

            for j in 0 .. (n - 1) do
                res.[0, j] <- firstRowArr.[j]

            for i in 1 .. (m - 1) do
                checkNonNullInvalidArg "rows" (SR.GetString(SR.nullsNotAllowedInArray)) rowsArr.[i]
                let rowiArr = getArray rowsArr.[i]

                if rowiArr.Length <> n then
                    invalidArg "vals" (SR.GetString(SR.arraysHadDifferentLengths))

                for j in 0 .. (n - 1) do
                    res.[i, j] <- rowiArr.[j]

            res

