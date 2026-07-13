// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests.CodeGen.EmittedIL

open FSharp.Test
open Xunit

module BooleanLogic =

    [<Fact>]
    let BooleanOrs() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions ([|"-g"; "--optimize+"|],
            """
module BooleanOrs
let compute (x: int) = 
    if (x = 1 || x = 2) then 2
    elif (x = 3 || x = 4) then 3
    else 4
            """,
            (fun verifier -> verifier.VerifyIL [
            """
.method public static int32  compute(int32 x) cil managed
  {
    
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.1
    IL_0003:  beq.s      IL_0009

    IL_0005:  ldarg.0
    IL_0006:  ldc.i4.2
    IL_0007:  bne.un.s   IL_000b

    IL_0009:  ldc.i4.2
    IL_000a:  ret

    IL_000b:  nop
    IL_000c:  ldarg.0
    IL_000d:  ldc.i4.3
    IL_000e:  beq.s      IL_0014

    IL_0010:  ldarg.0
    IL_0011:  ldc.i4.4
    IL_0012:  bne.un.s   IL_0016

    IL_0014:  ldc.i4.3
    IL_0015:  ret

    IL_0016:  ldc.i4.4
    IL_0017:  ret
}    
            """
            ]))


// We had a regression in debug code regression where we were falsely marking pipelines
// as non-side-effecting, causing them to be eliminated in loops.
//
// After the fix 
//   1. pipelines are correctly marked as having effect
//   2. we don't eliminate loops anyway
module DontEliminateForLoopsInDebugCode =

    [<Fact>]
    // See https://github.com/dotnet/fsharp/pull/12021
    let Regression12021() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"-g"; "--optimize-"|],
            """
module DontEliminateForLoops

let unsolved = [true]
let ApplyDefaults () = 

        for priority = 0 to 10 do
            unsolved |> List.iter (fun tp -> System.Console.WriteLine())

            """,
            (fun verifier -> verifier.VerifyIL [
            """
  .method public static void  ApplyDefaults() cil managed
  {
    
    .maxstack  5
    .locals init (int32 V_0,
             class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<bool> V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  br.s       IL_001a

    IL_0004:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<bool> DontEliminateForLoops::get_unsolved()
    IL_0009:  stloc.1
    IL_000a:  ldsfld     class DontEliminateForLoops/ApplyDefaults@8 DontEliminateForLoops/ApplyDefaults@8::@_instance
    IL_000f:  ldloc.1
    IL_0010:  call       void [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Iterate<bool>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<!!0,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,
                                                                                                  class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0>)
    IL_0015:  nop
    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.1
    IL_0018:  add
    IL_0019:  stloc.0
    IL_001a:  ldloc.0
    IL_001b:  ldc.i4.1
    IL_001c:  ldc.i4.s   10
    IL_001e:  add
    IL_001f:  blt.s      IL_0004

    IL_0021:  ret
  } 
            """
            ]))

