
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
.assembly Equals02
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 01 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Equals02
{
  // Offset: 0x00000000 Length: 0x00000230
}
.mresource public FSharpOptimizationData.Equals02
{
  // Offset: 0x00000238 Length: 0x000000B6
}
.module Equals02.dll
// MVID: {61F0294F-0759-B6D8-A745-03834F29F061}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x07610000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Equals02
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public EqualsMicroPerfAndCodeGenerationTests
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static bool  f4_tuple4() cil managed
    {
      // Code size       38 (0x26)
      .maxstack  4
      .locals init ([0] bool x,
               [1] int32 i)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 5,5 : 8,29 'C:\\GitHub\\dsyme\\fsharp\\tests\\fsharpqa\\source\\Optimizations\\GenericComparison\\Equals02.fsx'
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      .line 6,6 : 8,31 ''
      IL_0002:  nop
      .line 7,7 : 8,28 ''
      IL_0003:  nop
      .line 8,8 : 8,11 ''
      IL_0004:  ldc.i4.0
      IL_0005:  stloc.1
      IL_0006:  br.s       IL_001c

      .line 9,9 : 12,26 ''
      IL_0008:  ldstr      "five"
      IL_000d:  ldstr      "5"
      IL_0012:  call       bool [netstandard]System.String::Equals(string,
                                                                   string)
      IL_0017:  stloc.0
      IL_0018:  ldloc.1
      IL_0019:  ldc.i4.1
      IL_001a:  add
      IL_001b:  stloc.1
      .line 8,8 : 18,20 ''
      IL_001c:  ldloc.1
      IL_001d:  ldc.i4     0x989681
      IL_0022:  blt.s      IL_0008

      .line 10,10 : 8,9 ''
      IL_0024:  ldloc.0
      IL_0025:  ret
    } // end of method EqualsMicroPerfAndCodeGenerationTests::f4_tuple4

  } // end of class EqualsMicroPerfAndCodeGenerationTests

} // end of class Equals02

.class private abstract auto ansi sealed '<StartupCode$Equals02>'.$Equals02$fsx
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$Equals02>'.$Equals02$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
