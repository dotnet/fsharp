// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Language

open Xunit
open FSharp.Test.Compiler

module RegressionTests =

    [<Fact>]
    let ``No internal errors should be raised``() =
        FSharp """
namespace FSharpBug

type TestItemSeq = 
    static member Test1 item = item
    static member Test2 item = match item with Typo2 x -> x
        """
        |> compile
        |> withErrorCodes [39]
        |> ignore
