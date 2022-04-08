
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
.assembly TestFunction22f
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.TestFunction22f
{
  // Offset: 0x00000000 Length: 0x00000191
  // WARNING: managed resource file FSharpSignatureData.TestFunction22f created
}
.mresource public FSharpOptimizationData.TestFunction22f
{
  // Offset: 0x00000198 Length: 0x00000056
  // WARNING: managed resource file FSharpOptimizationData.TestFunction22f created
}
.module TestFunction22f.exe
// MVID: {624F8F70-0BE0-253F-A745-0383708F4F62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x03D80000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed TestFunction22f
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
} // end of class TestFunction22f

.class private abstract auto ansi sealed '<StartupCode$TestFunction22f>'.$TestFunction22f
       extends [mscorlib]System.Object
{
  .field static assembly int32 init@
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       34 (0x22)
    .maxstack  4
    .locals init (string V_0)
    IL_0000:  ldstr      "A"
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldstr      "A"
    IL_000c:  call       bool [netstandard]System.String::Equals(string,
                                                                 string)
    IL_0011:  brfalse.s  IL_001b

    IL_0013:  call       void [mscorlib]System.Console::WriteLine()
    IL_0018:  nop
    IL_0019:  br.s       IL_0021

    IL_001b:  call       void [mscorlib]System.Console::WriteLine()
    IL_0020:  nop
    IL_0021:  ret
  } // end of method $TestFunction22f::main@

} // end of class '<StartupCode$TestFunction22f>'.$TestFunction22f


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Debug\net472\tests\EmittedIL\TestFunctions\TestFunction22f_fs\TestFunction22f.res
