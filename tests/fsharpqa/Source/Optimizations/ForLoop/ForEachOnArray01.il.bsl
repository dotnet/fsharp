
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
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
.assembly ForEachOnArray01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ForEachOnArray01
{
  // Offset: 0x00000000 Length: 0x000001E0
}
.mresource public FSharpOptimizationData.ForEachOnArray01
{
  // Offset: 0x000001E8 Length: 0x0000007C
}
.module ForEachOnArray01.dll
// MVID: {5772F60B-7E2E-D3AE-A745-03830BF67257}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x006C0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed ForEachOnArray01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static void  test3(int32[] arr) cil managed
  {
    // Code size       30 (0x1e)
    .maxstack  4
    .locals init ([0] int32 z,
             [1] int32 V_1,
             [2] int32 x)
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 6,6 : 6,23 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\Optimizations\\ForLoop\\ForEachOnArray01.fs'
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    .line 7,7 : 6,21 ''
    IL_0003:  ldc.i4.0
    IL_0004:  stloc.1
    IL_0005:  br.s       IL_0017

    .line 7,7 : 6,21 ''
    IL_0007:  ldarg.0
    IL_0008:  ldloc.1
    IL_0009:  ldelem     [mscorlib]System.Int32
    IL_000e:  stloc.2
    IL_000f:  ldloc.0
    IL_0010:  ldloc.2
    IL_0011:  add
    IL_0012:  stloc.0
    IL_0013:  ldloc.1
    IL_0014:  ldc.i4.1
    IL_0015:  add
    IL_0016:  stloc.1
    .line 7,7 : 6,21 ''
    IL_0017:  ldloc.1
    IL_0018:  ldarg.0
    IL_0019:  ldlen
    IL_001a:  conv.i4
    IL_001b:  blt.s      IL_0007

    IL_001d:  ret
  } // end of method ForEachOnArray01::test3

} // end of class ForEachOnArray01

.class private abstract auto ansi sealed '<StartupCode$ForEachOnArray01>'.$ForEachOnArray01
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$ForEachOnArray01>'.$ForEachOnArray01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
