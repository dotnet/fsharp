// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

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


    [<Fact>]
    let ``Member val regression - not allowed without primary constructor``  () = 
        Fs """module Test
    type Bad3 = 
        member val X = 1 + 1   """     
        |> typecheck
        |> shouldFail
        |> withDiagnostics []
