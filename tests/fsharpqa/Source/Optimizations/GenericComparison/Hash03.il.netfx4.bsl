
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
.assembly Hash03
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Hash03
{
  // Offset: 0x00000000 Length: 0x00000220
}
.mresource public FSharpOptimizationData.Hash03
{
  // Offset: 0x00000228 Length: 0x000000B0
}
.module Hash03.dll
// MVID: {5772F66F-9642-788D-A745-03836FF67257}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x02290000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Hash03
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public HashMicroPerfAndCodeGenerationTests
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static void  f4_tuple4() cil managed
    {
      // Code size       40 (0x28)
      .maxstack  5
      .locals init ([0] int32 x,
               [1] int32 i)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 5,5 : 8,25 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\Optimizations\\GenericComparison\\Hash03.fsx'
      IL_0000:  nop
      IL_0001:  ldc.i4.1
      IL_0002:  stloc.0
      .line 7,7 : 8,32 ''
      IL_0003:  ldc.i4.0
      IL_0004:  stloc.1
      IL_0005:  br.s       IL_001f

      .line 8,8 : 12,24 ''
      IL_0007:  ldc.i4     0x483
      IL_000c:  ldc.i4.s   99
      IL_000e:  ldstr      "5"
      IL_0013:  call       instance int32 [mscorlib]System.String::GetHashCode()
      IL_0018:  xor
      IL_0019:  xor
      IL_001a:  stloc.0
      IL_001b:  ldloc.1
      IL_001c:  ldc.i4.1
      IL_001d:  add
      IL_001e:  stloc.1
      .line 7,7 : 8,32 ''
      IL_001f:  ldloc.1
      IL_0020:  ldc.i4     0x989681
      IL_0025:  blt.s      IL_0007

      IL_0027:  ret
    } // end of method HashMicroPerfAndCodeGenerationTests::f4_tuple4

  } // end of class HashMicroPerfAndCodeGenerationTests

} // end of class Hash03

.class private abstract auto ansi sealed '<StartupCode$Hash03>'.$Hash03$fsx
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$Hash03>'.$Hash03$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
