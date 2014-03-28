
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.1
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern FSharp.Core
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 4:0:0:0
}
.assembly Equals02
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Equals02
{
  // Offset: 0x00000000 Length: 0x00000248
}
.mresource public FSharpOptimizationData.Equals02
{
  // Offset: 0x00000250 Length: 0x000000B6
}
.module Equals02.dll
// MVID: {4BEB29FB-0759-B6D8-A745-0383FB29EB4B}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x005B0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Equals02
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public EqualsMicroPerfAndCodeGenerationTests
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static bool  f4_tuple4() cil managed
    {
      // Code size       73 (0x49)
      .maxstack  6
      .locals init ([0] bool x,
               [1] class [mscorlib]System.Tuple`4<int32,int32,int32,string> t1,
               [2] class [mscorlib]System.Tuple`4<int32,int32,int32,string> t2,
               [3] int32 i)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 5,5 : 8,29 
      IL_0000:  nop
      IL_0001:  ldc.i4.0
      IL_0002:  stloc.0
      .line 6,6 : 8,31 
      IL_0003:  ldc.i4.1
      IL_0004:  ldc.i4.2
      IL_0005:  ldc.i4.4
      IL_0006:  ldstr      "five"
      IL_000b:  newobj     instance void class [mscorlib]System.Tuple`4<int32,int32,int32,string>::.ctor(!0,
                                                                                                         !1,
                                                                                                         !2,
                                                                                                         !3)
      IL_0010:  stloc.1
      .line 7,7 : 8,28 
      IL_0011:  ldc.i4.1
      IL_0012:  ldc.i4.2
      IL_0013:  ldc.i4.4
      IL_0014:  ldstr      "5"
      IL_0019:  newobj     instance void class [mscorlib]System.Tuple`4<int32,int32,int32,string>::.ctor(!0,
                                                                                                         !1,
                                                                                                         !2,
                                                                                                         !3)
      IL_001e:  stloc.2
      .line 8,8 : 8,32 
      IL_001f:  ldc.i4.0
      IL_0020:  stloc.3
      IL_0021:  br.s       IL_003f

      .line 9,9 : 12,26 
      IL_0023:  ldc.i4.1
      IL_0024:  brfalse.s  IL_0038

      IL_0026:  ldstr      "five"
      IL_002b:  ldstr      "5"
      IL_0030:  call       bool [mscorlib]System.String::Equals(string,
                                                                string)
      IL_0035:  nop
      IL_0036:  br.s       IL_003a

      IL_0038:  ldc.i4.0
      IL_0039:  nop
      IL_003a:  stloc.0
      IL_003b:  ldloc.3
      IL_003c:  ldc.i4.1
      IL_003d:  add
      IL_003e:  stloc.3
      .line 8,8 : 21,29 
      IL_003f:  ldloc.3
      IL_0040:  ldc.i4     0x989681
      IL_0045:  blt.s      IL_0023

      .line 10,10 : 8,9 
      IL_0047:  ldloc.0
      IL_0048:  ret
    } // end of method EqualsMicroPerfAndCodeGenerationTests::f4_tuple4

  } // end of class EqualsMicroPerfAndCodeGenerationTests

} // end of class Equals02

.class private abstract auto ansi sealed '<StartupCode$Equals02>'.$Equals02$fsx
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$Equals02>'.$Equals02$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
