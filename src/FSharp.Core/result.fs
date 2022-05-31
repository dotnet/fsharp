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
