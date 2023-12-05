// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Core

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Result =

    [<CompiledName("Map")>]
    let inline map ([<InlineIfLambda>] mapping) result =
        match result with
        | Error e -> Error e
        | Ok x -> Ok(mapping x)

    [<CompiledName("MapError")>]
    let inline mapError ([<InlineIfLambda>] mapping) result =
        match result with
        | Error e -> Error(mapping e)
        | Ok x -> Ok x

    [<CompiledName("Bind")>]
    let inline bind ([<InlineIfLambda>] binder) result =
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
    let inline defaultValue value result =
        match result with
        | Error _ -> value
        | Ok v -> v

    [<CompiledName("DefaultWith")>]
    let inline defaultWith ([<InlineIfLambda>] defThunk) result =
        match result with
        | Error error -> defThunk error
        | Ok v -> v

    [<CompiledName("Count")>]
    let inline count result =
        match result with
        | Error _ -> 0
        | Ok _ -> 1

    [<CompiledName("Fold")>]
    let inline fold<'T, 'Error, 'State> ([<InlineIfLambda>] folder) (state: 'State) (result: Result<'T, 'Error>) =
        match result with
        | Error _ -> state
        | Ok x -> folder state x

    [<CompiledName("FoldBack")>]
    let inline foldBack<'T, 'Error, 'State> ([<InlineIfLambda>] folder) (result: Result<'T, 'Error>) (state: 'State) =
        match result with
        | Error _ -> state
        | Ok x -> folder x state

    [<CompiledName("Exists")>]
    let inline exists ([<InlineIfLambda>] predicate) result =
        match result with
        | Error _ -> false
        | Ok x -> predicate x

    [<CompiledName("ForAll")>]
    let inline forall ([<InlineIfLambda>] predicate) result =
        match result with
        | Error _ -> true
        | Ok x -> predicate x

    [<CompiledName("Contains")>]
    let inline contains value result =
        match result with
        | Error _ -> false
        | Ok v -> v = value

    [<CompiledName("Iterate")>]
    let inline iter ([<InlineIfLambda>] action) result =
        match result with
        | Error _ -> ()
        | Ok x -> action x

    [<CompiledName("ToArray")>]
    let inline toArray result =
        match result with
        | Error _ -> [||]
        | Ok x -> [| x |]

    [<CompiledName("ToList")>]
    let inline toList result =
        match result with
        | Error _ -> []
        | Ok x -> [ x ]

    [<CompiledName("ToOption")>]
    let inline toOption result =
        match result with
        | Error _ -> None
        | Ok x -> Some x

    [<CompiledName("ToValueOption")>]
    let inline toValueOption result =
        match result with
        | Error _ -> ValueNone
        | Ok x -> ValueSome x
