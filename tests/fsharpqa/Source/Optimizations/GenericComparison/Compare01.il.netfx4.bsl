
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
  .ver 4:4:1:0
}
.assembly Compare01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // 
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Compare01
{
  // Offset: 0x00000000 Length: 0x0000023F
}
.mresource public FSharpOptimizationData.Compare01
{
  // Offset: 0x00000248 Length: 0x000000B2
}
.module Compare01.dll
// MVID: {54400E31-04A0-F88E-A745-0383310E4054}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00AE0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Compare01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public CompareMicroPerfAndCodeGenerationTests
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static void  f4() cil managed
    {
      // 
      .maxstack  4
      .locals init ([0] int32 x,
               [1] int32 i,
               [2] int32 V_2)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 5,5 : 8,25 'C:\\Users\\latkin\\Source\\Repos\\visualfsharp\\tests\\fsharpqa\\Source\\Optimizations\\GenericComparison\\Compare01.fsx'
      IL_0000:  nop
      IL_0001:  ldc.i4.1
      IL_0002:  stloc.0
      .line 9,9 : 8,32 ''
      IL_0003:  ldc.i4.0
      IL_0004:  stloc.1
      IL_0005:  br.s       IL_001a

      .line 10,10 : 12,30 ''
      IL_0007:  ldc.i4.1
      IL_0008:  ldc.i4.1
      IL_0009:  cgt
      IL_000b:  stloc.2
      IL_000c:  ldloc.2
      IL_000d:  brfalse.s  IL_0013

      IL_000f:  ldloc.2
      IL_0010:  nop
      IL_0011:  br.s       IL_0015

      IL_0013:  ldc.i4.m1
      IL_0014:  nop
      IL_0015:  stloc.0
      IL_0016:  ldloc.1
      IL_0017:  ldc.i4.1
      IL_0018:  add
      IL_0019:  stloc.1
      .line 9,9 : 21,29 ''
      IL_001a:  ldloc.1
      IL_001b:  ldc.i4     0x989681
      IL_0020:  blt.s      IL_0007

      IL_0022:  ret
    } // end of method CompareMicroPerfAndCodeGenerationTests::f4

  } // end of class CompareMicroPerfAndCodeGenerationTests

} // end of class Compare01

.class private abstract auto ansi sealed '<StartupCode$Compare01>'.$Compare01$fsx
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$Compare01>'.$Compare01$fsx


// =============================================================

// 
