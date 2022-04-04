
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
.assembly InequalityComparison05
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.InequalityComparison05
{
  // Offset: 0x00000000 Length: 0x0000026A
  // WARNING: managed resource file FSharpSignatureData.InequalityComparison05 created
}
.mresource public FSharpOptimizationData.InequalityComparison05
{
  // Offset: 0x00000270 Length: 0x00000085
  // WARNING: managed resource file FSharpOptimizationData.InequalityComparison05 created
}
.module InequalityComparison05.exe
// MVID: {62463DAA-263A-E751-A745-0383AA3D4662}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x03AB0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed InequalityComparison05
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static !!a  f5<a>(int32 x,
                                   int32 y,
                                   !!a z,
                                   !!a w) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 04 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                    00 00 01 00 00 00 00 00 ) 
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldarg.1
    IL_0003:  ble.s      IL_0007

    IL_0005:  ldarg.2
    IL_0006:  ret

    IL_0007:  ldarg.3
    IL_0008:  ret
  } // end of method InequalityComparison05::f5

} // end of class InequalityComparison05

.class private abstract auto ansi sealed '<StartupCode$InequalityComparison05>'.$InequalityComparison05
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $InequalityComparison05::main@

} // end of class '<StartupCode$InequalityComparison05>'.$InequalityComparison05


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\InequalityComparison\InequalityComparison05_fs\InequalityComparison05.res
