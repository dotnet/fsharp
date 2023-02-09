
//  Microsoft (R) .NET IL Disassembler.  Version 5.0.0-preview.7.20364.11



// Metadata version: v4.0.30319
.assembly extern System.Runtime
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 6:0:0:0
}
.assembly extern FSharp.Core
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 6:0:0:0
}
.assembly Compare02
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Compare02
{
  // Offset: 0x00000000 Length: 0x0000024F
  // WARNING: managed resource file FSharpSignatureData.Compare02 created
}
.mresource public FSharpOptimizationData.Compare02
{
  // Offset: 0x00000258 Length: 0x000000B9
  // WARNING: managed resource file FSharpOptimizationData.Compare02 created
}
.module Compare02.exe
// MVID: {629F023F-DFF8-1315-A745-03833F029F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00000285F4090000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Compare02
       extends [System.Runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public CompareMicroPerfAndCodeGenerationTests
         extends [System.Runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static void  f4_triple() cil managed
    {
      // Code size       23 (0x17)
      .maxstack  4
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldc.i4.1
      IL_0001:  stloc.0
      IL_0002:  nop
      IL_0003:  nop
      IL_0004:  ldc.i4.0
      IL_0005:  stloc.1
      IL_0006:  br.s       IL_000e

      IL_0008:  ldc.i4.m1
      IL_0009:  stloc.0
      IL_000a:  ldloc.1
      IL_000b:  ldc.i4.1
      IL_000c:  add
      IL_000d:  stloc.1
      IL_000e:  ldloc.1
      IL_000f:  ldc.i4     0x989681
      IL_0014:  blt.s      IL_0008

      IL_0016:  ret
    } // end of method CompareMicroPerfAndCodeGenerationTests::f4_triple

  } // end of class CompareMicroPerfAndCodeGenerationTests

} // end of class Compare02

.class private abstract auto ansi sealed '<StartupCode$Compare02>'.$Compare02$fsx
       extends [System.Runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $Compare02$fsx::main@

} // end of class '<StartupCode$Compare02>'.$Compare02$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\dev\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net6.0\tests\EmittedIL\GenericComparison\Compare02_fsx\Compare02.res
