
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
.assembly Hash04
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Hash04
{
  // Offset: 0x00000000 Length: 0x00000250
  // WARNING: managed resource file FSharpSignatureData.Hash04 created
}
.mresource public FSharpOptimizationData.Hash04
{
  // Offset: 0x00000258 Length: 0x000000B0
  // WARNING: managed resource file FSharpOptimizationData.Hash04 created
}
.module Hash04.exe
// MVID: {624F9DB5-BC0C-9C58-A745-0383B59D4F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x0000012F0A6D0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Hash04
       extends [System.Runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public HashMicroPerfAndCodeGenerationTests
         extends [System.Runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static void  f4_tuple5() cil managed
    {
      // Code size       40 (0x28)
      .maxstack  5
      .locals init (int32 V_0,
               int32 V_1)
      IL_0000:  ldc.i4.1
      IL_0001:  stloc.0
      IL_0002:  nop
      IL_0003:  ldc.i4.0
      IL_0004:  stloc.1
      IL_0005:  br.s       IL_001f

      IL_0007:  ldc.i4     0x483
      IL_000c:  ldc.i4.s   99
      IL_000e:  ldstr      "5"
      IL_0013:  callvirt   instance int32 [netstandard]System.Object::GetHashCode()
      IL_0018:  xor
      IL_0019:  xor
      IL_001a:  stloc.0
      IL_001b:  ldloc.1
      IL_001c:  ldc.i4.1
      IL_001d:  add
      IL_001e:  stloc.1
      IL_001f:  ldloc.1
      IL_0020:  ldc.i4     0x989681
      IL_0025:  blt.s      IL_0007

      IL_0027:  ret
    } // end of method HashMicroPerfAndCodeGenerationTests::f4_tuple5

  } // end of class HashMicroPerfAndCodeGenerationTests

} // end of class Hash04

.class private abstract auto ansi sealed '<StartupCode$Hash04>'.$Hash04$fsx
       extends [System.Runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $Hash04$fsx::main@

} // end of class '<StartupCode$Hash04>'.$Hash04$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net6.0\tests\EmittedIL\GenericComparison\Hash04_fsx\Hash04.res
