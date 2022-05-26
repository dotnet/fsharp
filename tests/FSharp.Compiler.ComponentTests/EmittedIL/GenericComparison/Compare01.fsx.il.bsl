
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
.assembly Compare01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Compare01
{
  // Offset: 0x00000000 Length: 0x00000248
  // WARNING: managed resource file FSharpSignatureData.Compare01 created
}
.mresource public FSharpOptimizationData.Compare01
{
  // Offset: 0x00000250 Length: 0x000000B2
  // WARNING: managed resource file FSharpOptimizationData.Compare01 created
}
.module Compare01.exe
// MVID: {628F4C90-425E-A691-A745-0383904C8F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x0000020380F70000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Compare01
       extends [System.Runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public CompareMicroPerfAndCodeGenerationTests
         extends [System.Runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static void  f4() cil managed
    {
      // Code size       43 (0x2b)
      .maxstack  4
      .locals init (int32 V_0,
               int32 V_1,
               int32 V_2)
      IL_0000:  ldc.i4.1
      IL_0001:  stloc.0
      IL_0002:  nop
      IL_0003:  nop
      IL_0004:  ldc.i4.0
      IL_0005:  stloc.1
      IL_0006:  br.s       IL_0022

      IL_0008:  ldc.i4.1
      IL_0009:  ldc.i4.1
      IL_000a:  cgt
      IL_000c:  ldc.i4.0
      IL_000d:  sub
      IL_000e:  stloc.2
      IL_000f:  ldloc.2
      IL_0010:  brfalse.s  IL_0016

      IL_0012:  ldloc.2
      IL_0013:  nop
      IL_0014:  br.s       IL_001d

      IL_0016:  ldc.i4.2
      IL_0017:  ldc.i4.3
      IL_0018:  cgt
      IL_001a:  ldc.i4.1
      IL_001b:  sub
      IL_001c:  nop
      IL_001d:  stloc.0
      IL_001e:  ldloc.1
      IL_001f:  ldc.i4.1
      IL_0020:  add
      IL_0021:  stloc.1
      IL_0022:  ldloc.1
      IL_0023:  ldc.i4     0x989681
      IL_0028:  blt.s      IL_0008

      IL_002a:  ret
    } // end of method CompareMicroPerfAndCodeGenerationTests::f4

  } // end of class CompareMicroPerfAndCodeGenerationTests

} // end of class Compare01

.class private abstract auto ansi sealed '<StartupCode$Compare01>'.$Compare01$fsx
       extends [System.Runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $Compare01$fsx::main@

} // end of class '<StartupCode$Compare01>'.$Compare01$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\dev\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net6.0\tests\EmittedIL\GenericComparison\Compare01_fsx\Compare01.res
