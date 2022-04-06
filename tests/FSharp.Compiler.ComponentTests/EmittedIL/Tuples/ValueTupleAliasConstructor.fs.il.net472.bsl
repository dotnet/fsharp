
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
.assembly ValueTupleAliasConstructor
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.ValueTupleAliasConstructor
{
  // Offset: 0x00000000 Length: 0x0000022E
  // WARNING: managed resource file FSharpSignatureData.ValueTupleAliasConstructor created
}
.mresource public FSharpOptimizationData.ValueTupleAliasConstructor
{
  // Offset: 0x00000238 Length: 0x00000061
  // WARNING: managed resource file FSharpOptimizationData.ValueTupleAliasConstructor created
}
.module ValueTupleAliasConstructor.exe
// MVID: {624CF392-A8CF-BB34-A745-038392F34C62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x03200000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed ValueTupleAliasConstructor
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
} // end of class ValueTupleAliasConstructor

.class private abstract auto ansi sealed '<StartupCode$ValueTupleAliasConstructor>'.$ValueTupleAliasConstructor
       extends [mscorlib]System.Object
{
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  ldc.i4.2
    IL_0001:  ldc.i4.2
    IL_0002:  newobj     instance void valuetype [mscorlib]System.ValueTuple`2<int32,int32>::.ctor(!0,
                                                                                                   !1)
    IL_0007:  pop
    IL_0008:  ret
  } // end of method $ValueTupleAliasConstructor::main@

} // end of class '<StartupCode$ValueTupleAliasConstructor>'.$ValueTupleAliasConstructor


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Release\net472\tests\EmittedIL\Tuples\ValueTupleAliasConstructor_fs\ValueTupleAliasConstructor.res
