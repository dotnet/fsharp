
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
.assembly DoNotBoxStruct_Array_CSInterface
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.DoNotBoxStruct_Array_CSInterface
{
  // Offset: 0x00000000 Length: 0x000002AC
  // WARNING: managed resource file FSharpSignatureData.DoNotBoxStruct_Array_CSInterface created
}
.mresource public FSharpOptimizationData.DoNotBoxStruct_Array_CSInterface
{
  // Offset: 0x000002B0 Length: 0x0000007F
  // WARNING: managed resource file FSharpOptimizationData.DoNotBoxStruct_Array_CSInterface created
}
.module DoNotBoxStruct_Array_CSInterface.exe
// MVID: {622FACDF-1735-654E-A745-0383DFAC2F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x03D60000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed Program
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static void  F<([mscorlib]System.IDisposable) T>(!!T[] x) cil managed
  {
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  readonly.
    IL_0004:  ldelema    !!T
    IL_0009:  constrained. !!T
    IL_000f:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
    IL_0014:  ret
  } // end of method Program::F

} // end of class Program

.class private abstract auto ansi sealed '<StartupCode$DoNotBoxStruct_Array_CSInterface>'.$Program
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $Program::main@

} // end of class '<StartupCode$DoNotBoxStruct_Array_CSInterface>'.$Program


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\DoNotBoxStruct\DoNotBoxStruct_Array_CSInterface_fs\DoNotBoxStruct_Array_CSInterface.res
