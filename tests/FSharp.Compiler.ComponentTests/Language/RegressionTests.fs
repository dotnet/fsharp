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

    [<Fact>]
    let ``No null value should be returned from trait call``() =
        FSharp """
module FSharpBug

type A = A with
    static member ($) (A, a: float) = 0.0
    static member ($) (A, a: decimal) = 0M
    static member ($) (A, a: 't) = 0
    
let inline call x = ($) A x
let expected = 0.0
let actual = call 42.
if actual <> expected then failwith "Unexpected result"
        """
        |> asExe
        |> compileAndRun
        |> shouldSucceed