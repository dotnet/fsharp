
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
.assembly ZeroToArrLength01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ZeroToArrLength01
{
  // Offset: 0x00000000 Length: 0x00000228
  // WARNING: managed resource file FSharpSignatureData.ZeroToArrLength01 created
}
.mresource public FSharpOptimizationData.ZeroToArrLength01
{
  // Offset: 0x00000230 Length: 0x0000007B
  // WARNING: managed resource file FSharpOptimizationData.ZeroToArrLength01 created
}
.module ZeroToArrLength01.exe
// MVID: {624FA9CC-1872-1D93-A745-0383CCA94F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00E80000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed ZeroToArrLength01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static void  f1(int32[] arr) cil managed
  {
    // Code size       23 (0x17)
    .maxstack  5
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  br.s       IL_0010

    IL_0004:  ldarg.0
    IL_0005:  ldloc.0
    IL_0006:  ldloc.0
    IL_0007:  stelem     [mscorlib]System.Int32
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.1
    IL_000e:  add
    IL_000f:  stloc.0
    IL_0010:  ldloc.0
    IL_0011:  ldarg.0
    IL_0012:  ldlen
    IL_0013:  conv.i4
    IL_0014:  blt.s      IL_0004

    IL_0016:  ret
  } // end of method ZeroToArrLength01::f1

} // end of class ZeroToArrLength01

.class private abstract auto ansi sealed '<StartupCode$ZeroToArrLength01>'.$ZeroToArrLength01
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $ZeroToArrLength01::main@

} // end of class '<StartupCode$ZeroToArrLength01>'.$ZeroToArrLength01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\release\net472\tests\EmittedIL\ForLoop\ZeroToArrLength01_fs\ZeroToArrLength01.res
