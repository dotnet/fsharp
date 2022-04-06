
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
.assembly SteppingMatch08
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.SteppingMatch08
{
  // Offset: 0x00000000 Length: 0x00000213
  // WARNING: managed resource file FSharpSignatureData.SteppingMatch08 created
}
.mresource public FSharpOptimizationData.SteppingMatch08
{
  // Offset: 0x00000218 Length: 0x00000079
  // WARNING: managed resource file FSharpOptimizationData.SteppingMatch08 created
}
.module SteppingMatch08.exe
// MVID: {624CD660-C1FA-4792-A745-038360D64C62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x03590000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed SteppingMatch08
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static void  test(int32 x) cil managed
  {
    // Code size       21 (0x15)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  switch     ( 
                          IL_000d)
    IL_000b:  br.s       IL_0011

    IL_000d:  ldc.i4.2
    IL_000e:  nop
    IL_000f:  br.s       IL_0013

    IL_0011:  ldc.i4.0
    IL_0012:  nop
    IL_0013:  stloc.0
    IL_0014:  ret
  } // end of method SteppingMatch08::test

} // end of class SteppingMatch08

.class private abstract auto ansi sealed '<StartupCode$SteppingMatch08>'.$SteppingMatch08
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $SteppingMatch08::main@

} // end of class '<StartupCode$SteppingMatch08>'.$SteppingMatch08


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\SteppingMatch\SteppingMatch08_fs\SteppingMatch08.res
