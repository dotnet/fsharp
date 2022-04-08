
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
.assembly TestFunction09
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.TestFunction09
{
  // Offset: 0x00000000 Length: 0x0000021A
  // WARNING: managed resource file FSharpSignatureData.TestFunction09 created
}
.mresource public FSharpOptimizationData.TestFunction09
{
  // Offset: 0x00000220 Length: 0x00000080
  // WARNING: managed resource file FSharpOptimizationData.TestFunction09 created
}
.module TestFunction09.exe
// MVID: {624E2CBA-A705-46AE-A745-0383BA2C4E62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x03B90000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed TestFunction09
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static string  TestFunction9(int32 x) cil managed
  {
    // Code size       37 (0x25)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.3
    IL_0003:  sub
    IL_0004:  switch     ( 
                          IL_0013,
                          IL_0019)
    IL_0011:  br.s       IL_001f

    IL_0013:  ldstr      "three"
    IL_0018:  ret

    IL_0019:  ldstr      "four"
    IL_001e:  ret

    IL_001f:  ldstr      "five"
    IL_0024:  ret
  } // end of method TestFunction09::TestFunction9

} // end of class TestFunction09

.class private abstract auto ansi sealed '<StartupCode$TestFunction09>'.$TestFunction09
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $TestFunction09::main@

} // end of class '<StartupCode$TestFunction09>'.$TestFunction09


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\TestFunctions\TestFunction09_fs\TestFunction09.res
