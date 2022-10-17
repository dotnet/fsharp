
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
.assembly Compare04
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.Compare04
{
  // Offset: 0x00000000 Length: 0x0000025A
  // WARNING: managed resource file FSharpSignatureData.Compare04 created
}
.mresource public FSharpOptimizationData.Compare04
{
  // Offset: 0x00000260 Length: 0x000000B9
  // WARNING: managed resource file FSharpOptimizationData.Compare04 created
}
.module Compare04.exe
// MVID: {629F023F-7AE0-AE2A-A745-03833F029F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x000001BE779D0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Compare04
       extends [System.Runtime]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .class abstract auto ansi sealed nested public CompareMicroPerfAndCodeGenerationTests
         extends [System.Runtime]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
    .method public static int32  f4_tuple5() cil managed
    {
      // Code size       159 (0x9f)
      .maxstack  5
      .locals init (int32 V_0,
               int32 V_1,
               int32 V_2)
      IL_0000:  ldc.i4.1
      IL_0001:  stloc.0
      IL_0002:  nop
      IL_0003:  nop
      IL_0004:  ldc.i4.0
      IL_0005:  stloc.1
      IL_0006:  br         IL_0092

      IL_000b:  ldstr      "5"
      IL_0010:  ldstr      "5"
      IL_0015:  call       int32 [netstandard]System.String::CompareOrdinal(string,
                                                                            string)
      IL_001a:  stloc.2
      IL_001b:  ldloc.2
      IL_001c:  brfalse.s  IL_0022

      IL_001e:  ldloc.2
      IL_001f:  nop
      IL_0020:  br.s       IL_008d

      IL_0022:  ldc.r8     6.0999999999999996
      IL_002b:  ldc.r8     7.0999999999999996
      IL_0034:  clt
      IL_0036:  brfalse.s  IL_003c

      IL_0038:  ldc.i4.m1
      IL_0039:  nop
      IL_003a:  br.s       IL_008d

      IL_003c:  ldc.r8     6.0999999999999996
      IL_0045:  ldc.r8     7.0999999999999996
      IL_004e:  cgt
      IL_0050:  brfalse.s  IL_0056

      IL_0052:  ldc.i4.1
      IL_0053:  nop
      IL_0054:  br.s       IL_008d

      IL_0056:  ldc.r8     6.0999999999999996
      IL_005f:  ldc.r8     7.0999999999999996
      IL_0068:  ceq
      IL_006a:  brfalse.s  IL_0070

      IL_006c:  ldc.i4.0
      IL_006d:  nop
      IL_006e:  br.s       IL_008d

      IL_0070:  call       class [System.Runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_0075:  ldc.r8     6.0999999999999996
      IL_007e:  ldc.r8     7.0999999999999996
      IL_0087:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<float64>(class [System.Runtime]System.Collections.IComparer,
                                                                                                                                                    !!0,
                                                                                                                                                    !!0)
      IL_008c:  nop
      IL_008d:  stloc.0
      IL_008e:  ldloc.1
      IL_008f:  ldc.i4.1
      IL_0090:  add
      IL_0091:  stloc.1
      IL_0092:  ldloc.1
      IL_0093:  ldc.i4     0x989681
      IL_0098:  blt        IL_000b

      IL_009d:  ldloc.0
      IL_009e:  ret
    } // end of method CompareMicroPerfAndCodeGenerationTests::f4_tuple5

  } // end of class CompareMicroPerfAndCodeGenerationTests

} // end of class Compare04

.class private abstract auto ansi sealed '<StartupCode$Compare04>'.$Compare04$fsx
       extends [System.Runtime]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $Compare04$fsx::main@

} // end of class '<StartupCode$Compare04>'.$Compare04$fsx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\dev\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net6.0\tests\EmittedIL\GenericComparison\Compare04_fsx\Compare04.res
