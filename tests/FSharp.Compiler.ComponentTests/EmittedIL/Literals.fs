// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.EmittedIL

open Xunit
open FSharp.Test.Compiler

module ``Literals`` =

    [<Fact>]
    let ``Literal attribute generates literal static field``() =
        FSharp """
module LiteralValue

[<Literal>]
let x = 7

[<EntryPoint>]
let main _ =
    0
         """
         |> compile
         |> shouldSucceed
         |> verifyIL ["""
.field public static literal int32 x = int32(0x00000007)
.custom instance void [FSharp.Core]Microsoft.FSharp.Core.LiteralAttribute::.ctor() = ( 01 00 00 00 )"""]
