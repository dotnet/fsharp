// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

#nowarn "44" // This construct is deprecated. please use List.item
namespace FSharp.Core.UnitTests.Collections

open System
open FSharp.Core.UnitTests.LibraryTestFx
open Xunit

type ObsoleteListFunctions() =
    [<Fact>]
    member this.Nth() = 
        // integer List 
        let resultInt = List.nth [3;7;9;4;8;1;1;2] 3
        Assert.AreEqual(4, resultInt)

        // string List
        let resultStr = List.nth   ["a";"b";"c";"d"] 3
        Assert.AreEqual("d", resultStr)

        // empty List 
        CheckThrowsArgumentException ( fun() -> List.nth List.empty 1)

        ()