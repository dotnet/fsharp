
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
.assembly ZeroToArrLength02
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ZeroToArrLength02
{
  // Offset: 0x00000000 Length: 0x000001E0
}
.mresource public FSharpOptimizationData.ZeroToArrLength02
{
  // Offset: 0x000001E8 Length: 0x0000007B
}
.module ZeroToArrLength02.dll
// MVID: {59B18AEE-A36B-03A7-A745-0383EE8AB159}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x01550000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed ZeroToArrLength02
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static void  f1(int32[] arr) cil managed
  {
    // Code size       23 (0x17)
    .maxstack  5
    .locals init ([0] int32 i)
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 6,6 : 5,41 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\Optimizations\\ForLoop\\ZeroToArrLength02.fs'
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  br.s       IL_0010

    .line 7,7 : 9,21 ''
    IL_0004:  ldarg.0
    IL_0005:  ldloc.0
    IL_0006:  ldloc.0
    IL_0007:  stelem     [mscorlib]System.Int32
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.1
    IL_000e:  add
    IL_000f:  stloc.0
    .line 6,6 : 5,41 ''
    IL_0010:  ldloc.0
    IL_0011:  ldarg.0
    IL_0012:  ldlen
    IL_0013:  conv.i4
    IL_0014:  blt.s      IL_0004

    IL_0016:  ret
  } // end of method ZeroToArrLength02::f1

} // end of class ZeroToArrLength02

.class private abstract auto ansi sealed '<StartupCode$ZeroToArrLength02>'.$ZeroToArrLength02
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$ZeroToArrLength02>'.$ZeroToArrLength02


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
