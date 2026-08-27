// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Core

open System
open System.Text
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
open Microsoft.FSharp.Core.Operators
open Microsoft.FSharp.Core.Operators.Checked
open Microsoft.FSharp.Collections
open Microsoft.FSharp.NativeInterop
open Microsoft.FSharp.Primitives.Basics

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module String =

    /// LOH threshold is calculated from Internal.Utilities.Library.LOH_SIZE_THRESHOLD_BYTES,
    /// and is equal to 80_000 / sizeof<char>
    [<Literal>]
    let LOH_CHAR_THRESHOLD = 40_000
    
#if NETSTANDARD2_1_OR_GREATER
    [<Literal>]
    let STACKALLOC_THRESHOLD = 512
#endif
    
    [<CompiledName("Length")>]
    let length (str: string) =
        if isNull str then 0 else str.Length

    [<CompiledName("Concat")>]
    let concat sep (strings: seq<string>) =

        let concatArray sep (strings: string array) =
            match length sep with
            | 0 -> String.Concat strings
            // following line should be used when this overload becomes part of .NET Standard (it's only in .NET Core)
            //| 1 -> String.Join(sep.[0], strings, 0, strings.Length)
            | _ -> String.Join(sep, strings, 0, strings.Length)

        match strings with
        | :? (string array) as arr -> concatArray sep arr

        | :? (string list) as lst -> lst |> List.toArray |> concatArray sep

        | _ -> String.Join(sep, strings)

    [<CompiledName("Iterate")>]
    let iter (action: char -> unit) (str: string) =
        if not (String.IsNullOrEmpty str) then
            for i = 0 to str.Length - 1 do
                action str.[i]

    [<CompiledName("IterateIndexed")>]
    let iteri action (str: string) =
        if not (String.IsNullOrEmpty str) then
            let f = OptimizedClosures.FSharpFunc<_, _, _>.Adapt(action)

            for i = 0 to str.Length - 1 do
                f.Invoke(i, str.[i])

#if NETSTANDARD2_1_OR_GREATER
    // Cache SpanAction instance to avoid allocations
    let private _mapAction =
        System.Buffers.SpanAction<char, struct (string * (char -> char))>(fun (result: Span<char>) (struct (str: string, mapping: char -> char)) ->
            for i = 0 to result.Length - 1 do
                result[i] <- mapping str[i]
        )
#endif
    
    [<CompiledName("Map")>]
    let map (mapping: char -> char) (str: string) =
        if String.IsNullOrEmpty str then
            String.Empty
        else
#if NETSTANDARD2_1_OR_GREATER
            String.Create(str.Length, struct (str, mapping), _mapAction)
#else
            let result = str.ToCharArray()
            
            for i = 0 to result.Length - 1 do
                result[i] <- mapping result[i]
            
            String(result)
#endif

#if NETSTANDARD2_1_OR_GREATER
    // Cache SpanAction instance to avoid allocations
    let private _mapiAction =
        System.Buffers.SpanAction<char, struct (string * OptimizedClosures.FSharpFunc<int,char,char>)>(fun (result: Span<char>) (struct (str, mapping)) ->
            for i = 0 to result.Length - 1 do
                result[i] <- mapping.Invoke (i, str[i])
        )
#endif
    
    [<CompiledName("MapIndexed")>]
    let mapi (mapping: int -> char -> char) (str: string) =
        let len = length str

        if len = 0 then
            String.Empty
        else
            let f = OptimizedClosures.FSharpFunc<_, _, _>.Adapt(mapping)
#if NETSTANDARD2_1_OR_GREATER
            String.Create(len, struct (str, f), _mapiAction)
#else
            let result = str.ToCharArray()

            for i = 0 to result.Length - 1 do
                result[i] <- f.Invoke(i, result[i])

            String(result)
#endif

    // let inline filterBuildString (source: string, target: Span<char>, predicate: char -> bool) =
    
    [<CompiledName("Filter")>]
    let filter (predicate: char -> bool) (str: string) =
        
        let len = length str

        if len = 0 then
            String.Empty

        elif len > LOH_CHAR_THRESHOLD then
            // By using SB here, which is twice slower than the optimized path, we prevent LOH allocations
            // and 'stop the world' collections if the filtering results in smaller strings.
            // We also don't pre-allocate SB here, to allow for less mem pressure when filter result is small.
            let res = StringBuilder()

            str
            |> iter (fun c ->
                if predicate c then
                    res.Append c |> ignore)

            res.ToString()

        else
            
            let target =
