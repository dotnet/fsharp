// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.Compiler.UnitTests

open System
open System.Globalization
open NUnit.Framework

[<TestFixture>]
module ByteMemoryTests =
    open FSharp.Compiler.AbstractIL.Internal

    [<Test>]
    let ``ByteMemory.CreateMemoryMappedFile succeeds with byte length of zero``() =
        let memory = ByteMemory.Empty.AsReadOnly()
        let newMemory = ByteMemory.CreateMemoryMappedFile memory
        Assert.AreEqual(0, newMemory.Length)