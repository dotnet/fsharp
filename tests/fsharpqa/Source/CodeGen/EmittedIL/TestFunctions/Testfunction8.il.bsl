
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
.assembly TestFunction8
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.TestFunction8
{
  // Offset: 0x00000000 Length: 0x000001EC
}
.mresource public FSharpOptimizationData.TestFunction8
{
  // Offset: 0x000001F0 Length: 0x00000070
}
.module TestFunction8.exe
// MVID: {4DAC30D6-65CF-8929-A745-0383D630AC4D}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x0000000000270000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed TestFunction8
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static int32  TestFunction8(int32 x) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  8
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 5,5 : 5,18 
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.3
    IL_0003:  ble.s      IL_0007

    IL_0005:  br.s       IL_0009

    IL_0007:  br.s       IL_000d

    .line 6,6 : 9,12 
    IL_0009:  ldarg.0
    IL_000a:  ldc.i4.4
    IL_000b:  add
    IL_000c:  ret

    .line 7,7 : 10,13 
    IL_000d:  ldarg.0
    IL_000e:  ldc.i4.4
    IL_000f:  sub
    IL_0010:  ret
  } // end of method TestFunction8::TestFunction8

} // end of class TestFunction8

.class private abstract auto ansi sealed '<StartupCode$TestFunction8>'.$TestFunction8
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method $TestFunction8::main@

} // end of class '<StartupCode$TestFunction8>'.$TestFunction8


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
