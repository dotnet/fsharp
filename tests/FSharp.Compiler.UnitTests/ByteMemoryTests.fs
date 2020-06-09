// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.Compiler.UnitTests

open System
open System.Globalization
open Xunit
open FSharp.Test.Utilities


module ByteMemoryTests =
    open FSharp.Compiler.AbstractIL.Internal

    [<Fact>]
    let ``ByteMemory.CreateMemoryMappedFile succeeds with byte length of zero``() =
        let memory = ByteMemory.Empty.AsReadOnly()
        let newMemory = ByteMemory.CreateMemoryMappedFile memory
        Assert.shouldBe(0, newMemory.Length)