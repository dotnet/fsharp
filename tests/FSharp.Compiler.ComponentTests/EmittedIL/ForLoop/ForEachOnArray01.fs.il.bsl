
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
.assembly ForEachOnArray01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ForEachOnArray01
{
  // Offset: 0x00000000 Length: 0x00000227
  // WARNING: managed resource file FSharpSignatureData.ForEachOnArray01 created
}
.mresource public FSharpOptimizationData.ForEachOnArray01
{
  // Offset: 0x00000230 Length: 0x0000007C
  // WARNING: managed resource file FSharpOptimizationData.ForEachOnArray01 created
}
.module ForEachOnArray01.exe
// MVID: {624FA9CC-06EA-4D47-A745-0383CCA94F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00E50000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed ForEachOnArray01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static void  test3(int32[] arr) cil managed
  {
    // Code size       29 (0x1d)
    .maxstack  4
    .locals init (int32 V_0,
             int32 V_1,
             int32 V_2)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.1
    IL_0004:  br.s       IL_0016

    IL_0006:  ldarg.0
    IL_0007:  ldloc.1
    IL_0008:  ldelem     [mscorlib]System.Int32
    IL_000d:  stloc.2
    IL_000e:  ldloc.0
    IL_000f:  ldloc.2
    IL_0010:  add
    IL_0011:  stloc.0
    IL_0012:  ldloc.1
    IL_0013:  ldc.i4.1
    IL_0014:  add
    IL_0015:  stloc.1
    IL_0016:  ldloc.1
    IL_0017:  ldarg.0
    IL_0018:  ldlen
    IL_0019:  conv.i4
    IL_001a:  blt.s      IL_0006

    IL_001c:  ret
  } // end of method ForEachOnArray01::test3

} // end of class ForEachOnArray01

.class private abstract auto ansi sealed '<StartupCode$ForEachOnArray01>'.$ForEachOnArray01
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $ForEachOnArray01::main@

} // end of class '<StartupCode$ForEachOnArray01>'.$ForEachOnArray01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\release\net472\tests\EmittedIL\ForLoop\ForEachOnArray01_fs\ForEachOnArray01.res
