
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
.assembly Compare02
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Compare02
{
  // Offset: 0x00000000 Length: 0x00000250
}
.mresource public FSharpOptimizationData.Compare02
{
  // Offset: 0x00000258 Length: 0x000000B9
}
.module Compare02.dll
// MVID: {4DAC3A30-0481-F88E-A745-0383303AAC4D}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x0000000000260000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Compare02
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public CompareMicroPerfAndCodeGenerationTests
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static void  f4_triple() cil managed
    {
      // Code size       71 (0x47)
      .maxstack  5
      .locals init ([0] int32 x,
               [1] class [mscorlib]System.Tuple`3<int32,int32,int32> t1,
               [2] class [mscorlib]System.Tuple`3<int32,int32,int32> t2,
               [3] int32 i,
               [4] int32 V_4,
               [5] int32 V_5)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 5,5 : 8,25 
      IL_0000:  nop
      IL_0001:  ldc.i4.1
      IL_0002:  stloc.0
      .line 6,6 : 8,24 
      IL_0003:  ldc.i4.1
      IL_0004:  ldc.i4.2
      IL_0005:  ldc.i4.3
      IL_0006:  newobj     instance void class [mscorlib]System.Tuple`3<int32,int32,int32>::.ctor(!0,
                                                                                                  !1,
                                                                                                  !2)
      IL_000b:  stloc.1
      .line 7,7 : 8,24 
      IL_000c:  ldc.i4.1
      IL_000d:  ldc.i4.2
      IL_000e:  ldc.i4.4
      IL_000f:  newobj     instance void class [mscorlib]System.Tuple`3<int32,int32,int32>::.ctor(!0,
                                                                                                  !1,
                                                                                                  !2)
      IL_0014:  stloc.2
      .line 8,8 : 8,32 
      IL_0015:  ldc.i4.0
      IL_0016:  stloc.3
      IL_0017:  br.s       IL_003e

      .line 9,9 : 12,30 
      IL_0019:  ldc.i4.1
      IL_001a:  ldc.i4.1
      IL_001b:  cgt
      IL_001d:  stloc.s    V_4
      IL_001f:  ldloc.s    V_4
      IL_0021:  brfalse.s  IL_0028

      IL_0023:  ldloc.s    V_4
      IL_0025:  nop
      IL_0026:  br.s       IL_0039

      IL_0028:  ldc.i4.2
      IL_0029:  ldc.i4.2
      IL_002a:  cgt
      IL_002c:  stloc.s    V_5
      IL_002e:  ldloc.s    V_5
      IL_0030:  brfalse.s  IL_0037

      IL_0032:  ldloc.s    V_5
      IL_0034:  nop
      IL_0035:  br.s       IL_0039

      IL_0037:  ldc.i4.m1
      IL_0038:  nop
      IL_0039:  stloc.0
      IL_003a:  ldloc.3
      IL_003b:  ldc.i4.1
      IL_003c:  add
      IL_003d:  stloc.3
      .line 8,8 : 21,29 
      IL_003e:  ldloc.3
      IL_003f:  ldc.i4     0x989681
      IL_0044:  blt.s      IL_0019

      IL_0046:  ret
    } // end of method CompareMicroPerfAndCodeGenerationTests::f4_triple

  } // end of class CompareMicroPerfAndCodeGenerationTests

} // end of class Compare02

.class private abstract auto ansi sealed '<StartupCode$Compare02>'.$Compare02$fsx
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$Compare02>'.$Compare02$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
