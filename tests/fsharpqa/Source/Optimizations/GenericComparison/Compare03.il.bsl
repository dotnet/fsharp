
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
.assembly Compare03
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Compare03
{
  // Offset: 0x00000000 Length: 0x00000233
}
.mresource public FSharpOptimizationData.Compare03
{
  // Offset: 0x00000238 Length: 0x000000B9
}
.module Compare03.dll
// MVID: {61F0294F-0562-F88E-A745-03834F29F061}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x070D0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Compare03
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public CompareMicroPerfAndCodeGenerationTests
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static int32  f4_tuple4() cil managed
    {
      // Code size       78 (0x4e)
      .maxstack  4
      .locals init ([0] int32 x,
               [1] int32 i,
               [2] int32 V_2,
               [3] int32 V_3,
               [4] int32 V_4)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 5,5 : 8,25 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\Optimizations\\GenericComparison\\Compare03.fsx'
      IL_0000:  ldc.i4.1
      IL_0001:  stloc.0
      .line 6,6 : 8,31 ''
      IL_0002:  nop
      .line 7,7 : 8,28 ''
      IL_0003:  nop
      .line 8,8 : 8,11 ''
      IL_0004:  ldc.i4.0
      IL_0005:  stloc.1
      IL_0006:  br.s       IL_0044

      .line 9,9 : 12,30 ''
      IL_0008:  ldc.i4.1
      IL_0009:  ldc.i4.1
      IL_000a:  cgt
      IL_000c:  stloc.2
      .line 16707566,16707566 : 0,0 ''
      IL_000d:  ldloc.2
      IL_000e:  brfalse.s  IL_0014

      .line 16707566,16707566 : 0,0 ''
      IL_0010:  ldloc.2
      .line 16707566,16707566 : 0,0 ''
      IL_0011:  nop
      IL_0012:  br.s       IL_003f

      .line 16707566,16707566 : 0,0 ''
      IL_0014:  ldc.i4.2
      IL_0015:  ldc.i4.2
      IL_0016:  cgt
      IL_0018:  stloc.3
      .line 16707566,16707566 : 0,0 ''
      IL_0019:  ldloc.3
      IL_001a:  brfalse.s  IL_0020

      .line 16707566,16707566 : 0,0 ''
      IL_001c:  ldloc.3
      .line 16707566,16707566 : 0,0 ''
      IL_001d:  nop
      IL_001e:  br.s       IL_003f

      .line 16707566,16707566 : 0,0 ''
      IL_0020:  ldc.i4.4
      IL_0021:  ldc.i4.4
      IL_0022:  cgt
      IL_0024:  stloc.s    V_4
      .line 16707566,16707566 : 0,0 ''
      IL_0026:  ldloc.s    V_4
      IL_0028:  brfalse.s  IL_002f

      .line 16707566,16707566 : 0,0 ''
      IL_002a:  ldloc.s    V_4
      .line 16707566,16707566 : 0,0 ''
      IL_002c:  nop
      IL_002d:  br.s       IL_003f

      .line 16707566,16707566 : 0,0 ''
      IL_002f:  ldstr      "five"
      IL_0034:  ldstr      "5"
      IL_0039:  call       int32 [netstandard]System.String::CompareOrdinal(string,
                                                                            string)
      .line 16707566,16707566 : 0,0 ''
      IL_003e:  nop
      .line 16707566,16707566 : 0,0 ''
      IL_003f:  stloc.0
      IL_0040:  ldloc.1
      IL_0041:  ldc.i4.1
      IL_0042:  add
      IL_0043:  stloc.1
      .line 8,8 : 18,20 ''
      IL_0044:  ldloc.1
      IL_0045:  ldc.i4     0x989681
      IL_004a:  blt.s      IL_0008

      .line 10,10 : 8,9 ''
      IL_004c:  ldloc.0
      IL_004d:  ret
    } // end of method CompareMicroPerfAndCodeGenerationTests::f4_tuple4

  } // end of class CompareMicroPerfAndCodeGenerationTests

} // end of class Compare03

.class private abstract auto ansi sealed '<StartupCode$Compare03>'.$Compare03$fsx
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$Compare03>'.$Compare03$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
