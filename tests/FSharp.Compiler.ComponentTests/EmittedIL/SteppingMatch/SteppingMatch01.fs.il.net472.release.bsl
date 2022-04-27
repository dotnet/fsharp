
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
.assembly SteppingMatch01
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                      int32,
                                                                                                      int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 03 01 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.mresource public FSharpSignatureData.SteppingMatch01
{
  // Offset: 0x00000000 Length: 0x0000024C
  // WARNING: managed resource file FSharpSignatureData.SteppingMatch01 created
}
.mresource public FSharpOptimizationData.SteppingMatch01
{
  // Offset: 0x00000250 Length: 0x0000007A
  // WARNING: managed resource file FSharpOptimizationData.SteppingMatch01 created
}
.module SteppingMatch01.exe
// MVID: {624CDA9B-9289-5BFF-A745-03839BDA4C62}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x03A50000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed SteppingMatch01
       extends [mscorlib]System.Object
{
  .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
  .method public static void  funcA<a,b>(class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2<!!a,!!b> n) cil managed
  {
    // Code size       48 (0x30)
    .maxstack  3
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2<!!a,!!b> V_0,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2/Choice2Of2<!!a,!!b> V_1,
             class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2/Choice1Of2<!!a,!!b> V_2)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  isinst     class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2/Choice1Of2<!!a,!!b>
    IL_0008:  brfalse.s  IL_000c

    IL_000a:  br.s       IL_001e

    IL_000c:  ldloc.0
    IL_000d:  castclass  class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2/Choice2Of2<!!a,!!b>
    IL_0012:  stloc.1
    IL_0013:  ldstr      "A"
    IL_0018:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_001d:  ret

    IL_001e:  ldloc.0
    IL_001f:  castclass  class [FSharp.Core]Microsoft.FSharp.Core.FSharpChoice`2/Choice1Of2<!!a,!!b>
    IL_0024:  stloc.2
    IL_0025:  ldstr      "B"
    IL_002a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002f:  ret
  } // end of method SteppingMatch01::funcA

} // end of class SteppingMatch01

.class private abstract auto ansi sealed '<StartupCode$SteppingMatch01>'.$SteppingMatch01
       extends [mscorlib]System.Object
{
  .method public static void  main@() cil managed
  {
    .entrypoint
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method $SteppingMatch01::main@

} // end of class '<StartupCode$SteppingMatch01>'.$SteppingMatch01


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file c:\kevinransom\fsharp\artifacts\bin\FSharp.Compiler.ComponentTests\Release\net472\tests\EmittedIL\SteppingMatch\SteppingMatch01_fs\SteppingMatch01.res
