
//  Microsoft (R) .NET IL Disassembler.  Version 5.0.0-preview.7.20364.11



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
.assembly Hash11
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Hash11
{
  // Offset: 0x00000000 Length: 0x00000249
  // WARNING: managed resource file FSharpSignatureData.Hash11 created
}
.mresource public FSharpOptimizationData.Hash11
{
  // Offset: 0x00000250 Length: 0x000000A9
  // WARNING: managed resource file FSharpOptimizationData.Hash11 created
}
.module Hash11.exe
// MVID: {624F9D3B-966D-0441-A745-03833B9D4F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00E40000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Hash11
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public HashMicroPerfAndCodeGenerationTests
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static void  f8() cil managed
    {
      // Code size       45 (0x2d)
      .maxstack  5
      .locals init (int32[] V_0,
               int32 V_1,
               int32 V_2)
      IL_0000:  nop
      IL_0001:  ldc.i4.0
      IL_0002:  ldc.i4.1
      IL_0003:  ldc.i4.s   100
      IL_0005:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                       int32,
                                                                                                                                                                       int32)
      IL_000a:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::CreateSequence<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_000f:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0014:  stloc.0
      IL_0015:  ldc.i4.0
      IL_0016:  stloc.1
      IL_0017:  br.s       IL_0024

      IL_0019:  ldloc.0
      IL_001a:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericHashIntrinsic<int32[]>(!!0)
      IL_001f:  stloc.2
      IL_0020:  ldloc.1
      IL_0021:  ldc.i4.1
      IL_0022:  add
      IL_0023:  stloc.1
      IL_0024:  ldloc.1
      IL_0025:  ldc.i4     0x989681
      IL_002a:  blt.s      IL_0019

      IL_002c:  ret
    } // end of method HashMicroPerfAndCodeGenerationTests::f8

  } // end of class HashMicroPerfAndCodeGenerationTests

} // end of class Hash11

.class private abstract auto ansi sealed '<StartupCode$Hash11>'.$Hash11$fsx
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $Hash11$fsx::main@

} // end of class '<StartupCode$Hash11>'.$Hash11$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\GenericComparison\Hash11_fsx\Hash11.res
