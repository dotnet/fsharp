// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Interop

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ``Byref interop verification tests`` =

    [<FactForNETCOREAPP>]
    let ``Test that ref readonly is treated as ref`` () =

        FSharp """
        namespace ByrefTest
        open System.Runtime.CompilerServices
        type MyRecord = { Value : int } with
            member this.SetValue(v: int) = (Unsafe.AsRef<int> &this.Value) <- v
        """
        |> compile
        |> shouldSucceed
