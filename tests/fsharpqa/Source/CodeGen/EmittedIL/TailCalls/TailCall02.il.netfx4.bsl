
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.17014
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
  .ver 4:3:0:0
}
.assembly TailCall02
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.TailCall02
{
  // Offset: 0x00000000 Length: 0x00000226
}
.mresource public FSharpOptimizationData.TailCall02
{
  // Offset: 0x00000230 Length: 0x0000007C
}
.module TailCall02.exe
// MVID: {4E2CF047-7D8F-CE9D-A745-038347F02C4E}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x0000000000290000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed TailCall02
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static int32  foo(int32& x) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 3,3 : 24,25 
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldobj      [mscorlib]System.Int32
    IL_0007:  ret
  } // end of method TailCall02::foo

  .method public static int32  run() cil managed
  {
    // Code size       11 (0xb)
    .maxstack  3
    .locals init ([0] int32 x)
    .line 4,4 : 13,30 
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    .line 4,4 : 34,41 
    IL_0003:  ldloca.s   x
    IL_0005:  call       int32 TailCall02::foo(int32&)
    IL_000a:  ret
  } // end of method TailCall02::run

} // end of class TailCall02

.class private abstract auto ansi sealed '<StartupCode$TailCall02>'.$TailCall02
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method $TailCall02::main@

} // end of class '<StartupCode$TailCall02>'.$TailCall02


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
