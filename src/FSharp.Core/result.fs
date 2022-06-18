// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Core

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Result =

    [<CompiledName("Map")>]
    let map mapping result =
        match result with
        | Error e -> Error e
        | Ok x -> Ok(mapping x)

    [<CompiledName("MapError")>]
    let mapError mapping result =
        match result with
        | Error e -> Error(mapping e)
        | Ok x -> Ok x

    [<CompiledName("Bind")>]
    let bind binder result =
        match result with
        | Error e -> Error e
        | Ok x -> binder x

    [<CompiledName("IsOk")>]
    let inline isOk result =
        match result with
        | Ok _ -> true
        | Error _ -> false

    [<CompiledName("IsError")>]
    let inline isError result =
        match result with
        | Ok _ -> false
        | Error _ -> true

    [<CompiledName("DefaultValue")>]
    let defaultValue value result =
        match result with
        | Error _ -> value
        | Ok v -> v

    [<CompiledName("DefaultWith")>]
    let defaultWith defThunk result =
        match result with
        | Error _ -> defThunk ()
        | Ok v -> v

    [<CompiledName("Count")>]
    let count result =
        match result with
        | Error _ -> 0
        | Ok _ -> 1

    [<CompiledName("Fold")>]
    let fold<'T, 'Error, 'State> folder (state: 'State) (result: Result<'T, 'Error>) =
        match result with
        | Error _ -> state
        | Ok x -> folder state x

    [<CompiledName("FoldBack")>]
    let foldBack<'T, 'Error, 'State> folder (result: Result<'T, 'Error>) (state: 'State) =
        match result with
        | Error _ -> state
        | Ok x -> folder x state

    [<CompiledName("Exists")>]
    let exists predicate result =
        match result with
        | Error _ -> false
        | Ok x -> predicate x

    [<CompiledName("ForAll")>]
    let forall predicate result =
        match result with
        | Error _ -> true
        | Ok x -> predicate x

    [<CompiledName("Contains")>]
    let inline contains value result =
        match result with
        | Error _ -> false
        | Ok v -> v = value

    [<CompiledName("Iterate")>]
    let iter action result =
        match result with
        | Error _ -> ()
        | Ok x -> action x

    [<CompiledName("ToArray")>]
    let toArray result =
        match result with
        | Error _ -> [| |]
        | Ok x -> [| x |]

    [<CompiledName("ToList")>]
    let toList result =
        match result with
        | Error _ -> []
        | Ok x -> [ x ]

    [<CompiledName("ToSeq")>]
    let toSeq result =
        match result with
        | Error _ -> []
        | Ok x -> [ x ]

    [<CompiledName("ToOption")>]
    let toOption result =
        match result with
        | Error _ -> None
        | Ok x -> Some x

    [<CompiledName("ToValueOption")>]
    let toValueOption result =
        match result with
        | Error _ -> ValueNone
        | Ok x -> ValueSome x
