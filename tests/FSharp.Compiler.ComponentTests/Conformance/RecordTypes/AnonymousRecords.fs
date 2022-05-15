// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module AnonymousRecord =

    [<Fact>]
    let ``Anonymous Records with duplicate labels`` () =
        FSharp """
namespace FSharpTest

module AnonRecd =
    let v = {| A = 1; A = 2 |}
"""
        |> compile
        |> shouldFail
        |> withErrorCode 3522

    [<Fact>]
    let ``Anonymous Records with duplicate labels - Update and copy syntax`` () =
        FSharp """
namespace FSharpTest

module AnonRecd =
    let v = {| {| y = 3 |} with y = 2; y = 4 |}
"""
        |> compile
        |> shouldFail
        |> withErrorCode 3522