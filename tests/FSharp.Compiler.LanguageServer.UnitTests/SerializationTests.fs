// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.LanguageServer.UnitTests

open System
open FSharp.Compiler.LanguageServer
open NUnit.Framework
open Newtonsoft.Json

[<TestFixture>]
type SerializationTests() =

    let verifyRoundTrip (str: string) (typ: Type) =
        let deserialized = JsonConvert.DeserializeObject(str, typ)
        let roundTripped = JsonConvert.SerializeObject(deserialized)
        Assert.AreEqual(str, roundTripped)

    let verifyRoundTripWithConverter (str: string) (typ: Type) (converter: JsonConverter) =
        let deserialized = JsonConvert.DeserializeObject(str, typ, converter)
        let roundTripped = JsonConvert.SerializeObject(deserialized, converter)
        Assert.AreEqual(str, roundTripped)

    [<Test>]
    member __.``Discriminated union as lower-case string``() =
        verifyRoundTrip "\"plaintext\"" typeof<MarkupKind>
        verifyRoundTrip "\"markdown\"" typeof<MarkupKind>

    [<Test>]
    member __.``Option<'T> as obj/null``() =
        verifyRoundTripWithConverter "1" typeof<option<int>> (JsonOptionConverter())
        verifyRoundTripWithConverter "null" typeof<option<int>> (JsonOptionConverter())
        verifyRoundTripWithConverter "{\"contents\":{\"kind\":\"plaintext\",\"value\":\"v\"},\"range\":{\"start\":{\"line\":1,\"character\":2},\"end\":{\"line\":3,\"character\":4}}}" typeof<option<Hover>> (JsonOptionConverter())
        verifyRoundTripWithConverter "null" typeof<option<Hover>> (JsonOptionConverter())
