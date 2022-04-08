
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
.assembly Compare09
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Compare09
{
  // Offset: 0x00000000 Length: 0x00000263
  // WARNING: managed resource file FSharpSignatureData.Compare09 created
}
.mresource public FSharpOptimizationData.Compare09
{
  // Offset: 0x00000268 Length: 0x000000B2
  // WARNING: managed resource file FSharpOptimizationData.Compare09 created
}
.module Compare09.exe
// MVID: {624F9D3B-786A-7F19-A745-03833B9D4F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00E80000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Compare09
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public CompareMicroPerfAndCodeGenerationTests
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static int32  f8() cil managed
    {
      // Code size       68 (0x44)
      .maxstack  5
      .locals init (int32 V_0,
               int32[] V_1,
               int32[] V_2,
               int32 V_3)
      IL_0000:  ldc.i4.1
      IL_0001:  stloc.0
      IL_0002:  ldc.i4.0
      IL_0003:  ldc.i4.1
      IL_0004:  ldc.i4.s   100
      IL_0006:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                       int32,
                                                                                                                                                                       int32)
      IL_000b:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::CreateSequence<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0010:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0015:  stloc.1
      IL_0016:  ldc.i4.0
      IL_0017:  ldc.i4.1
      IL_0018:  ldc.i4.s   100
      IL_001a:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                       int32,
                                                                                                                                                                       int32)
      IL_001f:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [FSharp.Core]Microsoft.FSharp.Core.Operators::CreateSequence<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0024:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.SeqModule::ToArray<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
      IL_0029:  stloc.2
      IL_002a:  ldc.i4.0
      IL_002b:  stloc.3
      IL_002c:  br.s       IL_003a

      IL_002e:  ldloc.1
      IL_002f:  ldloc.2
      IL_0030:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonIntrinsic<int32[]>(!!0,
                                                                                                                                        !!0)
      IL_0035:  stloc.0
      IL_0036:  ldloc.3
      IL_0037:  ldc.i4.1
      IL_0038:  add
      IL_0039:  stloc.3
      IL_003a:  ldloc.3
      IL_003b:  ldc.i4     0x186a1
      IL_0040:  blt.s      IL_002e

      IL_0042:  ldloc.0
      IL_0043:  ret
    } // end of method CompareMicroPerfAndCodeGenerationTests::f8

  } // end of class CompareMicroPerfAndCodeGenerationTests

} // end of class Compare09

.class private abstract auto ansi sealed '<StartupCode$Compare09>'.$Compare09$fsx
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $Compare09$fsx::main@

} // end of class '<StartupCode$Compare09>'.$Compare09$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\GenericComparison\Compare09_fsx\Compare09.res
