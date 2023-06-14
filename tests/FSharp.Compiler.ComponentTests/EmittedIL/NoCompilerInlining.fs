﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace EmittedIL

open Xunit
open FSharp.Test.Compiler

module ``NoCompilerInlining`` =
    [<Fact>]
    let ``Function marked with NoCompilerInlining is not inlined by the compiler``() =
        FSharp """
module NoCompilerInlining

let functionInlined () = 3

[<NoCompilerInliningAttribute>]
let functionNotInlined () = 3

let x () = functionInlined () + functionNotInlined ()
"""
         |> compile
         |> shouldSucceed
         |> verifyIL ["""
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static int32  functionInlined() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.3
    IL_0001:  ret
  }"""

                      """
  .method public static int32  functionNotInlined() cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoCompilerInliningAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldc.i4.3
    IL_0001:  ret
  }"""

                      """
  .method public static int32  x() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.3
    IL_0001:  call       int32 NoCompilerInlining::functionNotInlined()
    IL_0006:  add
    IL_0007:  ret
  }"""]

    [<Fact>]
    let ``Value marked with NoCompilerInlining is not inlined by the compiler``() =
        FSharp """
module NoCompilerInlining

let valueInlined = 3

[<NoCompilerInliningAttribute>]
let valueNotInlined = 3

let x () = valueInlined + valueNotInlined
"""
         |> compile
         |> shouldSucceed
         |> verifyIL ["""
          get_valueInlined() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldc.i4.3
    IL_0001:  ret
  }"""

                      """
          get_valueNotInlined() cil managed
  {
    .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldc.i4.3
    IL_0001:  ret
  }"""

                      """
  .method public static int32  x() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.3
    IL_0001:  call       int32 NoCompilerInlining::get_valueNotInlined()
    IL_0006:  add
    IL_0007:  ret
  }"""

                      """
  .property int32 valueInlined()
  {
    .get int32 NoCompilerInlining::get_valueInlined()
  }"""

                      """
  .property int32 valueNotInlined()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoCompilerInliningAttribute::.ctor() = ( 01 00 00 00 ) 
    .get int32 NoCompilerInlining::get_valueNotInlined()
  } 
"""]