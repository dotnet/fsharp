// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.LanguageServer

open System
open FSharp.Reflection
open Newtonsoft.Json

type JsonOptionConverter() =
    inherit JsonConverter()
    override __.CanConvert(typ) = typ.IsGenericType && typ.GetGenericTypeDefinition() = typedefof<option<_>>
    override __.WriteJson(writer, value, serializer) =
        let value = match value with
                    | null -> null
                    | _ ->
                        let _, fields =  FSharpValue.GetUnionFields(value, value.GetType())
                        fields.[0]
        serializer.Serialize(writer, value)
    override __.ReadJson(reader, typ, _, serializer) =
        let innerType = typ.GetGenericArguments().[0]
        let innerType =
            if innerType.IsValueType then (typedefof<Nullable<_>>).MakeGenericType([|innerType|])
            else innerType
        let value = serializer.Deserialize(reader, innerType)
        let cases = FSharpType.GetUnionCases(typ)
        if value = null then FSharpValue.MakeUnion(cases.[0], [||])
        else FSharpValue.MakeUnion(cases.[1], [|value|])
