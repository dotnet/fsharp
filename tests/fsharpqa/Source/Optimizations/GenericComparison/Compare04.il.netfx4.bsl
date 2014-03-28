
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
.assembly Compare04
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Compare04
{
  // Offset: 0x00000000 Length: 0x0000025B
}
.mresource public FSharpOptimizationData.Compare04
{
  // Offset: 0x00000260 Length: 0x000000B9
}
.module Compare04.dll
// MVID: {4DAC3A34-053B-F88E-A745-0383343AAC4D}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00000000002F0000


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
      // Code size       261 (0x105)
      .maxstack  7
      .locals init ([0] int32 x,
               [1] class [mscorlib]System.Tuple`5<int32,int32,int32,string,float64> t1,
               [2] class [mscorlib]System.Tuple`5<int32,int32,int32,string,float64> t2,
               [3] int32 i,
               [4] int32 V_4,
               [5] int32 V_5,
               [6] int32 V_6,
               [7] int32 V_7)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 5,5 : 8,25 
      IL_0000:  nop
      IL_0001:  ldc.i4.1
      IL_0002:  stloc.0
      .line 6,6 : 8,32 
      IL_0003:  ldc.i4.1
      IL_0004:  ldc.i4.2
      IL_0005:  ldc.i4.4
      IL_0006:  ldstr      "5"
      IL_000b:  ldc.r8     6.
      IL_0014:  newobj     instance void class [mscorlib]System.Tuple`5<int32,int32,int32,string,float64>::.ctor(!0,
                                                                                                                 !1,
                                                                                                                 !2,
                                                                                                                 !3,
                                                                                                                 !4)
      IL_0019:  stloc.1
      .line 7,7 : 8,32 
      IL_001a:  ldc.i4.1
      IL_001b:  ldc.i4.2
      IL_001c:  ldc.i4.4
      IL_001d:  ldstr      "5"
      IL_0022:  ldc.r8     7.
      IL_002b:  newobj     instance void class [mscorlib]System.Tuple`5<int32,int32,int32,string,float64>::.ctor(!0,
                                                                                                                 !1,
                                                                                                                 !2,
                                                                                                                 !3,
                                                                                                                 !4)
      IL_0030:  stloc.2
      .line 8,8 : 8,32 
      IL_0031:  ldc.i4.0
      IL_0032:  stloc.3
      IL_0033:  br         IL_00f8

      .line 9,9 : 12,30 
      IL_0038:  ldc.i4.1
      IL_0039:  ldc.i4.1
      IL_003a:  cgt
      IL_003c:  stloc.s    V_4
      IL_003e:  ldloc.s    V_4
      IL_0040:  brfalse.s  IL_004a

      IL_0042:  ldloc.s    V_4
      IL_0044:  nop
      IL_0045:  br         IL_00f3

      IL_004a:  ldc.i4.2
      IL_004b:  ldc.i4.2
      IL_004c:  cgt
      IL_004e:  stloc.s    V_5
      IL_0050:  ldloc.s    V_5
      IL_0052:  brfalse.s  IL_005c

      IL_0054:  ldloc.s    V_5
      IL_0056:  nop
      IL_0057:  br         IL_00f3

      IL_005c:  ldc.i4.4
      IL_005d:  ldc.i4.4
      IL_005e:  cgt
      IL_0060:  stloc.s    V_6
      IL_0062:  ldloc.s    V_6
      IL_0064:  brfalse.s  IL_006e

      IL_0066:  ldloc.s    V_6
      IL_0068:  nop
      IL_0069:  br         IL_00f3

      IL_006e:  ldstr      "5"
      IL_0073:  ldstr      "5"
      IL_0078:  call       int32 [mscorlib]System.String::CompareOrdinal(string,
                                                                         string)
      IL_007d:  stloc.s    V_7
      IL_007f:  ldloc.s    V_7
      IL_0081:  brfalse.s  IL_0088

      IL_0083:  ldloc.s    V_7
      IL_0085:  nop
      IL_0086:  br.s       IL_00f3

      IL_0088:  ldc.r8     6.
      IL_0091:  ldc.r8     7.
      IL_009a:  clt
      IL_009c:  brfalse.s  IL_00a2

      IL_009e:  ldc.i4.m1
      IL_009f:  nop
      IL_00a0:  br.s       IL_00f3

      IL_00a2:  ldc.r8     6.
      IL_00ab:  ldc.r8     7.
      IL_00b4:  cgt
      IL_00b6:  brfalse.s  IL_00bc

      IL_00b8:  ldc.i4.1
      IL_00b9:  nop
      IL_00ba:  br.s       IL_00f3

      IL_00bc:  ldc.r8     6.
      IL_00c5:  ldc.r8     7.
      IL_00ce:  ceq
      IL_00d0:  brfalse.s  IL_00d6

      IL_00d2:  ldc.i4.0
      IL_00d3:  nop
      IL_00d4:  br.s       IL_00f3

      IL_00d6:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_00db:  ldc.r8     6.
      IL_00e4:  ldc.r8     7.
      IL_00ed:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<float64>(class [mscorlib]System.Collections.IComparer,
                                                                                                                                                    !!0,
                                                                                                                                                    !!0)
      IL_00f2:  nop
      IL_00f3:  stloc.0
      IL_00f4:  ldloc.3
      IL_00f5:  ldc.i4.1
      IL_00f6:  add
      IL_00f7:  stloc.3
      .line 8,8 : 21,29 
      IL_00f8:  ldloc.3
      IL_00f9:  ldc.i4     0x989681
      IL_00fe:  blt        IL_0038

      .line 10,10 : 8,9 
      IL_0103:  ldloc.0
      IL_0104:  ret
    } // end of method CompareMicroPerfAndCodeGenerationTests::f4_tuple5

  } // end of class CompareMicroPerfAndCodeGenerationTests

} // end of class Compare04

.class private abstract auto ansi sealed '<StartupCode$Compare04>'.$Compare04$fsx
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$Compare04>'.$Compare04$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
