
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
.assembly Compare03
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Compare03
{
  // Offset: 0x00000000 Length: 0x0000024B
}
.mresource public FSharpOptimizationData.Compare03
{
  // Offset: 0x00000250 Length: 0x000000B9
}
.module Compare03.dll
// MVID: {4BEB29E3-0562-F88E-A745-0383E329EB4B}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00370000


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
      // Code size       111 (0x6f)
      .maxstack  6
      .locals init ([0] int32 x,
               [1] class [mscorlib]System.Tuple`4<int32,int32,int32,string> t1,
               [2] class [mscorlib]System.Tuple`4<int32,int32,int32,string> t2,
               [3] int32 i,
               [4] int32 V_4,
               [5] int32 V_5,
               [6] int32 V_6)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 5,5 : 8,25 
      IL_0000:  nop
      IL_0001:  ldc.i4.1
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
      IL_0021:  br.s       IL_0065

      .line 9,9 : 12,30 
      IL_0023:  ldc.i4.1
      IL_0024:  ldc.i4.1
      IL_0025:  cgt
      IL_0027:  stloc.s    V_4
      IL_0029:  ldloc.s    V_4
      IL_002b:  brfalse.s  IL_0032

      IL_002d:  ldloc.s    V_4
      IL_002f:  nop
      IL_0030:  br.s       IL_0060

      IL_0032:  ldc.i4.2
      IL_0033:  ldc.i4.2
      IL_0034:  cgt
      IL_0036:  stloc.s    V_5
      IL_0038:  ldloc.s    V_5
      IL_003a:  brfalse.s  IL_0041

      IL_003c:  ldloc.s    V_5
      IL_003e:  nop
      IL_003f:  br.s       IL_0060

      IL_0041:  ldc.i4.4
      IL_0042:  ldc.i4.4
      IL_0043:  cgt
      IL_0045:  stloc.s    V_6
      IL_0047:  ldloc.s    V_6
      IL_0049:  brfalse.s  IL_0050

      IL_004b:  ldloc.s    V_6
      IL_004d:  nop
      IL_004e:  br.s       IL_0060

      IL_0050:  ldstr      "five"
      IL_0055:  ldstr      "5"
      IL_005a:  call       int32 [mscorlib]System.String::CompareOrdinal(string,
                                                                         string)
      IL_005f:  nop
      IL_0060:  stloc.0
      IL_0061:  ldloc.3
      IL_0062:  ldc.i4.1
      IL_0063:  add
      IL_0064:  stloc.3
      .line 8,8 : 21,29 
      IL_0065:  ldloc.3
      IL_0066:  ldc.i4     0x989681
      IL_006b:  blt.s      IL_0023

      .line 10,10 : 8,9 
      IL_006d:  ldloc.0
      IL_006e:  ret
    } // end of method CompareMicroPerfAndCodeGenerationTests::f4_tuple4

  } // end of class CompareMicroPerfAndCodeGenerationTests

} // end of class Compare03

.class private abstract auto ansi sealed '<StartupCode$Compare03>'.$Compare03$fsx
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$Compare03>'.$Compare03$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
