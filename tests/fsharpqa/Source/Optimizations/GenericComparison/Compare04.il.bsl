
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.8.3928.0
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
  .ver 4:7:0:0
}
.assembly Compare04
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Compare04
{
  // Offset: 0x00000000 Length: 0x0000024D
}
.mresource public FSharpOptimizationData.Compare04
{
  // Offset: 0x00000258 Length: 0x000000B9
}
.module Compare04.dll
// MVID: {5EE40408-053B-F88E-A745-03830804E45E}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x06AD0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Compare04
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public CompareMicroPerfAndCodeGenerationTests
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static int32  f4_tuple5() cil managed
    {
      // Code size       211 (0xd3)
      .maxstack  5
      .locals init ([0] int32 x,
               [1] int32 i,
               [2] int32 V_2,
               [3] int32 V_3,
               [4] int32 V_4,
               [5] int32 V_5)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 5,5 : 8,25 'C:\\Users\\phcart\\source\\repos\\fsharp\\tests\\fsharpqa\\source\\Optimizations\\GenericComparison\\Compare04.fsx'
      IL_0000:  ldc.i4.1
      IL_0001:  stloc.0
      .line 8,8 : 8,32 ''
      IL_0002:  ldc.i4.0
      IL_0003:  stloc.1
      IL_0004:  br         IL_00c6

      .line 9,9 : 12,30 ''
      IL_0009:  ldc.i4.1
      IL_000a:  ldc.i4.1
      IL_000b:  cgt
      IL_000d:  stloc.2
      IL_000e:  ldloc.2
      IL_000f:  brtrue     IL_00bf

      .line 16707566,16707566 : 0,0 ''
      IL_0014:  ldc.i4.2
      IL_0015:  ldc.i4.2
      IL_0016:  cgt
      IL_0018:  stloc.3
      IL_0019:  ldloc.3
      IL_001a:  brtrue     IL_00bb

      .line 16707566,16707566 : 0,0 ''
      IL_001f:  ldc.i4.4
      IL_0020:  ldc.i4.4
      IL_0021:  cgt
      IL_0023:  stloc.s    V_4
      IL_0025:  ldloc.s    V_4
      IL_0027:  brtrue     IL_00b6

      .line 16707566,16707566 : 0,0 ''
      IL_002c:  ldstr      "5"
      IL_0031:  ldstr      "5"
      IL_0036:  call       int32 [mscorlib]System.String::CompareOrdinal(string,
                                                                         string)
      IL_003b:  stloc.s    V_5
      IL_003d:  ldloc.s    V_5
      IL_003f:  brtrue     IL_00b1

      .line 16707566,16707566 : 0,0 ''
      IL_0044:  ldc.r8     6.
      IL_004d:  ldc.r8     7.
      IL_0056:  clt
      IL_0058:  brfalse.s  IL_005e

      .line 16707566,16707566 : 0,0 ''
      IL_005a:  ldc.i4.m1
      .line 16707566,16707566 : 0,0 ''
      IL_005b:  nop
      IL_005c:  br.s       IL_00c1

      .line 16707566,16707566 : 0,0 ''
      IL_005e:  ldc.r8     6.
      IL_0067:  ldc.r8     7.
      IL_0070:  cgt
      IL_0072:  brfalse.s  IL_0078

      .line 16707566,16707566 : 0,0 ''
      IL_0074:  ldc.i4.1
      .line 16707566,16707566 : 0,0 ''
      IL_0075:  nop
      IL_0076:  br.s       IL_00c1

      .line 16707566,16707566 : 0,0 ''
      IL_0078:  ldc.r8     6.
      IL_0081:  ldc.r8     7.
      IL_008a:  ceq
      IL_008c:  brfalse.s  IL_0092

      .line 16707566,16707566 : 0,0 ''
      IL_008e:  ldc.i4.0
      .line 16707566,16707566 : 0,0 ''
      IL_008f:  nop
      IL_0090:  br.s       IL_00c1

      .line 16707566,16707566 : 0,0 ''
      IL_0092:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0097:  ldc.r8     6.
      IL_00a0:  ldc.r8     7.
      IL_00a9:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<float64>(class [mscorlib]System.Collections.IComparer,
                                                                                                                                                    !!0,
                                                                                                                                                    !!0)
      .line 16707566,16707566 : 0,0 ''
      IL_00ae:  nop
      IL_00af:  br.s       IL_00c1

      .line 16707566,16707566 : 0,0 ''
      IL_00b1:  ldloc.s    V_5
      .line 16707566,16707566 : 0,0 ''
      IL_00b3:  nop
      IL_00b4:  br.s       IL_00c1

      .line 16707566,16707566 : 0,0 ''
      IL_00b6:  ldloc.s    V_4
      .line 16707566,16707566 : 0,0 ''
      IL_00b8:  nop
      IL_00b9:  br.s       IL_00c1

      .line 16707566,16707566 : 0,0 ''
      IL_00bb:  ldloc.3
      .line 16707566,16707566 : 0,0 ''
      IL_00bc:  nop
      IL_00bd:  br.s       IL_00c1

      .line 16707566,16707566 : 0,0 ''
      IL_00bf:  ldloc.2
      .line 16707566,16707566 : 0,0 ''
      IL_00c0:  nop
      .line 16707566,16707566 : 0,0 ''
      IL_00c1:  stloc.0
      IL_00c2:  ldloc.1
      IL_00c3:  ldc.i4.1
      IL_00c4:  add
      IL_00c5:  stloc.1
      .line 8,8 : 8,32 ''
      IL_00c6:  ldloc.1
      IL_00c7:  ldc.i4     0x989681
      IL_00cc:  blt        IL_0009

      .line 10,10 : 8,9 ''
      IL_00d1:  ldloc.0
      IL_00d2:  ret
    } // end of method CompareMicroPerfAndCodeGenerationTests::f4_tuple5

  } // end of class CompareMicroPerfAndCodeGenerationTests

} // end of class Compare04

.class private abstract auto ansi sealed '<StartupCode$Compare04>'.$Compare04$fsx
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$Compare04>'.$Compare04$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
