// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.Compiler.Service.Tests

open Xunit
open FSharp.Test

module ByteMemoryTests =
    open FSharp.Compiler.IO

    [<Fact>]
    let ``Mapped storage preserves its payload length`` () =
        for length in [0; 1; 4097] do
            let bytes = Array.init length (fun i -> byte (i % 251))
            let memory = ByteMemory.FromArray(bytes).AsReadOnly()
            let storage = ByteStorage.FromByteMemoryAndCopy(memory, useBackingMemoryMappedFile = true)
            for _ in 1..2 do
                let actual = storage.GetByteMemory()
                Assert.shouldBe length actual.Length
                Assert.shouldBe bytes (actual.ToArray())
