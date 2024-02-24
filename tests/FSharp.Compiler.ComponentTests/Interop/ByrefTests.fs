// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Interop

open FSharp.Test
open FSharp.Test.Compiler

module ``Byref interop verification tests`` =

    [<FactForNETCOREAPP>]
    let ``Test that ref readonly is treated as inref`` () =

        FSharp """
        module ByrefTest
        open System.Runtime.CompilerServices
        type MyRecord = { Value : int } with
            member this.SetValue(v: int) = (Unsafe.AsRef<int> &this.Value) <- v

        let check mr =
            if mr.Value <> 1 then
                failwith "Value should be 1"

            mr.SetValue(42)

            if mr.Value <> 42 then
                failwith $"Value should be 42, but is {mr.Value}"
            0

        [<EntryPoint>]
        let main _ =
            let mr = { Value = 1 }
            check mr
        """
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    [<FactForNETCOREAPP>]
    let ``Test that ref readonly is treated as inref for ROS .ctor`` () =
        FSharp """
        module Foo
        open System


        [<EntryPoint>]
        let main _ =
            let mutable bt: int = 42
            let ros = ReadOnlySpan<int>(&bt)

            if ros.Length <> 1 || ros[0] <> 42 then
                failwith "Unexpected result"
            0
        """
        |> asExe
        |> compileAndRun
        |> shouldSucceed