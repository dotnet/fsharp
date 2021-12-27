// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.MethodResolution.Parameters

open Xunit
open FSharp.Test.Compiler

module ParametersResolution =

    [<Fact>]
    let ``Static method with default parameter resolves correctnly`` () =
        Fsx """
open System.Runtime.InteropServices

type Thing =
    static member Do(_: outref<bool>, [<Optional; DefaultParameterValue(1)>]i: int) = true
let _, _ = Thing.Do(i = 1)
let _, _ = Thing.Do()
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Methods with byref param are resolved correctly`` () =
        Fsx """
open System.Collections.Generic

let dict = Dictionary<int, string>()
match dict.Remove(1) : bool * string with
| true, value -> Some value
| false, _ -> None
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

