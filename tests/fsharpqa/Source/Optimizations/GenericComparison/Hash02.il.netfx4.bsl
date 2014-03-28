
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.16774
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
.assembly Hash02
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Hash02
{
  // Offset: 0x00000000 Length: 0x00000244
}
.mresource public FSharpOptimizationData.Hash02
{
  // Offset: 0x00000248 Length: 0x000000B0
}
.module Hash02.dll
// MVID: {4DAC3A58-9642-796E-A745-0383583AAC4D}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x0000000000680000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Hash02
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public HashMicroPerfAndCodeGenerationTests
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static void  f4_triple() cil managed
    {
      // Code size       33 (0x21)
      .maxstack  5
      .locals init ([0] class [mscorlib]System.Tuple`3<int32,int32,int32> t3,
               [1] int32 i,
               [2] int32 V_2)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 6,6 : 8,24 
      IL_0000:  nop
      IL_0001:  ldc.i4.1
      IL_0002:  ldc.i4.2
      IL_0003:  ldc.i4.3
      IL_0004:  newobj     instance void class [mscorlib]System.Tuple`3<int32,int32,int32>::.ctor(!0,
                                                                                                  !1,
                                                                                                  !2)
      IL_0009:  stloc.0
      .line 7,7 : 8,32 
      IL_000a:  ldc.i4.0
      IL_000b:  stloc.1
      IL_000c:  br.s       IL_0018

      .line 8,8 : 12,29 
      IL_000e:  ldc.i4     0x480
      IL_0013:  stloc.2
      IL_0014:  ldloc.1
      IL_0015:  ldc.i4.1
      IL_0016:  add
      IL_0017:  stloc.1
      .line 7,7 : 21,29 
      IL_0018:  ldloc.1
      IL_0019:  ldc.i4     0x989681
      IL_001e:  blt.s      IL_000e

      IL_0020:  ret
    } // end of method HashMicroPerfAndCodeGenerationTests::f4_triple

  } // end of class HashMicroPerfAndCodeGenerationTests

} // end of class Hash02

.class private abstract auto ansi sealed '<StartupCode$Hash02>'.$Hash02$fsx
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$Hash02>'.$Hash02$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
