
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
.assembly Equals03
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Equals03
{
  // Offset: 0x00000000 Length: 0x00000248
}
.mresource public FSharpOptimizationData.Equals03
{
  // Offset: 0x00000250 Length: 0x000000B6
}
.module Equals03.dll
// MVID: {4BEB29FD-0759-3313-A745-0383FD29EB4B}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x005A0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Equals03
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public EqualsMicroPerfAndCodeGenerationTests
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static bool  f4_tuple5() cil managed
    {
      // Code size       118 (0x76)
      .maxstack  7
      .locals init ([0] bool x,
               [1] class [mscorlib]System.Tuple`5<int32,int32,int32,string,float64> t1,
               [2] class [mscorlib]System.Tuple`5<int32,int32,int32,string,float64> t2,
               [3] int32 i)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 5,5 : 8,29 
      IL_0000:  nop
      IL_0001:  ldc.i4.0
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
      IL_0033:  br.s       IL_006c

      .line 9,9 : 12,26 
      IL_0035:  ldc.i4.1
      IL_0036:  brfalse.s  IL_004a

      IL_0038:  ldstr      "5"
      IL_003d:  ldstr      "5"
      IL_0042:  call       bool [mscorlib]System.String::Equals(string,
                                                                string)
      IL_0047:  nop
      IL_0048:  br.s       IL_004c

      IL_004a:  ldc.i4.0
      IL_004b:  nop
      IL_004c:  brfalse.s  IL_0065

      IL_004e:  ldc.r8     6.
      IL_0057:  ldc.r8     7.
      IL_0060:  ceq
      IL_0062:  nop
      IL_0063:  br.s       IL_0067

      IL_0065:  ldc.i4.0
      IL_0066:  nop
      IL_0067:  stloc.0
      IL_0068:  ldloc.3
      IL_0069:  ldc.i4.1
      IL_006a:  add
      IL_006b:  stloc.3
      .line 8,8 : 21,29 
      IL_006c:  ldloc.3
      IL_006d:  ldc.i4     0x989681
      IL_0072:  blt.s      IL_0035

      .line 10,10 : 8,9 
      IL_0074:  ldloc.0
      IL_0075:  ret
    } // end of method EqualsMicroPerfAndCodeGenerationTests::f4_tuple5

  } // end of class EqualsMicroPerfAndCodeGenerationTests

} // end of class Equals03

.class private abstract auto ansi sealed '<StartupCode$Equals03>'.$Equals03$fsx
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$Equals03>'.$Equals03$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