#if NETSTANDARD2_1_OR_GREATER
#nowarn "9"
                if len <= STACKALLOC_THRESHOLD then
                    Span<char>((NativePtr.toVoidPtr (NativePtr.stackalloc<char> STACKALLOC_THRESHOLD)), len)
                else
                    // Using the primitive, because array.fs is not yet in scope. It's safe: both len and count are positive.
                    Span(Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked len)
#warnon "9"
#else
                    // same as above
                    Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked len
#endif
            let mutable i = 0

            for c in str do
                if predicate c then
                    target.[i] <- c
                    i <- i + 1
            
#if NETSTANDARD2_1_OR_GREATER
            String(target.Slice(0, i))
#else
            String(target, 0, i)
#endif

    [<CompiledName("Collect")>]
    let collect (mapping: char -> string) (str: string) =
        if String.IsNullOrEmpty str then
            String.Empty
        else
            let res = StringBuilder str.Length
            str |> iter (fun c -> res.Append(mapping c) |> ignore)
            res.ToString()

    [<CompiledName("Initialize")>]
    let init (count: int) (initializer: int -> string) =
        if count < 0 then
            invalidArgInputMustBeNonNegative "count" count

        let res = StringBuilder count

        for i = 0 to count - 1 do
            res.Append(initializer i) |> ignore

        res.ToString()

#if NETSTANDARD2_1_OR_GREATER
    let _replicateAction =
        System.Buffers.SpanAction<char, string>(fun (target: Span<char>) (str: string) ->
            let len = str.Length
            let source = str.AsSpan()

            // O(log(n)) performance loop:
            // Copy first string, then keep copying what we already copied
            // (i.e., doubling it) until we reach or pass the halfway point
            source.CopyTo(target)
            let mutable i = len
            
            while i * 2 < target.Length do
                target.Slice(0, i).CopyTo(target.Slice(i, i))
                i <- i * 2
            
            // finally, copy the remaining half, or less-then half
            target.Slice(0, target.Length - i).CopyTo(target.Slice(i, target.Length - i))
        )
#endif
    
    [<CompiledName("Replicate")>]
    let replicate (count: int) (str: string) =
        if count < 0 then
            invalidArgInputMustBeNonNegative "count" count

        let len = length str

        if len = 0 || count = 0 then
            String.Empty

        elif len = 1 then
            String(str.[0], count)

        elif count <= 4 then
            match count with
            | 1 -> str
            | 2 -> String.Concat(str, str)
            | 3 -> String.Concat(str, str, str)
            | _ -> String.Concat(str, str, str, str)

        else
#if NETSTANDARD2_1_OR_GREATER
            String.Create(len * count, str, _replicateAction)
#else
            // Using the primitive, because array.fs is not yet in scope. It's safe: both len and count are positive.
            let target =
                Microsoft.FSharp.Primitives.Basics.Array.zeroCreateUnchecked (len * count)

            let source = str.ToCharArray()

            // O(log(n)) performance loop:
            // Copy first string, then keep copying what we already copied
            // (i.e., doubling it) until we reach or pass the halfway point
            Array.Copy(source, 0, target, 0, len)
            let mutable i = len

            while i * 2 < target.Length do
                Array.Copy(target, 0, target, i, i)
                i <- i * 2

            // finally, copy the remaining half, or less-then half
            Array.Copy(target, 0, target, i, target.Length - i)
            String(target)
#endif

    [<CompiledName("ForAll")>]
    let forall predicate (str: string) =
        if String.IsNullOrEmpty str then
            true
        else
            let rec check i =
                (i >= str.Length) || (predicate str.[i] && check (i + 1))

            check 0

    [<CompiledName("Exists")>]
    let exists predicate (str: string) =
        if String.IsNullOrEmpty str then
            false
        else
            let rec check i =
                (i < str.Length) && (predicate str.[i] || check (i + 1))

            check 0
