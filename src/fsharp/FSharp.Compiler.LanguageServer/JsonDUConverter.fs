// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.LanguageServer

open System
open FSharp.Reflection
open Newtonsoft.Json

type JsonDUConverter() =
    inherit JsonConverter()
    override __.CanConvert(typ) = FSharpType.IsUnion(typ)
    override __.WriteJson(writer, value, _serializer) =
        writer.WriteValue(value.ToString().ToLowerInvariant())
    override __.ReadJson(reader, typ, x, serializer) =
        let cases = FSharpType.GetUnionCases(typ)
        let str = serializer.Deserialize(reader, typeof<string>) :?> string
        let case = cases |> Array.find (fun c -> String.Compare(c.Name, str, StringComparison.OrdinalIgnoreCase) = 0)
        FSharpValue.MakeUnion(case, [||])
