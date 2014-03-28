
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
.assembly Hash04
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Hash04
{
  // Offset: 0x00000000 Length: 0x00000234
}
.mresource public FSharpOptimizationData.Hash04
{
  // Offset: 0x00000238 Length: 0x000000B0
}
.module Hash04.dll
// MVID: {4BEB2A1A-9642-7838-A745-03831A2AEB4B}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x001E0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Hash04
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public HashMicroPerfAndCodeGenerationTests
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static void  f4_tuple5() cil managed
    {
      // Code size       66 (0x42)
      .maxstack  6
      .locals init ([0] int32 x,
               [1] class [mscorlib]System.Tuple`4<int32,int32,int32,string> t3,
               [2] int32 i)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 5,5 : 8,25 
      IL_0000:  nop
      IL_0001:  ldc.i4.1
      IL_0002:  stloc.0
      .line 6,6 : 8,28 
      IL_0003:  ldc.i4.1
      IL_0004:  ldc.i4.2
      IL_0005:  ldc.i4.3
      IL_0006:  ldstr      "5"
      IL_000b:  newobj     instance void class [mscorlib]System.Tuple`4<int32,int32,int32,string>::.ctor(!0,
                                                                                                         !1,
                                                                                                         !2,
                                                                                                         !3)
      IL_0010:  stloc.1
      .line 7,7 : 8,32 
      IL_0011:  ldc.i4.0
      IL_0012:  stloc.2
      IL_0013:  br.s       IL_0039

      .line 8,8 : 12,24 
      IL_0015:  ldc.i4     0x483
      IL_001a:  ldc.i4.s   99
      IL_001c:  ldstr      "5"
      IL_0021:  brfalse.s  IL_0030

      IL_0023:  ldstr      "5"
      IL_0028:  call       instance int32 [mscorlib]System.String::GetHashCode()
      IL_002d:  nop
      IL_002e:  br.s       IL_0032

      IL_0030:  ldc.i4.0
      IL_0031:  nop
      IL_0032:  xor
      IL_0033:  xor
      IL_0034:  stloc.0
      IL_0035:  ldloc.2
      IL_0036:  ldc.i4.1
      IL_0037:  add
      IL_0038:  stloc.2
      .line 7,7 : 21,29 
      IL_0039:  ldloc.2
      IL_003a:  ldc.i4     0x989681
      IL_003f:  blt.s      IL_0015

      IL_0041:  ret
    } // end of method HashMicroPerfAndCodeGenerationTests::f4_tuple5

  } // end of class HashMicroPerfAndCodeGenerationTests

} // end of class Hash04

.class private abstract auto ansi sealed '<StartupCode$Hash04>'.$Hash04$fsx
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$Hash04>'.$Hash04$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
