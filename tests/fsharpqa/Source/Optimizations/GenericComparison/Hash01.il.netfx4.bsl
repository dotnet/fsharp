
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
.assembly Hash01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Hash01
{
  // Offset: 0x00000000 Length: 0x0000023D
}
.mresource public FSharpOptimizationData.Hash01
{
  // Offset: 0x00000248 Length: 0x000000A9
}
.module Hash01.dll
// MVID: {4DAC3A56-9642-78D3-A745-0383563AAC4D}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x0000000000360000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Hash01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public HashMicroPerfAndCodeGenerationTests
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static void  f4() cil managed
    {
      // Code size       31 (0x1f)
      .maxstack  4
      .locals init ([0] int32 x,
               [1] int32 i,
               [2] class [mscorlib]System.Tuple`2<int32,int32> V_2)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 5,5 : 8,25 
      IL_0000:  nop
      IL_0001:  ldc.i4.1
      IL_0002:  stloc.0
      .line 6,6 : 8,32 
      IL_0003:  ldc.i4.0
      IL_0004:  stloc.1
      IL_0005:  br.s       IL_0016

      .line 7,7 : 12,27 
      IL_0007:  ldc.i4.1
      IL_0008:  ldc.i4.2
      IL_0009:  newobj     instance void class [mscorlib]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                            !1)
      IL_000e:  stloc.2
      IL_000f:  ldc.i4.s   35
      IL_0011:  stloc.0
      IL_0012:  ldloc.1
      IL_0013:  ldc.i4.1
      IL_0014:  add
      IL_0015:  stloc.1
      .line 6,6 : 21,29 
      IL_0016:  ldloc.1
      IL_0017:  ldc.i4     0x989681
      IL_001c:  blt.s      IL_0007

      IL_001e:  ret
    } // end of method HashMicroPerfAndCodeGenerationTests::f4

  } // end of class HashMicroPerfAndCodeGenerationTests

} // end of class Hash01

.class private abstract auto ansi sealed '<StartupCode$Hash01>'.$Hash01$fsx
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$Hash01>'.$Hash01$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
