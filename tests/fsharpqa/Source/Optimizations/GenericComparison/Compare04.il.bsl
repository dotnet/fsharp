
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
  .ver 6:0:0:0
}
.assembly extern netstandard
{
  .publickeytoken = (CC 7B 13 FF CD 2D DD 51 )                         // .{...-.Q
  .ver 2:0:0:0
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
  // Offset: 0x00000000 Length: 0x00000233
}
.mresource public FSharpOptimizationData.Compare04
{
  // Offset: 0x00000238 Length: 0x000000B9
}
.module Compare04.dll
// MVID: {61F0294F-053B-F88E-A745-03834F29F061}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x06CD0000


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
      // Code size       210 (0xd2)
      .maxstack  5
      .locals init ([0] int32 x,
               [1] int32 i,
               [2] int32 V_2,
               [3] int32 V_3,
               [4] int32 V_4,
               [5] int32 V_5)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 5,5 : 8,25 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\Optimizations\\GenericComparison\\Compare04.fsx'
      IL_0000:  ldc.i4.1
      IL_0001:  stloc.0
      .line 6,6 : 8,32 ''
      IL_0002:  nop
      .line 7,7 : 8,32 ''
      IL_0003:  nop
      .line 8,8 : 8,11 ''
      IL_0004:  ldc.i4.0
      IL_0005:  stloc.1
      IL_0006:  br         IL_00c5

      .line 9,9 : 12,30 ''
      IL_000b:  ldc.i4.1
      IL_000c:  ldc.i4.1
      IL_000d:  cgt
      IL_000f:  stloc.2
      .line 16707566,16707566 : 0,0 ''
      IL_0010:  ldloc.2
      IL_0011:  brfalse.s  IL_001a

      .line 16707566,16707566 : 0,0 ''
      IL_0013:  ldloc.2
      .line 16707566,16707566 : 0,0 ''
      IL_0014:  nop
      IL_0015:  br         IL_00c0

      .line 16707566,16707566 : 0,0 ''
      IL_001a:  ldc.i4.2
      IL_001b:  ldc.i4.2
      IL_001c:  cgt
      IL_001e:  stloc.3
      .line 16707566,16707566 : 0,0 ''
      IL_001f:  ldloc.3
      IL_0020:  brfalse.s  IL_0029

      .line 16707566,16707566 : 0,0 ''
      IL_0022:  ldloc.3
      .line 16707566,16707566 : 0,0 ''
      IL_0023:  nop
      IL_0024:  br         IL_00c0

      .line 16707566,16707566 : 0,0 ''
      IL_0029:  ldc.i4.4
      IL_002a:  ldc.i4.4
      IL_002b:  cgt
      IL_002d:  stloc.s    V_4
      .line 16707566,16707566 : 0,0 ''
      IL_002f:  ldloc.s    V_4
      IL_0031:  brfalse.s  IL_003b

      .line 16707566,16707566 : 0,0 ''
      IL_0033:  ldloc.s    V_4
      .line 16707566,16707566 : 0,0 ''
      IL_0035:  nop
      IL_0036:  br         IL_00c0

      .line 16707566,16707566 : 0,0 ''
      IL_003b:  ldstr      "5"
      IL_0040:  ldstr      "5"
      IL_0045:  call       int32 [netstandard]System.String::CompareOrdinal(string,
                                                                            string)
      IL_004a:  stloc.s    V_5
      .line 16707566,16707566 : 0,0 ''
      IL_004c:  ldloc.s    V_5
      IL_004e:  brfalse.s  IL_0055

      .line 16707566,16707566 : 0,0 ''
      IL_0050:  ldloc.s    V_5
      .line 16707566,16707566 : 0,0 ''
      IL_0052:  nop
      IL_0053:  br.s       IL_00c0

      .line 16707566,16707566 : 0,0 ''
      IL_0055:  ldc.r8     6.
      IL_005e:  ldc.r8     7.
      IL_0067:  clt
      IL_0069:  brfalse.s  IL_006f

      .line 16707566,16707566 : 0,0 ''
      IL_006b:  ldc.i4.m1
      .line 16707566,16707566 : 0,0 ''
      IL_006c:  nop
      IL_006d:  br.s       IL_00c0

      .line 16707566,16707566 : 0,0 ''
      IL_006f:  ldc.r8     6.
      IL_0078:  ldc.r8     7.
      IL_0081:  cgt
      IL_0083:  brfalse.s  IL_0089

      .line 16707566,16707566 : 0,0 ''
      IL_0085:  ldc.i4.1
      .line 16707566,16707566 : 0,0 ''
      IL_0086:  nop
      IL_0087:  br.s       IL_00c0

      .line 16707566,16707566 : 0,0 ''
      IL_0089:  ldc.r8     6.
      IL_0092:  ldc.r8     7.
      IL_009b:  ceq
      IL_009d:  brfalse.s  IL_00a3

      .line 16707566,16707566 : 0,0 ''
      IL_009f:  ldc.i4.0
      .line 16707566,16707566 : 0,0 ''
      IL_00a0:  nop
      IL_00a1:  br.s       IL_00c0

      .line 16707566,16707566 : 0,0 ''
      IL_00a3:  call       class [mscorlib]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_00a8:  ldc.r8     6.
      IL_00b1:  ldc.r8     7.
      IL_00ba:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<float64>(class [mscorlib]System.Collections.IComparer,
                                                                                                                                                    !!0,
                                                                                                                                                    !!0)
      .line 16707566,16707566 : 0,0 ''
      IL_00bf:  nop
      .line 16707566,16707566 : 0,0 ''
      IL_00c0:  stloc.0
      IL_00c1:  ldloc.1
      IL_00c2:  ldc.i4.1
      IL_00c3:  add
      IL_00c4:  stloc.1
      .line 8,8 : 18,20 ''
      IL_00c5:  ldloc.1
      IL_00c6:  ldc.i4     0x989681
      IL_00cb:  blt        IL_000b

      .line 10,10 : 8,9 ''
      IL_00d0:  ldloc.0
      IL_00d1:  ret
    } // end of method CompareMicroPerfAndCodeGenerationTests::f4_tuple5

  } // end of class CompareMicroPerfAndCodeGenerationTests

} // end of class Compare04

.class private abstract auto ansi sealed '<StartupCode$Compare04>'.$Compare04$fsx
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$Compare04>'.$Compare04$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
