
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
  .ver 5:0:0:0
}
.assembly TestFunction9
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.TestFunction9
{
  // Offset: 0x00000000 Length: 0x000001D2
}
.mresource public FSharpOptimizationData.TestFunction9
{
  // Offset: 0x000001D8 Length: 0x00000070
}
.module TestFunction9.exe
// MVID: {60B68B97-64F4-8929-A745-0383978BB660}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x06B70000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed TestFunction9
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static string  TestFunction9(int32 x) cil managed
  {
    // Code size       36 (0x24)
    .maxstack  8
    .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
    .line 5,5 : 5,17 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\CodeGen\\EmittedIL\\TestFunctions\\TestFunction9.fs'
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.3
    IL_0002:  sub
    IL_0003:  switch     ( 
                          IL_0012,
                          IL_0018)
    IL_0010:  br.s       IL_001e

    .line 6,6 : 12,19 ''
    IL_0012:  ldstr      "three"
    IL_0017:  ret

    .line 7,7 : 12,18 ''
    IL_0018:  ldstr      "four"
    IL_001d:  ret

    .line 8,8 : 12,18 ''
    IL_001e:  ldstr      "five"
    IL_0023:  ret
  } // end of method TestFunction9::TestFunction9

} // end of class TestFunction9

.class private abstract auto ansi sealed '<StartupCode$TestFunction9>'.$TestFunction9
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $TestFunction9::main@

} // end of class '<StartupCode$TestFunction9>'.$TestFunction9


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
