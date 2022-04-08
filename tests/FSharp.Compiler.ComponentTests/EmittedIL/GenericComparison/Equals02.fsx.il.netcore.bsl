
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
.assembly extern netstandard
{
  .publickeytoken = (CC 7B 13 FF CD 2D DD 51 )                         // .{...-.Q
  .ver 2:1:0:0
}
.assembly Equals02
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Equals02
{
  // Offset: 0x00000000 Length: 0x00000266
  // WARNING: managed resource file FSharpSignatureData.Equals02 created
}
.mresource public FSharpOptimizationData.Equals02
{
  // Offset: 0x00000270 Length: 0x000000B6
  // WARNING: managed resource file FSharpOptimizationData.Equals02 created
}
.module Equals02.exe
// MVID: {624F9DB5-0038-1B70-A745-0383B59D4F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00000275A75C0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Equals02
       extends [System.Runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public EqualsMicroPerfAndCodeGenerationTests
         extends [System.Runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static bool  f4_tuple4() cil managed
    {
      // Code size       38 (0x26)
      .maxstack  4
      .locals init (bool V_0,
               int32 V_1)
      IL_0000:  ldc.i4.0
      IL_0001:  stloc.0
      IL_0002:  nop
      IL_0003:  nop
      IL_0004:  ldc.i4.0
      IL_0005:  stloc.1
      IL_0006:  br.s       IL_001c

      IL_0008:  ldstr      "five"
      IL_000d:  ldstr      "5"
      IL_0012:  call       bool [netstandard]System.String::Equals(string,
                                                                   string)
      IL_0017:  stloc.0
      IL_0018:  ldloc.1
      IL_0019:  ldc.i4.1
      IL_001a:  add
      IL_001b:  stloc.1
      IL_001c:  ldloc.1
      IL_001d:  ldc.i4     0x989681
      IL_0022:  blt.s      IL_0008

      IL_0024:  ldloc.0
      IL_0025:  ret
    } // end of method EqualsMicroPerfAndCodeGenerationTests::f4_tuple4

  } // end of class EqualsMicroPerfAndCodeGenerationTests

} // end of class Equals02

.class private abstract auto ansi sealed '<StartupCode$Equals02>'.$Equals02$fsx
       extends [System.Runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $Equals02$fsx::main@

} // end of class '<StartupCode$Equals02>'.$Equals02$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net6.0\tests\EmittedIL\GenericComparison\Equals02_fsx\Equals02.res
