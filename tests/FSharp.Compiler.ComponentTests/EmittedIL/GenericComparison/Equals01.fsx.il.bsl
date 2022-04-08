
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
.assembly Equals01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Equals01
{
  // Offset: 0x00000000 Length: 0x00000266
  // WARNING: managed resource file FSharpSignatureData.Equals01 created
}
.mresource public FSharpOptimizationData.Equals01
{
  // Offset: 0x00000270 Length: 0x000000B6
  // WARNING: managed resource file FSharpOptimizationData.Equals01 created
}
.module Equals01.exe
// MVID: {624F9D3B-EB3D-8229-A745-03833B9D4F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00E70000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Equals01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public EqualsMicroPerfAndCodeGenerationTests
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static bool  f4_triple() cil managed
    {
      // Code size       24 (0x18)
      .maxstack  4
      .locals init (bool V_0,
               int32 V_1)
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  nop
      IL_0003:  nop
      IL_0004:  ldc.i4.0
      IL_0005:  stloc.1
      IL_0006:  br.s       IL_000e

      IL_0008:  ldc.i4.0
      IL_0009:  stloc.0
      IL_000a:  ldloc.1
      IL_000b:  ldc.i4.1
      IL_000c:  add
      IL_000d:  stloc.1
      IL_000e:  ldloc.1
      IL_000f:  ldc.i4     0x989681
      IL_0014:  blt.s      IL_0008

      IL_0016:  ldloc.0
      IL_0017:  ret
    } // end of method EqualsMicroPerfAndCodeGenerationTests::f4_triple

  } // end of class EqualsMicroPerfAndCodeGenerationTests

} // end of class Equals01

.class private abstract auto ansi sealed '<StartupCode$Equals01>'.$Equals01$fsx
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $Equals01$fsx::main@

} // end of class '<StartupCode$Equals01>'.$Equals01$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\GenericComparison\Equals01_fsx\Equals01.res
