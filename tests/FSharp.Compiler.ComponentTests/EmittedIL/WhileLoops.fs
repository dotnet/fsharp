// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.EmittedIL

open Xunit
open FSharp.Test.Compiler

module WhileLoops =

    [<Fact>]
    let ``Simple while loop``() =
        FSharp """
module WhileLoops

let f () =
    let mutable x = 0

    while x < 9 do
        x <- x + 3
         """
         |> compile
         |> shouldSucceed
         |> verifyIL ["""
  .method public static void  f() cil managed
  {
    
    .maxstack  4
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  br.s       IL_0008

    IL_0004:  ldloc.0
    IL_0005:  ldc.i4.3
    IL_0006:  add
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.s   9
    IL_000b:  blt.s      IL_0004

    IL_000d:  ret
  }""" ]

    [<Fact>]
    let ``While loop with nonempty stack``() =
         FSharp """
module WhileLoops

let something a b =
    printfn "%A %A" a b

let f () =
    let mutable x = 0
    
    something "arg1" ((while x < 9 do x <- x + 3); "arg2")
         """
         |> compile
         |> shouldSucceed
         |> verifyIL ["""
  .method public static void  f() cil managed
  {
    
    .maxstack  5
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldstr      "arg1"
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.s   9
    IL_000a:  bge.s      IL_0012

    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.3
    IL_000e:  add
    IL_000f:  stloc.0
    IL_0010:  br.s       IL_0007

    IL_0012:  ldstr      "arg2"
    IL_0017:  tail.
    IL_0019:  call       void WhileLoops::something<string,string>(!!0,
                                                                   !!1)
    IL_001e:  ret
  }""" ]
