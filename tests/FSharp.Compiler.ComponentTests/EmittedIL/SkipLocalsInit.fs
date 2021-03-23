// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.EmittedIL

open Xunit
open FSharp.Test.Utilities.Compiler

module ``SkipLocalsInit`` =

    [<Fact>]
    let ``Init in method not emitted when applied on method``() =
        FSharp """
module SkipLocalsInit

[<System.Runtime.CompilerServices.SkipLocalsInitAttribute>]
let x () =
    let x = "ssa".Length
    x + x
         """
         |> compile
         |> shouldSucceed
         |> verifyIL ["""
.method public static int32  x() cil managed
{
  .custom instance void [runtime]System.Runtime.CompilerServices.SkipLocalsInitAttribute::.ctor() = ( 01 00 00 00 )

  .maxstack  4
  .locals (int32 V_0)
  IL_0000:  ldstr      "ssa"
  IL_0005:  callvirt   instance int32 [runtime]System.String::get_Length()
  IL_000a:  stloc.0
  IL_000b:  ldloc.0
  IL_000c:  ldloc.0
  IL_000d:  add
  IL_000e:  ret
}"""]
