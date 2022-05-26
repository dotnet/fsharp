
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
// MVID: {628F4C90-160D-7101-A745-0383904C8F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x000001C008340000


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
      // Code size       216 (0xd8)
      .maxstack  5
      .locals init (int32 V_0,
               int32 V_1,
               int32 V_2,
               int32 V_3,
               int32 V_4,
               int32 V_5)
      IL_0000:  ldc.i4.1
      IL_0001:  stloc.0
      IL_0002:  nop
      IL_0003:  nop
      IL_0004:  ldc.i4.0
      IL_0005:  stloc.1
      IL_0006:  br         IL_00cb

      IL_000b:  ldc.i4.1
      IL_000c:  ldc.i4.1
      IL_000d:  cgt
      IL_000f:  ldc.i4.0
      IL_0010:  sub
      IL_0011:  stloc.2
      IL_0012:  ldloc.2
      IL_0013:  brfalse.s  IL_001c

      IL_0015:  ldloc.2
      IL_0016:  nop
      IL_0017:  br         IL_00c6

      IL_001c:  ldc.i4.2
      IL_001d:  ldc.i4.2
      IL_001e:  cgt
      IL_0020:  ldc.i4.0
      IL_0021:  sub
      IL_0022:  stloc.3
      IL_0023:  ldloc.3
      IL_0024:  brfalse.s  IL_002d

      IL_0026:  ldloc.3
      IL_0027:  nop
      IL_0028:  br         IL_00c6

      IL_002d:  ldc.i4.4
      IL_002e:  ldc.i4.4
      IL_002f:  cgt
      IL_0031:  ldc.i4.0
      IL_0032:  sub
      IL_0033:  stloc.s    V_4
      IL_0035:  ldloc.s    V_4
      IL_0037:  brfalse.s  IL_0041

      IL_0039:  ldloc.s    V_4
      IL_003b:  nop
      IL_003c:  br         IL_00c6

      IL_0041:  ldstr      "5"
      IL_0046:  ldstr      "5"
      IL_004b:  call       int32 [netstandard]System.String::CompareOrdinal(string,
                                                                            string)
      IL_0050:  stloc.s    V_5
      IL_0052:  ldloc.s    V_5
      IL_0054:  brfalse.s  IL_005b

      IL_0056:  ldloc.s    V_5
      IL_0058:  nop
      IL_0059:  br.s       IL_00c6

      IL_005b:  ldc.r8     6.0999999999999996
      IL_0064:  ldc.r8     7.0999999999999996
      IL_006d:  clt
      IL_006f:  brfalse.s  IL_0075

      IL_0071:  ldc.i4.m1
      IL_0072:  nop
      IL_0073:  br.s       IL_00c6

      IL_0075:  ldc.r8     6.0999999999999996
      IL_007e:  ldc.r8     7.0999999999999996
      IL_0087:  cgt
      IL_0089:  brfalse.s  IL_008f

      IL_008b:  ldc.i4.1
      IL_008c:  nop
      IL_008d:  br.s       IL_00c6

      IL_008f:  ldc.r8     6.0999999999999996
      IL_0098:  ldc.r8     7.0999999999999996
      IL_00a1:  ceq
      IL_00a3:  brfalse.s  IL_00a9

      IL_00a5:  ldc.i4.0
      IL_00a6:  nop
      IL_00a7:  br.s       IL_00c6

      IL_00a9:  call       class [System.Runtime]System.Collections.IComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericComparer()
      IL_00ae:  ldc.r8     6.0999999999999996
      IL_00b7:  ldc.r8     7.0999999999999996
      IL_00c0:  call       int32 [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericComparisonWithComparerIntrinsic<float64>(class [System.Runtime]System.Collections.IComparer,
                                                                                                                                                    !!0,
                                                                                                                                                    !!0)
      IL_00c5:  nop
      IL_00c6:  stloc.0
      IL_00c7:  ldloc.1
      IL_00c8:  ldc.i4.1
      IL_00c9:  add
      IL_00ca:  stloc.1
      IL_00cb:  ldloc.1
      IL_00cc:  ldc.i4     0x989681
      IL_00d1:  blt        IL_000b

      IL_00d6:  ldloc.0
      IL_00d7:  ret
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
