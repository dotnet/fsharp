// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open System
open Xunit
open FSharp.Test

module IndexerRegressionTests =

    [<Fact>]
    let ``Indexer has qualified type value``() =
        CompilerAssert.Pass 
            """
let a = [| 1 |]
let f y = a.[y:int]
            """
