
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
.assembly Equals08
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Equals08
{
  // Offset: 0x00000000 Length: 0x0000022D
}
.mresource public FSharpOptimizationData.Equals08
{
  // Offset: 0x00000238 Length: 0x000000AF
}
.module Equals08.dll
// MVID: {5772F665-0759-659E-A745-038365F67257}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x01F90000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Equals08
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public EqualsMicroPerfAndCodeGenerationTests
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static bool  f8() cil managed
    {
      // Code size       69 (0x45)
      .maxstack  5
      .locals init ([0] bool x,
               [1] int32[] t1,
               [2] int32[] t2,
               [3] int32 i)
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      .line 5,5 : 8,29 'C:\\GitHub\\dsyme\\visualfsharp\\tests\\fsharpqa\\Source\\Optimizations\\GenericComparison\\Equals08.fsx'
      IL_0000:  nop
      IL_0001:  ldc.i4.0
      IL_0002:  stloc.0
      .line 6,6 : 8,31 ''
      IL_0003:  ldc.i4.0
      IL_0004:  ldc.i4.1
      IL_0005:  ldc.i4.s   100
      IL_0007:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                       int32,
                                                                                                                                                                       int32)
      IL_000c:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::CreateSequence<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0011:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0016:  stloc.1
      .line 7,7 : 8,31 ''
      IL_0017:  ldc.i4.0
      IL_0018:  ldc.i4.1
      IL_0019:  ldc.i4.s   100
      IL_001b:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                       int32,
                                                                                                                                                                       int32)
      IL_0020:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::CreateSequence<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0025:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_002a:  stloc.2
      .line 8,8 : 8,32 ''
      IL_002b:  ldc.i4.0
      IL_002c:  stloc.3
      IL_002d:  br.s       IL_003b

      .line 9,9 : 12,26 ''
      IL_002f:  ldloc.1
      IL_0030:  ldloc.2
      IL_0031:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityIntrinsic<int32[]>(!!0,
                                                                                                                                     !!0)
      IL_0036:  stloc.0
      IL_0037:  ldloc.3
      IL_0038:  ldc.i4.1
      IL_0039:  add
      IL_003a:  stloc.3
      .line 8,8 : 8,32 ''
      IL_003b:  ldloc.3
      IL_003c:  ldc.i4     0x989681
      IL_0041:  blt.s      IL_002f

      .line 10,10 : 8,9 ''
      IL_0043:  ldloc.0
      IL_0044:  ret
    } // end of method EqualsMicroPerfAndCodeGenerationTests::f8

  } // end of class EqualsMicroPerfAndCodeGenerationTests

} // end of class Equals08

.class private abstract auto ansi sealed '<StartupCode$Equals08>'.$Equals08$fsx
       extends [mscorlib]System.Object
{
} // end of class '<StartupCode$Equals08>'.$Equals08$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
