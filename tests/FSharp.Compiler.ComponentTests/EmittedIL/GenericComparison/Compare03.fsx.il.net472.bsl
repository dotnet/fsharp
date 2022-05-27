
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
  // Offset: 0x00000000 Length: 0x00000276
  // WARNING: managed resource file FSharpSignatureData.Compare03 created
}
.mresource public FSharpOptimizationData.Compare03
{
  // Offset: 0x00000280 Length: 0x000000B9
  // WARNING: managed resource file FSharpOptimizationData.Compare03 created
}
.module Compare03.exe
// MVID: {6290E425-79F4-7F19-A745-038325E49062}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x03380000


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
      // Code size       84 (0x54)
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
      IL_0006:  br.s       IL_004a

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
      IL_0014:  br.s       IL_0045

      IL_0016:  ldc.i4.2
      IL_0017:  ldc.i4.2
      IL_0018:  cgt
      IL_001a:  ldc.i4.0
      IL_001b:  sub
      IL_001c:  stloc.3
      IL_001d:  ldloc.3
      IL_001e:  brfalse.s  IL_0024

      IL_0020:  ldloc.3
      IL_0021:  nop
      IL_0022:  br.s       IL_0045

      IL_0024:  ldc.i4.4
      IL_0025:  ldc.i4.4
      IL_0026:  cgt
      IL_0028:  ldc.i4.0
      IL_0029:  sub
      IL_002a:  stloc.s    V_4
      IL_002c:  ldloc.s    V_4
      IL_002e:  brfalse.s  IL_0035

      IL_0030:  ldloc.s    V_4
      IL_0032:  nop
      IL_0033:  br.s       IL_0045

      IL_0035:  ldstr      "five"
      IL_003a:  ldstr      "5"
      IL_003f:  call       int32 [netstandard]System.String::CompareOrdinal(string,
                                                                            string)
      IL_0044:  nop
      IL_0045:  stloc.0
      IL_0046:  ldloc.1
      IL_0047:  ldc.i4.1
      IL_0048:  add
      IL_0049:  stloc.1
      IL_004a:  ldloc.1
      IL_004b:  ldc.i4     0x989681
      IL_0050:  blt.s      IL_0008

      IL_0052:  ldloc.0
      IL_0053:  ret
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
// WARNING: Created Win32 resource file C:\Users\vlza\code\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Release\net472\tests\EmittedIL\GenericComparison\Compare03_fsx\Compare03.res
