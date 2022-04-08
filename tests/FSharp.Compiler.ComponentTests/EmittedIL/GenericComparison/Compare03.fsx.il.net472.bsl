
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
.assembly extern netstandard
{
  .publickeytoken = (CC 7B 13 FF CD 2D DD 51 )                         // .{...-.Q
  .ver 2:0:0:0
}
.assembly Compare03
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Compare03
{
  // Offset: 0x00000000 Length: 0x0000026A
  // WARNING: managed resource file FSharpSignatureData.Compare03 created
}
.mresource public FSharpOptimizationData.Compare03
{
  // Offset: 0x00000270 Length: 0x000000B9
  // WARNING: managed resource file FSharpOptimizationData.Compare03 created
}
.module Compare03.exe
// MVID: {624F9E44-79F4-7F19-A745-0383449E4F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x03180000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Compare03
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public CompareMicroPerfAndCodeGenerationTests
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static int32  f4_tuple4() cil managed
    {
      // Code size       78 (0x4e)
      .maxstack  4
      .locals init (int32 V_0,
               int32 V_1,
               int32 V_2,
               int32 V_3,
               int32 V_4)
      IL_0000:  ldc.i4.1
      IL_0001:  stloc.0
      IL_0002:  nop
      IL_0003:  nop
      IL_0004:  ldc.i4.0
      IL_0005:  stloc.1
      IL_0006:  br.s       IL_0044

      IL_0008:  ldc.i4.1
      IL_0009:  ldc.i4.1
      IL_000a:  cgt
      IL_000c:  stloc.2
      IL_000d:  ldloc.2
      IL_000e:  brfalse.s  IL_0014

      IL_0010:  ldloc.2
      IL_0011:  nop
      IL_0012:  br.s       IL_003f

      IL_0014:  ldc.i4.2
      IL_0015:  ldc.i4.2
      IL_0016:  cgt
      IL_0018:  stloc.3
      IL_0019:  ldloc.3
      IL_001a:  brfalse.s  IL_0020

      IL_001c:  ldloc.3
      IL_001d:  nop
      IL_001e:  br.s       IL_003f

      IL_0020:  ldc.i4.4
      IL_0021:  ldc.i4.4
      IL_0022:  cgt
      IL_0024:  stloc.s    V_4
      IL_0026:  ldloc.s    V_4
      IL_0028:  brfalse.s  IL_002f

      IL_002a:  ldloc.s    V_4
      IL_002c:  nop
      IL_002d:  br.s       IL_003f

      IL_002f:  ldstr      "five"
      IL_0034:  ldstr      "5"
      IL_0039:  call       int32 [netstandard]System.String::CompareOrdinal(string,
                                                                            string)
      IL_003e:  nop
      IL_003f:  stloc.0
      IL_0040:  ldloc.1
      IL_0041:  ldc.i4.1
      IL_0042:  add
      IL_0043:  stloc.1
      IL_0044:  ldloc.1
      IL_0045:  ldc.i4     0x989681
      IL_004a:  blt.s      IL_0008

      IL_004c:  ldloc.0
      IL_004d:  ret
    } // end of method CompareMicroPerfAndCodeGenerationTests::f4_tuple4

  } // end of class CompareMicroPerfAndCodeGenerationTests

} // end of class Compare03

.class private abstract auto ansi sealed '<StartupCode$Compare03>'.$Compare03$fsx
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $Compare03$fsx::main@

} // end of class '<StartupCode$Compare03>'.$Compare03$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\GenericComparison\Compare03_fsx\Compare03.res
