
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
.assembly Compare11
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Compare11
{
  // Offset: 0x00000000 Length: 0x00000244
}
.mresource public FSharpOptimizationData.Compare11
{
  // Offset: 0x00000248 Length: 0x000000B1
}
.module Compare11.dll
// MVID: {4BEB29F6-04A0-1753-A745-0383F629EB4B}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x002B0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Compare11
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public EqualsMicroPerfAndCodeGenerationTests
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static bool  f4() cil managed
    {
      // Code size       39 (0x27)
      .maxstack  4
      .locals init ([0] bool x,
               [1] class [mscorlib]System.Tuple`2<int32,int32> t1,
               [2] class [mscorlib]System.Tuple`2<int32,int32> t2,
               [3] int32 i)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 5,5 : 8,29 
      IL_0000:  nop
      IL_0001:  ldc.i4.0
      IL_0002:  stloc.0
      .line 6,6 : 8,22 
      IL_0003:  ldc.i4.1
      IL_0004:  ldc.i4.2
      IL_0005:  newobj     instance void class [mscorlib]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                            !1)
      IL_000a:  stloc.1
      .line 7,7 : 8,22 
      IL_000b:  ldc.i4.1
      IL_000c:  ldc.i4.3
      IL_000d:  newobj     instance void class [mscorlib]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                            !1)
      IL_0012:  stloc.2
      .line 9,9 : 8,32 
      IL_0013:  ldc.i4.0
      IL_0014:  stloc.3
      IL_0015:  br.s       IL_001d

      .line 10,10 : 12,26 
      IL_0017:  ldc.i4.0
      IL_0018:  stloc.0
      IL_0019:  ldloc.3
      IL_001a:  ldc.i4.1
      IL_001b:  add
      IL_001c:  stloc.3
      .line 9,9 : 21,29 
      IL_001d:  ldloc.3
      IL_001e:  ldc.i4     0x989681
      IL_0023:  blt.s      IL_0017

      .line 11,11 : 8,9 
      IL_0025:  ldloc.0
      IL_0026:  ret
    } // end of method EqualsMicroPerfAndCodeGenerationTests::f4

  } // end of class EqualsMicroPerfAndCodeGenerationTests

} // end of class Compare11

.class private abstract auto ansi sealed '<StartupCode$Compare11>'.$Compare11$fsx
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$Compare11>'.$Compare11$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
